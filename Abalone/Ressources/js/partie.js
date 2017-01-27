var partieSocket;
var partie;

function initPartie(uid, pseudoJNoir,emailJNoir, pseudoJBlanc, emailJBlanc, emailJCurrent){        
        //On veut communiquer avec le socket
        partieSocket = new WebSocket("ws://localhost:8080/Abalone/partieSocket");
        partieSocket.onmessage = onMessagePartie;
         
        //Pour une utilisation correcte, on vire le responsive.
        $id('corps').style.width="1000px";
    
        //On inialise l'interface
        initInterface(uid, pseudoJNoir,emailJNoir, pseudoJBlanc, emailJBlanc, emailJCurrent);
}


function initInterface(uid, pseudoJNoir,emailJNoir, pseudoJBlanc,emailJBlanc, emailJCurrent){
    hideAll();
    show("syncPartie");
    
    joueur1 = new JoueurPartie(new Joueur(uid,pseudoJNoir, emailJNoir),0, 0);
    joueur2 = new JoueurPartie(new Joueur(uid,pseudoJBlanc, emailJBlanc),0, 1); 
    
    if(joueur1.joueur.email == emailJCurrent){
        partie = new Partie(joueur1, joueur2);
    } else{
        partie = new Partie(joueur2, joueur1);
    }
    partieSocket.onopen = function(){
    	 sendPartiePlayer();
    }
}


/*****************************************************
 *
 *  Fonctions qui gère la partie
 *  
*****************************************************/


/**
 * Objet partie qui gère les joueurs / score / plateau
 * @param {JoueurPartie} JoueurMe  Joueur hote
 * @param {JoueurPartie} joueurAdv Joueur adverse
 */
function Partie(JoueurMe, joueurAdv){
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
 * [[Description]]
 * @param {Joueur} joueur [[Description]]
 * @param {int} score  [[Description]]
 * @param {int} color  [0 : noir, 1 : blanc]
 */
function JoueurPartie(joueur, score, color){
    this.joueur = joueur;
    this.score = score;
    this.color = color;
}

/**
 * Objet Bille
 * @param   {int} x [[Description]]
 * @param   {int} y [[Description]]
 */
function Bille(x, y) {
    this.x = x;
    this.y = y;
    
    this.equals = function(bille){
        return (this.x == bille.x && this.y == bille.y);
    }
    this.equalsX = function (x){
        return this.x == x;
    }
    this.equalsY = function (y){
        return this.y == y;
    }
    this.equalsCoordinate = function(x, y){
        return (this.x == x && this.y == y);
    }
}

/**
 * Object Plateay qui gère la position des billes et les dèplacements.
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
    
    
    //Initialise le vecteur, -1 par défaut
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
    
    //Ajoute la bille selectionnée au vecteur
    this.addBille = function(x, y){
        if(this.current < this.vecteur.length){
            this.vecteur[this.current] = new Bille(x,y);
            this.current++;
        }
    }
    
    //Retire la bille demandé du vecteur
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
            //On indique la valeur par défault
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

function redirectionMenu(){
    document.location.href="menu.html";
}

/**
 * Intervient quand je clique sur une bille
 * @param {span}   bille [[Description]]
 * @param {int} x     [[Description]]
 * @param {int} y     [[Description]]
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
 * @param {span}   bille [[Description]]
 * @param {int} x     [[Description]]
 * @param {int} y     [[Description]]
 */
function survolBille(bille, x, y){
    if(partie.isTurn  > 0){ //Si c'est mon tour
        if(partie.plateau.terrain[x][y] == 0){ //Aucune bille
            setBilleImage(bille, "hover");
        } else if(partie.plateau.terrain[x][y] == 1){ //Bille à moi
            setBilleImage(bille, "hover",partie.JoueurMe.color);
        }
    } 
}

/**
 * Intervient quand je ne survole plus une bille
 * @param {span}   bille [[Description]]
 * @param {int} x     [[Description]]
 * @param {int} y     [[Description]]
 */
function unsurvolBille(bille, x, y){
   if(partie.isTurn > 0){ //Si c'est mon tour
        if(partie.plateau.terrain[x][y] == 0){ //Aucune bille
            setBilleImage(bille, "empty");
        } else if(partie.plateau.terrain[x][y] == 1){ //Bille à moi
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
 * [[Description]]
 * @param {Bille} bille      [[Description]]
 * @param {int} x          [[Description]]
 * @param {int} y          [[Description]]
 * @param {boolean} isSelected [[Description]]
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
 * @param   {Bille} bille Objet HTML
 * @param   {int} x    Coordonée X de la bille 
 * @param   {int} y    Coordonée Y de la bille 
 * @returns {boolean}  [[Description]]
 */
function isSelectBille(x, y){
    var nbr = partie.plateau.billeSelected.current;
    if(nbr == 1){
        var bille = partie.plateau.billeSelected.vecteur[0];
        if(bille.equalsCoordinate(x, y+2)){
            return true;
        } 
        if(bille.equalsCoordinate(x, y-2)){
            return true;
        }
        if(bille.equalsCoordinate(x+1, y)){
            return true;
        }
        if(bille.equalsCoordinate(x-1, y)){
            return true;
        }
        if(bille.equalsCoordinate(x+1, y-1)){
            return true;
        }
        if(bille.equalsCoordinate(x+1, y+1)){
            return true;
        }
        if(bille.equalsCoordinate(x-1, y-1)){
            return true;
        }
        if(bille.equalsCoordinate(x-1, y+1)){
            return true;
        }
    } else if(nbr == 2){
        var bille1 = partie.plateau.billeSelected.vecteur[0];
        var bille2 = partie.plateau.billeSelected.vecteur[1];
        if(bille1.equalsX(bille2.x)){ //je suis sur la même ligne
            if(bille1.equalsCoordinate(x,y+2) || bille1.equalsCoordinate(x,y-2) || bille2.equalsCoordinate(x,y+2) || bille2.equalsCoordinate(x,y-2)){ //si je suis à gauche ou à droite
                return true;
            }
        }else if(bille1.equalsCoordinate(bille2.x-1, bille2.y+1) || bille1.equalsCoordinate(bille2.x+1, bille2.y-1)){ //Je suis à la verticale vers la gauche
            if(bille1.equalsCoordinate(x-1, y+1) || bille1.equalsCoordinate(x+1, y-1) || bille2.equalsCoordinate(x-1, y+1) || bille2.equalsCoordinate(x+1, y-1) ){
                return true;
            }
            
        } else if(bille1.equalsCoordinate(bille2.x+1, bille2.y+1) || bille1.equalsCoordinate(bille2.x-1, bille2.y-1)){ //Je suis à la verticale vers la droite
            if(bille1.equalsCoordinate(x+1, y+1) || bille1.equalsCoordinate(x-1, y-1) || bille2.equalsCoordinate(x+1, y+1) || bille2.equalsCoordinate(x-1, y-1)){
                return true;
            }
        }
    }
    return false;
}

/**
 * [[Description]]
 * @param   {int} x [[Description]]
 * @param   {int} y [[Description]]
 * @returns {boolean}  [[Description]]
 */
function checkMouvement(x, y){
    var nbr = partie.plateau.billeSelected.current;
    var vecteur = partie.plateau.billeSelected.vecteur;
    if( nbr == 1){ //J'ai une bille à déplacer
        if(isSelectBille(x,y)){
            partie.plateau.billeMove.addBille(x,y);
            return true;
        }
    } else if(nbr == 2){  //Si j'ai deux billes à déplacer
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
         } else{ //Je suis en horizontale
            if(middle.equalsX(x+1) || middle.equalsX(x-1)){ //Si je me déplace horizontalement
                var nbr = 0;
                
                if(middle.equalsCoordinate(x+1,y+1) && isNeighbourTopOrBottom(vecteur, true) == 2){ //Déplacement vers le haut gauche
                    partie.plateau.billeMove.addBille(vecteur[0].x-1, vecteur[0].y-1);
                    partie.plateau.billeMove.addBille(vecteur[1].x-1, vecteur[1].y-1);
                    
                    return true;
                } else if(middle.equalsCoordinate(x+1,y-1) && isNeighbourTopOrBottom(vecteur, true, false) == 2){ //Déplacement vers le haut droite
                	partie.plateau.billeMove.addBille(vecteur[0].x-1, vecteur[0].y+1);
                    partie.plateau.billeMove.addBille(vecteur[1].x-1, vecteur[1].y+1);
                    return true;
                }  else if(middle.equalsCoordinate(x-1,y-1) && isNeighbourTopOrBottom(vecteur, false) == 2){//Déplacement vers le bas droite
                    partie.plateau.billeMove.addBille(vecteur[0].x+1, vecteur[0].y+1);
                    partie.plateau.billeMove.addBille(vecteur[1].x+1, vecteur[1].y+1);
                    return true;
                }  else if(middle.equalsCoordinate(x-1,y+1) && isNeighbourTopOrBottom(vecteur, false, false) == 2){//Déplacement vers le bas gauche
                    partie.plateau.billeMove.addBille(vecteur[0].x+1, vecteur[0].y-1);
                    partie.plateau.billeMove.addBille(vecteur[1].x+1, vecteur[1].y-1);
                    return true;
                } 
            } else if(middle.equalsX(x)){ //déplacement gauche ou droite sur la même ligne
                if(isNeighbourY(vecteur[0], y) ||isNeighbourY(vecteur[1], y)){ //Elle est voisine à une de mes billes
                    if(middle.y > y){ //Je me déplace vers ma gauche
                        partie.plateau.billeMove.addBille(vecteur[0].x, vecteur[0].y-2);
                        partie.plateau.billeMove.addBille(vecteur[1].x, vecteur[1].y-2);
                    } else{ //Je me déplace vers ma droite
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
            		if(partie.plateau.terrain[vecteur[i].x-1][vecteur[i].y-1] == 0){
                        nbr++;
                    }
            	} else{
            		if(partie.plateau.terrain[vecteur[i].x-1][vecteur[i].y+1] == 0){
                        nbr++;
                    }
            	}
                
            } else{
            	if(isLeft){
            		if(partie.plateau.terrain[vecteur[i].x+1][vecteur[i].y+1] == 0){
                        nbr++;
                    }
            	} else{
            		if(partie.plateau.terrain[vecteur[i].x+1][vecteur[i].y-1] == 0){
                        nbr++;
                    }
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
 * @returns {boolean} [[Description]]
 */
function isVerticalRight(){
    var bille1 = partie.plateau.billeSelected.vecteur[0];
    var bille2 = partie.plateau.billeSelected.vecteur[1];
    
    return (bille1.equalsCoordinate(bille2.x-1, bille2.y+1) || bille1.equalsCoordinate(bille2.x+1, bille2.y-1));
}

/**
 * Retourne vrai si les deux billes indique un mouvement vertical vers la droite
 * @returns {[[Type]]} [[Description]]
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
 * @param   {int} x [[Description]]
 * @param   {int} y [[Description]]
 * @returns {boolean} [[Description]]
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
 * @param {object} json [[Description]]
 */
function setDebutPartie(json){
    if(partie.JoueurMe.color == 1){
        partie.isTurn = -1;
    }
    partie.init(); //Initialisation de la partie !
    
    $id('nameP1').innerHTML = partie.JoueurMe.joueur.pseudo;
    $id('nameP2').innerHTML = partie.joueurAdv.joueur.pseudo;
    //setScore(json); 
    
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
 * @param {boolean} isGagner  [[Description]]
 * @param {boolean} isAbandon [[Description]]
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
 *  Fonction qui gère les différents messages de fin de partie
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
 * @param {json} json Json reçu par le serveur
 */
function setMouvementBille(json, isMe = false){
    inversionMatrice(json);
    var row, color;
    //On effecture le mouvement demandé
    var plat = $id('plateauPartie').children;
    var valeur = 1;
    var color;
    
    if(isMe){
    	color = partie.JoueurMe.color;
    } else{
    	valeur *= -1; 
    	color = partie.joueurAdv.color;
    }
    
    if(json.ori_x1 != -1){
        partie.plateau.terrain[json.ori_x1][json.ori_y1] = 0;
        row = plat[json.ori_x1].children;
        setBilleImage(row[determineColDOM(json.ori_x1,json.ori_y1)], "empty");
    }
    if(json.ori_x2 != -1){
        partie.plateau.terrain[json.ori_x2][json.ori_y2] = 0;
        row = plat[json.ori_x2].children;
        setBilleImage(row[determineColDOM(json.ori_x2,json.ori_y2)], "empty");
    }
    if(json.ori_x3 != -1){
        partie.plateau.terrain[json.ori_x3][json.ori_y3] = 0;
        row = plat[json.ori_x3].children;
        setBilleImage(row[determineColDOM(json.ori_x3,json.ori_y3)], "empty");
    }
    
   
    //On bouge les billes
    if(json.des_x1 != -1){
        partie.plateau.terrain[json.des_x1][json.des_y1] = valeur;
        row = plat[json.des_x1].children;
        setBilleImage(row[determineColDOM(json.des_x1,json.des_y1)], "", color);
    }
    if(json.des_x2 != -1){
        partie.plateau.terrain[json.des_x2][json.des_y2] = valeur;
        row = plat[json.des_x2].children;
        setBilleImage(row[determineColDOM(json.des_x2,json.des_y2)], "", color);
    }
    if(json.des_x3 != -1){
        partie.plateau.terrain[json.des_x3][json.des_y3] = valeur;
        row = plat[json.des_x3].children;
        setBilleImage(row[determineColDOM(json.des_x3,json.des_y3)], "", color);
    }
    
   
   	if(!isMe){
   		color = partie.JoueurMe.color;
   	}else{
   		color = partie.joueurAdv.color;
   	}
	valeur *= -1; 
    if(json.des_x4 != -1){
        partie.plateau.terrain[json.des_x4][json.des_y4] = valeur;
        row = plat[json.des_x4].children;
        setBilleImage(row[determineColDOM(json.des_x4,json.des_y4)], "", color);
    }
    if(json.des_x5 != -1){
        partie.plateau.terrain[json.des_x5][json.des_y5] = valeur;
        row = plat[json.des_x5].children;
        setBilleImage(row[determineColDOM(json.des_x5,json.des_y5)], "", color);
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
function showTurn(){
    if(partie.isTurn > 0){
        $id('whoPlayed').children[0].innerHTML = "A vous de jouer !";
        $id('whoPlayed').children[0].style.color = "#21a637";
    } else{
        $id('whoPlayed').children[0].innerHTML = "Votre adversaire joue !";
        $id('whoPlayed').children[0].style.color = "#D9534F";
    }
}

/**
 * Affiche le score à l'écran.
 * @param {object} json [[Description]]
 */
function setScore(json){
    if(partie.JoueurMe.color == 0){
        partie.JoueurMe.score = json.pNoir;
        partie.joueurAdv.score = json.pBlanc;
        
        $id('scoreP1').innerHTML = json.pNoir;
        $id('scoreP2').innerHTML = json.pBlanc;
    } else{
        partie.JoueurMe.score = json.pBlanc;
        partie.joueurAdv.score = json.pNoir;
        
        $id('scoreP1').innerHTML = json.pBlanc;
        $id('scoreP2').innerHTML = json.pNoir;
    }
}

/**
 * Cache toute l'interface sauf la barre du header
 */
function hideAll(){
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
 * @param {object}   bille       [[Description]]
 * @param {string} type        [[Description]]
 * @param {int} isColor = -1 [[Description]]
 */
function setBilleImage(bille, type, color = -1){
    if(color != -1){
        if(type != ""){
             bille.style.backgroundImage="url(ressources/style/img/billes/bille_"+getNameColor(color)+"_"+type+".png)";
        } else{
            bille.style.backgroundImage="url(ressources/style/img/billes/bille_"+getNameColor(color)+".png)";
        }
       
    } else{
         bille.style.backgroundImage = "url(ressources/style/img/bille_"+type+".png)";
    }
}

function setUnauthorized(){
	 $('#noAuthorized').modal('show');
}

/*****************************************************
 *
 *  Fonctions de la gestion du socket de la partie
 *  
*****************************************************/


/**
 * Gère les messages que le serveur m'envoit
 * @param {object} event Json reçu par le socket
 */
function onMessagePartie(event) {
    var json = JSON.parse(event.data);
    
    switch(json.action){
        case "pret":{      //Synchronisaiton terminée
            setDebutPartie(json);
            break;
        }
        case "allowed":{    //Mouvement autorisé
            setMouvementBille(json, true);
            setScore(json);
            break;
        }    
        case "unallowed":{  //Mouvement pas autorisé
        	inversionMatrice();
        	partie.plateau.billeMove.init();   
        	resetBilleSelected();
        	setUnauthorized();
        	break;
        } 
        case "move":{  //Mouvement de l'adversaire
            setMouvementBille(json);
            setScore(json);
            break;
        }
        case "surrend":{    //Abandon de l'adversaire
            setFinPartie(true, true);
            break;
        }
        case "timeout":{    //L'adversaire à quitter violament 
            setFinPartie(true, true);
            break;
        }  
        case "beginTurn":{
        	partie.nextTurn();
        	break;
        }
        case "victoire":{    //Victoire
            //Vérifie si je suis la couleur gagnante.
            if(json.gagnant ==  partie.JoueurMe.color){
                setFinPartie(true, false);
            }else{
                 setFinPartie(false, false);
            }
            break;
        }
    }
}

function sendNextTurn(){
    var json = {
        action: "finTour"
    };
    partieSocket.send(JSON.stringify(json));
}

/**
 * Le joueur a abandonné
 */
function sendForfait(){
    var json = {
        action: "forfait"
    };
    partieSocket.send(JSON.stringify(json));
}

/**
 * Envois la demande de mouvement au serveur.
 */
function sendMouvement(){
    inversionMatrice();
    var json = {
        action: "move",   
        ori_x1 : sendMouvementFormate(partie.plateau.billeSelected.vecteur[0],0),
        ori_y1 : sendMouvementFormate(partie.plateau.billeSelected.vecteur[0],1),
        des_x1 : sendMouvementFormate(partie.plateau.billeMove.vecteur[0],0),
        des_y1 : sendMouvementFormate(partie.plateau.billeMove.vecteur[0],1),
        ori_x2 : sendMouvementFormate(partie.plateau.billeSelected.vecteur[1],0),
        ori_y2 : sendMouvementFormate(partie.plateau.billeSelected.vecteur[1],1),
        des_x2 : sendMouvementFormate(partie.plateau.billeMove.vecteur[1],0),
        des_y2 : sendMouvementFormate(partie.plateau.billeMove.vecteur[1],1),
        ori_x3 : sendMouvementFormate(partie.plateau.billeSelected.vecteur[2],0),
        ori_y3 : sendMouvementFormate(partie.plateau.billeSelected.vecteur[2],1),
        des_x3 : sendMouvementFormate(partie.plateau.billeMove.vecteur[2],0),
        des_y3 : sendMouvementFormate(partie.plateau.billeMove.vecteur[2],1),
        des_x4 : -1,
        des_y4 : -1,
        des_x5 : -1,
        des_y5 : -1
    };
    partieSocket.send(JSON.stringify(json));
}

function sendMouvementFormate(vecteur, coord){
    res = -1
    if(vecteur != null){
        if(coord == 0){ //x
            res = vecteur.x;
        }else{ //y
            res = vecteur.y;
        }
    }
    return res;
}
 
/**
 * J'ai bien reçu les informations et je passe la main à l'adversaire.
 */
function sendFinTour(){
    var json = {
        action: "finTour"
    };
    partieSocket.send(JSON.stringify(json));
}

function sendPartiePlayer(){
    var json = {
            action: "add",
            uid : partie.JoueurMe.joueur.id,
            couleur : partie.JoueurMe.color
      };
      partieSocket.send(JSON.stringify(json));
}


function inversionMatrice(json = null){
    if(partie.JoueurMe.color == 1){ //si je suis blanc, je dois inverser les lignes et col
        if(json == null){
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
        } else{
            if(json.ori_x1 > -1){
                json.ori_x1 = 8 -  json.ori_x1;
                json.ori_y1 = 16 -  json.ori_y1;
            }
            if(json.ori_x2 > -1){
                json.ori_x2 = 8 -  json.ori_x2;
                json.ori_y2 = 16 -  json.ori_y2;
            }
            if(json.ori_x3 > -1){
                json.ori_x3 = 8 -  json.ori_x3;
                 json.ori_y3 = 16 -  json.ori_y3;
            }
            if(json.des_x1 > -1){
                json.des_x1 = 8 -  json.des_x1;
                json.des_y1 = 16 -  json.des_y1;
            }
           if(json.des_x2 > -1){
                 json.des_x2 = 8 -  json.des_x2;
                 json.des_y2 = 16 -  json.des_y2;
            }
           if(json.des_x3 > -1){
                 json.des_x3 = 8 -  json.des_x3;
                 json.des_y3 = 16 -  json.des_y3;
            }
           if(json.des_x4 > -1){
                json.des_x4 = 8 -  json.des_x4;
                 json.des_y4 = 16 -  json.des_y4;
            }
            if(json.des_x5 > -1){
                json.des_x5 = 8 -  json.des_x5;
                json.des_y5 = 16 -  json.des_y5;
            }   
        }
    }
}
