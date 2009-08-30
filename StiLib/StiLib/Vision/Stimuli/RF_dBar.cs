#region File Description
//-----------------------------------------------------------------------------
// RF_dBar.cs
//
// StiLib Drifting Bar Receptive Field Mapping Stimulus
// Copyright (c) Zhang Li. 2009-03-09.
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
    /// Drifting Bar RF Mapping
    /// </summary>
    public class RF_dBar : SLForm
    {
        /// <summary>
        /// Init to Default SLForm Settings
        /// </summary>
        public RF_dBar()
            : base()
        {
        }

        /// <summary>
        /// Init to configurations
        /// </summary>
        /// <param name="configfile"></param>
        public RF_dBar(string configfile)
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
        public RF_dBar(int width, int height, int refreshrate, bool isvsync, bool isshowcursor)
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
        /// Drifting Bar
        /// </summary>
        public Bar bar;
        /// <summary>
        /// Scanning Row Number
        /// </summary>
        public int Rows;
        /// <summary>
        /// Scanning Rows Interval
        /// </summary>
        public float Step;
        /// <summary>
        /// Scanning Space
        /// </summary>
        public float ScanSpace;


        /// <summary>
        /// Init all information of the experiment
        /// </summary>
        protected override void Initialize()
        {
            text = new Text(GraphicsDevice, Services, SLConfig["content"], "Arial");

            // Init Experiment Parameters
            ex = new SLExperiment();
            ex.AddExType(ExType.RF_dBar);
            ex.AddCondition(ExPara.Direction, 4);
            ex.Exdesign.trial = 3;
            ex.Exdesign.trestT = 1.0f;
            ex.Exdesign.srestT = 0.5f;
            ex.Exdesign.preT = 0.25f;
            ex.Exdesign.durT = 1.0f;
            ex.Exdesign.posT = 0.25f;
            ex.Exdesign.bgcolor = Color.Black;

            // Init Bar Parameter
            BarPara bpara = BarPara.Default;
            bpara.width = 1.0f;
            bpara.height = 4.0f;
            bpara.BasePara.direction = 0.0f;
            bpara.BasePara.speed = 10.0f;
            bpara.BasePara.space = 10.0f;
            bpara.BasePara.center = new Vector3(0.0f, 0.0f, 0.0f);
            bpara.BasePara.color = Color.White;
            bar = new Bar(GraphicsDevice, bpara);

            Step = 3.0f;
            InitScan();
        }

        /// <summary>
        /// Init Scanning 'Rows' and 'ScanSpace' according to 'Step' and Bar's 'Space'
        /// </summary>
        public void InitScan()
        {
            Rows = (int)Math.Floor(bar.Para.BasePara.space / Step);
            if (Rows % 2 == 0)
            {
                Rows += 1;
            }
            ScanSpace = (Rows - 1) * Step;
        }

        /// <summary>
        /// Set Flow to control experiment
        /// </summary>
        protected override void SetFlow()
        {
            ex.Flow.StiCount = 0;
            ex.Flow.TrialCount = 0;
            ex.Flow.IsPred = false;
            ex.Flow.IsRested = false;
            ex.Flow.PreDurTime = ex.Exdesign.preT + ScanSpace / bar.Para.BasePara.speed;
            ex.Flow.Location = new Vector3((Rows - 1) * Step / 2);
            ex.Flow.StiTime = ex.Flow.PreDurTime + ex.Exdesign.posT;

            ex.Flow.RotateDir = Matrix.CreateRotationZ(bar.Para.BasePara.direction * (float)SLConstant.Rad_p_Deg);
            ex.Flow.TranslateCenter = Matrix.CreateTranslation(bar.Para.BasePara.center);
        }

        /// <summary>
        /// Send information in MarkerHeader 
        /// </summary>
        protected override void MarkHead()
        {
            DrawTip(ref text, ex.Exdesign.bgcolor, SLConstant.MarkHead);

            // Single Condition
            if (ex.Cond[0].VALUE.ValueN == 0)
            {
                ex.Exdesign.stimuli[0] = Rows;
            }
            else // Multiple Conditions
            {
                ex.Exdesign.stimuli[0] = ex.Cond[0].VALUE.ValueN * Rows;
                ex.Flow.CondStep = new float[] { (float)(2 * Math.PI / ex.Cond[0].VALUE.ValueN) };
            }
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
            bar.Para.Encode(ex.PPort);
            ex.PPort.MarkerEncode((int)Math.Floor(bar.display_H_deg * 100.0));
            ex.PPort.MarkerEncode((int)Math.Floor(bar.display_W_deg * 100.0));
            ex.PPort.MarkerEncode(Rows);
            ex.PPort.MarkerEncode((int)Math.Floor(Step * 100.0));

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
                // Single Condition
                if (ex.Cond[0].VALUE.ValueN == 0)
                {
                    Update_sdBar();
                }
                else // Multiple Conditions
                {
                    Update_mdBar();
                }
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
                bar.Draw(GraphicsDevice);

                ex.Flow.Info = ex.Flow.TrialCount.ToString() + " / " + ex.Exdesign.trial.ToString() + " Trials\n" +
                                     ex.Flow.StiCount.ToString() + " / " + ex.Exdesign.stimuli[0].ToString() + " Stimuli";
                text.Draw(ex.Flow.Info);
            }
            else
            {
                text.Draw();
            }
        }

        void Update_sdBar()
        {
            if (ex.Flow.IsStiOn)
            {
                ex.PPort.Timer.Start();
                // Stimulus Onset Marker
                ex.PPort.Trigger();
                ex.Flow.IsStiOn = false;
            }

            if (ex.Flow.IsStiOff)
            {
                // Stimulus Offset Marker
                ex.PPort.Trigger();
                ex.Flow.IsStiOff = false;
                if ((ex.Flow.TrialCount == ex.Exdesign.trial - 1) && (ex.Flow.StiCount == ex.Exdesign.stimuli[0] - 1))
                {
                    GO_OVER = false;
                    return;
                }
            }

            ex.Flow.LastingTime = ex.PPort.Timer.ElapsedSeconds;

            // In Presentation
            if (ex.Flow.LastingTime < ex.Flow.StiTime)
            {
                if (!ex.Flow.IsPred)
                {
                    ex.Flow.IsPred = true;

                    bar.WorldMatrix = Matrix.CreateTranslation(-ScanSpace / 2, ex.Flow.Location.Y - Step * ex.Rand.Sequence[ex.Flow.StiCount], 0.0f) *
                                                  ex.Flow.RotateDir * ex.Flow.TranslateCenter;
                    bar.Visible = true;

                    ex.Flow.IsStiOn = true;
                }

                if (ex.Flow.LastingTime > ex.Exdesign.preT && ex.Flow.LastingTime < ex.Flow.PreDurTime)
                {
                    float MovedDis = (float)(ex.Flow.LastingTime - ex.Exdesign.preT) * bar.Para.BasePara.speed;
                    bar.WorldMatrix = Matrix.CreateTranslation(-ScanSpace / 2 + MovedDis, ex.Flow.Location.Y - Step * ex.Rand.Sequence[ex.Flow.StiCount], 0.0f) *
                                                  ex.Flow.RotateDir * ex.Flow.TranslateCenter;
                }
            }
            else // End of Presentation
            {
                if (!ex.Flow.IsRested)
                {
                    ex.Flow.IsRested = true;
                    bar.Visible = false;
                    ex.Flow.IsStiOff = true;
                }

                if (ex.Flow.StiCount < ex.Exdesign.stimuli[0] - 1)
                {
                    if (ex.Flow.LastingTime > ex.Flow.StiTime + ex.Exdesign.srestT)
                    {
                        ex.Flow.IsPred = false;
                        ex.Flow.IsRested = false;
                        ex.Flow.StiCount += 1;
                        ex.PPort.Timer.Reset();
                    }
                }
                else
                {
                    if (ex.Flow.TrialCount < ex.Exdesign.trial - 1)
                    {
                        if (ex.Flow.LastingTime > ex.Flow.StiTime + ex.Exdesign.trestT)
                        {
                            ex.Rand.RandomizeSequence(ex.Exdesign.stimuli[0]);
                            ex.Flow.IsPred = false;
                            ex.Flow.IsRested = false;
                            ex.Flow.TrialCount += 1;
                            ex.Flow.StiCount = 0;
                            ex.PPort.Timer.Reset();
                        }
                    }
                }
            }
        }

        void Update_mdBar()
        {
            if (ex.Flow.IsStiOn)
            {
                ex.PPort.Timer.Start();
                // Stimulus Onset Marker
                ex.PPort.Trigger();
                ex.Flow.IsStiOn = false;
            }

            if (ex.Flow.IsStiOff)
            {
                // Stimulus Offset Marker
                ex.PPort.Trigger();
                ex.Flow.IsStiOff = false;
                if ((ex.Flow.TrialCount == ex.Exdesign.trial - 1) && (ex.Flow.StiCount == ex.Exdesign.stimuli[0] - 1))
                {
                    GO_OVER = false;
                    return;
                }
            }

            ex.Flow.LastingTime = ex.PPort.Timer.ElapsedSeconds;

            // In Presentation
            if (ex.Flow.LastingTime < ex.Flow.StiTime)
            {
                if (!ex.Flow.IsPred)
                {
                    ex.Flow.IsPred = true;

                    ex.Flow.ColumnCount = (int)Math.Floor((double)ex.Rand.Sequence[ex.Flow.StiCount] / Rows);
                    ex.Flow.RowCount = ex.Rand.Sequence[ex.Flow.StiCount] % Rows;
                    ex.Flow.Direction = ex.Flow.ColumnCount * ex.Flow.CondStep[0];

                    bar.WorldMatrix = Matrix.CreateTranslation(-ScanSpace / 2, ex.Flow.Location.Y - Step * ex.Flow.RowCount, 0.0f) *
                                                  Matrix.CreateRotationZ(ex.Flow.Direction) * ex.Flow.TranslateCenter;
                    bar.Visible = true;

                    ex.Flow.IsStiOn = true;
                }

                if (ex.Flow.LastingTime > ex.Exdesign.preT && ex.Flow.LastingTime < ex.Flow.PreDurTime)
                {
                    float MovedDis = (float)(ex.Flow.LastingTime - ex.Exdesign.preT) * bar.Para.BasePara.speed;
                    bar.WorldMatrix = Matrix.CreateTranslation(-ScanSpace / 2 + MovedDis, ex.Flow.Location.Y - Step * ex.Flow.RowCount, 0.0f) *
                                                  Matrix.CreateRotationZ(ex.Flow.Direction) * ex.Flow.TranslateCenter;
                }
            }
            else // End of Presentation
            {
                if (!ex.Flow.IsRested)
                {
                    ex.Flow.IsRested = true;
                    bar.Visible = false;
                    ex.Flow.IsStiOff = true;
                }

                if (ex.Flow.StiCount < ex.Exdesign.stimuli[0] - 1)
                {
                    if (ex.Flow.LastingTime > ex.Flow.StiTime + ex.Exdesign.srestT)
                    {
                        ex.Flow.IsPred = false;
                        ex.Flow.IsRested = false;
                        ex.Flow.StiCount += 1;
                        ex.PPort.Timer.Reset();
                    }
                }
                else
                {
                    if (ex.Flow.TrialCount < ex.Exdesign.trial - 1)
                    {
                        if (ex.Flow.LastingTime > ex.Flow.StiTime + ex.Exdesign.trestT)
                        {
                            ex.Rand.RandomizeSequence(ex.Exdesign.stimuli[0]);
                            ex.Flow.IsPred = false;
                            ex.Flow.IsRested = false;
                            ex.Flow.TrialCount += 1;
                            ex.Flow.StiCount = 0;
                            ex.PPort.Timer.Reset();
                        }
                    }
                }
            }
        }

    }
}
