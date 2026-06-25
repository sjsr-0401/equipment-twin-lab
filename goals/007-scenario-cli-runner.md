# Goal 007: Scenario CLI 실행기

작성일: 2026-06-25

## 목표

JSON 시나리오를 명령어로 직접 실행할 수 있는 CLI를 만든다.

## 왜 필요한가

시나리오 JSON과 Runner가 있어도, 사용자가 직접 실행할 방법이 없으면 데모와 검증이 불편하다.

CLI가 있으면 Unity 없이도 정상 사이클, Timeout, 비상정지 같은 시나리오를 명령어로 실행하고 결과를 확인할 수 있다.

## 범위

포함:

- `EquipmentTwin.Cli` 콘솔 프로젝트
- 시나리오 JSON 파일 실행
- 기본 Timeout 정책 옵션
- 초기 시간 옵션
- 단계별 PASS/FAIL 출력
- 최종 상태 출력
- 최종 IO Snapshot 출력
- CI에서 CLI 샘플 시나리오 실행

제외:

- 복수 시나리오 일괄 실행
- JSON 결과 파일 export
- HTML/Markdown 리포트 생성
- Unity UI 연동
- 실제 PLC 연결

## 위험 등급

- [ ] L0 문서/자동 검증 설정
- [x] L1 CLI/샘플/검증 도구
- [ ] L2 핵심 상태머신 변경
- [ ] L3 안전, 인터록, 알람 우선순위, 데이터 삭제

현재 구현은 시뮬레이션 실행 도구다. 실제 장비 운전 도구가 아니다.

## 완료 기준

- CLI 프로젝트가 솔루션에 포함된다.
- 정상 시나리오 JSON을 실행할 수 있다.
- Timeout 시나리오 JSON을 실행할 수 있다.
- 실패한 시나리오는 non-zero exit code를 반환한다.
- CI에서 CLI 샘플 시나리오가 실행된다.
- 작업로그에 막힌 점, 아키텍처, 유지보수 포인트가 기록된다.

## 실행 방법

```powershell
dotnet run --project src\EquipmentTwin.Cli -- scenarios\normal-cycle.json
dotnet run --project src\EquipmentTwin.Cli -- scenarios\loading-timeout.json --default-timeouts
```

## 검증 방법

```powershell
dotnet build --no-restore
dotnet run --project tests\EquipmentTwin.Core.Tests --no-restore
dotnet run --project src\EquipmentTwin.Cli --no-restore -- scenarios\normal-cycle.json
dotnet run --project src\EquipmentTwin.Cli --no-restore -- scenarios\loading-timeout.json --default-timeouts
```

## 막힌 점과 해결 방법

- 현재 막힌 점 없음.
- 외부 CLI 파서 패키지를 쓰지 않고 직접 인자 parsing을 구현했다.

## 보류한 판단

- `System.CommandLine` 같은 외부 패키지는 아직 쓰지 않는다.
- CLI 결과를 JSON/Markdown 파일로 export하는 기능은 넣지 않았다.
- 여러 시나리오를 한 번에 실행하는 batch 기능은 다음 단계 후보로 둔다.

## 아키텍처 설명

```text
Command Line
    ↓
EquipmentTwin.Cli
    ↓
EquipmentScenario.FromJson()
    ↓
ScenarioRunner
    ↓
EquipmentCellController
```

CLI는 Core 로직을 직접 다시 구현하지 않고, 기존 ScenarioRunner를 호출한다.

## 유지보수 포인트

- CLI 인자 처리: `src/EquipmentTwin.Cli/Program.cs`
- 시나리오 실행 로직: `src/EquipmentTwin.Core/Scenarios/ScenarioRunner.cs`
- 샘플 시나리오: `scenarios/`
- CI CLI 실행 단계: `.github/workflows/ci.yml`
