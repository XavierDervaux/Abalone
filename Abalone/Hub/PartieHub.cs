using System.Collections.Generic;
using Abalone.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System;
using Abalone.Models.Metier;

namespace Abalone.Hub
{
    public class PartieHub : Microsoft.AspNet.SignalR.Hub
    {
        private string connectionId;
        private static HashSet<bPartie> parties = new HashSet<bPartie>();

        public override Task OnConnected()
        {
            this.connectionId = Context.ConnectionId;
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            this.GestionAbandon(Context.ConnectionId, true);
            return base.OnDisconnected(stopCalled);
        }

        // Entrée
        //----------------------------------------------
        public void Add(int uid, int couleur)
        {
            this.GestionOuverture(Context.ConnectionId, uid, couleur);
        }

        public void Forfait()
        {
            this.GestionAbandon(Context.ConnectionId, false);
        }

        public void Move(string json)
        {
            bMove bean = JsonToBean(json);
        }

        private bMove JsonToBean(string json)
        {
            int x, y;
            JsonTextReader reader = new JsonTextReader(new StringReader(json));
            bMove bean = new bMove();

            for (int i = 0; i < 6; i++)
            {
                if (reader.Value != null)
                {
                    x = Int32.Parse(reader.Value.ToString());
                    reader.Read();
                    y = Int32.Parse(reader.Value.ToString());
                    if (i < 3)
                    {
                        bean.Origin[i] = new Bille(x, y);
                    }
                    else
                    {
                        bean.Origin[i - 3] = new Bille(x, y);
                    }
                    i++;
                }
            }
            return bean;
        }

        public void FinTour()
        {
            this.GestionFinTour(Context.ConnectionId);
        }

        // Sortie
        //----------------------------------------------

        private void sendReady(bPartie bean)
        {
            Clients.Client(bean.Session_blanc).pret();
            Clients.Client(bean.Session_noir).pret();
        }

        private void sendTimeOut(string session)
        {
            Clients.Client(session).timeout(); 
        }

        private void sendSurrend(string session)
        {
            Clients.Client(session).surrend();
        }

        private void sendBeginTurn(string session)
        {
            Clients.Client(session).beginTurn();
        }

        private void sendAllowed(string session, int sNoir, int sBlanc, bMoveResp bean)
        {
            string mov = JsonConvert.SerializeObject(bean);
            Clients.Client(session).allowed(sNoir, sBlanc, mov);
        }

        private void sendUnallowed(bPartie bean, string session)
        {
            Clients.Client(session).unallowed();
        }

        private void sendMoves(string session, int sNoir, int sBlanc, bMoveResp bean)
        {
            string mov = JsonConvert.SerializeObject(bean);
            Clients.Client(session).move(mov);
        }

        private void sendVictory(bPartie bean, int couleur)
        {
            Clients.Client(bean.Session_blanc).victory(couleur);
            Clients.Client(bean.Session_noir).victory(couleur);
        }

        // Méthodes publics
        //----------------------------------------------

        public void GestionOuverture(string session, int uid, int couleur)
        {
            bool partieExiste = false;

            bPartie partie = getPartieByUid(uid);
            if (partie != null)
            {
                partieExiste = true;
                if (couleur == 0)
                {
                    partie.Session_noir = session;
                }
                else
                {
                    partie.Session_blanc = session;
                }
                sendReady(partie);
            }
            if (!partieExiste)
            {
                bPartie bean = new bPartie();
                bean.Uid_partie = uid;

                if (couleur == 0)
                {
                    bean.Session_noir = session;
                }
                else
                {
                    bean.Session_blanc = session;
                }
                parties.Add(bean);
            }
        }

        public void GestionFinTour(string session)
        {
            int couleur = getCouleurBySession(session);
            bPartie bean = getPartieBySession(session);

            if (bean != null)
            { //La partie est toujours en cours
                Partie actuelle = Partie.TrouverPartie(bean.Uid_partie);

                if (actuelle.EstSonTour(couleur))
                { //Si ce n'est pas son tour ça ne posera pas de réel problème mais ça sera une nuisance graphique.
                    if (couleur == 0) { sendBeginTurn(bean.Session_blanc); }
                    else { sendBeginTurn(bean.Session_noir); }
                    actuelle.Tour = (actuelle.Tour * -1);
                    actuelle.PeutBouger = true;
                }
            }
        }

        public void GestionAbandon(string session, bool estTimeOut)
        {
            int couleur = getCouleurBySession(session);
            bPartie bean = getPartieBySession(session);

            if (bean != null)
            { //La partie est toujours en cours
                Partie actuelle = Partie.TrouverPartie(bean.Uid_partie);

                if (actuelle != null)
                {
                    if (couleur == 0)
                    { //C'est noir qui abandonne
                        actuelle.Fin(1, true);
                        if (estTimeOut) { sendTimeOut(bean.Session_blanc); }//On prévient blanc
                        else { sendSurrend(bean.Session_blanc); }
                    }
                    else
                    { //C'est blanc qui s'est déconnecté
                        actuelle.Fin(0, true);
                        if (estTimeOut) { sendTimeOut(bean.Session_noir); } //On prévient noir
                        else { sendSurrend(bean.Session_noir); }
                    }
                }
                parties.Remove(bean); //La partie est finie, pas de raison de la garder
            }
        }

        // Méthodes privées
        //----------------------------------------------
        private bPartie getPartieByUid(int uid)
        {
            bPartie res = null;
            foreach (bPartie bean in parties)
            {
                if (bean.Uid_partie == uid)
                {
                    res = bean;
                    break; //Inutile de parcourir le reste de la liste si on a trouvé ce qu'on cherchait
                }
            }
            return res;
        }

        private int getCouleurBySession(string session)
        {
            int res = 0;
            bPartie actuelle = getPartieBySession(session);

            if (actuelle.Session_blanc.Equals(session))
            {
                res = 1;
            }
            return res;
        }

        private bPartie getPartieBySession(string session)
        {
            bPartie res = null;
            foreach (bPartie bean in parties)
            {
                if (bean.Session_noir != null)
                {
                    if (bean.Session_noir.Equals(session))
                    {
                        res = bean;
                        break; //Inutile de parcourir le reste de la liste si on a trouvé ce qu'on cherchait
                    }
                }
                if (bean.Session_blanc != null)
                {
                    if (bean.Session_blanc.Equals(session))
                    {
                        res = bean;
                        break; //Inutile de parcourir le reste de la liste si on a trouvé ce qu'on cherchait
                    }
                }
            }
            return res;
        }
    }
}