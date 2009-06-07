// F# Script File : Plaid.fsx
//
// Plaid is Composed of Two Drifting Gratings
//
// Copyright (c) 2008-10-21 Zhang Li

#r @"StiLib.dll"
#r @"Microsoft.Xna.Framework.dll"

open System
open System.Windows.Forms
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open StiLib.Core
open StiLib.Vision
open StiLib.Vision.Stimuli

// Our Custom Experiment is inherited from StiLib.Vision.Stimuli.Two_dGrating
type ExPlaid() = class
    inherit Two_dGrating(800, 600, 0, true, true)
    
    override self.Initialize() =
        self.text <- new Text(self.GraphicsDevice, self.Services, self.SLConfig.["content"], "Arial")
        self.ex <- new SLExperiment()
        
        self.ex.AddExType(ExType.Plaid)
        self.ex.AddCondition(ExPara.Direction, 4)
        self.ex.Expara.trial <- 3
        self.ex.Expara.trestT <- 1.0f
        self.ex.Expara.srestT <- 0.5f
        self.ex.Expara.preT <- 0.25f
        self.ex.Expara.durT <- 1.0f
        self.ex.Expara.posT <- 0.25f
        self.ex.Expara.bgcolor <- Color.Gray
        
        let mutable gpara = GratingPara.Default
        gpara.gratingtype <- GratingType.Sinusoidal
        gpara.shape <- Shape.Circle
        gpara.sf <- 0.2f
        gpara.tf <- 2.0f
        gpara.sphase <- 0.0f
        gpara.luminance <- 0.5f
        gpara.contrast <- 1.0f
        gpara.BasePara.diameter <- 15.0f
        gpara.BasePara.center <- new Vector3(0.0f, 0.0f, 0.0f)
        gpara.lhcolor <- new Color(1.0f, 0.0f, 1.0f, 1.0f)
        gpara.rlcolor <- new Color(0.0f, 0.0f, 0.0f, 1.0f)
        self.Grating.[0] <- new Grating(self.GraphicsDevice, self.Services, self.SLConfig.["content"], gpara)
        
        gpara.sf <- 0.2f
        gpara.tf <- 2.0f
        gpara.sphase <- 0.0f
        gpara.direction <- 90.0f
        gpara.BasePara.diameter <- 15.0f
        gpara.BasePara.center <- new Vector3(0.0f, 0.0f, 0.0f)
        gpara.lhcolor <- new Color(1.0f, 1.0f, 0.0f, 0.5f)
        gpara.rlcolor <- new Color(0.0f, 0.0f, 0.0f, 0.5f)
        self.Grating.[1] <- new Grating(self.GraphicsDevice, self.Services, self.SLConfig.["content"], gpara)
        
        self.gratingangle <- 90.0f
        
end

let Experiment = new ExPlaid(Text = "F# Scripting Plaid")
Application.Run(Experiment)

#if COMPILED
[<STAThread>]
do Application.Run(Experiment)
#endif