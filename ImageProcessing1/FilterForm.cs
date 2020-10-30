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
    public partial class FilterForm : Form
    {
        Bitmap originalBmp;
        Bitmap bmp3,bmp5;
        int w, h;
        Rectangle rect;
        public double threshold = 0;   //Outlier的判斷值
        

        public FilterForm(Bitmap b)
        {
            InitializeComponent();
            w = b.Width;
            h =b.Height;
            rect = new Rectangle(0, 0, w, h);
            originalBmp = b.Clone(rect, PixelFormat.Format24bppRgb);

            pictureBox2.Location = pictureBox1.Location + new Size(pictureBox1.Size.Width + 50, 0);
            pictureBox1.Image = originalBmp;

            bmp3 = new Bitmap(w + 2, h + 2);
            bmp5 = new Bitmap(w + 4, h + 4);

            bmp3.SetPixel(0, 0, originalBmp.GetPixel(w - 1, h - 1));
            bmp3.SetPixel(w + 1, 0, originalBmp.GetPixel(0, h - 1));
            bmp3.SetPixel(0, h + 1, originalBmp.GetPixel(w - 1, 0));
            bmp3.SetPixel(w + 1, h + 1, originalBmp.GetPixel(0, 0));

            bmp5.SetPixel(0, 0, originalBmp.GetPixel(w - 2, h - 2));
            bmp5.SetPixel(0, 1, originalBmp.GetPixel(w - 1, h - 2));
            bmp5.SetPixel(1, 0, originalBmp.GetPixel(w - 2, h - 1));
            bmp5.SetPixel(1, 1, originalBmp.GetPixel(w - 1, h - 1));

            bmp5.SetPixel(w + 3, 0, originalBmp.GetPixel(1, h - 2));
            bmp5.SetPixel(w + 2, 0, originalBmp.GetPixel(0, h - 2));
            bmp5.SetPixel(w + 2, 1, originalBmp.GetPixel(0, h - 1));
            bmp5.SetPixel(w + 3, 1, originalBmp.GetPixel(1, h - 1));

            bmp5.SetPixel(0, h + 3, originalBmp.GetPixel(w - 2, 1));
            bmp5.SetPixel(0, h + 2, originalBmp.GetPixel(w - 2, 0));
            bmp5.SetPixel(1, h + 2, originalBmp.GetPixel(w - 1, 0));
            bmp5.SetPixel(1, h + 3, originalBmp.GetPixel(w - 1, 1));

            bmp5.SetPixel(w + 2, h + 2, originalBmp.GetPixel(0, 0));
            bmp5.SetPixel(w + 2, h + 3, originalBmp.GetPixel(0, 1));
            bmp5.SetPixel(w + 3, h + 2, originalBmp.GetPixel(1, 0));
            bmp5.SetPixel(w + 3, h + 3, originalBmp.GetPixel(1, 1));


            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    bmp3.SetPixel(x + 1, y + 1, originalBmp.GetPixel(x, y));
                    bmp5.SetPixel(x + 2, y + 2, originalBmp.GetPixel(x, y));
                }

            for (int x = 0; x < w; x++)
            {
                bmp3.SetPixel(x + 1, 0, originalBmp.GetPixel(x, h - 1));
                bmp3.SetPixel(x + 1, h + 1, originalBmp.GetPixel(x, 0));
                
                bmp5.SetPixel(x + 2, 1, originalBmp.GetPixel(x, h - 1));
                bmp5.SetPixel(x + 2, h + 2, originalBmp.GetPixel(x, 0));
                bmp5.SetPixel(x + 2, 0, originalBmp.GetPixel(x, h - 2));
                bmp5.SetPixel(x + 2, h + 3, originalBmp.GetPixel(x, 1));
            }

            for (int y = 0; y < h; y++)
            {
                bmp3.SetPixel(0, y + 1, originalBmp.GetPixel(w - 1, y));
                bmp3.SetPixel(w + 1, y + 1, originalBmp.GetPixel(0, y));

                bmp5.SetPixel(1, y + 2, originalBmp.GetPixel(w - 1, y));
                bmp5.SetPixel(w + 2, y + 2, originalBmp.GetPixel(0, y));
                bmp5.SetPixel(0, y + 2, originalBmp.GetPixel(w - 2, y));
                bmp5.SetPixel(w + 3, y + 2, originalBmp.GetPixel(1, y));
            }
            label1.Text = "Origin Image";
            SNRLabel.Text = "";
            //pictureBox2.Image = bmp5;
        }

        private void outlierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label2.Text = "Outlier";


            thresholdForm thf = new thresholdForm(this, 1);
            thf.ShowDialog();

            Bitmap b = originalBmp.Clone(rect, PixelFormat.Format24bppRgb);

            BitmapData bd = b.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int offset = bd.Stride - w * 3;

            unsafe
            {
                byte* p = (byte*)bd.Scan0;
                for (int y = 0; y < h; y++, p += offset)
                    for (int x = 0; x < w; x++, p += 3)
                    {
                        int x2 =x+1;
                        int y2 =y+1;
                        int r;
                        r = (bmp3.GetPixel(x2 - 1, y2 - 1).R + bmp3.GetPixel(x2, y2 - 1).R + bmp3.GetPixel(x2 + 1, y2 - 1).R
                            + bmp3.GetPixel(x2 - 1, y2).R + bmp3.GetPixel(x2 + 1, y2).R
                            + bmp3.GetPixel(x2 - 1, y2 + 1).R + bmp3.GetPixel(x2, y2 + 1).R + bmp3.GetPixel(x2 + 1, y2 + 1).R) / 8;
                        if (Math.Abs(p[0] - r) > threshold)
                        {
                            p[0] = p[1] = p[2] = (byte)r;
                        }
                    }

            }
           
           b.UnlockBits(bd);
           pictureBox2.Image = b;

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            label2.Text = "Outlier";
            thresholdForm thf = new thresholdForm(this, 1);
            thf.ShowDialog();

            Bitmap b = originalBmp.Clone(rect, PixelFormat.Format24bppRgb);

            BitmapData bd = b.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int offset = bd.Stride - w * 3;

            unsafe
            {
                byte* p = (byte*)bd.Scan0;
                for (int y = 0; y < h; y++, p += offset)
                    for (int x = 0; x < w; x++, p += 3)
                    {
                        int x2 = x + 1;
                        int y2 = y + 1;
                        int r;
                        r = (bmp3.GetPixel(x2 - 1, y2 - 1).R + bmp3.GetPixel(x2, y2 - 1).R + bmp3.GetPixel(x2 + 1, y2 - 1).R
                            + bmp3.GetPixel(x2 - 1, y2).R + bmp3.GetPixel(x2 + 1, y2).R
                            + bmp3.GetPixel(x2 - 1, y2 + 1).R + bmp3.GetPixel(x2, y2 + 1).R + bmp3.GetPixel(x2 + 1, y2 + 1).R) / 8;
                        if (Math.Abs(p[0] - r) > threshold)
                        {
                            p[0] = p[1] = p[2] = (byte)r;
                        }
                    }

            }

            b.UnlockBits(bd);
            pictureBox2.Image = b;
        }

        private void boxFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label2.Text = "Box Filter";
            Bitmap b = originalBmp.Clone(rect, PixelFormat.Format24bppRgb);

            BitmapData bd = b.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int offset = bd.Stride - w * 3;

            int maskx =3;
            int masky = 3;
            Color[] maskpiexel = new Color[maskx*masky];

            unsafe
            {
                byte* p = (byte*)bd.Scan0;
                for (int y = 0; y < h; y++, p += offset)
                    for (int x = 0; x < w; x++, p += 3)
                    {

                        for (int j = 0; j < masky; j++)
                            for (int i = 0; i < maskx; i++)
                                maskpiexel[maskx * j + i] = bmp3.GetPixel(x + i, y + j);

                        int red = 0;
                        int green = 0;
                        int blue = 0;

                        foreach (Color c in maskpiexel)
                        {
                            red += c.R;
                            green += c.G;
                            blue += c.B;
                        }
                        
                        p[2] = (byte)(red / 9);
                        p[1] = (byte)(green / 9);
                        p[0] = (byte)(blue / 9);                    
                    }

            }

            b.UnlockBits(bd);
            pictureBox2.Image = b;
        }


        private void weightedAverageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label2.Text = "Weighted Average";
            Bitmap b = originalBmp.Clone(rect, PixelFormat.Format24bppRgb);

            BitmapData bd = b.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int offset = bd.Stride - w * 3;

            int maskx = 3;
            int masky = 3;
            Color[] maskpiexel = new Color[maskx * masky];
            int[] mask = new int[] { 1, 2, 1, 2, 4, 2, 1, 2, 1 };


            unsafe
            {
                byte* p = (byte*)bd.Scan0;
                for (int y = 0; y < h; y++, p += offset)
                    for (int x = 0; x < w; x++, p += 3)
                    {

                        for (int j = 0; j < masky; j++)
                            for (int i = 0; i < maskx; i++)
                                maskpiexel[maskx * j + i] = bmp3.GetPixel(x + i, y + j);

                        int red = 0;
                        int green = 0;
                        int blue = 0;
                        int i2 = 0;
                        foreach (Color c in maskpiexel)
                        {
                            red += c.R * mask[i2];
                            green += c.G * mask[i2];
                            blue += c.B * mask[i2];
                            i2++;
                        }

                        p[2] = (byte)(red / 16);
                        p[1] = (byte)(green / 16);
                        p[0] = (byte)(blue / 16);
                    }

            }

            b.UnlockBits(bd);
            pictureBox2.Image = b;
            /*
            label2.Text = "Weighted Average";
            Bitmap b = originalBmp.Clone(rect, PixelFormat.Format24bppRgb);

            BitmapData bd = b.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int offset = bd.Stride - w * 3;

            unsafe
            {
                
                byte* p = (byte*)bd.Scan0;
                for (int y = 0; y < h; y++, p += offset)
                    for (int x = 0; x < w; x++, p += 3)
                    {
                        int x2 = x + 1;
                        int y2 = y + 1;

                        int r, g, blue;


                        r = (bmp3.GetPixel(x2 - 1, y2 - 1).R + 2*bmp3.GetPixel(x2, y2 - 1).R + bmp3.GetPixel(x2 + 1, y2 - 1).R
                            + 2*bmp3.GetPixel(x2 - 1, y2).R + 4*bmp3.GetPixel(x2, y2).R + 2*bmp3.GetPixel(x2 + 1, y2).R
                            + bmp3.GetPixel(x2 - 1, y2 + 1).R + 2*bmp3.GetPixel(x2, y2 + 1).R + bmp3.GetPixel(x2 + 1, y2 + 1).R) / 16;

                        g = (bmp3.GetPixel(x2 - 1, y2 - 1).G + 2 * bmp3.GetPixel(x2, y2 - 1).G + bmp3.GetPixel(x2 + 1, y2 - 1).G
                            + 2 * bmp3.GetPixel(x2 - 1, y2).G + 4 * bmp3.GetPixel(x2, y2).G + 2 * bmp3.GetPixel(x2 + 1, y2).G
                            + bmp3.GetPixel(x2 - 1, y2 + 1).G + 2 * bmp3.GetPixel(x2, y2 + 1).G + bmp3.GetPixel(x2 + 1, y2 + 1).G) / 16;

                        blue = (bmp3.GetPixel(x2 - 1, y2 - 1).B + 2 * bmp3.GetPixel(x2, y2 - 1).B + bmp3.GetPixel(x2 + 1, y2 - 1).B
                                + 2 * bmp3.GetPixel(x2 - 1, y2).B + 4 * bmp3.GetPixel(x2, y2).B + 2 * bmp3.GetPixel(x2 + 1, y2).B
                                + bmp3.GetPixel(x2 - 1, y2 + 1).B + 2 * bmp3.GetPixel(x2, y2 + 1).B + bmp3.GetPixel(x2 + 1, y2 + 1).B) / 16;

                        p[0] = (byte)blue;
                        p[1] = (byte)g;
                        p[2] = (byte)r;
                    }

            }

            b.UnlockBits(bd);
            pictureBox2.Image = b;
            */
        }


        private void squareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label2.Text = "Median";
            Bitmap b = originalBmp.Clone(rect, PixelFormat.Format24bppRgb);

            BitmapData bd = b.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int offset = bd.Stride - w * 3;

            unsafe
            {
                byte* p = (byte*)bd.Scan0;
                for (int y = 0; y < h; y++, p += offset)
                    for (int x = 0; x < w; x++, p += 3)
                    {
                        byte[] mask = new byte[9];

                        for (int y2 = -1; y2 < 2; y2++)
                            for (int x2 = -1; x2 < 2; x2++)
                                mask[(y2 + 1) * 3 + (x2 + 1)] = bmp3.GetPixel(x + x2 + 1, y + y2 + 1).R;

                        Array.Sort(mask);

                        p[0] = p[1] = p[2] = mask[4];
                    }

            }

            b.UnlockBits(bd);
            pictureBox2.Image = b;
        }

        private void crossToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label2.Text = "Median Cross";
            Bitmap b = originalBmp.Clone(rect, PixelFormat.Format24bppRgb);

            BitmapData bd = b.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int offset = bd.Stride - w * 3;

            unsafe
            {
                byte* p = (byte*)bd.Scan0;
                for (int y = 0; y < h; y++, p += offset)
                    for (int x = 0; x < w; x++, p += 3)
                    {
                        int x2 = x + 2;
                        int y2 = y + 2;
                        byte[] mask = new byte[9];

                        mask[0] = bmp5.GetPixel(x2, y2).R;
                        mask[1] = bmp5.GetPixel(x2 - 1, y2).R;
                        mask[2] = bmp5.GetPixel(x2 - 2, y2).R;
                        mask[3] = bmp5.GetPixel(x2 + 1, y2).R;
                        mask[4] = bmp5.GetPixel(x2 + 2, y2).R;
                        mask[5] = bmp5.GetPixel(x2, y2 - 1).R;
                        mask[6] = bmp5.GetPixel(x2, y2 - 2).R;
                        mask[7] = bmp5.GetPixel(x2, y2 + 1).R;
                        mask[8] = bmp5.GetPixel(x2, y2 + 2).R;

                        Array.Sort(mask);

                        p[0] = p[1] = p[2] = mask[4];
                    }

            }

            b.UnlockBits(bd);
            pictureBox2.Image = b;
        }

        private void basicSpaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            label2.Text = "Basic Highpass Spatial Filtering";

            Bitmap b = originalBmp.Clone(rect, PixelFormat.Format24bppRgb);
            BitmapData bd = b.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int offset = bd.Stride - w * 3;

            unsafe
            {
                byte* p = (byte*)bd.Scan0;
                for (int y = 0; y < h; y++, p += offset)
                    for (int x = 0; x < w; x++, p += 3)
                    {
                        int x2 = x + 1;
                        int y2 = y + 1;
                        int r, g, blue;

                        r = (-bmp3.GetPixel(x2 - 1, y2 - 1).R - bmp3.GetPixel(x2, y2 - 1).R - bmp3.GetPixel(x2 + 1, y2 - 1).R
                            - bmp3.GetPixel(x2 - 1, y2).R + 8 * bmp3.GetPixel(x2, y2).R - bmp3.GetPixel(x2 + 1, y2).R
                            - bmp3.GetPixel(x2 - 1, y2 + 1).R - bmp3.GetPixel(x2, y2 + 1).R - bmp3.GetPixel(x2 + 1, y2 + 1).R)/9;

                        g = (-bmp3.GetPixel(x2 - 1, y2 - 1).G - bmp3.GetPixel(x2, y2 - 1).G - bmp3.GetPixel(x2 + 1, y2 - 1).G
                            - bmp3.GetPixel(x2 - 1, y2).G + 8 * bmp3.GetPixel(x2, y2).G - bmp3.GetPixel(x2 + 1, y2).G
                            - bmp3.GetPixel(x2 - 1, y2 + 1).G - bmp3.GetPixel(x2, y2 + 1).G - bmp3.GetPixel(x2 + 1, y2 + 1).G)/9;

                        blue = (-bmp3.GetPixel(x2 - 1, y2 - 1).B - bmp3.GetPixel(x2, y2 - 1).B - bmp3.GetPixel(x2 + 1, y2 - 1).B
                                - bmp3.GetPixel(x2 - 1, y2).B + 8 * bmp3.GetPixel(x2, y2).B - bmp3.GetPixel(x2 + 1, y2).B
                                - bmp3.GetPixel(x2 - 1, y2 + 1).B - bmp3.GetPixel(x2, y2 + 1).B - bmp3.GetPixel(x2 + 1, y2 + 1).B)/9;

                        r = r > 255 ? 255 : r;
                        r = r < 0 ? 0 : r;
                        g = g > 255 ? 255 : g;
                        g = g < 0 ? 0 : g;
                        blue = blue > 255 ? 255 : blue;
                        blue = blue < 0 ? 0 : blue;

                        p[0] = (byte)blue;
                        p[1] = (byte)g;
                        p[2] = (byte)r;
                    }

            }

            b.UnlockBits(bd);
            pictureBox2.Image = b;
            /*
            int Height = this.pictureBox1.Image.Height;
            int Width = this.pictureBox1.Image.Width;
            Bitmap newBitmap = new Bitmap(Width, Height);
            Bitmap oldBitmap = (Bitmap)this.pictureBox1.Image;
            Color pixel;
            //拉普拉斯模板
            int[] Laplacian = { -1, -1, -1, -1, 9, -1, -1, -1, -1 };
            for (int x = 1; x < Width - 1; x++)
                for (int y = 1; y < Height - 1; y++)
                {
                    int r = 0, g = 0, b = 0;
                    int Index = 0;
                    for (int col = -1; col <= 1; col++)
                        for (int row = -1; row <= 1; row++)
                        {
                            pixel = oldBitmap.GetPixel(x + row, y + col); 
                            r += pixel.R * Laplacian[Index];
                            g += pixel.G * Laplacian[Index];
                            b += pixel.B * Laplacian[Index];
                            Index++;
                        }
                    //处理颜色值溢出
                    r = r > 255 ? 255 : r;
                    r = r < 0 ? 0 : r;
                    g = g > 255 ? 255 : g;
                    g = g < 0 ? 0 : g;
                    b = b > 255 ? 255 : b;
                    b = b < 0 ? 0 : b;
                    newBitmap.SetPixel(x - 1, y - 1, Color.FromArgb(r, g, b));
                }
            this.pictureBox2.Image = newBitmap;*/
        }

        //SNR
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            Bitmap bmpN, bmpN2;
            double snr = 0;
            double a = 0;
            double b = 0;
            SNRLabel.Text = "";

            bmpN = (Bitmap)pictureBox1.Image;
            bmpN2 = (Bitmap)pictureBox2.Image;

            if (bmpN != null && bmpN2 != null)
                if ((bmpN.Width == bmpN2.Width) && (bmpN.Height == bmpN2.Height))
                {
                    for (int y = 0; y < bmpN.Height; y++)
                        for (int x = 0; x < bmpN.Width; x++)
                        {
                            a += Convert.ToInt32(Math.Pow(bmpN.GetPixel(x, y).R, 2));
                            b += Convert.ToInt32(Math.Pow(bmpN.GetPixel(x, y).R - bmpN2.GetPixel(x, y).R, 2));
                            
                        }

                    if (b == 0)
                        SNRLabel.Text = "Same Image";
                    else
                    {
                        snr = 10 * Math.Log10(a * 1.0 / b);
                        SNRLabel.Text = "SNR : " + snr.ToString("F") + " dB ";
                    }
                }
        }

        private void mark1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label2.Text = "Edge Crispening Mark1";
            Bitmap b = originalBmp.Clone(rect, PixelFormat.Format24bppRgb);

            BitmapData bd = b.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int offset = bd.Stride - w * 3;

            int maskx = 3;
            int masky = 3;
            Color[] maskpiexel = new Color[maskx * masky];
            int[] mask = new int[] { 0, -1, 0, -1, 5, -1, 0, -1, 0 };


            unsafe
            {
                byte* p = (byte*)bd.Scan0;
                for (int y = 0; y < h; y++, p += offset)
                    for (int x = 0; x < w; x++, p += 3)
                    {

                        for (int j = 0; j < masky; j++)
                            for (int i = 0; i < maskx; i++)
                                maskpiexel[maskx * j + i] = bmp3.GetPixel(x + i, y + j);

                        int red = 0;
                        int green = 0;
                        int blue = 0;
                        int i2 = 0;
                        foreach (Color c in maskpiexel)
                        {
                            red += c.R * mask[i2];
                            green += c.G * mask[i2];
                            blue += c.B * mask[i2];
                            i2++;
                        }

                        red = red > 255 ? 255 : red;
                        red = red < 0 ? 0 : red;
                        green = green > 255 ? 255 : green;
                        green = green < 0 ? 0 : green;
                        blue = blue > 255 ? 255 : blue;
                        blue = blue < 0 ? 0 : blue;

                        p[2] = (byte)red;
                        p[1] = (byte)green;
                        p[0] = (byte)blue;
                    }

            }

            b.UnlockBits(bd);
            pictureBox2.Image = b;
        }

        private void mark2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label2.Text = "Edge Crispening Mark2";
            Bitmap b = originalBmp.Clone(rect, PixelFormat.Format24bppRgb);

            BitmapData bd = b.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int offset = bd.Stride - w * 3;

            int maskx = 3;
            int masky = 3;
            Color[] maskpiexel = new Color[maskx * masky];
            int[] mask = new int[] { -1, -1, -1, -1, 9, -1, -1, -1, -1 };


            unsafe
            {
                byte* p = (byte*)bd.Scan0;
                for (int y = 0; y < h; y++, p += offset)
                    for (int x = 0; x < w; x++, p += 3)
                    {

                        for (int j = 0; j < masky; j++)
                            for (int i = 0; i < maskx; i++)
                                maskpiexel[maskx * j + i] = bmp3.GetPixel(x + i, y + j);

                        int red = 0;
                        int green = 0;
                        int blue = 0;
                        int i2 = 0;
                        foreach (Color c in maskpiexel)
                        {
                            red += c.R * mask[i2];
                            green += c.G * mask[i2];
                            blue += c.B * mask[i2];
                            i2++;
                        }

                        red = red > 255 ? 255 : red;
                        red = red < 0 ? 0 : red;
                        green = green > 255 ? 255 : green;
                        green = green < 0 ? 0 : green;
                        blue = blue > 255 ? 255 : blue;
                        blue = blue < 0 ? 0 : blue;

                        p[2] = (byte)red;
                        p[1] = (byte)green;
                        p[0] = (byte)blue;
                    }

            }

            b.UnlockBits(bd);
            pictureBox2.Image = b;
        }

        private void mark3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label2.Text = "Edge Crispening Mark3";
            Bitmap b = originalBmp.Clone(rect, PixelFormat.Format24bppRgb);

            BitmapData bd = b.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int offset = bd.Stride - w * 3;

            int maskx = 3;
            int masky = 3;
            Color[] maskpiexel = new Color[maskx * masky];
            int[] mask = new int[] { 1, -2, 1, -2, 5, -2, 1, -2, 1 };


            unsafe
            {
                byte* p = (byte*)bd.Scan0;
                for (int y = 0; y < h; y++, p += offset)
                    for (int x = 0; x < w; x++, p += 3)
                    {

                        for (int j = 0; j < masky; j++)
                            for (int i = 0; i < maskx; i++)
                                maskpiexel[maskx * j + i] = bmp3.GetPixel(x + i, y + j);

                        int red = 0;
                        int green = 0;
                        int blue = 0;
                        int i2 = 0;
                        foreach (Color c in maskpiexel)
                        {
                            red += c.R * mask[i2];
                            green += c.G * mask[i2];
                            blue += c.B * mask[i2];
                            i2++;
                        }

                        red = red > 255 ? 255 : red;
                        red = red < 0 ? 0 : red;
                        green = green > 255 ? 255 : green;
                        green = green < 0 ? 0 : green;
                        blue = blue > 255 ? 255 : blue;
                        blue = blue < 0 ? 0 : blue;

                        p[2] = (byte)red;
                        p[1] = (byte)green;
                        p[0] = (byte)blue;
                    }

            }

            b.UnlockBits(bd);
            pictureBox2.Image = b;
        }

        private void highboostFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            thresholdForm tf = new thresholdForm(this, 2);
            tf.ShowDialog();
            
            
            label2.Text = "High-boost Filter" +" (A = " +threshold +")";
            Bitmap b = originalBmp.Clone(rect, PixelFormat.Format24bppRgb);

            BitmapData bd = b.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int offset = bd.Stride - w * 3;

            int maskx = 3;
            int masky = 3;
            Color[] maskpiexel = new Color[maskx * masky];
            double[] mask = new double[] { -1, -1, -1, -1, 5, -1, -1, -1, -1 };
            mask[4] = 9 * threshold - 1;

            unsafe
            {
                byte* p = (byte*)bd.Scan0;
                for (int y = 0; y < h; y++, p += offset)
                    for (int x = 0; x < w; x++, p += 3)
                    {

                        for (int j = 0; j < masky; j++)
                            for (int i = 0; i < maskx; i++)
                                maskpiexel[maskx * j + i] = bmp3.GetPixel(x + i, y + j);

                        double red = 0;
                        double green = 0;
                        double blue = 0;
                        int i2 = 0;
                        foreach (Color c in maskpiexel)
                        {
                            red += c.R * mask[i2];
                            green += c.G * mask[i2];
                            blue += c.B * mask[i2];
                            i2++;
                        }

                        red = red / 9;
                        green = green / 9;
                        blue = blue / 9;

                        red = red > 255 ? 255 : red;
                        red = red < 0 ? 0 : red;
                        green = green > 255 ? 255 : green;
                        green = green < 0 ? 0 : green;
                        blue = blue > 255 ? 255 : blue;
                        blue = blue < 0 ? 0 : blue;

                        p[2] = (byte)red;
                        p[1] = (byte)green;
                        p[0] = (byte)blue;
                    }

            }

            b.UnlockBits(bd);
            pictureBox2.Image = b;
        }



        private void sobelXgradientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label2.Text = "Sobel x-gradient";
            Bitmap b = originalBmp.Clone(rect, PixelFormat.Format24bppRgb);

            BitmapData bd = b.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int offset = bd.Stride - w * 3;

            int maskx = 3;
            int masky = 3;
            Color[] maskpiexel = new Color[maskx * masky];
            int[] mask = new int[] { -1, -2, -1, 0, 0, 0, 1, 2, 1 };


            unsafe
            {
                byte* p = (byte*)bd.Scan0;
                for (int y = 0; y < h; y++, p += offset)
                    for (int x = 0; x < w; x++, p += 3)
                    {

                        for (int j = 0; j < masky; j++)
                            for (int i = 0; i < maskx; i++)
                                maskpiexel[maskx * j + i] = bmp3.GetPixel(x + i, y + j);

                        int red = 0;
                        int green = 0;
                        int blue = 0;
                        int i2 = 0;
                        foreach (Color c in maskpiexel)
                        {
                            red += c.R * mask[i2];
                            green += c.G * mask[i2];
                            blue += c.B * mask[i2];
                            i2++;
                        }

                        red = red > 255 ? 255 : red;
                        red = red < 0 ? 0 : red;
                        green = green > 255 ? 255 : green;
                        green = green < 0 ? 0 : green;
                        blue = blue > 255 ? 255 : blue;
                        blue = blue < 0 ? 0 : blue;

                        p[2] = (byte)red;
                        p[1] = (byte)green;
                        p[0] = (byte)blue;
                    }

            }

            b.UnlockBits(bd);
            pictureBox2.Image = b;
        }

        private void sobelYgradientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label2.Text = "Sobel y-gradient";
            Bitmap b = originalBmp.Clone(rect, PixelFormat.Format24bppRgb);

            BitmapData bd = b.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int offset = bd.Stride - w * 3;

            int maskx = 3;
            int masky = 3;
            Color[] maskpiexel = new Color[maskx * masky];
            int[] mask = new int[] { -1, 0, 1, -2, 0, 2, -1, 0, 1 };


            unsafe
            {
                byte* p = (byte*)bd.Scan0;
                for (int y = 0; y < h; y++, p += offset)
                    for (int x = 0; x < w; x++, p += 3)
                    {

                        for (int j = 0; j < masky; j++)
                            for (int i = 0; i < maskx; i++)
                                maskpiexel[maskx * j + i] = bmp3.GetPixel(x + i, y + j);

                        int red = 0;
                        int green = 0;
                        int blue = 0;
                        int i2 = 0;
                        foreach (Color c in maskpiexel)
                        {
                            red += c.R * mask[i2];
                            green += c.G * mask[i2];
                            blue += c.B * mask[i2];
                            i2++;
                        }

                        red = red > 255 ? 255 : red;
                        red = red < 0 ? 0 : red;
                        green = green > 255 ? 255 : green;
                        green = green < 0 ? 0 : green;
                        blue = blue > 255 ? 255 : blue;
                        blue = blue < 0 ? 0 : blue;

                        p[2] = (byte)red;
                        p[1] = (byte)green;
                        p[0] = (byte)blue;
                    }

            }

            b.UnlockBits(bd);
            pictureBox2.Image = b;
        }

        private void sobelGradientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label2.Text = "Sobel gradient";
            Bitmap b = originalBmp.Clone(rect, PixelFormat.Format24bppRgb);

            BitmapData bd = b.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int offset = bd.Stride - w * 3;

            int maskx = 3;
            int masky = 3;
            Color[] maskpiexel = new Color[maskx * masky];
            int[] mask = new int[] { -1, 0, 1, -2, 0, 2, -1, 0, 1 };
            int[] mask2 = new int[] { -1, -2, -1, 0, 0, 0, 1, 2, 1 };

            unsafe
            {
                byte* p = (byte*)bd.Scan0;
                for (int y = 0; y < h; y++, p += offset)
                    for (int x = 0; x < w; x++, p += 3)
                    {

                        for (int j = 0; j < masky; j++)
                            for (int i = 0; i < maskx; i++)
                                maskpiexel[maskx * j + i] = bmp3.GetPixel(x + i, y + j);

                        int red = 0;
                        int green = 0;
                        int blue = 0;
                        int red2 = 0, green2 = 0, blue2 = 0;
                        int i2 = 0;
                        foreach (Color c in maskpiexel)
                        {
                            red += c.R * mask[i2];
                            green += c.G * mask[i2];
                            blue += c.B * mask[i2];
                            red2 += c.R * mask2[i2];
                            green2 += c.G * mask2[i2];
                            blue2 += c.B * mask2[i2];

                            i2++;
                        }
                        red = Math.Abs(red) + Math.Abs(red2);
                        green = Math.Abs(green) + Math.Abs(green2);
                        blue = Math.Abs(blue) + Math.Abs(blue2);
                        red /= 2;
                        green /= 2;
                        blue /= 2;
                        red = red > 255 ? 255 : red;
                        red = red < 0 ? 0 : red;
                        green = green > 255 ? 255 : green;
                        green = green < 0 ? 0 : green;
                        blue = blue > 255 ? 255 : blue;
                        blue = blue < 0 ? 0 : blue;

                        p[2] = (byte)red;
                        p[1] = (byte)green;
                        p[0] = (byte)blue;
                    }

            }

            b.UnlockBits(bd);
            pictureBox2.Image = b;
        }

        private void prewittGradientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label2.Text = "Prewitt gradient";
            Bitmap b = originalBmp.Clone(rect, PixelFormat.Format24bppRgb);

            BitmapData bd = b.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int offset = bd.Stride - w * 3;

            int maskx = 3;
            int masky = 3;
            Color[] maskpiexel = new Color[maskx * masky];
            int[] mask = new int[] { -1, 0, 1, -1, 0, 1, -1, 0, 1 };
            int[] mask2 = new int[] { -1, -1, -1, 0, 0, 0, 1, 1, 1 };

            unsafe
            {
                byte* p = (byte*)bd.Scan0;
                for (int y = 0; y < h; y++, p += offset)
                    for (int x = 0; x < w; x++, p += 3)
                    {

                        for (int j = 0; j < masky; j++)
                            for (int i = 0; i < maskx; i++)
                                maskpiexel[maskx * j + i] = bmp3.GetPixel(x + i, y + j);

                        int red = 0;
                        int green = 0;
                        int blue = 0;
                        int i2 = 0;
                        foreach (Color c in maskpiexel)
                        {
                            red += c.R * mask[i2] + c.R * mask2[i2];
                            green += c.G * mask[i2] + c.G * mask2[i2];
                            blue += c.B * mask[i2] + c.B * mask2[i2];
                            i2++;
                        }

                        red = red > 255 ? 255 : red;
                        red = red < 0 ? 0 : red;
                        green = green > 255 ? 255 : green;
                        green = green < 0 ? 0 : green;
                        blue = blue > 255 ? 255 : blue;
                        blue = blue < 0 ? 0 : blue;

                        p[2] = (byte)red;
                        p[1] = (byte)green;
                        p[0] = (byte)blue;
                    }

            }

            b.UnlockBits(bd);
            pictureBox2.Image = b;
        }

        private void prewittToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label2.Text = "Prewitt x-gradient";
            Bitmap b = originalBmp.Clone(rect, PixelFormat.Format24bppRgb);

            BitmapData bd = b.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int offset = bd.Stride - w * 3;

            int maskx = 3;
            int masky = 3;
            Color[] maskpiexel = new Color[maskx * masky];
            int[] mask = new int[] { -1, -1, -1, 0, 0, 0, 1, 1, 1 };


            unsafe
            {
                byte* p = (byte*)bd.Scan0;
                for (int y = 0; y < h; y++, p += offset)
                    for (int x = 0; x < w; x++, p += 3)
                    {

                        for (int j = 0; j < masky; j++)
                            for (int i = 0; i < maskx; i++)
                                maskpiexel[maskx * j + i] = bmp3.GetPixel(x + i, y + j);

                        int red = 0;
                        int green = 0;
                        int blue = 0;
                        int i2 = 0;
                        foreach (Color c in maskpiexel)
                        {
                            red += c.R * mask[i2];
                            green += c.G * mask[i2];
                            blue += c.B * mask[i2];
                            i2++;
                        }

                        red = red > 255 ? 255 : red;
                        red = red < 0 ? 0 : red;
                        green = green > 255 ? 255 : green;
                        green = green < 0 ? 0 : green;
                        blue = blue > 255 ? 255 : blue;
                        blue = blue < 0 ? 0 : blue;

                        p[2] = (byte)red;
                        p[1] = (byte)green;
                        p[0] = (byte)blue;
                    }

            }

            b.UnlockBits(bd);
            pictureBox2.Image = b;
        }

        private void prewittYgradientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label2.Text = "Prewitt y-gradient";
            Bitmap b = originalBmp.Clone(rect, PixelFormat.Format24bppRgb);

            BitmapData bd = b.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int offset = bd.Stride - w * 3;

            int maskx = 3;
            int masky = 3;
            Color[] maskpiexel = new Color[maskx * masky];
            int[] mask = new int[] { -1, 0, 1, -1, 0, 1, -1, 0, 1 };


            unsafe
            {
                byte* p = (byte*)bd.Scan0;
                for (int y = 0; y < h; y++, p += offset)
                    for (int x = 0; x < w; x++, p += 3)
                    {

                        for (int j = 0; j < masky; j++)
                            for (int i = 0; i < maskx; i++)
                                maskpiexel[maskx * j + i] = bmp3.GetPixel(x + i, y + j);

                        int red = 0;
                        int green = 0;
                        int blue = 0;
                        int i2 = 0;
                        foreach (Color c in maskpiexel)
                        {
                            red += c.R * mask[i2];
                            green += c.G * mask[i2];
                            blue += c.B * mask[i2];
                            i2++;
                        }

                        red = red > 255 ? 255 : red;
                        red = red < 0 ? 0 : red;
                        green = green > 255 ? 255 : green;
                        green = green < 0 ? 0 : green;
                        blue = blue > 255 ? 255 : blue;
                        blue = blue < 0 ? 0 : blue;

                        p[2] = (byte)red;
                        p[1] = (byte)green;
                        p[0] = (byte)blue;
                    }

            }

            b.UnlockBits(bd);
            pictureBox2.Image = b;
        }


    }
}
