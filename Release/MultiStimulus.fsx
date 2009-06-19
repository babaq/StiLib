// F# Script File : MultiStimulus.fsx
//
// StiLib Multiple Viewport and Stimulus Example
//
// Copyright (c) 2009-03-28 Zhang Li

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

    val mutable viewport: Viewport
    val mutable text: Text
    val mutable grating: Grating
    val mutable image: Image
    val mutable quad: SLQuad
    val mutable model: SLModel
    val mutable timer: SLTimer
    val mutable fi: FrameInfo
    val mutable audio: SLAudio

    new() as this = 
        { inherit SLForm(800, 600, 0, false, false); grating = null; text = null; image = null; quad = null; model = null;
         timer = new SLTimer(); viewport = new Viewport(); fi = new FrameInfo(); audio = null; }
        then
        this.timer.Start()
        this.audio <- new SLAudio(this.SLConfig.["content"] + "StiLib", 
                                               this.SLConfig.["content"] + "SLMWB", 
                                               this.SLConfig.["content"] + "SLSWB", 
                                               this.SLConfig.["content"] + "SLSB")
        this.audio.Update()
        this.audio.StartBgMusic("BgMusic")
        this.text <- new Text(this.GraphicsDevice, this.Services, this.SLConfig.["content"], "ArialLarge")
        this.grating <- new Grating(this.GraphicsDevice, this.Services, this.SLConfig.["content"])
        this.grating.SetDiameter(25.0f)
        this.grating.SetMask(MaskType.Gaussian)
        this.grating.SetSigma(3.5f)
        this.image <- new Image(this.GraphicsDevice, this.Services, this.SLConfig.["content"], "Turtle")
        this.quad <- new SLQuad(this.GraphicsDevice, this.Services, this.SLConfig.["content"], "StiLib_Logo")
        this.model <- new SLModel(this.GraphicsDevice, this.Services, this.SLConfig.["content"], "teapot")
        this.model.gcamera.projtype <- ProjectionType.Perspective
        this.model.gcamera.NearPlane <- 0.1f
        
    override this.Draw() = 
        this.audio.Update()
        this.fi.Update()
        
        // Upper Left Viewport
        this.viewport.X <- 6
        this.viewport.Y <- 6
        this.viewport.Width <- 391
        this.viewport.Height <- 291
        this.GraphicsDevice.Viewport <- this.viewport
        this.GraphicsDevice.Clear(Color.Gray)
        
        this.quad.Draw(this.GraphicsDevice)
        this.text.Draw(new Vector2(160.0f, 130.0f), "Unicode Text: ", Color.Gold, 
                            float32 this.timer.ElapsedSeconds / 3.0f, new Vector2(60.0f, 40.0f), 1.0f)
        
        // Upper Right ViewPort
        this.viewport.X <- 403
        this.viewport.Y <- 6
        this.GraphicsDevice.Viewport <- this.viewport
        this.GraphicsDevice.Clear(Color.Gray)

        this.image.Draw(new Vector2(45.0f, 32.0f), Color.White)
        
        // Lower Left Viewport
        this.viewport.X <- 6
        this.viewport.Y <- 303
        this.GraphicsDevice.Viewport <- this.viewport
        this.GraphicsDevice.Clear(Color.Gray)
        
        this.grating.SetTime(float32 this.timer.ElapsedSeconds)
        this.grating.Draw(this.GraphicsDevice)
        this.text.Draw(new Vector2(5.0f, 5.0f), this.fi.FPS.ToString() , Color.SkyBlue)
        
        // Lower Right Viewport
        this.viewport.X <- 403
        this.viewport.Y <- 303
        this.GraphicsDevice.Viewport <- this.viewport
        this.GraphicsDevice.Clear(Color.Gray)
        
        if this.audio.IsInitialized = true then
            this.text.Draw(new Vector2(5.0f, 5.0f),"Audio System Initialized !", Color.LightPink)
        else
            this.text.Draw(new Vector2(5.0f, 5.0f),"Audio System Not Initialized !", Color.Red)
        
        this.model.RotateModel(new Vector3(float32 this.timer.ElapsedSeconds))
        this.model.Draw()
        
end

let MyExperiment = new MyEx(Text = "F# Scripting Multiple Viewport and Stimulus")
Application.Run(MyExperiment)