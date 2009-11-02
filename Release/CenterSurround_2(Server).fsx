// F# Script File : CenterSurround_2(Server).fsx
//
// Center-Surround Experiment(two marker) Server Version
//
// Copyright (c) 2009-10-19 Zhang Li

#r @"StiLib.dll"
#r @"Microsoft.Xna.Framework.dll"
#r @"System.ServiceModel"

open System
open System.Windows.Forms
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open StiLib.Core
open StiLib.Vision
open System.ServiceModel
open System.ServiceModel.Description

// Our Custom Experiment is inherited from StiLib.Core.ExService
[<ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)>]
type MyEx = class
    inherit ExService
    
    val mutable text: Text
    val mutable ex: SLExperiment
    val mutable cgrating: Grating
    val mutable sgrating: Grating
    val mutable cmask: Primitive
    
    new() as this = 
        { inherit ExService(null); cgrating = null; text = null; ex = null; sgrating = null; cmask = null }
        then
        this.text <- new Text(this.GraphicsDevice, this.Services, this.SLConfig.["content"], "Thames")
        this.ex <- new SLExperiment()
        this.ex.AddExType(ExType.Context)
        this.ex.AddCondition(ExPara.Orientation, 8)
        this.ex.AddCondition(ExPara.Orientation, 4)
        this.ex.Exdesign.trial <- 12
        this.ex.Exdesign.srestT <- 0.4f
        this.ex.Exdesign.trestT <- 0.4f
        this.ex.Exdesign.durT <- 1.0f
        this.ex.Exdesign.bgcolor <- Color.Gray
        
        let mutable gpara = GratingPara.Default
        gpara.tf <- 0.0f
        gpara.sf <- 0.8f
        gpara.sphase <- 0.0f
        gpara.BasePara.diameter <- 2.0f // Center Size
        gpara.BasePara.center <- new Vector3(0.0f, 0.5f, 0.0f) // Center Center
        this.cgrating <- new Grating(this.GraphicsDevice, this.Services, this.SLConfig.["content"], gpara)
        
        this.cmask <- new Primitive(this.GraphicsDevice, PrimitivePara.Circle(gpara.BasePara.diameter, this.ex.Exdesign.bgcolor, gpara.BasePara.center, this.ex.Exdesign.bgcolor, 100, true))
        this.cmask.Visible <- false
        
        gpara.BasePara.diameter <- 5.0f // Surround Size
        gpara.BasePara.center <- new Vector3(0.0f, 0.5f, 0.0f) // Surround Center
        this.sgrating <- new Grating(this.GraphicsDevice, this.Services, this.SLConfig.["content"], gpara)
        
    override this.GetExType() = 
        "CenterSurround_2"
        
    override this.GetGrating(index) = 
        if index=0 then
            this.cgrating.Para
        else
            this.sgrating.Para
            
    override this.SetFlow() =         
        this.ex.Flow.TrialCount <- 0
        this.ex.Flow.StiCount <- 0
        this.ex.Flow.IsPred <- false
        this.ex.Flow.IsRested <- false
        
    override this.MarkHead() = 
        this.DrawTip(ref this.text, this.ex.Exdesign.bgcolor, SLConstant.MarkHead)
        
        this.ex.Exdesign.stimuli.[0] <- (this.ex.Cond.[0].VALUE.ValueN + 1) * (this.ex.Cond.[1].VALUE.ValueN + 1)
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
            this.cmask.Draw(this.GraphicsDevice)
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
                // Delayed Notification of Service Client that one experiment block has stoped.
                this.Draw()
                this.GraphicsDevice.Present()
                this.ex.PPort.Timer.Rest(1.0)
                this.OnRunStop(this.GO_OVER)
                ()
        if this.ex.PPort.Timer.ElapsedSeconds < float this.ex.Exdesign.durT then
            if this.ex.Flow.IsPred = false then
                this.ex.Flow.IsPred <- true
                
                this.ex.Flow.RowCount <- int( Math.Floor( float(this.ex.Rand.Sequence.[this.ex.Flow.StiCount] / (this.ex.Cond.[1].VALUE.ValueN + 1)) ) )
                this.ex.Flow.ColumnCount <- this.ex.Rand.Sequence.[this.ex.Flow.StiCount] % (this.ex.Cond.[1].VALUE.ValueN + 1)
                
                let mutable crad = 0.0f // cgrating orientation
                // Single condition of cgrating
                if this.ex.Cond.[0].VALUE.ValueN = 0 then
                    crad <- this.cgrating.Para.BasePara.direction * float32 SLConstant.Rad_p_Deg
                    this.cgrating.Ori3DMatrix <- Matrix.CreateRotationZ(crad)
                    this.cgrating.WorldMatrix <- Matrix.CreateTranslation(this.cgrating.Para.BasePara.center)
                    this.cgrating.Visible <- true
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
                
                this.ex.Flow.IsStiOn <- true
                    
            this.cgrating.SetTime(float32 this.ex.PPort.Timer.ElapsedSeconds)
            this.sgrating.SetTime(float32 this.ex.PPort.Timer.ElapsedSeconds)
        else
            if this.ex.Flow.IsRested = false then
                this.ex.Flow.IsRested <- true
                this.cgrating.Visible <- false
                this.sgrating.Visible <- false
                this.cmask.Visible <- false
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

let MyExperiment = new MyEx(Text = "F# Scripting CenterSurround_2(Server)")

// Server Hosting
let Host = new ServiceHost(MyExperiment, new Uri("net.tcp://" + MyExperiment.SLConfig.["localhost"] + ":8081/ExServer"))
Host.AddServiceEndpoint(typeof<IExService>, new NetTcpBinding(SecurityMode.None), "")

let mutable metabehavior = new ServiceMetadataBehavior()
metabehavior.HttpGetEnabled <- false
Host.Description.Behaviors.Add(metabehavior)
Host.AddServiceEndpoint(typeof<IMetadataExchange>, MetadataExchangeBindings.CreateMexTcpBinding(), "mex")

// Run Host
Host.Open()
Application.Run(MyExperiment)
Host.Close()
