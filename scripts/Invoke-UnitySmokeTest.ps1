param(
    [string] $UnityPath,
    [switch] $OpenProjectOnly
)

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot = Split-Path -Parent $ScriptDir
$ProjectPath = Join-Path $RepoRoot "unity\EquipmentTwin.Unity"
$ProjectVersionPath = Join-Path $ProjectPath "ProjectSettings\ProjectVersion.txt"
$LogDir = Join-Path $ProjectPath "Logs"
$LogPath = Join-Path $LogDir "codex-unity-smoke-test.log"
$SuccessMarker = "EQUIPMENT_TWIN_UNITY_SMOKE_TEST_PASS"

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

if ($OpenProjectOnly) {
    Start-Process -FilePath $ResolvedUnityPath -ArgumentList @("-projectPath", $ProjectPath)
    Write-Host "Unity project opened. In Unity, use Equipment Twin > Run Moly ALD Smoke Test."
    exit 0
}

& $ResolvedUnityPath `
    -batchmode `
    -quit `
    -nographics `
    -projectPath $ProjectPath `
    -executeMethod EquipmentTwin.Unity.EditorTools.MolyAldEditorSmokeTest.RunBatchSmokeTest `
    -logFile $LogPath

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

Write-Host "Unity smoke test passed."
