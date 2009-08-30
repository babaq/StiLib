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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using StiLib.Core;
#endregion

namespace StiLib.Vision
{
    /// <summary>
    /// StiLib Vision Stimulus Abstract Base Class
    /// </summary>
    public abstract class VisionStimulus : ICloneable
    {
        #region Visual Configurations

        /// <summary>
        /// Perpendicular Distance From Eye to Display(mm), Usually Map 1(deg) Visual Angle to 10(mm) on Display. 
        /// </summary>
        public float distance2Display;
        /// <summary>
        /// Display Viewable Width : Height AspectRatio, 1.333(4:3) or 1.778(16:9)
        /// </summary>
        public float displayRatio;
        /// <summary>
        /// Display Viewable Diagonal Length in Inch(1 Inch = 25.4 mm), 17inch CRT Display: 15 Viewable, 22inch CRT Display: 20 Viewable
        /// </summary>
        public float displaySize;

        #endregion

        #region Display Parameters

        /// <summary>
        /// Current Display Viewable Height in MM
        /// </summary>
        public float display_H_mm;
        /// <summary>
        /// Current Display Viewable Width in MM
        /// </summary>
        public float display_W_mm;
        /// <summary>
        /// Current Display Viewable Height in Current Full Screen Resolution Pixel
        /// </summary>
        public int display_H_pixel;
        /// <summary>
        /// Current Display Viewable Width in Current Full Screen Resolution Pixel
        /// </summary>
        public int display_W_pixel;
        /// <summary>
        /// Current Display Viewable Height in Visual Angle Degree
        /// </summary>
        public float display_H_deg;
        /// <summary>
        /// Current Display Viewable Width in Visual Angle Degree
        /// </summary>
        public float display_W_deg;

        #endregion

        #region Unit Conversion Factors

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

        #region Stimulus Internal States

        /// <summary>
        /// Global Camera to View Vision Stimulus
        /// </summary>
        public SLCamera globalCamera;
        /// <summary>
        /// Global Unit to Measure Vision Stimulus Size
        /// </summary>
        public Unit unit;
        /// <summary>
        /// Current Global Unit Conversion Factor
        /// </summary>
        public float unitFactor;
        /// <summary>
        /// Internal GraphicsDevice Reference
        /// </summary>
        public GraphicsDevice gdRef;
        /// <summary>
        /// Vision Stimulus 3D Orientation Rotation Matrix
        /// </summary>
        protected Matrix ori3DMatrix = Matrix.Identity;
        /// <summary>
        /// Vision Stimulus World Transform Matrix
        /// </summary>
        protected Matrix worldMatrix = Matrix.Identity;
        /// <summary>
        /// Vision Stimulus Vertex Declaration
        /// </summary>
        public VertexDeclaration vertexDeclaration;
        /// <summary>
        /// Vision Stimulus Vertex Array
        /// </summary>
        protected VertexPositionColor[] vertexArray;
        /// <summary>
        /// Vision Stimulus Index Array
        /// </summary>
        protected int[] indexArray;
        /// <summary>
        /// Vision Stimulus Vertex Buffer
        /// </summary>
        public VertexBuffer vertexBuffer;
        /// <summary>
        /// Vision Stimulus Index Buffer
        /// </summary>
        public IndexBuffer indexBuffer;
        /// <summary>
        /// Vision Stimulus Content Manager
        /// </summary>
        public ContentManager contentManager;
        /// <summary>
        /// Vision Stimulus Sprite Batch
        /// </summary>
        public SpriteBatch spriteBatch;
        /// <summary>
        /// Vision Stimulus Basic Effect
        /// </summary>
        public BasicEffect basicEffect;

        #endregion


        /// <summary>
        /// Initialize Vision Stimulus Using Default Settings -- distance2Display: 570mm, displayRatio: 1.333, displaySize: 20inch, 
        /// Fullscreen_w_pixel: 800, Fullscreen_h_pixel: 600, Camera: Default, Unit: Deg
        /// </summary>
        public VisionStimulus()
            : this(570, 1.333f, 20, 800, 600, new SLCamera(), Unit.Deg)
        {
        }

        /// <summary>
        /// Initialize Vision Stimulus Using StiLib Configuration File, 
        /// and Default Settings -- Fullscreen_w_pixel: 800, Fullscreen_h_pixel: 600, Camera: Default, Unit: Deg
        /// </summary>
        /// <param name="slconfig"></param>
        public VisionStimulus(AssemblySettings slconfig)
            : this(Convert.ToSingle(slconfig["distance2display"]), Convert.ToSingle(slconfig["displayratio"]), Convert.ToSingle(slconfig["displaysize"]), 800, 600, new SLCamera(), Unit.Deg)
        {
        }

        /// <summary>
        /// Initialize Vision Stimulus Using StiLib Configuration File, Current GraphicsDevice Fullscreen Resolution and Viewport, 
        /// and Default Settings -- Unit: Deg and Set Internal GraphicsDevice Reference
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="slconfig"></param>
        public VisionStimulus(GraphicsDevice gd, AssemblySettings slconfig)
            : this(Convert.ToSingle(slconfig["distance2display"]), Convert.ToSingle(slconfig["displayratio"]), Convert.ToSingle(slconfig["displaysize"]), gd.DisplayMode.Width, gd.DisplayMode.Height, new SLCamera(gd), Unit.Deg)
        {
            gdRef = gd;
        }

        /// <summary>
        /// Initialize Vision Stimulus Using Current GraphicsDevice Fullscreen Resolution and Viewport, 
        /// and Default Settings -- distance2Display: 570mm, displayRatio: 1.333, displaySize: 20inch, Unit: Deg
        /// and Set Internal GraphicsDevice Reference
        /// </summary>
        /// <param name="gd"></param>
        public VisionStimulus(GraphicsDevice gd)
            : this(570, 1.333f, 20, gd.DisplayMode.Width, gd.DisplayMode.Height, new SLCamera(gd), Unit.Deg)
        {
            gdRef = gd;
        }

        /// <summary>
        /// Initialize Vision Stimulus According to Current GraphicsDevice Fullscreen Resolution, 
        /// and Set Internal GraphicsDevice Reference
        /// </summary>
        /// <param name="distance2display"></param>
        /// <param name="displayratio"></param>
        /// <param name="displaysize"></param>
        /// <param name="gd"></param>
        /// <param name="camera"></param>
        /// <param name="unit"></param>
        public VisionStimulus(float distance2display, float displayratio, float displaysize, GraphicsDevice gd, SLCamera camera, Unit unit)
            : this(distance2display, displayratio, displaysize, gd.DisplayMode.Width, gd.DisplayMode.Height, camera, unit)
        {
            gdRef = gd;
        }

        /// <summary>
        /// Initialize Vision Stimulus
        /// </summary>
        /// <param name="distance2display"></param>
        /// <param name="displayratio"></param>
        /// <param name="displaysize"></param>
        /// <param name="fullscreen_w_pixel"></param>
        /// <param name="fullscreen_h_pixel"></param>
        /// <param name="camera"></param>
        /// <param name="unit"></param>
        public VisionStimulus(float distance2display, float displayratio, float displaysize, int fullscreen_w_pixel, int fullscreen_h_pixel, SLCamera camera, Unit unit)
        {
            InitVS(distance2display, displayratio, displaysize, fullscreen_w_pixel, fullscreen_h_pixel, camera, unit);
        }


        /// <summary>
        /// Set Configuration Parameters
        /// </summary>
        /// <param name="distance2display"></param>
        /// <param name="displayratio"></param>
        /// <param name="displaysize"></param>
        /// <param name="fullscreen_w_pixel"></param>
        /// <param name="fullscreen_h_pixel"></param>
        /// <param name="camera"></param>
        /// <param name="unit"></param>
        public void SetConfig(float distance2display, float displayratio, float displaysize, int fullscreen_w_pixel, int fullscreen_h_pixel, SLCamera camera, Unit unit)
        {
            this.distance2Display = distance2display;
            this.displayRatio = displayratio;
            this.displaySize = displaysize;
            this.display_W_pixel = fullscreen_w_pixel;
            this.display_H_pixel = fullscreen_h_pixel;
            this.globalCamera = camera.Clone() as SLCamera;
            this.unit = unit;
        }

        /// <summary>
        /// Setup Visual Environment According to Internal Configuration Parameters
        /// </summary>
        public void Config()
        {
            // Map Display(mm) to Visual Angle(deg)
            mm_p_deg = 2 * 0.008737f * distance2Display; // tan(0.5) = 0.008737

            // Get Display Viewable Size in MM
            display_H_mm = (float)Math.Sqrt(Math.Pow(displaySize * SLConstant.MM_p_Inch, 2) / (1 + Math.Pow(displayRatio, 2)));
            display_W_mm = display_H_mm * displayRatio;

            // Get Display Viewable Size in Deg
            display_H_deg = display_H_mm / mm_p_deg;
            display_W_deg = display_H_deg * displayRatio;

            // Map Display(pixel) to Display(mm)
            pixel_p_mm = display_W_pixel / display_W_mm;

            // Map Display(pixel) to Visual Angle(deg)
            pixel_p_deg = display_W_pixel / display_W_deg;

            // Set Current Global Unit Conversion Factor According to Current Global Unit
            switch (unit)
            {
                case Unit.Deg:
                    unitFactor = pixel_p_deg; break;
                case Unit.MM:
                    unitFactor = pixel_p_mm; break;
                case Unit.Pixel:
                    unitFactor = 1.0f; break;
            }
        }

        /// <summary>
        /// Initialize Vision Stimulus Configuration
        /// </summary>
        /// <param name="distance2display"></param>
        /// <param name="displayratio"></param>
        /// <param name="displaysize"></param>
        /// <param name="fullscreen_w_pixel"></param>
        /// <param name="fullscreen_h_pixel"></param>
        /// <param name="camera"></param>
        /// <param name="unit"></param>
        public void InitVS(float distance2display, float displayratio, float displaysize, int fullscreen_w_pixel, int fullscreen_h_pixel, SLCamera camera, Unit unit)
        {
            if ((this.distance2Display != distance2display) || (this.displayRatio != displayratio) || (this.displaySize != displaysize) || (this.display_W_pixel != fullscreen_w_pixel) || (this.display_H_pixel != fullscreen_h_pixel) || (!this.globalCamera.Equals(camera)) || (this.unit != unit))
            {
                SetConfig(distance2display, displayratio, displaysize, fullscreen_w_pixel, fullscreen_h_pixel, camera, unit);
                Config();
            }
        }

        /// <summary>
        /// Set Internal GraphicsDevice Reference,
        /// and Initialize Vision Stimulus Configuration According to Current GraphicsDevice Fullscreen Resolution and Viewport. 
        /// This method needs to be called before any derived stimulus initialization.
        /// </summary>
        /// <param name="gd"></param>
        public void InitVS(GraphicsDevice gd)
        {
            gdRef = gd;
            InitVS(distance2Display, displayRatio, displaySize, gd.DisplayMode.Width, gd.DisplayMode.Height, globalCamera.Clone(gd) as SLCamera, unit);
        }

        /// <summary>
        /// Set Internal GraphicsDevice Reference,
        /// and Initialize Vision Stimulus Configuration According to StiLib Configuration File, 
        /// Current GraphicsDevice Fullscreen Resolution and Viewport. 
        /// This method needs to be called before any derived stimulus initialization.
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="slconfig"></param>
        public void InitVS(GraphicsDevice gd, AssemblySettings slconfig)
        {
            gdRef = gd;
            InitVS(Convert.ToSingle(slconfig["distance2display"]), Convert.ToSingle(slconfig["displayratio"]), Convert.ToSingle(slconfig["displaysize"]), gd.DisplayMode.Width, gd.DisplayMode.Height, globalCamera.Clone(gd) as SLCamera, unit);
        }


        #region Abstract Virtual Functions

        /// <summary>
        /// Initialize Stimulus According to Internal Parameters
        /// </summary>
        /// <param name="gd"></param>
        public abstract void Init(GraphicsDevice gd);

        /// <summary>
        /// Draw Stimulus Using Current GraphicsDevice
        /// </summary>
        /// <param name="gd"></param>
        public abstract void Draw(GraphicsDevice gd);

        /// <summary>
        /// Creates a New Object That is a Copy of the Current Instance
        /// </summary>
        /// <returns></returns>
        public abstract object Clone();

        /// <summary>
        /// Vision Stimulus Basic Parameters
        /// </summary>
        public abstract vsBasePara BasePara { get; set; }

        /// <summary>
        /// Vision Stimulus Center
        /// </summary>
        public abstract Vector3 Center { get; set; }

        /// <summary>
        /// Vision Stimulus 3D Space Speed
        /// </summary>
        public abstract Vector3 Speed3D { get; set; }

        /// <summary>
        /// Vision Stimulus Visible State
        /// </summary>
        public abstract bool Visible { get; set; }

        #endregion


        /// <summary>
        /// Load Stimulus Content
        /// </summary>
        /// <param name="service"></param>
        /// <param name="contentpath"></param>
        /// <param name="content"></param>
        public virtual void LoadContent(IServiceProvider service, string contentpath, string content)
        {
        }

        /// <summary>
        /// Update Stimulus
        /// </summary>
        /// <param name="time"></param>
        public virtual void Update(double time)
        {
        }

        /// <summary>
        /// Draw Stimulus Using Internal GraphicsDevice Reference
        /// </summary>
        public virtual void Draw()
        {
            if (gdRef != null)
            {
                Draw(gdRef);
            }
            else
            {
                SLConstant.ShowMessage("No Internal GraphicsDevice Reference, Please InitVS(GraphicsDevice gd) First !");
            }
        }

        /// <summary>
        /// Sets Vision Stimulus Vertex Buffer
        /// </summary>
        /// <param name="gd"></param>
        public virtual void SetVertexBuffer(GraphicsDevice gd)
        {
            int temp = VertexArray.Length * VertexPositionColor.SizeInBytes;
            if (vertexBuffer == null)
            {
                vertexBuffer = new VertexBuffer(gd, temp, BufferUsage.None);
            }
            else
            {
                if (temp > vertexBuffer.SizeInBytes)
                {
                    vertexBuffer.Dispose();
                    vertexBuffer = new VertexBuffer(gd, temp, BufferUsage.None);
                }
            }
            vertexBuffer.SetData<VertexPositionColor>(VertexArray);
        }

        /// <summary>
        /// Sets Vision Stimulus Vertex Buffer Using Internal GraphicsDevice Reference
        /// </summary>
        public virtual void SetVertexBuffer()
        {
            SetVertexBuffer(gdRef);
        }

        /// <summary>
        /// Sets Vision Stimulus Index Buffer
        /// </summary>
        /// <param name="gd"></param>
        public virtual void SetIndexBuffer(GraphicsDevice gd)
        {
            int temp = IndexArray.Length * sizeof(int);
            if (indexBuffer == null)
            {
                indexBuffer = new IndexBuffer(gd, temp, BufferUsage.None, IndexElementSize.ThirtyTwoBits);
            }
            else
            {
                if (temp > indexBuffer.SizeInBytes)
                {
                    indexBuffer.Dispose();
                    indexBuffer = new IndexBuffer(gd, temp, BufferUsage.None, IndexElementSize.ThirtyTwoBits);
                }
            }
            indexBuffer.SetData<int>(IndexArray);
        }

        /// <summary>
        /// Sets Vision Stimulus Index Buffer Using Internal GraphicsDevice Reference
        /// </summary>
        public virtual void SetIndexBuffer()
        {
            SetIndexBuffer(gdRef);
        }

        /// <summary>
        /// Vision Stimulus Vertex Array
        /// </summary>
        public virtual VertexPositionColor[] VertexArray
        {
            get { return vertexArray; }
            set { vertexArray = value; }
        }

        /// <summary>
        /// Vision Stimulus Index Array
        /// </summary>
        public virtual int[] IndexArray
        {
            get { return indexArray; }
            set { indexArray = value; }
        }

        /// <summary>
        /// Vision Stimulus 3D Orientation Rotation Matrix
        /// </summary>
        public virtual Matrix Ori3DMatrix
        {
            get { return ori3DMatrix; }
            set { ori3DMatrix = value; }
        }

        /// <summary>
        /// Vision Stimulus World Transform Matrix
        /// </summary>
        public virtual Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set { worldMatrix = value; }
        }

        /// <summary>
        /// Gets Global Camera View. Sets BasicEffect View.
        /// </summary>
        public virtual Matrix ViewMatrix
        {
            get { return globalCamera.ViewMatrix; }
            set { basicEffect.View = value; }
        }

        /// <summary>
        /// Gets Global Camera Projection. Sets BasicEffect Projection.
        /// </summary>
        public virtual Matrix ProjectionMatrix
        {
            get { return globalCamera.GetUnitProjection(globalCamera.projectionType, unitFactor); }
            set { basicEffect.Projection = value; }
        }

        /// <summary>
        /// Vision Stimulus Projection Type
        /// </summary>
        public virtual ProjectionType ProjectionType
        {
            get { return globalCamera.projectionType; }
            set { globalCamera.projectionType = value; }
        }

        /// <summary>
        /// Gets Current Vision Stimulus Type
        /// </summary>
        public virtual VSType VSType
        {
            get { return BasePara.vstype; }
        }


        /// <summary>
        /// Check Primitive Count
        /// </summary>
        /// <param name="primitivetype"></param>
        /// <param name="numindices"></param>
        /// <param name="primitivecount"></param>
        /// <returns>Checked Primitive Count</returns>
        public static int CheckPrimitiveCount(PrimitiveType primitivetype, int numindices, int primitivecount)
        {
            int temp = 0;
            switch (primitivetype)
            {
                case PrimitiveType.PointList:
                    temp = numindices;
                    break;
                case PrimitiveType.LineList:
                    temp = numindices / 2;
                    break;
                case PrimitiveType.LineStrip:
                    temp = numindices - 1;
                    break;
                case PrimitiveType.TriangleList:
                    temp = numindices / 3;
                    break;
                case PrimitiveType.TriangleStrip:
                case PrimitiveType.TriangleFan:
                    temp = numindices - 2;
                    break;
            }

            if (primitivecount > temp)
            {
                primitivecount = temp;
            }
            return primitivecount;
        }

        /// <summary>
        /// Get 3D Orientation Rotation Matrix
        /// </summary>
        /// <param name="orientation3d"></param>
        /// <returns></returns>
        public static Matrix GetOri3DMatrix(Vector3 orientation3d)
        {
            return Matrix.CreateFromYawPitchRoll(orientation3d.Y, orientation3d.X, orientation3d.Z);
        }

    }

    #region Vision Stimulus Parameter Structs

    /// <summary>
    /// Visual Stimulus Basic Parameters
    /// </summary>
    public struct vsBasePara
    {
        /// <summary>
        /// Initialize BasePara Using Custom Parameters
        /// </summary>
        /// <param name="center"></param>
        /// <param name="diameter"></param>
        /// <param name="orientation"></param>
        /// <param name="orientation3d"></param>
        /// <param name="rotationspeed"></param>
        /// <param name="rotationspeed3d"></param>
        /// <param name="direction"></param>
        /// <param name="speed"></param>
        /// <param name="speed3d"></param>
        /// <param name="space"></param>
        /// <param name="color"></param>
        /// <param name="visible"></param>
        /// <param name="vstype"></param>
        /// <param name="contentname"></param>
        /// <param name="lifetime"></param>
        /// <param name="primitivetype"></param>
        public vsBasePara(Vector3 center, float diameter, float orientation, Vector3 orientation3d, float rotationspeed, Vector3 rotationspeed3d, float direction, float speed, Vector3 speed3d, float space, Color color, bool visible, VSType vstype, string contentname, double lifetime, PrimitiveType primitivetype)
        {
            this.center = center;
            this.diameter = diameter;
            this.orientation = orientation;
            this.orientation3D = orientation3d;
            this.rotationspeed = rotationspeed;
            this.rotationspeed3D = rotationspeed3d;
            this.direction = direction;
            this.speed = speed;
            this.speed3D = speed3d;
            this.space = space;
            this.color = color;
            this.visible = visible;
            this.vstype = vstype;
            this.contentname = contentname;
            this.lifetime = lifetime;
            this.primitivetype = primitivetype;
        }

        /// <summary>
        /// Gets Default BasePara -- center: (0, 0, 0), diameter: 5.0Unit, orientation: 0.0deg, orientation3D: (0, 0, 0), rotationspeed: 10.0Rad/s, rotationspeed3D: (0, 0, 0), 
        /// direction: 0.0deg, speed: 10.0Unit/s, speed3D: (0, 0, 0), space: 10.0Unit, color: White, visible: true, vstype: None, contentname: "BasicEffect", lifetime: 1.0sec, primitivetype: PointList
        /// </summary>
        public static vsBasePara Default
        {
            get
            {
                return new vsBasePara(Vector3.Zero, 5.0f, 0.0f, Vector3.Zero, 10.0f, Vector3.Zero, 0.0f, 10.0f, Vector3.Zero, 10.0f, Color.White, true, VSType.None, "BasicEffect", 1.0, PrimitiveType.PointList);
            }
        }

        /// <summary>
        /// Gets Default BasePara Except VSType
        /// </summary>
        /// <param name="vstype"></param>
        /// <returns></returns>
        public static vsBasePara VSTypeDefault(VSType vstype)
        {
            return new vsBasePara(Vector3.Zero, 5.0f, 0.0f, Vector3.Zero, 0.0f, Vector3.Zero, 0.0f, 0.0f, Vector3.Zero, 10.0f, Color.White, true, vstype, "BasicEffect", 1.0, PrimitiveType.PointList);
        }

        /// <summary>
        /// Gets Default BasePara Except VSType and PrimitiveType
        /// </summary>
        /// <param name="vstype"></param>
        /// <param name="ptype"></param>
        /// <returns></returns>
        public static vsBasePara VS_PTypeDefault(VSType vstype, PrimitiveType ptype)
        {
            return new vsBasePara(Vector3.Zero, 5.0f, 0.0f, Vector3.Zero, 0.0f, Vector3.Zero, 0.0f, 0.0f, Vector3.Zero, 10.0f, Color.White, true, vstype, "BasicEffect", 1.0, ptype);
        }

        /// <summary>
        /// Gets Default BasePara Except VSType and ContentName
        /// </summary>
        /// <param name="vstype"></param>
        /// <param name="contentname"></param>
        /// <returns></returns>
        public static vsBasePara VSTypeContentDefault(VSType vstype, string contentname)
        {
            return new vsBasePara(Vector3.Zero, 5.0f, 0.0f, Vector3.Zero, 0.0f, Vector3.Zero, 0.0f, 0.0f, Vector3.Zero, 10.0f, Color.White, true, vstype, contentname, 1.0, PrimitiveType.PointList);
        }

        /// <summary>
        /// Gets Default BasePara Except VSType, PrimitiveType and ContentName
        /// </summary>
        /// <param name="vstype"></param>
        /// <param name="ptype"></param>
        /// <param name="contentname"></param>
        /// <returns></returns>
        public static vsBasePara VS_PTypeContentDefault(VSType vstype, PrimitiveType ptype, string contentname)
        {
            return new vsBasePara(Vector3.Zero, 5.0f, 0.0f, Vector3.Zero, 0.0f, Vector3.Zero, 0.0f, 0.0f, Vector3.Zero, 10.0f, Color.White, true, vstype, contentname, 1.0, ptype);
        }

        /// <summary>
        /// Stimulus Center in World Space(Unit)
        /// </summary>
        public Vector3 center;
        /// <summary>
        /// Stimulus Size(Unit)
        /// </summary>
        public float diameter;
        /// <summary>
        /// Stimulus Orientation: [0, 180)(deg), Counterclockwise
        /// </summary>
        public float orientation;
        /// <summary>
        /// Stimulus 3D Space Orientation
        /// </summary>
        public Vector3 orientation3D;
        /// <summary>
        /// Stimulus Rotating Speed(Rad/s)
        /// </summary>
        public float rotationspeed;
        /// <summary>
        /// Stimulus 3D Space Rotating Speed(Rad/s)
        /// </summary>
        public Vector3 rotationspeed3D;
        /// <summary>
        /// Stimulus Moving Direction: [0, 360)(deg), Counterclockwise
        /// </summary>
        public float direction;
        /// <summary>
        /// Stimulus Translating Speed(Unit/s)
        /// </summary>
        public float speed;
        /// <summary>
        /// Stimulus 3D Space Translating Speed(Unit/s)
        /// </summary>
        public Vector3 speed3D;
        /// <summary>
        /// Define the space*space*space(Unit^3) 3D Space that Confines Stimulus
        /// </summary>
        public float space;
        /// <summary>
        /// Stimulus Color(R, G, B, A)
        /// </summary>
        public Color color;
        /// <summary>
        /// Stimulus Visible State
        /// </summary>
        public bool visible;
        /// <summary>
        /// Stimulus Type
        /// </summary>
        public VSType vstype;
        /// <summary>
        /// Stimulus Content Name
        /// </summary>
        public string contentname;
        /// <summary>
        /// Stimulus Life Time(sec)
        /// </summary>
        public double lifetime;
        /// <summary>
        /// Primitive Draw Type
        /// </summary>
        public PrimitiveType primitivetype;
    }

    /// <summary>
    /// Bar Parameters
    /// </summary>
    public struct BarPara
    {
        /// <summary>
        /// Initialize Bar Parameters
        /// </summary>
        /// <param name="basepara"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public BarPara(vsBasePara basepara, float width, float height)
        {
            basepara.vstype = VSType.Bar;
            basepara.primitivetype = PrimitiveType.TriangleFan;
            this.BasePara = basepara;
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Gets Default BarPara -- BasePara: VS_PTypeDefault(VSType.Bar, PrimitiveType.TriangleFan), width: 3.0, height: 1.0
        /// </summary>
        public static BarPara Default
        {
            get { return new BarPara(vsBasePara.VS_PTypeDefault(VSType.Bar, PrimitiveType.TriangleFan), 3.0f, 1.0f); }
        }

        /// <summary>
        /// Encode Common Bar Parameters in MarkerHeader
        /// </summary>
        /// <param name="PPort"></param>
        public void Encode(ParallelPort PPort)
        {
            PPort.MarkerEncode((int)Math.Floor(height * 100.0));
            PPort.MarkerEncode((int)Math.Floor(width * 100.0));
            PPort.MarkerEncode((int)Math.Floor(BasePara.orientation * 100.0));
            PPort.MarkerEncode((int)Math.Floor(BasePara.direction * 100.0));
            PPort.MarkerEncode((int)Math.Floor(BasePara.speed * 100.0));
            PPort.MarkerEncode((int)Math.Floor((BasePara.center.X + 60.0f) * 100.0));
            PPort.MarkerEncode((int)Math.Floor((BasePara.center.Y + 60.0f) * 100.0));
        }


        /// <summary>
        /// Basic Parameters
        /// </summary>
        public vsBasePara BasePara;
        /// <summary>
        /// Bar Width(Unit)
        /// </summary>
        public float width;
        /// <summary>
        /// Bar Height(Unit)
        /// </summary>
        public float height;
    }

    /// <summary>
    /// Grating Parameters
    /// </summary>
    public struct GratingPara
    {
        /// <summary>
        /// Initialize Grating Parameters
        /// </summary>
        /// <param name="basepara"></param>
        /// <param name="maskpara"></param>
        /// <param name="shape"></param>
        /// <param name="gratingtype"></param>
        /// <param name="movetype"></param>
        /// <param name="tf"></param>
        /// <param name="sf"></param>
        /// <param name="sphase"></param>
        /// <param name="luminance"></param>
        /// <param name="contrast"></param>
        /// <param name="lhcolor"></param>
        /// <param name="rlcolor"></param>
        /// <param name="resolution"></param>
        public GratingPara(vsBasePara basepara, MaskPara maskpara, Shape shape, GratingType gratingtype, MoveType movetype, float tf, float sf, float sphase, float luminance, float contrast, Color lhcolor, Color rlcolor, int resolution)
        {
            basepara.vstype = VSType.Grating;
            basepara.primitivetype = PrimitiveType.TriangleStrip;
            this.BasePara = basepara;
            this.maskpara = maskpara;
            this.shape = shape;
            this.gratingtype = gratingtype;
            this.movetype = movetype;
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
        /// Gets Default GratingPara -- BasePara:VS_PTypeContentDefault(VSType.Grating, PrimitiveType.TriangleStrip, "Grating"), Mask: None, Shape: Circle, 
        /// GratingType: Sinusoidal, MoveType: Drifting, TF: 2.0, SF: 0.5, SPhase: 0.0, Luminance: 0.5, Contrast: 1.0, LeftHighColor: White, RightLowColor: Black, Resolution: 100
        /// </summary>
        public static GratingPara Default
        {
            get
            {
                return new GratingPara(vsBasePara.VS_PTypeContentDefault(VSType.Grating, PrimitiveType.TriangleStrip, "Grating"), MaskPara.Default, Shape.Circle, GratingType.Sinusoidal, MoveType.Drifting, 2.0f, 0.5f, 0.0f, 0.5f, 1.0f, Color.White, Color.Black, 100);
            }
        }

        /// <summary>
        /// Encode Common Grating Parameters in MarkerHeader
        /// </summary>
        /// <param name="PPort"></param>
        public void Encode(ParallelPort PPort)
        {
            PPort.MarkerEncode((int)Math.Floor(tf * 100.0));
            PPort.MarkerEncode((int)Math.Floor(sf * 100.0));
            PPort.MarkerEncode((int)Math.Floor(sphase * 100.0));
            PPort.MarkerEncode((int)Math.Floor(BasePara.orientation * 100.0));
            PPort.MarkerEncode((int)Math.Floor(BasePara.direction * 100.0));
            PPort.MarkerEncode((int)Math.Floor(luminance * 100.0));
            PPort.MarkerEncode((int)Math.Floor(contrast * 100.0));
            PPort.MarkerEncode((int)Math.Floor((BasePara.center.X + 60.0f) * 100.0));
            PPort.MarkerEncode((int)Math.Floor((BasePara.center.Y + 60.0f) * 100.0));
            PPort.MarkerEncode((int)Math.Floor(BasePara.diameter * 100.0));
        }


        /// <summary>
        /// Basic Parameters
        /// </summary>
        public vsBasePara BasePara;
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
        /// Temporal Frequency(circle/s)
        /// </summary>
        public float tf;
        /// <summary>
        /// Spatial Frequency(circle/Unit)
        /// </summary>
        public float sf;
        /// <summary>
        /// Grating Spatial Phase [0, 1) == [0, 2pi)
        /// </summary>
        public float sphase;
        /// <summary>
        /// Grating Average Luminance [0, 1]
        /// </summary>
        public float luminance;
        /// <summary>
        /// Grating Luminance Contrast [0, 1]
        /// </summary>
        public float contrast;
        /// <summary>
        /// Grating Left High Peak Color
        /// </summary>
        public Color lhcolor;
        /// <summary>
        /// Grating Right Low Trough Color
        /// </summary>
        public Color rlcolor;
        /// <summary>
        /// Grating Interpolation Resolution of One Circle
        /// </summary>
        public int resolution;
    }

    /// <summary>
    /// Stimulus Mask Parameters
    /// </summary>
    public struct MaskPara
    {
        /// <summary>
        /// Initialize Mask Parameters
        /// </summary>
        /// <param name="basepara"></param>
        /// <param name="masktype"></param>
        public MaskPara(vsBasePara basepara, MaskType masktype)
        {
            basepara.vstype = VSType.Mask;
            this.BasePara = basepara;
            this.masktype = masktype;
        }

        /// <summary>
        /// Gets Default MaskPara -- Mask: None
        /// </summary>
        public static MaskPara Default
        {
            get
            {
                return new MaskPara(vsBasePara.VSTypeDefault(VSType.Mask), MaskType.None);
            }
        }


        /// <summary>
        /// Basic Parameters
        /// </summary>
        public vsBasePara BasePara;
        /// <summary>
        /// Mask Type
        /// </summary>
        public MaskType masktype;
    }

    /// <summary>
    /// Image Parameters
    /// </summary>
    public struct ImagePara
    {
        /// <summary>
        /// Initialize Image Parameters
        /// </summary>
        /// <param name="basepara"></param>
        public ImagePara(vsBasePara basepara)
        {
            basepara.vstype = VSType.Image;
            this.BasePara = basepara;
        }

        /// <summary>
        /// Gets Custom ImagePara
        /// </summary>
        /// <param name="imagename"></param>
        public ImagePara(string imagename)
        {
            this.BasePara = StiLib.Vision.vsBasePara.VSTypeContentDefault(VSType.Image, imagename);
        }


        /// <summary>
        /// Basic Parameters
        /// </summary>
        public vsBasePara BasePara;
    }

    /// <summary>
    /// Primitive Parameters
    /// </summary>
    public struct PrimitivePara
    {
        /// <summary>
        /// Initialize PrimitivePara Parameters
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="indices"></param>
        /// <param name="center"></param>
        /// <param name="diameter"></param>
        /// <param name="orientation"></param>
        /// <param name="orientation3d"></param>
        /// <param name="rotationspeed"></param>
        /// <param name="rotationspeed3d"></param>
        /// <param name="direction"></param>
        /// <param name="speed"></param>
        /// <param name="speed3d"></param>
        /// <param name="space"></param>
        /// <param name="color"></param>
        /// <param name="visible"></param>
        /// <param name="contentname"></param>
        /// <param name="lifetime"></param>
        /// <param name="primitivetype"></param>
        public PrimitivePara(VertexPositionColor[] vertices, int[] indices, Vector3 center, float diameter, float orientation, Vector3 orientation3d, float rotationspeed, Vector3 rotationspeed3d, float direction, float speed, Vector3 speed3d, float space, Color color, bool visible, string contentname, double lifetime, PrimitiveType primitivetype)
        {
            this.vertices = vertices;
            this.indices = indices;
            this.BasePara = new vsBasePara(center, diameter, orientation, orientation3d, rotationspeed, rotationspeed3d, direction, speed, speed3d, space, color, visible, VSType.Primitive, contentname, lifetime, primitivetype);
        }

        /// <summary>
        /// Initialize Primitive Parameters
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="indices"></param>
        /// <param name="basepara"></param>
        public PrimitivePara(VertexPositionColor[] vertices, int[] indices, vsBasePara basepara)
        {
            this.vertices = vertices;
            this.indices = indices;
            basepara.vstype = VSType.Primitive;
            this.BasePara = basepara;
        }

        /// <summary>
        /// Gets Default PrimitivePara -- A White Point at Origin in World Space
        /// </summary>
        public static PrimitivePara Default
        {
            get
            {
                return new PrimitivePara(new VertexPositionColor[] { new VertexPositionColor(Vector3.Zero, Color.White) }, new int[] { 0 }, vsBasePara.VSTypeDefault(VSType.Primitive));
            }
        }


        #region Custom Primitives

        /// <summary>
        /// Gets a Arrow Primitive Parameters with center = (0, 0, 0), headangle = 60.0(deg), headslopesize = 1.0(Unit)
        /// </summary>
        /// <param name="diameter"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static PrimitivePara Arrow(float diameter, Color color)
        {
            return Arrow(diameter, color, Vector3.Zero, 60.0f, 1.0f);
        }

        /// <summary>
        /// Gets a Arrow Primitive Parameters
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

            return new PrimitivePara(v, i, center, diameter, 0.0f, Vector3.Zero, 10.0f, Vector3.Zero, 0.0f, 10.0f, Vector3.Zero, 10.0f, color, true, "BasicEffect", 1.0, PrimitiveType.LineList);
        }

        /// <summary>
        /// Gets a Cross Primitive Parameters
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

            return new PrimitivePara(v, i, center, diameter, 0.0f, Vector3.Zero, 10.0f, Vector3.Zero, 0.0f, 10.0f, Vector3.Zero, 10.0f, color, true, "BasicEffect", 1.0, PrimitiveType.LineList);
        }

        /// <summary>
        /// Gets a Circle Primitive Parameters with center = (0, 0, 0), resolution = 100 points in a circle
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
        /// Gets a Circle Primitive Parameters
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
            PrimitiveType type;
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

                type = PrimitiveType.TriangleFan;
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

                type = PrimitiveType.LineStrip;
            }

            return new PrimitivePara(v, index, center, diameter, 0.0f, Vector3.Zero, 10.0f, Vector3.Zero, 0.0f, 10.0f, Vector3.Zero, 10.0f, color, true, "BasicEffect", 1.0, type);
        }

        /// <summary>
        /// Gets a RadialCircle Primitive Parameters with center = (0, 0, 0), resolution = 100 points in a circle
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
        /// Gets a RadialCircle Primitive Parameters
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

            return new PrimitivePara(v, index, center, diameter, 0.0f, Vector3.Zero, 10.0f, Vector3.Zero, 0.0f, 10.0f, Vector3.Zero, 10.0f, circlecolor, true, "BasicEffect", 1.0, PrimitiveType.TriangleFan);
        }

        /// <summary>
        /// Gets a Gaussian Mask Grid Primitive Parameters with Resolution = 150 (151*151 Grid)
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
        /// Gets a Gaussian Mask Grid Primitive Parameters
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

            return new PrimitivePara(v, vindex, center, diameter, 0.0f, Vector3.Zero, 10.0f, Vector3.Zero, 0.0f, 10.0f, Vector3.Zero, 10.0f, centercolor, true, "BasicEffect", 1.0, PrimitiveType.TriangleList);
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
        public vsBasePara BasePara;
    }

    /// <summary>
    /// Model Parameters
    /// </summary>
    public struct ModelPara
    {
        /// <summary>
        /// Initialize Model Parameters
        /// </summary>
        /// <param name="basepara"></param>
        /// <param name="modelname"></param>
        public ModelPara(vsBasePara basepara, string modelname)
        {
            basepara.vstype = VSType.SLModel;
            this.BasePara = basepara;
            this.modelname = modelname;
        }

        /// <summary>
        /// Gets Custom Model Parameters
        /// </summary>
        /// <param name="modelname"></param>
        public ModelPara(string modelname)
        {
            this.BasePara = StiLib.Vision.vsBasePara.VSTypeDefault(VSType.SLModel);
            this.modelname = modelname;
        }

        /// <summary>
        /// Gets Custom Model Parameters
        /// </summary>
        /// <param name="modelname"></param>
        /// <param name="shader"></param>
        public ModelPara(string modelname, string shader)
        {
            this.BasePara = StiLib.Vision.vsBasePara.VSTypeContentDefault(VSType.SLModel, shader);
            this.modelname = modelname;
        }


        /// <summary>
        /// Basic Parameters
        /// </summary>
        public vsBasePara BasePara;
        /// <summary>
        /// Model Name
        /// </summary>
        public string modelname;
    }

    /// <summary>
    /// Quad Parameters
    /// </summary>
    public struct Quad
    {
        /// <summary>
        /// Basic Parameters
        /// </summary>
        public vsBasePara BasePara;
        /// <summary>
        /// UpperLeft Vertex
        /// </summary>
        public Vector3 UpperLeft;
        /// <summary>
        /// LowerLeft Vertex
        /// </summary>
        public Vector3 LowerLeft;
        /// <summary>
        /// UpperRight Vertex
        /// </summary>
        public Vector3 UpperRight;
        /// <summary>
        /// LowerRight Vertex
        /// </summary>
        public Vector3 LowerRight;
        /// <summary>
        /// Normal Vector
        /// </summary>
        public Vector3 Normal;
        /// <summary>
        /// Up Direction
        /// </summary>
        public Vector3 Up;
        /// <summary>
        /// Left Direction
        /// </summary>
        public Vector3 Left;
        /// <summary>
        /// Vertex Array
        /// </summary>
        public VertexPositionNormalTexture[] vertices;
        /// <summary>
        /// Index Array
        /// </summary>
        public int[] indices;


        /// <summary>
        /// Initialize Quad Parameters with -- Center: (0, 0, 0), Normal: (0, 0, 1), Up: (0, 1, 0)
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Quad(float width, float height)
            : this(Vector3.Zero, Vector3.Backward, Vector3.Up, width, height)
        {
        }

        /// <summary>
        /// Initialize Quad Parameters
        /// </summary>
        /// <param name="center"></param>
        /// <param name="normal"></param>
        /// <param name="up"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Quad(Vector3 center, Vector3 normal, Vector3 up, float width, float height)
        {
            BasePara = StiLib.Vision.vsBasePara.VS_PTypeDefault(VSType.SLQuad, PrimitiveType.TriangleList);
            vertices = new VertexPositionNormalTexture[4];
            indices = new int[6];
            BasePara.center = center;
            Normal = normal;
            Up = up;

            // Calculate the quad corners
            Left = Vector3.Cross(normal, Up);
            Vector3 uppercenter = (Up * height / 2) + center;
            UpperLeft = uppercenter + (Left * width / 2);
            UpperRight = uppercenter - (Left * width / 2);
            LowerLeft = UpperLeft - (Up * height);
            LowerRight = UpperRight - (Up * height);

            FillVertices();
        }

        /// <summary>
        /// Gets a Quad Parameters with -- Width: 15, Height: 5
        /// </summary>
        public static Quad Default
        {
            get
            {
                return new Quad(15, 5);
            }
        }


        void FillVertices()
        {
            // Fill in texture coordinates to display full texture on quad
            Vector2 textureUpperLeft = new Vector2(0.0f, 0.0f);
            Vector2 textureUpperRight = new Vector2(1.0f, 0.0f);
            Vector2 textureLowerLeft = new Vector2(0.0f, 1.0f);
            Vector2 textureLowerRight = new Vector2(1.0f, 1.0f);

            // Provide a normal for each vertex
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Normal = Normal;
            }

            // Set the position and texture coordinate for each vertex
            vertices[0].Position = LowerLeft;
            vertices[0].TextureCoordinate = textureLowerLeft;
            vertices[1].Position = UpperLeft;
            vertices[1].TextureCoordinate = textureUpperLeft;
            vertices[2].Position = LowerRight;
            vertices[2].TextureCoordinate = textureLowerRight;
            vertices[3].Position = UpperRight;
            vertices[3].TextureCoordinate = textureUpperRight;

            // Set the index buffer for each vertex, using clockwise winding
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 2;
            indices[4] = 1;
            indices[5] = 3;
        }
    }

    /// <summary>
    /// Video Type Parameters
    /// </summary>
    public struct VideoPara
    {
        /// <summary>
        /// Initialize Video Parameters
        /// </summary>
        /// <param name="basepara"></param>
        public VideoPara(vsBasePara basepara)
        {
            basepara.vstype = VSType.SLVideo;
            this.BasePara = basepara;
        }

        /// <summary>
        /// Gets a Custom VideoPara
        /// </summary>
        /// <param name="videoname"></param>
        public VideoPara(string videoname)
        {
            this.BasePara = StiLib.Vision.vsBasePara.VSTypeContentDefault(VSType.SLVideo, videoname);
        }


        /// <summary>
        /// Basic Parameters
        /// </summary>
        public vsBasePara BasePara;
    }

    /// <summary>
    /// Text Parameters
    /// </summary>
    public struct TextPara
    {
        /// <summary>
        /// Initialize Text Parameters
        /// </summary>
        /// <param name="basepara"></param>
        public TextPara(vsBasePara basepara)
        {
            basepara.vstype = VSType.Text;
            this.BasePara = basepara;
        }

        /// <summary>
        /// Gets a Custom TextPara
        /// </summary>
        /// <param name="spritefont"></param>
        public TextPara(string spritefont)
        {
            this.BasePara = StiLib.Vision.vsBasePara.VSTypeContentDefault(VSType.Text, spritefont);
        }


        /// <summary>
        /// Basic Parameters
        /// </summary>
        public vsBasePara BasePara;
    }

    /// <summary>
    /// Collection Parameters
    /// </summary>
    public struct CollectionPara
    {
        /// <summary>
        /// Initialize Collection Parameters
        /// </summary>
        /// <param name="basepara"></param>
        /// <param name="collectioncenter"></param>
        /// <param name="collectionspeed"></param>
        public CollectionPara(vsBasePara basepara, Vector3 collectioncenter, Vector3 collectionspeed)
        {
            basepara.vstype = VSType.VSCollection;
            this.BasePara = basepara;
            this.CollectionCenter = collectioncenter;
            this.CollectionSpeed = collectionspeed;
        }

        /// <summary>
        /// Gets a Custom CollectionPara with -- CollectionCenter: (0, 0, 0), CollectionSpeed: (0, 0, 0) 
        /// </summary>
        /// <param name="shader"></param>
        public CollectionPara(string shader)
        {
            this.BasePara = StiLib.Vision.vsBasePara.VSTypeContentDefault(VSType.VSCollection, shader);
            this.CollectionCenter = Vector3.Zero;
            this.CollectionSpeed = Vector3.Zero;
        }


        /// <summary>
        /// Gets Default CollectionPara with -- BasePara: VSTypeDefault(VSType.VSCollection), CollectionCenter: (0, 0, 0), CollectionSpeed: (0, 0, 0) 
        /// </summary>
        public static CollectionPara Default
        {
            get { return new CollectionPara(vsBasePara.VSTypeDefault(VSType.VSCollection), Vector3.Zero, Vector3.Zero); }
        }


        /// <summary>
        /// Basic Parameters
        /// </summary>
        public vsBasePara BasePara;
        /// <summary>
        /// Current Collection Center
        /// </summary>
        public Vector3 CollectionCenter;
        /// <summary>
        /// Current Collection Speed
        /// </summary>
        public Vector3 CollectionSpeed;
    }

    #endregion

    #region Enum Types

    /// <summary>
    /// Vision Measurment Unit
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
        /// Display Resolution Pixel
        /// </summary>
        Pixel
    }

    /// <summary>
    /// Geometry Shape
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
        /// None
        /// </summary>
        None,
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
        /// Primitive
        /// </summary>
        Primitive,
        /// <summary>
        /// SLModel
        /// </summary>
        SLModel,
        /// <summary>
        /// SLQuad
        /// </summary>
        SLQuad,
        /// <summary>
        /// SLVideo
        /// </summary>
        SLVideo,
        /// <summary>
        /// Text
        /// </summary>
        Text,
        /// <summary>
        /// VSCollection
        /// </summary>
        VSCollection,
        /// <summary>
        /// Mask
        /// </summary>
        Mask,
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
