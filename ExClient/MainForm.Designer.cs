namespace ExClient
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
            this.mainmenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maintab = new System.Windows.Forms.TabControl();
            this.extab = new System.Windows.Forms.TabPage();
            this.exlist = new System.Windows.Forms.ComboBox();
            this.mainmenu.SuspendLayout();
            this.maintab.SuspendLayout();
            this.extab.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainmenu
            // 
            this.mainmenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.mainmenu.Location = new System.Drawing.Point(0, 0);
            this.mainmenu.Name = "mainmenu";
            this.mainmenu.Size = new System.Drawing.Size(624, 25);
            this.mainmenu.TabIndex = 0;
            this.mainmenu.Text = "mainmenu";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(39, 21);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // maintab
            // 
            this.maintab.Controls.Add(this.extab);
            this.maintab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.maintab.Location = new System.Drawing.Point(0, 25);
            this.maintab.Name = "maintab";
            this.maintab.SelectedIndex = 0;
            this.maintab.Size = new System.Drawing.Size(624, 419);
            this.maintab.TabIndex = 1;
            // 
            // extab
            // 
            this.extab.BackColor = System.Drawing.Color.Gainsboro;
            this.extab.Controls.Add(this.exlist);
            this.extab.Location = new System.Drawing.Point(4, 22);
            this.extab.Name = "extab";
            this.extab.Padding = new System.Windows.Forms.Padding(3);
            this.extab.Size = new System.Drawing.Size(616, 393);
            this.extab.TabIndex = 0;
            this.extab.Text = "Experiment";
            // 
            // exlist
            // 
            this.exlist.Font = new System.Drawing.Font("Microsoft YaHei", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exlist.ForeColor = System.Drawing.Color.Red;
            this.exlist.FormattingEnabled = true;
            this.exlist.Location = new System.Drawing.Point(18, 18);
            this.exlist.Name = "exlist";
            this.exlist.Size = new System.Drawing.Size(166, 28);
            this.exlist.TabIndex = 2;
            this.exlist.Text = "Select Experiment !";
            this.exlist.SelectedIndexChanged += new System.EventHandler(this.exlist_SelectedIndexChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 444);
            this.Controls.Add(this.maintab);
            this.Controls.Add(this.mainmenu);
            this.MainMenuStrip = this.mainmenu;
            this.Name = "MainForm";
            this.Text = "ExClient";
            this.mainmenu.ResumeLayout(false);
            this.mainmenu.PerformLayout();
            this.maintab.ResumeLayout(false);
            this.extab.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainmenu;
        private System.Windows.Forms.TabControl maintab;
        private System.Windows.Forms.TabPage extab;
        private System.Windows.Forms.ComboBox exlist;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    }
}

