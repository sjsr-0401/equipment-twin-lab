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

function Measure-BrightPixels {
    param(
        [System.Drawing.Bitmap]$Image,
        [int]$X,
        [int]$Y,
        [int]$Width,
        [int]$Height
    )

    $count = 0
    for ($sampleX = $X; $sampleX -lt ($X + $Width); $sampleX += 4) {
        for ($sampleY = $Y; $sampleY -lt ($Y + $Height); $sampleY += 4) {
            $pixel = $Image.GetPixel($sampleX, $sampleY)
            if (($pixel.R + $pixel.G + $pixel.B) -gt 180) {
                $count++
            }
        }
    }

    return $count
}

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

    $image = [System.Drawing.Bitmap]::FromFile($screenshotPath)
    try {
        $width = $image.Width
        $height = $image.Height

        $blackSampleCount = 0
        $totalSampleCount = 0
        for ($x = 20; $x -lt $width; $x += 40) {
            for ($y = 20; $y -lt $height; $y += 30) {
                $pixel = $image.GetPixel($x, $y)
                $totalSampleCount++
                if ($pixel.R -eq 0 -and $pixel.G -eq 0 -and $pixel.B -eq 0) {
                    $blackSampleCount++
                }
            }
        }

        $anchorChecks = @(
            (Measure-BrightPixels -Image $image -X 0 -Y 0 -Width 900 -Height 80) -ge 180
            (Measure-BrightPixels -Image $image -X 0 -Y 80 -Width 900 -Height 120) -ge 180
            (Measure-BrightPixels -Image $image -X 1000 -Y 80 -Width 580 -Height 140) -ge 350
            (Measure-BrightPixels -Image $image -X 0 -Y 760 -Width 1580 -Height 120) -ge 500
        )
    }
    finally {
        $image.Dispose()
    }

    if ($width -ne 1600 -or $height -ne 900) {
        throw "Unexpected screenshot size for '$state': ${width}x${height} (expected 1600x900)."
    }

    $blackSampleRatio = $blackSampleCount / $totalSampleCount
    if ($blackSampleRatio -gt 0.15) {
        $blackSamplePercent = [Math]::Round($blackSampleRatio * 100, 1)
        throw "Screenshot '$state' contains too many pure-black samples (${blackSamplePercent}%). The WPF window may not have rendered completely."
    }

    if ($anchorChecks -contains $false) {
        throw "Screenshot '$state' is missing one or more expected visual regions (header, equipment, controls, or debug area)."
    }

    [pscustomobject]@{
        State = $state
        Language = $Language
        Size = "${width}x${height}"
        BlackSamples = "$blackSampleCount/$totalSampleCount"
        Anchors = "4/4"
        File = $screenshotPath
    }
}

$results | Format-Table -AutoSize
Write-Host "Captured $($results.Count) WPF demo screenshots in: $OutputDirectory"
