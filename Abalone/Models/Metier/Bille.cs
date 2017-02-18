namespace Abalone.Models.Metier
{
    public class Bille
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Bille(){ }

        public Bille(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool EqualsCoordinate(Bille bille)
        {
            return (EqualsX(bille) && EqualsY(bille));
        }
        public bool EqualsCoordinate(int x, int y)
        {
            return (EqualsX(x) && EqualsY(y));
        }

        public bool EqualsY(Bille bille)
        {
            return this.Y == bille.Y;
        }
        public bool EqualsY(int y)
        {
            return this.Y == y;
        }

        public bool EqualsX(Bille bille)
        {
            return this.X == bille.X;
        }
        public bool EqualsX(int x)
        {
            return this.X == x;
        }

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
    }
}