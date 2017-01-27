using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;

namespace Abalone.Models {
    public abstract class DAO<T> {
        protected OracleConnection connect = null;

        public DAO(OracleConnection conn) {
            this.connect = conn;
        }

        public abstract bool Create(T obj);
        public abstract bool Delete(T obj);
        public abstract bool Update(T obj);
        public abstract T Find(int id);
        public abstract List<T> GetAll();
    }
}