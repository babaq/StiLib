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
using StiLib.Vision;
using Microsoft.Xna.Framework;
#endregion

namespace StiLib.Core
{
    /// <summary>
    /// StiLib Base Form Class to Host a XNA Render Window
    /// </summary>
    public class SLForm : Form, IGraphicsDeviceService
    {
        #region Fields

        GraphicsDevice gd;
        ServiceContainer services;
        ContentManager cm;
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
        /// Gets the SLForm content manager
        /// </summary>
        public ContentManager Content
        {
            get { return cm; }
        }

        /// <summary>
        /// Gets the Internal GraphicsDevice Presentation Parameters
        /// </summary>
        public PresentationParameters PresentPara
        {
            get { return pp; }
        }

        /// <summary>
        /// Gets current StiLib Configurations
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


        /// <summary>
        /// Init to Default -- buffercount: 1, width: 800, height: 600, refreshrate: 0, vsync: false, showcursor: true, border: true, sizable: false, gamma: (1.0, 1.0, 1.0)
        /// </summary>
        public SLForm()
            : this(1, 800, 600, 0, false, true, true, false, Vector3.One)
        {
        }

        /// <summary>
        /// Init Using StiLib Configuration File
        /// </summary>
        /// <param name="configfile">"" to load default StiLib.dll.config file, otherwise indicate full file path</param>
        public SLForm(string configfile)
        {
            if (string.IsNullOrEmpty(configfile))
            {
                config = new AssemblySettings(Assembly.GetAssembly(typeof(AssemblySettings)));
            }
            else
            {
                config = new AssemblySettings(configfile);
            }

            SetGraphicsDevice(Convert.ToInt32(config["buffercount"]), Convert.ToInt32(config["width"]), Convert.ToInt32(config["height"]), Convert.ToInt32(config["refreshrate"]), Convert.ToBoolean(config["isvsync"]));
            SetGamma(new Vector3(Convert.ToSingle(config["gammaR"]), Convert.ToSingle(config["gammaG"]), Convert.ToSingle(config["gammaB"])));
            SetSLForm(Convert.ToBoolean(config["isshowcursor"]), Convert.ToBoolean(config["isborder"]), Convert.ToBoolean(config["issizable"]));

            // Give derived classes a chance to load content.
            LoadContent();
            // Give derived classes a chance to initialize themselves.
            Initialize();
        }

        /// <summary>
        /// Initializes the SLForm hosting XNA GraphicsDevice with default -- buffercount: 1, border: true, sizable: false, gamma: (1.0, 1.0, 1.0)
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="refreshrate">windowed mode(0), fullscreen mode(>0)</param>
        /// <param name="isvsync"></param>
        /// <param name="isshowcursor"></param>
        public SLForm(int width, int height, int refreshrate, bool isvsync, bool isshowcursor)
            : this(1, width, height, refreshrate, isvsync, isshowcursor, true, false, Vector3.One)
        {
        }

        /// <summary>
        /// Initializes the SLForm hosting XNA GraphicsDevice
        /// </summary>
        /// <param name="buffercount">0-3</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="refreshrate">windowed mode(0), fullscreen mode(>0)</param>
        /// <param name="isvsync"></param>
        /// <param name="isshowcursor"></param>
        /// <param name="isborder"></param>
        /// <param name="issizable"></param>
        /// <param name="gamma">current R, G, B gamma value</param>
        public SLForm(int buffercount, int width, int height, int refreshrate, bool isvsync, bool isshowcursor, bool isborder, bool issizable, Vector3 gamma)
        {
            SetGraphicsDevice(buffercount, width, height, refreshrate, isvsync);
            SetGamma(gamma);
            SetSLForm(isshowcursor, isborder, issizable);

            // Give derived classes a chance to load content.
            LoadContent();
            // Give derived classes a chance to initialize themselves.
            Initialize();
        }


        /// <summary>
        /// Set Default Hardware Adapter Targeting Current SLForm with Custom Settings
        /// </summary>
        /// <param name="buffercount">0-3</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="refreshrate">windowed mode(0), fullscreen mode(>0)</param>
        /// <param name="isvsync"></param>
        public void SetGraphicsDevice(int buffercount, int width, int height, int refreshrate, bool isvsync)
        {
            if (pp != null)
            {
                pp.Dispose();
            }
            pp = new PresentationParameters();

            // Check Shader Model 2.0 Support
            GraphicsDeviceCapabilities gdcap = GraphicsAdapter.DefaultAdapter.GetCapabilities(DeviceType.Hardware);
            if (gdcap.MaxPixelShaderProfile < ShaderProfile.PS_2_0 || gdcap.MaxVertexShaderProfile < ShaderProfile.VS_2_0)
            {
                MessageBox.Show("This Adapter Does Not Support Shader Model 2.0.", "Warning !");
            }

            // Check Full Screen MultiSampling Support
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
            if (buffercount > 3)
            {
                buffercount = 3;
            }
            pp.BackBufferCount = buffercount;
            pp.BackBufferWidth = Math.Max(1, width);
            pp.BackBufferHeight = Math.Max(1, height);
            pp.BackBufferFormat = SurfaceFormat.Color;
            pp.EnableAutoDepthStencil = true;
            pp.AutoDepthStencilFormat = DepthFormat.Depth24Stencil8;

            SetGraphicsDevice(pp);
        }

        /// <summary>
        /// Set Default Hardware Adapter Targeting Current SLForm with Custom Presentation Parameters
        /// </summary>
        /// <param name="pp"></param>
        public void SetGraphicsDevice(PresentationParameters pp)
        {
            if (gd == null)
            {
                try
                {
                    gd = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, DeviceType.Hardware, Handle, pp);

                    if (DeviceCreated != null)
                        DeviceCreated(this, EventArgs.Empty);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message.ToString(), "GraphicsDevice Initialization Failed !");
                }
            }
            else
            {
                try
                {
                    if (DeviceResetting != null)
                        DeviceResetting(this, EventArgs.Empty);

                    gd.Reset(pp);

                    if (DeviceReset != null)
                        DeviceReset(this, EventArgs.Empty);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message.ToString(), "GraphicsDevice Reset Failed !");
                }
            }
        }

        /// <summary>
        /// Linearize Gamma According to Current Gamma Value Using GraphicsDevice GammaRamp
        /// </summary>
        /// <param name="gamma">current R, G, B gamma value</param>
        public void SetGamma(Vector3 gamma)
        {
            bool isfullscreengamma;
            GraphicsDeviceCapabilities gdcap = gd.GraphicsDeviceCapabilities;
            if (gdcap.DriverCapabilities.SupportsFullScreenGamma)
            {
                isfullscreengamma = true;
            }
            else
            {
                isfullscreengamma = false;
                SLConstant.ShowMessage("This GraphicsDevice Does Not Support Full Screen Gamma Correction !");
            }
            if (isfullscreengamma && gd.PresentationParameters.IsFullScreen && (gamma.X != 1.0f || gamma.Y != 1.0f || gamma.Z != 1.0f))
            {
                gd.SetGammaRamp(false, SLAlgorithm.GetGamma(gamma));
            }
        }

        /// <summary>
        /// Set SLForm State
        /// </summary>
        /// <param name="isshowcursor"></param>
        /// <param name="isborder"></param>
        /// <param name="issizable"></param>
        public void SetSLForm(bool isshowcursor, bool isborder, bool issizable)
        {
            if (config == null)
            {
                config = new AssemblySettings(Assembly.GetAssembly(typeof(AssemblySettings)));
            }
            if (services == null)
            {
                services = new ServiceContainer();
                // Register the service, so components like ContentManager can find it.
                services.AddService<IGraphicsDeviceService>(this);
                cm = new ContentManager(services, config["content"]);
                // Hook the idle event to constantly redraw, getting a game style loop as default.
                Application.Idle += delegate { Invalidate(); };
                this.KeyDown += new KeyEventHandler(SLForm_KeyDown);
                this.MouseDown += new MouseEventHandler(SLForm_MouseDown);
                this.MouseMove += new MouseEventHandler(SLForm_MouseMove);
                this.MouseWheel += new MouseEventHandler(SLForm_MouseWheel);
            }
            // Cursor State
            if (!isshowcursor)
            {
                Cursor.Hide();
            }
            // Border and Sizable States
            if (isborder)
            {
                if (issizable)
                {
                    this.Resize += new EventHandler(SLForm_Resize);
                }
                else
                {
                    this.MaximizeBox = false;
                    this.FormBorderStyle = FormBorderStyle.FixedSingle;
                }
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.None;
            }
        }

        /// <summary>
        /// Toggles between full screen and windowed mode
        /// </summary>
        public void ToggleFullScreen()
        {
            pp.IsFullScreen = !pp.IsFullScreen;
            if (pp.IsFullScreen)
            {
                pp.FullScreenRefreshRateInHz = gd.DisplayMode.RefreshRate;
                pp.BackBufferWidth = gd.DisplayMode.Width;
                pp.BackBufferHeight = gd.DisplayMode.Height;
            }
            else
            {
                pp.FullScreenRefreshRateInHz = 0;
                pp.BackBufferWidth = 800;
                pp.BackBufferHeight = 600;
                this.ClientSize = new System.Drawing.Size(800, 600);
            }
            SetGraphicsDevice(pp);
        }

        /// <summary>
        /// Check if GraphicsDevice is ready
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
                    catch (Exception e)
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
        /// Disposes the SLForm, unload all contents.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (DeviceDisposing != null)
                    DeviceDisposing(this, EventArgs.Empty);
                UnloadContent();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Immediately Draw Tip String
        /// </summary>
        /// <param name="text"></param>
        /// <param name="bgcolor"></param>
        /// <param name="position"></param>
        /// <param name="tip"></param>
        /// <param name="tipcolor"></param>
        public void DrawTip(ref Text text, Color bgcolor, Vector2 position, string tip, Color tipcolor)
        {
            gd.Clear(bgcolor);
            text.Draw(position, tip, tipcolor);
            gd.Present();
        }

        /// <summary>
        /// Immediately Draw Tip String with Red color at position: (5, 5) in Screen Coordinate
        /// </summary>
        /// <param name="text"></param>
        /// <param name="bgcolor"></param>
        /// <param name="tip"></param>
        public void DrawTip(ref Text text, Color bgcolor, string tip)
        {
            DrawTip(ref text, bgcolor, new Vector2(5, 5), tip, Color.Red);
        }


        #region Custom Virtual Functions

        /// <summary>
        ///  Registered the SLForm Resize event handler
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
        /// Registered the SLForm KeyDown event handler
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
        /// Registered the SLForm MouseDown event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void SLForm_MouseDown(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// Registered the SLForm MouseMove event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void SLForm_MouseMove(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// Registered the SLForm MouseWheel event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void SLForm_MouseWheel(object sender, MouseEventArgs e)
        {
        }


        /// <summary>
        /// Derived classes override this to initialize their drawing code.
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// LoadContent() will be called before Initialize() and is the place to load all SLForm contents.
        /// </summary>
        protected virtual void LoadContent()
        {
        }

        /// <summary>
        /// UnloadContent() will be called once SLForm is to disposed, 
        /// and is the place to unload all SLForm non ContentManager contents.
        /// </summary>
        protected virtual void UnloadContent()
        {
        }

        /// <summary>
        /// Custom SLForm Contents Update
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
        /// Set Flow Control Ready to begin Experiment
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
