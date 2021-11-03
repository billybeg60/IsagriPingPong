using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Exchange.WebServices.Data;

namespace IsagriPingPong
{
    public class ResultatHelper
    {
        public const string chemin = @"\\JBEGARD18-DE\IsaPingPong\";
        public const string adresse = "jbegard@isagri.fr";

        /// <summary>
        /// Renvoi la chaine de connexion à la BDD
        /// </summary>
        public static string GetConnectionString
        {
            get { return "Data Source=GCBUILD13\\IP08R2;Initial Catalog=PhoneTimer;User ID=UserGc;Password=UserGc;Connect Timeout=10"; }
        }


        public static void AfficherResultat(Dictionary<string, int> dicoResultat, bool solo, bool championnat)
        {
            var dicoTrie = dicoResultat.OrderByDescending(x => x.Value);

            // Fichier Html préformater (juste à mettre le nom des photos)
            string html = File.ReadAllText(chemin + "base.html");

            // En équipe, on mets une photo de chaque membres de l'équipe
            if (!solo)
            {
                html = RemplacerNom("first1", html, dicoTrie.ElementAtOrDefault(0).Key);
                html = RemplacerNom("first2", html, dicoTrie.ElementAtOrDefault(1).Key);
                html = RemplacerNom("deuxieme1", html, dicoTrie.ElementAtOrDefault(2).Key);
                html = RemplacerNom("deuxieme2", html, dicoTrie.ElementAtOrDefault(3).Key);
                html = RemplacerNom("troisieme1", html, dicoTrie.ElementAtOrDefault(4).Key);
                html = RemplacerNom("troisieme2", html, dicoTrie.ElementAtOrDefault(5).Key);

                // dernier de la liste, dernier du classement
                if (championnat && dicoTrie.ElementAtOrDefault(dicoTrie.Count() - 1).Value == 0)
                {
                    html = RemplacerNom("dernier1", html, dicoTrie.ElementAtOrDefault(dicoTrie.Count() - 2).Key);
                    html = RemplacerNom("dernier2", html, dicoTrie.ElementAtOrDefault(dicoTrie.Count() - 1).Key);
                }
            }
            else
            {
                // En solo, on mets deux fois la photo de la même personne
                html = RemplacerNom("first1", html, dicoTrie.ElementAtOrDefault(0).Key);
                html = RemplacerNom("first2", html, dicoTrie.ElementAtOrDefault(0).Key);
                html = RemplacerNom("deuxieme1", html, dicoTrie.ElementAtOrDefault(1).Key);
                html = RemplacerNom("deuxieme2", html, dicoTrie.ElementAtOrDefault(1).Key);
                html = RemplacerNom("troisieme1", html, dicoTrie.ElementAtOrDefault(2).Key);
                html = RemplacerNom("troisieme2", html, dicoTrie.ElementAtOrDefault(2).Key);

                // dernier de la liste, dernier du classement
                if (championnat && dicoTrie.ElementAtOrDefault(dicoTrie.Count() - 1).Value == 0)
                {
                    html = RemplacerNom("dernier1", html, dicoTrie.ElementAtOrDefault(dicoResultat.Count() - 1).Key);
                    html = RemplacerNom("dernier2", html, dicoTrie.ElementAtOrDefault(dicoResultat.Count() - 1).Key);
                }
            }
            int nbBonus = 0;

            // Affichage bonus
            foreach (var item in dicoTrie)
            {
                if (item.Value != 0)
                    nbBonus++;

            }
            if (!solo)
                nbBonus = nbBonus / 2;

            if (nbBonus == 1)
                html = html.Replace("BonusA", "Bonus10");
            if (nbBonus == 2)
            {
                html = html.Replace("BonusA", "Bonus20");
                html = html.Replace("BonusB", "Bonus10");
            }
            if (nbBonus == 3)
            {
                html = html.Replace("BonusA", "Bonus30");
                html = html.Replace("BonusB", "Bonus20");
                html = html.Replace("BonusC", "Bonus10");
            }

            // On crée un fichier html qui est la copie du fichier de base mais avec les photos, on formate avec la date du jour
            string time = DateTime.Now.ToShortTimeString().Replace(@":", "_");
            string date = DateTime.Now.ToShortDateString().Replace(@"/", "_");
            string fichier = chemin + @"Historique\Classement du " + date + "_" + time + ".html";
            File.WriteAllText(fichier, html);
            // On ouvre le fichier (navigateur web)
            Process.Start(fichier);

        }
        /// <summary>
        /// Méthode pour remplacer les nom de bases (ex: first, deuxieme... etc) du fichier pré formaté par les noms des personnes
        /// </summary>
        /// <param name="champs">nom de base</param>
        /// <param name="html">contenu html à modifier</param>
        /// <param name="i">Place de classement</param>
        /// <param name="liste">Liste des personnes du classement</param>
        /// <returns></returns>
        public static string RemplacerNom(string champs, string html, string mail)
        {
            if (!string.IsNullOrEmpty(mail))
            {
                return html.Replace(champs, mail.Replace("@isagri.fr", ""));
            }
            return html.Replace(champs, "Vide.JPG");
        }

        /// <summary>
        /// Envoyer un mail
        /// </summary>
        /// <param name="messageAEnvoyer">Objet</param>
        /// <param name="corpMessage">Corps du message</param>
        /// <param name="listeDestinataire">Liste des destinataires</param>
        public static void EnvoyerMail(string messageAEnvoyer, string corpMessage, List<string> listeDestinataire, string exped)
        {
            ExchangeService exchange = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            exchange.Credentials = new WebCredentials(@"jbegard@groupeisagri.com", LireMdp(), "groupe");
            exchange.Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx");
            EmailMessage message = new EmailMessage(exchange);
            message.From = new EmailAddress();
            message.From = exped;

            message.ToRecipients.Add(listeDestinataire[0]);

            corpMessage = corpMessage.Replace("\r\n", "<BR/>");
            AjouterAdresseMail(message, listeDestinataire);
            if (string.IsNullOrEmpty(corpMessage))
                message.Body = messageAEnvoyer;
            else
                message.Body = corpMessage;

            message.Subject = messageAEnvoyer;

            message.Send();
        }
        /// <summary>
        /// Ajouter des destinataire au mail
        /// </summary>
        /// <param name="msg">Mail</param>
        /// <param name="listeDestinataire">liste des destinataire (si rien, on envoi a tout le bureau)</param>
        private static void AjouterAdresseMail(EmailMessage msg, List<string> listeDestinataire)
        {

            foreach (string item in listeDestinataire)
            {
                msg.ToRecipients.Add(item);
            }
        }

        /// <summary>
        /// Récupérer le mots de passe de mon compte (je le mets à jour systématiquement)
        /// </summary>
        /// <returns>Mdp</returns>
        public static string LireMdp()
        {
            string MotPAsse = string.Empty;
            using (SqlConnection connect = new SqlConnection(GetConnectionString))
            {
                try
                {
                    connect.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        StringBuilder requete = new StringBuilder();
                        requete.Append("select RemarquesGeneration from GenerationSetup where NumVersionGC= '12.63.000'");
                        command.Connection = connect;
                        command.CommandTimeout = 5;
                        command.CommandText = requete.ToString();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MotPAsse = reader.GetString(0);
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
            return MotPAsse;
        }

        public static void CaptureScreen(UIElement source)
        {
            string time = DateTime.Now.ToShortTimeString().Replace(@":", "_"); ;
            string date = DateTime.Now.ToShortDateString().Replace(@"/", "_");
            string fichier = chemin + @"Historique\Classement du " + date + "_" + time + ".jpeg";
            double Height, renderHeight, Width, renderWidth;

            Height = renderHeight = source.RenderSize.Height;
            Width = renderWidth = source.RenderSize.Width;

            //Specification for target bitmap like width/height pixel etc.
            RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)renderWidth, (int)renderHeight, 96, 96, PixelFormats.Pbgra32);
            //creates Visual Brush of UIElement
            VisualBrush visualBrush = new VisualBrush(source);

            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                //draws image of element
                drawingContext.DrawRectangle(visualBrush, null, new Rect(new Point(0, 0), new Point(Width, Height)));
            }
            //renders image
            renderTarget.Render(drawingVisual);

            //PNG encoder for creating PNG file
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTarget));
            using (FileStream stream = new FileStream(fichier, FileMode.Create, FileAccess.Write))
            {
                encoder.Save(stream);
            }

        }
    }
}
