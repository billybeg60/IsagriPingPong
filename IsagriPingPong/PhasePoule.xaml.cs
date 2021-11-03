using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace IsagriPingPong
{
    /// <summary>
    /// Interaction logic for PhasePoule.xaml
    /// </summary>
    public partial class PhasePoule : Window
    {
        private List<JoueurBDD> _listeJoueur = new List<JoueurBDD>();
        private List<EquipeBDD> _listeEquipe = new List<EquipeBDD>();
        private bool _double = false;
        private bool _flagFermeture = true;

        bool _choixRaquette { get; set; }

        /// <summary>
        /// Liste des équipes Poule A
        /// </summary>
        public List<Participant> ListeEquipePA
        {
            get { return (List<Participant>)GetValue(ListeEquipePAProperty); }
            set { SetValue(ListeEquipePAProperty, value); }
        }

        public static readonly DependencyProperty ListeEquipePAProperty =
            DependencyProperty.Register("ListeEquipePA", typeof(List<Participant>), typeof(PhasePoule));

        /// <summary>
        /// Liste des équipes Poule B
        /// </summary>
        public List<Participant> ListeEquipePB
        {
            get { return (List<Participant>)GetValue(ListeEquipePBProperty); }
            set { SetValue(ListeEquipePBProperty, value); }
        }

        public static readonly DependencyProperty ListeEquipePBProperty =
            DependencyProperty.Register("ListeEquipePB", typeof(List<Participant>), typeof(PhasePoule));

        /// <summary>
        /// Liste des équipes Phase finale
        /// </summary>
        public List<Participant> ListeEquipePF
        {
            get { return (List<Participant>)GetValue(ListeEquipePFProperty); }
            set { SetValue(ListeEquipePFProperty, value); }
        }

        public static readonly DependencyProperty ListeEquipePFProperty =
            DependencyProperty.Register("ListeEquipePF", typeof(List<Participant>), typeof(PhasePoule));

        /// <summary>
        /// Liste des rencontres de la poule A
        /// </summary>
        public List<Rencontre> ListeRencontrePA
        {
            get { return (List<Rencontre>)GetValue(ListeRencontrePAProperty); }
            set { SetValue(ListeRencontrePAProperty, value); }
        }

        public static readonly DependencyProperty ListeRencontrePAProperty =
            DependencyProperty.Register("ListeRencontrePA", typeof(List<Rencontre>), typeof(PhasePoule), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Liste des rencontres de la poule B
        /// </summary>
        public List<Rencontre> ListeRencontrePB
        {
            get { return (List<Rencontre>)GetValue(ListeRencontrePBProperty); }
            set { SetValue(ListeRencontrePBProperty, value); }
        }

        public static readonly DependencyProperty ListeRencontrePBProperty =
            DependencyProperty.Register("ListeRencontrePB", typeof(List<Rencontre>), typeof(PhasePoule), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Liste des rencontres de la phase finale
        /// </summary>
        public List<Rencontre> ListeRencontrePF
        {
            get { return (List<Rencontre>)GetValue(ListeRencontrePFProperty); }
            set { SetValue(ListeRencontrePFProperty, value); }
        }

        public static readonly DependencyProperty ListeRencontrePFProperty =
            DependencyProperty.Register("ListeRencontrePF", typeof(List<Rencontre>), typeof(PhasePoule), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public PhasePoule(List<JoueurBDD> listeJoueur, List<EquipeBDD> listeEquipe, List<Participant> listeEquipeParam, bool enEquipe, bool choixRaquette)
        {
            InitializeComponent();
            this.DataContext = this;
            _double = enEquipe;
            _choixRaquette = choixRaquette;
            _listeJoueur.AddRange(listeJoueur);
            _listeEquipe.AddRange(listeEquipe);
            ListeEquipePA = listeEquipeParam.GetRange(0, (listeEquipeParam.Count / 2));
            ListeEquipePB = listeEquipeParam.GetRange(listeEquipeParam.Count / 2, listeEquipeParam.Count / 2);
            int id = 0;
            // On réaffecte les id pour la poule B
            foreach (var item in ListeEquipePB)
            {
                item.Id = id;
                id++;
            }

            ListeRencontrePA = RoundRobinHelper.GenererListeRencontre(ListeEquipePA, false);
            ListeRencontrePB = RoundRobinHelper.GenererListeRencontre(ListeEquipePB, false);
            List<Rencontre> listeRencontreTotal = new List<Rencontre>();
            listeRencontreTotal.AddRange(ListeRencontrePA);
            listeRencontreTotal.AddRange(ListeRencontrePB);
            DiversRules.DeterminerRaquette(listeRencontreTotal, choixRaquette, null);
        }

        private void xCBValiderPB_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Rencontre rencontre = xDGPouleB.CurrentItem as Rencontre;
            e.Handled = DiversRules.ValiderScore(rencontre);
            if (!e.Handled)
                VocalHelper.GenererPhraseFinMatch(rencontre);
        }

        private void xCBValiderPA_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Rencontre rencontre = xDGPouleA.CurrentItem as Rencontre;
            e.Handled = DiversRules.ValiderScore(rencontre);
            if (!e.Handled)
                VocalHelper.GenererPhraseFinMatch(rencontre);
        }

        private void xCBValiderPF_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Rencontre rencontre = xDGPhaseFinale.CurrentItem as Rencontre;
            e.Handled = DiversRules.ValiderScore(rencontre);
            if (!e.Handled)
                VocalHelper.GenererPhraseFinMatch(rencontre);
        }

        private void xBtValierScorePA_Click(object sender, RoutedEventArgs e)
        {
            DiversRules.RecalculerClassement(ListeRencontrePA, ListeEquipePA);
            xDGClassementPA.Items.Refresh();
            xOngletCL.IsSelected = true;
        }

        private void xBtValierScorePB_Click(object sender, RoutedEventArgs e)
        {
            DiversRules.RecalculerClassement(ListeRencontrePB, ListeEquipePB);
            xDGClassementPB.Items.Refresh();
            xOngletCL.IsSelected = true;
        }

        private void xBtPF_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder message = new StringBuilder();
            foreach (var item in ListeRencontrePA)
            {
                if (!item.Valider)
                {
                    message.AppendLine("Toutes les rencontres de la poule A n'ont pas été jouées");
                    break;
                }
            }
            foreach (var item in ListeRencontrePB)
            {
                if (!item.Valider)
                {
                    message.AppendLine("Toutes les rencontres de la poule B n'ont pas été jouées");
                    break;
                }
            }
            if (!string.IsNullOrEmpty(message.ToString()))
                MessageBox.Show(message.ToString());
            else
            {
                ListeEquipePF = new List<Participant>();
                ListeEquipePF.Add(ListeEquipePA.First(x => x.Classement == 1));
                ListeEquipePF.Add(ListeEquipePB.First(x => x.Classement == 2));
                ListeEquipePF.Add(ListeEquipePB.First(x => x.Classement == 1));
                ListeEquipePF.Add(ListeEquipePA.First(x => x.Classement == 2));
                ListeRencontrePF = EliminatoireHelper.GenererListeRencontre(ListeEquipePF, _choixRaquette, "Equipe1");
                xDGPhaseFinale.ItemsSource = ListeRencontrePF;
                xBtPF.IsEnabled = false;
                xBtTS.IsEnabled = true;
                xOngletPF.IsSelected = true;
            }
        }

        private void xBtTS_Click(object sender, RoutedEventArgs e)
        {
            if (EliminatoireHelper.PasserTourSuivant(ListeRencontrePF, ListeEquipePF, _choixRaquette))
            {
                xDGPhaseFinale.ItemsSource = null;
                xDGPhaseFinale.ItemsSource = ListeRencontrePF;
                xBtTS.IsEnabled = false;
                xBtFinTournoi.IsEnabled = true;
            }
        }

        private void xBtFinTournoi_Click(object sender, RoutedEventArgs e)
        {
            List<Rencontre> listeRencontreTotal = new List<Rencontre>();
            listeRencontreTotal.AddRange(ListeRencontrePA);
            listeRencontreTotal.AddRange(ListeRencontrePB);
            listeRencontreTotal.AddRange(ListeRencontrePF);
            List<Participant> listeParticipantTotal = new List<Participant>();
            listeParticipantTotal.AddRange(ListeEquipePA);
            listeParticipantTotal.AddRange(ListeEquipePB);
            if (DiversRules.EnregistrerResultat(listeRencontreTotal, _listeJoueur, _listeEquipe, listeParticipantTotal, _double, false, true, false))
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

        private void xDGPouleA_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Rencontre rencontre = xDGPouleA.CurrentItem as Rencontre;
            AfficheurHelper.AfficherMatch(rencontre, ListeRencontrePA);
        }

        private void xDGPouleB_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Rencontre rencontre = xDGPouleB.CurrentItem as Rencontre;
            AfficheurHelper.AfficherMatch(rencontre, ListeRencontrePB);
        }

        private void xDGPhaseFinale_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Rencontre rencontre = xDGPhaseFinale.CurrentItem as Rencontre;
            AfficheurHelper.AfficherMatch(rencontre, ListeRencontrePF);
        }

        private void xDGPouleA_KeyUp(object sender, KeyEventArgs e)
        {

            bool refresh = false;
            Rencontre rencontre = xDGPouleA.CurrentItem as Rencontre;
            if (e.Key == System.Windows.Input.Key.LeftCtrl)
            {
                rencontre = xDGPouleA.CurrentItem as Rencontre;
                rencontre.PointEquipe1 = rencontre.PointEquipe1 + 1;
                refresh = true;
            }
            if (e.Key == System.Windows.Input.Key.RightCtrl)
            {
                rencontre = xDGPouleA.CurrentItem as Rencontre;
                rencontre.PointEquipe2 = rencontre.PointEquipe2 + 1;
                refresh = true;
            }

            if (refresh)
            {
                int pos = xDGPouleA.SelectedIndex;
                xDGPouleA.Items.Refresh();
                AfficheurHelper.AfficherMatch(rencontre, ListeRencontrePA);
                xDGPouleA.Focus();
                xDGPouleA.Items.MoveCurrentToPosition(pos);
            }
        }

        private void xDGPouleB_KeyUp(object sender, KeyEventArgs e)
        {
            bool refresh = false;
            Rencontre rencontre = xDGPouleB.CurrentItem as Rencontre;
            if (e.Key == System.Windows.Input.Key.LeftCtrl)
            {
                rencontre = xDGPouleB.CurrentItem as Rencontre;
                rencontre.PointEquipe1 = rencontre.PointEquipe1 + 1;
                refresh = true;
            }
            if (e.Key == System.Windows.Input.Key.RightCtrl)
            {
                rencontre = xDGPouleB.CurrentItem as Rencontre;
                rencontre.PointEquipe2 = rencontre.PointEquipe2 + 1;
                refresh = true;
            }

            if (refresh)
            {
                int pos = xDGPouleB.SelectedIndex;
                xDGPouleB.Items.Refresh();
                AfficheurHelper.AfficherMatch(rencontre, ListeRencontrePB);
                xDGPouleB.Focus();
                xDGPouleB.Items.MoveCurrentToPosition(pos);
            }
        }
    }

}
