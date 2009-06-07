#region File Description
//-----------------------------------------------------------------------------
// SLCalib.cs
//
// StiLib Calibration Service
// Copyright (c) Zhang Li. 2009-02-12.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
#endregion

namespace StiLib.Core
{
    /// <summary>
    /// Cambridge Research System Calibration Device
    /// </summary>
    public class CRSCalDevice
    {
        CalDevice deviceType;
        int deviceHandle;

        /// <summary>
        /// calibration device type
        /// </summary>
        public CalDevice DeviceType
        {
            get { return deviceType; }
        }

        /// <summary>
        /// calibration device handle -- 0:OK, 1:FAIL
        /// </summary>
        public int DeviceHandle
        {
            get { return deviceHandle; }
        }


        /// <summary>
        /// Create a CRS calibration device
        /// </summary>
        /// <param name="devicetype"></param>
        public CRSCalDevice(CalDevice devicetype)
        {
            deviceType = devicetype;
            deviceHandle = 1;
        }

        /// <summary>
        /// Init and calibrate current device. 
        /// This function must be called before any of the others.
        /// </summary>
        /// <returns></returns>
        public int Init()
        {
            deviceHandle = calInitialise((int)deviceType);
            return deviceHandle;
        }

        /// <summary>
        /// Close communication with the device. 
        /// Call this procedure before closing your program
        /// </summary>
        public int Close()
        {
            int hresult = calCloseDevice();
            if (hresult == 0)
            {
                deviceHandle = 1;
            }
            else
            {
                deviceHandle = 0;
            }
            return hresult;
        }


        /// <summary>
        /// Read a luminance value in cd/m2 from device
        /// To convert this to fL, divide by 3.426259101
        /// </summary>
        public double ReadLuminance
        {
            get
            {
                double[] temp = new double[1];
                calReadLuminance(temp);
                return temp[0];
            }
        }

        /// <summary>
        /// Read a voltage (in Volts) value from the device.
        /// To convert this to mV, Multiply by 1000
        /// </summary>
        public double ReadVoltage
        {
            get
            {
                double[] temp = new double[1];
                calReadVoltage(temp);
                return temp[0];
            }
        }

        /// <summary>
        /// Read a colour in CIE x,y,l from the device.
        /// </summary>
        /// <param name="CieX"></param>
        /// <param name="CieY"></param>
        /// <param name="CieLum"></param>
        public void ReadColor(out double CieX, out double CieY, out double CieLum)
        {
            double[] x = new double[1];
            double[] y = new double[1];
            double[] l = new double[1];
            calReadColour(x, y, l);
            CieX = x[0];
            CieY = y[0];
            CieLum = l[0];
        }


        #region Cambridge Research System (CRS) Device Functions

        /// <summary>
        /// Initialise the ColorCAL. This function must be called before any of the others.
        /// The returned value is a handle to the device.  Use this when processing multiple devices
        /// </summary>
        /// <param name="Device"></param>
        /// <returns></returns>
        [DllImport("Calibrator.dll", EntryPoint = "calInitialise")]
        public static extern int calInitialise(int Device);

        /// <summary>
        /// Read a luminance value in cd/m2 from device
        /// To convert this to fL, divide by 3.426259101
        /// </summary>
        /// <param name="Luminance"></param>
        /// <returns></returns>
        [DllImport("Calibrator.dll", EntryPoint = "calReadLuminance")]
        public static extern int calReadLuminance(double[] Luminance);

        /// <summary>
        /// Read a voltage (in Volts) value from the device.
        /// To convert this to mV, Multiply by 1000
        /// </summary>
        /// <param name="Voltage"></param>
        /// <returns></returns>
        [DllImport("Calibrator.dll", EntryPoint = "calReadVoltage")]
        public static extern int calReadVoltage(double[] Voltage);

        /// <summary>
        /// Read a colour in CIE x,y,l from the device.
        /// Returns CALIB_NOTSUPPORTED(4) and (0,0,luminance) if colour feature is not supported by the device
        /// </summary>
        /// <param name="CieX"></param>
        /// <param name="CieY"></param>
        /// <param name="CieLum"></param>
        /// <returns></returns>
        [DllImport("Calibrator.dll", EntryPoint = "calReadColour")]
        public static extern int calReadColour(double[] CieX, double[] CieY, double[] CieLum);

        /// <summary>
        /// Close communication with the device.
        /// Call this procedure before closing your program.
        /// </summary>
        /// <returns></returns>
        [DllImport("Calibrator.dll", EntryPoint = "calCloseDevice")]
        public static extern int calCloseDevice();

        /// <summary>
        /// Call this to perform a auto calibration of the device.
        /// This is performed automatically during calInitialise.
        /// The procedure may differ depending on the device type.
        /// </summary>
        /// <returns></returns>
        [DllImport("Calibrator.dll", EntryPoint = "calAutoCalibrate")]
        public static extern int calAutoCalibrate();

        #endregion
    }

    /// <summary>
    /// Calibration Device Type
    /// </summary>
    public enum CalDevice
    {
        /// <summary>
        /// No Device
        /// </summary>
        None,
        /// <summary>
        /// OptiCal_COM1 photometer of Cambridge Research System, Ltd.
        /// </summary>
        OptiCal_COM1,
        /// <summary>
        /// OptiCal_COM2 photometer of Cambridge Research System, Ltd.
        /// </summary>
        OptiCal_COM2,
        /// <summary>
        /// OptiCal_COM3 photometer of Cambridge Research System, Ltd.
        /// </summary>
        OptiCal_COM3,
        /// <summary>
        /// OptiCal_COM4 photometer of Cambridge Research System, Ltd.
        /// </summary>
        OptiCal_COM4,
        /// <summary>
        /// OptiCal_USB photometer of Cambridge Research System, Ltd.
        /// </summary>
        OptiCal_USB,
        /// <summary>
        /// ColorCal_USB Colorimeter of Cambridge Research System, Ltd.
        /// </summary>
        ColorCal_USB,
    }
}
