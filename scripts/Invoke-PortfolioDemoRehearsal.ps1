param(
    [switch] $SkipUnity,
    [string] $ReportPath,
    [string] $ScreenshotPath
)

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot = Split-Path -Parent $ScriptDir
$OutputDir = Join-Path $RepoRoot "artifacts\demo-rehearsal"

if ([string]::IsNullOrWhiteSpace($ReportPath)) {
    $ReportPath = Join-Path $OutputDir "recording-rehearsal.md"
}
elseif (-not [System.IO.Path]::IsPathRooted($ReportPath)) {
    $ReportPath = [System.IO.Path]::GetFullPath((Join-Path $RepoRoot $ReportPath))
}

if ([string]::IsNullOrWhiteSpace($ScreenshotPath)) {
    $ScreenshotPath = Join-Path $OutputDir "moly-ald-demo.png"
}
elseif (-not [System.IO.Path]::IsPathRooted($ScreenshotPath)) {
    $ScreenshotPath = [System.IO.Path]::GetFullPath((Join-Path $RepoRoot $ScreenshotPath))
}

$ProcessReportPath = Join-Path $OutputDir "moly-ald-process-report.md"
$TimelinePath = Join-Path $OutputDir "moly-ald-timeline.json"
$DefaultUnityScreenshotPath = Join-Path $RepoRoot "artifacts\unity-demo\moly-ald-demo.png"
$StepResults = @()

function ConvertTo-RelativePath {
    param([string] $Path)

    if ([string]::IsNullOrWhiteSpace($Path)) {
        return ""
    }

    $basePath = [System.IO.Path]::GetFullPath($RepoRoot)
    if (-not $basePath.EndsWith([System.IO.Path]::DirectorySeparatorChar)) {
        $basePath += [System.IO.Path]::DirectorySeparatorChar
    }

    $targetPath = [System.IO.Path]::GetFullPath($Path)
    $baseUri = New-Object System.Uri($basePath)
    $targetUri = New-Object System.Uri($targetPath)
    $relativeUri = $baseUri.MakeRelativeUri($targetUri)
    return [System.Uri]::UnescapeDataString($relativeUri.ToString()).Replace("/", [System.IO.Path]::DirectorySeparatorChar)
}

function Invoke-DemoStep {
    param(
        [string] $Name,
        [string] $Executable,
        [string[]] $Arguments,
        [int[]] $AllowedExitCodes = @(0)
    )

    Write-Host "== $Name"
    Write-Host "$Executable $($Arguments -join ' ')"

    $output = & $Executable @Arguments 2>&1
    $exitCode = if ($null -eq $LASTEXITCODE) { 0 } else { $LASTEXITCODE }
    $passed = $AllowedExitCodes -contains $exitCode
    $commandText = "$Executable $($Arguments -join ' ')"

    $script:StepResults += [pscustomobject]@{
        Name = $Name
        Command = $commandText
        ExitCode = $exitCode
        Passed = $passed
        Output = ($output -join [Environment]::NewLine)
    }

    if (-not $passed) {
        Write-Host ($output -join [Environment]::NewLine)
        throw "Step failed: $Name exited with code $exitCode."
    }

    if ($output) {
        $output | Select-Object -Last 12 | ForEach-Object { Write-Host $_ }
    }
}

function Write-RehearsalReport {
    param([string] $Path)

    $lines = New-Object System.Collections.Generic.List[string]
    $lines.Add("# Portfolio Demo Rehearsal Report")
    $lines.Add("")
    $lines.Add("Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss zzz')")
    $lines.Add("")
    $lines.Add("## Purpose")
    $lines.Add("")
    $lines.Add("This report prepares the files and command outputs needed for a 3-minute Equipment Twin Lab demo recording.")
    $lines.Add("")
    $lines.Add("## Step results")
    $lines.Add("")
    $lines.Add("| Step | Exit code | Result |")
    $lines.Add("|---|---:|---|")

    foreach ($result in $StepResults) {
        $status = if ($result.Passed) { "PASS" } else { "FAIL" }
        $lines.Add("| $($result.Name) | $($result.ExitCode) | $status |")
    }

    $lines.Add("")
    $lines.Add("## Recording order")
    $lines.Add("")
    $lines.Add('1. Show `README.md` and the representative Unity screenshot.')
    $lines.Add("2. Show the normal process command and report.")
    $lines.Add('3. Show the pumpdown timeout fault command. Exit code `1` is expected for this fault demo.')
    $lines.Add("4. Show the Unity screenshot or Unity Editor scene.")
    $lines.Add("5. Close with the honest boundary: this is a public/synthetic process replay, not real vendor internals.")
    $lines.Add("")
    $lines.Add("## Generated files")
    $lines.Add("")
    $processReportRelativePath = ConvertTo-RelativePath $ProcessReportPath
    $timelineRelativePath = ConvertTo-RelativePath $TimelinePath
    $rehearsalReportRelativePath = ConvertTo-RelativePath $Path

    $lines.Add('- Normal process report: `' + $processReportRelativePath + '`')
    $lines.Add('- Timeline JSON: `' + $timelineRelativePath + '`')

    if (-not $SkipUnity) {
        $screenshotRelativePath = ConvertTo-RelativePath $ScreenshotPath
        $lines.Add('- Unity screenshot: `' + $screenshotRelativePath + '`')
    }

    $lines.Add('- Rehearsal report: `' + $rehearsalReportRelativePath + '`')
    $lines.Add("")
    $lines.Add("## Commands")
    $lines.Add("")

    foreach ($result in $StepResults) {
        $lines.Add("### $($result.Name)")
        $lines.Add("")
        $lines.Add('```powershell')
        $lines.Add($result.Command)
        $lines.Add('```')
        $lines.Add("")
    }

    $lines.Add("## Notes for recording")
    $lines.Add("")
    $lines.Add("- Do not say this reproduces real Lam/ALTUS/Halo/Halo HX internals.")
    $lines.Add("- Say Core/CLI is the source of truth and Unity is the replay visual layer.")
    $lines.Add('- If the fault command returns exit code `1`, explain that the failure is the expected fault-injection outcome.')

    New-Item -ItemType Directory -Force -Path (Split-Path -Parent $Path) | Out-Null
    Set-Content -Encoding UTF8 -Path $Path -Value $lines
}

Push-Location $RepoRoot
try {
    New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null

    Invoke-DemoStep `
        -Name "Release build" `
        -Executable "dotnet" `
        -Arguments @("build", "EquipmentTwinLab.sln", "--no-restore", "--configuration", "Release")

    Invoke-DemoStep `
        -Name "Core console tests" `
        -Executable "dotnet" `
        -Arguments @("run", "--project", "tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj", "--no-restore", "--configuration", "Release")

    Invoke-DemoStep `
        -Name "Normal public moly ALD process" `
        -Executable "dotnet" `
        -Arguments @(
            "run",
            "--project",
            "src\EquipmentTwin.Cli\EquipmentTwin.Cli.csproj",
            "--no-restore",
            "--configuration",
            "Release",
            "--",
            "process",
            "run",
            "processes\public-moly-ald-metallization.json",
            "--report",
            $ProcessReportPath,
            "--timeline",
            $TimelinePath)

    Invoke-DemoStep `
        -Name "Expected pumpdown timeout fault" `
        -Executable "dotnet" `
        -Arguments @(
            "run",
            "--project",
            "src\EquipmentTwin.Cli\EquipmentTwin.Cli.csproj",
            "--no-restore",
            "--configuration",
            "Release",
            "--",
            "process",
            "run",
            "processes\public-moly-ald-metallization.json",
            "--fault",
            "pumpdown-timeout") `
        -AllowedExitCodes @(1)

    if (-not $SkipUnity) {
        Invoke-DemoStep `
            -Name "Unity screenshot capture" `
            -Executable "powershell" `
            -Arguments @(
                "-NoProfile",
                "-ExecutionPolicy",
                "Bypass",
                "-File",
                (Join-Path $ScriptDir "Invoke-UnitySmokeTest.ps1"),
                "-CaptureScreenshot")

        if (-not (Test-Path -LiteralPath $DefaultUnityScreenshotPath)) {
            throw "Unity screenshot capture completed, but the default screenshot was not found: $DefaultUnityScreenshotPath"
        }

        Copy-Item -LiteralPath $DefaultUnityScreenshotPath -Destination $ScreenshotPath -Force
    }

    Write-RehearsalReport -Path $ReportPath
    Write-Host "Demo rehearsal report: $ReportPath"
}
finally {
    Pop-Location
}
