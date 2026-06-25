# Equipment Twin Lab Architecture

이 문서는 사용자가 나중에 직접 유지보수할 수 있도록 현재 코드 구조를 설명한다.

## 현재 아키텍처 요약

```text
Scenario / Unity / User Command
        ↓
EquipmentCellController
        ↓
EquipmentStateMachine
        ↓
VirtualIoController
```

현재는 Unity나 실제 PLC 없이 `.NET Core` 로직만 있다.

목표는 핵심 장비 로직을 Unity 화면, 실제 PLC, 실제 카메라에 종속시키지 않는 것이다.

## 계층별 역할

### 1. EquipmentStateMachine

파일:

- `src/EquipmentTwin.Core/EquipmentStateMachine.cs`
- `src/EquipmentTwin.Core/EquipmentState.cs`
- `src/EquipmentTwin.Core/EquipmentEvent.cs`

역할:

- 장비가 현재 어떤 상태인지 관리한다.
- 현재 상태에서 허용되는 이벤트만 받아들인다.
- 잘못된 이벤트는 거부한다.
- DoorOpened, EmergencyStop, Timeout 같은 알람 이벤트를 처리한다.

예:

```text
Idle + StartLoad → Loading
Loading + LoadComplete → Aligning
Inspecting + EmergencyStop → Alarmed
```

유지보수 포인트:

- 상태를 추가하려면 `EquipmentState`를 먼저 본다.
- 이벤트를 추가하려면 `EquipmentEvent`를 먼저 본다.
- 상태 전이 규칙은 `EquipmentStateMachine.AllowedTransitions`를 본다.
- 알람 처리 방식은 `ApplyAlarmEvent()`와 `MoveToAlarm()`을 본다.

주의:

- 이 계층은 실제 PLC나 Unity를 직접 몰라야 한다.
- 그래야 나중에 Unity/PLC/테스트가 같은 핵심 로직을 재사용할 수 있다.

## 2. VirtualIoController

파일:

- `src/EquipmentTwin.Core/Io/VirtualIoController.cs`
- `src/EquipmentTwin.Core/Io/EquipmentIoMap.cs`
- `src/EquipmentTwin.Core/Io/IoSignalDirection.cs`
- `src/EquipmentTwin.Core/Io/IoSignalDefinition.cs`
- `src/EquipmentTwin.Core/Io/IoSignalSnapshot.cs`
- `src/EquipmentTwin.Core/Io/IoChange.cs`

역할:

- 실제 PLC 없이 디지털 IO를 흉내 낸다.
- Input과 Output을 구분한다.
- 장비 SW는 Output만 쓴다.
- 시뮬레이터는 Input만 바꾼다.
- IO 변경 이력을 남긴다.

예:

```text
Input  = DI_LOAD_PRESENT
Output = DO_VACUUM_ON
```

유지보수 포인트:

- 새 센서나 출력이 필요하면 `EquipmentIoMap`에 이름을 추가한다.
- 기본 IO 목록은 `CreateDefaultCellIo()`에서 정의한다.
- IO 방향 규칙은 `WriteOutput()`과 `SetInput()`을 본다.

주의:

- Input을 장비 SW가 직접 쓰게 만들면 실제 장비 구조와 멀어진다.
- Output을 시뮬레이터가 마음대로 바꾸게 만들면 책임 경계가 흐려진다.

## 3. Clock / Timeout

파일:

- `src/EquipmentTwin.Core/IClock.cs`
- `src/EquipmentTwin.Core/SystemClock.cs`
- `src/EquipmentTwin.Core/ManualClock.cs`
- `src/EquipmentTwin.Core/StateTimeoutPolicy.cs`
- `src/EquipmentTwin.Core/TimeoutCheckResult.cs`

역할:

- 상태별 허용 시간을 관리한다.
- 실제 시간과 테스트 시간을 분리한다.
- 테스트에서는 `ManualClock`으로 시간을 직접 전진시킨다.

예:

```text
Loading 상태 진입
ManualClock 30초 전진
CheckTimeout()
Alarmed 전환
```

유지보수 포인트:

- Timeout 시간을 바꾸려면 `StateTimeoutPolicy`를 본다.
- 실제 실행 시간은 `SystemClock`을 쓴다.
- 테스트나 시뮬레이션 시간은 `ManualClock`을 쓴다.

주의:

- 이 Timeout은 실제 PLC 타이머나 안전 회로가 아니다.
- 소프트웨어 시뮬레이션과 테스트를 위한 모델이다.

## 4. EquipmentCellController

파일:

- `src/EquipmentTwin.Core/EquipmentCellController.cs`
- `src/EquipmentTwin.Core/EquipmentCellStepResult.cs`

역할:

- 상태머신과 IO를 연결한다.
- IO 입력을 읽어서 상태머신 이벤트로 바꾼다.
- 상태에 맞게 Output을 동기화한다.
- Safety 입력을 정상 공정 입력보다 먼저 처리한다.

예:

```text
DI_LOAD_PRESENT = true
→ LoadComplete 이벤트
→ Loading에서 Aligning으로 전환
```

유지보수 포인트:

- IO 입력과 이벤트 매핑은 `ReadProcessEventForCurrentState()`를 본다.
- DoorOpened/EmergencyStop 우선순위는 `ReadSafetyEvent()`를 본다.
- 상태별 출력 정책은 `SyncOutputsForCurrentState()`를 본다.

주의:

- 현재 출력 정책은 MVP 수준이다.
- 실제 장비에서는 더 많은 인터록과 조건이 필요하다.

## 5. Tests

파일:

- `tests/EquipmentTwin.Core.Tests/Program.cs`

역할:

- 외부 테스트 패키지 없이 콘솔 테스트를 실행한다.
- 빌드 환경이 단순해서 GitHub Actions에서 안정적으로 돈다.

현재 검증 범위:

- 상태머신 정상 전이
- 잘못된 전이 거부
- 알람 전이
- IO 방향 규칙
- IO 이력
- ManualClock
- Timeout
- IO와 상태머신 연결
- 안전 입력 우선순위

유지보수 포인트:

- 기능을 추가하면 이 파일에 테스트를 추가한다.
- 테스트 이름은 사용자가 읽어도 기능을 이해할 수 있게 쓴다.

## 변경할 때 기준

### 상태가 추가될 때

1. `EquipmentState`에 상태 추가
2. `EquipmentEvent`에 필요한 이벤트 추가
3. `EquipmentStateMachine.AllowedTransitions` 수정
4. `EquipmentCellController.SyncOutputsForCurrentState()` 수정
5. 테스트 추가
6. 작업로그에 상태 추가 이유 기록

### IO가 추가될 때

1. `EquipmentIoMap`에 이름 추가
2. `CreateDefaultCellIo()`에 정의 추가
3. 필요하면 `EquipmentCellController`에 매핑 추가
4. 테스트 추가
5. 작업로그에 Input/Output 방향 기록

### Timeout이 추가될 때

1. `StateTimeoutPolicy`에 정책 추가
2. 상태머신 또는 컨트롤러 테스트 추가
3. 실제 장비 안전성으로 과장하지 않기

## 현재 한계

- 실제 PLC 통신 없음
- 실제 모션 제어 없음
- 실제 카메라 없음
- Unity 화면 없음
- 시나리오 파일 없음
- 알람 복구 절차는 단순화됨

## 다음 아키텍처 목표

다음 단계는 공정 시나리오 파일이다.

예상 흐름:

```text
Scenario JSON
    ↓
ScenarioRunner
    ↓
EquipmentCellController
    ↓
StateMachine + Virtual IO + ManualClock
```

이렇게 하면 “정상 공정”, “센서 지연”, “비상정지”, “문 열림”, “Timeout” 같은 시나리오를 파일로 반복 실행할 수 있다.
