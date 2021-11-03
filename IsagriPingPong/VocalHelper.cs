using System;
using System.Collections.Generic;
using System.Speech.Synthesis;

namespace IsagriPingPong
{
    public static class VocalHelper
    {
        public static void GenererPhraseFinMatch(Rencontre rencontre)
        {
            string phrase = string.Empty;        
            int index = new Random().Next(0,3);
            if (rencontre.PointEquipe1 > rencontre.PointEquipe2)
            {
                if (rencontre.PointEquipe2 == 0)
                {
                    phrase = string.Format(_phrase0Pt[index], rencontre.Equipe1.NomAfficheur, rencontre.Equipe2.NomAfficheur);
                }
                else if (rencontre.PointEquipe2 == 1)
                {
                    phrase = string.Format(_phrase1Pt[index], rencontre.Equipe1.NomAfficheur, rencontre.Equipe2.NomAfficheur);
                }
                else if (rencontre.PointEquipe2 == 2 || rencontre.PointEquipe2 == 3)
                {
                    phrase = string.Format(_phrase23Pt[index], rencontre.Equipe1.NomAfficheur, rencontre.Equipe2.NomAfficheur);
                }
                else if (rencontre.PointEquipe2 == 4 || rencontre.PointEquipe2 == 5)
                {
                    phrase = string.Format(_phrase45Pt[index], rencontre.Equipe1.NomAfficheur, rencontre.Equipe2.NomAfficheur);
                }
                else if (rencontre.PointEquipe2 == 6)
                {
                    phrase = string.Format(_phrase6Pt[index], rencontre.Equipe1.NomAfficheur, rencontre.Equipe2.NomAfficheur);
                }
            }
            else
            {
                if (rencontre.PointEquipe1 == 0)
                {
                    phrase = string.Format(_phrase0Pt[index], rencontre.Equipe2.NomAfficheur, rencontre.Equipe1.NomAfficheur);
                }
                else if (rencontre.PointEquipe1 == 1)
                {
                    phrase = string.Format(_phrase1Pt[index], rencontre.Equipe2.NomAfficheur, rencontre.Equipe1.NomAfficheur);
                }
                else if (rencontre.PointEquipe1 == 2 || rencontre.PointEquipe1 == 3)
                {
                    phrase = string.Format(_phrase23Pt[index], rencontre.Equipe2.NomAfficheur, rencontre.Equipe1.NomAfficheur);
                }
                else if (rencontre.PointEquipe1 == 4 || rencontre.PointEquipe1 == 5)
                {
                    phrase = string.Format(_phrase45Pt[index], rencontre.Equipe2.NomAfficheur, rencontre.Equipe1.NomAfficheur);
                }
                else if (rencontre.PointEquipe1 == 6)
                {
                    phrase = string.Format(_phrase6Pt[index], rencontre.Equipe2.NomAfficheur, rencontre.Equipe1.NomAfficheur);
                }
            }

            SpeechSynthesizer speechSynthesizerObj = new SpeechSynthesizer();
            speechSynthesizerObj.Speak(phrase.Replace("/", ""));
        }

        static List<string> _phrase0Pt = new List<string>()
        {
            "Bravo {0} tu as démonté le cul de {1}",
            "Félicitation {0} tu as fisté l'anus de {1}",
            "Good job {0} tu as sodomisé {1}",
            "Bien joué {0} tu as fait du sale à {1}"
        };

        static List<string> _phrase1Pt = new List<string>()
        {
            "Bravo {0} tu as broyé {1}",
            "Félicitation {0} tu as roulé sur {1}",
            "Good job {0} tu as atomisé {1}",
            "Bien joué {0} tu as déglingué {1}"
        };

        static List<string> _phrase23Pt = new List<string>()
        {
            "Bravo {0} tu as battu {1} sans forcer",
            "Félicitation {0} tu as battu {1} tranquilement",
            "Good job {0} tu as battu {1} en marchant",
            "Bien joué {0} tu as battu {1} de la main gauche",
        };

        static List<string> _phrase45Pt = new List<string>()
        {
            "Bravo {0} tu as dominé {1} au forceps",
            "Félicitation {0} tu as dominé {1} à la bagarre",
            "Good job {0} tu as dominé {1} en serrant le jeu",
            "Bien joué {0} tu as dominé {1} au mental",
        };

        static List<string> _phrase6Pt = new List<string>()
        {
            "Bravo {0} tu t'es arraché face à {1} qui a le seum de ouf",
            "Félicitation {0} tu t'impose face à {1} qui a craqué mentalement",
            "Good job {0} tu domines {1} qui va chialer pendant longtemps",
            "Bien joué {0} tu domines {1} qui va rager comme jamais"
        };
    }
}
