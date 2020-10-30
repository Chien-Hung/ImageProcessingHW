using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageProcessing
{
    class bmpheader
    {
        //BITMAPFILEHEADER
        public short identity;
        public int file_size;
        public short reserved1;
        public short reserved2;
        public int data_offset;

        //BITMAPINFOHEADER 
        public int header_size;
        public int width;
        public int height;
        public short planes;
        public short bit_per_pixel;
        public int compression;
        public int data_size;
        public int hresolution;
        public int vresolution;
        public int used_colors;
        public int important_colors;
        
        //PALLETTE 
        public char blue;
        public char green;
        public char red;
        public char reserved;


        public bmpheader()
        {
            identity = Convert.ToInt16("0100001001001101", 2);
        }
    }
}
