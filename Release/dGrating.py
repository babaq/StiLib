##############################################
# File: dGrating.py
#
# Drifting Grating Stimulus
#
# Copyright (c) 2009-03-16 Zhang Li
##############################################

import clr
clr.AddReference("System.Windows.Forms")
clr.AddReference("Microsoft.Xna.Framework")
clr.AddReference("StiLib")

from System import *
from System.Windows.Forms import *
from Microsoft.Xna.Framework import *
from Microsoft.Xna.Framework.Graphics import *
from StiLib.Core import *
from StiLib.Vision import *
from StiLib.Vision.Stimuli import *

# Our Custom Experiment is inherited from StiLib.Vision.Stimuli.dGrating
class ExdGrating(dGrating):
        def __new__(self):
            return dGrating.__new__(self, "")
	def __init__(self):
            self.Text = 'IronPython Scripting Drifting Grating'
        def Initialize(self):
            self.text = Text(self.GraphicsDevice, self.Services, self.SLConfig["content"], "Arial")
            # Init Experiment Parameter
            self.ex = SLExperiment()
            self.ex.AddExType(ExType.dGrating)
            self.ex.AddCondition(ExPara.Direction, 4)
            self.ex.Exdesign = ExDesign(trial = 2, trestT = 1.0, srestT = 0.5, stimuli = Array[int](range(1)),
                                        preT = 0.25, durT = 1.0, posT = 0.25, bgcolor = Color.Gray)
            
            # Init Grating Parameter
            basepara = vsBasePara(center = Vector3(0.0, 0.0, 0.0), diameter = 10.0, visible = True,
                                  direction = 0.0, contentname = "Grating")
            gpara = GratingPara(BasePara = basepara, shape = Shape.Circle, gratingtype = GratingType.Sinusoidal,
                                movetype = MoveType.Drifting, tf = 3.0, sf = 0.8, resolution = 100,
                                luminance = 0.5, contrast = 1.0, lhcolor = Color.RosyBrown, rlcolor = Color.Blue)
            self.grating = Grating(self.GraphicsDevice, self.Services, self.SLConfig["content"], gpara)

Application.Run(ExdGrating())
