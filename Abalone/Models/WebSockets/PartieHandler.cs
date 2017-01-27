/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Abalone.Models{
    public class PartieHandler {
        private Set<bPartie> parties  = new HashSet<>();


    // Méthodes publiques
    //---------------------------------------------------
        public void gestionOuverture(Session session, int uid, int couleur) {
		    boolean partieExiste = false;
		
		    bPartie partie = getPartieByUid(uid);
		    if(partie != null){
			    partieExiste = true;
			    if(couleur == 0) {  partie.setSession_noir(session);  }
			    else {  partie.setSession_blanc(session);  }
			    sendReady(partie); 
		    }
		    if(partieExiste == false){
			    bPartie bean = new bPartie();
			    bean.setUid_partie(uid);
			
			    if(couleur == 0) {  bean.setSession_noir(session);  }
			    else {  bean.setSession_blanc(session);  }
			    this.parties.add(bean);
		    }
	    }

	    public void gestionFermeture(Session session) {
		    gestionAbandon(session, true); //Se déconnecter en partie revient a une défaite par abandon. S'il n'y avait pas de partie en cours (déjà finie) il ne se passera rien
	    }
	
	    public void gestionAbandon(Session session, boolean estTimeOut) {
		    int couleur = getCouleurBySession(session);
		    bPartie bean = getPartieBySession(session);
		
		    if(bean != null){ //La partie est toujours en cours
			    Partie actuelle = Partie.trouverPartie(bean.getUid_partie());
			
			    if(actuelle != null){
				    if(couleur == 0){ //C'est noir qui abandonne
					    actuelle.fin(1, true);
					    if(estTimeOut){ sendTimeOut(bean.getSession_blanc()); }//On prévient blanc
					    else          { sendSurrend(bean.getSession_blanc()); }
				    } else { //C'est blanc qui s'est déconnecté
					    actuelle.fin(0, true);
					    if(estTimeOut){ sendTimeOut(bean.getSession_noir()); } //On prévient noir
					    else          { sendSurrend(bean.getSession_noir()); }
				    }
			    }
			    parties.remove(actuelle); //La partie est finie, pas de raison de la garder
		    }
	    }

	    public void gestionMouvement(Session session, bMove moves) {
		    int res, couleur = getCouleurBySession(session);
		    bPartie bean = getPartieBySession(session);
		    bMoveResp reponse = new bMoveResp();

		    if(bean != null){ //La partie est toujours en cours
			    Partie actuelle = Partie.trouverPartie(bean.getUid_partie());
			
			    if(actuelle.estSonTour(couleur) && actuelle.isPeutBouger()){
				    res = actuelle.gestionMouvement(couleur, moves, reponse);
				    switch(res){
					    case -1: sendUnallowed(bean, session);  break; 
					    case 0 : sendVictory(bean, 0); break; //noir
					    case 1 : sendVictory(bean, 1); break; //blanc
					    case 2 : 
						    sendAllowed(session, actuelle.getScoreNoir(), actuelle.getScoreBlanc(), reponse);
						    if(bean.getSession_blanc().equals(session)) {   sendMoves(bean.getSession_noir(), actuelle.getScoreNoir(), actuelle.getScoreBlanc(), reponse);   }
						    else {   sendMoves(bean.getSession_blanc(), actuelle.getScoreNoir(), actuelle.getScoreBlanc(), reponse);   }
						    break; 
				    }
			    } //Sinon on l'ignore simplement
		    }
	    }

	    public void gestionFinTour(Session session) {
		    int couleur = getCouleurBySession(session);
		    bPartie bean = getPartieBySession(session);

		    if(bean != null){ //La partie est toujours en cours
			    Partie actuelle = Partie.trouverPartie(bean.getUid_partie());
			
			    if(actuelle.estSonTour(couleur)){ //Si ce n'est pas son tour ça ne posera pas de réel problème mais ça sera une nuisance graphique.
				    if(couleur == 0){ sendBeginTurn(bean.getSession_blanc()); }
				    else { sendBeginTurn(bean.getSession_noir()); }
				    actuelle.setTour( actuelle.getTour() * -1 );
				    actuelle.setPeutBouger(true);
			    }
		    }
	    }
	

    // Méthodes privées
    //---------------------------------------------------	
	    private void sendReady(bPartie bean) {
		    JsonProvider provider = JsonProvider.provider();
            JsonObject message = provider.createObjectBuilder()
                    .add("action", "pret")
                    .build();
            System.out.println(message.toString());
            sendToSession(bean.getSession_noir(), message);
            sendToSession(bean.getSession_blanc(), message);
        }
    
	    private void sendTimeOut(Session session) {
		    JsonProvider provider = JsonProvider.provider();
            JsonObject message = provider.createObjectBuilder()
                    .add("action", "timeout")
                    .build();
            System.out.println(message.toString());
            sendToSession(session, message);
	    }

	    private void sendSurrend(Session session) {
		    JsonProvider provider = JsonProvider.provider();
            JsonObject message = provider.createObjectBuilder()
                    .add("action", "surrend")
                    .build();
            System.out.println(message.toString());
            sendToSession(session,  message);
	    }
	
	    private void sendBeginTurn(Session session) {
		    JsonProvider provider = JsonProvider.provider();
	        JsonObject message = provider.createObjectBuilder()
	                .add("action", "beginTurn")
	                .build();
            System.out.println(message.toString());
	        sendToSession(session, message);
	    }
	
	    private void sendAllowed(Session session, int sNoir, int sBlanc, bMoveResp bean) {
		    JsonProvider provider = JsonProvider.provider();
            JsonObject message = provider.createObjectBuilder()
                    .add("action", "allowed")
        		    .add("pNoir", sNoir)
                    .add("pBlanc",sBlanc)
                    .add("ori_x1", bean.getM().ox1())
                    .add("ori_y1", bean.getM().oy1())
                    .add("ori_x2", bean.getM().ox2())
                    .add("ori_y2", bean.getM().oy2())
                    .add("ori_x3", bean.getM().ox3())
                    .add("ori_y3", bean.getM().oy3())
                    .add("des_x1", bean.getM().dx1())
                    .add("des_y1", bean.getM().dy1())
                    .add("des_x2", bean.getM().dx2())
                    .add("des_y2", bean.getM().dy2())
                    .add("des_x3", bean.getM().dx3())
                    .add("des_y3", bean.getM().dy3())
                    .add("des_x4", bean.dx4())
                    .add("des_y4", bean.dy4())
                    .add("des_x5", bean.dx5())
                    .add("des_y5", bean.dy5())
                    .build();
            System.out.println(message.toString());
            sendToSession(session,  message);
	    }
	
	    private void sendUnallowed(bPartie bean, Session session) {
		    JsonProvider provider = JsonProvider.provider();
            JsonObject message = provider.createObjectBuilder()
                    .add("action", "unallowed")
                    .build();
            System.out.println(message.toString());
            sendToSession(session,  message);
	    }

	    private void sendMoves(Session session, int sNoir, int sBlanc, bMoveResp bean) {
		    JsonProvider provider = JsonProvider.provider();
            JsonObject message = provider.createObjectBuilder()
                    .add("action", "move")
        		    .add("pNoir", sNoir)
                    .add("pBlanc",sBlanc)
                    .add("ori_x1", bean.getM().ox1())
                    .add("ori_y1", bean.getM().oy1())
                    .add("ori_x2", bean.getM().ox2())
                    .add("ori_y2", bean.getM().oy2())
                    .add("ori_x3", bean.getM().ox3())
                    .add("ori_y3", bean.getM().oy3())
                    .add("des_x1", bean.getM().dx1())
                    .add("des_y1", bean.getM().dy1())
                    .add("des_x2", bean.getM().dx2())
                    .add("des_y2", bean.getM().dy2())
                    .add("des_x3", bean.getM().dx3())
                    .add("des_y3", bean.getM().dy3())
                    .add("des_x4", bean.dx4())
                    .add("des_y4", bean.dy4())
                    .add("des_x5", bean.dx5())
                    .add("des_y5", bean.dy5())
                    .build();
            System.out.println(message.toString());
            sendToSession(session,  message);
	    }
	
	    private void sendVictory(bPartie bean, int couleur) {
		    JsonProvider provider = JsonProvider.provider();
            JsonObject message = provider.createObjectBuilder()
                    .add("action", "victoire")
                    .add("gagnant", couleur) 
                    .build();
            System.out.println(message.toString());
            sendToSession(bean.getSession_noir(),  message);
            sendToSession(bean.getSession_blanc(), message);
	    }
	
	    private int getCouleurBySession(Session session){
		    int res = 0;
		    bPartie actuelle = getPartieBySession(session);
		
		    if(actuelle.getSession_blanc().equals(session)){ 
			    res = 1;
		    }
		    return res;
	    }
	
        private bPartie getPartieByUid(int uid) {
    	    bPartie res = null;
            for (bPartie bean : this.parties) {
                if (bean.getUid_partie() == uid){
                    res = bean;
                    break; //Inutile de parcourir le reste de la liste si on a trouvé ce qu'on cherchait
                }
            }
            return res;
        }
    
        private bPartie getPartieBySession(Session session) {
    	    bPartie res = null;
            for (bPartie bean : this.parties) {
        	    if(bean.getSession_noir() != null){
        		    if (bean.getSession_noir().equals(session)) {
                        res = bean;
                        break; //Inutile de parcourir le reste de la liste si on a trouvé ce qu'on cherchait
                    }
        	    } 
        	    if(bean.getSession_blanc() != null){
        		    if (bean.getSession_blanc().equals(session)){
                        res = bean;
                        break; //Inutile de parcourir le reste de la liste si on a trouvé ce qu'on cherchait
                    }
        	    }
            }
            return res;
        }

        private void sendToSession(Session session, JsonObject message) {
            try {
                session.getBasicRemote().sendText(message.toString());
            } catch (IOException ex) {
                //Logger.getLogger(JoueurHandler.class.getName()).log(Level.SEVERE, null, ex);
            }
        }
    }
}*/