#region File Description
//-----------------------------------------------------------------------------
// SLQuad.cs
//
// StiLib Quad
// Copyright (c) Zhang Li. 2009-03-03.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Windows.Forms;
using StiLib.Core;
#endregion

namespace StiLib.Vision
{
    /// <summary>
    /// StiLib Quad
    /// </summary>
    public class SLQuad : VisionStimulus
    {
        #region Fields

        /// <summary>
        /// Quad Parameters
        /// </summary>
        public Quad Para;
        /// <summary>
        /// Quad Texture
        /// </summary>
        public Texture2D texture;

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

        /// <summary>
        /// Index Array
        /// </summary>
        public override int[] IndexArray
        {
            get { return Para.indices; }
            set { Para.indices = value; }
        }

        #endregion


        /// <summary>
        /// Sets Default Quad, need LoadContent() and Init()
        /// </summary>
        public SLQuad()
        {
            Para = Quad.Default;
        }

        /// <summary>
        /// Init SLQuad with Default Quad Parameters and Custom Texture
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="texture"></param>
        public SLQuad(GraphicsDevice gd, IServiceProvider service, string path, string texture)
            : base(gd)
        {
            Para = Quad.Default;
            LoadContent(service, path, texture);
            Init(gd);
        }

        /// <summary>
        /// Init SLQuad with Custom Quad Parameters
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="quad"></param>
        public SLQuad(GraphicsDevice gd, IServiceProvider service, string path, Quad quad)
            : base(gd)
        {
            Para = quad;
            LoadContent(service, path, quad.BasePara.contentname);
            Init(gd);
        }

        /// <summary>
        /// Init SLQuad with Custom Quad Parameters and Configuration
        /// </summary>
        /// <param name="distance2display"></param>
        /// <param name="displayratio"></param>
        /// <param name="displaysize"></param>
        /// <param name="camera"></param>
        /// <param name="unit"></param>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="quad"></param>
        public SLQuad(float distance2display, float displayratio, float displaysize, SLCamera camera, Unit unit, GraphicsDevice gd, IServiceProvider service, string path, Quad quad)
            : base(distance2display, displayratio, displaysize, gd, camera, unit)
        {
            Para = quad;
            LoadContent(service, path, quad.BasePara.contentname);
            Init(gd);
        }

        /// <summary>
        /// Init SLQuad with Custom Quad Parameters and StiLib Configuration File
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="slconfig"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="quad"></param>
        public SLQuad(GraphicsDevice gd, AssemblySettings slconfig, IServiceProvider service, string path, Quad quad)
            : base(gd, slconfig)
        {
            Para = quad;
            LoadContent(service, path, quad.BasePara.contentname);
            Init(gd);
        }

        /// <summary>
        /// Init SLQuad with Custom Quad Parameters and Texture
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="texture"></param>
        /// <param name="quad"></param>
        public SLQuad(GraphicsDevice gd, IServiceProvider service, string path, string texture, Quad quad)
            : base(gd)
        {
            Para = quad;
            LoadContent(service, path, texture);
            Init(gd);
        }


        /// <summary>
        /// Load Quad Texture
        /// </summary>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="texture"></param>
        public override void LoadContent(IServiceProvider service, string path, string texture)
        {
            contentManager = new ContentManager(service, path);
            try
            {
                this.texture = contentManager.Load<Texture2D>(texture);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error !");
            }

            Para.BasePara.contentname = texture;
        }

        /// <summary>
        /// Init according to internal quad parameters
        /// </summary>
        /// <param name="gd"></param>
        public override void Init(GraphicsDevice gd)
        {
            InitVS(gd);

            vertexDeclaration = new VertexDeclaration(gd, VertexPositionNormalTexture.VertexElements);
            SetVertexBuffer(gd);
            SetIndexBuffer(gd);

            basicEffect = new BasicEffect(gd, null);
            basicEffect.Texture = texture;
            basicEffect.TextureEnabled = true;

            ori3DMatrix = GetOri3DMatrix(Para.BasePara.orientation3D);
            worldMatrix = Matrix.CreateTranslation(Para.BasePara.center);
            ViewMatrix = ViewMatrix;
            ProjectionMatrix = ProjectionMatrix;
        }

        /// <summary>
        /// Init SLQuad with Custom Quad Parameters
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="quad"></param>
        public void Init(GraphicsDevice gd, Quad quad)
        {
            quad.BasePara.contentname = Para.BasePara.contentname;
            Para = quad;
            Init(gd);
        }

        /// <summary>
        /// Draw SLQuad
        /// </summary>
        /// <param name="gd"></param>
        public override void Draw(GraphicsDevice gd)
        {
            if (Para.BasePara.visible)
            {
                gd.VertexDeclaration = vertexDeclaration;
                gd.Vertices[0].SetSource(vertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes);
                gd.Indices = indexBuffer;
                gd.RenderState.CullMode = CullMode.None;
                gd.RenderState.AlphaBlendEnable = true;
                gd.RenderState.AlphaTestEnable = true;
                gd.RenderState.AlphaFunction = CompareFunction.Greater;

                basicEffect.World = ori3DMatrix * worldMatrix;

                basicEffect.Begin();
                basicEffect.CurrentTechnique.Passes[0].Begin();
                gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
                basicEffect.CurrentTechnique.Passes[0].End();
                basicEffect.End();
            }
        }

        /// <summary>
        /// Sets Quad Vertex Buffer
        /// </summary>
        /// <param name="gd"></param>
        public override void SetVertexBuffer(GraphicsDevice gd)
        {
            int temp = Para.vertices.Length * VertexPositionNormalTexture.SizeInBytes;
            if (vertexBuffer == null)
            {
                vertexBuffer = new VertexBuffer(gd, temp, BufferUsage.None);
            }
            else
            {
                if (temp > vertexBuffer.SizeInBytes)
                {
                    vertexBuffer.Dispose();
                    vertexBuffer = new VertexBuffer(gd, temp, BufferUsage.None);
                }
            }
            vertexBuffer.SetData<VertexPositionNormalTexture>(Para.vertices);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            if (gdRef != null)
            {
                return new SLQuad(distance2Display, displayRatio, displaySize, globalCamera, unit, gdRef, contentManager.ServiceProvider, contentManager.RootDirectory, Para);
            }
            else
            {
                SLConstant.ShowMessage("No Internal GraphicsDevice Reference, Please InitVS(GraphicsDevice gd) First !");
                return "No gdRef";
            }
        }

    }
}
