#region File Description
//-----------------------------------------------------------------------------
// RF_fBar.cs
//
// StiLib Flashing Bar Reverse-Correlation Receptive Field Mapping Stimulus
// Copyright (c) Zhang Li. 2008-11-09.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StiLib.Core;
#endregion

namespace StiLib.Vision.Stimuli
{
    /// <summary>
    /// Reverse-Correlation RF Mapping
    /// </summary>
    public class RF_fBar : SLForm
    {
        /// <summary>
        /// Default SLForm Settings
        /// </summary>
        public RF_fBar()
            : base()
        {
        }

        /// <summary>
        /// Custom SLForm Settings
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="refreshrate"></param>
        /// <param name="isvsync"></param>
        /// <param name="isshowcursor"></param>
        public RF_fBar(int width, int height, int refreshrate, bool isvsync, bool isshowcursor)
            : base(width, height, refreshrate, isvsync, isshowcursor)
        {
        }


        /// <summary>
        /// Text to draw Message
        /// </summary>
        public Text text;
        /// <summary>
        /// Hold the Information about the Experiment
        /// </summary>
        public SLExperiment ex;
        /// <summary>
        /// RF Mapping Black and White Bars
        /// </summary>
        public Bar[] Bar = new Bar[] { new Bar(), new Bar() };
        /// <summary>
        /// Mapping Grid Row Number
        /// </summary>
        public int Rows;
        /// <summary>
        /// Mapping Grid Column Number
        /// </summary>
        public int Columns;


        /// <summary>
        /// Init all information of the experiment
        /// </summary>
        protected override void Initialize()
        {
            text = new Text(GraphicsDevice, Services, "Content", "Arial");

            // Init Experiment Parameter
            ex = new SLExperiment();
            ex.AddExType(ExType.RF_fBar);
            ex.AddCondition(ExPara.Orientation, 0);
            ex.Expara.trial = 60;
            ex.Expara.durT = 0.030f;
            ex.Expara.bgcolor = Color.Gray;

            // Init Two Bars Parameter
            BarPara bpara = BarPara.Default;
            bpara.width = 1.5f;
            bpara.height = 0.5f;
            bpara.BasePara.color = Color.Black;
            Bar[0].Init(GraphicsDevice, bpara);

            bpara.BasePara.color = Color.White;
            Bar[1].Init(GraphicsDevice, bpara);

            InitGrid();
        }

        /// <summary>
        /// Init Mapping Grid
        /// </summary>
        public void InitGrid()
        {
            Rows = (int)Math.Floor(Bar[0].Para.BasePara.movearea / Bar[0].Para.height);
            Columns = (int)Math.Floor(Bar[0].Para.BasePara.movearea / Bar[0].Para.width);
            if (Rows % 2 == 0)
            {
                Rows += 1;
            }
            if (Columns % 2 == 0)
            {
                Columns += 1;
            }
        }

        /// <summary>
        /// Set Flow to control experiment
        /// </summary>
        protected override void SetFlow()
        {
            ex.Flow.TCount = 0;
            ex.Flow.SCount = 0;
            ex.Flow.IsPred = false;
            ex.Flow.StiTime = ex.Expara.durT;
        }

        /// <summary>
        /// Send crucial information in MarkerHeader 
        /// </summary>
        protected override void MarkHead()
        {
            ex.Expara.stimuli[0] = Rows * Columns * 2;
            ex.Rand.RandomizeSeed();
            ex.Rand.RandomizeSequence(ex.Expara.stimuli[0]);

            // Experiment Type Encoding
            ex.PPort.MarkerEncode(ex.Extype[0].Value);
            // Condition Parameter Type Encoding
            ex.PPort.MarkerEncode(ex.Cond[0].SKEY);
            // Condition Number Encoding
            ex.PPort.MarkerEncode(ex.Cond[0].VALUE.ValueN);
            // Random Seed Encoding
            ex.PPort.MarkerEncode(ex.Rand.RSeed);
            // Experiment Trials
            ex.PPort.MarkerEncode(ex.Expara.trial);

            // Keywords Group Seperator
            ex.PPort.MarkerSeparatorEncode();

            // Custom Parameters Encoding
            ex.PPort.MarkerEncode((int)Math.Floor(Bar[0].Para.height * 10.0));
            ex.PPort.MarkerEncode((int)Math.Floor(Bar[0].Para.width * 10.0));
            ex.PPort.MarkerEncode((int)Math.Floor((double)Bar[0].Para.BasePara.orientation));
            ex.PPort.MarkerEncode(Rows);
            ex.PPort.MarkerEncode(Columns);
            ex.PPort.MarkerEncode((int)Math.Floor((Bar[0].Para.BasePara.center.X + 60.0f) * 10.0));
            ex.PPort.MarkerEncode((int)Math.Floor((Bar[0].Para.BasePara.center.Y + 60.0f) * 10.0));
            ex.PPort.MarkerEncode((int)Math.Floor((double)Bar[0].view_h_deg));
            ex.PPort.MarkerEncode((int)Math.Floor((double)Bar[0].view_w_deg));

            // End of Header Encoding
            ex.PPort.MarkerEndEncode();
            // Set ready to begin
            ex.Flow.IsStiOn = true;
        }

        /// <summary>
        /// Update experiment
        /// </summary>
        protected override void Update()
        {
            if (GO_OVER)
            {
                Update_RF_fBar();
            }
        }

        /// <summary>
        /// Draw Stimuli
        /// </summary>
        protected override void Draw()
        {
            GraphicsDevice.Clear(ex.Expara.bgcolor);

            if (GO_OVER)
            {
                Bar[ex.Flow.Which].Draw(GraphicsDevice);
                ex.Flow.Info = ex.Flow.TCount.ToString() + " / " + ex.Expara.trial.ToString() + " Trials\n" +
                                       ex.Flow.SCount.ToString() + " / " + ex.Expara.stimuli[0].ToString() + " Stimuli";
                text.Draw(ex.Flow.Info);
            }
            else
            {
                text.Draw();
            }
        }

        void Update_RF_fBar()
        {
            if (ex.Flow.IsStiOn)
            {
                ex.Flow.IsStiOn = false;
                ex.PPort.timer.ReStart();
                // Stimulus Onset Marker 
                ex.PPort.Trigger();
            }

            // In Presentation
            if (ex.PPort.timer.ElapsedSeconds < ex.Flow.StiTime + 0.001)
            {
                // Set Stimulus Position in Grid only once at the begining of Presentation
                if (!ex.Flow.IsPred)
                {
                    ex.Flow.IsPred = true;

                    ex.Flow.RCount = (int)Math.Floor(ex.Rand.RSequence[ex.Flow.SCount] / (Columns * 2.0));
                    int t = ex.Rand.RSequence[ex.Flow.SCount] % (Columns * 2);
                    ex.Flow.CCount = (int)Math.Floor(t / 2.0);
                    ex.Flow.Which = t % 2;

                    float Xgrid = -(Columns - 1) * Bar[0].Para.width / 2 + Bar[0].Para.width * ex.Flow.CCount;
                    float Ygrid = (Rows - 1) * Bar[0].Para.height / 2 - Bar[0].Para.height * ex.Flow.RCount;
                    ex.Flow.Rotate = Matrix.CreateRotationZ((float)(Bar[0].Para.BasePara.orientation * Math.PI / 180.0));
                    ex.Flow.Translate = Matrix.CreateTranslation(Xgrid, Ygrid, 0.0f) * ex.Flow.Rotate * Matrix.CreateTranslation(Bar[0].Para.BasePara.center);
                    Bar[ex.Flow.Which].SetWorld(ex.Flow.Translate);
                }
            }
            else // End of Presentation
            {
                // If Begin Another Stimulus
                if (ex.Flow.SCount < ex.Expara.stimuli[0] - 1)
                {
                    ex.Flow.IsStiOn = true;
                    ex.Flow.IsPred = false;
                    ex.Flow.SCount += 1;
                }
                else
                {
                    // If Begin Another Trial
                    if (ex.Flow.TCount < ex.Expara.trial - 1)
                    {
                        // Each trial has different random sequence of stimulus
                        ex.Rand.RandomizeSequence(ex.Expara.stimuli[0]);
                        ex.Flow.IsStiOn = true;
                        ex.Flow.IsPred = false;
                        ex.Flow.TCount += 1;
                        ex.Flow.SCount = 0;
                    }
                    else // End of Experiment
                    {
                        GO_OVER = false;
                    }
                }

                // If presentation ended, immediately update the next Stimulus, because of the 
                // 'No Rest' requirement of reverse-correlation technique
                Update();
            }

        }

    }
}