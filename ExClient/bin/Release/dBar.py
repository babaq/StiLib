##############################################
# File: dBar.py
#
# Drifting Bar Stimulus
#
# Copyright (c) 2009-03-16 Zhang Li
##############################################

import clr
clr.AddReference("System.Windows.Forms")
clr.AddReference("Microsoft.Xna.Framework.dll")
clr.AddReference("StiLib.dll")

from System import *
from System.Windows.Forms import *
from Microsoft.Xna.Framework import *
from Microsoft.Xna.Framework.Graphics import *
from StiLib.Core import *
from StiLib.Vision import *
from StiLib.Vision.Stimuli import *

# Our Custom Experiment is inherited from StiLib.Vision.Stimuli.dBar
class ExdBar(dBar):
        def __new__(self):
                return dBar.__new__(self, 800, 600, 0, True, False)
        def __init__(self):
                self.Text = "IronPython Scripting Drifting Bar"
        def Initialize(self):
                self.text = Text(self.GraphicsDevice, self.Services, self.SLConfig["content"], "Arial")
                # Init Experiment Parameter
                self.ex = SLExperiment()
                self.ex.AddExType(ExType.dBar)
                self.ex.AddCondition(ExPara.Direction, 4)
                self.ex.Expara = ExDesign(trial = 3, trestT = 1.0, srestT = 0.5, stimuli = Array[int](range(10)),
                                          preT = 0.25, durT = 1.0, posT = 0.25, bgcolor = Color.Black)
                
                # Init Bar Parameter
                basepara = BasePara(center = Vector3(-2.0, -2.0, 0.0), orientation = 90.0,
                                    movearea = 10.0, color = Color.SeaGreen, visible = True)
                bpara = BarPara(BasePara = basepara, width = 4.0, height = 1.0,
                                direction = 45.0, speed = 10.0)
                self.Bar = Bar(self.GraphicsDevice, bpara)

Application.Run(ExdBar())
