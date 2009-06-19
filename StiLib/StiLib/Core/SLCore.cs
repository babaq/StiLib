#region File Description
//-----------------------------------------------------------------------------
// SLCore.cs
//
// StiLib Core Service
// Copyright (c) Zhang Li. 2009-02-22.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
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
#endregion

namespace StiLib.Core
{
    /// <summary>
    /// StiLib Two Keys/One Value Pair Structure
    /// </summary>
    public struct SLKeyValuePair<primaryKey, SecondaryKey, Value>
    {
        primaryKey pKey;
        SecondaryKey sKey;
        Value val;


        /// <summary>
        /// Create a new 2Keys/1Value Pair
        /// </summary>
        /// <param name="pK"></param>
        /// <param name="sK"></param>
        /// <param name="v"></param>
        public SLKeyValuePair(primaryKey pK, SecondaryKey sK, Value v)
        {
            pKey = pK;
            sKey = sK;
            val = v;
        }

        /// <summary>
        /// Primary Key
        /// </summary>
        public primaryKey PKEY
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

        /// <summary>
        /// Seed to Generate Random Sequence
        /// </summary>
        public int RSeed;
        /// <summary>
        /// Random Sequence
        /// </summary>
        public int[] RSequence;
        Random random;
        RandomMethod method;

        /// <summary>
        /// Get Current Random Method
        /// </summary>
        public RandomMethod RandomMethod
        {
            get { return method; }
        }

        /// <summary>
        /// Init to default random sequence length: 2000
        /// </summary>
        public SLRandom()
            : this(2000)
        {
        }

        /// <summary>
        /// Init with custom Random Sequence Length
        /// </summary>
        /// <param name="length"></param>
        public SLRandom(int length)
        {
            SetSequenceLength(length);
            method = RandomMethod.None;
        }


        /// <summary>
        /// Set Random Sequence Length
        /// </summary>
        /// <param name="Length"></param>
        public void SetSequenceLength(int Length)
        {
            RSequence = new int[Length];
        }

        /// <summary>
        /// Get time-dependent randomized seed(0-199) to generate random sequence
        /// </summary>
        public void RandomizeSeed()
        {
            Random rand = new Random();
            RSeed = rand.Next(200);

            random = new Random(RSeed);
            srand((uint)RSeed);
        }

        /// <summary>
        /// Use RSeed and C Library rand() method to generate a shuffled 0-N sequence
        /// </summary>
        /// <param name="N"></param>
        public void RandomizeSequence(int N)
        {
            RandomizeSequence(RandomMethod.C, N);
        }

        /// <summary>
        /// Use RSeed and custom method to generate a shuffled 0-N sequence
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
                case RandomMethod.dotNET:
                    method = RandomMethod.dotNET;
                    Randomize_NET(N);
                    break;
                case RandomMethod.None:
                    method = RandomMethod.None;
                    MessageBox.Show("Randomize Seed First !", "Warning !");
                    break;
            }
        }

        /// <summary>
        /// generate a shuffled 0-N sequence using C Library rand() method
        /// </summary>
        /// <param name="N"></param>
        public void Randomize_C(int N)
        {
            int i, j;
            for (i = 0; i < N; i++)
            {
                RSequence[i] = -1;
            }
            for (i = 0; i < N; i++)
            {
                do
                {
                    j = rand() % N;
                }
                while (RSequence[j] >= 0);
                RSequence[j] = i;
            }
        }

        /// <summary>
        /// generate a shuffled 0-N sequence using .NET Random Class
        /// </summary>
        /// <param name="N"></param>
        public void Randomize_NET(int N)
        {
            int i, j;
            for (i = 0; i < N; i++)
            {
                RSequence[i] = -1;
            }
            for (i = 0; i < N; i++)
            {
                do
                {
                    j = random.Next(N);
                }
                while (RSequence[j] >= 0);
                RSequence[j] = i;
            }
        }

    }

    /// <summary>
    /// StiLib Timing Service
    /// </summary>
    public class SLTimer : Stopwatch
    {
        /// <summary>
        /// Get Total Seconds Elapsed since last Start()
        /// </summary>
        public double ElapsedSeconds
        {
            get { return Elapsed.TotalSeconds; }
        }

        /// <summary>
        /// Do nothing but Rest a Precise Time Interval
        /// </summary>
        /// <param name="restT">rest time in second</param>
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
        /// Stop, Reset and Start
        /// </summary>
        public void ReStart()
        {
            Reset();
            Start();
        }
    }

    /// <summary>
    /// Direct Access to I/O Port and Physical Memory Using WinIo Library
    /// </summary>
    public class SLIO
    {
        bool isWINIOinitialized;

        /// <summary>
        /// If WinIO OK
        /// </summary>
        public bool IsWinIOok
        {
            get { return isWINIOinitialized; }
        }

        #region WinIO Driver Functions

        /// <summary>
        /// The InitializeWinIo must be called first
        /// </summary>
        /// <returns>true -- succeed, false -- failed</returns>
        [DllImport("WinIo.dll")]
        public static extern bool InitializeWinIo();
        /// <summary>
        /// The ShutdownWinIo must be called at end
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


        /// <summary>
        /// Init WinIo Library
        /// </summary>
        public SLIO()
        {
            try
            {
                isWINIOinitialized = InitializeWinIo();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString(), "WinIO Initialization Failed !");
            }
        }

        /// <summary>
        /// Shutdown WinIo
        /// </summary>
        ~SLIO()
        {
            if (isWINIOinitialized)
                ShutdownWinIo();
        }

        /// <summary>
        /// IO Port TTL
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool TTL(Int16 port)
        {
            if (!isWINIOinitialized)
                return false;

            return SetPortVal(port, 0x10, 1);
        }

    }

    /// <summary>
    /// SLIO Parallel Port and TTL Time Coding
    /// </summary>
    public class ParallelPort : SLIO
    {
        /// <summary>
        /// Timer
        /// </summary>
        public SLTimer timer;


        /// <summary>
        /// Init
        /// </summary>
        public ParallelPort()
        {
            timer = new SLTimer();
        }

        /// <summary>
        /// A 'time' ms TTL Pulse on 'port'
        /// </summary>
        /// <param name="port"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool Trigger(short port, double time)
        {
            if (!IsWinIOok)
                return false;

            if (!SetPortVal(port, 0x10, 1))
                return false;

            timer.Rest(time);

            return SetPortVal(port, 0x00, 1);
        }

        /// <summary>
        /// A 1ms TTL Pulse on parallel port 0x378
        /// </summary>
        /// <returns></returns>
        public bool Trigger()
        {
            return Trigger((Int16)0x378, 0.001);
        }

        /// <summary>
        /// Two Trigger to encode a number based on 5ms interval
        /// </summary>
        /// <param name="N"></param>
        public void Marker(int N)
        {
            Trigger();
            if (N == 0)
            {
                timer.Rest(0.001);
            }
            else
            {
                timer.Rest(0.005 * N);
            }
            Trigger();
        }

        /// <summary>
        /// Encode number in four Markers based on 16 number system which can encode 16×16×16×16=65536 numbers
        /// </summary>
        /// <param name="N"></param>
        public void MarkerEncode(int N)
        {
            // First Digit
            Marker((int)Math.Floor(N / 4096.0));
            int t = N % 4096;
            timer.Rest(0.002);
            // Second Digit
            Marker((int)Math.Floor(t / 256.0));
            t = t % 256;
            timer.Rest(0.002);
            // Third Digit
            Marker((int)Math.Floor(t / 16.0));
            timer.Rest(0.002);
            // Fourth Digit
            Marker((int)(t % 16));
            timer.Rest(0.002);
        }

        /// <summary>
        /// Encode(0, 0, 16, 0) seperate different groups of keywords
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
        /// Encode(0, 0, 0, 16) the end of MarkerEncode
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
        /// Transfer a Binary Stream
        /// </summary>
        /// <param name="bins"></param>
        public void BinaryEncode(string bins)
        {
            // Start Flag(<5ms Pulse Interval)
            Trigger();
            timer.Rest(0.001);
            Trigger();
            timer.Rest(0.004);
            // Binary Stream
            for (int i = 0; i < bins.Length; i++)
            {
                if (bins[i] == '1')
                {
                    Trigger();
                    timer.Rest(0.004);
                }
                else
                {
                    timer.Rest(0.005);
                }
            }
            // End Flag(<5ms Pulse Interval)
            Trigger();
            timer.Rest(0.001);
            Trigger();
            timer.Rest(0.004);
        }

        /// <summary>
        /// Decode a Pulse Train to Binary Stream
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
                var digit = (int)Math.Floor((plusetime[i + 1] - plusetime[i]) / 0.005);
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
    /// Experiment Flow Control
    /// </summary>
    public struct FlowControl
    {
        /// <summary>
        /// If To Begin a Stimulus
        /// </summary>
        public bool IsStiOn
        { get; set; }
        /// <summary>
        /// If Pre-Stimulus Operation has done
        /// </summary>
        public bool IsPred
        { get; set; }
        /// <summary>
        /// If Rest Operation has done
        /// </summary>
        public bool IsRested
        { get; set; }
        /// <summary>
        /// If Blank Operation has done
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
        public double LastTime
        { get; set; }
        /// <summary>
        /// Counter of Block
        /// </summary>
        public int BCount
        { get; set; }
        /// <summary>
        /// Counter of Trial
        /// </summary>
        public int TCount
        { get; set; }
        /// <summary>
        /// Counter of Stimulus
        /// </summary>
        public int SCount
        { get; set; }
        /// <summary>
        /// Counter of Rows
        /// </summary>
        public int RCount
        { get; set; }
        /// <summary>
        /// Counter of Columns
        /// </summary>
        public int CCount
        { get; set; }
        /// <summary>
        /// Index of Stimulus
        /// </summary>
        public int Which
        { get; set; }
        /// <summary>
        /// Rotate Matrix
        /// </summary>
        public Matrix Rotate
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
        /// Experiment Information
        /// </summary>
        public string Info
        { get; set; }
        /// <summary>
        /// Location of Stimulus
        /// </summary>
        public Vector3 Location
        { get; set; }
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
    /// Assembly Configuration
    /// </summary>
    public class AssemblySettings
    {
        IDictionary settings;


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
            settings = GetConfig(asm);
        }

        /// <summary>
        /// Get Setting Value according to Key
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
        }

        /// <summary>
        /// Get Calling Assembly Settings
        /// </summary>
        /// <returns></returns>
        public static IDictionary GetConfig()
        {
            return GetConfig(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Get Assembly Settings
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static IDictionary GetConfig(Assembly asm)
        {
            try
            {
                string cfgFile = asm.CodeBase + ".config";
                XmlDocument doc = new XmlDocument();
                doc.Load(new XmlTextReader(cfgFile));
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
                MessageBox.Show(e.Message);
            }

            return (null);
        }
    }

    /// <summary>
    /// Get System Information
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
        /// Get current main module
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
    /// Monitor Frame Information
    /// </summary>
    public class FrameInfo
    {
        #region Fields

        Stopwatch timer;
        List<double> ifi;
        double framestamp;

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
            get { return ifi.Count + 1; }
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
        public double FPS
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

        #endregion


        /// <summary>
        /// Init
        /// </summary>
        public FrameInfo()
        {
            timer = new Stopwatch();
            ifi = new List<double>();
            framestamp = 0.0;
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
        }

    }

    /// <summary>
    /// Experiment Design Parameters
    /// </summary>
    public struct ExDesign
    {
        /// <summary>
        /// Init to custom experiment design
        /// </summary>
        /// <param name="extype">Experiment Type</param>
        /// <param name="expara">Condition Parameters</param>
        /// <param name="cond">Conditions Interpolation Parameters</param>
        /// <param name="block">Experiment Block Number</param>
        /// <param name="trial">Experiment Trials</param>
        /// <param name="stimuli">Stimulus Numbers</param>
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
        /// Set to custom experiment design
        /// </summary>
        /// <param name="extype">Experiment Type</param>
        /// <param name="expara">Condition Parameters</param>
        /// <param name="cond">Conditions Interpolation Parameters</param>
        /// <param name="block">Experiment Block Number</param>
        /// <param name="trial">Experiment Trials</param>
        /// <param name="stimuli">Stimulus Numbers</param>
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
        /// Get default experiment design -- all default with array size:10
        /// </summary>
        public static ExDesign Default
        {
            get
            {
                ExType[] exType = new ExType[10];
                ExPara[] exPara = new ExPara[10];
                SLInterpolation[] condition = new SLInterpolation[10];
                int[] stimuli = new int[10];
                for (int i = 0; i < 10; i++)
                {
                    exType[i] = ExType.None;
                    exPara[i] = ExPara.None;
                    condition[i] = SLInterpolation.Default(ExPara.None, 4);
                    stimuli[i] = 0;
                }

                return new ExDesign(exType, exPara, condition, 0, 0, stimuli, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, Color.Black);
            }
        }


        /// <summary>
        /// Experiment Type
        /// </summary>
        public ExType[] exType;
        /// <summary>
        /// Experiment Condition Parameters
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
        /// Stimulus Numbers
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
        /// Stimulus Pre-duration Time
        /// </summary>
        public float preT;
        /// <summary>
        /// Stimulus Duration Time
        /// </summary>
        public float durT;
        /// <summary>
        /// Stimulus Post-duration Time
        /// </summary>
        public float posT;
        /// <summary>
        /// Experiment BackGround Color
        /// </summary>
        public Color bgcolor;
    }

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
        /// Two Flashing Gratings with angle between their orientations
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
        /// Moving Direction Condition
        /// </summary>
        Direction,
        /// <summary>
        /// Moving Speed Condition
        /// </summary>
        Speed,
        /// <summary>
        /// Uniform Luminance Condition
        /// </summary>
        Luminance,
        /// <summary>
        /// Uniform Contrast Condition
        /// </summary>
        Contrast,
        /// <summary>
        /// Spatial Change Frequency Condition
        /// </summary>
        SpatialFreq,
        /// <summary>
        /// Spatial Phase Condition
        /// </summary>
        SpatialPhase,
        /// <summary>
        /// Temporal Change Frequency Condition
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
    /// Pre-Defined Method to generate random number
    /// </summary>
    public enum RandomMethod
    {
        /// <summary>
        /// No Method
        /// </summary>
        None,
        /// <summary>
        /// C srand() and rand() Library in msvcrt.dll
        /// </summary>
        C,
        /// <summary>
        /// .NET Random Class
        /// </summary>
        dotNET
    }

    /// <summary>
    /// Interpolation Method
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
        /// Assembly Settings Root Node Name
        /// </summary>
        public const string AsmCfgNode = "assemblySettings";

        /// <summary>
        /// Radian per Degree
        /// </summary>
        public const double RadpDeg = Math.PI / 180.0;

        /// <summary>
        /// Degree per Radian
        /// </summary>
        public const double DegpRad = 180.0 / Math.PI;

        /// <summary>
        /// MilliMeter per Inch
        /// </summary>
        public const double MMpInch = 25.4;
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
                default:
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
        /// <param name="ortho"></param>
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
    }

}
