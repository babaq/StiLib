#region File Description
//-----------------------------------------------------------------------------
// Grating.cs
//
// StiLib Grating Stimulus
// Copyright (c) Zhang Li. 2008-9-3.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using StiLib.Core;
#endregion

namespace StiLib.Vision
{
    /// <summary>
    /// StiLib Grating Stimulus
    /// </summary>
    public class Grating : VisionStimulus
    {
        #region Fields

        /// <summary>
        /// Grating Parameters
        /// </summary>
        public GratingPara Para;
        /// <summary>
        /// Grating Effect
        /// </summary>
        public Effect gratingeffect;
        /// <summary>
        /// Grating Effect World Matrix Parameter
        /// </summary>
        public EffectParameter eWorld;
        /// <summary>
        /// Grating Effect time Parameter
        /// </summary>
        public EffectParameter eTime;

        int vertexN;
        float maxfactor;
        float minfactor;
        Vector4 mincolor;
        Vector4 maxcolor;
        Vector4 colorwidth;
        double radius;

        #endregion

        #region Properties

        /// <summary>
        /// Get Grating Peak Color
        /// </summary>
        public Vector4 MaxColor
        {
            get { return maxcolor; }
        }

        /// <summary>
        /// Get Grating Trough Color
        /// </summary>
        public Vector4 MinColor
        {
            get { return mincolor; }
        }

        /// <summary>
        /// Grating Basic Parameters
        /// </summary>
        public override vsBasePara BasePara
        {
            get { return Para.BasePara; }
            set { Para.BasePara = value; }
        }

        /// <summary>
        /// Grating Center
        /// </summary>
        public override Vector3 Center
        {
            get { return Para.BasePara.center; }
            set { Para.BasePara.center = value; }
        }

        /// <summary>
        /// Grating Speed3D
        /// </summary>
        public override Vector3 Speed3D
        {
            get { return Para.BasePara.speed3D; }
            set { Para.BasePara.speed3D = value; }
        }

        /// <summary>
        /// Grating Visible State
        /// </summary>
        public override bool Visible
        {
            get { return Para.BasePara.visible; }
            set { Para.BasePara.visible = value; }
        }

        /// <summary>
        /// Gets Global Camera View. Sets GratingEffect View.
        /// </summary>
        public override Matrix ViewMatrix
        {
            get { return globalCamera.ViewMatrix; }
            set { gratingeffect.Parameters["View"].SetValue(value); }
        }

        /// <summary>
        /// Gets Global Camera Projection. Sets GratingEffect Projection.
        /// </summary>
        public override Matrix ProjectionMatrix
        {
            get { return globalCamera.GetUnitProjection(globalCamera.projectionType, unitFactor); }
            set { gratingeffect.Parameters["Projection"].SetValue(value); }
        }

        #endregion


        /// <summary>
        /// Sets Default GratingPara, need LoadContent() and Init()
        /// </summary>
        public Grating()
        {
            Para = GratingPara.Default;
        }

        /// <summary>
        /// Init Grating with Default GratingPara and "Technique1"
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        public Grating(GraphicsDevice gd, IServiceProvider service, string path)
            : base(gd)
        {
            Para = GratingPara.Default;
            LoadContent(service, path);
            Init(gd);
        }

        /// <summary>
        /// Init Grating with Custom Shader Content and Default GratingPara
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="shader"></param>
        /// <param name="technique"></param>
        public Grating(GraphicsDevice gd, IServiceProvider service, string path, string shader, string technique)
            : base(gd)
        {
            Para = GratingPara.Default;
            LoadContent(service, path, shader, technique);
            Init(gd);
        }

        /// <summary>
        /// Init Grating with Custom GratingPara and Default: "Technique1"
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="gratingpara"></param>
        public Grating(GraphicsDevice gd, IServiceProvider service, string path, GratingPara gratingpara)
            : base(gd)
        {
            Para = gratingpara;
            LoadContent(service, path, gratingpara.BasePara.contentname);
            Init(gd);
        }

        /// <summary>
        /// Init Grating with Custom GratingPara, Configuration and Default: "Technique1"
        /// </summary>
        /// <param name="distance2display"></param>
        /// <param name="displayratio"></param>
        /// <param name="displaysize"></param>
        /// <param name="camera"></param>
        /// <param name="unit"></param>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="gratingpara"></param>
        public Grating(float distance2display, float displayratio, float displaysize, SLCamera camera, Unit unit, GraphicsDevice gd, IServiceProvider service, string path, GratingPara gratingpara)
            : base(distance2display, displayratio, displaysize, gd, camera, unit)
        {
            Para = gratingpara;
            LoadContent(service, path, gratingpara.BasePara.contentname);
            Init(gd);
        }

        /// <summary>
        /// Init Grating with Custom GratingPara, StiLib Configuration File and Default: "Technique1"
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="slconfig"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="gratingpara"></param>
        public Grating(GraphicsDevice gd, AssemblySettings slconfig, IServiceProvider service, string path, GratingPara gratingpara)
            : base(gd, slconfig)
        {
            Para = gratingpara;
            LoadContent(service, path, gratingpara.BasePara.contentname);
            Init(gd);
        }

        /// <summary>
        /// Init Grating with Custom GratingPara
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="shader"></param>
        /// <param name="technique"></param>
        /// <param name="gratingpara"></param>
        public Grating(GraphicsDevice gd, IServiceProvider service, string path, string shader, string technique, GratingPara gratingpara)
            : base(gd)
        {
            Para = gratingpara;
            LoadContent(service, path, shader, technique);
            Init(gd);
        }


        /// <summary>
        /// Load Internal GratingPara Shader Content and Default: "Technique1"
        /// </summary>
        /// <param name="service"></param>
        /// <param name="path"></param>
        public void LoadContent(IServiceProvider service, string path)
        {
            LoadContent(service, path, Para.BasePara.contentname, "Technique1");
        }

        /// <summary>
        /// Load Custom Grating Shader and Default: "Technique1"
        /// </summary>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="shader"></param>
        public override void LoadContent(IServiceProvider service, string path, string shader)
        {
            LoadContent(service, path, shader, "Technique1");
        }

        /// <summary>
        /// Load Custom Grating Shader and Technique
        /// </summary>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="shader"></param>
        /// <param name="technique"></param>
        public void LoadContent(IServiceProvider service, string path, string shader, string technique)
        {
            contentManager = new ContentManager(service, path);
            try
            {
                gratingeffect = contentManager.Load<Effect>(shader);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error !");
            }
            gratingeffect.CurrentTechnique = gratingeffect.Techniques[technique];

            // Save Content Name
            Para.BasePara.contentname = shader;
        }

        /// <summary>
        /// Init According to Internal GratingPara
        /// </summary>
        /// <param name="gd"></param>
        public override void Init(GraphicsDevice gd)
        {
            InitVS(gd);
            ColorFactor();
            SetMaxMinColor();

            // Init Vertex and Index According to Grating Type and Shape
            vertexDeclaration = new VertexDeclaration(gd, VertexPositionColor.VertexElements);
            FillGrating(true);
            SetGratingType(Para.gratingtype);

            // Set Mask
            SetMask(Para.maskpara.masktype);

            // Set Effect Parameters
            eWorld = gratingeffect.Parameters["World"];
            eTime = gratingeffect.Parameters["time"];
            ori3DMatrix = Matrix.CreateRotationZ(Para.BasePara.direction * (float)SLConstant.Rad_p_Deg);
            worldMatrix = Matrix.CreateTranslation(Para.BasePara.center);
            eTime.SetValue(0.0f);
            gratingeffect.Parameters["View"].SetValue(ViewMatrix);
            gratingeffect.Parameters["Projection"].SetValue(ProjectionMatrix);
            gratingeffect.Parameters["tf"].SetValue(Para.tf);
            gratingeffect.Parameters["sf"].SetValue(Para.sf);
            gratingeffect.Parameters["sphase"].SetValue(Para.sphase);
            gratingeffect.Parameters["sigma"].SetValue(Para.maskpara.BasePara.diameter);
        }

        /// <summary>
        /// Init Grating according to gratingpara
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="gratingpara"></param>
        public void Init(GraphicsDevice gd, GratingPara gratingpara)
        {
            // New content shader need LoadContent() first, so here just renew grating parameters except content
            gratingpara.BasePara.contentname = Para.BasePara.contentname;
            Para = gratingpara;
            Init(gd);
        }

        /// <summary>
        /// Draw Grating
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
                eWorld.SetValue(ori3DMatrix * worldMatrix);

                gratingeffect.Begin();
                gratingeffect.CurrentTechnique.Passes[0].Begin();
                gd.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, vertexN, 0, vertexN - 2);
                gratingeffect.CurrentTechnique.Passes[0].End();
                gratingeffect.End();
            }
        }

        /// <summary>
        /// Fill Grating Vertex and Index
        /// </summary>
        /// <param name="ischeckresolution">if optimize resolution of interpolation</param>
        public void FillGrating(bool ischeckresolution)
        {
            radius = Para.BasePara.diameter / 2.0;
            if (ischeckresolution)
            {
                // Set Max Effective Resolution(Every Pixel in a Circle)
                if (Para.resolution > unitFactor / Para.sf)
                {
                    Para.resolution = (int)Math.Floor(unitFactor / Para.sf);
                }
                // Set Min Effective Resolution(Every Two Pixel in a Circle)
                if (Para.resolution < unitFactor / Para.sf / 2)
                {
                    Para.resolution = (int)Math.Floor(unitFactor / Para.sf / 2);
                }
            }
            // Interpolation Interval in Unit
            double interval = (1 / Para.sf) / Para.resolution;

            vertexN = 2 * (2 * (int)Math.Round(radius / interval) + 1);
            vertexArray = new VertexPositionColor[vertexN];
            indexArray = new int[vertexN];

            int updown;
            double Ycoor = 0;
            double Xstep;
            for (int i = 0; i < vertexN; i++)
            {
                updown = i % 2;
                Xstep = Math.Floor(i / 2.0) * interval;
                //if (Xstep - radius > radius) Xstep = radius * 2;
                if (Xstep > Para.BasePara.diameter) Xstep = Para.BasePara.diameter;

                if (updown == 0) // Up
                {
                    switch (Para.shape)
                    {
                        case Shape.Circle:
                            //Ycoor = Math.Sqrt(Math.Abs(Math.Pow(radius, 2) - Math.Pow((radius - Xstep), 2))); break;
                            Ycoor = Math.Sqrt(Math.Pow(radius, 2) - Math.Pow((radius - Xstep), 2)); break;
                        default: // Quadrate
                            Ycoor = radius; break;
                    }
                }
                else // Down
                {
                    switch (Para.shape)
                    {
                        case Shape.Circle:
                            //Ycoor = -Math.Sqrt(Math.Abs(Math.Pow(radius, 2) - Math.Pow((radius - Xstep), 2))); break;
                            Ycoor = -Math.Sqrt(Math.Pow(radius, 2) - Math.Pow((radius - Xstep), 2)); break;
                        default: // Quadrate
                            Ycoor = -radius; break;
                    }
                }
                vertexArray[i].Position = new Vector3((float)(-radius + Xstep), (float)Ycoor, 0);
                indexArray[i] = i;
            }

            SetVertexBuffer();
            SetIndexBuffer();
        }


        /// <summary>
        /// Sets GratingEffect's Time Parameter
        /// </summary>
        /// <param name="time"></param>
        public void SetTime(float time)
        {
            eTime.SetValue(time);
        }

        /// <summary>
        /// Set Spatial Frequency
        /// </summary>
        /// <param name="sf"></param>
        public void SetSF(float sf)
        {
            Para.sf = sf;
            FillGrating(true);
            gratingeffect.Parameters["sf"].SetValue(sf);
        }

        /// <summary>
        /// Set Grating Diameter
        /// </summary>
        /// <param name="diameter"></param>
        public void SetDiameter(float diameter)
        {
            Para.BasePara.diameter = diameter;
            FillGrating(true);
        }

        /// <summary>
        /// Set Temporal Frequency
        /// </summary>
        /// <param name="tf"></param>
        public void SetTF(float tf)
        {
            Para.tf = tf;
            gratingeffect.Parameters["tf"].SetValue(tf);
        }

        /// <summary>
        /// Set Spatial Phase
        /// </summary>
        /// <param name="sphase"></param>
        public void SetSPhase(float sphase)
        {
            Para.sphase = sphase;
            gratingeffect.Parameters["sphase"].SetValue(sphase);
        }

        /// <summary>
        /// Set Grating Interpolation Resolution
        /// </summary>
        /// <param name="resolution"></param>
        /// <param name="ischeckresolution">if optimize resolution of interpolation</param>
        public void SetResolution(int resolution, bool ischeckresolution)
        {
            Para.resolution = resolution;
            FillGrating(ischeckresolution);
        }

        /// <summary>
        /// Sets Grating Type
        /// </summary>
        /// <param name="gratingtype"></param>
        public void SetGratingType(GratingType gratingtype)
        {
            Para.gratingtype = gratingtype;
            int temp = 0;
            switch (gratingtype)
            {
                case GratingType.Square:
                    temp = 1; break;
                case GratingType.Linear:
                    temp = 2; break;
                default: // Sinusoidal
                    temp = 0; break;
            }
            gratingeffect.Parameters["VSIndex"].SetValue(temp);
        }

        /// <summary>
        /// Set Grating Shape
        /// </summary>
        /// <param name="shape"></param>
        public void SetShape(Shape shape)
        {
            Para.shape = shape;
            FillGrating(true);
        }

        /// <summary>
        /// Gets Luminance and Contrast Constrains 
        /// </summary>
        public void ColorFactor()
        {
            if (Para.luminance > 0.5)
            {
                maxfactor = Para.luminance + Para.contrast * (1 - Para.luminance);
                minfactor = Para.luminance - Para.contrast * (1 - Para.luminance);
            }
            else
            {
                maxfactor = (1 + Para.contrast) * Para.luminance;
                minfactor = (1 - Para.contrast) * Para.luminance;
            }
        }

        /// <summary>
        /// Gets Color Interpolation Profile According to Luminance and Contrast
        /// </summary>
        public void SetMaxMinColor()
        {
            SetMaxMinColor(Para.lhcolor.ToVector4() * maxfactor, Para.rlcolor.ToVector4() * minfactor);
        }

        /// <summary>
        /// Gets Color Interpolation Profile According to Custom Colors
        /// </summary>
        /// <param name="maxc"></param>
        /// <param name="minc"></param>
        public void SetMaxMinColor(Vector4 maxc, Vector4 minc)
        {
            this.maxcolor = maxc;
            this.mincolor = minc;
            this.colorwidth = new Vector4(Math.Abs(maxc.X - minc.X), Math.Abs(maxc.Y - minc.Y), Math.Abs(maxc.Z - minc.Z), Math.Max(maxc.W, minc.W));
            gratingeffect.Parameters["maxcolor"].SetValue(maxc);
            gratingeffect.Parameters["mincolor"].SetValue(minc);
            gratingeffect.Parameters["colorwidth"].SetValue(colorwidth);
        }

        /// <summary>
        /// Set Grating Transparency
        /// </summary>
        /// <param name="alpha">[0, 1]</param>
        public void SetTransparency(float alpha)
        {
            maxcolor.W = alpha;
            mincolor.W = alpha;
            SetMaxMinColor(maxcolor, mincolor);
        }

        /// <summary>
        /// Set Luminance and Contrast
        /// </summary>
        /// <param name="luminance"></param>
        /// <param name="contrast"></param>
        public void SetLumCon(float luminance, float contrast)
        {
            Para.luminance = luminance;
            Para.contrast = contrast;
            ColorFactor();
            SetMaxMinColor();
        }

        /// <summary>
        /// Sets Mask
        /// </summary>
        /// <param name="masktype"></param>
        public void SetMask(MaskType masktype)
        {
            Para.maskpara.masktype = masktype;
            int temp = 0;
            switch (masktype)
            {
                case MaskType.Gaussian:
                    temp = 1; break;
                default: // None
                    temp = 0; break;
            }
            gratingeffect.Parameters["PSIndex"].SetValue(temp);
        }

        /// <summary>
        /// Set Gaussian Mask Sigma Parameter
        /// </summary>
        /// <param name="sigma"></param>
        public void SetGaussianSigma(float sigma)
        {
            Para.maskpara.BasePara.diameter = sigma;
            gratingeffect.Parameters["sigma"].SetValue(sigma);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            if (gdRef != null)
            {
                return new Grating(distance2Display, displayRatio, displaySize, globalCamera, unit, gdRef, contentManager.ServiceProvider, contentManager.RootDirectory, Para);
            }
            else
            {
                SLConstant.ShowMessage("No Internal GraphicsDevice Reference, Please InitVS(GraphicsDevice gd) First !");
                return "No gdRef";
            }
        }

    }
}
