// F# Script File : fGrating.fsx
//
// Examine basic properties of a neuron using reverse-correlation flashing grating
//
// Copyright (c) 2009-03-18 Zhang Li

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
        this.ex.AddExType(ExType.fGrating)
        this.ex.AddCondition(ExPara.Orientation, 8)
        this.ex.AddCondition(ExPara.SpatialFreq, 5)
        this.ex.AddCondition(ExPara.SpatialPhase, 4)
        this.ex.Exdesign.trial <- 2
        this.ex.Exdesign.durT <- 0.030f
        this.ex.Exdesign.bgcolor <- Color.Gray
        
        let mutable gpara = GratingPara.Default
        gpara.BasePara.diameter <- 2.5f
        gpara.BasePara.center <- new Vector3(0.0f, 0.0f, 0.0f)
        this.grating <- new Grating(this.GraphicsDevice, this.Services, this.SLConfig.["content"], gpara)
        
    override this.SetFlow() = 
        this.ex.Flow.TrialCount <- 0
        this.ex.Flow.StiCount <- 0
        this.ex.Flow.IsPred <- false
        
        this.ex.Flow.TranslateCenter <- Matrix.CreateTranslation(this.grating.Para.BasePara.center)
        
    override this.MarkHead() = 
        this.DrawTip(ref this.text, this.ex.Exdesign.bgcolor, SLConstant.MarkHead)
        
        this.ex.Exdesign.stimuli.[0] <- this.ex.Cond.[0].VALUE.ValueN * this.ex.Cond.[1].VALUE.ValueN * this.ex.Cond.[2].VALUE.ValueN
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

        this.ex.PPort.MarkerEndEncode()
        this.ex.PPort.Timer.Reset()
        
    override this.Update() = 
        if this.GO_OVER = true then
            this.Update_Grating()
    override this.Draw() = 
        this.GraphicsDevice.Clear(this.ex.Exdesign.bgcolor)
        if this.GO_OVER = true then
            this.grating.Draw(this.GraphicsDevice)
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
        if this.ex.PPort.Timer.ElapsedSeconds < float this.ex.Exdesign.durT then
            if this.ex.Flow.IsPred = false then
                this.ex.Flow.IsPred <- true
                
                this.ex.Flow.RowCount <- int( Math.Floor( float(this.ex.Rand.Sequence.[this.ex.Flow.StiCount] / this.ex.Cond.[1].VALUE.ValueN / this.ex.Cond.[2].VALUE.ValueN) ) )
                let t = this.ex.Rand.Sequence.[this.ex.Flow.StiCount] % (this.ex.Cond.[1].VALUE.ValueN * this.ex.Cond.[2].VALUE.ValueN)
                this.ex.Flow.ColumnCount <- int( Math.Floor( float(t /  this.ex.Cond.[2].VALUE.ValueN) ) )
                this.ex.Flow.SliceCount <- t % this.ex.Cond.[2].VALUE.ValueN
                
                this.grating.SetSPhase( float32(this.ex.Flow.SliceCount / this.ex.Cond.[2].VALUE.ValueN) )
                this.grating.SetSF( 0.1f * float32(Math.Pow(2.0, float this.ex.Flow.ColumnCount)) )
                this.grating.Ori3DMatrix <- Matrix.CreateRotationZ(float32 this.ex.Flow.RowCount * float32 Math.PI / float32 this.ex.Cond.[0].VALUE.ValueN)
                this.grating.WorldMatrix <- this.ex.Flow.TranslateCenter
                
                this.ex.Flow.IsStiOn <- true
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

let MyExperiment = new MyEx(Text = "F# Scripting fGrating")
Application.Run(MyExperiment)
