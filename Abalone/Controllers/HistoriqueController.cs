using Abalone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Abalone.Controllers{
    public class HistoriqueController : Controller{
        public ActionResult Index(){
            ActionResult res = null;
            bool estConnecte = Identification.estConnecte(Session, Request.Cookies);

            if (estConnecte) { //On redirige vers le menu 
                ViewData["titre"] = "- Historique";
                ViewData["joueur"] = ((Joueur) Session["joueur"]);
                getInfo(); //Définis le ViewData pour les succès.
                res = View("Index");
            } else {//N'est pas encore connecté, on affiche le formulaire de connexion/inscription
                Response.Redirect("/Home/Index");
            }
            return res;
        }

        private void getInfo(){
            int jouees = 0, gagnees = 0, perdues = 0, forfait = 0;
            Joueur actuel = (Joueur) Session["joueur"];
            List<Historique> listH = Historique.FindAllBDD(actuel);
            listH = listH.OrderByDescending(l => l.Date).ToList();

            if (listH != null){
                jouees = listH.Count;
                foreach(Historique tmp in listH){
                    if (tmp.Gagnant.Id == actuel.Id) { gagnees++; }
                    else if (tmp.EstForfait)         { forfait++; }
                    else                             { perdues++; }
                }
            }

            ViewData["jouees"] = jouees;
            ViewData["gagnes"] = gagnees;
            ViewData["perdues"] = perdues;
            ViewData["forfait"] = forfait;
            ViewData["liste"] = listH;
        }
    }
}
	
	