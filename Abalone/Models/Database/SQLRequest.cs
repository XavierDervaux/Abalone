using System;
using Oracle.ManagedDataAccess.Client;

namespace Abalone.Models {
    public class SQLRequest {
        static OracleConnection connect = new OracleConnection();

        public static OracleConnection GetInstance() {
            if (connect == null || connect.State == System.Data.ConnectionState.Closed) {
                try {
                    connect.ConnectionString = SQL_CREDENTIALS.SERVER; //A faire
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