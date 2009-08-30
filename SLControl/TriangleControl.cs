using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Windows.Forms;
using StiLib.Core;

namespace SLControl
{
    /// <summary>
    /// This Control inherits from SLGDControl, which allows it to
    /// render using a GraphicsDevice. This control shows how to draw animating
    /// 3D graphics inside a WinForms application. It hooks the Application.Idle
    /// event, using this to invalidate the control, which will cause the animation
    /// to constantly redraw.
    /// </summary>
    class TriangleControl : SLGDControl
    {
        BasicEffect effect;
        Stopwatch timer;
        VertexDeclaration tvdec;

        public readonly VertexPositionColor[] Vertices = 
        {
            new VertexPositionColor(new Vector3(-1, -1, 0), Color.Red),
            new VertexPositionColor(new Vector3( 1, -1, 0), Color.Blue),
            new VertexPositionColor(new Vector3( 0,  1, 0), Color.Green),
        };


        /// <summary>
        /// Initializes the control.
        /// </summary>
        protected override void Initialize()
        {
            tvdec = new VertexDeclaration(GraphicsDevice, VertexPositionColor.VertexElements);
            effect = new BasicEffect(GraphicsDevice, null);
            effect.VertexColorEnabled = true;

            // Set transform matrices.
            Matrix View = Matrix.CreateLookAt(new Vector3(0, 0, 3), Vector3.Zero, Vector3.Up);
            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(1, GraphicsDevice.Viewport.AspectRatio, 1, 10);
            effect.View = View;
            effect.Projection = Projection;

            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };

            // Start the animation timer
            timer = Stopwatch.StartNew();
        }

        /// <summary>
        /// Update control drawing
        /// </summary>
        protected override void Update()
        {
            // Spin the triangle according to how much time has passed.
            float time = (float)timer.Elapsed.TotalSeconds;

            float yaw = time * 0.7f;
            float pitch = time * 0.8f;
            float roll = time * 0.9f;

            effect.World = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
        }

        /// <summary>
        /// Disposes the control.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        /// <summary>
        /// Control Drawing
        /// </summary>
        protected override void Draw()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.VertexDeclaration = tvdec;
            GraphicsDevice.RenderState.CullMode = CullMode.None;

            //Draw the triangle.
            effect.Begin();
            effect.CurrentTechnique.Passes[0].Begin();
            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, Vertices, 0, 1);
            effect.CurrentTechnique.Passes[0].End();
            effect.End();
        }

    }
}