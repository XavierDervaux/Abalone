using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;

namespace Abalone.Models{
    public class AchievJoueurDAO : DAO<List<Achievement>>{
	    public AchievJoueurDAO(OracleConnection conn) : base(conn){ }
	
	    public override bool Create(List<Achievement> obj){		
		    return false;
	    }
	
	    public bool Create(int id_joueur, int id_achiev){
		    bool res = true;

		    try {
                OracleCommand cmd = new OracleCommand("pkg_achievement.createAchievJoueur", connect);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("id_joueur", "number").Value = id_joueur;
                cmd.Parameters.Add("id_achiev", "number").Value = id_achiev;
                cmd.ExecuteNonQuery();
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
                res = false;
            }
		    return res;
	    }
	
	    public override bool Delete(List<Achievement> obj){
		    return false; 
	    }
	
	    public override bool Update(List<Achievement> obj){
		    return false; 
	    }

	    public override List<Achievement> Find(int id_joueur){
		    List<Achievement> res = null;
		    DAOFactory adf = (DAOFactory) AbstractDAOFactory.GetFactory(0);
		
		    try {
                OracleCommand cmd = new OracleCommand("SELECT * FROM fait WHERE id_joueur = :idJoueur", connect);
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Add(new OracleParameter(":idJoueur", id_joueur));
                OracleDataReader odr = cmd.ExecuteReader();
			
			    if(odr != null){ 
				    res = new List<Achievement>();
				    while(odr.Read()){
					    res.Add( adf.GetAchievementDAO().Find( odr.GetInt32(1)) ); 
				    }
			    }
                odr.Close();
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
		
		    return res;
	    }
	
	    public override List<List<Achievement>> GetAll(){
		    return null;
	    }
    }
}

