using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ImageProcessing
{
    public partial class bitPlaneSlicingForm : Form
    {
        Bitmap bmp;
        Bitmap originalbmp;
        Rectangle rect;
        int codeType;
        Bitmap b0, b1, b2, b3, b4, b5, b6, b7;
        Bitmap[] bn = new Bitmap[8];
        bool block = false;
        int w;
        int h;
        MainForm m = null;

        public bitPlaneSlicingForm(Bitmap bmpN2, int ct)
        {
            InitializeComponent();

            rect = new Rectangle(0, 0, bmpN2.Width, bmpN2.Height);
            bmp = bmpN2.Clone(rect, PixelFormat.Format24bppRgb);
            originalbmp = bmp.Clone(rect, PixelFormat.Format24bppRgb);
            
            codeType = ct;
            w = bmp.Width;
            h = bmp.Height;
            
            for (int i = 0; i < 8; i++)
                bn[i] = new Bitmap(w, h, PixelFormat.Format1bppIndexed);

            button1.Visible = false;
            comboBox2.Visible = false;
            comboBox3.Visible = false;
            comboBox4.Visible = false;
            comboBox5.Visible = false;
            comboBox6.Visible = false;
            comboBox7.Visible = false;
            comboBox8.Visible = false;
            comboBox9.Visible = false;

            //BCD轉Gray code
            BitmapData bd = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int offset = bd.Stride - bmp.Width * 3;

            unsafe
            {
                byte* p = (byte*)bd.Scan0;

                //gray code需先轉換
                for (int y = 0; y < h; y++, p += offset)
                    for (int x = 0; x < w; x++, p += 3)
                    {
                        byte i = p[2];
                        byte n = Convert.ToByte(i ^ (i >> 1));
                        p[0] = p[1] = p[2] = n;
                    }
            }
            bmp.UnlockBits(bd);

            pictureBox1.Image = originalbmp;
        }

        public bitPlaneSlicingForm(Bitmap bmpN2, int ct, MainForm mainform)
        {
            InitializeComponent();
            rect = new Rectangle(0, 0, bmpN2.Width, bmpN2.Height);
            bmp = bmpN2.Clone(rect, PixelFormat.Format24bppRgb);
            originalbmp = bmp.Clone(rect, PixelFormat.Format24bppRgb);
            codeType = ct;
            w = bmp.Width;
            h = bmp.Height;
            m = mainform;
            for (int i = 0; i < 8; i++)
                bn[i] = new Bitmap(w, h, PixelFormat.Format1bppIndexed);

            pictureBox1.Image = originalbmp;
        }

        private void bitPlaneSlicingForm_Load(object sender, EventArgs e)
        {
            
            for (int i = 7; i >= 0; i--)
            {
                bitplaneProcessing(i);
            }
            block = true;

            setcomboBox(comboBox2);
            setcomboBox(comboBox3);
            setcomboBox(comboBox4);
            setcomboBox(comboBox5);
            setcomboBox(comboBox6);
            setcomboBox(comboBox7);
            setcomboBox(comboBox8);
            setcomboBox(comboBox9);
        }

        private void bitplaneProcessing(int num)
        {
            byte a = 0x80;
            a = (byte)(a >> (7 - num));
            Bitmap b = new Bitmap(w, h, PixelFormat.Format1bppIndexed);

            BitmapData bd = b.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);
            BitmapData bmpd = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int offset = bmpd.Stride - w * 3;
            
            //將深度為1bit的影像一列一列轉為一維陣列分別處理
            byte[] scan = new byte[(w + num) / 8];

            unsafe
            {
                byte* p = (byte*)bmpd.Scan0;

                for (int y = 0; y < h; y++, p += offset)
                {
                    for (int x = 0; x < w; x++, p += 3)
                    {
                        if (x % 8 == 0)
                            scan[x / 8] = 0; //先初始化儲存陣列為00000000 
                   
                        if ((p[2] & a) == a)
                            scan[x / 8] |= (byte)(0x80 >> (x % 8));
                    }
                    System.Runtime.InteropServices.Marshal.Copy(scan, 0, (IntPtr)((long)bd.Scan0 + bd.Stride * y), scan.Length);
                }
                b.UnlockBits(bd);
                bmp.UnlockBits(bmpd);
                showimage(b, num);

                if (!block)
                {
                    switch (num)
                    {
                        case 7:
                            b7 = b.Clone(rect, PixelFormat.Format1bppIndexed);
                            break;
                        case 6:
                            b6 = b.Clone(rect, PixelFormat.Format1bppIndexed);
                            break;
                        case 5:
                            b5 = b.Clone(rect, PixelFormat.Format1bppIndexed);
                            break;
                        case 4:
                            b4 = b.Clone(rect, PixelFormat.Format1bppIndexed);
                            break;
                        case 3:
                            b3 = b.Clone(rect, PixelFormat.Format1bppIndexed);
                            break;
                        case 2:
                            b2 = b.Clone(rect, PixelFormat.Format1bppIndexed);
                            break;
                        case 1:
                            b1 = b.Clone(rect, PixelFormat.Format1bppIndexed);
                            break;
                        case 0:
                            b0 = b.Clone(rect, PixelFormat.Format1bppIndexed);
                            break;
                    }
                }
            }
        }

        private void showimage(Bitmap b,int index)
        {
            switch (index)
            {
                case 7:
                    pictureBox2.Image = b;
                    break;
                case 6:
                    pictureBox3.Image = b;
                    break;
                case 5:
                    pictureBox4.Image = b;
                    break;
                case 4:
                    pictureBox5.Image = b;
                    break;
                case 3:
                    pictureBox6.Image = b;
                    break;
                case 2:
                    pictureBox7.Image = b;
                    break;
                case 1:
                    pictureBox8.Image = b;
                    break;
                case 0:
                    pictureBox9.Image = b;
                    break;
            }
        }
        private void setcomboBox(ComboBox cb)
        {
            cb.Items.Add("Original Bit-plane");
            cb.Items.Add("Random");
            cb.Items.Add("Insert Image");
            cb.Text = "Original bit-plane";
        }

        private void setcombobox(ComboBox c,int n)
        {
            PictureBox pb = pictureBox2;
            Bitmap bb = b7;
            switch (n)
            {
                case 7:
                    pb = pictureBox2;
                    bb = b7;
                    break;
                case 6:
                    pb = pictureBox3;
                    bb = b6;
                    break;
                case 5:
                    pb = pictureBox4;
                    bb = b5;
                    break;
                case 4:
                    pb = pictureBox5;
                    bb = b4;
                    break;
                case 3:
                    pb = pictureBox6;
                    bb = b3;
                    break;
                case 2:
                    pb = pictureBox7;
                    bb = b2;
                    break;
                case 1:
                    pb = pictureBox8;
                    bb = b1;
                    break;
                case 0:
                    pb = pictureBox9;
                    bb = b0;
                    break;
            }
            if (c.Text == "Original Bit-plane")
                bn[n] = bb.Clone(rect, PixelFormat.Format1bppIndexed);
            else if (c.Text == "Random")
                randomimage(n);
            else if (c.Text == "Insert Image")
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "BMP檔|*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    bn[n] = new Bitmap(ofd.FileName);
                }
            }
            pb.Image = bn[n];
        }

        private Bitmap randomimage(int n)
        {
            Bitmap b = new Bitmap(w, h, PixelFormat.Format1bppIndexed);
            BitmapData bd = b.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);


            byte[] scan0 = new byte[(w + 7) / 8];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                    scan0[x / 8] = (byte)(new Random(Guid.NewGuid().GetHashCode()).Next(0, 256));
                Marshal.Copy(scan0, 0, (IntPtr)((long)bd.Scan0 + bd.Stride * y), scan0.Length);
            }

            b.UnlockBits(bd);
            bn[n] = b.Clone(rect, PixelFormat.Format1bppIndexed);
            return b;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            setcombobox(comboBox2, 7);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            setcombobox(comboBox3, 6);
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            setcombobox(comboBox4, 5);
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            setcombobox(comboBox5, 4);
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            setcombobox(comboBox6, 3);
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            setcombobox(comboBox7, 2);
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            setcombobox(comboBox8, 1);
        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            setcombobox(comboBox9, 0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bmp = originalbmp.Clone(rect, PixelFormat.Format24bppRgb);
            for (int i = 7; i >= 0; i--)
            {
                //做替換bit-plane動作
                doinsert(i);
            }
        }

        private void doinsert(int i)
        {
            BitmapData bmpd = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bnd = bn[i].LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format1bppIndexed);

            IntPtr bmpptr = bmpd.Scan0;
            IntPtr bnptr = bnd.Scan0;

            int bmpbyte = bmpd.Stride * h;
            int bnbyte = bnd.Stride * h;

            byte[] bmpvalue = new byte[bmpbyte];
            byte[] bnvalue = new byte[bnbyte];

            Marshal.Copy(bmpptr, bmpvalue, 0, bmpbyte);
            Marshal.Copy(bnptr, bnvalue, 0, bnbyte);
            byte[,] insert = new byte[w, h];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; )
                {
                    if ((bnvalue[x / 8 + y * bnd.Stride] & 0x80) == 0x80) insert[x, y] = 255;
                    else insert[x, y] = 0;
                    x++;
                    if ((bnvalue[x / 8 + y * bnd.Stride] & 0x40) == 0x40) insert[x, y] = 255;
                    else insert[x, y] = 0;
                    x++;
                    if ((bnvalue[x / 8 + y * bnd.Stride] & 0x20) == 0x20) insert[x, y] = 255;
                    else insert[x, y] = 0;
                    x++;
                    if ((bnvalue[x / 8 + y * bnd.Stride] & 0x10) == 0x10) insert[x, y] = 255;
                    else insert[x, y] = 0;
                    x++;
                    if ((bnvalue[x / 8 + y * bnd.Stride] & 0x08) == 0x08) insert[x, y] = 255;
                    else insert[x, y] = 0;
                    x++;
                    if ((bnvalue[x / 8 + y * bnd.Stride] & 0x04) == 0x04) insert[x, y] = 255;
                    else insert[x, y] = 0;
                    x++;
                    if ((bnvalue[x / 8 + y * bnd.Stride] & 0x02) == 0x02) insert[x, y] = 255;
                    else insert[x, y] = 0;
                    x++;
                    if ((bnvalue[x / 8 + y * bnd.Stride] & 0x01) == 0x01) insert[x, y] = 255;
                    else insert[x, y] = 0;
                    x++;
                }
            }

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w * 3; x += 3)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        bmpvalue[x + c + y * bmpd.Stride] = setinsert(insert[x / 3, y], bmpvalue[x + c + y * bmpd.Stride], i);
                    }
                }
            }

            Marshal.Copy(bmpvalue, 0, bmpptr, bmpbyte);
            bn[i].UnlockBits(bnd);
            bmp.UnlockBits(bmpd);
            pictureBox10.Image = bmp;
            m.sendimage(bmp);
            //this.Close();
        }

        private byte setinsert(byte a, byte b, int i)
        {
            switch (i)
            {
                case 0:
                    if (((b & 0x01) == 0x01) && (a == 0)) b &= 0xFE;
                    else if (((b & 0x01) == 0x00) && (a == 255)) b |= 0x01;
                    break;
                case 1:
                    if (((b & 0x02) == 0x02) && (a == 0)) b &= 0xFD;
                    else if (((b & 0x02) == 0x00) && (a == 255)) b |= 0x02;
                    break;
                case 2:
                    if (((b & 0x04) == 0x04) && (a == 0)) b &= 0xFB;
                    else if (((b & 0x04) == 0x00) && (a == 255)) b |= 0x04;
                    break;
                case 3:
                    if (((b & 0x08) == 0x08) && (a == 0)) b &= 0xF7;
                    else if (((b & 0x08) == 0x00) && (a == 255)) b |= 0x08;
                    break;
                case 4:
                    if (((b & 0x10) == 0x10) && (a == 0)) b &= 0xEF;
                    else if (((b & 0x10) == 0x00) && (a == 255)) b |= 0x10;
                    break;
                case 5:
                    if (((b & 0x20) == 0x20) && (a == 0)) b &= 0xDF;
                    else if (((b & 0x20) == 0x00) && (a == 255)) b |= 0x20;
                    break;
                case 6:
                    if (((b & 0x40) == 0x40) && (a == 0)) b &= 0xBF;
                    else if (((b & 0x40) == 0x00) && (a == 255)) b |= 0x40;
                    break;
                case 7:
                    if (((b & 0x80) == 0x80) && (a == 0)) b &= 0x7F;
                    else if (((b & 0x80) == 0x00) && (a == 255)) b |= 0x80;
                    break;
            }
            return b;
        }
    }
}
