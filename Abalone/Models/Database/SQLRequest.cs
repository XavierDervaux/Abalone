using System;
using System.Data.SqlClient;

namespace Abalone.Models {
    public class SQLRequest {
        static SqlConnection connect = new SqlConnection();

        public static SqlConnection GetInstance() {
            if (connect == null || connect.State == System.Data.ConnectionState.Closed) {
                try {
                    connect.ConnectionString =  "Data Source="      + SQL_CREDENTIALS.SERVER   + ";"+
                                                "Initial Catalog="  + SQL_CREDENTIALS.DATABASE + ";"+
                                                "User ID="          + SQL_CREDENTIALS.USER     + ";"+
                                                "Password="         + SQL_CREDENTIALS.PASSWORD + ";"+
                                                "MultipleActiveResultSets=true;";
                    connect.Open();
                } catch (Exception e) {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            } 
            return connect;
        }

        public static void Close() {
            try {
                if (!(connect.State == System.Data.ConnectionState.Closed)) {
                    connect.Close();
                }
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
    }
}