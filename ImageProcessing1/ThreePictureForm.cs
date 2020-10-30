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
    public partial class ThreePictureForm : Form
    {
        string filename;
        Bitmap bmp1,bmp2,bmp3;
        string ss;    //判斷功能用變數

        public ThreePictureForm(string s)
        {
            ss = s;
            InitializeComponent();
            
            if (s == "Transparency")
            {
                
                label1.Visible = true;
                label2.Visible = true;
                label3.Visible = true;
                textBox1.Visible = true;
                textBox2.Visible = true;
                textBox3.Visible = true;
            }
            else if (s == "Pixel Division")
            {
                comboBox1.Visible = true;
            }

        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filename = ofd.FileName;
                bmp1 = (Bitmap)Bitmap.FromFile(filename);
                pictureBox1.Image = bmp1;
            }
        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filename = ofd.FileName;
                bmp2 = (Bitmap)Bitmap.FromFile(filename);
                pictureBox2.Image = bmp2;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (bmp1 != null && bmp2 != null)
                {


                    switch (ss)
                    {
                        case "Transparency":
                            int ox = Convert.ToInt32(textBox1.Text);
                            int oy = Convert.ToInt32(textBox2.Text);
                            float speed = float.Parse(textBox3.Text) / 100;

                            unsafe
                            {
                                for (float a = 0; a <= 1; a += speed)
                                {
                                    Rectangle rect1 = new Rectangle(0, 0, bmp1.Width, bmp1.Height);
                                    Rectangle rect2 = new Rectangle(0, 0, bmp2.Width, bmp2.Height);
                                    Rectangle rect3;

                                    BitmapData bmpd1 = bmp1.LockBits(rect1, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                                    BitmapData bmpd2 = bmp2.LockBits(rect2, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                                    BitmapData bmpd3;

                                    System.IntPtr scan1 = bmpd1.Scan0;
                                    System.IntPtr scan2 = bmpd2.Scan0;
                                    System.IntPtr scan3;

                                    bmp3 = new Bitmap(bmp2.Width + ox, bmp2.Height + oy, PixelFormat.Format24bppRgb);
                                    rect3 = new Rectangle(0, 0, bmp3.Width, bmp3.Height);
                                    bmpd3 = bmp3.LockBits(rect3, ImageLockMode.ReadWrite, bmp3.PixelFormat);
                                    scan3 = bmpd3.Scan0;

                                    byte* p1 = (byte*)scan1;
                                    byte* p2 = (byte*)scan2;
                                    byte* p3 = (byte*)scan3;

                                    int stride1 = bmpd1.Stride;
                                    int offset1 = stride1 - bmp1.Width * 3;
                                    int stride2 = bmpd2.Stride;
                                    int offset2 = stride2 - bmp2.Width * 3;
                                    int stride3 = bmpd3.Stride;
                                    int offset3 = stride3 - bmp3.Width * 3;

                                    for (int y = 0; y < bmp3.Height; y++)
                                        for (int x = 0; x < bmp3.Width; x++)
                                        {
                                            if ((y < oy && x < bmp1.Width) || (x < ox && y >= oy && y < bmp1.Height))
                                            {
                                                p3[0] = p1[0];
                                                p3[1] = p1[1];
                                                p3[2] = p1[2];
                                                p1 += 3;
                                                p3 += 3;
                                            }
                                            else if (x >= ox && x < bmp1.Width && y >= oy && y < bmp1.Height)
                                            {
                                                p3[0] = (byte)(a * p2[0] + (1 - a) * p1[0]);
                                                p3[1] = (byte)(a * p2[1] + (1 - a) * p1[1]);
                                                p3[2] = (byte)(a * p2[2] + (1 - a) * p1[2]);
                                                p1 += 3;
                                                p2 += 3;
                                                p3 += 3;

                                            }
                                            else if ((x >= bmp1.Width && y >= oy) || (y >= bmp1.Height && x >= ox))
                                            {
                                                p3[0] = p2[0];
                                                p3[1] = p2[1];
                                                p3[2] = p2[2];
                                                p2 += 3;
                                                p3 += 3;
                                            }
                                            else
                                                p3 += 3;

                                            if (x == bmp1.Width - 1)
                                                p1 += offset1;
                                            if (x == bmp3.Width - 1)
                                                p3 += offset3;
                                            if (x - ox == bmp2.Width - 1)
                                                p2 += offset2;
                                        }

                                    bmp1.UnlockBits(bmpd1);
                                    bmp2.UnlockBits(bmpd2);
                                    bmp3.UnlockBits(bmpd3);

                                    pictureBox3.Image = bmp3;
                                    pictureBox3.Refresh();
                                }
                            }

                            break;
                        case "Pixel Division":
                            pixelOperation();
                            break;
                        default:
                            break;
                    }
                }
        }

        private void pixelOperation()
        {
            Rectangle rect1 = new Rectangle(0, 0, bmp1.Width, bmp1.Height);
            Rectangle rect2 = new Rectangle(0, 0, bmp2.Width, bmp2.Height);
            Rectangle rect3;

            BitmapData bmpd1 = bmp1.LockBits(rect1, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData bmpd2 = bmp2.LockBits(rect2, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData bmpd3;

            System.IntPtr scan1 = bmpd1.Scan0;
            System.IntPtr scan2 = bmpd2.Scan0;
            System.IntPtr scan3;

            bmp3 = new Bitmap(bmp2.Width, bmp2.Height, PixelFormat.Format24bppRgb);
            rect3 = new Rectangle(0, 0, bmp3.Width, bmp3.Height);
            bmpd3 = bmp3.LockBits(rect3, ImageLockMode.ReadWrite, bmp3.PixelFormat);
            scan3 = bmpd3.Scan0;

            int stride1 = bmpd1.Stride;
            int offset1 = stride1 - bmp1.Width * 3;
            int stride2 = bmpd2.Stride;
            int offset2 = stride2 - bmp2.Width * 3;
            int stride3 = bmpd3.Stride;
            int offset3 = stride3 - bmp3.Width * 3;

            unsafe
            {
                byte* p1 = (byte*)scan1;
                byte* p2 = (byte*)scan2;
                byte* p3 = (byte*)scan3;

                for (int y = 0; y < bmp3.Height; y++)
                {
                    for (int x = 0; x < bmp3.Width; x++)
                    {
                        switch (comboBox1.SelectedIndex)
                        {
                            case 0:
                                p3[0] = Convert.ToByte((p1[0] - p2[0])/2+127);
                                p3[1] = Convert.ToByte((p1[1] - p2[1])/2+127);
                                p3[2] = Convert.ToByte((p1[2] - p2[2])/2+127);
                                p1 += 3;
                                p2 += 3;
                                p3 += 3;
                                break;
                            case 1:
                                p3[0] = Convert.ToByte((p1[0] + p2[0])/2);
                                p3[1] = Convert.ToByte((p1[1] + p2[1])/2);
                                p3[2] = Convert.ToByte((p1[2] + p2[2])/2);
                                p1 += 3;
                                p2 += 3;
                                p3 += 3;
                                break;
                            default:
                                break;

                        }
                    }
                    p1 += offset1;
                    p2 += offset2;
                    p3 += offset3;
                }
                bmp1.UnlockBits(bmpd1);
                bmp2.UnlockBits(bmpd2);
                bmp3.UnlockBits(bmpd3);

                pictureBox3.Image = bmp3;
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                filename = sfd.FileName;
                bmp3.Save(filename);
            }
        }
    }
}
