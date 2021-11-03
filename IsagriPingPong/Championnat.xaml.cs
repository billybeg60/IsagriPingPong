using System.Collections.Generic;
using System.Windows;

namespace IsagriPingPong
{
    /// <summary>
    /// Interaction logic for Championnat.xaml
    /// </summary>
    public partial class Championnat : Window
    {
        private List<JoueurBDD> _listeJoueur = new List<JoueurBDD>();
        private List<EquipeBDD> _listeEquipe = new List<EquipeBDD>();
        private bool _double = false;
        private bool _flagFermeture = true;

        /// <summary>
        /// Liste des rencontres
        /// </summary>
        public List<Rencontre> ListeRencontre
        {
            get { return (List<Rencontre>)GetValue(ListeRencontreProperty); }
            set { SetValue(ListeRencontreProperty, value); }
        }

        public static readonly DependencyProperty ListeRencontreProperty =
            DependencyProperty.Register("ListeRencontre", typeof(List<Rencontre>), typeof(Championnat), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Liste des équipes
        /// </summary>
        public List<Participant> ListeParticipant
        {
            get { return (List<Participant>)GetValue(ListeParticipantProperty); }
            set { SetValue(ListeParticipantProperty, value); }
        }

        public static readonly DependencyProperty ListeParticipantProperty =
            DependencyProperty.Register("ListeParticipant", typeof(List<Participant>), typeof(Championnat));

        public Championnat(List<JoueurBDD> listeJoueur, List<EquipeBDD> listeEquipe, List<Participant> listeEquipeParam, bool enEquipe, bool allerRetour, bool choixRaquette)
        {
            InitializeComponent();
            this.DataContext = this;
            _double = enEquipe;
            _listeJoueur.AddRange(listeJoueur);
            _listeEquipe.AddRange(listeEquipe);
            ListeParticipant = new List<Participant>();
            ListeParticipant.AddRange(listeEquipeParam);
            ListeRencontre = RoundRobinHelper.GenererListeRencontre(ListeParticipant, allerRetour);
            DiversRules.DeterminerRaquette(ListeRencontre, choixRaquette, null);
        }

        private void xBtValierScore_Click(object sender, RoutedEventArgs e)
        {
            DiversRules.RecalculerClassement(ListeRencontre, ListeParticipant);
            xDGClassement.Items.Refresh();
            xOngletCL.IsSelected = true;
        }

        private void xCBValider_Click(object sender, RoutedEventArgs e)
        {
            Rencontre rencontre = xDGCalendrier.CurrentItem as Rencontre;
            e.Handled = DiversRules.ValiderScore(rencontre);
            if (!e.Handled)
                VocalHelper.GenererPhraseFinMatch(rencontre);
        }

        private void xBtFinTournoi_Click(object sender, RoutedEventArgs e)
        {
            if (DiversRules.EnregistrerResultat(ListeRencontre, _listeJoueur, _listeEquipe, ListeParticipant, _double, true, false, false))
            {
                _flagFermeture = false;
                this.Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_flagFermeture)
            {
                if (MessageBox.Show("Etes-vous sur de vouloir fermer ? Les résultats en cours ne seront pas enregistrés", "", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    e.Cancel = true;
            }
        }

        private void xDGCalendrier_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Rencontre rencontre = xDGCalendrier.CurrentItem as Rencontre;
            AfficheurHelper.AfficherMatch(rencontre, ListeRencontre);
        }

        private void xDGCalendrier_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            bool refresh = false;
            Rencontre rencontre = xDGCalendrier.CurrentItem as Rencontre;
            if (e.Key == System.Windows.Input.Key.LeftCtrl)
            {
                rencontre = xDGCalendrier.CurrentItem as Rencontre;
                rencontre.PointEquipe1 = rencontre.PointEquipe1 + 1;
                refresh = true;
            }
            if (e.Key == System.Windows.Input.Key.RightCtrl)
            {
                rencontre = xDGCalendrier.CurrentItem as Rencontre;
                rencontre.PointEquipe2 = rencontre.PointEquipe2 + 1;
                refresh = true;
            }

            if (refresh)
            {
                int pos = xDGCalendrier.SelectedIndex;
                xDGCalendrier.Items.Refresh();
                AfficheurHelper.AfficherMatch(rencontre, ListeRencontre);
                xDGCalendrier.Focus();
                xDGCalendrier.Items.MoveCurrentToPosition(pos);
            }
        }
    }
}
