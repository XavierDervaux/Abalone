using System;
using System.Collections.Generic;

namespace Abalone.Models{
    public class bMove {
	    private int type = -1; //Le type  de déplacement
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

        public int Type { get; set; }
        public int Ori_x1 { get; set; }
        public int Ori_y1 { get; set; }
        public int Ori_x2 { get; set; }
        public int Ori_y2 { get; set; }
        public int Ori_x3 { get; set; }
        public int Ori_y3 { get; set; }
        public int Des_x1 { get; set; }
        public int Des_y1 { get; set; }
        public int Des_x2 { get; set; }
        public int Des_y2 { get; set; }
        public int Des_x3 { get; set; }
        public int Des_y3 { get; set; }
    }
}