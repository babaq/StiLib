#region File Description
//-----------------------------------------------------------------------------
// Primitive.cs
//
// StiLib Visual Primitive
// Copyright (c) Zhang Li. 2009-01-14.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
#endregion

namespace StiLib.Vision
{
    /// <summary>
    /// Visual Primitive Class using VertexPositionColor
    /// </summary>
    public class Primitive : VisionStimulus
    {
        #region Fields

        PrimitivePara Para;
        VertexDeclaration pvdec;
        VertexBuffer vbuffer;
        IndexBuffer ibuffer;
        BasicEffect basiceffect;

        #endregion

        #region Properties

        /// <summary>
        /// Primitive Parameters
        /// </summary>
        public PrimitivePara PPara
        {
            get { return Para; }
            set { Para = value; }
        }

        /// <summary>
        /// Primitive Basic Effect
        /// </summary>
        public BasicEffect Effect
        {
            get { return basiceffect; }
            set { basiceffect = value; }
        }

        #endregion


        /// <summary>
        /// Set primitive parameters to default before Init()
        /// </summary>
        public Primitive()
        {
            Para = PrimitivePara.Default;
        }

        /// <summary>
        /// Init to default
        /// </summary>
        /// <param name="gd"></param>
        public Primitive(GraphicsDevice gd) : this()
        {
            Init(gd);
        }

        /// <summary>
        /// Init to custom settings
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="Ppara"></param>
        public Primitive(GraphicsDevice gd, PrimitivePara Ppara) : base(gd)
        {
            Init(gd, Ppara);
        }


        /// <summary>
        /// Init according to internal primitive parameters
        /// </summary>
        /// <param name="gd"></param>
        public override void Init(GraphicsDevice gd)
        {
            // Give default constuctor a chance to correct fullscreen resolution and visual configuration
            InitVS(gd);

            // Get buffer data ready
            pvdec = new VertexDeclaration(gd, VertexPositionColor.VertexElements);
            vbuffer = new VertexBuffer(gd, VertexPositionColor.SizeInBytes * Para.vertices.Length, BufferUsage.None);
            vbuffer.SetData<VertexPositionColor>(Para.vertices);
            ibuffer = new IndexBuffer(gd, sizeof(int) * Para.indices.Length, BufferUsage.None, IndexElementSize.ThirtyTwoBits);
            ibuffer.SetData<int>(Para.indices);

            // Get BasicEffect ready
            basiceffect = new BasicEffect(gd, null);
            basiceffect.VertexColorEnabled = true;

            basiceffect.World = Matrix.CreateFromYawPitchRoll(Para.BasePara.orientation3d.Y,
                                                                                        Para.BasePara.orientation3d.X,
                                                                                        Para.BasePara.orientation3d.Z) * Matrix.CreateTranslation(Para.BasePara.center);
            basiceffect.View = GlobalView();
            basiceffect.Projection = GlobalProj();
        }

        /// <summary>
        /// Init according to custom primitive parameters
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="para"></param>
        public void Init(GraphicsDevice gd, PrimitivePara para)
        {
            Para = para;
            Init(gd);
        }

        /// <summary>
        /// Draw Primitive's all vertices according to vertexbuffer
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="ptype"></param>
        public void VertexDraw(GraphicsDevice gd, PrimitiveType ptype)
        {
            Draw(gd, false, ptype, 0, Para.vertices.Length);
        }

        /// <summary>
        /// Draw Primitive's all vertices according to indexbuffer
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="ptype"></param>
        public void IndexDraw(GraphicsDevice gd, PrimitiveType ptype)
        {
            Draw(gd, true, ptype, 0, Para.indices.Length);
        }

        /// <summary>
        /// Draw Primitive
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="isindexdraw"></param>
        /// <param name="ptype"></param>
        /// <param name="start"></param>
        /// <param name="pcount"></param>
        public void Draw(GraphicsDevice gd, bool isindexdraw, PrimitiveType ptype, int start, int pcount)
        {
            if (Para.BasePara.visible)
            {
                gd.VertexDeclaration = pvdec;
                gd.Vertices[0].SetSource(vbuffer, 0, VertexPositionColor.SizeInBytes);
                gd.Indices = ibuffer;

                int temp;
                int vex_n;
                if (isindexdraw)
                {
                    vex_n = Para.indices.Length;
                }
                else
                {
                    vex_n = Para.vertices.Length;
                }
                switch (ptype)
                {
                    case PrimitiveType.PointList:
                        break;
                    case PrimitiveType.LineList:
                        temp = vex_n / 2;
                        if (pcount > temp)
                        {
                            pcount = temp;
                        }
                        break;
                    case PrimitiveType.LineStrip:
                        temp = vex_n - 1;
                        if (pcount > temp)
                        {
                            pcount = temp;
                        }
                        break;
                    case PrimitiveType.TriangleList:
                        gd.RenderState.CullMode = CullMode.None;
                        temp = vex_n / 3;
                        if (pcount > temp)
                        {
                            pcount = temp;
                        }
                        break;
                    case PrimitiveType.TriangleStrip:
                        gd.RenderState.CullMode = CullMode.None;
                        temp = vex_n - 2;
                        if (pcount > temp)
                        {
                            pcount = temp;
                        }
                        break;
                    case PrimitiveType.TriangleFan:
                        gd.RenderState.CullMode = CullMode.None;
                        temp = vex_n - 2;
                        if (pcount > temp)
                        {
                            pcount = temp;
                        }
                        break;
                }

                // Begin Draw
                basiceffect.Begin();
                basiceffect.CurrentTechnique.Passes[0].Begin();
                if (isindexdraw)
                {
                    gd.DrawIndexedPrimitives(ptype, 0, 0, vex_n, start, pcount);
                }
                else
                {
                    gd.DrawPrimitives(ptype, start, pcount);
                }
                basiceffect.CurrentTechnique.Passes[0].End();
                basiceffect.End();
            }
        }


        /// <summary>
        /// Reset Vertex Buffer from Primitive Parameter's vertices
        /// </summary>
        /// <param name="gd"></param>
        public void ReSetVB(GraphicsDevice gd)
        {
            gd.Vertices[0].SetSource(null, 0, VertexPositionColor.SizeInBytes);
            int temp = Para.vertices.Length * VertexPositionColor.SizeInBytes;
            if (temp > vbuffer.SizeInBytes)
            {
                vbuffer = new VertexBuffer(gd, temp, BufferUsage.None);
            }
            vbuffer.SetData<VertexPositionColor>(Para.vertices);
        }

        /// <summary>
        /// Reset Index Buffer from Primitive Parameter's indices
        /// </summary>
        /// <param name="gd"></param>
        public void ReSetIB(GraphicsDevice gd)
        {
            int temp = Para.indices.Length * sizeof(int);
            if (temp > ibuffer.SizeInBytes)
            {
                ibuffer = new IndexBuffer(gd, temp, BufferUsage.None, IndexElementSize.ThirtyTwoBits);
            }
            ibuffer.SetData<int>(Para.indices);
        }

        /// <summary>
        /// Change Primitive to a uniform color
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="color"></param>
        public void ChangeColor(GraphicsDevice gd, Color color)
        {
            for (int i = 0; i < Para.vertices.Length; i++)
            {
                Para.vertices[i].Color = color;
            }
            ReSetVB(gd);
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
        /// Set Primitive Visible State
        /// </summary>
        /// <param name="isvisible"></param>
        public override void SetVisible(bool isvisible)
        {
            Para.BasePara.visible = isvisible;
        }

    }
}
