// F# Script File : RF_Surround_2.fsx
//
// Mapping Receptive Field Surround Use Drift Grating(two marker)
//
// Copyright (c) 2009-07-21 Zhang Li

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
        this.ex.Exdesign.srestT <- 0.25f
        this.ex.Exdesign.trestT <- 0.25f
        this.ex.Exdesign.durT <- 0.5f
        this.ex.Exdesign.bgcolor <- Color.Gray
        
        let mutable gpara = GratingPara.Default
        gpara.tf <- 4.0f
        gpara.sf <- 0.8f
        gpara.sphase <- 0.0f
        gpara.BasePara.diameter <- 2.0f
        gpara.BasePara.space <- 10.0f
        gpara.BasePara.direction <- 90.0f
        gpara.BasePara.center <- new Vector3(0.0f, 0.0f, 0.0f) // CRF Center
        this.grating <- new Grating(this.GraphicsDevice, this.Services, this.SLConfig.["content"], gpara)
        
        gpara.BasePara.diameter <- 3.0f // CRF Size
        this.cgrating <- new Grating(this.GraphicsDevice, this.Services, this.SLConfig.["content"], gpara)
        
        this.step <- 3.0f
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
        this.ex.Flow.IsRested <- false
        
        this.ex.Flow.RotateDir <- Matrix.CreateRotationZ(this.grating.Para.BasePara.direction * float32 SLConstant.Rad_p_Deg)
        this.ex.Flow.TranslateCenter <- Matrix.CreateTranslation(this.grating.Para.BasePara.center)
        
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
        this.ex.PPort.Timer.Reset()
        
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
            this.ex.PPort.Timer.Start()
            this.ex.PPort.Trigger()
            this.ex.Flow.IsStiOn <- false
        if this.ex.Flow.IsStiOff = true then
            this.ex.PPort.Trigger()
            this.ex.Flow.IsStiOff <- false
            if this.ex.Flow.TrialCount - this.ex.Exdesign.trial = -1 && this.ex.Flow.StiCount - this.ex.Exdesign.stimuli.[0] = -1 then
                this.GO_OVER <- false
                ()
        if this.ex.PPort.Timer.ElapsedSeconds < float this.ex.Exdesign.durT then
            if this.ex.Flow.IsPred = false then
                this.ex.Flow.IsPred <- true
                
                let RCount = Math.Floor(float (this.ex.Rand.Sequence.[this.ex.Flow.StiCount] / this.ex.Cond.[0].VALUE.ValueN))
                let CCount = this.ex.Rand.Sequence.[this.ex.Flow.StiCount] % this.ex.Cond.[0].VALUE.ValueN
                
                let Xgrid = -float32(this.ex.Cond.[0].VALUE.ValueN - 1) * this.step / 2.0f + this.step * float32(CCount)
                let Ygrid = float32(this.ex.Cond.[0].VALUE.ValueN - 1) * this.step / 2.0f - this.step * float32(RCount)
                this.grating.Ori3DMatrix <- Matrix.CreateTranslation(Xgrid, Ygrid, 0.0f) * this.ex.Flow.RotateDir
                this.grating.WorldMatrix <- this.ex.Flow.TranslateCenter
                this.grating.Visible <- true
                this.cgrating.Visible <- true
                
                this.ex.Flow.IsStiOn <- true
                
            this.grating.SetTime(float32 this.ex.PPort.Timer.ElapsedSeconds)
            this.cgrating.SetTime(float32 this.ex.PPort.Timer.ElapsedSeconds)
        else
            if this.ex.Flow.IsRested = false then
                this.ex.Flow.IsRested <- true
                this.grating.Visible <- false
                this.cgrating.Visible <- false
                this.ex.Flow.IsStiOff <- true
            if this.ex.Flow.StiCount - this.ex.Exdesign.stimuli.[0] < -1 then
                if this.ex.PPort.Timer.ElapsedSeconds > float this.ex.Exdesign.durT + float this.ex.Exdesign.srestT then
                    this.ex.Flow.IsPred <- false
                    this.ex.Flow.IsRested <- false
                    this.ex.Flow.StiCount <- this.ex.Flow.StiCount + 1
                    this.ex.PPort.Timer.Reset()
            else
                if this.ex.Flow.TrialCount - this.ex.Exdesign.trial < -1 then
                    if this.ex.PPort.Timer.ElapsedSeconds > float this.ex.Exdesign.durT + float this.ex.Exdesign.trestT then
                        this.ex.Rand.RandomizeSequence(this.ex.Exdesign.stimuli.[0])
                        this.ex.Flow.IsPred <- false
                        this.ex.Flow.IsRested <- false
                        this.ex.Flow.TrialCount <- this.ex.Flow.TrialCount + 1
                        this.ex.Flow.StiCount <- 0
                        this.ex.PPort.Timer.Reset()
        
end

let MyExperiment = new MyEx(Text = "F# Scripting RF_Surround_2")
Application.Run(MyExperiment)
