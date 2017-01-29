using System;
using System.Web.Mvc;
using Abalone.Models;
using System.Web;

namespace Abalone.Controllers{
    public class HomeController : Controller{
        public ActionResult Index(){
            ActionResult res = null;
            bool estConnecte = Identification.estConnecte(Session, Request.Cookies);

            if (estConnecte) { //On redirige vers le menu 
                Response.Redirect("/Menu/Index"); return null;
            } else //N'est pas encore connecté, on affiche le formulaire de connexion/inscription
                res = View("Index");
                ViewData["titre"] = "- Connexion";
            return res;
        }

        [HttpPost]
        public ActionResult Post(){
            if ("on".Equals(Request.Form["estConnexion"]))
                connexion();
            else if ("on".Equals(Request.Form["estInscription"])) 
                inscription();
            else
                deconnexion();
                ViewData["titre"] = "- Connexion";
            return View("Index");//Index se chargera de l'erreur s'il y en a une
        }


    // Méthodes privées
    //---------------------------------------------------	
        private void connexion() {
            String output = null;
            String mdp = Request.Form["passwordConnection"];
            String email = Request.Form["emailConnection"];
            Joueur input = new Joueur("", mdp, email);
            bool resterConnecte = "on".Equals(Request.Form["rememberConnection"]);
            int res = Identification.Connexion(input); // Si la connexion réussi, les propriétés de inputs seront modifiées pour obtenir un Joueur valide

            if (res == 1) { //Tout s'est bien passé, l'user est co, on a plus qu'a définir ses sessions et cookies
                Session["connected"] = true;
                Session["joueur"] = input;
                if (resterConnecte){ // Si l'utilisateur le souhaite, on lui crée un cookie pour ne pas qu'il se relogg a chaque fois
                    Utilitaire.SetCookie(Response.Cookies, "user_email", input.Email, 60 * 60 * 24 * 365);
                }
                Response.Redirect("/Home/Index");
            } else { //Une erreur est survenue, on défini le message d'erreur pour le retour sur la page.
                output = affichageConnexion(res);
                ViewData["erreurConn"] = output;
                ViewData["emailConnection"] = email;
            }
            
        }

        private void inscription(){
            String output = null;
            String pseudo = Request.Form["pseudoInscription"];
            String mdp    = Request.Form["passwordInscription"];
            String email  = Request.Form["emailInscription"];
            Joueur input  = new Joueur(pseudo, mdp, email);
            int res = Identification.inscription(input);

            if (res == 1) { //L'inscription s'est bien passé, on défini les sessions.
                Session["connected"] =  true;
                Session["joueur"] = input;
                ViewData["joueur"] = input.Pseudo;
                Response.Redirect("/Home/Index");
            } else { //Une erreur est survenue, on défini le message d'erreur pour le retour sur la page.
                output = affichageConnexion(res);
                ViewData["erreurInscr"] = output;
                ViewData["pseudoInscription"] = pseudo;
                ViewData["emailInscription"] = email;
            }
        }

        private void deconnexion(){
            Session.Remove("connected");
            Session.Remove("joueur"); //On supprime ses sessions
            ViewData["joueur"] = null;

            Request.Cookies.Remove("user_email"); 
            HttpCookie c = new HttpCookie("user_email");
            c.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(c); //Et son cookie éventuel
        }

        private String affichageConnexion(int res){
            String output = null;
            switch (res){ // Affiche du message de sortie
                case 0: output = "Le mot de passe ou l'adresse mail saisie est incorrecte."; break;
                case 2: output = "Merci de saisir une adresse mail valide."; break;
                case 3: output = "Le mot de passe doit contenir au moins 8 caractères."; break;
                case 4: output = "Le mot de passe doit contenir moins de 16 caractères."; break;
                case 5: output = "L'adresse E-mail saisie est déjà enregistrée."; break;
            }
            return output;
        }
    }
}
	
