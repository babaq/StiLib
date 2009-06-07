// F# Script File : fGrating(Auto).fsx
//
// Examine basic properties of a neuron using reverse-correlation flashing grating(Auto Experiment Design Version)
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
        { inherit SLForm(800, 600, 0, true, true); grating = null; text = null; ex = null }
        then
        this.text <- new Text(this.GraphicsDevice, this.Services, this.SLConfig.["content"], "Thames")
        this.ex <- new SLExperiment()
        this.ex.AddExType(ExType.fGrating)
        this.ex.AddCondition(ExPara.Orientation, 8)
        this.ex.AddCondition(ExPara.SpatialFreq, 5)
        this.ex.AddCondition(ExPara.SpatialPhase, 4)
        this.ex.Expara.trial <- 40
        this.ex.Expara.durT <- 0.030f
        this.ex.Expara.bgcolor <- Color.Gray
        this.ex.InitEx()
        
        let mutable gpara = GratingPara.Default
        gpara.BasePara.diameter <- 6.0f
        gpara.BasePara.center <- new Vector3(4.0f, 0.0f, 0.0f)
        this.grating <- new Grating(this.GraphicsDevice, this.Services, this.SLConfig.["content"], gpara)
        
    override this.SetFlow() = 
        this.ex.Flow.TCount <- 0
        this.ex.Flow.SCount <- 0
        this.ex.Flow.IsPred <- false
        
    override this.MarkHead() = 
        this.ex.Rand.RandomizeSeed()
        this.ex.Rand.RandomizeSequence(this.ex.Expara.stimuli.[0])
        
        this.ex.PPort.MarkerEncode(this.ex.Extype.[0].Value)
        this.ex.PPort.MarkerEncode(this.ex.Cond.[0].SKEY)
        this.ex.PPort.MarkerEncode(this.ex.Cond.[0].VALUE.ValueN)
        this.ex.PPort.MarkerEncode(this.ex.Cond.[1].SKEY)
        this.ex.PPort.MarkerEncode(this.ex.Cond.[1].VALUE.ValueN)
        this.ex.PPort.MarkerEncode(this.ex.Cond.[2].SKEY)
        this.ex.PPort.MarkerEncode(this.ex.Cond.[2].VALUE.ValueN)
        this.ex.PPort.MarkerEncode(this.ex.Rand.RSeed)
        this.ex.PPort.MarkerEncode(this.ex.Expara.trial)
        
        this.ex.PPort.MarkerSeparatorEncode()
        
        this.ex.PPort.MarkerEncode(int( Math.Floor(float(this.grating.Para.tf) * 10.0) ))
        this.ex.PPort.MarkerEncode(int( Math.Floor(float(this.grating.Para.sf) * 100.0) ))
        this.ex.PPort.MarkerEncode(int( Math.Floor(float(this.grating.Para.luminance) * 10.0) ))
        this.ex.PPort.MarkerEncode(int( Math.Floor(float(this.grating.Para.contrast) * 10.0) ))
        this.ex.PPort.MarkerEncode(int( Math.Floor(float(this.grating.Para.BasePara.center.X + 60.0f) * 10.0) ))
        this.ex.PPort.MarkerEncode(int( Math.Floor(float(this.grating.Para.BasePara.center.Y + 60.0f) * 10.0) ))
        this.ex.PPort.MarkerEncode(int( Math.Floor(float this.grating.Para.BasePara.diameter) ))

        this.ex.PPort.MarkerEndEncode()
        this.ex.Flow.IsStiOn <- true
        
    override this.Update() = 
        if this.GO_OVER = true then
            this.Update_fGrating()
    override this.Draw() = 
        this.GraphicsDevice.Clear(this.ex.Expara.bgcolor)
        if this.GO_OVER = true then
            this.grating.Draw(this.GraphicsDevice)
            this.ex.Flow.Info <- this.ex.Flow.TCount.ToString() + " / " + this.ex.Expara.trial.ToString() + " Trials\n" + 
                                        this.ex.Flow.SCount.ToString() + " / " + this.ex.Expara.stimuli.[0].ToString() + " Stimuli"
            this.text.Draw(this.ex.Flow.Info)
        else
            this.text.Draw()
    member this.Update_fGrating() = 
        if this.ex.Flow.IsStiOn = true then
            this.ex.Flow.IsStiOn <- false
            this.ex.PPort.timer.ReStart()
            do this.ex.PPort.Trigger()
        if this.ex.PPort.timer.ElapsedSeconds < float this.ex.Expara.durT + 0.001 then
            if this.ex.Flow.IsPred = false then
                this.ex.Flow.IsPred <- true
                
                let condpara = this.ex.GetCondition(this.ex.Rand.RSequence.[this.ex.Flow.SCount])
                
                this.grating.SetSPhase( condpara.[2] )
                this.grating.SetSF( condpara.[1] )
                this.ex.Flow.Translate <- Matrix.CreateRotationZ(condpara.[0] * float32 SLConstant.RadpDeg) * Matrix.CreateTranslation(this.grating.Para.BasePara.center)
                this.grating.SetWorld(this.ex.Flow.Translate)
        else
            if this.ex.Flow.SCount - this.ex.Expara.stimuli.[0] < -1 then
                this.ex.Flow.IsStiOn <- true
                this.ex.Flow.IsPred <- false
                this.ex.Flow.SCount <- this.ex.Flow.SCount + 1
            else
                if this.ex.Flow.TCount - this.ex.Expara.trial < -1 then
                    this.ex.Rand.RandomizeSequence(this.ex.Expara.stimuli.[0])
                    this.ex.Flow.IsStiOn <- true
                    this.ex.Flow.IsPred <- false
                    this.ex.Flow.TCount <- this.ex.Flow.TCount + 1
                    this.ex.Flow.SCount <- 0
                else
                    this.GO_OVER <- false
            this.Update()
        
end

let MyExperiment = new MyEx(Text = "F# Scripting fGrating(Auto)")
Application.Run(MyExperiment)