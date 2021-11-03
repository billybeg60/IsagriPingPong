using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace IsagriPingPong
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// Liste des joueurs
        /// </summary>
        public List<string> ListeJoueurs
        {
            get { return (List<string>)GetValue(ListeJoueursProperty); }
            set { SetValue(ListeJoueursProperty, value); }
        }

        public static readonly DependencyProperty ListeJoueursProperty =
            DependencyProperty.Register("ListeJoueurs", typeof(List<string>), typeof(MainWindow));

        /// <summary>
        /// Liste des joueurs
        /// </summary>
        public List<JoueurBDD> ListeJoueurClassement
        {
            get { return (List<JoueurBDD>)GetValue(ListeJoueurClassementProperty); }
            set { SetValue(ListeJoueurClassementProperty, value); }
        }

        public static readonly DependencyProperty ListeJoueurClassementProperty =
            DependencyProperty.Register("ListeJoueurClassement", typeof(List<JoueurBDD>), typeof(MainWindow));

        /// <summary>
        /// Liste des Equipe
        /// </summary>
        public List<EquipeBDD> ListeEquipeClassement
        {
            get { return (List<EquipeBDD>)GetValue(ListeEquipeClassementProperty); }
            set { SetValue(ListeEquipeClassementProperty, value); }
        }

        public static readonly DependencyProperty ListeEquipeClassementProperty =
            DependencyProperty.Register("ListeEquipeClassement", typeof(List<EquipeBDD>), typeof(MainWindow));

        public int NbJoueurs
        {
            get { return (int)GetValue(NbJoueursProperty); }
            set { SetValue(NbJoueursProperty, value); }
        }

        public static readonly DependencyProperty NbJoueursProperty =
            DependencyProperty.Register("NbJoueurs", typeof(int), typeof(MainWindow));

        public MainWindow()
        {
            Initialisation();           
        }

        private void Initialisation()
        {
            InitializeComponent();
            this.DataContext = this;

            // lecture des joueurs
            xLBJoueurs.Items.Clear();
            xLBJoueursSelection.Items.Clear();
            if (ListeJoueurClassement != null)
                ListeJoueurClassement.Clear();
            LireListeJoueurs();
            // lecture des équipes
            if (ListeEquipeClassement != null)
                ListeEquipeClassement.Clear();
            LireListeEquipes();

            NbJoueurs = 0;
            ListeJoueurs = new List<string>();
        }

        private void LancerPartie()
        {            
            ListeJoueurs.Clear();

            foreach (string item in xLBJoueursSelection.Items)
            {
                ListeJoueurs.Add(item);
            }

            List<Participant> listeParticipants = JoueursRules.CreerParticipants(ListeJoueurs, ListeJoueurClassement, NbJoueurs,
                                                                xRBDouble.IsChecked.HasValue ? xRBDouble.IsChecked.Value : false,
                                                                xRBTirageAleatoire.IsChecked.HasValue ? xRBTirageAleatoire.IsChecked.Value : false,
                                                                xRBTirageNiveau.IsChecked.HasValue ? xRBTirageNiveau.IsChecked.Value : false,
                                                                xRBTirageRatio.IsChecked.HasValue ? xRBTirageRatio.IsChecked.Value : false);

            StringBuilder message = new StringBuilder();
            message.AppendLine("Voici la liste des joueurs/équipes : ");
            message.AppendLine();

            int cpt = 1;
            foreach (Participant item in listeParticipants)
            {

                if (xRBPoule.IsChecked.HasValue && xRBPoule.IsChecked.Value)
                {
                    if (cpt == 1)
                        message.AppendLine("Poule A : ");
                    else if (cpt - 1 == listeParticipants.Count / 2)
                    {
                        message.AppendLine();
                        message.AppendLine("Poule B : ");
                    }
                }

                message.AppendLine(item.Nom);
                cpt++;
            }
            message.AppendLine();

            int nbMatch = DiversRules.CalculerNbMatch(listeParticipants, xRBChampionnat.IsChecked.HasValue && xRBChampionnat.IsChecked.Value,
                                                           xRBPoule.IsChecked.HasValue && xRBPoule.IsChecked.Value,
                                                           xRBElimination.IsChecked.HasValue && xRBElimination.IsChecked.Value,
                                                           xCBAR.IsChecked.HasValue ? xCBAR.IsChecked.Value : false);
            message.AppendLine("Nombre de rencontres : " + nbMatch);
            message.AppendLine("Durée approximative : ");
            message.AppendLine("  - 7pt gagnant : " + (int)(nbMatch * 2.5) + " min");
            message.AppendLine("  - 11pt gagnant : " + (int)(nbMatch * 4) + " min");

            message.AppendLine();
            message.AppendLine("Démarrer le tournoi ?");

            
            if (MessageBox.Show(message.ToString(), "Equipes", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                AfficheurHelper.AfficherMessage("Place au jeu !", "Soyez sans pitié !");
                if (xRBChampionnat.IsChecked.HasValue && xRBChampionnat.IsChecked.Value)
                {
                    Championnat championnat = new Championnat(ListeJoueurClassement, ListeEquipeClassement, listeParticipants,
                                                                           xRBDouble.IsChecked.HasValue ? xRBDouble.IsChecked.Value : false,
                                                                           xCBAR.IsChecked.HasValue ? xCBAR.IsChecked.Value : false,
                                                                           xCBCR.IsChecked.HasValue ? xCBCR.IsChecked.Value : false);
                    championnat.ShowDialog();
                }
                else if (xRBPoule.IsChecked.HasValue && xRBPoule.IsChecked.Value)
                {
                    PhasePoule phasePoule = new PhasePoule(ListeJoueurClassement, ListeEquipeClassement, listeParticipants,
                        xRBDouble.IsChecked.HasValue ? xRBDouble.IsChecked.Value : false,
                        xCBCR.IsChecked.HasValue ? xCBCR.IsChecked.Value : false);
                    phasePoule.ShowDialog();
                }
                else if (xRBElimination.IsChecked.HasValue && xRBElimination.IsChecked.Value)
                {
                    Eliminatoire eliminatoire = new Eliminatoire(ListeJoueurClassement, ListeEquipeClassement, listeParticipants,
                                                                              xRBDouble.IsChecked.HasValue ? xRBDouble.IsChecked.Value : false,
                                                                              xRBTirageAleatoire.IsChecked.HasValue ? xRBTirageAleatoire.IsChecked.Value : false,
                                                                              xCBCR.IsChecked.HasValue ? xCBCR.IsChecked.Value : false);
                    eliminatoire.ShowDialog();
                }
                Initialisation();
            }
            else
            {
                listeParticipants.Clear();
                ListeJoueurs.Clear();
            }
        }

        private void LireListeJoueurs()
        {
            ListeJoueurClassement = DataBaseRules.ChargerListeJoueur();

            foreach (var item in ListeJoueurClassement)
            {
                xLBJoueurs.Items.Add(item.Nom);
            }

            ListeJoueurClassement.Sort((x, y) => y.Points.CompareTo(x.Points));
        }

        private void LireListeEquipes()
        {
            ListeEquipeClassement = DataBaseRules.ChargerListeEquipe();
            ListeEquipeClassement.Sort((x, y) => y.Points.CompareTo(x.Points));
        }

        private void xBlancer_Click(object sender, RoutedEventArgs e)
        {
            bool enEquipe = xRBDouble.IsChecked.HasValue && xRBDouble.IsChecked.Value;
            bool EstChampionnat = xRBChampionnat.IsChecked.HasValue && xRBChampionnat.IsChecked.Value;
            bool EstPoule = xRBPoule.IsChecked.HasValue && xRBPoule.IsChecked.Value;
            bool estElimination = xRBElimination.IsChecked.HasValue && xRBElimination.IsChecked.Value;

            if (JoueursRules.ControlerNbJoueurs(NbJoueurs, enEquipe, EstChampionnat, EstPoule, estElimination))
                LancerPartie();
        }

        private void xBAjout_Click(object sender, RoutedEventArgs e)
        {
            for (int i = xLBJoueurs.SelectedItems.Count - 1; i >= 0; i--)
            {
                var selection = xLBJoueurs.SelectedItems[i];
                xLBJoueursSelection.Items.Add(selection);
                xLBJoueurs.Items.Remove(selection);
                NbJoueurs++;
            }
        }

        private void xBSuppr_Click(object sender, RoutedEventArgs e)
        {
            for (int i = xLBJoueursSelection.SelectedItems.Count - 1; i >= 0; i--)
            {
                var selection = xLBJoueursSelection.SelectedItems[i];
                xLBJoueurs.Items.Add(selection);
                xLBJoueursSelection.Items.Remove(selection);
                NbJoueurs--;
            }
        }

        private void xDGClassement_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void xDGClassementEquipe_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void xBtAddJoueur_Click(object sender, RoutedEventArgs e)
        {
            NouveauJoueur nouveauJoueur = new NouveauJoueur();
            nouveauJoueur.ShowDialog();
            Initialisation();
        }

        private void xBtEnvoyerClassement_Click(object sender, RoutedEventArgs e)
        {
            ResultatHelper.CaptureScreen(xDGClassement);
        }

        private void xBtEnvoyerClassementEquipe_Click(object sender, RoutedEventArgs e)
        {
            ResultatHelper.CaptureScreen(xDGClassementEquipe);
        }        
    }
}
