param(
    [string] $UnityPath,
    [switch] $OpenProjectOnly,
    [switch] $CaptureScreenshot,
    [string] $ScreenshotPath
)

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot = Split-Path -Parent $ScriptDir
$ProjectPath = Join-Path $RepoRoot "unity\EquipmentTwin.Unity"
$ProjectVersionPath = Join-Path $ProjectPath "ProjectSettings\ProjectVersion.txt"
$LogDir = Join-Path $ProjectPath "Logs"
$LogPath = Join-Path $LogDir "codex-unity-smoke-test.log"
$DefaultScreenshotPath = Join-Path $RepoRoot "artifacts\unity-demo\moly-ald-demo.png"
$SuccessMarker = "EQUIPMENT_TWIN_UNITY_SMOKE_TEST_PASS"
$ScreenshotMarker = "EQUIPMENT_TWIN_UNITY_SCREENSHOT_SAVED"

function Resolve-UnityEditor {
    param([string] $RequestedUnityPath)

    if (-not [string]::IsNullOrWhiteSpace($RequestedUnityPath)) {
        if (-not (Test-Path -LiteralPath $RequestedUnityPath)) {
            throw "UnityPath does not exist: $RequestedUnityPath"
        }

        return (Resolve-Path -LiteralPath $RequestedUnityPath).Path
    }

    $version = $null
    if (Test-Path -LiteralPath $ProjectVersionPath) {
        $versionLine = Get-Content -LiteralPath $ProjectVersionPath |
            Where-Object { $_ -like "m_EditorVersion:*" } |
            Select-Object -First 1
        if ($versionLine) {
            $version = ($versionLine -replace "m_EditorVersion:\s*", "").Trim()
        }
    }

    if (-not [string]::IsNullOrWhiteSpace($version)) {
        $versionPath = "C:\Program Files\Unity\Hub\Editor\$version\Editor\Unity.exe"
        if (Test-Path -LiteralPath $versionPath) {
            return $versionPath
        }
    }

    $hubEditorRoot = "C:\Program Files\Unity\Hub\Editor"
    if (Test-Path -LiteralPath $hubEditorRoot) {
        $candidate = Get-ChildItem -LiteralPath $hubEditorRoot -Directory |
            Sort-Object Name -Descending |
            ForEach-Object { Join-Path $_.FullName "Editor\Unity.exe" } |
            Where-Object { Test-Path -LiteralPath $_ } |
            Select-Object -First 1

        if ($candidate) {
            return $candidate
        }
    }

    throw "Unity Editor was not found. Install Unity Hub/Editor or pass -UnityPath."
}

New-Item -ItemType Directory -Force -Path $LogDir | Out-Null

$ResolvedUnityPath = Resolve-UnityEditor -RequestedUnityPath $UnityPath
Write-Host "Unity Editor: $ResolvedUnityPath"
Write-Host "Project:      $ProjectPath"
Write-Host "Log:          $LogPath"

if ([string]::IsNullOrWhiteSpace($ScreenshotPath)) {
    $ScreenshotPath = $DefaultScreenshotPath
}
elseif (-not [System.IO.Path]::IsPathRooted($ScreenshotPath)) {
    $ScreenshotPath = [System.IO.Path]::GetFullPath((Join-Path $RepoRoot $ScreenshotPath))
}

if ($OpenProjectOnly) {
    Start-Process -FilePath $ResolvedUnityPath -ArgumentList @("-projectPath", $ProjectPath)
    Write-Host "Unity project opened. In Unity, use Equipment Twin > Run Moly ALD Smoke Test or Equipment Twin > Capture Moly ALD Demo Screenshot."
    exit 0
}

$executeMethod = "EquipmentTwin.Unity.EditorTools.MolyAldEditorSmokeTest.RunBatchSmokeTest"
$unityArgs = @(
    "-batchmode",
    "-quit",
    "-projectPath",
    $ProjectPath,
    "-logFile",
    $LogPath
)

if (-not $CaptureScreenshot) {
    $unityArgs = @(
        "-batchmode",
        "-quit",
        "-nographics",
        "-projectPath",
        $ProjectPath,
        "-logFile",
        $LogPath
    )
}

if ($CaptureScreenshot) {
    New-Item -ItemType Directory -Force -Path (Split-Path -Parent $ScreenshotPath) | Out-Null
    $executeMethod = "EquipmentTwin.Unity.EditorTools.MolyAldEditorSmokeTest.RunBatchScreenshotCapture"
    $unityArgs += @("-equipmentTwinScreenshot", $ScreenshotPath)
    Write-Host "Screenshot:   $ScreenshotPath"
}

$unityArgs += @("-executeMethod", $executeMethod)

& $ResolvedUnityPath @unityArgs

$UnityExitCode = $LASTEXITCODE
if (Test-Path -LiteralPath $LogPath) {
    Get-Content -Tail 80 -LiteralPath $LogPath
}

if ($UnityExitCode -ne 0) {
    Write-Host "ERROR: Unity smoke test failed with exit code $UnityExitCode. If the log says 'No valid Unity Editor license found', sign in through Unity Hub and rerun this script." -ForegroundColor Red
    exit $UnityExitCode
}

if (-not (Select-String -LiteralPath $LogPath -Pattern $SuccessMarker -Quiet)) {
    Write-Host "ERROR: Unity exited successfully, but the smoke-test success marker was not found: $SuccessMarker" -ForegroundColor Red
    exit 1
}

if ($CaptureScreenshot) {
    if (-not (Select-String -LiteralPath $LogPath -Pattern $ScreenshotMarker -Quiet)) {
        Write-Host "ERROR: Unity exited successfully, but the screenshot marker was not found: $ScreenshotMarker" -ForegroundColor Red
        exit 1
    }

    if (-not (Test-Path -LiteralPath $ScreenshotPath)) {
        Write-Host "ERROR: Screenshot marker was found, but the screenshot file does not exist: $ScreenshotPath" -ForegroundColor Red
        exit 1
    }

    Write-Host "Unity screenshot saved: $ScreenshotPath"
}

Write-Host "Unity smoke test passed."
