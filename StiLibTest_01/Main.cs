using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using StiLib.Core;
using StiLib.Vision;

namespace StiLibTest
{
    public partial class Form1 : SLForm
    {
        public Form1()
            : base(2, 800, 600, 0, false, false, true, false, Vector3.One)
        {
        }


        public SLTimer timer;
        public SLLogger logger;
        public FrameInfo frameinfo;
        public SLAudio audio;
        public SLVideo video;

        public Bar bar;
        public Grating grating;
        public Text text;
        public Image image;
        public SLQuad quad;
        public SLModel model;

        public Primitive point;
        public Primitive circle;
        public Primitive disk;
        public Primitive cross;
        public Primitive arrow;
        public Primitive radialcircle;
        public Primitive gaussian;

        public VSCollection<Primitive> vsc;
        public vscPrimitive vscp;


        protected override void Initialize()
        {
            logger = new SLLogger();
            frameinfo = new FrameInfo();
            audio = new SLAudio(SLConfig["content"] + "StiLib", SLConfig["content"] + "SLMWB", SLConfig["content"] + "SLSWB", SLConfig["content"] + "SLSB");
            audio.Update();
            audio.StartBgMusic("BgMusic");

            BarPara bpara = BarPara.Default;
            bpara.width = 4.0f;
            bpara.height = 1.0f;
            bpara.BasePara.direction = 0.0f;
            bpara.BasePara.speed = 10.0f;
            bpara.BasePara.space = 10.0f;
            bpara.BasePara.center = new Vector3(-2.0f, -2.0f, 0.0f);
            bpara.BasePara.color = Color.SeaGreen;
            bar = new Bar(GraphicsDevice, SLConfig, bpara);

            GratingPara gpara = GratingPara.Default;
            gpara.shape = Shape.Circle;
            gpara.gratingtype = GratingType.Sinusoidal;
            gpara.BasePara.center = new Vector3(2.0f, 2.0f, 0.0f);
            gpara.lhcolor = Color.OrangeRed;
            gpara.rlcolor = new Color(0f, 1f, 0f, 1f);
            gpara.BasePara.diameter = 7.0f;
            gpara.sf = 0.5f;
            gpara.contrast = 0.8f;
            gpara.luminance = 0.3f;
            gpara.maskpara.BasePara.diameter = 1.2f;
            grating = new Grating(GraphicsDevice, Services, "Content", gpara);

            text = new Text(GraphicsDevice, Services, "Content", "Arial");
            image = new Image(GraphicsDevice, Services, "Content", "Turtle");
            quad = new SLQuad(GraphicsDevice, Services, "Content", "StiLib_Logo");
            model = new SLModel(GraphicsDevice, Services, "Content", "earth");
            video = new SLVideo(GraphicsDevice, Services, "Content", "Butterfly");

            point = new Primitive(GraphicsDevice, PrimitivePara.Default);
            circle = new Primitive(GraphicsDevice, PrimitivePara.Circle(5f, Color.Azure, Vector3.Zero, 100, false));
            disk = new Primitive(GraphicsDevice, PrimitivePara.Circle(0.3f, Color.DarkBlue, 4 * Vector3.UnitY, 100, true));
            cross = new Primitive(GraphicsDevice, PrimitivePara.Cross(3f, Color.Green, 4 * Vector3.UnitX));
            arrow = new Primitive(GraphicsDevice, PrimitivePara.Arrow(4f, Color.Red, -4 * Vector3.UnitX, 60f, 1f));
            radialcircle = new Primitive(GraphicsDevice, PrimitivePara.RadialCircle(5f, new Color(0f, 0f, 0f, 0f), new Color(0f, 0f, 0f, 0.5f)));
            gaussian = new Primitive(GraphicsDevice, PrimitivePara.Gaussian(10, 1.5f, Color.Gray, Vector3.Zero));

            disk.Para.BasePara.space = 10;
            model.Para.BasePara.rotationspeed3D = Vector3.UnitY;
            model.ProjectionType = ProjectionType.Perspective;
            //model.globalCamera.NearPlane = 0.1f;

            CollectionPara cpara = CollectionPara.Default;
            cpara.BasePara.center = Vector3.UnitX * 5;
            cpara.CollectionCenter = new Vector3(2f, 3f, 0.0f);
            cpara.CollectionSpeed = new Vector3(0.02f, 0.03f, 0.0f);
            vsc = new VSCollection<Primitive>(50, GraphicsDevice, cross, cpara);
            //vscp = new vscPrimitive(100, GraphicsDevice, disk, Services, "Content",cpara);

            timer = new SLTimer();
            timer.Start();

        }

        protected override void Update()
        {
            audio.Update();
            frameinfo.Update();
            vsc.Update(timer.ElapsedSeconds);
            //vscp.Update(timer.ElapsedSeconds);
        }

        protected override void Draw()
        {
            GraphicsDevice.Clear(Color.Gray);

            if (GO_OVER)
            {
                audio.PauseAll();
                video.Play();
                video.Draw();
            }
            else
            {
                audio.ResumeAll();
                video.Stop();

                //bar.Draw(GraphicsDevice);
                grating.SetTime((float)timer.ElapsedSeconds);
                grating.Draw(GraphicsDevice);
                //image.Draw(new Rectangle(210, 100, 400, 200), Color.White);
                //quad.Draw(GraphicsDevice);
                model.Ori3DMatrix = Matrix.CreateRotationY((float)timer.ElapsedSeconds);
                model.Draw(GraphicsDevice);
                //point.Draw(GraphicsDevice);
                circle.Draw(GraphicsDevice);
                //disk.Draw(GraphicsDevice);
                //cross.Draw(GraphicsDevice);
                //arrow.Draw(GraphicsDevice);
                radialcircle.Draw(GraphicsDevice);
                //gaussian.Draw(GraphicsDevice);
                vsc.BatchDraw(GraphicsDevice);
                //vscp.Draw(GraphicsDevice);

                text.Draw(new Vector2(5, 5), SLConstant.Help + "\n" + frameinfo.FPS.ToString() + " FPS", Color.PeachPuff);
                text.Draw(new Vector2(5, 70), "Unicode Text: ¦«¦²¦µ¦¶¦·¦¸¦°", Color.Red, 10f * (float)SLConstant.Rad_p_Deg, Vector2.Zero, Vector2.One);
            }

        }

        protected override void SLForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Escape)
            {
                logger.Log("Total Frames: " + frameinfo.TotalFrames.ToString() + "  \nTotal Time(sec): " + frameinfo.TotalTime.ToString("F3") +
                    "  \nMean IFI(ms): " + (frameinfo.MeanIFI * 1000).ToString("F3") + "  \nSTD IFI(ms): " + (frameinfo.StdIFI * 1000).ToString("F3") +
                    "  \nMax IFI(ms): " + (frameinfo.MaxIFI * 1000).ToString("F3") + "  \nMin IFI(ms): " + (frameinfo.MinIFI * 1000).ToString("F3"));
            }
            if (e.KeyCode == System.Windows.Forms.Keys.T)
            {
                ToggleFullScreen();
            }
            if (e.KeyCode == System.Windows.Forms.Keys.G)
            {
                SetGamma(new Vector3(2.1f, 2.1f, 2.1f));
            }
            if (e.KeyCode == System.Windows.Forms.Keys.A)
            {
                grating.SetGratingType(GratingType.Square);
                vsc.SetVisible(false);
            }
            if (e.KeyCode == System.Windows.Forms.Keys.S)
            {
                grating.SetGratingType(GratingType.Sinusoidal);
                vsc.SetVisible(true);
            }
            if (e.KeyCode == System.Windows.Forms.Keys.D)
                grating.SetGratingType(GratingType.Linear);
            if (e.KeyCode == System.Windows.Forms.Keys.M)
            {
                grating.SetMask(MaskType.Gaussian);
            }
            if (e.KeyCode == System.Windows.Forms.Keys.N)
            {
                grating.SetMask(MaskType.None);
            }
            if (e.KeyCode == System.Windows.Forms.Keys.R)
            {
                vsc.RandomCenterSpeed(1.0f);
            }
            base.SLForm_KeyDown(sender, e);
        }

        protected override void SLForm_MouseMove(object sender, MouseEventArgs e)
        {
            grating.WorldMatrix = Matrix.CreateTranslation((e.X - GraphicsDevice.Viewport.Width / 2) / grating.unitFactor, -(e.Y - GraphicsDevice.Viewport.Height / 2) / grating.unitFactor, 0.0f);
        }

    }



    public class Game1 : SLGame
    {
        Text text;
        FrameInfo frameinfo;
        SLModel model;
        VSCollection<SLModel> vsc;
        SLAudio audio;


        public Game1()
            : base(2, 800, 600, 0, 0, false, false, false, new Vector3(1.0f, 1.0f, 1.0f))
        {
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            audio = new SLAudio(SLConfig["content"] + "StiLib", SLConfig["content"] + "SLMWB", SLConfig["content"] + "SLSWB", SLConfig["content"] + "SLSB");
            audio.Update();
            audio.StartBgMusic("BgMusic");

            frameinfo = new FrameInfo();
            text = new Text(GraphicsDevice, Services, "Content", "Arial");
            model = new SLModel(GraphicsDevice, Services, "Content", "earth");
            model.Para.BasePara.space = 10;
            model.Para.BasePara.rotationspeed3D = Vector3.UnitY;
            model.ProjectionType = ProjectionType.Perspective;
            model.globalCamera.Position = Vector3.UnitZ * 20;

            CollectionPara cpara = CollectionPara.Default;
            cpara.BasePara.center = Vector3.Zero;
            cpara.CollectionCenter = new Vector3(2f, 3f, 0.0f);
            cpara.CollectionSpeed = new Vector3(0.02f, 0.03f, 0.0f);
            vsc = new VSCollection<SLModel>(10, GraphicsDevice, model, cpara);

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Input.Update();
            frameinfo.Update();
            audio.Update();
            vsc.Update(gameTime.TotalGameTime.TotalSeconds);

            if (Input.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                this.Exit();
            }
            if (Input.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F2))
            {
                ToggleFullScreen();
            }
            if (Input.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.G))
            {
                SetGamma(new Vector3(2.1f, 2.1f, 2.1f));
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);
            text.Draw(new Vector2(5, 5), SLConstant.Help + "\n" + frameinfo.FPS.ToString() + " FPS", Color.PeachPuff);
            text.Draw(new Vector2(5, 70), "Unicode Text: ¦«¦²¦µ¦¶¦·¦¸¦°", Color.Red, 10f * (float)SLConstant.Rad_p_Deg, Vector2.Zero, Vector2.One);
            //model.Draw();
            vsc.BatchDraw(GraphicsDevice);

        }

    }

}
