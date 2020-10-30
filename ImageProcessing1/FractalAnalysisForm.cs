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
    public partial class FractalAnalysisForm : Form
    {
        string filename;
        bool selectR = false;
        bool selectD = false;
        Bitmap range = new Bitmap(8, 8);
        Bitmap tdomain = new Bitmap(8, 8);
        double[,] td = new double[8, 8];
        Bitmap domain = new Bitmap(16, 16);
        Bitmap bmp;

        public FractalAnalysisForm()
        {
            InitializeComponent();
            /*
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filename = ofd.FileName;
                bmp = new Bitmap(ofd.FileName);
                pictureBox1.Image = bmp;
            }
            */
            filename = @"C:\Users\adm\Desktop\IMAGE\512xx.bmp";
            bmp = new Bitmap(filename);
            pictureBox1.Image = bmp;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                selectR = true;
                selectD = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                selectD = true;
                selectR = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filename = ofd.FileName;
                bmp = new Bitmap(ofd.FileName);
                pictureBox1.Image = bmp;
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (selectR == true)
            {
                textBox1.Text = "" + (e.X / 8  + e.Y / 8 * 64);
                for (int j = 0; j < 8; j++)
                    for (int i = 0; i < 8; i++)
                    {
                        range.SetPixel(i, j, bmp.GetPixel(e.X / 8 * 8 + i, e.Y / 8 * 8 + j));
                    }
                pictureBox2.Image = scaleImage(range, 10);
                selectR = false;

                Graphics g = pictureBox1.CreateGraphics();
                g.DrawRectangle(Pens.Red, new Rectangle(e.X / 8 * 8, e.Y / 8 * 8, 8, 8));
            }
            
            if (selectD == true)
            {
                if (e.X - 8 >= 0 && e.Y - 8 >= 0 && e.X + 7 < bmp.Width && e.Y + 7 < bmp.Height)
                {
                    textBox2.Text = "" + (e.X - 8);
                    textBox3.Text = "" + (e.Y - 8);
                    for (int j = 0; j < 16; j++)
                        for (int i = 0; i < 16; i++)
                        {
                            domain.SetPixel(i, j, bmp.GetPixel(e.X - 8 + i, e.Y - 8 + j));
                        }

                    for (int j = 0; j < 8; j++)
                        for (int i = 0; i < 8; i++)
                        {
                            double r = (domain.GetPixel(2 * i, 2 * j).R + domain.GetPixel(2 * i + 1, 2 * j).R + domain.GetPixel(2 * i, 2 * j + 1).R + domain.GetPixel(2 * i + 1, 2 * j + 1).R) / 4;
                            int g = (domain.GetPixel(2 * i, 2 * j).G + domain.GetPixel(2 * i + 1, 2 * j).G + domain.GetPixel(2 * i, 2 * j + 1).G + domain.GetPixel(2 * i + 1, 2 * j + 1).G) / 4;
                            int b = (domain.GetPixel(2 * i, 2 * j).B + domain.GetPixel(2 * i + 1, 2 * j).B + domain.GetPixel(2 * i, 2 * j + 1).B + domain.GetPixel(2 * i + 1, 2 * j + 1).B) / 4;
                            td[i, j] = r;
                            Color c = Color.FromArgb((int)r,g,b);
                            tdomain.SetPixel(i, j, c);
                        }

                    pictureBox4.Image = scaleImage(tdomain, 10);
                    pictureBox3.Image = scaleImage(domain, 10);
                    selectD = false;

                    Graphics g2 = pictureBox1.CreateGraphics();
                    g2.DrawRectangle(Pens.Blue, new Rectangle(e.X - 8, e.Y - 8, 16, 16));
                }
                
            }
        }

        private Bitmap scaleImage(Bitmap b,int num)
        {
            Bitmap sbmp = new Bitmap(b.Width * num, b.Height * num);

            for (int sj = 0; sj < sbmp.Height; sj++)
                for (int si = 0; si < sbmp.Width; si++)
                {
                    sbmp.SetPixel(si, sj, Color.Red);
                }
            for (int j = 0; j < b.Height; j++)
                for (int i = 0; i < b.Width; i++)
                {
                    for (int sj = 0; sj < num; sj++) 
                        for (int si = 0; si < num; si++)
                        {
                            sbmp.SetPixel(i * 10 + si, j * 10 + sj, b.GetPixel(i, j));
                        }
                }
            return sbmp;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Coordinate.Text = "( X , Y ) = ( " + e.X + " , " + e.Y + " )";
            if (selectR == true)
            {
                
                Bitmap b = new Bitmap(Properties.Resources.cursor);
                Color co = Color.FromArgb(0, 0, 0, 0);
                for (int j = 0; j < b.Height; j++)
                    for (int i = 0; i < b.Width; i++)
                    {
                        if (b.GetPixel(i, j).B == 255)
                        {
                            b.SetPixel(i, j, co);
                        }
                    }
                Cursor c = new Cursor(b.GetHicon());
                this.Cursor = c;
                      
            }
            else if (selectD == true)
            {
                Bitmap b = new Bitmap(Properties.Resources.cursor2);
                Color co = Color.FromArgb(0, 0, 0, 0);
                for (int j = 0; j < b.Height; j++)
                    for (int i = 0; i < b.Width; i++)
                    {
                        if (b.GetPixel(i, j).R == 255)
                        {
                            b.SetPixel(i, j, co);
                        }
                    }
                Cursor c = new Cursor(b.GetHicon());
                this.Cursor = c;
            }
            else
                this.Cursor = Cursors.Default;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            if (Cursor != Cursors.Default)
                Cursor = Cursors.Default;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            double error = 0;
            double sa = 0;
            double sb = 0;
            double saa = 0;
            double sab = 0;
            Bitmap temp = new Bitmap(8, 8);
            temp = tdomain.Clone(new Rectangle(0, 0, 8, 8), tdomain.PixelFormat);
            double[,] tempd = new double[8, 8];

            switch (comboBox1.Text[0])
            {
                case '0':
                    for (int j = 0; j < 8; j++)
                        for (int i = 0; i < 8; i++)
                        {
                            temp.SetPixel(i, j, tdomain.GetPixel(i, j));
                            tempd[i, j] = td[i, j];
                        }
                    break;
                case '1':
                    for (int j = 0; j < 8; j++)
                        for (int i = 0; i < 8; i++)
                        {
                            temp.SetPixel(i, j, tdomain.GetPixel(j, 8 - 1 - i));
                            tempd[i, j] = td[j, 8 - 1 - i];
                        }
                    break;
                case '2':
                    for (int j = 0; j < 8; j++)
                        for (int i = 0; i < 8; i++)
                        {
                            temp.SetPixel(i, j, tdomain.GetPixel(8 - 1 - j, i));
                            tempd[i, j] = td[8 - 1 - j, i];
                        }
                    break;
                case '3':
                    for (int j = 0; j < 8; j++)
                        for (int i = 0; i < 8; i++)
                        {
                            temp.SetPixel(i, j, tdomain.GetPixel(8 - 1 - i, 8 - 1 - j));
                            tempd[i, j] = td[8 - 1 - i, 8 - 1 - j];
                        }
                    break;
                case '4':
                    for (int j = 0; j < 8; j++)
                        for (int i = 0; i < 8; i++)
                        {
                            temp.SetPixel(i, j, tdomain.GetPixel(i, 8 - 1 - j));
                            tempd[i, j] = td[i, 8 - 1 - j];
                        }
                    break;
                case '5':
                    for (int j = 0; j < 8; j++)
                        for (int i = 0; i < 8; i++)
                        {
                            temp.SetPixel(i, j, tdomain.GetPixel(8 - 1 - i, j));
                            tempd[i, j] = td[8 - 1 - i, j];
                        }
                    break;
                case '6':
                    for (int j = 0; j < 8; j++)
                        for (int i = 0; i < 8; i++)
                        {
                            temp.SetPixel(i, j, tdomain.GetPixel(j, i));
                            tempd[i, j] = td[j, i];
                        }
                    break;
                case '7':
                    for (int j = 0; j < 8; j++)
                        for (int i = 0; i < 8; i++)
                        {
                            temp.SetPixel(i, j, tdomain.GetPixel(8 - 1 - j, 8 - 1 - i));
                            tempd[i, j] = td[8 - 1 - j, 8 - 1 - i];
                        }
                    break;
            }
            pictureBox4.Image = scaleImage(temp, 10);

            Graphics g = pictureBox5.CreateGraphics();
            g.Clear(pictureBox5.BackColor);
            g.DrawLine(Pens.Blue, 0, 0, 0, 255);
            g.DrawLine(Pens.Blue, 0, 255, 255, 255);

            for (int j = 0; j < 8; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    double b = range.GetPixel(i, j).R;
                    double a = tempd[i, j];

                    error += Math.Pow(b - a, 2);
                    sa += a;
                    sb += b;
                    sab += a * b;
                    saa += a * a;

                    textBox4.Text += " " + a;
                    textBox5.Text += " " + b;

                    g.DrawEllipse(Pens.Red, (int)a - 1, (int)b + 1, 3, 3);

                }
                textBox4.Text += "\r\n";
                textBox5.Text += "\r\n";
            }

            

            float s = (float)((64 * sab - sa * sb) / (64 * saa - sa * sa));
            float o = (float)((sb - s * sa) / 64);
            g.DrawLine(Pens.Green, 0, 255 - (0 * s + o), 255, 255 - (255 * s + o));
            label4.Text = "Error = " + error;
            label5.Text = "n * sab = " + 64 * sab;
            label6.Text = "sa * sb = " + sa * sb;
            label9.Text = "n * saa - sa * sa = " + (64 * saa - sa * sa);
            label7.Text = "S = " + s;
            label8.Text = "O = " + o;
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != null)
            {
                int x = Convert.ToInt32(textBox1.Text) * 8 % 512;
                int y = (Convert.ToInt32(textBox1.Text) / 64) * 8;
                Graphics g = pictureBox1.CreateGraphics();
                g.DrawRectangle(Pens.Red, new Rectangle(x, y, 8, 8));

                byte[,] range2 = new byte[8,8];
                for (int j = 0; j < 8; j++)
                    for (int i = 0; i < 8; i++)
                    {
                        range2[i, j] = bmp.GetPixel(x + i, y + j).R;
                        range.SetPixel(i, j, bmp.GetPixel(x + i, y + j));
                    }
                pictureBox2.Image = scaleImage(range, 10);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != null && textBox3.Text != null)
            {
                int x = Convert.ToInt32(textBox2.Text);
                int y = Convert.ToInt32(textBox3.Text);


                for (int j = 0; j < 16; j++)
                    for (int i = 0; i < 16; i++)
                    {
                        domain.SetPixel(i, j, bmp.GetPixel(x + i, y + j));
                    }

                for (int j = 0; j < 8; j++)
                    for (int i = 0; i < 8; i++)
                    {
                        double r = (domain.GetPixel(2 * i, 2 * j).R + domain.GetPixel(2 * i + 1, 2 * j).R + domain.GetPixel(2 * i, 2 * j + 1).R + domain.GetPixel(2 * i + 1, 2 * j + 1).R) / 4.0;
                        int g = (domain.GetPixel(2 * i, 2 * j).G + domain.GetPixel(2 * i + 1, 2 * j).G + domain.GetPixel(2 * i, 2 * j + 1).G + domain.GetPixel(2 * i + 1, 2 * j + 1).G) / 4;
                        int b = (domain.GetPixel(2 * i, 2 * j).B + domain.GetPixel(2 * i + 1, 2 * j).B + domain.GetPixel(2 * i, 2 * j + 1).B + domain.GetPixel(2 * i + 1, 2 * j + 1).B) / 4;
                        td[i, j] = r;
                        Color c = Color.FromArgb((int)r, g, b);
                        tdomain.SetPixel(i, j, c);
                    }

                pictureBox4.Image = scaleImage(tdomain, 10);
                pictureBox3.Image = scaleImage(domain, 10);
                selectD = false;

                Graphics g2 = pictureBox1.CreateGraphics();
                g2.DrawRectangle(Pens.Blue, new Rectangle(x, y, 16, 16));
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = new Bitmap(filename);
        }

    }
}
