// F# Script File : RF_fBar(Custom_Loop).fsx
//
// Two Flashing Bar Reverse-Correlation Receptive Field Mapping Diagram Using User Custom Loop
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
type ExRF_fBar_Custom() = class
    inherit RF_fBar(null)
    
    override self.Initialize() =
        self.text <- new Text(self.GraphicsDevice, self.Services, self.SLConfig.["content"], "Arial")
        self.ex <- new SLExperiment()
        
        self.ex.AddExType(ExType.RF_fBar)
        self.ex.AddCondition(ExPara.Orientation, 0)
        self.ex.Exdesign.trial <- 2
        self.ex.Exdesign.durT <- 0.030f
        self.ex.Exdesign.bgcolor <- Color.Gray
        
        let mutable bpara = BarPara.Default
        bpara.width <- 0.5f
        bpara.height <- 1.5f
        bpara.BasePara.orientation <- 0.0f
        bpara.BasePara.space <- 8.0f
        bpara.BasePara.center <- new Vector3(3.0f, 3.0f, 0.0f)
        bpara.BasePara.color <- Color.Black
        self.bars.[0].Init(self.GraphicsDevice, bpara)
        
        bpara.BasePara.color <- Color.White
        self.bars.[1].Init(self.GraphicsDevice, bpara)
        
        self.Rstep <- 1.5f
        self.Cstep <- 1.5f
        self.InitGrid()
        
    override self.Draw() =
        self.GraphicsDevice.Clear(self.ex.Exdesign.bgcolor)
        self.text.Draw()
        
    override self.Update() = 
          if self.GO_OVER = true then
              self.Custom_Loop()
              
    // Our Custom Loop
    member self.Custom_Loop() = 
        for i = 0 to self.ex.Exdesign.trial - 1 do
            for j = 0 to self.ex.Exdesign.stimuli.[0] - 1 do
                
                self.ex.Flow.RowCount <- int( Math.Floor( float self.ex.Rand.Sequence.[j] / (float self.Columns * 2.0) ) )
                let t = self.ex.Rand.Sequence.[j] % (self.Columns * 2)
                self.ex.Flow.ColumnCount <- int( Math.Floor(float t / 2.0) )
                self.ex.Flow.SliceCount <- t % 2
                
                let Xgrid = - float32 (self.Columns - 1) * self.Cstep / 2.0f + self.Cstep * float32 self.ex.Flow.ColumnCount
                let Ygrid = float32 (self.Rows - 1) * self.Rstep / 2.0f - self.Rstep * float32 self.ex.Flow.RowCount
                self.bars.[self.ex.Flow.SliceCount].Ori3DMatrix <- Matrix.CreateTranslation(Xgrid, Ygrid, 0.0f) * self.ex.Flow.RotateOri
                self.bars.[self.ex.Flow.SliceCount].WorldMatrix <- self.ex.Flow.TranslateCenter
                
                // Draw Stimulus and Present
                self.GraphicsDevice.Clear(self.ex.Exdesign.bgcolor)
                self.bars.[self.ex.Flow.SliceCount].Draw(self.GraphicsDevice)
                
                self.ex.Flow.Info <- i.ToString() + " / " + self.ex.Exdesign.trial.ToString() + " Trials\n" +
                                            j.ToString() + " / " + self.ex.Exdesign.stimuli.[0].ToString() + " Stimuli"
                self.text.Draw(self.ex.Flow.Info)
                
                self.GraphicsDevice.Present()
                
                // Stimulus Onset Marker
                self.ex.PPort.Trigger()
                
                // Stimulus Duration
                self.ex.PPort.Timer.Rest(float self.ex.Exdesign.durT-self.ex.PPort.PulseTime)
            
            // Randomize a new sequence of stimulus for new trial
            self.ex.Rand.RandomizeSequence(self.ex.Exdesign.stimuli.[0])
        // End Custom Loop to default game style loop
        self.GO_OVER <- false
        
end

let Experiment = new ExRF_fBar_Custom(Text = "F# Scripting RF_fBar(Custom_Loop)")
Application.Run(Experiment)

#if COMPILED
[<STAThread>]
do Application.Run(Experiment)
#endif