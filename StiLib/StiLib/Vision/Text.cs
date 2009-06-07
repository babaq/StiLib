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
using System.Linq;
using System.Text;
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
        /// <summary>
        /// Sprite Batch
        /// </summary>
        public SpriteBatch SpriteBatch;
        /// <summary>
        /// Text Font
        /// </summary>
        public SpriteFont Font;
        /// <summary>
        /// Basic Parameters
        /// </summary>
        public BasePara BasePara;
        /// <summary>
        /// Content Manager
        /// </summary>
        public ContentManager ContentManager;


        /// <summary>
        /// Set Text parameters to default -- Color:PeachPuff, 
        /// before LoadContent() and Init()
        /// </summary>
        public Text()
        {
            BasePara = BasePara.Default;
            BasePara.color = Color.PeachPuff;
        }

        /// <summary>
        /// Init to default
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="spritefont"></param>
        public Text(GraphicsDevice gd, IServiceProvider service, string path, string spritefont) : this()
        {
            LoadContent(service, path, spritefont);
            Init(gd);
        }

        /// <summary>
        /// Init to custom base parameters
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="spritefont"></param>
        /// <param name="bpara"></param>
        public Text(GraphicsDevice gd, IServiceProvider service, string path, string spritefont, BasePara bpara) : base(gd)
        {
            LoadContent(service, path, spritefont);
            Init(gd, bpara);
        }


        /// <summary>
        /// Load SpriteFont using Content Manager
        /// </summary>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="spritefont"></param>
        public void LoadContent(IServiceProvider service, string path, string spritefont)
        {
            ContentManager = new ContentManager(service, path);
            try
            {
                Font = ContentManager.Load<SpriteFont>(spritefont);
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
        /// Draw Standard Help Text at Position:(5,5)
        /// </summary>
        public void Draw()
        {
            Draw(new Vector2(5, 5), SLConstant.Help, BasePara.color);
        }

        /// <summary>
        /// Draw Custom Text at Position:(5,5)
        /// </summary>
        /// <param name="text"></param>
        public void Draw(string text)
        {
            Draw(new Vector2(5, 5), text, BasePara.color);
        }

        /// <summary>
        /// Draw Text
        /// </summary>
        /// <param name="position"></param>
        /// <param name="text"></param>
        /// <param name="color"></param>
        public void Draw(Vector2 position, string text, Color color)
        {
            if (BasePara.visible)
            {
                SpriteBatch.Begin();
                SpriteBatch.DrawString(Font, text, position, color);
                SpriteBatch.End();
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
        public void Draw(Vector2 position, string text, Color color, float rotate, Vector2 origin, float scale)
        {
            if (BasePara.visible)
            {
                SpriteBatch.Begin();
                SpriteBatch.DrawString(Font, text, position, color, rotate, origin, scale, SpriteEffects.None, 0.0f);
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
