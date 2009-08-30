// F# Script File : CenterSurround_1.fsx
//
// Center-Surround Experiment(one marker)
//
// Copyright (c) 2009-04-01 Zhang Li

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
    val mutable sgrating: Grating
    
    new() as this = 
        { inherit SLForm(null); cgrating = null; text = null; ex = null; sgrating = null }
        then
        this.text <- new Text(this.GraphicsDevice, this.Services, this.SLConfig.["content"], "Thames")
        this.ex <- new SLExperiment()
        this.ex.AddExType(ExType.Context)
        this.ex.AddCondition(ExPara.Orientation, 2)
        this.ex.AddCondition(ExPara.Orientation, 2)
        this.ex.Exdesign.trial <- 2
        this.ex.Exdesign.durT <- 0.5f
        this.ex.Exdesign.bgcolor <- Color.Gray
        
        let mutable gpara = GratingPara.Default
        gpara.tf <- 0.0f
        gpara.sf <- 0.8f
        gpara.sphase <- 0.25f
        gpara.BasePara.diameter <- 6.0f
        gpara.BasePara.center <- new Vector3(4.0f, 0.0f, 0.0f)
        this.cgrating <- new Grating(this.GraphicsDevice, this.Services, this.SLConfig.["content"], gpara)
        gpara.BasePara.diameter <- 12.0f
        this.sgrating <- new Grating(this.GraphicsDevice, this.Services, this.SLConfig.["content"], gpara)
        
    override this.SetFlow() = 
        this.ex.Flow.TrialCount <- 0
        this.ex.Flow.StiCount <- 0
        this.ex.Flow.IsPred <- false
        
    override this.MarkHead() = 
        this.DrawTip(ref this.text, this.ex.Exdesign.bgcolor, SLConstant.MarkHead)
        
        this.ex.Exdesign.stimuli.[0] <- this.ex.Cond.[0].VALUE.ValueN * (this.ex.Cond.[1].VALUE.ValueN + 1)
        this.ex.Rand.RandomizeSequence(this.ex.Exdesign.stimuli.[0])
        
        this.ex.PPort.MarkerEncode(this.ex.Extype.[0].Value)
        this.ex.PPort.MarkerEncode(this.ex.Cond.[0].SKEY)
        this.ex.PPort.MarkerEncode(this.ex.Cond.[0].VALUE.ValueN)
        this.ex.PPort.MarkerEncode(this.ex.Cond.[1].SKEY)
        this.ex.PPort.MarkerEncode(this.ex.Cond.[1].VALUE.ValueN)
        this.ex.PPort.MarkerEncode(this.ex.Rand.Seed)
        this.ex.PPort.MarkerEncode(this.ex.Exdesign.trial)
        
        this.ex.PPort.MarkerSeparatorEncode()
        
        this.cgrating.Para.Encode(this.ex.PPort)
        this.sgrating.Para.Encode(this.ex.PPort)

        this.ex.PPort.MarkerEndEncode()
        this.ex.PPort.Timer.Reset()
        
    override this.Update() = 
        if this.GO_OVER = true then
            this.Update_Grating()
    override this.Draw() = 
        this.GraphicsDevice.Clear(this.ex.Exdesign.bgcolor)
        if this.GO_OVER = true then
            this.sgrating.Draw(this.GraphicsDevice)
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
        if this.ex.PPort.Timer.ElapsedSeconds < float this.ex.Exdesign.durT + 0.001 then
            if this.ex.Flow.IsPred = false then
                this.ex.Flow.IsPred <- true
                
                this.ex.Flow.RowCount <- int( Math.Floor( float(this.ex.Rand.Sequence.[this.ex.Flow.StiCount] / this.ex.Cond.[1].VALUE.ValueN) ) )
                this.ex.Flow.ColumnCount <- this.ex.Rand.Sequence.[this.ex.Flow.StiCount] % this.ex.Cond.[1].VALUE.ValueN
                
                let crad = float32 this.ex.Flow.RowCount * float32 Math.PI / float32 this.ex.Cond.[0].VALUE.ValueN
                this.cgrating.Ori3DMatrix <- Matrix.CreateRotationZ(crad)
                this.cgrating.WorldMatrix <- Matrix.CreateTranslation(this.cgrating.Para.BasePara.center)
                
                if this.ex.Flow.ColumnCount = 0 then
                    this.sgrating.Visible <- false
                else
                    this.sgrating.Ori3DMatrix <- Matrix.CreateRotationZ(crad + float32(this.ex.Flow.ColumnCount - 1) * float32 Math.PI / float32( this.ex.Cond.[1].VALUE.ValueN - 1 ))
                    this.sgrating.WorldMatrix <- Matrix.CreateTranslation(this.sgrating.Para.BasePara.center)
                    this.sgrating.Visible <- true
                
                this.ex.Flow.IsStiOn <- true
                    
            this.cgrating.SetTime(float32 this.ex.PPort.Timer.ElapsedSeconds)
            this.sgrating.SetTime(float32 this.ex.PPort.Timer.ElapsedSeconds)
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

let MyExperiment = new MyEx(Text = "F# Scripting CenterSurround_1")
Application.Run(MyExperiment)
