using System;
using System.Collections.Generic;

namespace Abalone.Models{
    public class Joueur {
	    private int id = 0;
	    private String pseudo = null;
	    private String mdp = null;
	    private String email = null;
	    private List<Achievement> achievs = null;
	
	
    // Constructeurs
    //---------------------------------------------------	
	    public Joueur(String pseudo, String mdp, String email) {
		    this.pseudo = pseudo;
		    this.mdp = mdp;
		    this.email = email;
	    }
	    public Joueur(int id, String pseudo, String mdp, String email) {
		    this.id = id;
		    this.pseudo = pseudo;
		    this.mdp = mdp;
		    this.email = email;
	    }
	    public Joueur(int id, String pseudo, String mdp, String email, List<Achievement> achievs) {
		    this.id = id;
		    this.pseudo = pseudo;
		    this.mdp = mdp;
		    this.email = email;
		    this.achievs = achievs;
	    }
	

    // Getter / Setter
    //---------------------------------------------------		
	    public int Id {                   get { return this.id; }      set { this.id = value; }      }
	    public String Pseudo {            get { return this.pseudo; }  set { this.pseudo = value; }  }
        public String Mdp {               get { return this.mdp; }     set { this.mdp = value; }     }
        public String Email {             get { return this.email; }   set { this.email = value; }   }
        public List<Achievement> Achievs {get { return this.achievs; } set { this.achievs = value; } }


    // Méthodes publiques
    //---------------------------------------------------		
        public bool CreateBDD() {
		    DAOFactory adf = (DAOFactory) AbstractDAOFactory.GetFactory(0);
		    return adf.GetJoueurDAO().Create(this);
	    }
	
	    public bool UpdateBDD(){
		    DAOFactory adf = (DAOFactory) AbstractDAOFactory.GetFactory(0);
		    return adf.GetJoueurDAO().Update(this);
	    }
    
	    public bool FindBDD(int id) {
		    DAOFactory adf = (DAOFactory) AbstractDAOFactory.GetFactory(0);
		    Joueur tmp = adf.GetJoueurDAO().Find(id); // Il vaudra null si aucun n'existe
		
		    if(tmp != null){
			    this.pseudo  = tmp.Pseudo;
			    this.mdp     = tmp.Mdp;
			    this.email   = tmp.Email;
			    this.achievs = tmp.Achievs;
		    }
		    return tmp != null; //On confirme que l'objet a bien été modifié.		
	    }
    
	    public bool FindBDD(String email) {
		    DAOFactory adf = (DAOFactory) AbstractDAOFactory.GetFactory(0);
		    Joueur tmp = ((JoueurDAO) adf.GetJoueurDAO()).Find(email); // Il vaudra null si aucun n'existe
		
		    if(tmp != null){
			    this.id      = tmp.Id;
			    this.pseudo  = tmp.Pseudo;
			    this.mdp     = tmp.Mdp;
			    this.achievs = tmp.Achievs;
		    }
		    return tmp != null; //On confirme que l'objet a bien été modifié.	
	    }
	
	    public bool PossedeAchievement(int id_acv){
		    bool res = false;
		    DAOFactory adf = (DAOFactory) AbstractDAOFactory.GetFactory(0);
		    List<Achievement> tmp = adf.GetAchievJoueurDAO().Find(this.id);
		
		    foreach(Achievement a in tmp){
			    if(a.Id == id_acv){
				    res = true; break;
			    }
		    }
		    return res;
	    }
    
	    public bool CheckPassword(string uncryptedPassword) {
		    bool res = false;
		    String cryptedPassword = Utilitaire.CryptPassword(uncryptedPassword);
		
		    if (this.mdp.Equals(cryptedPassword)) { //Le pass est correct
			    res = true;
		    }
		    return res;	
	    }
	
	
    // Debug
    //---------------------------------------------------	
	    public override string ToString() {
		    return "Joueur [id=" + id + ", pseudo=" + pseudo + ", email=" + email + ", achievs=" + achievs + "]";
	    }

        public override bool Equals(Object obj)
        {
           if(obj is Joueur)
           {
                return (this.pseudo == ((Joueur)obj).Pseudo && this.id == ((Joueur)obj).Id && this.email == ((Joueur)obj).Email) ? true : false;
           }
           return false;
        }
    }
}