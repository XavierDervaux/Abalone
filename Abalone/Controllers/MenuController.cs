using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abalone.Models;

namespace Abalone.Controllers{
    public class MenuController : Controller{
        public ActionResult Index(){
            ActionResult res = null;
            bool estConnecte = Identification.estConnecte(Session, Request.Cookies);

            if (estConnecte) { //On redirige vers le menu 
                ViewData["titre"] = "- Menu";
                ViewData["joueur"] = ((Joueur) Session["joueur"]);
                res = View("Index");
            } else {//N'est pas encore connecté, on affiche le formulaire de connexion/inscription
                Response.Redirect("/Home/Index");
            }
            return res;
        }
    }
}