#region File Description
//-----------------------------------------------------------------------------
// Text.cs
//
// StiLib Text Stimulus
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
    /// StiLib Text Stimulus
    /// </summary>
    public class Text : VisionStimulus
    {
        #region Fields

        /// <summary>
        /// Text Parameters
        /// </summary>
        public TextPara Para;
        /// <summary>
        /// Text Font
        /// </summary>
        public SpriteFont spriteFont;

        #endregion

        #region Properties

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
        /// Sets Default TextPara -- Color: PeachPuff, need LoadContent() and Init()
        /// </summary>
        public Text()
        {
            Para = new TextPara("");
            Para.BasePara.color = Color.PeachPuff;
        }

        /// <summary>
        /// Init with Default TextPara and Custom SpriteFont
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="spritefont"></param>
        public Text(GraphicsDevice gd, IServiceProvider service, string path, string spritefont)
            : base(gd)
        {
            Para = new TextPara(spritefont);
            Para.BasePara.color = Color.PeachPuff;
            LoadContent(service, path, spritefont);
            Init(gd);
        }

        /// <summary>
        /// Init with Custom TextPara
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="textpara"></param>
        public Text(GraphicsDevice gd, IServiceProvider service, string path, TextPara textpara)
            : base(gd)
        {
            Para = textpara;
            LoadContent(service, path, textpara.BasePara.contentname);
            Init(gd);
        }

        /// <summary>
        /// Init with Custom TextPara and Configuration
        /// </summary>
        /// <param name="distance2display"></param>
        /// <param name="displayratio"></param>
        /// <param name="displaysize"></param>
        /// <param name="camera"></param>
        /// <param name="unit"></param>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="textpara"></param>
        public Text(float distance2display, float displayratio, float displaysize, SLCamera camera, Unit unit, GraphicsDevice gd, IServiceProvider service, string path, TextPara textpara)
            : base(distance2display, displayratio, displaysize, gd, camera, unit)
        {
            Para = textpara;
            LoadContent(service, path, textpara.BasePara.contentname);
            Init(gd);
        }

        /// <summary>
        /// Init with Custom TextPara and StiLib Configuration File
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="slconfig"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="textpara"></param>
        public Text(GraphicsDevice gd, AssemblySettings slconfig, IServiceProvider service, string path, TextPara textpara)
            : base(gd, slconfig)
        {
            Para = textpara;
            LoadContent(service, path, textpara.BasePara.contentname);
            Init(gd);
        }

        /// <summary>
        /// Init with Custom TextPara and SpriteFont
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="spritefont"></param>
        /// <param name="textpara"></param>
        public Text(GraphicsDevice gd, IServiceProvider service, string path, string spritefont, TextPara textpara)
            : base(gd)
        {
            Para = textpara;
            LoadContent(service, path, spritefont);
            Init(gd);
        }


        /// <summary>
        /// Load Text SpriteFont
        /// </summary>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="spritefont"></param>
        public override void LoadContent(IServiceProvider service, string path, string spritefont)
        {
            contentManager = new ContentManager(service, path);
            try
            {
                spriteFont = contentManager.Load<SpriteFont>(spritefont);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error !");
            }

            Para.BasePara.contentname = spritefont;
        }

        /// <summary>
        /// Init according to internal parameters
        /// </summary>
        /// <param name="gd"></param>
        public override void Init(GraphicsDevice gd)
        {
            InitVS(gd);
            spriteBatch = new SpriteBatch(gd);
        }

        /// <summary>
        /// Init according to custom text parameters
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="textpara"></param>
        public void Init(GraphicsDevice gd, TextPara textpara)
        {
            textpara.BasePara.contentname = Para.BasePara.contentname;
            Para = textpara;
            Init(gd);
        }

        /// <summary>
        /// Draw Standard Help Text
        /// </summary>
        /// <param name="gd"></param>
        public override void Draw(GraphicsDevice gd)
        {
            if (Para.BasePara.visible)
            {
                var size = spriteFont.MeasureString(SLConstant.Help);
                spriteBatch.Begin();
                spriteBatch.DrawString(spriteFont, SLConstant.Help, new Vector2(Center.X * unitFactor + gd.Viewport.Width / 2 - size.X / 2, gd.Viewport.Height / 2 - Center.Y * unitFactor - size.Y / 2), Para.BasePara.color);
                spriteBatch.End();
            }
        }

        /// <summary>
        /// Draw Standard Help Text at Position: (5, 5) in Screen Coordinate
        /// </summary>
        public override void Draw()
        {
            Draw(new Vector2(5, 5), SLConstant.Help, Para.BasePara.color);
        }

        /// <summary>
        /// Draw Custom Text at Position: (5, 5) in Screen Coordinate
        /// </summary>
        /// <param name="text"></param>
        public void Draw(string text)
        {
            Draw(new Vector2(5, 5), text, Para.BasePara.color);
        }

        /// <summary>
        /// Draw Text
        /// </summary>
        /// <param name="position"></param>
        /// <param name="text"></param>
        /// <param name="color"></param>
        public void Draw(Vector2 position, string text, Color color)
        {
            if (Para.BasePara.visible)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(spriteFont, text, position, color);
                spriteBatch.End();
            }
        }

        /// <summary>
        /// Draw Rotated and Scaled Text
        /// </summary>
        /// <param name="position"></param>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <param name="rotate"></param>
        /// <param name="origin"></param>
        /// <param name="scale"></param>
        public void Draw(Vector2 position, string text, Color color, float rotate, Vector2 origin, Vector2 scale)
        {
            if (Para.BasePara.visible)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(spriteFont, text, position, color, rotate, origin, scale, SpriteEffects.None, 0.0f);
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
                return new Text(distance2Display, displayRatio, displaySize, globalCamera, unit, gdRef, contentManager.ServiceProvider, contentManager.RootDirectory, Para);
            }
            else
            {
                SLConstant.ShowMessage("No Internal GraphicsDevice Reference, Please InitVS(GraphicsDevice gd) First !");
                return "No gdRef";
            }
        }

    }
}
