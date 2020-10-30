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

namespace ImageProcessing
{
    public partial class VideoAnalysisForm : Form
    {
        string folderPath;
        string folderName;
        string mvName;
        string mvbmp;
        int numcf, numrf;
        int mvdisplay = 1;
        Point[] mvxy;

        Pen arrow = new Pen(Color.Blue, 1);

        bool selectTarget = false;      //判定是否要在current frame畫框
        bool selectMatching = false;    //判定是否要在reference frame畫框

        Color c = Color.Blue;   //小方框的顏色

        Bitmap cf;
        Bitmap cfwithblock;
        Bitmap rf;
        Bitmap rfwithblock;
        Bitmap df;
        Bitmap dfwithblock;

        Bitmap targetblck = new Bitmap(8, 8);
        Bitmap candidateblock = new Bitmap(8, 8);

        int cfblockx = 0; //current frame小方框x座標
        int cfblocky = 0; //current frame小方框y座標
        int rfblockx = 0; //reference frame小方框x座標
        int rfblocky = 0; //reference frame小方框y座標
        bool rforcf = true;     //true:rf ,false":cf
        int compressionType = 1;

        public VideoAnalysisForm()
        {
            InitializeComponent();
            KeyMoveStatusLabel.Text = "";
            arrow.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;


            //方便
            string filename = @"C:\Users\adm\Desktop\IMAGE\TENNIS\type1\type1-1-10.mv";
            folderPath = Path.GetDirectoryName(filename);
            mvName = Path.GetFileName(filename);

            string[] s = folderPath.Split('\\');
            folderName = s[s.Length - 2];
            string[] s2 = mvName.Split('-', '.');
            mvbmp = folderPath + "\\" + s2[0] + "-mv" + s2[1] + "-" + s2[2] + ".bmp";
            folderPath = folderPath.Remove(folderPath.Length - s[s.Length - 2].Length - s[s.Length - 1].Length - 2);

            decodemv(filename);
            referenceFrameToolStripMenuItem1.PerformClick();
            //可刪
        }


        private void openMotionVectorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "MotionVector(*.mv)|*.mv";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                folderPath = Path.GetDirectoryName(ofd.FileName);
                mvName = Path.GetFileName(ofd.FileName);

                string[] s = folderPath.Split('\\');
                folderName = s[s.Length - 2];
                string[] s2 = mvName.Split('-', '.');

                switch (s2[0])
                {
                    case "type1":
                        compressionType = 1;
                        break;
                    case "type2":
                        compressionType = 2;
                        break;
                    case "type3":
                        compressionType = 3;
                        break;
                    case "type4":
                        compressionType = 4;
                        break;
                    default:
                        break;
                }

                mvbmp = folderPath + "\\" + s2[0] + "-mv" + s2[1] + "-" + s2[2] + ".bmp";
                folderPath = folderPath.Remove(folderPath.Length - s[s.Length - 2].Length - s[s.Length - 1].Length - 2);

                decodemv(ofd.FileName);
            }
        }

        //取得第幾張影像的完整路徑
        private string getName(int n)
        {
            return folderPath + "\\" + folderName + "\\" + folderName + "(" + n + ").bmp";
        }

        private string getName(string n)
        {
            return folderPath + "\\" + folderName + "\\" + folderName + "(" + n + ").bmp";
        }

        //顯示各PictureBox Image並對.mv檔進行解碼
        private void decodemv(string mvname)
        {
            //Ex: mvName = type1-1-3.mv 
            //s2[0]=type1 s2[1]=1 s2[2]=3 s2[3]=mv
            string[] s2 = mvname.Split('.', '-');
            numrf = Convert.ToInt32(s2[s2.Length - 3]);
            numcf = Convert.ToInt32(s2[s2.Length - 2]);

            showReferenceFrame();
            pictureBox2.Image = new Bitmap(getName(numcf));

            rf = (Bitmap)pictureBox1.Image.Clone();
            cf = (Bitmap)pictureBox2.Image.Clone();

            //解mv檔
            Bitmap b1 = (Bitmap)pictureBox1.Image.Clone();
            Bitmap b2 = (Bitmap)pictureBox2.Image.Clone();
            FileStream fs = File.OpenRead(mvname);
            BinaryReader br = new BinaryReader(fs);

            mvxy = new Point[b1.Width * b1.Height / 8 / 8];
            int px, py;
            int i = 0;

            for (int y = 0; y < b1.Height; y += 8)
                for (int x = 0; x < b1.Width; x += 8)
                {
                    px = br.ReadInt16();
                    if (px == -1)
                    {
                        mvxy[i] = new Point(-1, -1);
                        i++;
                        Rectangle r = new Rectangle(x, y, 8, 8);
                        BitmapData bd = b2.LockBits(r, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
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
                        b2.UnlockBits(bd);
                    }
                    else
                    {
                        py = br.ReadInt16();
                        mvxy[i] = new Point(px, py);
                        i++;

                        for (int bj = 0; bj < 8; bj++)
                            for (int bi = 0; bi < 8; bi++)
                            {
                                b2.SetPixel(bi + x, bj + y, b1.GetPixel(px + bi, py + bj));
                            }
                    }
                }
            df = b2;
            pictureBox3.Image = b2;
            fs.Close();
            br.Close();

            pictureBox4.Image = new Bitmap(mvbmp);
        }
        
        private void showReferenceFrame()
        {
            if (compressionType <= 2)
            {
                pictureBox1.Image = new Bitmap(getName(numrf));
            }
            else
            {
                if (numrf >= 2)
                {
                    FileStream fs;
                    BinaryReader br;
                    Bitmap b = new Bitmap(getName(1));
                    Bitmap b2 = new Bitmap(getName(1));
                    int w = b.Width, h = b.Height;
                    for (int n = 1; n < numrf; n++)
                    {
                        fs = File.OpenRead(folderPath + "\\" + folderName + "\\type" + compressionType + "\\type" + compressionType + "-" + n + "-" + (n + 1) + ".mv");
                        br = new BinaryReader(fs);

                        Rectangle r;
                        int x, y;

                        for (int j = 0; j < h; j += 8)
                            for (int i = 0; i < w; i += 8)
                            {
                                r = new Rectangle(i, j, 8, 8);
                                x = br.ReadInt16();
                                
                                if (x == -1)
                                {
                                    BitmapData bd2 = b2.LockBits(r, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                                    unsafe
                                    {
                                        byte* p = (byte*)bd2.Scan0;

                                        for (int bj = 0; bj < 8; bj++)
                                            for (int bi = 0; bi < 8; bi++, p += 3)
                                            {
                                                p[2] = br.ReadByte();
                                                p[1] = br.ReadByte();
                                                p[0] = br.ReadByte();
                                            }
                                    }
                                    b2.UnlockBits(bd2);
                                }
                                else
                                {
                                    y = br.ReadInt16();

                                    Rectangle r2 = new Rectangle(x, y, 8, 8);
                                    
                                    BitmapData bd = b.LockBits(r2, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                                    BitmapData bd2 = b2.LockBits(r, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
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
                                    b.UnlockBits(bd);
                                    b2.UnlockBits(bd2);
                                    
                                }
                            }

                        b = (Bitmap)b2.Clone();
                        fs.Close();
                        br.Close();
                    }
                    pictureBox1.Image = b2;
                }
                else
                {
                    pictureBox1.Image = new Bitmap(getName(numrf));
                }

            }
        }
        
        private void aLLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mvdisplay = 1;
            showMV();
        }

        private void noMoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mvdisplay = 2;
            showMV();
        }

        private void movingPartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mvdisplay = 3;
            showMV();
        }

        private void singleMotionVectorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mvdisplay = 4;
            showMV();
        }

        private void showMV()
        {
            if (pictureBox1.Image != null)
            {
                int xy = 0;
                Bitmap b = new Bitmap(pictureBox1.Image.Width, pictureBox1.Image.Height);

                for (int y = 0; y < b.Height; y++)
                    for (int x = 0; x < b.Width; x++)
                    {
                        b.SetPixel(x, y, Color.White);
                    }

                //顯示Motion Vector的型式
                switch (mvdisplay)
                {
                    //show all
                    case 1:
                        b = new Bitmap(mvbmp);
                        break;
                    //show no move
                    case 2:
                        for (int y = 0; y < b.Height; y += 8)
                            for (int x = 0; x < b.Width; x += 8)
                            {
                                if (mvxy[xy].X == x && mvxy[xy].Y == y)
                                {
                                    for (int bj = 0; bj < 8; bj++)
                                        for (int bi = 0; bi < 8; bi++)
                                        {
                                            b.SetPixel(bi + x, bj + y, Color.Blue);
                                        }
                                }
                                else if (mvxy[xy].X == -1)
                                {
                                    for (int bj = 0; bj < 8; bj++)
                                        for (int bi = 0; bi < 8; bi++)
                                        {
                                            b.SetPixel(bi + x, bj + y, Color.Green);
                                        }
                                }
                                xy++;
                            }
                        break;
                    //show move
                    case 3:
                        //draw motion vectors
                        Graphics g = Graphics.FromImage(b);
                        for (int y = 0; y < b.Height; y += 8)
                            for (int x = 0; x < b.Width; x += 8)
                            {
                                if ((mvxy[xy].X != x || mvxy[xy].Y != y) && mvxy[xy].X != -1) 
                                {
                                    for (int bj = 0; bj < 8; bj++)
                                        for (int bi = 0; bi < 8; bi++)
                                        {
                                            b.SetPixel(bi + x, bj + y, Color.Red);
                                        }

                                    //從block在reference frame 的位置 畫箭頭到 current frame的位置
                                    g.DrawLine(arrow, mvxy[xy].X + 4, mvxy[xy].Y + 4, x + 4, y + 4);

                                }
                                xy++;
                            }
                        g.Dispose();
                        break;
                    case 4:
                        break;
                }
                pictureBox4.Image = b;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image != null)
            {
                pictureBox2.Image = (Bitmap)cf.Clone();
                pictureBox3.Image = (Bitmap)df.Clone();
                if (blockToolStripMenuItem.Text == "Matching Block")
                    pictureBox1.Image = (Bitmap)rf.Clone();
                selectTarget = true;
                selectMatching = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null && blockToolStripMenuItem.Text == "Candidate Block")
            {
                pictureBox1.Image = (Bitmap)rf.Clone();
                selectMatching = true;
                selectTarget = false;
            }
        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            
            Coordinate.Text = "( X , Y ) = ( " + e.X + " , " + e.Y + " )";
            if (selectTarget == true)
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
                        else
                            b.SetPixel(i, j, this.c);
                    }
                  
                Cursor c = new Cursor(b.GetHicon());
                this.Cursor = c;
            }

            else
                this.Cursor = Cursors.Default;
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                c = cd.Color;
                colorToolStripMenuItem.ForeColor = c;
            }
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            if (Cursor != Cursors.Default)
                Cursor = Cursors.Default;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            if (Cursor != Cursors.Default)
                Cursor = Cursors.Default;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Coordinate.Text = "( X , Y ) = ( " + e.X + " , " + e.Y + " )";
            if (selectMatching == true)
            {
                Bitmap b = new Bitmap(Properties.Resources.cursor3);
                
                Color co = Color.FromArgb(0, 0, 0, 0);
                for (int j = 0; j < b.Height; j++)
                    for (int i = 0; i < b.Width; i++)
                    {
                        if (b.GetPixel(i, j).A != 0)
                        {
                            b.SetPixel(i, j, this.c);
                        }
                    }
                
                Cursor c = new Cursor(b.GetHicon());
                this.Cursor = c;
            }

            else
                this.Cursor = Cursors.Default;
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (selectTarget)
            {
                cfwithblock = (Bitmap)pictureBox2.Image.Clone();
                dfwithblock = (Bitmap)pictureBox3.Image.Clone();

                //存至Target block
                targetblck = cfwithblock.Clone(new Rectangle(e.X / 8 * 8, e.Y / 8 * 8, 8, 8), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                pictureBox6.Image = enlargeblock(targetblck, 20);
                evaluateError();

                Graphics g = pictureBox2.CreateGraphics();
                Pen p = new Pen(c);

                cfblockx = e.X / 8 * 8;
                cfblocky = e.Y / 8 * 8;

                g.DrawRectangle(p, new Rectangle(e.X / 8 * 8, e.Y / 8 * 8, 8, 8));

                g = pictureBox3.CreateGraphics();

                cfblockx = e.X / 8 * 8;
                cfblocky = e.Y / 8 * 8;

                g.DrawRectangle(p, new Rectangle(e.X / 8 * 8, e.Y / 8 * 8, 8, 8));

                //直接顯示Matching Block
                if (blockToolStripMenuItem.Text == "Matching Block")
                {
                    //找到Matching Block的位置畫方框
                    g = pictureBox1.CreateGraphics();
                    int xy = 0;
                    for (int y = 0; y < cf.Height; y += 8)
                        for (int x = 0; x < cf.Width; x += 8)
                        {
                            if (x == e.X / 8 * 8 && y == e.Y / 8 * 8)
                            {
                                rfblockx = mvxy[xy].X;
                                rfblocky = mvxy[xy].Y;

                                if (rfblockx != -1)
                                {
                                    g.DrawRectangle(p, new Rectangle(mvxy[xy].X, mvxy[xy].Y, 8, 8));

                                    rfwithblock = (Bitmap)pictureBox1.Image.Clone();
                                    candidateblock = rfwithblock.Clone(new Rectangle(mvxy[xy].X, mvxy[xy].Y, 8, 8), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                                    pictureBox5.Image = enlargeblock(candidateblock, 20);
                                    evaluateError();
                                }
                                else 
                                {
                                    candidateblock = (Bitmap)targetblck.Clone();
                                    pictureBox5.Image = enlargeblock(candidateblock, 20);
                                    evaluateError();
                                }
                                //顯示單一Motion Vector
                                if (mvdisplay == 4) 
                                {
                                    Bitmap b = new Bitmap(rf.Width, rf.Height);

                                    //draw motion vectors
                                    g = Graphics.FromImage(b);
                                    if (e.X / 8 * 8 == mvxy[xy].X && e.Y / 8 * 8 == mvxy[xy].Y)
                                    {
                                        for (int bj = 0; bj < 8; bj++)
                                            for (int bi = 0; bi < 8; bi++)
                                            {
                                                b.SetPixel(bi + x, bj + y, Color.Blue);
                                            }
                                    }
                                    else if (mvxy[xy].X == -1)
                                    {
                                        for (int bj = 0; bj < 8; bj++)
                                            for (int bi = 0; bi < 8; bi++)
                                            {
                                                b.SetPixel(bi + x, bj + y, Color.Green);
                                            }
                                    }
                                    else
                                    {
                                        //從block在reference frame 的位置 畫箭頭到 current frame的位置
                                        g.DrawLine(arrow, mvxy[xy].X + 4, mvxy[xy].Y + 4, x + 4, y + 4);
                                    }
                                    pictureBox4.Image = b;
                                }
                            }
                            xy++;
                        }
                }
                g.Dispose();

                selectTarget = false;
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (selectMatching)
            {
                rfwithblock = (Bitmap)pictureBox1.Image.Clone();

                //存至Candidate block
                if (e.X - 4 >= 0 && e.Y - 4 >= 0 && e.X + 8 < rf.Width && e.Y + 8 < rf.Height)
                {
                    rfblockx = e.X - 4;
                    rfblocky = e.Y - 4;

                    candidateblock = rfwithblock.Clone(new Rectangle(e.X - 4, e.Y - 4, 8, 8), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    pictureBox5.Image = enlargeblock(candidateblock, 10);
                    evaluateError();

                    Graphics g = pictureBox1.CreateGraphics();
                    Pen p = new Pen(c);
                    g.DrawRectangle(p, new Rectangle(e.X - 4, e.Y - 4, 8, 8));
                    selectMatching = false;
                    g.Dispose();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = (Bitmap)rf.Clone();
            pictureBox2.Image = (Bitmap)cf.Clone();
            pictureBox3.Image = (Bitmap)df.Clone();
        }

        private Bitmap enlargeblock(Bitmap b,int num)
        {
            Bitmap sbmp = new Bitmap(b.Width * num, b.Height * num);

            for (int j = 0; j < b.Height; j++)
                for (int i = 0; i < b.Width; i++)
                {
                    for (int sj = 0; sj < num; sj++)
                        for (int si = 0; si < num; si++)
                        {
                            sbmp.SetPixel(i * num + si, j * num + sj, b.GetPixel(i, j));
                        }
                }
            return sbmp;
        }

        //計算absolute difference
        private void evaluateError()
        {
            if (pictureBox5.Image != null && pictureBox6.Image != null)
            {
                double ae = 0;
                double se = 0;
                for (int y = 0; y < targetblck.Height; y++)
                    for (int x = 0; x < targetblck.Width; x++) 
                    {
                        Color tc = targetblck.GetPixel(x, y);
                        Color cc = candidateblock.GetPixel(x, y);
                        ae += Math.Abs(tc.R - cc.R);
                        ae += Math.Abs(tc.G - cc.G);
                        ae += Math.Abs(tc.B - cc.B);
                        se += Math.Pow(tc.R - cc.R, 2);
                        se += Math.Pow(tc.G - cc.G, 2);
                        se += Math.Pow(tc.B - cc.B, 2);
                    }
                label1.Text = "Absolute Difference : " + ae.ToString("F");
                label8.Text = "Square Difference : " + se.ToString("F");
            }
        }

        private void blockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(blockToolStripMenuItem.Text =="Matching Block")
            {
                label2.Text = "Candidate Block:";
                blockToolStripMenuItem.Text ="Candidate Block";
            }
            else
            {
                label2.Text = "Matching Block:";
                blockToolStripMenuItem.Text = "Matching Block";
            }
        }

        private void VideoAnalysisForm_KeyDown(object sender, KeyEventArgs e)
        {          
            if (KeyPreview==true&&rforcf == true)
            {
                switch (e.KeyCode)
                {
                    case Keys.B:
                        if (rfblockx - 1 >= 0)
                            rfblockx --;
                        break;

                    case Keys.M:
                        if (rfblockx + 1 < rf.Width - 8) 
                            rfblockx ++;
                        break;

                    case Keys.H:
                        if (rfblocky - 1 >= 0)
                            rfblocky --;
                        break;

                    case Keys.N:
                        if (rfblocky + 1 < rf.Height - 8) 
                            rfblocky ++;
                        break;

                    default:
                        break;
                }

                candidateblock = rfwithblock.Clone(new Rectangle(rfblockx, rfblocky, 8, 8), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                pictureBox5.Image = enlargeblock(candidateblock, 20);
                evaluateError();

                Graphics g = Graphics.FromImage(rfwithblock);
                Pen p = new Pen(c);
                g.DrawRectangle(p, new Rectangle(rfblockx, rfblocky, 8, 8));
                pictureBox1.Image = rfwithblock;
                g.Dispose();

                rfwithblock = (Bitmap)rf.Clone();

            }
            else if(KeyPreview == true && rforcf == false)
            {
                switch (e.KeyCode)
                {
                    case Keys.B:
                        if (cfblockx - 8 >= 0)
                            cfblockx -= 8;
                        break;

                    case Keys.M:
                        if (cfblockx + 8 < cf.Width)
                            cfblockx += 8;
                        break;

                    case Keys.H:
                        if (cfblocky - 8 >= 0)
                            cfblocky -= 8;
                        break;

                    case Keys.N:
                        if (cfblocky + 8 < cf.Height)
                            cfblocky += 8;
                        break;

                    default:
                        break;
                }

                cfwithblock = (Bitmap)cf.Clone();

                //存至Target block
                targetblck = cfwithblock.Clone(new Rectangle(cfblockx, cfblocky, 8, 8), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                pictureBox6.Image = enlargeblock(targetblck, 20);
                evaluateError();

                Graphics g = Graphics.FromImage(cfwithblock);
                Pen p = new Pen(c);
                g.DrawRectangle(p, new Rectangle(cfblockx, cfblocky, 8, 8));
                pictureBox2.Image = cfwithblock;

                //直接顯示Matching Block
                if (blockToolStripMenuItem.Text == "Matching Block")
                {
                    rfwithblock = (Bitmap)rf.Clone();

                    //找到Matching Block的位置畫方框
                    g = Graphics.FromImage(rfwithblock);
                    int xy = 0;
                    for (int y = 0; y < cf.Height; y += 8)
                        for (int x = 0; x < cf.Width; x += 8)
                        {
                            if (x == cfblockx && y == cfblocky)
                            {
                                rfblockx = mvxy[xy].X;
                                rfblocky = mvxy[xy].Y;

                                if (rfblockx != -1)
                                {
                                    candidateblock = rfwithblock.Clone(new Rectangle(mvxy[xy].X, mvxy[xy].Y, 8, 8), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                                    pictureBox5.Image = enlargeblock(candidateblock, 20);
                                    evaluateError();

                                    g.DrawRectangle(p, new Rectangle(mvxy[xy].X, mvxy[xy].Y, 8, 8));
                                    pictureBox1.Image = rfwithblock;
                                }
                                else
                                {
                                    candidateblock = (Bitmap)targetblck.Clone();
                                    pictureBox5.Image = enlargeblock(candidateblock, 20);
                                    evaluateError();
                                }

                                //顯示單一Motion Vector
                                if (mvdisplay == 4)
                                {
                                    Bitmap b = new Bitmap(rf.Width, rf.Height);

                                    //draw motion vectors
                                    g = Graphics.FromImage(b);
                                    if (cfblockx == mvxy[xy].X && cfblocky == mvxy[xy].Y)
                                    {
                                        for (int bj = 0; bj < 8; bj++)
                                            for (int bi = 0; bi < 8; bi++)
                                            {
                                                b.SetPixel(bi + x, bj + y, Color.Blue);
                                            }
                                    }
                                    else if (mvxy[xy].X == -1)
                                    {
                                        for (int bj = 0; bj < 8; bj++)
                                            for (int bi = 0; bi < 8; bi++)
                                            {
                                                b.SetPixel(bi + x, bj + y, Color.Green);
                                            }
                                    }
                                    else
                                    {
                                        //從block在reference frame 的位置 畫箭頭到 current frame的位置
                                        g.DrawLine(arrow, mvxy[xy].X + 4, mvxy[xy].Y + 4, x + 4, y + 4);
                                    }
                                    pictureBox4.Image = b;

                                }
                            }
                            xy++;
                        }
                }

                g.Dispose();
            }
            
        }

        private void disableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.KeyPreview = false;
            KeyMoveStatusLabel.Text = "";
        }

        private void referenceFrameToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            KeyMoveStatusLabel.Text = "Reference Frame Key Move";
            this.KeyPreview = true;
            rforcf = true;
        }

        private void currentFrameToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            KeyMoveStatusLabel.Text = "Current Frame Key Move";
            this.KeyPreview = true;
            rforcf = false;
        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PictureBox p = (PictureBox)(sender as ContextMenuStrip).SourceControl;
            if (p != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    p.Image.Save(sfd.FileName, ImageFormat.Bmp);
                }
            }
        }

 
    }
}