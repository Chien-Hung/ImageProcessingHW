using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Threading;

namespace ImageProcessing
{
    public partial class MainForm : Form
    {

        HeaderImformationForm hiform = new HeaderImformationForm();
        PixelDataForm pdf = new PixelDataForm();
        Bitmap originImage;
        Bitmap bmpN;
        Bitmap bmpN2 = null;
        int c = 0;              //用來互鎖PictureBox1的MouseMove和MouseDown事件
        int m = 0;              //Cut滑鼠圖示變化控制變數
        int x1, y1, x2, y2;     //Paste座標紀錄變數
        bool bb = false;        //Paste功能是否啟用變數
        bool middlem = false;   //滑鼠中鍵開啟輔助小視窗功能與否
        bool encoding = true;   //huffman code decode or encode

        ulong[] pixelnumber = new ulong[256];
        byte max = 0;
        List<Point> pt = new List<Point>();   //要做Cut Arbitrary功能用 但目前做不出來
        int line;

        //huffman table存取空間
        List<huffmanNode> data = new List<huffmanNode>();

        List<FractalDataNode> fractal = new List<FractalDataNode>();


        public MainForm()
        {
            InitializeComponent();
            statusOI.Text = "";
            statusLocation.Text = "";
            statusC.Text = "   ";
            statusR.Text = "";
            statusG.Text = "";
            statusB.Text = "";
            split.Text = "";
            statusPI.Text = "";
            statusC2.Text = "   ";
            statusR2.Text = "";
            statusG2.Text = "";
            statusB2.Text = "";
            label1.Text = "";
            label5.Text = "";
            label3.Text = velocity + "  frame / s";

            //反轉按鈕
            Bitmap b = Properties.Resources.play;
            b.RotateFlip(RotateFlipType.RotateNoneFlipX);
            button9.Image = b;

            //方便待刪
            tENNISToolStripMenuItem.PerformClick();

        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            bool check = true;

            switch (e.KeyCode)
            {
                case Keys.D1:
                    pictureBox1.Image = Properties.Resources.Lena256;
                    break;

                case Keys.D2:
                    pictureBox1.Image = Properties.Resources.fisher256;
                    break;

                case Keys.D3:
                    pictureBox1.Image = Properties.Resources.tree;
                    break;

                case Keys.D4:
                    pictureBox1.Image = Properties.Resources.demoSpe;
                    break;
                case Keys.D5:

                    pictureBox1.Image = Properties.Resources.gradient;
                    break;
                case Keys.D6:
                    pictureBox1.Image = Properties.Resources.CCL;
                    break;

                case Keys.D7:
                    pictureBox1.Image = Properties.Resources.GrayCode;
                    break;

                case Keys.Z:
                    pictureMoveToolStripMenuItem.PerformClick();
                    break;

                case Keys.X:
                    imageMoveToRightToolStripMenuItem.PerformClick();
                    break;

                case Keys.Space:
                    frequencyFilterDCTToolStripMenuItem.PerformClick();
                    break;

                default:
                    check = false;
                    break;
            }
            if (check)
            {
                originImage = (Bitmap)pictureBox1.Image.Clone();
                bmpN = (Bitmap)pictureBox1.Image.Clone();
                ImageProcessingToolStripMenuItem.Enabled = true;
            }
        }

        #region 主要程式碼

        //載入表單執行的事件
        private void MainForm_Load(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "PCX檔|*.pcx|BMP檔|*.bmp|所有檔案|*.*";
            openFileDialog1.RestoreDirectory = true;
            SNRLabel.Text = "";
            /*
//方便DEBUG  之後要刪除
            if (openFileDialog1.FilterIndex == 2)
                openFileDialog1.FilterIndex = 3;
            originImage = new Bitmap(@"D:\ImageProcessing\picture\Sample Pictures\fisher256.bmp");
            pictureBox1.Image = originImage;
            ImageProcessingToolStripMenuItem.Enabled = true;
//*************************************************/
        }

        // 開啟檔案
        private void openToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                int xmax, ymax, xmin, ymin;
                Bitmap platte = new Bitmap(16, 16);             //調色盤影像

                //判斷開啟圖片格式
                switch (openFileDialog1.FilterIndex)
                {
                    case 1:     //pcx file 

                        byte[,] colorPlatte = new byte[256, 3];     //調色盤陣列
                        int codeLength, codeValue;

                        //以二進位讀取選取的pcx檔

                        BinaryReader br1 = new BinaryReader(File.OpenRead(openFileDialog1.FileName));
                        hiform.change(openFileDialog1.SafeFileName);
                        hiform.addToTextBox("Manufacturer   : " + br1.ReadByte().ToString() + Environment.NewLine);             // Environment.NewLine表示換行
                        hiform.addToTextBox("Version        : " + br1.ReadByte().ToString() + Environment.NewLine);
                        hiform.addToTextBox("Encoding       : " + br1.ReadByte().ToString() + Environment.NewLine);
                        hiform.addToTextBox("BitsPerPixel   : " + br1.ReadByte().ToString() + Environment.NewLine);
                        hiform.addToTextBox("Xmin           : " + (xmin = br1.ReadInt16()).ToString() + Environment.NewLine);
                        hiform.addToTextBox("Ymin           : " + (ymin = br1.ReadInt16()).ToString() + Environment.NewLine);
                        hiform.addToTextBox("Xmax           : " + (xmax = br1.ReadInt16()).ToString() + Environment.NewLine);
                        hiform.addToTextBox("Ymax           : " + (ymax = br1.ReadInt16()).ToString() + Environment.NewLine);
                        hiform.addToTextBox("Hdpi           : " + br1.ReadInt16().ToString() + Environment.NewLine);
                        hiform.addToTextBox("Vdpi           : " + br1.ReadInt16().ToString() + Environment.NewLine);
                        //舊版調色盤的位置
                        for (int i = 0; i < 48; i++)
                            br1.ReadByte();
                        hiform.addToTextBox("ColorMap       : Undisplay" + Environment.NewLine);
                        hiform.addToTextBox("Reserved       : " + br1.ReadByte().ToString() + Environment.NewLine);
                        hiform.addToTextBox("NPlanes        : " + br1.ReadByte().ToString() + Environment.NewLine);
                        hiform.addToTextBox("BytesPerLine   : " + br1.ReadInt16().ToString() + Environment.NewLine);
                        hiform.addToTextBox("PlatteInfo     : " + br1.ReadInt16().ToString() + Environment.NewLine);
                        hiform.addToTextBox("HscreenSize    : " + br1.ReadInt16().ToString() + Environment.NewLine);
                        hiform.addToTextBox("VscreenSize    : " + br1.ReadInt16().ToString() + Environment.NewLine);
                        //未使用的byte
                        for (int i = 0; i < 54; i++)
                            br1.ReadByte();

                        //移到資料串流最後讀取調色盤

                        br1.BaseStream.Position = br1.BaseStream.Seek(-256 * 3, SeekOrigin.End);
                        for (int i = 0; i < 256; i++)
                            for (int j = 0; j < 3; j++)
                                colorPlatte[i, j] = br1.ReadByte();

                        //將調色盤陣列轉存成調色盤影像
                        for (int i = 0, k = 0; i < 16; i++)
                            for (int j = 0; j < 16; j++)
                                platte.SetPixel(i, j, Color.FromArgb(colorPlatte[k, 0], colorPlatte[k, 1], colorPlatte[k++, 2]));

                        //將調色盤影像傳到hiform class內做動作
                        hiform.setImage(platte);

                        //開始解碼像素資訊

                        //移動串流位置到第128個byte
                        br1.BaseStream.Position = br1.BaseStream.Seek(128, SeekOrigin.Begin);
                        originImage = new Bitmap(xmax - xmin + 1, ymax - ymin + 1);
                        for (int y = 0; y < originImage.Height; y++)
                            for (int x = 0; x < originImage.Width; )    //x在迴圈內部增加
                            {
                                byte b = br1.ReadByte();
                                //若此位元組內前2位元是高位(1)
                                if ((b & 0xC0) == 0xC0)  //11000000(0xC0的2進制) 只有11XXXXXX & 11000000會等於11000000
                                {
                                    //要重複次數
                                    codeLength = b & 0x3F;  //00111111(0x3F的二進制) 去掉最高2位元
                                    //要重複的値
                                    codeValue = br1.ReadByte();
                                }
                                else
                                {
                                    codeLength = 1;
                                    codeValue = b;
                                }
                                //由調色盤依照pixel填色
                                for (int i = 0; i < codeLength; i++)
                                    originImage.SetPixel(x++, y, Color.FromArgb(colorPlatte[codeValue, 0], colorPlatte[codeValue, 1], colorPlatte[codeValue, 2]));
                            }
                        pictureBox1.Image = originImage;
                        br1.Close();
                        break;
                    case 2:     //bmp file
                        originImage = new Bitmap(openFileDialog1.FileName);
                        pictureBox1.Image = originImage;
                        break;
                    default:    //其他類型圖檔
                        originImage = new Bitmap(openFileDialog1.FileName);
                        pictureBox1.Image = originImage;
                        break;
                }

                //有圖片才允許做圖片處理的動作
                ImageProcessingToolStripMenuItem.Enabled = true;
            }
        }

        //檢視檔頭資訊
        private void headerInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //******      **       **     *******
            //**    **    **       **   ***     ***
            //**    **    **       **  ***       ** 
            //********    **       **  ***
            //**      **  **       **  ***    ******
            //**      **  ***     ***   ***    *** * 
            //********     *********     *******   *       
            //只有讀pcx會有檔頭資訊，其他圖片類型不會有

            //判斷是否有檔頭資訊  
            if (hiform.create == true)
                hiform.Show();
            else
                MessageBox.Show("Please open the image first.", "Error");
        }

        //滑鼠移動，顯示圖片像素資訊
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //如果按滑鼠中鍵開啟輔助小視窗功能
            if (middlem)
            {
                //每次滑鼠移動圖就要重新繪製
                pictureBox1.Invalidate();
            }

            //如果圖片格1有圖片且互鎖變數c為0
            if (pictureBox1.Image != null && c == 0)
            {
                originImage = (Bitmap)pictureBox1.Image;
                int x = MousePosition.X - pictureBox1.Location.X - pictureBox1.Parent.Location.X - 1; //因為加入toolstripContainer 所以要-1
                int y = MousePosition.Y - 98;

                //視窗縮小有BUG**************************************************************
                //**************************************************************
                if (x < originImage.Width && y < originImage.Height && x >= 0 && y >= 0)
                {
                    statusOI.Text = "Origial Image : ";
                    statusLocation.Text = "Location : (" + x.ToString() + " , " + y.ToString() + ")";
                    statusR.Text = " R : " + originImage.GetPixel(x, y).R;
                    statusG.Text = " G : " + originImage.GetPixel(x, y).G;
                    statusB.Text = " B : " + originImage.GetPixel(x, y).B;
                    statusC.Text = "   ";
                    statusC.BackColor = originImage.GetPixel(x, y);


                    split.Text = "";
                    statusPI.Text = "";
                    statusC2.Text = "   ";
                    statusR2.Text = "";
                    statusG2.Text = "";
                    statusB2.Text = "";
                    statusC2.BackColor = Color.Transparent;

                    bmpN = (Bitmap)pictureBox2.Image;
                    if (bmpN != null)
                    {
                        if (bmpN.Width == originImage.Width && bmpN.Height == originImage.Height)
                        {
                            split.Text = " | ";
                            statusPI.Text = "Processed Image : ";
                            statusR2.Text = " R : " + bmpN.GetPixel(x, y).R;
                            statusG2.Text = " G : " + bmpN.GetPixel(x, y).G;
                            statusB2.Text = " B : " + bmpN.GetPixel(x, y).B;


                            statusC2.BackColor = bmpN.GetPixel(x, y);
                            //Graphics g = pictureBox2.CreateGraphics();
                            //g.DrawRectangle(Pens.White, x - 1, y - 1, 3, 3);                   
                        }
                    }
                }
            }
            if (m == 1 || m == 2 || m == 3)
                this.Cursor = Cursors.Cross;
            else
                this.Cursor = Cursors.Default;
        }

        Bitmap cutImage;
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    c = 1;
                    int x = MousePosition.X - pictureBox1.Location.X - pictureBox1.Parent.Location.X;
                    int y = MousePosition.Y - pictureBox1.Location.Y - pictureBox1.Parent.Location.Y - menuStrip1.Size.Height;
                    statusLocation.Text = "Location : (" + x.ToString() + " , " + y.ToString() + ")";
                    statusR.Text = " R : " + originImage.GetPixel(x, y).R;
                    statusG.Text = " G : " + originImage.GetPixel(x, y).G;
                    statusB.Text = " B : " + originImage.GetPixel(x, y).B;
                    pictureBox1.MouseMove -= new MouseEventHandler(this.pictureBox1_MouseMove);

                    if (m == 1)
                    {
                        x1 = e.Location.X;
                        y1 = e.Location.Y;
                        m++;
                        pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
                    }
                    else if (m == 2)
                    {
                        x2 = e.Location.X;
                        y2 = e.Location.Y;
                        int xmin = Math.Min(x1, x2);
                        int ymin = Math.Min(y1, y2);
                        int w = Math.Max(x1, x2) - xmin;
                        int h = Math.Max(y1, y2) - ymin;
                        Rectangle rect = new Rectangle(xmin, ymin, w, h);
                        Graphics g = pictureBox1.CreateGraphics();
                        g.DrawRectangle(Pens.White, rect);
                        pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
                        m = 0;

                        //MessageBox.Show(xmin + "  " + (xmin + w) + "   " + ymin+"  "+(ymin + h));
                        //MessageBox.Show(""+originImage.PixelFormat.ToString());
                        bmpN = (Bitmap)pictureBox1.Image.Clone();
                        bmpN = bmpN.Clone(new Rectangle(0, 0, originImage.Width, originImage.Height), PixelFormat.Format24bppRgb);
                        bmpN2 = bmpN.Clone(rect, PixelFormat.Format24bppRgb);
                        for (int j = 0; j < bmpN.Height; j++)
                            for (int i = 0; i < bmpN.Width; i++)
                                if ((i > xmin) && (i < xmin + w) && (j > ymin) && (j < ymin + h))
                                    bmpN.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                        pictureBox1.Image = bmpN;
                    }
                    else if (m == 3)
                    {
                        Point p = new Point(e.X, e.Y);

                        line++;
                        pt.Add(p);

                        if (pt.Count >= 2)
                        {
                            //看是否連成封閉區域
                            if (e.X >= pt[0].X - 3 && e.X <= pt[0].X + 3 && e.Y >= pt[0].Y - 3 && e.Y <= pt[0].Y + 3)
                            {
                                //是，進行切割動作
                                pt.RemoveAt(line - 1);
                                pt.Add(pt[0]);

                                int minx = pt[0].X;
                                int maxx = pt[0].X;
                                int miny = pt[0].Y;
                                int maxy = pt[0].Y;

                                //找點集合中最小和最大的x,y
                                for (int i = 0; i < pt.Count; i++)
                                {
                                    if (pt[i].X < minx)
                                        minx = pt[i].X;
                                    if (pt[i].X > maxx)
                                        maxx = pt[i].X;
                                    if (pt[i].Y < miny)
                                        miny = pt[i].Y;
                                    if (pt[i].Y > maxy)
                                        maxy = pt[i].Y;
                                }

                                bmpN = (Bitmap)pictureBox1.Image.Clone();
                                cutImage = new Bitmap(maxx - minx + 1, maxy - miny + 1);
                                Bitmap bm = bmpN.Clone(new Rectangle(0, 0, bmpN.Width, bmpN.Height), PixelFormat.Format24bppRgb);
                                cutImage = bm.Clone(new Rectangle(minx, miny, maxx - minx + 1, maxy - miny + 1), PixelFormat.Format24bppRgb);

                                Graphics g = Graphics.FromImage(bm);
                                g.FillPolygon(Brushes.White, pt.ToArray());

                                //點集合轉換成小方框內的座標
                                for (int i = 0; i < pt.Count; i++)
                                {
                                    Point pinsert = new Point(pt[i].X - minx, pt[i].Y - miny);
                                    pt.RemoveAt(i);
                                    pt.Insert(i, pinsert);
                                }

                                //作記號
                                Bitmap bcutImage = (Bitmap)cutImage.Clone();
                                Bitmap wcutImage = (Bitmap)cutImage.Clone();

                                g = Graphics.FromImage(wcutImage);
                                g.FillPolygon(Brushes.White, pt.ToArray());

                                g = Graphics.FromImage(bcutImage);
                                g.FillPolygon(Brushes.Black, pt.ToArray());

                                for (int y2 = 0; y2 < cutImage.Height; y2++)
                                    for (int x2 = 0; x2 < cutImage.Width; x2++)
                                    {
                                        Color c2 = new Color();
                                        Color c3 = new Color();
                                        c2 = wcutImage.GetPixel(x2, y2);
                                        c3 = bcutImage.GetPixel(x2, y2);

                                        if (wcutImage.GetPixel(x2, y2) == bcutImage.GetPixel(x2, y2))
                                            cutImage.SetPixel(x2, y2, Color.Transparent);
                                    }

                                pictureBox1.Image = bm;
                                bmpN2 = cutImage;
                                m = 0;

                            }
                            else
                            {
                                //否，繼續蒐集點
                                Graphics g = pictureBox1.CreateGraphics();
                                g.DrawLine(Pens.Red, pt[line - 2], pt[line - 1]);
                            }
                        }
                        else if (pt.Count == 1)
                        {
                            Graphics g = pictureBox1.CreateGraphics();
                            g.DrawEllipse(Pens.Red, e.X - 2, e.Y - 2, 5, 5);
                        }
                    }
                }

                if (e.Button == MouseButtons.Right)
                {
                    c = 0;
                    pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
                }
                if (e.Button == MouseButtons.Middle)
                {
                    middlem = !middlem;
                    pictureBox1.Invalidate();
                    if (middlem)
                    {
                        pdf.Show();
                    }
                    else
                    {
                        pdf.Hide();
                    }
                }

            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RGB('r');
        }

        private void greenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RGB('g');
        }

        private void blueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RGB('b');
        }

        private void RGB(char color)
        {
            if (pictureBox1.Image != null)
            {
                int c = 0;
                byte b = 0;
                switch (color)
                {
                    case 'r':
                        c = 2;
                        break;
                    case 'g':
                        c = 1;
                        break;
                    case 'b':
                        c = 0;
                        break;
                }
                Bitmap bmpN;
                Rectangle rect = new Rectangle(0, 0, originImage.Width, originImage.Height);
                bmpN = originImage.Clone(rect, PixelFormat.Format24bppRgb);
                BitmapData bmpData = bmpN.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                System.IntPtr scan0 = bmpData.Scan0;
                int stride = bmpData.Stride;
                unsafe
                {
                    byte* p = (byte*)scan0;
                    int offset = stride - originImage.Width * 3;
                    for (int y = 0; y < originImage.Height; y++)
                    {
                        for (int x = 0; x < originImage.Width; x++)
                        {
                            b = p[c];
                            p[0] = 0;
                            p[1] = 0;
                            p[2] = 0;
                            p[c] = b;
                            p += 3;
                        }
                        p += offset;
                    }
                }
                bmpN.UnlockBits(bmpData);
                pictureBox2.Image = bmpN;
            }
        }

        private void grayLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmpN;
            Rectangle rect = new Rectangle(0, 0, pictureBox1.Image.Width, pictureBox1.Image.Height);
            bmpN = (Bitmap)pictureBox1.Image.Clone();

            BitmapData bmpd = bmpN.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int offset = bmpd.Stride - bmpN.Width * 3;

            unsafe
            {
                byte* p = (byte*)bmpd.Scan0;

                for (int y = 0; y < pictureBox1.Image.Height; y++, p += offset)
                {
                    for (int x = 0; x < pictureBox1.Image.Width; x++, p += 3)
                    {
                        byte gray = Convert.ToByte(p[2] * 0.299 + p[1] * 0.587 + p[0] * 0.114);
                        if (p[0] == 22 || p[0] == 23)
                            offset = bmpd.Stride - bmpN.Width * 3;
                        p[0] = p[1] = p[2] = gray;
                    }
                }
            }

            bmpN.UnlockBits(bmpd);
            pictureBox2.Image = bmpN;
        }


        private void pictureMoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image != null)
            {
                originImage = (Bitmap)pictureBox2.Image;
                pictureBox1.Image = originImage;
            }
        }

        private void negativeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rectangle rect = new Rectangle(0, 0, originImage.Width, originImage.Height);
            bmpN = originImage.Clone(rect, PixelFormat.Format24bppRgb);
            for (int y = 0; y < originImage.Height; y++)
                for (int x = 0; x < originImage.Width; x++)
                {
                    bmpN.SetPixel(x, y, Color.FromArgb(255 - bmpN.GetPixel(x, y).B, 255 - bmpN.GetPixel(x, y).G, 255 - bmpN.GetPixel(x, y).R));
                }
            pictureBox2.Image = bmpN;
        }


        public float scalingValue;
        //縮小(decimation)
        private void shrinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int w, h;

            ScalingForm sf = new ScalingForm(this, "shrink (decimation)");
            sf.ShowDialog();
            if (scalingValue != float.MaxValue)
            {
                scalingValue = scalingValue / 100f;

                w = Convert.ToInt32(originImage.Width * scalingValue);
                h = Convert.ToInt32(originImage.Height * scalingValue);
                bmpN = new Bitmap(w, h);

                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                    {
                        Color c = originImage.GetPixel(Convert.ToInt32(x / scalingValue), Convert.ToInt32(y / scalingValue));
                        bmpN.SetPixel(x, y, c);
                    }
                pictureBox2.Image = bmpN;
            }
        }

        private void shrinkAverageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int w, h;
            int r, g, b;
            int[, ,] mark;

            ScalingForm sf = new ScalingForm(this, "shrink (Average)");
            sf.ShowDialog();
            if (scalingValue != float.MaxValue)
            {
                scalingValue = scalingValue / 100f;

                w = Convert.ToInt32(originImage.Width * scalingValue);
                h = Convert.ToInt32(originImage.Height * scalingValue);
                bmpN = new Bitmap(w, h);
                mark = new int[w, h, 2];    //紀錄縮小圖每個pixel由哪幾個原圖pixel(長 寬)構成

                //
                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                    {
                        if ((x + 1) < w)
                            mark[x, y, 0] = Convert.ToInt32((x + 1) / scalingValue) - Convert.ToInt32(x / scalingValue);
                        else
                            mark[x, y, 0] = originImage.Width - Convert.ToInt32(x / scalingValue);
                        if ((y + 1) < h)
                            mark[x, y, 1] = Convert.ToInt32((y + 1) / scalingValue) - Convert.ToInt32(y / scalingValue);
                        else
                            mark[x, y, 1] = originImage.Height - Convert.ToInt32(y / scalingValue);
                    }
                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                    {
                        r = g = b = 0;
                        for (int j = 0; j < mark[x, y, 1]; j++)
                            for (int i = 0; i < mark[x, y, 0]; i++)
                            {
                                r += originImage.GetPixel(Convert.ToInt32(x / scalingValue) + i, Convert.ToInt32(y / scalingValue) + j).R;
                                g += originImage.GetPixel(Convert.ToInt32(x / scalingValue) + i, Convert.ToInt32(y / scalingValue) + j).G;
                                b += originImage.GetPixel(Convert.ToInt32(x / scalingValue) + i, Convert.ToInt32(y / scalingValue) + j).B;
                            }
                        r /= (mark[x, y, 0] * mark[x, y, 1]);
                        g /= (mark[x, y, 0] * mark[x, y, 1]);
                        b /= (mark[x, y, 0] * mark[x, y, 1]);
                        bmpN.SetPixel(x, y, Color.FromArgb(r, g, b));
                    }
                pictureBox2.Image = bmpN;
            }
        }

        private void enlargeDuplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {

            int w, h;
            bool[,] mark;

            ScalingForm sf = new ScalingForm(this, "enlarge (Duplication)");
            sf.ShowDialog();
            if (scalingValue != float.MaxValue)
            {
                scalingValue = scalingValue / 100f;

                w = Convert.ToInt32(originImage.Width * scalingValue);
                h = Convert.ToInt32(originImage.Height * scalingValue);
                bmpN = new Bitmap(w, h);

                mark = new bool[w, h];
                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                        mark[x, y] = false;

                for (int y = 0; y < originImage.Height; y++)
                    for (int x = 0; x < originImage.Width; x++)
                    {
                        Color c = originImage.GetPixel(x, y);
                        bmpN.SetPixel(Convert.ToInt32(x * scalingValue), Convert.ToInt32(y * scalingValue), c);
                        mark[Convert.ToInt32(x * scalingValue), Convert.ToInt32(y * scalingValue)] = true;
                    }

                for (int y = 0; y < h; y++)
                {
                    bool head = false;
                    for (int x = 0; x < w; x++)
                    {
                        if (mark[x, y] == false)
                        {
                            if (y == 0)
                                bmpN.SetPixel(x, y, Color.FromArgb(bmpN.GetPixel(x - 1, y).R, bmpN.GetPixel(x - 1, y).G, bmpN.GetPixel(x - 1, y).B));
                            else if (x == 0)
                            {
                                bmpN.SetPixel(x, y, Color.FromArgb(bmpN.GetPixel(x, y - 1).R, bmpN.GetPixel(x, y - 1).G, bmpN.GetPixel(x, y - 1).B));
                                head = true;
                            }
                            else
                            {
                                if ((head == true) && (bmpN.GetPixel(x - 1, y) != bmpN.GetPixel(x, y - 1)))
                                    bmpN.SetPixel(x, y, Color.FromArgb(bmpN.GetPixel(x, y - 1).R, bmpN.GetPixel(x, y - 1).G, bmpN.GetPixel(x, y - 1).B));
                                else
                                    bmpN.SetPixel(x, y, Color.FromArgb(bmpN.GetPixel(x - 1, y).R, bmpN.GetPixel(x - 1, y).G, bmpN.GetPixel(x - 1, y).B));
                            }
                        }
                    }
                }
                pictureBox2.Image = bmpN;
            }
        }

        private void enlargeInterpolationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmpN;
            int w, h;
            bool[,] mark;

            int a, c;
            int r, g, b;

            ScalingForm sf = new ScalingForm(this, "enlarge (Interpolation)");
            sf.ShowDialog();
            if (scalingValue != float.MaxValue)
            {
                scalingValue = scalingValue / 100f;


                //圖片放大後的長寬
                w = Convert.ToInt32(originImage.Width * scalingValue);
                h = Convert.ToInt32(originImage.Height * scalingValue);
                bmpN = new Bitmap(w, h);
                mark = new bool[w, h];      //標記哪些PIXEL已填入色彩

                //先將全圖的pixel標記為false(代表未填入色彩)
                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                        mark[x, y] = false;

                //將原圖的每個pixel轉移到新圖的位置
                for (int y = 0; y < originImage.Height; y++)
                    for (int x = 0; x < originImage.Width; x++)
                    {
                        int nx = Convert.ToInt32(x * scalingValue);           //原圖x座標轉移後的新x座標
                        int ny = Convert.ToInt32(y * scalingValue);           //原圖y座標轉移後的新y座標
                        bmpN.SetPixel(nx, ny, originImage.GetPixel(x, y));  //填入新座標對應原圖舊座標的色彩
                        mark[nx, ny] = true;                                //將這些已填入色彩的座標做記號
                    }

                //開始進行內插作業
                //先執行橫向內插
                for (int y = 0; y < h; y++)
                {
                    //如果某列第一行的值為false(未填入色彩)，該列皆為false，所以不能進行橫向內插
                    if (mark[0, y] == false)
                        continue;
                    //如果某列的第一行的值為true(已填入色彩)，則該列可進行橫向內插
                    for (int x = 0; x < w; x++)
                    {
                        //若此列的某行未填入色彩
                        if (mark[x, y] == false)
                        {
                            c = a = 1;
                            //向左尋找距離多遠(a值)有已填色過的pixel
                            while (mark[x - a, y] != true)
                                a++;
                            //向右尋找距離多遠(c值)有已填色過的pixel，並且要注意最右端會有找到底還找不到有已填色pixel的狀況
                            while ((x + c < w) && (mark[x + c, y] != true))
                                c++;
                            //如果找到最右一行還找不到，則將最右端連接最左端，以左端第一個已填色的pixel當作內插右端值
                            if (x + c >= w)
                            {
                                //由a,c當作權重值乘以左右兩端已填色pixel值計算出此piexl應該填入的色彩
                                r = Convert.ToInt32((1.0 * c / (a + c)) * bmpN.GetPixel(x - a, y).R + (1.0 * a / (a + c)) * bmpN.GetPixel(0, y).R);
                                g = Convert.ToInt32((1.0 * c / (a + c)) * bmpN.GetPixel(x - a, y).G + (1.0 * a / (a + c)) * bmpN.GetPixel(0, y).G);
                                b = Convert.ToInt32((1.0 * c / (a + c)) * bmpN.GetPixel(x - a, y).B + (1.0 * a / (a + c)) * bmpN.GetPixel(0, y).B);
                                bmpN.SetPixel(x, y, Color.FromArgb(r, g, b));
                                mark[x, y] = true;      //填完色的pixel標記修改為true
                            }
                            else
                            {
                                //由a,c當作權重值乘以左右兩端已填色pixel值計算出此piexl應該填入的色彩
                                r = Convert.ToInt32((1.0 * c / (a + c)) * bmpN.GetPixel(x - a, y).R + (1.0 * a / (a + c)) * bmpN.GetPixel(x + c, y).R);
                                g = Convert.ToInt32((1.0 * c / (a + c)) * bmpN.GetPixel(x - a, y).G + (1.0 * a / (a + c)) * bmpN.GetPixel(x + c, y).G);
                                b = Convert.ToInt32((1.0 * c / (a + c)) * bmpN.GetPixel(x - a, y).B + (1.0 * a / (a + c)) * bmpN.GetPixel(x + c, y).B);
                                bmpN.SetPixel(x, y, Color.FromArgb(r, g, b));
                                mark[x, y] = true;      //填完色的pixel標記修改為true
                            }
                        }
                    }
                }
                //開始進行縱向內插，原理橫向內插
                for (int y = 0; y < h; y++)
                {
                    if (mark[0, y])
                        continue;
                    for (int x = 0; x < w; x++)
                    {
                        c = a = 1;
                        while (mark[x, y - a] != true)
                            a++;
                        while ((y + c < h) && (mark[x, y + c] != true))
                            c++;
                        if (y + c >= h)
                        {
                            r = Convert.ToInt32((1.0 * c / (a + c)) * bmpN.GetPixel(x, y - a).R + (1.0 * a / (a + c)) * bmpN.GetPixel(x, 0).R);
                            g = Convert.ToInt32((1.0 * c / (a + c)) * bmpN.GetPixel(x, y - a).G + (1.0 * a / (a + c)) * bmpN.GetPixel(x, 0).G);
                            b = Convert.ToInt32((1.0 * c / (a + c)) * bmpN.GetPixel(x, y - a).B + (1.0 * a / (a + c)) * bmpN.GetPixel(x, 0).B);
                            bmpN.SetPixel(x, y, Color.FromArgb(r, g, b));
                        }
                        else
                        {
                            r = Convert.ToInt32((1.0 * c / (a + c)) * bmpN.GetPixel(x, y - a).R + (1.0 * a / (a + c)) * bmpN.GetPixel(x, y + c).R);
                            g = Convert.ToInt32((1.0 * c / (a + c)) * bmpN.GetPixel(x, y - a).G + (1.0 * a / (a + c)) * bmpN.GetPixel(x, y + c).G);
                            b = Convert.ToInt32((1.0 * c / (a + c)) * bmpN.GetPixel(x, y - a).B + (1.0 * a / (a + c)) * bmpN.GetPixel(x, y + c).B);
                            bmpN.SetPixel(x, y, Color.FromArgb(r, g, b));
                        }

                    }
                }
                pictureBox2.Image = bmpN;
            }
        }

        private void sourceDestinationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double angle;

            ScalingForm sf = new ScalingForm(this, "rotation angle (sourceDestination)");
            sf.ShowDialog();
            if (scalingValue != float.MaxValue)
            {
                angle = scalingValue;

                while (angle < 0)
                    angle += 360;

                while (angle > 360)
                    angle -= 360;

                //使用math.cos()，角度須先轉成徑度
                angle = angle * Math.PI / 180;
                pictureBox2.Image = rotate(angle, originImage);

                /*
                double angle;
                double cosa, sina;
                int nw, nh;
                int rcx, rcy;
                int nx, ny;
                Bitmap bmpN,bmpN2;
                int x1,x2,x3,x4;
                int y1,y2,y3,y4;
                int xmin, xmax, ymin, ymax;
                xmin = xmax = ymin = ymax = 0;

                ScalingForm sf = new ScalingForm();
                sf.setLabel("Please enter the rotation angle :","degrees");
                sf.ShowDialog();
                angle = double.Parse("" + sf.getScalingValue());
                sf.Close();

                while (angle < 0)
                    angle += 360;

                while (angle > 360)
                    angle -= 360;
    
                //使用math.cos()，角度須先轉成徑度
                angle = angle * Math.PI / 180;
            
                cosa = Math.Cos(angle);
                sina = Math.Sin(angle);

                //算原圖對角線長度，和原圖的中心點(用來當旋轉中心)
                nh = nw = (int)Math.Round(Math.Sqrt(Math.Pow(originImage.Width, 2) + Math.Pow(originImage.Height, 2)));
                rcx = (int)Math.Round(originImage.Width / 2 + 0.5);
                rcy = (int)Math.Round(originImage.Height / 2 + 0.5);

                //開一張保證能放得下原圖各種旋轉角度的畫布(可能會含有白行或白列)
                bmpN = new Bitmap(nw, nh);

                for (int y = 0; y < originImage.Height; y++)
                    for (int x = 0; x < originImage.Width; x++)
                    {
                        //開始將新畫布填色
                        nx = (int)Math.Round(((x - rcx) * cosa - (y - rcy) * sina));
                        ny = (int)Math.Round(((x - rcx) * sina + (y - rcy) * cosa));

                        Color c = originImage.GetPixel(x, y);
                        bmpN.SetPixel(nx + nw / 2, ny + nh / 2, c);
                    
                        //紀錄有使用到的畫布大小(4個邊界值)
                        if (x == 0 && y == 0)
                        {
                            x1 = nx + nw / 2;
                            y1 = ny + nh / 2;
                            xmax = xmin = x1;
                            ymax = ymin = y1;
                        }
                        else if (x == 0 && y == originImage.Height - 1)
                        {
                            x2 = nx + nw / 2;
                            y2 = ny + nh / 2;
                            xmin = Math.Min(xmin, x2);
                            xmax = Math.Max(xmax, x2);
                            ymin = Math.Min(ymin, y2);
                            ymax = Math.Max(ymax, y2);
                        }
                        else if (x == originImage.Width - 1 && y == 0)
                        {
                            x3 = nx + nw / 2;
                            y3 = ny + nh / 2;
                            xmin = Math.Min(xmin, x3);
                            xmax = Math.Max(xmax, x3);
                            ymin = Math.Min(ymin, y3);
                            ymax = Math.Max(ymax, y3);
                        }
                        else if (x == originImage.Width - 1 && y == originImage.Height - 1)
                        {
                            x4 = nx + nw / 2;
                            y4 = ny + nh / 2;
                            xmin = Math.Min(xmin, x4);
                            xmax = Math.Max(xmax, x4);
                            ymin = Math.Min(ymin, y4);
                            ymax = Math.Max(ymax, y4);
                        }
                    }

                //要去除白行和白列→複製沒有白行和白列的影像部分
                bmpN2 = new Bitmap(xmax - xmin + 1, ymax - ymin + 1);
            
                for (int y = ymin; y <= ymax; y++)
                    for (int x = xmin; x <= xmax; x++)
                    {
                        Color c = bmpN.GetPixel(x, y);
                        bmpN2.SetPixel(x - xmin, y - ymin, c);
                    }
            
                pictureBox2.Image = bmpN2;
                */
            }
        }

        //pictureBox2隨pictureBox1移動位置
        private void pictureBox1_SizeChanged_1(object sender, EventArgs e)
        {
            Point p = new Point(pictureBox1.Location.X + pictureBox1.Size.Width + 50, pictureBox2.Location.Y);
            pictureBox2.Location = p;

        }

        private void destinationSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {

            double angle;
            //Bitmap bmp;
            double cosa, sina;
            int rcx, rcy;

            ScalingForm sf = new ScalingForm(this, "rotation angle (destinationSource)");
            sf.ShowDialog();
            if (scalingValue != float.MaxValue)
            {
                angle = scalingValue;

                while (angle < 0)
                    angle += 360;

                while (angle > 360)
                    angle -= 360;

                //使用math.cos()，角度須先轉成徑度
                angle = angle * Math.PI / 180;
                bmpN = rotate(angle, originImage);

                angle *= -1;
                cosa = Math.Cos(angle);
                sina = Math.Sin(angle);

                //計算旋轉中心座標
                rcx = (int)Math.Round(bmpN.Width / 2 + 0.5);
                rcy = (int)Math.Round(bmpN.Height / 2 + 0.5);

                for (int y = 0; y < bmpN.Height; y++)
                    for (int x = 0; x < bmpN.Width; x++)
                    {
                        //計算x,y對於旋轉中心旋轉回原角度後的值
                        int rx = (int)Math.Round(((x - rcx) * cosa - (y - rcy) * sina));
                        int ry = (int)Math.Round(((x - rcx) * sina + (y - rcy) * cosa));

                        //判斷旋轉後的值是否落在最原始影像的範圍內
                        if ((Math.Abs(rx) < originImage.Width / 2) && (Math.Abs(ry) < originImage.Height / 2))
                        {
                            Color c = originImage.GetPixel(rx + originImage.Width / 2, ry + originImage.Height / 2);
                            bmpN.SetPixel(x, y, c);
                        }
                    }
                pictureBox2.Image = bmpN;
            }
        }

        //正轉45度在轉-45度會有問題
        private Bitmap rotate(double angle, Bitmap bmp)
        {
            double cosa, sina;
            int nw, nh;
            int rcx, rcy;
            int nx, ny;
            Bitmap bmpN, bmpN2;
            int x1, x2, x3, x4;
            int y1, y2, y3, y4;
            int xmin, xmax, ymin, ymax;
            xmin = xmax = ymin = ymax = 0;

            cosa = Math.Cos(angle);
            sina = Math.Sin(angle);

            //算原圖對角線長度，和原圖的中心點(用來當旋轉中心)
            nh = nw = (int)Math.Round(Math.Sqrt(Math.Pow(bmp.Width, 2) + Math.Pow(bmp.Height, 2)));
            rcx = (int)Math.Round(bmp.Width / 2 + 0.5);
            rcy = (int)Math.Round(bmp.Height / 2 + 0.5);

            //開一張保證能放得下原圖各種旋轉角度的畫布(可能會含有白行或白列)
            bmpN = new Bitmap(nw, nh);

            for (int y = 0; y < bmp.Height; y++)
                for (int x = 0; x < bmp.Width; x++)
                {
                    //開始將新畫布填色
                    nx = (int)Math.Round(((x - rcx) * cosa - (y - rcy) * sina));
                    ny = (int)Math.Round(((x - rcx) * sina + (y - rcy) * cosa));

                    Color c = bmp.GetPixel(x, y);
                    bmpN.SetPixel(nx + nw / 2, ny + nh / 2, c);

                    //紀錄有使用到的畫布大小(4個邊界值)
                    if (x == 0 && y == 0)
                    {
                        x1 = nx + nw / 2;
                        y1 = ny + nh / 2;
                        xmax = xmin = x1;
                        ymax = ymin = y1;
                    }
                    else if (x == 0 && y == bmp.Height - 1)
                    {
                        x2 = nx + nw / 2;
                        y2 = ny + nh / 2;
                        xmin = Math.Min(xmin, x2);
                        xmax = Math.Max(xmax, x2);
                        ymin = Math.Min(ymin, y2);
                        ymax = Math.Max(ymax, y2);
                    }
                    else if (x == bmp.Width - 1 && y == 0)
                    {
                        x3 = nx + nw / 2;
                        y3 = ny + nh / 2;
                        xmin = Math.Min(xmin, x3);
                        xmax = Math.Max(xmax, x3);
                        ymin = Math.Min(ymin, y3);
                        ymax = Math.Max(ymax, y3);
                    }
                    else if (x == bmp.Width - 1 && y == bmp.Height - 1)
                    {
                        x4 = nx + nw / 2;
                        y4 = ny + nh / 2;
                        xmin = Math.Min(xmin, x4);
                        xmax = Math.Max(xmax, x4);
                        ymin = Math.Min(ymin, y4);
                        ymax = Math.Max(ymax, y4);
                    }
                }

            //要去除白行和白列→複製沒有白行和白列的影像部分
            bmpN2 = new Bitmap(xmax - xmin + 1, ymax - ymin + 1);

            for (int y = ymin; y <= ymax; y++)
                for (int x = xmin; x <= xmax; x++)
                {
                    Color c = bmpN.GetPixel(x, y);
                    bmpN2.SetPixel(x - xmin, y - ymin, c);
                }
            return bmpN2;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            m = 1;
        }

        private void rectangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton1.PerformClick();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bmpN2 != null)
            {
                bb = true;
                tabPage1.Invalidate();
            }
        }

        private void tabPage1_Paint(object sender, PaintEventArgs e)
        {
            if (bmpN2 != null && bb == true)
            {
                e.Graphics.DrawImage(bmpN2, x1, y1, bmpN2.Width, bmpN2.Height);
                bb = false;
            }
            else if (bmpN2 == null && bb == true)
            {
                bb = false;
                tabPage1.BackColor = Color.Transparent;
            }

        }

        private void tabPage1_MouseDown(object sender, MouseEventArgs e)
        {
            if (bmpN2 != null)
            {
                x1 = e.X;
                y1 = e.Y;
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bmpN2 = null;
            bb = true;
            tabPage1.Invalidate();
        }

        private void arbitraryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m = 3;
            line = 0;
        }

        private void transparencyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ThreePictureForm tf = new ThreePictureForm("Transparency");
            tf.Text = "Transparency";
            tf.Show();
        }

        private void pixelDivisionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ThreePictureForm tf = new ThreePictureForm("Pixel Division");
            tf.Text = "Pixel Division";
            tf.Show();
        }


        private void toolStripButton2_Click(object sender, EventArgs e)
        {
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
                            //MessageBox.Show(""+bmpN.GetPixel(x, y).R+"  "+bmpN2.GetPixel(x, y).R+"  " + Convert.ToInt32(Math.Pow(bmpN.GetPixel(x, y).R - bmpN2.GetPixel(x, y).R, 2)));
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

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bmpN = (Bitmap)pictureBox1.Image;

            histogramForm hf = new histogramForm(bmpN);
            hf.Show();
        }

        private void contrastStretchingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //不能用一樣，要複製，不然pictureBox1的影像也會同時被更改
            bmpN2 = (Bitmap)pictureBox1.Image.Clone();

            byte boundaryL = 0;
            byte boundaryR = 0;
            Rectangle rect = new Rectangle(0, 0, bmpN2.Width, bmpN2.Height);
            BitmapData bmpd = bmpN2.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int stride = bmpd.Stride;
            int offset = stride - bmpN2.Width * 3;
            ulong percent = 1;
            ulong dd;

            for (int i = 0; i < 256; i++)
                pixelnumber[i] = 0;
            max = 0;

            unsafe
            {
                byte* p = (byte*)bmpd.Scan0.ToPointer();

                for (int y = 0; y < bmpN2.Height; y++, p += offset)
                    for (int x = 0; x < bmpN2.Width; x++, p += 3)
                    {
                        byte g = Convert.ToByte(0.114 * p[0] + 0.587 * p[1] + 0.299 * p[2]);
                        pixelnumber[g]++;
                        if (pixelnumber[g] > pixelnumber[max])
                            max = g;
                    }

                dd = (ulong)(pixelnumber[max] * percent / 100.0);
                //MessageBox.Show("pixelnum[max] = " + pixelnumber[max] + "  max = " + max);
                p = (byte*)bmpd.Scan0.ToPointer();

                for (int x = 0; x <= 255; x++)
                    if (pixelnumber[x] <= dd)
                        continue;
                    else
                    {
                        boundaryL = (byte)x;
                        break;
                    }

                for (int x = 255; x >= 0; x--)
                    if (pixelnumber[x] <= dd)
                        continue;
                    else
                    {
                        boundaryR = (byte)x;
                        break;
                    }
                //MessageBox.Show("dd = " + dd + "  l= " + boundaryL + "  r=" + boundaryR);

                for (int y = 0; y < bmpN2.Height; y++, p += offset)
                    for (int x = 0; x < bmpN2.Width; x++, p += 3)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            int v = ((p[i] - boundaryL) * (255 - 0) / ((boundaryR - boundaryL) + 0));
                            if (v < 0)
                                v = 0;
                            else if (v > 255)
                                v = 255;
                            p[i] = (byte)v;
                        }

                    }
            }
            bmpN2.UnlockBits(bmpd);
            pictureBox2.Image = bmpN2;
        }

        private void histogramEqulizationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ulong[] cdf = new ulong[256];

            bmpN2 = (Bitmap)pictureBox1.Image.Clone();

            //統計直方圖
            pixelDataCaculate(bmpN2);

            for (int i = 0; i < 256; i++)
                cdf[i] = pixelnumber[i];

            //統計累計分布圖
            for (int i = 1; i < 256; i++)
                cdf[i] = cdf[i] + cdf[i - 1];

            Rectangle rect = new Rectangle(0, 0, bmpN2.Width, bmpN2.Height);
            BitmapData bmpd = bmpN2.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int stride = bmpd.Stride;
            int offset = stride - bmpN2.Width * 3;

            unsafe
            {
                byte* p = (byte*)bmpd.Scan0.ToPointer();

                for (int y = 0; y < bmpN2.Height; y++, p += offset)
                    for (int x = 0; x < bmpN2.Width; x++, p += 3)
                    {
                        //灰階化 g = 0 ~ 255
                        byte g = Convert.ToByte(0.114 * p[0] + 0.587 * p[1] + 0.299 * p[2]);
                        int newpixel;

                        //最大值 g =255 cdf[255] = width * height個數目 轉換後的値(newpixel)等於255
                        //cdf個數越多轉換後的newpixel越大，因此是依照cdf增加的値決定轉換後newpixel的灰階增量
                        newpixel = Convert.ToInt32(Math.Round(Convert.ToDouble(cdf[g] * 256.0 / (bmpN2.Width * bmpN2.Height))) - 1);
                        if (newpixel < 0)
                            newpixel = 0;
                        p[0] = p[1] = p[2] = Convert.ToByte(newpixel);
                    }
            }

            bmpN2.UnlockBits(bmpd);
            pictureBox2.Image = bmpN2;

        }

        private void cumlativeDistributionFunctionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bmpN2 = (Bitmap)pictureBox1.Image.Clone();
            pixelDataCaculate(bmpN2);

            CDFForm cdf = new CDFForm(pixelnumber);
            cdf.Show();
        }

        private void pixelDataCaculate(Bitmap bmpN2)
        {
            Rectangle rect = new Rectangle(0, 0, bmpN2.Width, bmpN2.Height);
            BitmapData bmpd = bmpN2.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int stride = bmpd.Stride;
            int offset = stride - bmpN2.Width * 3;

            for (int i = 0; i < 256; i++)
                pixelnumber[i] = 0;
            max = 0;

            unsafe
            {
                byte* p = (byte*)bmpd.Scan0.ToPointer();

                for (int y = 0; y < bmpN2.Height; y++, p += offset)
                    for (int x = 0; x < bmpN2.Width; x++, p += 3)
                    {
                        byte g = Convert.ToByte(0.114 * p[0] + 0.587 * p[1] + 0.299 * p[2]);
                        pixelnumber[g]++;
                        if (pixelnumber[g] > pixelnumber[max])
                            max = g;
                    }
            }

            bmpN2.UnlockBits(bmpd);
        }

        private void histogramSpecificationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bmpN2 = (Bitmap)pictureBox1.Image.Clone();
            pixelDataCaculate(bmpN2);
            TransferFunctionForm tfform = new TransferFunctionForm(bmpN2);
            tfform.Show();

        }

        private void binaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bmpN2 = (Bitmap)pictureBox1.Image.Clone();
            bitPlaneSlicingForm bpsf = new bitPlaneSlicingForm(bmpN2, 0, this);
            bpsf.Text = "Binary Code Bit-plane Slicing";
            bpsf.Show();
        }

        private void grayCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bmpN2 = (Bitmap)pictureBox1.Image.Clone();
            bitPlaneSlicingForm bpsf = new bitPlaneSlicingForm(bmpN2, 1);
            bpsf.Text = "Gray Code Bit-plane Slicing";
            bpsf.Show();
        }

        public void sendimage(Bitmap b)
        {
            pictureBox2.Image = b;
        }

        private void manulToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bmpN = (Bitmap)pictureBox1.Image.Clone();
            ManualSettingForm msf = new ManualSettingForm(bmpN, this, 0);
            msf.Show();
        }

        private void otsuMethodToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bmpN = (Bitmap)pictureBox1.Image.Clone();
            pixelDataCaculate(bmpN);
            Rectangle rect = new Rectangle(0, 0, bmpN.Width, bmpN.Height);
            BitmapData bmpd = bmpN.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            int offset = bmpd.Stride - bmpN.Width * 3;
            int threshold;
            double n = bmpN.Width * bmpN.Height;    //影像中像素的總量
            double min = double.MaxValue;
            byte t = 0;                             //存取最適合的閥值

            for (threshold = 1; threshold < 255; threshold++)
            {
                double n1 = 0;          //影像中小於等於閥值像素的總量
                double n2 = 0;          //影像中大於閥值像素的總量 
                //n1 + n2 = n
                double q1;              //小於等於閥值像素出現的機率
                double q2;              //大於閥值像素出現的機率
                double u1 = 0;          //小於等於閥值像素的平均數(以像素的數量為權重)
                //例:pixel 10出現5次  pixel 7 出現3次 pixel 200出現2次
                //mean = 10*5+7*3+200*2/(5+3+2)
                double u2 = 0;          //大於閥值像素的平均數
                double powsigma1 = 0;   //小於等於閥值像素的標準差平方
                double powsigma2 = 0;   //大於閥值像素的標準差平方
                double sigmaw = 0;
                unsafe
                {
                    byte* p = (byte*)bmpd.Scan0;
                    for (int y = 0; y < bmpN.Height; y++, p += offset)
                        for (int x = 0; x < bmpN.Width; x++, p += 3)
                        {
                            if (p[0] <= threshold)
                                n1++;
                            else
                                n2++;
                        }
                    q1 = n1 / n;
                    q2 = n2 / n;

                    for (int i = 0; i < 256; i++)
                    {
                        if (i <= threshold)
                            u1 += i * Convert.ToDouble(pixelnumber[i]) / n1;
                        else
                            u2 += i * Convert.ToDouble(pixelnumber[i]) / n2;

                    }

                    for (int i = 0; i < 256; i++)
                    {
                        if (i <= threshold)
                            powsigma1 += Math.Pow((i - u1), 2) * Convert.ToDouble(pixelnumber[i]) / n1;
                        else
                            powsigma2 += Math.Pow((i - u2), 2) * Convert.ToDouble(pixelnumber[i]) / n2;
                    }

                    sigmaw = q1 * powsigma1 + q2 * powsigma2;
                    if (min >= sigmaw)
                    {
                        min = sigmaw;
                        t = (byte)threshold;
                    }
                }
            }

            unsafe
            {
                byte* p = (byte*)bmpd.Scan0;
                for (int y = 0; y < bmpN.Height; y++, p += offset)
                    for (int x = 0; x < bmpN.Width; x++, p += 3)
                    {
                        if (p[0] > t)
                            p[2] = p[1] = p[0] = 255;
                        else
                            p[2] = p[1] = p[0] = 0;
                    }
            }

            bmpN.UnlockBits(bmpd);

            pictureBox2.Image = bmpN;
        }

        private void neighborsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bmpN = (Bitmap)pictureBox1.Image.Clone();
            int w = bmpN.Width;
            int h = bmpN.Height;
            Rectangle rect = new Rectangle(0, 0, w, h);
            int label = 1;
            int[,] labelarray = new int[w, h];
            BitmapData bmpd = bmpN.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int offset = bmpd.Stride - w * 3;
            int c = 0;
            List<Point> pt = new List<Point>();
            int count = 0;
            unsafe
            {
                byte* p = (byte*)bmpd.Scan0;

                for (int y = 0; y < h; y++, p += offset)
                    for (int x = 0; x < w; x++, p += 3)
                    {
                        if (p[0] != 255)
                        {
                            c = 0;
                            labelarray[x, y] = label;
                            c++;

                            if (x != 0 && labelarray[x - 1, y] != 0)
                            {
                                labelarray[x, y] = labelarray[x - 1, y];
                                c = 0;
                            }
                            else if (y != 0 && labelarray[x, y - 1] != 0)
                            {
                                labelarray[x, y] = labelarray[x, y - 1];
                                c = 0;
                            }
                            label += c;

                            if (x != 0 && y != 0 && labelarray[x - 1, y] != 0 && labelarray[x, y - 1] != 0)
                            {
                                int a = labelarray[x - 1, y];
                                int b = labelarray[x, y - 1];
                                int max = Math.Max(a, b);
                                int min = Math.Min(a, b);
                                int flag = 0;
                                if (a != b)
                                {
                                    for (int i = 0; i < pt.Count; i++)
                                    {
                                        if (pt[i].Y == min)
                                            min = pt[i].X;
                                    }

                                    for (int i = 0; i < pt.Count; i++)
                                    {
                                        if (pt[i].X == min && pt[i].Y == max)
                                            flag = 1;
                                        else if (pt[i].Y == max)
                                        {
                                            max = min;
                                            min = pt[i].X;
                                        }
                                    }
                                    if (flag == 0)
                                    {
                                        Point point = new Point(min, max);
                                        pt.Add(point);
                                    }
                                }
                            }
                        }
                    }


                for (int i = 0; i < pt.Count; i++)
                {
                    for (int y = 0; y < h; y++)
                        for (int x = 0; x < w; x++)
                        {
                            if (labelarray[x, y] == pt[i].Y)
                                labelarray[x, y] = pt[i].X;
                        }
                }

                List<int> intlabel = new List<int>();
                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                    {
                        if (labelarray[x, y] != 0)
                        {
                            bool flag = true;

                            for (int i = 0; i < intlabel.Count; i++)
                                if (labelarray[x, y] == intlabel[i])
                                    flag = false;
                            if (flag)
                            {
                                intlabel.Add(labelarray[x, y]);
                                count++;
                            }
                        }
                    }

                for (int i = 0; i < intlabel.Count; i++)
                {
                    byte r = (byte)new Random(Guid.NewGuid().GetHashCode()).Next(256);
                    byte g = (byte)new Random(Guid.NewGuid().GetHashCode()).Next(256);
                    byte b = (byte)new Random(Guid.NewGuid().GetHashCode()).Next(256);

                    p = (byte*)bmpd.Scan0;
                    for (int y = 0; y < h; y++, p += offset)
                        for (int x = 0; x < w; x++, p += 3)
                        {
                            if (labelarray[x, y] == intlabel[i])
                            {
                                p[0] = b;
                                p[1] = g;
                                p[2] = b;
                            }
                        }
                }
            }
            bmpN.UnlockBits(bmpd);
            pictureBox2.Image = bmpN;
            MessageBox.Show("This image contains " + count + ((count > 1) ? " compoments." : " compoment."));

        }

        private void neighborsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            bmpN = (Bitmap)pictureBox1.Image.Clone();
            int w = bmpN.Width;
            int h = bmpN.Height;
            Rectangle rect = new Rectangle(0, 0, w, h);

            BitmapData bmpd = bmpN.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int offset = bmpd.Stride - w * 3;

            //8鄰域所能表示最多的label數+1
            int[] link = new int[(int)(Math.Ceiling(w * 1.0 / 2) * Math.Ceiling(h * 1.0 / 2)) + 1];

            int currlabel = 1;
            int label;
            int[] a = new int[4];
            int[,] labelarray = new int[w + 2, h + 1];
            int count = 0;

            for (int i = 1; i <= (int)(Math.Ceiling(w * 1.0 / 2) * Math.Ceiling(h * 1.0 / 2)); i++)
                link[i] = i;


            unsafe
            {
                byte* p = (byte*)bmpd.Scan0;

                // first pass

                for (int y = 0; y < h; y++, p += offset)
                    for (int x = 0; x < w; x++, p += 3)
                    {
                        if (p[0] == 0)//黑
                        {
                            a[0] = labelarray[x + 1 - 1, y + 1 - 1];
                            a[1] = labelarray[x + 1, y + 1 - 1];
                            a[2] = labelarray[x + 1 + 1, y + 1 - 1];
                            a[3] = labelarray[x + 1 - 1, y + 1];
                            int index = 88;
                            int min = 550;

                            for (int i = 0; i < 4; i++)
                            {
                                if (a[i] < min && a[i] != 0)
                                {
                                    min = a[i];
                                    index = i;
                                }
                            }

                            //4個值中至少一值不為0
                            if (index != 88)
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    //建等值表
                                    if (a[i] != 0)
                                        link[a[i]] = link[a[index]];
                                }
                            }

                            if (index == 88)
                                label = 0;
                            else
                                label = a[index];

                            if (label != 0)
                                labelarray[x + 1, y + 1] = label;
                            else
                            {
                                labelarray[x + 1, y + 1] = currlabel;
                                currlabel++;
                            }
                        }
                    }

                //second pass
                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                    {
                        labelarray[x + 1, y + 1] = labelreset(link, labelarray[x + 1, y + 1]);
                        /*
                        if (labelarray[x + 1, y + 1] != 0)
                            labelarray[x + 1, y + 1] = link[labelarray[x + 1, y + 1]];
                         */
                    }

                List<int> intlabel = new List<int>();
                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                    {
                        if (labelarray[x + 1, y + 1] != 0)
                        {
                            bool flag = true;

                            for (int i = 0; i < intlabel.Count; i++)
                                if (labelarray[x + 1, y + 1] == intlabel[i])
                                    flag = false;
                            if (flag)
                            {
                                intlabel.Add(labelarray[x + 1, y + 1]);
                                count++;
                            }
                        }
                    }

                for (int i = 0; i < intlabel.Count; i++)
                {
                    byte r = (byte)new Random(Guid.NewGuid().GetHashCode()).Next(256);
                    byte g = (byte)new Random(Guid.NewGuid().GetHashCode()).Next(256);
                    byte b = (byte)new Random(Guid.NewGuid().GetHashCode()).Next(256);

                    p = (byte*)bmpd.Scan0;
                    for (int y = 0; y < h; y++, p += offset)
                        for (int x = 0; x < w; x++, p += 3)
                        {
                            if (labelarray[x + 1, y + 1] == intlabel[i])
                            {
                                p[0] = b;
                                p[1] = g;
                                p[2] = b;
                            }
                        }
                }
            }
            bmpN.UnlockBits(bmpd);
            pictureBox2.Image = bmpN;
            MessageBox.Show("This image contains " + count + ((count > 1) ? " compoments." : " compoment."));

            //List<equaltable> et = new List<equaltable>();    
        }

        private int labelreset(int[] link, int a)
        {
            if (link[a] == a)
                return a;
            else
                return labelreset(link, link[a]);
        }

        private void FilterStripMenuItem1_Click(object sender, EventArgs e)
        {
            bmpN = (Bitmap)pictureBox1.Image.Clone();
            FilterForm ff = new FilterForm(bmpN);
            ff.Show();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bmpN = (Bitmap)pictureBox1.Image.Clone();
            SaveFileDialog sfd = new SaveFileDialog();
            FileStream fs;
            BinaryWriter bw;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                bmpN.Save(sfd.FileName, ImageFormat.Bmp);
                /*
                string filename = sfd.FileName + ".bmp";
                fs = File.Open(filename, FileMode.Create, FileAccess.Write);
                bw = new BinaryWriter(fs);

                fs.Close();
                bw.Close();
                */
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                bmpN = (Bitmap)pictureBox1.Image.Clone();
                int d;
                int mx = MousePosition.X - pictureBox1.Location.X - pictureBox1.Parent.Location.X + 1;
                int my = MousePosition.Y - pictureBox1.Location.Y - pictureBox1.Parent.Location.Y - menuStrip1.Size.Height + 1;

                if (middlem == false)
                    d = 0;
                else
                    d = 25;
                Rectangle rectDest = new Rectangle(mx - d, my - d, 2 * d, 2 * d);
                Rectangle rectSRC = new Rectangle(mx - 3, my - 3, 5, 5);
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                e.Graphics.DrawImage(bmpN, rectDest, rectSRC, GraphicsUnit.Pixel);
                e.Graphics.DrawRectangle(Pens.Black, rectDest);

                if (mx > 2 && my > 2 && mx < bmpN.Width - 3 && my < bmpN.Height - 3)
                {
                    pdf.label1.Text = "";
                    pdf.label2.Text = "";
                    pdf.label3.Text = "";
                    pdf.label4.Text = "";
                    pdf.label5.Text = "";
                    pdf.label6.Text = "";
                    pdf.label7.Text = "";
                    pdf.label8.Text = "";
                    pdf.label9.Text = "";
                    pdf.label10.Text = "";
                    pdf.label11.Text = "";
                    pdf.label12.Text = "";
                    pdf.label13.Text = "";
                    pdf.label14.Text = "";
                    pdf.label15.Text = "";
                    pdf.label16.Text = "";
                    pdf.label17.Text = "";
                    pdf.label18.Text = "";
                    pdf.label19.Text = "";
                    pdf.label20.Text = "";
                    pdf.label21.Text = "";
                    pdf.label22.Text = "";
                    pdf.label23.Text = "";
                    pdf.label24.Text = "";
                    pdf.label25.Text = "";
                    pdf.label1.BackColor = bmpN.GetPixel(mx - 2 - 2, my - 2);
                    pdf.label2.BackColor = bmpN.GetPixel(mx - 1 - 2, my - 2);
                    pdf.label3.BackColor = bmpN.GetPixel(mx - 2, my - 2);
                    pdf.label4.BackColor = bmpN.GetPixel(mx + 1 - 2, my - 2);
                    pdf.label5.BackColor = bmpN.GetPixel(mx + 2 - 2, my - 2);
                    pdf.label6.BackColor = bmpN.GetPixel(mx - 2 - 2, my - 1);
                    pdf.label7.BackColor = bmpN.GetPixel(mx - 1 - 2, my - 1);
                    pdf.label8.BackColor = bmpN.GetPixel(mx - 2, my - 1);
                    pdf.label9.BackColor = bmpN.GetPixel(mx + 1 - 2, my - 1);
                    pdf.label10.BackColor = bmpN.GetPixel(mx + 2 - 2, my - 1);
                    pdf.label11.BackColor = bmpN.GetPixel(mx - 2 - 2, my);
                    pdf.label12.BackColor = bmpN.GetPixel(mx - 1 - 2, my);
                    pdf.label13.BackColor = bmpN.GetPixel(mx - 2, my);
                    pdf.label14.BackColor = bmpN.GetPixel(mx + 1 - 2, my);
                    pdf.label15.BackColor = bmpN.GetPixel(mx + 2 - 2, my);
                    pdf.label16.BackColor = bmpN.GetPixel(mx - 2 - 2, my + 1);
                    pdf.label17.BackColor = bmpN.GetPixel(mx - 1 - 2, my + 1);
                    pdf.label18.BackColor = bmpN.GetPixel(mx - 2, my + 1);
                    pdf.label19.BackColor = bmpN.GetPixel(mx + 1 - 2, my + 1);
                    pdf.label20.BackColor = bmpN.GetPixel(mx + 2 - 2, my + 1);
                    pdf.label21.BackColor = bmpN.GetPixel(mx - 2 - 2, my + 2);
                    pdf.label22.BackColor = bmpN.GetPixel(mx - 1 - 2, my + 2);
                    pdf.label23.BackColor = bmpN.GetPixel(mx - 2, my + 2);
                    pdf.label24.BackColor = bmpN.GetPixel(mx + 1 - 2, my + 2);
                    pdf.label25.BackColor = bmpN.GetPixel(mx + 2 - 2, my + 2);
                }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox3.Image = pictureBox1.Image;
            pictureBox3.Location = pictureBox1.Location;
            statusOI.Text = "";
            statusLocation.Text = "";
            statusC.Text = "";
            statusR.Text = "";
            statusG.Text = "";
            statusB.Text = "";
            split.Text = "";
        }
        #endregion


        #region  Huffman coding
        double meancodelength;
        string filepath;
        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            if (pictureBox3.Image == null)
            {
                MessageBox.Show("Please open an image.");
            }
            else
            {
                SaveFileDialog sfd = new SaveFileDialog();
                encoding = true;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    meancodelength = 0;
                    sfd.FileName += ".hf";
                    filepath = sfd.FileName;
                    backgroundWorker1.RunWorkerAsync(sfd.FileName);
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (encoding)
            {
                bmpN2 = (Bitmap)pictureBox3.Image.Clone();
                string filename = (string)e.Argument;
                int w = bmpN2.Width;
                int h = bmpN2.Height;
                Rectangle r = new Rectangle(0, 0, w, h);
                BitmapData bmpd = bmpN2.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                int offset = bmpd.Stride - w * 3;
                bool isgray = true; //判斷此影像是否為灰階

                FileStream fs = File.Open(filename, FileMode.OpenOrCreate, FileAccess.Write);
                BinaryWriter bw = new BinaryWriter(fs);

                /////這裡之後要修改  寫入檔頭資訊128位元組.....................
                bw.Write(w);
                bw.Write(h);
                byte x0 = 0;
                for (int i = 0; i < 128 - 4 * 2; i++)
                    bw.Write(x0);
                //..............................................................


                data.Clear();

                //判斷此影像是否為灰階
                unsafe
                {
                    byte* p = (byte*)bmpd.Scan0;

                    for (int y = 0; y < h & isgray; y++, p += offset)
                        for (int x = 0; x < w && isgray; x++, p += 3)
                        {
                            if ((p[0] != p[1]) || (p[1] != p[2]) || (p[0] != p[2]))
                                isgray = false;
                        }
                }

                //如果是灰階影像執行一次，彩色影像執行三次
                for (int time = (((isgray) ? 1 : 3) - 1); time >= 0; time--)
                {
                    List<huffmanNode> localdata = new List<huffmanNode>();

                    for (int i = 0; i < 256; i++)
                    {
                        huffmanNode d = new huffmanNode();
                        d.pixel = (byte)i;
                        localdata.Add(d);
                    }

                    //統計影像像素出現次數
                    unsafe
                    {
                        byte* p = (byte*)bmpd.Scan0;

                        for (int y = 0; y < h; y++, p += offset)
                            for (int x = 0; x < w; x++, p += 3)
                            {
                                localdata[p[time]].amount++;
                            }
                    }

                    //將影像依照出現次數排序
                    bubblesort(localdata);

                    //去掉沒有使用過的像素
                    localdata.RemoveAll(amountiszero);


                    //開始建huffman table
                    int l = 0;
                    byte z = 0;

                    while (l < localdata.Count - 1)
                    {
                        string s = "00000001";
                        int index = localdata.Count;

                        //二進位code前7位元不變第八位元設為1
                        //ex: 00000010 | 00000001 = 00000011
                        localdata[l].code = (byte)(localdata[l].code | Convert.ToByte(s, 2));

                        //二進位code前7位元不變第八位元設為0
                        //ex: 00000010 & 11111110 = 00000010
                        s = "11111110";
                        localdata[l + 1].code = (byte)(localdata[l + 1].code & Convert.ToByte(s, 2));

                        //建輔助編碼的節點
                        huffmanNode temp = new huffmanNode();
                        temp.amount = localdata[l].amount + localdata[l + 1].amount;

                        //輔助編碼的節點以code值右邊第2位位元設為1區別
                        s = "00000011";
                        temp.code = (byte)(temp.code | Convert.ToByte(s, 2));

                        //找新的節點依照像素出現次數插入huffman table的位置
                        for (int i = 0; i < localdata.Count; i++)
                        {
                            if (localdata[i].amount >= temp.amount)
                            {
                                index = i;
                                break;
                            }
                        }

                        //節點以parent和child相同連結
                        temp.child = z;
                        localdata[l].parent = z;
                        localdata[l + 1].parent = z;

                        //將新的節點依照像素出現次數插入huffman table
                        localdata.Insert(index, temp);

                        z++;
                        l += 2;
                    }

                    //當節點為原始像素非零的節點時，對此像素進行編碼
                    for (int i = 0; i < localdata.Count; i++)
                    {
                        if (localdata[i].code < 2)
                        {
                            localdata[i].s = encode(localdata, i, localdata[i].s);
                        }
                    }

                    //將編完碼的資訊寫入檔案  

                    //localdata.Count最多511 -> 00000001 11111111(二進制)
                    //把左邊第七個位元拿來存是否為灰階圖
                    //00000010 00000000 -> 512   1 表示灰階  0表示彩色
                    Int16 len = Convert.ToInt16(localdata.Count);

                    if (isgray)
                        len += 512;

                    bw.Write(len);

                    //開始寫入huffman table
                    for (int i = 0; i < localdata.Count; i++)
                    {
                        bw.Write(localdata[i].pixel);
                        bw.Write(localdata[i].parent);
                        bw.Write(localdata[i].child);
                        bw.Write(localdata[i].code);
                    }

                    //開始寫入影像碼
                    unsafe
                    {
                        byte* p = (byte*)bmpd.Scan0;

                        List<byte> b = new List<byte>();

                        string sss = "";

                        //從頭開始比對影像像素值並寫入對應編碼
                        for (int y = 0; y < h; y++, p += offset)
                            for (int x = 0; x < w; x++, p += 3)
                            {
                                //從huffman table找出對應編碼
                                for (int i = 0; i < localdata.Count; i++)
                                {
                                    //若pixel值相等且不為新增設的節點
                                    if (p[time] == localdata[i].pixel && localdata[i].code < 2)
                                    {

                                        sss += localdata[i].s;

                                        //若碼長等於8 -> 1byte
                                        if (sss.Length == 8)
                                        {
                                            //存取後將碼長歸零
                                            b.Add(Convert.ToByte(sss, 2));
                                            sss = "";
                                        }
                                        //若碼長大於8 -> 1byte
                                        if (sss.Length > 8)
                                        {
                                            //先取出前8碼存取成1byte
                                            string ss = sss.Remove(8);
                                            b.Add(Convert.ToByte(ss, 2));

                                            //將前8碼刪除，使碼長小於8
                                            sss = sss.Substring(8);
                                        }
                                    }
                                }
                            }

                        //若最後剩餘的碼長不到8碼，則補0直到長度為8後存取成1byte
                        //ex: sss = "101"  ->   sss = "10100000"
                        if (sss.Length < 8 && sss.Length > 0)
                        {
                            int f = 8 - sss.Length;
                            for (int aaa = 0; aaa < f; aaa++)
                                sss += "0";
                            b.Add(Convert.ToByte(sss, 2));
                        }

                        //存讀取碼長
                        bw.Write(b.Count);

                        //將整理後的像素編碼存入檔案
                        for (int i = 0; i < b.Count; )
                        {
                            bw.Write(b[i]);
                            b.Remove(b[i]);
                        }
                    }

                    //將整理後的資料移到data中
                    for (int i = 0; i < localdata.Count; i++)
                        data.Add(localdata[i]);

                    //有BUG 沒考慮到3色  只處理灰階
                    //算平均碼長
                    for (int i = 0; i < localdata.Count; i++)
                    {
                        if (localdata[i].code < 2)
                        {
                            meancodelength += localdata[i].s.Length * localdata[i].amount * 1.0 / (w * h);
                        }
                    }
                }



                bmpN2.UnlockBits(bmpd);
                fs.Close();
                bw.Close();
                e.Result = isgray;
            }
            else   //decoding
            {
                //pictureBox3.Image = null;
                string filename = (string)e.Argument;
                FileStream fs = new FileStream(filename, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);
                bool isgray;

                //待修改.................................
                int w = br.ReadInt32();
                int h = br.ReadInt32();

                for (int i = 0; i < 128 - 8; i++)
                    br.ReadByte();
                //........................................

                bmpN2 = new Bitmap(w, h);

                //清空data list
                data.Clear();

                //判斷是否為灰階影像 00000010 (右邊第二碼是否為1)
                long pos = br.BaseStream.Position;
                Int16 ax = br.ReadInt16();

                isgray = (ax >= 512) ? true : false;

                br.BaseStream.Position = pos;

                for (int time = (((isgray) ? 1 : 3) - 1); time >= 0; time--)
                {
                    List<huffmanNode> localdata = new List<huffmanNode>();

                    //讀取huffman table陣列的長度
                    Int16 length = br.ReadInt16();

                    //判斷是否為灰階影像
                    if (isgray)
                    {
                        length -= 512;
                    }

                    //讀取huffman table陣列資訊
                    for (int i = 0; i < length; i++)
                    {
                        huffmanNode temp = new huffmanNode();
                        temp.pixel = br.ReadByte();
                        temp.parent = br.ReadByte();
                        temp.child = br.ReadByte();
                        temp.code = br.ReadByte();

                        localdata.Add(temp);
                    }

                    //進行解像素對應的編碼
                    for (int i = 0; i < localdata.Count; i++)
                    {
                        if (localdata[i].code < 2)
                        {
                            localdata[i].s = encode(localdata, i, localdata[i].s);
                        }
                    }

                    //刪除中繼節點
                    for (int i = localdata.Count - 1; i >= 0; i--)
                    {
                        if (localdata[i].code >= 2)
                            data.Remove(localdata[i]);
                    }


                    List<byte> b = new List<byte>();
                    int bcount = br.ReadInt32();

                    //先將剩餘的位元組以二進位方式讀出
                    while (b.Count < bcount)
                    {
                        b.Add(br.ReadByte());
                    }


                    string sx = "";
                    int k = 0;
                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            bool flag = true;
                            sx = "";

                            //一直比對直到此像素(x,y)被比對出來
                            for (; flag; )
                            {
                                //將位元組轉換成字串並取此字串第k個字元加到累積字串上
                                //若不加.PadLeft(8,'0')，00001101會只輸出1101
                                sx += Convert.ToString(b[0], 2).PadLeft(8, '0')[k];
                                for (int j = localdata.Count - 1; j >= 0; j--)
                                {
                                    //若比對成功
                                    if (sx == localdata[j].s)
                                    {
                                        byte pixel = localdata[j].pixel;
                                        //設定灰階像素
                                        switch (time)
                                        {
                                            case 0:
                                                if (isgray)
                                                    bmpN2.SetPixel(x, y, Color.FromArgb(pixel, pixel, pixel));
                                                else
                                                    bmpN2.SetPixel(x, y, Color.FromArgb(bmpN2.GetPixel(x, y).R, bmpN2.GetPixel(x, y).G, pixel));
                                                break;
                                            case 1:
                                                bmpN2.SetPixel(x, y, Color.FromArgb(bmpN2.GetPixel(x, y).R, pixel, bmpN2.GetPixel(x, y).B));
                                                break;
                                            case 2:
                                                bmpN2.SetPixel(x, y, Color.FromArgb(pixel, bmpN2.GetPixel(x, y).G, bmpN2.GetPixel(x, y).B));
                                                break;
                                        }
                                        //跳出迴圈到下一個像素做比對
                                        flag = false;
                                        break;
                                    }
                                    //若比對不成功，累積字串加上一個字元後再回此迴圈做比對
                                }
                                //前進下一個字元
                                k++;
                                //若此字串的字元都已加到累積字串
                                if (k == 8)
                                {
                                    k = 0;
                                    //將位元組b[0]刪除使原b[1]變成b[0]
                                    b.Remove(b[0]);
                                }
                            }
                        }
                    }

                    //將整理後的資料移到data中
                    for (int i = 0; i < localdata.Count; i++)
                        data.Add(localdata[i]);
                }

                br.Close();
                fs.Close();
                e.Result = isgray;
            }
        }


        private void bubblesort(List<huffmanNode> c)
        {
            huffmanNode temp = new huffmanNode();
            for (int i = 256 - 1; i >= 0; i--)
                for (int j = 1; j <= i; j++)
                {
                    if (c[j - 1].amount > c[j].amount)
                    {
                        temp = c[j - 1];
                        c[j - 1] = c[j];
                        c[j] = temp;
                    }
                }
        }

        private bool amountiszero(huffmanNode c)
        {
            if (c.amount == 0)
                return true;
            else
                return false;
        }

        private string encode(List<huffmanNode> c, int index, string s)
        {
            int indexx = 0;
            if (c[index].parent != 255)
            {
                byte b = (byte)(c[index].code & Convert.ToByte("00000001"));
                s = b + s;
                for (int i = index; i < c.Count; i++)
                {
                    if (c[i].child == c[index].parent)
                    {
                        indexx = i;
                        break;
                    }
                }
                return s = encode(c, indexx, s);
            }
            else
            {
                return "" + s;
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }


        //副執行緒完成(編碼/解碼)後的作業
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (encoding)
            {
                int w = bmpN2.Width;
                int h = bmpN2.Height;

                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                dataGridView1.Columns.Add("Num", "Num");
                dataGridView1.Columns.Add("Pixel", "Pixel");
                dataGridView1.Columns.Add("Amount", "Amount");
                dataGridView1.Columns.Add("Pixel", "Probability");
                dataGridView1.Columns.Add("Parent", "Parent");
                dataGridView1.Columns.Add("Child", "Child");
                dataGridView1.Columns.Add("Code", "Code");
                dataGridView1.Columns.Add("Decode", "Decode");

                for (int i = 0; i < dataGridView1.Columns.Count - 1; i++)
                    dataGridView1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[dataGridView1.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                DataGridViewRowCollection rows = dataGridView1.Rows;

                int k = (bool)(e.Result) ? 0 : 1;

                for (int i = 0; i < data.Count; i++)
                {
                    double d;
                    d = data[i].amount * 1.0 / (w * h);
                    rows.Add(new Object[] { i, data[i].pixel, data[i].amount, d.ToString("0.000"), data[i].parent, data[i].child, data[i].code, data[i].s });

                    switch (k)
                    {
                        case 0:
                            dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                            break;
                        case 1:
                            dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.Red;
                            break;
                        case 2:
                            dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.Green;
                            break;
                        case 3:
                            dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.Blue;
                            break;
                    }
                    if (d == 1)
                    {
                        k++;
                    }
                }
                label5.Text += "Average Code Length  " + meancodelength.ToString("0.00") + "  bits / symbol";

                //有BUG 沒考慮到3色  只處理灰階
                //算壓縮比
                FileInfo f1 = new FileInfo(filepath);
                double compression = (double)(pictureBox3.Image.Width * pictureBox3.Image.Height + 128.0) / f1.Length;
                label1.Text = "Compression Ratio : " + compression.ToString("0.00");

                MessageBox.Show("Huffman encoding has been completed.");
            }
            else     //decoding
            {
                pictureBox3.Image = bmpN2;

                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                dataGridView1.Columns.Add("Num", "Num");
                dataGridView1.Columns.Add("Pixel", "Pixel");
                dataGridView1.Columns.Add("Parent", "Parent");
                dataGridView1.Columns.Add("Child", "Child");
                dataGridView1.Columns.Add("Code", "Code");
                dataGridView1.Columns.Add("Decode", "Decode");

                for (int i = 0; i < dataGridView1.Columns.Count - 1; i++)
                    dataGridView1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[dataGridView1.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


                DataGridViewRowCollection rows = dataGridView1.Rows;

                int k = (bool)(e.Result) ? 0 : 1;
                for (int i = 0; i < data.Count; i++)
                {
                    rows.Add(new Object[] { i, data[i].pixel, data[i].parent, data[i].child, data[i].code, data[i].s });
                    switch (k)
                    {
                        case 0:
                            dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                            break;
                        case 1:
                            dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.Red;
                            break;
                        case 2:
                            dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.Green;
                            break;
                        case 3:
                            dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.Blue;
                            break;
                    }
                    if (data[i].parent == 255)
                    {
                        k++;
                    }

                }
                MessageBox.Show("Huffman decoding has been completed.");
            }
        }

        //開始解碼huffman code
        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "huffman coding|*.hf|所有檔案|*.*";
            encoding = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                backgroundWorker1.RunWorkerAsync(ofd.FileName);
            }
        }
        #endregion


        #region fractal
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            DateTime d = DateTime.Now;
            FileStream fs = File.Open((string)e.Argument, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);

            fractal.Clear();
            bmpN2 = (Bitmap)pictureBox4.Image;

            int w = bmpN2.Width;
            int h = bmpN2.Height;
            const int block_R = 4;
            const int block_D = 8;
            double[,] tempDomain = new double[block_R, block_R];
            byte[,] image = new byte[w, h];
            double min;
            double error;
            double sa, saa;
            double sb, sab;

            backgroundWorker2.ReportProgress(10);



            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    image[x, y] = bmpN2.GetPixel(x, y).R;

            //將影像切割成不重疊的range block
            for (int y = 0; y < h; y += block_R)
                for (int x = 0; x < w; x += block_R)
                {
                    FractalDataNode temp = new FractalDataNode();
                    backgroundWorker2.ReportProgress((x + 1) * (y + 1) / (w * h));

                    sb = 0;

                    for (int i = 0; i < block_R; i++)
                        for (int j = 0; j < block_R; j++)
                        {
                            sb += image[x + i, y + j];
                        }

                    min = double.MaxValue;
                    //將影像切割成重疊的domain block
                    for (int dy = 0; dy <= h - block_D; dy++)
                        for (int dx = 0; dx < w - block_D; dx++)
                        {
                            sa = saa = 0;

                            //將domain block縮小成range block的大小
                            for (int j = 0; j < block_R; j++)
                                for (int i = 0; i < block_R; i++)
                                {
                                    tempDomain[i, j] = (image[dx + 2 * i, dy + 2 * j] + image[dx + 2 * i + 1, dy + 2 * j]
                                        + image[dx + 2 * i, dy + 2 * j + 1] + image[dx + 2 * i + 1, dy + 2 * j + 1]) / 4.0;
                                    sa += tempDomain[i, j];
                                    saa += tempDomain[i, j] * tempDomain[i, j];
                                }

                            //計算和哪種幾何轉換誤差最小
                            for (byte k = 0; k < 8; k++)
                            {
                                sab = 0;
                                error = 0;
                                switch (k)
                                {
                                    case 0:
                                        for (int j = 0; j < block_R; j++)
                                            for (int i = 0; i < block_R; i++)
                                            {
                                                error += Math.Pow((tempDomain[i, j] - image[x + i, y + j]), 2);
                                                sab += tempDomain[i, j] * image[x + i, y + j];
                                            }
                                        break;
                                    case 1:
                                        for (int j = 0; j < block_R; j++)
                                            for (int i = 0; i < block_R; i++)
                                            {
                                                error += Math.Pow((tempDomain[j, block_R - 1 - i] - image[x + i, y + j]), 2);
                                                sab += tempDomain[j, block_R - 1 - i] * image[x + i, y + j];
                                            }
                                        break;
                                    case 2:
                                        for (int j = 0; j < block_R; j++)
                                            for (int i = 0; i < block_R; i++)
                                            {
                                                error += Math.Pow((tempDomain[block_R - 1 - j, i] - image[x + i, y + j]), 2);
                                                sab += tempDomain[block_R - 1 - j, i] * image[x + i, y + j];
                                            }
                                        break;
                                    case 3:
                                        for (int j = 0; j < block_R; j++)
                                            for (int i = 0; i < block_R; i++)
                                            {
                                                error += Math.Pow((tempDomain[block_R - 1 - i, block_R - 1 - j] - image[x + i, y + j]), 2);
                                                sab += tempDomain[block_R - 1 - i, block_R - 1 - j] * image[x + i, y + j];
                                            }
                                        break;
                                    case 4:
                                        for (int j = 0; j < block_R; j++)
                                            for (int i = 0; i < block_R; i++)
                                            {
                                                error += Math.Pow((tempDomain[i, block_R - 1 - j] - image[x + i, y + j]), 2);
                                                sab += tempDomain[i, block_R - 1 - j] * image[x + i, y + j];
                                            }
                                        break;
                                    case 5:
                                        for (int j = 0; j < block_R; j++)
                                            for (int i = 0; i < block_R; i++)
                                            {
                                                error += Math.Pow((tempDomain[block_R - 1 - i, j] - image[x + i, y + j]), 2);
                                                sab += tempDomain[block_R - 1 - i, j] * image[x + i, y + j];
                                            }
                                        break;
                                    case 6:
                                        for (int j = 0; j < block_R; j++)
                                            for (int i = 0; i < block_R; i++)
                                            {
                                                error += Math.Pow((tempDomain[j, i] - image[x + i, y + j]), 2);
                                                sab += tempDomain[j, i] * image[x + i, y + j];
                                            }
                                        break;
                                    case 7:
                                        for (int j = 0; j < block_R; j++)
                                            for (int i = 0; i < block_R; i++)
                                            {
                                                error += Math.Pow((tempDomain[block_R - 1 - j, block_R - 1 - i] - image[x + i, y + j]), 2);
                                                sab += tempDomain[block_R - 1 - j, block_R - 1 - i] * image[x + i, y + j];
                                            }
                                        break;
                                }

                                if (error < min)
                                {
                                    float o = 0;
                                    float s = 0.9f;
                                    min = error;
                                    int n = block_R * block_R;

                                    //計算對比和明亮度

                                    s = (float)((n * sab - sa * sb) / (n * saa - sa * sa));
                                    o = (float)((sb - s * sa) / n);

                                    if (Single.IsNaN(s))
                                    {
                                        s = 1;
                                        o = 0;
                                    }


                                    //明亮度(o)依照元區域像素值分割
                                    //黑(像素值為0)表示明亮度為零(o = 0)再疊代多次後會最趨近於0
                                    //白(像素值為255)表示明亮度為255(o = 255)再疊代多次後會最趨近於255

                                    /*
                                    //s不變 調o
                                    double sumrb = 0;
                                    double sumtdb = 0;

                                    for (int i = 0; i < block_R; i++)
                                        for (int j = 0; j < block_R; j++)
                                        {
                                            sumrb += image[x + i, y + j];
                                            sumtdb += tempDomain[i, j];
                                        }
                                    sumrb /= n;
                                    sumtdb /= n;
                                    if (Math.Abs(sumrb - sumtdb) <= 5) 
                                        s = 0.8f;
                                    else if (Math.Abs(sumrb - sumtdb) <= 10)
                                        s = 0.6f;
                                    else if (Math.Abs(sumrb - sumtdb) <= 15)
                                        s = 0.4f;
                                    else if (Math.Abs(sumrb - sumtdb) <= 20)
                                        s = 0.2f;
                                    else if (Math.Abs(sumrb - sumtdb) <= 25)
                                        s = 0f;

                                    o = (float)(sumrb - s * sumtdb);
                                    */
                                    //紀錄誤差最小的資訊(x,y,k,s,o)
                                    temp.x = (Int16)dx;
                                    temp.y = (Int16)dy;
                                    temp.type = k;
                                    temp.s = s;
                                    temp.o = o;

                                }
                            }

                        }

                    fractal.Add(temp);
                }


            //完成後寫入檔案
            for (int i = 0; i < fractal.Count; i++)
            {
                bw.Write(fractal[i].x);
                bw.Write(fractal[i].y);
                bw.Write(fractal[i].type);

                if (Single.IsNaN(fractal[i].s))
                {
                    fractal[i].s = 1;
                    fractal[i].o = 0;
                }
                bw.Write(fractal[i].s);
                bw.Write(fractal[i].o);
            }

            fs.Close();
            bw.Close();
            TimeSpan pasttime = DateTime.Now - d;
            e.Result = pasttime;

        }

        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar2.Value = e.ProgressPercentage;
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label2.Text = ((TimeSpan)e.Result) + "";
            MessageBox.Show("Fractal Compression has been completed.");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox4.Image != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    backgroundWorker2.RunWorkerAsync(sfd.FileName);
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Bitmap b = new Bitmap(ofd.FileName);
                pictureBox4.Image = b;
            }
        }

        //進行疊代
        OpenFileDialog fractalofd = new OpenFileDialog();
        bool first = true;
        private void button3_Click(object sender, EventArgs e)
        {
            fractal.Clear();

            if (fractalofd.FileName != null)
            {
                FileStream fs = File.OpenRead(fractalofd.FileName);
                //FileStream fs = File.OpenRead(@"D:\ImageProcessing\picture\fractal\4x512lena.f");
                BinaryReader br = new BinaryReader(fs);
                int w = 512;
                int h = 512;
                Bitmap bmp = (Bitmap)pictureBox4.Image.Clone();
                byte[,] image = new byte[w, h];
                byte[,] nimage = new byte[w, h];
                int block_R = 8;
                byte[,] tempDomain = new byte[block_R, block_R];

                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                    {
                        image[x, y] = bmp.GetPixel(x, y).R;
                    }



                DataGridViewRowCollection rows = dataGridView2.Rows;
                int ii = 0;
                if (first)
                    rows.Clear();

                for (int y = 0; y < h; y += block_R)
                    for (int x = 0; x < w; x += block_R)
                    {
                        FractalDataNode temp = new FractalDataNode();
                        temp.x = br.ReadInt16();
                        temp.y = br.ReadInt16();
                        temp.type = br.ReadByte();
                        temp.s = br.ReadSingle();
                        temp.o = br.ReadSingle();

                        if (first)
                        {
                            rows.Add(new object[] { ii, temp.x, temp.y, temp.type, Math.Round(temp.s, 2), Math.Round(temp.o, 2) });

                            if (temp.s > 1.2)
                                dataGridView2.Rows[ii].DefaultCellStyle.ForeColor = Color.Red;

                            ii++;
                        }

                        int dx = temp.x;
                        int dy = temp.y;
                        byte[,] tDomain = new byte[block_R, block_R];

                        for (int j = 0; j < block_R; j++)
                            for (int i = 0; i < block_R; i++)
                            {
                                tempDomain[i, j] = (byte)((image[dx + 2 * i, dy + 2 * j] + image[dx + 2 * i + 1, dy + 2 * j]
                                    + image[dx + 2 * i, dy + 2 * j + 1] + image[dx + 2 * i + 1, dy + 2 * j + 1]) / 4.0);
                            }

                        for (int j = 0; j < block_R; j++)
                            for (int i = 0; i < block_R; i++)
                            {
                                tDomain[i, j] = tempDomain[i, j];
                            }

                        switch (temp.type)
                        {
                            case 0:
                                for (int j = 0; j < block_R; j++)
                                    for (int i = 0; i < block_R; i++)
                                    {
                                        tempDomain[i, j] = tDomain[i, j];
                                    }
                                break;
                            case 1:
                                for (int j = 0; j < block_R; j++)
                                    for (int i = 0; i < block_R; i++)
                                    {
                                        tempDomain[i, j] = tDomain[j, block_R - 1 - i];
                                    }
                                break;
                            case 2:
                                for (int j = 0; j < block_R; j++)
                                    for (int i = 0; i < block_R; i++)
                                    {
                                        tempDomain[i, j] = tDomain[block_R - 1 - j, i];
                                    }
                                break;
                            case 3:
                                for (int j = 0; j < block_R; j++)
                                    for (int i = 0; i < block_R; i++)
                                    {
                                        tempDomain[i, j] = tDomain[block_R - 1 - i, block_R - 1 - j];
                                    }
                                break;
                            case 4:
                                for (int j = 0; j < block_R; j++)
                                    for (int i = 0; i < block_R; i++)
                                    {
                                        tempDomain[i, j] = tDomain[i, block_R - 1 - j];
                                    }
                                break;
                            case 5:
                                for (int j = 0; j < block_R; j++)
                                    for (int i = 0; i < block_R; i++)
                                    {
                                        tempDomain[i, j] = tDomain[block_R - 1 - i, j];
                                    }
                                break;
                            case 6:
                                for (int j = 0; j < block_R; j++)
                                    for (int i = 0; i < block_R; i++)
                                    {
                                        tempDomain[i, j] = tDomain[j, i];
                                    }
                                break;
                            case 7:
                                for (int j = 0; j < block_R; j++)
                                    for (int i = 0; i < block_R; i++)
                                    {
                                        tempDomain[i, j] = tDomain[block_R - 1 - j, block_R - 1 - i];
                                    }
                                break;
                        }

                        for (int j = 0; j < block_R; j++)
                            for (int i = 0; i < block_R; i++)
                            {
                                float t = tempDomain[i, j] * temp.s + temp.o;
                                if (t > 255)
                                    t = 255;
                                else if (t < 0)
                                    t = 0;
                                tempDomain[i, j] = (byte)t;
                            }

                        for (int j = 0; j < block_R; j++)
                            for (int i = 0; i < block_R; i++)
                            {
                                nimage[x + i, y + j] = tempDomain[i, j];
                            }
                    }

                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                    {
                        byte pixel = nimage[x, y];
                        bmp.SetPixel(x, y, Color.FromArgb(pixel, pixel, pixel));
                    }

                pictureBox4.Image = bmp;
                first = false;
            }
        }

        private void pixelGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pdf.label1.Text = "";
            pdf.label2.Text = "";
            pdf.label3.Text = "";
            pdf.label4.Text = "";
            pdf.label5.Text = "";
            pdf.label6.Text = "";
            pdf.label7.Text = "";
            pdf.label8.Text = "";
            pdf.label9.Text = "";
            pdf.label10.Text = "";
            pdf.label11.Text = "";
            pdf.label12.Text = "";
            pdf.label13.Text = "";
            pdf.label14.Text = "";
            pdf.label15.Text = "";
            pdf.label16.Text = "";
            pdf.label17.Text = "";
            pdf.label18.Text = "";
            pdf.label19.Text = "";
            pdf.label20.Text = "";
            pdf.label21.Text = "";
            pdf.label22.Text = "";
            pdf.label23.Text = "";
            pdf.label24.Text = "";
            pdf.label25.Text = "";
            middlem = !middlem;
            pictureBox1.Invalidate();
            if (middlem)
            {
                pdf.Show();
            }
            else
            {
                pdf.Hide();
            }
        }
        bool dgvclick = false;
        private void dataGridView2_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvclick = true;
            x1 = (int)dataGridView2.Rows[e.RowIndex].Cells[0].Value * 8 % 512;
            y1 = ((int)dataGridView2.Rows[e.RowIndex].Cells[0].Value / 64) * 8;

            x2 = Convert.ToInt32(dataGridView2.Rows[e.RowIndex].Cells[1].Value);
            y2 = Convert.ToInt32(dataGridView2.Rows[e.RowIndex].Cells[2].Value);
            pictureBox4.Refresh();
        }

        private void pictureBox4_Paint(object sender, PaintEventArgs e)
        {
            if (dgvclick)
            {
                e.Graphics.DrawRectangle(Pens.Red, new Rectangle(x1 - 1, y1 - 1, 8 + 1, 8 + 1));
                e.Graphics.DrawRectangle(Pens.Blue, new Rectangle(x2 - 1, y2 - 1, 16 + 1, 16 + 1));
                this.pictureBox4.Refresh();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FractalAnalysisForm faform = new FractalAnalysisForm();
            faform.Show();
        }
        #endregion


        #region   其他功能


        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
                    int[] a = new int[200];
                    a[75] = 10;
                    MessageBox.Show("" + Array.IndexOf(a,10));
            
            byte i = 128;
            byte n = (byte)(i ^ (i >> 1));

            MessageBox.Show("" + i + " BCD " + Convert.ToString(i, 2).PadLeft(8) +" Gray "+ Convert.ToString(n, 2).PadLeft(8));
            */
            Bitmap b = new Bitmap(256, 256);

            for (int y = 0; y < b.Height; y++)
                for (int x = 0; x < b.Width; x++)
                {
                    Color c = Color.FromArgb(x, x, x);
                    b.SetPixel(x, y, c);
                }
            for (int y = 0; y < b.Height; y++)
                for (int x = 0; x < b.Width; x++)
                {
                    Color c = Color.Black;
                    if (y - x <= 0)
                        b.SetPixel(x, y, c);
                }
            pictureBox1.Image = b;
        }

        private void movingBallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MovingBall mb = new MovingBall();
            mb.ShowDialog();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void hitogramSpecificationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            grayLevelToolStripMenuItem.PerformClick();
            pixelDataCaculate(pictureBox2.Image as Bitmap);
            histMatchForm hs = new histMatchForm((Bitmap)pictureBox2.Image, pixelnumber);
            hs.Show();
        }



        private void closeImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            pictureBox2.Image = null;
            ImageProcessingToolStripMenuItem.Enabled = false;
        }

        private void brightnessAdjustToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bmpN = (Bitmap)pictureBox1.Image.Clone();
            ManualSettingForm msf = new ManualSettingForm(bmpN, this, 1);
            msf.Show();

        }

        private void imageMoveToRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                originImage = (Bitmap)pictureBox1.Image.Clone();
                bmpN = (Bitmap)pictureBox1.Image.Clone();
                pictureBox2.Image = originImage;
            }
        }

        private void borderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.BorderStyle == BorderStyle.None)
            {
                pictureBox1.BorderStyle = BorderStyle.FixedSingle;
                pictureBox2.BorderStyle = BorderStyle.FixedSingle;
            }
            else
            {
                pictureBox1.BorderStyle = BorderStyle.None;
                pictureBox2.BorderStyle = BorderStyle.None;
            }

        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "BMP檔|*.bmp|所有檔案|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                bmpN2 = new Bitmap(ofd.FileName);
                pictureBox3.Image = bmpN2;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (fractalofd.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = File.OpenRead(fractalofd.FileName);
            }

        }

        private void jpegCompressionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                grayLevelToolStripMenuItem.PerformClick();
                pictureMoveToolStripMenuItem.PerformClick();
                pictureBox2.Image = null;
                JpegForm jf = new JpegForm(this, (Bitmap)pictureBox1.Image);
                jf.Show();
            }
        }

        private void rGBToYCrCbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                YCrCbForm yf = new YCrCbForm(this, (Bitmap)pictureBox1.Image);
                yf.Show();
            }
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                Bitmap bmp = (Bitmap)pictureBox1.Image.Clone();
                SaveFileDialog sfd = new SaveFileDialog();

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    FileStream fs = File.Open(sfd.FileName, FileMode.Create);
                    BinaryWriter bw = new BinaryWriter(fs);

                    /////這裡之後要修改  寫入檔頭資訊128位元組.....................
                    bw.Write(bmp.Width);
                    bw.Write(bmp.Height);
                    byte x0 = 0;
                    for (int i = 0; i < 128 - 4 * 2; i++)
                        bw.Write(x0);
                    //..............................................................

                    for (int y = 0; y < bmp.Height; y++)
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            bw.Write(bmp.GetPixel(x, y).R);
                        }
                    bw.Close();
                    fs.Close();
                }
            }
        }

        private void openBmp256ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = File.Open(ofd.FileName, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);

                int w = br.ReadInt32();
                int h = br.ReadInt32();
                Bitmap b = new Bitmap(w, h);

                for (int i = 0; i < 128 - 4 * 2; i++)
                    br.ReadByte();
                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                    {
                        byte p = br.ReadByte();
                        b.SetPixel(x, y, Color.FromArgb(p, p, p));
                    }

                pictureBox1.Image = b;
                br.Close();
                fs.Close();
            }
        }

        private void highFrequentImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap b = new Bitmap(256, 256);
            byte p = 255;
            for (int y = 0; y < 256; y++)
            {
                for (int x = 0; x < 256; x++)
                {
                    b.SetPixel(x, y, Color.FromArgb(p, p, p));
                    if (p == 0)
                        p = 255;
                    else if (p == 255)
                        p = 0;
                }
                if (p == 0)
                    p = 255;
                else if (p == 255)
                    p = 0;
            }
            pictureBox1.Image = b;
        }

        private void x8BlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap b = (Bitmap)pictureBox1.Image.Clone();
            Bitmap a = new Bitmap(b.Width + b.Width / 8 + 1, b.Height + b.Height / 8 + 1);

            for (int y = 0; y < a.Height; y++)
                for (int x = 0; x < a.Width; x++)
                {
                    a.SetPixel(x, y, Color.Blue);
                }

            for (int y = 0; y < b.Height; y += 8)
                for (int x = 0; x < b.Width; x += 8)
                {
                    for (int j = 0; j < 8; j++)
                        for (int i = 0; i < 8; i++)
                            a.SetPixel(1 + x + i + x / 8, 1 + y + j + y / 8, b.GetPixel(x + i, y + j));
                }
            pictureBox2.Image = a;
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            int w, h;
            bool[,] mark;

            scalingValue = 300 / 100f;

            w = Convert.ToInt32(originImage.Width * scalingValue);
            h = Convert.ToInt32(originImage.Height * scalingValue);
            bmpN = new Bitmap(w, h);

            mark = new bool[w, h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    mark[x, y] = false;

            for (int y = 0; y < originImage.Height; y++)
                for (int x = 0; x < originImage.Width; x++)
                {
                    Color c = originImage.GetPixel(x, y);
                    bmpN.SetPixel(Convert.ToInt32(x * scalingValue), Convert.ToInt32(y * scalingValue), c);
                    mark[Convert.ToInt32(x * scalingValue), Convert.ToInt32(y * scalingValue)] = true;
                }

            for (int y = 0; y < h; y++)
            {
                bool head = false;
                for (int x = 0; x < w; x++)
                {
                    if (mark[x, y] == false)
                    {
                        if (y == 0)
                            bmpN.SetPixel(x, y, Color.FromArgb(bmpN.GetPixel(x - 1, y).R, bmpN.GetPixel(x - 1, y).G, bmpN.GetPixel(x - 1, y).B));
                        else if (x == 0)
                        {
                            bmpN.SetPixel(x, y, Color.FromArgb(bmpN.GetPixel(x, y - 1).R, bmpN.GetPixel(x, y - 1).G, bmpN.GetPixel(x, y - 1).B));
                            head = true;
                        }
                        else
                        {
                            if ((head == true) && (bmpN.GetPixel(x - 1, y) != bmpN.GetPixel(x, y - 1)))
                                bmpN.SetPixel(x, y, Color.FromArgb(bmpN.GetPixel(x, y - 1).R, bmpN.GetPixel(x, y - 1).G, bmpN.GetPixel(x, y - 1).B));
                            else
                                bmpN.SetPixel(x, y, Color.FromArgb(bmpN.GetPixel(x - 1, y).R, bmpN.GetPixel(x - 1, y).G, bmpN.GetPixel(x - 1, y).B));
                        }
                    }
                }
            }
            pictureBox2.Image = bmpN;

        }

        private void highToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap b = new Bitmap(256, 256);
            byte p = 0;
            for (int y = 0; y < 256; y++)
            {
                for (int x = 0; x < 256; x++)
                {
                    if (x + y < 256)
                        p = Convert.ToByte((x + y));

                    b.SetPixel(x, y, Color.FromArgb(p, p, p));
                }
            }

            for (int y = 0; y < 256; y++)
            {
                for (int x = 0; x < 256; x++)
                {
                    if (x + y >= 256)
                    {
                        p = b.GetPixel(255 - x, 255 - y).R;
                        b.SetPixel(x, y, Color.FromArgb(p, p, p));
                    }
                }
            }

            pictureBox1.Image = b;
        }

        private void tEstToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Bitmap b = new Bitmap(8, 8);
            for (int y = 0; y < 8; y++)
                for (int x = 0; x < 8; x++)
                {
                    b.SetPixel(x, y, Color.FromArgb(0, 255, 255, 255));
                    if(x==0||x==7||y==0||y==7)
                    {
                        b.SetPixel(x, y, Color.FromArgb(255, Color.Blue));
                    }
                }
            b.Save(@"D:\ImageProcessing\picture\cursor3",ImageFormat.Png);
            //string s = "123456";
            /*
            char[] c = s.ToCharArray();
            Array.Reverse(c);
            s = new string (c);*/
            //MessageBox.Show(s.Substring(1, 3));
        }

        private void lowFrequentImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap b = new Bitmap(256, 256);
            byte p = 200;
            for (int y = 0; y < 256; y++)
            {
                for (int x = 0; x < 256; x++)
                {
                    b.SetPixel(x, y, Color.FromArgb(p, p, p));
                }
            }
            pictureBox1.Image = b;
        }

        private void frequencyFilterDCTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                this.grayLevelToolStripMenuItem.PerformClick();
                this.pictureMoveToolStripMenuItem.PerformClick();
                pictureBox2.Image = null;
                DCTFilterForm df = new DCTFilterForm(pictureBox1.Image as Bitmap, this);
                df.Show();
            }
        }

        private void separateClareToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        #endregion


        #region  Video

        int frame = 0;              //總共張數
        int runningframe = 1;       //現在撥到第幾張
        float velocity = 26;        //一秒撥幾張
        delegate void SetImageCallback(Bitmap bmp);     //不同執行緒委派
        delegate void SetTrackBarCallback(int value);   //不同執行緒委派
        DateTime d;
        bool playorpause = true;
        bool reverseorpause = true;
        int compressionType = 1;

        private void pictureBox5_SizeChanged(object sender, EventArgs e)
        {
            trackBar1.Size = new Size(pictureBox5.Image.Width, 64);
        }

        //Play / Pause
        private void button5_Click(object sender, EventArgs e)
        {
            if (folderToolStripMenuItem.Enabled == true)
            {
                if (playorpause == true)
                {
                    playorpause = false;
                    button5.Image = Properties.Resources.pause;
                    backgroundWorker3.RunWorkerAsync(0);
                }
                else
                {
                    playorpause = true;
                    button5.Image = Properties.Resources.play;
                    backgroundWorker3.CancelAsync();
                }
                trackBar1.Value = runningframe;
            }
            else
                MessageBox.Show("Please open file first.", "Error");
        }

        //File - Open
        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = e.Argument;
            if ((int)e.Argument == 0)
            {
                for (; runningframe <= frame; runningframe++)
                {
                    PeriodWork();

                    while ((DateTime.Now - d) < TimeSpan.FromMilliseconds(1000 / velocity))
                    {
                        if (backgroundWorker3.CancellationPending)
                            return;
                    }

                }

            }
            else
            {
                for (; runningframe > 0; runningframe--)
                {
                    PeriodWork();

                    while ((DateTime.Now - d) < TimeSpan.FromMilliseconds(1000 / velocity))
                    {
                        if (backgroundWorker3.CancellationPending)
                            return;
                    }

                }
            }
        }

        //每次Frame變換要做的事情
        private void PeriodWork()
        {
            //subsampling
            if (compressionType < 0)
            {
                //with interpolation
                if (compressionType == -1)
                {
                    Bitmap bmp;
                    bmp = new Bitmap(getName(runningframe));
                    d = DateTime.Now;
                    setPictureBox5(bmp);
                    setTrackBar(runningframe);

                    if (runningframe > 1)
                    {
                        //實際frame
                        if ((runningframe - 1) % coarsenum == 0)
                        {
                            bmp = new Bitmap(getName(runningframe));
                        }
                        else//內插frame
                        {
                            int n = (runningframe - 1) / coarsenum * coarsenum + 1;
                            bmp = new Bitmap(bmp.Width, bmp.Height);
                            Bitmap lastb = new Bitmap(getName(n));
                            Bitmap nextb;
                            if(n + coarsenum<=frame)
                                nextb = new Bitmap(getName(n + coarsenum));
                            else
                                nextb = new Bitmap(getName(n));

                            int w = bmp.Width;
                            int h = bmp.Height;
                            BitmapData interbd = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                            BitmapData lastbd = lastb.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                            BitmapData nextbd = nextb.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                            int offset = interbd.Stride - w * 3;

                            unsafe
                            {
                                byte* p = (byte*)interbd.Scan0;
                                byte* pl = (byte*)lastbd.Scan0;
                                byte* pn = (byte*)nextbd.Scan0;

                                for (int y = 0; y < h; y++, p += offset, pl += offset, pn += offset)
                                    for (int x = 0; x < w; x++, p += 3, pl += 3, pn += 3)
                                    {
                                        p[0] = Convert.ToByte(((n + coarsenum - runningframe) * 1.0 / coarsenum) * pl[0] + ((runningframe - n) * 1.0 / coarsenum) * pn[0]);
                                        p[1] = Convert.ToByte(((n + coarsenum - runningframe) * 1.0 / coarsenum) * pl[1] + ((runningframe - n) * 1.0 / coarsenum) * pn[1]);
                                        p[2] = Convert.ToByte(((n + coarsenum - runningframe) * 1.0 / coarsenum) * pl[2] + ((runningframe - n) * 1.0 / coarsenum) * pn[2]);
                                    }
                            }
                            bmp.UnlockBits(interbd);
                            lastb.UnlockBits(lastbd);
                            nextb.UnlockBits(nextbd);
                        }

                        setPictureBox6(bmp);

                    }
                }
                //without interpolation
                else if (compressionType == -2)
                {
                    Bitmap bmp;
                    bmp = new Bitmap(getName(runningframe));
                    d = DateTime.Now;
                    setPictureBox5(bmp);
                    setTrackBar(runningframe);

                    if (runningframe > 1)
                    {
                        //實際frame
                        if ((runningframe - 1) % coarsenum == 0)
                        {
                            bmp = new Bitmap(getName(runningframe));
                        }
                        else//上一frame
                        {
                            int n = runningframe / coarsenum * coarsenum - 1;
                            bmp = new Bitmap(getName(n));
                        }

                        setPictureBox6(bmp);
                    }
                }
            }
            else
            {
                Bitmap bmp;
                //bmp = new Bitmap(filename[runningframe]);     //活
                //寫死
                bmp = new Bitmap(getName(runningframe));
                d = DateTime.Now;
                setPictureBox5(bmp);
                setTrackBar(runningframe);
                if (runningframe > 1)
                {
                    //顯示MV檔解碼出來的圖片
                    setPictureBox6(MvtoImage());

                    //顯示Motion Vector Bitmap
                    bmp = new Bitmap(getMvBmpName(runningframe));
                    setPictureBox7(bmp);

                    //畫PSNR
                    PSNRTable();
                }
            }
        }
        
        double maxpsnr = 0;
        Point lastpt;
        //畫PSNR
        private void PSNRTable()
        {
            Bitmap b = (Bitmap)pictureBox5.Image;
            Bitmap b2 = (Bitmap)pictureBox6.Image;
            int w = b.Width, h = b.Height;
            Rectangle r = new Rectangle(0,0,b.Width,b.Height);
            BitmapData bd = b.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData bd2 = b2.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            int offset = bd.Stride - 3 * b.Width;

            double denominator = 0;
            unsafe
            {
                byte* p = (byte*)bd.Scan0;
                byte* p2 = (byte*)bd2.Scan0;

                for (int y = 0; y < h; y++, p += offset, p2 += offset)
                    for (int x = 0; x < w; x++, p += 3, p2 += 3)
                    {
                        denominator += Math.Pow(p[0] - p2[0], 2);
                        denominator += Math.Pow(p[1] - p2[1], 2);
                        denominator += Math.Pow(p[2] - p2[2], 2);
                    }
                denominator /= (w * h * 3);

            }
            b.UnlockBits(bd);
            b2.UnlockBits(bd2);

            int psnrvalue = Convert.ToInt32( 10 * Math.Log(255 * 255 / denominator, 10));

            if (psnrvalue > maxpsnr)
                maxpsnr = psnrvalue;

            Graphics g = Graphics.FromImage(psnr);
            //畫psnr座標軸
            Pen pen;
            switch(type1ToolStripMenuItem.Text)
            {
                case "type1": pen = Pens.Red; break;
                case "type2": pen = Pens.Green; break;
                case "type3": pen = Pens.Blue; break;
                case "type4": pen = Pens.Cyan; break;
                default: pen = Pens.Red; break;
            }

            g.DrawEllipse(pen, new Rectangle(20 + runningframe * 290 / frame - 1, 220 - psnrvalue * 2 - 1, 3, 3));

            if (runningframe > 2)
                g.DrawLine(pen, lastpt, new Point(20 + runningframe * 290 / frame, 220 - psnrvalue * 2));
            //g.DrawLine(Pens.Black, new Point(20 + runningframe * 290 / frame, 220), new Point(20 + runningframe * 290 / frame, 224));

            lastpt = new Point(20 + runningframe * 290 / frame, 220 - psnrvalue * 2);
            setPictureBox8(psnr);
        }

        Bitmap PreviousBmp;

        //Decode MV to Image
        private Bitmap MvtoImage()
        {
            FileStream fs = File.OpenRead(getMvFileName(runningframe));
            BinaryReader br = new BinaryReader(fs);

            Bitmap bmp2 = new Bitmap(PreviousBmp.Width,PreviousBmp.Height);

            Rectangle r;
            int x, y;

            for (int j = 0; j < PreviousBmp.Height; j += 8)
                for (int i = 0; i < PreviousBmp.Width; i += 8)
                {
                    r = new Rectangle(i, j, 8, 8);
                    x = br.ReadInt16();

                    if (x == -1)
                    {
                        BitmapData bd = bmp2.LockBits(r, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                        unsafe
                        {
                            byte* p = (byte*)bd.Scan0;

                            for (int bj = 0; bj < 8; bj++)
                                for (int bi = 0; bi < 8; bi++, p += 3)
                                {
                                    p[2] = br.ReadByte();
                                    p[1] = br.ReadByte();
                                    p[0] = br.ReadByte();
                                }
                        }
                        bmp2.UnlockBits(bd);
                    }
                    else
                    {

                        y = br.ReadInt16();
                        Rectangle r2 = new Rectangle(x, y, 8, 8);

                        BitmapData bd = PreviousBmp.LockBits(r2, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                        BitmapData bd2 = bmp2.LockBits(r, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                        unsafe
                        {
                            byte* p = (byte*)bd.Scan0;
                            byte* p2 = (byte*)bd2.Scan0;

                            for (int bj = 0; bj < 8; bj++)
                                for (int bi = 0; bi < 8; bi++, p += 3, p2 += 3)
                                {
                                    p2[0] = p[0];
                                    p2[1] = p[1];
                                    p2[2] = p[2];
                                }
                        }
                        PreviousBmp.UnlockBits(bd);
                        bmp2.UnlockBits(bd2);
                    }
                }

            fs.Close();
            br.Close();
            if (compressionType != 1) 
                PreviousBmp = bmp2;
            return bmp2;
        }

        //執行緒3完成要做的事
        private void backgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((int)e.Result == 0)
            {
                button5.Image = Properties.Resources.play;
                playorpause = true;
                if (runningframe > frame)
                {
                    runningframe = 1;
                    setTrackBar(runningframe);
                    //Bitmap b = new Bitmap(filename[0]);   //活
                    //路靜寫死
                    Bitmap b = new Bitmap(getName(1));
                    setPictureBox5(b);
                    PreviousBmp = new Bitmap(getName(1));
                    setPictureBox6(b);
                    pictureBox7.Image = null;
                }
            }
            else
            {
                reverseorpause = true;
                Bitmap b = Properties.Resources.play;
                b.RotateFlip(RotateFlipType.RotateNoneFlipX);
                button9.Image = b;
                if (runningframe < 1)
                {
                    runningframe = 1;
                    PreviousBmp = new Bitmap(getName(1));
                    setPictureBox6(PreviousBmp);
                    pictureBox7.Image = null;
                }
            }
        }

        private void setPictureBox5(Bitmap bmp)
        {
            if (this.pictureBox5.InvokeRequired)
            {
                SetImageCallback i = new SetImageCallback(setPictureBox5);
                this.Invoke(i, new object[] { bmp });
            }
            else
            {
                pictureBox5.Image = (Bitmap)bmp.Clone();
            }
        }

        private void setPictureBox6(Bitmap bmp)
        {
            if (this.pictureBox6.InvokeRequired)
            {
                SetImageCallback i = new SetImageCallback(setPictureBox6);
                this.Invoke(i, new object[] { bmp });
            }
            else
            {
                pictureBox6.Image = (Bitmap)bmp.Clone();
            }
        }

        private void setPictureBox7(Bitmap bmp)
        {
            if (this.pictureBox7.InvokeRequired)
            {
                SetImageCallback i = new SetImageCallback(setPictureBox7);
                this.Invoke(i, new object[] { bmp });
            }
            else
            {
                pictureBox7.Image = (Bitmap)bmp.Clone();
            }
        }

        private void setPictureBox8(Bitmap bmp)
        {
            if (this.pictureBox8.InvokeRequired)
            {
                SetImageCallback i = new SetImageCallback(setPictureBox8);
                this.Invoke(i, new object[] { bmp });
            }
            else
            {
                pictureBox8.Image = (Bitmap)bmp.Clone();
            }
        }

        private void setTrackBar(int runningframe)
        {
            if (this.trackBar1.InvokeRequired)
            {
                SetTrackBarCallback b = new SetTrackBarCallback(setTrackBar);
                this.Invoke(b, new object[] { runningframe });
            }
            else
                trackBar1.Value = runningframe;
        }

        //Step
        private void button11_Click(object sender, EventArgs e)
        {
            if (folderToolStripMenuItem.Enabled == true)
            {
                runningframe++;

                if (runningframe > frame)
                    runningframe = 1;

                Bitmap b = new Bitmap(getName(runningframe));
                pictureBox5.Image = b;

                if (runningframe == 1)
                {
                    PreviousBmp = new Bitmap(getName(1));
                    pictureBox6.Image = new Bitmap(getName(1));
                    pictureBox7.Image = null;
                }
                else
                {
                    pictureBox6.Image = MvtoImage();
                    b = new Bitmap(getMvBmpName(runningframe));
                    pictureBox7.Image = b;

                    //畫PSNR
                    PSNRTable();
                }
                trackBar1.Value = runningframe;


            }
            else
                MessageBox.Show("Please open file first.", "Error");
        }

        //Stop
        private void button6_Click(object sender, EventArgs e)
        {
            backgroundWorker3.CancelAsync();
            runningframe = 1;
            trackBar1.Value = runningframe;
            pictureBox5.Image = new Bitmap(getName(1));
            PreviousBmp = new Bitmap(getName(1));
            pictureBox6.Image = new Bitmap(getName(1));
            pictureBox7.Image = null;
        }

        //Fast
        private void button7_Click(object sender, EventArgs e)
        {
            if (velocity < frame - 5)
                velocity += 5;
            label3.Text = velocity + "  frame / s";
        }

        //Slow
        private void button8_Click(object sender, EventArgs e)
        {
            if (velocity > 5)
                velocity -= 5;
            label3.Text = +velocity + "  frame / s";
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            label4.Text = "Frame : " + trackBar1.Value + " / " + frame;
        }

        //Reverse / Pause
        private void button9_Click(object sender, EventArgs e)
        {
            if (runningframe != 1)
            {
                if (reverseorpause == true)
                {
                    reverseorpause = false;
                    button9.Image = Properties.Resources.pause;
                    backgroundWorker3.RunWorkerAsync(1);
                }
                else
                {
                    reverseorpause = true;
                    Bitmap b = Properties.Resources.play;
                    b.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    button9.Image = b;
                    backgroundWorker3.CancelAsync();
                }

                trackBar1.Value = runningframe;
            }
        }

        //待刪除  直接開啟壓縮視窗
        private void tabPage4_Enter_1(object sender, EventArgs e)
        {
            //analysisToolStripMenuItem.PerformClick();
            //compressionToolStripMenuItem.PerformClick();
            //this.Hide();
            //velocity = 1;
        }

        #endregion



        private void openCompressedDataToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //string folderPath = @"C:\Users\iis\Desktop\IMAGE";
        string folderPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "IMAGE");
        string folderName;

        //選擇資料夾(讀檔寫死)
        private void openFlodToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                folderToolStripMenuItem.Enabled = true;
                folderPath = fbd.SelectedPath;
            }
        }

        private void cLaireGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderName = cLaireGToolStripMenuItem.Text;
            label10.Text = "Frame Width : " + new Bitmap(getName(1)).Width + "  Frame Height : " + new Bitmap(getName(1)).Height;
            frame = 28;
            setInitialFrame();
        }

        private void fOOTBAGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderName = fOOTBAGToolStripMenuItem.Text;
            label10.Text = "Frame Width : " + new Bitmap(getName(1)).Width + "  Frame Height : " + new Bitmap(getName(1)).Height;
            frame = 28;
            setInitialFrame();
        }

        private void tENNISToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderName = tENNISToolStripMenuItem.Text;
            label10.Text = "Frame Width : " + new Bitmap(getName(1)).Width + "  Frame Height : " + new Bitmap(getName(1)).Height;
            frame = 66;
            setInitialFrame();
        }

        private void gARDENToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderName = gARDENToolStripMenuItem.Text;
            label10.Text = "Frame Width : " + new Bitmap(getName(1)).Width + "  Frame Height : " + new Bitmap(getName(1)).Height;
            frame = 144;
            setInitialFrame();
        }

        Bitmap psnr;

        private void setInitialFrame()
        {
            pictureBox5.Image = new Bitmap(getName(1));
            pictureBox6.Image = new Bitmap(getName(1));
            PreviousBmp = new Bitmap(getName(1));
            this.trackBar1.Maximum = frame;
            this.trackBar1.Minimum = 1;
            label4.Text = "Frame : " + trackBar1.Value + " / " + frame;

            
            psnr = new Bitmap(352, 240);
            Graphics g = Graphics.FromImage(psnr);
            //畫psnr座標軸
            g.DrawLine(Pens.Black, new Point(20, 20), new Point(20, 220));
            g.DrawLine(Pens.Black, new Point(20, 220), new Point(320, 220));
            //刻度
            for (int i = 220; i >= 20; i -= 20)
            {
                g.DrawLine(Pens.Black, new Point(16, i), new Point(20, i));
                //g.DrawString((110 - i / 2).ToString(), new Font("New Timer", 8), Brushes.Black, new Point(0, i));
            }

            pictureBox8.Image = psnr;
            g.Dispose();
        }

        private void tabPage4_Paint(object sender, PaintEventArgs e)
        {
            //刻度
            for (int i = 220; i >= 20; i -= 20)
            {
                int offset = 0;
                Graphics g = e.Graphics;
                if (110 - i / 2 == 100)
                    offset = 10;
                else if (110 - i / 2 == 0)
                    offset = 0;
                else
                    offset = 5;
                g.DrawString((110 - i / 2).ToString(), new Font("New Timer", 7), Brushes.Black, new PointF(pictureBox8.Location.X - offset, pictureBox8.Location.Y + i - 5));
                
            }
        }

        //取得第幾張影像的完整路徑
        private string getName(int n)
        {
            return folderPath + "\\" + folderName + "\\" + folderName + "(" + n + ").bmp";
        }

        //取得第幾張Motion Vector File的完整路徑
        private string getMvFileName(int n)
        {
            switch (compressionType)
            {
                case 1:
                    return folderPath + "\\" + folderName + "\\" + type1ToolStripMenuItem.Text + "\\" + type1ToolStripMenuItem.Text + "-1-" + n + ".mv";
                case 2:
                    return folderPath + "\\" + folderName + "\\" + type1ToolStripMenuItem.Text + "\\" + type1ToolStripMenuItem.Text + "-" + (n - 1) + "-" + n + ".mv";
                case 3:
                    return folderPath + "\\" + folderName + "\\" + type1ToolStripMenuItem.Text + "\\" + type1ToolStripMenuItem.Text + "-" + (n - 1) + "-" + n + ".mv";
                case 4:
                    return folderPath + "\\" + folderName + "\\" + type1ToolStripMenuItem.Text + "\\" + type1ToolStripMenuItem.Text + "-" + (n - 1) + "-" + n + ".mv";
                default:
                    break;
            }
            return "";
        }

        //取得第幾張Motion Vector影像的完整路徑
        private string getMvBmpName(int n)
        {
            switch (compressionType)
            {
                case 1:
                    return folderPath + "\\" + folderName + "\\" + type1ToolStripMenuItem.Text + "\\" + type1ToolStripMenuItem.Text + "-mv1-" + n + ".bmp";
                case 2:
                    return folderPath + "\\" + folderName + "\\" + type1ToolStripMenuItem.Text + "\\" + type1ToolStripMenuItem.Text + "-mv" + (n - 1) + "-" + n + ".bmp";
                case 3:
                    return folderPath + "\\" + folderName + "\\" + type1ToolStripMenuItem.Text + "\\" + type1ToolStripMenuItem.Text + "-mv" + (n - 1) + "-" + n + ".bmp";
                case 4:
                    return folderPath + "\\" + folderName + "\\" + type1ToolStripMenuItem.Text + "\\" + type1ToolStripMenuItem.Text + "-mv" + (n - 1) + "-" + n + ".bmp";
                default :
                    break;
            }
            return "";
        }

        private void analysisToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            VideoAnalysisForm vaf = new VideoAnalysisForm();
            vaf.Show();
        }

        private void type1ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            type1ToolStripMenuItem.Text = "type1";
            compressionType = 1;
        }

        private void type2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            type1ToolStripMenuItem.Text = "type2";
            compressionType = 2;
        }

        private void type3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            type1ToolStripMenuItem.Text = "type3";
            compressionType = 3;
        }

        private void type4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            type1ToolStripMenuItem.Text = "type4";
            compressionType = 4;
        }

        //Close
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox5.Image = null;
            pictureBox6.Image = null;
            pictureBox7.Image = null;
        }

        //Compression
        private void compressionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CompressForm cf = new CompressForm();
            cf.Show();
        }
        public int coarsenum = 0;
        private void coarseQuantizationOfVectorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(pictureBox1.Image!=null)
            {
                Bitmap b = new Bitmap(pictureBox1.Image.Width, pictureBox1.Image.Height);
                AskForm af = new AskForm(this,0);

                af.ShowDialog();

                for (int y = 0; y < b.Height; y++)
                    for (int x = 0; x < b.Width; x++)
                    {
                        int n = 256 / Convert.ToInt32(Math.Pow(2, coarsenum));
                        byte r = (byte)(((Bitmap)pictureBox1.Image).GetPixel(x, y).R / n * n);
                        byte g = (byte)(((Bitmap)pictureBox1.Image).GetPixel(x, y).G / n * n);
                        byte blue = (byte)(((Bitmap)pictureBox1.Image).GetPixel(x, y).B / n * n);
                        b.SetPixel(x, y, Color.FromArgb(r, g, blue));
                    }

                pictureBox1.Image = b;
            }
        }

        private void withInterpolationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            type1ToolStripMenuItem.Text = "Sub-sampling1";
            compressionType = -1;
            AskForm af = new AskForm(this, 1);
            af.ShowDialog();
        }

        private void withoutInterpolationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            type1ToolStripMenuItem.Text = "Sub-sampling2";
            compressionType = -2;
            AskForm af = new AskForm(this,1);
            af.ShowDialog();
        }

        private void contextMenuStrip1_Click(object sender, EventArgs e)
        {
            PictureBox p = (PictureBox)(sender as ContextMenuStrip).SourceControl;
            if (p != null) 
            {
                SaveFileDialog sfd = new SaveFileDialog();
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    p.Image.Save(sfd.FileName,ImageFormat.Bmp);
                }
            }
        }

    }
}
