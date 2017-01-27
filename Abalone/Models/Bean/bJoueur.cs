namespace Abalone.Models{
    public class bJoueur{
        //private Session sess;
        private int id; //L'id de la session du socket, aucun rapport avec l'id de BDD
        private string joueur_pseudo;
        private string joueur_email;

        //public Session Sess { get; set; }
        public int Id { get; set; }
        public string Joueur_pseudo { get; set; }
        public string Joueur_email { get; set; }
    }
}