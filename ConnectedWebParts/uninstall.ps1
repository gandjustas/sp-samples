param($siteUrl= $(Read-Host "siteUrl"))

$dir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$name = "ConnectedWebParts.wsp"
$path = "$dir\$name"
$featureId = "287ecf12-d893-4d56-8ce9-6ebca4f472ee" 

Add-PSSnapin Microsoft.SharePoint.Powershell -ErrorAction:SilentlyContinue

if((Get-SPFeature $featureId -Site $siteUrl -ErrorAction:SilentlyContinue))
{
	Disable-SPFeature $featureId -Url $siteUrl -Confirm:$false
}

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
		
		while($solution.Deployed -eq $true)
		{
			sleep -s 5	 
		}
	}
	Remove-SPSolution $solution -Confirm:$false
}