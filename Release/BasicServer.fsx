// F# Script File : BasicServer.fsx
//
// StiLib Basic Experiment Server In F# Scripting
//
// Copyright (c) 2009-10-17 Zhang Li

#r @"StiLib.dll"
#r @"Microsoft.Xna.Framework.dll"
#r @"System.ServiceModel"

open System
open System.Windows.Forms
open Microsoft.Xna.Framework.Graphics
open StiLib.Core
open StiLib.Vision
open System.ServiceModel
open System.ServiceModel.Description

// Our Custom Experiment Server is inherited from StiLib.Core.ExService
[<ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)>]
type MyEx = class
    inherit ExService
    // Our Stimulus
    val mutable text: Text
    val mutable grating: Grating
    // Init Experiment
    new() as this = 
        { inherit ExService(800, 600, 0, true, true); grating = null; text = null }
        then
        this.text <- new Text(this.GraphicsDevice, this.Services, this.SLConfig.["content"], "SegoeUI")
        this.grating <- new Grating(this.GraphicsDevice, this.Services, this.SLConfig.["content"])
    // Override methods to custom your own experiment logic
    override this.Draw() = 
        this.GraphicsDevice.Clear(Color.Gray)
        if this.GO_OVER = true then
            this.grating.Draw(this.GraphicsDevice)
        else
            this.text.Draw()
end

let MyExperiment = new MyEx(Text = "BasicServer In F# Scripting")

// Server Hosting
let Host = new ServiceHost(MyExperiment, new Uri("net.tcp://" + MyExperiment.SLConfig.["localhost"] + ":8080/ExServer"))
Host.AddServiceEndpoint(typeof<IExService>, new NetTcpBinding(SecurityMode.None), "")

let mutable metabehavior = new ServiceMetadataBehavior()
metabehavior.HttpGetEnabled <- false
Host.Description.Behaviors.Add(metabehavior)
Host.AddServiceEndpoint(typeof<IMetadataExchange>, MetadataExchangeBindings.CreateMexTcpBinding(), "mex")

// Run Host
Host.Open()
Application.Run(MyExperiment)
Host.Close