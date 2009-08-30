// F# Script File : RF_Size_1.fsx
//
// Varing Sizes of Grating to Measure RF Size in Center-Surround Modulation(one marker)
//
// Copyright (c) 2009-04-16 Zhang Li

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
    
    new() as this = 
        { inherit SLForm(null); grating = null; text = null; ex = null }
        then
        this.text <- new Text(this.GraphicsDevice, this.Services, this.SLConfig.["content"], "Thames")
        this.ex <- new SLExperiment()
        this.ex.AddExType(ExType.RF)
        this.ex.AddCondition(ExPara.Size, new SLInterpolation(0.0f, 10.0f, 21, Interpolation.Linear))
        this.ex.Exdesign.trial <- 2
        this.ex.Exdesign.durT <- 0.5f
        this.ex.Exdesign.bgcolor <- Color.Gray
        this.ex.InitEx()
        
        let mutable gpara = GratingPara.Default
        gpara.tf <- 0.0f
        gpara.sf <- 0.8f
        gpara.sphase <- 0.5f
        gpara.BasePara.direction <- 90.0f
        gpara.BasePara.center <- new Vector3(0.0f, 0.0f, 0.0f)
        this.grating <- new Grating(this.GraphicsDevice, this.Services, this.SLConfig.["content"], gpara)
        
    override this.SetFlow() = 
        this.ex.Flow.TrialCount <- 0
        this.ex.Flow.StiCount <- 0
        this.ex.Flow.IsPred <- false
        
    override this.MarkHead() = 
        this.DrawTip(ref this.text, this.ex.Exdesign.bgcolor, SLConstant.MarkHead)
        
        this.ex.Rand.RandomizeSequence(this.ex.Exdesign.stimuli.[0])
        
        this.ex.PPort.MarkerEncode(this.ex.Extype.[0].Value)
        this.ex.PPort.MarkerEncode(this.ex.Cond.[0].SKEY)
        this.ex.PPort.MarkerEncode(this.ex.Cond.[0].VALUE.ValueN)
        this.ex.PPort.MarkerEncode(int( this.ex.Cond.[0].VALUE.StartValue ))
        this.ex.PPort.MarkerEncode(int( this.ex.Cond.[0].VALUE.EndValue ))
        this.ex.PPort.MarkerEncode(this.ex.Rand.Seed)
        this.ex.PPort.MarkerEncode(this.ex.Exdesign.trial)
        
        this.ex.PPort.MarkerSeparatorEncode()
        
        this.grating.Para.Encode(this.ex.PPort)

        this.ex.PPort.MarkerEndEncode()
        this.ex.PPort.Timer.Reset()
        
    override this.Update() = 
        if this.GO_OVER = true then
            this.Update_RF_Size()
    override this.Draw() = 
        this.GraphicsDevice.Clear(this.ex.Exdesign.bgcolor)
        if this.GO_OVER = true then
            this.grating.Draw(this.GraphicsDevice)
            this.ex.Flow.Info <- this.ex.Flow.TrialCount.ToString() + " / " + this.ex.Exdesign.trial.ToString() + " Trials\n" + 
                                        this.ex.Flow.StiCount.ToString() + " / " + this.ex.Exdesign.stimuli.[0].ToString() + " Stimuli"
            this.text.Draw(this.ex.Flow.Info)
        else
            this.text.Draw()
    member this.Update_RF_Size() = 
        if this.ex.Flow.IsStiOn = true then
            this.ex.PPort.Timer.Start()
            this.ex.PPort.Trigger()
            this.ex.Flow.IsStiOn <- false
        if this.ex.PPort.Timer.ElapsedSeconds < float this.ex.Exdesign.durT then
            if this.ex.Flow.IsPred = false then
                this.ex.Flow.IsPred <- true
                
                let condpara = this.ex.GetCondition(this.ex.Rand.Sequence.[this.ex.Flow.StiCount])
                if condpara.[0] = 0.0f then
                    this.grating.Visible <- false
                else
                    this.grating.SetDiameter( condpara.[0] )
                    this.grating.Visible <- true
                
                this.ex.Flow.IsStiOn <- true
            
            this.grating.SetTime(float32 this.ex.PPort.Timer.ElapsedSeconds)
        else
            if this.ex.Flow.StiCount - this.ex.Exdesign.stimuli.[0] < -1 then
                this.ex.Flow.IsPred <- false
                this.ex.Flow.StiCount <- this.ex.Flow.StiCount + 1
                this.ex.PPort.Timer.Reset()
            else
                if this.ex.Flow.TrialCount - this.ex.Exdesign.trial < -1 then
                    this.ex.Rand.RandomizeSequence(this.ex.Exdesign.stimuli.[0])
                    this.ex.Flow.IsPred <- false
                    this.ex.Flow.TrialCount <- this.ex.Flow.TrialCount + 1
                    this.ex.Flow.StiCount <- 0
                    this.ex.PPort.Timer.Reset()
                else
                    this.GO_OVER <- false
                    ()
            this.Update()
        
end

let MyExperiment = new MyEx(Text = "F# Scripting RF_Size_1")
Application.Run(MyExperiment)
