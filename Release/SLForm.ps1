# SLForm.ps1
# StiLib Basic PowerShell Scripting
# 2009-07-20 Zhang Li

[void][reflection.assembly]::LoadWithPartialName("System.Windows.Forms")
[void][reflection.assembly]::LoadFrom("E:\Programs\Stimulus\StiLib\Release\StiLib.dll")
[void][reflection.assembly]::LoadFrom("E:\Programs\Stimulus\StiLib\Release\Microsoft.Xna.Framework.dll")

$MyExperiment = new-object StiLib.Core.SLForm
$MyExperiment.Text = "Basic PowerShell Scripting"
$MyExperiment.ShowDialog()