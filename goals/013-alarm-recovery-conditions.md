# Goal 013: 알람 복구 조건 세분화

## 목표

`ClearAlarm`을 무조건 허용하지 않고, 알람 원인이 제거됐을 때만 복구되게 한다.

## 왜 필요한가

실제 장비에서는 알람이 발생한 뒤 원인이 남아 있는데 Reset/Clear가 되면 위험하다.

예를 들어 문이 열린 상태에서 알람을 해제하거나, 비상정지가 눌린 상태에서 복구되면 장비 SW 모델이 실제 현장 감각과 멀어진다. 이번 Goal은 MVP 수준에서 최소 복구 조건을 추가한다.

## 한 일

- `AlarmRecoveryCheck`를 추가했다.
- `EquipmentCellController.CheckAlarmRecoveryCondition()`을 추가했다.
- `ClearAlarm()` 전에 복구 조건을 확인하게 했다.
- 문 열림 알람은 `DI_DOOR_CLOSED = true`일 때만 해제되게 했다.
- 비상정지 알람은 `DI_EMERGENCY_STOP_PRESSED = false`일 때만 해제되게 했다.
- Timeout 알람은 MVP 모델에서는 작업자 확인 후 해제 가능한 것으로 남겼다.
- 복구 조건 테스트 5개를 추가했다.
- 복구 조건 시나리오 2개를 추가했다.
- CLI Markdown 리포트에 `Active Alarm`과 `Clear Condition` 컬럼을 추가했다.
- 성공한 시나리오에도 `Errors`가 표시되던 기존 리포트 오류를 수정했다.

## 바뀐 파일

- `src/EquipmentTwin.Core/Alarms/AlarmRecoveryCheck.cs`
- `src/EquipmentTwin.Core/EquipmentCellController.cs`
- `src/EquipmentTwin.Core/EquipmentStateMachine.cs`
- `src/EquipmentTwin.Cli/Program.cs`
- `tests/EquipmentTwin.Core.Tests/Program.cs`
- `scenarios/door-open-clear-blocked.json`
- `scenarios/emergency-stop-recovery.json`
- `README.md`
- `docs/architecture.md`
- `docs/core-validation.md`
- `logs/2026-06-28.md`
- `plan.md`
- `state/comprehension.md`
- `state/loop-state.md`
- `state/triage.md`

## 검증 결과

- Release 빌드 성공
- 경고 0개
- 오류 0개
- 콘솔 테스트 41개 통과
- CLI batch 시나리오 7개 통과
- Markdown 리포트에 알람 코드/복구 조건 표시 확인
- 성공 시나리오에 불필요한 `Errors`가 표시되지 않는 것 확인
- Draft PR #11 생성 완료
- GitHub Actions push 이벤트: 성공
- GitHub Actions pull_request 이벤트: 성공

## 막힌 점과 해결 방법

- 테스트와 batch를 병렬로 실행했을 때 빌드 산출물 파일 잠금이 한 번 발생했다.
- 원인은 코드 오류가 아니라 같은 `Release` 산출물을 두 프로세스가 동시에 잡은 것이었다.
- 해결 방법: 테스트와 batch를 순차 실행했고 둘 다 성공했다.
- 리포트 검증 중 성공한 시나리오에도 `Errors`가 표시되는 기존 버그를 발견했다.
- 해결 방법: `ScenarioCliRun.Errors`가 성공 결과에서는 빈 배열을 반환하게 수정했다.

## 보류한 판단

- Timeout 알람의 실제 복구 조건은 아직 단순화했다.
- 작업자 확인 버튼, Reset 권한, 알람 레벨, 조치 문구는 아직 없다.
- 실제 안전 PLC나 하드웨어 인터록 검증은 아니다.

## 소프트웨어 아키텍처 설명

```text
ClearAlarm command
    ↓
EquipmentCellController.CheckAlarmRecoveryCondition()
    ↓
Virtual IO 상태 확인
    ↓
허용: EquipmentStateMachine.Apply(ClearAlarm)
거부: EquipmentStateMachine.RejectCommand(ClearAlarm)

Scenario batch report
    ↓
ScenarioCliRun
    ↓
BuildMarkdownReport()
    ↓
DescribeActiveAlarm() / DescribeClearCondition()
```

복구 조건은 IO 상태를 봐야 하므로 상태머신 단독 책임이 아니다. 상태머신은 알람 상태와 이력을 관리하고, 셀 컨트롤러가 “현재 센서 상태상 복구 가능한가”를 판단한다.

CLI 리포트는 장비 로직을 새로 실행하지 않고 이미 실행된 `ScenarioRunner`의 최종 상태를 읽는다. 그래서 리포트 계층은 Core 로직을 바꾸지 않는 표시 계층으로 유지된다.

## 유지보수할 때 봐야 할 파일

- 복구 조건 결과: `src/EquipmentTwin.Core/Alarms/AlarmRecoveryCheck.cs`
- 복구 조건 판단: `src/EquipmentTwin.Core/EquipmentCellController.cs`
- 거부된 ClearAlarm 이력 기록: `src/EquipmentTwin.Core/EquipmentStateMachine.cs`
- CLI Markdown 리포트: `src/EquipmentTwin.Cli/Program.cs`
- 테스트: `tests/EquipmentTwin.Core.Tests/Program.cs`
- 시나리오: `scenarios/door-open-clear-blocked.json`, `scenarios/emergency-stop-recovery.json`

## 사용자가 이해해야 할 개념

알람 해제는 “버튼을 눌렀다”가 아니라 “원인이 제거됐고 해제 조건을 만족했다”에 가깝다.

이번 구현은 그 개념을 최소한으로 모델링했다.

## 다음 작업

다음 후보는 PR #11 병합 여부를 결정하는 것이다. 이후 모션 모델 또는 리포트 filter/tag 기능을 시작한다.
