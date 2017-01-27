/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Abalone.Models{
    //@ServerEndpoint("/joueurSocket")
    public class JoueurWebSocketServer {
	    private JoueurHandler sessionHandler;
	
	    public void OnOpen(Session session) { 
		    this.sessionHandler.addSession(session);
	    }
	
        public void OnClose(Session session) { 
    	    this.sessionHandler.removeSession(session);
        }
        
        public void OnError(Throwable error) { 
            //Logger.getLogger(JoueurWebSocketServer.class.getName()).log(Level.SEVERE, null, error);
        }
    
        public void OnMessage(String message, Session session) {
            try (JsonReader reader = Json.createReader(new StringReader(message))) {
                JsonObject jsonMessage = reader.readObject(); 

                if ("add".equals(jsonMessage.getString("action"))) { 
                    bJoueur bean = new bJoueur();
                    bean.setSession(session);
                    bean.setJoueur_pseudo(jsonMessage.getString("pseudo"));
                    bean.setJoueur_email(jsonMessage.getString("email"));
                    this.sessionHandler.gestionDoublon(bean);
                }
            
                if("demande".equals(jsonMessage.getString("action"))) {
                    int destId = (int) jsonMessage.getInt("destinataire");
                    this.sessionHandler.gestionDemande(destId, session);
                }
            
                if("reponse".equals(jsonMessage.getString("action"))) {
        		    int destId = (int) jsonMessage.getInt("destinataire");
            	    boolean confirm = (boolean) jsonMessage.getBoolean("confirm");
            	    this.sessionHandler.gestionConfirmation(destId, confirm, session);
                }
            } catch (Exception e){
			    e.printStackTrace();
            }
        }
    }  
}*/