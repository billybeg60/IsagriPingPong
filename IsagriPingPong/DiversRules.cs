using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace IsagriPingPong
{
    public static class DiversRules
    {
        public static void RecalculerClassement(List<Rencontre> listeRencontre, List<Participant> listeEquipe)
        {
            ReinitialiserClassement(listeEquipe);

            foreach (Rencontre rencontre in listeRencontre)
            {
                if (rencontre.Valider)
                {
                    Participant equipe1 = listeEquipe.Find(x => x.Id == rencontre.Equipe1.Id);
                    Participant equipe2 = listeEquipe.Find(x => x.Id == rencontre.Equipe2.Id);

                    equipe1.NbMatchJoue++;
                    equipe2.NbMatchJoue++;
                    equipe1.Plus += rencontre.PointEquipe1;
                    equipe1.Moins += rencontre.PointEquipe2;
                    equipe2.Plus += rencontre.PointEquipe2;
                    equipe2.Moins += rencontre.PointEquipe1;

                    if (rencontre.PointEquipe1 > rencontre.PointEquipe2)
                    {
                        equipe1.NbVictoires++;
                    }
                    else
                    {
                        equipe2.NbVictoires++;
                    }
                }
            }

            List<Participant> listeEquipeOrdonnee = listeEquipe.OrderByDescending(x => x.NbVictoires).ThenByDescending(x => x.Difference).ThenByDescending(x => x.Plus).ToList();

            int cpt = 1;
            foreach (var item in listeEquipeOrdonnee)
            {
                Participant equipe = listeEquipe.Find(x => x.Id == item.Id);
                equipe.Classement = cpt;
                cpt++;
            }
            listeEquipe.Sort((x, y) => x.Classement.CompareTo(y.Classement));
        }

        private static void ReinitialiserClassement(List<Participant> listeEquipe)
        {
            foreach (var equipe in listeEquipe)
            {
                equipe.Classement = 0;
                equipe.NbVictoires = 0;
                equipe.Plus = 0;
                equipe.Moins = 0;
                equipe.NbMatchJoue = 0;
            }
        }

        public static bool ValiderScore(Rencontre rencontre)
        {
            bool annuler = false;

            if (rencontre == null)
                annuler = true;
            else
            {
                // Contrôler le score de la rencontre ici
                if (rencontre.PointEquipe1 < 0 || rencontre.PointEquipe2 < 0)
                {
                    MessageBox.Show("Le score ne peut pas être négatif");
                    annuler = true;
                }
                else if (rencontre.PointEquipe1 == 0 && rencontre.PointEquipe2 == 0)
                {
                    MessageBox.Show("Le score doit être supérieur à 0");
                    annuler = true;
                }
                else if (rencontre.PointEquipe1 == rencontre.PointEquipe2)
                {
                    MessageBox.Show("Il ne peut pas y avoir de match nul");
                    annuler = true;
                }
            }

            return annuler;
        }

        public static void DeterminerRaquette(List<Rencontre> listeRencontre, bool choixRaquette, string forcerEquipe)
        {
            Random aleatoire = new Random();

            foreach (var rencontre in listeRencontre)
            {
                if (!string.IsNullOrEmpty(forcerEquipe))
                {
                    if (choixRaquette)
                        rencontre.Raquette = forcerEquipe;
                }
                else
                {
                    if (choixRaquette)
                    {
                        int random = aleatoire.Next(2);
                        if (random == 0)
                        {
                            rencontre.Raquette = "Equipe1";
                        }
                        else
                            rencontre.Raquette = "Equipe2";
                    }
                }
            }
        }

        public static int CalculerNbMatch(List<Participant> listeParticipant, bool championnat, bool poule, bool eliminatoire, bool allerRetour)
        {
            int nbMatch = 0;
            int nbParticipant = listeParticipant.Count();

            if (championnat)
            {
                nbMatch = (nbParticipant * (nbParticipant - 1)) / 2;
            }
            else if (poule)
            {
                int nbParticipantParPoule = nbParticipant / 2;
                nbMatch = (((nbParticipantParPoule * (nbParticipantParPoule - 1)) / 2) * 2) + 3;
            }
            else if (eliminatoire)
            {
                nbMatch = nbParticipant - 1;
            }

            if (allerRetour)
                nbMatch = nbMatch * 2;

            return nbMatch;
        }

        public static bool EnregistrerResultat(List<Rencontre> listeRencontre, List<JoueurBDD> listeJoueur, List<EquipeBDD> listeEquipe, List<Participant> listeParticipant, bool enEquipe, bool championnat, bool poule, bool eliminatoire)
        {
            StringBuilder message = new StringBuilder();
            foreach (var item in listeRencontre)
            {
                if (!item.Valider)
                {
                    message.AppendLine("Toutes les rencontres n'ont pas été jouées");
                }
            }

            if (!string.IsNullOrEmpty(message.ToString()))
            {
                MessageBox.Show(message.ToString());
                return false;
            }
            else
            {
                Dictionary<string, int> dicoResultat = new Dictionary<string, int>();
                DiversRules.MajClassement(listeJoueur, listeRencontre, listeParticipant, dicoResultat, championnat, poule, eliminatoire);
                DataBaseRules.MAJClassementJoueur(listeJoueur);
                if (enEquipe)
                {
                    DiversRules.MajClassementEquipe(listeEquipe, listeRencontre, listeParticipant, championnat, poule, eliminatoire);
                    DataBaseRules.MAJClassementEquipe(listeEquipe);
                }
                ResultatHelper.AfficherResultat(dicoResultat, !enEquipe, championnat);
            }

            return true;
        }

        public static void MajClassement(List<JoueurBDD> listeJoueurComplet, List<Rencontre> listeRencontre, List<Participant> listeParticipant, Dictionary<string, int> dicoResultat, bool championnat, bool poule, bool eliminatoire)
        {
            JoueurBDD joueur11 = null;
            JoueurBDD joueur12 = null;
            JoueurBDD joueur21 = null;
            JoueurBDD joueur22 = null;
            foreach (var rencontre in listeRencontre)
            {
                joueur11 = listeJoueurComplet.Find(x => string.Equals(x.Nom, rencontre.Equipe1.Joueurs[0]));
                if (rencontre.Equipe1.Joueurs.Count > 1)
                    joueur12 = listeJoueurComplet.Find(x => string.Equals(x.Nom, rencontre.Equipe1.Joueurs[1]));
                else
                    joueur12 = null;
                joueur21 = listeJoueurComplet.Find(x => string.Equals(x.Nom, rencontre.Equipe2.Joueurs[0]));
                if (rencontre.Equipe2.Joueurs.Count > 1)
                    joueur22 = listeJoueurComplet.Find(x => string.Equals(x.Nom, rencontre.Equipe2.Joueurs[1]));
                else
                    joueur22 = null;

                if (rencontre.PointEquipe1 > rencontre.PointEquipe2)
                {
                    if (string.Equals(rencontre.Tour, "Finale"))
                    {
                        joueur11.NbTitres++;
                        joueur11.Points += 20;
                        joueur21.Points += 10;
                        dicoResultat.Add(joueur11.Mail, 20);
                        dicoResultat.Add(joueur21.Mail, 10);
                        if (joueur12 != null)
                        {
                            joueur12.NbTitres++;
                            joueur12.Points += 20;
                            dicoResultat.Add(joueur12.Mail, 20);                           
                        }

                        if (joueur22 != null)
                        {
                            joueur22.Points += 10;
                            dicoResultat.Add(joueur22.Mail, 10);
                        }

                        if (eliminatoire && listeRencontre.Count < 7)
                        {
                            joueur11.Points -= 10;
                            dicoResultat[joueur11.Mail] -= 10;
                            joueur21.Points -= 10;
                            dicoResultat.Remove(joueur21.Mail);

                            if (joueur12 != null)
                            {
                                joueur12.Points -= 10;
                                dicoResultat[joueur12.Mail] -= 10;
                            }

                            if (joueur22 != null)
                            {
                                joueur22.Points -= 10;
                                dicoResultat.Remove(joueur22.Mail);
                            }
                        }
                    }

                    joueur11.Points += 10;
                    joueur11.NbVictoires++;
                    joueur21.NbDefaites++;

                    if (joueur12 != null)
                    {
                        joueur12.Points += 10;
                        joueur12.NbVictoires++;
                    }

                    if (joueur22 != null)
                        joueur22.NbDefaites++;
                }
                else
                {
                    if (string.Equals(rencontre.Tour, "Finale"))
                    {
                        joueur21.NbTitres++;
                        joueur21.Points += 20;
                        joueur11.Points += 10;
                        dicoResultat.Add(joueur21.Mail, 20);
                        dicoResultat.Add(joueur11.Mail, 10);
                        if (joueur22 != null)
                        {
                            joueur22.NbTitres++;
                            joueur22.Points += 20;
                            dicoResultat.Add(joueur22.Mail, 20);
                        }

                        if (joueur12 != null)
                        {
                            joueur12.Points += 10;
                            dicoResultat.Add(joueur12.Mail, 10);
                        }

                        if (eliminatoire && listeRencontre.Count < 7)
                        {
                            joueur21.Points -= 10;
                            dicoResultat[joueur21.Mail] -= 10;
                            joueur11.Points -= 10;
                            dicoResultat.Remove(joueur11.Mail);

                            if (joueur22 != null)
                            {
                                joueur22.Points -= 10;
                                dicoResultat[joueur22.Mail] -= 10;
                            }

                            if (joueur12 != null)
                            {
                                joueur12.Points -= 10;
                                dicoResultat.Remove(joueur12.Mail);
                            }
                        }
                    }

                    joueur21.Points += 10;
                    joueur21.NbVictoires++;
                    joueur11.NbDefaites++;

                    if (joueur22 != null)
                    {
                        joueur22.Points += 10;
                        joueur22.NbVictoires++;
                    }

                    if (joueur12 != null)
                        joueur12.NbDefaites++;
                }
            }

            JoueurBDD joueur1 = null;
            JoueurBDD joueur2 = null;
            JoueurBDD joueur1Derniers = null;
            JoueurBDD joueur2Derniers = null;
            // Gestion des bonus
            foreach (var participant in listeParticipant)
            {
                joueur1 = listeJoueurComplet.Find(x => string.Equals(x.Nom, participant.Joueurs[0]));
                if (participant.Joueurs.Count > 1)
                    joueur2 = listeJoueurComplet.Find(x => string.Equals(x.Nom, participant.Joueurs[1]));
                else
                    joueur2 = null;

                joueur1.NbTournois++;
                if (joueur2 != null)
                    joueur2.NbTournois++;

                if (championnat)
                {
                    if (listeParticipant.Count == participant.Classement)
                    {
                        joueur1Derniers = joueur1;
                        if (joueur2 != null)
                            joueur2Derniers = joueur2;
                    }

                    if (listeParticipant.Count == 3)
                    {
                        if (participant.Classement == 1)
                        {
                            joueur1.NbTitres++;
                            joueur1.Points += 10;
                            dicoResultat.Add(joueur1.Mail, 10);
                            if (joueur2 != null)
                            {
                                joueur2.NbTitres++;
                                joueur2.Points += 10;
                                dicoResultat.Add(joueur2.Mail, 10);
                            }
                        }
                    }
                    else if (listeParticipant.Count == 4)
                    {
                        if (participant.Classement == 1)
                        {
                            joueur1.NbTitres++;
                            joueur1.Points += 20;
                            dicoResultat.Add(joueur1.Mail, 20);
                            if (joueur2 != null)
                            {
                                joueur2.NbTitres++;
                                joueur2.Points += 20;
                                dicoResultat.Add(joueur2.Mail, 20);
                            }
                        }
                        else if (participant.Classement == 2)
                        {
                            joueur1.Points += 10;
                            dicoResultat.Add(joueur1.Mail, 10);
                            if (joueur2 != null)
                            {
                                joueur2.Points += 10;
                                dicoResultat.Add(joueur2.Mail, 10);
                            }
                        }
                    }
                    else if (listeParticipant.Count > 4)
                    {
                        if (participant.Classement == 1)
                        {
                            joueur1.NbTitres++;
                            joueur1.Points += 30;
                            dicoResultat.Add(joueur1.Mail, 30);
                            if (joueur2 != null)
                            {
                                joueur2.NbTitres++;
                                joueur2.Points += 30;
                                dicoResultat.Add(joueur2.Mail, 30);
                            }
                        }
                        else if (participant.Classement == 2)
                        {
                            joueur1.Points += 20;
                            dicoResultat.Add(joueur1.Mail, 20);
                            if (joueur2 != null)
                            {
                                joueur2.Points += 20;
                                dicoResultat.Add(joueur2.Mail, 20);
                            }
                        }
                        else if (participant.Classement == 3)
                        {
                            joueur1.Points += 10;
                            dicoResultat.Add(joueur1.Mail, 10);
                            if (joueur2 != null)
                            {
                                joueur2.Points += 10;
                                dicoResultat.Add(joueur2.Mail, 10);
                            }
                        }
                    }
                }
                else if (poule)
                {
                    if (participant.Classement == 1 || participant.Classement == 2)
                    {
                        joueur1.Points += 10;
                        if (dicoResultat.ContainsKey(joueur1.Mail))
                            dicoResultat[joueur1.Mail] += 10;
                        else
                            dicoResultat.Add(joueur1.Mail, 10);
                        if (joueur2 != null)
                        {
                            joueur2.Points += 10;
                            if (dicoResultat.ContainsKey(joueur2.Mail))
                                dicoResultat[joueur2.Mail] += 10;
                            else
                                dicoResultat.Add(joueur2.Mail, 10);
                        }
                    }
                }
            }

            if (joueur1Derniers != null)            
                dicoResultat.Add(joueur1Derniers.Mail, 0);
            if (joueur2Derniers != null)
                dicoResultat.Add(joueur2Derniers.Mail, 0);

        }

        public static void MajClassementEquipe(List<EquipeBDD> listeEquipeExistante, List<Rencontre> listeRencontre, List<Participant> listeParticipant, bool championnat, bool poule, bool eliminatoire)
        {
            EquipeBDD equipe1 = null;
            EquipeBDD equipe2 = null;

            foreach (var rencontre in listeRencontre)
            {
                equipe1 = listeEquipeExistante.Find(x => (string.Equals(x.Joueur1Nom, rencontre.Equipe1.Joueurs[0]) && string.Equals(x.Joueur2Nom, rencontre.Equipe1.Joueurs[1]) ||
                                                         (string.Equals(x.Joueur1Nom, rencontre.Equipe1.Joueurs[1]) && string.Equals(x.Joueur2Nom, rencontre.Equipe1.Joueurs[0]))));

                equipe2 = listeEquipeExistante.Find(x => (string.Equals(x.Joueur1Nom, rencontre.Equipe2.Joueurs[0]) && string.Equals(x.Joueur2Nom, rencontre.Equipe2.Joueurs[1]) ||
                                                         (string.Equals(x.Joueur1Nom, rencontre.Equipe2.Joueurs[1]) && string.Equals(x.Joueur2Nom, rencontre.Equipe2.Joueurs[0]))));

                if (equipe1 == null)
                {
                    equipe1 = new EquipeBDD() { Joueur1Nom = rencontre.Equipe1.Joueurs[0], Joueur2Nom = rencontre.Equipe1.Joueurs[1] };
                    listeEquipeExistante.Add(equipe1);
                }

                if (equipe2 == null)
                {
                    equipe2 = new EquipeBDD() { Joueur1Nom = rencontre.Equipe2.Joueurs[0], Joueur2Nom = rencontre.Equipe2.Joueurs[1] };
                    listeEquipeExistante.Add(equipe2);
                }

                if (rencontre.PointEquipe1 > rencontre.PointEquipe2)
                {
                    if (string.Equals(rencontre.Tour, "Finale"))
                    {
                        equipe1.NbTitres++;
                        equipe1.Points += 20;
                        equipe2.Points += 10;
                        if (eliminatoire && listeRencontre.Count < 7)
                        {
                            equipe1.Points -= 10;
                            equipe2.Points -= 10;
                        }
                    }

                    equipe1.Points += 10;
                    equipe1.NbVictoires++;
                    equipe2.NbDefaites++;
                }
                else
                {
                    if (string.Equals(rencontre.Tour, "Finale"))
                    {
                        equipe2.NbTitres++;
                        equipe2.Points += 20;
                        equipe1.Points += 10;
                        if (eliminatoire && listeRencontre.Count < 7)
                        {
                            equipe2.Points -= 10;
                            equipe1.Points -= 10;
                        }
                    }

                    equipe2.Points += 10;
                    equipe2.NbVictoires++;
                    equipe1.NbDefaites++;
                }

                equipe1 = null;
                equipe2 = null;
            }

            EquipeBDD equipe = null;
            // Gestion des bonus
            foreach (var participant in listeParticipant)
            {
                equipe = listeEquipeExistante.Find(x => (string.Equals(x.Joueur1Nom, participant.Joueurs[0]) && string.Equals(x.Joueur2Nom, participant.Joueurs[1]) ||
                                                         (string.Equals(x.Joueur1Nom, participant.Joueurs[1]) && string.Equals(x.Joueur2Nom, participant.Joueurs[0]))));

                equipe.NbTournois++;

                if (championnat)
                {
                    if (listeParticipant.Count == 3)
                    {
                        if (participant.Classement == 1)
                        {
                            equipe.NbTitres++;
                            equipe.Points += 10;
                        }
                    }
                    else if (listeParticipant.Count == 4)
                    {
                        if (participant.Classement == 1)
                        {
                            equipe.NbTitres++;
                            equipe.Points += 20;
                        }
                        else if (participant.Classement == 2)
                            equipe.Points += 10;
                    }
                    else if (listeParticipant.Count > 4)
                    {
                        if (participant.Classement == 1)
                        {
                            equipe.NbTitres++;
                            equipe.Points += 30;
                        }
                        else if (participant.Classement == 2)
                            equipe.Points += 20;
                        else if (participant.Classement == 3)
                            equipe.Points += 10;
                    }
                }
                else if (poule)
                {
                    if (participant.Classement == 1 || participant.Classement == 2)
                        equipe.Points += 10;
                }
            }
        }
    }
}
