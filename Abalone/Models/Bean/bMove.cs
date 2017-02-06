using System;
using System.Collections.Generic;

namespace Abalone.Models{
    public class bMove {
	    private int type   = -1; //Le type  de déplacement
	    private int ori_x1 = -1;
	    private int ori_y1 = -1;
	    private int ori_x2 = -1;
	    private int ori_y2 = -1;
	    private int ori_x3 = -1;
	    private int ori_y3 = -1;
	    private int des_x1 = -1;
	    private int des_y1 = -1;
	    private int des_x2 = -1;
	    private int des_y2 = -1;
	    private int des_x3 = -1;
	    private int des_y3 = -1;

        public int Type   { get { return this.type; }   set { this.type   = value; } }
        public int Ori_x1 { get { return this.ori_x1; } set { this.ori_x1 = value; } }
        public int Ori_y1 { get { return this.ori_y1; } set { this.ori_y1 = value; } }
        public int Ori_x2 { get { return this.ori_x2; } set { this.ori_x2 = value; } }
        public int Ori_y2 { get { return this.ori_y2; } set { this.ori_y2 = value; } }
        public int Ori_x3 { get { return this.ori_x3; } set { this.ori_x3 = value; } }
        public int Ori_y3 { get { return this.ori_y3; } set { this.ori_y3 = value; } }
        public int Des_x1 { get { return this.des_x1; } set { this.des_x1 = value; } }
        public int Des_y1 { get { return this.des_y1; } set { this.des_y1 = value; } }
        public int Des_x2 { get { return this.des_x2; } set { this.des_x2 = value; } }
        public int Des_y2 { get { return this.des_y2; } set { this.des_y2 = value; } }
        public int Des_x3 { get { return this.des_x3; } set { this.des_x3 = value; } }
        public int Des_y3 { get { return this.des_y3; } set { this.des_y3 = value; } }
    }
}