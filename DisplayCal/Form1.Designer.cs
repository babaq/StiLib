namespace DisplayCal
{
    partial class Form1
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
            if(CRSDevice!=null)
            {
                CRSDevice.Dispose();
                CRSDevice = null;
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
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.alum_n = new System.Windows.Forms.Label();
            this.alum = new System.Windows.Forms.Label();
            this.rlum_n = new System.Windows.Forms.Label();
            this.rlum = new System.Windows.Forms.Label();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.blue_n = new System.Windows.Forms.NumericUpDown();
            this.green_n = new System.Windows.Forms.NumericUpDown();
            this.red_n = new System.Windows.Forms.NumericUpDown();
            this.blue = new System.Windows.Forms.Label();
            this.green = new System.Windows.Forms.Label();
            this.red = new System.Windows.Forms.Label();
            this.cal_device = new System.Windows.Forms.Button();
            this.gammatip = new System.Windows.Forms.Label();
            this.sample = new System.Windows.Forms.Label();
            this.sample_n = new System.Windows.Forms.NumericUpDown();
            this.auto_gamma = new System.Windows.Forms.Button();
            this.zedGraphControl = new ZedGraph.ZedGraphControl();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.calControl = new DisplayCal.CalControl();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.blue_n)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.green_n)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.red_n)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sample_n)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.calControl);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(984, 664);
            this.splitContainer1.SplitterDistance = 607;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.BackColor = System.Drawing.Color.LightSkyBlue;
            this.splitContainer2.Panel2.Controls.Add(this.zedGraphControl);
            this.splitContainer2.Size = new System.Drawing.Size(373, 664);
            this.splitContainer2.SplitterDistance = 379;
            this.splitContainer2.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.BackColor = System.Drawing.Color.MediumSpringGreen;
            this.splitContainer3.Panel1.Controls.Add(this.alum_n);
            this.splitContainer3.Panel1.Controls.Add(this.alum);
            this.splitContainer3.Panel1.Controls.Add(this.rlum_n);
            this.splitContainer3.Panel1.Controls.Add(this.rlum);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.splitContainer4);
            this.splitContainer3.Size = new System.Drawing.Size(373, 379);
            this.splitContainer3.SplitterDistance = 160;
            this.splitContainer3.TabIndex = 0;
            // 
            // alum_n
            // 
            this.alum_n.AutoSize = true;
            this.alum_n.Font = new System.Drawing.Font("Times New Roman", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.alum_n.ForeColor = System.Drawing.Color.DeepPink;
            this.alum_n.Location = new System.Drawing.Point(268, 106);
            this.alum_n.Name = "alum_n";
            this.alum_n.Size = new System.Drawing.Size(55, 22);
            this.alum_n.TabIndex = 17;
            this.alum_n.Text = "0.000";
            // 
            // alum
            // 
            this.alum.AutoSize = true;
            this.alum.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.alum.Location = new System.Drawing.Point(30, 104);
            this.alum.Name = "alum";
            this.alum.Size = new System.Drawing.Size(170, 21);
            this.alum.TabIndex = 16;
            this.alum.Text = "Absolute Luminance:";
            // 
            // rlum_n
            // 
            this.rlum_n.AutoSize = true;
            this.rlum_n.Font = new System.Drawing.Font("Times New Roman", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rlum_n.ForeColor = System.Drawing.Color.DeepPink;
            this.rlum_n.Location = new System.Drawing.Point(268, 42);
            this.rlum_n.Name = "rlum_n";
            this.rlum_n.Size = new System.Drawing.Size(55, 22);
            this.rlum_n.TabIndex = 15;
            this.rlum_n.Text = "0.000";
            // 
            // rlum
            // 
            this.rlum.AutoSize = true;
            this.rlum.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rlum.Location = new System.Drawing.Point(30, 40);
            this.rlum.Name = "rlum";
            this.rlum.Size = new System.Drawing.Size(164, 21);
            this.rlum.TabIndex = 14;
            this.rlum.Text = "Relative Luminance:";
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.BackColor = System.Drawing.Color.LightPink;
            this.splitContainer4.Panel1.Controls.Add(this.blue_n);
            this.splitContainer4.Panel1.Controls.Add(this.green_n);
            this.splitContainer4.Panel1.Controls.Add(this.red_n);
            this.splitContainer4.Panel1.Controls.Add(this.blue);
            this.splitContainer4.Panel1.Controls.Add(this.green);
            this.splitContainer4.Panel1.Controls.Add(this.red);
            this.splitContainer4.Panel1.Controls.Add(this.cal_device);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.splitContainer4.Panel2.Controls.Add(this.gammatip);
            this.splitContainer4.Panel2.Controls.Add(this.sample);
            this.splitContainer4.Panel2.Controls.Add(this.sample_n);
            this.splitContainer4.Panel2.Controls.Add(this.auto_gamma);
            this.splitContainer4.Panel2.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.splitContainer4.Size = new System.Drawing.Size(373, 215);
            this.splitContainer4.SplitterDistance = 193;
            this.splitContainer4.TabIndex = 0;
            // 
            // blue_n
            // 
            this.blue_n.DecimalPlaces = 2;
            this.blue_n.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.blue_n.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.blue_n.Location = new System.Drawing.Point(108, 159);
            this.blue_n.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.blue_n.Name = "blue_n";
            this.blue_n.Size = new System.Drawing.Size(56, 21);
            this.blue_n.TabIndex = 7;
            this.blue_n.ValueChanged += new System.EventHandler(this.blue_n_ValueChanged);
            // 
            // green_n
            // 
            this.green_n.DecimalPlaces = 2;
            this.green_n.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.green_n.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.green_n.Location = new System.Drawing.Point(108, 129);
            this.green_n.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.green_n.Name = "green_n";
            this.green_n.Size = new System.Drawing.Size(56, 21);
            this.green_n.TabIndex = 6;
            this.green_n.ValueChanged += new System.EventHandler(this.green_n_ValueChanged);
            // 
            // red_n
            // 
            this.red_n.DecimalPlaces = 2;
            this.red_n.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.red_n.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.red_n.Location = new System.Drawing.Point(108, 97);
            this.red_n.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.red_n.Name = "red_n";
            this.red_n.Size = new System.Drawing.Size(56, 21);
            this.red_n.TabIndex = 5;
            this.red_n.ValueChanged += new System.EventHandler(this.red_n_ValueChanged);
            // 
            // blue
            // 
            this.blue.AutoSize = true;
            this.blue.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.blue.ForeColor = System.Drawing.Color.Blue;
            this.blue.Location = new System.Drawing.Point(20, 159);
            this.blue.Name = "blue";
            this.blue.Size = new System.Drawing.Size(40, 19);
            this.blue.TabIndex = 3;
            this.blue.Text = "Blue";
            // 
            // green
            // 
            this.green.AutoSize = true;
            this.green.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.green.ForeColor = System.Drawing.Color.Lime;
            this.green.Location = new System.Drawing.Point(20, 129);
            this.green.Name = "green";
            this.green.Size = new System.Drawing.Size(51, 19);
            this.green.TabIndex = 2;
            this.green.Text = "Green";
            // 
            // red
            // 
            this.red.AutoSize = true;
            this.red.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.red.ForeColor = System.Drawing.Color.Red;
            this.red.Location = new System.Drawing.Point(20, 97);
            this.red.Name = "red";
            this.red.Size = new System.Drawing.Size(37, 19);
            this.red.TabIndex = 1;
            this.red.Text = "Red";
            // 
            // cal_device
            // 
            this.cal_device.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cal_device.Location = new System.Drawing.Point(47, 33);
            this.cal_device.Name = "cal_device";
            this.cal_device.Size = new System.Drawing.Size(104, 35);
            this.cal_device.TabIndex = 0;
            this.cal_device.Text = "Calibrate Device";
            this.cal_device.UseVisualStyleBackColor = true;
            this.cal_device.Click += new System.EventHandler(this.cal_device_Click);
            // 
            // gammatip
            // 
            this.gammatip.AutoSize = true;
            this.gammatip.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gammatip.ForeColor = System.Drawing.Color.Red;
            this.gammatip.Location = new System.Drawing.Point(22, 160);
            this.gammatip.Name = "gammatip";
            this.gammatip.Size = new System.Drawing.Size(135, 19);
            this.gammatip.TabIndex = 13;
            this.gammatip.Text = "Set Samples First !";
            // 
            // sample
            // 
            this.sample.AutoSize = true;
            this.sample.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sample.Location = new System.Drawing.Point(22, 104);
            this.sample.Name = "sample";
            this.sample.Size = new System.Drawing.Size(65, 19);
            this.sample.TabIndex = 12;
            this.sample.Text = "Samples";
            // 
            // sample_n
            // 
            this.sample_n.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sample_n.Location = new System.Drawing.Point(101, 105);
            this.sample_n.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.sample_n.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.sample_n.Name = "sample_n";
            this.sample_n.Size = new System.Drawing.Size(56, 21);
            this.sample_n.TabIndex = 11;
            this.sample_n.Value = new decimal(new int[] {
            11,
            0,
            0,
            0});
            // 
            // auto_gamma
            // 
            this.auto_gamma.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.auto_gamma.Location = new System.Drawing.Point(44, 33);
            this.auto_gamma.Name = "auto_gamma";
            this.auto_gamma.Size = new System.Drawing.Size(104, 35);
            this.auto_gamma.TabIndex = 1;
            this.auto_gamma.Text = "Auto Gamma";
            this.auto_gamma.UseVisualStyleBackColor = true;
            this.auto_gamma.Click += new System.EventHandler(this.auto_gamma_Click);
            // 
            // zedGraphControl
            // 
            this.zedGraphControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zedGraphControl.Location = new System.Drawing.Point(0, 0);
            this.zedGraphControl.Name = "zedGraphControl";
            this.zedGraphControl.ScrollGrace = 0;
            this.zedGraphControl.ScrollMaxX = 0;
            this.zedGraphControl.ScrollMaxY = 0;
            this.zedGraphControl.ScrollMaxY2 = 0;
            this.zedGraphControl.ScrollMinX = 0;
            this.zedGraphControl.ScrollMinY = 0;
            this.zedGraphControl.ScrollMinY2 = 0;
            this.zedGraphControl.Size = new System.Drawing.Size(373, 281);
            this.zedGraphControl.TabIndex = 0;
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 1500;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // calControl
            // 
            this.calControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.calControl.Location = new System.Drawing.Point(0, 0);
            this.calControl.Name = "calControl";
            this.calControl.Size = new System.Drawing.Size(607, 664);
            this.calControl.TabIndex = 0;
            this.calControl.Text = "calControl";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 664);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Text = "Display Calibration";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel1.PerformLayout();
            this.splitContainer4.Panel2.ResumeLayout(false);
            this.splitContainer4.Panel2.PerformLayout();
            this.splitContainer4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.blue_n)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.green_n)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.red_n)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sample_n)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private CalControl calControl;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.Label blue;
        private System.Windows.Forms.Label green;
        private System.Windows.Forms.Label red;
        private System.Windows.Forms.Button cal_device;
        private System.Windows.Forms.NumericUpDown blue_n;
        private System.Windows.Forms.NumericUpDown green_n;
        private System.Windows.Forms.NumericUpDown red_n;
        private System.Windows.Forms.Button auto_gamma;
        private System.Windows.Forms.NumericUpDown sample_n;
        private System.Windows.Forms.Label gammatip;
        private System.Windows.Forms.Label sample;
        private System.Windows.Forms.Label alum_n;
        private System.Windows.Forms.Label alum;
        private System.Windows.Forms.Label rlum_n;
        private System.Windows.Forms.Label rlum;
        private System.Windows.Forms.Timer timer;
        private ZedGraph.ZedGraphControl zedGraphControl;
    }
}

