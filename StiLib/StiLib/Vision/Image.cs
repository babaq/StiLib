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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Windows.Forms;
using StiLib.Core;
#endregion

namespace StiLib.Vision
{
    /// <summary>
    /// StiLib Image Stimulus
    /// </summary>
    public class Image : VisionStimulus
    {
        #region Fields

        /// <summary>
        /// Image Parameters
        /// </summary>
        public ImagePara Para;
        /// <summary>
        /// Image Texture
        /// </summary>
        public Texture2D texture;

        #endregion

        #region Properties

        /// <summary>
        /// Image Basic Parameters
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
        /// Sets Default ImagePara, need LoadContent() and Init()
        /// </summary>
        public Image()
        {
            Para = new ImagePara("");
        }

        /// <summary>
        /// Init with Default ImagePara
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="imagename"></param>
        public Image(GraphicsDevice gd, IServiceProvider service, string path, string imagename)
            : base(gd)
        {
            Para = new ImagePara(imagename);
            LoadContent(service, path, imagename);
            Init(gd);
        }

        /// <summary>
        /// Init with Custom ImagePara
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="imagepara"></param>
        public Image(GraphicsDevice gd, IServiceProvider service, string path, ImagePara imagepara)
            : base(gd)
        {
            Para = imagepara;
            LoadContent(service, path, Para.BasePara.contentname);
            Init(gd);
        }

        /// <summary>
        /// Init with Custom ImagePara and Configuration
        /// </summary>
        /// <param name="distance2display"></param>
        /// <param name="displayratio"></param>
        /// <param name="displaysize"></param>
        /// <param name="camera"></param>
        /// <param name="unit"></param>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="imagepara"></param>
        public Image(float distance2display, float displayratio, float displaysize, SLCamera camera, Unit unit, GraphicsDevice gd, IServiceProvider service, string path, ImagePara imagepara)
            : base(distance2display, displayratio, displaysize, gd, camera, unit)
        {
            Para = imagepara;
            LoadContent(service, path, Para.BasePara.contentname);
            Init(gd);
        }

        /// <summary>
        /// Init with Custom ImagePara and StiLib Configuration
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="slconfig"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="imagepara"></param>
        public Image(GraphicsDevice gd, AssemblySettings slconfig, IServiceProvider service, string path, ImagePara imagepara)
            : base(gd, slconfig)
        {
            Para = imagepara;
            LoadContent(service, path, Para.BasePara.contentname);
            Init(gd);
        }

        /// <summary>
        /// Init with Custom Parameters and Image
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="imagename"></param>
        /// <param name="imagepara"></param>
        public Image(GraphicsDevice gd, IServiceProvider service, string path, string imagename, ImagePara imagepara)
            : base(gd)
        {
            Para = imagepara;
            LoadContent(service, path, imagename);
            Init(gd);
        }


        /// <summary>
        /// Load Compiled ImageName.xnb File
        /// </summary>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="imagename"></param>
        public override void LoadContent(IServiceProvider service, string path, string imagename)
        {
            contentManager = new ContentManager(service, path);
            try
            {
                texture = contentManager.Load<Texture2D>(imagename);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error !");
            }

            Para.BasePara.contentname = imagename;
        }

        /// <summary>
        /// Init According to Internal Parameters
        /// </summary>
        /// <param name="gd"></param>
        public override void Init(GraphicsDevice gd)
        {
            spriteBatch = new SpriteBatch(gd);
            InitVS(gd);
        }

        /// <summary>
        /// Init According to Custom ImagePara
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="imagepara"></param>
        public void Init(GraphicsDevice gd, ImagePara imagepara)
        {
            imagepara.BasePara.contentname = Para.BasePara.contentname;
            Para = imagepara;
            Init(gd);
        }

        /// <summary>
        /// Draw Image
        /// </summary>
        /// <param name="gd"></param>
        public override void Draw(GraphicsDevice gd)
        {
            if (Para.BasePara.visible)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(texture, new Vector2(Center.X * unitFactor + gd.Viewport.Width / 2 - texture.Width / 2, gd.Viewport.Height / 2 - Center.Y * unitFactor - texture.Height / 2), Para.BasePara.color);
                spriteBatch.End();
            }
        }

        /// <summary>
        /// Draw Image at Position: (5, 5) in Screen Coordinate
        /// </summary>
        public override void Draw()
        {
            if (Para.BasePara.visible)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(texture, new Vector2(5, 5), Para.BasePara.color);
                spriteBatch.End();
            }
        }

        /// <summary>
        /// Draw Custom Position and Tinted Image
        /// </summary>
        /// <param name="position"></param>
        /// <param name="color"></param>
        public void Draw(Vector2 position, Color color)
        {
            if (Para.BasePara.visible)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(texture, position, color);
                spriteBatch.End();
            }
        }

        /// <summary>
        /// Draw Tinted Image to Custom Rectangle
        /// </summary>
        /// <param name="destrect"></param>
        /// <param name="color"></param>
        public void Draw(Rectangle destrect, Color color)
        {
            if (Para.BasePara.visible)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(texture, destrect, color);
                spriteBatch.End();
            }
        }

        /// <summary>
        /// Draw Part of Tinted Image to Custom Rectangle
        /// </summary>
        /// <param name="destrect"></param>
        /// <param name="sourrect"></param>
        /// <param name="color"></param>
        public void Draw(Rectangle destrect, Rectangle sourrect, Color color)
        {
            if (Para.BasePara.visible)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(texture, destrect, sourrect, color);
                spriteBatch.End();
            }
        }

        /// <summary>
        /// Draw Rotated Part of Tinted Image to Custom Rectangle
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
                spriteBatch.Begin();
                spriteBatch.Draw(texture, destrect, sourrect, color, rotate, origin, SpriteEffects.None, 0.0f);
                spriteBatch.End();
            }
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            if (gdRef != null)
            {
                return new Image(distance2Display, displayRatio, displaySize, globalCamera, unit, gdRef, contentManager.ServiceProvider, contentManager.RootDirectory, Para);
            }
            else
            {
                SLConstant.ShowMessage("No Internal GraphicsDevice Reference, Please InitVS(GraphicsDevice gd) First !");
                return "No gdRef";
            }
        }

    }
}
