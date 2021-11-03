using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace IsagriPingPong
{
    public static class JoueursRules
    {
        public static bool ControlerNbJoueurs(int nbJoueurs, bool enEquipe, bool estChampionnat, bool estPoule, bool estEliminatoire)
        {
            if (enEquipe && nbJoueurs % 2 == 1)
            {
                MessageBox.Show("Le nombre de joueurs doit être pair");
            }
            else
            {
                if (estChampionnat)
                {
                    if (enEquipe && nbJoueurs < 6)
                    {
                        MessageBox.Show("Vous devez sélectionner au moins 6 joueurs pour faire un championnat en double");
                    }
                    else if (!enEquipe && nbJoueurs < 3)
                    {
                        MessageBox.Show("Vous devez sélectionner au moins 3 joueurs pour faire un championnat en simple");
                    }
                    else
                        return true;
                }
                else if (estPoule)
                {
                    if (enEquipe)
                    {
                        if (nbJoueurs < 12)
                            MessageBox.Show("Vous devez sélectionner au moins 12 joueurs pour faire une phase de poule en double");
                        else if ((nbJoueurs / 2) % 2 == 1)
                            MessageBox.Show("Le nombre d'équipes doit être pair");
                        else
                            return true;
                    }
                    else
                    {
                        if (nbJoueurs < 6)
                            MessageBox.Show("Vous devez sélectionner au moins 6 joueurs pour faire une phase de poule en simple");
                        else if (nbJoueurs % 2 == 1)
                            MessageBox.Show("Le nombre de joueurs doit être pair");
                        else
                            return true;
                    }
                }
                else if (estEliminatoire)
                {
                    if (enEquipe && nbJoueurs < 8)
                    {
                        MessageBox.Show("Vous devez sélectionner au moins 8 joueurs pour faire un tournoi à élimination directe en double");
                    }
                    else if (!enEquipe && nbJoueurs < 4)
                    {
                        MessageBox.Show("Vous devez sélectionner au moins 4 joueurs pour faire un tournoi à élimination directe en simple");
                    }
                    else
                        return true;
                }
            }

            return false;
        }

        public static List<Participant> CreerParticipants(List<string> listeJoueurs, List<JoueurBDD> listeJoueurComplet, int nbJoueurs, bool enEquipe, bool tirageAleatoire, bool tirageParNiveau, bool tirageParRatio)
        {
            List<Participant> listeEquipe = new List<Participant>();
            int id = 0;
            Random aleatoire = new Random();

            // Double
            if (enEquipe)
            {
                if (tirageAleatoire)
                {
                    Participant equipeCourante = null;
                    for (int i = 0; i < nbJoueurs; i++)
                    {
                        int random = aleatoire.Next(listeJoueurs.Count);
                        string joueur = listeJoueurs[random];
                        listeJoueurs.RemoveAt(random);
                        if (equipeCourante == null)
                        {
                            equipeCourante = new Participant() { Id = id };
                            equipeCourante.Joueurs.Add(joueur);
                        }
                        else
                        {
                            equipeCourante.Joueurs.Add(joueur);
                            listeEquipe.Add(equipeCourante);
                            equipeCourante = null;
                            id++;
                        }
                    }
                }
                else if (tirageParNiveau || tirageParRatio)
                {
                    List<JoueurBDD> listeJoueurTriees = new List<JoueurBDD>();
                    // Récupération des joueur
                    foreach (string item in listeJoueurs)
                    {
                        listeJoueurTriees.Add(listeJoueurComplet.Find(x => string.Equals(x.Nom, item)));
                    }
                    if (tirageParNiveau)
                        listeJoueurTriees = listeJoueurTriees.OrderBy(x => x.Niveau).ToList();
                    else
                        listeJoueurTriees = listeJoueurTriees.OrderBy(x => x.Pourcentage).ToList();
                    List<JoueurBDD> ListeJoueurFaible = listeJoueurTriees.GetRange(0, (listeJoueurTriees.Count / 2));
                    List<JoueurBDD> ListeJoueurFort = listeJoueurTriees.GetRange(listeJoueurTriees.Count / 2, listeJoueurTriees.Count / 2);

                    Participant equipeCourante = null;
                    for (int i = 0; i < listeJoueurTriees.Count / 2; i++)
                    {
                        int random = aleatoire.Next(ListeJoueurFaible.Count);
                        JoueurBDD joueur1 = ListeJoueurFaible[random];
                        ListeJoueurFaible.RemoveAt(random);
                        int random2 = aleatoire.Next(ListeJoueurFort.Count);
                        JoueurBDD joueur2 = ListeJoueurFort[random2];
                        ListeJoueurFort.RemoveAt(random2);
                        equipeCourante = new Participant() { Id = id };
                        equipeCourante.Joueurs.Add(joueur1.Nom);
                        equipeCourante.Joueurs.Add(joueur2.Nom);
                        listeEquipe.Add(equipeCourante);
                        id++;
                    }
                }
                else
                {
                    Participant equipeCourante = null;
                    for (int i = 0; i < nbJoueurs; i++)
                    {
                        string joueur = listeJoueurs[i];
                        if (equipeCourante == null)
                        {
                            equipeCourante = new Participant() { Id = id };
                            equipeCourante.Joueurs.Add(joueur);
                        }
                        else
                        {
                            equipeCourante.Joueurs.Add(joueur);
                            listeEquipe.Add(equipeCourante);
                            equipeCourante = null;
                            id++;
                        }
                    }
                }
            }
            // Simple
            else
            {
                for (int i = 0; i < nbJoueurs; i++)
                {
                    string joueur;
                    if (tirageAleatoire)
                    {
                        int random = aleatoire.Next(listeJoueurs.Count);
                        joueur = listeJoueurs[random];
                        listeJoueurs.RemoveAt(random);
                    }
                    else
                        joueur = listeJoueurs[i];

                    Participant equipe = new Participant() { Id = id };
                    equipe.Joueurs.Add(joueur);
                    listeEquipe.Add(equipe);
                    id++;
                }
            }

            return listeEquipe;
        }
    }
}
