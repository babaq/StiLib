// F# Script File : Basic.fsx
//
// StiLib Basic F# Scripting
//
// Copyright (c) 2009-02-08 Zhang Li

#r @"StiLib.dll"
#r @"Microsoft.Xna.Framework.dll"

open System.Windows.Forms
open Microsoft.Xna.Framework.Graphics
open StiLib.Core
open StiLib.Vision

// Our Custom Experiment is inherited from StiLib.Core.SLForm
type MyEx = class
    inherit SLForm
    // Our Stimulus
    val mutable grating: Grating
    // Init Experiment
    new() as this = 
        { inherit SLForm(800, 600, 0, true, true); grating = null }
        then
        this.grating <- new Grating(this.GraphicsDevice, this.Services, this.SLConfig.["content"])
    // Override methods to custom your own experiment logic
    override this.Draw() = 
        this.GraphicsDevice.Clear(Color.Gray)
        this.grating.Draw(this.GraphicsDevice)
end

let MyExperiment = new MyEx(Text = "Basic F# Scripting")
Application.Run(MyExperiment)