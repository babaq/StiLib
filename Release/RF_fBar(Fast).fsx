// F# Script File : RF_fBar(Fast).fsx
//
// Fast Version of Two Flashing Bar Reverse-Correlation Receptive Field Mapping Diagram Using User Custom Deterministic Loop
//
// Copyright (c) 2009-02-13 Zhang Li

#r @"StiLib.dll"
#r @"Microsoft.Xna.Framework.dll"

open System
open System.Windows.Forms
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open StiLib.Core
open StiLib.Vision
open StiLib.Vision.Stimuli

// Our Custom Experiment is inherited from StiLib.Vision.Stimuli.RF_fBar
type ExRF_fBar_Fast() = class
    inherit RF_fBar(800, 600, 0, true, true)
    
    override self.Initialize() =
        self.text <- new Text(self.GraphicsDevice, self.Services, self.SLConfig.["content"], "Arial")
        self.ex <- new SLExperiment()
        
        self.ex.AddExType(ExType.RF_fBar)
        self.ex.AddCondition(ExPara.Orientation, 0)
        self.ex.Expara.trial <- 60
        self.ex.Expara.durT <- 0.030f
        self.ex.Expara.bgcolor <- Color.Gray
        
        let mutable bpara = BarPara.Default
        bpara.width <- 0.5f
        bpara.height <- 1.5f
        bpara.BasePara.orientation <- 0.0f
        bpara.BasePara.movearea <- 8.0f
        bpara.BasePara.center <- new Vector3(3.0f, 3.0f, 0.0f)
        bpara.BasePara.color <- Color.Black
        self.Bar.[0].Init(self.GraphicsDevice, bpara)
        
        bpara.BasePara.color <- Color.White
        self.Bar.[1].Init(self.GraphicsDevice, bpara)
        
        self.InitGrid()
        
    override self.Draw() =
        self.GraphicsDevice.Clear(self.ex.Expara.bgcolor)
        self.text.Draw()
        
    override self.Update() = 
          if self.GO_OVER = true then
              self.Deterministic_Loop()
              
    member self.Deterministic_Loop() = 
        self.ex.PPort.timer.ReStart()
        // Our Custom Loop
        for i = 1 to self.ex.Expara.trial do
            for j = 1 to self.ex.Expara.stimuli.[0] do
                // Stimulus Onset
                self.ex.PPort.Trigger()
                
                self.ex.Flow.RCount <- int( Math.Floor( float self.ex.Rand.RSequence.[j] / (float self.Columns * 2.0) ) )
                let t = self.ex.Rand.RSequence.[j] % (self.Columns * 2)
                self.ex.Flow.CCount <- int( Math.Floor(float t / 2.0) )
                self.ex.Flow.Which <- t % 2
                
                let Xgrid = - float32 (self.Columns - 1) * self.Bar.[0].Para.width / 2.0f + self.Bar.[0].Para.width * float32 self.ex.Flow.CCount
                let Ygrid = float32 (self.Rows - 1) * self.Bar.[0].Para.height / 2.0f - self.Bar.[0].Para.height * float32 self.ex.Flow.RCount
                self.ex.Flow.Rotate <- Matrix.CreateRotationZ(self.Bar.[0].Para.BasePara.orientation * float32 Math.PI / 180.0f)
                self.ex.Flow.Translate <- Matrix.CreateTranslation(Xgrid, Ygrid, 0.0f) * self.ex.Flow.Rotate * Matrix.CreateTranslation(self.Bar.[0].Para.BasePara.center)
                self.Bar.[self.ex.Flow.Which].SetWorld(self.ex.Flow.Translate)
                
                // Draw Stimulus and Present
                self.GraphicsDevice.Clear(self.ex.Expara.bgcolor)
                self.Bar.[self.ex.Flow.Which].Draw(self.GraphicsDevice)
                
                self.ex.Flow.Info <- i.ToString() + " / " + self.ex.Expara.trial.ToString() + " Trials\n" +
                                            j.ToString() + " / " + self.ex.Expara.stimuli.[0].ToString() + " Stimuli"
                self.text.Draw(self.ex.Flow.Info)
                
                self.GraphicsDevice.Present()
                
                // Stimulus Duration
                self.ex.PPort.timer.Rest(float self.ex.Flow.StiTime)
            
            // Randomize a new sequence of stimulus for new trial
            self.ex.Rand.RandomizeSequence(self.ex.Expara.stimuli.[0])
        // End Deterministic Loop to default game style loop
        self.GO_OVER <- false
        
end

let Experiment = new ExRF_fBar_Fast(Text = "F# Scripting RF_fBar(Fast)")
Application.Run(Experiment)

#if COMPILED
[<STAThread>]
do Application.Run(Experiment)
#endif