// F# Script File : RF_dBar.fsx
//
// Drifting Bar Receptive Field Mapping Diagram
//
// Copytight (c) 2009-03-12 Zhang Li

#r @"StiLib.dll"
#r @"Microsoft.Xna.Framework.dll"

open System
open System.Windows.Forms
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open StiLib.Core
open StiLib.Vision
open StiLib.Vision.Stimuli

// Our Custom Experiment is inherited from StiLib.Vision.Stimuli.RF_dBar
type ExRF_dBar() = class
    inherit RF_dBar(null)
    
    override self.Initialize() =
        self.text <- new Text(self.GraphicsDevice, self.Services, self.SLConfig.["content"], "Arial")
        self.ex <- new SLExperiment()
        
        self.ex.AddExType(ExType.RF_dBar)
        self.ex.AddCondition(ExPara.Direction, 0)
        self.ex.Exdesign.trial <- 2
        self.ex.Exdesign.trestT <- 0.25f
        self.ex.Exdesign.srestT <- 0.25f
        self.ex.Exdesign.preT <- 0.25f
        self.ex.Exdesign.durT <- 1.0f
        self.ex.Exdesign.posT <- 0.25f
        self.ex.Exdesign.bgcolor <- Color.Black
        
        let mutable bpara = BarPara.Default
        bpara.width <- 0.5f
        bpara.height <- 1.0f
        bpara.BasePara.direction <- 0.0f
        bpara.BasePara.speed <- 10.0f
        bpara.BasePara.space <- 20.0f
        bpara.BasePara.center <- new Vector3(0.0f, 0.0f, 0.0f)
        bpara.BasePara.color <- Color.White
        self.bar <- new Bar(self.GraphicsDevice, bpara)
        
        self.Step <- 1.0f
        self.InitScan()
        
end

let Experiment = new ExRF_dBar(Text = "F# Scripting RF_dBar")
Application.Run(Experiment)

#if COMPILED
[<STAThread>]
do Application.Run(Experiment)
#endif