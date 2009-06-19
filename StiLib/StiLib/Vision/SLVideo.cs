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
#endregion

namespace StiLib.Vision
{
    /// <summary>
    /// StiLib Video
    /// </summary>
    public class SLVideo : VisionStimulus
    {
        /// <summary>
        /// Sprite Batch
        /// </summary>
        public SpriteBatch SpriteBatch;
        /// <summary>
        /// Basic Parameters
        /// </summary>
        public BasePara BasePara;
        /// <summary>
        /// Content Manager
        /// </summary>
        public ContentManager ContentManager;
        /// <summary>
        /// Video Texture
        /// </summary>
        public Texture2D Texture;
        Video video;
        VideoPlayer vplayer;
        /// <summary>
        /// Get the Video Player
        /// </summary>
        public VideoPlayer Player
        {
            get { return vplayer; }
        }


        /// <summary>
        /// Set Video parameters to default, 
        /// before LoadContent() and Init()
        /// </summary>
        public SLVideo()
        {
            BasePara = BasePara.Default;
        }

        /// <summary>
        /// Init to default
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="videoname"></param>
        public SLVideo(GraphicsDevice gd, IServiceProvider service, string path, string videoname)
            : this()
        {
            LoadContent(service, path, videoname);
            Init(gd);
        }

        /// <summary>
        /// Init to custom base parameters
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="videoname"></param>
        /// <param name="bpara"></param>
        public SLVideo(GraphicsDevice gd, IServiceProvider service, string path, string videoname, BasePara bpara)
            : base(gd)
        {
            LoadContent(service, path, videoname);
            Init(gd, bpara);
        }


        /// <summary>
        /// Load Compiled Video.xnb File using Content Manager
        /// </summary>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="videoname"></param>
        public void LoadContent(IServiceProvider service, string path, string videoname)
        {
            ContentManager = new ContentManager(service, path);
            try
            {
                video = ContentManager.Load<Video>(videoname);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error !");
            }
        }

        /// <summary>
        /// Init according to internal parameters
        /// </summary>
        /// <param name="gd"></param>
        public override void Init(GraphicsDevice gd)
        {
            SpriteBatch = new SpriteBatch(gd);
            vplayer = new VideoPlayer();
            InitVS(gd);
        }

        /// <summary>
        /// Init according to custom base parameters
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="bpara"></param>
        public void Init(GraphicsDevice gd, BasePara bpara)
        {
            BasePara = bpara;
            Init(gd);
        }

        /// <summary>
        /// Play Video
        /// </summary>
        public void Play()
        {
            vplayer.Play(video);
        }

        /// <summary>
        /// Stop Video
        /// </summary>
        public void Stop()
        {
            vplayer.Stop();
        }

        /// <summary>
        /// Pause Video
        /// </summary>
        public void Pause()
        {
            vplayer.Pause();
        }

        /// <summary>
        /// Resume Video
        /// </summary>
        public void Resume()
        {
            vplayer.Resume();
        }

        /// <summary>
        /// Draw Video at Position:(5,5)
        /// </summary>
        public void Draw()
        {
            if (BasePara.visible)
            {
                if (vplayer.State != MediaState.Stopped)
                    Texture = vplayer.GetTexture();
                if (Texture != null)
                {
                    SpriteBatch.Begin();
                    SpriteBatch.Draw(Texture, new Vector2(5, 5), BasePara.color);
                    SpriteBatch.End();
                }
            }
        }

        /// <summary>
        /// Draw custom position and tinted Video
        /// </summary>
        /// <param name="position"></param>
        /// <param name="color"></param>
        public void Draw(Vector2 position, Color color)
        {
            if (BasePara.visible)
            {
                if (vplayer.State != MediaState.Stopped)
                    Texture = vplayer.GetTexture();
                if (Texture != null)
                {
                    SpriteBatch.Begin();
                    SpriteBatch.Draw(Texture, position, color);
                    SpriteBatch.End();
                }
            }
        }

        /// <summary>
        /// Draw tinted Video to custom rectangle
        /// </summary>
        /// <param name="destrect"></param>
        /// <param name="color"></param>
        public void Draw(Rectangle destrect, Color color)
        {
            if (BasePara.visible)
            {
                if (vplayer.State != MediaState.Stopped)
                    Texture = vplayer.GetTexture();
                if (Texture != null)
                {
                    SpriteBatch.Begin();
                    SpriteBatch.Draw(Texture, destrect, color);
                    SpriteBatch.End();
                }
            }
        }

        /// <summary>
        /// Draw part of tinted Video to custom rectangle
        /// </summary>
        /// <param name="destrect"></param>
        /// <param name="sourrect"></param>
        /// <param name="color"></param>
        public void Draw(Rectangle destrect, Rectangle sourrect, Color color)
        {
            if (BasePara.visible)
            {
                if (vplayer.State != MediaState.Stopped)
                    Texture = vplayer.GetTexture();
                if (Texture != null)
                {
                    SpriteBatch.Begin();
                    SpriteBatch.Draw(Texture, destrect, sourrect, color);
                    SpriteBatch.End();
                }
            }
        }

        /// <summary>
        /// Set Visible State
        /// </summary>
        /// <param name="isvisible"></param>
        public override void SetVisible(bool isvisible)
        {
            BasePara.visible = isvisible;
        }

    }
}
