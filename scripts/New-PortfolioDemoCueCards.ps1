param(
    [string] $OutputPath,
    [string] $RehearsalReportPath
)

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot = Split-Path -Parent $ScriptDir
$OutputDir = Join-Path $RepoRoot "artifacts\demo-rehearsal"

function Resolve-RepoPath {
    param(
        [string] $Path,
        [string] $DefaultPath
    )

    if ([string]::IsNullOrWhiteSpace($Path)) {
        return $DefaultPath
    }

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $RepoRoot $Path))
}

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

$OutputPath = Resolve-RepoPath `
    -Path $OutputPath `
    -DefaultPath (Join-Path $OutputDir "recording-cue-cards.md")

$RehearsalReportPath = Resolve-RepoPath `
    -Path $RehearsalReportPath `
    -DefaultPath (Join-Path $OutputDir "recording-rehearsal.md")

$rehearsalStatus = "미실행"
if (Test-Path -LiteralPath $RehearsalReportPath) {
    $rehearsalText = Get-Content -Raw -Encoding UTF8 -Path $RehearsalReportPath
    if ($rehearsalText -match "데모 테스트 결과:\s*\*\*통과\*\*") {
        $rehearsalStatus = "통과"
    }
    elseif ($rehearsalText -match "데모 테스트 결과:\s*\*\*실패\*\*") {
        $rehearsalStatus = "실패"
    }
    else {
        $rehearsalStatus = "확인 필요"
    }
}

$rehearsalReportRelativePath = ConvertTo-RelativePath $RehearsalReportPath
$cueCardsRelativePath = ConvertTo-RelativePath $OutputPath

$lines = New-Object System.Collections.Generic.List[string]

$lines.Add("# 포트폴리오 데모 녹화 큐카드")
$lines.Add("")
$lines.Add("생성 시간: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss zzz')")
$lines.Add("")
$lines.Add("## 먼저 볼 결론")
$lines.Add("")
$lines.Add("- 리허설 상태: **$rehearsalStatus**")
$lines.Add('- 리허설 결과 파일: `' + $rehearsalReportRelativePath + '`')
$lines.Add('- 이 큐카드 파일: `' + $cueCardsRelativePath + '`')
$lines.Add("")
$lines.Add('리허설 상태가 `통과`이면 녹화 재료는 준비된 것이다. 녹화 중에는 아래 대본을 그대로 읽어도 된다.')
$lines.Add("")
$lines.Add("## 녹화 전 30초 준비")
$lines.Add("")
$lines.Add("PowerShell에서 먼저 리허설을 실행한다.")
$lines.Add("")
$lines.Add('```powershell')
$lines.Add(".\scripts\Invoke-PortfolioDemoRehearsal.ps1")
$lines.Add('```')
$lines.Add("")
$lines.Add("Unity 확인을 잠시 빼고 대본만 확인할 때는 아래 명령을 쓴다.")
$lines.Add("")
$lines.Add('```powershell')
$lines.Add(".\scripts\Invoke-PortfolioDemoRehearsal.ps1 -SkipUnity")
$lines.Add('```')
$lines.Add("")
$lines.Add("열어둘 화면:")
$lines.Add("")
$lines.Add('- `README.md`')
$lines.Add('- `docs/demo/moly-ald-demo.png`')
$lines.Add('- `artifacts/demo-rehearsal/recording-rehearsal.md`')
$lines.Add('- `artifacts/demo-rehearsal/moly-ald-process-report.md`')
$lines.Add('- `artifacts/demo-rehearsal/moly-ald-timeline.json`')
$lines.Add("")
$lines.Add("## 3분 대본")
$lines.Add("")
$lines.Add("| 시간 | 화면 | 말할 내용 | 목적 |")
$lines.Add("|---|---|---|---|")
$lines.Add("| 0:00-0:20 | README 첫 화면 | 이 프로젝트는 실제 장비가 없어도 장비 SW의 공정 흐름, fault handling, Unity replay를 검증하기 위한 디지털 트윈 포트폴리오입니다. | 한 줄 정의 |")
$lines.Add("| 0:20-0:50 | README 구조/이미지 | 핵심은 공정 로직을 Unity에 넣지 않은 것입니다. C#/.NET Core가 source of truth이고, Unity는 결과 timeline을 읽어서 보여주는 계층입니다. | 아키텍처 방향 |")
$lines.Add("| 0:50-1:25 | 정상 공정 report | 정상 공정은 공개/합성 molybdenum ALD 흐름으로 만들었습니다. pumpdown, dose, purge 같은 step 결과가 report와 timeline JSON으로 남습니다. | 정상 sequence 증명 |")
$lines.Add("| 1:25-1:55 | fault 결과 | 정상만 보여주면 장비 SW 관점에서 부족합니다. pumpdown timeout을 넣으면 공정은 Alarmed로 멈추고, 이 실패는 의도한 fault demo입니다. | fault handling 증명 |")
$lines.Add("| 1:55-2:30 | Unity screenshot | Unity는 pressure, valve, gas line, film thickness 같은 값을 화면으로 재생합니다. 나중에 CAD나 Blender 모델이 들어와도 visual adapter만 교체하는 구조입니다. | 보이는 산출물 |")
$lines.Add("| 2:30-3:00 | portfolio package 또는 README | 이 프로젝트는 실제 Lam/ALTUS/Halo 내부 sequence를 복제한 것이 아닙니다. 공개 정보와 합성 값을 사용해서 장비 SW 구조를 정직하게 보여주는 데모입니다. | 한계와 정직성 |")
$lines.Add("")
$lines.Add("## 한 문장 답변")
$lines.Add("")
$lines.Add("면접에서 짧게 말해야 할 때:")
$lines.Add("")
$lines.Add("> 실제 장비 없이도 장비 SW의 sequence, fault, replay visualization을 검증할 수 있도록 Core/CLI/Unity를 분리한 제조 장비 디지털 트윈 프로젝트입니다.")
$lines.Add("")
$lines.Add("## 질문이 들어왔을 때")
$lines.Add("")
$lines.Add("### 실제 장비랑 연결돼 있나요?")
$lines.Add("")
$lines.Add("아직 실제 장비와 연결한 것은 아닙니다. 대신 실제 장비 SW에서 중요한 상태 전이, timeout, fault stop, replay data boundary를 소프트웨어 모델로 검증했습니다.")
$lines.Add("")
$lines.Add("### 실제 ALD 장비 내부 sequence를 구현한 건가요?")
$lines.Add("")
$lines.Add("아닙니다. 공개된 ALD 개념과 합성 값을 사용했습니다. 회사 장비 내부 recipe나 비공개 sequence를 구현했다고 주장하지 않습니다.")
$lines.Add("")
$lines.Add("### 왜 Unity가 공정을 계산하지 않게 했나요?")
$lines.Add("")
$lines.Add("Unity가 공정을 계산하면 화면 코드와 공정 코드가 서로 다른 truth가 될 수 있습니다. 그래서 Core/CLI가 공정 결과를 만들고 Unity는 timeline replay만 담당하게 나눴습니다.")
$lines.Add("")
$lines.Add("### CAD/Blender 모델이 생기면 어떻게 바꾸나요?")
$lines.Add("")
$lines.Add('timeline을 `MolyAldVisualState`로 바꾸는 경계는 유지하고, primitive renderer 대신 imported model binding을 연결합니다. 즉 공정 로직은 그대로 두고 외형 계층만 교체합니다.')
$lines.Add("")
$lines.Add("## 말하면 안 되는 표현")
$lines.Add("")
$lines.Add("- 실제 Lam/ALTUS/Halo/Halo HX 장비를 그대로 구현했다.")
$lines.Add("- 실제 회사 recipe나 내부 sequence를 재현했다.")
$lines.Add("- 실제 증착 물리를 정확히 시뮬레이션했다.")
$lines.Add("- Unity 화면이 실제 장비 동작 검증을 대체한다.")
$lines.Add("")
$lines.Add("## 녹화 중 막히면")
$lines.Add("")
$lines.Add('- 영어 로그를 읽으려고 하지 말고 `recording-rehearsal.md`의 `결론`만 보여준다.')
$lines.Add('- fault command가 exit code `1`로 끝나도 당황하지 않는다. 이 데모에서는 expected failure다.')
$lines.Add('- Unity가 느리면 실제 Editor 대신 `docs/demo/moly-ald-demo.png` screenshot을 보여준다.')
$lines.Add("- 시간이 부족하면 0:00-0:20, 1:25-1:55, 2:30-3:00 세 구간만 말한다.")

New-Item -ItemType Directory -Force -Path (Split-Path -Parent $OutputPath) | Out-Null
Set-Content -Encoding UTF8 -Path $OutputPath -Value $lines

Write-Host "데모 녹화 큐카드 생성 완료: $OutputPath"
Write-Host "리허설 상태: $rehearsalStatus"
