# Goal 008: CLI batch 실행 + Markdown 리포트

작성일: 2026-06-25

## 목표

여러 시나리오 JSON을 한 번에 실행하고, 결과를 Markdown 리포트로 저장한다.

## 왜 필요한가

시나리오가 늘어나면 파일을 하나씩 실행하는 방식은 불편하다.

Batch 실행과 리포트 저장이 있으면 장비 시뮬레이션을 “반복 가능한 검증 묶음”으로 보여줄 수 있다. 포트폴리오에서는 “시나리오 기반 자동 검증”으로 설명할 수 있다.

## 범위

포함:

- CLI `batch` 명령
- 시나리오 디렉터리 실행
- 단일 파일 batch 실행 지원
- Markdown 리포트 저장
- batch 결과 PASS/FAIL 요약 출력
- CI에서 batch 실행 및 리포트 생성

제외:

- JSON 리포트 export
- HTML 리포트
- 여러 디렉터리 동시 실행
- 시나리오 태그/필터 기능
- 리포트 artifact 업로드
- Unity 연동

## 위험 등급

- [ ] L0 문서/자동 검증 설정
- [x] L1 CLI/리포트/검증 도구
- [ ] L2 핵심 상태머신 변경
- [ ] L3 안전, 인터록, 알람 우선순위, 데이터 삭제

현재 구현은 시뮬레이션 검증 편의 기능이다. 실제 장비 검증 시스템이 아니다.

## 완료 기준

- `batch` 명령으로 `scenarios/` 폴더의 JSON 파일을 실행한다.
- 모든 시나리오가 성공하면 exit code 0을 반환한다.
- 하나라도 실패하면 exit code 1을 반환한다.
- `--report` 옵션으로 Markdown 리포트를 저장한다.
- CI에서 batch 명령을 실행한다.

## 실행 방법

```powershell
dotnet run --project src\EquipmentTwin.Cli -- batch scenarios --default-timeouts --report artifacts\scenario-report.md
```

## 검증 방법

```powershell
dotnet restore EquipmentTwinLab.sln --ignore-failed-sources
dotnet build EquipmentTwinLab.sln --no-restore --configuration Release
dotnet run --project tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj --no-restore --configuration Release
dotnet run --project src\EquipmentTwin.Cli\EquipmentTwin.Cli.csproj --no-restore --configuration Release -- scenarios\normal-cycle.json
dotnet run --project src\EquipmentTwin.Cli\EquipmentTwin.Cli.csproj --no-restore --configuration Release -- scenarios\loading-timeout.json --default-timeouts
dotnet run --project src\EquipmentTwin.Cli\EquipmentTwin.Cli.csproj --no-restore --configuration Release -- batch scenarios --default-timeouts --report artifacts\scenario-report.md
```

## 막힌 점과 해결 방법

- 현재 막힌 점 없음.
- `artifacts/`는 이미 `.gitignore`에 들어 있어 생성된 리포트를 Git에 실수로 올리지 않는다.

## 보류한 판단

- JSON/HTML 리포트는 아직 만들지 않았다.
- GitHub Actions artifact 업로드는 아직 넣지 않았다.
- 시나리오 필터링/tag 기능은 아직 넣지 않았다.

## 아키텍처 설명

```text
Command Line batch
    ↓
EquipmentTwin.Cli
    ↓
ResolveScenarioPaths()
    ↓
ExecuteScenario() repeated
    ↓
BuildMarkdownReport()
```

Batch 기능은 Core 로직을 바꾸지 않고 CLI 실행 계층에서 여러 시나리오를 반복 호출한다.

## 유지보수 포인트

- Batch 인자 처리: `src/EquipmentTwin.Cli/Program.cs`
- 리포트 생성: `BuildMarkdownReport()`
- CI batch 실행: `.github/workflows/ci.yml`
- 리포트 출력 경로: `artifacts/`
