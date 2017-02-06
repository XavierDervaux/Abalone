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

        public bMove M    { get { return this.m; }      set { this.m = value; }      }
        public int Ori_x4 { get { return this.ori_x4; } set { this.ori_x4 = value; } }
        public int Ori_y4 { get { return this.ori_y4; } set { this.ori_y4 = value; } }
        public int Ori_x5 { get { return this.ori_x5; } set { this.ori_x5 = value; } }
        public int Ori_y5 { get { return this.ori_y5; } set { this.ori_y5 = value; } }
        public int Des_x4 { get { return this.des_x4; } set { this.des_x4 = value; } }
        public int Des_y4 { get { return this.des_y4; } set { this.des_y4 = value; } }
        public int Des_x5 { get { return this.des_x5; } set { this.des_x5 = value; } }
        public int Des_y5 { get { return this.des_y5; } set { this.des_y5 = value; } }
    }
}