using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageProcessing
{
    class huffmanNode
    {
        public byte pixel;
        public double amount;
        public byte code;
        public byte parent;
        public byte child;
        public string s;

        public huffmanNode()
        {
            amount = 0;
            pixel = 0;
            code = 0;
            parent = 255;
            child = 255;
            s = "";
        }
    }
}
