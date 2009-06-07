#region File Description
//-----------------------------------------------------------------------------
// Image.cs
//
// StiLib Image Stimulus
// Copyright (c) Zhang Li. 2009-02-26.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Windows.Forms;
#endregion

namespace StiLib.Vision
{
    /// <summary>
    /// StiLib Image Stimulus
    /// </summary>
    public class Image : VisionStimulus
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
        /// Image Texture
        /// </summary>
        public Texture2D Texture;


        /// <summary>
        /// Set Image parameters to default, 
        /// before LoadContent() and Init()
        /// </summary>
        public Image()
        {
            BasePara = BasePara.Default;
        }

        /// <summary>
        /// Init to default
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="imagename"></param>
        public Image(GraphicsDevice gd, IServiceProvider service, string path, string imagename)
            : this()
        {
            LoadContent(service, path, imagename);
            Init(gd);
        }

        /// <summary>
        /// Init to custom base parameters
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="imagename"></param>
        /// <param name="bpara"></param>
        public Image(GraphicsDevice gd, IServiceProvider service, string path, string imagename, BasePara bpara)
            : base(gd)
        {
            LoadContent(service, path, imagename);
            Init(gd, bpara);
        }


        /// <summary>
        /// Load Compiled Image.xnb File using Content Manager
        /// </summary>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="imagename"></param>
        public void LoadContent(IServiceProvider service, string path, string imagename)
        {
            ContentManager = new ContentManager(service, path);
            try
            {
                Texture = ContentManager.Load<Texture2D>(imagename);
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
        /// Draw Image at Position:(5,5)
        /// </summary>
        public void Draw()
        {
            if (BasePara.visible)
            {
                SpriteBatch.Begin();
                SpriteBatch.Draw(Texture, new Vector2(5, 5), BasePara.color);
                SpriteBatch.End();
            }
        }

        /// <summary>
        /// Draw custom position and tinted image
        /// </summary>
        /// <param name="position"></param>
        /// <param name="color"></param>
        public void Draw(Vector2 position, Color color)
        {
            if (BasePara.visible)
            {
                SpriteBatch.Begin();
                SpriteBatch.Draw(Texture, position, color);
                SpriteBatch.End();
            }
        }

        /// <summary>
        /// Draw tinted image to custom rectangle
        /// </summary>
        /// <param name="destrect"></param>
        /// <param name="color"></param>
        public void Draw(Rectangle destrect, Color color)
        {
            if (BasePara.visible)
            {
                SpriteBatch.Begin();
                SpriteBatch.Draw(Texture, destrect, color);
                SpriteBatch.End();
            }
        }

        /// <summary>
        /// Draw part of tinted image to custom rectangle
        /// </summary>
        /// <param name="destrect"></param>
        /// <param name="sourrect"></param>
        /// <param name="color"></param>
        public void Draw(Rectangle destrect, Rectangle sourrect, Color color)
        {
            if (BasePara.visible)
            {
                SpriteBatch.Begin();
                SpriteBatch.Draw(Texture, destrect, sourrect, color);
                SpriteBatch.End();
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
