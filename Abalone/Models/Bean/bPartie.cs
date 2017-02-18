namespace Abalone.Models{
    public class bPartie
    {
        private int uid_partie = 0;
        private string session_noir = null;
        private string session_blanc = null;

        public int Uid_partie { get { return this.uid_partie; } set { this.uid_partie = value; } }
        public string Session_noir { get; set; }
        public string Session_blanc { get; set; }
    }
}