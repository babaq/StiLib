#region File Description
//-----------------------------------------------------------------------------
// SLCore.cs
//
// StiLib Core Services.
// Copyright (c) Zhang Li. 2009-02-22.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Windows.Forms;
using dnAnalytics.Statistics;
using System.Reflection;
using System.Xml;
using System.Configuration;
using System.Text;
#endregion

namespace StiLib.Core
{
    #region Core Class

    /// <summary>
    /// StiLib Randomization Service
    /// </summary>
    public class SLRandom
    {
        #region External C Library Functions

        /// <summary>
        /// Set Random Seed using C runtime library
        /// </summary>
        /// <param name="seed"></param>
        [DllImport("msvcrt")]
        public static extern void srand(uint seed);
        /// <summary>
        /// Get Random Integer using C runtime library
        /// </summary>
        /// <returns></returns>
        [DllImport("msvcrt")]
        public static extern int rand();

        #endregion

        int seed;
        int[] sequence;
        Random random;
        RandomMethod method;

        /// <summary>
        /// Seed to Generate Random Sequence
        /// </summary>
        public int Seed
        {
            get { return seed; }
            set
            {
                seed = value;
                random = new Random(value);
                srand((uint)value);
            }
        }

        /// <summary>
        /// Random Sequence
        /// </summary>
        public int[] Sequence
        {
            get { return sequence; }
            set { sequence = value; }
        }

        /// <summary>
        /// Random Method
        /// </summary>
        public RandomMethod RandomMethod
        {
            get { return method; }
            set { method = value; }
        }


        /// <summary>
        /// Init with default -- seed: time-dependent 0-199, random sequence length: 2000, random method: C runtime library method
        /// </summary>
        public SLRandom()
            : this(2000, RandomMethod.C)
        {
        }

        /// <summary>
        /// Init with default -- seed: time-dependent 0-199, random method: C runtime library method
        /// </summary>
        /// <param name="sequencelength"></param>
        public SLRandom(int sequencelength)
            : this(sequencelength, RandomMethod.C)
        {
        }

        /// <summary>
        /// Init with custom settings and default -- seed: time-dependent 0-199
        /// </summary>
        /// <param name="sequencelength"></param>
        /// <param name="method"></param>
        public SLRandom(int sequencelength, RandomMethod method)
        {
            Seed = GenerateRandomSeed(200);
            SetSequenceLength(sequencelength);
            this.method = method;
        }

        /// <summary>
        /// Init with custom settings
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="sequencelength"></param>
        /// <param name="method"></param>
        public SLRandom(int seed, int sequencelength, RandomMethod method)
        {
            Seed = seed;
            SetSequenceLength(sequencelength);
            this.method = method;
        }


        /// <summary>
        /// Set Random Sequence Length
        /// </summary>
        /// <param name="Length"></param>
        public void SetSequenceLength(int Length)
        {
            sequence = new int[Length];
        }

        /// <summary>
        /// Gets a time-dependent random seed in [0, maxseed)
        /// </summary>
        /// <param name="maxseed"></param>
        /// <returns></returns>
        public int GenerateRandomSeed(int maxseed)
        {
            Random rand = new Random();
            return rand.Next(maxseed);
        }

        /// <summary>
        /// Shuffle N length internal sequence using internal random method
        /// </summary>
        /// <param name="N"></param>
        public void RandomizeSequence(int N)
        {
            RandomizeSequence(method, N);
        }

        /// <summary>
        /// Shuffle N length internal sequence using custom random method
        /// </summary>
        /// <param name="method"></param>
        /// <param name="N"></param>
        public void RandomizeSequence(RandomMethod method, int N)
        {
            switch (method)
            {
                case RandomMethod.C:
                    method = RandomMethod.C;
                    Randomize_C(N);
                    break;
                case RandomMethod.DotNet:
                    method = RandomMethod.DotNet;
                    Randomize_NET(N);
                    break;
                case RandomMethod.None:
                    method = RandomMethod.None;
                    MessageBox.Show("No Random Method !", "Warning !");
                    break;
            }
        }

        /// <summary>
        /// Shuffle N length internal sequence using C Library rand() method
        /// </summary>
        /// <param name="N"></param>
        public void Randomize_C(int N)
        {
            int i, j;
            for (i = 0; i < N; i++)
            {
                sequence[i] = -1;
            }
            for (i = 0; i < N; i++)
            {
                do
                {
                    j = rand() % N;
                }
                while (sequence[j] >= 0);
                sequence[j] = i;
            }
        }

        /// <summary>
        /// Shuffle N length internal sequence using .NET Random Class
        /// </summary>
        /// <param name="N"></param>
        public void Randomize_NET(int N)
        {
            int i, j;
            for (i = 0; i < N; i++)
            {
                sequence[i] = -1;
            }
            for (i = 0; i < N; i++)
            {
                do
                {
                    j = random.Next(N);
                }
                while (sequence[j] >= 0);
                sequence[j] = i;
            }
        }

        /// <summary>
        /// Generate a shuffled N length sequence using C Library rand() method
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public int[] RandomSequence_C(int N)
        {
            int[] seq = new int[N];

            int i, j;
            for (i = 0; i < N; i++)
            {
                seq[i] = -1;
            }
            for (i = 0; i < N; i++)
            {
                do
                {
                    j = rand() % N;
                }
                while (seq[j] >= 0);
                seq[j] = i;
            }

            return seq;
        }

        /// <summary>
        /// Generate a shuffled N length sequence using .NET Random Class
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public int[] RandomSequence_NET(int N)
        {
            int[] seq = new int[N];

            int i, j;
            for (i = 0; i < N; i++)
            {
                seq[i] = -1;
            }
            for (i = 0; i < N; i++)
            {
                do
                {
                    j = random.Next(N);
                }
                while (seq[j] >= 0);
                seq[j] = i;
            }

            return seq;
        }

        /// <summary>
        /// Generate a ascending N length sequence
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public int[] AscendSequence(int N)
        {
            int[] seq = new int[N];

            int i;
            for (i = 0; i < N; i++)
            {
                seq[i] = i;
            }

            return seq;
        }

        /// <summary>
        /// Generate a descending N length sequence
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public int[] DescendSequence(int N)
        {
            int[] seq = new int[N];

            int i;
            for (i = 0; i < N; i++)
            {
                seq[i] = (N - 1) - i;
            }

            return seq;
        }

        /// <summary>
        /// Generate a sequence of random 3D position coordinates(-1:1, -1:1, -1:1)
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public Vector3[] RandomPosition(int N)
        {
            var pos = new Vector3[N];

            for (int i = 0; i < N; i++)
            {
                pos[i] = new Vector3();
                pos[i].X = (float)random.NextDouble() * 2 - 1;
                pos[i].Y = (float)random.NextDouble() * 2 - 1;
                pos[i].Z = (float)random.NextDouble() * 2 - 1;
            }

            return pos;
        }

    }

    /// <summary>
    /// StiLib Timing Service
    /// </summary>
    public class SLTimer : Stopwatch
    {
        /// <summary>
        /// Gets Total Seconds Elapsed since last Start()
        /// </summary>
        public double ElapsedSeconds
        {
            get { return Elapsed.TotalSeconds; }
        }

        /// <summary>
        /// Do nothing but Rest a Precise Time Interval
        /// </summary>
        /// <param name="restT">rest time in seconds</param>
        public void Rest(double restT)
        {
            if (!IsRunning)
            {
                Start();
            }
            double startT, endT;
            startT = Elapsed.TotalSeconds;
            endT = Elapsed.TotalSeconds;
            while ((endT - startT) < restT)
            {
                endT = Elapsed.TotalSeconds;
            }
        }

        /// <summary>
        /// Stop, Reset and Start Timing
        /// </summary>
        public void ReStart()
        {
            Reset();
            Start();
        }

    }

    /// <summary>
    /// Direct Access to I/O Port and Physical Memory Using WinIO Library
    /// </summary>
    public class SLIO: IDisposable
    {
        bool isWINIOinitialized;
        /// <summary>
        /// If WinIO Initialized
        /// </summary>
        public bool IsWinIOok
        {
            get { return isWINIOinitialized; }
        }


        /// <summary>
        /// Init WinIO Library
        /// </summary>
        public SLIO()
        {
            try
            {
                isWINIOinitialized = InitializeWinIo();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "WinIO Initialization Failed !");
            }
        }
        /// <summary>
        /// Shutdown WinIO Library
        /// </summary>
        ~SLIO()
        {
            Dispose(false);
        }


        #region WinIO Driver Functions

        /// <summary>
        /// The InitializeWinIo must be called first.
        /// </summary>
        /// <returns>true -- succeed, false -- failed</returns>
        [DllImport("WinIo.dll")]
        public static extern bool InitializeWinIo();
        /// <summary>
        /// The ShutdownWinIo must be called at end.
        /// </summary>
        [DllImport("WinIo.dll")]
        public static extern void ShutdownWinIo();
        /// <summary>
        /// Read 1, 2, 4 bytes from the specified I/O port
        /// </summary>
        /// <param name="wPortAddr">port address</param>
        /// <param name="pdwPortVal">Int32 Pointer to the received value</param>
        /// <param name="bSize">number of bytes to read</param>
        /// <returns>true -- succeed, false -- failed</returns>
        [DllImport("WinIo.dll")]
        public static extern bool GetPortVal(Int16 wPortAddr, out Int32 pdwPortVal, Byte bSize);
        /// <summary>
        /// Write 1, 2, 4 bytes to the specified I/O port
        /// </summary>
        /// <param name="wPortAddr">port address</param>
        /// <param name="dwPortVal">value to be written</param>
        /// <param name="bSize">number of bytes to write</param>
        /// <returns>true -- succeed, false -- failed</returns>
        [DllImport("WinIo.dll")]
        public static extern bool SetPortVal(Int16 wPortAddr, Int32 dwPortVal, Byte bSize);

        #endregion


        #region IDisposable Members

        /// <summary>
        /// Shutdown WinIO Library
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        /// <summary>
        /// Shutdown WinIO Library
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
            }
            if (isWINIOinitialized)
                ShutdownWinIo();
            isWINIOinitialized = false;
        }

        #endregion

    }

    /// <summary>
    /// SLIO Parallel Port and TTL Time Coding
    /// </summary>
    public class ParallelPort : SLIO
    {
        SLTimer timer;
        /// <summary>
        /// Timer
        /// </summary>
        public SLTimer Timer
        {
            get { return timer; }
        }
        /// <summary>
        /// Port Address
        /// </summary>
        public Int16 Port;
        /// <summary>
        /// Pin Number
        /// </summary>
        public int Pin;
        /// <summary>
        /// Pulse Time
        /// </summary>
        public double PulseTime;
        /// <summary>
        /// TTL Coding Time Base
        /// </summary>
        public double CodeTime;


        /// <summary>
        /// Init with default -- port address: 0x378, pin number: 6, pulsetime: 0.001(sec), codetime: 0.005(sec)
        /// </summary>
        public ParallelPort()
            : this((Int16)0x378, 6, 0.001, 0.005)
        {
        }

        /// <summary>
        /// Init with custom settings
        /// </summary>
        /// <param name="port"></param>
        /// <param name="pin"></param>
        /// <param name="pulsetime"></param>
        /// <param name="codetime"></param>
        public ParallelPort(short port, int pin, double pulsetime, double codetime)
        {
            this.Port = port;
            this.Pin = pin;
            this.PulseTime = pulsetime;
            this.CodeTime = codetime;
            timer = new SLTimer();
        }


        /// <summary>
        /// Set Parallel Port Pin State using internal port address and pin number
        /// </summary>
        /// <param name="state">high state: true, low state: false</param>
        /// <returns>true -- succeed, false -- failed</returns>
        public bool SetPinState(bool state)
        {
            return SetPinState(Port, Pin, state);
        }

        /// <summary>
        /// Set Parallel Port Pin State using internal port address
        /// </summary>
        /// <param name="pin">pin number (D0:D7 -- Pin2:Pin9)</param>
        /// <param name="state">high state: true, low state: false</param>
        /// <returns>true -- succeed, false -- failed</returns>
        public bool SetPinState(int pin, bool state)
        {
            return SetPinState(Port, pin, state);
        }

        /// <summary>
        /// Set Parallel Port Pin State
        /// </summary>
        /// <param name="port">port address</param>
        /// <param name="pin">pin number (D0:D7 -- Pin2:Pin9)</param>
        /// <param name="state">high state: true, low state: false</param>
        /// <returns>true -- succeed, false -- failed</returns>
        public bool SetPinState(short port, int pin, bool state)
        {
            if (!IsWinIOok)
                return false;

            int value;
            if (state)
            {
                value = (int)Math.Pow(2.0, pin - 2);
            }
            else
            {
                value = 0;
            }

            return SetPortVal(port, value, 1);
        }

        /// <summary>
        /// Get Parallel Port Pin State using internal port address and pin number
        /// </summary>
        /// <param name="state">high state: true, low state: false</param>
        /// <returns>true -- succeed, false -- failed</returns>
        public bool GetPinState(out bool state)
        {
            return GetPinState(Port, Pin, out state);
        }

        /// <summary>
        /// Get Parallel Port Pin State using internal port address
        /// </summary>
        /// <param name="pin">pin number (D0:D7 -- Pin2:Pin9)</param>
        /// <param name="state">high state: true, low state: false</param>
        /// <returns>true -- succeed, false -- failed</returns>
        public bool GetPinState(int pin, out bool state)
        {
            return GetPinState(Port, pin, out state);
        }

        /// <summary>
        /// Get Parallel Port Pin State
        /// </summary>
        /// <param name="port">port address</param>
        /// <param name="pin">pin number (D0:D7 -- Pin2:Pin9)</param>
        /// <param name="state">high state: true, low state: false</param>
        /// <returns>true -- succeed, false -- failed</returns>
        public bool GetPinState(short port, int pin, out bool state)
        {
            bool hr = false;
            state = false;

            if (!IsWinIOok)
                return hr;

            int value;
            hr = GetPortVal(port, out value, 1);
            string v = Convert.ToString(value, 2).PadLeft(8, '0');
            if (v[7 - (pin - 2)] == '1')
            {
                state = true;
            }

            return hr;
        }

        /// <summary>
        /// A 'pulsetime' TTL Pulse on internal port address and pin number
        /// </summary>
        /// <param name="pulsetime">Pulse Time (sec)</param>
        /// <returns>true -- succeed, false -- failed</returns>
        public bool Trigger(double pulsetime)
        {
            if (!SetPinState(true))
                return false;

            timer.Rest(pulsetime);

            return SetPinState(false);
        }

        /// <summary>
        /// A 'pulsetime' TTL Pulse on internal port address, pin number and pulse time
        /// </summary>
        /// <returns>true -- succeed, false -- failed</returns>
        public bool Trigger()
        {
            return Trigger(PulseTime);
        }

        /// <summary>
        /// Two Trigger's Interval to encode a number based on 'codetime'
        /// </summary>
        /// <param name="N"></param>
        /// <param name="codetime"></param>
        public void Marker(int N, double codetime)
        {
            Trigger();
            if (N == 0)
            {
                timer.Rest(codetime * 0.2);
            }
            else
            {
                timer.Rest(codetime * N);
            }
            Trigger();
        }

        /// <summary>
        /// Two Trigger's Interval to encode a number based on internal codetime
        /// </summary>
        /// <param name="N"></param>
        public void Marker(int N)
        {
            Marker(N, CodeTime);
        }

        /// <summary>
        /// Encode number in four Markers based on 16 number system which can encode 16×16×16×16=65536 numbers
        /// </summary>
        /// <param name="N"></param>
        public void MarkerEncode(int N)
        {
            // First Digit
            Marker(Convert.ToInt32(Math.Floor(N / 4096.0)));
            int t = N % 4096;
            timer.Rest(0.002);
            // Second Digit
            Marker(Convert.ToInt32(Math.Floor(t / 256.0)));
            t = t % 256;
            timer.Rest(0.002);
            // Third Digit
            Marker(Convert.ToInt32(Math.Floor(t / 16.0)));
            timer.Rest(0.002);
            // Fourth Digit
            Marker(t % 16);
            timer.Rest(0.002);
        }

        /// <summary>
        /// Encode(0, 0, 16, 0) to seperate different groups of keywords
        /// </summary>
        public void MarkerSeparatorEncode()
        {
            Marker(0);
            timer.Rest(0.002);
            Marker(0);
            timer.Rest(0.002);
            Marker(16);
            timer.Rest(0.002);
            Marker(0);
            timer.Rest(0.002);
        }

        /// <summary>
        /// Encode(0, 0, 0, 16) to end the MarkerEncode
        /// </summary>
        public void MarkerEndEncode()
        {
            Marker(0);
            timer.Rest(0.002);
            Marker(0);
            timer.Rest(0.002);
            Marker(0);
            timer.Rest(0.002);
            Marker(16);
            timer.Rest(0.002);
        }

        /// <summary>
        /// Transfer a Binary Stream according to internal codetime and pulsetime
        /// </summary>
        /// <param name="bins"></param>
        public void BinaryEncode(string bins)
        {
            // Start Flag(<CodeTime Pulse Interval)
            Trigger();
            timer.Rest(CodeTime * 0.2);
            Trigger();
            timer.Rest(CodeTime - PulseTime);
            // Binary Stream
            for (int i = 0; i < bins.Length; i++)
            {
                if (bins[i] == '1')
                {
                    Trigger();
                    timer.Rest(CodeTime - PulseTime);
                }
                else
                {
                    timer.Rest(CodeTime);
                }
            }
            // End Flag(<CodeTime Pulse Interval)
            Trigger();
            timer.Rest(CodeTime * 0.2);
            Trigger();
            timer.Rest(CodeTime - PulseTime);
        }

        /// <summary>
        /// Decode a Pulse Train to Binary Stream according to internal codetime
        /// </summary>
        /// <param name="plusetime"></param>
        /// <returns></returns>
        public string BinaryDecode(double[] plusetime)
        {
            string bins = "";
            int begin = 0;
            bool end = true;
            for (int i = 0; i < plusetime.Length - 1; i++)
            {
                var digit = (int)Math.Floor((plusetime[i + 1] - plusetime[i]) / CodeTime);
                if (digit == 0)
                {
                    begin = i;
                    end = !end;
                }
                if (i > begin && !end)
                {
                    bins += "1".PadLeft(digit, '0');
                }
            }
            return bins;
        }
    }

    /// <summary>
    /// StiLib Service Container
    /// </summary>
    public class ServiceContainer : IServiceProvider
    {
        Dictionary<Type, object> s = new Dictionary<Type, object>();


        /// <summary>
        /// Add a new service to the services collection
        /// </summary>
        public void AddService<T>(T service)
        {
            s.Add(typeof(T), service);
        }

        /// <summary>
        /// Looks up the specified service
        /// </summary>
        public object GetService(Type serviceType)
        {
            object service;

            s.TryGetValue(serviceType, out service);

            return service;
        }

    }

    /// <summary>
    /// Assembly Configurations
    /// </summary>
    public class AssemblySettings
    {
        IDictionary settings;
        /// <summary>
        /// Gets Internal Assembly Settings
        /// </summary>
        public IDictionary Settings
        {
            get { return settings; }
        }


        /// <summary>
        /// Init Calling Assembly Settings
        /// </summary>
        public AssemblySettings()
            : this(Assembly.GetCallingAssembly())
        {
        }

        /// <summary>
        /// Init Assembly Settings
        /// </summary>
        /// <param name="asm"></param>
        public AssemblySettings(Assembly asm)
        {
            settings = ReadConfig(asm);
        }

        /// <summary>
        /// Init Settings from Configuration File
        /// </summary>
        /// <param name="configfile"></param>
        public AssemblySettings(string configfile)
        {
            settings = ReadConfig(configfile);
        }


        /// <summary>
        /// Gets/Sets Setting Value According to Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key]
        {
            get
            {
                string settingValue = "";
                if (settings != null)
                {
                    settingValue = settings[key] as string;
                }
                return settingValue;
            }
            set
            {
                if (settings != null)
                {
                    settings[key] = value;
                }
            }
        }

        /// <summary>
        /// Get Calling Assembly Settings
        /// </summary>
        /// <returns></returns>
        public static IDictionary ReadConfig()
        {
            return ReadConfig(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Get Assembly Settings
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static IDictionary ReadConfig(Assembly asm)
        {
            string cfgFile = asm.CodeBase + ".config";
            return ReadConfig(cfgFile);
        }

        /// <summary>
        /// Get Settings from Configuration File
        /// </summary>
        /// <param name="configfile"></param>
        /// <returns></returns>
        public static IDictionary ReadConfig(string configfile)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(new XmlTextReader(configfile));
                XmlNodeList nodes = doc.GetElementsByTagName(SLConstant.AsmCfgNode);

                foreach (XmlNode node in nodes)
                {
                    if (node.LocalName == SLConstant.AsmCfgNode)
                    {
                        DictionarySectionHandler handler = new DictionarySectionHandler();
                        return (IDictionary)handler.Create(null, null, node);
                    }
                }
            }
            catch (Exception e)
            {
                SLConstant.ShowException(e);
            }

            return null;
        }

        /// <summary>
        /// Save Internal Settings to CallingAssembly Configuration File
        /// </summary>
        /// <returns></returns>
        public bool SaveAs()
        {
            return SaveConfig(settings);
        }

        /// <summary>
        /// Save Internal Settings to Assembly Configuration File
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public bool SaveAs(Assembly asm)
        {
            return SaveConfig(settings, asm);
        }

        /// <summary>
        /// Save Internal Settings to Configuration File
        /// </summary>
        /// <param name="configfile"></param>
        /// <returns></returns>
        public bool SaveAs(string configfile)
        {
            return SaveConfig(settings, configfile);
        }

        /// <summary>
        /// Save Custom Settings to CallingAssembly Configuration File
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static bool SaveConfig(IDictionary settings)
        {
            return SaveConfig(settings, Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Save Custom Settings to Assembly Configuration File
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static bool SaveConfig(IDictionary settings, Assembly asm)
        {
            string cfgFile = asm.CodeBase + ".config";
            return SaveConfig(settings, cfgFile);
        }

        /// <summary>
        /// Save Custom Settings to Configuration File
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="configfile"></param>
        /// <returns></returns>
        public static bool SaveConfig(IDictionary settings, string configfile)
        {
            bool hr = false;

            try
            {
                // Modify Existing Configuration File
                if (File.Exists(configfile))
                {
                    if (settings != null)
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(configfile);
                        XmlNode node = doc.DocumentElement.SelectSingleNode(SLConstant.AsmCfgNode);
                        bool iscfgexist = false;

                        foreach (string key in settings.Keys)
                        {
                            iscfgexist = false;
                            foreach (XmlNode subnode in node.ChildNodes)
                            {
                                if ((subnode.Name == "add") && (subnode.Attributes.GetNamedItem("key").Value == key))
                                {
                                    iscfgexist = true;
                                    subnode.Attributes.GetNamedItem("value").Value = settings[key] as string;
                                    break;
                                }
                            }
                            if (!iscfgexist)
                            {
                                XmlAttribute newkey = doc.CreateAttribute("key");
                                newkey.Value = key;
                                XmlAttribute newvalue = doc.CreateAttribute("value");
                                newvalue.Value = settings[key] as string;

                                XmlNode newnode = doc.CreateNode(XmlNodeType.Element, "add", "");
                                newnode.Attributes.Append(newkey);
                                newnode.Attributes.Append(newvalue);
                                node.AppendChild(newnode);
                            }
                        }

                        XmlTextWriter writer = new XmlTextWriter(configfile, Encoding.UTF8);
                        writer.Formatting = Formatting.Indented;
                        doc.PreserveWhitespace = true;
                        doc.Save(writer);

                        hr = true;
                    }
                    else
                    {
                        SLConstant.ShowMessage("No Configuration Settings to Save !");
                    }
                }
                else // Create New Configuration File
                {
                    if (settings != null)
                    {
                        XmlDocument doc = new XmlDocument();
                        XmlNode cfg = doc.CreateNode(XmlNodeType.Element, "configuration", "");
                        XmlNode asm = doc.CreateNode(XmlNodeType.Element, SLConstant.AsmCfgNode, "");

                        foreach (string key in settings.Keys)
                        {
                            XmlAttribute newkey = doc.CreateAttribute("key");
                            newkey.Value = key;
                            XmlAttribute newvalue = doc.CreateAttribute("value");
                            newvalue.Value = settings[key] as string;

                            XmlNode newnode = doc.CreateNode(XmlNodeType.Element, "add", "");
                            newnode.Attributes.Append(newkey);
                            newnode.Attributes.Append(newvalue);
                            asm.AppendChild(newnode);
                        }
                        cfg.AppendChild(asm);
                        doc.AppendChild(cfg);

                        XmlTextWriter writer = new XmlTextWriter(configfile, Encoding.UTF8);
                        writer.Formatting = Formatting.Indented;
                        doc.PreserveWhitespace = true;
                        doc.Save(writer);

                        hr = true;
                    }
                    else
                    {
                        SLConstant.ShowMessage("No Configuration Settings to Save !");
                    }
                }
            }
            catch (Exception e)
            {
                SLConstant.ShowException(e);
            }

            return hr;
        }

    }

    /// <summary>
    /// System Informations
    /// </summary>
    public class SystemInfo
    {
        Process currentprocess;

        /// <summary>
        /// Current Process Priority
        /// </summary>
        public ProcessPriorityClass ProcessPriority
        {
            get { return currentprocess.PriorityClass; }
            set { currentprocess.PriorityClass = value; }
        }

        /// <summary>
        /// Get Current Process ID
        /// </summary>
        public int ProcessID
        {
            get { return currentprocess.Id; }
        }

        /// <summary>
        /// Get Current Computer Name
        /// </summary>
        public string MachineName
        {
            get { return currentprocess.MachineName; }
        }

        /// <summary>
        /// Get current process main module
        /// </summary>
        public ProcessModule MainModule
        {
            get { return currentprocess.MainModule; }
        }

        /// <summary>
        /// Get current process name
        /// </summary>
        public string ProcessName
        {
            get { return currentprocess.ProcessName; }
        }


        /// <summary>
        /// Init
        /// </summary>
        public SystemInfo()
        {
            currentprocess = Process.GetCurrentProcess();
        }

    }

    /// <summary>
    /// Frame Informations
    /// </summary>
    public class FrameInfo
    {
        #region Fields

        Stopwatch timer;
        List<double> ifi;
        double framestamp;
        double fpsresettime;
        int fpsframecount;
        double fps;

        #endregion

        #region Properties

        /// <summary>
        /// Get Inter-Frame Interval List
        /// </summary>
        public List<double> IFI
        {
            get { return ifi; }
        }

        /// <summary>
        /// Get Total Frames Number
        /// </summary>
        public int TotalFrames
        {
            get { return ifi.Count; }
        }

        /// <summary>
        /// Get Total Frame Time
        /// </summary>
        public double TotalTime
        {
            get { return ifi.Sum(); }
        }

        /// <summary>
        /// Get Mean IFI
        /// </summary>
        public double MeanIFI
        {
            get { return ifi.Average(); }
        }

        /// <summary>
        /// Get Standard Deviation of IFIs
        /// </summary>
        public double StdIFI
        {
            get { return ifi.StandardDeviation(); }
        }

        /// <summary>
        /// Get Maximum IFI
        /// </summary>
        public double MaxIFI
        {
            get { return ifi.Max(); }
        }

        /// <summary>
        /// Get Minimum IFI
        /// </summary>
        public double MinIFI
        {
            get { return ifi.Min(); }
        }

        /// <summary>
        /// Get Instant Frame Rate per Second
        /// </summary>
        public double InstantFPS
        {
            get
            {
                if (ifi.Count == 0)
                {
                    return 0.0;
                }
                else
                {
                    return Math.Round(1 / ifi[ifi.Count - 1], 3);
                }
            }
        }

        /// <summary>
        /// Get Frame Rate per Second
        /// </summary>
        public double FPS
        {
            get
            {
                return Math.Round(fps, 1);
            }
        }

        #endregion


        /// <summary>
        /// Init
        /// </summary>
        public FrameInfo()
        {
            timer = new Stopwatch();
            ifi = new List<double>();
        }


        /// <summary>
        /// Update Frame Information
        /// </summary>
        public void Update()
        {
            if (!timer.IsRunning)
            {
                timer.Start();
            }

            if (framestamp == 0)
            {
                framestamp = timer.Elapsed.TotalSeconds;
            }
            else
            {
                double temp = timer.Elapsed.TotalSeconds;
                ifi.Add(temp - framestamp);
                framestamp = temp;

                temp = framestamp - fpsresettime;
                fpsframecount += 1;
                if (temp >= 1.0)
                {
                    fps = fpsframecount / temp;
                    fpsresettime = framestamp;
                    fpsframecount = 0;
                }
            }
        }

        /// <summary>
        /// Clear all frame info and reset to begin state
        /// </summary>
        public void Clear()
        {
            timer.Reset();
            ifi.Clear();
            framestamp = 0.0;
            fpsresettime = 0.0;
            fpsframecount = 0;
        }

    }

    #endregion

    #region Parameter Structure

    /// <summary>
    /// Experiment Design Parameters
    /// </summary>
    public struct ExDesign
    {
        /// <summary>
        /// Init with custom experiment design
        /// </summary>
        /// <param name="extype">Experiment Types</param>
        /// <param name="expara">Experiment Conditions</param>
        /// <param name="cond">Conditions Interpolation Parameters</param>
        /// <param name="block">Experiment Block Number</param>
        /// <param name="trial">Experiment Trial Number</param>
        /// <param name="stimuli">Size of Each Stimulus Set</param>
        /// <param name="brestT">Rest Time Between Blocks</param>
        /// <param name="trestT">Rest Time Between Trials</param>
        /// <param name="srestT">Rest Time Between Stimulus</param>
        /// <param name="preT"> Pre-Stimulus Time</param>
        /// <param name="durT">Stimulus Duration Time</param>
        /// <param name="posT"> Post-Stimulus Time</param>
        /// <param name="bgcolor">Experiment BackGround Color</param>
        public ExDesign(ExType[] extype, ExPara[] expara, SLInterpolation[] cond, int block, int trial, int[] stimuli, float brestT, float trestT, float srestT, float preT, float durT, float posT, Color bgcolor)
        {
            this.exType = extype;
            this.exPara = expara;
            this.condition = cond;
            this.block = block;
            this.trial = trial;
            this.stimuli = stimuli;
            this.brestT = brestT;
            this.trestT = trestT;
            this.srestT = srestT;
            this.preT = preT;
            this.durT = durT;
            this.posT = posT;
            this.bgcolor = bgcolor;
        }

        /// <summary>
        /// Set custom experiment design parameters
        /// </summary>
        /// <param name="extype">Experiment Types</param>
        /// <param name="expara">Experiment Conditions</param>
        /// <param name="cond">Conditions Interpolation Parameters</param>
        /// <param name="block">Experiment Block Number</param>
        /// <param name="trial">Experiment Trial Number</param>
        /// <param name="stimuli">Size of Each Stimulus Set</param>
        /// <param name="brestT">Rest Time Between Blocks</param>
        /// <param name="trestT">Rest Time Between Trials</param>
        /// <param name="srestT">Rest Time Between Stimulus</param>
        /// <param name="preT"> Pre-Stimulus Time</param>
        /// <param name="durT">Stimulus Duration Time</param>
        /// <param name="posT"> Post-Stimulus Time</param>
        /// <param name="bgcolor">Experiment BackGround Color</param>
        public void SetPara(ExType[] extype, ExPara[] expara, SLInterpolation[] cond, int block, int trial, int[] stimuli, float brestT, float trestT, float srestT, float preT, float durT, float posT, Color bgcolor)
        {
            this.exType = extype;
            this.exPara = expara;
            this.condition = cond;
            this.block = block;
            this.trial = trial;
            this.stimuli = stimuli;
            this.brestT = brestT;
            this.trestT = trestT;
            this.srestT = srestT;
            this.preT = preT;
            this.durT = durT;
            this.posT = posT;
            this.bgcolor = bgcolor;
        }

        /// <summary>
        /// Get default experiment design -- all set to None/Zero
        /// </summary>
        /// <param name="n">multiple number of extype and condition</param>
        /// <returns></returns>
        public static ExDesign Default(int n)
        {
            ExType[] exType = new ExType[n];
            ExPara[] exPara = new ExPara[n];
            SLInterpolation[] condition = new SLInterpolation[n];
            int[] stimuli = new int[n];
            for (int i = 0; i < n; i++)
            {
                exType[i] = ExType.None;
                exPara[i] = ExPara.None;
                condition[i] = SLInterpolation.Default(ExPara.None, 4);
                stimuli[i] = 0;
            }

            return new ExDesign(exType, exPara, condition, 0, 0, stimuli, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, Color.Black);
        }


        /// <summary>
        /// Experiment Types
        /// </summary>
        public ExType[] exType;
        /// <summary>
        /// Experiment Conditions
        /// </summary>
        public ExPara[] exPara;
        /// <summary>
        /// Experiment Conditions Interpolation Parameters
        /// </summary>
        public SLInterpolation[] condition;
        /// <summary>
        /// Experiment Blocks
        /// </summary>
        public int block;
        /// <summary>
        /// Experiment Trials
        /// </summary>
        public int trial;
        /// <summary>
        /// Number of Element of Each Stimulus Set
        /// </summary>
        public int[] stimuli;
        /// <summary>
        /// Rest Time Between Blocks
        /// </summary>
        public float brestT;
        /// <summary>
        /// Rest Time Between Trials
        /// </summary>
        public float trestT;
        /// <summary>
        /// Rest Time Between Stimulus
        /// </summary>
        public float srestT;
        /// <summary>
        /// Stimulus Pre-Duration Time
        /// </summary>
        public float preT;
        /// <summary>
        /// Stimulus Duration Time
        /// </summary>
        public float durT;
        /// <summary>
        /// Stimulus Post-Duration Time
        /// </summary>
        public float posT;
        /// <summary>
        /// Experiment BackGround Color
        /// </summary>
        public Color bgcolor;
    }

    /// <summary>
    /// Experiment Flow Control Parameters
    /// </summary>
    public struct FlowControl
    {
        /// <summary>
        /// If Stimulus Begins Drawing
        /// </summary>
        public bool IsStiOn
        { get; set; }
        /// <summary>
        /// If Stimulus Ends Drawing
        /// </summary>
        public bool IsStiOff
        { get; set; }
        /// <summary>
        /// If Pre-Stimulus Operation Has Done
        /// </summary>
        public bool IsPred
        { get; set; }
        /// <summary>
        /// If Rest Operation Has Done
        /// </summary>
        public bool IsRested
        { get; set; }
        /// <summary>
        /// If Blank Operation Has Done
        /// </summary>
        public bool IsBlanked
        { get; set; }
        /// <summary>
        /// PreTime + DurTime
        /// </summary>
        public float PreDurTime
        { get; set; }
        /// <summary>
        /// Total Time of a Stimulus
        /// </summary>
        public float StiTime
        { get; set; }
        /// <summary>
        /// Stimulus Lasting Time
        /// </summary>
        public double LastingTime
        { get; set; }
        /// <summary>
        /// Counter of Block
        /// </summary>
        public int BlockCount
        { get; set; }
        /// <summary>
        /// Counter of Trial
        /// </summary>
        public int TrialCount
        { get; set; }
        /// <summary>
        /// Counter of Stimulus
        /// </summary>
        public int StiCount
        { get; set; }
        /// <summary>
        /// Counter of Rows
        /// </summary>
        public int RowCount
        { get; set; }
        /// <summary>
        /// Counter of Columns
        /// </summary>
        public int ColumnCount
        { get; set; }
        /// <summary>
        /// Counter of Slices
        /// </summary>
        public int SliceCount
        { get; set; }
        /// <summary>
        /// Rotate Matrix
        /// </summary>
        public Matrix Rotate
        { get; set; }
        /// <summary>
        /// Orientation Rotate Matrix
        /// </summary>
        public Matrix RotateOri
        { get; set; }
        /// <summary>
        /// Direction Rotate Matrix
        /// </summary>
        public Matrix RotateDir
        { get; set; }
        /// <summary>
        /// Scale Matrix
        /// </summary>
        public Matrix Scale
        { get; set; }
        /// <summary>
        /// Translate Matrix
        /// </summary>
        public Matrix Translate
        { get; set; }
        /// <summary>
        /// Center Translate Matrix
        /// </summary>
        public Matrix TranslateCenter
        { get; set; }
        /// <summary>
        /// Experiment Information
        /// </summary>
        public string Info
        { get; set; }
        /// <summary>
        /// Location of Stimulus
        /// </summary>
        public Vector3 Location
        { get; set; }
        /// <summary>
        /// Step of Conditions
        /// </summary>
        public float[] CondStep
        { get; set; }
        /// <summary>
        /// Current Orientation
        /// </summary>
        public float Orientation
        { get; set; }
        /// <summary>
        /// Current Direction
        /// </summary>
        public float Direction
        { get; set; }
    }

    /// <summary>
    /// StiLib Two Keys/One Value Pair Structure
    /// </summary>
    public struct SLKeyValuePair<PrimaryKey, SecondaryKey, Value>
    {
        PrimaryKey pKey;
        SecondaryKey sKey;
        Value val;


        /// <summary>
        /// Create a new 2Keys/1Value Pair
        /// </summary>
        /// <param name="pK"></param>
        /// <param name="sK"></param>
        /// <param name="v"></param>
        public SLKeyValuePair(PrimaryKey pK, SecondaryKey sK, Value v)
        {
            pKey = pK;
            sKey = sK;
            val = v;
        }


        /// <summary>
        /// Primary Key
        /// </summary>
        public PrimaryKey PKEY
        {
            get { return pKey; }
            set { pKey = value; }
        }

        /// <summary>
        /// Secondary Key
        /// </summary>
        public SecondaryKey SKEY
        {
            get { return sKey; }
            set { sKey = value; }
        }

        /// <summary>
        /// Value
        /// </summary>
        public Value VALUE
        {
            get { return val; }
            set { val = value; }
        }

        /// <summary>
        /// Returns a string representation of the SLKeyValuePair.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "[" + pKey.ToString() + ", " + sKey.ToString() + ", " + val.ToString() + "]";
        }

    }

    /// <summary>
    /// Interpolation Parameters
    /// </summary>
    public struct SLInterpolation
    {
        /// <summary>
        /// Init with Custom Settings
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="n"></param>
        /// <param name="method"></param>
        public SLInterpolation(float start, float end, int n, Interpolation method)
        {
            StartValue = start;
            EndValue = end;
            ValueN = n;
            Method = method;
        }

        /// <summary>
        /// Set Interpolation Parameters
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="n"></param>
        /// <param name="method"></param>
        public void SetPara(float start, float end, int n, Interpolation method)
        {
            StartValue = start;
            EndValue = end;
            ValueN = n;
            Method = method;
        }

        /// <summary>
        /// Get Default SLInterpolation according to Pre-Definded Experiment Parameters
        /// </summary>
        /// <param name="expara"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static SLInterpolation Default(ExPara expara, int n)
        {
            switch (expara)
            {
                case ExPara.Orientation:
                    return new SLInterpolation(0.0f, 180.0f, n, Interpolation.Linear);
                case ExPara.Speed:
                    return new SLInterpolation(0.0f, 50.0f, n, Interpolation.Linear);
                case ExPara.Luminance:
                    return new SLInterpolation(0.0f, 0.5f, n, Interpolation.Linear);
                case ExPara.Contrast:
                    return new SLInterpolation(0.0f, 1.0f, n, Interpolation.Linear);
                case ExPara.SpatialFreq:
                    return new SLInterpolation(0.1f, 3.2f, n, Interpolation.Log2);
                case ExPara.SpatialPhase:
                    return new SLInterpolation(0.0f, 1.0f, n, Interpolation.Linear);
                case ExPara.TemporalFreq:
                    return new SLInterpolation(1.0f, 32.0f, n, Interpolation.Log2);
                case ExPara.TemporalPhase:
                    return new SLInterpolation(0.0f, 1.0f, n, Interpolation.Linear);
                case ExPara.Color:
                    return new SLInterpolation(0.0f, 1.0f, n, Interpolation.Linear);
                case ExPara.Disparity:
                    return new SLInterpolation(-1.0f, 1.0f, n, Interpolation.Linear);
                case ExPara.Size:
                    return new SLInterpolation(0.5f, 20.0f, n, Interpolation.Linear);
                default:
                    return new SLInterpolation(0.0f, 360.0f, n, Interpolation.Linear);
            }
        }

        /// <summary>
        /// Generate a Interpolation Sequence according to Internal Parameters
        /// </summary>
        /// <returns></returns>
        public float[] Interpolate()
        {
            return SLAlgorithm.Interpolate(StartValue, EndValue, ValueN, Method);
        }


        /// <summary>
        /// Interpolation Start Value
        /// </summary>
        public float StartValue;
        /// <summary>
        /// Interpolation End Value
        /// </summary>
        public float EndValue;
        /// <summary>
        /// Number of Interpolation
        /// </summary>
        public int ValueN;
        /// <summary>
        /// Interpolation Method
        /// </summary>
        public Interpolation Method;
    }

    #endregion

    #region Type Enum

    /// <summary>
    /// Pre-Defined Experiment Types
    /// </summary>
    public enum ExType
    {
        /// <summary>
        /// No Type
        /// </summary>
        None,
        /// <summary>
        /// Drifting Bar
        /// </summary>
        dBar,
        /// <summary>
        /// Drifting Bar RF Mapping
        /// </summary>
        RF_dBar,
        /// <summary>
        /// Flashing Bar
        /// </summary>
        fBar,
        /// <summary>
        /// Flashing Bar RF Mapping
        /// </summary>
        RF_fBar,
        /// <summary>
        /// Drifting Grating
        /// </summary>
        dGrating,
        /// <summary>
        /// Standing Grating
        /// </summary>
        fGrating,
        /// <summary>
        /// Plaid is composed of two gratings with angle between their directions
        /// </summary>
        Plaid,
        /// <summary>
        /// Randomly Distributated Bars Moving at certain or different directions
        /// </summary>
        RandomLine,
        /// <summary>
        /// Local Motion Visual Field Optic Flow
        /// </summary>
        OpticFlow,
        /// <summary>
        /// Two Drifting Bars with angle between their directions
        /// </summary>
        Two_dBar,
        /// <summary>
        /// Two Drifting Gratings with angle between their directions
        /// </summary>
        Two_dGrating,
        /// <summary>
        /// Two Flashing Bars with angle between their orientations
        /// </summary>
        Two_fBar,
        /// <summary>
        /// Two Standing Gratings with angle between their orientations
        /// </summary>
        Two_fGrating,
        /// <summary>
        /// Contextual Effect
        /// </summary>
        Context,
        /// <summary>
        /// Receptive Field
        /// </summary>
        RF
    }

    /// <summary>
    /// Pre-Defined Experiment Parameters
    /// </summary>
    public enum ExPara
    {
        /// <summary>
        /// No Condition
        /// </summary>
        None,
        /// <summary>
        /// Orientation Condition
        /// </summary>
        Orientation,
        /// <summary>
        /// Direction Condition
        /// </summary>
        Direction,
        /// <summary>
        /// Speed Condition
        /// </summary>
        Speed,
        /// <summary>
        /// Uniform/Average Luminance Condition
        /// </summary>
        Luminance,
        /// <summary>
        /// Uniform/Average Contrast Condition
        /// </summary>
        Contrast,
        /// <summary>
        /// Spatial Frequency Condition
        /// </summary>
        SpatialFreq,
        /// <summary>
        /// Spatial Phase Condition
        /// </summary>
        SpatialPhase,
        /// <summary>
        /// Temporal Frequency Condition
        /// </summary>
        TemporalFreq,
        /// <summary>
        /// Temporal Phase Condition
        /// </summary>
        TemporalPhase,
        /// <summary>
        /// Color Condition
        /// </summary>
        Color,
        /// <summary>
        /// Disparity Condition
        /// </summary>
        Disparity,
        /// <summary>
        /// Size Condition
        /// </summary>
        Size,
        /// <summary>
        /// Location Condition
        /// </summary>
        Location
    }

    /// <summary>
    /// Pre-Defined Method to Generate Random Number
    /// </summary>
    public enum RandomMethod
    {
        /// <summary>
        /// No Method
        /// </summary>
        None,
        /// <summary>
        /// Using C Runtime Library(msvcrt.dll) Functions -- srand() and rand()
        /// </summary>
        C,
        /// <summary>
        /// .NET Random Class
        /// </summary>
        DotNet
    }

    /// <summary>
    /// Pre-Defined Interpolation Method
    /// </summary>
    public enum Interpolation
    {
        /// <summary>
        /// Linear Interpolation(0, 1, 2, ...)
        /// </summary>
        Linear,
        /// <summary>
        /// Base 2 Log Interpolation(2^0, 2^1, 2^2, ...)
        /// </summary>
        Log2,
        /// <summary>
        /// Base 10 Log Interpolation(10^0, 10^1, 10^2, ...)
        /// </summary>
        Log10
    }

    #endregion

    #region Static Method

    /// <summary>
    /// StiLib Constants
    /// </summary>
    public static class SLConstant
    {
        /// <summary>
        /// Standard Help Tip String
        /// </summary>
        public const string Help = "[Esc] - Quit\n[Space] - Run/Stop";

        /// <summary>
        /// Standard MarkHead Tip String
        /// </summary>
        public const string MarkHead = "Transfering Marker Header . . .";

        /// <summary>
        /// Standard Assembly Settings Root Node Name
        /// </summary>
        public const string AsmCfgNode = "assemblySettings";

        /// <summary>
        /// Radian per Degree
        /// </summary>
        public const double Rad_p_Deg = Math.PI / 180.0;

        /// <summary>
        /// Degree per Radian
        /// </summary>
        public const double Deg_p_Rad = 180.0 / Math.PI;

        /// <summary>
        /// MilliMeter per Inch
        /// </summary>
        public const double MM_p_Inch = 25.4;

        /// <summary>
        /// General Display Dot Pitch(mm)
        /// </summary>
        public const double DotPitch = 0.22;

        /// <summary>
        /// Size of Float Vector4
        /// </summary>
        public const int SizeOfVector4 = sizeof(float) * 4;

        /// <summary>
        /// Size of 4*4 Float Matrix
        /// </summary>
        public const int SizeOfMatrix = sizeof(float) * 16;

        /// <summary>
        /// Display a message that describes the exception.
        /// </summary>
        /// <param name="e"></param>
        public static void ShowException(Exception e)
        {
            MessageBox.Show("Module: " + e.Source +
                                        ".\nMethod: " + e.TargetSite.Name +
                                        ".\nException: " + e.Message, "Exception !");
        }

        /// <summary>
        /// Display Custom Message
        /// </summary>
        /// <param name="msg"></param>
        public static void ShowMessage(string msg)
        {
            MessageBox.Show(msg, "Message !");
        }

    }

    /// <summary>
    /// StiLib Common Algorithm
    /// </summary>
    public static class SLAlgorithm
    {
        /// <summary>
        /// Product of Array Elements
        /// </summary>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static int Prod(int[] source, int start, int end)
        {
            int prod = 1;
            for (int i = start; i <= end; i++)
            {
                prod *= source[i];
            }
            return prod;
        }

        /// <summary>
        /// Get a Sequence by Interpolating from A to B
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="n"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static float[] Interpolate(float start, float end, int n, Interpolation method)
        {
            float[] sequence = new float[n];
            switch (method)
            {
                default: // Linear
                    float step = (end - start) / n;
                    for (int i = 0; i < n; i++)
                    {
                        sequence[i] = start + i * step;
                    }
                    return sequence;
                case Interpolation.Log2:
                    for (int i = 0; i < n; i++)
                    {
                        sequence[i] = start * (float)Math.Pow(2.0, i);
                    }
                    return sequence;
                case Interpolation.Log10:
                    for (int i = 0; i < n; i++)
                    {
                        sequence[i] = start * (float)Math.Pow(10.0, i);
                    }
                    return sequence;
            }
        }

        /// <summary>
        /// Map a Sequence to Orthogonal Table Index
        /// </summary>
        /// <param name="ortho">size of each orthogonal table dimentions</param>
        /// <returns></returns>
        public static int[][] OrthoTable(int[] ortho)
        {
            int n = Prod(ortho, 0, ortho.Length - 1);
            int[][] table = new int[n][];
            int subn, subprod;
            for (int i = 0; i < n; i++)
            {
                int[] index = new int[ortho.Length];
                index[0] = i;
                for (int j = 0; j < ortho.Length - 1; j++)
                {
                    subprod = Prod(ortho, j + 1, ortho.Length - 1);
                    subn = index[j];
                    index[j] = (int)Math.Floor((double)subn / subprod);
                    index[j + 1] = subn % subprod;
                }
                table[i] = index;
            }
            return table;
        }

        /// <summary>
        /// Gets GammaRamp for Linearization According to Current Gamma Values
        /// </summary>
        /// <param name="gamma"></param>
        /// <returns></returns>
        public static GammaRamp GetGamma(Vector3 gamma)
        {
            GammaRamp gr = new GammaRamp();
            short[] ramp = new short[256];
            double g = 1.0 / gamma.X;
            for (int i = 0; i < 256; i++)
            {
                ramp[i] = unchecked((short)Math.Round((Math.Pow((i / 255.0), g) * 65535)));
            }
            gr.SetRed(ramp);

            if (gamma.Y != gamma.X)
            {
                g = 1.0 / gamma.Y;
                for (int i = 0; i < 256; i++)
                {
                    ramp[i] = unchecked((short)Math.Round((Math.Pow((i / 255.0), g) * 65535)));
                }
            }
            gr.SetGreen(ramp);

            if (gamma.Z != gamma.Y)
            {
                g = 1.0 / gamma.Z;
                for (int i = 0; i < 256; i++)
                {
                    ramp[i] = unchecked((short)Math.Round((Math.Pow((i / 255.0), g) * 65535)));
                }
            }
            gr.SetBlue(ramp);

            return gr;
        }

    }

    #endregion

}
