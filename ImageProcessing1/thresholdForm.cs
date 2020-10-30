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
    public partial class thresholdForm : Form
    {
        FilterForm f = null;
        CompressForm c = null;
        public thresholdForm(FilterForm ff,int type)
        {
            InitializeComponent();
            switch (type)
            {
                case 1:
                    label1.Text = "Please input the threshold :";
                    break;
                case 2:
                    label1.Text = "Please input the amplification factor :";
                    break;
            }
            button1.Text = "Enter";
            f = ff;
        }

        public thresholdForm(CompressForm cc)
        {
            InitializeComponent();
            c = cc;
            button1.Text = "Enter";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && c == null) 
            {
                f.threshold = Convert.ToDouble(textBox1.Text);
                this.Close();
            }
            else if (textBox1.Text != "" && f == null)
            {
                c.thresholdforPDC = Convert.ToInt32(textBox1.Text);
                this.Close();
            }
        }

    }
}
