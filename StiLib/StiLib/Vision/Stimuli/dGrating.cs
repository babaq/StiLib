#region File Description
//-----------------------------------------------------------------------------
// dGrating.cs
//
// StiLib Drifting Grating Stimulus
// Copyright (c) Zhang Li. 2009-03-11.
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
    /// Drifting Grating Stimulus
    /// </summary>
    public class dGrating : SLForm
    {
        /// <summary>
        /// Init to Default SLForm Settings
        /// </summary>
        public dGrating()
            : base()
        {
        }

        /// <summary>
        /// Init to configurations
        /// </summary>
        /// <param name="configfile"></param>
        public dGrating(string configfile)
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
        public dGrating(int width, int height, int refreshrate, bool isvsync, bool isshowcursor)
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
        /// Drifting Grating
        /// </summary>
        public Grating grating;


        /// <summary>
        /// Init all information of the experiment
        /// </summary>
        protected override void Initialize()
        {
            text = new Text(GraphicsDevice, Services, SLConfig["content"], "Arial");

            // Init Experiment Parameters
            ex = new SLExperiment();
            ex.AddExType(ExType.dGrating);
            ex.AddCondition(ExPara.Direction, 4);
            ex.Exdesign.trial = 3;
            ex.Exdesign.trestT = 1.0f;
            ex.Exdesign.srestT = 0.5f;
            ex.Exdesign.preT = 0.25f;
            ex.Exdesign.durT = 1.0f;
            ex.Exdesign.posT = 0.25f;
            ex.Exdesign.bgcolor = Color.Gray;

            // Init Grating Parameters
            GratingPara gpara = GratingPara.Default;
            gpara.gratingtype = GratingType.Sinusoidal;
            gpara.shape = Shape.Circle;
            gpara.tf = 2.0f;
            gpara.sf = 1.0f;
            gpara.luminance = 0.5f;
            gpara.contrast = 1.0f;
            gpara.BasePara.diameter = 5.0f;
            gpara.BasePara.center = new Vector3(0.0f, 0.0f, 0.0f);
            gpara.lhcolor = Color.RosyBrown;
            gpara.rlcolor = Color.Blue;
            grating = new Grating(GraphicsDevice, Services, SLConfig["content"], gpara);
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
            ex.Flow.IsBlanked = false;
            ex.Flow.PreDurTime = ex.Exdesign.preT + ex.Exdesign.durT;
            ex.Flow.StiTime = ex.Flow.PreDurTime + ex.Exdesign.posT;

            ex.Flow.RotateDir = Matrix.CreateRotationZ(grating.Para.BasePara.direction * (float)SLConstant.Rad_p_Deg);
            ex.Flow.TranslateCenter = Matrix.CreateTranslation(grating.Para.BasePara.center);
        }

        /// <summary>
        /// Send information in MarkerHeader 
        /// </summary>
        protected override void MarkHead()
        {
            DrawTip(ref text, ex.Exdesign.bgcolor, SLConstant.MarkHead);

            ex.Exdesign.stimuli[0] = ex.Cond[0].VALUE.ValueN + 1;
            if (ex.Exdesign.stimuli[0] > 1)
            {
                ex.Rand.RandomizeSequence(ex.Exdesign.stimuli[0]);
                ex.Flow.CondStep = new float[] { (float)(2 * Math.PI / ex.Cond[0].VALUE.ValueN) };
            }

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
            grating.Para.Encode(ex.PPort);

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
                    Update_sdGrating();
                }
                else // Multiple Conditions
                {
                    Update_mdGrating();
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
                grating.Draw(GraphicsDevice);

                ex.Flow.Info = ex.Flow.TrialCount.ToString() + " / " + ex.Exdesign.trial.ToString() + " Trials\n" +
                                     ex.Flow.StiCount.ToString() + " / " + ex.Exdesign.stimuli[0].ToString() + " Stimuli";
                text.Draw(ex.Flow.Info);
            }
            else
            {
                text.Draw();
            }
        }

        void Update_sdGrating()
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
                if (ex.Flow.TrialCount == ex.Exdesign.trial - 1)
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

                    grating.Ori3DMatrix = ex.Flow.RotateDir;
                    grating.WorldMatrix = ex.Flow.TranslateCenter;
                    grating.Visible = true;

                    ex.Flow.IsStiOn = true;
                }

                if (ex.Flow.LastingTime > ex.Exdesign.preT && ex.Flow.LastingTime < ex.Flow.PreDurTime)
                {
                    grating.SetTime((float)ex.Flow.LastingTime - ex.Exdesign.preT);
                }
            }
            else // End of Presentation
            {
                if (!ex.Flow.IsRested)
                {
                    ex.Flow.IsRested = true;
                    grating.Visible = false;
                    ex.Flow.IsStiOff = true;
                }

                if (ex.Flow.TrialCount < ex.Exdesign.trial - 1)
                {
                    if (ex.Flow.LastingTime > ex.Flow.StiTime + ex.Exdesign.trestT)
                    {
                        ex.Flow.IsPred = false;
                        ex.Flow.IsRested = false;
                        ex.Flow.TrialCount += 1;
                        // Set Temporal Phase back to zero for new stimulus
                        grating.SetTime(0.0f);
                        ex.PPort.Timer.Reset();
                    }
                }
            }
        }

        void Update_mdGrating()
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
                // Blank Control
                if (ex.Rand.Sequence[ex.Flow.StiCount] == 0)
                {
                    if (!ex.Flow.IsBlanked)
                    {
                        ex.Flow.IsBlanked = true;
                        grating.Visible = false;
                        ex.Flow.IsStiOn = true;
                    }
                }
                else // Normal Stimulus
                {
                    if (!ex.Flow.IsPred)
                    {
                        ex.Flow.IsPred = true;

                        grating.Ori3DMatrix = Matrix.CreateRotationZ((ex.Rand.Sequence[ex.Flow.StiCount] - 1) * ex.Flow.CondStep[0]);
                        grating.WorldMatrix = ex.Flow.TranslateCenter;
                        grating.Visible = true;
                        ex.Flow.IsStiOn = true;
                    }

                    if (ex.Flow.LastingTime > ex.Exdesign.preT && ex.Flow.LastingTime < ex.Flow.PreDurTime)
                    {
                        grating.SetTime((float)ex.Flow.LastingTime - ex.Exdesign.preT);
                    }
                }
            }
            else // End of Presentation
            {
                if (!ex.Flow.IsRested)
                {
                    ex.Flow.IsRested = true;
                    grating.Visible = false;
                    ex.Flow.IsStiOff = true;
                }

                if (ex.Flow.StiCount < ex.Exdesign.stimuli[0] - 1)
                {
                    if (ex.Flow.LastingTime > ex.Flow.StiTime + ex.Exdesign.srestT)
                    {
                        ex.Flow.IsPred = false;
                        ex.Flow.IsRested = false;
                        ex.Flow.StiCount += 1;
                        // Set Temporal Phase back to zero for new stimulus
                        grating.SetTime(0.0f);
                        ex.PPort.Timer.Reset();
                    }
                }
                else
                {
                    if (ex.Flow.TrialCount < ex.Exdesign.trial - 1)
                    {
                        if (ex.Flow.LastingTime > ex.Flow.StiTime + ex.Exdesign.trestT)
                        {
                            // Each trial has different random sequence of stimulus
                            ex.Rand.RandomizeSequence(ex.Exdesign.stimuli[0]);
                            ex.Flow.IsPred = false;
                            ex.Flow.IsRested = false;
                            ex.Flow.IsBlanked = false;
                            ex.Flow.TrialCount += 1;
                            ex.Flow.StiCount = 0;
                            // Set Temporal Phase back to zero for new stimulus
                            grating.SetTime(0.0f);
                            ex.PPort.Timer.Reset();
                        }
                    }
                }
            }
        }

    }
}
