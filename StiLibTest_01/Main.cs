using System;
using System.IO;
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
            : base(800, 600, 0, false, false)
        {
        }


        public SLTimer timer;
        public SLLogger logger;
        public FrameInfo frameinfo;
        public SLAudio audio;

        public Bar bar;
        public Grating grating;
        public Text text;
        public Image image;
        public SLQuad quad;
        public SLModel model;

        public Primitive points;
        public Primitive circle;
        public Primitive disk;
        public Primitive cross;
        public Primitive arrow;
        public Primitive radialcircle;
        public Primitive gaussian;


        protected override void Initialize()
        {
            timer = new SLTimer();
            timer.Start();
            logger = new SLLogger();
            frameinfo = new FrameInfo();
            audio = new SLAudio("Content\\StiLib", "Content\\SLMWB", "Content\\SLSWB", "Content\\SLSB");
            audio.Update();
            audio.StartBgMusic("BgMusic");

            BarPara bpara = BarPara.Default;
            bpara.width = 4.0f;
            bpara.height = 1.0f;
            bpara.direction = 0.0f;
            bpara.speed = 10.0f;
            bpara.BasePara.movearea = 10.0f;
            bpara.BasePara.center = new Vector3(-2.0f, -2.0f, 0.0f);
            bpara.BasePara.color = Color.SeaGreen;
            bar = new Bar(GraphicsDevice, bpara);

            GratingPara gpara = GratingPara.Default;
            gpara.shape = Shape.Circle;
            gpara.gratingtype = GratingType.Linear;
            gpara.BasePara.center = new Vector3(2.0f, 2.0f, 0.0f);
            gpara.lhcolor = Color.Red;
            gpara.rlcolor = new Color(0f, 1f, 0f, 1f);
            gpara.BasePara.diameter = 10.0f;
            gpara.sf = 0.2f;
            gpara.contrast = 0.8f;
            gpara.luminance = 0.3f;
            grating = new Grating(GraphicsDevice, Services, "Content", gpara);

            text = new Text(GraphicsDevice, Services, "Content", "Arial");
            image = new Image(GraphicsDevice, Services, "Content", "Turtle");
            quad = new SLQuad(GraphicsDevice, Services, "Content", "StiLib_Logo");
            model = new SLModel(GraphicsDevice, Services, "Content", "earth");

            points = new Primitive(GraphicsDevice, PrimitivePara.Default);
            circle = new Primitive(GraphicsDevice, PrimitivePara.Circle(5f, Color.Gray, Vector3.Zero, 100, false));
            disk = new Primitive(GraphicsDevice, PrimitivePara.Circle(5f, Color.DarkBlue, 4 * Vector3.UnitY, 100, true));
            cross = new Primitive(GraphicsDevice, PrimitivePara.Cross(3f, Color.Green, 4 * Vector3.UnitX));
            arrow = new Primitive(GraphicsDevice, PrimitivePara.Arrow(4f, Color.Red, -4 * Vector3.UnitX, 60f, 1f));
            radialcircle = new Primitive(GraphicsDevice, PrimitivePara.RadialCircle(5f, new Color(0f, 0f, 0f, 0f), new Color(0f, 0f, 0f, 0.5f)));
            gaussian = new Primitive(GraphicsDevice, PrimitivePara.Gaussian(10, 1.5f, Color.Gray, Vector3.Zero));

        }

        protected override void Update()
        {
            audio.Update();
        }

        protected override void Draw()
        {
            frameinfo.Update();
            GraphicsDevice.Clear(Color.Gray);

            if (GO_OVER)
            {
            }
            else
            {
                bar.Draw(GraphicsDevice);

                grating.SetTime((float)timer.ElapsedSeconds);
                grating.Draw(GraphicsDevice);

                //image.Draw(new Rectangle(210, 100, 400, 200), Color.White);

                //quad.SetWorld(Matrix.CreateRotationZ((float)timer.ElapsedSeconds));
                //quad.Draw(GraphicsDevice);

                //model.RotateModel(new Vector3(0.0f, (float)timer.ElapsedSeconds, 0.0f));
                //model.gcamera.projtype = ProjectionType.Perspective;
                //model.Draw();

                //points.IndexDraw(GraphicsDevice, PrimitiveType.PointList);
                //circle.IndexDraw(GraphicsDevice, PrimitiveType.LineStrip);
                //disk.IndexDraw(GraphicsDevice, PrimitiveType.TriangleFan);
                //cross.IndexDraw(GraphicsDevice, PrimitiveType.LineList);
                //arrow.IndexDraw(GraphicsDevice, PrimitiveType.LineList);
                //radialcircle.IndexDraw(GraphicsDevice, PrimitiveType.TriangleFan);
                //gaussian.IndexDraw(GraphicsDevice, PrimitiveType.TriangleList);

                text.Draw(new Vector2(5, 5), SLConstant.Help + "\n" + frameinfo.FPS.ToString(), Color.PeachPuff);
                text.Draw(new Vector2(40, 40), "Unicode Text: ¦«¦²¦µ¦¶¦·¦¸¦°", Color.Red, 30f * (float)SLConstant.RadpDeg, Vector2.Zero, 1.0f);
            }

        }

        protected override void SLForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Escape)
            {
                logger.Log("Total Frames: " + frameinfo.TotalFrames.ToString() + "  \nTotal Time: " + frameinfo.TotalTime.ToString() +
                    "  \nMean IFI: " + frameinfo.MeanIFI + "  \nSTD IFI: " + frameinfo.StdIFI + "  \nMax IFI: " + frameinfo.MaxIFI + "  \nMin IFI: " + frameinfo.MinIFI);
            }
            if (e.KeyCode == System.Windows.Forms.Keys.A)
                grating.SetGratingType(GratingType.Square);
            if (e.KeyCode == System.Windows.Forms.Keys.S)
                grating.SetGratingType(GratingType.Sinusoidal);
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
            grating.SetWorld(Matrix.CreateTranslation((e.X - GraphicsDevice.Viewport.Width / 2) / grating.ufactor, -(e.Y - GraphicsDevice.Viewport.Height / 2) / grating.ufactor, 0.0f));
        }

    }

    //public class Game1 : Game
    //{
    //    GraphicsDeviceManager graphics;
    //    SpriteBatch spriteBatch;

    //    public Game1()
    //    {
    //        graphics = new GraphicsDeviceManager(this);
    //        Content.RootDirectory = "Content";
    //    }

    //    /// <summary>
    //    /// Allows the game to perform any initialization it needs to before starting to run.
    //    /// This is where it can query for any required services and load any non-graphic
    //    /// related content.  Calling base.Initialize will enumerate through any components
    //    /// and initialize them as well.
    //    /// </summary>
    //    protected override void Initialize()
    //    {
    //        // TODO: Add your initialization logic here

    //        base.Initialize();
    //    }

    //    /// <summary>
    //    /// LoadContent will be called once per game and is the place to load
    //    /// all of your content.
    //    /// </summary>
    //    protected override void LoadContent()
    //    {
    //        // Create a new SpriteBatch, which can be used to draw textures.
    //        spriteBatch = new SpriteBatch(GraphicsDevice);

    //        // TODO: use this.Content to load your game content here
    //    }

    //    /// <summary>
    //    /// UnloadContent will be called once per game and is the place to unload
    //    /// all content.
    //    /// </summary>
    //    protected override void UnloadContent()
    //    {
    //        // TODO: Unload any non ContentManager content here
    //    }

    //    /// <summary>
    //    /// Allows the game to run logic such as updating the world,
    //    /// checking for collisions, gathering input, and playing audio.
    //    /// </summary>
    //    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    //    protected override void Update(GameTime gameTime)
    //    {
    //        // Allows the game to exit
    //        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
    //            this.Exit();

    //        // TODO: Add your update logic here

    //        base.Update(gameTime);
    //    }

    //    /// <summary>
    //    /// This is called when the game should draw itself.
    //    /// </summary>
    //    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    //    protected override void Draw(GameTime gameTime)
    //    {
    //        GraphicsDevice.Clear(Color.CornflowerBlue);

    //        // TODO: Add your drawing code here

    //        base.Draw(gameTime);
    //    }
    //}
}
