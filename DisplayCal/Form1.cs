using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using StiLib.Core;
using ZedGraph;

namespace DisplayCal
{
    public partial class Form1 : Form
    {
        CRSCalDevice CRSDevice;
        double[] lum;


        public Form1()
        {
            InitializeComponent();
            CRSDevice = new CRSCalDevice(CalDevice.ColorCal_USB);
            InitGraph();
        }


        void InitGraph()
        {
            zedGraphControl.GraphPane.Title.Text = "Gamma";
            zedGraphControl.GraphPane.Title.FontSpec.IsBold = true;
            zedGraphControl.GraphPane.Title.FontSpec.Size = 24;
            zedGraphControl.GraphPane.XAxis.Title.Text = "Relative Luminance";
            zedGraphControl.GraphPane.XAxis.Title.FontSpec.IsBold = true;
            zedGraphControl.GraphPane.XAxis.Title.FontSpec.Size = 20;
            zedGraphControl.GraphPane.YAxis.Title.Text = "Absolute Luminance";
            zedGraphControl.GraphPane.YAxis.Title.FontSpec.IsBold = true;
            zedGraphControl.GraphPane.YAxis.Title.FontSpec.Size = 20;
            zedGraphControl.GraphPane.Fill = new Fill(Color.LightSkyBlue);
            zedGraphControl.GraphPane.Chart.Fill = new Fill(Color.Snow);
            zedGraphControl.GraphPane.XAxis.MinorTic.IsAllTics = false;
            zedGraphControl.GraphPane.YAxis.MinorTic.IsAllTics = false;
        }

        void PlotGamma(double[] lum)
        {
            double[] x = new double[lum.Length];
            for (int i = 0; i < x.Length; i++)
            {
                x[i] = i / (x.Length - 1.0);
            }

            // Measured Gamma Data
            LineItem measure = zedGraphControl.GraphPane.AddCurve("Measure", x, lum, Color.Black, SymbolType.Circle);
            zedGraphControl.GraphPane.XAxis.Scale.Min = 0.0;
            zedGraphControl.GraphPane.XAxis.Scale.Max = x.Max();
            zedGraphControl.GraphPane.YAxis.Scale.Min = 0.0;
            zedGraphControl.GraphPane.YAxis.Scale.Max = Math.Ceiling(lum.Max());
            measure.Symbol.Size = 10.0f;
            measure.Symbol.Fill = new Fill(Color.Blue);
            measure.Symbol.Border.IsVisible = false;

            // Fitted Gamma Curve

            zedGraphControl.AxisChange();
            zedGraphControl.Refresh();
        }


        private void red_n_ValueChanged(object sender, EventArgs e)
        {
            calControl.calColor.X = (float)red_n.Value;
        }

        private void green_n_ValueChanged(object sender, EventArgs e)
        {
            calControl.calColor.Y = (float)green_n.Value;
        }

        private void blue_n_ValueChanged(object sender, EventArgs e)
        {
            calControl.calColor.Z = (float)blue_n.Value;
        }

        private void cal_device_Click(object sender, EventArgs e)
        {
            if (CRSDevice.DeviceHandle == 0)
            {
                if (CRSDevice.Close() > 0)
                {
                    MessageBox.Show("Device Disconnection Failed !", "Error !");
                }
                else
                {
                    cal_device.Text = "Calibrate Device";
                }
            }
            else
            {
                if (CRSDevice.Init() > 0)
                {
                    MessageBox.Show("Device Initialization Failed !", "Error !");
                }
                else
                {
                    cal_device.Text = "Close Device";
                }
            }
        }

        private void auto_gamma_Click(object sender, EventArgs e)
        {
            if (CRSDevice.DeviceHandle != 0)
            {
                MessageBox.Show("Calibrate Device First !", "Device Not Initialized !");
                return;
            }

            timer.Enabled = false;
            int n = (int)sample_n.Value;
            lum = new double[n];
            float lumstep = 1.0f / (n - 1);
            for (int i = 0; i < n; i++)
            {
                gammatip.Text = "   Sampling ... " + (i + 1).ToString();
                calControl.calColor = new Vector3(lumstep * i);
                rlum_n.Text = ((calControl.calColor.X + calControl.calColor.Y + calControl.calColor.Z) / 3).ToString("F3");
                Refresh();

                Thread.Sleep(1500);
                lum[i] = CRSDevice.ReadLuminance;
                alum_n.Text = lum[i].ToString("F3");
                alum_n.Refresh();
            }

            PlotGamma(lum);
            gammatip.Text = "Set Sample First !";
            calControl.calColor = Vector3.Zero;
            timer.Enabled = true;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (CRSDevice.DeviceHandle == 0)
            {
                alum_n.Text = CRSDevice.ReadLuminance.ToString("F3");
                rlum_n.Text = ((red_n.Value + green_n.Value + blue_n.Value) / 3).ToString("F3");
            }
        }

    }
}
