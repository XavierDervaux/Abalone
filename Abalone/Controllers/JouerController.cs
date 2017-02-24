using Abalone.Models;
using Abalone.Models.Metier;
using System;
using System.Web.Mvc;

namespace Abalone.Controllers
{
    public class JouerController : Controller
    {

       // private bool estCorrect = false;
        public ActionResult Toto()
        { // GET: Jouer
            Response.Redirect("/Menu/Index");
            return View("Index");
        }

        [HttpPost]
        public ActionResult Index()
        {
            int nbrJ1, nbrJ2;
            Joueur joueur1, joueur2;
            Partie play = null;
            Random rand = new Random();
            String mJ1 = Request.Form["joueur1"];
            String mJ2 = Request.Form["joueur2"];

            if (mJ1 != null && mJ2 != null && !(mJ1.Equals(mJ2)))
            {
                joueur1 = new Joueur("", "", mJ1); joueur1.FindBDD(mJ1);
                joueur2 = new Joueur("", "", mJ2); joueur2.FindBDD(mJ2); //On récupère les joueurs en BDD
                if (joueur1.Id != 0 && joueur2.Id != 0)
                { //Si tout est correct
                    play = Partie.TrouverPartie(joueur1, joueur2);
                    if (play == null)
                    {//Si on ne peut trouver aucune partie existante pour ces deux joueurs, on la crée. Si on en trouve une ça veut dire que l'autre joueur est passé en premier, on la récupère
                        nbrJ1 = rand.Next(100);
                        nbrJ2 = rand.Next(100);

                        if (nbrJ1 >= nbrJ2) { play = new Partie(joueur1, joueur2); } //On décide aléatoirement qui sera blanc et qui sera noir
                        else { play = new Partie(joueur2, joueur1); }
                        Partie.ListParties.Add(play);
                    }
                    ViewData["joueur"] = ((Joueur)Session["joueur"]);
                    ViewData["id_partie"] = play.Uid;
                    ViewData["noir_pseudo"] = play.Noir.Pseudo;
                    ViewData["noir_email"] = play.Noir.Email;
                    ViewData["blanc_pseudo"] = play.Blanc.Pseudo;
                    ViewData["blanc_email"] = play.Blanc.Email;
                }
                else
                {
                    Response.Redirect("/Menu/Index");
                }
            }
            return View("Index");
        }
    }
}