using Abalone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Abalone.Controllers{
    public class SuccesController : Controller{
        // GET: Succes
        public ActionResult Index(){
            ActionResult res = null;
            bool estConnecte = Identification.estConnecte(Session, Request.Cookies);

            if (estConnecte) { //On redirige vers le menu 
                ViewData["titre"] = "- Succès";
                ViewData["joueur"] = ((Joueur) Session["joueur"]);
                getInfo(); //Définis le ViewData pour les succès.
                res = View("Index");
            } else {//N'est pas encore connecté, on affiche le formulaire de connexion/inscription
                Response.Redirect("/Home/Index");
            }
            return res;
        }

        private void getInfo(){
    	    Joueur actuel = (Joueur) Session["joueur"];
		    List<Achievement> listA = Achievement.FindAllBDD(); //Ne sera jamais null
		    actuel.FindBDD(actuel.Email); //On actualise le joueur
		    List<Achievement> listB = actuel.Achievs;
		
		    listA.Sort((Achievement a, Achievement b) => b.CompareTo(a)); //On trie la liste pour que l'affichage se fasse de façon cohérente
		    if(listB != null) { listB.Sort((Achievement a, Achievement b) => b.CompareTo(a)); }

			foreach(Achievement fait in listB) {
                listA.Remove(fait); //On le supprime s'il est déjà fait pour laisser une liste de ce qui reste à faire.
			}

            ViewData["pasEncoreAccomplis"] = listA;
            ViewData["accomplis"] = listB;
	    }
    }
}