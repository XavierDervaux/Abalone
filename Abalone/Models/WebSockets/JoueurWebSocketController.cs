using Abalone.Models.WebSockets;
using Microsoft.Web.WebSockets;
using Newtonsoft.Json;
using System; 
using System.IO;
using System.Net.Sockets;

namespace Abalone.Models
{
    public class JoueurWebSocketController : WebSocketHandler
    {
        private static WebSocketCollection SESSIONS = new WebSocketCollection();

        public override void OnOpen()
        {
            SESSIONS.Add(this);
        }

        public override void OnError()
        {

        }

        public override void OnMessage(string message)
        {
            try
            {
                string action;
                JsonTextReader reader = new JsonTextReader(new StringReader(message));
                reader.Read();
                action = reader.Value.ToString();

                SessionHandler sessionHandler = SessionHandler.Instance;

                if ("add".Equals(action))
                {
                    bJoueur bean = new bJoueur();
                   // bean.Session = session;
                    reader.Read();
                    bean.Joueur_pseudo = reader.Value.ToString();
                    reader.Read();
                    bean.Joueur_email = reader.Value.ToString();
                    sessionHandler.Session.GestionDoublon(bean);
                }

                if ("demande".Equals(action))
                {
                   /* int destId = (int)jsonMessage.getInt("destinataire");
                     sessionHandler.Session.GestionDemande(destId, session);*/
                }

                if ("reponse".Equals(action))
                {
                    /*int destId = (int)jsonMessage.getInt("destinataire");
                    bool confirm = (bool)jsonMessage.getBoolean("confirm");
                    sessionHandler.Session.GestionConfirmation(destId, confirm, session);*/
                }
            }
            catch (Exception)
            {

            }
        }

        public override void OnClose()
        {
            SESSIONS.Remove(this);
        }
    }
}