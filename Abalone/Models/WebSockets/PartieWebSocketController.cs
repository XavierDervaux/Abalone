/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Abalone.Models {
    //@ServerEndpoint("/partieSocket")
    public class PartieWebSocketServer {
	    @Inject
	    private PartieHandler partieHandler;
	
	    @OnOpen
	    public void open(Session session) { 
		    //System.out.println("Connexion entrante.");
	    }
	
        @OnClose
        public void close(Session session) { 
    	    this.partieHandler.gestionFermeture(session);
        }

        @OnError
        public void onError(Throwable error) { 
            Logger.getLogger(PartieWebSocketServer.class.getName()).log(Level.SEVERE, null, error);
        }

        @OnMessage
        public void OnMessage(String message, Session session) { 
            try (JsonReader reader = Json.createReader(new StringReader(message))) {
                JsonObject jsonMessage = reader.readObject(); 

                if ("add".equals(jsonMessage.getString("action"))) { //uid, couleur
                    int uid = jsonMessage.getInt("uid");
                    int couleur = jsonMessage.getInt("couleur");
                    this.partieHandler.gestionOuverture(session, uid, couleur);
                }
            
                if ("forfait".equals(jsonMessage.getString("action"))) {  
                    this.partieHandler.gestionAbandon(session, false);
                }
            
                if ("move".equals(jsonMessage.getString("action"))) {  
            	    bMove bean = new bMove();
            	    //bean.setType(jsonMessage.getInt("type"));
            	    bean.setOri_x1(jsonMessage.getInt("ori_x1")); //Jusqu'a 3 billes pour bouger simultanément, source et destination
            	    bean.setOri_y1(jsonMessage.getInt("ori_y1"));
            	    bean.setOri_x2(jsonMessage.getInt("ori_x2"));
            	    bean.setOri_y2(jsonMessage.getInt("ori_y2"));
            	    bean.setOri_x3(jsonMessage.getInt("ori_x3"));
            	    bean.setOri_y3(jsonMessage.getInt("ori_y3"));        	
            	    bean.setDes_x1(jsonMessage.getInt("des_x1")); 
            	    bean.setDes_y1(jsonMessage.getInt("des_y1"));
            	    bean.setDes_x2(jsonMessage.getInt("des_x2"));
            	    bean.setDes_y2(jsonMessage.getInt("des_y2"));
            	    bean.setDes_x3(jsonMessage.getInt("des_x3"));
            	    bean.setDes_y3(jsonMessage.getInt("des_y3"));       
                    this.partieHandler.gestionMouvement(session, bean);
                }
            
                if("finTour".equals(jsonMessage.getString("action"))){
                    this.partieHandler.gestionFinTour(session);
                }
            } catch (Exception e){
        	    System.out.println(e);
            }
        }
    }
}*/