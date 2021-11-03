using System.Collections.Generic;

namespace IsagriPingPong
{
    public class Participant
    {
        public int Id { get; set; }
        public int Classement { get; set; }
        public int NbMatchJoue { get; set; }
        public int NbVictoires { get; set; }
        public int Plus { get; set; }
        public int Moins { get; set; }
        /// <summary>
        /// Liste des lignes CAVE du fichier
        /// </summary>
        private List<string> _joueurs = new List<string>();
        /// <summary>
        /// Liste des lignes CAVE du fichier
        /// </summary>        
        public List<string> Joueurs
        {
            get { return _joueurs; }
            private set { _joueurs = value; }
        }
        public string Nom
        {
            get
            {
                if (Joueurs.Count == 2)
                    return "Team " + Joueurs[0] + " / " + Joueurs[1];
                else if (Joueurs.Count == 1)
                    return Joueurs[0];
                else
                    return string.Empty;
            }
        }

        public string NomAfficheur
        {
            get
            {
                if (Joueurs.Count == 2)
                {
                    string joueur1 = Joueurs[0];
                    string joueur2 = Joueurs[1];
                    if (joueur1.Length > 7)
                        joueur1 = joueur1.Substring(0, 5);
                    if (joueur2.Length > 7)
                        joueur2 =joueur2.Substring(0, 5);

                    return joueur1 + "/" + joueur2;
                }
                else if (Joueurs.Count == 1)
                    return Joueurs[0];
                else
                    return string.Empty;
            }
        }

        public int Difference
        {
            get
            {
                return Plus - Moins;
            }
        }
    }
}
