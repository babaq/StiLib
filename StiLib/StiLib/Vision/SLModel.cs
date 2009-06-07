#region File Description
//-----------------------------------------------------------------------------
// SLModel.cs
//
// StiLib Model Service.
// Copyright (c) Zhang Li. 2009-03-06.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Windows.Forms;
#endregion

namespace StiLib.Vision
{
    /// <summary>
    /// StiLib Model Service to load, render, manipulate a model
    /// </summary>
    public class SLModel : VisionStimulus
    {
        #region Fields

        /// <summary>
        /// Model Parameter
        /// </summary>
        public ModelPara Para;
        Model model;
        ContentManager cm;
        Matrix[] BoneTransforms;
        Matrix Matrix_R;
        Matrix Matrix_T;

        #endregion

        #region Properties

        /// <summary>
        /// Get the XNA Model
        /// </summary>
        public Model Model
        {
            get { return model; }
        }

        /// <summary>
        /// Get Content Manager
        /// </summary>
        public ContentManager Content
        {
            get { return cm; }
        }

        #endregion


        /// <summary>
        /// Set SLModel parameters to default before LoadContent() and Init()
        /// </summary>
        public SLModel()
        {
            Para = ModelPara.Default;
            RotateModel(Para.BasePara.orientation3d);
            TranslateModel(Para.BasePara.center);
        }

        /// <summary>
        /// Init to default SLModel parameter
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="mfilename"></param>
        public SLModel(GraphicsDevice gd, IServiceProvider service, string path, string mfilename) : this()
        {
            LoadContent(service, path, mfilename);
            Init(gd);
        }

        /// <summary>
        /// Init
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="mpara"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="mfilename"></param>
        public SLModel(GraphicsDevice gd, ModelPara mpara, IServiceProvider service, string path, string mfilename)
        {
            Para = mpara;
            RotateModel(Para.BasePara.orientation3d);
            TranslateModel(Para.BasePara.center);
            LoadContent(service, path, mfilename);
            Init(gd);
        }


        /// <summary>
        /// Load a Model
        /// </summary>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="mfilename"></param>
        public void LoadContent(IServiceProvider service, string path, string mfilename)
        {
            cm = new ContentManager(service, path);
            try
            {
                model = cm.Load<Model>(mfilename);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error !");
            }
            Para.MFilename = mfilename;
        }

        /// <summary>
        /// Init according to internal SLModel parameters
        /// </summary>
        /// <param name="gd"></param>
        public override void Init(GraphicsDevice gd)
        {
            InitVS(gd);

            BoneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(BoneTransforms);
        }

        /// <summary>
        /// Init to custom SLModel parameters
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="mpara"></param>
        public void Init(GraphicsDevice gd, ModelPara mpara)
        {
            Para = mpara;
            Init(gd);
        }

        /// <summary>
        /// Draw Model Using BasicEffect
        /// </summary>
        public void Draw()
        {
            if (Para.BasePara.visible)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.GraphicsDevice.RenderState.DepthBufferEnable = true;

                        effect.World = Matrix_R * BoneTransforms[mesh.ParentBone.Index] * Matrix_T;
                        effect.View = GlobalView();
                        effect.Projection = GlobalProj();
                    }
                    mesh.Draw();
                }
            }
        }


        /// <summary>
        /// Rotate Model in model space
        /// </summary>
        /// <param name="rotate"></param>
        public void RotateModel(Vector3 rotate)
        {
            Matrix_R = Matrix.CreateFromYawPitchRoll(rotate.Y, rotate.X, rotate.Z);
        }

        /// <summary>
        /// Translate Model in world space
        /// </summary>
        /// <param name="translate"></param>
        public void TranslateModel(Vector3 translate)
        {
            Matrix_T = Matrix.CreateTranslation(translate);
        }

        /// <summary>
        /// Set World Transform
        /// </summary>
        /// <param name="world"></param>
        public override void SetWorld(Matrix world)
        {
            Matrix_T = world;
        }

        /// <summary>
        /// Set Visible State
        /// </summary>
        /// <param name="isvisible"></param>
        public override void SetVisible(bool isvisible)
        {
            Para.BasePara.visible = isvisible;
        }

    }
}
