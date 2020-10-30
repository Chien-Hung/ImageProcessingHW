using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageProcessing
{
    public partial class CDFForm : Form
    {
        ulong[,] pixel = new ulong[256,2];
        int maxindex = 0;
        public CDFForm(ulong[] pixelnumber)
        {
            InitializeComponent();

            this.Text = "Cumlative Distribution Function";

            for (int i = 0; i < 256; i++)
            {
                pixel[i, 0] = pixel[i, 1] = pixelnumber[i];
                if (pixel[i, 0] > pixel[maxindex, 0])
                    maxindex = i;
            }
            for (int i = 1; i < 256; i++)
                pixel[i, 1] = pixel[i - 1, 1] + pixel[i, 1];

        }

        private void CDFForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen curPen = new Pen(Brushes.Gray, 1);
            ulong maxPixel = pixel[255, 1];

            g.DrawLine(curPen, 50, 240, 320, 240);
            g.DrawLine(curPen, 50, 240, 50, 30);
            
            g.DrawLine(curPen, 100, 240, 100, 242);
            g.DrawLine(curPen, 150, 240, 150, 242);
            g.DrawLine(curPen, 200, 240, 200, 242);
            g.DrawLine(curPen, 250, 240, 250, 242);
            g.DrawLine(curPen, 300, 240, 300, 242);
            g.DrawString("0", new Font("New Timer", 8), Brushes.Gray, new PointF(46, 242));
            g.DrawString("50", new Font("New Timer", 8), Brushes.Gray, new PointF(92, 242));
            g.DrawString("100", new Font("New Timer", 8), Brushes.Gray, new PointF(139, 242));
            g.DrawString("150", new Font("New Timer", 8), Brushes.Gray, new PointF(189, 242));
            g.DrawString("200", new Font("New Timer", 8), Brushes.Gray, new PointF(239, 242));
            g.DrawString("250", new Font("New Timer", 8), Brushes.Gray, new PointF(289, 242));
            
            //標最大值(後來因有兩個不同圖的最大值而沒標設)
            //g.DrawLine(curPen, 48, 40, 50, 40);
            //g.DrawString(maxPixel.ToString(), new Font("New Timer", 8), Brushes.Gray, new PointF(5, 38));

            
            
            double temp = 0;
            int last = 0;
            curPen.Color = Color.Red;
            for (int i = 0; i < 256; i++)
            {
                temp = 200.0 * pixel[i,1] / maxPixel;
                g.DrawLine(Pens.Red, 50 + i, 240, 50 + i, 240 - (int)(200 * pixel[i, 0] / pixel[maxindex,0]));

                if(last!=0)
                {
                    g.DrawLine(Pens.Black, 50 + i, last, 50 + i, 240 - (int)temp);
                }
                g.FillRectangle(Brushes.Black, 50 + i, 240 - (int)temp, 1, 1);
  
                last = 240 - (int)temp;
            }
            
            curPen.Dispose(); 
        }
    }
}
