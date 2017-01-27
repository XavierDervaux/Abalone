using Abalone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Abalone.Controllers{
    public class SettingsController : Controller{
        public ActionResult Index(){
            ActionResult res = null;
            bool estConnecte = Identification.estConnecte(Session, Request.Cookies);

            if (estConnecte) { //On redirige vers le menu 
                ViewData["titre"] = "- Paramètres";
                ViewData["joueur"] = ((Joueur) Session["joueur"]);
                res = View("Index");
            } else {//N'est pas encore connecté, on affiche le formulaire de connexion/inscription
                Response.Redirect("/Home/Index");
            }
            return res;
        }

        public ActionResult Post(){
            int res = -1;
            String output = null;
            String mail = Request.Form["emailSetting"];
            String mdp = Request.Form["passwordSetting"];
            Joueur actuel = (Joueur)Session["joueur"]; //On est sur que le joueur existe car on vérifie estConnecté avant d'arriver ici

            res = changementMail(mail, actuel);
    	    if(res <=0){ res = changementMdp(mdp, actuel); } //Si on a pas déjà trouvé une erreur avec le mail, alors on vérifie le mdp.
		    if(res == 0) { //Au moins une opération s'est bien passé
			    actuel.UpdateBDD();
			    Session["joueur"] = actuel; //On met à jour la session
			    Response.Redirect("/Menu/Index"); return null;
		    } else { //Au moins une opération a échoué, on retourne l'erreur
			    output = affichageSettings(res);
                ViewData["erreur"] = output;
		    }
            return View("Index");
        }

        private int changementMdp(String mdp, Joueur actuel){
            int res = 0;

            if (mdp != null && res <= 0){ //changement de mdp  et si on n'a pas déjà trouvé une erreur dans le mail
                res = Identification.validationMdp(mdp);
                if (res == 0) //Le mdp est valide
                    actuel.Mdp = Utilitaire.CryptPassword(mdp);
                else
                    res = -1;
            }
            return res;
        }

        private int changementMail(String mail, Joueur actuel){
            int res = -1;

            if (mail != null && !(actuel.Email.Equals(mail))){ //Si l'utilisateur a entré un nouvel email, différent de celui qu'il utilise déjà 
                res = Identification.validationEmail(mail);
                if (res == 0){ //Le mail est valide
                    if (actuel.FindBDD(mail)) //Si on arrive à trouver un joueur correspondant en bdd ça veut dire que l'email existe déjà
                        res = 5;
                    else
                        actuel.Email = mail;
                }
            }
            return res;
        }

        private String affichageSettings(int res){
            String output = null;
            switch (res){ // Affiche du message de sortie
                case 2: output = "Merci de saisir une adresse mail valide."; break;
                case 3: output = "Le mot de passe doit contenir au moins 8 caractères."; break;
                case 4: output = "Le mot de passe doit contenir moins de 16 caractères."; break;
                case 5: output = "L'adresse E-mail saisie est déjà enregistrée."; break;
            }
            return output;
        }
    }
}