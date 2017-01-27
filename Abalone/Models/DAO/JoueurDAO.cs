using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Abalone.Models {
    public class JoueurDAO : DAO<Joueur>{
	    public JoueurDAO(OracleConnection conn) : base(conn){ }
	
	    public override bool Create(Joueur obj){		
		    int returning;
		    bool res = true;

		    try {
                OracleCommand cmd = new OracleCommand("pkg_joueur.createjoueur", connect);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;
                cmd.Parameters.Add("Return_Value", OracleDbType.Int32, ParameterDirection.ReturnValue);
                cmd.Parameters.Add("pseudo", "varchar2").Value = obj.Pseudo;
                cmd.Parameters.Add("mdp", "varchar2").Value = obj.Mdp;
                cmd.Parameters.Add("email", "varchar2").Value = obj.Email;
                returning = cmd.ExecuteNonQuery();
			
			    if(returning != 0){ //L'insert à bien eu lieu
                    obj.Id = int.Parse(cmd.Parameters["Return_Value"].Value.ToString());
                }
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
                res = false;
		    }
		    return res;
	    }
	
	    public override bool Delete(Joueur obj){
		    return false;
	    }
	
	    public override bool Update(Joueur obj){
		    bool res = false;
		
		    if(obj.Id == 0){ //L'objet vient d'etre créé et ne sort pas de la DB
                Console.WriteLine("Erreur, vous ne pouvez pas mettre un enregistrement à jour sur base de cet objet.");
                Console.WriteLine("Réessayez avec un objet provenant de la BDD.\n");
		    }else{
			    try {
                    OracleCommand cmd = new OracleCommand("pkg_joueur.updateJoueur", connect);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("v_pseudo", "varchar2").Value = obj.Pseudo;
                    cmd.Parameters.Add("v_mdp", "varchar2").Value    = obj.Mdp;
                    cmd.Parameters.Add("v_email", "varchar2").Value  = obj.Email;
                    cmd.Parameters.Add("v_id", "number").Value       = obj.Id;
                    cmd.ExecuteNonQuery();
				    res = true;
                } catch (Exception e) {
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                }
		    }

		    return res;
	    }
	
	    public override Joueur Find(int id){
		    DAOFactory adf = (DAOFactory) AbstractDAOFactory.GetFactory(0);
		    Joueur res = null;

		    try {
                OracleCommand cmd = new OracleCommand("SELECT * FROM joueur WHERE id = :idJoueur", connect);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new OracleParameter(":idJoueur", id));
                OracleDataReader odr = cmd.ExecuteReader();

                if (odr != null){
                    while (odr.Read()){
                        res = new Joueur(id, odr.GetString(1), odr.GetString(2), odr.GetString(3), adf.GetAchievJoueurDAO().Find(id));
                    }
                }
                odr.Close();
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
		
		    return res;
	    }
	
	    public Joueur Find(String email){
		    DAOFactory adf = (DAOFactory) AbstractDAOFactory.GetFactory(0);
		    Joueur res = null;

		    try {
                OracleCommand cmd = new OracleCommand("SELECT * FROM joueur WHERE email = :email", connect);
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Add(new OracleParameter(":email", email));
                OracleDataReader odr = cmd.ExecuteReader();

                if (odr != null){
                    while (odr.Read()){
                        int id = odr.GetInt32(0);
                        res = new Joueur(id, odr.GetString(1), odr.GetString(2), odr.GetString(3), adf.GetAchievJoueurDAO().Find(id));
                    }
                }
                odr.Close();
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
		
		    return res;
	    }
	
	    public override List<Joueur> GetAll(){
		    return null;
	    }
    }
}