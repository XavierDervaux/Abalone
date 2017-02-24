using Abalone.Models.Bean;
using Abalone.Models.Metier;
using System;
using System.Collections.Generic; 

namespace Abalone.Models.Metier { 
    public class Partie {
	    public static List<Partie> ListParties = new List<Partie>(); //Les parties en cours.
	    private static int _idManuel = 1;
	    private int _comboNoir = 0; //Pour l'achievement combo 2 3 4
	    private int _comboBlanc = 0;//Pour l'achievement combo 2 3 4
	    private int _billeColission = -1;
	    private int _couleur;
	    private bool _possiblePoint = false;
	
	
    // Constructeurs
    //---------------------------------------------------	
	    public Partie(Joueur j1, Joueur j2) {
		    this.Uid = _idManuel; _idManuel++;
		    this.Noir = j1;
		    this.Blanc = j2;
		    InitPlateau();
	    }
	

    // Getter / Setter
    //---------------------------------------------------	
	    public int Uid { get; set; }
        public int[][] Plateau { get; set; } = null;
        public int Tour { get; set; } = 1;
        public bool PeutBouger { get; set; } = true;
        public int ScoreNoir { get; set; } = 0;
        public int ScoreBlanc { get; set; } = 0;
        public Joueur Blanc { get; set; } = null;
        public Joueur Noir { get; set; } = null;


    // Méthodes publiques
    //---------------------------------------------------	
        public int GestionMouvement(int couleur, bMove moves, BMoveResp retour){
		    int tmp, res = -1;
		    this._couleur = couleur;
		
		    if(this.PeutBouger){ //Si on a déjà bougé une fois ce tour ci on ne peut plus le faire
			    res = ValidationMouvement(moves, retour); //-1 si non   2 si oui    3 si oui et score +1
			
			    if(res == 3){ //Une bille a  été prise
				    tmp = AugmenterScore(couleur); //Si le score atteinds 6, la partie se finit ici.
				    if(tmp >= 0){ //Situation de victoire
					    res = tmp;
				    } else { res = 2; }
			    }
			
			    if(res == 2){ this.PeutBouger = false; } //Un seul déplacement par tour.
		    }
		    return res; //-1=non-autorise    0=VictoireNoir    1=VictoireBlanc     2=autorisé
	    }

	    public void Fin(int couleurGagnant, bool estForfait) {		
		    Historique fin = null;
		
		    if(couleurGagnant == 0){ //Noir a gagné
			    fin = new Historique(new DateTime(), this.ScoreNoir, this.ScoreBlanc, estForfait, this.Noir, this.Blanc);
			    fin.CreateBDD();
			
			    Achievement.ACV_FIRST_WIN(this.Noir); //Trigger aussi les 10 et 100
			    Achievement.ACV_PERFECT(this.Noir, this.ScoreNoir, this.ScoreBlanc);
			    Achievement.ACV_SIX_FIVE(this.Noir, this.ScoreNoir, this.ScoreBlanc);
			    if(estForfait) { Achievement.ACV_SURRENDER(this.Noir); }
		    } else { //Blanc a gagné
			    fin = new Historique(new DateTime(), this.ScoreBlanc, this.ScoreNoir, estForfait, this.Blanc, this.Noir);
			    fin.CreateBDD();
			
			    Achievement.ACV_FIRST_WIN(this.Blanc); //Trigger aussi les 10 et 100
			    Achievement.ACV_PERFECT(this.Blanc, this.ScoreBlanc, this.ScoreNoir);
			    Achievement.ACV_SIX_FIVE(this.Blanc, this.ScoreBlanc, this.ScoreNoir);
			    if(estForfait) { Achievement.ACV_SURRENDER(this.Blanc); }
		    }
		    ListParties.Remove(this); //The End
	    }

        public static Partie TrouverPartie(Joueur joueur1, Joueur joueur2) {
		    Partie res = null;
		
		    foreach(Partie tmp in ListParties){
                /*if((tmp.noir.Equals(joueur1) && tmp.blanc.Equals(joueur2)) ||   
                    (tmp.noir.Equals(joueur2) && tmp.blanc.Equals(joueur1))){
				    res = tmp; break;//On ne maitrise pas l'ordre des joueurs, il faut donc vérifier les deux possibilités d'ordre.
			    }*/
                if (tmp.Noir.Equals(joueur1) && tmp.Blanc.Equals(joueur2))
                {
                    res = tmp; break;
                } 
                else if (tmp.Noir.Equals(joueur2) && tmp.Blanc.Equals(joueur1))
                {
                    res = tmp; break;
                }
		    }
		    return res;
	    }

        public static Partie TrouverPartie(int uid) {
		    Partie res = null;
		
		    foreach(Partie tmp in ListParties){
			    if(tmp.Uid == uid){
				    res = tmp; break;
			    }
		    }
		    return res;
	    }
	    public bool EstSonTour(int couleur)
        {
		    bool res = false;
		    if( (couleur == 0 && this.Tour == 1)   ||   (couleur == 1 && this.Tour == -1) )
            {
			    res = true;
		    }
		    return res;
	    }
	
    // Méthode privées
    //---------------------------------------------------	
	    private void InitPlateau(){ //Génère le Plateau de base, dût à sa forme hexagonale beaucoup de chose sont écrites manuelement.
		    int i, j;
		    this.Plateau = new int[9][]; //Est nul par défaut //Voir dossier sur le pourquoi 17*9
            for(i=0; i< this.Plateau.Length; i++) {
                this.Plateau[i] = new int[17];
            } //int[,] tab = new int[9, 17];

            for (i=0; i<9; i++){
			    for(j=0; j<17; j++){
				    this.Plateau[i][j] = -99; //Les cases non parcourables sont -99
			    }
		    }
		
		    /*Ligne 1*/ for(i=4; i<=12; i=i+2) {    this.Plateau[0][i] =  1;	} //Sur la première ligne toutes les billes sont blanches
		    /*Ligne 2*/ for(i=3; i<=13; i=i+2) {    this.Plateau[1][i] =  1;    } //Sur la deuxième aussi.
		    /*Ligne 3*/ for(i=6; i<=10; i=i+2) {    this.Plateau[2][i] =  1;    } //Les autres cases sont blanches
					    this.Plateau[2][2]  = -1; 
					    this.Plateau[2][4]  = -1; 
					    this.Plateau[2][12] = -1; 
					    this.Plateau[2][14] = -1; //Ce sont les 4 seules cases vides de la ligne 3
		    /*Ligne 4*/ for(i=1; i<=15; i=i+2) {    this.Plateau[3][i] = -1;    } //Aucune bille ici
		    /*Ligne 5*/ for(i=0; i<=16; i=i+2) {    this.Plateau[4][i] = -1;    } //Aucune bille ici
		    /*Ligne 6*/ for(i=1; i<=15; i=i+2) {    this.Plateau[5][i] = -1;    } //Aucune bille ici
		    /*Ligne 7*/ this.Plateau[6][2]  = -1; //Ce sont les 4 seules cases vides de la ligne 7
					    this.Plateau[6][4]  = -1; 
					    this.Plateau[6][12] = -1; 
					    this.Plateau[6][14] = -1; 
					    for(i=6; i<=10; i=i+2) {    this.Plateau[6][i] =  0;    } //Les autres cases sont noires
		    /*Ligne 8*/ for(i=3; i<=13; i=i+2) {    this.Plateau[7][i] =  0;    } //Toutes les billes de la ligne 8 sont noires
		    /*Ligne 9*/ for(i=4; i<=12; i=i+2) {    this.Plateau[8][i] =  0;	} //Pareil pour la 9
		
		    //-99 = case invallide    -1 = Aucune bille     0 = Bille noire     1=Bille blanche
	    }

	    private int AugmenterScore(int i)
        { //0=noir+1   1=blanc+1
		    int res = -1;
		
		    if(i == 0)
            {
			    this.ScoreNoir++;
			    this._comboNoir++;
			    this._comboBlanc = 0;
			    Achievement.ACV_COMBO_2(this.Noir, this._comboNoir); //Gere tous les autres combos
			    if(this.ScoreNoir  >= 6){  Fin(0, false); res=0;  }
		    }
            else
            {
			    this.ScoreBlanc++;
			    this._comboBlanc++;
			    this._comboNoir = 0;
			    Achievement.ACV_COMBO_2(this.Blanc, this._comboBlanc); //Gere tous les autres combos
			    if(this.ScoreBlanc >= 6){  Fin(1, false); res=1;}
		    }
		    return res; //0=noirGagne   1=BlancGagne
	    }
	
	    private int ValidationMouvement(bMove moves, BMoveResp r){ 
		    int res = -1;
		    int nbrBille = 0;
		    int xDirection, yDirection, nbrBillesEnnemi = 0;
		
		    if(IsColission(moves)){ //On rencontre une bille ennemie sur notre chemin
			
			    nbrBille = nbrBilles(moves); //Nombre de bille avec moi
			    if(nbrBille > 1){
				    if(moves.Origin[0].IsVerticalRight(moves.Origin[1])){ //Verticale droite
					    if(getBilleOX(moves, _billeColission) > getBilleDX(moves, _billeColission)){ //je remonte sur le Plateau
						    xDirection = -1;
						    yDirection = 1;
					    } else{ //je descend
						    xDirection = 1;
						    yDirection = -1;
					    }
				    } else if(moves.Origin[0].IsVerticalLeft(moves.Origin[1])){//Verticale gauche 
					    if(getBilleOX(moves,_billeColission) > getBilleDX(moves, _billeColission)){ //je remonte sur le Plateau
						    xDirection = -1;
						    yDirection = -1;
					    } else{ //je descend
						    xDirection = 1;
						    yDirection = 1;
					    } 
				    }else{ //Hozirontale
					    if(getBilleOY(moves, _billeColission) < getBilleDY(moves, _billeColission)){ //je vais vers à la droite
						    xDirection = 0;
						    yDirection = 2;
					    } else{ //je vais vers la gauche
						    xDirection = 0;
						    yDirection = -2;
					    } 
				    }
				
				    nbrBillesEnnemi = NbrBillesEnnemie(moves,xDirection, yDirection); //nombre de bille ennemie sur le chemin
				    if( nbrBillesEnnemi < nbrBille){ //si elle sont en infériorité numérique je pousse
					    SetCoordonneeResp(moves, r, nbrBillesEnnemi, xDirection, yDirection);
					    if(_possiblePoint){ //Je gagne un point
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
			    UpdateBillePlateau(moves);
		    }
		
		    _billeColission = 0;
		    _possiblePoint = false;
		    return res;
	    }
	
	    private void SetCoordonneeResp (bMove moves, BMoveResp r, int nbrBille ,int xDirection, int yDirection){ //Déplace les billes poussées
		    int i, tmpX, tmpY;
		
		    r.M = moves;
		    tmpX = getBilleDX(moves, _billeColission);
		    tmpY = getBilleDY(moves, _billeColission);
		
		    for(i=0; i < nbrBille; i++)
            {
			    tmpX += xDirection;
			    tmpY += yDirection;
			    if((tmpX > -1 && tmpX < 9) && (tmpY > -1 && tmpX < 17))
                {
				    if(this.Plateau[tmpX][tmpY] != -99)
                    {
					    this.Plateau[tmpX][tmpY] = (_couleur+1)%2;
                        if (i == 0)
                            r.Destination[0] = new Bille(tmpX, tmpY);
                        else
                            r.Destination[1] = new Bille(tmpX, tmpY);
				    }
			    }
		    }
		    //Met à jour le reste du Plateau
		    UpdateBillePlateau(moves);
	    }

        private void UpdateBillePlateau(bMove moves){
            int i;
            for (i = 0; i < moves.Origin.Length; i++)
            {
                if (moves.Origin[i] > -1)
                    this.Plateau[moves.Origin[i].X][moves.Origin[i].Y] = -1;
            }
            for (i = 0; i < moves.Destination.Length; i++)
            {
                if (moves.Destination[i] > -1)
                    this.Plateau[moves.Destination[i].X][moves.Destination[i].Y] = _couleur;
            }
	    }
	
	    private int NbrBillesEnnemie(bMove moves, int xDirection, int yDirection)
        { 
		    int i, j, nbr = 1;
		    bool stop = false;
		
		    i = getBilleDX(moves, _billeColission);
		    j = getBilleDY(moves, _billeColission);
		    i += xDirection;
		    j += yDirection;
		
		    while(nbr < 3 && i > -1 &&  i < 9 && j > -1 && j < 17 && !stop && this.Plateau[i][j] != -1)
            {
			   if(this.Plateau[i][j] == -99)
				    stop = true;
			   else if(this.Plateau[i][j] == _couleur)
				    nbr = 10;
			   else if(this.Plateau[i][j] == (_couleur+1)%2)
				    nbr++;

			    i += xDirection;
			    j += yDirection;
		    }
		    if(!stop)
		    {
			    if(i < 0 || i > 8 || j < 0 || j > 16)
                {
				    stop = true;
			    }
		    }
		    _possiblePoint = stop;
		
		    return nbr;
	    }
	
	    private int nbrBilles(bMove moves)
        { 
		    int i, nbr = 0;
            for (i = 0; i < moves.Origin.Length; i++)
            {
                if (moves.Origin[i] > -1)
                {
                    nbr++;
                }
            }
		    return nbr;
	    }

        private int getBilleDX(bMove moves, int pos) => moves.Destination[pos].X;
        private int getBilleDY(bMove moves, int pos) => moves.Destination[pos].Y;
	    private int getBilleOY(bMove moves, int pos) => moves.Origin[pos].Y;
	    private int getBilleOX(bMove moves, int pos) => moves.Origin[pos].X;

        private bool IsColission(bMove moves)
        { 
		    bool colission = false;
            int i = 0;
            while (!colission && i < 3)
            {
                if (IsColissionBille(moves.Destination[i]))
                {
                    _billeColission = i;
                    colission = true;
                }
                i++;
            }
            return colission;
	    }

        private bool IsColissionBille(Bille bille)
        {
		    bool colission = false;
		    if(bille.X > -1 && bille.Y > -1)
            {
			    if(this.Plateau[bille.X][bille.Y] == (_couleur+1)%2)
                { 
				    colission = true;
			    }
		    }
		    return colission;
	    }

	
    // Debug
    //---------------------------------------------------
	    public override string ToString()
        {
		    return "Partie [uid=" + Uid + ", Plateau=" + Plateau.ToString() + ", tour=" + Tour + ", peutBouger="
				    + PeutBouger + ", scoreNoir=" + ScoreNoir + ", scoreBlanc=" + ScoreBlanc + ", _comboNoir=" + _comboNoir
				    + ", _comboBlanc=" + _comboBlanc + ", blanc=" + Blanc + ", noir=" + Noir + "]";
	    }
    }
}