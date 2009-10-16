// F# Script File : RF_Surround_1.fsx
//
// Reverse-Correlation Mapping Receptive Field Surround Using Static Grating(one marker)
//
// Copyright (c) 2009-09-09 Zhang Li

#r @"StiLib.dll"
#r @"Microsoft.Xna.Framework.dll"

open System
open System.Windows.Forms
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open StiLib.Core
open StiLib.Vision

// Our Custom Experiment is inherited from StiLib.Core.SLForm
type MyEx = class
    inherit SLForm
    
    val mutable text: Text
    val mutable ex: SLExperiment
    val mutable cgrating: Grating
    val mutable grating: Grating
    val mutable step: float32
    
    new() as this = 
        { inherit SLForm(null); cgrating = null; grating = null; text = null; ex = null; step = 0.0f }
        then
        this.text <- new Text(this.GraphicsDevice, this.Services, this.SLConfig.["content"], "Thames")
        this.ex <- new SLExperiment()
        this.ex.AddExType(ExType.RF)
        
        this.ex.Exdesign.trial <- 2
        this.ex.Exdesign.durT <- 0.030f
        this.ex.Exdesign.bgcolor <- Color.Gray
        
        let mutable gpara = GratingPara.Default
        gpara.BasePara.direction <- 90.0f
        gpara.tf <- 3.0f
        gpara.sf <- 0.8f
        gpara.sphase <- 0.0f
        gpara.BasePara.space <- 10.0f
        gpara.BasePara.diameter <- 2.0f // CRF Size
        gpara.BasePara.center <- new Vector3(0.0f, 0.0f, 0.0f) // CRF Center
        this.cgrating <- new Grating(this.GraphicsDevice, this.Services, this.SLConfig.["content"], gpara)
        
        gpara.BasePara.diameter <- 1.0f // Probe Grating Size
        this.grating <- new Grating(this.GraphicsDevice, this.Services, this.SLConfig.["content"], gpara)
        
        this.step <- 1.0f;
        this.InitGrid()
        
    member this.InitGrid() = 
        let mutable row = int( Math.Floor(float( this.grating.Para.BasePara.space / this.step )) )
        if row % 2 = 0 then
            row <- row + 1
        this.ex.AddCondition(ExPara.Location, row)
        
    override this.SetFlow() = 
        this.ex.Flow.TrialCount <- 0
        this.ex.Flow.StiCount <- 0
        this.ex.Flow.IsPred <- false
        
        this.ex.Flow.TranslateCenter <- Matrix.CreateTranslation(this.grating.Para.BasePara.center)
        this.ex.Flow.StiTime <- 0.0f
        
    override this.MarkHead() = 
        this.DrawTip(ref this.text, this.ex.Exdesign.bgcolor, SLConstant.MarkHead)
        
        this.ex.Exdesign.stimuli.[0] <- this.ex.Cond.[0].VALUE.ValueN * this.ex.Cond.[0].VALUE.ValueN
        this.ex.Rand.RandomizeSequence(this.ex.Exdesign.stimuli.[0])
        
        this.ex.PPort.MarkerEncode(this.ex.Extype.[0].Value)
        this.ex.PPort.MarkerEncode(this.ex.Cond.[0].SKEY)
        this.ex.PPort.MarkerEncode(this.ex.Cond.[0].VALUE.ValueN)
        this.ex.PPort.MarkerEncode(this.ex.Rand.Seed)
        this.ex.PPort.MarkerEncode(this.ex.Exdesign.trial)
        
        this.ex.PPort.MarkerSeparatorEncode()
        
        this.grating.Para.Encode(this.ex.PPort)
        this.ex.PPort.MarkerEncode(int( Math.Floor(float(this.cgrating.Para.BasePara.diameter) * 100.0) ))
        this.ex.PPort.MarkerEncode(int( Math.Floor(float(this.grating.display_H_deg) * 100.0) ))
        this.ex.PPort.MarkerEncode(int( Math.Floor(float(this.grating.display_W_deg) * 100.0) ))
        this.ex.PPort.MarkerEncode(int( Math.Floor(float(this.step) * 100.0) ))

        this.ex.PPort.MarkerEndEncode()
        this.ex.PPort.Timer.ReStart()
        
    override this.Update() = 
        if this.GO_OVER = true then
            this.Update_Grating()
    override this.Draw() = 
        this.GraphicsDevice.Clear(this.ex.Exdesign.bgcolor)
        if this.GO_OVER = true then
            this.grating.Draw(this.GraphicsDevice)
            this.cgrating.Draw(this.GraphicsDevice)
            this.ex.Flow.Info <- this.ex.Flow.TrialCount.ToString() + " / " + this.ex.Exdesign.trial.ToString() + " Trials\n" + 
                                        this.ex.Flow.StiCount.ToString() + " / " + this.ex.Exdesign.stimuli.[0].ToString() + " Stimuli"
            this.text.Draw(this.ex.Flow.Info)
        else
            this.text.Draw()
    member this.Update_Grating() = 
        if this.ex.Flow.IsStiOn = true then
            this.ex.Flow.StiTime <- float32 this.ex.PPort.Timer.ElapsedSeconds
            this.ex.PPort.Trigger()
            this.ex.Flow.IsStiOn <- false
        if float32 this.ex.PPort.Timer.ElapsedSeconds - this.ex.Flow.StiTime < this.ex.Exdesign.durT then
            if this.ex.Flow.IsPred = false then
                this.ex.Flow.IsPred <- true
                
                let RCount = Math.Floor(float (this.ex.Rand.Sequence.[this.ex.Flow.StiCount] / this.ex.Cond.[0].VALUE.ValueN))
                let CCount = this.ex.Rand.Sequence.[this.ex.Flow.StiCount] % this.ex.Cond.[0].VALUE.ValueN
                
                let Xgrid = -float32(this.ex.Cond.[0].VALUE.ValueN - 1) * this.step / 2.0f + this.step * float32(CCount)
                let Ygrid = float32(this.ex.Cond.[0].VALUE.ValueN - 1) * this.step / 2.0f - this.step * float32(RCount)
                this.grating.WorldMatrix <- Matrix.CreateTranslation(Xgrid, Ygrid, 0.0f) * this.ex.Flow.TranslateCenter                
                this.ex.Flow.IsStiOn <- true
                
            this.cgrating.SetTime(float32 this.ex.PPort.Timer.ElapsedSeconds)
        else
            if this.ex.Flow.StiCount - this.ex.Exdesign.stimuli.[0] < -1 then
                this.ex.Flow.IsPred <- false
                this.ex.Flow.StiCount <- this.ex.Flow.StiCount + 1
                this.ex.Flow.StiTime <- float32 this.ex.PPort.Timer.ElapsedSeconds
            else
                if this.ex.Flow.TrialCount - this.ex.Exdesign.trial < -1 then
                    this.ex.Rand.RandomizeSequence(this.ex.Exdesign.stimuli.[0])
                    this.ex.Flow.IsPred <- false
                    this.ex.Flow.TrialCount <- this.ex.Flow.TrialCount + 1
                    this.ex.Flow.StiCount <- 0
                    this.ex.Flow.StiTime <- float32 this.ex.PPort.Timer.ElapsedSeconds
                else
                    this.GO_OVER <- false
                    ()
            this.Update()
        
end

let MyExperiment = new MyEx(Text = "F# Scripting RF_Surround_1")
Application.Run(MyExperiment)