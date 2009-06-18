#region File Description
//-----------------------------------------------------------------------------
// VisionStimulus.cs
//
// StiLib Vision Stimulus Base Class
// Copyright (c) Zhang Li. 2008-8-6.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StiLib.Core;
#endregion

namespace StiLib.Vision
{
    /// <summary>
    /// Vision stimulus base class
    /// </summary>
    abstract public class VisionStimulus
    {
        #region Visual Configuration

        /// <summary>
        /// Perpendicular Distance from eye to display (mm), Usually Map 1deg Visual Angle to 10mm on Display 
        /// </summary>
        public float distance2display;

        /// <summary>
        /// Display AspectRatio (Width : Height), 1.333(4:3) or 1.778(16:9)
        /// </summary>
        public float displayratio;

        /// <summary>
        /// Display Size (Viewable Diagonal Length in Inch (1 Inch = 25.4mm), 17inch: 15, 22inch: 20 )
        /// </summary>
        public float displaysize;

        #endregion

        #region Display Parameter

        /// <summary>
        /// Current Display viewable height in mm
        /// </summary>
        public float view_h_mm;

        /// <summary>
        /// Current Display viewable width in mm
        /// </summary>
        public float view_w_mm;

        /// <summary>
        /// Current Display viewable height in pixel
        /// </summary>
        public int view_h_pixel;

        /// <summary>
        /// Current Display viewable width in pixel
        /// </summary>
        public int view_w_pixel;

        /// <summary>
        /// Current Display viewable height in visual angle degree
        /// </summary>
        public float view_h_deg;

        /// <summary>
        /// Current Display viewable width in visual angle degree
        /// </summary>
        public float view_w_deg;

        #endregion

        #region Unit Conversion Factor

        /// <summary>
        /// Current Display Size (MM) per Visual Angle (Deg)
        /// </summary>
        public float mm_p_deg;

        /// <summary>
        /// Current Full Screen Resolution (Pixel) per Visual Angle (Deg)
        /// </summary>
        public float pixel_p_deg;

        /// <summary>
        /// Current Full Screen Resolution (Pixel) per Display Size (MM)
        /// </summary>
        public float pixel_p_mm;

        #endregion

        #region Presentation Parameter

        /// <summary>
        /// Global Camera
        /// </summary>
        public SLCamera gcamera;
        /// <summary>
        /// Global Unit
        /// </summary>
        public Unit unit;
        /// <summary>
        /// Current Global Unit Conversion Factor
        /// </summary>
        public float ufactor;

        #endregion


        /// <summary>
        /// Init to default settings -- distance2display:570mm, displayratio:1.333, displaysize:20inch, 
        /// fullscreen_w_p:800, fullscreen_h_p:600, camera:default, unit:Deg
        /// </summary>
        public VisionStimulus() : this(570, 1.333f, 20, 800, 600, new SLCamera(), Unit.Deg)
        {
        }

        /// <summary>
        /// Init to default settings -- distance2display:570mm, displayratio:1.333, displaysize:20inch, unit:Deg
        /// and current GraphicsDevice fullscreen resolution and viewport
        /// </summary>
        /// <param name="gd"></param>
        public VisionStimulus(GraphicsDevice gd) : this(570, 1.333f, 20, gd.DisplayMode.Width, gd.DisplayMode.Height, new SLCamera(gd), Unit.Deg)
        {
        }

        /// <summary>
        /// Init to custom settings according to current GraphicsDevice
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="ratio"></param>
        /// <param name="size"></param>
        /// <param name="gd"></param>
        /// <param name="u"></param>
        public VisionStimulus(float distance, float ratio, float size, GraphicsDevice gd, Unit u) : this(distance, ratio, size, gd.DisplayMode.Width, gd.DisplayMode.Height, new SLCamera(gd), u)
        {
        }

        /// <summary>
        /// Init to custom settings
        /// </summary>
        /// <param name="distance">distance2display</param>
        /// <param name="ratio">displayratio</param>
        /// <param name="size">displaysize</param>
        /// <param name="fullscreen_w_p">full screen resolution pixel in width</param>
        /// <param name="fullscreen_h_p">full screen resolution pixel in height</param>
        /// <param name="cam">Global Camera</param>
        /// <param name="u">Global Unit</param>
        public VisionStimulus(float distance, float ratio, float size, int fullscreen_w_p, int fullscreen_h_p, SLCamera cam, Unit u)
        {
            InitVS(distance, ratio, size, fullscreen_w_p, fullscreen_h_p, cam, u);
        }


        /// <summary>
        /// Set Configuration Parameters
        /// </summary>
        /// <param name="distance">distance2display</param>
        /// <param name="ratio">displayratio</param>
        /// <param name="size">displaysize</param>
        /// <param name="fullscreen_w_p">full screen resolution pixel in width</param>
        /// <param name="fullscreen_h_p">full screen resolution pixel in height</param>
        /// <param name="cam">Global Camera</param>
        /// <param name="u">Global Unit</param>
        public void SetConfigPara(float distance, float ratio, float size, int fullscreen_w_p, int fullscreen_h_p, SLCamera cam, Unit u)
        {
            distance2display = distance;
            displayratio = ratio;
            displaysize = size;
            view_w_pixel = fullscreen_w_p;
            view_h_pixel = fullscreen_h_p;
            gcamera = cam.Clone() as SLCamera;
            unit = u;
        }

        /// <summary>
        /// Setup Visual Environment according to Config Parameters after SetConfigPara() Method
        /// </summary>
        public void Config()
        {
            // Map Display(mm) to Visual Angle(deg)
            mm_p_deg = 2 * 0.008737f * distance2display; // tan(0.5)=0.008737

            // Get Display viewable size in mm
            view_h_mm = (float)Math.Sqrt(Math.Pow(displaysize * SLConstant.MMpInch, 2) / (1 + Math.Pow(displayratio, 2)));
            view_w_mm = view_h_mm * displayratio;

            // Get Display viewable size in deg
            view_h_deg = view_h_mm / mm_p_deg;
            view_w_deg = view_h_deg * displayratio;

            // Map Display(pixel) to Display(mm)
            pixel_p_mm = view_w_pixel / view_w_mm;

            // Map Display(pixel) to Visual Angle(deg)
            pixel_p_deg = view_w_pixel / view_w_deg;

            // Set Current Global Unit Conversion Factor according to Current Global Unit
            switch (unit)
            {
                case Unit.Deg:
                    ufactor = pixel_p_deg; break;
                case Unit.MM:
                    ufactor = pixel_p_mm; break;
                case Unit.Pixel:
                    ufactor = 1.0f; break;
            }
        }

        /// <summary>
        /// Initialize all configuration
        /// </summary>
        /// <param name="distance">distance2display</param>
        /// <param name="ratio">displayratio</param>
        /// <param name="size">displaysize</param>
        /// <param name="fullscreen_w_p">full screen resolution pixel in width</param>
        /// <param name="fullscreen_h_p">full screen resolution pixel in height</param>
        /// <param name="cam">Global Camera</param>
        /// <param name="u">Global Unit</param>
        public void InitVS(float distance, float ratio, float size, int fullscreen_w_p, int fullscreen_h_p, SLCamera cam, Unit u)
        {
            if ((view_w_pixel!=fullscreen_w_p)||(view_h_pixel!=fullscreen_h_p)||(distance2display!=distance)||(displayratio!=ratio)||(displaysize!=size)||(!gcamera.Equals(cam))||(unit!=u))
            {
                SetConfigPara(distance, ratio, size, fullscreen_w_p, fullscreen_h_p, cam, u);
                Config();
            }
        }

        /// <summary>
        /// Init according to current GraphicsDevice fullscreen resolution and viewport
        /// </summary>
        /// <param name="gd"></param>
        public void InitVS(GraphicsDevice gd)
        {
            InitVS(distance2display, displayratio, displaysize, gd.DisplayMode.Width, gd.DisplayMode.Height, new SLCamera(gd), unit);
        }

        /// <summary>
        /// Get Global Camera View Matrix
        /// </summary>
        /// <returns></returns>
        public Matrix GlobalView()
        {
            return gcamera.ViewMatrix;
        }

        /// <summary>
        /// Get Global Camera Unified Projection Matrix
        /// </summary>
        /// <returns></returns>
        public Matrix GlobalProj()
        {
            return gcamera.GetUnitProj(gcamera.projtype, ufactor);
        }

        /// <summary>
        /// Global Projection Type
        /// </summary>
        public ProjectionType GlobalProjType
        {
            get { return gcamera.projtype; }
            set { gcamera.projtype = value; }
        }


        /// <summary>
        /// Load Stimulus Content
        /// </summary>
        /// <param name="service"></param>
        /// <param name="path"></param>
        public virtual void LoadContent(IServiceProvider service, string path)
        {
        }

        /// <summary>
        /// Init Stimulus according to internal parameters
        /// </summary>
        /// <param name="gd"></param>
        public virtual void Init(GraphicsDevice gd)
        {
        }

        /// <summary>
        /// Draw Stimulus
        /// </summary>
        /// <param name="gd"></param>
        public virtual void Draw(GraphicsDevice gd)
        {
        }

        /// <summary>
        /// Set World Transform
        /// </summary>
        /// <param name="world"></param>
        public virtual void SetWorld(Matrix world)
        {
        }

        /// <summary>
        /// Set View Transform
        /// </summary>
        /// <param name="view"></param>
        public virtual void SetView(Matrix view)
        {
        }

        /// <summary>
        /// Set Projection Transform
        /// </summary>
        /// <param name="proj"></param>
        public virtual void SetProjection(Matrix proj)
        {
        }

        /// <summary>
        /// Set Visible State
        /// </summary>
        /// <param name="isvisible"></param>
        public virtual void SetVisible(bool isvisible)
        {
        }

    }

    #region Parameter Type

    /// <summary>
    /// Visual Stimulus Basic Parameters
    /// </summary>
    public struct BasePara
    {
        /// <summary>
        /// Init to custom parameters
        /// </summary>
        /// <param name="center"></param>
        /// <param name="diameter"></param>
        /// <param name="orientation"></param>
        /// <param name="orientation3d"></param>
        /// <param name="speed"></param>
        /// <param name="movearea"></param>
        /// <param name="color"></param>
        /// <param name="visible"></param>
        public BasePara(Vector3 center, float diameter, float orientation, Vector3 orientation3d, Vector3 speed, float movearea, Color color, bool visible)
        {
            this.center = center;
            this.diameter = diameter;
            this.orientation = orientation;
            this.orientation3d = orientation3d;
            this.speed = speed;
            this.movearea = movearea;
            this.color = color;
            this.visible = visible;
        }

        /// <summary>
        /// Get Default BasePara -- center:(0,0,0), diameter:10Unit, orientation:0.0deg, orientation3d:(0,0,0), speed:(0,0,0), movearea:10Unit, color:White, visible:true
        /// </summary>
        public static BasePara Default
        {
            get 
            {
                return new BasePara(Vector3.Zero, 10.0f, 0.0f, Vector3.Zero, Vector3.Zero, 10.0f, Color.White, true);
            }
        }


        /// <summary>
        /// center in World Space(Unit)
        /// </summary>
        public Vector3 center;
        /// <summary>
        /// Size(Unit)
        /// </summary>
        public float diameter;
        /// <summary>
        /// Orientation: [0,180)(deg), Counterclockwise
        /// </summary>
        public float orientation;
        /// <summary>
        /// 3D Space Orientation Vector
        /// </summary>
        public Vector3 orientation3d;
        /// <summary>
        /// Moving Speed(Unit/s)
        /// </summary>
        public Vector3 speed;
        /// <summary>
        /// Moving Area(Unit)
        /// </summary>
        public float movearea;
        /// <summary>
        /// Color
        /// </summary>
        public Color color;
        /// <summary>
        /// Visible State
        /// </summary>
        public bool visible;
    }

    /// <summary>
    /// Primitive Type Parameters
    /// </summary>
    public struct PrimitivePara
    {
        /// <summary>
        /// Init All Parameters to custom
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="indices"></param>
        /// <param name="center"></param>
        /// <param name="diameter"></param>
        /// <param name="orientation"></param>
        /// <param name="orientation3d"></param>
        /// <param name="speed"></param>
        /// <param name="movearea"></param>
        /// <param name="color"></param>
        /// <param name="visible"></param>
        public PrimitivePara(VertexPositionColor[] vertices, int[] indices, Vector3 center, float diameter, float orientation, Vector3 orientation3d, Vector3 speed, float movearea, Color color, bool visible)
        {
            this.vertices = vertices;
            this.indices = indices;
            this.BasePara = new BasePara(center, diameter, orientation, orientation3d, speed, movearea, color, visible);
        }

        /// <summary>
        /// Init all parameters to custom
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="indices"></param>
        /// <param name="basepara"></param>
        public PrimitivePara(VertexPositionColor[] vertices, int[] indices, BasePara basepara)
        {
            this.vertices = vertices;
            this.indices = indices;
            this.BasePara = basepara;
        }

        /// <summary>
        /// Get default Primitive parameters -- A White Point at Origin in World Space
        /// </summary>
        public static PrimitivePara Default
        {
            get
            {
                return new PrimitivePara(new VertexPositionColor[] { new VertexPositionColor(Vector3.Zero, Color.White) }, new int[] { 0 }, BasePara.Default);
            }
        }


        #region Primitive Functions

        /// <summary>
        /// Get a Arrow parameter with center=(0,0,0), headangle=60(deg), headslopesize=1(visual angle degree)
        /// </summary>
        /// <param name="diameter"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static PrimitivePara Arrow(float diameter, Color color)
        {
            return Arrow(diameter, color, Vector3.Zero, 60.0f, 1.0f);
        }

        /// <summary>
        /// Get a Arrow parameter
        /// </summary>
        /// <param name="diameter"></param>
        /// <param name="color"></param>
        /// <param name="center"></param>
        /// <param name="headangle"></param>
        /// <param name="headslopesize"></param>
        /// <returns></returns>
        public static PrimitivePara Arrow(float diameter, Color color, Vector3 center, float headangle, float headslopesize)
        {
            VertexPositionColor[] v = new VertexPositionColor[4];
            int[] i = new int[6];
            v[0].Position = Vector3.UnitY * diameter / 2;
            v[0].Color = color;
            v[1].Position = -v[0].Position;
            v[1].Color = color;
            v[2].Position.X = -(float)Math.Sin(headangle / 2 * Math.PI / 180) * headslopesize;
            v[2].Position.Y = v[0].Position.Y - (float)Math.Cos(headangle / 2 * Math.PI / 180) * headslopesize;
            v[2].Position.Z = 0.0f;
            v[2].Color = color;
            v[3].Position.X = -v[2].Position.X;
            v[3].Position.Y = v[2].Position.Y;
            v[3].Position.Z = 0.0f;
            v[3].Color = color;

            i[0] = 0;
            i[1] = 1;
            i[2] = 0;
            i[3] = 2;
            i[4] = 0;
            i[5] = 3;

            return new PrimitivePara(v, i, center, diameter, 0.0f, Vector3.Zero, Vector3.Zero, 10.0f, color, true);
        }

        /// <summary>
        /// Get a Cross parameter
        /// </summary>
        /// <param name="diameter"></param>
        /// <param name="color"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        public static PrimitivePara Cross(float diameter, Color color, Vector3 center)
        {
            VertexPositionColor[] v = new VertexPositionColor[4];
            int[] i = new int[4];
            v[0].Position = -Vector3.UnitX * diameter / 2;
            v[0].Color = color;
            v[1].Position = -v[0].Position;
            v[1].Color = color;
            v[2].Position = Vector3.UnitY * diameter / 2;
            v[2].Color = color;
            v[3].Position = -v[2].Position;
            v[3].Color = color;

            i[0] = 0;
            i[1] = 1;
            i[2] = 2;
            i[3] = 3;

            return new PrimitivePara(v, i, center, diameter, 0.0f, Vector3.Zero, Vector3.Zero, 10.0f, color, true);
        }

        /// <summary>
        /// Get a circle parameter with center=(0,0,0), resolution=100 points in the circle
        /// </summary>
        /// <param name="diameter"></param>
        /// <param name="color"></param>
        /// <param name="isfilled"></param>
        /// <returns></returns>
        public static PrimitivePara Circle(float diameter, Color color, bool isfilled)
        {
            return Circle(diameter, color, Vector3.Zero, 100, isfilled);
        }

        /// <summary>
        /// Get a Circle parameter
        /// </summary>
        /// <param name="diameter"></param>
        /// <param name="color"></param>
        /// <param name="center"></param>
        /// <param name="resolution"></param>
        /// <param name="isfilled"></param>
        /// <returns></returns>
        public static PrimitivePara Circle(float diameter, Color color, Vector3 center, int resolution, bool isfilled)
        {
            VertexPositionColor[] v;
            int[] index;
            double res_rad = 2 * Math.PI / resolution;

            if (isfilled)
            {
                v = new VertexPositionColor[resolution + 2];
                index = new int[resolution + 2];
                v[0].Position = Vector3.Zero;
                v[0].Color = color;
                index[0] = 0;
                for (int i = 1; i < v.Length - 1; i++)
                {
                    v[i].Position.X = (float)Math.Sin(res_rad * (i - 1)) * diameter / 2;
                    v[i].Position.Y = (float)Math.Cos(res_rad * (i - 1)) * diameter / 2;
                    v[i].Position.Z = 0.0f;
                    v[i].Color = color;
                    index[i] = i;
                }
                v[resolution + 1] = v[1];
                index[resolution + 1] = resolution + 1;
            }
            else
            {
                v = new VertexPositionColor[resolution + 1];
                index = new int[resolution + 2];
                for (int i = 0; i < v.Length - 1; i++)
                {
                    v[i].Position.X = (float)Math.Sin(res_rad * i) * diameter / 2;
                    v[i].Position.Y = (float)Math.Cos(res_rad * i) * diameter / 2;
                    v[i].Position.Z = 0.0f;
                    v[i].Color = color;
                    index[i] = i;
                }
                v[resolution] = v[0];
                index[resolution] = resolution;
            }

            return new PrimitivePara(v, index, center, diameter, 0.0f, Vector3.Zero, Vector3.Zero, 10.0f, color, true);
        }

        /// <summary>
        /// Get a RadialCircle parameter with center=(0,0,0), resolution=100 points in the circle
        /// </summary>
        /// <param name="diameter"></param>
        /// <param name="centercolor"></param>
        /// <param name="circlecolor"></param>
        /// <returns></returns>
        public static PrimitivePara RadialCircle(float diameter, Color centercolor, Color circlecolor)
        {
            return RadialCircle(diameter, centercolor, circlecolor, Vector3.Zero, 100);
        }

        /// <summary>
        /// Get a RadialCircle parameter
        /// </summary>
        /// <param name="diameter"></param>
        /// <param name="centercolor"></param>
        /// <param name="circlecolor"></param>
        /// <param name="center"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public static PrimitivePara RadialCircle(float diameter, Color centercolor, Color circlecolor, Vector3 center, int resolution)
        {
            VertexPositionColor[] v;
            int[] index;
            double res_rad = 2 * Math.PI / resolution;

            v = new VertexPositionColor[resolution + 2];
            index = new int[resolution + 2];
            v[0].Position = Vector3.Zero;
            v[0].Color = centercolor;
            index[0] = 0;
            for (int i = 1; i < v.Length - 1; i++)
            {
                v[i].Position.X = (float)Math.Sin(res_rad * (i - 1)) * diameter / 2;
                v[i].Position.Y = (float)Math.Cos(res_rad * (i - 1)) * diameter / 2;
                v[i].Position.Z = 0.0f;
                v[i].Color = circlecolor;
                index[i] = i;
            }
            v[resolution + 1] = v[1];
            index[resolution + 1] = resolution + 1;

            return new PrimitivePara(v, index, center, diameter, 0.0f, Vector3.Zero, Vector3.Zero, 10.0f, circlecolor, true);
        }

        /// <summary>
        /// Get Gaussian Mask Parameters using default Resolution:150
        /// </summary>
        /// <param name="diameter"></param>
        /// <param name="sigma"></param>
        /// <param name="color"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        public static PrimitivePara Gaussian(float diameter, float sigma, Color color, Vector3 center)
        {
            return PrimitivePara.Gaussian(diameter, sigma, color, color, center, 150);
        }

        /// <summary>
        /// Get Gaussian Mask Parameters
        /// </summary>
        /// <param name="diameter"></param>
        /// <param name="sigma"></param>
        /// <param name="centercolor"></param>
        /// <param name="circlecolor"></param>
        /// <param name="center"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public static PrimitivePara Gaussian(float diameter, float sigma, Color centercolor, Color circlecolor, Vector3 center, int resolution)
        {
            if (resolution % 2 == 0)
            {
                resolution += 1;
            }
            double radius = diameter / 2;
            double res_length = diameter / (resolution - 1);
            VertexPositionColor[] v = new VertexPositionColor[resolution * resolution];
            int[] vindex = new int[6 * (resolution - 1) * (resolution - 1)];

            int column, row;
            float x, y;
            double distance;
            for (int i = 0; i < v.Length; i++)
            {
                column = i % resolution;
                row = (int)Math.Floor((double)(i / resolution));
                x = (float)(-radius + column * res_length);
                y = (float)(radius - row * res_length);
                distance = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
                v[i].Position.X = x;
                v[i].Position.Y = y;
                v[i].Position.Z = 0.0f;
                if (distance > radius)
                {
                    v[i].Color = circlecolor;
                }
                else
                {
                    v[i].Color = new Color(centercolor.ToVector4() * (1 - (float)Math.Exp(-Math.Pow(distance, 2) / (2 * Math.Pow(sigma, 2)))));
                }
            }

            int d = 0;
            for (row = 0; row < resolution - 1; row++)
            {
                for (column = 0; column < resolution - 1; column++)
                {
                    vindex[d++] = row * resolution + column;
                    vindex[d++] = row * resolution + (column + 1);
                    vindex[d++] = (row + 1) * resolution + column;

                    vindex[d++] = row * resolution + (column + 1);
                    vindex[d++] = (row + 1) * resolution + (column + 1);
                    vindex[d++] = (row + 1) * resolution + column;
                }
            }

            return new PrimitivePara(v, vindex, center, diameter, 0.0f, Vector3.Zero, Vector3.Zero, 10.0f, centercolor, true);
        }

        #endregion


        /// <summary>
        /// Primitive Vertices Array
        /// </summary>
        public VertexPositionColor[] vertices;
        /// <summary>
        /// Primitive Indices Array
        /// </summary>
        public int[] indices;
        /// <summary>
        /// Basic Parameters
        /// </summary>
        public BasePara BasePara;
    }

    /// <summary>
    /// Stimulus Mask Parameters
    /// </summary>
    public struct MaskPara
    {
        /// <summary>
        /// Init to custom settings
        /// </summary>
        /// <param name="bpara"></param>
        /// <param name="mtype"></param>
        public MaskPara(BasePara bpara, MaskType mtype)
        {
            this.BasePara = bpara;
            this.MaskType = mtype;
        }

        /// <summary>
        /// Get Default:None Mask
        /// </summary>
        public static MaskPara Default
        {
            get
            {
                return new MaskPara(BasePara.Default, MaskType.None);
            }
        }


        /// <summary>
        /// Mask Basic Parameters
        /// </summary>
        public BasePara BasePara;
        /// <summary>
        /// Mask Type
        /// </summary>
        public MaskType MaskType;
    }

    /// <summary>
    /// Bar Type Parameters
    /// </summary>
    public struct BarPara
    {
        /// <summary>
        /// Init Bar Parameters
        /// </summary>
        /// <param name="bpara"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="direction"></param>
        /// <param name="speed"></param>
        public BarPara(BasePara bpara, float width, float height, float direction, float speed)
        {
            this.BasePara = bpara;
            this.width = width;
            this.height = height;
            this.direction = direction;
            this.speed = speed;
        }

        /// <summary>
        /// Get default parameters -- BasePara:default, width:3, height:1, direction:0, speed:10
        /// </summary>
        public static BarPara Default
        {
            get { return new BarPara(BasePara.Default, 3.0f, 1.0f, 0.0f, 10.0f); }
        }

        /// <summary>
        /// Encode common Bar Parameters in MarkerHeader
        /// </summary>
        /// <param name="PPort"></param>
        public void Encode(ParallelPort PPort)
        {
            PPort.MarkerEncode((int)Math.Floor(height * 100.0));
            PPort.MarkerEncode((int)Math.Floor(width * 100.0));
            PPort.MarkerEncode((int)Math.Floor((double)BasePara.orientation));
            PPort.MarkerEncode((int)Math.Floor((double)direction));
            PPort.MarkerEncode((int)Math.Floor((double)speed * 100.0));
            PPort.MarkerEncode((int)Math.Floor((BasePara.center.X + 60.0f) * 100.0));
            PPort.MarkerEncode((int)Math.Floor((BasePara.center.Y + 60.0f) * 100.0));
        }


        /// <summary>
        /// Basic Parameters
        /// </summary>
        public BasePara BasePara;
        /// <summary>
        /// Bar Width
        /// </summary>
        public float width;
        /// <summary>
        /// Bar Height
        /// </summary>
        public float height;
        /// <summary>
        /// Bar Moving Direction(deg)
        /// </summary>
        public float direction;
        /// <summary>
        /// Bar Moving Speed(Unit/s)
        /// </summary>
        public float speed;
    }

    /// <summary>
    /// Grating Type Parameters
    /// </summary>
    public struct GratingPara
    {
        /// <summary>
        /// Init Grating Parameters
        /// </summary>
        /// <param name="bpara"></param>
        /// <param name="mpara"></param>
        /// <param name="shape"></param>
        /// <param name="gtype"></param>
        /// <param name="mtype"></param>
        /// <param name="direction"></param>
        /// <param name="tf"></param>
        /// <param name="sf"></param>
        /// <param name="sphase"></param>
        /// <param name="luminance"></param>
        /// <param name="contrast"></param>
        /// <param name="lhcolor"></param>
        /// <param name="rlcolor"></param>
        /// <param name="resolution"></param>
        public GratingPara(BasePara bpara, MaskPara mpara, Shape shape, GratingType gtype, MoveType mtype, float direction, float tf, float sf, float sphase, float luminance, float contrast, Color lhcolor, Color rlcolor, int resolution)
        {
            this.BasePara = bpara;
            this.maskpara = mpara;
            this.shape = shape;
            this.gratingtype = gtype;
            this.movetype = mtype;
            this.direction = direction;
            this.tf = tf;
            this.sf = sf;
            this.sphase = sphase;
            this.luminance = luminance;
            this.contrast = contrast;
            this.lhcolor = lhcolor;
            this.rlcolor = rlcolor;
            this.resolution = resolution;
        }

        /// <summary>
        /// Get default parameters -- BasePara:default, Mask:None, Shape:circle, GratingType:sinusoidal, MoveType:drifting, 
        /// Direction:0, TF:2, SF:0.5, Phase:0, Luminance:0.5, Contrast:1, LeftHighColor:white, RightLowColor:black, Resolution:100
        /// </summary>
        public static GratingPara Default
        {
            get
            {
                return new GratingPara(BasePara.Default, MaskPara.Default, Shape.Circle, GratingType.Sinusoidal, MoveType.Drifting, 0.0f, 2.0f, 0.5f, 0.0f, 0.5f, 1.0f, Color.White, Color.Black, 100);
            }
        }

        /// <summary>
        /// Encode common Grating Parameters in MarkerHeader
        /// </summary>
        /// <param name="PPort"></param>
        public void Encode(ParallelPort PPort)
        {
            PPort.MarkerEncode((int)Math.Floor(tf * 100.0));
            PPort.MarkerEncode((int)Math.Floor(sf * 100.0));
            PPort.MarkerEncode((int)Math.Floor(sphase * 100.0));
            PPort.MarkerEncode((int)Math.Floor((double)BasePara.orientation));
            PPort.MarkerEncode((int)Math.Floor((double)direction));
            PPort.MarkerEncode((int)Math.Floor(luminance * 100.0));
            PPort.MarkerEncode((int)Math.Floor(contrast * 100.0));
            PPort.MarkerEncode((int)Math.Floor((BasePara.center.X + 60.0f) * 100.0));
            PPort.MarkerEncode((int)Math.Floor((BasePara.center.Y + 60.0f) * 100.0));
            PPort.MarkerEncode((int)Math.Floor(BasePara.diameter * 100.0));
        }


        /// <summary>
        /// Basic Parameters
        /// </summary>
        public BasePara BasePara;
        /// <summary>
        /// Mask Parameters
        /// </summary>
        public MaskPara maskpara;
        /// <summary>
        /// Grating Geometry
        /// </summary>
        public Shape shape;
        /// <summary>
        /// Grating Type
        /// </summary>
        public GratingType gratingtype;
        /// <summary>
        /// Move Type
        /// </summary>
        public MoveType movetype;
        /// <summary>
        /// Move Direction
        /// </summary>
        public float direction;
        /// <summary>
        /// Temporal Frequency(circle/s)
        /// </summary>
        public float tf;
        /// <summary>
        /// Spatial Frequency(circle/Unit)
        /// </summary>
        public float sf;
        /// <summary>
        /// Grating Spatial Phase (0-1):(0:2pi)
        /// </summary>
        public float sphase;
        /// <summary>
        /// Grating Average Luminance(0-1)
        /// </summary>
        public float luminance;
        /// <summary>
        /// Grating Brightness Contrast(0-1)
        /// </summary>
        public float contrast;
        /// <summary>
        /// Grating Left High Color
        /// </summary>
        public Color lhcolor;
        /// <summary>
        /// Grating Right Low Color
        /// </summary>
        public Color rlcolor;
        /// <summary>
        /// Grating Interpolation Resolution
        /// </summary>
        public int resolution;
    }

    /// <summary>
    /// Model Type Parameter
    /// </summary>
    public struct ModelPara
    {
        /// <summary>
        /// Init
        /// </summary>
        /// <param name="bpara"></param>
        /// <param name="mfile"></param>
        public ModelPara(BasePara bpara, string mfile)
        {
            this.BasePara = bpara;
            this.MFilename = mfile;
        }

        /// <summary>
        /// Get default model parameter -- BasePara:default, ModelName:""
        /// </summary>
        public static ModelPara Default
        {
            get
            {
                return new ModelPara(BasePara.Default, "");
            }
        }


        /// <summary>
        /// Basic Parameter
        /// </summary>
        public BasePara BasePara;
        /// <summary>
        /// Model File Name
        /// </summary>
        public string MFilename;
    }

    #endregion

    #region Type Define

    /// <summary>
    /// Visual Measure Unit
    /// </summary>
    public enum Unit
    {
        /// <summary>
        /// Visual Angle Degree
        /// </summary>
        Deg,
        /// <summary>
        /// Millimeter
        /// </summary>
        MM,
        /// <summary>
        /// GraphicsDevice Resolution
        /// </summary>
        Pixel
    }

    /// <summary>
    /// Visual Stimulus Shape
    /// </summary>
    public enum Shape
    {
        /// <summary>
        /// Point
        /// </summary>
        Point,
        /// <summary>
        /// Line
        /// </summary>
        Line,
        /// <summary>
        /// Curve
        /// </summary>
        Curve,
        /// <summary>
        /// Grid
        /// </summary>
        Grid,
        /// <summary>
        /// Triangle
        /// </summary>
        Triangle,
        /// <summary>
        /// Rectangle
        /// </summary>
        Rectangle,
        /// <summary>
        /// Quadrate
        /// </summary>
        Quadrate,
        /// <summary>
        /// Cuboid
        /// </summary>
        Cuboid,
        /// <summary>
        /// Cube
        /// </summary>
        Cube,
        /// <summary>
        /// Ellipse
        /// </summary>
        Ellipse,
        /// <summary>
        /// Circle
        /// </summary>
        Circle,
        /// <summary>
        /// Ellipsoid
        /// </summary>
        Ellipsoid,
        /// <summary>
        /// Sphere
        /// </summary>
        Sphere,
        /// <summary>
        /// Cone
        /// </summary>
        Cone,
        /// <summary>
        /// Cylinder
        /// </summary>
        Cylinder
    }

    /// <summary>
    /// Vision Stimulus Type
    /// </summary>
    public enum VSType
    {
        /// <summary>
        /// Bar
        /// </summary>
        Bar,
        /// <summary>
        /// Grating
        /// </summary>
        Grating,
        /// <summary>
        /// Image
        /// </summary>
        Image,
        /// <summary>
        /// Simple Primitive
        /// </summary>
        Primitive,
        /// <summary>
        /// Model
        /// </summary>
        SLModel,
        /// <summary>
        /// Quad
        /// </summary>
        SLQuad,
        /// <summary>
        /// Texture
        /// </summary>
        SLTexture,
        /// <summary>
        /// Text
        /// </summary>
        Text,
    }

    /// <summary>
    /// Grating Type
    /// </summary>
    public enum GratingType
    {
        /// <summary>
        /// Sinusoidal
        /// </summary>
        Sinusoidal,
        /// <summary>
        /// Linear
        /// </summary>
        Linear,
        /// <summary>
        /// Square
        /// </summary>
        Square,
    }

    /// <summary>
    /// Move Type
    /// </summary>
    public enum MoveType
    {
        /// <summary>
        /// Drifting
        /// </summary>
        Drifting,
        /// <summary>
        /// Standing
        /// </summary>
        Standing,
    }

    /// <summary>
    /// Mask Type
    /// </summary>
    public enum MaskType
    {
        /// <summary>
        /// No Mask
        /// </summary>
        None,
        /// <summary>
        /// Gaussian Mask with sigma
        /// </summary>
        Gaussian,
    }

    #endregion

}
