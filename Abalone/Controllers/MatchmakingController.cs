using Abalone.Models;
using System.Web.Mvc;

namespace Abalone.Controllers
{
    public class MatchmakingController : Controller{
        public ActionResult Index(){ // GET: Matchmaking
            ActionResult res = null;
            bool estConnecte = Identification.estConnecte(Session, Request.Cookies);

            if (estConnecte) { //On redirige vers le menu 
                ViewData["titre"] = "- Matchmaking";
                ViewData["joueur"] = ((Joueur) Session["joueur"]);
                res = View("Index");
            } else {//N'est pas encore connecté, on affiche le formulaire de connexion/inscription
                Response.Redirect("/Matchmaking/Index");
            }
            return res;
        }
    }
}