#region File Description
//-----------------------------------------------------------------------------
// SLExperiment.cs
//
// StiLib Experiment Service.
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
        /// All Real Conditions Values --- First Dim: Which Condition, Second Dim: Condition Level
        /// </summary>
        public float[][] CondTable;
        /// <summary>
        /// Conversion Table of Stimuli Index to Condition Index of CondTable
        /// </summary>
        public int[][] StiTable;
        /// <summary>
        /// Experiment Design Parameters
        /// </summary>
        public ExDesign Exdesign;
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
        /// Init with default -- Exdesign: Default(1), Random Sequence Length: 2000
        /// </summary>
        public SLExperiment()
            : this(ExDesign.Default(1), 2000)
        {
        }

        /// <summary>
        /// Init with custom experiment design
        /// </summary>
        /// <param name="design"></param>
        /// <param name="length"></param>
        public SLExperiment(ExDesign design, int length)
            : this(design.exType, design.exPara, design.condition, design.block, design.trial, design.stimuli, design.brestT, design.trestT, design.srestT, design.preT, design.durT, design.posT, design.bgcolor, length)
        {
        }

        /// <summary>
        /// Init with custom experiment design parameters
        /// </summary>
        /// <param name="extype"></param>
        /// <param name="expara"></param>
        /// <param name="cond"></param>
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
        public SLExperiment(ExType[] extype, ExPara[] expara, SLInterpolation[] cond, int block, int trial, int[] stimuli, float brestT, float trestT, float srestT, float preT, float durT, float posT, Color bgcolor, int length)
        {
            Extype = new List<KeyValuePair<string, int>>();
            Cond = new List<SLKeyValuePair<string, int, SLInterpolation>>();

            Exdesign = new ExDesign(extype, expara, cond, block, trial, stimuli, brestT, trestT, srestT, preT, durT, posT, bgcolor);
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
        /// Construct CondTable and StiTable According to Internal Experiment Design Condition List
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
            Exdesign.stimuli[0] = StiTable.Length;
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

}
