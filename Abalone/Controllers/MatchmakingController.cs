using Abalone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Abalone.Controllers{
    public class MatchmakingController : Controller{
        // GET: Matchmaking
        public ActionResult Index(){
            //Check output
            Console.WriteLine("Début tests connexion SQL");
            if(SQLRequest.GetInstance().State == System.Data.ConnectionState.Open) {
                AbstractDAOFactory adf = AbstractDAOFactory.GetFactory(0);
                Joueur test = new Joueur("","","");
                Historique test2 = new Historique(DateTime.Now, 6, 4, false, adf.GetJoueurDAO().Find(1), adf.GetJoueurDAO().Find(2));
                List<Historique> test21 = null;


                System.Diagnostics.Debug.WriteLine("Connecté.");
                System.Diagnostics.Debug.WriteLine("Test des requetes");

               
                System.Diagnostics.Debug.WriteLine("\n Historique Create : returned ");
                    if (test2.CreateBDD()) { System.Diagnostics.Debug.WriteLine("OK"); } else { System.Diagnostics.Debug.WriteLine("Error"); }
                System.Diagnostics.Debug.WriteLine("Historique GetAll : returned ");
                    test21 = adf.GetHistoriqueDAO().GetAll();
                    if (test21 != null) { System.Diagnostics.Debug.WriteLine("OK"); } else { System.Diagnostics.Debug.WriteLine("Error"); }

            } else {
                System.Diagnostics.Debug.WriteLine("Erreur, non-Connecté.");
            }
            System.Diagnostics.Debug.WriteLine("Fin tests connexion SQL");
            return View("testSQL");
        }
    }
}