param(
    [string]$OutputDirectory = "",
    [ValidateSet("en", "ko")]
    [string]$Language = "ko",
    [switch]$SkipBuild
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$projectPath = Join-Path $repoRoot "src\EquipmentTwin.Hmi.Wpf\EquipmentTwin.Hmi.Wpf.csproj"
$executablePath = Join-Path $repoRoot "src\EquipmentTwin.Hmi.Wpf\bin\Release\net8.0-windows\EquipmentTwin.Hmi.Wpf.exe"

if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $repoRoot "artifacts\wpf-demo-screenshots"
}
elseif (-not [System.IO.Path]::IsPathRooted($OutputDirectory)) {
    $OutputDirectory = Join-Path $repoRoot $OutputDirectory
}

$OutputDirectory = [System.IO.Path]::GetFullPath($OutputDirectory)
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null

if (-not $SkipBuild) {
    dotnet build $projectPath -c Release
    if ($LASTEXITCODE -ne 0) {
        throw "WPF Release build failed with exit code $LASTEXITCODE."
    }
}

if (-not (Test-Path -LiteralPath $executablePath -PathType Leaf)) {
    throw "WPF executable was not found: $executablePath"
}

Add-Type -AssemblyName System.Drawing

$states = @("load", "process", "alarm", "transfer-out", "complete")
$results = foreach ($state in $states) {
    $screenshotPath = Join-Path $OutputDirectory "wpf-$state-$Language.png"
    $errorPath = "$screenshotPath.error.txt"

    Remove-Item -LiteralPath $screenshotPath -Force -ErrorAction SilentlyContinue
    Remove-Item -LiteralPath $errorPath -Force -ErrorAction SilentlyContinue

    $arguments = @(
        "--capture-screenshot",
        "`"$screenshotPath`"",
        "--capture-state",
        $state,
        "--capture-language",
        $Language
    )

    $process = Start-Process `
        -FilePath $executablePath `
        -ArgumentList $arguments `
        -WindowStyle Hidden `
        -Wait `
        -PassThru

    if ($process.ExitCode -ne 0 -or -not (Test-Path -LiteralPath $screenshotPath -PathType Leaf)) {
        $detail = if (Test-Path -LiteralPath $errorPath -PathType Leaf) {
            Get-Content -LiteralPath $errorPath -Raw
        }
        else {
            "No capture error file was written."
        }

        throw "Screenshot capture failed for '$state' (exit $($process.ExitCode)).`n$detail"
    }

    $image = [System.Drawing.Image]::FromFile($screenshotPath)
    try {
        $width = $image.Width
        $height = $image.Height
    }
    finally {
        $image.Dispose()
    }

    if ($width -ne 1600 -or $height -ne 900) {
        throw "Unexpected screenshot size for '$state': ${width}x${height} (expected 1600x900)."
    }

    [pscustomobject]@{
        State = $state
        Language = $Language
        Size = "${width}x${height}"
        File = $screenshotPath
    }
}

$results | Format-Table -AutoSize
Write-Host "Captured $($results.Count) WPF demo screenshots in: $OutputDirectory"
