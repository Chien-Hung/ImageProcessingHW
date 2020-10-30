using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace ImageProcessing
{
    public partial class histogramForm : Form
    {
        Bitmap bmp;
        ulong[,] pixelnumber = new ulong[4, 256];
        byte[] max = new byte[4];

        public histogramForm(Bitmap bmpN)
        {
            InitializeComponent();
            bmp = bmpN;
        }

        private void histogramForm_Load(object sender, EventArgs e)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

            BitmapData bmpd = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            int stride = bmpd.Stride;
            int offset = stride - bmp.Width * 3;

            unsafe
            {
                byte* p = (byte*)bmpd.Scan0.ToPointer();
                

                for (int y = 0; y < bmp.Height; y++, p += offset)
                    for (int x = 0; x < bmp.Width; x++, p += 3) 
                    {
                        byte g = Convert.ToByte(0.114 * p[0] + 0.587 * p[1] + 0.299 * p[2]);
                        pixelnumber[0, p[2]]++;  //R
                        pixelnumber[1, p[1]]++;  //G
                        pixelnumber[2, p[0]]++;  //B
                        pixelnumber[3, g]++; //Gray

                        if (pixelnumber[0, p[2]] > pixelnumber[0, max[0]])
                            max[0] = p[2];
                        if (pixelnumber[1, p[1]] > pixelnumber[1, max[1]])
                            max[1] = p[1];
                        if (pixelnumber[2, p[0]] > pixelnumber[2, max[2]])
                            max[2] = p[0];
                        if (pixelnumber[3, g] > pixelnumber[3, max[3]])
                            max[3] = g;
                    }
            }
            bmp.UnlockBits(bmpd);

        }

        private void histogramForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen curPen =Pens.Red;
            byte type = 0;

            if (radioButton1.Checked)
            {
                curPen = Pens.Red;
                type = 0;
            }
            else if (radioButton2.Checked)
            {
                curPen = Pens.Green;
                type = 1;
            }
            else if (radioButton3.Checked)
            {
                curPen = Pens.Blue;
                type = 2;
            }
            else if (radioButton4.Checked)
            {
                curPen = Pens.Gray;
                type = 3;
            }

            g.DrawLine(curPen, 50, 290, 320, 290);      //橫座標軸
            g.DrawLine(curPen, 50, 290, 50, 80);        //縱座標軸
            
            //橫座標軸刻度
            g.DrawLine(curPen, 100, 290, 100, 292);
            g.DrawLine(curPen, 150, 290, 150, 292);
            g.DrawLine(curPen, 200, 290, 200, 292);
            g.DrawLine(curPen, 250, 290, 250, 292);
            g.DrawLine(curPen, 300, 290, 300, 292);
            g.DrawString("0", new Font("New Timer", 8), Brushes.Black, new PointF(46, 292));
            g.DrawString("50", new Font("New Timer", 8), Brushes.Black, new PointF(92, 292));
            g.DrawString("100", new Font("New Timer", 8), Brushes.Black, new PointF(139, 292));
            g.DrawString("150", new Font("New Timer", 8), Brushes.Black, new PointF(189, 292));
            g.DrawString("200", new Font("New Timer", 8), Brushes.Black, new PointF(239, 292));
            g.DrawString("250", new Font("New Timer", 8), Brushes.Black, new PointF(289, 292));
            g.DrawString("Pixel", new Font("New Timer", 8), Brushes.Black, new PointF(320, 292));
            g.DrawLine(curPen, 48, 80, 50, 80);
            g.DrawString(pixelnumber[type, max[type]].ToString(), new Font("New Timer", 8), Brushes.Black, new PointF(18, 88));
            
            
            double temp = 0;
            DataGridViewRowCollection rows = dataGridView1.Rows;
            rows.Clear();
            for (int i = 0; i < 256; i++)
            {
                temp = 200.0 * pixelnumber[type, i] / pixelnumber[type, max[type]];

                g.DrawLine(curPen, 50 + i, 290, 50 + i, 290 - (int)temp);
                rows.Add(new Object[] { i, pixelnumber[0, i], pixelnumber[1, i], pixelnumber[2, i], pixelnumber[3, i] });
            }

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }
    }
}
