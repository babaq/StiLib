#region File Description
//-----------------------------------------------------------------------------
// Two_dGrating.cs
//
// StiLib Two Drifting Gratings Stimulus
// Copyright (c) Zhang Li. 2008-09-21.
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
    /// Two Drifting Gratings
    /// </summary>
    public class Two_dGrating : SLForm
    {
        /// <summary>
        /// Init to Default SLForm Settings
        /// </summary>
        public Two_dGrating()
            : base()
        {
        }

        /// <summary>
        /// Init to configurations
        /// </summary>
        /// <param name="configfile"></param>
        public Two_dGrating(string configfile)
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
        public Two_dGrating(int width, int height, int refreshrate, bool isvsync, bool isshowcursor)
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
        /// Two Drifting Gratings
        /// </summary>
        public Grating[] gratings = new Grating[2];
        /// <summary>
        /// Angle between directions of two drifting gratings
        /// </summary>
        public float gratingangle;


        /// <summary>
        /// Init all information of the experiment
        /// </summary>
        protected override void Initialize()
        {
            text = new Text(GraphicsDevice, Services, SLConfig["content"], "Arial");

            // Init Experiment Parameters
            ex = new SLExperiment();
            ex.AddExType(ExType.Two_dGrating);
            ex.AddCondition(ExPara.Direction, 4);
            ex.Exdesign.trial = 3;
            ex.Exdesign.trestT = 1.0f;
            ex.Exdesign.srestT = 0.5f;
            ex.Exdesign.preT = 0.25f;
            ex.Exdesign.durT = 1.0f;
            ex.Exdesign.posT = 0.25f;
            ex.Exdesign.bgcolor = Color.Gray;

            // Init Two Gratings Parameters
            GratingPara gpara = GratingPara.Default;
            gpara.gratingtype = GratingType.Sinusoidal;
            gpara.shape = Shape.Circle;
            gpara.sf = 0.5f;
            gpara.tf = 3.0f;
            gpara.sphase = 0.0f;
            gpara.luminance = 0.5f;
            gpara.contrast = 1.0f;
            gpara.BasePara.diameter = 5.0f;
            gpara.BasePara.center = new Vector3(-5.0f, 0.0f, 0.0f);
            gpara.lhcolor = Color.RosyBrown;
            gpara.rlcolor = Color.Blue;
            gratings[0] = new Grating(GraphicsDevice, Services, SLConfig["content"], gpara);

            gpara.sf = 1.0f;
            gpara.tf = 2.0f;
            gpara.BasePara.direction = 90.0f;
            gpara.BasePara.diameter = 4.0f;
            gpara.BasePara.center = new Vector3(5.0f, 0.0f, 0.0f);
            gpara.lhcolor = Color.Red;
            gpara.rlcolor = Color.GreenYellow;
            gratings[1] = new Grating(GraphicsDevice, Services, SLConfig["content"], gpara);

            gratingangle = 90.0f;
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
            ex.Flow.IsStiOn = false;
            ex.Flow.IsStiOff = false;
            ex.Flow.PreDurTime = ex.Exdesign.preT + ex.Exdesign.durT;
            ex.Flow.StiTime = ex.Flow.PreDurTime + ex.Exdesign.posT;
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
            for (int i = 0; i < gratings.Length; i++)
            {
                gratings[i].Para.Encode(ex.PPort);
            }

            // Angle Between Direations of Two Drifting Gratings
            ex.PPort.MarkerEncode((int)Math.Floor(gratingangle * 100.0));

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
                gratings[0].Draw(GraphicsDevice);
                gratings[1].Draw(GraphicsDevice);

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

                    for (int i = 0; i < gratings.Length; i++)
                    {
                        gratings[i].Ori3DMatrix = Matrix.CreateRotationZ(gratings[i].Para.BasePara.direction * (float)SLConstant.Rad_p_Deg);
                        gratings[i].WorldMatrix = Matrix.CreateTranslation(gratings[i].Para.BasePara.center);
                        gratings[i].Visible = true;
                    }
                    ex.Flow.IsStiOn = true;
                }

                if (ex.Flow.LastingTime > ex.Exdesign.preT && ex.Flow.LastingTime < ex.Flow.PreDurTime)
                {
                    for (int i = 0; i < gratings.Length; i++)
                    {
                        gratings[i].SetTime((float)ex.Flow.LastingTime - ex.Exdesign.preT);
                    }
                }
            }
            else // End of Presentation
            {
                if (!ex.Flow.IsRested)
                {
                    ex.Flow.IsRested = true;
                    for (int i = 0; i < gratings.Length; i++)
                    {
                        gratings[i].Visible = false;
                    }
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
                        for (int i = 0; i < gratings.Length; i++)
                        {
                            gratings[i].SetTime(0.0f);
                        }
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
                        for (int i = 0; i < gratings.Length; i++)
                        {
                            gratings[i].Visible = false;
                        }
                        ex.Flow.IsStiOn = true;
                    }
                }
                else // Normal Stimulus
                {
                    if (!ex.Flow.IsPred)
                    {
                        ex.Flow.IsPred = true;
                        ex.Flow.Direction = (ex.Rand.Sequence[ex.Flow.StiCount] - 1) * ex.Flow.CondStep[0];

                        for (int i = 0; i < gratings.Length; i++)
                        {
                            gratings[i].Ori3DMatrix = Matrix.CreateRotationZ(gratingangle * i * (float)SLConstant.Rad_p_Deg + ex.Flow.Direction);
                            gratings[i].WorldMatrix = Matrix.CreateTranslation(gratings[i].Para.BasePara.center);
                            gratings[i].Visible = true;
                        }
                        ex.Flow.IsStiOn = true;
                    }

                    if (ex.Flow.LastingTime > ex.Exdesign.preT && ex.Flow.LastingTime < ex.Flow.PreDurTime)
                    {
                        for (int i = 0; i < gratings.Length; i++)
                        {
                            gratings[i].SetTime((float)ex.Flow.LastingTime - ex.Exdesign.preT);
                        }
                    }
                }
            }
            else // End of Presentation
            {
                if (!ex.Flow.IsRested)
                {
                    ex.Flow.IsRested = true;
                    for (int i = 0; i < gratings.Length; i++)
                    {
                        gratings[i].Visible = false;
                    }
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
                        for (int i = 0; i < gratings.Length; i++)
                        {
                            gratings[i].SetTime(0.0f);
                        }
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
                            ex.Flow.IsBlanked = false;
                            ex.Flow.TrialCount += 1;
                            ex.Flow.StiCount = 0;
                            // Set Temporal Phase back to zero for new stimulus
                            for (int i = 0; i < gratings.Length; i++)
                            {
                                gratings[i].SetTime(0.0f);
                            }
                            ex.PPort.Timer.Reset();
                        }
                    }
                }
            }
        }

    }
}
