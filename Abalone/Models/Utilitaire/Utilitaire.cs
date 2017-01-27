using System;
using System.Text;
using System.Web;

namespace Abalone.Models{
    public class Utilitaire {
	    public static String CryptPassword(string password){
            System.Security.Cryptography.SHA256Managed crypt = new System.Security.Cryptography.SHA256Managed();
            StringBuilder hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

	    public static int BoolToInt(bool b) {
		    int res = 0;
		
		    if(b == true){
			    res = 1;
		    }
		    return res;
	    }
	
	    public static bool IntToBool(int i) {
		    return i>0;
	    }
	
        public static void SetCookie(HttpCookieCollection cookies, string nom, string valeur, int maxAge ) {
            HttpCookie cookie = new HttpCookie(nom, valeur);
            cookie.Expires = DateTime.Now.Add(new TimeSpan(0, 0, maxAge));
            cookies.Add(cookie);
        }
    }
}
