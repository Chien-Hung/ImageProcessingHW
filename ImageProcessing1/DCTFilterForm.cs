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
    public partial class DCTFilterForm : Form
    {
        Bitmap bmp;
        Rectangle rect;
        double[,] mask = new double[8, 8];
        int[,] s;
        int[,] f;
        int w, h;
        MainForm mf;

        public DCTFilterForm(Bitmap b,MainForm m)
        {
            InitializeComponent();
            bmp = b;
            mf = m;
            /*bmp = new Bitmap(256, 256);

            for (int y = 0; y < 256; y++)
                for (int x = 0; x < 256;x++ )
                {
                    bmp.SetPixel(x, y, Color.White);
                }
            */

            pictureBox1.Image = bmp;
            int k = 0;
            for (int j = 0; j < 8; j++)
                for (int i = 0; i < 8; i++)
                {
                    TextBox t = new TextBox();
                    t.Size = new Size(25, 20);
                    t.Text = "1";
                    t.TextAlign = HorizontalAlignment.Right;
                    t.Location = new Point(pictureBox2.Location.X + pictureBox1.Width + 50 + i * 32, pictureBox2.Location.Y + j * 32 + 5);
                    t.MouseDown += new MouseEventHandler(this.MU);
                    this.Controls.Add(t);
                    k++;
                    t.Name = "textbox" + k;
                }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bmp = (Bitmap)pictureBox2.Image.Clone();
            int i = 0, j = 0;

            //抓取textbox 的値
            foreach(var c in this.Controls)
            {
                if(c is TextBox)
                {
                    mask[i, j] = Convert.ToDouble(((TextBox)c).Text);
                    i++;
                    if (i == 8)
                    {
                        i = 0;
                        j++;
                    }
                    /*
                    if((c as TextBox).Name == "textbox2")
                        MessageBox.Show(((TextBox)c).Text);
                    */
                }
            }

            //對頻域濾波
            for (int y = 0; y < 256; y += 8)
                for (int x = 0; x < 256; x += 8) 
                {
                    for (j = 0; j < 8; j++)
                        for (i = 0; i < 8; i++)
                        {
                            //影像部分
                            byte p = Convert.ToByte(Math.Round(bmp.GetPixel(x + i, y + j).R * mask[i, j] * 1.0));
                            bmp.SetPixel(x + i, y + j, Color.FromArgb(p, p, p));

                            //矩陣部分
                            f[x + i, y + j] = Convert.ToInt32(Math.Round(f[x + i, y + j] * mask[i, j] * 1.0));

                        }
                }

            pictureBox3.Image = bmp;
        }

        private void MU(object sender, MouseEventArgs e)
        {
            TextBox t = (TextBox)sender;
            t.Text = "0";
            t.SelectAll();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            Bitmap bn = (Bitmap)pictureBox1.Image.Clone();
            w = bn.Width;
            h = bn.Height;
            rect = new Rectangle(0, 0, w, h);

            BitmapData bnd = bn.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int offset = bnd.Stride - bn.Width * 3;
            double cu, cv;
            double sum;
            int N = 8;
            s = new int[w, h];
            f = new int[w, h];

            unsafe
            {
                byte* p = (byte*)bnd.Scan0;

                for (int y = 0; y < h; y++, p += offset)
                {
                    for (int x = 0; x < w; x++, p += 3)
                    {
                        s[x, y] = p[0] - 128;
                    }
                }
            }

            //DCT
            for (int y = 0; y < h; y += 8)
            {
                for (int x = 0; x < w; x += 8)
                {
                    for (int v = 0; v < 8; v++)
                        for (int u = 0; u < 8; u++)
                        {
                            cu = (u == 0) ? Math.Sqrt(1 / (double)2) : 1;
                            cv = (v == 0) ? Math.Sqrt(1 / (double)2) : 1;
                            sum = 0;
                            for (int j = 0; j < N; j++)
                                for (int i = 0; i < N; i++)
                                {
                                    sum += s[(x + j), (y + i)] * Math.Cos((2 * i + 1) * u * Math.PI / 16) * Math.Cos((2 * j + 1) * v * Math.PI / 16);
                                }
                            f[(x + v), (y + u)] = Convert.ToInt32(Math.Round(cu * cv * sum / 4.0));
                        }
                }
            }

            int max = int.MinValue;
            int min = int.MaxValue;
            unsafe
            {
                byte* p = (byte*)bnd.Scan0;

                for (int y = 0; y < h; y++, p += offset)
                {
                    for (int x = 0; x < w; x++, p += 3)
                    {
                        max = (max < f[x, y]) ? f[x, y] : max;
                        min = (min > f[x, y]) ? f[x, y] : min;
                        byte pix;
                        if (f[x, y] > 255)
                            pix = 255;
                        else if (f[x, y] < 0)
                            pix = 0;
                        else
                            pix = Convert.ToByte(f[x, y]);
                        p[0] = p[1] = p[2] = pix;

                    }
                }
            }
            bn.UnlockBits(bnd);
            
            pictureBox2.Image = bn;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //inverse dct
            double cu, cv, sum;
            int N = 8;
            for (int y = 0; y < h; y += 8)
            {
                for (int x = 0; x < w; x += 8)
                {
                    for (int v = 0; v < 8; v++)
                        for (int u = 0; u < 8; u++)
                        {
                            sum = 0;
                            for (int j = 0; j < N; j++)
                                for (int i = 0; i < N; i++)
                                {
                                    cu = (i == 0) ? Math.Sqrt(1 / (double)2) : 1;
                                    cv = (j == 0) ? Math.Sqrt(1 / (double)2) : 1;
                                    sum += cu * cv * f[(x + j), (y + i)] * Math.Cos((2 * u + 1) * i * Math.PI / 16) * Math.Cos((2 * v + 1) * j * Math.PI / 16);
                                }
                            s[(x + v), (y + u)] = Convert.ToInt32(sum / 4) + 128;
                        }
                }
            }

            Bitmap bb = new Bitmap(w, h);

            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    if (s[x, y] < 0)
                    {
                        s[x, y] = 0;
                    }
                    if (s[x, y] > 255)
                    {
                        s[x, y] = 255;
                    }
                    byte pix = Convert.ToByte(s[x, y]);
                    bb.SetPixel(x, y, Color.FromArgb(pix, pix, pix));
                }

 
            pictureBox4.Image = bb;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text[0] == '1')
            {
                int i = 0, j = 0;
                foreach (var c in this.Controls)
                {
                    if(c is TextBox)
                    {
                        if (i < 3 && j < 3)
                            ((TextBox)c).Text = "1";
                        else
                            ((TextBox)c).Text = "0";

                        i++;
                        if (i >= 8)
                        {
                            j++;
                            i = 0;
                        }
                    }
                }
                
            }
            else if (comboBox1.Text[0] == '2')
            {
                int i = 0, j = 0;
                foreach (var c in this.Controls)
                {
                    if (c is TextBox)
                    {
                        if (i >= 3 || j >= 3)
                            ((TextBox)c).Text = "1";
                        else
                            ((TextBox)c).Text = "0";

                        i++;
                        if (i >= 8)
                        {
                            j++;
                            i = 0;
                        }
                    }
                }
            }
            else if (comboBox1.Text[0] == '3')
            {
                int i = 0, j = 0;
                foreach (var c in this.Controls)
                {
                    if (c is TextBox)
                    {
                        if (i >= 4)
                            ((TextBox)c).Text = "0";
                        else
                            ((TextBox)c).Text = "1";

                        i++;
                        if (i >= 8)
                        {
                            j++;
                            i = 0;
                        }
                    }
                }
            }
            else if (comboBox1.Text[0] == '4')
            {
                int i = 0, j = 0;
                foreach (var c in this.Controls)
                {
                    if (c is TextBox)
                    {
                        if (j >= 4)
                            ((TextBox)c).Text = "0";
                        else
                            ((TextBox)c).Text = "1";

                        i++;
                        if (i >= 8)
                        {
                            j++;
                            i = 0;
                        }
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            PictureBox p = new PictureBox();
            switch(comboBox2.Text[0])
            {
                case '1':
                    p = pictureBox1;
                    break;
                case '2':
                    p = pictureBox2;
                    break;
                case '3':
                    p = pictureBox3;
                    break;
                case '4':
                    p = pictureBox4;
                    break;
                default:
                    p = pictureBox1;
                    break;
            }
            mf.sendimage((Bitmap)p.Image);
        }

    }
}
