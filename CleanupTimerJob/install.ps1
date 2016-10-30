#require Microsoft.SharePoint.Powershell

param($webApp = $(Read-Host "webApp"))
 
$dir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$name = "CleanupTimerJob.wsp"
$path = "$dir\$name"
 

 
$solution = Get-SPSolution $name -ErrorAction:SilentlyContinue
 
if(!$solution)
{    
    $solution = Add-SPSolution -LiteralPath $path
}
 
if($solution.Deployed –eq $false)
{
    if ( $solution.ContainsWebApplicationResource ) {
        Install-SPSolution $solution -GACDeployment -WebApplication $webApp
    } else {
        Install-SPSolution $solution -GACDeployment 
    }
    while($solution.JobExists)
    {
        sleep -s 1
    }
}