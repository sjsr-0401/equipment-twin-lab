# Equipment Twin Lab Architecture

이 문서는 사용자가 나중에 직접 유지보수할 수 있도록 현재 코드 구조를 설명한다.

검증 범위와 한계는 `docs/core-validation.md`에 별도로 정리한다.

## 현재 아키텍처 요약

```text
Scenario JSON / CLI / Unity / User Command
        ↓
Equipment Template / Product Recipe
        ↓
ScenarioRunner
        ├─ EquipmentCellController → EquipmentStateMachine + VirtualIoController
        └─ MotionAxis
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

### 1.1 Alarm model

파일:

- `src/EquipmentTwin.Core/Alarms/AlarmCode.cs`
- `src/EquipmentTwin.Core/Alarms/AlarmInfo.cs`

역할:

- 알람을 문자열 메시지뿐 아니라 코드로 추적한다.
- 알람 발생 원인이 된 이벤트를 함께 남긴다.
- 상태 전이 결과와 이력에서 같은 알람 정보를 확인할 수 있게 한다.

현재 코드:

```text
DoorOpened    = 1001
EmergencyStop = 1002
StateTimeout  = 1003
```

주의:

- 현재 번호 체계는 MVP용이다.
- 실제 장비 알람 표준, 조치 문구, 복구 조건 테이블은 아직 아니다.

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
- JSON 시나리오 실행
- 알람/복구 시나리오 실행
- 모션 축 단위 동작
- 모션 축 JSON 시나리오 실행
- Equipment Template / Product Recipe JSON 검증

유지보수 포인트:

- 기능을 추가하면 이 파일에 테스트를 추가한다.
- 테스트 이름은 사용자가 읽어도 기능을 이해할 수 있게 쓴다.

## 6. ScenarioRunner

파일:

- `src/EquipmentTwin.Core/Scenarios/EquipmentScenario.cs`
- `src/EquipmentTwin.Core/Scenarios/ScenarioStep.cs`
- `src/EquipmentTwin.Core/Scenarios/ScenarioStepAction.cs`
- `src/EquipmentTwin.Core/Scenarios/ScenarioRunner.cs`
- `src/EquipmentTwin.Core/Scenarios/ScenarioRunResult.cs`
- `src/EquipmentTwin.Core/Scenarios/ScenarioStepRunResult.cs`
- `scenarios/normal-cycle.json`
- `scenarios/loading-timeout.json`
- `scenarios/door-open-alarm.json`
- `scenarios/emergency-stop-alarm.json`
- `scenarios/clear-alarm-recovery.json`
- `scenarios/door-open-clear-blocked.json`
- `scenarios/emergency-stop-recovery.json`
- `scenarios/motion-axis-normal.json`
- `scenarios/motion-axis-timeout.json`

역할:

- JSON 파일로 장비 운전 흐름을 정의한다.
- 정의된 step을 순서대로 실행한다.
- 상태 expectation과 IO signal expectation을 검증한다.
- 모션 축 action을 실행하고 축 상태, 알람, 위치 expectation을 검증한다.
- 성공/실패 결과를 리포트한다.

예:

```text
SetInput DI_LOAD_PRESENT = true
PollInputs
ExpectState Aligning
```

유지보수 포인트:

- 새 시나리오 action이 필요하면 `ScenarioStepAction`과 `ScenarioRunner.RunStep()`을 같이 수정한다.
- JSON 필드가 바뀌면 `ScenarioStep` validation도 같이 수정한다.
- 모션 축 JSON 필드는 `axis`, `targetPosition`, `durationMilliseconds`, `timeoutMilliseconds`, `expectMotionState`, `expectMotionAlarmCode`, `expectPosition`을 쓴다.
- 샘플 시나리오는 `scenarios/` 폴더에 추가한다.

주의:

- 현재 시나리오 문법은 단순한 순차 실행만 지원한다.
- 분기, 반복, 변수, 복잡한 recipe 기능은 아직 없다.
- 실제 장비 recipe 시스템이 아니라 시뮬레이션 테스트용이다.

## 7. EquipmentTwin.Cli

파일:

- `src/EquipmentTwin.Cli/Program.cs`
- `src/EquipmentTwin.Cli/EquipmentTwin.Cli.csproj`

역할:

- JSON 시나리오 파일을 명령어로 실행한다.
- 정상/실패 결과를 콘솔에 출력한다.
- 최종 장비 상태와 IO Snapshot을 보여준다.
- 실패 시 non-zero exit code를 반환해서 CI에서 잡을 수 있게 한다.
- 여러 시나리오를 batch로 실행하고 Markdown 리포트를 저장한다.
- Markdown 리포트에서 활성 알람 코드와 ClearAlarm 가능 조건을 보여준다.
- Markdown 리포트에서 각 모션 축의 최종 상태, 위치, 모션 알람을 보여준다.

예:

```powershell
dotnet run --project src\EquipmentTwin.Cli -- scenarios\normal-cycle.json
dotnet run --project src\EquipmentTwin.Cli -- scenarios\loading-timeout.json --default-timeouts
dotnet run --project src\EquipmentTwin.Cli -- batch scenarios --default-timeouts --report artifacts\scenario-report.md
```

유지보수 포인트:

- CLI 인자 처리는 `CliOptions.Parse()`를 본다.
- 출력 형식은 `PrintResult()`를 본다.
- Batch 출력 형식은 `PrintBatchResult()`를 본다.
- Markdown 리포트 형식은 `BuildMarkdownReport()`를 본다.
- 리포트의 알람 표시 형식은 `DescribeActiveAlarm()`과 `DescribeClearCondition()`을 본다.
- 리포트의 모션 축 표시 형식은 `DescribeMotionAxes()`를 본다.
- 실제 실행은 `ScenarioRunner`에 위임한다.

주의:

- CLI는 Core 로직을 다시 구현하지 않는다.
- CLI는 사용자/CI가 ScenarioRunner를 쉽게 호출하게 하는 얇은 껍데기다.
- CLI 리포트는 `CheckAlarmRecoveryCondition()`을 읽기 전용으로 호출해서 복구 가능 여부를 표시한다.
- 실제 장비 운전용 UI나 recipe editor가 아니다.

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

### Scenario action이 추가될 때

1. `ScenarioStepAction`에 action 추가
2. `ScenarioStep.Validate()`에 필수 필드 규칙 추가
3. `ScenarioRunner.RunStep()`에 실행 로직 추가
4. 정상/실패 테스트 추가
5. 샘플 JSON 추가 또는 수정

### CLI 옵션이 추가될 때

1. `EquipmentTwin.Cli/Program.cs`의 `CliOptions.Parse()` 수정
2. `PrintUsage()` 수정
3. README 실행 예시 수정
4. 필요하면 CI 실행 단계 추가

## 현재 한계

- 실제 PLC 통신 없음
- 실제 모션 제어 없음
- 실제 카메라 없음
- Unity 화면 없음
- 시나리오 분기/반복 없음
- CLI batch filtering 없음
- 리포트 artifact 업로드 없음
- 알람 복구 조건은 문 열림/비상정지 기준으로 시작했지만 Timeout 복구는 아직 단순화됨
- 모션 축은 JSON 시나리오로 실행 가능하지만 아직 장비 공정 상태와 자동으로 동기화되는 Equipment Template 단계는 아니다.
- Equipment Template는 아직 축과 recipe를 정의하는 모델이며, ScenarioRunner가 자동으로 template를 실행 계획으로 변환하지는 않는다.
- Template Runner는 template/recipe를 가상 모션 실행으로 변환하고, 선택한 Fault Scenario를 모션 실행 중 주입할 수 있다. 아직 IO/검사 결과까지 포함한 전체 공정 실행 엔진은 아니다.

## 다음 아키텍처 목표

다음 단계는 모션 축, IO, 검사, fault 조건을 하나의 Equipment Template로 묶는 것이다.

예상 흐름:

```text
Equipment Template / Product Recipe
    ↓
ScenarioRunner
    ├─ EquipmentCellController
    ├─ MotionAxis
    └─ future Inspection / Fault Model
```

이렇게 하면 “정상 공정”, “센서 지연”, “비상정지”, “문 열림”, “모션 Timeout”, “검사 NG” 같은 시나리오를 파일로 반복 실행할 수 있다.

## 8. 알람/복구 시나리오

파일:

- `scenarios/door-open-alarm.json`
- `scenarios/emergency-stop-alarm.json`
- `scenarios/clear-alarm-recovery.json`
- `scenarios/door-open-clear-blocked.json`
- `scenarios/emergency-stop-recovery.json`

역할:

- 문 열림이나 비상정지 입력이 들어오면 `Alarmed` 상태로 전환되는지 확인한다.
- 알람 상태에서 진공, 스테이지 이동 같은 정상 공정 출력이 꺼지는지 확인한다.
- 적색 램프와 부저가 켜지는지 확인한다.
- Door close 이후 `ClearAlarm`으로 `Idle` 상태로 돌아오는 기본 복구 흐름을 확인한다.
- Door open 상태에서는 `ClearAlarm`이 거부되는지 확인한다.
- Emergency stop 해제 이후 `ClearAlarm`으로 `Idle` 상태로 돌아오는지 확인한다.

아키텍처 흐름:

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

유지보수 포인트:

- Safety 입력 우선순위는 `EquipmentCellController.ReadSafetyEvent()`를 본다.
- 알람 상태 전이는 `EquipmentStateMachine.ApplyAlarmEvent()`와 `MoveToAlarm()`을 본다.
- 알람 출력 정책은 `EquipmentCellController.SyncOutputsForCurrentState()`를 본다.
- 알람 복구 조건은 `EquipmentCellController.CheckAlarmRecoveryCondition()`을 본다.
- 새 알람 시나리오는 `scenarios/`에 JSON을 추가하고 `tests/EquipmentTwin.Core.Tests/Program.cs`에 회귀 테스트를 추가한다.

주의:

- 이 검증은 실제 장비 안전 인증이나 실제 인터록 검증이 아니다.
- 현재는 소프트웨어 모델이 의도한 상태 전이와 출력 정책을 지키는지 확인하는 단계다.

## 9. Motion Axis

파일:

- `src/EquipmentTwin.Core/Motion/MotionAxis.cs`
- `src/EquipmentTwin.Core/Motion/MotionAxisState.cs`
- `src/EquipmentTwin.Core/Motion/MotionAxisAlarmCode.cs`
- `src/EquipmentTwin.Core/Motion/MotionAxisAlarm.cs`
- `src/EquipmentTwin.Core/Motion/MotionCommandResult.cs`

역할:

- 실제 모션 컨트롤러 없이 축 상태를 모델링한다.
- Servo On, Home, Move, InPosition, Timeout, Servo Alarm 흐름을 검증한다.
- 나중에 Unity, Equipment Template, Product Recipe가 같은 모션 모델을 사용할 수 있게 한다.

흐름:

```text
MotionAxis
    ↓
ServoOn()
    ↓
StartHome()
    ↓
Poll()
    ↓
StartMove()
    ↓
Poll() 또는 CheckTimeout()
    ↓
InPosition 또는 Alarmed
```

유지보수 포인트:

- 축 상태 추가/변경: `MotionAxisState`
- 모션 명령 동작 변경: `MotionAxis`
- 모션 알람 코드 변경: `MotionAxisAlarmCode`
- 테스트 추가: `tests/EquipmentTwin.Core.Tests/Program.cs`

주의:

- 이 계층은 실제 서보 드라이버가 아니다.
- 현재는 위치 완료와 Timeout을 검증하는 MVP 모델이다.
- 속도/가속도/감속도 프로파일은 아직 없다.
- 현재는 Scenario JSON action으로 직접 실행할 수 있지만, 아직 Equipment Template가 자동으로 축/IO/검사를 묶어주는 단계는 아니다.

### 9.1 Motion Scenario Actions

파일:

- `src/EquipmentTwin.Core/Scenarios/ScenarioStepAction.cs`
- `src/EquipmentTwin.Core/Scenarios/ScenarioStep.cs`
- `src/EquipmentTwin.Core/Scenarios/ScenarioRunner.cs`
- `scenarios/motion-axis-normal.json`
- `scenarios/motion-axis-timeout.json`

역할:

- JSON 시나리오에서 모션 축을 직접 실행한다.
- `MotionServoOn`, `StartMotionHome`, `StartMotionMove`, `PollMotion`, `CheckMotionTimeout`, `ExpectMotionState` action을 제공한다.
- 같은 ScenarioRunner 안에서 축 이름별 `MotionAxis` 인스턴스를 보관한다.
- CLI batch 리포트가 마지막 축 상태를 사람이 볼 수 있게 표시한다.

흐름:

```text
motion-axis-normal.json
    ↓
ScenarioRunner
    ↓
X MotionAxis
    ↓
ServoOn → Home → Move → InPosition
```

유지보수 포인트:

- 새 모션 action 추가: `ScenarioStepAction`, `ScenarioStep.Validate()`, `ScenarioRunner.RunStep()`
- 모션 expectation 추가: `ScenarioRunner.AddMotionExpectations()`
- CLI 표시 변경: `EquipmentTwin.Cli/Program.cs`의 `DescribeMotionAxes()`

## 10. Equipment Template / Product Recipe

파일:

- `src/EquipmentTwin.Core/Templates/EquipmentTemplate.cs`
- `src/EquipmentTwin.Core/Templates/MotionAxisTemplate.cs`
- `src/EquipmentTwin.Core/Templates/ProductRecipe.cs`
- `src/EquipmentTwin.Core/Templates/InspectionMode.cs`
- `templates/vision-inspection-cell.json`

역할:

- 사용자가 선택할 수 있는 장비 구성을 JSON으로 정의한다.
- 장비가 어떤 모션 축을 갖는지 정의한다.
- 제품 recipe별 목표 축 위치와 검사 모드를 정의한다.
- 중복 축 이름이나 존재하지 않는 축을 참조하는 recipe를 거부한다.

현재 예시:

```text
vision-inspection-cell
    ├─ Motion axis: X
    ├─ Motion axis: Z
    ├─ Recipe: default-panel → X=25, Z=5, DatasetCamera
    └─ Recipe: tall-part     → X=40, Z=12, DatasetCamera
```

아키텍처 흐름:

```text
EquipmentTemplate JSON
    ↓
EquipmentTemplate.FromJson()
    ↓
Validate axes and recipes
    ↓
CreateMotionAxes(clock)
```

유지보수 포인트:

- 장비 구성 필드 변경: `EquipmentTemplate`
- 축 정의 변경: `MotionAxisTemplate`
- 제품별 목표 위치 변경: `ProductRecipe`
- 검사 모드 추가: `InspectionMode`
- 샘플 장비 추가: `templates/`

주의:

- 이 단계는 아직 Unity 장비 빌더가 아니다.
- 이 단계는 아직 recipe 실행 엔진이 아니다.
- 현재는 “장비 구성 데이터를 안전하게 읽고 검증하는 Core 모델”이다.

## 11. Template Runner

파일:

- `src/EquipmentTwin.Core/Templates/TemplateRunner.cs`
- `src/EquipmentTwin.Core/Templates/TemplateRunnerOptions.cs`
- `src/EquipmentTwin.Core/Templates/TemplateRunResult.cs`
- `src/EquipmentTwin.Core/Templates/TemplateMotionCommandLog.cs`

역할:

- 선택한 `EquipmentTemplate`과 `ProductRecipe`를 실제 가상 모션 실행으로 변환한다.
- 템플릿에 정의된 축을 생성한다.
- 각 축에 Servo On, Home, Move 명령을 순서대로 실행한다.
- 실행 결과와 명령 로그를 `TemplateRunResult`로 남긴다.
- 선택한 fault scenario가 있으면 해당 축 move 중 모션 Timeout 또는 Servo Alarm을 주입한다.

흐름:

```text
EquipmentTemplate + recipeName
    ↓
TemplateRunner.RunRecipe()
    ↓
CreateMotionAxes(clock)
    ↓
ServoOn / Home / Move
    ↓
optional FaultScenario
    ↓
TemplateRunResult
```

유지보수 포인트:

- 실행 순서 변경: `TemplateRunner.RunRecipe()`
- 표준 home/move 시간 변경: `TemplateRunnerOptions`
- 실행 결과에 더 많은 정보 추가: `TemplateRunResult`
- 명령 로그 형식 변경: `TemplateMotionCommandLog`

주의:

- 현재 Template Runner는 모션 축과 모션 fault만 실행한다.
- IO 시뮬레이션은 아직 연결하지 않았다.
- 검사 결과는 `ProductRecipe.InspectionResult`를 통해 데이터 기반으로 생성한다.
- 실제 장비 recipe 엔진이 아니라 configurable twin을 위한 첫 실행 계층이다.

## 12. Fault Model

파일:

- `src/EquipmentTwin.Core/Templates/FaultScenario.cs`
- `src/EquipmentTwin.Core/Templates/FaultKind.cs`
- `templates/vision-inspection-cell.json`

역할:

- 사용자가 선택할 수 있는 트러블 조건을 JSON으로 정의한다.
- 현재는 `MotionTimeout`, `ServoAlarm` 두 종류를 지원한다.
- fault가 참조하는 축이 template에 실제로 있는지 검증한다.
- Timeout fault는 `timeoutMilliseconds`와 `elapsedMilliseconds`를 검증한다.

현재 예시:

```text
x-axis-move-timeout
    └─ X축 move 중 timeout 발생

z-axis-servo-alarm
    └─ Z축 move 중 servo amplifier alarm 발생
```

아키텍처 흐름:

```text
FaultScenario JSON
    ↓
EquipmentTemplate.Validate()
    ↓
TemplateRunner.RunRecipe(template, recipe, faultName)
    ↓
MotionAxis.CheckTimeout() 또는 MotionAxis.TriggerServoAlarm()
    ↓
TemplateRunResult.Success = false
```

유지보수 포인트:

- 새 fault 종류 추가: `FaultKind`
- fault 필드/검증 변경: `FaultScenario`
- fault 실행 위치 변경: `TemplateRunner.InjectFault()`
- 샘플 fault 추가: `templates/vision-inspection-cell.json`

주의:

- 이 fault model은 실제 고장 물리 모델이 아니다.
- 현재는 소프트웨어 시뮬레이션에서 재현 가능한 대표 트러블 조건을 정의하는 단계다.

## 13. Inspection Result Model

파일:

- `src/EquipmentTwin.Core/Templates/InspectionOutcome.cs`
- `src/EquipmentTwin.Core/Templates/InspectionScenario.cs`
- `src/EquipmentTwin.Core/Templates/InspectionResultSpec.cs`
- `src/EquipmentTwin.Core/Templates/InspectionResult.cs`
- `src/EquipmentTwin.Core/Templates/ProductRecipe.cs`
- `src/EquipmentTwin.Core/Templates/TemplateRunner.cs`
- `src/EquipmentTwin.Core/Templates/TemplateRunResult.cs`
- `templates/vision-inspection-cell.json`

역할:

- 실제 카메라가 없어도 제품 검사 PASS/FAIL을 데이터로 표현한다.
- `DatasetCamera`, `UnityCamera` 같은 inspection mode를 가진 recipe는 `inspectionResult`를 반드시 정의해야 한다.
- `inspectionScenarios`를 추가하면 같은 recipe에서도 검사 케이스를 이름으로 선택할 수 있다.
- 검사 결과에는 outcome, defect code, message, measurement를 담는다.
- Template Runner는 모션 실행이 정상 완료된 뒤에만 검사 결과를 만든다.
- 모션 fault가 발생하면 검사 결과를 만들지 않는다.

아키텍처 흐름:

```text
ProductRecipe JSON
    ↓
InspectionResultSpec
    ↓
InspectionScenario(optional)
    ↓
ProductRecipe.Validate()
    ↓
TemplateRunner.RunRecipe(..., inspectionScenarioName)
    ↓
TemplateRunResult.InspectionResult
```

중요한 설계 판단:

```text
TemplateRunResult.Success
    = 장비 실행이 알람 없이 완료됐는가

TemplateRunResult.ProductPassed
    = 제품 검사 결과가 PASS인가
```

둘은 같은 값이 아니다.

예를 들어 `tall-part`는 장비가 정상으로 움직여도 제품 검사 결과는 `HEIGHT_OVER_LIMIT`로 Fail이 될 수 있다.

유지보수 포인트:

- 검사 outcome 종류 추가: `InspectionOutcome`
- recipe 검사 결과 필드/검증 변경: `InspectionResultSpec`
- recipe별 검사 케이스 추가/검증 변경: `InspectionScenario`, `ProductRecipe.InspectionScenarios`
- 실행 결과에 검사 항목 추가: `InspectionResult`
- 검사 생성 시점 변경: `TemplateRunner.CreateInspectionResult()`
- 샘플 PASS/FAIL 데이터 변경: `templates/vision-inspection-cell.json`

주의:

- 현재는 실제 이미지 처리 결과가 아니다.
- 데이터 기반 가상 검사 결과다.
- 나중에 실제 카메라, 데이터셋 카메라, Unity 가상 카메라가 붙어도 같은 `InspectionResult` 구조로 결과를 넘기게 만드는 것이 목표다.

## 14. Template Runner CLI

파일:

- `src/EquipmentTwin.Cli/Program.cs`
- `src/EquipmentTwin.Cli/Properties/launchSettings.json`
- `.github/workflows/ci.yml`

역할:

- `TemplateRunner`를 명령어로 직접 실행한다.
- 사용자가 template JSON과 recipe 이름을 선택할 수 있다.
- `template batch`로 template 안의 모든 recipe를 한 번에 실행할 수 있다.
- 선택적으로 fault scenario를 주입할 수 있다.
- 선택적으로 inspection scenario를 선택할 수 있다.
- 실행 결과를 콘솔에 사람이 읽을 수 있는 형태로 출력한다.
- `--report` 옵션으로 실행 결과를 Markdown 파일에 저장한다.
- GitHub Actions에서 대표 template 실행을 자동 검증한다.

사용 예:

```powershell
dotnet run --project src\EquipmentTwin.Cli -- template run templates\vision-inspection-cell.json default-panel --report artifacts\template-run-report.md
dotnet run --project src\EquipmentTwin.Cli -- template run templates\vision-inspection-cell.json tall-part
dotnet run --project src\EquipmentTwin.Cli -- template run templates\vision-inspection-cell.json default-panel --fault x-axis-move-timeout
dotnet run --project src\EquipmentTwin.Cli -- template run templates\vision-inspection-cell.json default-panel --inspection scratch-detected --report artifacts\template-inspection-scenario-report.md
dotnet run --project src\EquipmentTwin.Cli -- template batch templates\vision-inspection-cell.json --report artifacts\template-batch-report.md
```

출력 구조:

```text
Template
Recipe
Execution
Product
Fault
Inspection scenario
Inspection
Motion axes
Command log
```

Markdown report 구조:

```text
Summary
Inspection
Motion Axes
Command Log
```

중요한 설계 판단:

- `Execution`은 장비 실행 성공/실패다.
- `Product`는 제품 검사 PASS/FAIL이다.
- fault가 있으면 `Execution: FAIL`, `Product: NOT_INSPECTED`가 될 수 있다.
- 제품 검사 Fail은 장비 실행 Fail이 아니다.

유지보수 포인트:

- CLI mode 추가/변경: `CliMode`, `CliOptions.Parse()`
- template 실행 흐름: `RunTemplate()`
- template batch 실행 흐름: `RunTemplateBatch()`
- 콘솔 출력 형식: `PrintTemplateResult()`
- Markdown 저장 형식: `BuildTemplateMarkdownReport()`
- batch Markdown 저장 형식: `BuildTemplateBatchMarkdownReport()`
- Visual Studio 실행 프로필: `launchSettings.json`
- CI template 검증 명령: `.github/workflows/ci.yml`
