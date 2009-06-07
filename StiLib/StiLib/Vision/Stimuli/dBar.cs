#region File Description
//-----------------------------------------------------------------------------
// dBar.cs
//
// StiLib Drifting Bar Stimulus
// Copyright (c) Zhang Li. 2009-03-12.
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
    /// Drifting Bar
    /// </summary>
    public class dBar : SLForm
    {
        /// <summary>
        /// Default SLForm Settings
        /// </summary>
        public dBar() : base()
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
        public dBar(int width, int height, int refreshrate, bool isvsync, bool isshowcursor)
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
        public Bar Bar;


        /// <summary>
        /// Init all information of the experiment
        /// </summary>
        protected override void Initialize()
        {
            text = new Text(GraphicsDevice, Services, "Content", "Arial");

            // Init Experiment Parameter
            ex = new SLExperiment();
            ex.AddExType(ExType.dBar);
            ex.AddCondition(ExPara.Direction, 4);
            ex.Expara.trial = 3;
            ex.Expara.trestT = 1.0f;
            ex.Expara.srestT = 0.5f;
            ex.Expara.preT = 0.25f;
            ex.Expara.durT = 1.0f;
            ex.Expara.posT = 0.25f;
            ex.Expara.bgcolor = Color.Black;

            // Init Bar Parameter
            BarPara bpara = BarPara.Default;
            bpara.width = 4.0f;
            bpara.height = 1.0f;
            bpara.BasePara.orientation = 90.0f;
            bpara.direction = 0.0f;
            bpara.speed = 10.0f;
            bpara.BasePara.movearea = 10.0f;
            bpara.BasePara.center = new Vector3(-2.0f, -2.0f, 0.0f);
            bpara.BasePara.color = Color.SeaGreen;
            Bar = new Bar(GraphicsDevice, bpara);
        }

        /// <summary>
        /// Set Flow to control experiment
        /// </summary>
        protected override void  SetFlow()
        {
            ex.Flow.SCount = 0;
            ex.Flow.TCount = 0;
            ex.Flow.IsPred = false;
            ex.Flow.IsRested = false;
            ex.Flow.IsBlanked = false;
            ex.Flow.PreDurTime = ex.Expara.preT + Bar.Para.BasePara.movearea / Bar.Para.speed;
            ex.Flow.StiTime = ex.Flow.PreDurTime + ex.Expara.posT;
        }

        /// <summary>
        /// Send crucial information in MarkerHeader 
        /// </summary>
        protected override void  MarkHead()
        {
            // Single Condition
            if (ex.Cond[0].VALUE.ValueN == 0) 
            {
                MarkHead_sdBar();
            }
            else // Multiple Conditions
            {
                MarkHead_mdBar();
            }
        }

        void MarkHead_sdBar()
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
            ex.PPort.MarkerEncode((int)Math.Floor(Bar.Para.height * 10.0));
            ex.PPort.MarkerEncode((int)Math.Floor(Bar.Para.width * 10.0));
            ex.PPort.MarkerEncode((int)Math.Floor((double)Bar.Para.BasePara.orientation));
            ex.PPort.MarkerEncode((int)Math.Floor((double)Bar.Para.direction));
            ex.PPort.MarkerEncode((int)Math.Floor((double)Bar.Para.speed));
            ex.PPort.MarkerEncode((int)Math.Floor((Bar.Para.BasePara.center.X + 60.0f) * 10.0));
            ex.PPort.MarkerEncode((int)Math.Floor((Bar.Para.BasePara.center.Y + 60.0f) * 10.0));

            // End of Header Encoding
            ex.PPort.MarkerEndEncode();
            // Set ready to begin
            ex.Flow.IsStiOn = true;
        }

        void MarkHead_mdBar()
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
            ex.PPort.MarkerEncode((int)Math.Floor(Bar.Para.height * 10.0));
            ex.PPort.MarkerEncode((int)Math.Floor(Bar.Para.width * 10.0));
            ex.PPort.MarkerEncode((int)Math.Floor((double)Bar.Para.BasePara.orientation));
            ex.PPort.MarkerEncode((int)Math.Floor((double)Bar.Para.speed));
            ex.PPort.MarkerEncode((int)Math.Floor((Bar.Para.BasePara.center.X + 60.0f) * 10.0));
            ex.PPort.MarkerEncode((int)Math.Floor((Bar.Para.BasePara.center.Y + 60.0f) * 10.0));

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
                    Update_sdBar();
                }
                else // Multiple Condition
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
            GraphicsDevice.Clear(ex.Expara.bgcolor);

            if (GO_OVER)
            {
                Bar.Draw(GraphicsDevice);

                ex.Flow.Info = ex.Flow.TCount.ToString() + " / " + ex.Expara.trial.ToString() + " Trials\n" +
                                     ex.Flow.SCount.ToString() + " / " + ex.Expara.stimuli[0].ToString() + " Stimuli";
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

                    ex.Flow.Rotate = Matrix.CreateRotationZ((float)(Bar.Para.BasePara.orientation * Math.PI / 180.0)) *
                                             Matrix.CreateTranslation(-Bar.Para.BasePara.movearea / 2, 0.0f, 0.0f) *
                                             Matrix.CreateRotationZ((float)(Bar.Para.direction * Math.PI / 180.0));
                    ex.Flow.Translate = ex.Flow.Rotate * Matrix.CreateTranslation(Bar.Para.BasePara.center);
                    Bar.SetWorld(ex.Flow.Translate);
                    Bar.SetVisible(true);
                }

                if (ex.Flow.LastTime > ex.Expara.preT && ex.Flow.LastTime < ex.Flow.PreDurTime)
                {
                    float MovedDis = (float)(ex.Flow.LastTime - ex.Expara.preT) * Bar.Para.speed;
                    ex.Flow.Rotate = Matrix.CreateRotationZ((float)(Bar.Para.BasePara.orientation * Math.PI / 180.0)) *
                                             Matrix.CreateTranslation(-Bar.Para.BasePara.movearea / 2 + MovedDis, 0.0f, 0.0f) *
                                             Matrix.CreateRotationZ((float)(Bar.Para.direction * Math.PI / 180.0));
                    ex.Flow.Translate = ex.Flow.Rotate * Matrix.CreateTranslation(Bar.Para.BasePara.center);
                    Bar.SetWorld(ex.Flow.Translate);
                }
            }
            else // End of Presentation
            {
                if (!ex.Flow.IsRested)
                {
                    // Stimulus Offset Marker
                    ex.PPort.Trigger(); 

                    ex.Flow.IsRested = true;
                    Bar.SetVisible(false);
                }

                if (ex.Flow.TCount < ex.Expara.trial - 1)
                {
                    if (ex.Flow.LastTime > ex.Flow.StiTime + ex.Expara.trestT)
                    {
                        ex.Flow.IsStiOn = true;
                        ex.Flow.IsPred = false;
                        ex.Flow.IsRested = false;
                        ex.Flow.TCount += 1;
                    }
                }
                else
                {
                    GO_OVER = false;
                }

            }
        }

        void Update_mdBar()
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
                        Bar.SetVisible(false);
                    }
                }
                else // Normal Stimulus
                {
                    float rad = (float)((ex.Rand.RSequence[ex.Flow.SCount] - 1) * (2 * Math.PI / ex.Cond[0].VALUE.ValueN));
                    if (!ex.Flow.IsPred)
                    {
                        ex.Flow.IsPred = true;

                        ex.Flow.Rotate = Matrix.CreateRotationZ((float)(Bar.Para.BasePara.orientation * Math.PI / 180.0)) *
                                                 Matrix.CreateTranslation(-Bar.Para.BasePara.movearea / 2, 0.0f, 0.0f) *
                                                 Matrix.CreateRotationZ(rad);
                        ex.Flow.Translate = ex.Flow.Rotate * Matrix.CreateTranslation(Bar.Para.BasePara.center);
                        Bar.SetWorld(ex.Flow.Translate);
                        Bar.SetVisible(true);
                    }

                    if (ex.Flow.LastTime > ex.Expara.preT && ex.Flow.LastTime < ex.Flow.PreDurTime)
                    {
                        float MovedDis = (float)(ex.Flow.LastTime - ex.Expara.preT) * Bar.Para.speed;
                        ex.Flow.Rotate = Matrix.CreateRotationZ((float)(Bar.Para.BasePara.orientation * Math.PI / 180.0)) *
                                                 Matrix.CreateTranslation(-Bar.Para.BasePara.movearea / 2 + MovedDis, 0.0f, 0.0f) *
                                                 Matrix.CreateRotationZ(rad);
                        ex.Flow.Translate = ex.Flow.Rotate * Matrix.CreateTranslation(Bar.Para.BasePara.center);
                        Bar.SetWorld(ex.Flow.Translate);
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
                    Bar.SetVisible(false);
                }

                if (ex.Flow.SCount < ex.Expara.stimuli[0] - 1)
                {
                    if (ex.Flow.LastTime > ex.Flow.StiTime + ex.Expara.srestT)
                    {
                        ex.Flow.IsStiOn = true;
                        ex.Flow.IsPred = false;
                        ex.Flow.IsRested = false;
                        ex.Flow.SCount += 1;
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
