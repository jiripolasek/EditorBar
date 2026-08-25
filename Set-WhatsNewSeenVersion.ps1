<#
.SYNOPSIS
Sets the Editor Bar What's New marker in a Visual Studio experimental hive.

.DESCRIPTION
Use this to test an upgrade path from a specific previously seen Editor Bar version.
Close the target Visual Studio experimental instance before running this script.

.EXAMPLE
.\Set-WhatsNewSeenVersion.ps1 -Version 4.0.0
#>

[CmdletBinding(SupportsShouldProcess)]
param(
    [Parameter(Mandatory)]
    [version] $Version,

    [ValidateNotNullOrEmpty()]
    [string] $RootSuffix = 'Exp',

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
$valueName = 'VsixVersion'
$vsRegEditPath = Resolve-VsRegEditPath -InstallationPath $VisualStudioInstallationPath
$versionText = $Version.ToString()
$target = "$collectionName\$valueName in the '$RootSuffix' Visual Studio hive"

if ($PSCmdlet.ShouldProcess($target, "Set to $versionText"))
{
    & $vsRegEditPath set local $RootSuffix HKCU $collectionName $valueName string $versionText
    if ($LASTEXITCODE -ne 0)
    {
        throw "VSRegEdit failed with exit code $LASTEXITCODE."
    }

    Write-Host "Set the Editor Bar What's New marker to '$versionText' in the '$RootSuffix' hive."
}
