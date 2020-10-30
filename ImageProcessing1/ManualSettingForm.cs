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
    public partial class ManualSettingForm : Form
    {
        Bitmap b;
        Bitmap ob;
        int threshold = 0;
        MainForm mf = null;
        int type;
        public ManualSettingForm(Bitmap bmp,MainForm mainform,int i)
        {
            InitializeComponent();
            b = bmp;
            ob = b.Clone(new Rectangle(0, 0, b.Width, b.Height), PixelFormat.Format24bppRgb);
            pictureBox1.Image = b;
            mf = mainform;
            type = i;

            trackBar1.Maximum = 255;
            if (i!=0)
            {
                trackBar1.Minimum = -255;

            }
            else
            {
                trackBar1.Minimum = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                threshold = Convert.ToInt32(textBox1.Text);
                setimagebythreshold();
                trackBar1.Value = threshold;
            }
        }

        private void setimagebythreshold()
        {
            Rectangle rect = new Rectangle(0, 0, b.Width, b.Height);
            BitmapData bd = b.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData obd = ob.LockBits(rect,ImageLockMode.ReadOnly,PixelFormat.Format24bppRgb);
            int offset = bd.Stride - 3 * b.Width;

            unsafe
            {

                byte* p0 = (byte*)obd.Scan0;
                byte* p = (byte*)bd.Scan0;

                for (int y = 0; y < b.Height; y++, p += offset, p0 += offset)
                    for (int x = 0; x < b.Width; x++, p += 3, p0 += 3)
                    {
                        if (type == 0)
                        {
                            if (p0[0] > threshold)
                                p[2] = p[1] = p[0] = 255;
                            else
                                p[2] = p[1] = p[0] = 0;
                        }
                        else
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                if (p0[i] + threshold > 255)
                                    p[i] = 255;
                                else if (p0[i] + threshold < 0)
                                    p[i] = 0;
                                else
                                    p[i] = Convert.ToByte(p0[i] + threshold);
                            }
                        }
                    }


            }
            b.UnlockBits(bd);
            ob.UnlockBits(obd);
            pictureBox1.Image = b;
            mf.sendimage(b);
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            threshold = trackBar1.Value;
            setimagebythreshold();
            textBox1.Text = "" + threshold;
        }

        private void ManualSettingForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawString("0", Font, Brushes.Black, 20, 70);
            e.Graphics.DrawString("255", Font, Brushes.Black, 236, 70);
        }

        private void ManualSettingForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBox1.Text != "")
            {
                threshold = Convert.ToInt32(textBox1.Text);
                setimagebythreshold();
                trackBar1.Value = threshold;
            }
        }

        private void textBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                threshold = Convert.ToInt32(textBox1.Text);
                setimagebythreshold();
                trackBar1.Value = threshold;
            }
        }



    }
}
