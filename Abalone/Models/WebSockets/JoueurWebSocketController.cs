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
        public void OnOpen(TcpClient session)
        {
            SessionHandler sessionHandler = SessionHandler.Instance;
            sessionHandler.Session.AddSession(session);
        }

        public override void OnError()
        {

        }

        public void OnMessage(string message, TcpClient session)
        {
            try
            {
                /*JsonReader reader = Json.createReader(new StringReader(message));
                JsonObject jsonMessage = reader.readObject();

                SessionHandler sessionHandler = SessionHandler.Instance;

                if ("add".Equals(jsonMessage.getString("action")))
                {
                    bJoueur bean = new bJoueur();
                    bean.Session = session;
                    bean.Joueur_pseudo = jsonMessage.getString("pseudo");
                    bean.Joueur_email = jsonMessage.getString("email");
                    sessionHandler.Session.GestionDoublon(bean);
                }

                if ("demande".Equals(jsonMessage.getString("action")))
                {
                    int destId = (int)jsonMessage.getInt("destinataire");
                     sessionHandler.Session.GestionDemande(destId, session);
                }

                if ("reponse".Equals(jsonMessage.getString("action")))
                {
                    int destId = (int)jsonMessage.getInt("destinataire");
                    bool confirm = (bool)jsonMessage.getBoolean("confirm");
                    sessionHandler.Session.GestionConfirmation(destId, confirm, session);
                }*/
            }
            catch (Exception)
            {

            }
        }

        public void OnClose(TcpClient session)
        {
            SessionHandler sessionHandler = SessionHandler.Instance;
            sessionHandler.Session.RemoveSession(session);
        }
    }
}