// F# Script File : fGrating_Surround.fsx
//
// Examine basic properties of a neuron's RF_Surround using reverse-correlation flashing grating
//
// Copyright (c) 2009-09-11 Zhang Li

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
    val mutable grating: Grating
    val mutable cgrating: Grating
    
    new() as this = 
        { inherit SLForm(null); grating = null; text = null; ex = null; cgrating = null }
        then
        this.text <- new Text(this.GraphicsDevice, this.Services, this.SLConfig.["content"], "Thames")
        this.ex <- new SLExperiment()
        this.ex.AddExType(ExType.fGrating)
        this.ex.AddCondition(ExPara.Orientation, 8)
        this.ex.AddCondition(ExPara.SpatialFreq, 5)
        this.ex.AddCondition(ExPara.SpatialPhase, 4)
        this.ex.Exdesign.trial <- 2
        this.ex.Exdesign.durT <- 0.030f
        this.ex.Exdesign.bgcolor <- Color.Gray
        this.ex.InitEx()
        
        let mutable gpara = GratingPara.Default
        gpara.BasePara.direction <- 90.0f
        gpara.tf <- 3.0f
        gpara.sf <- 0.8f
        gpara.sphase <- 0.0f
        gpara.BasePara.diameter <- 2.0f // CRF Size
        gpara.BasePara.center <- new Vector3(0.0f, 0.0f, 0.0f) // CRF Center
        this.cgrating <- new Grating(this.GraphicsDevice, this.Services, this.SLConfig.["content"], gpara)
        
        gpara.BasePara.diameter <- 3.0f // CRF Surround Size
        gpara.BasePara.center <- new Vector3(4.0f, 0.0f, 0.0f) // CRF Surround Center
        this.grating <- new Grating(this.GraphicsDevice, this.Services, this.SLConfig.["content"], gpara)
        
    override this.SetFlow() = 
        this.ex.Flow.TrialCount <- 0
        this.ex.Flow.StiCount <- 0
        this.ex.Flow.IsPred <- false
        this.ex.Flow.StiTime <- 0.0f
        
        this.ex.Flow.TranslateCenter <- Matrix.CreateTranslation(this.grating.Para.BasePara.center)
                
    override this.MarkHead() = 
        this.DrawTip(ref this.text, this.ex.Exdesign.bgcolor, SLConstant.MarkHead)
        
        this.ex.Rand.RandomizeSequence(this.ex.Exdesign.stimuli.[0])
        
        this.ex.PPort.MarkerEncode(this.ex.Extype.[0].Value)
        this.ex.PPort.MarkerEncode(this.ex.Cond.[0].SKEY)
        this.ex.PPort.MarkerEncode(this.ex.Cond.[0].VALUE.ValueN)
        this.ex.PPort.MarkerEncode(this.ex.Cond.[1].SKEY)
        this.ex.PPort.MarkerEncode(this.ex.Cond.[1].VALUE.ValueN)
        this.ex.PPort.MarkerEncode(this.ex.Cond.[2].SKEY)
        this.ex.PPort.MarkerEncode(this.ex.Cond.[2].VALUE.ValueN)
        this.ex.PPort.MarkerEncode(this.ex.Rand.Seed)
        this.ex.PPort.MarkerEncode(this.ex.Exdesign.trial)
        
        this.ex.PPort.MarkerSeparatorEncode()
        
        this.grating.Para.Encode(this.ex.PPort)
        this.ex.PPort.MarkerEncode(int( Math.Floor(float(this.cgrating.Para.BasePara.diameter) * 100.0) ))

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
                
                let condpara = this.ex.GetCondition(this.ex.Rand.Sequence.[this.ex.Flow.StiCount])
                
                this.grating.SetSPhase( condpara.[2] )
                this.grating.SetSF( condpara.[1] )
                this.grating.Ori3DMatrix <- Matrix.CreateRotationZ(condpara.[0] * float32 SLConstant.Rad_p_Deg)
                this.grating.WorldMatrix <- this.ex.Flow.TranslateCenter
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

let MyExperiment = new MyEx(Text = "F# Scripting fGrating_Surround")
Application.Run(MyExperiment)
