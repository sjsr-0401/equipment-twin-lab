# Goal 009: 알람/복구 시나리오 추가

## 목표

장비가 정상 흐름뿐 아니라 대표적인 알람 상황과 기본 복구 흐름도 시나리오 파일로 반복 검증할 수 있게 한다.

이번 Goal에서 다루는 범위:

- 문 열림 알람
- 비상정지 알람
- 문 열림 알람 이후 ClearAlarm 복구

## 왜 필요한가

제조 장비 SW는 정상 공정만 잘 도는 것으로 충분하지 않다.

실제 현장에서는 문 열림, 비상정지, 센서 지연, Timeout 같은 예외 상황이 더 중요하게 검증된다. 이번 작업은 장비 없이도 “알람이 발생하면 출력이 안전 상태로 내려가는지”와 “복구 명령 후 Idle로 돌아가는지”를 반복 실행 가능한 JSON 시나리오로 남기는 단계다.

## 한 일

- `door-open-alarm.json` 시나리오를 추가했다.
- `emergency-stop-alarm.json` 시나리오를 추가했다.
- `clear-alarm-recovery.json` 시나리오를 추가했다.
- 새 시나리오 3개를 테스트에서 직접 실행하게 했다.
- CLI batch 실행 대상이 2개에서 5개 시나리오로 늘었다.

## 바뀐 파일

- `scenarios/door-open-alarm.json`
- `scenarios/emergency-stop-alarm.json`
- `scenarios/clear-alarm-recovery.json`
- `tests/EquipmentTwin.Core.Tests/Program.cs`
- `README.md`
- `docs/architecture.md`
- `state/loop-state.md`
- `state/triage.md`
- `state/comprehension.md`
- `logs/2026-06-25.md`
- `plan.md`

## 검증 결과

- `dotnet restore EquipmentTwinLab.sln --ignore-failed-sources`: 성공
- `dotnet build EquipmentTwinLab.sln --no-restore --configuration Release`: 성공
- 콘솔 테스트: 30개 통과
- CLI batch: 5개 시나리오 통과
- Draft PR #7 생성 완료
- GitHub Actions push 이벤트: 성공
- GitHub Actions pull_request 이벤트: 성공

Batch 통과 시나리오:

- `clear-alarm-recovery` → `Idle`
- `door-open-alarm` → `Alarmed`
- `emergency-stop-alarm` → `Alarmed`
- `loading-timeout` → `Alarmed`
- `normal-cycle` → `Complete`

## 막힌 점과 해결 방법

- 현재 막힌 점 없음.
- 다만 실제 장비 안전 검증은 아니다. 현재 검증은 Core 상태머신, 가상 IO, 시나리오 Runner가 의도대로 동작하는지 확인하는 소프트웨어 시뮬레이션 검증이다.

## 보류한 판단

- 알람 코드 체계는 아직 없다.
- 알람 이력/발생 시간 리포트는 아직 단순하다.
- 실제 장비처럼 복구 전 작업자 확인, Door Close 확인, Reset 조건 테이블을 세분화하지 않았다.
- `ClearAlarm`은 현재 MVP용 단순 복구 명령이다.

## 소프트웨어 아키텍처 설명

```text
Alarm Scenario JSON
    ↓
ScenarioRunner
    ↓
EquipmentCellController
    ↓
ReadSafetyEvent()
    ↓
EquipmentStateMachine
    ↓
Alarmed 또는 Idle
```

알람 시나리오는 기존 Core 로직을 새로 만들지 않고, JSON 파일로 입력 조건과 기대 결과를 정의한다.

`ScenarioRunner`는 JSON step을 실행하고, `EquipmentCellController`는 문 열림/비상정지 같은 safety 입력을 정상 공정 입력보다 먼저 읽는다. 그 결과 `EquipmentStateMachine`이 `Alarmed`로 전환하고, 출력은 적색 램프/부저 ON, 진공/스테이지 이동 OFF로 동기화된다.

## 유지보수할 때 봐야 할 파일

- 알람 상태 전이: `src/EquipmentTwin.Core/EquipmentStateMachine.cs`
- Safety 입력 우선순위: `src/EquipmentTwin.Core/EquipmentCellController.cs`
- IO 이름과 방향: `src/EquipmentTwin.Core/Io/EquipmentIoMap.cs`
- 시나리오 실행: `src/EquipmentTwin.Core/Scenarios/ScenarioRunner.cs`
- 시나리오 파일: `scenarios/`
- 회귀 테스트: `tests/EquipmentTwin.Core.Tests/Program.cs`

## 사용자가 이해해야 할 개념

알람 시나리오는 “고장이 났다”를 화면으로 보여주는 기능이 아니라, 고장 입력이 들어왔을 때 장비 SW가 안전한 상태로 가는지 확인하는 테스트다.

현재 프로젝트에서 중요한 검증 질문은 아래와 같다.

1. Door open 또는 Emergency stop이 정상 공정 신호보다 먼저 처리되는가?
2. Alarmed 상태가 되면 위험한 출력이 꺼지는가?
3. ClearAlarm 이후 장비가 다시 Idle로 돌아가는가?
4. 이 흐름을 사람이 손으로 누르지 않고 JSON + batch로 반복 검증할 수 있는가?

## 다음 작업

권장 다음 작업은 PR #7을 병합한 뒤 `Core 검증 정리 문서`를 작성하는 것이다.

이유는 현재 Core에 상태머신, IO, Timeout, Scenario, CLI batch까지 들어갔으므로, 면접/포트폴리오에서 설명할 수 있는 형태로 “무엇을 검증했고 무엇을 검증하지 않았는지”를 정리할 타이밍이기 때문이다.
