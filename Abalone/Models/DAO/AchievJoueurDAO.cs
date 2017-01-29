using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Abalone.Models{
    public class AchievJoueurDAO : DAO<List<Achievement>>{
	    public AchievJoueurDAO(SqlConnection conn) : base(conn){ }
	
	    public override bool Create(List<Achievement> obj){ return false; }
	
	    public bool Create(int id_joueur, int id_achiev){
		    bool res = false;

		    try {
                string sql = "INSERT INTO fait VALUES(@id_joueur, @id_achiev);";
                SqlCommand cmd = new SqlCommand(sql, connect);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("id_joueur", id_joueur));
                cmd.Parameters.Add(new SqlParameter("id_achiev", id_achiev));
                cmd.ExecuteNonQuery();
                res = true;
            } catch (SqlException e) {
                res = true; //Si une erreur sql se produit, c'est que la combi achiev/joueur existe déjà, donc pas besoin de la créer
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
		    return res;
	    }
	
	    public override bool Delete(List<Achievement> obj){ return false; }
	
	    public override bool Update(List<Achievement> obj){ return false; }

	    public override List<Achievement> Find(int id_joueur){
		    List<Achievement> res = null;
		    DAOFactory adf = (DAOFactory) AbstractDAOFactory.GetFactory(0);
		
		    try {
                SqlCommand cmd = new SqlCommand("SELECT * FROM fait WHERE id_joueur = @idJoueur", connect);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("idJoueur", id_joueur));

                using (SqlDataReader sdr = cmd.ExecuteReader()){
                    if(sdr != null){ 
				        res = new List<Achievement>();
				        while(sdr.Read()){
					        res.Add( adf.GetAchievementDAO().Find( sdr.GetInt32(1)) ); 
				        }
			        }
                }
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
		    return res;
	    }
	
	    public override List<List<Achievement>> GetAll(){ return null; }
    }
}

