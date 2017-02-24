var partieSocket;
var partie;
var ressource = "/ressources/";

function initPartie(uid, pseudoJNoir, emailJNoir, pseudoJBlanc, emailJBlanc, emailJCurrent) {    
    if (window.location.href.indexOf("/Jouer/Index") > -1) {
        joueurSocket = $.connection.partieHub;

        joueurSocket.client.pret = function () {
            setDebutPartie();
        };
        joueurSocket.client.move = function (sNoir, sBlanc, mov) {
            var origin = new Mouvements();
            var destination = new Mouvements();
            origin.init();
            destination.init();    

            mouvementToClient(mov, origin, destination);
            setMouvementBille(origin, destination);
            setScore(sNoir, sBlanc);
        };
        joueurSocket.client.allowed = function (sNoir, sBlanc, mov) {
            var origin = new Mouvements();
            var destination = new Mouvements();
            origin.init();
            destination.init();

            mouvementToClient(mov, origin, destination);
            setMouvementBille(origin, destination, true);
            setScore(sNoir, sBlanc);
        };
        joueurSocket.client.unallowed = function () {
            inversionMatrice();
            partie.plateau.billeMove.init();
            resetBilleSelected();
            setUnauthorized(); 
        };
        joueurSocket.client.surrend= function () {  //Abandon de l'adversaire
            setFinPartie(true, true);
        };
        joueurSocket.client.timeout = function () { //L'adversaire à quitter violament 
            setFinPartie(true, true);
        };
        joueurSocket.client.beginTurn = function () { //Changement de tour
            partie.nextTurn();
        };
        joueurSocket.client.victoire = function (colorG) { //Victoire d'un joueur
            if (colorG == partie.JoueurMe.color) {
                setFinPartie(true, false);
            } else {
                setFinPartie(false, false);
            }
        };

        //Pour une utilisation correcte, on vire le responsive.
        $id('corps').style.width = "1000px";
        //On inialise l'interface
        initInterface(uid, pseudoJNoir, emailJNoir, pseudoJBlanc, emailJBlanc, emailJCurrent);
    }
}


function initInterface(uid, pseudoJNoir,emailJNoir, pseudoJBlanc,emailJBlanc, emailJCurrent) {
    hideAll();
    show("syncPartie");
    
    joueur1 = new JoueurPartie(new Joueur(uid,pseudoJNoir, emailJNoir),0, 0);
    joueur2 = new JoueurPartie(new Joueur(uid,pseudoJBlanc, emailJBlanc),0, 1); 
    
    if(joueur1.joueur.email == emailJCurrent){
        partie = new Partie(joueur1, joueur2);
    } else{
        partie = new Partie(joueur2, joueur1);
    }
    $.connection.hub.start().done(function () { //Entame la connexion
        sendPartiePlayer();
    });
}


/*****************************************************
 *  Objets  
*****************************************************/

/**
 * Objet partie qui gére les joueurs / score / plateau
 */
function Partie(JoueurMe, joueurAdv) {
    this.JoueurMe = JoueurMe;
    this.joueurAdv = joueurAdv;
    this.isTurn = 1;
    this.plateau = new Plateau();
    
    this.init = function(){
        this.plateau.init();
    }

    //On change de tour;
    this.nextTurn = function(){
        this.isTurn *= -1
        showTurn();
    }
}

/**
 * Objet Joueur
 */
function JoueurPartie(joueur, score, color) {
    this.joueur = joueur;
    this.score = score;
    this.color = color;
}

/**
 * Objet Bille
 */
function Bille(x, y) {
    this.x = x;
    this.y = y;
    
    this.equals = function(bille) {
        return (this.x == bille.x && this.y == bille.y);
    }
    this.equalsX = function (x) {
        return this.x == x;
    }
    this.equalsY = function (y) {
        return this.y == y;
    }
    this.equalsCoordinate = function(x, y){
        return (this.x == x && this.y == y);
    }
}

/**
 * Object Plateay qui gére la position des billes et les déplacements.
 */
function Plateau(){
    this.terrain = new Array(9);
    this.billeSelected = new VecteurBille(3);
    this.billeMove = new VecteurBille(3);
    
    this.init = function(){
        this.initTerrain(17);
        this.initBillePlayer();
        this.billeSelected.init();
    }
    
    this.initTerrain = function(size ,val = -99){
        for(i = 0; i < this.terrain.length; i++){
            this.terrain[i] = new Array(size);
            for(j = 0; j < size; j++){
                this.terrain[i][j] = val;
            }
        }
         for(i = 0; i < this.terrain.length/2; i++){
            for(j = 4 - i; j < 13 + i; j++){
                this.terrain[i][j] = 0;
            }
         }
        for(i = this.terrain.length-1; i >= this.terrain.length/2; i--){
            for(j = i - 4; j < 21 - i; j++){
                this.terrain[i][j] = 0;
            }
         }
    }
    
    this.initBillePlayer = function(){
        //Initialisation de la matrice 
        for(i = 0; i < 2; i++){
             for(j = 4 - i; j < 13+i; j = j+2){
                   this.terrain[i][j] = -1;
             }
         }
        for(i = 6; i < 11; i = i+2){
            this.terrain[2][i] = -1;
        }
        
         for(i = this.terrain.length-1; i >= this.terrain.length-2; i--){
             for(j = i - 4; j <  21 - i; j = j+2){
                   this.terrain[i][j] = 1;
             }
         }
         for(i = 6; i < 11; i = i+2){
            this.terrain[6][i] = 1;
         }
        
        //Initialisation des billes au niveau de l'interface !
        var plat = $id('plateauPartie').children;
        for(i = 0; i < 2; i++){
            var row = plat[i].children;
            for(j = 0; j < row.length; j++){
                setBilleImage(row[j], "", partie.joueurAdv.color);
            }
        }
        for(i = 2; i < 5; i++){
            setBilleImage(plat[2].children[i], "", partie.joueurAdv.color);
        }
        
        for(i = 2; i < 5; i++){
            setBilleImage(plat[6].children[i], "", partie.JoueurMe.color);
        }
        for(i = 7; i < 9; i++){
            var row = plat[i].children;
            for(j = 0; j < row.length; j++){
                 setBilleImage(row[j], "", partie.JoueurMe.color);
            }
        }
    }

}

function VecteurBille(size, valDefault = null){
    this.vecteur = new Array(size);
    this.current= 0;
    this.valDefault = valDefault;
    
    
    //Initialise le vecteur, -1 par d�faut
    this.init= function(){
        for(i = 0; i < this.vecteur.length; i++){
            this.vecteur[i] =  this.valDefault;
        }
        this.current= 0;
    }
    
    //Retour vrai si le vecteur est vide
    this.isEmptyBilleSelected = function(){
       return this.current == 0;
    }
    
    //Ajoute la bille selectionn�e au vecteur
    this.addBille = function(x, y){
        if(this.current < this.vecteur.length){
            this.vecteur[this.current] = new Bille(x,y);
            this.current++;
        }
    }
    
    //Retire la bille demand� du vecteur
    this.removebille = function(x,y){
        var i=0, trouver = false;
        while(i < this.vecteur.length && !trouver){
            if(this.vecteur[i] != null){
                if(this.vecteur[i].x == x && this.vecteur[i].y == y){
                    trouver = true;
                }
            }
            i++;
        }
        if(trouver){
            //On indique la valeur par d�fault
            this.vecteur[i-1] = valDefault;
            this.current--;
            //On trie le tableau.
            this.vecteur.sort();
        }
    }
    
    this.copy = function(vecteur){
        for(i = 0; i < this.vecteur.length; i++){
             this.vecteur[i] = vecteur[i];
        }
    }
}

function Mouvements(size = 5) {
    this.coord = new Array(size);
    this.current = 0;
    this.length = size;

    this.add = function (x, y) {
        if (this.current < this.length){
            this.coord[this.current] = new Bille(x, y);
            this.current++;
        }
    }
    this.addPos = function (pos, x, y) {
        this.coord[pos] = new Bille(x,y);
    }

    this.init = function (val = -1) {
        for (i = 0; i < this.coord.length; i++) {
            this.coord[i] = new Bille(val, val);
        }
    }

    this.move = function (vect, born, valX, valY){
        for (i = 0; i < born; i++) {
            this.coord[i].X = vect[i].x;
        }
    }
}

function redirectionMenu(){
    document.location.href="menu.html";
}

/**
 * Intervient quand je clique sur une bille
 */
function clickBille(bille, x, y){
    if(partie.isTurn > 0){ //Si c'est mon tour
        if(partie.plateau.billeSelected.isEmptyBilleSelected()){ //si pas de bille de selectionner
            if(partie.plateau.terrain[x][y] == 1){ //Je selectionne UNE bille à moi
                updateBille(bille, x, y, true);
            }
        } else{ //si j'ai une bille de selectionnée
            if(partie.plateau.terrain[x][y] == 2){  //si je clique dessus, je la retire 
                //Si j'ai 3 billes de selectionné, je vérifie que la bille qu'on veut pas
                //retirer est la bille du milieu
                if(partie.plateau.billeSelected.current == 3){ 
                    if(!isBilleMiddle(x,y)){
                        updateBille(bille, x, y, false);
                    }
                } else{
                    updateBille(bille, x, y, false);
                }
            } else if(partie.plateau.terrain[x][y] == 1){ //je clique sur une bille quelconque à moi.
                if(isSelectBille(x, y)){
                     updateBille(bille, x, y, true);
                }
            } else if(partie.plateau.terrain[x][y] < 1){ //je veux me déplacer
                   if(checkMouvement(x, y)){ //J'ai l'autorisation
                       //J'envois la demande au serveur
                       sendMouvement();
                   }
            }
        }
    }
}

/**
 * Intervient quand je survole une bille
 */
function survolBille(bille, x, y){
    if(partie.isTurn  > 0){ //Si c'est mon tour
        if(partie.plateau.terrain[x][y] == 0){ //Aucune bille
            setBilleImage(bille, "hover");
        } else if(partie.plateau.terrain[x][y] == 1){ //Bille � moi
            setBilleImage(bille, "hover",partie.JoueurMe.color);
        }
    } 
}

/**
 * Intervient quand je ne survole plus une bille
 */
function unsurvolBille(bille, x, y){
   if(partie.isTurn > 0){ //Si c'est mon tour
        if(partie.plateau.terrain[x][y] == 0){ //Aucune bille
            setBilleImage(bille, "empty");
        } else if(partie.plateau.terrain[x][y] == 1){ //Bille � moi
            setBilleImage(bille,"",partie.JoueurMe.color);
        }    
   }
}

function resetBilleSelected(){
	var vecteur = partie.plateau.billeSelected.vecteur;
	var plat = $id('plateauPartie').children;
	var row;
	
	for(i=0; i < partie.plateau.billeSelected.current; i++){
		partie.plateau.terrain[vecteur[i].x][vecteur[i].y] = 1;
		 row = plat[vecteur[i].x].children;
		 setBilleImage(row[determineColDOM(vecteur[i].x,vecteur[i].y)], "", partie.JoueurMe.color);
	}
	partie.plateau.billeSelected.init();   
}


/**
 * Fonction qui est appelée quand un le joueur clique
 * sur le bouton abandonner
 */
function setAbandonner(){
    sendForfait();                          //On envoit au socket
    setFinPartie(false, true);              //On affiche la fin de la partie
    $('#modalAbandonner').modal('hide');    
}

function getNameColor(color){
    return (color == 1) ? "white" : "black";
}

/**
 * Met à jour une bille
 */
function updateBille(bille, x, y, isSelected){
    if(isSelected){
        setBilleImage(bille, "selected",partie.JoueurMe.color);
        partie.plateau.terrain[x][y] = 2;
        partie.plateau.billeSelected.addBille(x,y);
    } else{
        partie.plateau.billeSelected.removebille(x, y);
        partie.plateau.terrain[x][y] = 1;
        setBilleImage(bille, "",partie.JoueurMe.color);
    }
}

/**
 * Vérifie si la bille est confirme au règle du jeu
 */
function isSelectBille(x, y){
    var nbr = partie.plateau.billeSelected.current;
    if(nbr == 1){
        var bille = partie.plateau.billeSelected.vecteur[0];
        if(bille.equalsCoordinate(x, y+2)){
            return true;
        } 
        if(bille.equalsCoordinate(x, y-2) || bille.equalsCoordinate(x+1, y) || bille.equalsCoordinate(x-1, y) || bille.equalsCoordinate(x+1, y-1) || 
            bille.equalsCoordinate(x+1, y+1) || bille.equalsCoordinate(x-1, y-1) || bille.equalsCoordinate(x-1, y+1)){
            return true;
        }
    } else if(nbr == 2){
        var bille1 = partie.plateau.billeSelected.vecteur[0];
        var bille2 = partie.plateau.billeSelected.vecteur[1];
        if(bille1.equalsX(bille2.x)){ //je suis sur la même ligne
            if(bille1.equalsCoordinate(x,y+2) || bille1.equalsCoordinate(x,y-2) || bille2.equalsCoordinate(x,y+2) || bille2.equalsCoordinate(x,y-2)){ //si je suis � gauche ou � droite
                return true;
            }
        }else if(bille1.equalsCoordinate(bille2.x-1, bille2.y+1) || bille1.equalsCoordinate(bille2.x+1, bille2.y-1)){ //Je suis � la verticale vers la gauche
            if(bille1.equalsCoordinate(x-1, y+1) || bille1.equalsCoordinate(x+1, y-1) || bille2.equalsCoordinate(x-1, y+1) || bille2.equalsCoordinate(x+1, y-1) ){
                return true;
            }
            
        } else if(bille1.equalsCoordinate(bille2.x+1, bille2.y+1) || bille1.equalsCoordinate(bille2.x-1, bille2.y-1)){ //Je suis � la verticale vers la droite
            if(bille1.equalsCoordinate(x+1, y+1) || bille1.equalsCoordinate(x-1, y-1) || bille2.equalsCoordinate(x+1, y+1) || bille2.equalsCoordinate(x-1, y-1)){
                return true;
            }
        }
    }
    return false;
}

/**
 * Vérifie si le mouvement est autorisé
 */
function checkMouvement(x, y){
    var nbr = partie.plateau.billeSelected.current;
    var vecteur = partie.plateau.billeSelected.vecteur;
    if( nbr == 1){ //J'ai une bille � d�placer
        if(isSelectBille(x,y)){
            partie.plateau.billeMove.addBille(x,y);
            return true;
        }
    } else if(nbr == 2){  //Si j'ai deux billes � d�placer
        var middle = billeLeft();
         if(isVerticalRight()){ 
           if(isNeighbourVerticalRight(vecteur[0],x,y) || isNeighbourVerticalRight(vecteur[1],x,y)){
              if(x < middle.x){
                    partie.plateau.billeMove.addBille(vecteur[0].x-1, vecteur[0].y+1);
                    partie.plateau.billeMove.addBille(vecteur[1].x-1, vecteur[1].y+1);
                 } else{
                    partie.plateau.billeMove.addBille(vecteur[0].x+1, vecteur[0].y-1);
                    partie.plateau.billeMove.addBille(vecteur[1].x+1, vecteur[1].y-1);
                 } 
               return true;
            } else if(middle.equalsY(y+2) || middle.equalsY(y-2)){//je me déplace à gauche ou à droite
                if(middle.equalsCoordinate(x,y-2) && isNeighbourLeftOrRight(vecteur, true) == 2){
                    partie.plateau.billeMove.addBille(vecteur[0].x, vecteur[0].y+2);
                    partie.plateau.billeMove.addBille(vecteur[1].x, vecteur[1].y+2);
                     return true;
                } else if(middle.equalsCoordinate(x,y+2) && isNeighbourLeftOrRight(vecteur, false) == 2){
                    partie.plateau.billeMove.addBille(vecteur[0].x, vecteur[0].y-2);
                    partie.plateau.billeMove.addBille(vecteur[1].x, vecteur[1].y-2);
                     return true;
                }
            }
         } else if(isVerticalLeft()){ 
                if(isNeighbourVerticalLeft(vecteur[0],x,y) || isNeighbourVerticalLeft(vecteur[1],x,y)){
                if(x < middle.x){ //On remonte sur le plateau
                    partie.plateau.billeMove.addBille(vecteur[0].x-1, vecteur[0].y-1);
                    partie.plateau.billeMove.addBille(vecteur[1].x-1, vecteur[1].y-1);
                } else{
                    partie.plateau.billeMove.addBille(vecteur[0].x+1, vecteur[0].y+1);
                    partie.plateau.billeMove.addBille(vecteur[1].x+1, vecteur[1].y+1);
                }
                return true;
            } else if(middle.equalsY(y+2) || middle.equalsY(y-2)){//je me d�place � gauche ou � droite
                if(middle.equalsCoordinate(x,y-2) && isNeighbourLeftOrRight(vecteur, true) == 2){
                    partie.plateau.billeMove.addBille(vecteur[0].x, vecteur[0].y+2);
                    partie.plateau.billeMove.addBille(vecteur[1].x, vecteur[1].y+2);
                    return true;
                } else if(middle.equalsCoordinate(x,y+2) && isNeighbourLeftOrRight(vecteur, false) == 2){
                    partie.plateau.billeMove.addBille(vecteur[0].x, vecteur[0].y-2);
                    partie.plateau.billeMove.addBille(vecteur[1].x, vecteur[1].y-2);
                    return true;
                }
            }
         } else{ //Je suis en horizontale
            if(middle.equalsX(x+1) || middle.equalsX(x-1)){ //Si je me d�place horizontalement
                var nbr = 0;
                
                if(middle.equalsCoordinate(x+1,y+1) && isNeighbourTopOrBottom(vecteur, true) == 2){ //D�placement vers le haut gauche
                    partie.plateau.billeMove.addBille(vecteur[0].x-1, vecteur[0].y-1);
                    partie.plateau.billeMove.addBille(vecteur[1].x-1, vecteur[1].y-1);
                    
                    return true;
                } else if(middle.equalsCoordinate(x+1,y-1) && isNeighbourTopOrBottom(vecteur, true, false) == 2){ //D�placement vers le haut droite
                	partie.plateau.billeMove.addBille(vecteur[0].x-1, vecteur[0].y+1);
                    partie.plateau.billeMove.addBille(vecteur[1].x-1, vecteur[1].y+1);
                    return true;
                }  else if(middle.equalsCoordinate(x-1,y-1) && isNeighbourTopOrBottom(vecteur, false) == 2){//D�placement vers le bas droite
                    partie.plateau.billeMove.addBille(vecteur[0].x+1, vecteur[0].y+1);
                    partie.plateau.billeMove.addBille(vecteur[1].x+1, vecteur[1].y+1);
                    return true;
                }  else if(middle.equalsCoordinate(x-1,y+1) && isNeighbourTopOrBottom(vecteur, false, false) == 2){//D�placement vers le bas gauche
                    partie.plateau.billeMove.addBille(vecteur[0].x+1, vecteur[0].y-1);
                    partie.plateau.billeMove.addBille(vecteur[1].x+1, vecteur[1].y-1);
                    return true;
                } 
            } else if(middle.equalsX(x)){ //d�placement gauche ou droite sur la m�me ligne
                if(isNeighbourY(vecteur[0], y) ||isNeighbourY(vecteur[1], y)){ //Elle est voisine � une de mes billes
                    if(middle.y > y){ //Je me d�place vers ma gauche
                        partie.plateau.billeMove.addBille(vecteur[0].x, vecteur[0].y-2);
                        partie.plateau.billeMove.addBille(vecteur[1].x, vecteur[1].y-2);
                    } else{ //Je me d�place vers ma droite
                        partie.plateau.billeMove.addBille(vecteur[0].x, vecteur[0].y+2);
                        partie.plateau.billeMove.addBille(vecteur[1].x, vecteur[1].y+2);
                    }

                    return true;
                }
            }
         }
    } 
    else if(nbr == 3){ //Si j'ai 3 billes à deplacer
        var middle = billeMiddle();
        if(isVerticalRight()){  //Je suis en verticale vers la droite
            if(isNeighbourVerticalRight(vecteur[0],x,y) || isNeighbourVerticalRight(vecteur[1],x,y) || isNeighbourVerticalRight(vecteur[2],x,y)){
                 if(x < middle.x){
                    partie.plateau.billeMove.addBille(vecteur[0].x-1, vecteur[0].y+1);
                    partie.plateau.billeMove.addBille(vecteur[1].x-1, vecteur[1].y+1);
                    partie.plateau.billeMove.addBille(vecteur[2].x-1, vecteur[2].y+1);
                 } else{
                    partie.plateau.billeMove.addBille(vecteur[0].x+1, vecteur[0].y-1);
                    partie.plateau.billeMove.addBille(vecteur[1].x+1, vecteur[1].y-1);
                    partie.plateau.billeMove.addBille(vecteur[2].x+1, vecteur[2].y-1);
                 } 
                return true;
             } else if(middle.equalsY(y+2) || middle.equalsY(y-2)){//je me déplace à gauche ou à droite
                if(middle.equalsCoordinate(x,y-2) && isNeighbourLeftOrRight(vecteur, true) == 3){
                    partie.plateau.billeMove.addBille(vecteur[0].x, vecteur[0].y+2);
                    partie.plateau.billeMove.addBille(vecteur[1].x, vecteur[1].y+2);
                    partie.plateau.billeMove.addBille(vecteur[2].x, vecteur[2].y+2);
                    return true;
                } else if(middle.equalsCoordinate(x,y+2) && isNeighbourLeftOrRight(vecteur, false) == 3){
                    partie.plateau.billeMove.addBille(vecteur[0].x, vecteur[0].y-2);
                    partie.plateau.billeMove.addBille(vecteur[1].x, vecteur[1].y-2);
                    partie.plateau.billeMove.addBille(vecteur[2].x, vecteur[2].y-2);
                    return true;
                }
                
            }
        } else if(isVerticalLeft()){  //Je suis en verticale vers la gauche
            if(isNeighbourVerticalLeft(vecteur[0],x,y) || isNeighbourVerticalLeft(vecteur[1],x,y) || isNeighbourVerticalLeft(vecteur[2],x,y)){
                if(x < middle.x){ //On remonte sur le plateau
                    partie.plateau.billeMove.addBille(vecteur[0].x-1, vecteur[0].y-1);
                    partie.plateau.billeMove.addBille(vecteur[1].x-1, vecteur[1].y-1);
                    partie.plateau.billeMove.addBille(vecteur[2].x-1, vecteur[2].y-1);
                } else{
                    partie.plateau.billeMove.addBille(vecteur[0].x+1, vecteur[0].y+1);
                    partie.plateau.billeMove.addBille(vecteur[1].x+1, vecteur[1].y+1);
                    partie.plateau.billeMove.addBille(vecteur[2].x+1, vecteur[2].y+1);
                    
                }
                return true;
            } else if(middle.equalsY(y+2) || middle.equalsY(y-2)){//je me déplace à gauche ou à droite
                if(middle.equalsCoordinate(x,y-2) && isNeighbourLeftOrRight(vecteur, true) == 3){
                    partie.plateau.billeMove.addBille(vecteur[0].x, vecteur[0].y+2);
                    partie.plateau.billeMove.addBille(vecteur[1].x, vecteur[1].y+2);
                    partie.plateau.billeMove.addBille(vecteur[2].x, vecteur[2].y+2);
                     return true;
                } else if(middle.equalsCoordinate(x,y+2) && isNeighbourLeftOrRight(vecteur, false) == 3){
                    partie.plateau.billeMove.addBille(vecteur[0].x, vecteur[0].y-2);
                    partie.plateau.billeMove.addBille(vecteur[1].x, vecteur[1].y-2);
                    partie.plateau.billeMove.addBille(vecteur[2].x, vecteur[2].y-2);
                     return true;
                }
               
            }
        } else{ //Je suis en horizontale
            if(middle.equalsX(x+1) || middle.equalsX(x-1)){ //Si je me déplace horizontalement
                var nbr = 0;
                
                if(middle.equalsCoordinate(x+1,y+1) && isNeighbourTopOrBottom(vecteur, true) == 3){ //Déplacement vers le haut
                    partie.plateau.billeMove.addBille(vecteur[0].x-1, vecteur[0].y-1);
                    partie.plateau.billeMove.addBille(vecteur[1].x-1, vecteur[1].y-1);
                    partie.plateau.billeMove.addBille(vecteur[2].x-1, vecteur[2].y-1);
                    
                    return true;
                } else if(middle.equalsCoordinate(x-1,y-1) && isNeighbourTopOrBottom(vecteur, false) == 3){//Déplacement vers le bas
                    partie.plateau.billeMove.addBille(vecteur[0].x+1, vecteur[0].y+1);
                    partie.plateau.billeMove.addBille(vecteur[1].x+1, vecteur[1].y+1);
                    partie.plateau.billeMove.addBille(vecteur[2].x+1, vecteur[2].y+1);
                    
                    return true;
                }
            } else if(middle.equalsX(x)){ //déplacement gauche ou droite sur la même ligne
                if(isNeighbourY(vecteur[0], y) ||isNeighbourY(vecteur[1], y) || isNeighbourY(vecteur[2], y) ){ //Elle est voisine à une de mes billes
                    if(middle.y > y){ //Je me déplace vers ma gauche
                        partie.plateau.billeMove.addBille(vecteur[0].x, vecteur[0].y-2);
                        partie.plateau.billeMove.addBille(vecteur[1].x, vecteur[1].y-2);
                        partie.plateau.billeMove.addBille(vecteur[2].x, vecteur[2].y-2);
                    } else{ //Je me déplace vers ma droite
                        partie.plateau.billeMove.addBille(vecteur[0].x, vecteur[0].y+2);
                        partie.plateau.billeMove.addBille(vecteur[1].x, vecteur[1].y+2);
                        partie.plateau.billeMove.addBille(vecteur[2].x, vecteur[2].y+2);
                    }

                    return true;
                }
            }
        }
    }
    return false;
}

function isNeighbourTopOrBottom(vecteur, isTop, isLeft = true){
    var nbr = 0; 
    for(i=0; i < vecteur.length; i++){
        if(vecteur[i] != null){
            if(isTop){
            	if(isLeft){
            		if(partie.plateau.terrain[vecteur[i].x-1][vecteur[i].y-1] == 0)
                        nbr++;
            	} else{
            		if(partie.plateau.terrain[vecteur[i].x-1][vecteur[i].y+1] == 0)
                        nbr++;
            	}
                
            } else{
            	if(isLeft){
            		if(partie.plateau.terrain[vecteur[i].x+1][vecteur[i].y+1] == 0)
                        nbr++;
            	} else{
            		if(partie.plateau.terrain[vecteur[i].x+1][vecteur[i].y-1] == 0)
                        nbr++;
            	}
            }

        }
    }
    return nbr;
}

function isNeighbourLeftOrRight(vecteur, isRight){
    var nbr = 0; 
    for(i=0; i < vecteur.length; i++){
        if(vecteur[i] != null){
            if(isRight){
                if(partie.plateau.terrain[vecteur[i].x][vecteur[i].y+2] == 0){
                    nbr++;
                }
            } else{
                if(partie.plateau.terrain[vecteur[i].x][vecteur[i].y-2] == 0){
                    nbr++;
                }
            }
        }
    }
    return nbr;
}

function isNeighbourVerticalLeft(bille, x, y){
    return (bille.equalsCoordinate(x-1, y-1) || bille.equalsCoordinate(x+1, y+1));
}

function isNeighbourVerticalRight(bille, x, y){
    return (bille.equalsCoordinate(x-1, y+1) || bille.equalsCoordinate(x+1, y-1));
}

function isNeighbourY(bille, y){
    return (bille.equalsY(y+2) || bille.equalsY(y-2));         
}
                   
function isBilleMiddle (x,y){
    return billeMiddle().equalsCoordinate(x,y);
}

/**
 * Retourne vrai si les deux billes indique un mouvement vertical vers la gauche
 */
function isVerticalRight(){
    var bille1 = partie.plateau.billeSelected.vecteur[0];
    var bille2 = partie.plateau.billeSelected.vecteur[1];
    
    return (bille1.equalsCoordinate(bille2.x-1, bille2.y+1) || bille1.equalsCoordinate(bille2.x+1, bille2.y-1));
}

/**
 * Retourne vrai si les deux billes indique un mouvement vertical vers la droite
 */
function isVerticalLeft(){
    var bille1 = partie.plateau.billeSelected.vecteur[0];
    var bille2 = partie.plateau.billeSelected.vecteur[1];
    
    return (bille1.equalsCoordinate(bille2.x+1, bille2.y+1) || bille1.equalsCoordinate(bille2.x-1, bille2.y-1));
}

function billeLeft(){
    var n = partie.plateau.billeSelected.vecteur.length;
    var tmp = partie.plateau.billeSelected.vecteur[0];
    var less = 0;
    for(i=1; i < 2; i++){
        if(partie.plateau.billeSelected.vecteur[i] != null){
            if(tmp.y > partie.plateau.billeSelected.vecteur[i].y){
            less = i;
            }
        }
    }
    return partie.plateau.billeSelected.vecteur[less];
}

/**
 * Retourne la bille du milieu
 */
function billeMiddle(){
    var n = partie.plateau.billeSelected.vecteur.length;
    var tmp = partie.plateau.billeSelected.vecteur[0];
    var more = 0, less = 0;
    for(i=1; i < n; i++){
        if(tmp.y < partie.plateau.billeSelected.vecteur[i].y){
            more = i;
        }
        if(tmp.y > partie.plateau.billeSelected.vecteur[i].y){
            less = i;
        }
    }
    middle =  partie.plateau.billeSelected.vecteur[intersection(less, more)];
        
    return middle;
}

function intersection(less, more){
    var result = 0;
    if(less == 0 || more == 0){
        if(less == 1 || more == 1){
            result = 2;
        } else{
            result = 1;
        }
    } else if(less == 1 || more == 1){
        if(less == 0 || more == 2){
            result = 2;
        }
    } else{
        if(less == 0 || more == 0){
            result = 1;
        }
    }
    
    return result;
}

/*****************************************************
 *
 *  Fonctions d'interfaces
 *  
*****************************************************/

/**
 *  Est appeler quand les joueurs sont synchronisés
 */
function setDebutPartie(){
    if(partie.JoueurMe.color == 1){
        partie.isTurn = -1;
    }
    partie.init(); //Initialisation de la partie !
    
    $id('nameP1').innerHTML = partie.JoueurMe.joueur.pseudo;
    $id('nameP2').innerHTML = partie.joueurAdv.joueur.pseudo;
    
    //On affiche le plateau
    hideAll();
    show('whoPlayed');
    show('backgroundPlateau');
    show("hubScore");
    show("plateauPartie");
    show('buttonAbandonner');
    showTurn();
}


/**
 *  Intervient quand la partie se termine (victoire, defaite ou abandon)
 */
function setFinPartie(isGagner, isAbandon){
    hideAll();
    show("finPartie");
    setMessageFinPartie(isGagner, isAbandon);
    span = document.createElement('span');
    if(isGagner){
        $id('finPartie').style.color = "#21a637";
        span.className="glyphicon glyphicon-thumbs-up";
        
    } else{
         $id('finPartie').style.color = "#C9302C";
         span.className="glyphicon glyphicon-thumbs-down";
    }
    $id('finPartieGlyph').appendChild(span);
}

/**
 *  Fonction qui gére les diffèrents messages de fin de partie
 * @param {boolean} isGagner  [[Description]]
 * @param {boolean} isAbandon [[Description]]
 */
function setMessageFinPartie(isGagner, isAbandon){ 
    if(isGagner){ //Vous avez gagné
        $id('finPartieTitre').innerHTML="Victoire !";
        if(isAbandon){  //Vous avez gagnez par abandon
             $id('finPartieMessage').innerHTML="Votre adversaire a abandonné !"; 
        } else{ //Vous avez gagnez normalement
             $id('finPartieMessage').innerHTML="Vous avez gagné "+ (partie.JoueurMe.score + 1) + " - " +  partie.joueurAdv.score;
        }
    } else{
         $id('finPartieTitre').innerHTML="Defaite !";
        if(isAbandon){  //Vous avez gagnez par abandon
             $id('finPartieMessage').innerHTML="Votre avez abandonné !";
        } else{ //Vous avez gagnez normalement
             $id('finPartieMessage').innerHTML="Vous avez perdu "+partie.JoueurMe.score + " - " + (partie.joueurAdv.score + 1);
        }
    }
}

/**
 * Appeler quand le serveur indique un mouvement
 */
function setMouvementBille(origin, destination, isMe = false){
    inversionMatrice(origin);
    inversionMatrice(destination);
    var row, color;
    //On effecture le mouvement demandé
    var plat = $id('plateauPartie').children;
    var valeur = 1;
    
    if(isMe){
    	color = partie.JoueurMe.color;
    } else{
    	valeur *= -1; 
    	color = partie.joueurAdv.color;
    }

    for(i=0; i < 5; i++){
        if(origin.coord[i].x != -1){
            partie.plateau.terrain[origin.coord[i].x][origin.coord[i].y] = 0;
            row = plat[origin.coord[i].x].children;
            setBilleImage(row[determineColDOM(origin.coord[i].x,origin.coord[i].y)], "empty");
        }
    }
    //On bouge les billes
    for(i=0; i < 3; i++){
        if(destination.coord[i].x != -1){
            partie.plateau.terrain[destination.coord[i].x][destination.coord[i].y] = valeur;
            row = plat[destination.coord[i].x].children;
            setBilleImage(row[determineColDOM(destination.coord[i].x,destination.coord[i].y)], "", color);
        }
    }
    
   	if(!isMe){
   		color = partie.JoueurMe.color;
   	}else{
   		color = partie.joueurAdv.color;
   	}
	valeur *= -1; 

    for(i = 3; i < 5; i++){
        if(destination.coord[i].x != -1){
            partie.plateau.terrain[destination.coord[i].x][destination.coord[i].y] = valeur;
            row = plat[destination.coord[i].x].children;
            setBilleImage(row[determineColDOM(destination.coord[i].x,destination.coord[i].y)], "", color);
        }
    }
    
    //on vide la selection
    partie.plateau.billeSelected.init();  //On vide les billes selectionnées.
    partie.plateau.billeMove.init();
    
    if(isMe){
    	sendNextTurn();
    	partie.nextTurn();   //On change de tour
    }
}

function determineColDOM(x, y){
	var tmp, res ;
	if(x <= 4){
		tmp = -0.5 * x + 2;
	} else{
		tmp = 0.5 * x - 2;
	}
    res = (y - (0.5*y+ tmp));
	return res; 
}

/**
 * Affiche le message d'indication pour savoir le tour.
 */
function showTurn() {
    if(partie.isTurn > 0) {
        $id('whoPlayed').children[0].innerHTML = "A vous de jouer !";
        $id('whoPlayed').children[0].style.color = "#21a637";
    } else {
        $id('whoPlayed').children[0].innerHTML = "Votre adversaire joue !";
        $id('whoPlayed').children[0].style.color = "#D9534F";
    }
}


function setScore(pNoir, pBlanc) {
    if(partie.JoueurMe.color == 0) {
        partie.JoueurMe.score = pNoir;
        partie.joueurAdv.score = pBlanc;
        
        $id('scoreP1').innerHTML = pNoir;
        $id('scoreP2').innerHTML = pBlanc;
    } else {
        partie.JoueurMe.score = pBlanc;
        partie.joueurAdv.score = pNoir;
        
        $id('scoreP1').innerHTML = pBlanc;
        $id('scoreP2').innerHTML = pNoir;
    }
}

/**
 * Cache toute l'interface sauf la barre du header
 */
function hideAll() {
    hide('syncPartie');
    hide('whoPlayed');
    hide('backgroundPlateau');
    hide('hubScore');
    hide('plateauPartie');
    hide('finPartie');
    hide('buttonAbandonner');
}

/**
 * Affiche l'image dans l'objet bille. 
 */
function setBilleImage(bille, type, color = -1) {
    if(color != -1) {
        if(type != "")
            bille.style.backgroundImage = "url("+ressource+"style/img/billes/bille_"+getNameColor(color)+"_"+type+".png)";
        else 
            bille.style.backgroundImage = "url(" + ressource +"/style/img/billes/bille_"+getNameColor(color)+".png)";    
    } else 
        bille.style.backgroundImage = "url(" + ressource +"/style/img/bille_"+type+".png)";
}

function setUnauthorized() {
	 $('#noAuthorized').modal('show');
}

/*****************************************************
 *
 *  Fonctions de la gestion du socket de la partie
 *  
*****************************************************/

function sendNextTurn() {
    joueurSocket.server.finTour();
}

/**
 * Le joueur a abandonné
 */
function sendForfait() {
    joueurSocket.server.forfait();
}

/**
 * Envois la demande de mouvement au serveur.
 */ 
function sendMouvement() {
    inversionMatrice();
    var mov = new Mouvements(6);
    mov.init();
    for (i = 0; i < partie.plateau.billeSelected.current; i++) {
            mov.addPos(i, partie.plateau.billeSelected.vecteur[i].x, partie.plateau.billeSelected.vecteur[i].y);
    }
    for(i = 0; i < partie.plateau.billeMove.current; i++){
            mov.addPos(i + 3, partie.plateau.billeMove.vecteur[i].x, partie.plateau.billeMove.vecteur[i].y);
    }
    joueurSocket.server.move(JSON.stringify(mov.coord)); 
}
 
function sendMouvementFormate(vecteur, coord) {
    res = -1
    if(vecteur != null){
        if(coord == 0) //x
            res = vecteur.x;
        else //y
            res = vecteur.y;
    }
    return res;
}
 
/**
 * J'ai bien reçu les informations et je passe la main à l'adversaire.
 */
function sendFinTour() {
    joueurSocket.server.finTour();
}

function sendPartiePlayer() {
    joueurSocket.server.add(partie.JoueurMe.joueur.id, partie.JoueurMe.color);
}


function inversionMatrice(mouv) {
    if(partie.JoueurMe.color == 1){ //si je suis blanc, je dois inverser les lignes et col
        if (mouv == null){
           for(i=0; i < partie.plateau.billeSelected.current; i++){
                if(partie.plateau.billeSelected.vecteur[i].x > -1){
                    partie.plateau.billeSelected.vecteur[i].x = 8 - partie.plateau.billeSelected.vecteur[i].x;
                    partie.plateau.billeSelected.vecteur[i].y = 16 - partie.plateau.billeSelected.vecteur[i].y;
                }
            }
            for(i=0; i < partie.plateau.billeMove.current; i++){
                if(partie.plateau.billeMove.vecteur[i].x > -1){
                    partie.plateau.billeMove.vecteur[i].x = 8 - partie.plateau.billeMove.vecteur[i].x;
                    partie.plateau.billeMove.vecteur[i].y = 16 - partie.plateau.billeMove.vecteur[i].y;
                }
            } 
        } else {
            var i;
            for (i = 0; i < mouv.coord.length; i++){
                if (mouv.coord[i].x > -1) {
                    mouv.coord[i].x = 8 - mouv.coord[i].x;
                    mouv.coord[i].y = 16 - mouv.coord[i].y;
                }
            } 
        }
    }
}

function mouvementToClient(json, origin, destination){
    var parse = JSON.parse(json);
    for(i = 0; i < 5; i++){
        if(i < 3)
            origin.addPos(i, parse.M.Origin[i].X, parse.M.Origin[i].Y);
        else
            origin.addPos(i, parse.Origin[i-3].X, parse.Origin[i-3].Y);
    }
    for(i = 0; i < 5; i++){
        if(i < 3)
            destination.addPos(i, parse.M.Destination[i].X, parse.M.Destination[i].Y);
        else
            destination.addPos(i, parse.Destination[i-3].X, parse.Destination[i-3].Y);
    } 
}