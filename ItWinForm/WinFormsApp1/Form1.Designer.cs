
namespace WinFormsApp1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.startButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.infoRichTextBox = new System.Windows.Forms.RichTextBox();
            this.scriptTextBox = new System.Windows.Forms.TextBox();
            this.stopButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.searchProdTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buyMoneyTextBox = new System.Windows.Forms.TextBox();
            this.clearCacheButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(24, 22);
            this.startButton.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(136, 42);
            this.startButton.TabIndex = 0;
            this.startButton.Text = "启动";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.EnabledChanged += new System.EventHandler(this.button1_EnabledChanged);
            this.startButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.infoRichTextBox);
            this.groupBox1.Location = new System.Drawing.Point(24, 452);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.groupBox1.Size = new System.Drawing.Size(2448, 620);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "信息";
            // 
            // infoRichTextBox
            // 
            this.infoRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoRichTextBox.Location = new System.Drawing.Point(6, 36);
            this.infoRichTextBox.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.infoRichTextBox.Name = "infoRichTextBox";
            this.infoRichTextBox.Size = new System.Drawing.Size(2436, 579);
            this.infoRichTextBox.TabIndex = 0;
            this.infoRichTextBox.Text = "";
            // 
            // scriptTextBox
            // 
            this.scriptTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scriptTextBox.Location = new System.Drawing.Point(12, 40);
            this.scriptTextBox.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.scriptTextBox.Multiline = true;
            this.scriptTextBox.Name = "scriptTextBox";
            this.scriptTextBox.Size = new System.Drawing.Size(2420, 288);
            this.scriptTextBox.TabIndex = 2;
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(172, 22);
            this.stopButton.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(150, 42);
            this.stopButton.TabIndex = 3;
            this.stopButton.Text = "停止";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.scriptTextBox);
            this.groupBox2.Location = new System.Drawing.Point(24, 97);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.groupBox2.Size = new System.Drawing.Size(2448, 361);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "脚本";
            // 
            // searchProdTextBox
            // 
            this.searchProdTextBox.Location = new System.Drawing.Point(456, 27);
            this.searchProdTextBox.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.searchProdTextBox.Name = "searchProdTextBox";
            this.searchProdTextBox.Size = new System.Drawing.Size(1236, 38);
            this.searchProdTextBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(358, 33);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 31);
            this.label1.TabIndex = 6;
            this.label1.Text = "商品";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1772, 38);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 31);
            this.label2.TabIndex = 6;
            this.label2.Text = "金额";
            // 
            // buyMoneyTextBox
            // 
            this.buyMoneyTextBox.Location = new System.Drawing.Point(1848, 27);
            this.buyMoneyTextBox.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.buyMoneyTextBox.Name = "buyMoneyTextBox";
            this.buyMoneyTextBox.Size = new System.Drawing.Size(196, 38);
            this.buyMoneyTextBox.TabIndex = 1;
            this.buyMoneyTextBox.Text = "10000";
            this.buyMoneyTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBox3_KeyUp);
            // 
            // clearCacheButton
            // 
            this.clearCacheButton.Location = new System.Drawing.Point(2168, 27);
            this.clearCacheButton.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.clearCacheButton.Name = "clearCacheButton";
            this.clearCacheButton.Size = new System.Drawing.Size(150, 42);
            this.clearCacheButton.TabIndex = 7;
            this.clearCacheButton.Text = "清除缓存";
            this.clearCacheButton.UseVisualStyleBackColor = true;
            this.clearCacheButton.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2496, 1094);
            this.Controls.Add(this.clearCacheButton);
            this.Controls.Add(this.buyMoneyTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.searchProdTextBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox infoRichTextBox;
        private System.Windows.Forms.TextBox scriptTextBox;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox searchProdTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox buyMoneyTextBox;
        private System.Windows.Forms.Button clearCacheButton;
    }
}

