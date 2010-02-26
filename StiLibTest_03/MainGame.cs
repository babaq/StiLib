using System;
using System.Collections.Generic;
using System.Linq;
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

namespace StiLibTest_03
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : SLGame
    {
        Text text;
        FrameInfo frameinfo;
        SLModel model;
        SLAudio audio;
        AudioEmitter audioemitter;
        AudioListener audiolistener;


        public MainGame()
            : base(1, 800, 600, 0, 0, false, false, false, new Vector3(1.0f, 1.0f, 1.0f))
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
            model = new SLModel(GraphicsDevice, Services, "Content", "earth");
            model.Para.BasePara.space = 200;
            model.Para.BasePara.speed3D = Vector3.Backward * 3f;
            model.Para.BasePara.rotationspeed3D = Vector3.UnitY * 0.3f;
            model.ProjectionType = ProjectionType.Perspective;
            model.globalCamera.Position = Vector3.UnitZ * 200;
            model.globalCamera.NearPlane = 1.0f;
            model.globalCamera.FarPlane = 1000f;

            audio = new SLAudio(SLConfig["content"] + "StiLib", SLConfig["content"] + "SLMWB", SLConfig["content"] + "SLSWB", SLConfig["content"] + "SLSB");
            audiolistener = new AudioListener()
                                {
                                    Forward = model.globalCamera.Direction,
                                    Position = model.globalCamera.Position,
                                    Up = model.globalCamera.Up,
                                    Velocity = Vector3.Zero
                                };
            audio.Listeners.Add(audiolistener);
            audioemitter = new AudioEmitter()
                               {
                                   DopplerScale = 1.0f,
                                   Forward = Vector3.Forward,
                                   Position = model.BasePara.center,
                                   Up = Vector3.Up,
                                   Velocity = model.BasePara.speed3D
                               };
            //audio.Update();
            audio.Play("Buzz", audioemitter);
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

            if (Input.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }
            if (Input.IsKeyDown(Keys.F2))
            {
                ToggleFullScreen();
            }

            if (Input.IsKeyDown(Keys.W))
            {
                model.Para.BasePara.center += Vector3.Forward * 0.02f;
                model.WorldMatrix = Matrix.CreateTranslation(model.BasePara.center);
                audioemitter.Position = model.BasePara.center;
            }
            if (Input.IsKeyDown(Keys.S))
            {
                model.Para.BasePara.center += Vector3.Backward * 0.02f;
                model.WorldMatrix = Matrix.CreateTranslation(model.BasePara.center);
                audioemitter.Position = model.BasePara.center;
            }
            if (Input.IsKeyDown(Keys.A))
            {
                model.Para.BasePara.center += Vector3.Left * 0.02f;
                model.WorldMatrix = Matrix.CreateTranslation(model.BasePara.center);
                audioemitter.Position = model.BasePara.center;
            }
            if (Input.IsKeyDown(Keys.D))
            {
                model.Para.BasePara.center += Vector3.Right * 0.02f;
                model.WorldMatrix = Matrix.CreateTranslation(model.BasePara.center);
                audioemitter.Position = model.BasePara.center;
            }

            model.Para.BasePara.orientation3D += model.BasePara.rotationspeed3D * (float)gameTime.ElapsedGameTime.TotalSeconds;
            model.Ori3DMatrix = VisionStimulus.GetOri3DMatrix(model.BasePara.orientation3D);

            if (audio.ActiveCue3Ds.Count > 0)
            {
                audio.ActiveCue3Ds[0].Emitter = audioemitter;
            }
            else
            {
                audio.Play("BgMusic", audioemitter);
            }
            audio.Update();

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
            model.Draw();
        }

    }
}
