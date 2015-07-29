namespace DF_BlobViewer.cs
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.StartBtn = new System.Windows.Forms.Button();
            this.Stop = new System.Windows.Forms.Button();
            this.sourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.modeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Live = new System.Windows.Forms.ToolStripMenuItem();
            this.Playback = new System.Windows.Forms.ToolStripMenuItem();
            this.Panel2 = new System.Windows.Forms.PictureBox();
            this.fpsLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.blobSmooth = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ContourSmooth = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtMaxDepth = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtMaxBlobs = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.AccessByDirection = new System.Windows.Forms.RadioButton();
            this.AccessByDistance = new System.Windows.Forms.RadioButton();
            this.AccessBySize = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.BlobDataPoints = new System.Windows.Forms.CheckBox();
            this.Blob = new System.Windows.Forms.CheckBox();
            this.Contour = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.Status2 = new System.Windows.Forms.StatusStrip();
            this.labelFPS = new System.Windows.Forms.TextBox();
            this.MainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Panel2)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.Status2.SuspendLayout();
            this.SuspendLayout();
            // 
            // StartBtn
            // 
            this.StartBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.StartBtn.Location = new System.Drawing.Point(1054, 771);
            this.StartBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.StartBtn.Name = "StartBtn";
            this.StartBtn.Size = new System.Drawing.Size(120, 35);
            this.StartBtn.TabIndex = 2;
            this.StartBtn.Text = "Start";
            this.StartBtn.UseVisualStyleBackColor = true;
            this.StartBtn.Click += new System.EventHandler(this.Start_Click);
            // 
            // Stop
            // 
            this.Stop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Stop.Enabled = false;
            this.Stop.Location = new System.Drawing.Point(1184, 771);
            this.Stop.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Stop.Name = "Stop";
            this.Stop.Size = new System.Drawing.Size(120, 35);
            this.Stop.TabIndex = 3;
            this.Stop.Text = "Stop";
            this.Stop.UseVisualStyleBackColor = true;
            this.Stop.Click += new System.EventHandler(this.Stop_Click);
            // 
            // sourceToolStripMenuItem
            // 
            this.sourceToolStripMenuItem.Name = "sourceToolStripMenuItem";
            this.sourceToolStripMenuItem.Size = new System.Drawing.Size(76, 29);
            this.sourceToolStripMenuItem.Text = "Device";
            // 
            // MainMenu
            // 
            this.MainMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sourceToolStripMenuItem,
            this.modeToolStripMenuItem});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
            this.MainMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.MainMenu.Size = new System.Drawing.Size(1395, 35);
            this.MainMenu.TabIndex = 0;
            this.MainMenu.Text = "MainMenu";
            // 
            // modeToolStripMenuItem
            // 
            this.modeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Live,
            this.Playback});
            this.modeToolStripMenuItem.Name = "modeToolStripMenuItem";
            this.modeToolStripMenuItem.Size = new System.Drawing.Size(71, 29);
            this.modeToolStripMenuItem.Text = "Mode";
            // 
            // Live
            // 
            this.Live.Checked = true;
            this.Live.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Live.Name = "Live";
            this.Live.Size = new System.Drawing.Size(166, 30);
            this.Live.Text = "Live";
            this.Live.Click += new System.EventHandler(this.Live_Click);
            // 
            // Playback
            // 
            this.Playback.Name = "Playback";
            this.Playback.Size = new System.Drawing.Size(166, 30);
            this.Playback.Text = "Playback";
            this.Playback.Click += new System.EventHandler(this.Playback_Click);
            // 
            // Panel2
            // 
            this.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.Panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Panel2.ErrorImage = null;
            this.Panel2.InitialImage = null;
            this.Panel2.Location = new System.Drawing.Point(18, 42);
            this.Panel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Panel2.Name = "Panel2";
            this.Panel2.Size = new System.Drawing.Size(958, 736);
            this.Panel2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Panel2.TabIndex = 27;
            this.Panel2.TabStop = false;
            // 
            // fpsLabel
            // 
            this.fpsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fpsLabel.AutoSize = true;
            this.fpsLabel.Location = new System.Drawing.Point(1264, 557);
            this.fpsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.fpsLabel.Name = "fpsLabel";
            this.fpsLabel.Size = new System.Drawing.Size(0, 20);
            this.fpsLabel.TabIndex = 31;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 231);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(163, 20);
            this.label1.TabIndex = 32;
            this.label1.Text = "Blob Smoothing (0-1):";
            // 
            // blobSmooth
            // 
            this.blobSmooth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.blobSmooth.Location = new System.Drawing.Point(236, 220);
            this.blobSmooth.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.blobSmooth.Name = "blobSmooth";
            this.blobSmooth.Size = new System.Drawing.Size(98, 26);
            this.blobSmooth.TabIndex = 33;
            this.blobSmooth.Text = "1";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 31);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(188, 20);
            this.label2.TabIndex = 35;
            this.label2.Text = "Contour Smoothing (0-1):";
            // 
            // ContourSmooth
            // 
            this.ContourSmooth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ContourSmooth.Location = new System.Drawing.Point(236, 26);
            this.ContourSmooth.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ContourSmooth.Name = "ContourSmooth";
            this.ContourSmooth.Size = new System.Drawing.Size(98, 26);
            this.ContourSmooth.TabIndex = 36;
            this.ContourSmooth.Text = "1";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 271);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 20);
            this.label3.TabIndex = 37;
            this.label3.Text = "Max Depth:";
            // 
            // txtMaxDepth
            // 
            this.txtMaxDepth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMaxDepth.Location = new System.Drawing.Point(236, 260);
            this.txtMaxDepth.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtMaxDepth.Name = "txtMaxDepth";
            this.txtMaxDepth.Size = new System.Drawing.Size(98, 26);
            this.txtMaxDepth.TabIndex = 38;
            this.txtMaxDepth.Text = "550";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 311);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(123, 20);
            this.label4.TabIndex = 39;
            this.label4.Text = "Max Blobs (1-4):";
            // 
            // txtMaxBlobs
            // 
            this.txtMaxBlobs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMaxBlobs.Location = new System.Drawing.Point(236, 300);
            this.txtMaxBlobs.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtMaxBlobs.Name = "txtMaxBlobs";
            this.txtMaxBlobs.Size = new System.Drawing.Size(102, 26);
            this.txtMaxBlobs.TabIndex = 40;
            this.txtMaxBlobs.Text = "1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.ContourSmooth);
            this.groupBox1.Location = new System.Drawing.Point(996, 482);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(368, 71);
            this.groupBox1.TabIndex = 41;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Contour Data";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.AccessByDirection);
            this.groupBox2.Controls.Add(this.AccessByDistance);
            this.groupBox2.Controls.Add(this.AccessBySize);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.BlobDataPoints);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtMaxBlobs);
            this.groupBox2.Controls.Add(this.blobSmooth);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtMaxDepth);
            this.groupBox2.Location = new System.Drawing.Point(996, 123);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Size = new System.Drawing.Size(368, 348);
            this.groupBox2.TabIndex = 42;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Blob Data";
            // 
            // AccessByDirection
            // 
            this.AccessByDirection.AutoSize = true;
            this.AccessByDirection.Location = new System.Drawing.Point(21, 137);
            this.AccessByDirection.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.AccessByDirection.Name = "AccessByDirection";
            this.AccessByDirection.Size = new System.Drawing.Size(205, 24);
            this.AccessByDirection.TabIndex = 45;
            this.AccessByDirection.TabStop = true;
            this.AccessByDirection.Text = "By direction (right to left)";
            this.AccessByDirection.UseVisualStyleBackColor = true;
            // 
            // AccessByDistance
            // 
            this.AccessByDistance.AutoSize = true;
            this.AccessByDistance.Location = new System.Drawing.Point(21, 100);
            this.AccessByDistance.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.AccessByDistance.Name = "AccessByDistance";
            this.AccessByDistance.Size = new System.Drawing.Size(203, 24);
            this.AccessByDistance.TabIndex = 44;
            this.AccessByDistance.TabStop = true;
            this.AccessByDistance.Text = "By distance (near to far)";
            this.AccessByDistance.UseVisualStyleBackColor = true;
            // 
            // AccessBySize
            // 
            this.AccessBySize.AutoSize = true;
            this.AccessBySize.Location = new System.Drawing.Point(21, 63);
            this.AccessBySize.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.AccessBySize.Name = "AccessBySize";
            this.AccessBySize.Size = new System.Drawing.Size(191, 24);
            this.AccessBySize.TabIndex = 43;
            this.AccessBySize.TabStop = true;
            this.AccessBySize.Text = "By size (large to small)";
            this.AccessBySize.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 31);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 20);
            this.label5.TabIndex = 42;
            this.label5.Text = "Access By:";
            // 
            // BlobDataPoints
            // 
            this.BlobDataPoints.AutoSize = true;
            this.BlobDataPoints.Location = new System.Drawing.Point(21, 185);
            this.BlobDataPoints.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.BlobDataPoints.Name = "BlobDataPoints";
            this.BlobDataPoints.Size = new System.Drawing.Size(158, 24);
            this.BlobDataPoints.TabIndex = 41;
            this.BlobDataPoints.Text = "Show data points";
            this.BlobDataPoints.UseVisualStyleBackColor = true;
            // 
            // Blob
            // 
            this.Blob.AutoSize = true;
            this.Blob.Location = new System.Drawing.Point(1011, 55);
            this.Blob.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Blob.Name = "Blob";
            this.Blob.Size = new System.Drawing.Size(119, 24);
            this.Blob.TabIndex = 43;
            this.Blob.Text = "Show Blobs";
            this.Blob.UseVisualStyleBackColor = true;
            // 
            // Contour
            // 
            this.Contour.AutoSize = true;
            this.Contour.Location = new System.Drawing.Point(1011, 89);
            this.Contour.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Contour.Name = "Contour";
            this.Contour.Size = new System.Drawing.Size(144, 24);
            this.Contour.TabIndex = 44;
            this.Contour.Text = "Show Contours";
            this.Contour.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBox3);
            this.groupBox3.Controls.Add(this.textBox2);
            this.groupBox3.Controls.Add(this.textBox1);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Location = new System.Drawing.Point(996, 562);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox3.Size = new System.Drawing.Size(368, 154);
            this.groupBox3.TabIndex = 45;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Legend";
            // 
            // textBox3
            // 
            this.textBox3.BackColor = System.Drawing.Color.DarkRed;
            this.textBox3.Enabled = false;
            this.textBox3.Location = new System.Drawing.Point(15, 69);
            this.textBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(28, 26);
            this.textBox3.TabIndex = 8;
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.Red;
            this.textBox2.Enabled = false;
            this.textBox2.Location = new System.Drawing.Point(15, 32);
            this.textBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(28, 26);
            this.textBox2.TabIndex = 7;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(154)))), ((int)(((byte)(0)))));
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(15, 106);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(28, 26);
            this.textBox1.TabIndex = 6;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(50, 75);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(96, 20);
            this.label8.TabIndex = 5;
            this.label8.Text = "Center point";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(48, 38);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(101, 20);
            this.label7.TabIndex = 3;
            this.label7.Text = "Closest point";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(48, 112);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(225, 20);
            this.label6.TabIndex = 1;
            this.label6.Text = "Left, Right, Top, Bottom points";
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(36, 25);
            this.StatusLabel.Text = "OK";
            // 
            // Status2
            // 
            this.Status2.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.Status2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel});
            this.Status2.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.Status2.Location = new System.Drawing.Point(0, 864);
            this.Status2.Name = "Status2";
            this.Status2.Padding = new System.Windows.Forms.Padding(2, 0, 21, 0);
            this.Status2.Size = new System.Drawing.Size(1395, 30);
            this.Status2.SizingGrip = false;
            this.Status2.TabIndex = 25;
            this.Status2.Text = "Status2";
            // 
            // labelFPS
            // 
            this.labelFPS.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.labelFPS.Enabled = false;
            this.labelFPS.Location = new System.Drawing.Point(996, 726);
            this.labelFPS.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.labelFPS.Name = "labelFPS";
            this.labelFPS.Size = new System.Drawing.Size(150, 19);
            this.labelFPS.TabIndex = 46;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1395, 894);
            this.Controls.Add(this.labelFPS);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.Contour);
            this.Controls.Add(this.Blob);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.fpsLabel);
            this.Controls.Add(this.Panel2);
            this.Controls.Add(this.Status2);
            this.Controls.Add(this.Stop);
            this.Controls.Add(this.StartBtn);
            this.Controls.Add(this.MainMenu);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "Intel(R) RealSense(TM) SDK: Blob Module";
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Panel2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.Status2.ResumeLayout(false);
            this.Status2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StartBtn;
        private System.Windows.Forms.Button Stop;
        private System.Windows.Forms.ToolStripMenuItem sourceToolStripMenuItem;
        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.PictureBox Panel2;
        private System.Windows.Forms.ToolStripMenuItem modeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Live;
        private System.Windows.Forms.ToolStripMenuItem Playback;
        private System.Windows.Forms.Label fpsLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox blobSmooth;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ContourSmooth;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtMaxDepth;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtMaxBlobs;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox Blob;
        private System.Windows.Forms.CheckBox Contour;
        private System.Windows.Forms.RadioButton AccessByDirection;
        private System.Windows.Forms.RadioButton AccessByDistance;
        private System.Windows.Forms.RadioButton AccessBySize;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox BlobDataPoints;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        private System.Windows.Forms.StatusStrip Status2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox labelFPS;
    }
}