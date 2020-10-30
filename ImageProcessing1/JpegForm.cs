using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

namespace ImageProcessing
{
    public partial class JpegForm : Form
    {
        Bitmap bmp;
        Rectangle rect;
        int w, h;
        MainForm mf;
        int[,] s;
        int[,] f;
        int[,] df;

        byte[,] q = new byte[8, 8]{     { 16, 11, 10, 16, 24, 40, 51, 61},
                                        { 12, 12, 14, 19, 26, 58, 60, 55},
                                        { 14, 13, 16, 24, 40, 57, 69, 56},
                                        { 14, 17, 22, 29, 51, 87, 80, 82},
                                        { 18, 22, 37, 56, 68, 109, 103, 77},
                                        { 24, 35, 55, 64, 81, 104, 113, 92},
                                        { 99, 64, 78, 87, 103, 121, 120, 101},
                                        { 72, 92, 95, 98, 112, 100, 103, 99}};

        public JpegForm(MainForm m, Bitmap o)
        {
            InitializeComponent();
            textBox1.Text = ""; 
            label2.Text = "";
            label3.Text = "";
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (q[i, j] < 100)
                        textBox1.Text += "  " + q[i, j] + "   ";
                    else
                        textBox1.Text += q[i, j] + "   ";
                }
                textBox1.Text += Environment.NewLine;
                textBox1.Text += Environment.NewLine;
            }
            
            mf = m;
            bmp = (Bitmap)o.Clone();
            pictureBox1.Image = bmp;
            w = bmp.Width;
            h = bmp.Height;
            rect = new Rectangle(0, 0, w, h);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap bn = (Bitmap)pictureBox1.Image.Clone();
            BitmapData bnd = bn.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int offset = bnd.Stride - bn.Width * 3;
            double cu, cv;
            double sum;
            int N = 8;
            s = new int[w, h];
            f = new int[w, h];

            unsafe
            {
                byte* p = (byte*)bnd.Scan0;

                for (int y = 0; y < h; y++, p += offset)
                {
                    for (int x = 0; x < w; x++, p += 3)
                    {
                        s[x, y] = p[0] - 128;
                    }
                }
            }

            //DCT
            for (int y = 0; y < h; y += 8)
            {
                for (int x = 0; x < w; x += 8)
                {
                    for (int v = 0; v < 8; v++)
                        for (int u = 0; u < 8; u++)
                        {
                            cu = (u == 0) ? Math.Sqrt(1 / (double)2) : 1;
                            cv = (v == 0) ? Math.Sqrt(1 / (double)2) : 1;
                            sum = 0;
                            for (int j = 0; j < N; j++)
                                for (int i = 0; i < N; i++)
                                {
                                    sum += s[(x + j), (y + i)] * Math.Cos((2 * i + 1) * u * Math.PI / 16) * Math.Cos((2 * j + 1) * v * Math.PI / 16);
                                }
                            f[(x + v), (y + u)] = Convert.ToInt32(Math.Round(cu * cv * sum / 4.0));
                        }
                }
            }
            
            int max = int.MinValue; 
            int min = int.MaxValue;
            unsafe
            {
                byte* p = (byte*)bnd.Scan0;
                
                for (int y = 0; y < h; y++, p += offset)
                {
                    for (int x = 0; x < w; x++, p += 3)
                    {
                        max = (max < f[x, y]) ? f[x, y] : max;
                        min = (min > f[x, y]) ? f[x, y] : min;
                        byte pix;
                        if (f[x, y] > 255)
                            pix = 255;
                        else if (f[x, y] < 0)
                            pix = 0;
                        else
                            pix = Convert.ToByte(f[x, y]);
                        p[0] = p[1] = p[2] = pix;

                    }
                }             
            }
            bn.UnlockBits(bnd);
            label2.Text = "Max pixel: " + max + "    Min pixel: " + min;
            pictureBox2.Image = bn;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            //inverse dct
            double cu, cv, sum;
            int N = 8;
            for (int y = 0; y < h; y += 8)
            {
                for (int x = 0; x < w; x += 8)
                {
                    for (int v = 0; v < 8; v++)
                        for (int u = 0; u < 8; u++)
                        {
                            sum = 0;
                            for (int j = 0; j < N; j++)
                                for (int i = 0; i < N; i++)
                                {
                                    cu = (i == 0) ? Math.Sqrt(1 / (double)2) : 1;
                                    cv = (j == 0) ? Math.Sqrt(1 / (double)2) : 1;
                                    sum += cu * cv * f[(x + j), (y + i)] * q[j, i] * Math.Cos((2 * u + 1) * i * Math.PI / 16) * Math.Cos((2 * v + 1) * j * Math.PI / 16);
                                }
                            s[(x + v), (y + u)] = Convert.ToInt32(sum / 4) + 128;
                        }
                }
            }

            Bitmap bb = new Bitmap(w, h);
            List<Point> pt = new List<Point>();
            textBox2.Text = "";

            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    if (s[x, y] < 0)
                    {
                        if (checkBox1.Checked == true)
                        {
                            textBox2.Text += "(" + x + "," + y + ")" + "  " + s[x, y] + Environment.NewLine;
                            pt.Add(new Point(x, y));
                        }
                        s[x, y] = 0;
                    }
                    if (s[x, y] > 255)
                    {
                        if (checkBox1.Checked == true)
                        {
                            textBox2.Text += "(" + x + "," + y + ")" + "  " + s[x, y] + Environment.NewLine;
                            pt.Add(new Point(x, y));
                        }
                        s[x, y] = 255;
                    }
                    byte pix = Convert.ToByte(s[x, y]);
                    bb.SetPixel(x, y, Color.FromArgb(pix, pix, pix));
                }

            if (checkBox1.Checked == true)
            {
                Graphics g = Graphics.FromImage(bb);
                for (int i = 0; i < pt.Count; i++)
                    g.DrawEllipse(Pens.Blue, pt[i].X - 2, pt[i].Y + 2, 5, 5);
            }
            pictureBox3.Image = bb;
        }

        //Encode
        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = File.Open(sfd.FileName + ".myjpeg", FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);

                w = pictureBox1.Image.Width;
                h = pictureBox1.Image.Height;
                int[] zigzag2 = new int[w * h];       //陣列大小等於空間域大小

                rect = new Rectangle(0, 0, w, h);
                Bitmap bn = (Bitmap)pictureBox1.Image.Clone();
                BitmapData bnd = bn.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                int offset = bnd.Stride - bn.Width * 3;

                //dct計算用變數
                double cu, cv;
                double sum;
                int N = 8;

                //dct寄算用存取空間  s表空間域  f表頻域
                s = new int[w, h];
                f = new int[w, h];

                //將影像pixel資訊(在此只處理灰階)減128後存到2為陣列S內
                //減128使S的値域落在 127 ~ -128
                unsafe
                {
                    byte* p = (byte*)bnd.Scan0;

                    for (int y = 0; y < h; y++, p += offset)
                    {
                        for (int x = 0; x < w; x++, p += 3)
                        {
                            s[x, y] = p[0] - 128;
                        }
                    }
                }
                int max = int.MinValue, min = int.MaxValue;

                //DCT  轉換
                int num = 0;

                //先將空間域切成8x8彼此不重疊的區間，再對每個區間處理
                for (int y = 0; y < h; y += 8)
                {
                    for (int x = 0; x < w; x += 8)
                    {
                        //u,v表示計算頻域f的pixel座標x,y
                        for (int v = 0; v < 8; v++)
                            for (int u = 0; u < 8; u++)
                            {
                                //如果是計算頻域f的第一行(即u==0)→cu = Sqrt(1 / (double)2)，否則等於cu = 1
                                cu = (u == 0) ? Math.Sqrt(1 / (double)2) : 1;
                                //如果是計算頻域f的第一列(即v==0)→cv = Sqrt(1 / (double)2)，否則等於cv = 1
                                cv = (v == 0) ? Math.Sqrt(1 / (double)2) : 1;

                                //頻域f的每個座標上的值要由空間域s8x8的每個値計算，並除以對應的量化表
                                sum = 0;
                                for (int j = 0; j < N; j++)
                                    for (int i = 0; i < N; i++)
                                    {
                                        sum += s[(x + j), (y + i)] * Math.Cos((2 * i + 1) * u * Math.PI / 16) * Math.Cos((2 * j + 1) * v * Math.PI / 16);
                                    }
                                f[(x + v), (y + u)] = Convert.ToInt32(Math.Round(cu * cv * sum / 4) / q[v, u]);

                                if (f[(x + v), (y + u)] > max)
                                    max = f[(x + v), (y + u)];
                                if (f[(x + v), (y + u)] < min)
                                    min = f[(x + v), (y + u)];
                            }

                    }
                }

                //zig-zag動作
                for (int x = 0; x < w; x += 8)
                    for (int y = 0; y < h; y += 8)
                    {
                        zigzag2[num++] = f[x + 0, y + 0]; zigzag2[num++] = f[x + 0, y + 1]; zigzag2[num++] = f[x + 1, y + 0]; zigzag2[num++] = f[x + 2, y + 0]; zigzag2[num++] = f[x + 1, y + 1]; zigzag2[num++] = f[x + 0, y + 2];
                        zigzag2[num++] = f[x + 0, y + 3]; zigzag2[num++] = f[x + 1, y + 2]; zigzag2[num++] = f[x + 2, y + 1]; zigzag2[num++] = f[x + 3, y + 0]; zigzag2[num++] = f[x + 4, y + 0]; zigzag2[num++] = f[x + 3, y + 1];
                        zigzag2[num++] = f[x + 2, y + 2]; zigzag2[num++] = f[x + 1, y + 3]; zigzag2[num++] = f[x + 0, y + 4]; zigzag2[num++] = f[x + 0, y + 5]; zigzag2[num++] = f[x + 1, y + 4]; zigzag2[num++] = f[x + 2, y + 3];
                        zigzag2[num++] = f[x + 3, y + 2]; zigzag2[num++] = f[x + 4, y + 1]; zigzag2[num++] = f[x + 5, y + 0]; zigzag2[num++] = f[x + 6, y + 0]; zigzag2[num++] = f[x + 5, y + 1]; zigzag2[num++] = f[x + 4, y + 2];
                        zigzag2[num++] = f[x + 3, y + 3]; zigzag2[num++] = f[x + 2, y + 4]; zigzag2[num++] = f[x + 1, y + 5]; zigzag2[num++] = f[x + 0, y + 6]; zigzag2[num++] = f[x + 0, y + 7]; zigzag2[num++] = f[x + 1, y + 6];
                        zigzag2[num++] = f[x + 2, y + 5]; zigzag2[num++] = f[x + 3, y + 4]; zigzag2[num++] = f[x + 4, y + 3]; zigzag2[num++] = f[x + 5, y + 2]; zigzag2[num++] = f[x + 6, y + 1]; zigzag2[num++] = f[x + 7, y + 0];
                        zigzag2[num++] = f[x + 7, y + 1]; zigzag2[num++] = f[x + 6, y + 2]; zigzag2[num++] = f[x + 5, y + 3]; zigzag2[num++] = f[x + 4, y + 4]; zigzag2[num++] = f[x + 3, y + 5]; zigzag2[num++] = f[x + 2, y + 6];
                        zigzag2[num++] = f[x + 1, y + 7]; zigzag2[num++] = f[x + 2, y + 7]; zigzag2[num++] = f[x + 3, y + 6]; zigzag2[num++] = f[x + 4, y + 5]; zigzag2[num++] = f[x + 5, y + 4]; zigzag2[num++] = f[x + 6, y + 3];
                        zigzag2[num++] = f[x + 7, y + 2]; zigzag2[num++] = f[x + 7, y + 3]; zigzag2[num++] = f[x + 6, y + 4]; zigzag2[num++] = f[x + 5, y + 5]; zigzag2[num++] = f[x + 4, y + 6]; zigzag2[num++] = f[x + 3, y + 7];
                        zigzag2[num++] = f[x + 4, y + 7]; zigzag2[num++] = f[x + 5, y + 6]; zigzag2[num++] = f[x + 6, y + 5]; zigzag2[num++] = f[x + 7, y + 4]; zigzag2[num++] = f[x + 7, y + 5]; zigzag2[num++] = f[x + 6, y + 6];
                        zigzag2[num++] = f[x + 5, y + 7]; zigzag2[num++] = f[x + 6, y + 7]; zigzag2[num++] = f[x + 7, y + 6]; zigzag2[num++] = f[x + 7, y + 7];
                    }

                /*
                using(StreamWriter sw = new StreamWriter(@"D:\f.txt"))
                {
                    for (int i = 0; i < 32; i++)
                    {
                        for (int j = 0; j < 32; j++)
                        {
                            if (f[i, j] >= 0 && f[i, j] < 10)
                                sw.Write("   ");
                            else if (f[i, j]<=-10)
                                sw.Write(" ");
                            else
                                sw.Write("  ");
                            sw.Write(f[i, j]);
                            sw.Write(" ");
                        }
                        sw.Write(Environment.NewLine);
                    }
                }

                using (StreamWriter sw = new StreamWriter(@"D:\zigzag2.txt"))
                {
                    for (int i = 0; i < zigzag2.Length; i++)
                    {
                        sw.Write(zigzag2[i]);
                        sw.Write(Environment.NewLine);
                    }
                }
                */
                
                unsafe
                {
                    byte* p = (byte*)bnd.Scan0;

                    for (int y = 0; y < h; y++, p += offset)
                    {
                        for (int x = 0; x < w; x++, p += 3)
                        {
                            //因為pixel值域為0~255  所以要將有負號的頻域+128
                            p[0] = p[1] = p[2] = Convert.ToByte(f[x, y] + 128);
                        }
                    }
                }


                //開始或夫曼編碼
                List<huffmanNode> node = new List<huffmanNode>();

                //假設轉成頻域f的所有值落在-128~127上
                double[] count = new double[256];

                //統計出現次數，因為array index從0開始，所以先偏移128
                for (int i = 0; i < zigzag2.Length; i++)
                    count[zigzag2[i] + 128]++;

                //顯示出現次數
                for (int i = 0; i < 256; i++)
                {
                    huffmanNode n = new huffmanNode();
                    n.amount = count[i];
                    n.pixel = Convert.ToByte(i);
                    node.Add(n);
                }

                //依出現次數排序
                bubblesort(node);

                //刪除沒用到的node
                for (int i = 0; node[i].amount == 0; i++)
                {
                    if (node[i].amount == 0)
                    {
                        node.Remove(node[i--]);
                    }
                }

                //開始建huffman table
                int l = 0;
                byte z = 0;

                while (l < node.Count - 1)
                {
                    string st = "00000001";
                    int index = node.Count;

                    //二進位code前7位元不變第八位元設為1
                    //ex: 00000010 | 00000001 = 00000011
                    node[l].code = (byte)(node[l].code | Convert.ToByte(st, 2));

                    //二進位code前7位元不變第八位元設為0
                    //ex: 00000010 & 11111110 = 00000010
                    st = "11111110";
                    node[l + 1].code = (byte)(node[l + 1].code & Convert.ToByte(st, 2));

                    //建輔助編碼的節點
                    huffmanNode temp = new huffmanNode();
                    temp.amount = node[l].amount + node[l + 1].amount;

                    //輔助編碼的節點以code值右邊第2位位元設為1區別
                    st = "00000011";
                    temp.code = (byte)(temp.code | Convert.ToByte(st, 2));

                    //找新的節點依照像素出現次數插入huffman table的位置
                    for (int i = 0; i < node.Count; i++)
                    {
                        if (node[i].amount >= temp.amount)
                        {
                            index = i;
                            break;
                        }
                    }

                    //節點以parent和child相同連結
                    temp.child = z;
                    node[l].parent = z;
                    node[l + 1].parent = z;

                    //將新的節點依照像素出現次數插入huffman table
                    node.Insert(index, temp);

                    z++;
                    l += 2;
                }

                //當節點為原始像素非零的節點時，對此像素進行編碼
                for (int i = 0; i < node.Count; i++)
                {
                    if (node[i].code < 2)
                    {
                        node[i].s = encode(node, i, node[i].s);
                    }
                }


                /////這裡之後要修改  寫入檔頭資訊128位元組.....................
                bw.Write(w);
                bw.Write(h);
                byte x0 = 0;
                for (int i = 0; i < 128 - 4 * 2; i++)
                    bw.Write(x0);
                //..............................................................

                //localdata.Count最多511 -> 00000001 11111111(二進制)
                Int16 len = Convert.ToInt16(node.Count);
                bw.Write(len);

                //開始寫入huffman table
                for (int i = 0; i < node.Count; i++)
                {
                    bw.Write(node[i].pixel);
                    bw.Write(node[i].parent);
                    bw.Write(node[i].child);
                    bw.Write(node[i].code);

                }


                //開始寫入影像碼

                List<byte> b = new List<byte>();

                string sss = "";
                //從頭開始比對影像像素值並寫入對應編碼
                for (int x = 0; x < zigzag2.Length; x++)
                {
                    //從huffman table找出對應編碼
                    for (int i = 0; i < node.Count; i++)
                    {
                        //若pixel值相等且不為新增設的節點
                        if ((zigzag2[x] + 128) == node[i].pixel && node[i].code < 2)
                        {

                            sss += node[i].s;

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
                    int ff = 8 - sss.Length;
                    for (int aaa = 0; aaa < ff; aaa++)
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


                bn.UnlockBits(bnd);

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


                for (int i = 0; i < node.Count; i++)
                {
                    double d;
                    d = node[i].amount * 1.0 / (w * h);
                    rows.Add(new Object[] { i, node[i].pixel - 128, node[i].amount, d.ToString("0.000"), node[i].parent, node[i].child, node[i].code, node[i].s });

                    dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.Black;

                }

                //算平均碼長
                double meancodelength = 0;

                for (int i = 0; i < node.Count; i++)
                {
                    if (node[i].code < 2)
                    {
                        meancodelength += node[i].s.Length * node[i].amount * 1.0 / (w * h);
                    }
                }
                label2.Text = "Average Code Length  " + meancodelength.ToString("0.00") + "  bits / symbol";

                //算壓縮比
                double compression;
                FileInfo f1 = new FileInfo(sfd.FileName + ".myjpeg");
                compression = (double)(w * h + 128) / f1.Length;
                //和上面值會不同，但不知道原因
                //compression = (double)(w * h + 128) / bw.BaseStream.Length;
                label3.Text = "Compression Ratio : " + compression.ToString("0.00");

                fs.Close();
                bw.Close();
                
                MessageBox.Show("Huffman encoding has been completed.");
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

        //Decode
        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(ofd.FileName, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);
                List<huffmanNode> localdata = new List<huffmanNode>();
                int[,] ds;



                //待修改.................................
                int w = br.ReadInt32();
                int h = br.ReadInt32();

                for (int i = 0; i < 128 - 8; i++)
                    br.ReadByte();
                //........................................

                int[] izigzag = new int[w * h];
                df = new int[w, h];
                ds = new int[w, h];

                //讀取huffman table陣列的長度
                Int16 length = br.ReadInt16();

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
                /*
                dataGridView2.Columns.Add("Num", "Num");
                dataGridView2.Columns.Add("Pixel", "Pixel");
                dataGridView2.Columns.Add("Amount", "Amount");
                dataGridView2.Columns.Add("Pixel", "Probability");
                dataGridView2.Columns.Add("Parent", "Parent");
                dataGridView2.Columns.Add("Child", "Child");
                dataGridView2.Columns.Add("Code", "Code");
                dataGridView2.Columns.Add("Decode", "Decode");

                for (int i = 0; i < dataGridView2.Columns.Count - 1; i++)
                    dataGridView2.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView2.Columns[dataGridView2.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                DataGridViewRowCollection rows = dataGridView2.Rows;


                for (int i = 0; i < localdata.Count; i++)
                {
                    double d;
                    d = localdata[i].amount * 1.0 / (w * h);
                    rows.Add(new Object[] { i, localdata[i].pixel - 128, localdata[i].amount, d.ToString("0.000"), localdata[i].parent, localdata[i].child, localdata[i].code, localdata[i].s });

                    dataGridView2.Rows[i].DefaultCellStyle.ForeColor = Color.Black;

                }
                */

                //刪除中繼節點
                for (int i = localdata.Count - 1; i >= 0; i--)
                {
                    if (localdata[i].code >= 2)
                        localdata.Remove(localdata[i]);
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

                for (int i = 0; i < izigzag.Length; i++)
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
                                int pixel = localdata[j].pixel - 128;

                                izigzag[i] = pixel;

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
                br.Close();
                fs.Close();

                using (StreamWriter sw = new StreamWriter(@"D:\izigzag.txt"))
                {
                    for (int i = 0; i < izigzag.Length; i++)
                    {
                        sw.Write(izigzag[i]);
                        sw.Write(Environment.NewLine);
                    }
                }

                //inverse zig-zag
                int n = 0;
                for (int x = 0; x < w; x += 8)
                    for (int y = 0; y < h; y += 8)
                    {
                        for (int i = 0; i < 64; i++)
                            zig(izigzag[n++], i, x, y);
                    }



                //inverse dct
                double cu, cv, sum;
                int N = 8;
                for (int y = 0; y < h; y += 8)
                {
                    for (int x = 0; x < w; x += 8)
                    {
                        for (int v = 0; v < 8; v++)
                            for (int u = 0; u < 8; u++)
                            {
                                sum = 0;
                                for (int j = 0; j < N; j++)
                                    for (int i = 0; i < N; i++)
                                    {
                                        cu = (i == 0) ? Math.Sqrt(1 / (double)2) : 1;
                                        cv = (j == 0) ? Math.Sqrt(1 / (double)2) : 1;
                                        sum += cu * cv * df[(x + j), (y + i)] * q[j, i] * Math.Cos((2 * u + 1) * i * Math.PI / 16) * Math.Cos((2 * v + 1) * j * Math.PI / 16);
                                    }
                                ds[(x + v), (y + u)] = Convert.ToInt32(sum / 4) + 128;
                            }
                    }
                }


                Bitmap bb = new Bitmap(w, h);
                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                    {
                        if (ds[x, y] < 0)
                            ds[x, y] = 0;
                        if (ds[x, y] > 255)
                            ds[x, y] = 255;
                        byte pix = Convert.ToByte(ds[x, y]);
                        bb.SetPixel(x, y, Color.FromArgb(pix, pix, pix));
                    }
                pictureBox4.Image = bb;

                mf.sendimage(bb);
            }
        }
        private void zig(int p, int n, int x, int y)
        {
            switch (n)
            {
                case 0: df[x, y] = p; break;

                case 2: df[x + 1, y] = p; break;
                case 1: df[x, y + 1] = p; break;

                case 5: df[x, y + 2] = p; break;
                case 4: df[x + 1, y + 1] = p; break;
                case 3: df[x + 2, y + 0] = p; break;

                case 9: df[x + 3, y + 0] = p; break;
                case 8: df[x + 2, y + 1] = p; break;
                case 7: df[x + 1, y + 2] = p; break;
                case 6: df[x + 0, y + 3] = p; break;

                case 14: df[x + 0, y + 4] = p; break;
                case 13: df[x + 1, y + 3] = p; break;
                case 12: df[x + 2, y + 2] = p; break;
                case 11: df[x + 3, y + 1] = p; break;
                case 10: df[x + 4, y + 0] = p; break;

                case 20: df[x + 5, y + 0] = p; break;
                case 19: df[x + 4, y + 1] = p; break;
                case 18: df[x + 3, y + 2] = p; break;
                case 17: df[x + 2, y + 3] = p; break;
                case 16: df[x + 1, y + 4] = p; break;
                case 15: df[x + 0, y + 5] = p; break;

                case 27: df[x + 0, y + 6] = p; break;
                case 26: df[x + 1, y + 5] = p; break;
                case 25: df[x + 2, y + 4] = p; break;
                case 24: df[x + 3, y + 3] = p; break;
                case 23: df[x + 4, y + 2] = p; break;
                case 22: df[x + 5, y + 1] = p; break;
                case 21: df[x + 6, y + 0] = p; break;

                case 35: df[x + 7, y + 0] = p; break;
                case 34: df[x + 6, y + 1] = p; break;
                case 33: df[x + 5, y + 2] = p; break;
                case 32: df[x + 4, y + 3] = p; break;
                case 31: df[x + 3, y + 4] = p; break;
                case 30: df[x + 2, y + 5] = p; break;
                case 29: df[x + 1, y + 6] = p; break;
                case 28: df[x + 0, y + 7] = p; break;

                case 42: df[x + 1, y + 7] = p; break;
                case 41: df[x + 2, y + 6] = p; break;
                case 40: df[x + 3, y + 5] = p; break;
                case 39: df[x + 4, y + 4] = p; break;
                case 38: df[x + 5, y + 3] = p; break;
                case 37: df[x + 6, y + 2] = p; break;
                case 36: df[x + 7, y + 1] = p; break;

                case 48: df[x + 7, y + 2] = p; break;
                case 47: df[x + 6, y + 3] = p; break;
                case 46: df[x + 5, y + 4] = p; break;
                case 45: df[x + 4, y + 5] = p; break;
                case 44: df[x + 3, y + 6] = p; break;
                case 43: df[x + 2, y + 7] = p; break;

                case 53: df[x + 3, y + 7] = p; break;
                case 52: df[x + 4, y + 6] = p; break;
                case 51: df[x + 5, y + 5] = p; break;
                case 50: df[x + 6, y + 4] = p; break;
                case 49: df[x + 7, y + 3] = p; break;

                case 57: df[x + 7, y + 4] = p; break;
                case 56: df[x + 6, y + 5] = p; break;
                case 55: df[x + 5, y + 6] = p; break;
                case 54: df[x + 4, y + 7] = p; break;

                case 60: df[x + 5, y + 7] = p; break;
                case 59: df[x + 6, y + 6] = p; break;
                case 58: df[x + 7, y + 5] = p; break;

                case 62: df[x + 7, y + 6] = p; break;
                case 61: df[x + 6, y + 7] = p; break;

                case 63: df[x + 7, y + 7] = p; break;

            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = "";
            if (comboBox1.Text[0] == '1')
            {
                q = new byte[8, 8]{     { 16, 11, 10, 16, 24, 40, 51, 61},
                                        { 12, 12, 14, 19, 26, 58, 60, 55},
                                        { 14, 13, 16, 24, 40, 57, 69, 56},
                                        { 14, 17, 22, 29, 51, 87, 80, 82},
                                        { 18, 22, 37, 56, 68, 109, 103, 77},
                                        { 24, 35, 55, 64, 81, 104, 113, 92},
                                        { 99, 64, 78, 87, 103, 121, 120, 101},
                                        { 72, 92, 95, 98, 112, 100, 103, 99}};

                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (q[i, j] < 100)
                            textBox1.Text += "  " + q[i, j] + "   ";
                        else
                            textBox1.Text += q[i, j] + "   ";
                    }
                    textBox1.Text += Environment.NewLine;
                    textBox1.Text += Environment.NewLine;
                }

            }
            else if (comboBox1.Text[0] == '2')
            {
                q = new byte[8, 8]{     { 10, 15, 20, 25, 30, 35, 40, 45},
                                        { 15, 20, 25, 30, 35, 40, 45, 50},
                                        { 20, 25, 30, 35, 40, 45, 50, 55},
                                        { 25, 30, 35, 40, 45, 50, 55, 60},
                                        { 30, 25, 40, 45, 50, 55, 60, 65},
                                        { 35, 30, 45, 50, 55, 60, 65, 70},
                                        { 40, 35, 50, 55, 60, 65, 70, 75},
                                        { 45, 50, 55, 60, 65, 70, 75, 80}};

                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (q[i, j] < 100)
                            textBox1.Text += "  " + q[i, j] + "   ";
                        else
                            textBox1.Text += q[i, j] + "   ";
                    }
                    textBox1.Text += Environment.NewLine;
                    textBox1.Text += Environment.NewLine;
                }
            }
            else if (comboBox1.Text[0] == '3')
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        q[i, j] = 10;
                    }
                }
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (q[i, j] < 100)
                            textBox1.Text += "  " + q[i, j] + "   ";
                        else
                            textBox1.Text += q[i, j] + "   ";
                    }
                    textBox1.Text += Environment.NewLine;
                    textBox1.Text += Environment.NewLine;
                }
            }
            else if (comboBox1.Text[0] == '4')
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        q[i, j] = 50;
                    }
                }
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (q[i, j] < 100)
                            textBox1.Text += "  " + q[i, j] + "   ";
                        else
                            textBox1.Text += q[i, j] + "   ";
                    }
                    textBox1.Text += Environment.NewLine;
                    textBox1.Text += Environment.NewLine;
                }
            }
            else if (comboBox1.Text[0] == '5')
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        q[i, j] = 100;
                    }
                }
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (q[i, j] < 100)
                            textBox1.Text += "  " + q[i, j] + "   ";
                        else
                            textBox1.Text += q[i, j] + "   ";
                    }
                    textBox1.Text += Environment.NewLine;
                    textBox1.Text += Environment.NewLine;
                }
            }
            else if (comboBox1.Text[0] == '6')
            {
                byte[,] q2 = new byte[8, 8]{     { 10, 15, 20, 25, 30, 35, 40, 45},
                                                 { 15, 20, 25, 30, 35, 40, 45, 50},
                                                 { 20, 25, 30, 35, 40, 45, 50, 55},
                                                 { 25, 30, 35, 40, 45, 50, 55, 60},
                                                 { 30, 25, 40, 45, 50, 55, 60, 65},
                                                 { 35, 30, 45, 50, 55, 60, 65, 70},
                                                 { 40, 35, 50, 55, 60, 65, 70, 75},
                                                 { 45, 50, 55, 60, 65, 70, 75, 80}};

                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        q[i, j] = q2[7 - i, 7 - j];
                    }
                }
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (q[i, j] < 100)
                            textBox1.Text += "  " + q[i, j] + "   ";
                        else
                            textBox1.Text += q[i, j] + "   ";
                    }
                    textBox1.Text += Environment.NewLine;
                    textBox1.Text += Environment.NewLine;
                }
            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
            mf.sendimage(pictureBox2.Image as Bitmap);
        }

        //DCT + quantify
        private void button9_Click(object sender, EventArgs e)
        {
            Bitmap bn = (Bitmap)pictureBox1.Image.Clone();
            BitmapData bnd = bn.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int offset = bnd.Stride - bn.Width * 3;
            double cu, cv;
            double sum;
            int N = 8;
            s = new int[w, h];
            f = new int[w, h];

            unsafe
            {
                byte* p = (byte*)bnd.Scan0;

                for (int y = 0; y < h; y++, p += offset)
                {
                    for (int x = 0; x < w; x++, p += 3)
                    {
                        s[x, y] = p[0] - 128;
                    }
                }
            }

            //DCT
            for (int y = 0; y < h; y += 8)
            {
                for (int x = 0; x < w; x += 8)
                {
                    for (int v = 0; v < 8; v++)
                        for (int u = 0; u < 8; u++)
                        {
                            cu = (u == 0) ? Math.Sqrt(1 / (double)2) : 1;
                            cv = (v == 0) ? Math.Sqrt(1 / (double)2) : 1;
                            sum = 0;
                            for (int j = 0; j < N; j++)
                                for (int i = 0; i < N; i++)
                                {
                                    sum += s[(x + j), (y + i)] * Math.Cos((2 * i + 1) * u * Math.PI / 16) * Math.Cos((2 * j + 1) * v * Math.PI / 16);
                                }
                            f[(x + v), (y + u)] = Convert.ToInt32(Math.Round(cu * cv * sum / 4)/q[v, u]);
                        }
                }
            }

            unsafe
            {
                byte* p = (byte*)bnd.Scan0;

                for (int y = 0; y < h; y++, p += offset)
                {
                    for (int x = 0; x < w; x++, p += 3)
                    {
                        p[0] = p[1] = p[2] = Convert.ToByte(f[x, y] + 128);
                    }
                }
            }
            bn.UnlockBits(bnd);
            pictureBox2.Image = bn;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //inverse dct
            double cu, cv, sum;
            int N = 8;
            for (int y = 0; y < h; y += 8)
            {
                for (int x = 0; x < w; x += 8)
                {
                    for (int v = 0; v < 8; v++)
                        for (int u = 0; u < 8; u++)
                        {
                            sum = 0;
                            for (int j = 0; j < N; j++)
                                for (int i = 0; i < N; i++)
                                {
                                    cu = (i == 0) ? Math.Sqrt(1 / (double)2) : 1;
                                    cv = (j == 0) ? Math.Sqrt(1 / (double)2) : 1;
                                    sum += cu * cv * f[(x + j), (y + i)] * Math.Cos((2 * u + 1) * i * Math.PI / 16) * Math.Cos((2 * v + 1) * j * Math.PI / 16);
                                }
                            s[(x + v), (y + u)] = Convert.ToInt32(sum / 4) + 128;
                        }
                }
            }

            Bitmap bb = new Bitmap(w, h);
            List<Point> pt = new List<Point>();
            textBox2.Text = "";

            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    if (s[x, y] < 0)
                    {
                        textBox2.Text += "(" + x + "," + y + ")" + "  " + s[x, y] + Environment.NewLine;
                        pt.Add(new Point(x, y));
                        s[x, y] = 0;
                    }
                    if (s[x, y] > 255)
                    {
                        textBox2.Text += "(" + x + "," + y + ")" + "  " + s[x, y] + Environment.NewLine;
                        pt.Add(new Point(x, y));
                        s[x, y] = 255;
                    }
                    byte pix = Convert.ToByte(s[x, y]);
                    bb.SetPixel(x, y, Color.FromArgb(pix, pix, pix));
                }
            
            Graphics g = Graphics.FromImage(bb);
            for (int i = 0; i < pt.Count; i++)
                g.DrawEllipse(Pens.Blue, pt[i].X - 2, pt[i].Y + 2, 5, 5);
            
            pictureBox3.Image = bb;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            mf.sendimage(pictureBox3.Image as Bitmap);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Bitmap bn = (Bitmap)pictureBox1.Image.Clone();
            BitmapData bnd = bn.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int offset = bnd.Stride - bn.Width * 3;
            double cu, cv;
            double sum;
            int N = 8;
            s = new int[w, h];
            f = new int[w, h];

            unsafe
            {
                byte* p = (byte*)bnd.Scan0;

                for (int y = 0; y < h; y++, p += offset)
                {
                    for (int x = 0; x < w; x++, p += 3)
                    {
                        s[x, y] = p[0] - 128;
                    }
                }
            }

            //DCT
            for (int y = 0; y < h; y += 8)
            {
                for (int x = 0; x < w; x += 8)
                {
                    for (int v = 0; v < 8; v++)
                        for (int u = 0; u < 8; u++)
                        {
                            cu = (u == 0) ? Math.Sqrt(1 / (double)2) : 1;
                            cv = (v == 0) ? Math.Sqrt(1 / (double)2) : 1;
                            sum = 0;
                            for (int j = 0; j < N; j++)
                                for (int i = 0; i < N; i++)
                                {
                                    sum += s[(x + j), (y + i)] * Math.Cos((2 * i + 1) * u * Math.PI / 16) * Math.Cos((2 * j + 1) * v * Math.PI / 16);
                                }
                            f[(x + v), (y + u)] = Convert.ToInt32(Math.Round(cu * cv * sum / 4.0));
                        }
                }
            }

            int max = int.MinValue;
            int min = int.MaxValue;
            unsafe
            {
                byte* p = (byte*)bnd.Scan0;

                for (int y = 0; y < h; y++, p += offset)
                {
                    for (int x = 0; x < w; x++, p += 3)
                    {
                        p[0] = p[1] = p[2] = 0;
                        max = (max < f[x, y]) ? f[x, y] : max;
                        min = (min > f[x, y]) ? f[x, y] : min;
                        string st1 = "000000000000";
                        string st2 = "000000000000";
                        if (f[x, y] >= 0)
                            st1 = Convert.ToString(f[x, y], 2).PadLeft(12, '0');
                        else if (f[x, y] < 0)
                        {
                            st2 = Convert.ToString(Math.Abs(f[x, y]), 2);
                            char[] c = st2.ToCharArray();
                            Array.Reverse(c);
                            st2 = new string(c);
                            st2 = st2.PadRight(12, '0');
                        }
                        st1 = st1 + st2;
                        string sub = st1.Substring(0, 8);
                        p[0] = Convert.ToByte(Convert.ToInt32(sub, 2));
                        sub = st1.Substring(8, 8);
                        p[1] = Convert.ToByte(Convert.ToInt32(sub, 2));
                        sub = st1.Substring(16, 8);
                        p[2] = Convert.ToByte(Convert.ToInt32(sub, 2));
                    }
                }

            }
            bn.UnlockBits(bnd);
            label2.Text = "Max pixel: " + max + "    Min pixel: " + min;
            pictureBox2.Image = bn;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            pictureBox4.Image = null;
        }
    }

}