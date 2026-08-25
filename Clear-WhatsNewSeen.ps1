<#
.SYNOPSIS
Clears the Editor Bar What's New marker in a Visual Studio experimental hive.

.DESCRIPTION
Removes only the VsixVersion value. Close the target Visual Studio experimental
instance before running this script so its cached options cannot restore the value.

Use -SimulateFirstRun to also reset the configuration version. This reproduces the
first-run audience; clearing VsixVersion alone still represents an existing install.

.EXAMPLE
.\Clear-WhatsNewSeen.ps1 -SimulateFirstRun
#>

[CmdletBinding(SupportsShouldProcess)]
param(
    [ValidateNotNullOrEmpty()]
    [string] $RootSuffix = 'Exp',

    [switch] $SimulateFirstRun,

    [string] $VisualStudioInstallationPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Resolve-VsRegEditPath
{
    param([string] $InstallationPath)

    if ([string]::IsNullOrWhiteSpace($InstallationPath))
    {
        $vsWherePath = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
        if (-not (Test-Path -LiteralPath $vsWherePath))
        {
            throw "Could not find vswhere.exe. Pass -VisualStudioInstallationPath explicitly."
        }

        $InstallationPath = & $vsWherePath -latest -prerelease -products `
            Microsoft.VisualStudio.Product.Enterprise `
            Microsoft.VisualStudio.Product.Professional `
            Microsoft.VisualStudio.Product.Community `
            -property installationPath
    }

    if ([string]::IsNullOrWhiteSpace($InstallationPath))
    {
        throw "Could not find a Visual Studio installation."
    }

    $path = Join-Path $InstallationPath.Trim() 'Common7\IDE\VSRegEdit.exe'
    if (-not (Test-Path -LiteralPath $path))
    {
        throw "Could not find VSRegEdit.exe at '$path'."
    }

    return $path
}

$collectionName = 'JPSoftworks.EditorBar.Options.GeneralPage'
$seenVersionValueName = 'VsixVersion'
$vsRegEditPath = Resolve-VsRegEditPath -InstallationPath $VisualStudioInstallationPath
$target = "$collectionName in the '$RootSuffix' Visual Studio hive"
$action = if ($SimulateFirstRun) { 'Reset first-run state' } else { "Remove $seenVersionValueName" }

if ($PSCmdlet.ShouldProcess($target, $action))
{
    & $vsRegEditPath remove local $RootSuffix HKCU $collectionName $seenVersionValueName
    if ($LASTEXITCODE -ne 0)
    {
        throw "VSRegEdit failed with exit code $LASTEXITCODE."
    }

    if ($SimulateFirstRun)
    {
        & $vsRegEditPath set local $RootSuffix HKCU $collectionName Version dword 0
        if ($LASTEXITCODE -ne 0)
        {
            throw "VSRegEdit failed with exit code $LASTEXITCODE."
        }

        Write-Host "Reset the Editor Bar What's New state to simulate a first run in the '$RootSuffix' hive."
    }
    else
    {
        Write-Host "Cleared the Editor Bar What's New marker from the '$RootSuffix' hive."
    }
}
