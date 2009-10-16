#region File Description
//-----------------------------------------------------------------------------
// SLVideo.cs
//
// StiLib Video
// Copyright (c) Zhang Li. 2009-06-19.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using System.Windows.Forms;
using StiLib.Core;
#endregion

namespace StiLib.Vision
{
    using MediaState = Microsoft.Xna.Framework.Media.MediaState;
    /// <summary>
    /// StiLib Video
    /// </summary>
    public class SLVideo : VisionStimulus
    {
        #region Fields

        /// <summary>
        /// SLVideo Parameters
        /// </summary>
        public VideoPara Para;
        /// <summary>
        /// Video Frame Texture
        /// </summary>
        public Texture2D texture;
        /// <summary>
        /// Loaded Video
        /// </summary>
        public Video video;
        VideoPlayer videoplayer;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Video Player
        /// </summary>
        public VideoPlayer Player
        {
            get { return videoplayer; }
        }

        /// <summary>
        /// Gets Video State
        /// </summary>
        public MediaState MediaState
        {
            get { return videoplayer.State; }
        }

        /// <summary>
        /// Basic Parameters
        /// </summary>
        public override vsBasePara BasePara
        {
            get { return Para.BasePara; }
            set { Para.BasePara = value; }
        }

        /// <summary>
        /// Center
        /// </summary>
        public override Vector3 Center
        {
            get { return Para.BasePara.center; }
            set { Para.BasePara.center = value; }
        }

        /// <summary>
        /// Speed3D
        /// </summary>
        public override Vector3 Speed3D
        {
            get { return Para.BasePara.speed3D; }
            set { Para.BasePara.speed3D = value; }
        }

        /// <summary>
        /// Visible State
        /// </summary>
        public override bool Visible
        {
            get { return Para.BasePara.visible; }
            set { Para.BasePara.visible = value; }
        }

        #endregion


        /// <summary>
        /// Sets Default VideoPara, need LoadContent() and Init()
        /// </summary>
        public SLVideo()
        {
            Para = new VideoPara("");
        }

        /// <summary>
        /// Init with Default VideoPara and Custom Video Name
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="videoname"></param>
        public SLVideo(GraphicsDevice gd, IServiceProvider service, string path, string videoname)
            : base(gd)
        {
            Para = new VideoPara(videoname);
            LoadContent(service, path, videoname);
            Init(gd);
        }

        /// <summary>
        /// Init with Custom VideoPara
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="videopara"></param>
        public SLVideo(GraphicsDevice gd, IServiceProvider service, string path, VideoPara videopara)
            : base(gd)
        {
            Para = videopara;
            LoadContent(service, path, videopara.BasePara.contentname);
            Init(gd);
        }

        /// <summary>
        /// Init with Custom VideoPara and Configuration
        /// </summary>
        /// <param name="distance2display"></param>
        /// <param name="displayratio"></param>
        /// <param name="displaysize"></param>
        /// <param name="camera"></param>
        /// <param name="unit"></param>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="videopara"></param>
        public SLVideo(float distance2display, float displayratio, float displaysize, SLCamera camera, Unit unit, GraphicsDevice gd, IServiceProvider service, string path, VideoPara videopara)
            : base(distance2display, displayratio, displaysize, gd, camera, unit)
        {
            Para = videopara;
            LoadContent(service, path, videopara.BasePara.contentname);
            Init(gd);
        }

        /// <summary>
        /// Init with Custom VideoPara and StiLib Configuration File
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="slconfig"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="videopara"></param>
        public SLVideo(GraphicsDevice gd, AssemblySettings slconfig, IServiceProvider service, string path, VideoPara videopara)
            : base(gd, slconfig)
        {
            Para = videopara;
            LoadContent(service, path, videopara.BasePara.contentname);
            Init(gd);
        }

        /// <summary>
        /// Init with Custom VideoPara and Video Name
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="videoname"></param>
        /// <param name="videopara"></param>
        public SLVideo(GraphicsDevice gd, IServiceProvider service, string path, string videoname, VideoPara videopara)
            : base(gd)
        {
            Para = videopara;
            LoadContent(service, path, videoname);
            Init(gd);
        }


        /// <summary>
        /// Load the Video
        /// </summary>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="videoname"></param>
        public override void LoadContent(IServiceProvider service, string path, string videoname)
        {
            contentManager = new ContentManager(service, path);
            try
            {
                video = contentManager.Load<Video>(videoname);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error !");
            }

            Para.BasePara.contentname = videoname;
        }

        /// <summary>
        /// Init according to internal parameters
        /// </summary>
        /// <param name="gd"></param>
        public override void Init(GraphicsDevice gd)
        {
            InitVS(gd);
            spriteBatch = new SpriteBatch(gd);
            videoplayer = new VideoPlayer();
        }

        /// <summary>
        /// Init with Custom VideoPara
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="videopara"></param>
        public void Init(GraphicsDevice gd, VideoPara videopara)
        {
            videopara.BasePara.contentname = Para.BasePara.contentname;
            Para = videopara;
            Init(gd);
        }

        /// <summary>
        /// Draw Video
        /// </summary>
        /// <param name="gd"></param>
        public override void Draw(GraphicsDevice gd)
        {
            if (Para.BasePara.visible)
            {
                if (videoplayer.State == MediaState.Stopped)
                    videoplayer.Play(video);
                if (videoplayer.State != MediaState.Stopped)
                    texture = videoplayer.GetTexture();
                if (texture != null)
                {
                    spriteBatch.Begin();
                    spriteBatch.Draw(texture, new Vector2(Center.X * unitFactor + gd.Viewport.Width / 2 - video.Width / 2, gd.Viewport.Height / 2 - Center.Y * unitFactor - video.Height / 2), Para.BasePara.color);
                    spriteBatch.End();
                }
            }
        }

        /// <summary>
        /// Draw Video at Position: (5, 5) in Screen Coordinate
        /// </summary>
        public override void Draw()
        {
            if (Para.BasePara.visible)
            {
                if (videoplayer.State != MediaState.Stopped)
                    texture = videoplayer.GetTexture();
                if (texture != null)
                {
                    spriteBatch.Begin();
                    spriteBatch.Draw(texture, new Vector2(5, 5), Para.BasePara.color);
                    spriteBatch.End();
                }
            }
        }

        /// <summary>
        /// Draw Custom Position and Tinted Video
        /// </summary>
        /// <param name="position"></param>
        /// <param name="color"></param>
        public void Draw(Vector2 position, Color color)
        {
            if (Para.BasePara.visible)
            {
                if (videoplayer.State != MediaState.Stopped)
                    texture = videoplayer.GetTexture();
                if (texture != null)
                {
                    spriteBatch.Begin();
                    spriteBatch.Draw(texture, position, color);
                    spriteBatch.End();
                }
            }
        }

        /// <summary>
        /// Draw Tinted Video to Custom Rectangle
        /// </summary>
        /// <param name="destrect"></param>
        /// <param name="color"></param>
        public void Draw(Rectangle destrect, Color color)
        {
            if (Para.BasePara.visible)
            {
                if (videoplayer.State != MediaState.Stopped)
                    texture = videoplayer.GetTexture();
                if (texture != null)
                {
                    spriteBatch.Begin();
                    spriteBatch.Draw(texture, destrect, color);
                    spriteBatch.End();
                }
            }
        }

        /// <summary>
        /// Draw Part of Tinted Video to Custom Rectangle
        /// </summary>
        /// <param name="destrect"></param>
        /// <param name="sourrect"></param>
        /// <param name="color"></param>
        public void Draw(Rectangle destrect, Rectangle sourrect, Color color)
        {
            if (Para.BasePara.visible)
            {
                if (videoplayer.State != MediaState.Stopped)
                    texture = videoplayer.GetTexture();
                if (texture != null)
                {
                    spriteBatch.Begin();
                    spriteBatch.Draw(texture, destrect, sourrect, color);
                    spriteBatch.End();
                }
            }
        }

        /// <summary>
        /// Draw Rotated Part of Tinted Video to Custom Rectangle
        /// </summary>
        /// <param name="destrect"></param>
        /// <param name="sourrect"></param>
        /// <param name="color"></param>
        /// <param name="rotate"></param>
        /// <param name="origin"></param>
        public void Draw(Rectangle destrect, Rectangle sourrect, Color color, float rotate, Vector2 origin)
        {
            if (Para.BasePara.visible)
            {
                if (videoplayer.State != MediaState.Stopped)
                    texture = videoplayer.GetTexture();
                if (texture != null)
                {
                    spriteBatch.Begin();
                    spriteBatch.Draw(texture, destrect, sourrect, color, rotate, origin, SpriteEffects.None, 0.0f);
                    spriteBatch.End();
                }
            }
        }


        /// <summary>
        /// Play Video
        /// </summary>
        public void Play()
        {
            videoplayer.Play(video);
        }

        /// <summary>
        /// Stop Video
        /// </summary>
        public void Stop()
        {
            videoplayer.Stop();
        }

        /// <summary>
        /// Pause Video
        /// </summary>
        public void Pause()
        {
            videoplayer.Pause();
        }

        /// <summary>
        /// Resume Video
        /// </summary>
        public void Resume()
        {
            videoplayer.Resume();
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            if (gdRef != null)
            {
                return new SLVideo(distance2Display, displayRatio, displaySize, globalCamera, unit, gdRef, contentManager.ServiceProvider, contentManager.RootDirectory, Para);
            }
            else
            {
                MessageBox.Show("No Internal GraphicsDevice Reference, Please InitVS(GraphicsDevice gd) First !");
                return "No gdRef";
            }

        }

    }
}
