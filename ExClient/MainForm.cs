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
using System.ServiceModel;

namespace ExClient
{
    [CallbackBehavior(UseSynchronizationContext = false)]
    public partial class MainForm : Form, ExService.IExServiceCallback
    {
        ExService.ExServiceClient proxy;


        public MainForm()
        {
            InitializeComponent();

            try
            {
                proxy = new ExClient.ExService.ExServiceClient(new InstanceContext(this));
                proxy.Subscribe(true);
                exlist.Items.AddRange(proxy.GetEx());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }


        public string OnRunStop(bool runstop)
        {
            try
            {
                string hr = "";
                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker) delegate
                                                    {
                                                        this.info.Text = "Server has changed its Run/Stop state to " +
                                                                         runstop.ToString();
                                                        hr = this.Text + " has Done.";
                                                    });
                }
                else
                {
                    this.info.Text = "Server has changed its Run/Stop state to " + runstop.ToString();
                    hr = this.Text + " has Done.";
                }
                return hr;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void invoke_Click(object sender, EventArgs e)
        {
            try
            {
                string hresult = proxy.Invoke(exlist.SelectedItem.ToString());
                if(string.IsNullOrEmpty(hresult))
                {
                    this.info.Text = exlist.SelectedItem.ToString() + " has invoked in server.";
                }
                else
                {
                    this.info.Text = hresult;
                }
            }
            catch (Exception ex)
            {
                this.info.Text = ex.Message;
            }
        }

        private void terminate_Click(object sender, EventArgs e)
        {
            try
            {
                string hr = proxy.Terminate(exlist.SelectedItem.ToString());
                if (string.IsNullOrEmpty(hr))
                {
                    this.info.Text = exlist.SelectedItem.ToString() + " has terminated in server.";
                }
                else
                {
                    this.info.Text = hr;
                }
            }
            catch (Exception ex)
            {
                this.info.Text = ex.Message;
            }
        }

        private void run_Click(object sender, EventArgs e)
        {
            if(!proxy.get_RunStop())
            {
                proxy.set_RunStop(true);
                refresh_Click(sender,e);
            }
        }

        private void stop_Click(object sender, EventArgs e)
        {
            if(proxy.get_RunStop())
            {
                proxy.set_RunStop(false);
                refresh_Click(sender, e);
            }
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            string hr = proxy.get_RunStop().ToString();
            this.info.Text = "Server Run/Stop State is " + hr;
        }

    }
}
