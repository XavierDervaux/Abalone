using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;

namespace Abalone.Models{
    public class DAOFactory : AbstractDAOFactory{
	    private static OracleConnection conn = SQLRequest.GetInstance();

	    public override DAO<Achievement> GetAchievementDAO(){
		    return new AchievementDAO(conn);
	    }
	
	    public override DAO<List<Achievement>> GetAchievJoueurDAO(){
		    return new AchievJoueurDAO(conn);
	    }
	
	    public override DAO<Joueur> GetJoueurDAO(){
		    return new JoueurDAO(conn);
	    }
	
	    public override DAO<Historique> GetHistoriqueDAO(){
		    return new HistoriqueDAO(conn);
	    }
    }
}
