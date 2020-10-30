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
    public partial class ScalingForm : Form
    {
        MainForm m = null;
        string s;
        public ScalingForm(MainForm main, string type)
        {
            InitializeComponent();
            m = main;
            s = type;
            m.scalingValue = float.MaxValue;
            this.Text = type.ToUpper();
            if (type[0] == 'r')
            {
                label1.Text = "Please input the " + type + " :";
                label2.Text = "degrees";
            }
            else
            {
                label1.Text = "Please input the scaling of " + type + " :";
                label2.Text = "%";
            }
            label1.Left = (this.ClientSize.Width - label1.Width) / 2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1();   
        }
        private void button1()
        {
            int i;

            if (int.TryParse(textBox1.Text, out i))
            {
                if (s[0] == 's')
                {
                    if (i > 0 && i <= 100)
                    {
                        m.scalingValue = i;
                        this.Close();
                    }
                    else
                    {
                        textBox1.SelectionStart = 0;
                        textBox1.SelectionLength = textBox1.TextLength;
                        textBox1.Select();
                        MessageBox.Show("Please input a value 0 < x <= 100.");
                    }
                }
                else if (s[0] == 'e')
                {
                    if (i >= 100)
                    {
                        m.scalingValue = i;
                        this.Close();
                    }
                    else
                    {
                        textBox1.SelectionStart = 0;
                        textBox1.SelectionLength = textBox1.TextLength;
                        textBox1.Select();
                        MessageBox.Show("Please input a value x >= 100.");
                    }
                }
                else if (s[0] == 'r')
                {

                    m.scalingValue = i;
                    this.Close();
                }
            }
            else
            {
                textBox1.SelectionStart = 0;
                textBox1.SelectionLength = textBox1.TextLength;
                textBox1.Select();
                MessageBox.Show("Please input a  number.");
            }
        }

        private void ScalingForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button1();
        }

    }
}
