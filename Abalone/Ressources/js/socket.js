var joueurSocket;
var making;
var player_current;
var player_invitation;

    function initMatchMaking(pseudo, email) {
        if (window.location.href.indexOf("/Matchmaking/Index") > -1) { //Si l'adresse contiens ceci, autrement dit si on est sur cette page
            joueurSocket = $.connection.matchmakingHub;

            joueurSocket.client.receiveAdd = function (id, pseudo, email) {
                window.alert("1");
                //making.addJoueur(new Joueur(id, pseudo, email));
            };

            joueurSocket.client.receiveRemove = function (id) {
                window.alert("2");
                //making.removeJoueur(new Joueur(id, "", ""));
            };

            joueurSocket.client.receiveDemande = function (id, joueur_pseudo, joueur_email) {
                window.alert("3");
                //getInvitation(json);
            };

            joueurSocket.client.receiveConfirmation = function (id, joueur_pseudo, joueur_email, confirm) {
                window.alert("4");
                //getRespond(json);
            };

            joueurSocket.client.receiveAlreadyConnected = function (joueur_pseudo) {
                window.alert("5");
                //getDejaConnect(json);
            };

            $.connection.hub.start().done(function (){ //Entame la connexion
                window.alert("0");
                sendJoueur(player_current);
            });

            making = new MatchMaking();
            player_current = new Joueur(-1, pseudo, email); 
            making.addJoueur(player_current);
        
            $('#invitation').on('hide.bs.modal', function (e) {
                if(player_invitation != null){
                    sendResponse(player_invitation, false);
                    player_invitation = null;
                }
            });
            $('#doubleUser').on('hidden.bs.modal', function (e) {
        	    window.location = "/Menu/Index";
            });
        }
    }

    function MatchMaking(){
        this.joueurs= [];

        this.addJoueur = function(joueur){
    	    if(this.joueurs[this.joueurs.length-1] == null){
    		    this.joueurs[this.joueurs.length-1] = joueur;
    	    } else{
    		    this.joueurs.push(joueur);
    	        this.joueurs.sort();
    	    }
            this.refresh();
        }

        this.removeJoueur = function(joueur){
            var i = 0, trouver = false;
        
            while(i < this.joueurs.length && !trouver){
                if(joueur.id == this.joueurs[i].id){
                    trouver = true;
                }
                i++;
            }
        
            if(trouver){
                this.joueurs[i-1] = null;
                this.joueurs.sort();
                this.refresh(); 
            }
        }
 
        this.refresh = function(){
            genereTableJoueur(this.joueurs);
        }
    }

    function Joueur(id, pseudo, email){
        this.id = id;
        this.pseudo = pseudo;
        this.email = email;
    }

    function genereTableJoueur(joueurs){
        var table = $id('listJoueur');
        var title = table.firstChild;
    
        //Je vide le tableau
        table.innerHTML = "";
    
        var trHEader = document.createElement("tr");
        var th = document.createElement("th");
        th.innerHTML = "Joueurs connectés";
        th.colSpan="2" ;
        th.className="th-title define-th";
   
        trHEader.appendChild(th);
        table.appendChild(trHEader);
        for(var i = 0; i < joueurs.length; i++){
           if(joueurs[i] != null){
    	   
    	       item = joueurs[i];
    	       if(item.email != player_current.email){
                   //Genetation du tableau
                    var tr = document.createElement("tr");
                    var tdPseudo = document.createElement("td");
                    var tdButton = document.createElement("td");
                    var button = document.createElement("button");

                   //Propriété de la td pseudo
                   tdPseudo.innerHTML= item.pseudo;
                   tdPseudo.className="grey td-listPlayer";

                   tdButton.className="wth-100 td-listPlayer";
                   //Propriété du bouton
                    button.className ="btn btn-lg btn-success mrg-right-10";
                    button.type="button";
                    button.title="Invitation";
                    button.innerHTML="Envoyer une invitation";
               
                    onClickNotification(button, item);

                   //Assemblage
                   tdButton.appendChild(button);
                   tr.appendChild(tdPseudo);
                   tr.appendChild(tdButton);

                   table.appendChild(tr);
               }
           }
       }
    }

    function onClickNotification(button, item){
	    button.onclick = function(){
		    sendInvitation(item);
		     $(button).popover(
	            {
	                content: "Invitation envoyée !",
	                trigger:"focus",
	                placement:"top"
	            }
		    );
	    }
    
    }

    function respondInvitation(respond){
       if(player_invitation != null){
	       tmp= player_invitation;
            sendResponse(player_invitation, respond);
            $('#invitation').modal('hide');
        
            if(respond){
       		    sendRequestPost(tmp.email_source);
       	    }
            player_invitation = null;
       }
    }

    function getInvitation(json){
        player_invitation = json;
        $id('messageInvitation').innerHTML= json.pseudo_source + " veut jouer avec vous.";
        $('#invitation').modal('show');
    }

    function getRespond(json){
        if(json.confirm == true){
            $id('respondMessageInvitation').innerHTML=json.pseudo_source + " a accepté votre invitation, lancement de la partie...";
            $('#respondInvitation').modal('show');
            //request post Ã  faire
            sendRequestPost(player_invitation.email);
            player_invitation = null;
        } else{
            $id('respondMessageInvitation').innerHTML=json.pseudo_source + " a refusé votre invitation";
            $('#respondInvitation').modal('show');
        }
    }

    function getDejaConnect(json){
	    $('#doubleUser').modal('show');  
    }

    function sendRequestPost(email){
	    request = {
    	    joueur1 : player_current.email,
    	    joueur2 : email
        };
        redirect_post("/Jouer/Index", request);
    }

    function sendJoueur(joueur) {
        joueurSocket.server.add(joueur.pseudo, joueur.email);
    }

    function sendInvitation(joueur) {
        joueurSocket.server.demande(joueur.id);
    }

    function sendResponse(joueur, response) {
        joueurSocket.server.reponse(joueur.id_source, response);
    }
