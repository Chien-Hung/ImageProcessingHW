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
    public partial class YCrCbForm : Form
    {
        MainForm mf;
        Bitmap bmp;
        Bitmap YCbCr;
        Rectangle rect;
        int w, h;

        public YCrCbForm(MainForm m, Bitmap o)
        {
            InitializeComponent();

            mf = m;
            bmp = (Bitmap)o.Clone();
            pictureBox1.Image = bmp;
            w = bmp.Width;
            h = bmp.Height;
            YCbCr = new Bitmap(w, h);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap bn = new Bitmap(w, h);
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    byte r = bmp.GetPixel(x, y).R;
                    byte g = bmp.GetPixel(x, y).G;
                    byte b = bmp.GetPixel(x, y).B;
                    int yy = (int)(0.299 * r + 0.587 * g + 0.114 * b);
                    int cb = (int)(-0.169 * r - 0.331 * g + 0.5 * b + 128);
                    int cr = (int)(0.5 * r - 0.419 * g - 0.081 * b + 128);

                    Color c = Color.FromArgb(yy, cr, cb);
                    YCbCr.SetPixel(x, y, c);
                }

            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    bn.SetPixel(x, y, Color.FromArgb(YCbCr.GetPixel(x, y).R, YCbCr.GetPixel(x, y).R, YCbCr.GetPixel(x, y).R));
                }

            pictureBox2.Image = bn;

            bn = new Bitmap(w, h);
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    bn.SetPixel(x, y, Color.FromArgb(YCbCr.GetPixel(x, y).G, YCbCr.GetPixel(x, y).G, YCbCr.GetPixel(x, y).G));
                }

            pictureBox3.Image = bn;

            bn = new Bitmap(w, h);
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    bn.SetPixel(x, y, Color.FromArgb(YCbCr.GetPixel(x, y).B, YCbCr.GetPixel(x, y).B, YCbCr.GetPixel(x, y).B));
                }

            pictureBox4.Image = bn;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap bn = new Bitmap(w, h);

            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    byte yy = ((Bitmap)pictureBox2.Image).GetPixel(x, y).R;
                    byte cb = ((Bitmap)pictureBox3.Image).GetPixel(x, y).G;
                    byte cr = ((Bitmap)pictureBox4.Image).GetPixel(x, y).B;
                    int r = Convert.ToInt32(yy + 1.402 * (cr - 128));
                    int g = Convert.ToInt32(yy - 0.344 * (cb - 128) - 0.714 * (cr - 128));
                    int b = Convert.ToInt32(yy + 1.772 * (cb - 128));

                    if (r > 255)
                        r = 255;
                    else if (r < 0)
                        r = 0;
                    if (g > 255)
                        g = 255;
                    else if (g < 0)
                        g = 0;
                    if (b > 255)
                        b = 255;
                    else if (b < 0)
                        b = 0;

                    Color c = Color.FromArgb(b, g, r);
                    bn.SetPixel(x, y, c);
                }
            mf.sendimage(bn);
            pictureBox5.Image = bn;
        }


    }
}
