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
    public partial class StartPicture : Form
    {
        public StartPicture()
        {
            InitializeComponent();
            StartPictureTimer.Interval = 2500;
        }

        private void StartPictureTimer_Tick(object sender, EventArgs e)
        {
            StartPictureTimer.Enabled = false;          //關閉計時器
            MainForm mainForm = new MainForm();         //建立main form物件   
            this.Hide();                                //隱藏start picture form
            mainForm.Show();                            //main form物件顯示
        }
    }
}
