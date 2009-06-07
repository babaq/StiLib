#region File Description
//-----------------------------------------------------------------------------
// Two_dGrating.cs
//
// StiLib Two Drifting Grating Stimulus
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
    /// Two Drifting Grating
    /// </summary>
    public class Two_dGrating : SLForm
    {
        /// <summary>
        /// Default SLForm Settings
        /// </summary>
        public Two_dGrating() : base()
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
        public Grating[] Grating = new Grating[2];
        /// <summary>
        /// Angle between directions of two drifting gratings
        /// </summary>
        public float gratingangle;


        /// <summary>
        /// Init all information of the experiment
        /// </summary>
        protected override void Initialize()
        {
            text = new Text(GraphicsDevice, Services, "Content", "Arial");

            // Init Experiment Parameter
            ex = new SLExperiment();
            ex.AddExType(ExType.Two_dGrating);
            ex.AddCondition(ExPara.Direction, 4);
            ex.Expara.trial = 3;
            ex.Expara.trestT = 1.0f;
            ex.Expara.srestT = 0.5f;
            ex.Expara.preT = 0.25f;
            ex.Expara.durT = 1.0f;
            ex.Expara.posT = 0.25f;
            ex.Expara.bgcolor = Color.Gray;

            // Init Two Gratings Parameter
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
            Grating[0] = new Grating(GraphicsDevice, Services, "Content", gpara);

            gpara.sf = 1.0f;
            gpara.tf = 2.0f;
            gpara.direction = 90.0f;
            gpara.BasePara.diameter = 4.0f;
            gpara.BasePara.center = new Vector3(5.0f, 0.0f, 0.0f);
            gpara.lhcolor = Color.Red;
            gpara.rlcolor = Color.GreenYellow;
            Grating[1] = new Grating(GraphicsDevice, Services, "Content", gpara);

            gratingangle = 90.0f;
        }

        /// <summary>
        /// Set Flow to control experiment
        /// </summary>
        protected override void SetFlow()
        {
            ex.Flow.SCount = 0;
            ex.Flow.TCount = 0;
            ex.Flow.IsPred = false;
            ex.Flow.IsRested = false;
            ex.Flow.IsBlanked = false;
            ex.Flow.PreDurTime = ex.Expara.preT + ex.Expara.durT;
            ex.Flow.StiTime = ex.Flow.PreDurTime + ex.Expara.posT;
        }

        /// <summary>
        /// Send crucial information in MarkerHeader 
        /// </summary>
        protected override void MarkHead()
        {
            // Single Condition
            if (ex.Cond[0].VALUE.ValueN == 0) 
            {
                MarkHead_sdGrating();
            }
            else // Multiple Conditions
            {
                MarkHead_mdGrating();
            }
        }

        void MarkHead_sdGrating()
        {
            ex.Expara.stimuli[0] = 1;

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
            for (int i = 0; i < Grating.Length; i++)
            {
                ex.PPort.MarkerEncode((int)Math.Floor(Grating[i].Para.tf * 10.0));
                ex.PPort.MarkerEncode((int)Math.Floor(Grating[i].Para.sf * 100.0));
                ex.PPort.MarkerEncode((int)Math.Floor((double)Grating[i].Para.direction));
                ex.PPort.MarkerEncode((int)Math.Floor(Grating[i].Para.luminance * 10.0));
                ex.PPort.MarkerEncode((int)Math.Floor(Grating[i].Para.contrast * 10.0));
                ex.PPort.MarkerEncode((int)Math.Floor((Grating[i].Para.BasePara.center.X + 60.0f) * 10.0));
                ex.PPort.MarkerEncode((int)Math.Floor((Grating[i].Para.BasePara.center.Y + 60.0f) * 10.0));
                ex.PPort.MarkerEncode((int)Math.Floor(Grating[i].Para.BasePara.diameter));
            }

            // End of Header Encoding
            ex.PPort.MarkerEndEncode();
            // Set ready to begin
            ex.Flow.IsStiOn = true;
        }

        void MarkHead_mdGrating()
        {
            ex.Expara.stimuli[0] = ex.Cond[0].VALUE.ValueN + 1;
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
            for (int i = 0; i < Grating.Length; i++)
            {
                ex.PPort.MarkerEncode((int)Math.Floor(Grating[i].Para.tf * 10.0));
                ex.PPort.MarkerEncode((int)Math.Floor(Grating[i].Para.sf * 100.0));
                ex.PPort.MarkerEncode((int)Math.Floor(Grating[i].Para.luminance * 10.0));
                ex.PPort.MarkerEncode((int)Math.Floor(Grating[i].Para.contrast * 10.0));
                ex.PPort.MarkerEncode((int)Math.Floor((Grating[i].Para.BasePara.center.X + 60.0f) * 10.0));
                ex.PPort.MarkerEncode((int)Math.Floor((Grating[i].Para.BasePara.center.Y + 60.0f) * 10.0));
                ex.PPort.MarkerEncode((int)Math.Floor(Grating[i].Para.BasePara.diameter));
            }
            // Angle Between Direations of Two Drifting Gratings
            ex.PPort.MarkerEncode((int)Math.Floor((double)gratingangle));

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
            GraphicsDevice.Clear(ex.Expara.bgcolor);

            if (GO_OVER)
            {
                Grating[0].Draw(GraphicsDevice);
                Grating[1].Draw(GraphicsDevice);

                ex.Flow.Info = ex.Flow.TCount.ToString() + " / " + ex.Expara.trial.ToString() + " Trials\n" +
                                     ex.Flow.SCount.ToString() + " / " + ex.Expara.stimuli[0].ToString() + " Stimuli";
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
                ex.Flow.IsStiOn = false;
                ex.PPort.timer.ReStart();
                // Stimulus Onset Marker
                ex.PPort.Trigger();
            }

            ex.Flow.LastTime = ex.PPort.timer.ElapsedSeconds;

            // In Presentation
            if (ex.Flow.LastTime < ex.Flow.StiTime)
            {
                if (!ex.Flow.IsPred)
                {
                    ex.Flow.IsPred = true;

                    for (int i = 0; i < Grating.Length; i++)
                    {
                        ex.Flow.Translate = Matrix.CreateRotationZ((float)(Grating[i].Para.direction * Math.PI / 180.0)) *
                                                     Matrix.CreateTranslation(Grating[i].Para.BasePara.center);
                        Grating[i].SetWorld(ex.Flow.Translate);
                        Grating[i].SetVisible(true);
                    }
                }

                if (ex.Flow.LastTime > ex.Expara.preT && ex.Flow.LastTime < ex.Flow.PreDurTime)
                {
                    for (int i = 0; i < Grating.Length; i++)
                    {
                        Grating[i].SetTime((float)ex.Flow.LastTime - ex.Expara.preT);
                    }
                }
            }
            else // End of Presentation
            {
                if (!ex.Flow.IsRested)
                {
                    // Stimulus Offset Marker
                    ex.PPort.Trigger();

                    ex.Flow.IsRested = true;
                    for (int i = 0; i < Grating.Length; i++)
                    {
                        Grating[i].SetVisible(false);
                    }
                }

                if (ex.Flow.TCount < ex.Expara.trial - 1)
                {
                    if (ex.Flow.LastTime > ex.Flow.StiTime + ex.Expara.trestT)
                    {
                        ex.Flow.IsStiOn = true;
                        ex.Flow.IsPred = false;
                        ex.Flow.IsRested = false;
                        ex.Flow.TCount += 1;
                        // Set Temporal Phase back to zero for new stimulus
                        for (int i = 0; i < Grating.Length; i++)
                        {
                            Grating[i].SetTime(0.0f);
                        }
                    }
                }
                else
                {
                    GO_OVER = false;
                }

            }
        }

        void Update_mdGrating()
        {
            if (ex.Flow.IsStiOn)
            {
                ex.Flow.IsStiOn = false;
                ex.PPort.timer.ReStart();
                // Stimulus Onset Marker
                ex.PPort.Trigger();
            }

            ex.Flow.LastTime = ex.PPort.timer.ElapsedSeconds;

            // In Presentation
            if (ex.Flow.LastTime < ex.Flow.StiTime)
            {
                // Blank Control
                if (ex.Rand.RSequence[ex.Flow.SCount] == 0)
                {
                    if (!ex.Flow.IsBlanked)
                    {
                        ex.Flow.IsBlanked = true;
                        for (int i = 0; i < Grating.Length; i++)
                        {
                            Grating[i].SetVisible(false);
                        }
                    }
                }
                else // Normal Stimulus
                {
                    if (!ex.Flow.IsPred)
                    {
                        ex.Flow.IsPred = true;

                        float rad = (float)((ex.Rand.RSequence[ex.Flow.SCount] - 1) * (2 * Math.PI / ex.Cond[0].VALUE.ValueN));
                        for (int i = 0; i < Grating.Length; i++)
                        {
                            ex.Flow.Translate = Matrix.CreateRotationZ((float)(gratingangle * i * Math.PI / 180.0) + rad) *
                                                         Matrix.CreateTranslation(Grating[i].Para.BasePara.center);
                            Grating[i].SetWorld(ex.Flow.Translate);
                            Grating[i].SetVisible(true);
                        }
                    }

                    if (ex.Flow.LastTime > ex.Expara.preT && ex.Flow.LastTime < ex.Flow.PreDurTime)
                    {
                        for (int i = 0; i < Grating.Length; i++)
                        {
                            Grating[i].SetTime((float)ex.Flow.LastTime - ex.Expara.preT);
                        }
                    }
                }
            }
            else // End of Presentation
            {
                if (!ex.Flow.IsRested)
                {
                    // Stimulus Offset Marker
                    ex.PPort.Trigger();

                    ex.Flow.IsRested = true;
                    for (int i = 0; i < Grating.Length; i++)
                    {
                        Grating[i].SetVisible(false);
                    }
                }

                if (ex.Flow.SCount < ex.Expara.stimuli[0] - 1)
                {
                    if (ex.Flow.LastTime > ex.Flow.StiTime + ex.Expara.srestT)
                    {
                        ex.Flow.IsStiOn = true;
                        ex.Flow.IsPred = false;
                        ex.Flow.IsRested = false;
                        ex.Flow.SCount += 1;
                        // Set Temporal Phase back to zero for new stimulus
                        for (int i = 0; i < Grating.Length; i++)
                        {
                            Grating[i].SetTime(0.0f);
                        }
                    }
                }
                else
                {
                    if (ex.Flow.TCount < ex.Expara.trial - 1)
                    {
                        if (ex.Flow.LastTime > ex.Flow.StiTime + ex.Expara.trestT)
                        {
                            ex.Rand.RandomizeSequence(ex.Expara.stimuli[0]);
                            ex.Flow.IsStiOn = true;
                            ex.Flow.IsPred = false;
                            ex.Flow.IsRested = false;
                            ex.Flow.IsBlanked = false;
                            ex.Flow.TCount += 1;
                            ex.Flow.SCount = 0;
                            // Set Temporal Phase back to zero for new stimulus
                            for (int i = 0; i < Grating.Length; i++)
                            {
                                Grating[i].SetTime(0.0f);
                            }
                        }
                    }
                    else
                    {
                        GO_OVER = false;
                    }
                }
            }
        }

    }
}
