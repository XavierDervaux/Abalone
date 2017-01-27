namespace Abalone.Models{
    public class bMoveResp {
	    private bMove m = new bMove();
	    private int ori_x4 = -1;
	    private int ori_y4 = -1;
	    private int ori_x5 = -1;
	    private int ori_y5 = -1;
	    private int des_x4 = -1;
	    private int des_y4 = -1;
	    private int des_x5 = -1;
	    private int des_y5 = -1;

        public bMove M { get; set; }
        public int Ori_x4 { get; set; }
        public int Ori_y4 { get; set; }
        public int Ori_x5 { get; set; }
        public int Ori_y5 { get; set; }
        public int Des_x4 { get; set; }
        public int Des_y4 { get; set; }
        public int Des_x5 { get; set; }
        public int Des_y5 { get; set; }
    }
}