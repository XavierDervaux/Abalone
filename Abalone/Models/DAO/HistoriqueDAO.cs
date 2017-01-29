using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace Abalone.Models {
    public class HistoriqueDAO : DAO<Historique>{
	    public HistoriqueDAO(SqlConnection conn) : base(conn) { }
	
	    public override bool Create(Historique obj){
		    bool res = false;

		    try {
                string sql = "INSERT INTO historique(date_partie,score_gagnant,score_perdant,est_forfait,id_gagnant,id_perdant)"
                           + "OUTPUT INSERTED.id "
                           + "VALUES(CONVERT(DATETIME, @datePartie, 102), @sGag, @sPer, @forf, @jGag, @jPer)";
                SqlCommand cmd = new SqlCommand(sql, connect);
                cmd.CommandType = CommandType.Text; 
                cmd.Parameters.Add(new SqlParameter("@datePartie", obj.Date.ToString("MM/dd/yyyy") )); 
                cmd.Parameters.Add(new SqlParameter("@sGag", obj.ScoreGagnant));
                cmd.Parameters.Add(new SqlParameter("@sPer", obj.ScorePerdant));
                cmd.Parameters.Add(new SqlParameter("@forf", Utilitaire.BoolToInt(obj.EstForfait) ));
                cmd.Parameters.Add(new SqlParameter("@jGag", obj.Gagnant.Id));
                cmd.Parameters.Add(new SqlParameter("@jPer", obj.Perdant.Id));
                obj.Id = (int)cmd.ExecuteScalar();
                res = true;
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
		    }
		    return res;
	    }
	
	    public override bool Delete(Historique obj){ return false; }
	
	    public override bool Update(Historique obj){ return false; }
	
	    public override Historique Find(int id){ return null; }
	
	    public override List<Historique> GetAll(){ return null; }
	
	    public List<Historique> GetAll(Joueur joueur){
		    List<Historique> res = null;
		    DAOFactory adf = (DAOFactory) AbstractDAOFactory.GetFactory(0);
            
            try {
                string sql = "SELECT * FROM historique WHERE id_gagnant = @id OR id_perdant = @id";
                SqlCommand cmd = new SqlCommand(sql, connect);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@id", joueur.Id));

                using (SqlDataReader sdr = cmd.ExecuteReader()){
                    if (sdr != null){
                        res = new List<Historique>();
                        while (sdr.Read()){
                            res.Add(new Historique( sdr.GetInt32(0), sdr.GetDateTime(1), sdr.GetInt32(2), sdr.GetInt32(3), sdr.GetBoolean(4), adf.GetJoueurDAO().Find(sdr.GetInt32(5)), adf.GetJoueurDAO().Find(sdr.GetInt32(6))) );
                        }
                    }
                }
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
		    }
		    return res;
	    }
    }
}