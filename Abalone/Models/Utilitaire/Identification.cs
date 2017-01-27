using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;

namespace Abalone.Models{
    public class Identification{
        public static int Connexion(Joueur joueur){
            int rmail, rmdp, res = 0;
            string tmpMdp, tmpMail;
            rmail = validationEmail(joueur.Email);
            rmdp = validationMdp(joueur.Mdp);

            if (rmail != 0){ // Le mail est incorrect
                res = rmail;
            }
            else if (rmdp != 0){ // Le mdp est incorrect
                res = rmdp;
            } else { // Tout s'est bien passé
                tmpMdp = joueur.Mdp;
                tmpMail = joueur.Email;

                if (joueur.FindBDD(tmpMail))
                {  //Si on arrive à trouver un joueur correspondant en bdd
                    if (joueur.CheckPassword(tmpMdp))
                    { //Si le mdp fourni est correct
                        res = 1;
                    }
                }
            }
            return res;
        }

        public static int inscription(Joueur joueur){
            int rmail, rmdp, res = 0;
            string tmpMdp, tmpMail;
            rmail = validationEmail(joueur.Email);
            rmdp = validationMdp(joueur.Mdp);

            if (rmail != 0){ //Le mail est incorrect
                res = rmail;
            } else if (rmdp != 0) { //Le mdp est incorrect
                res = rmdp;
            } else { //Les duex sont bon
                tmpMdp = joueur.Mdp;
                tmpMail = joueur.Email;
                if (joueur.FindBDD(tmpMail)){  //Si on arrive à trouver un joueur correspondant en bdd ça veut dire que l'email existe déjà
                    res = 5;
                } else { //Tout est ok
                    joueur.Mdp = Utilitaire.CryptPassword(tmpMdp); //On crypte le mot de passe
                    joueur.CreateBDD(); //Il est désormais enregistré dans la db
                    res = 1;
                }
            }
            return res;
        }

        public static bool estConnecte(HttpSessionStateBase sessions, HttpCookieCollection cookies){
            DAOFactory adf = (DAOFactory)AbstractDAOFactory.GetFactory(0);
            String connect = null;
            Object oRes, oJoueur;
            bool res = false;
            Joueur joueur = null;

            if (sessions != null){
                oRes = sessions["connected"]; //Si la session existe, alors ça veut dire qu'on est connecté, pas besoin de rentrer dans les conditions
                oJoueur = sessions["joueur"];
                if (oRes != null) { res = (bool)oRes; }
                if (oJoueur != null) { joueur = (Joueur)oJoueur; }
            }

            if (res == false || joueur == null){ // Si les sessions n'existent pas, on vérifie si un cookie existe
                if (cookies != null && cookies["user_email"] != null) { 
                    connect = cookies["user_email"].Value;
                }
                if (connect != null){ // SI les cookies existent
                    joueur = ((JoueurDAO)adf.GetJoueurDAO()).Find(connect); //On récupère l'user correspondant au mail
                    sessions["connected"] = true;
                    sessions["joueur"] = joueur; //On crée nos sessions, il est désormais connecté.
                    res = true;
                }
            }
            return res;
        }

        public static int validationEmail(String email){
            int res = 0;
            Regex regex = new Regex("([^.@]+)(\\.[^.@]+)*@([^.@]+\\.)+([^.@]+)");

            if (email != null && !regex.IsMatch(email)){
                res = 2;
            }
            return res;
        }

        public static int validationMdp(String mdp){
            int res = 0;

            if (mdp != null){
                if (mdp.Length < 8)
                    res = 3;
                if (mdp.Length > 16)
                    res = 4;
            }
            return res;
        }
    }
}