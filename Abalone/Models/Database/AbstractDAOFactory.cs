using System.Collections.Generic;

namespace Abalone.Models{
    public abstract class AbstractDAOFactory {
        public const int ORACLE_DAO_FACTORY = 0;

        public abstract DAO<Achievement> GetAchievementDAO();
        public abstract DAO<List<Achievement>> GetAchievJoueurDAO();
        public abstract DAO<Joueur> GetJoueurDAO();
        public abstract DAO<Historique> GetHistoriqueDAO();
        public static AbstractDAOFactory GetFactory(int type) {
            switch (type) {
                case ORACLE_DAO_FACTORY:
                    return new DAOFactory();
                default:
                    return null;
            }
        }
    }
}