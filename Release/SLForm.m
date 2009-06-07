% SLForm.m
% StiLib Basic Matlab Scripting
% 2009-04-06 Zhang Li

NET.addAssembly('E:\Programs\Stimulus\StiLib\Release\StiLib.dll')
NET.addAssembly('E:\Programs\Stimulus\StiLib\Release\Microsoft.Xna.Framework.dll')

import System.Windows.Forms.*
import Microsoft.Xna.Framework.Graphics.*
import StiLib.Core.*
import StiLib.Vision.*

MyExperiment = SLForm();
MyExperiment.Text = 'Basic Matlab Scripting';
Application.Run(MyExperiment);