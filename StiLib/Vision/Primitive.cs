#region File Description
//-----------------------------------------------------------------------------
// Primitive.cs
//
// StiLib Visual Primitive Stimulus
// Copyright (c) Zhang Li. 2009-01-14.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using StiLib.Core;
#endregion

namespace StiLib.Vision
{
    /// <summary>
    /// StiLib Visual Primitive Using VertexPositionColor
    /// </summary>
    public class Primitive : VisionStimulus
    {
        #region Fields

        /// <summary>
        /// Primitive Parameters
        /// </summary>
        public PrimitivePara Para;

        #endregion

        #region Properties

        /// <summary>
        /// Primitive Basic Parameters
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
        /// Primitive Vertex Array
        /// </summary>
        public override VertexPositionColor[] VertexArray
        {
            get { return Para.vertices; }
            set { Para.vertices = value; }
        }

        /// <summary>
        /// Primitive Index Array
        /// </summary>
        public override int[] IndexArray
        {
            get { return Para.indices; }
            set { Para.indices = value; }
        }

        #endregion


        /// <summary>
        /// Sets default PrimitivePara, need Init()
        /// </summary>
        public Primitive()
        {
            Para = PrimitivePara.Default;
        }

        /// <summary>
        /// Init with Default PrimitivePara
        /// </summary>
        /// <param name="gd"></param>
        public Primitive(GraphicsDevice gd)
            : base(gd)
        {
            Para = PrimitivePara.Default;
            Init(gd);
        }

        /// <summary>
        /// Init with Custom PrimitivePara
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="primitivepara"></param>
        public Primitive(GraphicsDevice gd, PrimitivePara primitivepara)
            : base(gd)
        {
            Para = primitivepara;
            Init(gd);
        }

        /// <summary>
        /// Init with Custom PrimitivePara and Configuration
        /// </summary>
        /// <param name="distance2display"></param>
        /// <param name="displayratio"></param>
        /// <param name="displaysize"></param>
        /// <param name="camera"></param>
        /// <param name="unit"></param>
        /// <param name="gd"></param>
        /// <param name="primitivepara"></param>
        public Primitive(float distance2display, float displayratio, float displaysize, SLCamera camera, Unit unit, GraphicsDevice gd, PrimitivePara primitivepara)
            : base(distance2display, displayratio, displaysize, gd, camera, unit)
        {
            Para = primitivepara;
            Init(gd);
        }

        /// <summary>
        /// Init with Custom PrimitivePara and StiLib Configuration File
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="slconfig"></param>
        /// <param name="primitivepara"></param>
        public Primitive(GraphicsDevice gd, AssemblySettings slconfig, PrimitivePara primitivepara)
            : base(gd, slconfig)
        {
            Para = primitivepara;
            Init(gd);
        }


        /// <summary>
        /// Init according to internal primitive parameters
        /// </summary>
        /// <param name="gd"></param>
        public override void Init(GraphicsDevice gd)
        {
            InitVS(gd);

            vertexDeclaration = new VertexDeclaration(gd, VertexPositionColor.VertexElements);
            SetVertexBuffer(gd);
            SetIndexBuffer(gd);

            // Get BasicEffect Ready
            basicEffect = new BasicEffect(gd, null);
            basicEffect.VertexColorEnabled = true;

            ori3DMatrix = GetOri3DMatrix(Para.BasePara.orientation3D);
            worldMatrix = Matrix.CreateTranslation(Para.BasePara.center);
            ViewMatrix = ViewMatrix;
            ProjectionMatrix = ProjectionMatrix;
        }

        /// <summary>
        /// Init according to custom primitive parameters
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="primitivepara"></param>
        public void Init(GraphicsDevice gd, PrimitivePara primitivepara)
        {
            primitivepara.BasePara.contentname = Para.BasePara.contentname;
            Para = primitivepara;
            Init(gd);
        }

        /// <summary>
        /// Draw Total Primitive according to indexbuffer and internal primitive type
        /// </summary>
        /// <param name="gd"></param>
        public override void Draw(GraphicsDevice gd)
        {
            IndexDraw(gd, Para.BasePara.primitivetype);
        }

        /// <summary>
        /// Draw Total Primitive using vertexbuffer
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="primitivetype"></param>
        public void VertexDraw(GraphicsDevice gd, PrimitiveType primitivetype)
        {
            VertexDraw(gd, primitivetype, 0, CheckPrimitiveCount(primitivetype, Para.vertices.Length, Para.vertices.Length));
        }

        /// <summary>
        /// Draw Primitive Using Vertex
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="primitivetype"></param>
        /// <param name="startvertex"></param>
        /// <param name="primitivecount"></param>
        public void VertexDraw(GraphicsDevice gd, PrimitiveType primitivetype, int startvertex, int primitivecount)
        {
            if (Para.BasePara.visible)
            {
                gd.VertexDeclaration = vertexDeclaration;
                gd.Vertices[0].SetSource(vertexBuffer, 0, VertexPositionColor.SizeInBytes);
                gd.RenderState.CullMode = CullMode.None;
                basicEffect.World = ori3DMatrix * worldMatrix;

                // Begin Draw
                basicEffect.Begin();
                basicEffect.CurrentTechnique.Passes[0].Begin();

                gd.DrawPrimitives(primitivetype, startvertex, primitivecount);

                basicEffect.CurrentTechnique.Passes[0].End();
                basicEffect.End();
            }
        }

        /// <summary>
        /// Draw Total Primitive using indexbuffer
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="primitivetype"></param>
        public void IndexDraw(GraphicsDevice gd, PrimitiveType primitivetype)
        {
            IndexDraw(gd, primitivetype, 0, 0, Para.vertices.Length, 0, CheckPrimitiveCount(primitivetype, Para.indices.Length, Para.indices.Length));
        }

        /// <summary>
        /// Draw Primitive Using Index
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="primitivetype"></param>
        /// <param name="basevertex"></param>
        /// <param name="minvertexindex"></param>
        /// <param name="numvertices"></param>
        /// <param name="startindex"></param>
        /// <param name="primitivecount"></param>
        public void IndexDraw(GraphicsDevice gd, PrimitiveType primitivetype, int basevertex, int minvertexindex, int numvertices, int startindex, int primitivecount)
        {
            if (Para.BasePara.visible)
            {
                gd.VertexDeclaration = vertexDeclaration;
                gd.Vertices[0].SetSource(vertexBuffer, 0, VertexPositionColor.SizeInBytes);
                gd.Indices = indexBuffer;
                gd.RenderState.CullMode = CullMode.None;
                basicEffect.World = ori3DMatrix*worldMatrix;

                // Begin Draw
                basicEffect.Begin();
                basicEffect.CurrentTechnique.Passes[0].Begin();

                gd.DrawIndexedPrimitives(primitivetype, basevertex, minvertexindex, numvertices, startindex,
                                         primitivecount);

                basicEffect.CurrentTechnique.Passes[0].End();
                basicEffect.End();
            }
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
            SetVertexBuffer(gd);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            if (gdRef != null)
            {
                return new Primitive(distance2Display, displayRatio, displaySize, globalCamera, unit, gdRef, Para);
            }
            else
            {
                SLConstant.ShowMessage("No Internal GraphicsDevice Reference, Please InitVS(GraphicsDevice gd) First !");
                return "No gdRef";
            }
        }

    }
}
