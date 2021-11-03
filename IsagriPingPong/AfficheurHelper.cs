using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using DevAfficheursTPV;

namespace IsagriPingPong
{
    public static class AfficheurHelper
    {
        public static void AfficherMessage(string ligneSup, string ligneInf)
        {
            AfficherMessage(ligneSup, ligneInf, "COM3");
            AfficherMessage(ligneSup, ligneInf, "COM4");
        }

        private static void AfficherMessage(string ligneSup, string ligneInf, string nomPortCom)
        {
            string enncodageParDefaut = Encoding.Default.EncodingName;
            using (ManagerAfficheurDev manager = new ManagerAfficheurDev(nomPortCom))
            {
                manager.AfficherLignes(ligneSup, ligneInf);
            }
        }

        public static void AfficherMatch(Rencontre rencontre, List<Rencontre> listeRencontre)
        {
            if (rencontre != null)
            {
                AfficherMessage(rencontre.Equipe1.NomAfficheur.PadRight(15) + " : " + rencontre.PointEquipe1, rencontre.Equipe2.NomAfficheur.PadRight(15) + " : " + rencontre.PointEquipe2, "COM3");

                int index = listeRencontre.FindIndex(x => x.Id == rencontre.Id);
                if (index < listeRencontre.Count - 1)
                {
                    AfficherMessage(listeRencontre[index + 1].Equipe1.NomAfficheur, listeRencontre[index + 1].Equipe2.NomAfficheur, "COM4");
                }
            }
        }
    }
}
