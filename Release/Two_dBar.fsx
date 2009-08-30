// F# Script File : Two_dBar.fsx
//
// Two Drifting Bars Diagram
//
// Copytight (c) 2008-10-22 Zhang Li

#r @"StiLib.dll"
#r @"Microsoft.Xna.Framework.dll"

open System
open System.Windows.Forms
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open StiLib.Core
open StiLib.Vision
open StiLib.Vision.Stimuli

// Our Custom Experiment is inherited from StiLib.Vision.Stimuli.Two_dBar
type ExTwo_dBar() = class
    inherit Two_dBar(null)
    
    override self.Initialize() =
        self.text <- new Text(self.GraphicsDevice, self.Services, self.SLConfig.["content"], "Arial")
        self.ex <- new SLExperiment()
        
        self.ex.AddExType(ExType.Two_dBar)
        self.ex.AddCondition(ExPara.Direction, 4)
        self.ex.Exdesign.trial <- 2
        self.ex.Exdesign.trestT <- 1.0f
        self.ex.Exdesign.srestT <- 0.5f
        self.ex.Exdesign.preT <- 0.25f
        self.ex.Exdesign.durT <- 1.0f
        self.ex.Exdesign.posT <- 0.25f
        self.ex.Exdesign.bgcolor <- Color.Black
        
        let mutable bpara = BarPara.Default
        bpara.width <- 4.0f
        bpara.height <- 2.0f
        bpara.BasePara.orientation <- 90.0f
        bpara.BasePara.direction <- 0.0f
        bpara.BasePara.speed <- 10.0f
        bpara.BasePara.space <- 10.0f
        bpara.BasePara.center <- new Vector3(-3.0f, -3.0f, 0.0f)
        bpara.BasePara.color <- Color.SeaGreen
        self.bars.[0] <- new Bar(self.GraphicsDevice, bpara)
        
        bpara.width <- 3.0f;
        bpara.height <- 1.0f;
        bpara.BasePara.direction <- 90.0f
        bpara.BasePara.center <- new Vector3(3.0f, 3.0f, 0.0f)
        bpara.BasePara.color <- new Color(1.0f, 0.0f, 0.0f, 0.5f)
        self.bars.[1] <- new Bar(self.GraphicsDevice, bpara)
        
        self.barangle <- 90.0f
        
end

let Experiment = new ExTwo_dBar(Text = "F# Scripting Two Drifting Bars")
Application.Run(Experiment)

#if COMPILED
[<STAThread>]
do Application.Run(Experiment)
#endif