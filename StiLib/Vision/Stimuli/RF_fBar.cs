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
    /// Flashing Bar Reverse-Correlation RF Mapping
    /// </summary>
    public class RF_fBar : SLForm
    {
        /// <summary>
        /// Init to Default SLForm Settings
        /// </summary>
        public RF_fBar()
            : base()
        {
        }

        /// <summary>
        /// Init to configurations
        /// </summary>
        /// <param name="configfile"></param>
        public RF_fBar(string configfile)
            : base(configfile)
        {
        }

        /// <summary>
        /// Init to Custom SLForm Settings
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
        public Bar[] bars = new Bar[] { new Bar(), new Bar() };
        /// <summary>
        /// Mapping Grid Row Number
        /// </summary>
        public int Rows;
        /// <summary>
        /// Mapping Grid Row Resolution
        /// </summary>
        public float Rstep;
        /// <summary>
        /// Mapping Grid Column Number
        /// </summary>
        public int Columns;
        /// <summary>
        /// Mapping Grid Column Resolution
        /// </summary>
        public float Cstep;


        /// <summary>
        /// Init all information of the experiment
        /// </summary>
        protected override void Initialize()
        {
            text = new Text(GraphicsDevice, Services, SLConfig["content"], "Arial");

            // Init Experiment Parameter
            ex = new SLExperiment();
            ex.AddExType(ExType.RF_fBar);
            ex.AddCondition(ExPara.Orientation, 0);
            ex.Exdesign.trial = 50;
            ex.Exdesign.durT = 0.030f;
            ex.Exdesign.bgcolor = Color.Gray;

            // Init Two Bars Parameters
            BarPara bpara = BarPara.Default;
            bpara.width = 1.0f;
            bpara.height = 0.5f;
            bpara.BasePara.orientation = 0.0f;
            bpara.BasePara.color = Color.Black;
            bars[0].Init(GraphicsDevice, bpara);

            bpara.BasePara.color = Color.White;
            bars[1].Init(GraphicsDevice, bpara);

            Rstep = 0.5f;
            Cstep = 0.5f;
            InitGrid();
        }

        /// <summary>
        /// Init Mapping Grid 'Rows' and 'Columns' according to RowStep, ColumnStep and Bar's space
        /// </summary>
        public void InitGrid()
        {
            Rows = (int)Math.Floor(bars[0].Para.BasePara.space / Rstep);
            if (Rows % 2 == 0)
            {
                Rows += 1;
            }
            Columns = (int)Math.Floor(bars[0].Para.BasePara.space / Cstep);
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
            ex.Flow.TrialCount = 0;
            ex.Flow.StiCount = 0;
            ex.Flow.IsPred = false;
            ex.Flow.IsStiOn = false;
            ex.Flow.StiTime = ex.Exdesign.durT;

            ex.Flow.RotateOri = Matrix.CreateRotationZ(bars[0].Para.BasePara.orientation * (float)SLConstant.Rad_p_Deg);
            ex.Flow.TranslateCenter = Matrix.CreateTranslation(bars[0].Para.BasePara.center);
        }

        /// <summary>
        /// Send information in MarkerHeader 
        /// </summary>
        protected override void MarkHead()
        {
            DrawTip(ref text, ex.Exdesign.bgcolor, SLConstant.MarkHead);

            ex.Exdesign.stimuli[0] = Rows * Columns * 2;
            ex.Rand.RandomizeSequence(ex.Exdesign.stimuli[0]);

            // Experiment Type Encoding
            ex.PPort.MarkerEncode(ex.Extype[0].Value);
            // Condition Parameter Type Encoding
            ex.PPort.MarkerEncode(ex.Cond[0].SKEY);
            // Condition Number Encoding
            ex.PPort.MarkerEncode(ex.Cond[0].VALUE.ValueN);
            // Random Seed Encoding
            ex.PPort.MarkerEncode(ex.Rand.Seed);
            // Experiment Trials
            ex.PPort.MarkerEncode(ex.Exdesign.trial);

            // Keywords Group Seperator
            ex.PPort.MarkerSeparatorEncode();

            // Custom Parameters Encoding
            bars[0].Para.Encode(ex.PPort);
            ex.PPort.MarkerEncode((int)Math.Floor(bars[0].display_H_deg * 100.0));
            ex.PPort.MarkerEncode((int)Math.Floor(bars[0].display_W_deg * 100.0));
            ex.PPort.MarkerEncode(Rows);
            ex.PPort.MarkerEncode(Columns);
            ex.PPort.MarkerEncode((int)Math.Floor(Rstep * 100.0));
            ex.PPort.MarkerEncode((int)Math.Floor(Cstep * 100.0));

            // End of Header Encoding
            ex.PPort.MarkerEndEncode();
            // Set Timer to begin
            ex.PPort.Timer.Reset();
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
            GraphicsDevice.Clear(ex.Exdesign.bgcolor);

            if (GO_OVER)
            {
                bars[ex.Flow.SliceCount].Draw(GraphicsDevice);
                ex.Flow.Info = ex.Flow.TrialCount.ToString() + " / " + ex.Exdesign.trial.ToString() + " Trials\n" +
                                       ex.Flow.StiCount.ToString() + " / " + ex.Exdesign.stimuli[0].ToString() + " Stimuli";
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
                ex.PPort.Timer.Start();
                // Stimulus Onset Marker
                ex.PPort.Trigger();
                ex.Flow.IsStiOn = false;
            }

            // In Presentation
            if (ex.PPort.Timer.ElapsedSeconds < ex.Flow.StiTime)
            {
                // Set Stimulus Position in Grid only once at the begining of Presentation
                if (!ex.Flow.IsPred)
                {
                    ex.Flow.IsPred = true;

                    ex.Flow.RowCount = (int)Math.Floor(ex.Rand.Sequence[ex.Flow.StiCount] / (Columns * 2.0));
                    int t = ex.Rand.Sequence[ex.Flow.StiCount] % (Columns * 2);
                    ex.Flow.ColumnCount = (int)Math.Floor(t / 2.0);
                    ex.Flow.SliceCount = t % 2;

                    float Xgrid = -(Columns - 1) * Cstep / 2 + Cstep * ex.Flow.ColumnCount;
                    float Ygrid = (Rows - 1) * Rstep / 2 - Rstep * ex.Flow.RowCount;
                    bars[ex.Flow.SliceCount].Ori3DMatrix = Matrix.CreateTranslation(Xgrid, Ygrid, 0.0f) * ex.Flow.RotateOri;
                    bars[ex.Flow.SliceCount].WorldMatrix = ex.Flow.TranslateCenter;

                    ex.Flow.IsStiOn = true;
                }
            }
            else // End of Presentation
            {
                // If Begin Another Stimulus
                if (ex.Flow.StiCount < ex.Exdesign.stimuli[0] - 1)
                {
                    ex.Flow.IsPred = false;
                    ex.Flow.StiCount += 1;
                    ex.PPort.Timer.Reset();
                }
                else
                {
                    // If Begin Another Trial
                    if (ex.Flow.TrialCount < ex.Exdesign.trial - 1)
                    {
                        // Each trial has different random sequence of stimulus
                        ex.Rand.RandomizeSequence(ex.Exdesign.stimuli[0]);
                        ex.Flow.IsPred = false;
                        ex.Flow.TrialCount += 1;
                        ex.Flow.StiCount = 0;
                        ex.PPort.Timer.Reset();
                    }
                    else // End of Experiment
                    {
                        GO_OVER = false;
                        return;
                    }
                }

                // If presentation ended, immediately update the next Stimulus, because of the 
                // 'No Rest' requirement of reverse-correlation technique
                Update();
            }

        }

    }
}