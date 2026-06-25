# Goal 006: 공정 시나리오 JSON Runner

작성일: 2026-06-25

## 목표

장비 운전 흐름을 JSON 파일로 정의하고, 그 시나리오를 실행해서 상태머신/IO/Clock 동작을 반복 검증한다.

## 왜 필요한가

코드 안에 테스트 흐름을 직접 박아두면 나중에 Unity, CLI, 포트폴리오 데모에서 재사용하기 어렵다.

시나리오를 JSON으로 분리하면 정상 공정, 센서 지연, Timeout, 비상정지 같은 흐름을 파일로 만들고 같은 Runner로 반복 실행할 수 있다.

## 범위

포함:

- Scenario JSON 모델
- Scenario step action 정의
- Scenario runner
- 정상 사이클 JSON
- Loading Timeout JSON
- JSON 파일을 읽는 테스트
- 실패 expectation을 리포트하는 테스트

제외:

- CLI 실행기
- Unity 연동
- 실제 PLC 연결
- 복잡한 분기/루프 시나리오 문법
- YAML 지원

## 위험 등급

- [ ] L0 문서/자동 검증 설정
- [x] L1 테스트/샘플/작은 리팩터링
- [x] L2 시뮬레이션 실행 구조
- [ ] L3 안전, 인터록, 알람 우선순위, 데이터 삭제

현재 구현은 시뮬레이션 테스트용 시나리오 실행기다. 실제 장비 운전 레시피 시스템이 아니다.

## 완료 기준

- 정상 사이클 시나리오 파일이 있다.
- Timeout 시나리오 파일이 있다.
- JSON을 파싱할 수 있다.
- ScenarioRunner가 StartCycle, SetInput, AdvanceTime, PollInputs, ClearAlarm, ExpectState, ExpectSignal을 처리한다.
- 성공/실패 결과를 반환한다.
- 테스트가 통과한다.

## 검증 방법

```powershell
dotnet build --no-restore
dotnet run --project tests\EquipmentTwin.Core.Tests --no-restore
```

## 막힌 점과 해결 방법

- 현재 막힌 점 없음.
- 외부 JSON 패키지를 쓰지 않고 .NET 기본 `System.Text.Json`만 사용했다.

## 보류한 판단

- 시나리오 분기, 반복, 변수 기능은 넣지 않았다.
- 아직 CLI 실행기를 만들지 않았다.
- Unity에서 직접 시나리오를 선택하는 UI는 다음 단계 이후로 미뤘다.

## 아키텍처 설명

```text
Scenario JSON
    ↓
EquipmentScenario
    ↓
ScenarioRunner
    ↓
EquipmentCellController
    ↓
EquipmentStateMachine + VirtualIoController + ManualClock
```

시나리오는 “무엇을 할지”만 담고, 실제 실행은 `ScenarioRunner`가 담당한다.

## 유지보수 포인트

- 시나리오 문법 변경: `ScenarioStep.cs`
- 새 action 추가: `ScenarioStepAction.cs`, `ScenarioRunner.cs`
- 샘플 흐름 추가: `scenarios/`
- 시나리오 실행 테스트: `tests/EquipmentTwin.Core.Tests/Program.cs`
