namespace Abalone.Models{
    public class bJoueur{
        //private Session sess;
        private string session; //L'id de la session du socket, aucun rapport avec l'id de BDD
        private int id; //L'id de la session du socket, aucun rapport avec l'id de BDD
        private string joueur_pseudo;
        private string joueur_email;

        //public Session Sess { get; set; }
        public int Id               { get { return this.id; }            set { this.id            = value; } }
        public string Session       { get { return this.session; }       set { this.session       = value; } }
        public string Joueur_pseudo { get { return this.joueur_pseudo; } set { this.joueur_pseudo = value; } }
        public string Joueur_email  { get { return this.joueur_email; }  set { this.joueur_email  = value; } }
    }
}