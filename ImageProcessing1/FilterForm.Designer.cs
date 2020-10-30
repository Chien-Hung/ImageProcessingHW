namespace ImageProcessing
{
    partial class FilterForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilterForm));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.outlierToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripSplitButton();
            this.boxFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.weightedAverageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripSplitButton();
            this.squareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.crossToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripSplitButton();
            this.basicSpaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.edgeCrispeningToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mark1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mark2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mark3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.highboostFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.derivativeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSplitButton2 = new System.Windows.Forms.ToolStripSplitButton();
            this.sobelGradientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sobelXgradientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sobelYgradientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSplitButton3 = new System.Windows.Forms.ToolStripSplitButton();
            this.prewittGradientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.prewittToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.prewittYgradientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.SNRLabel = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(9, 216);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(283, 273);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(336, 216);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(283, 273);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.outlierToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(965, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // outlierToolStripMenuItem
            // 
            this.outlierToolStripMenuItem.Name = "outlierToolStripMenuItem";
            this.outlierToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.outlierToolStripMenuItem.Text = "Outlier";
            this.outlierToolStripMenuItem.Click += new System.EventHandler(this.outlierToolStripMenuItem_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("新細明體", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(447, 175);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 19);
            this.label2.TabIndex = 4;
            this.label2.Text = "label2";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSplitButton1,
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripButton4,
            this.toolStripSplitButton2,
            this.toolStripSplitButton3});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(965, 25);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.boxFilterToolStripMenuItem,
            this.weightedAverageToolStripMenuItem});
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            this.toolStripSplitButton1.Size = new System.Drawing.Size(72, 22);
            this.toolStripSplitButton1.Text = "Lowpass";
            // 
            // boxFilterToolStripMenuItem
            // 
            this.boxFilterToolStripMenuItem.Name = "boxFilterToolStripMenuItem";
            this.boxFilterToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.boxFilterToolStripMenuItem.Text = "Box Filter";
            this.boxFilterToolStripMenuItem.Click += new System.EventHandler(this.boxFilterToolStripMenuItem_Click);
            // 
            // weightedAverageToolStripMenuItem
            // 
            this.weightedAverageToolStripMenuItem.Name = "weightedAverageToolStripMenuItem";
            this.weightedAverageToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.weightedAverageToolStripMenuItem.Text = "Weighted Average";
            this.weightedAverageToolStripMenuItem.Click += new System.EventHandler(this.weightedAverageToolStripMenuItem_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(50, 22);
            this.toolStripButton1.Text = "Outlier";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.squareToolStripMenuItem,
            this.crossToolStripMenuItem});
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(68, 22);
            this.toolStripButton2.Text = "Median";
            // 
            // squareToolStripMenuItem
            // 
            this.squareToolStripMenuItem.Name = "squareToolStripMenuItem";
            this.squareToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.squareToolStripMenuItem.Text = "Square";
            this.squareToolStripMenuItem.Click += new System.EventHandler(this.squareToolStripMenuItem_Click);
            // 
            // crossToolStripMenuItem
            // 
            this.crossToolStripMenuItem.Name = "crossToolStripMenuItem";
            this.crossToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.crossToolStripMenuItem.Text = "Cross";
            this.crossToolStripMenuItem.Click += new System.EventHandler(this.crossToolStripMenuItem_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.basicSpaceToolStripMenuItem,
            this.edgeCrispeningToolStripMenuItem,
            this.highboostFilterToolStripMenuItem,
            this.derivativeToolStripMenuItem});
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(89, 22);
            this.toolStripButton3.Text = "Sharpening";
            // 
            // basicSpaceToolStripMenuItem
            // 
            this.basicSpaceToolStripMenuItem.Name = "basicSpaceToolStripMenuItem";
            this.basicSpaceToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.basicSpaceToolStripMenuItem.Text = "Basic Highpass Spatial Filtering";
            this.basicSpaceToolStripMenuItem.Click += new System.EventHandler(this.basicSpaceToolStripMenuItem_Click);
            // 
            // edgeCrispeningToolStripMenuItem
            // 
            this.edgeCrispeningToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mark1ToolStripMenuItem,
            this.mark2ToolStripMenuItem,
            this.mark3ToolStripMenuItem});
            this.edgeCrispeningToolStripMenuItem.Name = "edgeCrispeningToolStripMenuItem";
            this.edgeCrispeningToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.edgeCrispeningToolStripMenuItem.Text = "Edge Crispening";
            // 
            // mark1ToolStripMenuItem
            // 
            this.mark1ToolStripMenuItem.Name = "mark1ToolStripMenuItem";
            this.mark1ToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.mark1ToolStripMenuItem.Text = "Mark1";
            this.mark1ToolStripMenuItem.Click += new System.EventHandler(this.mark1ToolStripMenuItem_Click);
            // 
            // mark2ToolStripMenuItem
            // 
            this.mark2ToolStripMenuItem.Name = "mark2ToolStripMenuItem";
            this.mark2ToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.mark2ToolStripMenuItem.Text = "Mark2";
            this.mark2ToolStripMenuItem.Click += new System.EventHandler(this.mark2ToolStripMenuItem_Click);
            // 
            // mark3ToolStripMenuItem
            // 
            this.mark3ToolStripMenuItem.Name = "mark3ToolStripMenuItem";
            this.mark3ToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.mark3ToolStripMenuItem.Text = "Mark3";
            this.mark3ToolStripMenuItem.Click += new System.EventHandler(this.mark3ToolStripMenuItem_Click);
            // 
            // highboostFilterToolStripMenuItem
            // 
            this.highboostFilterToolStripMenuItem.Name = "highboostFilterToolStripMenuItem";
            this.highboostFilterToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.highboostFilterToolStripMenuItem.Text = "High-boost Filter";
            this.highboostFilterToolStripMenuItem.Click += new System.EventHandler(this.highboostFilterToolStripMenuItem_Click);
            // 
            // derivativeToolStripMenuItem
            // 
            this.derivativeToolStripMenuItem.Name = "derivativeToolStripMenuItem";
            this.derivativeToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.derivativeToolStripMenuItem.Text = "Derivative Filters";
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(37, 22);
            this.toolStripButton4.Text = "SNR";
            this.toolStripButton4.Click += new System.EventHandler(this.toolStripButton4_Click);
            // 
            // toolStripSplitButton2
            // 
            this.toolStripSplitButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSplitButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sobelGradientToolStripMenuItem,
            this.sobelXgradientToolStripMenuItem,
            this.sobelYgradientToolStripMenuItem});
            this.toolStripSplitButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton2.Image")));
            this.toolStripSplitButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton2.Name = "toolStripSplitButton2";
            this.toolStripSplitButton2.Size = new System.Drawing.Size(57, 22);
            this.toolStripSplitButton2.Text = "Sobel";
            // 
            // sobelGradientToolStripMenuItem
            // 
            this.sobelGradientToolStripMenuItem.Name = "sobelGradientToolStripMenuItem";
            this.sobelGradientToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.sobelGradientToolStripMenuItem.Text = "sobel gradient";
            this.sobelGradientToolStripMenuItem.Click += new System.EventHandler(this.sobelGradientToolStripMenuItem_Click);
            // 
            // sobelXgradientToolStripMenuItem
            // 
            this.sobelXgradientToolStripMenuItem.Name = "sobelXgradientToolStripMenuItem";
            this.sobelXgradientToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.sobelXgradientToolStripMenuItem.Text = "sobel x-gradient ";
            this.sobelXgradientToolStripMenuItem.Click += new System.EventHandler(this.sobelXgradientToolStripMenuItem_Click);
            // 
            // sobelYgradientToolStripMenuItem
            // 
            this.sobelYgradientToolStripMenuItem.Name = "sobelYgradientToolStripMenuItem";
            this.sobelYgradientToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.sobelYgradientToolStripMenuItem.Text = "sobel y-gradient ";
            this.sobelYgradientToolStripMenuItem.Click += new System.EventHandler(this.sobelYgradientToolStripMenuItem_Click);
            // 
            // toolStripSplitButton3
            // 
            this.toolStripSplitButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSplitButton3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.prewittGradientToolStripMenuItem,
            this.prewittToolStripMenuItem,
            this.prewittYgradientToolStripMenuItem});
            this.toolStripSplitButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton3.Image")));
            this.toolStripSplitButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton3.Name = "toolStripSplitButton3";
            this.toolStripSplitButton3.Size = new System.Drawing.Size(62, 22);
            this.toolStripSplitButton3.Text = "Prewitt";
            // 
            // prewittGradientToolStripMenuItem
            // 
            this.prewittGradientToolStripMenuItem.Name = "prewittGradientToolStripMenuItem";
            this.prewittGradientToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.prewittGradientToolStripMenuItem.Text = "Prewitt gradient";
            this.prewittGradientToolStripMenuItem.Click += new System.EventHandler(this.prewittGradientToolStripMenuItem_Click);
            // 
            // prewittToolStripMenuItem
            // 
            this.prewittToolStripMenuItem.Name = "prewittToolStripMenuItem";
            this.prewittToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.prewittToolStripMenuItem.Text = "Prewitt x-gradient";
            this.prewittToolStripMenuItem.Click += new System.EventHandler(this.prewittToolStripMenuItem_Click);
            // 
            // prewittYgradientToolStripMenuItem
            // 
            this.prewittYgradientToolStripMenuItem.Name = "prewittYgradientToolStripMenuItem";
            this.prewittYgradientToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.prewittYgradientToolStripMenuItem.Text = "Prewitt y-gradient";
            this.prewittYgradientToolStripMenuItem.Click += new System.EventHandler(this.prewittYgradientToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("新細明體", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(93, 175);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 19);
            this.label1.TabIndex = 6;
            this.label1.Text = "label1";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SNRLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 548);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.statusStrip1.Size = new System.Drawing.Size(965, 22);
            this.statusStrip1.TabIndex = 10;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // SNRLabel
            // 
            this.SNRLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.SNRLabel.Name = "SNRLabel";
            this.SNRLabel.Size = new System.Drawing.Size(129, 17);
            this.SNRLabel.Text = "toolStripStatusLabel1";
            // 
            // FilterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(965, 570);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FilterForm";
            this.Text = "FilterForm";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem outlierToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton1;
        private System.Windows.Forms.ToolStripMenuItem boxFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem weightedAverageToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripSplitButton toolStripButton2;
        private System.Windows.Forms.ToolStripMenuItem squareToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem crossToolStripMenuItem;
        private System.Windows.Forms.ToolStripSplitButton toolStripButton3;
        private System.Windows.Forms.ToolStripMenuItem basicSpaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem edgeCrispeningToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem highboostFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem derivativeToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel SNRLabel;
        private System.Windows.Forms.ToolStripMenuItem mark1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mark2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mark3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton2;
        private System.Windows.Forms.ToolStripMenuItem sobelGradientToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sobelXgradientToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sobelYgradientToolStripMenuItem;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton3;
        private System.Windows.Forms.ToolStripMenuItem prewittGradientToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem prewittToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem prewittYgradientToolStripMenuItem;
    }
}