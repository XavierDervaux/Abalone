using Abalone.Models.Metier;

namespace Abalone.Models.Bean{
    public class BMoveResp
    {
        public bMove M { get; set; } = new bMove();
        public Bille[] Origin { get; set; } = new Bille[2];
        public Bille[] Destination { get; set; } = new Bille[2];
    }
}