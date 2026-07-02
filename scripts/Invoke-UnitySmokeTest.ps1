param(
    [string] $UnityPath,
    [switch] $OpenProjectOnly,
    [switch] $CaptureScreenshot,
    [switch] $CaptureFaultScreenshot,
    [switch] $CaptureRecoveryScreenshot,
    [string] $ScreenshotPath
)

$ErrorActionPreference = "Stop"
$NativeCommandUseErrorActionPreferenceVariable = Get-Variable -Name PSNativeCommandUseErrorActionPreference -ErrorAction SilentlyContinue
$PreviousNativeCommandUseErrorActionPreference = $null
if ($NativeCommandUseErrorActionPreferenceVariable) {
    $PreviousNativeCommandUseErrorActionPreference = $PSNativeCommandUseErrorActionPreference
    $PSNativeCommandUseErrorActionPreference = $false
}

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot = Split-Path -Parent $ScriptDir
$ProjectPath = Join-Path $RepoRoot "unity\EquipmentTwin.Unity"
$ProjectVersionPath = Join-Path $ProjectPath "ProjectSettings\ProjectVersion.txt"
$LogDir = Join-Path $ProjectPath "Logs"
$LogPath = Join-Path $LogDir "codex-unity-smoke-test.log"
$DefaultScreenshotPath = Join-Path $RepoRoot "artifacts\unity-demo\moly-ald-demo.png"
$DefaultFaultScreenshotPath = Join-Path $RepoRoot "artifacts\unity-demo\moly-ald-demo-fault.png"
$DefaultRecoveryScreenshotPath = Join-Path $RepoRoot "artifacts\unity-demo\moly-ald-demo-recovery.png"
$SuccessMarker = "EQUIPMENT_TWIN_UNITY_SMOKE_TEST_PASS"
$ScreenshotMarker = "EQUIPMENT_TWIN_UNITY_SCREENSHOT_SAVED"
$FaultScreenshotMarker = "EQUIPMENT_TWIN_UNITY_FAULT_SCREENSHOT_SAVED"
$RecoveryScreenshotMarker = "EQUIPMENT_TWIN_UNITY_RECOVERY_SCREENSHOT_SAVED"

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

function ConvertTo-CommandLineArgument {
    param([string] $Argument)

    if ($Argument -match '[\s"]') {
        return '"' + ($Argument -replace '"', '\"') + '"'
    }

    return $Argument
}

New-Item -ItemType Directory -Force -Path $LogDir | Out-Null

$ResolvedUnityPath = Resolve-UnityEditor -RequestedUnityPath $UnityPath
Write-Host "Unity Editor: $ResolvedUnityPath"
Write-Host "Project:      $ProjectPath"
Write-Host "Log:          $LogPath"

if ([string]::IsNullOrWhiteSpace($ScreenshotPath)) {
    $ScreenshotPath = if ($CaptureRecoveryScreenshot) { $DefaultRecoveryScreenshotPath } elseif ($CaptureFaultScreenshot) { $DefaultFaultScreenshotPath } else { $DefaultScreenshotPath }
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

if (-not ($CaptureScreenshot -or $CaptureFaultScreenshot -or $CaptureRecoveryScreenshot)) {
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

$ExpectedScreenshotMarker = $ScreenshotMarker

if ($CaptureRecoveryScreenshot) {
    New-Item -ItemType Directory -Force -Path (Split-Path -Parent $ScreenshotPath) | Out-Null
    $executeMethod = "EquipmentTwin.Unity.EditorTools.MolyAldEditorSmokeTest.RunBatchRecoveryScreenshotCapture"
    $unityArgs += @("-equipmentTwinRecoveryScreenshot", $ScreenshotPath)
    $ExpectedScreenshotMarker = $RecoveryScreenshotMarker
    Write-Host "Recovery screenshot: $ScreenshotPath"
}
elseif ($CaptureFaultScreenshot) {
    New-Item -ItemType Directory -Force -Path (Split-Path -Parent $ScreenshotPath) | Out-Null
    $executeMethod = "EquipmentTwin.Unity.EditorTools.MolyAldEditorSmokeTest.RunBatchFaultScreenshotCapture"
    $unityArgs += @("-equipmentTwinFaultScreenshot", $ScreenshotPath)
    $ExpectedScreenshotMarker = $FaultScreenshotMarker
    Write-Host "Fault screenshot: $ScreenshotPath"
}
elseif ($CaptureScreenshot) {
    New-Item -ItemType Directory -Force -Path (Split-Path -Parent $ScreenshotPath) | Out-Null
    $executeMethod = "EquipmentTwin.Unity.EditorTools.MolyAldEditorSmokeTest.RunBatchScreenshotCapture"
    $unityArgs += @("-equipmentTwinScreenshot", $ScreenshotPath)
    Write-Host "Screenshot:   $ScreenshotPath"
}

$unityArgs += @("-executeMethod", $executeMethod)

$UnityArgumentLine = ($unityArgs | ForEach-Object { ConvertTo-CommandLineArgument -Argument $_ }) -join " "
$UnityProcess = Start-Process -FilePath $ResolvedUnityPath -ArgumentList $UnityArgumentLine -Wait -PassThru -WindowStyle Hidden
$UnityExitCode = $UnityProcess.ExitCode
if ($NativeCommandUseErrorActionPreferenceVariable) {
    $PSNativeCommandUseErrorActionPreference = $PreviousNativeCommandUseErrorActionPreference
}
if (Test-Path -LiteralPath $LogPath) {
    Get-Content -Tail 80 -LiteralPath $LogPath
}

$SuccessMarkerFound = Select-String -LiteralPath $LogPath -Pattern $SuccessMarker -Quiet
$ScreenshotMarkerFound = $true
$ScreenshotFileExists = $true

if ($CaptureScreenshot -or $CaptureFaultScreenshot -or $CaptureRecoveryScreenshot) {
    $ScreenshotMarkerFound = Select-String -LiteralPath $LogPath -Pattern $ExpectedScreenshotMarker -Quiet
    $ScreenshotFileExists = Test-Path -LiteralPath $ScreenshotPath
}

if ($UnityExitCode -ne 0) {
    if (-not $SuccessMarkerFound -or -not $ScreenshotMarkerFound -or -not $ScreenshotFileExists) {
        Write-Host "ERROR: Unity smoke test failed with exit code $UnityExitCode. If the log says 'No valid Unity Editor license found', sign in through Unity Hub and rerun this script." -ForegroundColor Red
        exit $UnityExitCode
    }

    Write-Host "WARNING: Unity returned exit code $UnityExitCode after writing the expected success marker and screenshot artifact. Treating this as a verified capture because Unity can emit shutdown warnings after successful batch rendering." -ForegroundColor Yellow
}

if (-not $SuccessMarkerFound) {
    Write-Host "ERROR: Unity exited successfully, but the smoke-test success marker was not found: $SuccessMarker" -ForegroundColor Red
    exit 1
}

if ($CaptureScreenshot -or $CaptureFaultScreenshot -or $CaptureRecoveryScreenshot) {
    if (-not $ScreenshotMarkerFound) {
        Write-Host "ERROR: Unity exited successfully, but the screenshot marker was not found: $ExpectedScreenshotMarker" -ForegroundColor Red
        exit 1
    }

    if (-not $ScreenshotFileExists) {
        Write-Host "ERROR: Screenshot marker was found, but the screenshot file does not exist: $ScreenshotPath" -ForegroundColor Red
        exit 1
    }

    Write-Host "Unity screenshot saved: $ScreenshotPath"
}

Write-Host "Unity smoke test passed."
exit 0
