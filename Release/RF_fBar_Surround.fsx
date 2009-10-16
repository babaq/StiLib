// F# Script File : RF_fBar_Surround.fsx
//
// Two Flashing bars Reverse-Correlation Receptive Field Surround Mapping Diagram
//
// Copyright (c) 2009-07-28 Zhang Li

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
    val mutable bars: Bar[]
    val mutable Rows: int
    val mutable Rstep: float32
    val mutable Columns: int
    val mutable Cstep: float32
    val mutable cgrating: Grating
    
    new() as self = 
        { inherit SLForm(null); text = null; ex = null; bars = [| (new Bar()); (new Bar()) |]; Rows = 0; Rstep = 0.0f; Columns = 0; Cstep = 0.0f; cgrating = null }
        then
        self.text <- new Text(self.GraphicsDevice, self.Services, self.SLConfig.["content"], "Arial")
        self.ex <- new SLExperiment()
        
        self.ex.AddExType(ExType.RF_fBar)
        self.ex.AddCondition(ExPara.Orientation, 0)
        self.ex.Exdesign.trial <- 2
        self.ex.Exdesign.durT <- 0.030f
        self.ex.Exdesign.bgcolor <- Color.Gray
        
        let mutable bpara = BarPara.Default
        bpara.width <- 0.5f
        bpara.height <- 0.8f
        bpara.BasePara.orientation <- 0.0f
        bpara.BasePara.space <- 6.0f
        bpara.BasePara.center <- new Vector3(3.0f, 3.0f, 0.0f) // Surround Center
        bpara.BasePara.color <- Color.Black
        self.bars.[0].Init(self.GraphicsDevice, bpara)
        
        bpara.BasePara.color <- Color.White
        self.bars.[1].Init(self.GraphicsDevice, bpara)
        
        self.Rstep <- 0.3f
        self.Cstep <- 0.6f
        self.InitGrid()
        
        let mutable gpara = GratingPara.Default
        gpara.BasePara.direction <- 90.0f
        gpara.tf <- 3.0f
        gpara.sf <- 0.8f
        gpara.sphase <- 0.0f
        gpara.BasePara.diameter <- 2.0f // CRF Size
        gpara.BasePara.center <- new Vector3(0.0f, 0.0f, 0.0f) // CRF Center
        self.cgrating <- new Grating(self.GraphicsDevice, self.Services, self.SLConfig.["content"], gpara)
        
    member self.InitGrid() = 
        self.Rows <- int( Math.Floor(float( self.bars.[0].Para.BasePara.space / self.Rstep )) )
        if self.Rows % 2 = 0 then
            self.Rows <- self.Rows + 1
        self.Columns <- int( Math.Floor(float(self.bars.[0].Para.BasePara.space / self.Cstep)))
        if self.Columns % 2 = 0 then
            self.Columns <- self.Columns + 1
        
    override self.SetFlow() = 
        self.ex.Flow.TrialCount <- 0
        self.ex.Flow.StiCount <- 0
        self.ex.Flow.IsPred <- false
        self.ex.Flow.StiTime <- 0.0f
        
        self.ex.Flow.RotateOri <- Matrix.CreateRotationZ(self.bars.[0].Para.BasePara.orientation * float32 SLConstant.Rad_p_Deg)
        self.ex.Flow.TranslateCenter <- Matrix.CreateTranslation(self.bars.[0].Para.BasePara.center)
        
    override self.MarkHead() = 
        self.DrawTip(ref self.text, self.ex.Exdesign.bgcolor, SLConstant.MarkHead)
        
        self.ex.Exdesign.stimuli.[0] <- self.Rows * self.Columns * 2
        self.ex.Rand.RandomizeSequence(self.ex.Exdesign.stimuli.[0])
        
        self.ex.PPort.MarkerEncode(self.ex.Extype.[0].Value)
        self.ex.PPort.MarkerEncode(self.ex.Cond.[0].SKEY)
        self.ex.PPort.MarkerEncode(self.ex.Cond.[0].VALUE.ValueN)
        self.ex.PPort.MarkerEncode(self.ex.Rand.Seed)
        self.ex.PPort.MarkerEncode(self.ex.Exdesign.trial)
        
        self.ex.PPort.MarkerSeparatorEncode()
        
        self.bars.[0].Para.Encode(self.ex.PPort)
        self.ex.PPort.MarkerEncode(int( Math.Floor(float(self.cgrating.Para.BasePara.diameter) * 100.0) ))
        self.ex.PPort.MarkerEncode(int(Math.Floor(float self.bars.[0].display_H_deg * 100.0)))
        self.ex.PPort.MarkerEncode(int(Math.Floor(float self.bars.[0].display_W_deg * 100.0)));
        self.ex.PPort.MarkerEncode(self.Rows);
        self.ex.PPort.MarkerEncode(self.Columns);
        self.ex.PPort.MarkerEncode(int(Math.Floor(float self.Rstep * 100.0)));
        self.ex.PPort.MarkerEncode(int(Math.Floor(float self.Cstep * 100.0)));
        
        self.ex.PPort.MarkerEndEncode()
        self.ex.PPort.Timer.ReStart()
        
    override self.Update() = 
        if self.GO_OVER = true then
            self.Update_RF_fBar_Surround()
        
    override self.Draw() = 
        self.GraphicsDevice.Clear(self.ex.Exdesign.bgcolor)
        if self.GO_OVER then
            self.bars.[self.ex.Flow.SliceCount].Draw(self.GraphicsDevice)
            self.cgrating.Draw(self.GraphicsDevice)
            self.ex.Flow.Info <- self.ex.Flow.TrialCount.ToString() + " / " + self.ex.Exdesign.trial.ToString() + " Trials\n" + 
                                         self.ex.Flow.StiCount.ToString() + " / " + self.ex.Exdesign.stimuli.[0].ToString() + " Stimuli"
            self.text.Draw(self.ex.Flow.Info)
        else
            self.text.Draw()
        
    member self.Update_RF_fBar_Surround() = 
        if self.ex.Flow.IsStiOn = true then
            self.ex.Flow.StiTime <- float32 self.ex.PPort.Timer.ElapsedSeconds
            self.ex.PPort.Trigger()
            self.ex.Flow.IsStiOn <- false
        if float32 self.ex.PPort.Timer.ElapsedSeconds - self.ex.Flow.StiTime < self.ex.Exdesign.durT then
            if self.ex.Flow.IsPred = false then
                self.ex.Flow.IsPred <- true
                
                self.ex.Flow.RowCount <- int(Math.Floor(float (self.ex.Rand.Sequence.[self.ex.Flow.StiCount] / (self.Columns * 2))))
                let t = self.ex.Rand.Sequence.[self.ex.Flow.StiCount] % (self.Columns * 2)
                self.ex.Flow.ColumnCount <- int(Math.Floor(float t / 2.0))
                self.ex.Flow.SliceCount <- t % 2
                
                let Xgrid = -float32(self.Columns - 1) * self.Cstep / 2.0f + self.Cstep * float32 self.ex.Flow.ColumnCount
                let Ygrid = float32(self.Rows - 1) * self.Rstep / 2.0f - self.Rstep * float32 self.ex.Flow.RowCount
                self.bars.[self.ex.Flow.SliceCount].Ori3DMatrix <- Matrix.CreateTranslation(Xgrid, Ygrid, 0.0f) * self.ex.Flow.RotateOri
                self.bars.[self.ex.Flow.SliceCount].WorldMatrix <- self.ex.Flow.TranslateCenter
                self.ex.Flow.IsStiOn <- true
                
            self.cgrating.SetTime(float32 self.ex.PPort.Timer.ElapsedSeconds)
        else
            if self.ex.Flow.StiCount < self.ex.Exdesign.stimuli.[0] - 1 then
                self.ex.Flow.IsPred <- false
                self.ex.Flow.StiCount <- self.ex.Flow.StiCount + 1
                self.ex.Flow.StiTime <- float32 self.ex.PPort.Timer.ElapsedSeconds
            else
                if self.ex.Flow.TrialCount < self.ex.Exdesign.trial - 1 then
                    self.ex.Rand.RandomizeSequence(self.ex.Exdesign.stimuli.[0])
                    self.ex.Flow.IsPred <- false
                    self.ex.Flow.TrialCount <- self.ex.Flow.TrialCount + 1
                    self.ex.Flow.StiCount <- 0
                    self.ex.Flow.StiTime <- float32 self.ex.PPort.Timer.ElapsedSeconds
                else
                    self.GO_OVER <- false
                    ()
            self.Update()
        
end

let Experiment = new MyEx(Text = "F# Scripting RF_fBar_Surround")
Application.Run(Experiment)
