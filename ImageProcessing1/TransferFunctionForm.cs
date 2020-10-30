using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ImageProcessing
{
    public partial class TransferFunctionForm : Form
    {
        //四個頂點(50,50)  (305,50)  (50,305)  (305,305)
        Point firstP = new Point(50, 305);
        Point lastP = new Point(305, 50);
        List<Point> pt = new List<Point>();
        bool ableToMove = false;
        int CIRCLER = 3;
        Bitmap bmp = new Bitmap(350, 350);  //轉換線圖
        int movePNum = 0;
        Bitmap bmpN;
        byte[] tpixel = new byte[256];
        Bitmap b;
        bool isFrame = false;

        public TransferFunctionForm(Bitmap bmpN2)
        {
            InitializeComponent();
            //bmpN = new Bitmap(256,256);
            bmpN = bmpN2.Clone(new Rectangle(0, 0, bmpN2.Width, bmpN2.Height), PixelFormat.Format24bppRgb);
            b = new Bitmap(bmpN.Width, bmpN.Height);
            b = bmpN.Clone(new Rectangle(0, 0, bmpN.Width, bmpN.Height), PixelFormat.Format24bppRgb);
            
            for (int y = 0; y < 350; y++)
                for (int x = 0; x < 350; x++)
                    bmp.SetPixel(x, y, Color.White);
            pictureBox1.Image =bmp;
            pt.Add(firstP);
            pt.Add(lastP);

            for (int i = 0; i < 256; i++)
                tpixel[i] = (byte)i;
        }


        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (!isFrame)
            {
                //外框
                Rectangle rect = new Rectangle(49, 49, 257, 257);
                //外框258 x 258 裡面的格點數為256 x256
                //Rectangle rect = new Rectangle(49, 49, 258, 258);
                Pen pen = new Pen(Color.FromArgb(255, 0, 0, 255));

                //畫在pictureBox1上
                e.Graphics.DrawRectangle(pen, rect);
                e.Graphics.DrawLine(pen, 50, 307, 50, 310);
                e.Graphics.DrawString("0", new Font("New Timer", 8), Brushes.Blue, new PointF(45, 310));

                e.Graphics.DrawLine(pen, 100, 307, 100, 310);
                e.Graphics.DrawString("50", new Font("New Timer", 8), Brushes.Blue, new PointF(91, 310));

                e.Graphics.DrawLine(pen, 150, 307, 150, 310);
                e.Graphics.DrawString("100", new Font("New Timer", 8), Brushes.Blue, new PointF(137, 310));

                e.Graphics.DrawLine(pen, 200, 307, 200, 310);
                e.Graphics.DrawString("150", new Font("New Timer", 8), Brushes.Blue, new PointF(187, 310));

                e.Graphics.DrawLine(pen, 250, 307, 250, 310);
                e.Graphics.DrawString("200", new Font("New Timer", 8), Brushes.Blue, new PointF(237, 310));

                e.Graphics.DrawLine(pen, 300, 307, 300, 310);
                e.Graphics.DrawString("250", new Font("New Timer", 8), Brushes.Blue, new PointF(287, 310));

                e.Graphics.DrawString("Origin Pixel", new Font("New Timer", 8), Brushes.Blue, new PointF(140, 330));


                e.Graphics.DrawLine(pen, 48, 305, 45, 305);
                e.Graphics.DrawString("0", new Font("New Timer", 8), Brushes.Blue, new PointF(33, 297));

                e.Graphics.DrawLine(pen, 48, 255, 45, 255);
                e.Graphics.DrawString("50", new Font("New Timer", 8), Brushes.Blue, new PointF(27, 247));

                e.Graphics.DrawLine(pen, 48, 205, 45, 205);
                e.Graphics.DrawString("100", new Font("New Timer", 8), Brushes.Blue, new PointF(20, 197));

                e.Graphics.DrawLine(pen, 48, 155, 45, 155);
                e.Graphics.DrawString("150", new Font("New Timer", 8), Brushes.Blue, new PointF(20, 147));

                e.Graphics.DrawLine(pen, 48, 105, 45, 105);
                e.Graphics.DrawString("200", new Font("New Timer", 8), Brushes.Blue, new PointF(20, 97));

                e.Graphics.DrawLine(pen, 48, 55, 45, 55);
                e.Graphics.DrawString("250", new Font("New Timer", 8), Brushes.Blue, new PointF(20, 47));

                e.Graphics.DrawString("Transfered Pixel", new Font("New Timer", 8), Brushes.Blue, new PointF(5, 20));
            }

            pt[0] = firstP;
            pt[pt.Count - 1] = lastP;

            e.Graphics.FillEllipse(Brushes.Red, firstP.X - CIRCLER, firstP.Y - CIRCLER, 2 * CIRCLER, 2 * CIRCLER);
            e.Graphics.FillEllipse(Brushes.Red, lastP.X - CIRCLER, lastP.Y - CIRCLER, 2 * CIRCLER, 2 * CIRCLER);


            int c = pt.Count;

            while (c > 2)
            {
                e.Graphics.FillEllipse(Brushes.Orange, pt[c - 2].X - CIRCLER, pt[c - 2].Y - CIRCLER, 2 * CIRCLER, 2 * CIRCLER);
                c--;
            }

            Graphics g = Graphics.FromImage(bmp);
            //g.DrawLine(Pens.Green, 0, 0, 400, 400);
            
            //開反鋸齒曲線顏色會有漸層非純色，且單一行的非白色pixel值會有1~3個
            //g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle bmprect = new Rectangle(0, 0, 350, 350);
            BitmapData bmpd = bmp.LockBits(bmprect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int offset = bmpd.Stride - bmp.Width * 3;
            
            //將轉換線圖清空(全白)
            unsafe
            {
                byte* p = (byte*)bmpd.Scan0;
                
                for (int y = 0; y < 350; y++, p += offset)
                    for (int x = 0; x < 350; x++, p += 3)
                    {
                        p[0] = p[1] = p[2] = 255;
                    }
            }
            bmp.UnlockBits(bmpd);
            
            //畫直線或曲線
            if (radioButton1.Checked == true)
                g.DrawLines(Pens.Green, pt.ToArray());
            else
            {
                g.DrawCurve(Pens.Green, pt.ToArray());

                //處理畫出框框的部分(超出框須設為0)
                
                bmpd = bmp.LockBits(bmprect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                
                unsafe
                {
                    byte* p = (byte*)bmpd.Scan0;

                    for (int y = 0; y < 350; y++, p += offset)
                        for (int x = 0; x < 350; x++, p += 3)
                        {
                            if (y > 49 && y < 306 && (x <= 49 || x >= 306))
                            {
                                if (p[1] == 128)
                                {
                                    p[0] = p[1] = p[2] = 255;
                                    if (x <= 49)
                                    {
                                        int temp = (50 - x) * 3;
                                        p[0 + temp] = Color.Green.B;
                                        p[1 + temp] = Color.Green.G;
                                        p[2 + temp] = Color.Green.R;
                                    }
                                    else
                                    {
                                        int temp = (x - 306 + 1) * 3;
                                        p[0 - temp] = Color.Green.B;
                                        p[1 - temp] = Color.Green.G;
                                        p[2 - temp] = Color.Green.R;
                                    }
                                }
                            }

                            else if (x > 49 && x < 306 && (y <= 49 || y >= 306))
                            {
                                if (p[1] == 128)
                                {
                                    p[0] = p[1] = p[2] = 255;
                                    if (y <= 49)
                                    {
                                        int temp = (50 - y) * bmpd.Stride;
                                        p[0 + temp] = Color.Green.B;
                                        p[1 + temp] = Color.Green.G;
                                        p[2 + temp] = Color.Green.R;
                                    }
                                    else
                                    {
                                        int temp = (y - 306 + 1) * bmpd.Stride;
                                        p[0 - temp] = Color.Green.B;
                                        p[1 - temp] = Color.Green.G;
                                        p[2 - temp] = Color.Green.R;
                                    }
                                }
                            }

                            //處理線跑到4個邊框
                            else if (x <= 49 && (y <= 49 || y >= 306) || x >= 306 && (y <= 49 || y >= 306))
                            {
                                if (p[1] == 128)
                                {
                                    p[0] = p[1] = p[2] = 255;
                                }
                            }
                        }
                }
                bmp.UnlockBits(bmpd);
            }

            //顯示轉換線圖
            pictureBox1.Image = bmp;

            bmpd = bmp.LockBits(bmprect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* p = (byte*)bmpd.Scan0;

                for (int y = 0; y < 350; y++, p += offset)
                    for (int x = 0; x < 350; x++, p += 3)
                    {
                        if (p[1] == 128)
                            tpixel[x - 50] = (byte)(305 - y);
                    }
            }
 
            bmp.UnlockBits(bmpd);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataGridViewRowCollection rows = dataGridView1.Rows;
            rows.Clear();
            for (int i1 = 0; i1 < 256; i1++)
            {
                rows.Add(new Object[] { i1, tpixel[i1] });
            }
            /*
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            string curDir;
            saveFileDialog1.ShowDialog();
            curDir = saveFileDialog1.FileName;
            bmp.Save(curDir);
            */
            //button1.Text += " " + y +" ";
            
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (isInRegion(e.X, e.Y))
                ableToMove = true;
            
            /*
            if (isInRegion(e.X, e.Y, firstP.X, firstP.Y))
            {
                ableToMove = true;
            }*/
        }

        private bool isInRegion(int mx, int my)
        {
            int px, py;
            for (int i = 0; i < pt.Count; i++)
            {
                px = pt[i].X;
                py = pt[i].Y;
                
                if (mx >= px - CIRCLER && mx <= px + CIRCLER && my >= py - CIRCLER && my <= py + CIRCLER)
                {
                    if (i + 1 == pt.Count)
                        movePNum = -1;
                    else
                        movePNum = i;
                    return true;
                }
            }
            
            return false;
            /*
            if (mx >= px - CIRCLER && mx <= px + CIRCLER && my >= py - CIRCLER && my <= py + CIRCLER)
                return true;
            else
                return false;
             * */
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            ableToMove = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;

            if (ableToMove) 
            {
                switch (movePNum)
                {
                    case 0:
                        if (e.Y < 50)
                            firstP.Y = 50;
                        else if (e.Y > 305)
                            firstP.Y = 305;
                        else
                            firstP.Y = e.Y;
                        break;
                    case -1:
                        if (e.Y < 50)
                            lastP.Y = 50;
                        else if (e.Y > 305)
                            lastP.Y = 305;
                        else
                            lastP.Y = e.Y;
                        break;
                    //case 1:
                    default:
                        if (x < 50)
                            x = 50;
                        else if (x > 305)
                            x = 305;

                        if (y < 50)
                            y = 50;
                        else if (y > 305)
                            y = 305;
                        pt[movePNum] = new Point(x, y);

                        break;
                }
                this.Invalidate();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(textBox1.Text) >= 0 && Convert.ToInt32(textBox1.Text) <= 255 && Convert.ToInt32(textBox2.Text) >= 0 && Convert.ToInt32(textBox2.Text) <= 255)
                pt.Insert(pt.Count - 1, new Point(Convert.ToInt32(textBox1.Text) + 50, 305 - Convert.ToInt32(textBox2.Text)));
        }

        private void TransferFunctionForm_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rectn = new Rectangle(0, 0, bmpN.Width, bmpN.Height);
            BitmapData bmpnd = bmpN.LockBits(rectn, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bd = b.LockBits(rectn, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            int offset = bmpnd.Stride - bmpN.Width * 3;
            unsafe
            {
                byte* p = (byte*)bmpnd.Scan0;
                byte* bp = (byte*)bd.Scan0;

                for (int y = 0; y < bmpnd.Height; y++, p += offset,bp+=offset)
                    for (int x = 0; x < bmpN.Width; x++, p += 3, bp += 3)
                    {
                        p[0] = tpixel[bp[0]];
                        p[1] = tpixel[bp[1]];
                        p[2] = tpixel[bp[2]];
                    }
            }
            bmpN.UnlockBits(bmpnd);
            b.UnlockBits(bd);
            e.Graphics.DrawImage(bmpN, new Rectangle(400, 62, bmpN.Width, bmpN.Height));

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (movePNum != 0 && pt.Count > 2) 
            {
                pt.Remove(pt[pt.Count - 2]);
                this.Invalidate();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }

    }
}
