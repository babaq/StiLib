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
    inherit RF_dBar(800, 600, 0, true, true)
    
    override self.Initialize() =
        self.text <- new Text(self.GraphicsDevice, self.Services, self.SLConfig.["content"], "Arial")
        self.ex <- new SLExperiment()
        
        self.ex.AddExType(ExType.RF_dBar)
        self.ex.AddCondition(ExPara.Direction, 0)
        self.ex.Expara.trial <- 15
        self.ex.Expara.trestT <- 0.25f
        self.ex.Expara.srestT <- 0.25f
        self.ex.Expara.preT <- 0.25f
        self.ex.Expara.durT <- 1.0f
        self.ex.Expara.posT <- 0.25f
        self.ex.Expara.bgcolor <- Color.Black
        
        let mutable bpara = BarPara.Default
        bpara.width <- 0.5f
        bpara.height <- 1.0f
        bpara.direction <- 0.0f
        bpara.speed <- 10.0f
        bpara.BasePara.movearea <- 20.0f
        bpara.BasePara.center <- new Vector3(0.0f, 0.0f, 0.0f)
        bpara.BasePara.color <- Color.White
        self.Bar <- new Bar(self.GraphicsDevice, bpara)
        
        self.InitScan()
        
end

let Experiment = new ExRF_dBar(Text = "F# Scripting RF_dBar")
Application.Run(Experiment)

#if COMPILED
[<STAThread>]
do Application.Run(Experiment)
#endif