#region File Description
//-----------------------------------------------------------------------------
// SLExperiment.cs
//
// StiLib Experiment Type
// Copyright (c) Zhang Li. 2009-02-22.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
#endregion

namespace StiLib.Core
{
    /// <summary>
    /// StiLib Experiment Design and Control Services
    /// </summary>
    public class SLExperiment
    {
        #region Fields

        /// <summary>
        /// Experiment Type
        /// </summary>
        public List<KeyValuePair<string, int>> Extype;
        /// <summary>
        /// Experiment Design Conditions
        /// </summary>
        public List<SLKeyValuePair<string, int, SLInterpolation>> Cond;
        /// <summary>
        /// All Real Conditions --- First Dim:Which Condition, Second Dim:Condition Level
        /// </summary>
        public float[][] CondTable;
        /// <summary>
        /// Conversion Table of Stimuli Index to Condition Index of CondTable
        /// </summary>
        public int[][] StiTable;
        /// <summary>
        /// Experiment Design Parameters
        /// </summary>
        public ExDesign Expara;
        /// <summary>
        /// Flow Control Service
        /// </summary>
        public FlowControl Flow;
        /// <summary>
        /// Parallel Port Service
        /// </summary>
        public ParallelPort PPort;
        /// <summary>
        /// Randomization Service
        /// </summary>
        public SLRandom Rand;

        #endregion


        /// <summary>
        /// Init to default
        /// </summary>
        public SLExperiment()
            : this(ExDesign.Default, 2000)
        {
        }

        /// <summary>
        /// Init with empty Experiment Type and Conditions
        /// </summary>
        /// <param name="design"></param>
        /// <param name="length"></param>
        public SLExperiment(ExDesign design, int length)
            : this(design.block, design.trial, design.stimuli, design.brestT, design.trestT, design.srestT, design.preT, design.durT, design.posT, design.bgcolor, length)
        {
        }

        /// <summary>
        /// Init with empty Experiment Type and Conditions
        /// </summary>
        /// <param name="block"></param>
        /// <param name="trial"></param>
        /// <param name="stimuli"></param>
        /// <param name="brestT"></param>
        /// <param name="trestT"></param>
        /// <param name="srestT"></param>
        /// <param name="preT"></param>
        /// <param name="durT"></param>
        /// <param name="posT"></param>
        /// <param name="bgcolor"></param>
        /// <param name="length"></param>
        public SLExperiment(int block, int trial, int[] stimuli, float brestT, float trestT, float srestT, float preT, float durT, float posT, Color bgcolor, int length)
        {
            Extype = new List<KeyValuePair<string, int>>();
            Cond = new List<SLKeyValuePair<string, int, SLInterpolation>>();
            Expara = new ExDesign(new ExType[] { ExType.None }, new ExPara[] { ExPara.None }, new SLInterpolation[] { SLInterpolation.Default(ExPara.None, 4) }, block, trial, stimuli, brestT, trestT, srestT, preT, durT, posT, bgcolor);
            Flow = new FlowControl();
            PPort = new ParallelPort();
            Rand = new SLRandom(length);
        }


        /// <summary>
        /// Add Pre-Defined Experiment Type
        /// </summary>
        /// <param name="type"></param>
        public void AddExType(ExType type)
        {
            AddExType(type.ToString(), (int)type);
        }

        /// <summary>
        /// Add custom experiment type's name(string) and code(int)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="code"></param>
        public void AddExType(string name, int code)
        {
            Extype.Add(new KeyValuePair<string, int>(name, code));
        }

        /// <summary>
        /// Add Pre-Defined Experiment Condition Parameter and Condition Levels
        /// </summary>
        /// <param name="para"></param>
        /// <param name="n"></param>
        public void AddCondition(ExPara para, int n)
        {
            AddCondition(para.ToString(), (int)para, SLInterpolation.Default(para, n));
        }

        /// <summary>
        /// Add Pre-Defined Experiment Condition Parameter and Condition Interpolation Parameters
        /// </summary>
        /// <param name="para"></param>
        /// <param name="interpolate"></param>
        public void AddCondition(ExPara para, SLInterpolation interpolate)
        {
            AddCondition(para.ToString(), (int)para, interpolate);
        }

        /// <summary>
        /// Add custom experiment design's condition parmeter name(string), code(int) and condition interpolation parameters
        /// </summary>
        /// <param name="paraname"></param>
        /// <param name="code"></param>
        /// <param name="interpolate"></param>
        public void AddCondition(string paraname, int code, SLInterpolation interpolate)
        {
            Cond.Add(new SLKeyValuePair<string, int, SLInterpolation>(paraname, code, interpolate));
        }

        /// <summary>
        /// Add custom experiment design's condition parmeter name(string), code(int) and condition interpolation parameters
        /// </summary>
        /// <param name="paraname"></param>
        /// <param name="code"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="n"></param>
        /// <param name="method"></param>
        public void AddCondition(string paraname, int code, float start, float end, int n, Interpolation method)
        {
            Cond.Add(new SLKeyValuePair<string, int, SLInterpolation>(paraname, code, new SLInterpolation(start, end, n, method)));
        }


        /// <summary>
        /// Construct CondTable and StiTable According to Experiment Design
        /// </summary>
        public void InitEx()
        {
            CondTable = new float[Cond.Count][];
            int[] ortho = new int[Cond.Count];
            for (int i = 0; i < Cond.Count; i++)
            {
                CondTable[i] = Cond[i].VALUE.Interpolate();
                ortho[i] = Cond[i].VALUE.ValueN;
            }
            StiTable = SLAlgorithm.OrthoTable(ortho);
            Expara.stimuli[0] = StiTable.Length;
        }

        /// <summary>
        /// Get Condition Level of All Conditions According to a Stimuli Index
        /// </summary>
        /// <param name="stiindex"></param>
        /// <returns></returns>
        public float[] GetCondition(int stiindex)
        {
            return GetCondition(GetConditionIndex(stiindex));
        }

        /// <summary>
        /// Get Condition Level of All Conditions According to Condition Level Index
        /// </summary>
        /// <param name="condindex"></param>
        /// <returns></returns>
        public float[] GetCondition(int[] condindex)
        {
            float[] condition = new float[condindex.Length];
            for (int i = 0; i < condindex.Length; i++)
            {
                condition[i] = CondTable[i][condindex[i]];
            }
            return condition;
        }

        /// <summary>
        /// Get Condition Level Index of All Conditions According to a Stimuli Index
        /// </summary>
        /// <param name="stiindex"></param>
        /// <returns></returns>
        public int[] GetConditionIndex(int stiindex)
        {
            return StiTable[stiindex];
        }

    }

    /// <summary>
    /// Interpolation Parameters
    /// </summary>
    public struct SLInterpolation
    {
        /// <summary>
        /// Init a custom SLInterpolation Structure
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
                default:
                    return new SLInterpolation(0.0f, 360.0f, n, Interpolation.Linear);
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
            }
        }

        /// <summary>
        /// Interpolate a Sequence according to Internal Parameters
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
        /// Interpolation Value Numbers
        /// </summary>
        public int ValueN;
        /// <summary>
        /// Interpolation Method
        /// </summary>
        public Interpolation Method;
    }
}
