// F# Script File : CenterSurround_IOI.fsx
//
// Center-Surround intransic optic imaging experiment
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
    val mutable cgrating: Grating
    val mutable sgrating: Grating
    val mutable cmask: Primitive
    
    new() as this = 
        { inherit SLForm(null); cgrating = null; text = null; ex = null; sgrating = null; cmask = null }
        then
        this.text <- new Text(this.GraphicsDevice, this.Services, this.SLConfig.["content"], "Arial")
        this.ex <- new SLExperiment()
        this.ex.AddCondition(ExPara.Orientation, 2)
        this.ex.AddCondition(ExPara.Orientation, 2)
        this.ex.Exdesign.preT <- 3.0f
        this.ex.Exdesign.durT <- 6.0f
        this.ex.Exdesign.posT <- 4.0f
        this.ex.Exdesign.bgcolor <- Color.Gray
        this.ex.PPort.IsDataOutput <- false
        
        let mutable gpara = GratingPara.Default
        gpara.tf <- 3.0f
        gpara.sf <- 0.2f
        gpara.sphase <- 0.0f
        gpara.BasePara.direction <- 0.0f
        gpara.BasePara.diameter <- 3.0f // Center Size
        gpara.BasePara.center <- new Vector3(0.0f, 0.0f, 0.0f) // Center Center
        this.cgrating <- new Grating(this.GraphicsDevice, this.Services, this.SLConfig.["content"], gpara)
        
        this.cmask <- new Primitive(this.GraphicsDevice, PrimitivePara.Circle(gpara.BasePara.diameter, this.ex.Exdesign.bgcolor, gpara.BasePara.center, this.ex.Exdesign.bgcolor, 100, true))
        this.cmask.Visible <- false
        
        gpara.BasePara.diameter <- 15.0f // Surround Size
        gpara.BasePara.center <- new Vector3(0.0f, 0.0f, 0.0f) // Surround Center
        this.sgrating <- new Grating(this.GraphicsDevice, this.Services, this.SLConfig.["content"], gpara)
        
        this.SetFlow()
        
    override this.SetFlow() = 
        this.ex.Flow.IsPred <- false
        this.ex.Flow.IsBlanked <- false
        
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
            this.sgrating.Draw(this.GraphicsDevice)
            this.cmask.Draw(this.GraphicsDevice)
            this.cgrating.Draw(this.GraphicsDevice)
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
                
                this.ex.Flow.RowCount <- int( Math.Floor( float(this.ex.Flow.StiCount / (this.ex.Cond.[1].VALUE.ValueN + 1)) ) )
                this.ex.Flow.ColumnCount <- this.ex.Flow.StiCount % (this.ex.Cond.[1].VALUE.ValueN + 1)
                
                let mutable crad = 0.0f // cgrating orientation
                // Single condition of cgrating
                if this.ex.Cond.[0].VALUE.ValueN = 0 then
                    crad <- this.cgrating.Para.BasePara.direction * float32 SLConstant.Rad_p_Deg
                    this.cgrating.Ori3DMatrix <- Matrix.CreateRotationZ(crad)
                    this.cgrating.WorldMatrix <- Matrix.CreateTranslation(this.cgrating.Para.BasePara.center)
                else // Multiple condition of cgrating
                    if this.ex.Flow.RowCount = 0 then // Blank
                        this.cgrating.Visible <- false
                        this.cmask.Visible <- true
                    else
                        crad <- float32 (this.ex.Flow.RowCount - 1) * float32 Math.PI / float32 this.ex.Cond.[0].VALUE.ValueN
                        this.cgrating.Ori3DMatrix <- Matrix.CreateRotationZ(crad)
                        this.cgrating.WorldMatrix <- Matrix.CreateTranslation(this.cgrating.Para.BasePara.center)
                        this.cgrating.Visible <- true
                
                if this.ex.Flow.ColumnCount = 0 then // Blank
                    this.sgrating.Visible <- false
                else
                    this.sgrating.Ori3DMatrix <- Matrix.CreateRotationZ(crad + float32(this.ex.Flow.ColumnCount - 1) * float32 Math.PI / float32( this.ex.Cond.[1].VALUE.ValueN ))
                    this.sgrating.WorldMatrix <- Matrix.CreateTranslation(this.sgrating.Para.BasePara.center)
                    this.sgrating.Visible <- true
                       
            this.cgrating.SetTime(float32 this.ex.PPort.Timer.ElapsedSeconds)
            this.sgrating.SetTime(float32 this.ex.PPort.Timer.ElapsedSeconds)
        else
            this.DrawTip(ref this.text, this.ex.Exdesign.bgcolor, "Stimulus End !")
            this.ex.PPort.Timer.Rest(float this.ex.Exdesign.posT)
            this.ex.Flow.IsPred <- false
            this.ex.Flow.IsBlanked <- false
            this.cmask.Visible <- false
            this.GO_OVER <- false
            
end

let MyExperiment = new MyEx(Text = "F# Scripting CenterSurround_IOI")
Application.Run(MyExperiment)
