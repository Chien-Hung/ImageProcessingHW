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
    public partial class AskForm : Form
    {
        MainForm mf;
        public AskForm(MainForm m,int type)
        {
            InitializeComponent();
            mf = m;
            switch (type)
            {
                case 0:
                    label1.Text = "Enter the numbers of bits used to describe each pixel :";
                    break;
                case 1:
                    label1.Text = "Enter the numbers of sub-sampling frame :";
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                mf.coarsenum = Convert.ToInt32(textBox1.Text);
                this.Close();
            }
        }

        private void AskForm_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
        }
    }
}
