using System.Collections.Generic;
using Abalone.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Abalone.Models.Metier;
using Abalone.Models.Bean;

namespace Abalone.Hub
{
    public class PartieHub : Microsoft.AspNet.SignalR.Hub
    {

        private static readonly HashSet<bPartie> Parties = new HashSet<bPartie>();

        public override Task OnDisconnected(bool stopCalled)
        {
            GestionAbandon(Context.ConnectionId, true);
            return base.OnDisconnected(stopCalled);
        }

        // Entrée
        //----------------------------------------------
        public void Add(int uid, int couleur) => GestionOuverture(Context.ConnectionId, uid, couleur);
        public void Forfait() => GestionAbandon(Context.ConnectionId, false);

        public void Move(string json)
        {
            var moves = JsonToBean(json);
            int res, couleur = GetCouleurBySession(Context.ConnectionId);
            var bean = GetPartieBySession(Context.ConnectionId);
            var reponse = new BMoveResp();
            Bille.VecteurInit(reponse.Origin);
            Bille.VecteurInit(reponse.Destination);

            if (bean != null)
            { //La partie est toujours en cours
                Partie actuelle = Partie.TrouverPartie(bean.Uid_partie);

                if (actuelle.EstSonTour(couleur) && actuelle.PeutBouger)
                {
                    res = actuelle.GestionMouvement(couleur, moves, reponse);
                    switch (res)
                    {
                        case -1: SendUnallowed(Context.ConnectionId); break;
                        case 0: SendVictory(bean, 0); break; //noir
                        case 1: SendVictory(bean, 1); break; //blanc
                        case 2:
                            SendAllowed(Context.ConnectionId, actuelle.ScoreNoir, actuelle.ScoreBlanc, reponse);
                            if (bean.Session_blanc.Equals(Context.ConnectionId))
                                SendMoves(bean.Session_noir, actuelle.ScoreNoir, actuelle.ScoreBlanc, reponse);
                            else 
                                SendMoves(bean.Session_blanc, actuelle.ScoreNoir, actuelle.ScoreBlanc, reponse);
                            break;
                    }
                } //Sinon on l'ignore simplement
            }
        }

        private bMove JsonToBean(string json)
        {
            dynamic reader = JsonConvert.DeserializeObject(json);
            bMove bean = new bMove();

            for (int i = 0; i < reader.Count; i++)
            {
                dynamic item = reader[i];
                if (i < 3)
                    bean.Origin[i] = new Bille((int)item.x, (int)item.y);
                else
                    bean.Destination[i - 3] = new Bille((int)item.x, (int)item.y);
            }

            return bean;
        }

        public void FinTour() => GestionFinTour(Context.ConnectionId);

        // Sortie
        //----------------------------------------------

        private void SendReady(bPartie bean)
        {
            Clients.Client(bean.Session_blanc).pret();
            Clients.Client(bean.Session_noir).pret();
        }

        private void SendTimeOut(string session) => Clients.Client(session).timeout(); 
        private void SendSurrend(string session) => Clients.Client(session).surrend();
        private void SendBeginTurn(string session) => Clients.Client(session).beginTurn();
        private void SendUnallowed(string session) => Clients.Client(session).unallowed();

        private void SendAllowed(string session, int sNoir, int sBlanc, BMoveResp bean)
        {
            string mov = JsonConvert.SerializeObject(bean);
            Clients.Client(session).allowed(sNoir, sBlanc, mov);
        }

        private void SendMoves(string session, int sNoir, int sBlanc, BMoveResp bean)
        {
            var mov = JsonConvert.SerializeObject(bean);
            Clients.Client(session).move(sNoir, sBlanc, mov);
        }

        private void SendVictory(bPartie bean, int couleur)
        {
            Clients.Client(bean.Session_blanc).victory(couleur);
            Clients.Client(bean.Session_noir).victory(couleur);
        }

        // Méthodes publics
        //----------------------------------------------

        public void GestionOuverture(string session, int uid, int couleur)
        {
            bool partieExiste = false;

            bPartie partie = GetPartieByUid(uid);
            if (partie != null)
            {
                partieExiste = true;
                if (couleur == 0)
                    partie.Session_noir = session;
                else
                    partie.Session_blanc = session;

                SendReady(partie);
            }
            if (!partieExiste)
            {
                bPartie bean = new bPartie();
                bean.Uid_partie = uid;

                if (couleur == 0)
                    bean.Session_noir = session;
                else
                    bean.Session_blanc = session;

                Parties.Add(bean);
            }
        }

        public void GestionFinTour(string session)
        {
            var couleur = GetCouleurBySession(session);
            var bean = GetPartieBySession(session);

            if (bean != null)
            { //La partie est toujours en cours
                Partie actuelle = Partie.TrouverPartie(bean.Uid_partie);

                if (actuelle.EstSonTour(couleur))
                { //Si ce n'est pas son tour ça ne posera pas de réel problème mais ça sera une nuisance graphique.
                    if (couleur == 0)
                        SendBeginTurn(bean.Session_blanc);
                    else
                        SendBeginTurn(bean.Session_noir);

                    actuelle.Tour = (actuelle.Tour * -1);
                    actuelle.PeutBouger = true;
                }
            }
        }

        public void GestionAbandon(string session, bool estTimeOut)
        {
            var couleur = GetCouleurBySession(session);
            var bean = GetPartieBySession(session);

            if (bean != null)
            { //La partie est toujours en cours
                Partie actuelle = Partie.TrouverPartie(bean.Uid_partie);

                if (actuelle != null)
                {
                    if (couleur == 0)
                    { //C'est noir qui abandonne
                        actuelle.Fin(1, true);
                        if (estTimeOut) { SendTimeOut(bean.Session_blanc); }//On prévient blanc
                        else { SendSurrend(bean.Session_blanc); }
                    }
                    else
                    { //C'est blanc qui s'est déconnecté
                        actuelle.Fin(0, true);
                        if (estTimeOut) { SendTimeOut(bean.Session_noir); } //On prévient noir
                        else { SendSurrend(bean.Session_noir); }
                    }
                }
                Parties.Remove(bean); //La partie est finie, pas de raison de la garder
            }
        }

        // Méthodes privées
        //----------------------------------------------
        private bPartie GetPartieByUid(int uid)
        {
            bPartie res = null;
            foreach (var bean in Parties)
            {
                if (bean.Uid_partie == uid)
                {
                    res = bean;
                    break; //Inutile de parcourir le reste de la liste si on a trouvé ce qu'on cherchait
                }
            }
            return res;
        }

        private int GetCouleurBySession(string session)
        {
            var res = 0;
            var actuelle = GetPartieBySession(session);

            if (actuelle.Session_blanc.Equals(session))
            {
                res = 1;
            }
            return res;
        }

        private bPartie GetPartieBySession(string session)
        {
            bPartie res = null;
            foreach (var bean in Parties)
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