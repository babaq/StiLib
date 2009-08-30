#region File Description
//-----------------------------------------------------------------------------
// SLModel.cs
//
// StiLib Model Stimulus.
// Copyright (c) Zhang Li. 2009-03-06.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Windows.Forms;
using StiLib.Core;
#endregion

namespace StiLib.Vision
{
    /// <summary>
    /// StiLib Model Service to Load, Render and Manipulate Models
    /// </summary>
    public class SLModel : VisionStimulus
    {
        #region Fields

        /// <summary>
        /// Model Parameters
        /// </summary>
        public ModelPara Para;
        /// <summary>
        /// Model Bone Transforms
        /// </summary>
        public Matrix[] BoneTransforms;
        Model model;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the XNA Model
        /// </summary>
        public Model Model
        {
            get { return model; }
        }

        /// <summary>
        /// Model Basic Parameters
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

        #endregion


        /// <summary>
        /// Sets Default ModelPara, need LoadContent() and Init()
        /// </summary>
        public SLModel()
        {
            Para = new ModelPara("");
        }

        /// <summary>
        /// Init to default SLModel parameter
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="modelname"></param>
        public SLModel(GraphicsDevice gd, IServiceProvider service, string path, string modelname)
            : base(gd)
        {
            Para = new ModelPara(modelname);
            LoadContent(service, path, modelname);
            Init(gd);
        }

        /// <summary>
        /// Init with Custom ModelPara
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="modelpara"></param>
        public SLModel(GraphicsDevice gd, IServiceProvider service, string path, ModelPara modelpara)
            : base(gd)
        {
            Para = modelpara;
            LoadContent(service, path, modelpara.modelname);
            Init(gd);
        }

        /// <summary>
        /// Init with Custom ModelPara and Configuration
        /// </summary>
        /// <param name="distance2display"></param>
        /// <param name="displayratio"></param>
        /// <param name="displaysize"></param>
        /// <param name="camera"></param>
        /// <param name="unit"></param>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="modelpara"></param>
        public SLModel(float distance2display, float displayratio, float displaysize, SLCamera camera, Unit unit, GraphicsDevice gd, IServiceProvider service, string path, ModelPara modelpara)
            : base(distance2display, displayratio, displaysize, gd, camera, unit)
        {
            Para = modelpara;
            LoadContent(service, path, modelpara.modelname);
            Init(gd);
        }

        /// <summary>
        /// Init with Custom ModelPara and StiLib Configuration File
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="slconfig"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="modelpara"></param>
        public SLModel(GraphicsDevice gd, AssemblySettings slconfig, IServiceProvider service, string path, ModelPara modelpara)
            : base(gd, slconfig)
        {
            Para = modelpara;
            LoadContent(service, path, modelpara.modelname);
            Init(gd);
        }

        /// <summary>
        /// Init with Custom ModelPara and Model Name
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="modelname"></param>
        /// <param name="modelpara"></param>
        public SLModel(GraphicsDevice gd, IServiceProvider service, string path, string modelname, ModelPara modelpara)
            : base(gd)
        {
            Para = modelpara;
            LoadContent(service, path, modelname);
            Init(gd);
        }


        /// <summary>
        /// Load the Model
        /// </summary>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="modelname"></param>
        public override void LoadContent(IServiceProvider service, string path, string modelname)
        {
            contentManager = new ContentManager(service, path);
            try
            {
                model = contentManager.Load<Model>(modelname);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error !");
            }

            Para.modelname = modelname;
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

            ori3DMatrix = GetOri3DMatrix(Para.BasePara.orientation3D);
            worldMatrix = Matrix.CreateTranslation(Para.BasePara.center);
        }

        /// <summary>
        /// Init with Custom ModelPara
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="modelpara"></param>
        public void Init(GraphicsDevice gd, ModelPara modelpara)
        {
            modelpara.modelname = Para.modelname;
            modelpara.BasePara.contentname = Para.BasePara.contentname;
            Para = modelpara;
            Init(gd);
        }

        /// <summary>
        /// Draw Model Using BasicEffect
        /// </summary>
        /// <param name="gd"></param>
        public override void Draw(GraphicsDevice gd)
        {
            Draw();
        }

        /// <summary>
        /// Draw Model Using BasicEffect
        /// </summary>
        public override void Draw()
        {
            if (Para.BasePara.visible)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.GraphicsDevice.RenderState.DepthBufferEnable = true;

                        effect.World = ori3DMatrix * BoneTransforms[mesh.ParentBone.Index] * worldMatrix;
                        effect.View = ViewMatrix;
                        effect.Projection = ProjectionMatrix;
                    }
                    mesh.Draw();
                }
            }
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            if (gdRef != null)
            {
                return new SLModel(distance2Display, displayRatio, displaySize, globalCamera, unit, gdRef, contentManager.ServiceProvider, contentManager.RootDirectory, Para);
            }
            else
            {
                SLConstant.ShowMessage("No Internal GraphicsDevice Reference, Please InitVS(GraphicsDevice gd) First !");
                return "No gdRef";
            }
        }

    }
}
