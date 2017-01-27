/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Abalone.Models{
    public class JoueurHandler {
        private int joueurId = 1;
        private final Set<Session> sessions = new HashSet<>();
        private final Set<bJoueur> joueurs = new HashSet<>();


    // Méthodes publiques
    //---------------------------------------------------
        //---------------------------------------------------
        //Gestion des sessions
        public void addSession(Session session) { //On ajoute l'objet a notre session et on le renvoie au client pour qu'il puisse l'afficher a son tour
    	    this.sessions.add(session);
            for (bJoueur bean : this.joueurs) {
                sendFirstAdd(bean, session);
            }
        }
    
        public void removeSession(Session session) {
    	    bJoueur bean = getJoueurBySession(session);
    	
    	    sessions.remove(session); //On le vire de la liste des sessions, on ne lui enverra plus les messages
    	    if(bean != null){
			    this.joueurs.remove(bean);
			    sendRemove(bean);
            }
        }

        //---------------------------------------------------
        //Gestion d'envois
	    public void gestionDoublon(bJoueur bean) {
		    boolean dejaConnect = false;
		
		    for(bJoueur tmp :  joueurs){
			    if( bean.getJoueur_email().equals( tmp.getJoueur_email() ) ){ //le mail est unique, si on en trouve un identique c'est quec'est un doublon -> LE joueur est déjà connecté.
				    dejaConnect = true; break; //Inutile de continuer a parcourir si on a déjà trouvé ce qu'on cherchait.
			    }
		    }
		    if(dejaConnect){   sendAlreadyConnected(bean, bean.getSession());   }
		    else {
			    bean.setId(this.joueurId); //On définit l'id dont on se sert pour l'identification
	            this.joueurs.add(bean);
	            this.joueurId++;
	            sendAdd(bean);
		    }
	    }

	    public void gestionDemande(int destId, Session session) {
    	    bJoueur source = getJoueurBySession(session);
    	    bJoueur destin = getJoueurById(destId);
    	
		    sendDemand(source, destin.getSession());
	    }  

	    public void gestionConfirmation(int destId, boolean confirm, Session session) {
    	    bJoueur source = getJoueurBySession(session);
    	    bJoueur destin = getJoueurById(destId);
    	
    	    sendConfirmation(source, confirm, destin.getSession());
	    } 


    // Méthodes privées
    //---------------------------------------------------	
	    private void sendFirstAdd(bJoueur bean, Session session) {
		    JsonProvider provider = JsonProvider.provider();
            JsonObject message = provider.createObjectBuilder()
	                .add("action", "add")
	                .add("id", bean.getId())
	                .add("pseudo", bean.getJoueur_pseudo())
	                .add("email", bean.getJoueur_email())
	                .build();
	        sendToSession(session, message); //On envoie tous les joueurs a la personne qui vient de se co, pour qu'elle aie la liste à jour.
        }
    
	    private void sendAdd(bJoueur bean) {
		    JsonProvider provider = JsonProvider.provider();
            JsonObject message = provider.createObjectBuilder()
                    .add("action", "add")
                    .add("id", bean.getId())
                    .add("pseudo", bean.getJoueur_pseudo())
                    .add("email", bean.getJoueur_email())
                    .build();
            sendToAllConnectedSessions(message);
        }
    
	    private void sendRemove(bJoueur bean) {
		    JsonProvider provider = JsonProvider.provider();
            JsonObject message = provider.createObjectBuilder()
                    .add("action", "remove")
                    .add("id", bean.getId())
                    .build();
            sendToAllConnectedSessions(message);
        }
    
	    private void sendDemand(bJoueur bean, Session session) {
		    JsonProvider provider = JsonProvider.provider();
            JsonObject message = provider.createObjectBuilder()
                    .add("action", "demande")
                    .add("id_source", bean.getId())
                    .add("pseudo_source", bean.getJoueur_pseudo())
                    .add("email_source", bean.getJoueur_email())
                    .build();
            sendToSession(session, message);
	    }  

	    private void sendConfirmation(bJoueur bean, boolean confirm, Session session) {
		    JsonProvider provider = JsonProvider.provider();
            JsonObject reponse = provider.createObjectBuilder()
                    .add("action", "reponse")
                    .add("source", bean.getId())
                    .add("pseudo_source", bean.getJoueur_pseudo())
                    .add("email_source", bean.getJoueur_email())
                    .add("confirm", confirm)
                    .build();
            sendToSession(session, reponse);
	    }
	
	    private void sendAlreadyConnected(bJoueur bean, Session session) {
		    JsonProvider provider = JsonProvider.provider();
            JsonObject reponse = provider.createObjectBuilder()
                    .add("action", "dejaConnect")
                    .add("pseudo", bean.getJoueur_pseudo())
                    .build();
            sendToSession(session, reponse);
	    }
	
        private bJoueur getJoueurById(int id) {
    	    bJoueur res = null;
            for (bJoueur bean : this.joueurs) {
                if (bean.getId() == id ){
                    res = bean;
                    break; //Inutile de parcourir le reste de la liste si on a trouvé ce qu'on cherchait
                }
            }
            return res;
        }
    
        private bJoueur getJoueurBySession(Session session) {
    	    bJoueur res = null;
            for (bJoueur bean : this.joueurs) {
                if (bean.getSession().equals(session) ){
                    res = bean;
                    break; //Inutile de parcourir le reste de la liste si on a trouvé ce qu'on cherchait
                }
            }
            return res;
        }

        private void sendToAllConnectedSessions(JsonObject message) {
            for (Session session : this.sessions) {
                sendToSession(session, message);
            }
        }

        private void sendToSession(Session session, JsonObject message) {
            try {
                session.getBasicRemote().sendText(message.toString());
            } catch (IOException ex) {
        	    this.sessions.remove(session);
                Logger.getLogger(JoueurHandler.class.getName()).log(Level.SEVERE, null, ex);
            }
        }
    }
}*/