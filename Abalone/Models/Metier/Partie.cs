using System;
using System.Collections.Generic; 

namespace Abalone.Models {
    public class Partie {
	    public static List<Partie> listParties = new List<Partie>(); //Les parties en cours.
	    private static int idManuel = 1;
	    private int uid;
	    private int[][] plateau = null;
	    private int tour = 1; //1=Tour de noir    -1=Tour de blanc
	    private bool peutBouger = true;
	    private int scoreNoir = 0;
	    private int scoreBlanc = 0;
	    private int comboNoir = 0; //Pour l'achievement combo 2 3 4
	    private int comboBlanc = 0;//Pour l'achievement combo 2 3 4
	    private Joueur blanc = null;
	    private Joueur noir = null;
	    private int billeColission = -1;
	    private int couleur;
	    private bool possiblePoint = false;
	
	
    // Constructeurs
    //---------------------------------------------------	
	    public Partie(Joueur j1, Joueur j2) {
		    this.uid = idManuel; idManuel++;
		    this.noir = j1;
		    this.blanc = j2;
		    initPlateau();
	    }
	

    // Getter / Setter
    //---------------------------------------------------	
	    public int Uid {        get { return this.uid; }        set { this.uid = value; }        }
	    public int[][] Plateau {get { return this.plateau; }    set { this.plateau = value; }    }
        public int Tour {       get { return this.tour; }       set { this.tour = value; }       }
        public bool PeutBouger {get { return this.peutBouger; } set { this.peutBouger = value; } }
        public int ScoreNoir {  get { return this.scoreNoir; }  set { this.scoreNoir = value; }  }
        public int ScoreBlanc { get { return this.scoreBlanc; } set { this.scoreBlanc = value; } }
        public Joueur Blanc {   get { return this.blanc; }      set { this.blanc = value; }      }
        public Joueur Noir {    get { return this.noir; }       set { this.noir = value; }       }


    // Méthodes publiques
    //---------------------------------------------------	
        public int GestionMouvement(int couleur, bMove moves, bMoveResp retour){
		    int tmp, res = -1;
		    this.couleur = couleur;
		
		    if(this.peutBouger){ //Si on a déjà bougé une fois ce tour ci on ne peut plus le faire
			    res = validationMouvement(moves, retour); //-1 si non   2 si oui    3 si oui et score +1
			
			    if(res == 3){ //Une bille a  été prise
				    tmp = augmenterScore(couleur); //Si le score atteinds 6, la partie se finit ici.
				    if(tmp >= 0){ //Situation de victoire
					    res = tmp;
				    } else { res = 2; }
			    }
			
			    if(res == 2){ this.peutBouger = false; } //Un seul déplacement par tour.
		    }
		    return res; //-1=non-autorise    0=VictoireNoir    1=VictoireBlanc     2=autorisé
	    }

	    public void Fin(int couleurGagnant, bool estForfait) {		
		    Historique fin = null;
		
		    if(couleurGagnant == 0){ //Noir a gagné
			    fin = new Historique(new DateTime(), this.scoreNoir, this.scoreBlanc, estForfait, this.noir, this.blanc);
			    fin.CreateBDD();
			
			    Achievement.ACV_FIRST_WIN(this.noir); //Trigger aussi les 10 et 100
			    Achievement.ACV_PERFECT(this.noir, this.scoreNoir, this.scoreBlanc);
			    Achievement.ACV_SIX_FIVE(this.noir, this.scoreNoir, this.scoreBlanc);
			    if(estForfait) { Achievement.ACV_SURRENDER(this.noir); }
		    } else { //Blanc a gagné
			    fin = new Historique(new DateTime(), this.scoreBlanc, this.scoreNoir, estForfait, this.blanc, this.noir);
			    fin.CreateBDD();
			
			    Achievement.ACV_FIRST_WIN(this.blanc); //Trigger aussi les 10 et 100
			    Achievement.ACV_PERFECT(this.blanc, this.scoreBlanc, this.scoreNoir);
			    Achievement.ACV_SIX_FIVE(this.blanc, this.scoreBlanc, this.scoreNoir);
			    if(estForfait) { Achievement.ACV_SURRENDER(this.blanc); }
		    }
		    listParties.Remove(this); //The End
	    }
	
	    public static Partie TrouverPartie(Joueur joueur1, Joueur joueur2) {
		    Partie res = null;
		
		    foreach(Partie tmp in listParties){
			    if( (tmp.noir.Equals(joueur1) && tmp.blanc.Equals(joueur2))   ||   (tmp.noir.Equals(joueur2) && tmp.blanc.Equals(joueur1)) ){
				    res = tmp; break;//On ne maitrise pas l'ordre des joueurs, il faut donc vérifier les deux possibilités d'ordre.
			    }
		    }
		    return res;
	    }

        public static Partie TrouverPartie(int uid) {
		    Partie res = null;
		
		    foreach(Partie tmp in listParties){
			    if(tmp.Uid == uid){
				    res = tmp; break;
			    }
		    }
		    return res;
	    }
	    public bool EstSonTour(int couleur) {
		    bool res = false;
		
		    if( (couleur == 0 && this.tour == 1)   ||   (couleur == 1 && this.tour == -1) ){
			    res = true;
		    }
		    return res;
	    }
	
    // Méthode privées
    //---------------------------------------------------	
	    private void initPlateau(){ //Génère le plateau de base, dût à sa forme hexagonale beaucoup de chose sont écrites manuelement.
		    int i, j;
		    this.plateau = new int[9][]; //Est nul par défaut //Voir dossier sur le pourquoi 17*9
            for(i=0; i< this.plateau.Length; i++) {
                this.plateau[i] = new int[17];
            } //int[,] tab = new int[9, 17];

            for (i=0; i<9; i++){
			    for(j=0; j<17; j++){
				    this.plateau[i][j] = -99; //Les cases non parcourables sont -99
			    }
		    }
		
		    /*Ligne 1*/ for(i=4; i<=12; i=i+2) {    this.plateau[0][i] =  1;	} //Sur la première ligne toutes les billes sont blanches
		    /*Ligne 2*/ for(i=3; i<=13; i=i+2) {    this.plateau[1][i] =  1;    } //Sur la deuxième aussi.
		    /*Ligne 3*/ for(i=6; i<=10; i=i+2) {    this.plateau[2][i] =  1;    } //Les autres cases sont blanches
					    this.plateau[2][2]  = -1; 
					    this.plateau[2][4]  = -1; 
					    this.plateau[2][12] = -1; 
					    this.plateau[2][14] = -1; //Ce sont les 4 seules cases vides de la ligne 3
		    /*Ligne 4*/ for(i=1; i<=15; i=i+2) {    this.plateau[3][i] = -1;    } //Aucune bille ici
		    /*Ligne 5*/ for(i=0; i<=16; i=i+2) {    this.plateau[4][i] = -1;    } //Aucune bille ici
		    /*Ligne 6*/ for(i=1; i<=15; i=i+2) {    this.plateau[5][i] = -1;    } //Aucune bille ici
		    /*Ligne 7*/ this.plateau[6][2]  = -1; //Ce sont les 4 seules cases vides de la ligne 7
					    this.plateau[6][4]  = -1; 
					    this.plateau[6][12] = -1; 
					    this.plateau[6][14] = -1; 
					    for(i=6; i<=10; i=i+2) {    this.plateau[6][i] =  0;    } //Les autres cases sont noires
		    /*Ligne 8*/ for(i=3; i<=13; i=i+2) {    this.plateau[7][i] =  0;    } //Toutes les billes de la ligne 8 sont noires
		    /*Ligne 9*/ for(i=4; i<=12; i=i+2) {    this.plateau[8][i] =  0;	} //Pareil pour la 9
		
		    //-99 = case invallide    -1 = Aucune bille     0 = Bille noire     1=Bille blanche
	    }

	    private int augmenterScore(int i){ //0=noir+1   1=blanc+1
		    int res = -1;
		
		    if(i == 0){
			    this.scoreNoir++;
			    this.comboNoir++;
			    this.comboBlanc = 0;
			    Achievement.ACV_COMBO_2(this.noir, this.comboNoir); //Gere tous les autres combos
			    if(this.scoreNoir  >= 6){  Fin(0, false); res=0;  }
		    } else {
			    this.scoreBlanc++;
			    this.comboBlanc++;
			    this.comboNoir = 0;
			    Achievement.ACV_COMBO_2(this.blanc, this.comboBlanc); //Gere tous les autres combos
			    if(this.scoreBlanc >= 6){  Fin(1, false); res=1;}
		    }
		    return res; //0=noirGagne   1=BlancGagne
	    }
	
	    private int validationMouvement(bMove moves, bMoveResp r){ 
		    int res = -1;
		    int nbrBille = 0;
		    int xDirection, yDirection, nbrBillesEnnemi = 0;
		
		    if(isColission(moves)){ //On rencontre une bille ennemie sur notre chemin
			
			    nbrBille = nbrBilles(moves); //Nombre de bille avec moi
			    if(nbrBille > 1){
				    if(isVerticalRight(moves.Ori_x1, moves.Ori_y1, moves.Ori_x2, moves.Ori_y2)){ //Verticale droite
					    if(getBilleOX(moves,this.billeColission) > getBilleDX(moves, this.billeColission)){ //je remonte sur le plateau
						    xDirection = -1;
						    yDirection = 1;
					    } else{ //je descend
						    xDirection = 1;
						    yDirection = -1;
					    }
				    } else if(isVerticalLeft(moves.Ori_x1, moves.Ori_y1, moves.Ori_x2, moves.Ori_y2)){//Verticale gauche 
					    if(getBilleOX(moves,this.billeColission) > getBilleDX(moves, this.billeColission)){ //je remonte sur le plateau
						    xDirection = -1;
						    yDirection = -1;
					    } else{ //je descend
						    xDirection = 1;
						    yDirection = 1;
					    } 
				    }else{ //Hozirontale
					    if(getBilleOY(moves,this.billeColission) < getBilleDY(moves, this.billeColission)){ //je vais vers à la droite
						    xDirection = 0;
						    yDirection = 2;
					    } else{ //je vais vers la gauche
						    xDirection = 0;
						    yDirection = -2;
					    } 
				    }
				
				    nbrBillesEnnemi = nbrBillesEnnemie(moves,xDirection, yDirection); //nombre de bille ennemie sur le chemin
				    if( nbrBillesEnnemi < nbrBille){ //si elle sont en infériorité numérique je pousse
					    setCoordonneeResp(moves, r, nbrBillesEnnemi, xDirection, yDirection);
					    if(this.possiblePoint){ //Je gagne un point
						    res = 3;
					    }else{
						    res = 2;
					    }
				    } else{ //Je peux pas pousser, envoyer le packet de refus
					    res = -1;
				    }
			    } else{
				    res = -1;
			    }
		    }else{
			    r.M = moves;
			    res = 2;
			    updateBillePlateau(moves);
		    }
		
		    this.billeColission = 0;
		    this.possiblePoint = false;
		    return res;
	    }
	
	    private void setCoordonneeResp (bMove moves, bMoveResp r, int nbrBille ,int xDirection, int yDirection){ //Déplace les billes poussées
		    int i, tmpX, tmpY;
		
		    r.M = moves;
		    tmpX = getBilleDX(moves, this.billeColission);
		    tmpY = getBilleDY(moves, this.billeColission);
		
		    for(i=0; i < nbrBille; i++){
			    tmpX += xDirection;
			    tmpY += yDirection;
			    if((tmpX > -1 && tmpX < 9) && (tmpY > -1 && tmpX < 17)){
				    if(this.plateau[tmpX][tmpY] != -99){
					    this.plateau[tmpX][tmpY] = (this.couleur+1)%2;
					    if(i == 0){
						    r.Des_x4 = tmpX;
						    r.Des_y4 = tmpY;
					    } else {
						    r.Des_x5 = tmpX;
						    r.Des_y5 = tmpY;
					    }
				    }
			    }
		    }
		    //Met à jour le reste du plateau
		    updateBillePlateau(moves);
	    }

        private void updateBillePlateau(bMove moves){ 
		    if(moves.Ori_x1 > -1 && moves.Ori_y1 > -1){
			    this.plateau[moves.Ori_x1][moves.Ori_y1] = -1;
		    }
		    if(moves.Ori_x2 > -1 && moves.Ori_y2 > -1){
			    this.plateau[moves.Ori_x2][moves.Ori_y2] = -1;
		    }
		    if(moves.Ori_x3 > -1 && moves.Ori_y3 > -1){
			    this.plateau[moves.Ori_x3][moves.Ori_y3] = -1;
		    }
		
		    if(moves.Des_x1 > -1 && moves.Des_y1 > -1){
			    this.plateau[moves.Des_x1][moves.Des_y1] = this.couleur;
		    }
		    if(moves.Des_x2 > -1 && moves.Des_y2 > -1){
			    this.plateau[moves.Des_x2][moves.Des_y2] = this.couleur;
		    }
		    if(moves.Des_x3 > -1 && moves.Des_y3 > -1){
			    this.plateau[moves.Des_x3][moves.Des_y3] = this.couleur;
		    }
	    }
	
	    private int nbrBillesEnnemie(bMove moves, int xDirection, int yDirection){ 
		    int i, j, nbr = 1;
		    bool stop = false;
		
		    i = getBilleDX(moves, this.billeColission);
		    j = getBilleDY(moves, this.billeColission);
		    i += xDirection;
		    j += yDirection;
		
		    while(nbr < 3 && i > -1 &&  i < 9 && j > -1 && j < 17 && !stop && this.plateau[i][j] != -1){
			    if(this.plateau[i][j] == -99){
				    stop = true;
			    } else if(this.plateau[i][j] == this.couleur){
				    nbr = 10;
			    } else if(this.plateau[i][j] == (this.couleur+1)%2){
				    nbr++;
			    }
			    i += xDirection;
			    j += yDirection;
		    }
		    if(!stop)
		    {
			    if(i < 0 || i > 8 || j < 0 || j > 16){
				    stop = true;
			    }
		    }
		    this.possiblePoint = stop;
		
		    return nbr;
	    }
	
	    private int nbrBilles(bMove moves){ 
		    int nbr = 0;
		    if(moves.Ori_x1 > -1 && moves.Ori_y1 > -1){
			    nbr++;
		    }
		    if(moves.Ori_x2 > -1 && moves.Ori_y2 > -1){
			    nbr++;
		    }
		    if(moves.Ori_x3 > -1 && moves.Ori_y3 > -1){
			    nbr++;
		    }
		    return nbr;
	    }

        private int getBilleDX(bMove moves, int bille){ 
		    int x;
		    if(bille == 1){
			    x = moves.Des_x1;
		    } else if(bille == 2){
			    x = moves.Des_x2;
		    } else{
			    x = moves.Des_x3;
		    }
		    return x;
	    }

        private int getBilleDY(bMove moves, int bille){ 
		    int y;
		    if(bille == 1){
			    y = moves.Des_y1;
		    } else if(bille == 2){
			    y = moves.Des_y2;
		    } else{
			    y = moves.Des_y3;
		    }
		    return y;
	    }
	
	    private int getBilleOY(bMove moves, int bille){ 
		    int y;
		    if(bille == 1){
			    y = moves.Ori_y1;
		    } else if(bille == 2){
			    y = moves.Ori_y2;
		    } else{
			    y = moves.Ori_y3;
		    }
		    return y;
	    }
	
	    private int getBilleOX(bMove moves, int bille){ 
		    int x;
		    if(bille == 1){
			    x = moves.Ori_x1;
		    } else if(bille == 2){
			    x = moves.Ori_x2;
		    } else{
			    x = moves.Ori_x3;
		    }
		    return x;
	    }

        private bool isColission(bMove moves){ 
		    bool colission = false;
		
		    if(isColissionBille(moves.Des_x1,moves.Des_y1)){
			    this.billeColission = 1;
			    colission = true;
		    } else if(isColissionBille(moves.Des_x2, moves.Des_y2)){
			    this.billeColission = 2;
			    colission = true;
		    } else if(isColissionBille(moves.Des_x3, moves.Des_y3)){
			    this.billeColission = 3;
			    colission = true;
		    }
		    return colission;
	    }

        private bool isColissionBille(int x, int y){
		    bool colission = false;
		    if(x > -1 && y > -1){
			    if(this.plateau[x][y] == (this.couleur+1)%2){ 
				    colission = true;
			    }
		    }
		    return colission;
	    }

        private bool equalsCoordinate(int x1, int y1, int x2, int y2){
		    return (equalsX(x1,x2) && equalsY(y1, y2));
	    }

        private bool equalsY(int y1, int y2){
		    return y1 == y2;
	    }

        private bool equalsX(int x1, int x2){
		    return x1 == x2;
	    }

        private bool isVerticalRight(int x1, int y1, int x2, int y2){
		    return(equalsCoordinate(x1, y1, x2-1, y2+1) || equalsCoordinate(x1, y1, x2+1, y2-1));
	    }

        private bool isVerticalLeft(int x1, int y1, int x2, int y2){
		    return(equalsCoordinate(x1, y1, x2+1, y2+1) || equalsCoordinate(x1, y1, x2-1, y2-1));
	    }	

	
    // Debug
    //---------------------------------------------------
	    public override string ToString(){
		    return "Partie [uid=" + uid + ", plateau=" + plateau.ToString() + ", tour=" + tour + ", peutBouger="
				    + peutBouger + ", scoreNoir=" + scoreNoir + ", scoreBlanc=" + scoreBlanc + ", comboNoir=" + comboNoir
				    + ", comboBlanc=" + comboBlanc + ", blanc=" + blanc + ", noir=" + noir + "]";
	    }
    }
}