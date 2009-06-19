// F# Script File : RF_Center_2.fsx
//
// Mapping Receptive Field Center Location Use Drift Grating(two marker)
//
// Copyright (c) 2009-06-13 Zhang Li

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
    val mutable step: float32
    
    new() as this = 
        { inherit SLForm(800, 600, 0, true, true); grating = null; text = null; ex = null; step = 0.0f }
        then
        this.text <- new Text(this.GraphicsDevice, this.Services, this.SLConfig.["content"], "Thames")
        this.ex <- new SLExperiment()
        this.ex.AddExType(ExType.RF)
        
        this.ex.Expara.trial <- 15
        this.ex.Expara.srestT <- 0.25f
        this.ex.Expara.trestT <- 0.25f
        this.ex.Expara.durT <- 0.5f
        this.ex.Expara.bgcolor <- Color.Black
        
        let mutable gpara = GratingPara.Default
        gpara.tf <- 4.0f
        gpara.sf <- 0.8f
        gpara.sphase <- 0.0f
        gpara.BasePara.diameter <- 2.0f
        gpara.BasePara.movearea <- 5.0f
        gpara.direction <- 90.0f
        gpara.BasePara.center <- new Vector3(0.0f, 0.0f, 0.0f)
        this.grating <- new Grating(this.GraphicsDevice, this.Services, this.SLConfig.["content"], gpara)
        
        this.step <- 0.25f
        this.InitGrid()
        
    member this.InitGrid() = 
        let mutable row = int( Math.Floor(float( this.grating.Para.BasePara.movearea / this.step )) )
        if row % 2 = 0 then
            row <- row + 1
        this.ex.AddCondition(ExPara.Location, row)
        
    override this.SetFlow() = 
        this.ex.Flow.TCount <- 0
        this.ex.Flow.SCount <- 0
        this.ex.Flow.IsPred <- false
        this.ex.Flow.IsRested <- false
        
    override this.MarkHead() = 
        this.DrawTip(ref this.text, this.ex.Expara.bgcolor, SLConstant.MarkHead)
        
        this.ex.Expara.stimuli.[0] <- this.ex.Cond.[0].VALUE.ValueN * this.ex.Cond.[0].VALUE.ValueN
        this.ex.Rand.RandomizeSeed()
        this.ex.Rand.RandomizeSequence(this.ex.Expara.stimuli.[0])
        
        this.ex.PPort.MarkerEncode(this.ex.Extype.[0].Value)
        this.ex.PPort.MarkerEncode(this.ex.Cond.[0].SKEY)
        this.ex.PPort.MarkerEncode(this.ex.Cond.[0].VALUE.ValueN)
        this.ex.PPort.MarkerEncode(this.ex.Rand.RSeed)
        this.ex.PPort.MarkerEncode(this.ex.Expara.trial)
        
        this.ex.PPort.MarkerSeparatorEncode()
        
        this.grating.Para.Encode(this.ex.PPort)
        this.ex.PPort.MarkerEncode(int( Math.Floor(float(this.step) * 100.0) ))

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
        if this.ex.PPort.timer.ElapsedSeconds < float this.ex.Expara.durT then
            if this.ex.Flow.IsPred = false then
                this.ex.Flow.IsPred <- true
                
                let RCount = Math.Floor(float (this.ex.Rand.RSequence.[this.ex.Flow.SCount] / this.ex.Cond.[0].VALUE.ValueN))
                let CCount = this.ex.Rand.RSequence.[this.ex.Flow.SCount] % this.ex.Cond.[0].VALUE.ValueN
                
                let Xgrid = -float32(this.ex.Cond.[0].VALUE.ValueN - 1) * this.step / 2.0f + this.step * float32(CCount)
                let Ygrid = float32(this.ex.Cond.[0].VALUE.ValueN - 1) * this.step / 2.0f - this.step * float32(RCount)
                this.ex.Flow.Rotate <- Matrix.CreateRotationZ(this.grating.Para.direction * float32 SLConstant.RadpDeg)
                this.ex.Flow.Translate <- Matrix.CreateTranslation(Xgrid, Ygrid, 0.0f) * this.ex.Flow.Rotate * Matrix.CreateTranslation(this.grating.Para.BasePara.center)
                this.grating.SetWorld(this.ex.Flow.Translate)
                this.grating.SetVisible(true)
                
            this.grating.SetTime(float32 this.ex.PPort.timer.ElapsedSeconds)
        else
            if this.ex.Flow.IsRested = false then
                do this.ex.PPort.Trigger()
                this.ex.Flow.IsRested <- true
                this.grating.SetVisible(false)
            if this.ex.Flow.SCount - this.ex.Expara.stimuli.[0] < -1 then
                if this.ex.PPort.timer.ElapsedSeconds > float this.ex.Expara.durT + float this.ex.Expara.srestT then
                    this.ex.Flow.IsStiOn <- true
                    this.ex.Flow.IsPred <- false
                    this.ex.Flow.IsRested <- false
                    this.ex.Flow.SCount <- this.ex.Flow.SCount + 1
            else
                if this.ex.Flow.TCount - this.ex.Expara.trial < -1 then
                    if this.ex.PPort.timer.ElapsedSeconds > float this.ex.Expara.durT + float this.ex.Expara.trestT then
                        this.ex.Rand.RandomizeSequence(this.ex.Expara.stimuli.[0])
                        this.ex.Flow.IsStiOn <- true
                        this.ex.Flow.IsPred <- false
                        this.ex.Flow.IsRested <- false
                        this.ex.Flow.TCount <- this.ex.Flow.TCount + 1
                        this.ex.Flow.SCount <- 0
                else
                    this.GO_OVER <- false
        
end

let MyExperiment = new MyEx(Text = "F# Scripting RF_Center_2")
Application.Run(MyExperiment)