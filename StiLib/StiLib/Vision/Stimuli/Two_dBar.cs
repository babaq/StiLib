#region File Description
//-----------------------------------------------------------------------------
// Two_dBar.cs
//
// StiLib Two Drifting Bar Stimulus
// Copyright (c) Zhang Li. 2008-10-22.
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
    /// Two Drifting Bar
    /// </summary>
    public class Two_dBar : SLForm
    {
        /// <summary>
        /// Default SLForm Settings
        /// </summary>
        public Two_dBar()
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
        public Two_dBar(int width, int height, int refreshrate, bool isvsync, bool isshowcursor)
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
        /// Two Drifting Bars
        /// </summary>
        public Bar[] Bar = new Bar[2];
        /// <summary>
        /// Angle between directions of two drifting bars
        /// </summary>
        public float barangle;


        /// <summary>
        /// Init all information of the experiment
        /// </summary>
        protected override void Initialize()
        {
            text = new Text(GraphicsDevice, Services, SLConfig["content"], "Arial");

            // Init Experiment Parameter
            ex = new SLExperiment();
            ex.AddExType(ExType.Two_dBar);
            ex.AddCondition(ExPara.Direction, 4);
            ex.Expara.trial = 3;
            ex.Expara.trestT = 1.0f;
            ex.Expara.srestT = 0.5f;
            ex.Expara.preT = 0.25f;
            ex.Expara.durT = 1.0f;
            ex.Expara.posT = 0.25f;
            ex.Expara.bgcolor = Color.Black;

            // Init Two Bars Parameter
            BarPara bpara = BarPara.Default;
            bpara.width = 4.0f;
            bpara.height = 1.0f;
            bpara.BasePara.orientation = 90.0f;
            bpara.direction = 0.0f;
            bpara.speed = 10.0f;
            bpara.BasePara.movearea = 8.0f;
            bpara.BasePara.center = new Vector3(-2.0f, -2.0f, 0.0f);
            bpara.BasePara.color = Color.SeaGreen;
            Bar[0] = new Bar(GraphicsDevice, bpara);

            bpara.BasePara.center = new Vector3(2.0f, 2.0f, 0.0f);
            bpara.BasePara.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
            Bar[1] = new Bar(GraphicsDevice, bpara);

            barangle = 90.0f;
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
            ex.Flow.PreDurTime = ex.Expara.preT + Bar[0].Para.BasePara.movearea / Bar[0].Para.speed;
            ex.Flow.StiTime = ex.Flow.PreDurTime + ex.Expara.posT;
        }

        /// <summary>
        /// Send crucial information in MarkerHeader 
        /// </summary>
        protected override void MarkHead()
        {
            DrawTip(ref text, ex.Expara.bgcolor, SLConstant.MarkHead);

            ex.Expara.stimuli[0] = ex.Cond[0].VALUE.ValueN + 1;
            if (ex.Expara.stimuli[0] > 1)
            {
                ex.Rand.RandomizeSeed();
                ex.Rand.RandomizeSequence(ex.Expara.stimuli[0]);
            }

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
            for (int i = 0; i < Bar.Length; i++)
            {
                Bar[i].Para.Encode(ex.PPort);
            }

            if (ex.Expara.stimuli[0] > 1)
            {
                // Angle Between Direations of Two Drifting Bars
                ex.PPort.MarkerEncode((int)Math.Floor((double)barangle));
            }

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
            GraphicsDevice.Clear(ex.Expara.bgcolor);

            if (GO_OVER)
            {
                Bar[0].Draw(GraphicsDevice);
                Bar[1].Draw(GraphicsDevice);

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

                    for (int i = 0; i < Bar.Length; i++)
                    {
                        ex.Flow.Rotate = Matrix.CreateRotationZ((float)(Bar[i].Para.BasePara.orientation * Math.PI / 180.0)) *
                                                 Matrix.CreateTranslation(-Bar[i].Para.BasePara.movearea / 2, 0.0f, 0.0f) *
                                                 Matrix.CreateRotationZ((float)(Bar[i].Para.direction * Math.PI / 180.0));
                        ex.Flow.Translate = ex.Flow.Rotate * Matrix.CreateTranslation(Bar[i].Para.BasePara.center);
                        Bar[i].SetWorld(ex.Flow.Translate);
                        Bar[i].SetVisible(true);
                    }
                }

                if (ex.Flow.LastTime > ex.Expara.preT && ex.Flow.LastTime < ex.Flow.PreDurTime)
                {
                    for (int i = 0; i < Bar.Length; i++)
                    {
                        float MovedDis = (float)(ex.Flow.LastTime - ex.Expara.preT) * Bar[i].Para.speed;
                        ex.Flow.Rotate = Matrix.CreateRotationZ((float)(Bar[i].Para.BasePara.orientation * Math.PI / 180.0)) *
                                                 Matrix.CreateTranslation(-Bar[i].Para.BasePara.movearea / 2 + MovedDis, 0.0f, 0.0f) *
                                                 Matrix.CreateRotationZ((float)(Bar[i].Para.direction * Math.PI / 180.0));
                        ex.Flow.Translate = ex.Flow.Rotate * Matrix.CreateTranslation(Bar[i].Para.BasePara.center);
                        Bar[i].SetWorld(ex.Flow.Translate);
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
                    for (int i = 0; i < Bar.Length; i++)
                    {
                        Bar[i].SetVisible(false);
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
                        for (int i = 0; i < Bar.Length; i++)
                        {
                            Bar[i].SetVisible(false);
                        }
                    }
                }
                else // Normal Stimulus
                {
                    float rad = (float)((ex.Rand.RSequence[ex.Flow.SCount] - 1) * (2 * Math.PI / ex.Cond[0].VALUE.ValueN));
                    if (!ex.Flow.IsPred)
                    {
                        ex.Flow.IsPred = true;

                        for (int i = 0; i < Bar.Length; i++)
                        {
                            ex.Flow.Rotate = Matrix.CreateRotationZ((float)(Bar[i].Para.BasePara.orientation * Math.PI / 180.0)) *
                                                     Matrix.CreateTranslation(-Bar[i].Para.BasePara.movearea / 2, 0.0f, 0.0f) *
                                                     Matrix.CreateRotationZ((float)(barangle * i * Math.PI / 180.0) + rad);
                            ex.Flow.Translate = ex.Flow.Rotate * Matrix.CreateTranslation(Bar[i].Para.BasePara.center);
                            Bar[i].SetWorld(ex.Flow.Translate);
                            Bar[i].SetVisible(true);
                        }
                    }

                    if (ex.Flow.LastTime > ex.Expara.preT && ex.Flow.LastTime < ex.Flow.PreDurTime)
                    {
                        for (int i = 0; i < Bar.Length; i++)
                        {
                            float MovedDis = (float)(ex.Flow.LastTime - ex.Expara.preT) * Bar[i].Para.speed;
                            ex.Flow.Rotate = Matrix.CreateRotationZ((float)(Bar[i].Para.BasePara.orientation * Math.PI / 180.0)) *
                                                     Matrix.CreateTranslation(-Bar[i].Para.BasePara.movearea / 2 + MovedDis, 0.0f, 0.0f) *
                                                     Matrix.CreateRotationZ((float)(barangle * i * Math.PI / 180.0) + rad);
                            ex.Flow.Translate = ex.Flow.Rotate * Matrix.CreateTranslation(Bar[i].Para.BasePara.center);
                            Bar[i].SetWorld(ex.Flow.Translate);
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
                    for (int i = 0; i < Bar.Length; i++)
                    {
                        Bar[i].SetVisible(false);
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
