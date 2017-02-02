using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Web;
using System.Web.WebSockets;
using System.Text;
using Newtonsoft.Json;

namespace Abalone.Models
{
    public class JoueurHandler : IHttpHandler
    {
        private int joueurId = 1;
        private HashSet<bJoueur> joueurs = new HashSet<bJoueur>();
        private HashSet<TcpClient> sessions = new HashSet<TcpClient>();

        public void ProcessRequest(HttpContext context)
        {

        }

        public void AddSession(TcpClient session)
        {
            //On ajoute l'objet a notre session et on le renvoie au client pour qu'il puisse l'afficher a son tour
            this.sessions.Add(session);

            foreach (bJoueur bean in this.joueurs)
            {
                this.SendFirstAdd(bean, session);
            }
        }

        public void RemoveSession(TcpClient session)
        {
            bJoueur bean = GetJoueurBySession(session);

            sessions.Remove(session); //On le vire de la liste des sessions, on ne lui enverra plus les messages
            if (bean != null)
            {
                this.joueurs.Remove(bean);
                SendRemove(bean);
            }
        }


        public bool IsReusable { get { return false; } }

        public void GestionDoublon(bJoueur bean)
        {
            bool dejaConnect = false;

            foreach (bJoueur tmp in  joueurs)
            {
                if (bean.Joueur_email.Equals(tmp.Joueur_email))
                { //le mail est unique, si on en trouve un identique c'est quec'est un doublon -> LE joueur est déjà connecté.
                    dejaConnect = true; break; //Inutile de continuer a parcourir si on a déjà trouvé ce qu'on cherchait.
                }
            }
            if (dejaConnect)
            {
                this.SendAlreadyConnected(bean, bean.Session);
            }
            else
            {
                bean.Id = this.joueurId; //On définit l'id dont on se sert pour l'identification
                this.joueurs.Add(bean);
                this.joueurId++;
                this.SendAdd(bean);
            }
        }

        public void GestionDemande(int destId, TcpClient session)
        {
            bJoueur source = GetJoueurBySession(session);
            bJoueur destin = getJoueurById(destId);

            this.SendDemand(source, destin.Session);
        }

        public void GestionConfirmation(int destId, bool confirm, TcpClient session)
        {
            bJoueur source = GetJoueurBySession(session);
            bJoueur destin = getJoueurById(destId);

            this.SendConfirmation(source, confirm, destin.Session);
        }

        private void SendFirstAdd(bJoueur bean, TcpClient session)
        {
            StringWriter sw = new StringWriter(new StringBuilder());
            using (JsonWriter message = new JsonTextWriter(sw))
            {
                message.Formatting = Formatting.Indented;
                message.WriteStartObject();

                message.WritePropertyName("action");
                message.WriteValue("add");
                message.WritePropertyName("id");
                message.WriteValue(bean.Id);
                message.WritePropertyName("pseudo");
                message.WriteValue(bean.Joueur_pseudo);
                message.WritePropertyName("email");
                message.WriteValue(bean.Joueur_email);

                message.WriteEnd();
                message.WriteEndObject();

                this.SendToSession(session, message);
            }
        }

        private void SendAdd(bJoueur bean)
        {
            StringWriter sw = new StringWriter(new StringBuilder());
            using (JsonWriter message = new JsonTextWriter(sw))
            {
                message.Formatting = Formatting.Indented;
                message.WriteStartObject();

                message.WritePropertyName("action");
                message.WriteValue("add");
                message.WritePropertyName("id");
                message.WriteValue(bean.Id);
                message.WritePropertyName("pseudo");
                message.WriteValue(bean.Joueur_pseudo);
                message.WritePropertyName("email");
                message.WriteValue(bean.Joueur_email);

                message.WriteEnd();
                message.WriteEndObject();

                this.SendToAllConnectedSessions(message);
            }
        }

        private void SendRemove(bJoueur bean)
        {
            StringWriter sw = new StringWriter(new StringBuilder());
            using (JsonWriter message = new JsonTextWriter(sw))
            {
                message.Formatting = Formatting.Indented;
                message.WriteStartObject();

                message.WritePropertyName("action");
                message.WriteValue("remove");
                message.WritePropertyName("id");
                message.WriteValue(bean.Id);

                message.WriteEnd();
                message.WriteEndObject();

                this.SendToAllConnectedSessions(message);
            }
        }

        private void SendDemand(bJoueur bean, TcpClient session)
        {
            StringWriter sw = new StringWriter(new StringBuilder());
            using (JsonWriter message = new JsonTextWriter(sw))
            {
                message.Formatting = Formatting.Indented;
                message.WriteStartObject();

                message.WritePropertyName("action");
                message.WriteValue("demande");
                message.WritePropertyName("id_source");
                message.WriteValue(bean.Id);
                message.WritePropertyName("pseudo_source");
                message.WriteValue(bean.Joueur_pseudo);
                message.WritePropertyName("email_source");
                message.WriteValue(bean.Joueur_email);

                message.WriteEnd();
                message.WriteEndObject();

                this.SendToSession(session, message);
            }
        }

        private void SendConfirmation(bJoueur bean, bool confirm, TcpClient session)
        {
            StringWriter sw = new StringWriter(new StringBuilder());
            using (JsonWriter message = new JsonTextWriter(sw))
            {
                message.Formatting = Formatting.Indented;
                message.WriteStartObject();

                message.WritePropertyName("action");
                message.WriteValue("reponse");
                message.WritePropertyName("source");
                message.WriteValue(bean.Id);
                message.WritePropertyName("pseudo_source");
                message.WriteValue(bean.Joueur_pseudo);
                message.WritePropertyName("email_source");
                message.WriteValue(bean.Joueur_email);
                message.WritePropertyName("confirm");
                message.WriteValue(confirm);

                message.WriteEnd();
                message.WriteEndObject();

                this.SendToSession(session, message);
            }            
        }

        private void SendToAllConnectedSessions(JsonWriter message)
        {
            foreach (TcpClient session in this.sessions)
            {
                this.SendToSession(session, message);
            }
        }

        private void SendAlreadyConnected(bJoueur bean, TcpClient session)
        {
            StringWriter sw = new StringWriter(new StringBuilder());
            using (JsonWriter message = new JsonTextWriter(sw))
            {
                message.Formatting = Formatting.Indented;
                message.WriteStartObject();

                message.WritePropertyName("action");
                message.WriteValue("dejaConnect");
                message.WritePropertyName("pseudo");
                message.WriteValue(bean.Joueur_pseudo);

                message.WriteEnd();
                message.WriteEndObject();

                this.SendToSession(session, message);
            }
        }

        private bJoueur GetJoueurById(int id)
        {
            bJoueur res = null;
            foreach (bJoueur bean in this.joueurs)
            {
                if (bean.Id == id)
                {
                    res = bean;
                    break; //Inutile de parcourir le reste de la liste si on a trouvé ce qu'on cherchait
                }
            }
            return res;
        }

        private bJoueur getJoueurById(int id)
        {
            bJoueur res = null;
            foreach (bJoueur bean in this.joueurs)
            {
                if (bean.Id == id)
                {
                    res = bean;
                    break; //Inutile de parcourir le reste de la liste si on a trouvé ce qu'on cherchait
                }
            }
            return res;
        }

        private bJoueur GetJoueurBySession(TcpClient session)
        {
            bJoueur res = null;
            foreach (bJoueur bean in this.joueurs)
            {
                if (bean.Session.Equals(session))
                {
                    res = bean;
                    break; //Inutile de parcourir le reste de la liste si on a trouvé ce qu'on cherchait
                }
            }
            return res;
        }

        private void SendToSession(TcpClient session, JsonWriter message)
        {
            try
            {
                var stream = session.GetStream();
                stream.Write(Utilitaire.GetBytes(message.ToString()), 0, message.ToString().Length);
            }
            catch (IOException)
            {
                this.sessions.Remove(session);
                //Logger.getLogger(JoueurHandler.class.getName()).log(Level.SEVERE, null, ex);
            }
        }
    }
}