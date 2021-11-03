using System;
using System.Collections.Generic;
using System.Linq;

namespace IsagriPingPong
{
    public static class RoundRobinHelper
    {
        private const int BYE = -1;

        // Return an array where results(i, j) gives
        // the opponent of team i in round j.
        // Note: num_teams must be odd.
        private static int[,] GenerateRoundRobinOdd(int num_teams)
        {
            int n2 = (int)((num_teams - 1) / 2);
            int[,] results = new int[num_teams, num_teams];

            // Initialize the list of teams.
            int[] teams = new int[num_teams];
            for (int i = 0; i < num_teams; i++) teams[i] = i;

            // Start the rounds.
            for (int round = 0; round < num_teams; round++)
            {
                for (int i = 0; i < n2; i++)
                {
                    int team1 = teams[n2 - i];
                    int team2 = teams[n2 + i + 1];
                    results[team1, round] = team2;
                    results[team2, round] = team1;
                }

                // Set the team with the bye.
                results[teams[0], round] = BYE;

                // Rotate the array.
                RotateArray(teams);
            }

            return results;
        }

        // Rotate the entries one position.
        private static void RotateArray(int[] teams)
        {
            int tmp = teams[teams.Length - 1];
            Array.Copy(teams, 0, teams, 1, teams.Length - 1);
            teams[0] = tmp;
        }

        private static int[,] GenerateRoundRobinEven(int num_teams)
        {
            // Generate the result for one fewer teams.
            int[,] results = GenerateRoundRobinOdd(num_teams - 1);

            // Copy the results into a bigger array,
            // replacing the byes with the extra team.
            int[,] results2 = new int[num_teams, num_teams - 1];
            for (int team = 0; team < num_teams - 1; team++)
            {
                for (int round = 0; round < num_teams - 1; round++)
                {
                    if (results[team, round] == BYE)
                    {
                        // Change the bye to the new team.
                        results2[team, round] = num_teams - 1;
                        results2[num_teams - 1, round] = team;
                    }
                    else
                    {
                        results2[team, round] = results[team, round];
                    }
                }
            }

            return results2;
        }

        private static int[,] GenerateRoundRobin(int num_teams)
        {
            if (num_teams % 2 == 0)
                return GenerateRoundRobinEven(num_teams);
            else
                return GenerateRoundRobinOdd(num_teams);
        }

        public static List<Rencontre> GenererListeRencontre(List<Participant> listeEquipe, bool allerRetour)
        {
            List<Rencontre> listeRencontre = new List<Rencontre>();
            int match = 1;
            int[,] calendrier = RoundRobinHelper.GenerateRoundRobin(listeEquipe.Count());
            Rencontre rencontreCourante = null;
            for (int y = 0; y < calendrier.GetLength(1); y += 1)
            {
                for (int x = 0; x < calendrier.GetLength(0); x += 1)
                {
                    int equipeId = calendrier[x, y];
                    if (equipeId != -1)
                    {
                        // Match aller
                        if (!listeRencontre.Exists(z => z.Equipe2.Id == x && z.Equipe1.Id == equipeId))
                        {
                            rencontreCourante = new Rencontre() { Id = match, Journee = y + 1 };
                            rencontreCourante.Equipe1 = listeEquipe[x];
                            rencontreCourante.Equipe2 = listeEquipe[equipeId];
                            listeRencontre.Add(rencontreCourante);
                            rencontreCourante = null;
                            match++;
                        }
                        // Match retour
                        else if (allerRetour)
                        {
                            rencontreCourante = new Rencontre() { Id = match, Journee = calendrier.GetLength(1) + y + 1 };
                            rencontreCourante.Equipe1 = listeEquipe[x];
                            rencontreCourante.Equipe2 = listeEquipe[equipeId];
                            listeRencontre.Add(rencontreCourante);
                            rencontreCourante = null;
                            match++;
                        }
                    }
                }
            }

            listeRencontre.Sort((x, y) => x.Journee.CompareTo(y.Journee));
            return listeRencontre;
        }
    }
}
