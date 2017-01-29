using System.Collections.Generic;
using System.Data.SqlClient;

namespace Abalone.Models {
    public abstract class DAO<T> {
        protected SqlConnection connect = null;

        public DAO(SqlConnection conn) {
            this.connect = conn;
        }

        public abstract bool Create(T obj);
        public abstract bool Delete(T obj);
        public abstract bool Update(T obj);
        public abstract T Find(int id);
        public abstract List<T> GetAll();
    }
}