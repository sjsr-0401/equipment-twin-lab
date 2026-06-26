# Goal 011: 알람 코드 체계

## 목표

알람을 문자열 메시지로만 다루지 않고, 코드와 원인 이벤트를 함께 추적한다.

## 왜 필요한가

현업 장비에서는 알람을 사람이 빠르게 식별하고, 로그/화면/복구 절차에서 같은 기준으로 추적해야 한다.

문자열만 있으면 메시지가 바뀔 때 검색과 집계가 흔들릴 수 있다. 알람 코드가 있으면 “문 열림”, “비상정지”, “Timeout”을 안정적으로 분류할 수 있다.

## 한 일

- `AlarmCode` enum을 추가했다.
- `AlarmInfo` record를 추가했다.
- 상태머신이 알람 발생 시 `LastAlarm`을 저장하게 했다.
- `TransitionResult`와 `EquipmentTransition`에 알람 정보를 포함했다.
- `ClearAlarm` 이후 알람 코드와 알람 메시지가 지워지는지 검증했다.
- 알람 코드 테스트 4개를 추가했다.

## 바뀐 파일

- `src/EquipmentTwin.Core/Alarms/AlarmCode.cs`
- `src/EquipmentTwin.Core/Alarms/AlarmInfo.cs`
- `src/EquipmentTwin.Core/EquipmentStateMachine.cs`
- `src/EquipmentTwin.Core/EquipmentTransition.cs`
- `src/EquipmentTwin.Core/TransitionResult.cs`
- `tests/EquipmentTwin.Core.Tests/Program.cs`
- `README.md`
- `docs/architecture.md`
- `docs/core-validation.md`
- `state/comprehension.md`
- `state/loop-state.md`
- `state/triage.md`
- `logs/2026-06-26.md`
- `plan.md`

## 검증 결과

- Release 빌드 성공
- 경고 0개
- 오류 0개
- 콘솔 테스트 34개 통과
- CLI batch 시나리오 5개 통과

## 막힌 점과 해결 방법

- 현재 막힌 점 없음.

## 보류한 판단

- 알람 코드 번호 체계는 MVP용 임시 범위다.
- 실제 장비처럼 알람 레벨, 작업자 조치 문구, 복구 조건 테이블은 아직 만들지 않았다.
- 알람 이력 리포트와 UI 표시는 아직 없다.

## 소프트웨어 아키텍처 설명

```text
Safety Event / Timeout
    ↓
AlarmInfo.FromEvent()
    ↓
EquipmentStateMachine.LastAlarm
    ↓
TransitionResult / EquipmentTransition
    ↓
Test / Log / future UI
```

핵심은 알람을 단순 문자열이 아니라 구조화된 값으로 남기는 것이다.

## 유지보수할 때 봐야 할 파일

- 알람 코드: `src/EquipmentTwin.Core/Alarms/AlarmCode.cs`
- 알람 정보 생성: `src/EquipmentTwin.Core/Alarms/AlarmInfo.cs`
- 알람 상태 전이: `src/EquipmentTwin.Core/EquipmentStateMachine.cs`
- 전이 결과: `src/EquipmentTwin.Core/TransitionResult.cs`
- 이력: `src/EquipmentTwin.Core/EquipmentTransition.cs`
- 테스트: `tests/EquipmentTwin.Core.Tests/Program.cs`

## 사용자가 이해해야 할 개념

알람 코드는 현장에서 문제를 분류하기 위한 식별자다.

예를 들어 “Door opened during operation.”이라는 문구는 바뀔 수 있지만, `DoorOpened = 1001`이라는 코드는 로그, UI, 시나리오 검증에서 같은 의미로 남는다.

## 다음 작업

다음 후보는 복구 조건 세분화다.

현재 `ClearAlarm`은 단순 복구 명령이다. 다음에는 “문이 다시 닫혔는가”, “비상정지가 해제됐는가”, “작업자 Reset을 눌렀는가” 같은 조건을 분리할 수 있다.
