namespace IsagriPingPong
{
    public class Rencontre
    {
        public int Id { get; set; }
        public int Journee { get; set; }
        public Participant Equipe1 { get; set; }
        public Participant Equipe2 { get; set; }
        public int PointEquipe1 { get; set; }
        public int PointEquipe2 { get; set; }
        public bool Valider { get; set; }
        public string Tour { get; set; }
        public string Raquette { get; set; }
    }
}
