using Abalone.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abalone.Controllers
{
    public class MatchmakingHub : Microsoft.AspNet.SignalR.Hub
    {
        private static int joueurId = 1;
        private static List<string> sessions = new List<string>();
        private static List<bJoueur> joueurs = new List<bJoueur>();

        public override Task OnConnected(){
            sessions.Add(Context.ConnectionId);

            foreach (bJoueur bean in joueurs) {
                sendFirstAdd(bean, Context.ConnectionId);
            } //Envoie tous les joueurs déjà connecté pour que l'utilisateur soit synchronisé avec le serveur
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled){
            bJoueur bean = getJoueurBySession(Context.ConnectionId);
    	    sessions.Remove(Context.ConnectionId); //On le vire de la liste des sessions, on ne lui enverra plus les messages
            
    	    if(bean != null){
			    joueurs.Remove(bean);
			    sendRemove(bean);
            }
            return base.OnDisconnected(stopCalled);
        }        

    // Entrée
    //----------------------------------------------
        public void Add(string pseudo, string mail){
            bool dejaConnect = false;
            bJoueur bean = new bJoueur();
            bean.Session       = Context.ConnectionId;
            bean.Joueur_pseudo = pseudo;
            bean.Joueur_email  = mail;
		
		    foreach(bJoueur tmp in joueurs){
			    if( bean.Joueur_email.Equals(tmp.Joueur_email) ){ //le mail est unique, si on en trouve un identique c'est quec'est un doublon -> LE joueur est déjà connecté.
				    dejaConnect = true; break; //Inutile de continuer a parcourir si on a déjà trouvé ce qu'on cherchait.
			    }
		    }

		    if(dejaConnect){   sendAlreadyConnected(bean, bean.Session);   }
		    else {
			    bean.Id = joueurId; //On définit l'id dont on se sert pour l'identification
	            joueurs.Add(bean);
	            joueurId++;
	            sendAdd(bean); //On envoie a tout le monde qu'un nouveau joueur est connecté.
		    }
        }

        public void Demande(int destinataire){
            bJoueur source = getJoueurBySession(Context.ConnectionId);
            bJoueur destin = getJoueurById(destinataire);

            sendDemand(source, destin.Session);
        }

        public void Reponse(int destinataire, bool confirm){
            bJoueur source = getJoueurBySession(Context.ConnectionId);
            bJoueur destin = getJoueurById(destinataire);

            sendConfirmation(source, confirm, destin.Session);
        }

    // Sortie
    //----------------------------------------------
        private void sendFirstAdd(bJoueur bean, string session) {
            Clients.Client(session).receiveAdd(bean.Id, bean.Joueur_pseudo, bean.Joueur_email); //On envoie tous les joueurs a la personne qui vient de se co, pour qu'elle aie la liste à jour.
        }
    
	    private void sendAdd(bJoueur bean) {
            Clients.All.receiveAdd(bean.Id, bean.Joueur_pseudo, bean.Joueur_email);
        }
    
	    private void sendRemove(bJoueur bean) {
            Clients.All.receiveRemove(bean.Id);
        }
    
	    private void sendDemand(bJoueur bean, string session) {
            Clients.Client(session).receiveDemande(bean.Id, bean.Joueur_pseudo, bean.Joueur_email); //On envoie tous les joueurs a la personne qui vient de se co, pour qu'elle aie la liste à jour.
        }

        private void sendConfirmation(bJoueur bean, bool confirm, string session) {
            Clients.Client(session).receiveConfirmation(bean.Id, bean.Joueur_pseudo, bean.Joueur_email, confirm); //On envoie tous les joueurs a la personne qui vient de se co, pour qu'elle aie la liste à jour.
        }

        private void sendAlreadyConnected(bJoueur bean, string session) {
            Clients.Client(session).receiveAlreadyConnected(bean.Joueur_pseudo); //On envoie tous les joueurs a la personne qui vient de se co, pour qu'elle aie la liste à jour.
        }

    // Méthodes privées
    //----------------------------------------------	
        private bJoueur getJoueurById(int id) {
    	    bJoueur res = null;
            foreach(bJoueur bean in joueurs) {
                if (bean.Id == id ){
                    res = bean;
                    break; //Inutile de parcourir le reste de la liste si on a trouvé ce qu'on cherchait
                }
            }
            return res;
        }
    
        private bJoueur getJoueurBySession(string session) {
    	    bJoueur res = null;
            foreach (bJoueur bean in joueurs) {
                if (bean.Session.Equals(session) ){
                    res = bean;
                    break; //Inutile de parcourir le reste de la liste si on a trouvé ce qu'on cherchait
                }
            }
            return res;
        }
    }
}