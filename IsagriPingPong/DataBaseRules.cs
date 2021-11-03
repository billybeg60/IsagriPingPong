using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace IsagriPingPong
{
    public static class DataBaseRules
    {
        /// <summary>
        /// Renvoi la chaine de connexion à la BDD
        /// </summary>
        public static string GetConnectionString
        {            
            get { return "Data Source=CPOIREL19-DE\\IP08R2;Initial Catalog=ISAPingPong;User ID=PingPong;Password=Ping_Pong;Connect Timeout=10"; }
        }

        public static void AjouterJoueur(JoueurBDD joueur)
        {
            using (SqlConnection connect = new SqlConnection(DataBaseRules.GetConnectionString))
            {
                try
                {
                    connect.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connect;
                        command.CommandText = "INSERT INTO Joueur(Nom, Niveau, Mail) VALUES " +
                                              "(@NOM, @NIVEAU, @MAIL) "+
                                              "INSERT INTO Classement VALUES " +
                                              "(@NOM, 0,0,0,0,0) ";
                        command.Parameters.AddWithValue("@NOM", joueur.Nom);
                        command.Parameters.AddWithValue("@Niveau", joueur.Niveau);
                        command.Parameters.AddWithValue("@Mail", joueur.Mail);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
        }        

        public static List<JoueurBDD> ChargerListeJoueur()
        {
            List<JoueurBDD> listeJoueur = new List<JoueurBDD>();
            using (SqlConnection connect = new SqlConnection(DataBaseRules.GetConnectionString))
            {
                try
                {
                    connect.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        StringBuilder requete = new StringBuilder();
                        requete.Append("SELECT J.Nom, J.Niveau, C.Points, C.NbVictoires, C.NbDefaites, C.NbTournois, C.NbTitres, J.Mail FROM Joueur J ");
                        requete.Append("INNER JOIN Classement C on J.Nom = C.JoueurNom ");
                        requete.Append("Order by J.Nom");
                        command.Connection = connect;
                        command.CommandText = requete.ToString();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                JoueurBDD joueur = new JoueurBDD();
                                joueur.Nom = reader.GetString(0);
                                joueur.Niveau = reader.GetInt16(1);
                                joueur.Points = reader.GetInt32(2);
                                joueur.NbVictoires = reader.GetInt32(3);
                                joueur.NbDefaites = reader.GetInt32(4);
                                joueur.NbTournois = reader.GetInt32(5);
                                joueur.NbTitres = reader.GetInt32(6);
                                joueur.Mail = reader.GetString(7);
                                listeJoueur.Add(joueur);
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
            return listeJoueur;
        }

        public static List<EquipeBDD> ChargerListeEquipe()
        {
            List<EquipeBDD> listeEquipe = new List<EquipeBDD>();
            using (SqlConnection connect = new SqlConnection(DataBaseRules.GetConnectionString))
            {
                try
                {
                    connect.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        StringBuilder requete = new StringBuilder();
                        requete.Append("SELECT E.Joueur1Nom, E.Joueur2Nom, E.Points, E.NbVictoires, E.NbDefaites, E.NbTournois, E.NbTitres FROM Equipe E ");
                        requete.Append("Order by E.Joueur1Nom, E.Joueur2Nom");
                        command.Connection = connect;
                        command.CommandText = requete.ToString();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EquipeBDD equipe = new EquipeBDD();
                                equipe.Joueur1Nom = reader.GetString(0);
                                equipe.Joueur2Nom = reader.GetString(1);
                                equipe.Points = reader.GetInt32(2);
                                equipe.NbVictoires = reader.GetInt32(3);
                                equipe.NbDefaites = reader.GetInt32(4);
                                equipe.NbTournois = reader.GetInt32(5);
                                equipe.NbTitres = reader.GetInt32(6);
                                listeEquipe.Add(equipe);
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
            return listeEquipe;
        }

        public static void MAJClassementJoueur(List<JoueurBDD> listeJoueur)
        {
            using (SqlConnection connect = new SqlConnection(DataBaseRules.GetConnectionString))
            {
                try
                {
                    connect.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connect;
                        foreach (var item in listeJoueur)
                        {
                            command.Parameters.Clear();
                            command.CommandText = "UPDATE Classement SET Points = @POINTS, " +
                                                  "NbVictoires = @NBVICTOIRES, " +
                                                  "NbDefaites = @NBDEFAITES, " +
                                                  "NbTournois = @NBTOURNOIS, " +
                                                  "NbTitres = @NBTITRES " +
                                                  "WHERE JoueurNom = @JOUEURNOM";
                            command.Parameters.AddWithValue("@POINTS", item.Points);
                            command.Parameters.AddWithValue("@NBVICTOIRES", item.NbVictoires);
                            command.Parameters.AddWithValue("@NBDEFAITES", item.NbDefaites);
                            command.Parameters.AddWithValue("@NBTOURNOIS", item.NbTournois);
                            command.Parameters.AddWithValue("@NBTITRES", item.NbTitres);
                            command.Parameters.AddWithValue("@JOUEURNOM", item.Nom);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
        }

        public static void MAJClassementEquipe(List<EquipeBDD> listeEquipe)
        {
            using (SqlConnection connect = new SqlConnection(DataBaseRules.GetConnectionString))
            {
                try
                {
                    connect.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connect;
                        foreach (var item in listeEquipe)
                        {
                            command.Parameters.Clear();
                            string requete = "UPDATE Equipe SET Points = @POINTS, " +
                                                  "NbVictoires = @NBVICTOIRES, " +
                                                  "NbDefaites = @NBDEFAITES, " +
                                                  "NbTournois = @NBTOURNOIS, " +
                                                  "NbTitres = @NBTITRES " +
                                                  "WHERE Joueur1Nom = @JOUEUR1NOM AND Joueur2Nom = @JOUEUR2NOM " +
                                                  "IF @@ROWCOUNT=0" +
                                                  "INSERT INTO Equipe(Joueur1Nom, Joueur2Nom, Points, NbVictoires, NbDefaites, NbTournois, NbTitres)" +
                                                  "VALUES(@JOUEUR1NOM, @JOUEUR2NOM, @POINTS, @NBVICTOIRES, @NBDEFAITES, @NBTOURNOIS, @NBTITRES)";
                            command.CommandText = requete;
                            command.Parameters.AddWithValue("@POINTS", item.Points);
                            command.Parameters.AddWithValue("@NBVICTOIRES", item.NbVictoires);
                            command.Parameters.AddWithValue("@NBDEFAITES", item.NbDefaites);
                            command.Parameters.AddWithValue("@NBTOURNOIS", item.NbTournois);
                            command.Parameters.AddWithValue("@NBTITRES", item.NbTitres);
                            command.Parameters.AddWithValue("@JOUEUR1NOM", item.Joueur1Nom);
                            command.Parameters.AddWithValue("@JOUEUR2NOM", item.Joueur2Nom);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
        }
    }
}
