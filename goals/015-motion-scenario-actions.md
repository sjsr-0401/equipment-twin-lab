# Goal 015: Motion Scenario Actions

## 목표

`MotionAxis`를 테스트 코드 안에서만 쓰지 않고 Scenario JSON action으로 실행할 수 있게 만든다.

CLI batch 리포트에도 각 모션 축의 최종 상태, 위치, 알람을 표시해서 사용자가 “이 시나리오에서 축이 실제로 어디까지 갔는지” 확인할 수 있게 한다.

## 왜 필요한가

Goal 014에서 만든 `MotionAxis`는 모션 축 자체가 Servo On, Home, Move, Timeout을 처리할 수 있는지 검증했다.

하지만 그 상태로는 아직 “사용자가 원하는 장비/공정/트러블 조건을 파일로 골라 실행한다”는 방향과 연결되지 않는다.

이번 Goal은 모션을 JSON 시나리오로 노출한다.

```text
Scenario JSON
    ↓
ScenarioRunner
    ↓
MotionAxis
    ↓
CLI batch report
```

이 구조가 있어야 나중에 Unity UI나 장비 템플릿이 같은 시나리오 실행기를 재사용할 수 있다.

## 한 일

- Scenario action에 모션 명령을 추가했다.
- `ScenarioStep`에 모션 실행/검증 필드를 추가했다.
- `ScenarioRunner`가 축 이름별 `MotionAxis`를 생성하고 보관하게 했다.
- 모션 축 상태, 알람 코드, 위치 expectation을 검증하게 했다.
- CLI Markdown 리포트에 `Motion Axes` 컬럼과 상세 정보를 추가했다.
- 정상 모션 시나리오 JSON을 추가했다.
- 모션 Timeout 시나리오 JSON을 추가했다.
- 회귀 테스트에 모션 시나리오 2개를 추가했다.

## 새 JSON action

```text
MotionServoOn
StartMotionHome
StartMotionMove
PollMotion
CheckMotionTimeout
ExpectMotionState
```

## 새 JSON 필드

```text
axis
targetPosition
durationMilliseconds
timeoutMilliseconds
expectMotionState
expectMotionAlarmCode
expectPosition
```

## 바뀐 파일

- `src/EquipmentTwin.Core/Scenarios/ScenarioStepAction.cs`
- `src/EquipmentTwin.Core/Scenarios/ScenarioStep.cs`
- `src/EquipmentTwin.Core/Scenarios/ScenarioRunner.cs`
- `src/EquipmentTwin.Cli/Program.cs`
- `tests/EquipmentTwin.Core.Tests/Program.cs`
- `scenarios/motion-axis-normal.json`
- `scenarios/motion-axis-timeout.json`
- `README.md`
- `docs/architecture.md`
- `docs/core-validation.md`
- `state/loop-state.md`
- `state/triage.md`
- `state/comprehension.md`
- `logs/2026-07-01.md`
- `plan.md`

## 검증 결과

- Release 빌드 성공
- 경고 0개
- 오류 0개
- 콘솔 테스트 51개 통과
- CLI batch 시나리오 9개 통과
- `artifacts/scenario-report.md`에서 `Motion Axes` 표시 확인

## 막힌 점과 해결 방법

- 큰 막힘은 없었다.
- PowerShell에서 한글 문서가 깨져 보이는 일이 있었지만 파일 인코딩 자체는 UTF-8로 유지되어 있었다.
- 생성 리포트 `artifacts/scenario-report.md`는 검증용 산출물이라 Git에는 포함하지 않는다.

## 보류한 판단

- 모션 축은 아직 `EquipmentCellController`의 공정 상태와 자동 동기화하지 않는다.
- `motion-axis-timeout.json`에서 장비 본체 상태는 `Idle`이고, 축만 `Alarmed`로 남는다. 장비 전체 알람과 축 알람을 연결하는 것은 다음 Equipment Template/Fault Model 단계에서 다룬다.
- 속도/가속도/감속도 프로파일은 아직 없다.
- Unity 오브젝트와 축 상태를 연결하지 않았다.

## 소프트웨어 아키텍처 설명

```text
motion-axis-normal.json
    ↓
EquipmentScenario.FromJson()
    ↓
ScenarioRunner.Run()
    ↓
GetMotionAxis("X")
    ↓
MotionAxis.ServoOn() / StartHome() / StartMove()
    ↓
ScenarioRunner.AddMotionExpectations()
    ↓
EquipmentTwin.Cli BuildMarkdownReport()
```

핵심은 CLI가 모션을 직접 계산하지 않는다는 점이다.

CLI는 `ScenarioRunner`가 실행한 결과를 읽어서 보고서에 표시한다. 모션 판단은 Core의 `MotionAxis`와 `ScenarioRunner`에 남아 있다.

## 유지보수할 때 봐야 할 파일

- 새 action 추가: `src/EquipmentTwin.Core/Scenarios/ScenarioStepAction.cs`
- JSON 필드 검증: `src/EquipmentTwin.Core/Scenarios/ScenarioStep.cs`
- 실행 로직: `src/EquipmentTwin.Core/Scenarios/ScenarioRunner.cs`
- CLI 리포트 표시: `src/EquipmentTwin.Cli/Program.cs`
- 샘플 시나리오: `scenarios/motion-axis-normal.json`, `scenarios/motion-axis-timeout.json`
- 회귀 테스트: `tests/EquipmentTwin.Core.Tests/Program.cs`

## 사용자가 이해해야 할 개념

이번 변경은 “모션 축을 만들었다”가 아니라 “모션 축을 데이터 기반 시나리오로 실행할 수 있게 했다”가 핵심이다.

이 차이가 중요하다.

- 테스트 코드에만 있는 모션: 개발자만 실행하기 쉽다.
- JSON action이 된 모션: CLI, CI, Unity, 장비 템플릿이 같은 입력을 공유할 수 있다.

## 다음 작업

다음 후보는 `Equipment Template / Product Recipe`다.

이제 축 하나는 JSON으로 움직일 수 있으므로, 다음은 “이 장비는 X축, 컨베이어, 검사기, IO를 가진다”는 장비 구성을 데이터로 묶는 단계가 필요하다.
