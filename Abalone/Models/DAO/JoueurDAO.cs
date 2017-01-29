using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Abalone.Models {
    public class JoueurDAO : DAO<Joueur>{
	    public JoueurDAO(SqlConnection conn) : base(conn){ }
	
	    public override bool Create(Joueur obj) {
		    bool res = false;
            string sql = "INSERT INTO joueur(pseudo, mdp, email) OUTPUT INSERTED.id VALUES(@pseudo, @mdp, @email);";

            try {
                using (SqlCommand cmd = new SqlCommand(sql, connect)){
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("pseudo", obj.Pseudo));
                    cmd.Parameters.Add(new SqlParameter("mdp", obj.Mdp));
                    cmd.Parameters.Add(new SqlParameter("email", obj.Email));
                    obj.Id = (int)cmd.ExecuteScalar();
                }
                res = true;
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
		    }
		    return res;
	    }
	
	    public override bool Delete(Joueur obj){ return false; }
	
	    public override bool Update(Joueur obj){
		    bool res = false;
            string sql = "UPDATE joueur SET pseudo=@pseudo, mdp=@mdp, email=@email WHERE id=@id";

            if (obj.Id > 0){ //L'objet vient d'etre créé et ne sort pas de la DB
			    try {
                    using (SqlCommand cmd = new SqlCommand(sql, connect)) {
                        cmd.Parameters.Add(new SqlParameter("pseudo", obj.Pseudo));
                        cmd.Parameters.Add(new SqlParameter("mdp", obj.Mdp));
                        cmd.Parameters.Add(new SqlParameter("email", obj.Email));
                        cmd.Parameters.Add(new SqlParameter("id", obj.Id));
                        cmd.ExecuteNonQuery();
                    }
				    res = true;
                } catch (Exception e) {
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                }
		    } else {
                System.Diagnostics.Debug.WriteLine("Erreur, vous ne pouvez pas mettre un enregistrement à jour sur base de cet objet.");
                System.Diagnostics.Debug.WriteLine("Réessayez avec un objet provenant de la BDD.\n");
		    }
		    return res;
	    }
	
	    public override Joueur Find(int id){
		    DAOFactory adf = (DAOFactory) AbstractDAOFactory.GetFactory(0);
		    Joueur res = null;

		    try {
                string sql = "SELECT * FROM joueur WHERE id = @idJoueur";
                using (SqlCommand cmd = new SqlCommand(sql, connect)) {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@idJoueur", id));

                    using (SqlDataReader sdr = cmd.ExecuteReader()){
                        if (sdr != null){
                            while (sdr.Read()){
                                res = new Joueur(id, sdr.GetString(1), sdr.GetString(2), sdr.GetString(3), adf.GetAchievJoueurDAO().Find(id));
                            }
                        }
                    }
                }
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
		    return res;
	    }
	
	    public Joueur Find(String email){
            DAOFactory adf = (DAOFactory) AbstractDAOFactory.GetFactory(0);
		    Joueur res = null;

		    try {
                string sql = "SELECT * FROM joueur WHERE email = @email";
                using (SqlCommand cmd = new SqlCommand(sql, connect)) {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@email", email));

                    using (SqlDataReader sdr = cmd.ExecuteReader()){
                        if (sdr != null){
                            while (sdr.Read()){
                                int id = sdr.GetInt32(0);
                                res = new Joueur(id, sdr.GetString(1), sdr.GetString(2), sdr.GetString(3), adf.GetAchievJoueurDAO().Find(id));
                            }
                        }
                    }
                }
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }		
		    return res;
	    }
	
	    public override List<Joueur> GetAll(){ return null; }
    }
}