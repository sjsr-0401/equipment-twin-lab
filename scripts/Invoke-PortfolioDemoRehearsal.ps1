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
    $allPassed = -not ($StepResults | Where-Object { -not $_.Passed } | Select-Object -First 1)
    $overallStatus = if ($allPassed) { "통과" } else { "실패" }

    $lines.Add("# 포트폴리오 데모 리허설 결과")
    $lines.Add("")
    $lines.Add("생성 시간: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss zzz')")
    $lines.Add("")
    $lines.Add("## 결론")
    $lines.Add("")
    $lines.Add("데모 테스트 결과: **$overallStatus**")
    $lines.Add("")
    $lines.Add("이 파일은 3분 데모 녹화 전에 빌드, 테스트, 정상 공정, 고장 공정, Unity screenshot이 준비됐는지 확인한 결과다.")
    $lines.Add("")
    $lines.Add("## 단계별 확인 결과")
    $lines.Add("")
    $lines.Add("| 확인 항목 | 종료 코드 | 결과 | 의미 |")
    $lines.Add("|---|---:|---|---|")

    foreach ($result in $StepResults) {
        $status = if ($result.Passed) { "통과" } else { "실패" }
        $meaning = Get-StepMeaning -Name $result.Name
        $lines.Add("| $($result.Name) | $($result.ExitCode) | $status | $meaning |")
    }

    $lines.Add("")
    $lines.Add("## 녹화할 때 보여줄 순서")
    $lines.Add("")
    $lines.Add('1. `README.md` 첫 화면과 대표 Unity screenshot을 보여준다.')
    $lines.Add("2. 정상 공정 report를 보여준다.")
    $lines.Add('3. `pumpdown-timeout` 고장 데모를 보여준다. 이 항목은 종료 코드 `1`이 정상이다.')
    $lines.Add("4. Unity screenshot 또는 Unity Editor scene을 보여준다.")
    $lines.Add("5. 실제 장비 내부 복제가 아니라 공개/합성 process replay라는 한계를 설명한다.")
    $lines.Add("")
    $lines.Add("## 생성된 파일")
    $lines.Add("")
    $processReportRelativePath = ConvertTo-RelativePath $ProcessReportPath
    $timelineRelativePath = ConvertTo-RelativePath $TimelinePath
    $rehearsalReportRelativePath = ConvertTo-RelativePath $Path

    $lines.Add('- 정상 공정 report: `' + $processReportRelativePath + '`')
    $lines.Add('- Unity replay timeline JSON: `' + $timelineRelativePath + '`')

    if (-not $SkipUnity) {
        $screenshotRelativePath = ConvertTo-RelativePath $ScreenshotPath
        $lines.Add('- Unity screenshot: `' + $screenshotRelativePath + '`')
    }

    $lines.Add('- 리허설 결과 report: `' + $rehearsalReportRelativePath + '`')
    $lines.Add("")
    $lines.Add("## 실행한 명령")
    $lines.Add("")

    foreach ($result in $StepResults) {
        $lines.Add("### $($result.Name)")
        $lines.Add("")
        $lines.Add('```powershell')
        $lines.Add($result.Command)
        $lines.Add('```')
        $lines.Add("")
    }

    $lines.Add("## 녹화할 때 주의할 말")
    $lines.Add("")
    $lines.Add("- 실제 Lam/ALTUS/Halo/Halo HX 내부 구조나 recipe를 재현했다고 말하지 않는다.")
    $lines.Add("- Core/CLI가 공정 결과의 기준이고, Unity는 그 결과를 재생하는 화면 계층이라고 말한다.")
    $lines.Add('- `pumpdown-timeout`의 종료 코드 `1`은 고장 주입 데모의 기대 결과라고 설명한다.')

    New-Item -ItemType Directory -Force -Path (Split-Path -Parent $Path) | Out-Null
    Set-Content -Encoding UTF8 -Path $Path -Value $lines
}

function Get-StepMeaning {
    param([string] $Name)

    switch ($Name) {
        "Release build" { return "프로젝트가 Release 설정으로 빌드되는지 확인" }
        "Core console tests" { return "장비 Core 로직 테스트가 통과하는지 확인" }
        "Normal public moly ALD process" { return "정상 합성 ALD 공정이 Complete까지 가는지 확인" }
        "Expected pumpdown timeout fault" { return "고장 주입 시 Alarmed로 멈추는지 확인. 종료 코드 1이 정상" }
        "Unity screenshot capture" { return "Unity 화면 산출물이 생성되는지 확인" }
        default { return "데모 리허설 항목 확인" }
    }
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
    Write-Host "데모 리허설 통과. 결과 파일: $ReportPath"
}
finally {
    Pop-Location
}
