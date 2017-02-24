using Abalone.Models.Metier;

namespace Abalone.Models
{
    public class bMove
    {
        public Bille[] Origin { get; set; } = new Bille[3];
        public Bille[] Destination { get; set; } = new Bille[3];
        public int Type { get; set; } = -1;
    }
}