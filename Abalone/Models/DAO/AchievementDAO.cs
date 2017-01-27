using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;

namespace Abalone.Models {
    public class AchievementDAO : DAO<Achievement>{
	    public AchievementDAO(OracleConnection conn) : base(conn){ }
	
	    public override bool Create(Achievement obj){		
		    return false;
	    }

        public override bool Delete(Achievement obj){
		    return false;
	    }

        public override bool Update(Achievement obj){
		    return false;
	    }

        public override Achievement Find(int id){
		    Achievement res = null;

		    try {
                OracleCommand cmd = new OracleCommand("SELECT * FROM achievement WHERE id = :id", connect);
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Add(new OracleParameter(":id", id));
                OracleDataReader odr = cmd.ExecuteReader();

                if (odr != null) {
                    while (odr.Read()) {
                        res = new Achievement(odr.GetInt32(0), odr.GetString(1), odr.GetString(2), odr.GetString(3));
                    }
                }
                odr.Close();
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
		    return res;
	    }

        public override List<Achievement> GetAll(){
		    List<Achievement> res = null;
            try {
                OracleCommand cmd = new OracleCommand("SELECT * FROM achievement", connect);
                cmd.CommandType = System.Data.CommandType.Text;
                OracleDataReader odr = cmd.ExecuteReader();

                if (odr != null) {
                    res = new List<Achievement>();
                    while (odr.Read()) {
                        res.Add( new Achievement(odr.GetInt32(0), odr.GetString(1), odr.GetString(2), odr.GetString(3)) );
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