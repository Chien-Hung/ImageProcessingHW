using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageProcessing
{
    public partial class HeaderImformationForm : Form
    {
        //判斷檔頭資訊是否存入用
        public bool create;

        public HeaderImformationForm()
        {
            InitializeComponent();
            create = false;
        }

        //設定TextBox內的字串
        public void addToTextBox(string s)
        {
            HItextBox.Text = HItextBox.Text + s;
        }
        
        public void change(string s)
        {
            label1.Text = "" + s;
        }

        public void setImage(Bitmap bmp)
        {
            Bitmap scalebmp = new Bitmap(256,256);
            for (int i = 0; i < 16; i++)
                for (int j = 0; j < 16; j++)
                    for (int i2 = 0; i2 < 16; i2++)
                        for (int j2 = 0; j2 < 16; j2++)
                            scalebmp.SetPixel(i * 16 + i2, j * 16 + j2, bmp.GetPixel(i, j));
            platte.Image = scalebmp;
            create = true;
        }

        private void HeaderImformationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
    }
}
