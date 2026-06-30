# Goal 018: Fault Model

## 목표

사용자가 선택할 수 있는 트러블 조건을 Equipment Template에 추가하고, Template Runner가 실행 중 해당 fault를 주입할 수 있게 만든다.

이번 Goal은 모든 고장 모델이 아니다. 우선 모션 계열 트러블인 `MotionTimeout`, `ServoAlarm`만 다룬다.

## 왜 필요한가

사용자가 원하는 대표 프로젝트는 정상 공정만 보여주는 프로그램이 아니다.

장비, 제품, 공정, 검사 조건뿐 아니라 트러블 조건도 선택해서 실제처럼 시뮬레이션해야 한다.

이번 작업은 그 첫 단계다.

```text
Equipment Template
    ↓
Fault Scenario
    ↓
Template Runner
    ↓
Motion Timeout 또는 Servo Alarm 주입
```

## 한 일

- `FaultKind`를 추가했다.
- `FaultScenario`를 추가했다.
- Equipment Template에 `faultScenarios`를 추가했다.
- 샘플 fault 2개를 추가했다.
  - `x-axis-move-timeout`
  - `z-axis-servo-alarm`
- Template Runner가 optional fault scenario를 받아 실행 중 주입하게 했다.
- fault 관련 테스트 6개를 추가했다.

## 바뀐 파일

- `src/EquipmentTwin.Core/Templates/FaultKind.cs`
- `src/EquipmentTwin.Core/Templates/FaultScenario.cs`
- `src/EquipmentTwin.Core/Templates/EquipmentTemplate.cs`
- `src/EquipmentTwin.Core/Templates/TemplateRunner.cs`
- `src/EquipmentTwin.Core/Templates/TemplateRunResult.cs`
- `templates/vision-inspection-cell.json`
- `tests/EquipmentTwin.Core.Tests/Program.cs`
- `README.md`
- `docs/architecture.md`
- `docs/core-validation.md`
- `logs/2026-07-01.md`
- `plan.md`
- `state/loop-state.md`
- `state/triage.md`
- `state/comprehension.md`

## 검증 결과

- Release 빌드 성공
- 경고 0개
- 오류 0개
- 콘솔 테스트 66개 통과
- CLI batch 시나리오 9개 통과

## 막힌 점과 해결 방법

- 이번 구현 자체에서 큰 막힘은 없었다.
- 이전 Goal에서 배운 대로 Release build/test는 병렬이 아니라 순차 실행했다.

## 보류한 판단

- IO fault는 아직 없다.
- Sensor stuck, door open, emergency stop 같은 공정 fault는 아직 template fault로 통합하지 않았다.
- 검사 NG/PASS 결과 모델은 아직 없다.
- fault severity, operator action, recovery guide는 아직 없다.

## 소프트웨어 아키텍처 설명

```text
TemplateRunner.RunRecipe(template, "default-panel", "x-axis-move-timeout")
    ↓
FindFaultScenario()
    ↓
StartMove(X)
    ↓
Advance fault elapsed time
    ↓
CheckTimeout()
    ↓
TemplateRunResult.Success = false
```

Servo alarm도 같은 구조로 실행된다.

```text
TemplateRunner.RunRecipe(template, "default-panel", "z-axis-servo-alarm")
    ↓
StartMove(Z)
    ↓
TriggerServoAlarm()
    ↓
TemplateRunResult.Success = false
```

## 유지보수할 때 봐야 할 파일

- fault 종류: `src/EquipmentTwin.Core/Templates/FaultKind.cs`
- fault 정의와 validation: `src/EquipmentTwin.Core/Templates/FaultScenario.cs`
- template에 fault 목록 포함: `src/EquipmentTwin.Core/Templates/EquipmentTemplate.cs`
- fault 실행: `src/EquipmentTwin.Core/Templates/TemplateRunner.cs`
- 샘플 fault: `templates/vision-inspection-cell.json`
- 테스트: `tests/EquipmentTwin.Core.Tests/Program.cs`

## 사용자가 이해해야 할 개념

Fault Scenario는 “실제 물리 고장”을 완전히 재현하는 것이 아니다.

지금 단계에서는 장비 SW가 트러블 조건을 어떻게 받아들이고, 실행 중 어떤 축이 Alarmed로 가는지 반복 검증하는 데이터 모델이다.

이 모델이 있어야 나중에 Unity 화면에서 사용자가 “X축 Timeout”, “Z축 Servo Alarm” 같은 트러블을 선택할 수 있다.

## 다음 작업

다음 후보는 `Inspection Result Model`이다.

정상 실행과 모션 fault가 생겼으니, 이제 제품 검사 PASS/FAIL을 데이터로 표현하는 단계가 필요하다.
