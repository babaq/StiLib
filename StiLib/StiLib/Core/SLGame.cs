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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;
#endregion

namespace StiLib.Core
{
    /// <summary>
    /// StiLib game service
    /// </summary>
    public class SLGame : Game
    {
        #region Fields

        SLInput input;
        SLFreeCamera freecamera;
        GraphicsDeviceManager gdm;
        bool go_over;
        int bbwidth, bbheight, refreshrate;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Default FreeCamera
        /// </summary>
        public SLFreeCamera FCamera
        {
            get { return freecamera; }
        }

        /// <summary>
        /// Gets the GraphicsDeviceManager.
        /// </summary>
        public GraphicsDeviceManager GDManager
        {
            get { return gdm; }
        }

        /// <summary>
        /// Gets the SLInput
        /// </summary>
        public SLInput Input
        {
            get { return input; }
        }

        /// <summary>
        /// Toggle Between Content Run(true) and RunOver(false) State
        /// </summary>
        public bool GO_OVER
        {
            get { return go_over; }
            set { go_over = value; }
        }

        #endregion


        /// <summary>
        /// Init to Default -- width:800, height:600, refreshrate:0, Vsync:true, updaterate:120, ismousevisible:false
        /// </summary>
        public SLGame() : this(800, 600, 0, true, 120, false) { }

        /// <summary>
        /// Init SLGame
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="refreshrate"></param>
        /// <param name="isvsync"></param>
        /// <param name="updaterate"></param>
        /// <param name="ismousevisible"></param>
        public SLGame(int width, int height, int refreshrate, bool isvsync, int updaterate, bool ismousevisible)
        {
            gdm = new GraphicsDeviceManager(this);
            input = new SLInput();
            freecamera = new SLFreeCamera();

            gdm.SynchronizeWithVerticalRetrace = isvsync;
            this.IsMouseVisible = ismousevisible;
            Content.RootDirectory = "Content";

            if (updaterate > 0)
            {
                this.IsFixedTimeStep = true;
                this.TargetElapsedTime = TimeSpan.FromSeconds(1.0 / updaterate);
            }
            else
            {
                this.IsFixedTimeStep = false;
            }
            
            this.bbwidth = width;
            this.bbheight = height;
            this.refreshrate = refreshrate;
            gdm.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(gdm_PreparingDeviceSettings);
        }

        /// <summary>
        /// Prepare the settings for creation of graphicsdevice
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void gdm_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            GraphicsDeviceCapabilities gdcap = gdm.GraphicsDevice.GraphicsDeviceCapabilities;
            if (gdcap.MaxPixelShaderProfile < ShaderProfile.PS_2_0 || gdcap.MaxVertexShaderProfile < ShaderProfile.VS_2_0)
            {
                System.Diagnostics.Debug.WriteLine("This Adapter does not support Shader Model 2.0.");
                MessageBox.Show("This Adapter does not support Shader Model 2.0.", "Warning !");
            }

            int quality;
            if (GraphicsAdapter.DefaultAdapter.CheckDeviceMultiSampleType(DeviceType.Hardware, SurfaceFormat.Color, false, MultiSampleType.NonMaskable, out quality))
            {
                e.GraphicsDeviceInformation.PresentationParameters.MultiSampleType = MultiSampleType.NonMaskable;
                if (quality < 2)
                {
                    e.GraphicsDeviceInformation.PresentationParameters.MultiSampleQuality = quality;
                }
                else
                {
                    e.GraphicsDeviceInformation.PresentationParameters.MultiSampleQuality = 2;
                }
            }

            e.GraphicsDeviceInformation.DeviceType = DeviceType.Hardware;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferCount = 3;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferFormat = SurfaceFormat.Color;
            e.GraphicsDeviceInformation.PresentationParameters.EnableAutoDepthStencil = true;
            e.GraphicsDeviceInformation.PresentationParameters.AutoDepthStencilFormat = DepthFormat.Depth24Stencil8;
            e.GraphicsDeviceInformation.PresentationParameters.PresentationInterval = PresentInterval.One;

            e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight = bbheight;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth = bbwidth;
            if (refreshrate > 0)
            {
                e.GraphicsDeviceInformation.PresentationParameters.IsFullScreen = true;
                e.GraphicsDeviceInformation.PresentationParameters.FullScreenRefreshRateInHz = refreshrate;
            }
            else
            {
                e.GraphicsDeviceInformation.PresentationParameters.IsFullScreen = false;
                e.GraphicsDeviceInformation.PresentationParameters.FullScreenRefreshRateInHz = 0;
            }
        }


        /// <summary>
        /// Set Flow Controller to begin a new Experiment
        /// </summary>
        protected virtual void SetFlowControl()
        {
        }

        /// <summary>
        /// Transfer Experiment MarkerHeader only once before it's running
        /// </summary>
        protected virtual void MarkerHeader()
        {
        }

    }
}