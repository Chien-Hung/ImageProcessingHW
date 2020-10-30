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
    public partial class histMatchForm : Form
    {
        //原圖、等畫圖預備資訊設置
        Bitmap bmpe;
        Bitmap bmpo;
        Bitmap bmps;
        Bitmap histogram = new Bitmap(256, 256);
        Rectangle rect;
        ulong[,] pixel = new ulong[256, 2];
        int maxindex = 0;

        ulong[,] pixele = new ulong[256, 2];
        int maxindexe = 0;

        ulong[,] pixels = new ulong[256, 2];
        int maxindexs = 0;

        ulong[,] pixels2 = new ulong[256, 2];
        int maxindexs2 = 0;

        //控制項
        Bitmap controltable = new Bitmap(350, 350);
        //四個頂點(50,50)  (305,50)  (50,305)  (305,305)
        Point firstP = new Point(50, 50 + 127);
        Point lastP = new Point(305, 50 + 127);
        int CIRCLER = 3;
        List<Point> pt = new List<Point>();
        bool ableToMove = false;
        int movePNum = 0;

        //轉換表 index：原圖顏色 byte[i]：原圖等化對應顏色
        byte[] table = new byte[256];
        byte[] transfer = new byte[256];
        byte[] connectedtable = new byte[256];

        public histMatchForm(Bitmap b, ulong[] pixelnumber)
        {
            InitializeComponent();

            //將起點終點加入點串
            pt.Add(firstP);
            pt.Add(lastP);

            //處理原圖、等化圖資訊
            for (int i = 0; i < 256; i++)
            {
                pixel[i, 0] = pixel[i, 1] = pixelnumber[i];
                if (pixel[i, 0] > pixel[maxindex, 0])
                    maxindex = i;
            }
            for (int i = 1; i < 256; i++)
                pixel[i, 1] = pixel[i - 1, 1] + pixel[i, 1];


            bmpo = b as Bitmap;
            bmpe = bmpo.Clone() as Bitmap;
            rect = new Rectangle(0, 0, b.Width, b.Height);

            //原圖和等化圖轉換建立
            for (int i = 0; i < 256; i++)
            {
                int newpixel = Convert.ToInt32(Math.Round(pixel[i, 1] * 256.0 / (bmpo.Width * bmpo.Height)) - 1);
                if (newpixel < 0)
                    newpixel = 0;
                table[i] = Convert.ToByte(newpixel);
            }

            BitmapData ebmpdata = bmpe.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int offset = ebmpdata.Stride - b.Width * 3;

            unsafe
            {
                byte* p = (byte*)ebmpdata.Scan0;

                for (int j = 0; j < bmpo.Height; j++, p += offset)
                    for (int i = 0; i < bmpo.Width; i++, p += 3)
                    {
                        p[0] = p[1] = p[2] = table[p[0]];
                        pixele[p[0],0]++;
                    }
            }
            bmpe.UnlockBits(ebmpdata);

            for (int i = 0; i < 256; i++)
            {
                pixele[i, 1] = pixele[i, 0];
                if (pixele[i, 0] > pixele[maxindexe, 0])
                    maxindexe = i;
            }
            for (int i = 1; i < 256; i++)
                pixele[i, 1] = pixele[i - 1, 1] + pixele[i, 1];

            bmps = bmpo.Clone() as Bitmap;
            
            pictureBox1.Image = bmpo;
            pictureBox2.Image = bmpe;
            pictureBox6.Image = bmps;


            //顯示原圖、等化圖的直方圖和CDF

            Bitmap bmpocdf = new Bitmap(256, 256);
            Graphics g = Graphics.FromImage(bmpocdf);
            Pen curPen = new Pen(Brushes.Gray, 1);
            ulong maxPixel = pixel[255, 1];


            double temp = 0;
            int last = 0;
            curPen.Color = Color.Red;
            for (int i = 0; i < 256; i++)
            {
                temp = 256.0 * pixel[i, 1] / maxPixel;
                g.DrawLine(Pens.Red, i, 255, i, 255 - (int)(256 * pixel[i, 0] / pixel[maxindex, 0]));

                if (last != 0)
                {
                    g.DrawLine(Pens.Black, i, last, i, 256 - (int)temp);
                }
                g.FillRectangle(Brushes.Black, i, 256 - (int)temp, 1, 1);

                last = 256 - (int)temp;
            }

            pictureBox3.Image = bmpocdf;

            //畫等化後圖的CDF
            Bitmap bmpecdf = new Bitmap(256, 256);
            g = Graphics.FromImage(bmpecdf);

            temp = 0;
            last = 0;
            maxPixel = pixele[255, 1];

            curPen.Color = Color.Red;
            for (int i = 0; i < 256; i++)
            {
                temp = 256.0 * pixele[i, 1] / maxPixel;
                g.DrawLine(Pens.Red, i, 255, i, 255 - (int)(256 * pixele[i, 0] / pixele[maxindexe, 0]));

                if (last != 0)
                {
                    g.DrawLine(Pens.Black, i, last, i, 256 - (int)temp);
                }
                g.FillRectangle(Brushes.Black, i, 256 - (int)temp, 1, 1);

                last = 256 - (int)temp;
            }

            g.Dispose();
            curPen.Dispose();
            pictureBox4.Image = bmpecdf;
            
        }

        private void pictureBox5_Paint(object sender, PaintEventArgs e)
        {
            //將controltable清空為空白
            BitmapData bd = controltable.LockBits(new Rectangle(0, 0, controltable.Width, controltable.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int offset = bd.Stride - controltable.Width * 3;

            unsafe
            {
                byte* p = (byte*)bd.Scan0;


                for (int y = 0; y < controltable.Height; y++, p += offset)
                    for (int x = 0; x < controltable.Width; x++, p += 3) 
                    {
                        p[0] = p[1] = p[2] = 255;
                    }
            }
            controltable.UnlockBits(bd);

            //控制線
            Graphics g = Graphics.FromImage(controltable);
            //外框
            Rectangle rect = new Rectangle(49, 49, 257, 257);
            
            pt[0] = firstP;
            pt[pt.Count - 1] = lastP;


            e.Graphics.DrawRectangle(Pens.Gray, rect);



            // 畫點
            e.Graphics.FillEllipse(Brushes.Red, firstP.X - CIRCLER, firstP.Y - CIRCLER, 2 * CIRCLER, 2 * CIRCLER);
            e.Graphics.FillEllipse(Brushes.Red, lastP.X - CIRCLER, lastP.Y - CIRCLER, 2 * CIRCLER, 2 * CIRCLER);

            e.Graphics.DrawString("Histogram Distribution", new Font("Arial", 16), Brushes.Blue, 70, 317);
            
            int c = pt.Count;
            
            while (c > 2)
            {
                e.Graphics.FillEllipse(Brushes.Orange, pt[c - 2].X - CIRCLER, pt[c - 2].Y - CIRCLER, 2 * CIRCLER, 2 * CIRCLER);
                c--;
            }

            //畫線
            e.Graphics.DrawLines(Pens.Green, pt.ToArray());
            g.DrawLines(Pens.Green, pt.ToArray());
            
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(textBox1.Text) >= 0 && Convert.ToInt32(textBox1.Text) <= 255 && Convert.ToInt32(textBox2.Text) >= 0 && Convert.ToInt32(textBox2.Text) <= 255)
            {
                pt.Insert(pt.Count - 1, new Point(Convert.ToInt32(textBox1.Text) + 50, 305 - Convert.ToInt32(textBox2.Text)));
                pictureBox5.Invalidate();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (movePNum != 0 && pt.Count > 2)
            {
                pt.Remove(pt[pt.Count - 2]);
                pictureBox5.Invalidate();
            }
        }

        private void pictureBox5_MouseMove(object sender, MouseEventArgs e)
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
                pictureBox5.Invalidate();
            }
        }

        private void pictureBox5_MouseDown(object sender, MouseEventArgs e)
        {
            if (isInRegion(e.X, e.Y))
                ableToMove = true;
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
        }

        private void pictureBox5_MouseUp(object sender, MouseEventArgs e)
        {
            ableToMove = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            bmps = (Bitmap)bmpo.Clone();

            //控制線資訊紀錄
            BitmapData bd = controltable.LockBits(new Rectangle(0, 0, controltable.Width, controltable.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int offset = bd.Stride - controltable.Width * 3;

            unsafe
            {
                byte* p = (byte*)bd.Scan0;


                for (int y = 0; y < controltable.Height; y++, p += offset)
                    for (int x = 0; x < controltable.Width; x++, p += 3)
                    {
                        if (p[1] == 128)
                            pixels[x - 50, 0] = (ulong)(305 - y);
                    }
            }
            controltable.UnlockBits(bd);



            for (int i = 0; i < 256; i++)
            {
                pixels[i, 1] = pixels[i, 0];
                if (pixels[i, 0] > pixels[maxindexs, 0])
                    maxindexs = i;
            }

            for (int i = 1; i < 256; i++)
                pixels[i, 1] = pixels[i - 1, 1] + pixels[i, 1];
            
            //將CDF比例轉成最大值等於Width*Height
            for (int i = 0; i < 256; i++)
                pixels[i, 1] = (ulong)(Convert.ToDouble(pixels[i, 1]) * bmps.Width * bmps.Height / pixels[255, 1]);

            //畫自訂histogram的CDF
            Bitmap bmpscdf = new Bitmap(256, 256);
            Graphics g = Graphics.FromImage(bmpscdf);

            double temp = 0;
            int last = 0;
            double maxPixel = pixels[255, 1];

            for (int i = 0; i < 256; i++)
            {
                temp = 256.0 * pixels[i, 1] / maxPixel;
                g.DrawLine(Pens.Red, i, 255, i, 255 - (int)(255 * pixels[i, 0] / pixels[maxindexs, 0]));
                
                if (last != 0)
                {
                    g.DrawLine(Pens.Black, i, last, i, 256 - (int)temp);
                }
                g.FillRectangle(Brushes.Black, i, 256 - (int)temp, 1, 1);

                last = 256 - (int)temp;
            }

            g.Dispose();

            

            //自訂histogram圖和等化圖的轉換表建立
            for (int i = 0; i < 256; i++)
            {
                int newpixel = Convert.ToInt32(Math.Round(pixels[i, 1] * 256.0 / (bmps.Width * bmps.Height)) - 1);
                if (newpixel < 0)
                    newpixel = 0;
                transfer[i] = Convert.ToByte(newpixel);
            }

            //2個等化圖的連結
            
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    if (transfer[j] == table[i])
                    {
                        connectedtable[i] = Convert.ToByte(j);
                        break;
                    }
                    else if (table[i] == 0)
                    {
                        connectedtable[i] = 0;
                        break;
                    }
                    else if(table[i]<transfer[0])
                    {
                        connectedtable[i] = 0;
                        break;
                    }
                    else if (table[i] > transfer[j] & table[i] < transfer[j + 1])
                    {
                        if (Math.Abs(table[i] - transfer[j]) > Math.Abs(table[i] - transfer[j + 1]))
                            connectedtable[i] = Convert.ToByte(j + 1);
                        else
                            connectedtable[i] = Convert.ToByte(j);
                        break;
                    }
                    
                }
            }

            //將算個數的陣列歸零
            for (int i = 0; i < 256; i++)
            {
                pixels2[i, 0] = 0;
            }

            //將原圖依照轉換表轉換成特化圖
            BitmapData sbmpdata = bmps.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            offset = sbmpdata.Stride - bmps.Width * 3;
                unsafe
                {
                    byte* p = (byte*)sbmpdata.Scan0;

                    for (int j = 0; j < bmps.Height; j++, p += offset)
                        for (int i = 0; i < bmps.Width; i++, p += 3)
                        {
                            p[0] = p[1] = p[2] = connectedtable[p[0]];
                            pixels2[p[0],0]++;
                        }
                }
            bmps.UnlockBits(sbmpdata);


            for (int i = 0; i < 256; i++)
            {
                pixels2[i, 1] = pixels2[i, 0];
                if (pixels2[i, 0] > pixels2[maxindexs2, 0])
                    maxindexs2 = i;
            }
            for (int i = 1; i < 256; i++)
                pixels2[i, 1] = pixels2[i - 1, 1] + pixels2[i, 1];

            //畫等化後圖的CDF
            Bitmap bmpscdf2 = new Bitmap(256, 256);
            g = Graphics.FromImage(bmpscdf2);

            temp = 0;
            last = 0;
            maxPixel = pixels2[255, 1];

            for (int i = 0; i < 256; i++)
            {
                temp = 256.0 * pixels2[i, 1] / maxPixel;
                g.DrawLine(Pens.Red, i, 255, i, 255 - (int)(256 * pixels2[i, 0] / pixels2[maxindexs2, 0]));

                if (last != 0)
                {
                    g.DrawLine(Pens.Black, i, last, i, 256 - (int)temp);
                }
                g.FillRectangle(Brushes.Black, i, 256 - (int)temp, 1, 1);

                last = 256 - (int)temp;
            }
            
            pictureBox6.Image = bmps;
            pictureBox7.Image = bmpscdf;
            pictureBox8.Image = bmpscdf2;
            
        }

    }
}
