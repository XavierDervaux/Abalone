using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Abalone.Models {
    public class HistoriqueDAO : DAO<Historique>{
	    public HistoriqueDAO(OracleConnection conn) : base(conn) { }
	
	    public override bool Create(Historique obj){	
		    int returning;
		    bool res = true;

		    try {
                OracleCommand cmd = new OracleCommand("INSERT INTO historique(id,date_partie,score_gagnant,score_perdant,est_forfait,id_gagnant,id_perdant)" 
                                                     + "VALUES('', :datePartie, :sGag, :sPer, :forf, :jGag, :jPer)", connect);
                cmd.CommandType = CommandType.Text;
                cmd.BindByName = true;
                
                cmd.Parameters.Add(new OracleParameter(":datePartie", "date")).Value    = obj.Date.ToString("dd/MM/yyyy"); 
                cmd.Parameters.Add(new OracleParameter(":sGag", "integer")).Value = obj.ScoreGagnant;
                cmd.Parameters.Add(new OracleParameter(":sPer", "integer")).Value = obj.ScorePerdant;
                cmd.Parameters.Add(new OracleParameter(":forf", "number")).Value  = Utilitaire.BoolToInt(obj.EstForfait);
                cmd.Parameters.Add(new OracleParameter(":jGag", "integer")).Value = obj.Gagnant.Id;
                cmd.Parameters.Add(new OracleParameter(":jPer", "integer")).Value = obj.Perdant.Id;
                cmd.ExecuteNonQuery();


                OracleCommand cmd2 = new OracleCommand("SELECT pkg_historique.last_id_historique FROM dual", connect);
                cmd2.CommandType = CommandType.Text;
                cmd2.BindByName = true;
                cmd2.ExecuteNonQuery();
                OracleDataReader odr = cmd.ExecuteReader();

                if (odr != null){
                    while (odr.Read()){
                        obj.Id = odr.GetInt32(0);
                    }
                }
                odr.Close();
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
                res = false;
		    }
		    return res;
	    }
	
	    public override bool Delete(Historique obj){
		    return false;
	    }
	
	    public override bool Update(Historique obj){
		    return false;
	    }
	
	    public override Historique Find(int id){
		    return null;
	    }
	
	    public override List<Historique> GetAll(){
		    return null;
	    }
	
	    public List<Historique> GetAll(Joueur joueur){
		    List<Historique> res = null;
		    DAOFactory adf = (DAOFactory) AbstractDAOFactory.GetFactory(0);
            
            try {
                OracleCommand cmd = new OracleCommand("SELECT * FROM historique WHERE id_gagnant = :id OR id_perdant = :id", connect);
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Add(new OracleParameter(":id", joueur.Id));
                OracleDataReader odr = cmd.ExecuteReader();

                if (odr != null){
                    res = new List<Historique>();
                    while (odr.Read()){
                        res.Add(new Historique( odr.GetInt32(0), odr.GetDateTime(1), odr.GetInt32(2), odr.GetInt32(3), Utilitaire.IntToBool(odr.GetInt32(4)), adf.GetJoueurDAO().Find(odr.GetInt32(5)), adf.GetJoueurDAO().Find(odr.GetInt32(6))) );
                    }
                }
                odr.Close();
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
		    }
		    return res;
	    }
    }
}