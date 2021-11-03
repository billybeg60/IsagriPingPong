using System;
using System.Windows;

namespace IsagriPingPong
{
    /// <summary>
    /// Interaction logic for NouveauJoueur.xaml
    /// </summary>
    public partial class NouveauJoueur : Window
    {
        public NouveauJoueur()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            JoueurBDD joueur = new JoueurBDD()
            {
                Nom = xTBNom.Text,
                Mail = xTBMail.Text,
                Niveau = Int32.Parse(xCBNiveau.SelectionBoxItem.ToString())
            };

            if (string.IsNullOrEmpty(joueur.Nom))
                MessageBox.Show("Le nom est obligatoire");
            else if (string.IsNullOrEmpty(joueur.Mail))
                MessageBox.Show("Le mail est obligatoire");
            else
            {
                DataBaseRules.AjouterJoueur(joueur);
                this.Close();
            }
        }
    }
}
