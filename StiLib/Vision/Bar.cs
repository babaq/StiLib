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

        #endregion

        #region Properties

        /// <summary>
        /// Bar Basic Parameters
        /// </summary>
        public override vsBasePara BasePara
        {
            get { return Para.BasePara; }
            set { Para.BasePara = value; }
        }

        /// <summary>
        /// Bar Center
        /// </summary>
        public override Vector3 Center
        {
            get { return Para.BasePara.center; }
            set { Para.BasePara.center = value; }
        }

        /// <summary>
        /// Bar Speed3D
        /// </summary>
        public override Vector3 Speed3D
        {
            get { return Para.BasePara.speed3D; }
            set { Para.BasePara.speed3D = value; }
        }

        /// <summary>
        /// Bar Visible State
        /// </summary>
        public override bool Visible
        {
            get { return Para.BasePara.visible; }
            set { Para.BasePara.visible = value; }
        }

        #endregion


        /// <summary>
        /// Set Default BarPara, need Init()
        /// </summary>
        public Bar()
        {
            Para = BarPara.Default;
        }

        /// <summary>
        /// Init Bar with Default BarPara
        /// </summary>
        /// <param name="gd"></param>
        public Bar(GraphicsDevice gd)
            : base(gd)
        {
            Para = BarPara.Default;
            Init(gd);
        }

        /// <summary>
        /// Init Bar with Custom BarPara
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="barpara"></param>
        public Bar(GraphicsDevice gd, BarPara barpara)
            : base(gd)
        {
            Para = barpara;
            Init(gd);
        }

        /// <summary>
        /// Init Bar with Custom BarPara and StiLib Configuration File
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="slconfig"></param>
        /// <param name="barpara"></param>
        public Bar(GraphicsDevice gd, AssemblySettings slconfig, BarPara barpara)
            : base(gd, slconfig)
        {
            Para = barpara;
            Init(gd);
        }

        /// <summary>
        /// Init Bar with Custom BarPara and Configuration
        /// </summary>
        /// <param name="distance2display"></param>
        /// <param name="displayratio"></param>
        /// <param name="displaysize"></param>
        /// <param name="camera"></param>
        /// <param name="unit"></param>
        /// <param name="gd"></param>
        /// <param name="barpara"></param>
        public Bar(float distance2display, float displayratio, float displaysize, SLCamera camera, Unit unit, GraphicsDevice gd, BarPara barpara)
            : base(distance2display, displayratio, displaysize, gd, camera, unit)
        {
            Para = barpara;
            Init(gd);
        }


        /// <summary>
        /// Init according to internal bar parameters
        /// </summary>
        /// <param name="gd"></param>
        public override void Init(GraphicsDevice gd)
        {
            InitVS(gd);

            // Fill Vertex and Index Array
            vertexDeclaration = new VertexDeclaration(gd, VertexPositionColor.VertexElements);
            VertexArray = new VertexPositionColor[4];
            vertexArray[0].Position = new Vector3(-Para.width / 2f, -Para.height / 2f, 0);
            vertexArray[0].Color = Para.BasePara.color;
            vertexArray[1].Position = new Vector3(-Para.width / 2f, Para.height / 2f, 0);
            vertexArray[1].Color = Para.BasePara.color;
            vertexArray[2].Position = new Vector3(Para.width / 2f, Para.height / 2f, 0);
            vertexArray[2].Color = Para.BasePara.color;
            vertexArray[3].Position = new Vector3(Para.width / 2f, -Para.height / 2f, 0);
            vertexArray[3].Color = Para.BasePara.color;
            indexArray = new int[4];
            indexArray[0] = 0;
            indexArray[1] = 1;
            indexArray[2] = 2;
            indexArray[3] = 3;

            // Fill Vertex and Index Buffer
            SetVertexBuffer(gd);
            SetIndexBuffer(gd);

            // Get BasicEffect Ready
            basicEffect = new BasicEffect(gd, null);
            basicEffect.VertexColorEnabled = true;

            ori3DMatrix = Matrix.CreateRotationZ(Para.BasePara.orientation * (float)SLConstant.Rad_p_Deg);
            worldMatrix = Matrix.CreateTranslation(Para.BasePara.center);
            ViewMatrix = ViewMatrix;
            ProjectionMatrix = ProjectionMatrix;
        }

        /// <summary>
        /// Init According to Custom BarPara
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="barpara"></param>
        public void Init(GraphicsDevice gd, BarPara barpara)
        {
            barpara.BasePara.contentname = Para.BasePara.contentname;
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
                gd.VertexDeclaration = vertexDeclaration;
                gd.Vertices[0].SetSource(vertexBuffer, 0, VertexPositionColor.SizeInBytes);
                gd.Indices = indexBuffer;
                gd.RenderState.CullMode = CullMode.None;
                basicEffect.World = ori3DMatrix * worldMatrix;

                basicEffect.Begin();
                basicEffect.CurrentTechnique.Passes[0].Begin();
                gd.DrawIndexedPrimitives(PrimitiveType.TriangleFan, 0, 0, 4, 0, 2);
                basicEffect.CurrentTechnique.Passes[0].End();
                basicEffect.End();
            }
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
                vertexArray[i].Position *= x;
            }
            SetVertexBuffer();
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
                vertexArray[i].Position.X *= x;
            }
            SetVertexBuffer();
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
                vertexArray[i].Position.Y *= x;
            }
            SetVertexBuffer();
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
            vertexArray[0].Position = new Vector3(-width / 2f, -height / 2f, 0);
            vertexArray[1].Position = new Vector3(-width / 2f, height / 2f, 0);
            vertexArray[2].Position = new Vector3(width / 2f, height / 2f, 0);
            vertexArray[3].Position = new Vector3(width / 2f, -height / 2f, 0);
            SetVertexBuffer();
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
            vertexArray[0].Color = L_B;
            vertexArray[1].Color = L_T;
            vertexArray[2].Color = R_T;
            vertexArray[3].Color = R_B;
            SetVertexBuffer();
        }

        /// <summary>
        /// Set Bar to Uniform Color
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(Color color)
        {
            Para.BasePara.color = color;
            for (int i = 0; i < 4; i++)
            {
                vertexArray[i].Color = color;
            }
            SetVertexBuffer();
        }

        /// <summary>
        /// Scale Bar Color
        /// </summary>
        /// <param name="x">Scale Adding Factor</param>
        public void ScaleColor(float x)
        {
            for (int i = 0; i < 4; i++)
            {
                var cv = vertexArray[i].Color.ToVector4();
                cv.X += x;
                cv.Y += x;
                cv.Z += x;
                cv.W += x;
                vertexArray[i].Color = new Color(cv);
            }
            SetVertexBuffer();
        }

        /// <summary>
        /// Reverse Bar Color
        /// </summary>
        public void ReverseColor()
        {
            for (int i = 0; i < 4; i++)
            {
                vertexArray[i].Color = new Color(new Vector4(1.0f) - vertexArray[i].Color.ToVector4());
            }
            SetVertexBuffer();
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            if (gdRef != null)
            {
                return new Bar(distance2Display, displayRatio, displaySize, globalCamera, unit, gdRef, Para);
            }
            else
            {
                SLConstant.ShowMessage("No Internal GraphicsDevice Reference, Please InitVS(GraphicsDevice gd) First !");
                return "No gdRef";
            }
        }

    }
}
