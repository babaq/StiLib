// F# Script File : Dir_SF_IOI.fsx
//
// Direction and spatial frequency test for direction/orientation map and v1/v2 border in intransic optic imaging
//
// Copyright (c) 2010-01-11 Zhang Li

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
        this.text <- new Text(this.GraphicsDevice, this.Services, this.SLConfig.["content"], "Arial")
        this.ex <- new SLExperiment()
        this.ex.AddCondition(ExPara.Orientation, 2)
        this.ex.AddCondition(ExPara.SpatialFreq, 4)
        this.ex.Exdesign.srestT <- 1.0f
        this.ex.Exdesign.durT <- 4.0f
        this.ex.Exdesign.bgcolor <- Color.Gray
        this.ex.InitEx()
        // Prepare for ParallelPort DataPort Reading
        this.ex.PPort.IsDataOutput <- false
        
        let mutable gpara = GratingPara.Default
        gpara.tf <- 3.0f
        gpara.sphase <- 0.0f
        gpara.BasePara.diameter <- 15.0f 
        gpara.BasePara.center <- new Vector3(0.0f, 0.0f, 0.0f)
        this.grating <- new Grating(this.GraphicsDevice, this.Services, this.SLConfig.["content"], gpara)
        
        // Init Flow Control Parameters
        this.SetFlow()
        
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
            this.ex.PPort.Timer.Rest(float this.ex.Exdesign.srestT)
            this.ex.PPort.Timer.ReStart()
            
        if this.ex.PPort.Timer.ElapsedSeconds < float this.ex.Exdesign.durT then
            if this.ex.Flow.IsPred = false then
                this.ex.Flow.IsPred <- true
                
                let condpara = this.ex.GetCondition(this.ex.Flow.StiCount)
                this.grating.SetSF(condpara.[1])
                this.grating.Ori3DMatrix <- Matrix.CreateRotationZ(condpara.[0] * float32 SLConstant.Rad_p_Deg)
                this.grating.WorldMatrix <- this.ex.Flow.TranslateCenter
            this.grating.SetTime(float32 this.ex.PPort.Timer.ElapsedSeconds)
        else
            this.ex.Flow.IsPred <- false
            this.ex.Flow.IsBlanked <- false
            this.GO_OVER <- false
            
end

let MyExperiment = new MyEx(Text = "F# Scripting Dir_SF_IOI")
Application.Run(MyExperiment)