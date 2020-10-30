namespace ImageProcessing
{
    partial class HeaderImformationForm
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
            this.HItextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.platte = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.platte)).BeginInit();
            this.SuspendLayout();
            // 
            // HItextBox
            // 
            this.HItextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.HItextBox.Enabled = false;
            this.HItextBox.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.HItextBox.Location = new System.Drawing.Point(9, 37);
            this.HItextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.HItextBox.Multiline = true;
            this.HItextBox.Name = "HItextBox";
            this.HItextBox.ReadOnly = true;
            this.HItextBox.Size = new System.Drawing.Size(351, 288);
            this.HItextBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("新細明體", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(9, 7);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 27);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // platte
            // 
            this.platte.Location = new System.Drawing.Point(9, 360);
            this.platte.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.platte.Name = "platte";
            this.platte.Size = new System.Drawing.Size(192, 205);
            this.platte.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.platte.TabIndex = 2;
            this.platte.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("新細明體", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(9, 327);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(146, 27);
            this.label2.TabIndex = 3;
            this.label2.Text = "Platte Color";
            // 
            // HeaderImformationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(376, 583);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.platte);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.HItextBox);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "HeaderImformationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HeaderImformationForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HeaderImformationForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.platte)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox HItextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox platte;
        private System.Windows.Forms.Label label2;
    }
}