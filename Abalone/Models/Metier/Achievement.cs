using System;
using System.Collections.Generic;

namespace Abalone.Models{
    public class Achievement : IEquatable<Achievement>
    { 
        private int id = 0;
        private String titre = null;
        private String nom = null;
        private String description = null;


    // Constructeurs
    //---------------------------------------------------	
        public Achievement(String titre, String nom, String description){
            this.titre = titre;
            this.nom = nom;
            this.description = description;
        }
        public Achievement(int id, String titre, String nom, String description){
            this.id = id;
            this.titre = titre;
            this.nom = nom;
            this.description = description;
        }


    // Getter / Setter
    //---------------------------------------------------	
        public int Id {             get { return this.id; }          set { this.id = value; }           }
        public String Titre {       get { return this.titre; }       set { this.titre = value; }        }
        public String Nom {         get { return this.nom; }         set { this.nom = value; }          }
        public String Description { get { return this.description; } set { this.description = value; }  }


    // Méthodes publiques
    //---------------------------------------------------	
        public bool CreateBDD(){
            DAOFactory adf = (DAOFactory)AbstractDAOFactory.GetFactory(0);
            return adf.GetAchievementDAO().Create(this);
        }

        public static List<Achievement> FindAllBDD(){
            DAOFactory adf = (DAOFactory)AbstractDAOFactory.GetFactory(0);
            List<Achievement> tmp = adf.GetAchievementDAO().GetAll();
            return tmp;
        }


    // Méthode statiques
    //---------------------------------------------------	
        public static void ACV_FIRST_WIN(Joueur j){ //Gagner une partie
            DAOFactory adf = (DAOFactory)AbstractDAOFactory.GetFactory(0);
            const int n = 1;
            List<Historique> listH = Historique.FindAllBDD(j);
            int c = listH.Count;

            if (c >= 100) { ACV_HUNDRED_WIN(j); }
            if (c >= 10)  { ACV_TEN_WIN(j); }
            if (c >= 1)   {
                if (!(j.PossedeAchievement(n))){ //S'il ne possède PAS le succes
                    ((AchievJoueurDAO)adf.GetAchievJoueurDAO()).Create(j.Id, n); //On ajoute l'achievement au joueur
                }
            }
        }

        public static void ACV_TEN_WIN(Joueur j){ //Gagner 10 parties
            DAOFactory adf = (DAOFactory)AbstractDAOFactory.GetFactory(0);
            const int n = 2;
            if (!(j.PossedeAchievement(n))){ //On vérifie les conditions dans ACV_FIRST_WIN, il reste juste à vérifier qu'il ne l'a pas déjà
                ((AchievJoueurDAO)adf.GetAchievJoueurDAO()).Create(j.Id, n); //On ajoute l'achievement au joueur
            }
        }

        public static void ACV_HUNDRED_WIN(Joueur j){ //Gagner 100 parties
            DAOFactory adf = (DAOFactory)AbstractDAOFactory.GetFactory(0);
            const int n = 3;
            if (!(j.PossedeAchievement(n))){ //On vérifie les conditions dans ACV_FIRST_WIN, il reste juste à vérifier qu'il ne l'a pas déjà
                ((AchievJoueurDAO)adf.GetAchievJoueurDAO()).Create(j.Id, n); //On ajoute l'achievement au joueur
            }
        }

        public static void ACV_PERFECT(Joueur j, int s1, int s2){ //Gagner 6-0
            DAOFactory adf = (DAOFactory)AbstractDAOFactory.GetFactory(0);
            const int n = 4;
            if (!(j.PossedeAchievement(n))){ //S'il ne possède PAS le succes, on vérifie s'il remplis les conditions.
                if (s1 == 6 && s2 == 0){
                    ((AchievJoueurDAO)adf.GetAchievJoueurDAO()).Create(j.Id, n); //On ajoute l'achievement au joueur
                }
            }
        }

        public static void ACV_SIX_FIVE(Joueur j, int s1, int s2){ //Gagner 6-5
            DAOFactory adf = (DAOFactory)AbstractDAOFactory.GetFactory(0);
            const int n = 5;
            if (!(j.PossedeAchievement(n))){ //S'il ne possède PAS le succes, on vérifie s'il remplis les conditions.
                if (s1 == 6 && s2 == 5){
                    ((AchievJoueurDAO)adf.GetAchievJoueurDAO()).Create(j.Id, n); //On ajoute l'achievement au joueur
                }
            }
        }

        public static void ACV_SURRENDER(Joueur j){ //Gagner par abandon
            DAOFactory adf = (DAOFactory)AbstractDAOFactory.GetFactory(0);
            const int n = 6;
            if (!(j.PossedeAchievement(n))){ //N'est appellé qu'en cas d'abandon, rien d'autre à vérifier
                ((AchievJoueurDAO)adf.GetAchievJoueurDAO()).Create(j.Id, n); //On ajoute l'achievement au joueur
            }
        }

        public static void ACV_COMBO_2(Joueur j, int combo){ //Prendre 2 billes de suite 
            DAOFactory adf = (DAOFactory)AbstractDAOFactory.GetFactory(0);
            const int n = 7;

            if (combo >= 4) { ACV_COMBO_4(j); }
            if (combo >= 3) { ACV_COMBO_3(j); }
            if (combo >= 2) {
                if (!(j.PossedeAchievement(n))){ //S'il ne possède PAS le succes
                    ((AchievJoueurDAO)adf.GetAchievJoueurDAO()).Create(j.Id, n); //On ajoute l'achievement au joueur
                }
            }
        }

        public static void ACV_COMBO_3(Joueur j){ //Prendre 3 billes de suite
            DAOFactory adf = (DAOFactory)AbstractDAOFactory.GetFactory(0);
            const int n = 8;
            if (!(j.PossedeAchievement(n))){ //Conditions déjà gerée dans combo 2
                ((AchievJoueurDAO)adf.GetAchievJoueurDAO()).Create(j.Id, n); //On ajoute l'achievement au joueur
            }
        }

        public static void ACV_COMBO_4(Joueur j){ //Prendre 4 billes de suite
            DAOFactory adf = (DAOFactory)AbstractDAOFactory.GetFactory(0);
            const int n = 9;
            if (!(j.PossedeAchievement(n))){ ///Conditions déjà gerée dans combo 2
                ((AchievJoueurDAO)adf.GetAchievJoueurDAO()).Create(j.Id, n); //On ajoute l'achievement au joueur
            }
        }


    // Debug
    //---------------------------------------------------	
        public override string ToString(){
            return "Achievement [id=" + id + ", titre=" + titre + ", nom=" + nom + ", description=" + description + "]";
        }
        public int CompareTo(Achievement a){
            int res = 0;

            if (a.Id > this.id){
                res = 1;
            } else if (a.Id < this.id) {
                res = -1;
            }
            return res;
        }

        public bool Equals(Achievement a){
            bool res = false;

            if(this.id          == a.Id 
            && this.titre       == a.Titre
            && this.nom         == a.Nom
            && this.description == a.Description) {
                res = true;
            }

            return res;
        }
    }
}