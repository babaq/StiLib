#region File Description
//-----------------------------------------------------------------------------
// SLGDService.cs
//
// StiLib GraphicsDevice Service.
// Copyright (c) Zhang Li. 2008-7-31.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;
#endregion

namespace StiLib.Core
{
    /// <summary>
    /// The GraphicsDeviceService is responsible for creating and managing the GraphicsDevice.
    /// All GraphicsDeviceService consumer instances share the same GraphicsDeviceService.
    /// This class implements the standard IGraphicsDeviceService interface, 
    /// which provides notification events for when the device is reset or disposed.
    /// </summary>
    public class SLGDService : IGraphicsDeviceService
    {
        #region Fields

        // Singleton device service instance.
        static SLGDService singletonInstance;
        // Keep track of how many consumers are sharing the singletonInstance.
        static int referenceCount;
        // Store the current device settings.
        PresentationParameters pp;
        GraphicsDevice gd;

        // IGraphicsDeviceService events.
        /// <summary>
        /// DeviceCreated EventHandler
        /// </summary>
        public event EventHandler DeviceCreated;
        /// <summary>
        /// DeviceDisposing EventHandler
        /// </summary>
        public event EventHandler DeviceDisposing;
        /// <summary>
        /// DeviceReset EventHandler
        /// </summary>
        public event EventHandler DeviceReset;
        /// <summary>
        /// DeviceResetting EventHandler
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

        #endregion


        /// <summary>
        /// Constructor is private, because this is a singleton class.
        /// Client should use the public AddRef method instead.
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        SLGDService(IntPtr windowHandle, int width, int height)
        {
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

            pp.PresentationInterval = PresentInterval.One;
            pp.BackBufferCount = 1;
            pp.BackBufferWidth = Math.Max(width, 1);
            pp.BackBufferHeight = Math.Max(height, 1);
            pp.BackBufferFormat = SurfaceFormat.Color;
            pp.EnableAutoDepthStencil = true;
            pp.AutoDepthStencilFormat = DepthFormat.Depth24;

            try
            {
                gd = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, DeviceType.Hardware, windowHandle, pp);

                if (DeviceCreated != null)
                    DeviceCreated(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "GraphicsDevice Initialization Failed !");
            }
        }


        /// <summary>
        /// Gets a reference to the singleton instance.
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static SLGDService AddRef(IntPtr windowHandle, int width, int height)
        {
            // Increment the "how many client sharing the device" reference count.
            if (Interlocked.Increment(ref referenceCount) == 1)
            {
                // If this is the first client to start using the device, we must create the singleton instance.
                singletonInstance = new SLGDService(windowHandle, width, height);
            }

            return singletonInstance;
        }

        /// <summary>
        /// Releases a reference to the singleton instance.
        /// </summary>
        /// <param name="disposing"></param>
        public void Release(bool disposing)
        {
            // Decrement the "how many client sharing the device" reference count.
            if (Interlocked.Decrement(ref referenceCount) == 0)
            {
                // If this is the last client to finish using the device, we should dispose the singleton instance.
                if (disposing)
                {
                    if (DeviceDisposing != null)
                        DeviceDisposing(this, EventArgs.Empty);

                    gd.Dispose();
                }
                gd = null;
            }
        }

        /// <summary>
        /// Resets the graphics device to whichever is bigger out of the specified
        /// resolution or its current size. This behavior means the device will
        /// demand-grow to the largest of all its clients.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void ResetDevice(int width, int height)
        {
            if (DeviceResetting != null)
                DeviceResetting(this, EventArgs.Empty);

            pp.BackBufferWidth = Math.Max(pp.BackBufferWidth, width);
            pp.BackBufferHeight = Math.Max(pp.BackBufferHeight, height);

            gd.Reset(pp);

            if (DeviceReset != null)
                DeviceReset(this, EventArgs.Empty);
        }

    }
}
