param($siteUrl= $(Read-Host "siteUrl"))
 
$dir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$name = "ConnectedWebParts.wsp"
$path = "$dir\$name"
$featureId = "287ecf12-d893-4d56-8ce9-6ebca4f472ee" 
 
Add-PSSnapin Microsoft.SharePoint.Powershell -ErrorAction:SilentlyContinue
 
$solution = Get-SPSolution $name -ErrorAction:SilentlyContinue
 
if(!$solution)
{    
    $solution = Add-SPSolution -LiteralPath $path
}
 
if($solution.Deployed –eq $false)
{
    if ( $solution.ContainsWebApplicationResource ) {
        Install-SPSolution $solution -CasPolicies -AllWebapplications
    } else {
        Install-SPSolution $solution -CasPolicies
    }
    while($solution.Deployed –eq $false)
    {
        sleep -s 1
    }
}

while(!(Get-SPFeature $featureId -ErrorAction:SilentlyContinue)) 
{
    sleep -s 1
}
 
if(!(Get-SPFeature $featureId -Site $siteUrl -ErrorAction:SilentlyContinue)) 
{
    Enable-SPFeature $featureId -Url $siteUrl
}