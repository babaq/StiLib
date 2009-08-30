#region File Description
//-----------------------------------------------------------------------------
// SLCOM.cs
//
// StiLib COM Port Service
// Copyright (c) Zhang Li. 2009-06-28.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
#endregion

namespace StiLib.Core
{
    /// <summary>
    /// StiLib Serial COM Port Service
    /// </summary>
    public class SLCOM
    {
        #region Fields

        static bool isCOMExist;
        static string[] avaliblePorts;
        /// <summary>
        /// Serial Port
        /// </summary>
        public SerialPort Port;

        SerialDataReceivedEventHandler DataReceivedEventHandler;
        SerialErrorReceivedEventHandler ErrorReceivedEventHandler;
        SerialPinChangedEventHandler PinChangedEventHandler;

        string newReceivedData;
        int receivedDataLength;

        #endregion

        #region Properties

        /// <summary>
        /// If any COM ports found on current computer
        /// </summary>
        public bool IsCOMExist
        {
            get { return isCOMExist; }
        }

        /// <summary>
        /// All currently avalible COM ports on current computer
        /// </summary>
        public string[] AvaliblePorts
        {
            get { return avaliblePorts; }
        }

        /// <summary>
        /// Received data when DataReceived event fires
        /// </summary>
        public string NewReceivedData
        {
            get { return newReceivedData; }
        }

        /// <summary>
        /// Total received data length
        /// </summary>
        public int ReceivedDataLength
        {
            get { return receivedDataLength; }
        }

        #endregion


        /// <summary>
        /// Init, need to OpenCOM()
        /// </summary>
        public SLCOM()
        {
            FindCOM();
            Port = new SerialPort();
            DataReceivedEventHandler = new SerialDataReceivedEventHandler(DataReceived);
            ErrorReceivedEventHandler = new SerialErrorReceivedEventHandler(ErrorReceived);
            PinChangedEventHandler = new SerialPinChangedEventHandler(PinChanged);
        }

        /// <summary>
        /// Init and open a port with default parameters
        /// </summary>
        /// <param name="portName"></param>
        public SLCOM(string portName)
            : this()
        {
            OpenCOM(portName);
        }

        /// <summary>
        /// Init and open a port with custom parameters
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="readTimeout">0: InfiniteTimeout(ms)</param>
        /// <param name="writeTimeout">0: InfiniteTimeout(ms)</param>
        /// <param name="baudRate"></param>
        /// <param name="parity"></param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        /// <param name="handShake"></param>
        public SLCOM(string portName, int readTimeout, int writeTimeout, int baudRate, Parity parity, int dataBits, StopBits stopBits, Handshake handShake)
            : this()
        {
            OpenCOM(portName, readTimeout, writeTimeout, baudRate, parity, dataBits, stopBits, handShake);
        }


        /// <summary>
        /// Find current computer's all avalible COM ports
        /// </summary>
        public static void FindCOM()
        {
            avaliblePorts = SerialPort.GetPortNames();
            if (avaliblePorts.Length > 0)
            {
                isCOMExist = true;
                Array.Sort(avaliblePorts);
            }
            else
            {
                isCOMExist = false;
                MessageBox.Show("No COM Ports Found On This Computer !");
            }
        }

        /// <summary>
        /// Open a COM port with default -- readTimeout: 0ms, writeTimeout: 0ms, baudRate: 9600, parity: None, dataBits: 8, stopBits: One, handShake: None
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        public bool OpenCOM(string portName)
        {
            return OpenCOM(portName, 0, 0, 9600, Parity.None, 8, StopBits.One, Handshake.None);
        }

        /// <summary>
        /// Open a COM port with custom parameters
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="readTimeout">0: InfiniteTimeout(ms)</param>
        /// <param name="writeTimeout">0: InfiniteTimeout(ms)</param>
        /// <param name="baudRate"></param>
        /// <param name="parity"></param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        /// <param name="handShake"></param>
        /// <returns></returns>
        public bool OpenCOM(string portName, int readTimeout, int writeTimeout, int baudRate, Parity parity, int dataBits, StopBits stopBits, Handshake handShake)
        {
            bool hr = false;

            try
            {
                if (isCOMExist)
                {
                    if (!Port.IsOpen)
                    {
                        if (readTimeout > 0)
                        {
                            Port.ReadTimeout = readTimeout;
                        }
                        else
                        {
                            Port.ReadTimeout = SerialPort.InfiniteTimeout;
                        }
                        if (writeTimeout > 0)
                        {
                            Port.WriteTimeout = writeTimeout;
                        }
                        else
                        {
                            Port.WriteTimeout = SerialPort.InfiniteTimeout;
                        }

                        Port.PortName = portName;
                        Port.BaudRate = baudRate;
                        Port.Parity = parity;
                        Port.DataBits = dataBits;
                        Port.StopBits = stopBits;
                        Port.Handshake = handShake;

                        Port.DataReceived += DataReceivedEventHandler;
                        Port.ErrorReceived += ErrorReceivedEventHandler;
                        Port.PinChanged += PinChangedEventHandler;

                        Port.Open();
                        hr = true;
                    }
                }
            }
            catch (Exception e)
            {
                SLConstant.ShowException(e);
            }

            return hr;
        }

        /// <summary>
        /// Close current port
        /// </summary>
        public void CloseCOM()
        {
            try
            {
                if (Port.IsOpen)
                {
                    Port.Close();
                }
            }
            catch (Exception e)
            {
                SLConstant.ShowException(e);
            }
        }

        /// <summary>
        /// Read COM port
        /// </summary>
        /// <returns></returns>
        public string ReadCOM()
        {
            int count = Port.BytesToRead;
            byte[] msg = new byte[count];
            string MSG = "";
            try
            {
                if (!Port.IsOpen)
                {
                    OpenCOM(Port.PortName);
                }

                if (Port.IsOpen)
                {
                    Port.Read(msg, 0, count);
                    Port.DiscardInBuffer();
                    MSG = Port.Encoding.GetString(msg);
                }
            }
            catch (Exception e)
            {
                SLConstant.ShowException(e);
            }

            return MSG;
        }

        /// <summary>
        /// Write COM port
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool WriteCOM(string text)
        {
            bool success = false;
            try
            {
                if (!Port.IsOpen)
                {
                    OpenCOM(Port.PortName);
                }

                if (Port.IsOpen)
                {
                    Port.WriteLine(text);
                    Port.DiscardOutBuffer();
                    success = true;
                }
            }
            catch (Exception e)
            {
                SLConstant.ShowException(e);
            }

            return success;
        }

        /// <summary>
        /// SerialPinChanged EventHandler Delegate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void PinChanged(object sender, SerialPinChangedEventArgs e)
        {
        }

        /// <summary>
        /// SerialErrorReceived EventHandler Delegate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            switch (e.EventType)
            {
                case SerialError.Frame:
                    MessageBox.Show("Framing Error !");
                    break;
                case SerialError.Overrun:
                    MessageBox.Show("Character Buffer Overrun !");
                    break;
                case SerialError.RXOver:
                    MessageBox.Show("Input Buffer Overflow !");
                    break;
                case SerialError.RXParity:
                    MessageBox.Show("Parity Error !");
                    break;
                case SerialError.TXFull:
                    MessageBox.Show("Output Buffer Full !");
                    break;
            }
        }

        /// <summary>
        /// SerialDataReceived EventHandler Delegate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            newReceivedData = "";
            try
            {
                newReceivedData = Port.ReadExisting();
                receivedDataLength += newReceivedData.Length;
            }
            catch (Exception ex)
            {
                SLConstant.ShowException(ex);
            }
        }

    }
}
