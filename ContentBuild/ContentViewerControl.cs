using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using StiLib.Core;

namespace ContentBuild
{
    /// <summary>
    /// The main form is responsible for loading
    /// the content, this control just displays it.
    /// </summary>
    class ContentViewerControl : SLGDControl
    {
        Model model;
        Texture2D texture;
        SpriteFont spritefont;
        SoundEffect soundeffect;
        string FontShow = "This is how the currently builded SpriteFont looks like !";

        SpriteBatch spriteBatch;
        Vector2 textureposition;
        Matrix[] boneTransforms;
        Vector3 modelCenter;
        float modelRadius;
        Stopwatch timer;

        /// <summary>
        /// Current Model.
        /// </summary>
        public Model Model
        {
            get { return model; }
            set
            {
                model = value;
                if (model != null)
                {
                    MeasureModel();
                }
            }
        }

        /// <summary>
        /// Current Texture
        /// </summary>
        public Texture2D Image
        {
            get { return texture; }
            set
            {
                texture = value;
                if (texture != null)
                {
                    MeasureTexture();
                }
            }
        }

        /// <summary>
        /// Current SpriteFont
        /// </summary>
        public SpriteFont SpriteFont
        {
            get { return spritefont; }
            set
            {
                spritefont = value;
                if (spritefont != null)
                {
                    MeasureFontShow();
                }
            }
        }

        /// <summary>
        /// Current SoundEffect
        /// </summary>
        public SoundEffect SoundEffect
        {
            get { return soundeffect; }
            set
            {
                soundeffect = value;
                if (soundeffect != null)
                {
                    soundeffect.Play(1.0f, 0.0f, 0.0f, true);
                }
            }
        }


        /// <summary>
        /// Initializes the control.
        /// </summary>
        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            timer = Stopwatch.StartNew();

            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };
        }

        /// <summary>
        /// Update the control.
        /// </summary>
        protected override void Update()
        {
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            // Clear to the default control background color.
            Color backColor = new Color(BackColor.R, BackColor.G, BackColor.B);
            GraphicsDevice.Clear(backColor);

            if (model != null)
            {
                Vector3 eyePosition = modelCenter;
                eyePosition.Z += modelRadius * 2;
                eyePosition.Y += modelRadius;

                Matrix world = Matrix.CreateRotationY((float)timer.Elapsed.TotalSeconds);
                Matrix view = Matrix.CreateLookAt(eyePosition, modelCenter, Vector3.Up);
                Matrix projection = Matrix.CreatePerspectiveFieldOfView(1, GraphicsDevice.Viewport.AspectRatio, modelRadius / 100, modelRadius * 100);

                // Draw the model.
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = boneTransforms[mesh.ParentBone.Index] * world;
                        effect.View = view;
                        effect.Projection = projection;

                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;
                        effect.SpecularPower = 16;
                    }

                    mesh.Draw();
                }
            }

            if (texture != null)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(texture, textureposition, Color.White);
                spriteBatch.End();
            }

            if (spritefont != null)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(spritefont, FontShow, textureposition, Color.Pink);
                spriteBatch.End();
            }
        }

        /// <summary>
        /// Clear Content
        /// </summary>
        public void Clear()
        {
            model = null;
            texture = null;
            spritefont = null;
            soundeffect = null;
        }


        /// <summary>
        /// Examine model's size and center, so we can correctly display it.
        /// </summary>
        void MeasureModel()
        {
            // Look up the absolute bone transforms for this model.
            boneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            // Compute an (approximate) model center position by
            // averaging the center of each mesh bounding sphere.
            modelCenter = Vector3.Zero;
            foreach (ModelMesh mesh in model.Meshes)
            {
                BoundingSphere meshBounds = mesh.BoundingSphere;
                Matrix transform = boneTransforms[mesh.ParentBone.Index];
                Vector3 meshCenter = Vector3.Transform(meshBounds.Center, transform);

                modelCenter += meshCenter;
            }
            modelCenter /= model.Meshes.Count;

            // Now we know the center point, we can compute the model radius
            // by examining the radius of each mesh bounding sphere.
            modelRadius = 0;
            foreach (ModelMesh mesh in model.Meshes)
            {
                BoundingSphere meshBounds = mesh.BoundingSphere;
                Matrix transform = boneTransforms[mesh.ParentBone.Index];
                Vector3 meshCenter = Vector3.Transform(meshBounds.Center, transform);

                float transformScale = transform.Forward.Length();
                float meshRadius = (meshCenter - modelCenter).Length() + (meshBounds.Radius * transformScale);

                modelRadius = Math.Max(modelRadius, meshRadius);
            }
        }

        /// <summary>
        /// Examine texture's size and get the texture position.
        /// </summary>
        void MeasureTexture()
        {
            textureposition = new Vector2(ClientSize.Width / 2 - texture.Width / 2, ClientSize.Height / 2 - texture.Height / 2);
        }

        /// <summary>
        /// Examine FontShow's size and get the texture position.
        /// </summary>
        void MeasureFontShow()
        {
            Vector2 FontShowSize = spritefont.MeasureString(FontShow);
            textureposition = new Vector2(ClientSize.Width / 2 - FontShowSize.X / 2, ClientSize.Height / 2 - FontShowSize.Y / 2);
        }

    }
}
