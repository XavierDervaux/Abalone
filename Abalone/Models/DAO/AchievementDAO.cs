using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Abalone.Models {
    public class AchievementDAO : DAO<Achievement>{
	    public AchievementDAO(SqlConnection conn) : base(conn){ }
	
	    public override bool Create(Achievement obj) => false;

        public override bool Delete(Achievement obj) => false;

        public override bool Update(Achievement obj) => false;

        public override Achievement Find(int id){
		    Achievement res = null;

		    try {
                string sql = "SELECT * FROM achievement WHERE id = @id";
                SqlCommand cmd = new SqlCommand(sql, connect);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("id", id));

                using (SqlDataReader sdr = cmd.ExecuteReader()){
                    if (sdr != null) {
                        while (sdr.Read()) {
                            res = new Achievement(sdr.GetInt32(0), sdr.GetString(1), sdr.GetString(2), sdr.GetString(3));
                        }
                    }
                }
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
		    return res;
	    }

        public override List<Achievement> GetAll(){
		    List<Achievement> res = null;
            try {
                string sql = "SELECT * FROM achievement";
                SqlCommand cmd = new SqlCommand(sql, connect);
                cmd.CommandType = CommandType.Text;

                using (SqlDataReader sdr = cmd.ExecuteReader()){
                    if (sdr != null) {
                        res = new List<Achievement>();
                        while (sdr.Read()) {
                            res.Add( new Achievement(sdr.GetInt32(0), sdr.GetString(1), sdr.GetString(2), sdr.GetString(3)) );
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