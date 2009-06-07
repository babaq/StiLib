#region File Description
//-----------------------------------------------------------------------------
// Bar.cs
//
// StiLib Bar Stimulus
// Copyright (c) Zhang Li. 2008-8-6.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StiLib.Core;
#endregion

namespace StiLib.Vision
{
    /// <summary>
    /// StiLib Bar Stimulus
    /// </summary>
    public class Bar : VisionStimulus
    {
        #region Fields

        /// <summary>
        /// Bar Parameters
        /// </summary>
        public BarPara Para;
        /// <summary>
        /// Bar Vertex Array
        /// </summary>
        public VertexPositionColor[] barvertex;
        VertexDeclaration barvdec;
        BasicEffect basiceffect;

        #endregion

        #region Properties

        /// <summary>
        /// Bar Basic Effect
        /// </summary>
        public BasicEffect Effect
        {
            get { return basiceffect; }
            set { basiceffect = value; }
        }

        #endregion


        /// <summary>
        /// Set bar parameters to default before Init()
        /// </summary>
        public Bar()
        {
            Para = BarPara.Default;
        }

        /// <summary>
        /// Init Bar to default
        /// </summary>
        /// <param name="gd"></param>
        public Bar(GraphicsDevice gd) : this()
        {
            Init(gd);
        }

        /// <summary>
        /// Init Bar to custom settings
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="barpara"></param>
        public Bar(GraphicsDevice gd, BarPara barpara) : base(gd)
        {
            Init(gd, barpara);
        }


        /// <summary>
        /// Init according to internal bar parameters
        /// </summary>
        /// <param name="gd"></param>
        public override void Init(GraphicsDevice gd)
        {
            InitVS(gd);

            // Get Vertex Ready
            barvdec = new VertexDeclaration(gd, VertexPositionColor.VertexElements);
            barvertex = new VertexPositionColor[4];
            barvertex[0].Position = new Vector3(-Para.width / 2f, -Para.height / 2f, 0);
            barvertex[0].Color = Para.BasePara.color;
            barvertex[1].Position = new Vector3(-Para.width / 2f, Para.height / 2f, 0);
            barvertex[1].Color = Para.BasePara.color;
            barvertex[2].Position = new Vector3(Para.width / 2f, Para.height / 2f, 0);
            barvertex[2].Color = Para.BasePara.color;
            barvertex[3].Position = new Vector3(Para.width / 2f, -Para.height / 2f, 0);
            barvertex[3].Color = Para.BasePara.color;

            // Get BasicEffect ready
            basiceffect = new BasicEffect(gd, null);
            basiceffect.VertexColorEnabled = true;

            basiceffect.World = Matrix.CreateRotationZ(Para.BasePara.orientation * (float)SLConstant.RadpDeg) * Matrix.CreateTranslation(Para.BasePara.center);
            basiceffect.View = GlobalView();
            basiceffect.Projection = GlobalProj();
        }

        /// <summary>
        /// Init Bar according to barpara
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="barpara"></param>
        public void Init(GraphicsDevice gd, BarPara barpara)
        {
            Para = barpara;
            Init(gd);
        }

        /// <summary>
        /// Draw Bar
        /// </summary>
        /// <param name="gd"></param>
        public override void Draw(GraphicsDevice gd)
        {
            if (Para.BasePara.visible)
            {
                gd.VertexDeclaration = barvdec;

                basiceffect.Begin();
                basiceffect.CurrentTechnique.Passes[0].Begin();
                gd.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleFan, barvertex, 0, 2);
                basiceffect.CurrentTechnique.Passes[0].End();
                basiceffect.End();
            }
        }


        /// <summary>
        /// Set effect world matrix
        /// </summary>
        /// <param name="world"></param>
        public override void SetWorld(Matrix world)
        {
            basiceffect.World = world;
        }

        /// <summary>
        /// Set effect view matrix
        /// </summary>
        /// <param name="view"></param>
        public override void SetView(Matrix view)
        {
            basiceffect.View = view;
        }

        /// <summary>
        /// Set effect projection matrix
        /// </summary>
        /// <param name="proj"></param>
        public override void SetProjection(Matrix proj)
        {
            basiceffect.Projection = proj;
        }

        /// <summary>
        /// Set visible state
        /// </summary>
        /// <param name="isvisible"></param>
        public override void SetVisible(bool isvisible)
        {
            Para.BasePara.visible = isvisible;
        }

        /// <summary>
        /// Scale Bar Size
        /// </summary>
        /// <param name="x">Scale Multiple Factor</param>
        public void ScaleSize(float x)
        {
            Para.height *= x;
            Para.width *= x;
            for (int i = 0; i < 4; i++)
            {
                barvertex[i].Position *= x;
            }
        }

        /// <summary>
        /// Scale Bar Width
        /// </summary>
        /// <param name="x"></param>
        public void Scale_Width(float x)
        {
            Para.width *= x;
            for (int i = 0; i < 4; i++)
            {
                barvertex[i].Position.X *= x;
            }
        }

        /// <summary>
        /// Scale Bar Height
        /// </summary>
        /// <param name="x"></param>
        public void Scale_Height(float x)
        {
            Para.height *= x;
            for (int i = 0; i < 4; i++)
            {
                barvertex[i].Position.Y *= x;
            }
        }

        /// <summary>
        /// Set Bar Size
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetSize(float width, float height)
        {
            Para.height = height;
            Para.width = width;
            barvertex[0].Position = new Vector3(-width / 2f, -height / 2f, 0);
            barvertex[1].Position = new Vector3(-width / 2f, height / 2f, 0);
            barvertex[2].Position = new Vector3(width / 2f, height / 2f, 0);
            barvertex[3].Position = new Vector3(width / 2f, -height / 2f, 0);
        }

        /// <summary>
        /// Set Bar Vertex Color
        /// </summary>
        /// <param name="L_T">Left_Top Vertex Color</param>
        /// <param name="R_T">Right_Top Vertex Color</param>
        /// <param name="L_B">Left_Bottom Vertex Color</param>
        /// <param name="R_B">Right_Bottom Vertex Color</param>
        public void SetColor(Color L_T, Color R_T, Color L_B, Color R_B)
        {
            barvertex[0].Color = L_B;
            barvertex[1].Color = L_T;
            barvertex[2].Color = R_T;
            barvertex[3].Color = R_B;
        }

        /// <summary>
        /// Set Bar a Uniform Color
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(Color color)
        {
            Para.BasePara.color = color;
            for (int i = 0; i < 4; i++)
            {
                barvertex[i].Color = color;
            }
        }

        /// <summary>
        /// Scale Bar Color
        /// </summary>
        /// <param name="x">Scale Adding Factor</param>
        public void ScaleColor(float x)
        {
            for (int i = 0; i < 4; i++)
            {
                barvertex[i].Color = new Color(new Vector4(x) + barvertex[i].Color.ToVector4());
            }
        }

        /// <summary>
        /// Reverse Bar Color
        /// </summary>
        public void ReverseColor()
        {
            for (int i = 0; i < 4; i++)
            {
                barvertex[i].Color = new Color(new Vector4(1.0f) - barvertex[i].Color.ToVector4());
            }
        }

    }
}
