namespace Abalone.Models.Metier
{
    public class Bille
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Bille(){ }

        public Bille(int x = -1, int y = -1)
        {
            X = x;
            Y = y;
        }

        public bool EqualsCoordinate(Bille bille) => (EqualsX(bille) && EqualsY(bille));
        public bool EqualsCoordinate(int x, int y) => (EqualsX(x) && EqualsY(y));
        public bool EqualsY(Bille bille) => Y == bille.Y;
        public bool EqualsY(int y) => Y == y;
        public bool EqualsX(Bille bille) => X == bille.X;
        public bool EqualsX(int x) => X == x;

        public bool IsVerticalRight(Bille bille)
        {
            return (EqualsCoordinate(bille.X - 1, bille.Y + 1) || EqualsCoordinate(bille.X + 1, bille.Y - 1));
        }
        public bool IsVerticalRight(int x, int y)
        {
            return (EqualsCoordinate(x - 1, y + 1) || EqualsCoordinate(x + 1, y - 1));
        }

        public bool IsVerticalLeft(Bille bille)
        {
            return (EqualsCoordinate(bille.X + 1, bille.Y + 1) || EqualsCoordinate(bille.X - 1, bille.Y - 1));
        }
        public bool IsVerticalLeft(int x, int y)
        {
            return (EqualsCoordinate(x + 1, y + 1) || EqualsCoordinate(x - 1, y - 1));
        }

        #region static
        public static void VecteurInit(Bille[] bille, int val = -1) 
        {
            for (int i = 0; i < bille.Length; i++)
            {
                bille[i] = new Bille(val, val);
            } 
        }

        #endregion

        #region operators
        public static bool operator < (Bille b1, int val) => (b1.X < val && b1.Y < val);
        public static bool operator <= (Bille b1, int val) => (b1.X <= val && b1.Y <= val);
        public static bool operator > (Bille b1, int val) => (b1.X > val && b1.Y > val);
        public static bool operator >= (Bille b1, int val) => (b1.X >= val && b1.Y >= val);

        public static bool operator == (Bille b1, Bille b2)
        {
            if (b1 != null && b2 != null)
                return (b1.X == b2.X && b1.Y == b2.Y);
            return false;
        }

        public static bool operator != (Bille b1, Bille b2)
        {
            if (b1 != null && b2 != null)
                return (b1.X != b2.X && b1.Y != b2.Y);
            return false;
        }
        #endregion

        public override bool Equals(object obj)
        {
            if (obj != null)
                if(obj is Bille)
                    return GetHashCode() == ((Bille) obj).GetHashCode();
            return false;
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}