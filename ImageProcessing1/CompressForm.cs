using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Threading;
using System.IO;

namespace ImageProcessing
{
    public partial class CompressForm : Form
    {
        Bitmap mv;              //Motion vector
        Bitmap cf;              //Current frame
        Bitmap rf;              //reference frame
        Rectangle rect;         //影像大小
        Bitmap bmp, bmp2;       //加上方框的影像
        int w, h;               //frame寬高
        double error;
        Pen pen = new Pen(Color.Blue, 1);
        byte[, ,] cfmatrix;
        byte[, ,] rfmatrix;
        Color c = Color.Magenta;
        Thread th1;
        int searchMethod = 1;
        int matchingCriteria = 1;
        BinaryWriter bw;
        BinaryReader br;
        FileStream fs;
        public int thresholdforPDC = 0;
        string curFile;
        string img_path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "IMAGE");

        delegate void SetImageCallback(Bitmap bmp);     //不同執行緒委派
        delegate void SetButtonCallback(string s);      //不同執行緒委派
        delegate void SetLabelCallback(string s);      //不同執行緒委派
        delegate void SetTextBoxCallback(int n);      //不同執行緒委派

        public CompressForm()
        {
            InitializeComponent();
            comboBox1.Text = comboBox1.Items[0].ToString();
            comboBox2.Text = comboBox2.Items[0].ToString();

            //方便 可刪
            //pictureBox1.Image = new Bitmap(@"C:\Users\iis\Desktop\IMAGE\TEST\TENNIS(1).bmp");
            //pictureBox2.Image = new Bitmap(@"C:\Users\iis\Desktop\IMAGE\TEST\TENNIS(2).bmp");
            //string sss = img_path + @"\TENNIS(1).bmp";

            pictureBox1.Image = new Bitmap(img_path + @"\TEST\TENNIS(1).bmp");
            pictureBox2.Image = new Bitmap(img_path + @"\TEST\TENNIS(2).bmp");


            rf = (Bitmap)pictureBox1.Image.Clone();
            cf = (Bitmap)pictureBox2.Image.Clone();
        }

        private void referenceFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if(ofd.ShowDialog()==DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(ofd.FileName);
                rf = (Bitmap)pictureBox1.Image.Clone();
            }
        }

        private void currentFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.Image = new Bitmap(ofd.FileName);
                cf = (Bitmap)pictureBox2.Image.Clone();
            }
        }

        //Start Block Matching
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Start")
            {
                //curFile = @"C:\Users\iis\Desktop\IMAGE\TEST\test.mv";
                curFile = img_path + @"\TEST\test.mv";
                int n = 1;
                while (File.Exists(curFile))
                {
                    //curFile = @"C:\Users\iis\Desktop\IMAGE\TEST\test" + "(" + n + ").mv";
                    curFile = img_path + @"\TEST\test" + "(" + n + ").mv";
                    n++;
                }
                fs = File.Create(curFile);
                bw = new BinaryWriter(fs);

                pictureBox4.Image = null;
                pictureBox4.Refresh();

                th1 = new Thread(block_matching);
                th1.Start();
                button1.Text = "Abort";
            }
            else
            {
                if (button2.Text == "Resume")
                    th1.Resume();
                th1.Abort();
                button1.Text = "Start";
                pictureBox1.Image = (Bitmap)rf.Clone();
                pictureBox1.Refresh();
                pictureBox2.Image = (Bitmap)cf.Clone();
                pictureBox1.Refresh();
                pictureBox2.Refresh();
                
            }
        }
        Bitmap decodedbmp;
        private void block_matching()
        {
            if (pictureBox1.Image != null && pictureBox2.Image != null)
            {
                DateTime dt = DateTime.Now;

                //預處理
                bmp = (Bitmap)rf.Clone();
                bmp2 = (Bitmap)cf.Clone();
                w = rf.Width;
                h = rf.Height;
                decodedbmp = new Bitmap(w, h);
                rect = new Rectangle(0, 0, w, h);
                
                //放大四倍
                mv = new Bitmap(4 * w, 4 * h);

                for (int y = 0; y < 4*h; y += 32)
                    for (int x = 0; x < 4*w; x += 32)
                    {
                        Graphics g = Graphics.FromImage(mv);
                        g.DrawRectangle(Pens.Wheat, x, y, 31, 31);
                    }
                /*
                mv = new Bitmap(w, h);

                for (int y = 0; y < h; y += 8)
                    for (int x = 0; x < w; x += 8)
                    {
                        Graphics g = Graphics.FromImage(mv);
                        g.DrawRectangle(Pens.Wheat, x, y, 7, 7);
                    }
                */

                setPictureBox3(mv);
                //pictureBox3.Refresh();

                cfmatrix = new byte[w, h, 3];
                rfmatrix = new byte[w, h, 3];

                for (int y = 0; y < cf.Height; y++)
                    for (int x = 0; x < cf.Width; x++)
                    {
                        cfmatrix[x, y, 0] = cf.GetPixel(x, y).R;
                        cfmatrix[x, y, 1] = cf.GetPixel(x, y).G;
                        cfmatrix[x, y, 2] = cf.GetPixel(x, y).B;

                        rfmatrix[x, y, 0] = rf.GetPixel(x, y).R;
                        rfmatrix[x, y, 1] = rf.GetPixel(x, y).G;
                        rfmatrix[x, y, 2] = rf.GetPixel(x, y).B;
                    }

                //箭頭線
                pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;


                //開始匹配
                switch (searchMethod)
                {
                    case 1:
                        Exhausted_Search();
                        break;
                    case 2:
                        Coarse_quantization_of_vectors();
                        break;
                    case 3:
                        Principle_of_Locality();
                        break;
                    case 4:
                        Two_Dimensional_Logarithmic_Search();
                        break;
                    case 5:
                        Three_Step_Search();
                        break;
                    case 6:
                        Orthogonal_Search_Algorithm();
                        break;
                    case 7:
                        One_at_a_Time_Search();
                        break;
                    case 8:
                        Cross_Search_Algorithm();
                        break;
                    case 9:
                        Exhausted_Search_with_Block_Based_Difference_Coding();
                        break;
                    default:
                        break;
                }

                //後處理
                bw.Close();
                TimeSpan ts = DateTime.Now - dt;
                setLabel4Text("Compression Time : " + ts.ToString(@"hh\.mm\.ss\.fff"));
                setButton1Text("Start");
                fs.Close();

                //顯示圖片
                FileStream fs2 = File.OpenRead(curFile);
                br = new BinaryReader(fs2);
                bmp = (Bitmap)rf.Clone();
                Bitmap debmp = new Bitmap(w, h);

                int xx, yy;

                for (int j = 0; j < h; j += 8)
                    for (int i = 0; i < w; i += 8)
                    {
                        xx = br.ReadInt16();
                        Rectangle r = new Rectangle(i, j, 8, 8);

                        if(xx!=-1)
                        {
                        yy = br.ReadInt16();

                        for (int bj = 0; bj < 8; bj++)
                            for (int bi = 0; bi < 8; bi++)
                            {
                                debmp.SetPixel(bi + i, bj + j, bmp.GetPixel(xx + bi, yy + bj));
                            }
                        }
                        else
                        {
                            BitmapData bd = debmp.LockBits(r, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
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
                            debmp.UnlockBits(bd);
                        }
                    }
                setPictureBox4(debmp);
                br.Close();
                fs2.Close();
                caculatePSNR(debmp, cf);

                settextBox1Text(1);

            }
            else
            {
                MessageBox.Show("Open reference frame and current frame.");
            }
        }

        #region delegate

        private void setPictureBox1(Bitmap bmp)
        {
            if (this.pictureBox1.InvokeRequired)
            {
                SetImageCallback i = new SetImageCallback(setPictureBox1);
                this.Invoke(i, new object[] { bmp });
            }
            else
            {
                pictureBox1.Image = (Bitmap)bmp.Clone();
                pictureBox1.Refresh();
            }
        }

        private void setPictureBox2(Bitmap bmp)
        {
            if (this.pictureBox2.InvokeRequired)
            {
                SetImageCallback i = new SetImageCallback(setPictureBox2);
                this.Invoke(i, new object[] { bmp });
            }
            else
            {
                pictureBox2.Image = (Bitmap)bmp.Clone();
                pictureBox2.Refresh();
            }
        }

        private void setPictureBox3(Bitmap bmp)
        {
            if (this.pictureBox3.InvokeRequired)
            {
                SetImageCallback i = new SetImageCallback(setPictureBox3);
                this.Invoke(i, new object[] { bmp });
            }
            else
            {
                pictureBox3.Image = (Bitmap)bmp.Clone();
                pictureBox3.Refresh();
            }
        }

        private void setPictureBox4(Bitmap bmp)
        {
            if (this.pictureBox4.InvokeRequired)
            {
                SetImageCallback i = new SetImageCallback(setPictureBox4);
                this.Invoke(i, new object[] { bmp });
            }
            else
            {
                pictureBox4.Image = (Bitmap)bmp.Clone();
                pictureBox4.Refresh();
            }
        }

        private void setLabel4Text(string s)
        {
            if (this.label4.InvokeRequired)
            {
                SetLabelCallback b = new SetLabelCallback(setLabel4Text);
                this.Invoke(b, new object[] { s });
            }
            else
                label4.Text = s; 
        }
        private void setLabel5Text(string s)
        {
            if (this.label5.InvokeRequired)
            {
                SetLabelCallback b = new SetLabelCallback(setLabel5Text);
                this.Invoke(b, new object[] { s });
            }
            else
                label5.Text = s;
        }

        private void setButton1Text(string s)
        {
            if (this.button1.InvokeRequired)
            {
                SetButtonCallback b = new SetButtonCallback(setButton1Text);
                this.Invoke(b, new object[] { s });
            }
            else
                button1.Text = s;
        }

        private void settextBox1Text(int n)
        {
            if (this.textBox1.InvokeRequired)
            {
                SetTextBoxCallback b = new SetTextBoxCallback(settextBox1Text);
                this.Invoke(b, new object[] { n });
            }
            else
            {
                switch (n)
                {
                    case 1:
                        textBox1.Text += "Search Algorithm : " + comboBox2.Text + "     ";
                        textBox1.Text += "Matching Criteria : " + comboBox1.Text + Environment.NewLine;
                        textBox1.Text += label4.Text + "        ";
                        textBox1.Text += label5.Text + Environment.NewLine + Environment.NewLine;
                        break;
                }
            }
        }

        # endregion

        private double errorCaculate(int i,int j,int ri,int rj)
        {
            //將上一輪計算過的誤差歸零，重新計算誤差
            double er = 0;

            //開始匹配
            switch (matchingCriteria)
            {
                case 1:     //1. Absolute Difference
                    //計算目標block(current frame的block)和候選block(reference frame的block)間的誤差(RGB絕對值相加)
                    for (int bj = 0; bj < 8; bj++)
                        for (int bi = 0; bi < 8; bi++)
                        {
                            er += Math.Abs(Convert.ToDouble(cfmatrix[i + bi, j + bj, 0]) - Convert.ToDouble(rfmatrix[ri + bi, rj + bj, 0]));
                            er += Math.Abs(Convert.ToDouble(cfmatrix[i + bi, j + bj, 1]) - Convert.ToDouble(rfmatrix[ri + bi, rj + bj, 1]));
                            er += Math.Abs(Convert.ToDouble(cfmatrix[i + bi, j + bj, 2]) - Convert.ToDouble(rfmatrix[ri + bi, rj + bj, 2]));
                        }
                    break;
                case 2:     //2. Square Difference
                    for (int bj = 0; bj < 8; bj++)
                        for (int bi = 0; bi < 8; bi++)
                        {
                            er += Math.Pow((Convert.ToDouble(cfmatrix[i + bi, j + bj, 0]) - Convert.ToDouble(rfmatrix[ri + bi, rj + bj, 0])), 2);
                            er += Math.Pow((Convert.ToDouble(cfmatrix[i + bi, j + bj, 1]) - Convert.ToDouble(rfmatrix[ri + bi, rj + bj, 1])), 2);
                            er += Math.Pow((Convert.ToDouble(cfmatrix[i + bi, j + bj, 2]) - Convert.ToDouble(rfmatrix[ri + bi, rj + bj, 2])), 2);
                        }
                    break;
                case 3:     //3. Pel Difference Classification
                    for (int bj = 0; bj < 8; bj++)
                        for (int bi = 0; bi < 8; bi++)
                        {
                            if (Math.Abs(Convert.ToDouble(cfmatrix[i + bi, j + bj, 0]) - Convert.ToDouble(rfmatrix[ri + bi, rj + bj, 0])) >= thresholdforPDC)
                                er++;
                            if (Math.Abs(Convert.ToDouble(cfmatrix[i + bi, j + bj, 1]) - Convert.ToDouble(rfmatrix[ri + bi, rj + bj, 1])) >= thresholdforPDC)
                                er++;
                            if (Math.Abs(Convert.ToDouble(cfmatrix[i + bi, j + bj, 2]) - Convert.ToDouble(rfmatrix[ri + bi, rj + bj, 2])) >= thresholdforPDC)
                                er++;
                        }
                    break;
                case 4:     //4. Integral Projection  
                    for (int bj = 0; bj < 8; bj++)
                        for (int bi = 0; bi < 8; bi++)
                        {
                            er += 2 * Math.Abs(Convert.ToDouble(cfmatrix[i + bi, j + bj, 0]) - Convert.ToDouble(rfmatrix[ri + bi, rj + bj, 0]));
                            er += 2 * Math.Abs(Convert.ToDouble(cfmatrix[i + bi, j + bj, 1]) - Convert.ToDouble(rfmatrix[ri + bi, rj + bj, 1]));
                            er += 2 * Math.Abs(Convert.ToDouble(cfmatrix[i + bi, j + bj, 2]) - Convert.ToDouble(rfmatrix[ri + bi, rj + bj, 2]));
                        }
                    break;

                default:
                    break;
            }

            return er;
        }

        private void Exhausted_Search_with_Block_Based_Difference_Coding()
        {
            //由current frame的每個block(8 x 8)開始搜尋
            for (int j = 0; j < h; j += 8)
                for (int i = 0; i < w; i += 8)
                {
                    int x = 0, y = 0;

                    if (checkBox1.Checked)
                    {
                        searchareax = 0;
                        searchareay = 0;
                        sblockx = i;
                        sblocky = j;
                        //show the current frame which is painted rectangular block
                        showFrame(i, j, 1);
                    }

                    //表示第一次找current frame block的初始座標
                    bool flag = false;

                    //存目前找對最小的誤差值
                    error = double.MaxValue;

                    //對應reference frame 逐格(pixel)尋找
                    for (int rj = -1; rj <= h - 8; rj++)
                        for (int ri = 0; ri <= w - 8; ri++)
                        {
                            //先從current frame block的原始位置尋找
                            if (rj == -1)
                            {
                                rj = j;
                                ri = i;
                            }

                            if (checkBox1.Checked)
                            {
                                //在尋找位置畫方框
                                showFrame(ri, rj, 0);

                                if (checkBox2.Checked)
                                    Thread.Sleep(500);
                            }


                            //計算目標block(current frame的block)和候選block(reference frame的block)間的誤差(RGB絕對值相加)
                            double er = errorCaculate(i, j, ri, rj);

                            //如果計算出的誤差值比之前找到的誤差值小
                            if (er < error)
                            {
                                //把之前的誤差值用現在的取代
                                error = er;

                                //紀錄座標
                                x = ri;
                                y = rj;

                                //如果一模一樣，就跳出迴圈
                                if (error == 0)
                                {
                                    ri = w;
                                    rj = h;
                                }
                            }

                            //如果原始座標找不到一模一樣的，那就整個找一遍，取誤差最小的
                            if (flag == false && rj == j)
                            {
                                rj = 0;
                                ri = 0;
                                flag = true;
                            }
                        }

                    //draw motion vectors
                    Graphics g = Graphics.FromImage(mv);

                    if (error < 3500)
                    {

                        //放大四倍
                        //如果block沒有移動
                        if (x == i && y == j)
                        {
                            //畫一個點
                            g.FillRectangle(Brushes.Orange, i*4 + 15, j*4 + 15, 2, 2);
                        }
                        //如果block有移動
                        else
                        {
                            //從block在reference frame 的位置 畫箭頭到 current frame的位置
                            g.DrawLine(pen,4* x + 16, 4*y + 16, 4*i + 16, 4*j + 16);
                        }
                        /*
                        //如果block沒有移動
                        if (x == i && y == j)
                        {
                            //畫一個點
                            g.FillRectangle(Brushes.Orange, i + 3, j + 3, 2, 2);
                        }
                        //如果block有移動
                        else
                        {
                            //從block在reference frame 的位置 畫箭頭到 current frame的位置
                            g.DrawLine(pen, x + 4, y + 4, i + 4, j + 4);
                        }
                        */

                        //save motion vectors data;
                        bw.Write(Convert.ToInt16(x));
                        bw.Write(Convert.ToInt16(y));
                    }
                    //difference coding
                    else
                    {
                        g.FillRectangle(Brushes.Green, 4*i, 4*j, 32, 32);

                        //做記號，表示做difference coding
                        bw.Write(Convert.ToInt16(-1));

                        //記錄block的所有值
                        for (int bj = 0; bj < 8; bj++)
                            for (int bi = 0; bi < 8; bi++)
                            {
                                bw.Write(cfmatrix[i + bi, j + bj, 0]);
                                bw.Write(cfmatrix[i + bi, j + bj, 1]);
                                bw.Write(cfmatrix[i + bi, j + bj, 2]);
                            }
                    }
                    g.Dispose();
                    setPictureBox3(mv);
                    Thread.Sleep(100);

                }
        }

        private void Exhausted_Search()
        {
            //由current frame的每個block(8 x 8)開始搜尋
            for (int j = 0; j < h; j += 8)
                for (int i = 0; i < w; i += 8)
                {
                    int x = 0, y = 0;

                    if (checkBox1.Checked)
                    {
                        searchareax = 0;
                        searchareay = 0;
                        sblockx = i;
                        sblocky = j;
                        //show the current frame which is painted rectangular block
                        showFrame(i, j, 1);
                    }

                    //表示第一次找current frame block的初始座標
                    bool flag = false;

                    //存目前找對最小的誤差值
                    error = double.MaxValue;

                    //對應reference frame 逐格(pixel)尋找
                    for (int rj = -1; rj <= h - 8; rj++)
                        for (int ri = 0; ri <= w - 8; ri++)
                        {
                            //先從current frame block的原始位置尋找
                            if (rj == -1)
                            {
                                rj = j;
                                ri = i;
                            }

                            if (checkBox1.Checked)
                            {
                                //在尋找位置畫方框
                                showFrame(ri, rj, 0);

                                if (checkBox2.Checked)
                                    Thread.Sleep(500);
                            }

                            //計算目標block(current frame的block)和候選block(reference frame的block)間的誤差(RGB絕對值相加)
                            double er = errorCaculate(i, j, ri, rj);

                            //如果計算出的誤差值比之前找到的誤差值小
                            if (er < error)
                            {
                                //把之前的誤差值用現在的取代
                                error = er;

                                //紀錄座標
                                x = ri;
                                y = rj;

                                //如果一模一樣，就跳出迴圈
                                if (error == 0)
                                {
                                    ri = w;
                                    rj = h;
                                }
                            }

                            //如果原始座標找不到一模一樣的，那就整個找一遍，取誤差最小的
                            if (flag == false && rj == j)
                            {
                                rj = 0;
                                ri = 0;
                                flag = true;
                            }
                        }

                    //draw motion vectors
                    Graphics g = Graphics.FromImage(mv);
                    //放大四倍
                    //如果block沒有移動
                    if (x == i && y == j)
                    {
                        //畫一個點
                        g.FillRectangle(Brushes.Orange, 4*i + 15, 4*j + 15, 2, 2);
                    }
                    //如果block有移動
                    else
                    {
                        //從block在reference frame 的位置 畫箭頭到 current frame的位置
                        g.DrawLine(pen, 4*x + 16, 4*y + 16, 4*i + 16, 4*j + 16);
                    }
                    /*
                    //如果block沒有移動
                    if (x == i && y == j)
                    {
                        //畫一個點
                        g.FillRectangle(Brushes.Orange, i + 3, j + 3, 2, 2);
                    }
                    //如果block有移動
                    else
                    {
                        //從block在reference frame 的位置 畫箭頭到 current frame的位置
                        g.DrawLine(pen, x + 4, y + 4, i + 4, j + 4);
                    }
                    */
                    //save motion vectors data;
                    bw.Write(Convert.ToInt16(x));
                    bw.Write(Convert.ToInt16(y));

                    g.Dispose();
                    setPictureBox3(mv);
                    Thread.Sleep(100);

                }
        }

        private void Coarse_quantization_of_vectors()
        {
            //由current frame的每個block(8 x 8)開始搜尋
            for (int j = 0; j < h; j += 8)
                for (int i = 0; i < w; i += 8)
                {
                    int x = 0, y = 0;

                    if (checkBox1.Checked)
                    {
                        searchareax = 13;
                        searchareay = 13;
                        sblockx = i;
                        sblocky = j;
                        //show the current frame which is painted rectangular block
                        showFrame(i, j, 1);
                    }

                    //存目前找對最小的誤差值
                    error = double.MaxValue;

                    //對應reference frame 開始Coarse Quantization of Vectors
                    int ri = i;
                    int rj = j;
                    int step = 2;

                    for (int ay = -6; ay <= 6; ay += step)
                        for (int ax = -6; ax <= 6; ax += step)
                        {
                            //判斷是否超出邊界
                            if (ri + ax >= 0 && rj + ay >= 0 && ri + 7 + ax < w && rj + 7 + ay < h)
                            {
                                if (checkBox1.Checked)
                                {
                                    //在尋找位置畫方框
                                    showFrame(ri + ax, rj + ay, 0);

                                    if (checkBox2.Checked)
                                        Thread.Sleep(500);
                                }
                                double er = errorCaculate(i, j, ri + ax, rj + ay);

                                //如果計算出的誤差值比之前找到的誤差值小
                                if (er < error)
                                {
                                    //把之前的誤差值用現在的取代
                                    error = er;

                                    //紀錄座標
                                    x = ri + ax;
                                    y = rj + ay;
                                }

                                if (ax >= -2 && ax <= 2 && ay >= -2 && ay <= 2)
                                    step = 1;
                                else
                                    step = 2;
                            }
                        }

                    //draw motion vectors
                    Graphics g = Graphics.FromImage(mv);
                    //放大四倍
                    //如果block沒有移動
                    if (x == i && y == j)
                    {
                        //畫一個點
                        g.FillRectangle(Brushes.Orange, 4*i + 15, 4*j + 15, 2, 2);
                    }
                    //如果block有移動
                    else
                    {
                        //從block在reference frame 的位置 畫箭頭到 current frame的位置
                        g.DrawLine(pen, 4*x + 16, 4*y + 16, 4*i + 16, 4*j + 16);
                    }
                    /*
                    //如果block沒有移動
                    if (x == i && y == j)
                    {
                        //畫一個點
                        g.FillRectangle(Brushes.Orange, i + 3, j + 3, 2, 2);
                    }
                    //如果block有移動
                    else
                    {
                        //從block在reference frame 的位置 畫箭頭到 current frame的位置
                        g.DrawLine(pen, x + 4, y + 4, i + 4, j + 4);
                    }
                     * */
                    //save motion vectors data;
                    bw.Write(Convert.ToInt16(x));
                    bw.Write(Convert.ToInt16(y));

                    g.Dispose();
                    setPictureBox3(mv);

                    Thread.Sleep(100);
                }
        }

        private void Principle_of_Locality()
        {
            //由current frame的每個block(8 x 8)開始搜尋
            for (int j = 0; j < h; j += 8)
                for (int i = 0; i < w; i += 8)
                {
                    int x = 0, y = 0;

                    if (checkBox1.Checked)
                    {
                        searchareax = 21;
                        searchareay = 11;
                        sblockx = i;
                        sblocky = j;
                        //show the current frame which is painted rectangular block
                        showFrame(i, j, 1);

                    }

                    //存目前找對最小的誤差值
                    error = double.MaxValue;

                    //對應reference frame 開始Principle of Locality
                    int ri = i;
                    int rj = j;

                    //第一輪稀疏的找
                    for (int ay = -5; ay <= 5; ay += 2)
                        for (int ax = -10; ax <= 10; ax += 3)
                        {
                            //判斷是否超出邊界
                            if (ri + ax >= 0 && rj + ay >= 0 && ri + 7 + ax < w && rj + 7 + ay < h)
                            {
                                if (checkBox1.Checked)
                                {
                                    //在尋找位置畫方框
                                    showFrame(ri + ax, rj + ay, 0);

                                    if (checkBox2.Checked)
                                        Thread.Sleep(500);
                                }
                                double er = errorCaculate(i, j, ri + ax, rj + ay);

                                //如果計算出的誤差值比之前找到的誤差值小
                                if (er < error)
                                {
                                    //把之前的誤差值用現在的取代
                                    error = er;

                                    //紀錄座標
                                    x = ri + ax;
                                    y = rj + ay;
                                }

                            }
                        }
                    //第二輪密集的找
                    ri = x;
                    rj = y;
                    for (int ay = -1; ay <= 1; ay ++)
                        for (int ax = -2; ax <= 2; ax ++)
                        {
                            //判斷是否超出邊界
                            if (ri + ax >= 0 && rj + ay >= 0 && ri + 7 + ax < w && rj + 7 + ay < h)
                            {
                                if (checkBox1.Checked)
                                {
                                    //在尋找位置畫方框
                                    showFrame(ri + ax, rj + ay, 0);

                                    if (checkBox2.Checked)
                                        Thread.Sleep(500);
                                }
                                double er = errorCaculate(i, j, ri + ax, rj + ay);

                                //如果計算出的誤差值比之前找到的誤差值小
                                if (er < error)
                                {
                                    //把之前的誤差值用現在的取代
                                    error = er;

                                    //紀錄座標
                                    x = ri + ax;
                                    y = rj + ay;
                                }

                            }
                        }

                    //draw motion vectors
                    Graphics g = Graphics.FromImage(mv);

                    //放大四倍
                    //如果block沒有移動
                    if (x == i && y == j)
                    {
                        //畫一個點
                        g.FillRectangle(Brushes.Orange, 4*i + 15, 4*j + 15, 2, 2);
                    }
                    //如果block有移動
                    else
                    {
                        //從block在reference frame 的位置 畫箭頭到 current frame的位置
                        g.DrawLine(pen, 4*x + 16, 4*y + 16, 4*i + 16, 4*j + 16);
                    }
                    /*
                    //如果block沒有移動
                    if (x == i && y == j)
                    {
                        //畫一個點
                        g.FillRectangle(Brushes.Orange, i + 3, j + 3, 2, 2);
                    }
                    //如果block有移動
                    else
                    {
                        //從block在reference frame 的位置 畫箭頭到 current frame的位置
                        g.DrawLine(pen, x + 4, y + 4, i + 4, j + 4);
                    }
                    */

                    //save motion vectors data;
                    bw.Write(Convert.ToInt16(x));
                    bw.Write(Convert.ToInt16(y));

                    g.Dispose();
                    setPictureBox3(mv);

                    Thread.Sleep(100);
                }
        }

        private void Two_Dimensional_Logarithmic_Search()
        {
            //由current frame的每個block(8 x 8)開始搜尋
            for (int j = 0; j < h; j += 8)
                for (int i = 0; i < w; i += 8)
                {
                    int x = 0, y = 0;

                    if (checkBox1.Checked)
                    {
                        searchareax = 17;
                        searchareay = 17;
                        sblockx = i;
                        sblocky = j;
                        //show the current frame which is painted rectangular block
                        showFrame(i, j, 1);
                    }

                    //存目前找對最小的誤差值
                    error = double.MaxValue;

                    //對應reference frame 開始Two Dimensional Logarithmic Search
                    int ri = i;
                    int rj = j;
                    int step = 4;

                    for (; step > 0; )
                    {
                        //找原點和週邊四點
                        for (int n = 0; n < 9; n+=2)
                        {
                            int offsetx = 0;
                            int offsety = 0;

                            setoxoy(n, ref offsetx, ref offsety);

                            //判斷是否超出邊界
                            if (ri + offsetx * step >= 0 && rj + offsety * step >= 0 && ri + 7 + offsetx * step < w && rj + 7 + offsety * step < h)
                            {
                                if (checkBox1.Checked)
                                {
                                    //在尋找位置畫方框
                                    showFrame(ri + offsetx * step, rj + offsety * step, 0);

                                    if (checkBox2.Checked)
                                        Thread.Sleep(500);
                                }

                                double er = errorCaculate(i, j, ri + offsetx * step, rj + offsety * step);

                                //如果計算出的誤差值比之前找到的誤差值小
                                if (er < error)
                                {
                                    //把之前的誤差值用現在的取代
                                    error = er;

                                    //紀錄座標
                                    x = ri + offsetx * step;
                                    y = rj + offsety * step;
                                }
                            }
                        }
                        if (x == ri && y == rj)
                        {
                            if (step == 1)
                                step = 0;
                            else
                                step /= 2;
                        }
                        ri = x;
                        rj = y;
                    }

                    //draw motion vectors
                    Graphics g = Graphics.FromImage(mv);

                    //放大四倍
                    //如果block沒有移動
                    if (x == i && y == j)
                    {
                        //畫一個點
                        g.FillRectangle(Brushes.Orange, 4*i + 15, 4*j + 15, 2, 2);
                    }
                    //如果block有移動
                    else
                    {
                        //從block在reference frame 的位置 畫箭頭到 current frame的位置
                        g.DrawLine(pen, 4*x + 16, 4*y + 16, 4*i + 16, 4*j + 16);
                    }
                    /*
                    //如果block沒有移動
                    if (x == i && y == j)
                    {
                        //畫一個點
                        g.FillRectangle(Brushes.Orange, i + 3, j + 3, 2, 2);
                    }
                    //如果block有移動
                    else
                    {
                        //從block在reference frame 的位置 畫箭頭到 current frame的位置
                        g.DrawLine(pen, x + 4, y + 4, i + 4, j + 4);
                    }
                    */ 

                    //save motion vectors data;
                    bw.Write(Convert.ToInt16(x));
                    bw.Write(Convert.ToInt16(y));

                    g.Dispose();
                    setPictureBox3(mv);

                    Thread.Sleep(100);
                }
        }

        private void Three_Step_Search()
        {
            //由current frame的每個block(8 x 8)開始搜尋
            for (int j = 0; j < h; j += 8)
                for (int i = 0; i < w; i += 8)
                {
                    int x = 0, y = 0;

                    if (checkBox1.Checked)
                    {
                        searchareax = 13;
                        searchareay = 13;
                        sblockx = i;
                        sblocky = j;
                        //show the current frame which is painted rectangular block
                        showFrame(i, j, 1);
                    }

                    //存目前找對最小的誤差值
                    error = double.MaxValue;

                    //對應reference frame 開始三步搜尋法
                    int ri = i;
                    int rj = j;

                    for (int step = 3; step > 0; step--)
                    {
                        for (int n = 0; n < 9; n++)
                        {
                            int offsetx = 0;
                            int offsety = 0;

                            setoxoy(n, ref offsetx, ref offsety);

                            //判斷是否超出邊界
                            if (ri + offsetx * step >= 0 && rj + offsety * step >= 0 && ri + 7 + offsetx * step < w && rj + 7 + offsety * step < h)
                            {
                                if (checkBox1.Checked)
                                {
                                    //在尋找位置畫方框
                                    showFrame(ri + offsetx * step, rj + offsety * step, 0);

                                    if (checkBox2.Checked)
                                        Thread.Sleep(500);
                                }

                                double er = errorCaculate(i, j, ri + offsetx * step, rj + offsety * step);

                                //如果計算出的誤差值比之前找到的誤差值小
                                if (er < error)
                                {
                                    //把之前的誤差值用現在的取代
                                    error = er;

                                    //紀錄座標
                                    x = ri + offsetx * step;
                                    y = rj + offsety * step;
                                }
                            }
                        }
                        ri = x;
                        rj = y;
                    }

                    //draw motion vectors
                    Graphics g = Graphics.FromImage(mv);

                    //放大四倍
                    //如果block沒有移動
                    if (x == i && y == j)
                    {
                        //畫一個點
                        g.FillRectangle(Brushes.Orange, 4*i + 15, 4*j + 15, 2, 2);
                    }
                    //如果block有移動
                    else
                    {
                        //從block在reference frame 的位置 畫箭頭到 current frame的位置
                        g.DrawLine(pen, 4*x + 16, 4*y + 16, 4*i + 16, 4*j + 16);
                    }
                    /*
                    //如果block沒有移動
                    if (x == i && y == j)
                    {
                        //畫一個點
                        g.FillRectangle(Brushes.Orange, i + 3, j + 3, 2, 2);
                    }
                    //如果block有移動
                    else
                    {
                        //從block在reference frame 的位置 畫箭頭到 current frame的位置
                        g.DrawLine(pen, x + 4, y + 4, i + 4, j + 4);
                    }
                    */

                    //save motion vectors data;
                    bw.Write(Convert.ToInt16(x));
                    bw.Write(Convert.ToInt16(y));

                    g.Dispose();
                    setPictureBox3(mv);

                    Thread.Sleep(100);

                }
        }

        private void setoxoy(int n,ref int offsetx,ref int offsety)
        {
            switch(n)
            {
                case 0:
                    offsetx = 0;
                    offsety = 0;
                    break;
                case 1:
                    offsetx = -1;
                    offsety = -1;
                    break;
                case 2:
                    offsetx = 0;
                    offsety = -1;
                    break;
                case 3:
                    offsetx = 1;
                    offsety = -1;
                    break;
                case 4:
                    offsetx = 1;
                    offsety = 0;
                    break;
                case 5:
                    offsetx = 1;
                    offsety = 1;
                    break;
                case 6:
                    offsetx = 0;
                    offsety = 1;
                    break;
                case 7:
                    offsetx = -1;
                    offsety = 1;
                    break;
                case 8:
                    offsetx = -1;
                    offsety = 0;
                    break;
            }
        }

        private void Orthogonal_Search_Algorithm()
        {
            //由current frame的每個block(8 x 8)開始搜尋
            for (int j = 0; j < h; j += 8)
                for (int i = 0; i < w; i += 8)
                {
                    int x = 0, y = 0;

                    if (checkBox1.Checked)
                    {
                        searchareax = 17;
                        searchareay = 17;
                        sblockx = i;
                        sblocky = j;
                        //show the current frame which is painted rectangular block
                        showFrame(i, j, 1);
                    }

                    //存目前找對最小的誤差值
                    error = double.MaxValue;

                    //對應reference frame 開始Orthogonal Search Algorithm
                    int ri = i;
                    int rj = j;

                    for (int step = 4; step > 0; )
                    {
                        //水平
                        for (int n = 0; n < 3; n++)
                        {
                            int offsetx = 0;
                            int offsety = 0;
                            
                            switch(n)
                            {
                                case 0: offsetx = 0;  break;
                                case 1: offsetx = 1;  break;
                                case 2: offsetx = -1; break;
                            }

                            //判斷是否超出邊界
                            if (ri + offsetx * step >= 0 && rj + offsety * step >= 0 && ri + 7 + offsetx * step < w && rj + 7 + offsety * step < h)
                            {
                                if (checkBox1.Checked)
                                {
                                    //在尋找位置畫方框
                                    showFrame(ri + offsetx * step, rj + offsety * step, 0);

                                    if (checkBox2.Checked)
                                        Thread.Sleep(500);
                                }

                                double er = errorCaculate(i, j, ri + offsetx * step, rj + offsety * step);

                                //如果計算出的誤差值比之前找到的誤差值小
                                if (er < error)
                                {
                                    //把之前的誤差值用現在的取代
                                    error = er;

                                    //紀錄座標
                                    x = ri + offsetx * step;
                                    y = rj + offsety * step;
                                }
                            }
                        }
                        //垂直
                        for (int n = 0; n < 3; n++)
                        {
                            int offsetx = 0;
                            int offsety = 0;

                            switch (n)
                            {
                                case 0: offsety = 0; break;
                                case 1: offsety = 1; break;
                                case 2: offsety = -1; break;
                            }

                            //判斷是否超出邊界
                            if (ri + offsetx * step >= 0 && rj + offsety * step >= 0 && ri + 7 + offsetx * step < w && rj + 7 + offsety * step < h)
                            {
                                if (checkBox1.Checked)
                                {
                                    //在尋找位置畫方框
                                    showFrame(ri + offsetx * step, rj + offsety * step, 0);

                                    if (checkBox2.Checked)
                                        Thread.Sleep(500);
                                }

                                double er = errorCaculate(i, j, ri + offsetx * step, rj + offsety * step);

                                //如果計算出的誤差值比之前找到的誤差值小
                                if (er < error)
                                {
                                    //把之前的誤差值用現在的取代
                                    error = er;

                                    //紀錄座標
                                    x = ri + offsetx * step;
                                    y = rj + offsety * step;
                                }
                            }
                        }
                        ri = x;
                        rj = y;

                        if (step == 1)
                            step = 0;
                        else
                            step /= 2;
                    }

                    //draw motion vectors
                    Graphics g = Graphics.FromImage(mv);

                    //放大四倍
                    //如果block沒有移動
                    if (x == i && y == j)
                    {
                        //畫一個點
                        g.FillRectangle(Brushes.Orange, 4*i + 15, 4*j + 15, 2, 2);
                    }
                    //如果block有移動
                    else
                    {
                        //從block在reference frame 的位置 畫箭頭到 current frame的位置
                        g.DrawLine(pen, 4*x + 16, 4*y + 16, 4*i + 16, 4*j + 16);
                    }
                    /*
                    //如果block沒有移動
                    if (x == i && y == j)
                    {
                        //畫一個點
                        g.FillRectangle(Brushes.Orange, i + 3, j + 3, 2, 2);
                    }
                    //如果block有移動
                    else
                    {
                        //從block在reference frame 的位置 畫箭頭到 current frame的位置
                        g.DrawLine(pen, x + 4, y + 4, i + 4, j + 4);
                    }
                     */

                    //save motion vectors data;
                    bw.Write(Convert.ToInt16(x));
                    bw.Write(Convert.ToInt16(y));
                    
                    g.Dispose();
                    setPictureBox3(mv);

                    Thread.Sleep(100);

                }
        }


        //可能有問題，未驗正
        private void One_at_a_Time_Search()
        {
            //由current frame的每個block(8 x 8)開始搜尋
            for (int j = 0; j < h; j += 8)
                for (int i = 0; i < w; i += 8)
                {
                    int x = 0, y = 0;

                    if (checkBox1.Checked)
                    {
                        searchareax = 13;
                        searchareay = 13;
                        sblockx = i;
                        sblocky = j;
                        //show the current frame which is painted rectangular block
                        showFrame(i, j, 1);
                    }

                    //存目前找對最小的誤差值
                    error = double.MaxValue;

                    //對應reference frame 開始One at a Time Search
                    int ri = i;
                    int rj = j;
                    bool flag = false;
                    //先比自己和身邊兩點

                    if (checkBox1.Checked)
                    {
                        //在尋找位置畫方框
                        showFrame(ri, rj, 0);

                        if (checkBox2.Checked)
                            Thread.Sleep(500);
                    }

                    double er = errorCaculate(i,j,ri,rj);
                    //如果計算出的誤差值比之前找到的誤差值小
                    if (er < error)
                    {
                        //把之前的誤差值用現在的取代
                        error = er;

                        //紀錄座標
                        x = ri;
                        y = rj;
                    }

                    do
                    {
                        if(flag)
                            if (x == ri) break;
                        else if (x > ri)
                        {
                            ri++;
                            if (ri + 7 < w)
                            {
                                if (checkBox1.Checked)
                                {
                                    //在尋找位置畫方框
                                    showFrame(ri, rj, 0);

                                    if (checkBox2.Checked)
                                        Thread.Sleep(500);
                                }

                                er = errorCaculate(i, j, ri, rj);
                                //如果計算出的誤差值比之前找到的誤差值小
                                if (er < error)
                                {
                                    //把之前的誤差值用現在的取代
                                    error = er;

                                    //紀錄座標
                                    x = ri;
                                    y = rj;
                                }
                            }
                            else break;
                        }
                        else if (x < ri)
                        {
                            ri--;
                            if (ri >= 0)
                            {
                                if (checkBox1.Checked)
                                {
                                    //在尋找位置畫方框
                                    showFrame(ri, rj, 0);

                                    if (checkBox2.Checked)
                                        Thread.Sleep(500);
                                }

                                er = errorCaculate(i, j, ri, rj);
                                //如果計算出的誤差值比之前找到的誤差值小
                                if (er < error)
                                {
                                    //把之前的誤差值用現在的取代
                                    error = er;

                                    //紀錄座標
                                    x = ri;
                                    y = rj;
                                }
                            }
                        }
                        flag = true;
                    } while (true);

                    flag = false;

                    do
                    {
                        if(flag)
                            if (y == rj) break;
                        else if (y > rj)
                        {
                            rj++;
                            if (rj + 7 < h)
                            {
                                if (checkBox1.Checked)
                                {
                                    //在尋找位置畫方框
                                    showFrame(ri, rj, 0);

                                    if (checkBox2.Checked)
                                        Thread.Sleep(500);
                                }

                                er = errorCaculate(i, j, ri, rj);
                                //如果計算出的誤差值比之前找到的誤差值小
                                if (er < error)
                                {
                                    //把之前的誤差值用現在的取代
                                    error = er;

                                    //紀錄座標
                                    x = ri;
                                    y = rj;
                                }
                            }
                            else break;
                        }
                        else if (y < rj)
                        {
                            rj--;
                            if (rj >= 0)
                            {
                                if (checkBox1.Checked)
                                {
                                    //在尋找位置畫方框
                                    showFrame(ri, rj, 0);

                                    if (checkBox2.Checked)
                                        Thread.Sleep(500);
                                }

                                er = errorCaculate(i, j, ri, rj);
                                //如果計算出的誤差值比之前找到的誤差值小
                                if (er < error)
                                {
                                    //把之前的誤差值用現在的取代
                                    error = er;

                                    //紀錄座標
                                    x = ri;
                                    y = rj;
                                }
                            }
                        }
                        flag = true;
                    } while (true);


                    //draw motion vectors
                    Graphics g = Graphics.FromImage(mv);

                    //放大四倍
                    //如果block沒有移動
                    if (x == i && y == j)
                    {
                        //畫一個點
                        g.FillRectangle(Brushes.Orange, 4*i + 15, 4*j + 15, 2, 2);
                    }
                    //如果block有移動
                    else
                    {
                        //從block在reference frame 的位置 畫箭頭到 current frame的位置
                        g.DrawLine(pen, 4*x + 16, 4*y + 16, 4*i + 16, 4*j + 16);
                    }
                    /*
                    //如果block沒有移動
                    if (x == i && y == j)
                    {
                        //畫一個點
                        g.FillRectangle(Brushes.Orange, i + 3, j + 3, 2, 2);
                    }
                    //如果block有移動
                    else
                    {
                        //從block在reference frame 的位置 畫箭頭到 current frame的位置
                        g.DrawLine(pen, x + 4, y + 4, i + 4, j + 4);
                    }
                    */ 
                    //save motion vectors data;
                    bw.Write(Convert.ToInt16(x));
                    bw.Write(Convert.ToInt16(y));

                    g.Dispose();
                    setPictureBox3(mv);

                    Thread.Sleep(100);

                }
        }

        private void Cross_Search_Algorithm()
        {
            //由current frame的每個block(8 x 8)開始搜尋
            for (int j = 0; j < h; j += 8)
                for (int i = 0; i < w; i += 8)
                {
                    int x = 0, y = 0;

                    if (checkBox1.Checked)
                    {
                        searchareax = 17;
                        searchareay = 17;
                        sblockx = i;
                        sblocky = j;
                        //show the current frame which is painted rectangular block
                        showFrame(i, j, 1);
                    }

                    //存目前找對最小的誤差值
                    error = double.MaxValue;

                    //對應reference frame 開始Cross Search Algorithm
                    int ri = i;
                    int rj = j;
                    int step = 4;

                    for (; step > 0; )
                    {
                        //找原點和週邊四點
                        for (int n = 0; n < 9; )
                        {
                            int offsetx = 0;
                            int offsety = 0;

                            setoxoy(n, ref offsetx, ref offsety);

                            //判斷是否超出邊界
                            if (ri + offsetx * step >= 0 && rj + offsety * step >= 0 && ri + 7 + offsetx * step < w && rj + 7 + offsety * step < h)
                            {
                                if (checkBox1.Checked)
                                {
                                    //在尋找位置畫方框
                                    showFrame(ri + offsetx * step, rj + offsety * step, 0);

                                    if (checkBox2.Checked)
                                        Thread.Sleep(500);
                                }

                                double er = errorCaculate(i, j, ri + offsetx * step, rj + offsety * step);

                                //如果計算出的誤差值比之前找到的誤差值小
                                if (er < error)
                                {
                                    //把之前的誤差值用現在的取代
                                    error = er;

                                    //紀錄座標
                                    x = ri + offsetx * step;
                                    y = rj + offsety * step;
                                }
                            }

                            if (n == 0)
                                n++;
                            else
                                n += 2;
                        }

                        if (step == 1)
                        {
                            int offsetx = 0;
                            int offsety = 0;
                            int n;
                            //右上或左下  十字
                            if ((x > ri && y < rj) || (x < ri && y > rj))
                                n = 2;
                            //左上或右下 叉字
                            else
                                n = 1;

                            ri = x;
                            rj = y;
                            for (; n < 9; n += 2)
                            {
                                setoxoy(n, ref offsetx, ref offsety);

                                //判斷是否超出邊界
                                if (ri + offsetx * step >= 0 && rj + offsety * step >= 0 && ri + 7 + offsetx * step < w && rj + 7 + offsety * step < h)
                                {
                                    if (checkBox1.Checked)
                                    {
                                        //在尋找位置畫方框
                                        showFrame(ri + offsetx * step, rj + offsety * step, 0);

                                        if (checkBox2.Checked)
                                            Thread.Sleep(500);
                                    }

                                    double er = errorCaculate(i, j, ri + offsetx * step, rj + offsety * step);

                                    //如果計算出的誤差值比之前找到的誤差值小
                                    if (er < error)
                                    {
                                        //把之前的誤差值用現在的取代
                                        error = er;

                                        //紀錄座標
                                        x = ri + offsetx * step;
                                        y = rj + offsety * step;
                                    }
                                }
                            }
                            break;
                        }
                        else
                            step /= 2;

                        ri = x;
                        rj = y;
                    }

                    //draw motion vectors
                    Graphics g = Graphics.FromImage(mv);

                    //放大四倍
                    //如果block沒有移動
                    if (x == i && y == j)
                    {
                        //畫一個點
                        g.FillRectangle(Brushes.Orange, 4*i + 15, 4*j + 15, 2, 2);
                    }
                    //如果block有移動
                    else
                    {
                        //從block在reference frame 的位置 畫箭頭到 current frame的位置
                        g.DrawLine(pen, 4*x + 16, 4*y + 16, 4*i + 16, 4*j + 16);
                    }
                    /*
                    //如果block沒有移動
                    if (x == i && y == j)
                    {
                        //畫一個點
                        g.FillRectangle(Brushes.Orange, i + 3, j + 3, 2, 2);
                    }
                    //如果block有移動
                    else
                    {
                        //從block在reference frame 的位置 畫箭頭到 current frame的位置
                        g.DrawLine(pen, x + 4, y + 4, i + 4, j + 4);
                    }*/

                    //save motion vectors data;
                    bw.Write(Convert.ToInt16(x));
                    bw.Write(Convert.ToInt16(y));

                    g.Dispose();
                    setPictureBox3(mv);

                    Thread.Sleep(100);
                }
        }

        int searchareax = 0, searchareay = 0;
        int sblockx=0, sblocky=0;
        //在blockx,blocky的位置加上方框
        //n = 0 處理current frame , n = 1 處理reference frame
        private void showFrame(int blockx, int blocky, int n)
        {
            unsafe
            {
                BitmapData bmpd;
                BitmapData bd;
                
                if (n == 0)
                {
                    bmpd = rf.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                    bd = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                }
                else
                {
                    bmpd = cf.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                    bd = bmp2.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                }

                byte* pb = (byte*)bd.Scan0;
                int offsetb = bd.Stride - w * 3;

                byte* pbmp = (byte*)bmpd.Scan0;
                int offsetbmp = bmpd.Stride - w * 3;

                for (int j = 0; j < h; j++, pb += offsetb, pbmp += offsetbmp)
                    for (int i = 0; i < w; i++, pb += 3, pbmp += 3)
                    {
                        pb[0] = pbmp[0];
                        pb[1] = pbmp[1];
                        pb[2] = pbmp[2];
                    }

                for (int j = 0; j < 8; j++)
                    for (int i = 0; i < 8; i++)
                    {
                        if (i == 0 || i == 7 || j == 0 || j == 7) 
                        {
                            pb = (byte*)bd.Scan0;
                            pb = pb + bd.Stride * (blocky + j) + (blockx + i) * 3;
                            if (i == 0 && j == 0)
                            {
                                pb[0] = Color.Green.B;
                                pb[1] = Color.Green.G;
                                pb[2] = Color.Green.R;
                            }
                            else
                            {
                                pb[0] = c.B;
                                pb[1] = c.G;
                                pb[2] = c.R;
                            }
                        }
                    }

                if (n == 0)
                {
                    //draw search area
                    for (int j = -(searchareay/2+1); j < searchareay; j++)
                        for (int i = -(searchareax / 2 + 1); i < searchareax; i++)
                        {
                            if (i == -(searchareax / 2 + 1) || i == searchareax - 1 || j == -(searchareay / 2 + 1) || j == searchareay - 1)  
                            {
                                pb = (byte*)bd.Scan0;
                                if (sblockx + i >= 0 && sblockx + i < w && sblocky + j >= 0 && sblocky + j < h)
                                {
                                    pb = pb + bd.Stride * (sblocky + j) + (sblockx + i) * 3;
                                    pb[0] = Color.Yellow.R;
                                    pb[1] = Color.Yellow.G;
                                    pb[2] = Color.Yellow.B;
                                }
                            }
                        }

                    bmp.UnlockBits(bd);
                    rf.UnlockBits(bmpd);
                    setPictureBox1(bmp);
                }
                else if (n == 1)
                {
                    bmp2.UnlockBits(bd);
                    cf.UnlockBits(bmpd);
                    setPictureBox2(bmp2);
                }

            }
        }

        private void blockColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();

            if(cd.ShowDialog()==DialogResult.OK)
            {
                c = cd.Color;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button1.Text != "Start")
            {
                if (button2.Text == "Suspend")
                {
                    th1.Suspend();
                    button2.Text = "Resume";
                }
                else
                {
                    th1.Resume();
                    button2.Text = "Suspend";
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(comboBox2.Text[0])
            {
                case '1': searchMethod = 1; break;
                case '2': searchMethod = 2; break;
                case '3': searchMethod = 3; break;
                case '4': searchMethod = 4; break;
                case '5': searchMethod = 5; break;
                case '6': searchMethod = 6; break;
                case '7': searchMethod = 7; break;
                case '8': searchMethod = 8; break;
                case '9': searchMethod = 9; break;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.Text[0])
            {
                case '1': matchingCriteria = 1; break;
                case '2': matchingCriteria = 2; break;
                case '3': 
                    matchingCriteria = 3;
                    thresholdForm th = new thresholdForm(this);
                    th.ShowDialog();
                    break;
                case '4': matchingCriteria = 4; break;
            }
        }

        private void pixelBasedDifferenceCodingToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //預處理
            bmp = (Bitmap)rf.Clone();
            bmp2 = (Bitmap)cf.Clone();
            w = rf.Width;
            h = rf.Height;
            decodedbmp = new Bitmap(w, h);
            rect = new Rectangle(0, 0, w, h);

            cfmatrix = new byte[w, h, 3];
            rfmatrix = new byte[w, h, 3];

            for (int y = 0; y < cf.Height; y++)
                for (int x = 0; x < cf.Width; x++)
                {
                    cfmatrix[x, y, 0] = cf.GetPixel(x, y).R;
                    cfmatrix[x, y, 1] = cf.GetPixel(x, y).G;
                    cfmatrix[x, y, 2] = cf.GetPixel(x, y).B;

                    rfmatrix[x, y, 0] = rf.GetPixel(x, y).R;
                    rfmatrix[x, y, 1] = rf.GetPixel(x, y).G;
                    rfmatrix[x, y, 2] = rf.GetPixel(x, y).B;
                }

            //curFile = @"C:\Users\iis\Desktop\IMAGE\TEST\test.mv";
            curFile = img_path + @"\TEST\test.mv";
            int n = 1;
            while (File.Exists(curFile))
            {
                curFile = img_path + @"\TEST\test" + "(" + n + ").mv";
                n++;
            }
            fs = File.Create(curFile);
            bw = new BinaryWriter(fs);

            //由current frame的每個pixel開始搜尋
            for (int j = 0; j < h; j++)
                for (int i = 0; i < w; i++)
                {
                    //difference coding

                    bw.Write(Convert.ToInt16(cfmatrix[i, j, 0] - rfmatrix[i, j, 0]));
                    bw.Write(Convert.ToInt16(cfmatrix[i, j, 1] - rfmatrix[i, j, 1]));
                    bw.Write(Convert.ToInt16(cfmatrix[i, j, 2] - rfmatrix[i, j, 2]));

                }
            bw.Close();
            fs.Close();

            fs = File.OpenRead(curFile);
            br = new BinaryReader(fs);

            //由current frame的每個pixel開始搜尋
            for (int j = 0; j < h; j++)
                for (int i = 0; i < w; i++)
                {
                    //difference decoding
                    byte r = Convert.ToByte(rf.GetPixel(i, j).R + br.ReadInt16());
                    byte g = Convert.ToByte(rf.GetPixel(i, j).G + br.ReadInt16());
                    byte b = Convert.ToByte(rf.GetPixel(i, j).B + br.ReadInt16());
                    decodedbmp.SetPixel(i, j, Color.FromArgb(r, g, b));
                }
            pictureBox4.Image = decodedbmp;

            caculatePSNR(decodedbmp, cf);
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

        private void caculatePSNR(Bitmap b1,Bitmap b2)
        {
            double denominator = 0;
            for (int y = 0; y < b1.Height; y++)
                for (int x = 0; x < b1.Width; x++)
                {
                    denominator += Math.Pow(b1.GetPixel(x, y).R - b2.GetPixel(x, y).R, 2);
                    denominator += Math.Pow(b1.GetPixel(x, y).G - b2.GetPixel(x, y).G, 2);
                    denominator += Math.Pow(b1.GetPixel(x, y).B - b2.GetPixel(x, y).B, 2);
                }

            denominator /= (w * h * 3.0);
            if (denominator != 0)
            {
                int psnrvalue = Convert.ToInt32(10 * Math.Log(255 * 255 / denominator, 10));
                
                setLabel5Text("PSNR : " + psnrvalue.ToString("F") + " dB ");
            }
            else 
            {
                setLabel5Text("PSNR : INF");
            }
        }

        private void clearTextBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

    }
}
