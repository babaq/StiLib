#region File Description
//-----------------------------------------------------------------------------
// SLForm.cs
//
// StiLib Form Service
// Copyright (c) Zhang Li. 2008-7-31.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;
using System.Reflection;
#endregion

namespace StiLib.Core
{
    /// <summary>
    /// StiLib base form class to Host a XNA render window
    /// </summary>
    public class SLForm : Form, IGraphicsDeviceService
    {
        #region Fields

        GraphicsDevice gd;
        ServiceContainer services;
        ContentManager content;
        PresentationParameters pp;
        AssemblySettings config;
        bool Go_Over;

        // IGraphicsDeviceService events.
        /// <summary>
        /// Handle DeviceCreated Event
        /// </summary>
        public event EventHandler DeviceCreated;
        /// <summary>
        /// Handle DeviceDisposing Event
        /// </summary>
        public event EventHandler DeviceDisposing;
        /// <summary>
        /// Handle DeviceReset Event
        /// </summary>
        public event EventHandler DeviceReset;
        /// <summary>
        /// Handle DeviceResetting Event
        /// </summary>
        public event EventHandler DeviceResetting;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current graphics device.
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            get { return gd; }
        }

        /// <summary>
        /// Gets an IServiceProvider containing our IGraphicsDeviceService.
        /// This can be used with components such as the ContentManager,
        /// which use this service to look up the GraphicsDevice.
        /// </summary>
        public ServiceContainer Services
        {
            get { return services; }
        }

        /// <summary>
        /// Gets the content manager
        /// </summary>
        public ContentManager Content
        {
            get { return content; }
        }

        /// <summary>
        /// Get StiLib Configuration
        /// </summary>
        public AssemblySettings SLConfig
        {
            get { return config; }
        }

        /// <summary>
        /// Toggle Between Content Run(true) and RunOver(false) State
        /// </summary>
        public bool GO_OVER
        {
            get { return Go_Over; }
            set { Go_Over = value; }
        }

        #endregion

        #region External Win32API Functions

        [DllImport("user32")]
        extern static bool ShowCursor(bool bShow);

        #endregion


        /// <summary>
        /// Init to Default -- width:800, height:600, refreshrate:0, Vsync:true, showcursor:true
        /// </summary>
        public SLForm() : this(800, 600, 0, true, true) { }

        /// <summary>
        /// Initializes the SLForm hosting XNA GraphicsDevice
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="refreshrate">windowed mode(0), fullscreen mode(>0)</param>
        /// <param name="isvsync"></param>
        /// <param name="isshowcursor"></param>
        public SLForm(int width, int height, int refreshrate, bool isvsync, bool isshowcursor)
        {
            #region Init GraphicsDevice

            // Check Shader Model 2.0 Support
            GraphicsDeviceCapabilities gdcap = GraphicsAdapter.DefaultAdapter.GetCapabilities(DeviceType.Hardware);
            if (gdcap.MaxPixelShaderProfile < ShaderProfile.PS_2_0 || gdcap.MaxVertexShaderProfile < ShaderProfile.VS_2_0)
            {
                System.Diagnostics.Debug.WriteLine("This Adapter does not support Shader Model 2.0.");
                MessageBox.Show("This Adapter does not support Shader Model 2.0.", "Warning !");
            }

            // Check MultiSampling Support
            pp = new PresentationParameters();
            int quality;
            if (GraphicsAdapter.DefaultAdapter.CheckDeviceMultiSampleType(DeviceType.Hardware, SurfaceFormat.Color, false, MultiSampleType.NonMaskable, out quality))
            {
                pp.MultiSampleType = MultiSampleType.NonMaskable;
                if (quality < 2)
                {
                    pp.MultiSampleQuality = quality;
                }
                else
                {
                    pp.MultiSampleQuality = 2;
                }
            }

            // Set Screen Presentation
            if (isvsync)
            {
                pp.PresentationInterval = PresentInterval.One;
            }
            else
            {
                pp.PresentationInterval = PresentInterval.Immediate;
            }
            if (refreshrate == 0)
            {
                pp.IsFullScreen = false;
                ClientSize = new System.Drawing.Size(width, height);
            }
            else
            {
                pp.IsFullScreen = true;
            }
            pp.FullScreenRefreshRateInHz = refreshrate;

            // Set Buffer
            pp.BackBufferCount = 3;
            pp.BackBufferWidth = width;
            pp.BackBufferHeight = height;
            pp.BackBufferFormat = SurfaceFormat.Color;
            pp.EnableAutoDepthStencil = true;
            pp.AutoDepthStencilFormat = DepthFormat.Depth24Stencil8;

            try
            {
                gd = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, DeviceType.Hardware, Handle, pp);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString(), "GraphicsDevice Initialization Failed !");
            }

            #endregion

            #region Init SLForm

            services = new ServiceContainer();
            // Register the service, so components like ContentManager can find it.
            services.AddService<IGraphicsDeviceService>(this);
            content = new ContentManager(services, "Content");

            // Cursor State
            ShowCursor(isshowcursor);

            // Hook the idle event to constantly redraw, getting a game style loop as default.
            Application.Idle += delegate { Invalidate(); };

            this.KeyDown += new KeyEventHandler(SLForm_KeyDown);
            this.MouseMove += new MouseEventHandler(SLForm_MouseMove);
            this.MouseDown += new MouseEventHandler(SLForm_MouseDown);
            this.Resize += new EventHandler(SLForm_Resize);

            // Get StiLib Configuration
            config = new AssemblySettings(Assembly.GetAssembly(typeof(AssemblySettings)));

            #endregion

            // Give derived classes a chance to load content.
            LoadContent();

            // Give derived classes a chance to initialize themselves.
            Initialize();
        }

        
        /// <summary>
        /// Check if Device is ready
        /// </summary>
        /// <returns></returns>
        bool CheckDevice()
        {
            if (gd == null)
            {
                return false;
            }

             switch (gd.GraphicsDeviceStatus)
             {
                 case GraphicsDeviceStatus.Lost:
                     return false;
                 case GraphicsDeviceStatus.NotReset:
                     try
                     {
                         gd.Reset(pp);
                         return true;
                     }
                     catch(Exception e)
                     {
                         MessageBox.Show(e.Message, "Error !");
                         return false;
                     }
                 default:
                     return true;
             }
        }

        /// <summary>
        /// Redraws in response to a WinForms paint message
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (CheckDevice())
            {
                // Draw using the GraphicsDevice
                Update();
                Draw();

                gd.Present();
            }
        }

        /// <summary>
        /// Ignores WinForms paint-background messages. The default implementation
        /// would clear the client to the current background color, causing
        /// flickering when our OnPaint implementation then immediately draws some
        /// other color over the top using the XNA Framework GraphicsDevice.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        /// <summary>
        /// Disposes the SLForm, unloading the ContentManager.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnloadContent();
            }

            base.Dispose(disposing);
        }


        #region Custom Virtual Functions

        /// <summary>
        ///  Registered the SLForm Resize handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void SLForm_Resize(object sender, EventArgs e)
        {
            if (gd.GraphicsDeviceStatus == GraphicsDeviceStatus.Normal && !gd.PresentationParameters.IsFullScreen)
            {
                pp.BackBufferHeight = Math.Max(1, ClientSize.Height);
                pp.BackBufferWidth = Math.Max(1, ClientSize.Width);
                gd.Reset(pp);
            }
        }

        /// <summary>
        /// Registered the SLForm MouseDown handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void SLForm_MouseDown(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// Registered the SLForm MouseMove handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void SLForm_MouseMove(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// Registered the SLForm key handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void SLForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
                Application.Exit();
            }
            if (e.KeyCode == Keys.Space)
            {
                Go_Over = !Go_Over;
                if (Go_Over) // Experiment Begin
                {
                    SetFlow();
                    MarkHead();
                }
            }
        }


        /// <summary>
        /// Derived classes override this to initialize their drawing code.
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// LoadContent will be called before Initialize() and is the place to load
        /// all of your content.
        /// </summary>
        protected virtual void LoadContent()
        {
        }

        /// <summary>
        /// UnloadContent will be called once SLForm is to disposed and is the place to unload
        /// all content.
        /// </summary>
        protected virtual void UnloadContent()
        {
        }

        /// <summary>
        /// Custom Content Update
        /// </summary>
        protected new virtual void Update()
        {
        }

        /// <summary>
        /// Derived classes override this to draw themselves using the GraphicsDevice.
        /// </summary>
        protected virtual void Draw()
        {
            gd.Clear(Color.Pink);
        }

        /// <summary>
        /// Set Flow Control to begin a new Experiment
        /// </summary>
        protected virtual void SetFlow()
        {
        }

        /// <summary>
        /// Transfer Experiment MarkerHeader only once before it's running
        /// </summary>
        protected virtual void MarkHead()
        {
        }

        #endregion

    }
}
