#require Microsoft.SharePoint.Powershell

$dir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$name = "CleanupTimerJob.wsp"
$path = "$dir\$name"

$solution = Get-SPSolution $name -ErrorAction:SilentlyContinue

if($solution)
{
	if($solution.Deployed -eq $true)
	{
		if ( $solution.ContainsWebApplicationResource ) {
			Uninstall-SPSolution $solution -Confirm:$false -AllWebapplications
		} else {
			Uninstall-SPSolution $solution -Confirm:$false
		}
		
	    while($solution.JobExists)
		{
			sleep -s 3	 
		}
	}
	Remove-SPSolution $solution -Confirm:$false
}