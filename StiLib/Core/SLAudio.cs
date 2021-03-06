﻿#region File Description
//-----------------------------------------------------------------------------
// SLAudio.cs
//
// StiLib Audio Service
// Copyright (c) Zhang Li. 2009-03-02.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using System.Windows.Forms;
#endregion

namespace StiLib.Core
{
    /// <summary>
    /// StiLib Audio Service
    /// </summary>
    public class SLAudio
    {
        #region Fields

        /// <summary>
        /// XACT Audio Engine
        /// </summary>
        AudioEngine audioEngine;
        /// <summary>
        /// Wave bank in memory
        /// </summary>
        WaveBank memoryWaveBank;
        /// <summary>
        /// Wave bank in streaming
        /// </summary>
        WaveBank streamWaveBank;
        /// <summary>
        /// Sound bank
        /// </summary>
        SoundBank soundBank;

        /// <summary>
        /// A list of all non-3D cues
        /// </summary>
        List<Cue> activeCues;
        /// <summary>
        /// A list of all 3D Cues
        /// </summary>
        List<Cue3D> activeCue3Ds;
        /// <summary>
        /// A queue that keeps inactive Cue3D instances, so a new instance does not
        /// need to be created each time a sound is played
        /// </summary>
        Queue<Cue3D> inactiveCue3Ds;
        /// <summary>
        /// Sound Catagories
        /// </summary>
        SLDictionary<string, AudioCategory, float> categories;

        float globalVolume;
        bool isInitialized;
        Cue bgMusic;
        List<AudioListener> listeners;

        #endregion

        #region Properties

        /// <summary>
        /// The global sound volume.
        /// </summary>
        /// <remarks>
        /// Categories that have not been loaded into the 
        /// AudioSystem will not have their volume automatically adjusted.
        /// </remarks>
        public float GlobalVolume
        {
            get { return globalVolume; }
            set
            {
                globalVolume = value;
                ApplyGlobalVolume();
            }
        }

        /// <summary>
        /// Returns whether the sound bank or wave bank is currently in use.
        /// </summary>
        public bool IsInUse
        {
            get { return soundBank.IsInUse || memoryWaveBank.IsInUse || streamWaveBank.IsInUse; }
        }

        /// <summary>
        /// If the audio system has been initialized or not.
        /// </summary>
        public bool IsInitialized
        {
            get { return isInitialized; }
        }

        /// <summary>
        /// A list of all AudioListeners for 3D sound
        /// </summary>
        public List<AudioListener> Listeners
        {
            get { return listeners; }
        }

        /// <summary>
        /// A list of all active Cues
        /// </summary>
        public List<Cue> ActiveCues
        {
            get { return activeCues; }
        }

        /// <summary>
        /// A list of all active 3D Cues
        /// </summary>
        public List<Cue3D> ActiveCue3Ds
        {
            get { return activeCue3Ds; }
        }

        #endregion


        /// <summary>
        /// Creates a new AudioSystem instance, need Initialize()
        /// </summary>
        public SLAudio()
        {
            listeners = new List<AudioListener>();
            activeCues = new List<Cue>();
            activeCue3Ds = new List<Cue3D>();
            inactiveCue3Ds = new Queue<Cue3D>();
            categories = new SLDictionary<string, AudioCategory, float>();

            // Set default volume
            globalVolume = 1.0f;
        }

        /// <summary>
        /// Init Audio System
        /// </summary>
        /// <param name="settingsFilePath"></param>
        /// <param name="memoryWBFilePath"></param>
        /// <param name="streamWBFilePath"></param>
        /// <param name="SBFilePath"></param>
        public SLAudio(string settingsFilePath, string memoryWBFilePath, string streamWBFilePath, string SBFilePath)
            : this()
        {
            Initialize(settingsFilePath, memoryWBFilePath, streamWBFilePath, SBFilePath);
        }


        /// <summary>
        /// Loads the audio engine, sound bank and wave bank. Must be called before the audio system can be used.
        /// </summary>
        /// <param name="settingsFilePath">The filepath to the .xgs file.</param>
        /// <param name="memoryWBFilePath">The filepath to the .xwb in-memory wave bank.</param>
        /// <param name="streamWBFilePath">The filepath to the streaming wave bank with default -- offset: 0, packetsize: 64</param>
        /// <param name="SBFilePath">The filepath to the .xsb file.</param>
        public void Initialize(string settingsFilePath, string memoryWBFilePath, string streamWBFilePath, string SBFilePath)
        {
            try
            {
                audioEngine = new AudioEngine(settingsFilePath + ".xgs");
                memoryWaveBank = new WaveBank(audioEngine, memoryWBFilePath + ".xwb");
                if (!string.IsNullOrEmpty(streamWBFilePath))
                {
                    streamWaveBank = new WaveBank(audioEngine, streamWBFilePath + ".xwb", 0, 64);
                }
                soundBank = new SoundBank(audioEngine, SBFilePath + ".xsb");

                isInitialized = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Audio System Initialization Failed !");
                isInitialized = false;
            }

            // Attempt to load some default AudioCatagories
            LoadCategories("Global", "Default", "Music");
        }

        /// <summary>
        /// Loads the categories into the AudioSystem.
        /// This allows the system to adjust the volume of the categories with the GolbalVolume.
        /// </summary>
        /// <param name="categoryNames"></param>
        public void LoadCategories(params string[] categoryNames)
        {
            if (!isInitialized)
                MessageBox.Show("Audio System Not Initialized !", "Error !");

            // Load each category.
            for (int i = 0; i < categoryNames.Length; i++)
            {
                GetCategory(categoryNames[i]);
            }

            // Force the system to set each category volume.
            ApplyGlobalVolume();
        }

        /// <summary>
        /// Gets a sound catagory from the audio engine.
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public AudioCategory GetCategory(string categoryName)
        {
            // Categories are remembered as they are got,
            // so their volume can be set when globalVolume changes.
            if (categories.ContainsKey(categoryName))
            {
                AudioCategory c;
                categories.TryGetKey(categoryName, out c);
                return c;
            }
            else
            {
                AudioCategory c = audioEngine.GetCategory(categoryName);
                categories.Add(categoryName, c, 1.0f);
                return c;
            }
        }

        /// <summary>
        /// Scales the volume of each AudioCatagory loaded into the AudioSystem.
        /// </summary>
        public void ApplyGlobalVolume()
        {
            AudioCategory c;
            IEnumerator<KeyValuePair<string, float>> e = categories.GetEnumerator();
            for (int i = 0; i < categories.Count; i++)
            {
                e.MoveNext();
                categories.TryGetKey(e.Current.Key, out c);
                c.SetVolume(e.Current.Value * globalVolume);
            }
        }


        /// <summary>
        /// Updates the AudioSystem.
        /// </summary>
        public void Update()
        {
            if (!isInitialized)
                return;

            if (bgMusic != null && bgMusic.IsStopped)
            {
                bgMusic = soundBank.GetCue(bgMusic.Name);
                bgMusic.Play();
                activeCues.Add(bgMusic);
            }

            // Remove cues from the activeCues list which have stopped.
            for (int i = activeCues.Count - 1; i >= 0; i--)
            {
                if (activeCues[i].IsStopped)
                    activeCues.RemoveAt(i);
            }

            // Update 3D cues.
            for (int i = activeCue3Ds.Count - 1; i >= 0; i--)
            {
                Cue3D cue3D = activeCue3Ds[i];

                if (cue3D.Cues[0].IsStopped)
                {
                    // If the cue has stopped playing, dispose it.
                    cue3D.DisposeCues();

                    // Store the Cue3D instance for future reuse.
                    inactiveCue3Ds.Enqueue(cue3D);

                    // Remove it from the active list.
                    activeCue3Ds.RemoveAt(i);
                }
                else
                {
                    // If the cue is still playing, update its 3D settings.
                    Update3DSettings(cue3D);
                }
            }

            // Update the audio engine.
            audioEngine.Update();
        }

        /// <summary>
        /// Applies 3D settings to a cue.
        /// </summary>
        public void Update3DSettings(Cue3D cue3D)
        {
            for (int i = 0; i < listeners.Count; i++)
            {
                cue3D.Cues[i].Apply3D(listeners[i], cue3D.Emitter);
            }
        }


        /// <summary>
        /// Sets the value of a global variable.
        /// </summary>
        public void SetGlobalVariable(string variableName, float value)
        {
            audioEngine.SetGlobalVariable(variableName, value);
        }

        /// <summary>
        /// Gets the value of a global variable.
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public float GetGlobalVariable(string variableName)
        {
            return audioEngine.GetGlobalVariable(variableName);
        }

        /// <summary>
        /// Pauses all cues currently being played.
        /// </summary>
        public void PauseAll()
        {
            for (int i = 0; i < activeCues.Count; i++)
            {
                if (activeCues[i].IsPlaying)
                {
                    activeCues[i].Pause();
                }
            }

            for (int i = 0; i < activeCue3Ds.Count; i++)
            {
                if (activeCue3Ds[i].Cues[0].IsPlaying)
                {
                    activeCue3Ds[i].Pause();
                }
            }
        }

        /// <summary>
        /// Resumes all paused cues played through the AudioSystem.
        /// </summary>
        public void ResumeAll()
        {
            for (int i = 0; i < activeCues.Count; i++)
            {
                if (activeCues[i].IsPaused)
                {
                    activeCues[i].Resume();
                }
            }

            for (int i = 0; i < activeCue3Ds.Count; i++)
            {
                if (activeCue3Ds[i].Cues[0].IsPaused)
                {
                    activeCue3Ds[i].Resume();
                }
            }
        }

        /// <summary>
        /// Stops all cues played through the AudioSystem.
        /// Stopped cues cannot be resumed.
        /// </summary>
        /// <param name="stopOptions">Controls how the cues should stop.</param>
        public void StopAll(AudioStopOptions stopOptions)
        {
            for (int i = 0; i < activeCues.Count; i++)
            {
                activeCues[i].Stop(stopOptions);
            }

            for (int i = 0; i < activeCue3Ds.Count; i++)
            {
                activeCue3Ds[i].Stop(stopOptions);
            }
        }


        /// <summary>
        /// Plays a cue
        /// </summary>
        /// <param name="cueName">Name of the cue</param>
        public void Play(string cueName)
        {
            if (!isInitialized)
                MessageBox.Show("Audio System Not Initialized !", "Error !");

            Cue cue = soundBank.GetCue(cueName);

            cue.Play();
            activeCues.Add(cue);
        }

        /// <summary>
        /// Plays a cue
        /// </summary>
        /// <param name="cueName">Name of the cue</param>
        /// <param name="variables">Variables to be applied to the cue.</param>
        public void Play(string cueName, params CueVariable[] variables)
        {
            if (!isInitialized)
                MessageBox.Show("Audio System Not Initialized !", "Error !");

            Cue cue = soundBank.GetCue(cueName);

            for (int i = 0; i < variables.Length; i++)
                variables[i].Apply(cue);

            cue.Play();
            activeCues.Add(cue);
        }

        /// <summary>
        /// Plays a cue. 
        /// This method prevents the cue being collected by the garbage collecter while the cue is playing,
        /// so references to the cue outside of the AudioSystem can be safely lost.
        /// </summary>
        /// <param name="cue">The cue to be played</param>
        /// <param name="variables">Variables to be applied to the cue.</param>
        public void Play(Cue cue, params CueVariable[] variables)
        {
            for (int i = 0; i < variables.Length; i++)
                variables[i].Apply(cue);

            cue.Play();
            activeCues.Add(cue);
        }

        /// <summary>
        /// Plays a sound in 3D to all the AudioListeners listed in Listeners.
        /// </summary>
        /// <param name="cueName">Name of the sound to be played.</param>
        /// <param name="emitter">The emitter of the sound.</param>
        /// <remarks>If there are no listeners, then the sound is just played normally.</remarks>
        /// <param name="variables">Variables to be applied to the cue.</param>
        public void Play(string cueName, AudioEmitter emitter, params CueVariable[] variables)
        {
            // Play the cue normally if there are no listeners for 3D sound.
            if (listeners.Count == 0)
            {
                Play(cueName);
                return;
            }

            // Generate a cue instance for each listener
            List<Cue> cues = new List<Cue>(listeners.Count);
            for (int i = 0; i < listeners.Count; i++)
            {
                Cue cue = soundBank.GetCue(cueName);

                for (int v = 0; v < variables.Length; v++)
                    variables[v].Apply(cue);

                cues.Add(cue);
            }

            // Get an inactive instance if available
            Cue3D cue3D;
            if (inactiveCue3Ds.Count > 0)
            {
                cue3D = inactiveCue3Ds.Dequeue();
                cue3D.Cues = cues;
                cue3D.Emitter = emitter;
            }
            else
            {
                // we need to create a new one
                cue3D = new Cue3D(cues, emitter);
            }

            // apply the 3D settings for each cue instance of each listener in Cue3D
            Update3DSettings(cue3D);

            // play the cue3D
            cue3D.Play();

            // add to the activeCue3Ds list.
            activeCue3Ds.Add(cue3D);
        }


        /// <summary>
        /// Start Playing BackGround Music
        /// </summary>
        /// <param name="bgmusic_cue"></param>
        public void StartBgMusic(string bgmusic_cue)
        {
            if (!isInitialized)
                MessageBox.Show("Audio System Not Initialized !", "Error !");

            if (bgMusic != null && !bgMusic.IsStopped)
            {
                bgMusic.Stop(AudioStopOptions.AsAuthored);
            }
            bgMusic = soundBank.GetCue(bgmusic_cue);
            bgMusic.Play();
            activeCues.Add(bgMusic);
        }

        /// <summary>
        /// Pause Background Music
        /// </summary>
        public void PauseBgMusic()
        {
            if (!isInitialized)
                MessageBox.Show("Audio System Not Initialized !", "Error !");

            if (bgMusic != null && bgMusic.IsPlaying)
            {
                bgMusic.Pause();
            }
        }

        /// <summary>
        /// Resume Background Music
        /// </summary>
        public void ResumeBgMusic()
        {
            if (!isInitialized)
                MessageBox.Show("Audio System Not Initialized !", "Error !");

            if (bgMusic != null && bgMusic.IsPaused)
            {
                bgMusic.Resume();
            }
        }

        /// <summary>
        /// Stop Background Music
        /// </summary>
        /// <param name="stopOptions"></param>
        public void StopBgMusic(AudioStopOptions stopOptions)
        {
            if (!isInitialized)
                MessageBox.Show("Audio System Not Initialized !", "Error !");

            if (bgMusic != null)
            {
                bgMusic.Stop(stopOptions);
            }
        }

        /// <summary>
        /// Gets a cue by name
        /// </summary>
        /// <param name="cueName"></param>
        /// <returns></returns>
        public Cue GetCue(string cueName)
        {
            if (!isInitialized)
                MessageBox.Show("Audio System Not Initialized !", "Error !");

            return soundBank.GetCue(cueName);
        }


        /// <summary>
        /// Stops a catagory
        /// </summary>
        public void StopCategory(string category, AudioStopOptions stopOptions)
        {
            if (!isInitialized)
                MessageBox.Show("Audio System Not Initialized !", "Error !");

            GetCategory(category).Stop(stopOptions);
        }

        /// <summary>
        /// Pauses a catagory
        /// </summary>
        public void PauseCategory(string category)
        {
            if (!isInitialized)
                MessageBox.Show("Audio System Not Initialized !", "Error !");

            GetCategory(category).Pause();
        }

        /// <summary>
        /// Resumes a catagory
        /// </summary>
        public void ResumeCategory(string category)
        {
            if (!isInitialized)
                MessageBox.Show("Audio System Not Initialized !", "Error !");

            GetCategory(category).Resume();
        }

        /// <summary>
        /// Sets the volume of a catagory
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="volume">[0, 1]</param>
        public void SetVolume(string categoryName, float volume)
        {
            if (!isInitialized)
                MessageBox.Show("Audio System Not Initialized !", "Error !");

            AudioCategory category = GetCategory(categoryName);

            categories[categoryName] = volume;
            category.SetVolume(volume * globalVolume);
        }

    }

    /// <summary>
    /// Hold informations about a 3D cue.
    /// </summary>
    public class Cue3D
    {
        /// <summary>
        /// A list of cues, one for each listener.
        /// </summary>
        public List<Cue> Cues;
        /// <summary>
        /// The emitter of the sound.
        /// </summary>
        public AudioEmitter Emitter;


        /// <summary>
        /// Creates a new Cue3D.
        /// </summary>
        /// <param name="cues">A list of cues. One for each AuidoListener.</param>
        /// <param name="emitter">The emitter of the sound.</param>
        public Cue3D(List<Cue> cues, AudioEmitter emitter)
        {
            Cues = cues;
            Emitter = emitter;
        }


        /// <summary>
        /// Plays the sound.
        /// </summary>
        public void Play()
        {
            for (int i = 0; i < Cues.Count; i++)
            {
                Cues[i].Play();
            }
        }

        /// <summary>
        /// Pauses the sound.
        /// </summary>
        public void Pause()
        {
            for (int i = 0; i < Cues.Count; i++)
            {
                Cues[i].Pause();
            }
        }

        /// <summary>
        /// Resumes the sound.
        /// </summary>
        public void Resume()
        {
            for (int i = 0; i < Cues.Count; i++)
            {
                Cues[i].Resume();
            }
        }

        /// <summary>
        /// Stops the sound.
        /// </summary>
        /// <param name="stopOptions"></param>
        public void Stop(AudioStopOptions stopOptions)
        {
            for (int i = 0; i < Cues.Count; i++)
            {
                Cues[i].Stop(stopOptions);
            }
        }

        /// <summary>
        /// Disposes all cues.
        /// </summary>
        public void DisposeCues()
        {
            for (int i = 0; i < Cues.Count; i++)
            {
                Cues[i].Dispose();
            }
        }

    }

    /// <summary>
    /// Holds information about a cue variable.
    /// </summary>
    public struct CueVariable
    {
        /// <summary>
        /// The name of the variable.
        /// </summary>
        public string Name;
        /// <summary>
        /// The value of the variable.
        /// </summary>
        public float Value;


        /// <summary>
        /// Applies the value of the variable to the specified cue instance.
        /// </summary>
        /// <param name="cue">Cue that contains the variable to be set.</param>
        public void Apply(Cue cue)
        {
            cue.SetVariable(Name, Value);
        }

    }

}
