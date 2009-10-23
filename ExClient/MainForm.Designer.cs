using System;

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
            if (proxy != null)
            {
                try
                {
                    proxy.Subscribe(false);
                    proxy.Close();
                }
                catch (Exception)
                {
                }
                proxy = null;
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
            this.info = new System.Windows.Forms.Label();
            this.refresh = new System.Windows.Forms.Button();
            this.stop = new System.Windows.Forms.Button();
            this.run = new System.Windows.Forms.Button();
            this.terminate = new System.Windows.Forms.Button();
            this.invoke = new System.Windows.Forms.Button();
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
            this.openToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
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
            this.extab.Controls.Add(this.info);
            this.extab.Controls.Add(this.refresh);
            this.extab.Controls.Add(this.stop);
            this.extab.Controls.Add(this.run);
            this.extab.Controls.Add(this.terminate);
            this.extab.Controls.Add(this.invoke);
            this.extab.Controls.Add(this.exlist);
            this.extab.Location = new System.Drawing.Point(4, 22);
            this.extab.Name = "extab";
            this.extab.Padding = new System.Windows.Forms.Padding(3);
            this.extab.Size = new System.Drawing.Size(616, 393);
            this.extab.TabIndex = 0;
            this.extab.Text = "Experiment";
            // 
            // info
            // 
            this.info.AutoSize = true;
            this.info.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.info.ForeColor = System.Drawing.Color.Navy;
            this.info.Location = new System.Drawing.Point(20, 246);
            this.info.Name = "info";
            this.info.Size = new System.Drawing.Size(0, 19);
            this.info.TabIndex = 8;
            this.info.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // refresh
            // 
            this.refresh.Location = new System.Drawing.Point(78, 173);
            this.refresh.Name = "refresh";
            this.refresh.Size = new System.Drawing.Size(81, 29);
            this.refresh.TabIndex = 7;
            this.refresh.Text = "Refresh";
            this.refresh.UseVisualStyleBackColor = true;
            this.refresh.Click += new System.EventHandler(this.refresh_Click);
            // 
            // stop
            // 
            this.stop.Location = new System.Drawing.Point(133, 123);
            this.stop.Name = "stop";
            this.stop.Size = new System.Drawing.Size(81, 29);
            this.stop.TabIndex = 6;
            this.stop.Text = "Stop";
            this.stop.UseVisualStyleBackColor = true;
            this.stop.Click += new System.EventHandler(this.stop_Click);
            // 
            // run
            // 
            this.run.Location = new System.Drawing.Point(22, 123);
            this.run.Name = "run";
            this.run.Size = new System.Drawing.Size(81, 29);
            this.run.TabIndex = 5;
            this.run.Text = "Run";
            this.run.UseVisualStyleBackColor = true;
            this.run.Click += new System.EventHandler(this.run_Click);
            // 
            // terminate
            // 
            this.terminate.Location = new System.Drawing.Point(133, 74);
            this.terminate.Name = "terminate";
            this.terminate.Size = new System.Drawing.Size(81, 29);
            this.terminate.TabIndex = 4;
            this.terminate.Text = "Terminate";
            this.terminate.UseVisualStyleBackColor = true;
            this.terminate.Click += new System.EventHandler(this.terminate_Click);
            // 
            // invoke
            // 
            this.invoke.Location = new System.Drawing.Point(22, 74);
            this.invoke.Name = "invoke";
            this.invoke.Size = new System.Drawing.Size(81, 29);
            this.invoke.TabIndex = 3;
            this.invoke.Text = "Invoke";
            this.invoke.UseVisualStyleBackColor = true;
            this.invoke.Click += new System.EventHandler(this.invoke_Click);
            // 
            // exlist
            // 
            this.exlist.Font = new System.Drawing.Font("Microsoft YaHei", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exlist.ForeColor = System.Drawing.Color.Red;
            this.exlist.FormattingEnabled = true;
            this.exlist.Location = new System.Drawing.Point(37, 27);
            this.exlist.Name = "exlist";
            this.exlist.Size = new System.Drawing.Size(166, 28);
            this.exlist.TabIndex = 2;
            this.exlist.Text = "Select Experiment !";
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
            this.extab.PerformLayout();
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
        private System.Windows.Forms.Button terminate;
        private System.Windows.Forms.Button invoke;
        private System.Windows.Forms.Button refresh;
        private System.Windows.Forms.Button stop;
        private System.Windows.Forms.Button run;
        private System.Windows.Forms.Label info;
    }
}

