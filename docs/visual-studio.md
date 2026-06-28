# Visual Studio Build / Debug 가이드

이 프로젝트는 Visual Studio 2022에서 build와 debug가 가능하다.

열어야 할 파일:

```text
EquipmentTwinLab.sln
```

## 1. Build 방법

Visual Studio에서 솔루션을 연 뒤:

```text
Build > Build Solution
```

또는 단축키:

```text
Ctrl + Shift + B
```

빌드되는 프로젝트:

- `EquipmentTwin.Core`
- `EquipmentTwin.Cli`
- `EquipmentTwin.Core.Tests`

## 2. Debug 가능한 대상

현재는 Unity나 실제 PLC가 없으므로 아래 두 프로젝트를 주로 디버그한다.

| 시작 프로젝트 | 용도 |
|---|---|
| `EquipmentTwin.Cli` | JSON 시나리오를 직접 실행하면서 디버그 |
| `EquipmentTwin.Core.Tests` | 상태머신/IO/Timeout/알람 코드 테스트를 한 번에 디버그 |

## 3. CLI 시나리오 디버그

1. Solution Explorer에서 `EquipmentTwin.Cli`를 우클릭한다.
2. `Set as Startup Project`를 선택한다.
3. 상단 실행 프로필에서 아래 중 하나를 고른다.

```text
CLI - normal cycle
CLI - loading timeout
CLI - batch report
```

4. `F5`를 누른다.

브레이크포인트 추천 위치:

- `src/EquipmentTwin.Cli/Program.cs`
- `src/EquipmentTwin.Core/Scenarios/ScenarioRunner.cs`
- `src/EquipmentTwin.Core/EquipmentCellController.cs`
- `src/EquipmentTwin.Core/EquipmentStateMachine.cs`

## 4. 테스트 디버그

현재 테스트는 xUnit/NUnit 패키지를 쓰지 않는 단순 콘솔 테스트 러너다.

1. Solution Explorer에서 `EquipmentTwin.Core.Tests`를 우클릭한다.
2. `Set as Startup Project`를 선택한다.
3. 실행 프로필에서 아래를 고른다.

```text
Tests - run all console tests
```

4. `F5`를 누른다.

브레이크포인트 추천 위치:

- `tests/EquipmentTwin.Core.Tests/Program.cs`
- `src/EquipmentTwin.Core/EquipmentStateMachine.cs`
- `src/EquipmentTwin.Core/Alarms/AlarmInfo.cs`

## 5. 지금 가능한 것과 아직 안 되는 것

가능한 것:

- Visual Studio에서 솔루션 build
- CLI 시나리오 debug
- 콘솔 테스트 debug
- Core 코드에 breakpoint 걸고 상태 전이 추적
- 알람 코드 생성 흐름 추적

아직 안 되는 것:

- Unity 3D 화면 debug
- 실제 PLC 통신 debug
- 실제 카메라 입력 debug
- Visual Studio Test Explorer에서 테스트 목록을 개별 실행

Test Explorer를 쓰려면 나중에 xUnit/NUnit 프로젝트로 전환해야 한다. 현재는 외부 테스트 패키지 없이 단순하게 유지하는 것이 목적이다.

## 6. 자주 볼 코드 흐름

정상 시나리오:

```text
EquipmentTwin.Cli
    ↓
ScenarioRunner
    ↓
EquipmentCellController
    ↓
EquipmentStateMachine
```

알람 코드 흐름:

```text
DoorOpened / EmergencyStop / Timeout
    ↓
AlarmInfo.FromEvent()
    ↓
EquipmentStateMachine.LastAlarm
    ↓
TransitionResult.Alarm
    ↓
EquipmentTransition.Alarm
```

## 7. 디버그할 때 확인할 값

상태머신:

- `CurrentState`
- `StateEnteredAtUtc`
- `LastAlarm`
- `History`

IO:

- `DI_DOOR_CLOSED`
- `DI_EMERGENCY_STOP_PRESSED`
- `DI_LOAD_PRESENT`
- `DO_VACUUM_ON`
- `DO_TOWER_LAMP_RED`
- `DO_BUZZER_ON`

시나리오:

- 현재 step 이름
- 기대 상태
- 실제 상태
- 기대 IO
- 실제 IO
