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
        /// Quad Structure
        /// </summary>
        public Quad Para;
        VertexDeclaration qvdec;
        BasicEffect basiceffect;
        ContentManager cm;
        Texture2D texture;

        #endregion

        #region Properties

        /// <summary>
        /// SLQuad Basic Effect
        /// </summary>
        public BasicEffect Effect
        {
            get { return basiceffect; }
            set { basiceffect = value; }
        }

        /// <summary>
        /// SLQuad Content Manager
        /// </summary>
        public ContentManager Content
        {
            get { return cm; }
            set { cm = value; }
        }

        #endregion


        /// <summary>
        /// Set Quad parameters to default before LoadContent() and Init()
        /// </summary>
        public SLQuad()
        {
            Para = Quad.Default;
        }

        /// <summary>
        /// Init SLQuad to default
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="tex"></param>
        public SLQuad(GraphicsDevice gd, IServiceProvider service, string path, string tex) : this()
        {
            LoadContent(service, path, tex);
            Init(gd);
        }

        /// <summary>
        /// Init SLQuad to custom settings
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="tex"></param>
        /// <param name="q"></param>
        public SLQuad(GraphicsDevice gd, IServiceProvider service, string path, string tex, Quad q) : base(gd)
        {
            LoadContent(service, path, tex);
            Init(gd, q);
        }


        /// <summary>
        /// Load Quad Texture
        /// </summary>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="tex"></param>
        public void LoadContent(IServiceProvider service, string path, string tex)
        {
            cm = new ContentManager(service, path);
            try
            {
                texture = cm.Load<Texture2D>(tex);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error !");
            }
        }

        /// <summary>
        /// Init according to internal quad parameters
        /// </summary>
        /// <param name="gd"></param>
        public override void Init(GraphicsDevice gd)
        {
            InitVS(gd);

            qvdec = new VertexDeclaration(gd, VertexPositionNormalTexture.VertexElements);

            basiceffect = new BasicEffect(gd, null);
            basiceffect.Texture = texture;
            basiceffect.TextureEnabled = true;

            basiceffect.World = Matrix.CreateFromYawPitchRoll(Para.BasePara.orientation3d.Y,
                                                                                        Para.BasePara.orientation3d.X,
                                                                                        Para.BasePara.orientation3d.Z) * Matrix.CreateTranslation(Para.BasePara.center);
            basiceffect.View = GlobalView();
            basiceffect.Projection = GlobalProj();
        }

        /// <summary>
        /// Init SLQuad according to custom quad
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="q"></param>
        public void Init(GraphicsDevice gd, Quad q)
        {
            Para = q;
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
                gd.VertexDeclaration = qvdec;
                gd.RenderState.AlphaBlendEnable = true;
                gd.RenderState.AlphaTestEnable = true;
                gd.RenderState.AlphaFunction = CompareFunction.Greater;

                basiceffect.Begin();
                basiceffect.CurrentTechnique.Passes[0].Begin();
                gd.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, Para.Vertices, 0, 4, Para.Indices, 0, 2);
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

    }

    /// <summary>
    /// Quad Structure
    /// </summary>
    public struct Quad
    {
        /// <summary>
        /// Basic Parameter
        /// </summary>
        public BasePara BasePara;
        /// <summary>
        /// UpperLeft
        /// </summary>
        public Vector3 UpperLeft;
        /// <summary>
        /// LowerLeft
        /// </summary>
        public Vector3 LowerLeft;
        /// <summary>
        /// UpperRight
        /// </summary>
        public Vector3 UpperRight;
        /// <summary>
        /// LowerRight
        /// </summary>
        public Vector3 LowerRight;
        /// <summary>
        /// Normal
        /// </summary>
        public Vector3 Normal;
        /// <summary>
        /// Up
        /// </summary>
        public Vector3 Up;
        /// <summary>
        /// Left
        /// </summary>
        public Vector3 Left;
        /// <summary>
        /// Vertex Array
        /// </summary>
        public VertexPositionNormalTexture[] Vertices;
        /// <summary>
        /// Index Array
        /// </summary>
        public int[] Indices;


        /// <summary>
        /// Init to default -- Center:(0,0,0), Normal:(0,0,1), Up:(0,1,0)
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Quad(float width, float height)
            : this(Vector3.Zero, Vector3.Backward, Vector3.Up, width, height)
        {
        }

        /// <summary>
        /// Init to custom settings
        /// </summary>
        /// <param name="center"></param>
        /// <param name="normal"></param>
        /// <param name="up"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Quad(Vector3 center, Vector3 normal, Vector3 up, float width, float height)
        {
            BasePara = BasePara.Default;
            Vertices = new VertexPositionNormalTexture[4];
            Indices = new int[6];
            BasePara.center = center;
            Normal = normal;
            Up = up;

            // Calculate the quad corners
            Left = Vector3.Cross(normal, Up);
            Vector3 uppercenter = (Up * height / 2) + center;
            UpperLeft = uppercenter + (Left * width / 2);
            UpperRight = uppercenter - (Left * width / 2);
            LowerLeft = UpperLeft - (Up * height);
            LowerRight = UpperRight - (Up * height);

            FillVertices();
        }

        /// <summary>
        /// Get a default quad -- Width:15, Height:5
        /// </summary>
        public static Quad Default
        {
            get
            {
                return new Quad(15, 5);
            }
        }


        private void FillVertices()
        {
            // Fill in texture coordinates to display full texture on quad
            Vector2 textureUpperLeft = new Vector2(0.0f, 0.0f);
            Vector2 textureUpperRight = new Vector2(1.0f, 0.0f);
            Vector2 textureLowerLeft = new Vector2(0.0f, 1.0f);
            Vector2 textureLowerRight = new Vector2(1.0f, 1.0f);

            // Provide a normal for each vertex
            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertices[i].Normal = Normal;
            }

            // Set the position and texture coordinate for each vertex
            Vertices[0].Position = LowerLeft;
            Vertices[0].TextureCoordinate = textureLowerLeft;
            Vertices[1].Position = UpperLeft;
            Vertices[1].TextureCoordinate = textureUpperLeft;
            Vertices[2].Position = LowerRight;
            Vertices[2].TextureCoordinate = textureLowerRight;
            Vertices[3].Position = UpperRight;
            Vertices[3].TextureCoordinate = textureUpperRight;

            // Set the index buffer for each vertex, using clockwise winding
            Indices[0] = 0;
            Indices[1] = 1;
            Indices[2] = 2;
            Indices[3] = 2;
            Indices[4] = 1;
            Indices[5] = 3;
        }

    }
}
