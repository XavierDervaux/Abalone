using System;
using System.Collections.Generic;

namespace Abalone.Models {
    public class Historique {
	    private int id = 0;
	    private DateTime date;
	    private int scoreGagnant = 0;
	    private int scorePerdant = 0;
	    private bool estForfait = false;
	    private Joueur gagnant = null;
	    private Joueur perdant = null;
	
	
    // Constructeurs
    //---------------------------------------------------	
	    public Historique(DateTime date, int scoreGagnant, int scorePerdant, bool estForfait, Joueur gagnant, Joueur perdant) {
		    this.date = date;
		    this.scoreGagnant = scoreGagnant;
		    this.scorePerdant = scorePerdant;
		    this.estForfait = estForfait;
		    this.gagnant = gagnant;
		    this.perdant = perdant;
	    }
	    public Historique(int id, DateTime date, int scoreGagnant, int scorePerdant, bool estForfait, Joueur gagnant, Joueur perdant) {
		    this.id = id;
		    this.date = date;
		    this.scoreGagnant = scoreGagnant;
		    this.scorePerdant = scorePerdant;
		    this.estForfait = estForfait;
		    this.gagnant = gagnant;
		    this.perdant = perdant;
	    }
	

    // Getter / Setter
    //---------------------------------------------------	
        public int Id {           get { return this.id; }           set { this.id = value; }            }
        public DateTime Date {    get { return this.date; }         set { this.date = value; }          }
        public int ScoreGagnant { get { return this.scoreGagnant; } set { this.scoreGagnant = value; }  }
        public int ScorePerdant { get { return this.scorePerdant; } set { this.scorePerdant = value; }  }
        public bool EstForfait {  get { return this.estForfait; }   set { this.estForfait = value; }    }
        public Joueur Gagnant {   get { return this.gagnant; }      set { this.gagnant = value; }       }
        public Joueur Perdant {   get { return this.perdant; }      set { this.perdant = value; }       }


    // Méthodes publiques
    //---------------------------------------------------	
        public bool CreateBDD() {
		    DAOFactory adf = (DAOFactory) AbstractDAOFactory.GetFactory(0);
		    return adf.GetHistoriqueDAO().Create(this);
	    }
    
	    public static List<Historique> FindAllBDD(Joueur joueur) {
		    DAOFactory adf = (DAOFactory) AbstractDAOFactory.GetFactory(0);
		    List<Historique> tmp = ((HistoriqueDAO) adf.GetHistoriqueDAO()).GetAll(joueur);
		    return tmp;	
	    }
	
	
    // Debug
    //---------------------------------------------------		
	    public override String ToString() {
		    return "Historique [id=" + id + ", date=" + date + ", scoreGagnant=" + scoreGagnant + ", scorePerdant="
				    + scorePerdant + ", estForfait=" + estForfait + ", gagnant=" + gagnant + ", perdant=" + perdant + "]";
	    }
    }
}