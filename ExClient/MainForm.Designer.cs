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
            if (proxyExC != null)
            {
                try
                {
                    proxyExC.Subscribe(false);
                    proxyExC.Close();
                }
                catch (Exception)
                {
                }
                proxyExC = null;
            }
            if (proxyExS != null)
            {
                try
                {
                    proxyExS.Subscribe(false);
                    proxyExS.Close();
                }
                catch (Exception)
                {
                }
                proxyExS = null;
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
            this.controlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maintab = new System.Windows.Forms.TabControl();
            this.extab = new System.Windows.Forms.TabPage();
            this.exdataGrid = new System.Windows.Forms.DataGridView();
            this.exsstate = new System.Windows.Forms.Label();
            this.excstate = new System.Windows.Forms.Label();
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
            ((System.ComponentModel.ISupportInitialize)(this.exdataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // mainmenu
            // 
            this.mainmenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.controlToolStripMenuItem});
            this.mainmenu.Location = new System.Drawing.Point(0, 0);
            this.mainmenu.Name = "mainmenu";
            this.mainmenu.Size = new System.Drawing.Size(654, 25);
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
            // controlToolStripMenuItem
            // 
            this.controlToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.searchServerToolStripMenuItem});
            this.controlToolStripMenuItem.Name = "controlToolStripMenuItem";
            this.controlToolStripMenuItem.Size = new System.Drawing.Size(63, 21);
            this.controlToolStripMenuItem.Text = "Control";
            // 
            // searchServerToolStripMenuItem
            // 
            this.searchServerToolStripMenuItem.Name = "searchServerToolStripMenuItem";
            this.searchServerToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.searchServerToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.searchServerToolStripMenuItem.Text = "Search Online Servers";
            this.searchServerToolStripMenuItem.Click += new System.EventHandler(this.searchServerToolStripMenuItem_Click);
            // 
            // maintab
            // 
            this.maintab.Controls.Add(this.extab);
            this.maintab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.maintab.Location = new System.Drawing.Point(0, 25);
            this.maintab.Name = "maintab";
            this.maintab.SelectedIndex = 0;
            this.maintab.Size = new System.Drawing.Size(654, 445);
            this.maintab.TabIndex = 1;
            // 
            // extab
            // 
            this.extab.BackColor = System.Drawing.Color.Gainsboro;
            this.extab.Controls.Add(this.exdataGrid);
            this.extab.Controls.Add(this.exsstate);
            this.extab.Controls.Add(this.excstate);
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
            this.extab.Size = new System.Drawing.Size(646, 419);
            this.extab.TabIndex = 0;
            this.extab.Text = "Experiment";
            // 
            // exdataGrid
            // 
            this.exdataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.exdataGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.exdataGrid.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.exdataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.exdataGrid.Dock = System.Windows.Forms.DockStyle.Right;
            this.exdataGrid.Location = new System.Drawing.Point(299, 3);
            this.exdataGrid.Name = "exdataGrid";
            this.exdataGrid.RowHeadersWidth = 18;
            this.exdataGrid.RowTemplate.Height = 23;
            this.exdataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.exdataGrid.Size = new System.Drawing.Size(344, 413);
            this.exdataGrid.TabIndex = 11;
            // 
            // exsstate
            // 
            this.exsstate.AutoSize = true;
            this.exsstate.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.exsstate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.exsstate.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.exsstate.ForeColor = System.Drawing.Color.Red;
            this.exsstate.Location = new System.Drawing.Point(166, 15);
            this.exsstate.Name = "exsstate";
            this.exsstate.Size = new System.Drawing.Size(114, 19);
            this.exsstate.TabIndex = 10;
            this.exsstate.Text = "ExService Offline";
            // 
            // excstate
            // 
            this.excstate.AutoSize = true;
            this.excstate.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.excstate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.excstate.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.excstate.ForeColor = System.Drawing.Color.Red;
            this.excstate.Location = new System.Drawing.Point(16, 15);
            this.excstate.Name = "excstate";
            this.excstate.Size = new System.Drawing.Size(120, 19);
            this.excstate.TabIndex = 9;
            this.excstate.Text = "ExConsole Offline";
            // 
            // info
            // 
            this.info.AutoEllipsis = true;
            this.info.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.info.ForeColor = System.Drawing.Color.Navy;
            this.info.Location = new System.Drawing.Point(13, 234);
            this.info.Name = "info";
            this.info.Size = new System.Drawing.Size(280, 180);
            this.info.TabIndex = 8;
            this.info.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // refresh
            // 
            this.refresh.Location = new System.Drawing.Point(110, 188);
            this.refresh.Name = "refresh";
            this.refresh.Size = new System.Drawing.Size(81, 29);
            this.refresh.TabIndex = 7;
            this.refresh.Text = "Refresh";
            this.refresh.UseVisualStyleBackColor = true;
            this.refresh.Click += new System.EventHandler(this.refresh_Click);
            // 
            // stop
            // 
            this.stop.Location = new System.Drawing.Point(165, 138);
            this.stop.Name = "stop";
            this.stop.Size = new System.Drawing.Size(81, 29);
            this.stop.TabIndex = 6;
            this.stop.Text = "Stop";
            this.stop.UseVisualStyleBackColor = true;
            this.stop.Click += new System.EventHandler(this.stop_Click);
            // 
            // run
            // 
            this.run.Location = new System.Drawing.Point(54, 138);
            this.run.Name = "run";
            this.run.Size = new System.Drawing.Size(81, 29);
            this.run.TabIndex = 5;
            this.run.Text = "Run";
            this.run.UseVisualStyleBackColor = true;
            this.run.Click += new System.EventHandler(this.run_Click);
            // 
            // terminate
            // 
            this.terminate.Location = new System.Drawing.Point(165, 89);
            this.terminate.Name = "terminate";
            this.terminate.Size = new System.Drawing.Size(81, 29);
            this.terminate.TabIndex = 4;
            this.terminate.Text = "Terminate";
            this.terminate.UseVisualStyleBackColor = true;
            this.terminate.Click += new System.EventHandler(this.terminate_Click);
            // 
            // invoke
            // 
            this.invoke.Location = new System.Drawing.Point(54, 89);
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
            this.exlist.Location = new System.Drawing.Point(69, 42);
            this.exlist.Name = "exlist";
            this.exlist.Size = new System.Drawing.Size(166, 28);
            this.exlist.TabIndex = 2;
            this.exlist.Text = "Select Experiment !";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(654, 470);
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
            ((System.ComponentModel.ISupportInitialize)(this.exdataGrid)).EndInit();
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
        private System.Windows.Forms.Label excstate;
        private System.Windows.Forms.ToolStripMenuItem controlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchServerToolStripMenuItem;
        private System.Windows.Forms.Label exsstate;
        private System.Windows.Forms.DataGridView exdataGrid;
    }
}

