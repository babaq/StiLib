#region File Description
//-----------------------------------------------------------------------------
// HearingStimulus.cs
//
// StiLib Hearing Stimulus Base Class
// Copyright (c) Zhang Li. 2009-02-18.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
#endregion

namespace StiLib.Hearing
{
    /// <summary>
    /// StiLib Hearing Stimulus Abstract Base Class
    /// </summary>
    public abstract class HearingStimulus : ICloneable
    {


        #region Abstract Virtual Functions

        /// <summary>
        /// Initialize Stimulus According to Internal Parameters
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// Play Stimulus
        /// </summary>
        public abstract void Play();

        /// <summary>
        /// Creates a New Object That is a Copy of the Current Instance
        /// </summary>
        /// <returns></returns>
        public abstract object Clone();

        /// <summary>
        /// Hearing Stimulus Basic Parameters
        /// </summary>
        public abstract hsBasePara BasePara { get; set; }

        /// <summary>
        /// Hearing Stimulus Hearable State
        /// </summary>
        public abstract bool Hearable { get; set; }

        #endregion


    }

    #region Hearing Stimulus Parameter Structs

    /// <summary>
    /// Hearing Stimulus Basic Parameters
    /// </summary>
    public struct hsBasePara
    {
    }

    #endregion

}
