// F# Script File : MultiStimuli.fsx
//
// StiLib Multiple Viewports and Stimuli Example
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
    val mutable vsc: VSCollection<Primitive>
    val mutable dots: vscPrimitive

    new() as this = 
        { inherit SLForm(2, 800, 600, 0, false, false, false, false, Vector3.One); grating = null; text = null; image = null; quad = null; model = null;
         timer = new SLTimer(); viewport = new Viewport(); fi = new FrameInfo(); audio = null; vsc = null; dots = null }
        then
        this.audio <- new SLAudio(this.SLConfig.["content"] + "StiLib", 
                                               this.SLConfig.["content"] + "SLMWB", 
                                               this.SLConfig.["content"] + "SLSWB", 
                                               this.SLConfig.["content"] + "SLSB")
        this.audio.Update()
        this.audio.StartBgMusic("BgMusic")
        
        this.text <- new Text(this.GraphicsDevice, this.Services, this.SLConfig.["content"], "ArialLarge")
        
        let mutable gpara = GratingPara.Default
        gpara.BasePara.diameter <- 20.0f
        gpara.maskpara.masktype <- MaskType.Gaussian
        gpara.maskpara.BasePara.diameter <- 3.2f
        gpara.sf <- 0.25f
        gpara.BasePara.direction <- 45.0f
        this.grating <- new Grating(this.GraphicsDevice, this.Services, this.SLConfig.["content"], gpara)
        
        this.image <- new Image(this.GraphicsDevice, this.Services, this.SLConfig.["content"], "Turtle")
        
        let q = new Quad(24.0f, 8.0f)
        this.quad <- new SLQuad(this.GraphicsDevice, this.Services, this.SLConfig.["content"], "StiLib_Logo", q)
        
        this.model <- new SLModel(this.GraphicsDevice, this.Services, this.SLConfig.["content"], "teapot")
        this.model.ProjectionType <- ProjectionType.Perspective
        this.model.globalCamera.NearPlane <- 0.1f
        
        let mutable cpara = CollectionPara.Default
        cpara.CollectionCenter <- new Vector3(1.5f, 1.0f, 0.0f)
        cpara.CollectionSpeed <- new Vector3(0.002f, -0.001f, 0.0f)
        cpara.BasePara.center <- new Vector3(0.0f, 0.0f, 0.0f)
        cpara.BasePara.space <- 25.0f
        this.vsc <- new VSCollection<Primitive>(100, VSType.Primitive, this.GraphicsDevice, this.Services, this.SLConfig.["content"], PrimitivePara.Circle(0.6f, Color.Red, false))
//        this.dots <- new vscPrimitive(100, this.GraphicsDevice, PrimitivePara.Circle(0.6f, Color.Tomato, true), this.Services, this.SLConfig.["content"], cpara)
//        let mutable temp = this.dots.PrimitiveInstance.BasePara
//        temp.space <- 16.0f
//        this.dots.PrimitiveInstance.BasePara <- temp
        
        this.timer.Start()
        
    override this.Draw() = 
        this.audio.Update()
        this.fi.Update()
        this.vsc.Update(this.timer.ElapsedSeconds)
        //this.dots.Update(this.timer.ElapsedSeconds)
        
        let w = 800
        let h = 600
        let m = 10
        let vw = (w - 3 * m) / 2
        let vh = (h - 3 * m) / 2
        // Upper Left Viewport
        this.viewport.X <- m
        this.viewport.Y <- m
        this.viewport.Width <- vw
        this.viewport.Height <- vh
        this.GraphicsDevice.Viewport <- this.viewport
        this.GraphicsDevice.Clear(Color.Gray)
        
        this.quad.Draw(this.GraphicsDevice)
        let ss = "Unicode Text: ¦²¦³¦©¦«¦©¦¢ ¦Ô¦Í¦É¦Ã¦Ï¦Ä¦Å ¦Ó¦Å¦Ö¦Ó"
        let s = "Unicode Text: "
        let size = this.text.spriteFont.MeasureString(s)
        this.text.Draw(new Vector2(float32 this.viewport.Width / 2.0f, float32 this.viewport.Height / 2.0f - size.Y / 2.0f), s, Color.Gold, 
                            float32 this.timer.ElapsedSeconds / 3.0f, new Vector2(size.X / 2.0f, 0.0f), Vector2.One)
        
        // Upper Right ViewPort
        this.viewport.X <- 2 * m + vw
        this.viewport.Y <- m
        this.GraphicsDevice.Viewport <- this.viewport
        this.GraphicsDevice.Clear(Color.SlateGray)

        this.image.Draw(this.GraphicsDevice)
        this.vsc.BatchDraw(this.GraphicsDevice)
        //this.dots.Draw(this.GraphicsDevice)
        
        // Lower Left Viewport
        this.viewport.X <- m
        this.viewport.Y <- 2 * m + vh
        this.GraphicsDevice.Viewport <- this.viewport
        this.GraphicsDevice.Clear(Color.Gray)
        
        this.grating.SetTime(float32 this.timer.ElapsedSeconds)
        this.grating.Draw(this.GraphicsDevice)
        this.text.Draw(new Vector2(5.0f, 5.0f), this.fi.FPS.ToString("F0") + " FPS" , Color.SkyBlue)
        
        // Lower Right Viewport
        this.viewport.X <- 2 * m + vw
        this.viewport.Y <- 2 * m + vh
        this.GraphicsDevice.Viewport <- this.viewport
        this.GraphicsDevice.Clear(Color.SlateGray)
        
        if this.audio.IsInitialized = true then
            this.text.Draw(new Vector2(5.0f, 5.0f),"Audio System Initialized !", Color.LightPink)
        else
            this.text.Draw(new Vector2(5.0f, 5.0f),"Audio System Not Initialized !", Color.Red)
        
        this.model.Ori3DMatrix <- VisionStimulus.GetOri3DMatrix(new Vector3(float32 this.timer.ElapsedSeconds))
        this.model.Draw()
        
end

let MyExperiment = new MyEx(Text = "Multiple Viewports and Stimuli")
Application.Run(MyExperiment)
