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
    public partial class MovingBall : Form
    {
        Bitmap bmp = new Bitmap(512, 512);
        Random r = new Random();
        int x = 256;
        int y = 256;
        byte type = 0;
        int xstep = 5;
        int ystep = 5;
        public MovingBall()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (type == 0)
            {
                x = r.Next(20, 480);
                y = r.Next(20, 480);
            }
            else
            {
                x += xstep;
                y += ystep;

                if (x + 20 >= 512)
                {
                    xstep = -xstep;
                }

                if (y + 20 >= 512)
                {
                    ystep = -ystep;
                }

                if (x <= pictureBox1.Location.X)
                {
                    xstep = -xstep;
                }

                if (y <= pictureBox1.Location.Y -53)
                {
                    ystep = -ystep;
                }
            }

            Graphics g = Graphics.FromImage(bmp);
            
            Rectangle rect = new Rectangle(0, 0, 512, 512);
            BitmapData bd = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int offset = bd.Stride - bmp.Width * 3;

            unsafe
            {
                byte* p = (byte*)bd.Scan0;
                for (int j = 0; j < bmp.Height; j++, p += offset)
                    for (int i = 0; i < bmp.Width; i++, p += 3)
                    {
                        p[0] = Color.SkyBlue.B;
                        p[1] = Color.SkyBlue.G;
                        p[2] = Color.SkyBlue.R;
                    }
            }
            bmp.UnlockBits(bd);

            g.FillEllipse(Brushes.Red,new Rectangle(x,y,20,20));     
            pictureBox1.Image = bmp;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer1.Interval = 500;
            type = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            x = 256; y = 256;
            timer1.Enabled = true;
            timer1.Interval = 100;
            type = 1;
        }
    }
}
