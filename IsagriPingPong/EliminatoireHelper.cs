using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace IsagriPingPong
{
    public static class EliminatoireHelper
    {

        public static List<Rencontre> GenererListeRencontre(List<Participant> listeEquipe, bool choixRaquette, string forcerEquipe)
        {
            List<Rencontre> listeRencontre = new List<Rencontre>();

            int nbJoueurs = listeEquipe.Count;
            int nbJoueursQualifies = 0;
            double max = 0;

            // Nombre de joueurs max : 32
            for (int power = 1; power <= 5; power++)
            {
                max = Math.Pow(2, (double)power);

                if (nbJoueurs <= max)
                {
                    nbJoueursQualifies = (int)max - nbJoueurs;
                    break;
                }
            }

            List<Participant> listeEquipeConcerne = listeEquipe.GetRange(0, nbJoueurs - nbJoueursQualifies);

            int match = 0;
            Rencontre rencontreCourante = null;
            for (int i = 0; i < listeEquipeConcerne.Count - 1; i = i + 2)
            {
                rencontreCourante = new Rencontre() { Id = match, Tour = DonnerLibelleTour((int)max) };
                rencontreCourante.Equipe1 = listeEquipe[i];
                rencontreCourante.Equipe2 = listeEquipe[i + 1];
                listeRencontre.Add(rencontreCourante);
                match++;
            }

            DiversRules.DeterminerRaquette(listeRencontre, choixRaquette, forcerEquipe);

            return listeRencontre;
        }

        private static string DonnerLibelleTour(int nbJoueurs)
        {
            if (nbJoueurs == 2)
                return "Finale";
            else
                return "1/" + nbJoueurs / 2 + " finale";
        }

        public static bool PasserTourSuivant(List<Rencontre> listeRencontreActuel, List<Participant> listeEquipe, bool choixRaquette)
        {            
            StringBuilder message = new StringBuilder();
            foreach (var item in listeRencontreActuel)
            {
                if (!item.Valider)
                {
                    message.AppendLine("Toutes les rencontres n'ont pas été jouées");
                    break;
                }
            }

            if (!string.IsNullOrEmpty(message.ToString()))
            {
                MessageBox.Show(message.ToString());
                return false;
            }
            else
            {
                foreach (var rencontre in listeRencontreActuel)
                {
                    if (rencontre.PointEquipe1 > rencontre.PointEquipe2)
                        listeEquipe.Remove(rencontre.Equipe2);
                    else
                        listeEquipe.Remove(rencontre.Equipe1);
                }

                listeRencontreActuel.AddRange(EliminatoireHelper.GenererListeRencontre(listeEquipe, choixRaquette, null));

                return true;               
            }
        }
    }
}
