#region File Description
//-----------------------------------------------------------------------------
// VSCollection.cs
//
// StiLib Vision Stimulus Collection Service
// Copyright (c) Zhang Li. 2009-07-11.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Windows.Forms;
using StiLib.Core;
#endregion

namespace StiLib.Vision
{
    using MediaState = Microsoft.Xna.Framework.Media.MediaState;
    /// <summary>
    /// StiLib Visual Stimulus Collection
    /// </summary>
    public class VSCollection<vsType> where vsType : VisionStimulus, new()
    {
        #region Fields

        /// <summary>
        /// Vision Stimulus Collection List
        /// </summary>
        public List<vsType> Collection;
        /// <summary>
        /// Collection Parameters
        /// </summary>
        public CollectionPara Para;
        /// <summary>
        /// Time of Last Frame
        /// </summary>
        public double LastFrameTime;
        /// <summary>
        /// Used in Random Algorithm
        /// </summary>
        public SLRandom Random;
        /// <summary>
        /// Vision Stimulus Index that need to be reset 
        /// </summary>
        public List<int> ResetList;
        /// <summary>
        /// Vision Stimulus Index that need to be updated
        /// </summary>
        public List<int> UpdateList;

        #endregion


        /// <summary>
        /// Sets Default Collection Parameters, need Init()
        /// </summary>
        public VSCollection()
        {
            Collection = new List<vsType>();
            Para = CollectionPara.Default;
            Random = new SLRandom();
            ResetList = new List<int>();
            UpdateList = new List<int>();
        }

        /// <summary>
        /// Sets Custom Collection Parameters, need Init()
        /// </summary>
        /// <param name="collectpara"></param>
        public VSCollection(CollectionPara collectpara)
        {
            Collection = new List<vsType>();
            Para = collectpara;
            Random = new SLRandom();
            ResetList = new List<int>();
            UpdateList = new List<int>();
        }

        /// <summary>
        /// Init Each Element of Collection According to Custom Vision Stimulus Instance
        /// </summary>
        /// <param name="count"></param>
        /// <param name="gd"></param>
        /// <param name="visionstimulus"></param>
        public VSCollection(int count, GraphicsDevice gd, VisionStimulus visionstimulus)
            : this()
        {
            Init(count, gd, visionstimulus);
        }

        /// <summary>
        /// Init Each Element of Collection According to Custom Vision Stimulus Instance and Collection Parameters
        /// </summary>
        /// <param name="count"></param>
        /// <param name="gd"></param>
        /// <param name="visionstimulus"></param>
        /// <param name="collectpara"></param>
        public VSCollection(int count, GraphicsDevice gd, VisionStimulus visionstimulus, CollectionPara collectpara)
            : this(collectpara)
        {
            Init(count, gd, visionstimulus);
        }

        /// <summary>
        /// Init Each Element of Collection According to Custom Vision Stimulus Type and Parameters Object
        /// </summary>
        /// <param name="count"></param>
        /// <param name="vstype"></param>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="vspara"></param>
        public VSCollection(int count, VSType vstype, GraphicsDevice gd, IServiceProvider service, string path, object vspara)
            : this()
        {
            Init(count, vstype, gd, service, path, vspara);
        }

        /// <summary>
        /// Init Each Element of Collection According to Custom CollectionPara, Vision Stimulus Type and Parameters Object
        /// </summary>
        /// <param name="count"></param>
        /// <param name="vstype"></param>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="vspara"></param>
        /// <param name="collectpara"></param>
        public VSCollection(int count, VSType vstype, GraphicsDevice gd, IServiceProvider service, string path, object vspara, CollectionPara collectpara)
            : this(collectpara)
        {
            Init(count, vstype, gd, service, path, vspara);
        }

        /// <summary>
        /// Init Each Element of Collection According to StiLib Configuration, Custom Vision Stimulus Type and Parameters Object
        /// </summary>
        /// <param name="count"></param>
        /// <param name="vstype"></param>
        /// <param name="gd"></param>
        /// <param name="slconfig"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="vspara"></param>
        public VSCollection(int count, VSType vstype, GraphicsDevice gd, AssemblySettings slconfig, IServiceProvider service, string path, object vspara)
            : this()
        {
            Init(count, vstype, gd, slconfig, service, path, vspara);
        }

        /// <summary>
        /// Init Each Element of Collection According to StiLib Configuration, CollectionPara, 
        /// Custom Vision Stimulus Type and Parameters Object
        /// </summary>
        /// <param name="count"></param>
        /// <param name="vstype"></param>
        /// <param name="gd"></param>
        /// <param name="slconfig"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="vspara"></param>
        /// <param name="collectpara"></param>
        public VSCollection(int count, VSType vstype, GraphicsDevice gd, AssemblySettings slconfig, IServiceProvider service, string path, object vspara, CollectionPara collectpara)
            : this(collectpara)
        {
            Init(count, vstype, gd, slconfig, service, path, vspara);
        }


        /// <summary>
        /// Init Each Element of Collection According to Custom Vision Stimulus Instance
        /// </summary>
        /// <param name="count"></param>
        /// <param name="gd"></param>
        /// <param name="visionstimulus"></param>
        public virtual void Init(int count, GraphicsDevice gd, VisionStimulus visionstimulus)
        {
            if (count <= 0)
            {
                MessageBox.Show("Collection Number <= 0, Automatically Set To One !");
                count = 1;
            }

            for (int i = 0; i < count; i++)
            {
                Collection.Add(visionstimulus.Clone() as vsType);
            }

            RandomCenterSpeed(1.0f);
        }

        /// <summary>
        /// Init Each Element of Collection According to Custom Vision Stimulus Type and Parameters Object
        /// </summary>
        /// <param name="count"></param>
        /// <param name="vstype"></param>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="vspara"></param>
        public virtual void Init(int count, VSType vstype, GraphicsDevice gd, IServiceProvider service, string path, object vspara)
        {
            vsType temp = null;

            try
            {
                switch (vstype)
                {
                    case VSType.Bar:
                        temp = new Bar(gd, (BarPara)vspara) as vsType;
                        break;
                    case VSType.Grating:
                        temp = new Grating(gd, service, path, (GratingPara)vspara) as vsType;
                        break;
                    case VSType.Image:
                        temp = new Image(gd, service, path, (ImagePara)vspara) as vsType;
                        break;
                    case VSType.Primitive:
                        temp = new Primitive(gd, (PrimitivePara)vspara) as vsType;
                        break;
                    case VSType.SLModel:
                        temp = new SLModel(gd, service, path, (ModelPara)vspara) as vsType;
                        break;
                    case VSType.SLQuad:
                        temp = new SLQuad(gd, service, path, (Quad)vspara) as vsType;
                        break;
                    case VSType.SLVideo:
                        temp = new SLVideo(gd, service, path, (VideoPara)vspara) as vsType;
                        break;
                    case VSType.Text:
                        temp = new Text(gd, service, path, (TextPara)vspara) as vsType;
                        break;
                }

                Init(count, gd, temp);
            }
            catch (Exception e)
            {
                SLConstant.ShowException(e);
            }
        }

        /// <summary>
        /// Init Each Element of Collection According to StiLib Configuration, Custom Vision Stimulus Type and Parameters Object
        /// </summary>
        /// <param name="count"></param>
        /// <param name="vstype"></param>
        /// <param name="gd"></param>
        /// <param name="slconfig"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="vspara"></param>
        public virtual void Init(int count, VSType vstype, GraphicsDevice gd, AssemblySettings slconfig, IServiceProvider service, string path, object vspara)
        {
            vsType temp = null;

            try
            {
                switch (vstype)
                {
                    case VSType.Bar:
                        temp = new Bar(gd, slconfig, (BarPara)vspara) as vsType;
                        break;
                    case VSType.Grating:
                        temp = new Grating(gd, slconfig, service, path, (GratingPara)vspara) as vsType;
                        break;
                    case VSType.Image:
                        temp = new Image(gd, slconfig, service, path, (ImagePara)vspara) as vsType;
                        break;
                    case VSType.Primitive:
                        temp = new Primitive(gd, slconfig, (PrimitivePara)vspara) as vsType;
                        break;
                    case VSType.SLModel:
                        temp = new SLModel(gd, slconfig, service, path, (ModelPara)vspara) as vsType;
                        break;
                    case VSType.SLQuad:
                        temp = new SLQuad(gd, slconfig, service, path, (Quad)vspara) as vsType;
                        break;
                    case VSType.SLVideo:
                        temp = new SLVideo(gd, slconfig, service, path, (VideoPara)vspara) as vsType;
                        break;
                    case VSType.Text:
                        temp = new Text(gd, slconfig, service, path, (TextPara)vspara) as vsType;
                        break;
                }

                Init(count, gd, temp);
            }
            catch (Exception e)
            {
                SLConstant.ShowException(e);
            }
        }

        /// <summary>
        /// Update each element of collection
        /// </summary>
        /// <param name="time"></param>
        public virtual void Update(double time)
        {
            float elapse = (float)(time - LastFrameTime);
            LastFrameTime = time;
            ResetList.Clear();
            UpdateList.Clear();

            for (int i = 0; i < Collection.Count; i++)
            {
                Para.CollectionCenter += Vector3.Multiply(Para.CollectionSpeed, elapse);
                Collection[i].Center += Vector3.Multiply(Collection[i].Speed3D, elapse);

                if (Vector3.Distance(Para.CollectionCenter, Para.BasePara.center) > Para.BasePara.space / 2)
                {
                    Para.CollectionSpeed = Vector3.Reflect(Para.CollectionSpeed, Vector3.Normalize(Para.BasePara.center - Para.CollectionCenter));
                }

                if (Vector3.Distance(Collection[i].Center, Para.CollectionCenter) > Collection[i].BasePara.space / 2)
                {
                    ResetList.Add(i);
                }
                else
                {
                    UpdateList.Add(i);
                }

            }

            RandomCenterSpeed(ResetList);

            for (int i = 0; i < UpdateList.Count; i++)
            {
                Collection[UpdateList[i]].Ori3DMatrix = VisionStimulus.GetOri3DMatrix(Collection[UpdateList[i]].BasePara.orientation3D + Collection[UpdateList[i]].BasePara.rotationspeed3D * (float)time);
                Collection[UpdateList[i]].WorldMatrix = Matrix.CreateTranslation(Collection[UpdateList[i]].Center);
            }
        }

        /// <summary>
        /// Draw each element of collection
        /// </summary>
        /// <param name="gd"></param>
        public virtual void Draw(GraphicsDevice gd)
        {
            for (int i = 0; i < Collection.Count; i++)
            {
                Collection[i].Draw(gd);
            }
        }

        /// <summary>
        /// Optimized Batch Draw
        /// </summary>
        /// <param name="gd"></param>
        public virtual void BatchDraw(GraphicsDevice gd)
        {
            switch (Collection[0].VSType)
            {
                case VSType.Bar:
                case VSType.Primitive:
                    PrimitiveType ptype = Collection[0].BasePara.primitivetype;
                    int vexn = Collection[0].VertexArray.Length;
                    int pcount = VisionStimulus.CheckPrimitiveCount(ptype, Collection[0].IndexArray.Length, Collection[0].IndexArray.Length);

                    gd.VertexDeclaration = Collection[0].vertexDeclaration;
                    gd.Vertices[0].SetSource(Collection[0].vertexBuffer, 0, VertexPositionColor.SizeInBytes);
                    gd.Indices = Collection[0].indexBuffer;
                    gd.RenderState.CullMode = CullMode.None;

                    // Begin Draw
                    Collection[0].basicEffect.Begin();
                    Collection[0].basicEffect.CurrentTechnique.Passes[0].Begin();
                    for (int i = 0; i < Collection.Count; i++)
                    {
                        if (Collection[i].Visible)
                        {
                            Collection[0].basicEffect.World = Collection[i].Ori3DMatrix * Collection[i].WorldMatrix;
                            Collection[0].basicEffect.CommitChanges();
                            gd.DrawIndexedPrimitives(ptype, 0, 0, vexn, 0, pcount);
                        }
                    }
                    Collection[0].basicEffect.CurrentTechnique.Passes[0].End();
                    Collection[0].basicEffect.End();
                    break;
                case VSType.Grating:
                    ptype = Collection[0].BasePara.primitivetype;
                    vexn = Collection[0].VertexArray.Length;
                    pcount = VisionStimulus.CheckPrimitiveCount(ptype, Collection[0].IndexArray.Length, Collection[0].IndexArray.Length);

                    gd.VertexDeclaration = Collection[0].vertexDeclaration;
                    gd.Vertices[0].SetSource(Collection[0].vertexBuffer, 0, VertexPositionColor.SizeInBytes);
                    gd.Indices = Collection[0].indexBuffer;
                    gd.RenderState.CullMode = CullMode.None;

                    Grating grating = Collection[0] as Grating;
                    // Begin Draw
                    grating.gratingeffect.Begin();
                    grating.gratingeffect.CurrentTechnique.Passes[0].Begin();
                    for (int i = 0; i < Collection.Count; i++)
                    {
                        if (Collection[i].Visible)
                        {
                            grating.eWorld.SetValue(Collection[i].Ori3DMatrix * Collection[i].WorldMatrix);
                            grating.gratingeffect.CommitChanges();
                            gd.DrawIndexedPrimitives(ptype, 0, 0, vexn, 0, pcount);
                        }
                    }
                    grating.gratingeffect.CurrentTechnique.Passes[0].End();
                    grating.gratingeffect.End();
                    break;
                case VSType.Image:
                    Vector3 Center;
                    float unitFactor = Collection[0].unitFactor;
                    Image image = Collection[0] as Image;
                    // Begin Draw
                    image.spriteBatch.Begin();
                    for (int i = 0; i < Collection.Count; i++)
                    {
                        if (Collection[i].Visible)
                        {
                            Center = Collection[i].Center;
                            image.spriteBatch.Draw(image.texture, new Vector2(Center.X * unitFactor + gd.Viewport.Width / 2 - image.texture.Width / 2, gd.Viewport.Height / 2 - Center.Y * unitFactor - image.texture.Height / 2), Collection[i].BasePara.color);
                        }
                    }
                    image.spriteBatch.End();
                    break;
                case VSType.SLModel:
                    SLModel model = Collection[0] as SLModel;
                    foreach (ModelMesh mesh in model.Model.Meshes)
                    {
                        for (int i = 0; i < Collection.Count; i++)
                        {
                            if (Collection[i].Visible)
                            {
                                foreach (BasicEffect effect in mesh.Effects)
                                {
                                    effect.EnableDefaultLighting();
                                    effect.GraphicsDevice.RenderState.DepthBufferEnable = true;

                                    effect.World = Collection[i].Ori3DMatrix * model.BoneTransforms[mesh.ParentBone.Index] * Collection[i].WorldMatrix;
                                    effect.View = model.ViewMatrix;
                                    effect.Projection = model.ProjectionMatrix;
                                }
                                mesh.Draw();
                            }
                        }
                    }
                    break;
                case VSType.SLQuad:
                    SLQuad quad = Collection[0] as SLQuad;
                    ptype = Collection[0].BasePara.primitivetype;
                    vexn = quad.Para.vertices.Length;
                    pcount = VisionStimulus.CheckPrimitiveCount(ptype, Collection[0].IndexArray.Length, Collection[0].IndexArray.Length);

                    gd.VertexDeclaration = quad.vertexDeclaration;
                    gd.Vertices[0].SetSource(quad.vertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes);
                    gd.Indices = quad.indexBuffer;
                    gd.RenderState.CullMode = CullMode.None;
                    gd.RenderState.AlphaBlendEnable = true;
                    gd.RenderState.AlphaTestEnable = true;
                    gd.RenderState.AlphaFunction = CompareFunction.Greater;

                    // Begin Draw
                    quad.basicEffect.Begin();
                    quad.basicEffect.CurrentTechnique.Passes[0].Begin();
                    for (int i = 0; i < Collection.Count; i++)
                    {
                        if (Collection[i].Visible)
                        {
                            quad.basicEffect.World = Collection[i].Ori3DMatrix * Collection[i].WorldMatrix;
                            quad.basicEffect.CommitChanges();
                            gd.DrawIndexedPrimitives(ptype, 0, 0, vexn, 0, pcount);
                        }
                    }
                    quad.basicEffect.CurrentTechnique.Passes[0].End();
                    quad.basicEffect.End();
                    break;
                case VSType.SLVideo:
                    SLVideo video = Collection[0] as SLVideo;
                    unitFactor = Collection[0].unitFactor;
                    // Begin Draw
                    video.spriteBatch.Begin();
                    for (int i = 0; i < Collection.Count; i++)
                    {
                        if (Collection[i].Visible)
                        {
                            Center = Collection[i].Center;
                            if (video.MediaState == MediaState.Stopped)
                            {
                                video.Play();
                            }
                            if (video.MediaState != MediaState.Stopped)
                                video.texture = video.Player.GetTexture();
                            if (video.texture != null)
                            {
                                video.spriteBatch.Draw(video.texture, new Vector2(Center.X * unitFactor + gd.Viewport.Width / 2 - video.texture.Width / 2, gd.Viewport.Height / 2 - Center.Y * unitFactor - video.texture.Height / 2), Collection[i].BasePara.color);
                            }
                        }
                    }
                    video.spriteBatch.End();
                    break;
                case VSType.Text:
                    Text text = Collection[0] as Text;
                    Vector2 size = text.spriteFont.MeasureString(SLConstant.Help);
                    unitFactor = Collection[0].unitFactor;
                    // Begin Draw
                    text.spriteBatch.Begin();
                    for (int i = 0; i < Collection.Count; i++)
                    {
                        if (Collection[i].Visible)
                        {
                            Center = Collection[i].Center;
                            text.spriteBatch.DrawString(text.spriteFont, SLConstant.Help, new Vector2(Center.X * unitFactor + gd.Viewport.Width / 2 - size.X / 2, gd.Viewport.Height / 2 - Center.Y * unitFactor - size.Y / 2), Collection[i].BasePara.color);
                        }
                    }
                    text.spriteBatch.End();
                    break;
            }
        }

        /// <summary>
        /// Randomize Collection Using Indices
        /// </summary>
        /// <param name="indices"></param>
        public virtual void RandomCenterSpeed(List<int> indices)
        {
            var randomcenter = Random.RandomPosition(indices.Count);
            var randomspeed = Random.RandomPosition(indices.Count);
            for (int i = 0; i < indices.Count; i++)
            {
                randomcenter[i].X = randomcenter[i].X * (Collection[indices[i]].BasePara.space / 2) * (float)Math.Sin(Math.PI / 4);
                randomcenter[i].Y = randomcenter[i].Y * (Collection[indices[i]].BasePara.space / 2) * (float)Math.Sin(Math.PI / 4);
                randomcenter[i].Z = randomcenter[i].Z * (Collection[indices[i]].BasePara.space / 2) * (float)Math.Sin(Math.PI / 4);

                Collection[indices[i]].Center = randomcenter[i] + Para.CollectionCenter;
                Collection[indices[i]].Speed3D = randomspeed[i] + Para.CollectionSpeed;
                Collection[indices[i]].Ori3DMatrix = VisionStimulus.GetOri3DMatrix(Collection[indices[i]].BasePara.orientation3D);
                Collection[indices[i]].WorldMatrix = Matrix.CreateTranslation(Collection[indices[i]].Center);
            }
        }

        /// <summary>
        /// Randomize Part of Collection Elements
        /// </summary>
        /// <param name="percentage">[0, 1]</param>
        public virtual void RandomCenterSpeed(float percentage)
        {
            int num = (int)Math.Round(Collection.Count * percentage);
            var randomcenter = Random.RandomPosition(num);
            var randomspeed = Random.RandomPosition(num);
            for (int i = 0; i < num; i++)
            {
                randomcenter[i].X = randomcenter[i].X * (Collection[i].BasePara.space / 2) * (float)Math.Sin(Math.PI / 4);
                randomcenter[i].Y = randomcenter[i].Y * (Collection[i].BasePara.space / 2) * (float)Math.Sin(Math.PI / 4);
                randomcenter[i].Z = randomcenter[i].Z * (Collection[i].BasePara.space / 2) * (float)Math.Sin(Math.PI / 4);

                Collection[i].Center = randomcenter[i] + Para.CollectionCenter;
                Collection[i].Speed3D = randomspeed[i] + Para.CollectionSpeed;
                Collection[i].Ori3DMatrix = VisionStimulus.GetOri3DMatrix(Collection[i].BasePara.orientation3D);
                Collection[i].WorldMatrix = Matrix.CreateTranslation(Collection[i].Center);
            }
        }


        /// <summary>
        /// Set All Visible State
        /// </summary>
        /// <param name="isvisible"></param>
        public virtual void SetVisible(bool isvisible)
        {
            for (int i = 0; i < Collection.Count; i++)
            {
                Collection[i].Visible = isvisible;
            }
        }

        /// <summary>
        /// Set All Projection Type
        /// </summary>
        /// <param name="projtype"></param>
        public virtual void SetProjType(ProjectionType projtype)
        {
            for (int i = 0; i < Collection.Count; i++)
            {
                Collection[i].ProjectionType = projtype;
            }
        }

        /// <summary>
        /// Set All Global Camera
        /// </summary>
        /// <param name="camera"></param>
        public virtual void SetGlobalCamera(SLCamera camera)
        {
            for (int i = 0; i < Collection.Count; i++)
            {
                Collection[i].globalCamera = camera.Clone() as SLCamera;
            }
        }

        /// <summary>
        /// Set All 3D Space Speed
        /// </summary>
        /// <param name="speed3d"></param>
        public virtual void SetSpeed3D(Vector3 speed3d)
        {
            for (int i = 0; i < Collection.Count; i++)
            {
                Collection[i].Speed3D = speed3d;
            }
        }

    }

    /// <summary>
    /// Optimized Primitive Collection Using Hardware Instancing, need Shader Model 3.0
    /// </summary>
    public class vscPrimitive
    {
        /// <summary>
        /// The Original Primitive Instance
        /// </summary>
        public Primitive PrimitiveInstance;
        /// <summary>
        /// Collection Parameters
        /// </summary>
        public CollectionPara Para;
        /// <summary>
        /// Time of Last Frame
        /// </summary>
        public double LastFrameTime;
        /// <summary>
        /// Used in Random Algorithm
        /// </summary>
        public SLRandom Random;
        /// <summary>
        /// Primitive Instances vsBasePara
        /// </summary>
        public vsBasePara[] InstancePara;
        /// <summary>
        /// Primitive Instances Transform Matrix
        /// </summary>
        public Matrix[] InstanceMatrix;
        Effect hiEffect;
        DynamicVertexBuffer InstanceDataStream;
        PrimitiveType ptype;
        int vexn;
        int pcount;
        bool isshadermodel3;


        /// <summary>
        /// Sets Custom CollectionPara, need Init()
        /// </summary>
        /// <param name="collectpara"></param>
        public vscPrimitive(CollectionPara collectpara)
        {
            Para = collectpara;
            Random = new SLRandom();
        }

        /// <summary>
        /// Init with Custom Settings
        /// </summary>
        /// <param name="count"></param>
        /// <param name="gd"></param>
        /// <param name="primitive"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="collectpara"></param>
        public vscPrimitive(int count, GraphicsDevice gd, Primitive primitive, IServiceProvider service, string path, CollectionPara collectpara)
            : this(collectpara)
        {
            Init(count, gd, primitive, service, path);
        }

        /// <summary>
        /// Init with Custom PrimitivePara
        /// </summary>
        /// <param name="count"></param>
        /// <param name="gd"></param>
        /// <param name="primitivepara"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="collectpara"></param>
        public vscPrimitive(int count, GraphicsDevice gd, PrimitivePara primitivepara, IServiceProvider service, string path, CollectionPara collectpara)
            : this(collectpara)
        {
            PrimitiveInstance = new Primitive(gd, primitivepara);
            Init(count, gd, PrimitiveInstance, service, path);
        }

        /// <summary>
        /// Init with Custom StiLib Configuration and PrimitivePara
        /// </summary>
        /// <param name="count"></param>
        /// <param name="gd"></param>
        /// <param name="slconfig"></param>
        /// <param name="primitivepara"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="collectpara"></param>
        public vscPrimitive(int count, GraphicsDevice gd, AssemblySettings slconfig, PrimitivePara primitivepara, IServiceProvider service, string path, CollectionPara collectpara)
            : this(collectpara)
        {
            PrimitiveInstance = new Primitive(gd, slconfig, primitivepara);
            Init(count, gd, PrimitiveInstance, service, path);
        }


        /// <summary>
        /// Init Primitive Collection
        /// </summary>
        /// <param name="count"></param>
        /// <param name="gd"></param>
        /// <param name="primitive"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        public void Init(int count, GraphicsDevice gd, Primitive primitive, IServiceProvider service, string path)
        {
            // Check Shader Model 3.0 Support
            GraphicsDeviceCapabilities gdcap = gd.GraphicsDeviceCapabilities;
            if (gdcap.MaxPixelShaderProfile < ShaderProfile.PS_3_0 || gdcap.MaxVertexShaderProfile < ShaderProfile.VS_3_0)
            {
                isshadermodel3 = false;
                MessageBox.Show("This GraphicsDevice Does Not Support Shader Model 3.0.", "Warning !");
            }
            else
            {
                isshadermodel3 = true;
            }

            if (count <= 0)
            {
                MessageBox.Show("Collection Number <= 0, Automatically Set To One !");
                count = 1;
            }
            if (PrimitiveInstance == null)
            {
                PrimitiveInstance = primitive.Clone() as Primitive;
            }
            InstancePara = new vsBasePara[count];
            InstanceMatrix = new Matrix[count];
            for (int i = 0; i < count; i++)
            {
                InstancePara[i] = PrimitiveInstance.BasePara;
            }

            // Randomize Center and Speed of Primitive Instances
            var center = Random.RandomPosition(InstancePara.Length);
            var speed = Random.RandomPosition(InstancePara.Length);
            for (int i = 0; i < InstancePara.Length; i++)
            {
                center[i].X = center[i].X * (PrimitiveInstance.BasePara.space / 2) * (float)Math.Sin(Math.PI / 4);
                center[i].Y = center[i].Y * (PrimitiveInstance.BasePara.space / 2) * (float)Math.Sin(Math.PI / 4);
                center[i].Z = 0;
                speed[i].X = speed[i].X * 0.5f;
                speed[i].Y = speed[i].Y * 0.5f;
                speed[i].Z = 0;
                InstancePara[i].center = center[i] + Para.CollectionCenter;
                InstancePara[i].speed3D = speed[i] + Para.CollectionSpeed;
                InstanceMatrix[i] = VisionStimulus.GetOri3DMatrix(InstancePara[i].orientation3D) * Matrix.CreateTranslation(InstancePara[i].center);
            }

            // Load Hardware Instancing Shader
            PrimitiveInstance.contentManager = new ContentManager(service, path);
            try
            {
                hiEffect = PrimitiveInstance.contentManager.Load<Effect>("HardwareInstancing");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error !");
            }
            hiEffect.Parameters["View"].SetValue(PrimitiveInstance.ViewMatrix);
            hiEffect.Parameters["Projection"].SetValue(PrimitiveInstance.ProjectionMatrix);

            PrimitiveInstance.vertexDeclaration = InstancingVDec(gd);

            ptype = PrimitiveInstance.BasePara.primitivetype;
            vexn = PrimitiveInstance.VertexArray.Length;
            pcount = VisionStimulus.CheckPrimitiveCount(ptype, PrimitiveInstance.IndexArray.Length, PrimitiveInstance.IndexArray.Length);

            InstanceDataStream = new DynamicVertexBuffer(gd, SLConstant.SizeOfMatrix * InstanceMatrix.Length, BufferUsage.WriteOnly);
        }

        /// <summary>
        /// Get Instancing VertexDeclaration
        /// </summary>
        /// <param name="gd"></param>
        /// <returns></returns>
        public VertexDeclaration InstancingVDec(GraphicsDevice gd)
        {
            // When using hardware instancing, the instance transform matrix is
            // specified using a second vertex stream that provides 4x4 matrices
            // in texture coordinate channels 1 to 4. We must modify our vertex
            // declaration to include these channels.
            VertexElement[] extraElements = new VertexElement[4];
            short offset = 0;
            byte usageIndex = 1;
            short stream = 1;

            for (int i = 0; i < extraElements.Length; i++)
            {
                extraElements[i] = new VertexElement(stream, offset,
                                                VertexElementFormat.Vector4,
                                                VertexElementMethod.Default,
                                                VertexElementUsage.TextureCoordinate,
                                                usageIndex);

                offset += SLConstant.SizeOfVector4;
                usageIndex++;
            }

            // Append the new elements to the original format.
            int length = VertexPositionColor.VertexElements.Length + extraElements.Length;

            VertexElement[] elements = new VertexElement[length];
            VertexPositionColor.VertexElements.CopyTo(elements, 0);
            extraElements.CopyTo(elements, VertexPositionColor.VertexElements.Length);

            // Create a new vertex declaration.
            return new VertexDeclaration(gd, elements);
        }

        /// <summary>
        /// Update Collection
        /// </summary>
        /// <param name="time"></param>
        public void Update(double time)
        {
            float elapse = (float)(time - LastFrameTime);
            LastFrameTime = time;

            for (int i = 0; i < InstancePara.Length; i++)
            {
                Para.CollectionCenter += Vector3.Multiply(Para.CollectionSpeed, elapse);
                InstancePara[i].center += Vector3.Multiply(InstancePara[i].speed3D, elapse);
                if (Vector3.Distance(Para.CollectionCenter, Para.BasePara.center) > Para.BasePara.space / 2)
                {
                    Para.CollectionSpeed = Vector3.Reflect(Para.CollectionSpeed, Vector3.Normalize(Para.BasePara.center - Para.CollectionCenter));
                }
                if (Vector3.Distance(InstancePara[i].center, Para.CollectionCenter) > PrimitiveInstance.BasePara.space / 2)
                {
                    var center = Random.RandomPosition(1);
                    var speed = Random.RandomPosition(1);
                    center[0].X = center[0].X * (PrimitiveInstance.BasePara.space / 2) * (float)Math.Sin(Math.PI / 4);
                    center[0].Y = center[0].Y * (PrimitiveInstance.BasePara.space / 2) * (float)Math.Sin(Math.PI / 4);
                    center[0].Z = 0;
                    speed[0].X = speed[0].X * 0.5f;
                    speed[0].Y = speed[0].Y * 0.5f;
                    speed[0].Z = 0;
                    InstancePara[i].center = center[0] + Para.CollectionCenter;
                    InstancePara[i].speed3D = speed[0] + Para.CollectionSpeed;
                }
                InstanceMatrix[i] = VisionStimulus.GetOri3DMatrix(InstancePara[i].orientation3D) * Matrix.CreateTranslation(InstancePara[i].center);
            }
        }

        /// <summary>
        /// Draw Collection
        /// </summary>
        /// <param name="gd"></param>
        public void Draw(GraphicsDevice gd)
        {
            if (isshadermodel3)
            {
                gd.VertexDeclaration = PrimitiveInstance.vertexDeclaration;
                // Begin Draw
                hiEffect.Begin();
                hiEffect.CurrentTechnique.Passes[0].Begin();

                InstanceDataStream.SetData(InstanceMatrix, 0, InstanceMatrix.Length, SetDataOptions.Discard);
                gd.Vertices[0].SetSource(PrimitiveInstance.vertexBuffer, 0, VertexPositionColor.SizeInBytes);
                gd.Indices = PrimitiveInstance.indexBuffer;
                gd.Vertices[0].SetFrequencyOfIndexData(InstanceMatrix.Length);
                gd.Vertices[1].SetSource(InstanceDataStream, 0, SLConstant.SizeOfMatrix);
                gd.Vertices[1].SetFrequencyOfInstanceData(1);
                gd.RenderState.CullMode = CullMode.None;

                gd.DrawIndexedPrimitives(ptype, 0, 0, vexn, 0, pcount);

                hiEffect.CurrentTechnique.Passes[0].End();
                hiEffect.End();
            }
        }

    }

}
