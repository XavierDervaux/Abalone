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
                Joueur test = ((JoueurDAO)adf.GetJoueurDAO()).Find("test1@test.com");
                Historique test2 = new Historique(DateTime.Now, 6, 4, false, new Joueur(28,"","",""), new Joueur(29, "", "", ""));
                List<Historique> test21 = null;


                System.Diagnostics.Debug.WriteLine("Connecté.");
                System.Diagnostics.Debug.WriteLine("Test des requetes");


                /*System.Diagnostics.Debug.WriteLine("\nJoueur Find(mail) : returned ");
                    if(test != null) { System.Diagnostics.Debug.WriteLine("OK"); } else { System.Diagnostics.Debug.WriteLine("Error"); }
                System.Diagnostics.Debug.WriteLine("Joueur Create : returned ");
                    test.Email = "test76@test.com"; test.Pseudo = "Test 76";
                    if (test.CreateBDD() ) { System.Diagnostics.Debug.WriteLine("OK"); } else { System.Diagnostics.Debug.WriteLine("Error"); }
                System.Diagnostics.Debug.WriteLine("Joueur Update : returned ");
                    test.Email = "test42@test.com"; test.Pseudo = "Test 42";
                    if (test != null) { System.Diagnostics.Debug.WriteLine("OK"); } else { System.Diagnostics.Debug.WriteLine("Error"); }
                System.Diagnostics.Debug.WriteLine("Joueur Find(id) : returned ");
                    test = ((JoueurDAO)adf.GetJoueurDAO()).Find(test.Id);
                    if (test != null) { System.Diagnostics.Debug.WriteLine("OK"); } else { System.Diagnostics.Debug.WriteLine("Error"); }
                */

                System.Diagnostics.Debug.WriteLine("\n Historique Create : returned ");
                    if (test2.CreateBDD()) { System.Diagnostics.Debug.WriteLine("OK"); } else { System.Diagnostics.Debug.WriteLine("Error"); }
                System.Diagnostics.Debug.WriteLine("Historique GetAll : returned ");
                    test21 = adf.GetHistoriqueDAO().GetAll();
                    if (test21 != null) { System.Diagnostics.Debug.WriteLine("OK"); } else { System.Diagnostics.Debug.WriteLine("Error"); }

                System.Diagnostics.Debug.WriteLine("\n Achiev-Joueur Create(id, id) : returned ");
                    if (((AchievJoueurDAO)adf.GetAchievJoueurDAO()).Create(30, 10)) { System.Diagnostics.Debug.WriteLine("OK"); } else { System.Diagnostics.Debug.WriteLine("Error"); }
                System.Diagnostics.Debug.WriteLine("Achiev-Joueur Find(id) : returned ");
                    if (adf.GetAchievJoueurDAO().Find(22) != null) { System.Diagnostics.Debug.WriteLine("OK"); } else { System.Diagnostics.Debug.WriteLine("Error"); }


                System.Diagnostics.Debug.WriteLine("\n Achievement Find(id) : returned ");
                    if (adf.GetAchievementDAO().Find(2) != null) { System.Diagnostics.Debug.WriteLine("OK"); } else { System.Diagnostics.Debug.WriteLine("Error"); }
                System.Diagnostics.Debug.WriteLine("Achievement GetAll(id) : returned ");
                    if (adf.GetAchievJoueurDAO().GetAll() != null) { System.Diagnostics.Debug.WriteLine("OK"); } else { System.Diagnostics.Debug.WriteLine("Error"); }

            } else {
                System.Diagnostics.Debug.WriteLine("Erreur, non-Connecté.");
            }
            System.Diagnostics.Debug.WriteLine("Fin tests connexion SQL");
            return View("testSQL");
        }
    }
}