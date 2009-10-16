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
using System.ServiceModel;

namespace StiLibTest_02
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class Server : ExService
    {
        public Server()
            : base(1, 800, 600, 0, false, false, true, false, Vector3.One)
        {
        }


        public string serverstate = "Server Started !";

        public SLTimer timer;
        public SLLogger logger;
        public FrameInfo frameinfo;
        public SLAudio audio;

        public Bar bar;
        public Grating grating;
        public Text text;
        public SLModel model;

        public Primitive cross;
        public Primitive arrow;


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
            model = new SLModel(GraphicsDevice, Services, "Content", "earth");

            cross = new Primitive(GraphicsDevice, PrimitivePara.Cross(3f, Color.Green, 4 * Vector3.UnitX));
            arrow = new Primitive(GraphicsDevice, PrimitivePara.Arrow(4f, Color.Red, -4 * Vector3.UnitX, 60f, 1f));

            model.Para.BasePara.rotationspeed3D = Vector3.UnitY;
            model.ProjectionType = ProjectionType.Perspective;
            model.globalCamera.NearPlane = 0.1f;

            timer = new SLTimer();
            timer.Start();
        }

        protected override void Update()
        {
            audio.Update();
            frameinfo.Update();
        }

        protected override void Draw()
        {
            GraphicsDevice.Clear(Color.Gray);

            if (GO_OVER)
            {
                audio.PauseAll();
                model.Ori3DMatrix = Matrix.CreateRotationY((float)timer.ElapsedSeconds);
                model.Draw(GraphicsDevice);
            }
            else
            {
                audio.ResumeAll();

                //bar.Draw(GraphicsDevice);
                grating.SetTime((float)timer.ElapsedSeconds);
                grating.Draw(GraphicsDevice);
                //cross.Draw(GraphicsDevice);
                //arrow.Draw(GraphicsDevice);

                text.Draw(new Vector2(10, 10), SLConstant.Help + "\n" + frameinfo.FPS.ToString() + " FPS", Color.PeachPuff);
                text.Draw(new Vector2(10, 80), serverstate, Color.Red);
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
            if (e.KeyCode == System.Windows.Forms.Keys.C)
            {
                serverstate = OnRunStop(GO_OVER);
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
            }
            if (e.KeyCode == System.Windows.Forms.Keys.S)
            {
                grating.SetGratingType(GratingType.Sinusoidal);
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
        Primitive point;
        Primitive line;
        Primitive linestrip;
        Primitive triangle;
        Primitive rectangle;
        Primitive quadrate;
        Primitive ellipse;
        Primitive circle;
        Primitive grid;


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

            frameinfo = new FrameInfo();
            text = new Text(GraphicsDevice, Services, "Content", "Arial");

            point = new Primitive(GraphicsDevice, PrimitivePara.Point(Vector3.UnitX,Color.White));
            line = new Primitive(GraphicsDevice,PrimitivePara.Line(Vector3.UnitX*3, -Vector3.UnitX*5,Color.Blue,Vector3.UnitY*8,Color.Red));
            linestrip = new Primitive(GraphicsDevice,PrimitivePara.LineStrip(true,Vector3.Zero, Color.Yellow,-Vector3.UnitX*7,Vector3.UnitY*6,Vector3.UnitX*4,-Vector3.UnitY*2));
            triangle = new Primitive(GraphicsDevice,PrimitivePara.Triangle(-Vector3.UnitY*2,Color.Tomato,-Vector3.UnitX,Vector3.UnitX,Vector3.UnitY,true));
            rectangle = new Primitive(GraphicsDevice,PrimitivePara.Rectangle(Vector3.UnitY*2,Color.MistyRose,4f,3f,true));
            quadrate = new Primitive(GraphicsDevice,PrimitivePara.Quadrate(Vector3.UnitX*4,Color.Chocolate,3f,true));
            ellipse = new Primitive(GraphicsDevice,PrimitivePara.Ellipse(4f,3f,Color.Purple,Vector3.UnitY*5,Color.TransparentWhite,100,true));
            circle = new Primitive(GraphicsDevice,PrimitivePara.Circle(3f,Color.Tan,-Vector3.UnitY*8,Color.Green,100,true));
            grid = new Primitive(GraphicsDevice,PrimitivePara.Grid(1.0f,1.5f,12,15,-Vector3.UnitY*3,Color.LightSalmon,Color.PowderBlue));
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

            if (Input.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                this.Exit();
            }
            if (Input.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F2))
            {
                ToggleFullScreen();
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

            grid.Draw();
            point.Draw(GraphicsDevice);
            line.Draw(GraphicsDevice);
            linestrip.Draw();
            triangle.Draw();
            rectangle.Draw();
            quadrate.Draw();
            ellipse.Draw();
            circle.Draw();
            
        }

    }

}
