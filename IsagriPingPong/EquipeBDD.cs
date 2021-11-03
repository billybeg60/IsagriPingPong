using System;

namespace IsagriPingPong
{
    public class EquipeBDD
    {
        public string Joueur1Nom { get; set; }
        public string Joueur2Nom { get; set; }
        public string Nom
        {
            get
            {
                return Joueur1Nom + " / " + Joueur2Nom;
            }
        }
        public int Points { get; set; }
        public int NbVictoires { get; set; }
        public int NbDefaites { get; set; }
        public int NbTournois { get; set; }
        public int NbTitres { get; set; }

        public int NbMatch
        {
            get
            {
                return NbVictoires + NbDefaites;
            }
        }
        public Decimal Pourcentage
        {
            get
            {
                if (NbMatch != 0)
                    return Math.Round(Decimal.Divide(NbVictoires * 100, NbMatch), 2);
                else
                    return 0;
            }
        }

        public Decimal RatioPoint
        {
            get
            {
                if (NbMatch != 0)
                    return Math.Round(Decimal.Divide(Points, NbMatch), 2);
                else
                    return 0;
            }
        }
    }
}
