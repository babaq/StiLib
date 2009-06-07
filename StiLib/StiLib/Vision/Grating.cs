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
        VertexPositionColor[] gratingvertex;
        int[] gvindex;
        VertexDeclaration gvdec;
        ContentManager cm;
        Effect effect;
        EffectParameter eWorld;
        EffectParameter eTime;

        int vex_n;
        float maxfactor;
        float minfactor;
        Vector4 mincolor;
        Vector4 maxcolor;
        Vector4 colorwidth;
        double radius;

        #endregion

        #region Properties

        /// <summary>
        /// Grating Effect
        /// </summary>
        public Effect Effect
        {
            get { return effect; }
            set { effect = value; }
        }

        /// <summary>
        /// Grating Content Manager
        /// </summary>
        public ContentManager Content
        {
            get { return cm; }
            set { cm = value; }
        }

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

        #endregion


        /// <summary>
        /// Set Grating Parameters to default before LoadContent() and Init()
        /// </summary>
        public Grating()
        {
            Para = GratingPara.Default;
        }

        /// <summary>
        /// Init Grating to default
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        public Grating(GraphicsDevice gd, IServiceProvider service, string path)
            : this()
        {
            LoadContent(service, path);
            Init(gd);
        }

        /// <summary>
        /// Init Grating to custom settings
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="shader"></param>
        /// <param name="technique"></param>
        public Grating(GraphicsDevice gd, IServiceProvider service, string path, string shader, string technique)
            : this()
        {
            LoadContent(service, path, shader, technique);
            Init(gd);
        }

        /// <summary>
        /// Init Grating to custom settings and default -- Shader:"Grating", Technique:"Technique1"
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="gratingpara"></param>
        public Grating(GraphicsDevice gd, IServiceProvider service, string path, GratingPara gratingpara)
        {
            LoadContent(service, path);
            Init(gd, gratingpara);
        }

        /// <summary>
        /// Init Grating to custom settings
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="gratingpara"></param>
        /// <param name="shader"></param>
        /// <param name="technique"></param>
        public Grating(GraphicsDevice gd, IServiceProvider service, string path, string shader, string technique, GratingPara gratingpara)
        {
            LoadContent(service, path, shader, technique);
            Init(gd, gratingpara);
        }


        /// <summary>
        /// Load default shader "Grating" and "Technique1"
        /// </summary>
        /// <param name="service"></param>
        /// <param name="path"></param>
        public override void LoadContent(IServiceProvider service, string path)
        {
            LoadContent(service, path, "Grating", "Technique1");
        }

        /// <summary>
        /// Load Grating Shader and Technique
        /// </summary>
        /// <param name="service"></param>
        /// <param name="path"></param>
        /// <param name="shader"></param>
        /// <param name="technique"></param>
        public void LoadContent(IServiceProvider service, string path, string shader, string technique)
        {
            cm = new ContentManager(service, path);
            try
            {
                effect = cm.Load<Effect>(shader);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error !");
            }
            effect.CurrentTechnique = effect.Techniques[technique];
        }

        /// <summary>
        /// Init according to internal grating parameters
        /// </summary>
        /// <param name="gd"></param>
        public override void Init(GraphicsDevice gd)
        {
            InitVS(gd);
            ColorFactor();
            SetMaxMinColor();

            // Init Vertex according to grating type and shape
            gvdec = new VertexDeclaration(gd, VertexPositionColor.VertexElements);
            SetVertex();
            SetGratingType(Para.gratingtype);

            // Set Mask
            SetMask(Para.maskpara.MaskType);

            // Set Effect Parameters
            eWorld = effect.Parameters["World"];
            eTime = effect.Parameters["time"];
            eWorld.SetValue(Matrix.CreateRotationZ(Para.direction * (float)SLConstant.RadpDeg) * Matrix.CreateTranslation(Para.BasePara.center));
            eTime.SetValue(0.0f);
            effect.Parameters["View"].SetValue(GlobalView());
            effect.Parameters["Projection"].SetValue(GlobalProj());
            effect.Parameters["tf"].SetValue(Para.tf);
            effect.Parameters["sf"].SetValue(Para.sf);
            effect.Parameters["sphase"].SetValue(Para.sphase);
            effect.Parameters["sigma"].SetValue(Para.maskpara.BasePara.diameter);

        }

        /// <summary>
        /// Init Grating according to gratingpara
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="gratingpara"></param>
        public void Init(GraphicsDevice gd, GratingPara gratingpara)
        {
            Para = gratingpara;
            Init(gd);
        }

        /// <summary>
        /// Initialize Grating Vertex
        /// </summary>
        public void SetVertex()
        {
            radius = Para.BasePara.diameter / 2.0;
            // Set Max Effective Resolution
            if (Para.resolution > ufactor / Para.sf)
            {
                Para.resolution = (int)Math.Floor(ufactor / Para.sf);
            }
            // Set Min Effective Resolution
            if (Para.resolution < ufactor / Para.sf / 2)
            {
                Para.resolution = (int)Math.Floor(ufactor / Para.sf / 2);
            }
            // Interpolation Interval in Unit
            double interval = (1 / Para.sf) / Para.resolution;

            vex_n = 2 * (2 * (int)Math.Round(radius / interval) + 1);
            gratingvertex = new VertexPositionColor[vex_n];
            gvindex = new int[vex_n];

            int updown;
            double Ycoor = 0;
            double Xstep;
            for (int i = 0; i < vex_n; i++)
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
                gratingvertex[i].Position = new Vector3((float)(-radius + Xstep), (float)Ycoor, 0);
                gvindex[i] = i;
            }
        }

        /// <summary>
        /// Draw Grating
        /// </summary>
        /// <param name="gd"></param>
        public override void Draw(GraphicsDevice gd)
        {
            if (Para.BasePara.visible)
            {
                gd.VertexDeclaration = gvdec;
                gd.RenderState.CullMode = CullMode.None;

                effect.Begin();
                effect.CurrentTechnique.Passes[0].Begin();
                gd.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, gratingvertex, 0, vex_n, gvindex, 0, vex_n - 2);
                effect.CurrentTechnique.Passes[0].End();
                effect.End();
            }
        }


        /// <summary>
        /// Set Effect's World matrix
        /// </summary>
        /// <param name="world"></param>
        public override void SetWorld(Matrix world)
        {
            eWorld.SetValue(world);
        }

        /// <summary>
        /// Set Effect's View matrix
        /// </summary>
        /// <param name="view"></param>
        public override void SetView(Matrix view)
        {
            effect.Parameters["View"].SetValue(view);
        }

        /// <summary>
        /// Set Effect's Projection matrix
        /// </summary>
        /// <param name="proj"></param>
        public override void SetProjection(Matrix proj)
        {
            effect.Parameters["Projection"].SetValue(proj);
        }

        /// <summary>
        /// Set visible state
        /// </summary>
        /// <param name="isvisible"></param>
        public override void SetVisible(bool isvisible)
        {
            Para.BasePara.visible = isvisible;
        }

        /// <summary>
        /// Set effect's Time Parameter
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
            SetVertex();
            effect.Parameters["sf"].SetValue(sf);
        }

        /// <summary>
        /// Set Grating Diameter
        /// </summary>
        /// <param name="diameter"></param>
        public void SetDiameter(float diameter)
        {
            Para.BasePara.diameter = diameter;
            radius = diameter / 2;
            SetVertex();
        }

        /// <summary>
        /// Set Temporal Frequency
        /// </summary>
        /// <param name="tf"></param>
        public void SetTF(float tf)
        {
            Para.tf = tf;
            effect.Parameters["tf"].SetValue(tf);
        }

        /// <summary>
        /// Set Spatial Phase
        /// </summary>
        /// <param name="sphase"></param>
        public void SetSPhase(float sphase)
        {
            Para.sphase = sphase;
            effect.Parameters["sphase"].SetValue(sphase);
        }

        /// <summary>
        /// Set Grating Interpolation Resolution
        /// </summary>
        /// <param name="res"></param>
        public void SetResolution(int res)
        {
            Para.resolution = res;
            SetVertex();
        }

        /// <summary>
        /// Set Grating Type
        /// </summary>
        /// <param name="gtype"></param>
        public void SetGratingType(GratingType gtype)
        {
            Para.gratingtype = gtype;
            int temp = 0;
            switch (gtype)
            {
                case GratingType.Square:
                    temp = 1; break;
                case GratingType.Linear:
                    temp = 2; break;
                default: // Sinusoidal
                    temp = 0; break;
            }
            effect.Parameters["VSIndex"].SetValue(temp);
        }

        /// <summary>
        /// Set Grating Shape
        /// </summary>
        /// <param name="shape"></param>
        public void SetShape(Shape shape)
        {
            Para.shape = shape;
            SetVertex();
        }

        /// <summary>
        /// Get Luminance and Contrast Constrains 
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
        /// Get Color Interpolation Profile according to Luminance and Contrast
        /// </summary>
        public void SetMaxMinColor()
        {
            SetMaxMinColor(Para.lhcolor.ToVector4() * maxfactor, Para.rlcolor.ToVector4() * minfactor);
        }

        /// <summary>
        /// Get Color Interpolation Profile according to custom colors
        /// </summary>
        /// <param name="max"></param>
        /// <param name="min"></param>
        public void SetMaxMinColor(Vector4 max, Vector4 min)
        {
            maxcolor = max;
            mincolor = min;
            colorwidth = new Vector4(Math.Abs(max.X - min.X), Math.Abs(max.Y - min.Y), Math.Abs(max.Z - min.Z), Math.Max(max.W, min.W));
            effect.Parameters["maxcolor"].SetValue(max);
            effect.Parameters["mincolor"].SetValue(min);
            effect.Parameters["colorwidth"].SetValue(colorwidth);
        }

        /// <summary>
        /// Set Grating Transparency
        /// </summary>
        /// <param name="a">0-1</param>
        public void SetTransparency(float a)
        {
            maxcolor.W = a;
            mincolor.W = a;
            SetMaxMinColor(maxcolor, mincolor);
        }

        /// <summary>
        /// Set Luminance and Contrast
        /// </summary>
        /// <param name="lum"></param>
        /// <param name="con"></param>
        public void SetLum_Con(float lum, float con)
        {
            Para.luminance = lum;
            Para.contrast = con;
            ColorFactor();
            SetMaxMinColor();
        }

        /// <summary>
        /// Set Mask
        /// </summary>
        /// <param name="mtype"></param>
        public void SetMask(MaskType mtype)
        {
            Para.maskpara.MaskType = mtype;
            int temp = 0;
            switch (mtype)
            {
                case MaskType.Gaussian:
                    temp = 1; break;
                default: // None
                    temp = 0; break;
            }
            effect.Parameters["PSIndex"].SetValue(temp);
        }

        /// <summary>
        /// Set Gaussian Mask Sigma Parameter
        /// </summary>
        /// <param name="sigma"></param>
        public void SetSigma(float sigma)
        {
            Para.maskpara.BasePara.diameter = sigma;
            effect.Parameters["sigma"].SetValue(sigma);
        }

    }
}
