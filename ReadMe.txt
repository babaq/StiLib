  StiLib is currently not actively developed, effort has been shifted to VLAB(http://vlabsys.github.io) which supersedes StiLib.


	
			StiLib		
			
========================================================================================================
					
//\\---Introduction---//\\

========================================================================================================
StiLib stands for stimulus library for neurobiology research distributed under GNU Lesser General Public License (GNU LGPL).
It allows precise, real time manipulation of various stimuli in psychophysical, electrophysiological and behavioral experiments.
StiLib is designed to be easy and powerful. It provides services for general experiment construction and interfaces for direct hardware access 
which can be easily used and extended by any .NET programming languages.


				Zhang Li  ( Visual Information Processing --- Institute of Biophysics --- Chinese Academy of Science )
========================================================================================================

//\\---System Requirements---//\\

========================================================================================================
Software:
				Windows Vista® or Later 
				.NET Framework 3.5 or Higher
				DirectX 9.0c or Higher
Hardware:
				Video card:		Shader Model 2.0 or Higher			
========================================================================================================

//\\---Marker Encode Protocal and Marker Header Structure---//\\

========================================================================================================
A stimulus is marked by one TTL pluse at onset(one marker mode) or by two TTL pluses at onset and offset(two marker mode) through Parallel Port.
Each experiment has a 'MarkerHeader' which encode the information about the experiment in keyword sequence.
MarkerHeader encoding is based on time interval of two TTL pluses in 16 number system, such as 0: (0-5ms)   1: (5-10ms)    ...   15: (75-80ms).
Each keyword in MarkerHeader is represented by a 4 digits number in 16 number system which can encode 16×16×16×16=65536 different keyworks.
The keyword -- [ 00__00__16__00 ] separate different groups of keyworks.
The keyword -- [ 00__00__00__16 ] marks the end of MarkerHeader.

MarkerHeader Structure: 
							ExperimentType --- Custom_Keywords_Group_1 --- [ 00__00__16__00 ] --- Custom_Keywords_Group_2 --- [ 00__00__00__16 ] --------- Stimulus Marker ON  ... ... ...   
							
========================================================================================================

//\\---Third Party Libraries---//\\

========================================================================================================
dnAnalytics
Copyright © 2003-2009, dnAnalytics Project
dnAnalytics is a numerical library licensed under the Microsoft Public License. See http://www.codeplex.com/dnAnalytics

ZLIB.NET  
Copyright © 2006-2007, ComponentAce
ZLIB.NET provides compression routines for Matlab reader and writer, licensed under a BSD style license. See http://www.componentace.com

WinIo
Copyright © 1998-2002, Yariv Kaplan 
Direct Hardware Access Under Windows 9x/NT/2000/XP. See http://www.internals.com 

Media Foundation .NET
Copyright © 2007-2009, David Wohlferd 
Access to Microsoft Media Foundation from .NET, licensed under the LGPL license. See http://mfnet.sourceforge.net 

ZedGraph
Copyright © 2007, John Champion 
2D graph library for .NET, licensed under the LGPL license. See http://zedgraph.org
========================================================================================================

//\\---Contact---//\\

========================================================================================================
Zhang Li ( fff008@gmail.com )
