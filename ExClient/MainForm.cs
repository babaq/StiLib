using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace ExClient
{
    public partial class MainForm : Form
    {
        ExService.ExServiceClient proxy;


        public MainForm()
        {
            InitializeComponent();

            try
            {
                proxy = new ExClient.ExService.ExServiceClient();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            exlist.Items.AddRange(proxy.GetEx());
        }
        ~MainForm()
        {
            proxy.Close();
        }

        private void exlist_SelectedIndexChanged(object sender, EventArgs e)
        {
            string hresult = proxy.Invoke(exlist.SelectedItem.ToString());
            if (hresult != null)
            {
                MessageBox.Show(hresult);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfileDialog = new OpenFileDialog();

            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            string exPath = Path.GetFullPath(assemblyLocation);

            openfileDialog.InitialDirectory = exPath;
            openfileDialog.Title = "Load Script";
            openfileDialog.Filter = "All Files (*.*)|*.*|" +
                                "F# Script (*.fsx)|*.fsx|" +
                                "IronPython Script (*.py)|*.py";

            if (openfileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamReader reader = new StreamReader(openfileDialog.FileName);

                string ex = openfileDialog.FileName.Substring(openfileDialog.FileName.LastIndexOf("\\") + 1);
                string hresult = proxy.InvokeScript(ex, reader.ReadToEnd());
                if (hresult != null)
                {
                    MessageBox.Show(hresult);
                }
                reader.Close();
            }
        }

    }
}
