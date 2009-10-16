#region File Description
//-----------------------------------------------------------------------------
// SLGame.cs
//
// StiLib Game Service
// Copyright (c) Zhang Li. 2008-8-3.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;
#endregion

namespace StiLib.Core
{
    /// <summary>
    /// StiLib Game Service
    /// </summary>
    public class SLGame : Game
    {
        #region Fields

        GraphicsDeviceManager gdm;
        PresentationParameters pp;
        AssemblySettings config;
        SLInput input;
        SLFreeCamera freecamera;
        bool Go_Over;
        Vector3 gamma;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the GraphicsDeviceManager.
        /// </summary>
        public GraphicsDeviceManager GDManager
        {
            get { return gdm; }
        }

        /// <summary>
        /// Gets the Internal GraphicsDevice Presentation Parameters
        /// </summary>
        public PresentationParameters PresentPara
        {
            get { return pp; }
        }

        /// <summary>
        /// Gets Current StiLib Configurations
        /// </summary>
        public AssemblySettings SLConfig
        {
            get { return config; }
        }

        /// <summary>
        /// Gets the SLInput
        /// </summary>
        public SLInput Input
        {
            get { return input; }
        }

        /// <summary>
        /// Gets the Default FreeCamera
        /// </summary>
        public SLFreeCamera FreeCamera
        {
            get { return freecamera; }
        }

        /// <summary>
        /// Toggle Between Content Run(true) and RunOver(false) State
        /// </summary>
        public bool GO_OVER
        {
            get { return Go_Over; }
            set { Go_Over = value; }
        }

        /// <summary>
        /// Internal Current Gamma Values
        /// </summary>
        public Vector3 Gamma
        {
            get { return gamma; }
            set { gamma = value; }
        }

        #endregion


        /// <summary>
        /// Init to Default -- buffercount: 1, width: 800, height: 600, refreshrate: 0, updaterate: 0, vsync: false, showcursor: true, sizable: false, gamma: (1.0, 1.0, 1.0)
        /// </summary>
        public SLGame()
            : this(1, 800, 600, 0, 0, false, true, false, Vector3.One)
        {
        }

        /// <summary>
        /// Init Using StiLib Configuration File
        /// </summary>
        /// <param name="configfile">empty/null to load default StiLib.dll.config file, otherwise indicate full file path</param>
        public SLGame(string configfile)
        {
            if (string.IsNullOrEmpty(configfile))
            {
                config = new AssemblySettings(Assembly.GetAssembly(typeof(AssemblySettings)));
            }
            else
            {
                config = new AssemblySettings(configfile);
            }

            SetSLGame(Convert.ToBoolean(config["isshowcursor"]), Convert.ToBoolean(config["issizable"]));
            SetGraphicsDevice(Convert.ToInt32(config["buffercount"]), Convert.ToInt32(config["width"]), Convert.ToInt32(config["height"]),
                                         Convert.ToInt32(config["refreshrate"]), Convert.ToInt32(config["updaterate"]), Convert.ToBoolean(config["isvsync"]));
            this.gamma = new Vector3(Convert.ToSingle(config["gammaR"]), Convert.ToSingle(config["gammaG"]), Convert.ToSingle(config["gammaB"]));
        }

        /// <summary>
        /// Initializes the SLGame with default -- buffercount: 1, sizable: false, gamma: (1.0, 1.0, 1.0)
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="refreshrate">windowed mode(0), fullscreen mode(>0)</param>
        /// <param name="updaterate">Variable-Step Game Loop(0), Fixed-Step Game Loop(>0)</param>
        /// <param name="isvsync"></param>
        /// <param name="isshowcursor"></param>
        public SLGame(int width, int height, int refreshrate, int updaterate, bool isvsync, bool isshowcursor)
            : this(1, width, height, refreshrate, updaterate, isvsync, isshowcursor, false, Vector3.One)
        {
        }

        /// <summary>
        /// Gets SLGame Ready for Run() to Create GraphicsDevice and Call Initialize() and LoadContent()
        /// </summary>
        /// <param name="buffercount">0-3</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="refreshrate">windowed mode(0), fullscreen mode(>0)</param>
        /// <param name="updaterate">Variable-Step Game Loop(0), Fixed-Step Game Loop(>0)</param>
        /// <param name="isvsync"></param>
        /// <param name="isshowcursor"></param>
        /// <param name="issizable"></param>
        /// <param name="gamma"></param>
        public SLGame(int buffercount, int width, int height, int refreshrate, int updaterate, bool isvsync, bool isshowcursor, bool issizable, Vector3 gamma)
        {
            SetSLGame(isshowcursor, issizable);
            SetGraphicsDevice(buffercount, width, height, refreshrate, updaterate, isvsync);
            this.gamma = gamma;
        }


        /// <summary>
        /// Set SLGame State
        /// </summary>
        /// <param name="isshowcursor"></param>
        /// <param name="issizable"></param>
        public void SetSLGame(bool isshowcursor, bool issizable)
        {
            if (config == null)
            {
                config = new AssemblySettings(Assembly.GetAssembly(typeof(AssemblySettings)));
            }
            if (gdm == null)
            {
                gdm = new GraphicsDeviceManager(this);
                pp = new PresentationParameters();
                input = new SLInput();
                freecamera = new SLFreeCamera();
                Content.RootDirectory = config["content"];
                gdm.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(gdm_PreparingDeviceSettings);
            }
            this.IsMouseVisible = isshowcursor;
            this.Window.AllowUserResizing = issizable;
        }

        /// <summary>
        /// Set Default Hardware Adapter with Custom Settings
        /// </summary>
        /// <param name="buffercount">0-3</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="refreshrate">windowed mode(0), fullscreen mode(>0)</param>
        /// <param name="updaterate">Variable-Step Game Loop(0), Fixed-Step Game Loop(>0)</param>
        /// <param name="isvsync"></param>
        public void SetGraphicsDevice(int buffercount, int width, int height, int refreshrate, int updaterate, bool isvsync)
        {
            GraphicsDeviceCapabilities gdcap = GraphicsAdapter.DefaultAdapter.GetCapabilities(DeviceType.Hardware);
            if (gdcap.MaxPixelShaderProfile < ShaderProfile.PS_2_0 || gdcap.MaxVertexShaderProfile < ShaderProfile.VS_2_0)
            {
                MessageBox.Show("This Adapter Does Not Support Shader Model 2.0.", "Warning !");
            }

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

            if (refreshrate > 0)
            {
                pp.IsFullScreen = true;
            }
            else
            {
                pp.IsFullScreen = false;
            }
            pp.FullScreenRefreshRateInHz = refreshrate;
            gdm.IsFullScreen = pp.IsFullScreen;

            if (updaterate > 0)
            {
                this.IsFixedTimeStep = true;
                this.TargetElapsedTime = TimeSpan.FromSeconds(1.0 / updaterate);
            }
            else
            {
                this.IsFixedTimeStep = false;
            }

            if (isvsync)
            {
                pp.PresentationInterval = PresentInterval.One;
            }
            else
            {
                pp.PresentationInterval = PresentInterval.Immediate;
            }
            gdm.SynchronizeWithVerticalRetrace = isvsync;

            pp.BackBufferCount = buffercount;
            pp.BackBufferHeight = Math.Max(1, height);
            pp.BackBufferWidth = Math.Max(1, width);
            pp.BackBufferFormat = SurfaceFormat.Color;
            pp.EnableAutoDepthStencil = true;
            pp.AutoDepthStencilFormat = DepthFormat.Depth24Stencil8;

            // Reset GraphicsDevice
            if (gdm.GraphicsDevice != null)
            {
                // Dirty GraphicsDeviceManager to reset using custom presentationparameters through PreparingDeviceSettings()
                gdm.PreferMultiSampling = !gdm.PreferMultiSampling;
                gdm.ApplyChanges();
            }
        }

        /// <summary>
        /// Prepare the settings for reset or recreation of graphicsdevice
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void gdm_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.Adapter = GraphicsAdapter.DefaultAdapter;
            e.GraphicsDeviceInformation.DeviceType = DeviceType.Hardware;
            e.GraphicsDeviceInformation.PresentationParameters = pp;
        }

        /// <summary>
        /// Linearize Gamma According to Current Gamma Value Using GraphicsDevice GammaRamp
        /// </summary>
        /// <param name="gamma">current R, G, B gamma value</param>
        public void SetGamma(Vector3 gamma)
        {
            bool isfullscreengamma;
            GraphicsDeviceCapabilities gdcap = GraphicsAdapter.DefaultAdapter.GetCapabilities(DeviceType.Hardware);
            if (gdcap.DriverCapabilities.SupportsFullScreenGamma)
            {
                isfullscreengamma = true;
            }
            else
            {
                isfullscreengamma = false;
                SLConstant.ShowMessage("This Adapter Does Not Support Full Screen Gamma Correction !");
            }
            if (isfullscreengamma && gdm.IsFullScreen && (gamma.X != 1.0f || gamma.Y != 1.0f || gamma.Z != 1.0f))
            {
                gdm.GraphicsDevice.SetGammaRamp(false, SLAlgorithm.GetGamma(gamma));
                this.gamma = gamma;
            }
        }

        /// <summary>
        /// SetGamma According to Internal Current Gamma Values
        /// </summary>
        public void SetGamma()
        {
            SetGamma(gamma);
        }

        /// <summary>
        /// Disposes the SLGame, unload all contents.
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

        /// <summary>
        /// Called After GraphicsDevice Has Created to Set Gamma Correction Ramp, 
        /// Derived Class Should Call This Method to Set GammaRamp.
        /// </summary>
        protected override void Initialize()
        {
            SetGamma();
            base.Initialize();
        }

        /// <summary>
        /// Toggles between full screen and windowed mode
        /// </summary>
        public void ToggleFullScreen()
        {
            pp.IsFullScreen = !pp.IsFullScreen;
            gdm.IsFullScreen = pp.IsFullScreen;
            if (pp.IsFullScreen)
            {
                pp.FullScreenRefreshRateInHz = gdm.GraphicsDevice.DisplayMode.RefreshRate;
                pp.BackBufferWidth = gdm.GraphicsDevice.DisplayMode.Width;
                pp.BackBufferHeight = gdm.GraphicsDevice.DisplayMode.Height;
            }
            else
            {
                pp.FullScreenRefreshRateInHz = 0;
                pp.BackBufferWidth = 800;
                pp.BackBufferHeight = 600;
            }
            gdm.ApplyChanges();
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

    }
}