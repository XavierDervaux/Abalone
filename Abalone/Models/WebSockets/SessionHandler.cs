namespace Abalone.Models.WebSockets
{
    public class SessionHandler
    {
        private static SessionHandler instance;
        public JoueurHandler Session { get; set; }

        private SessionHandler() { }

        public static SessionHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SessionHandler();
                }
                return instance;
            }
        }
    }
}