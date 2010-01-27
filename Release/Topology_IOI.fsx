// F# Script File : Topology_IOI.fsx
//
// Mapping Topology Relationship between Visual Field and Cortex Structure Map in intransic optic imaging
//
// Copyright (c) 2010-01-16 Zhang Li

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
        { inherit SLForm(null); grating = null; text = null; ex = null; step = 0.0f }
        then
        this.text <- new Text(this.GraphicsDevice, this.Services, this.SLConfig.["content"], "Arial")
        this.ex <- new SLExperiment()
        this.ex.Exdesign.preT <- 3.0f
        this.ex.Exdesign.durT <- 6.0f
        this.ex.Exdesign.posT <- 4.0f
        this.ex.Exdesign.bgcolor <- Color.Gray
        this.ex.PPort.IsDataOutput <- false
        
        let mutable gpara = GratingPara.Default
        gpara.tf <- 3.0f
        gpara.sf <- 0.2f
        gpara.sphase <- 0.0f
        gpara.BasePara.diameter <- 3.0f
        gpara.BasePara.space <- 9.0f
        gpara.BasePara.direction <- 0.0f
        gpara.BasePara.center <- new Vector3(0.0f, 0.0f, 0.0f)
        this.grating <- new Grating(this.GraphicsDevice, this.Services, this.SLConfig.["content"], gpara)
        
        this.step <- 3.0f
        this.InitGrid()
        this.SetFlow()
        
    member this.InitGrid() = 
        let mutable row = int( Math.Floor(float( this.grating.Para.BasePara.space / this.step )) )
        if row % 2 = 0 then
            row <- row + 1
        this.ex.AddCondition(ExPara.Location, row)
        
    override this.SetFlow() = 
        this.ex.Flow.IsPred <- false
        this.ex.Flow.IsBlanked <- false
        this.ex.Flow.TranslateCenter <- Matrix.CreateTranslation(this.grating.Para.BasePara.center)
        
    override this.Update() = 
        if this.GO_OVER = true then
            this.Update_Grating()
        else
            let signal = this.ex.PPort.GetData()
            // According to defination of Optic Imaging Ltd., bit 7 is the GO bit,
            // and bit 0-6 can be used to represent Stimulus ID: 0-127
            if signal > 127 then
                this.ex.Flow.StiCount <- signal - 128
                this.GO_OVER <- true
                this.Update_Grating()
                
    override this.Draw() = 
        this.GraphicsDevice.Clear(this.ex.Exdesign.bgcolor)
        if this.GO_OVER = true then
            this.grating.Draw(this.GraphicsDevice)
            this.ex.Flow.Info <- "Stimulus ID: " + this.ex.Flow.StiCount.ToString()
            this.text.Draw(this.ex.Flow.Info)
        else
            this.text.Draw("Waiting Signal . . .")
    member this.Update_Grating() = 
        if this.ex.Flow.IsBlanked = false then
            this.DrawTip(ref this.text, this.ex.Exdesign.bgcolor, "Stimulus Begin !")
            this.ex.Flow.IsBlanked <- true
            this.ex.PPort.Timer.Rest(float this.ex.Exdesign.preT)
            this.ex.PPort.Timer.ReStart()
            
        if this.ex.PPort.Timer.ElapsedSeconds < float this.ex.Exdesign.durT then
            if this.ex.Flow.IsPred = false then
                this.ex.Flow.IsPred <- true
                
                let RCount = Math.Floor(float (this.ex.Flow.StiCount / this.ex.Cond.[0].VALUE.ValueN))
                let CCount = this.ex.Flow.StiCount % this.ex.Cond.[0].VALUE.ValueN
                
                let Xgrid = -float32(this.ex.Cond.[0].VALUE.ValueN - 1) * this.step / 2.0f + this.step * float32(CCount)
                let Ygrid = float32(this.ex.Cond.[0].VALUE.ValueN - 1) * this.step / 2.0f - this.step * float32(RCount)
                this.grating.WorldMatrix <- Matrix.CreateTranslation(Xgrid, Ygrid, 0.0f) * this.ex.Flow.TranslateCenter                
            this.grating.SetTime(float32 this.ex.PPort.Timer.ElapsedSeconds)
        else
            this.DrawTip(ref this.text, this.ex.Exdesign.bgcolor, "Stimulus End !")
            this.ex.PPort.Timer.Rest(float this.ex.Exdesign.posT)
            this.ex.Flow.IsPred <- false
            this.ex.Flow.IsBlanked <- false
            this.GO_OVER <- false
            
end

let MyExperiment = new MyEx(Text = "F# Scripting Topology_IOI")
Application.Run(MyExperiment)
