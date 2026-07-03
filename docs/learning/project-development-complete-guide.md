# Equipment Twin Lab 전체 개발 과정 해설서

Guide marker: `ETL-COMPLETE-DEVELOPMENT-GUIDE`

이 문서는 프로젝트를 처음 보는 주니어 개발자가 “왜 이런 구조로 만들었는지” 이해하고, 비슷한 기능을 직접 추가할 수 있게 만드는 것을 목표로 한다.

짧은 소개 문서가 아니다. 개발 과정, 설계 이유, 파일 책임, 코드 작성 순서, 테스트 방식까지 같이 설명한다.

## 이 문서를 읽는 법

이 프로젝트는 한 번에 WPF 화면부터 만든 프로젝트가 아니다.

개발 순서는 대략 이렇다.

```text
장비 동작의 진실(Core)
  -> 상태머신
  -> 가상 IO
  -> 시간/Timeout
  -> Cell Controller
  -> JSON 시나리오
  -> CLI 실행기
  -> Motion 축
  -> Template/Recipe
  -> ALD 공정 모델
  -> Unity 시각화 시도
  -> WPF 메인 HMI 전환
  -> 문서화/포트폴리오화
```

중요한 관점은 하나다.

```text
UI는 바뀔 수 있지만, 장비 동작의 진실은 Core에 남아야 한다.
```

그래서 이 프로젝트의 중심은 `src/EquipmentTwin.Core`다.

WPF나 Unity는 Core를 보여주는 화면이다. 화면이 마음에 안 들면 갈아엎을 수 있다. 하지만 Core가 안정적이면 프로젝트는 무너지지 않는다.

## 0. 이 프로젝트는 무엇을 보여주려는가

처음 문제는 현실적이었다.

사용자는 장비 SW 엔지니어 경험이 있지만, 집에는 실제 장비가 없다.

장비가 없으면 다음을 실제로 테스트하기 어렵다.

- 센서 입력
- 액추에이터 출력
- 상태 전이
- 알람 발생
- 알람 복구
- Motion 축 동작
- 공정 시퀀스
- HMI 조작

그래서 목표를 이렇게 잡았다.

```text
실제 장비 대신, 장비를 소프트웨어로 모델링한다.
그 모델을 테스트하고, CLI와 HMI로 보여준다.
```

이 프로젝트의 포트폴리오 메시지는 이런 식이다.

```text
나는 화면만 만든 것이 아니라,
장비 SW의 핵심인 상태, IO, 알람, 공정 시퀀스, 테스트 자동화를
Core부터 설계해서 HMI까지 연결했다.
```

## 1. 처음부터 UI를 만들지 않은 이유

초보자가 가장 흔하게 하는 실수는 화면부터 만드는 것이다.

```text
버튼 만들기
색 넣기
화면 배치하기
애니메이션 넣기
```

이것도 필요하다. 하지만 장비 SW에서는 화면보다 먼저 정해야 하는 것이 있다.

```text
장비가 지금 어떤 상태인가?
어떤 이벤트를 받으면 다음 상태로 가는가?
어떤 이벤트는 거부해야 하는가?
알람은 어떤 상태에서 발생하는가?
```

이것이 정해지지 않으면 UI 버튼은 껍데기가 된다.

예를 들어 START 버튼을 눌렀을 때 장비가 어디로 가야 하는지 Core가 모르면, WPF에서 아무리 예쁜 버튼을 만들어도 실무적인 의미가 약하다.

그래서 첫 작업은 상태머신이었다.

## 2. Goal 001: 상태머신

관련 파일:

```text
src/EquipmentTwin.Core/EquipmentState.cs
src/EquipmentTwin.Core/EquipmentEvent.cs
src/EquipmentTwin.Core/EquipmentStateMachine.cs
src/EquipmentTwin.Core/TransitionResult.cs
tests/EquipmentTwin.Core.Tests/Program.cs
```

### 2.1 상태와 이벤트를 분리한 이유

장비 SW에서 상태와 이벤트는 다르다.

상태는 “현재 장비가 어디에 있는가”다.

```csharp
public enum EquipmentState
{
    Idle,
    Loading,
    Aligning,
    Inspecting,
    Unloading,
    Complete,
    Alarmed
}
```

이벤트는 “상태를 바꾸려고 들어온 신호”다.

```csharp
public enum EquipmentEvent
{
    StartLoad,
    LoadComplete,
    AlignmentComplete,
    InspectionComplete,
    UnloadComplete,
    Reset,
    DoorOpened,
    EmergencyStop,
    Timeout,
    ClearAlarm
}
```

주니어가 기억해야 할 점:

```text
상태 = 현재 위치
이벤트 = 상태를 바꾸는 입력
```

예를 들어:

```text
현재 상태: Loading
들어온 이벤트: LoadComplete
다음 상태: Aligning
```

### 2.2 허용된 전이만 딕셔너리로 둔 이유

`EquipmentStateMachine` 안에는 허용된 전이만 등록되어 있다.

```csharp
private static readonly IReadOnlyDictionary<(EquipmentState State, EquipmentEvent Event), EquipmentState> AllowedTransitions =
    new Dictionary<(EquipmentState State, EquipmentEvent Event), EquipmentState>
    {
        [(EquipmentState.Idle, EquipmentEvent.StartLoad)] = EquipmentState.Loading,
        [(EquipmentState.Loading, EquipmentEvent.LoadComplete)] = EquipmentState.Aligning,
        [(EquipmentState.Aligning, EquipmentEvent.AlignmentComplete)] = EquipmentState.Inspecting,
        [(EquipmentState.Inspecting, EquipmentEvent.InspectionComplete)] = EquipmentState.Unloading,
        [(EquipmentState.Unloading, EquipmentEvent.UnloadComplete)] = EquipmentState.Complete,
        [(EquipmentState.Complete, EquipmentEvent.Reset)] = EquipmentState.Idle,
        [(EquipmentState.Alarmed, EquipmentEvent.ClearAlarm)] = EquipmentState.Idle
    };
```

이 구조의 장점:

- 어떤 전이가 가능한지 한눈에 보인다.
- 잘못된 이벤트를 받았을 때 거부할 수 있다.
- 테스트하기 쉽다.
- UI가 어떤 버튼을 활성화할지 판단하기 쉽다.

반대로 모든 것을 `if`로 쓰면 시간이 지나면서 이런 코드가 된다.

```csharp
if (state == Idle && event == StartLoad) ...
else if (state == Loading && event == LoadComplete) ...
else if ...
```

처음에는 괜찮지만 기능이 늘면 읽기 힘들다.

### 2.3 `Apply()`는 왜 결과 객체를 반환하는가

상태머신은 이벤트를 적용할 때 `TransitionResult`를 반환한다.

```csharp
public sealed record TransitionResult(
    EquipmentState PreviousState,
    EquipmentEvent Event,
    EquipmentState CurrentState,
    bool Accepted,
    string Message,
    AlarmInfo? Alarm = null);
```

이 객체는 단순히 성공/실패만 말하지 않는다.

다음을 모두 알려준다.

- 이전 상태
- 들어온 이벤트
- 현재 상태
- 받아들였는지 여부
- 설명 메시지
- 알람 정보

이렇게 만든 이유는 나중에 다음 곳에서 같은 결과를 써야 하기 때문이다.

- 테스트
- CLI 출력
- WPF 화면
- 작업 로그
- 디버그 테이블

즉 Core는 UI를 모른다. 대신 UI가 사용할 수 있는 결과를 잘 만들어준다.

### 2.4 상태머신 테스트는 무엇을 검증했나

처음 테스트는 이런 식이었다.

```text
정상 순서로 이벤트를 넣으면 Complete까지 간다.
잘못된 이벤트를 넣으면 거부된다.
Inspection 중 DoorOpened가 들어오면 Alarmed로 간다.
ClearAlarm을 하면 Idle로 돌아간다.
성공/실패 전이가 History에 남는다.
```

여기서 중요한 사고방식:

```text
장비 SW는 “된 것 같다”가 아니라 “전이가 증명됐다”가 되어야 한다.
```

그래서 테스트를 먼저 쌓았다.

## 3. Goal 002: 가상 IO 모델

관련 파일:

```text
src/EquipmentTwin.Core/Io/EquipmentIoMap.cs
src/EquipmentTwin.Core/Io/VirtualIoController.cs
src/EquipmentTwin.Core/Io/IoSignalDirection.cs
src/EquipmentTwin.Core/Io/IoSignalDefinition.cs
src/EquipmentTwin.Core/Io/IoChange.cs
src/EquipmentTwin.Core/Io/IoSignalSnapshot.cs
```

### 3.1 왜 IO가 필요한가

상태머신은 이벤트만 받는다.

하지만 실제 장비에서는 이벤트가 보통 IO에서 온다.

예를 들어:

```text
DoorClosed 센서가 false가 됨
  -> DoorOpened 이벤트

LoadPresent 센서가 true가 됨
  -> LoadComplete 이벤트

EmergencyStopPressed가 true가 됨
  -> EmergencyStop 이벤트
```

그래서 가상 IO가 필요했다.

### 3.2 Input과 Output을 나눈 이유

`VirtualIoController`는 입력과 출력을 구분한다.

```text
Input  = 센서가 장비 SW에게 알려주는 값
Output = 장비 SW가 장치에게 내보내는 명령
```

예:

```text
Input:
- DoorClosed
- EmergencyStopPressed
- LoadPresent
- AlignmentDone
- InspectionDone
- UnloadComplete

Output:
- VacuumOn
- StageMoveRequested
- TowerLampRed
- BuzzerOn
```

코드에서는 이런 규칙을 강제한다.

```csharp
public void WriteOutput(string name, bool value, string source = "EquipmentSoftware")
{
    var definition = EnsureDirection(name, IoSignalDirection.Output, "Only output signals can be written by equipment software.");
    SetValue(definition, value, source);
}

public void SetInput(string name, bool value, string source = "Simulator")
{
    var definition = EnsureDirection(name, IoSignalDirection.Input, "Only input signals can be changed by the simulator.");
    SetValue(definition, value, source);
}
```

이 설계가 중요한 이유:

```text
장비 SW는 센서 값을 마음대로 쓰면 안 된다.
시뮬레이터는 출력 명령을 마음대로 쓰면 안 된다.
```

실무에서는 이 구분이 매우 중요하다.

### 3.3 IO History를 남긴 이유

IO 값이 바뀔 때마다 `IoChange`가 기록된다.

이것은 디버깅에 필요하다.

예를 들어 장비가 갑자기 Alarmed가 됐다고 하자.

그때 알고 싶은 것은 이거다.

```text
언제 DoorClosed가 false가 됐나?
누가 바꿨나?
이전 값은 무엇이었나?
```

그래서 History를 둔다.

장비 SW에서는 로그가 단순 장식이 아니라 원인 분석 도구다.

## 4. Goal 003: GitHub Actions CI

관련 파일:

```text
.github/workflows/ci.yml
```

### 4.1 CI가 무엇인가

CI는 Continuous Integration의 약자다.

쉽게 말하면:

```text
GitHub에 코드를 올릴 때마다 자동으로 빌드와 테스트를 돌리는 장치
```

이 프로젝트에서 CI는 이런 역할을 한다.

- 빌드가 깨졌는지 확인
- Core 테스트가 통과하는지 확인
- 중요한 문서/파일이 빠지지 않았는지 확인
- WPF/Unity 관련 산출물이 실수로 사라지지 않았는지 확인

### 4.2 왜 초반에 CI를 넣었나

처음에는 귀찮아 보인다.

하지만 프로젝트가 커지면 이런 일이 생긴다.

```text
어제는 됐는데 오늘은 안 됨
내 PC에서는 되는데 GitHub에서는 안 됨
문서를 추가했다고 생각했는데 커밋에서 빠짐
테스트 파일이 깨졌는데 모르고 병합함
```

CI는 이런 실수를 줄인다.

이 프로젝트는 매일 조금씩 커지는 프로젝트라, 자동 검증이 없으면 유지보수가 어려워진다.

## 5. Goal 004: Clock과 Timeout

관련 파일:

```text
src/EquipmentTwin.Core/IClock.cs
src/EquipmentTwin.Core/SystemClock.cs
src/EquipmentTwin.Core/ManualClock.cs
src/EquipmentTwin.Core/StateTimeoutPolicy.cs
src/EquipmentTwin.Core/TimeoutCheckResult.cs
```

### 5.1 왜 실제 시간을 바로 쓰지 않았나

초보자는 보통 이렇게 생각한다.

```csharp
DateTimeOffset.UtcNow
```

하지만 테스트에서는 문제가 생긴다.

예를 들어 Loading 상태가 30초 넘으면 Timeout이 나야 한다고 하자.

실제 시간으로 테스트하면 테스트가 30초를 기다려야 한다.

이건 좋지 않다.

그래서 시간을 추상화했다.

```text
IClock = 지금 시간이 몇 시인지 알려주는 인터페이스
SystemClock = 실제 시간을 쓰는 구현
ManualClock = 테스트에서 직접 시간을 앞으로 미는 구현
```

### 5.2 ManualClock의 의미

테스트에서는 이런 식으로 시간을 움직인다.

```text
현재 시간: 00:00:00
Loading 시작
ManualClock을 31초 앞으로 이동
Timeout 검사
Alarmed 상태인지 확인
```

이렇게 하면 실제로 31초를 기다리지 않아도 된다.

장비 SW 테스트에서는 “시간을 제어할 수 있게 만드는 것”이 매우 중요하다.

### 5.3 TimeoutPolicy를 분리한 이유

`StateTimeoutPolicy`는 상태별 제한 시간을 가진다.

```csharp
policy.SetTimeout(EquipmentState.Loading, TimeSpan.FromSeconds(30));
policy.SetTimeout(EquipmentState.Aligning, TimeSpan.FromSeconds(15));
policy.SetTimeout(EquipmentState.Inspecting, TimeSpan.FromSeconds(60));
policy.SetTimeout(EquipmentState.Unloading, TimeSpan.FromSeconds(30));
```

이걸 상태머신 안에 직접 박아두지 않은 이유:

- 장비마다 제한 시간이 다를 수 있다.
- 테스트마다 timeout을 다르게 줄 수 있다.
- 나중에 recipe나 설정 파일로 뺄 수 있다.

즉 값은 정책으로 분리하고, 상태머신은 정책을 받아 검사한다.

## 6. Goal 005: Cell Controller

관련 파일:

```text
src/EquipmentTwin.Core/EquipmentCellController.cs
src/EquipmentTwin.Core/EquipmentCellStepResult.cs
src/EquipmentTwin.Core/Io/EquipmentIoMap.cs
```

### 6.1 왜 Controller가 필요했나

상태머신은 이벤트를 받는다.

IO는 센서/출력 값을 가진다.

하지만 둘을 연결하는 코드가 필요하다.

```text
IO 입력을 읽는다
  -> 어떤 이벤트인지 판단한다
  -> 상태머신에 이벤트를 적용한다
  -> 현재 상태에 맞게 Output을 갱신한다
```

이 역할을 `EquipmentCellController`가 한다.

### 6.2 안전 이벤트를 먼저 처리한 이유

`PollInputs()` 안에서는 안전 이벤트를 먼저 본다.

```csharp
var safetyEvent = ReadSafetyEvent();
if (safetyEvent is not null)
{
    var transition = ApplyAndSync(safetyEvent.Value);
    return EquipmentCellStepResult.FromTransition(safetyEvent.Value, transition);
}
```

장비 SW에서는 안전이 일반 공정보다 우선이다.

예:

```text
LoadPresent가 true여도
DoorClosed가 false면
LoadComplete보다 DoorOpened 알람이 우선이다.
```

이 사고방식은 실무적이다.

### 6.3 상태에 따라 Output을 동기화한 이유

상태가 바뀌면 출력도 바뀌어야 한다.

예:

```text
Idle/Complete:
- VacuumOff
- StageMoveRequestedOff

Loading/Inspecting/Unloading:
- VacuumOn
- StageMoveRequestedOff

Aligning:
- VacuumOn
- StageMoveRequestedOn

Alarmed:
- VacuumOff
- StageMoveRequestedOff
- TowerLampRedOn
- BuzzerOn
```

이것을 `SyncOutputsForCurrentState()`에 모아뒀다.

이렇게 하면 상태와 출력의 관계를 한 곳에서 볼 수 있다.

## 7. Goal 006~008: JSON 시나리오와 CLI

관련 파일:

```text
src/EquipmentTwin.Core/Scenarios/EquipmentScenario.cs
src/EquipmentTwin.Core/Scenarios/ScenarioStep.cs
src/EquipmentTwin.Core/Scenarios/ScenarioStepAction.cs
src/EquipmentTwin.Core/Scenarios/ScenarioRunner.cs
src/EquipmentTwin.Cli/Program.cs
scenarios/*.json
```

### 7.1 왜 JSON 시나리오가 필요했나

테스트 코드를 C#으로만 쓰면 개발자는 이해할 수 있지만, 비개발자는 읽기 어렵다.

그래서 장비 흐름을 JSON으로 표현했다.

예상 흐름:

```text
StartCycle
SetInput LoadPresent true
PollInputs
ExpectState Aligning
```

이렇게 하면 “장비가 어떤 순서로 움직이는지”가 데이터로 보인다.

### 7.2 ScenarioRunner의 책임

`ScenarioRunner`는 JSON 시나리오를 받아서 실행한다.

하는 일:

- step을 순서대로 읽는다.
- action 종류에 따라 Core를 호출한다.
- 기대값을 확인한다.
- 실패하면 그 지점에서 멈춘다.
- 결과를 `ScenarioRunResult`로 반환한다.

핵심 구조:

```csharp
for (var index = 0; index < scenario.Steps.Count; index++)
{
    var step = scenario.Steps[index];
    var result = RunStep(index, step);
    results.Add(result);

    if (!result.Success)
    {
        break;
    }
}
```

이 구조는 제조 시퀀스와 잘 맞는다.

```text
앞 단계가 실패하면 뒤 단계는 실행하면 안 된다.
```

### 7.3 CLI를 만든 이유

CLI는 Command Line Interface다.

쉽게 말하면 PowerShell에서 실행하는 프로그램이다.

이 프로젝트에서 CLI는 이런 역할을 한다.

```text
dotnet run --project src/EquipmentTwin.Cli ...
```

CLI를 만든 이유:

- Visual Studio 없이 검증 가능
- GitHub Actions에서 실행 가능
- 리포트 자동 생성 가능
- 나중에 WPF/Unity와 별개로 Core 검증 가능

UI가 없어도 CLI가 있으면 Core가 살아있는지 확인할 수 있다.

## 8. Goal 009~013: 알람 코드와 복구 조건

관련 파일:

```text
src/EquipmentTwin.Core/Alarms/AlarmCode.cs
src/EquipmentTwin.Core/Alarms/AlarmInfo.cs
src/EquipmentTwin.Core/Alarms/AlarmRecoveryCheck.cs
src/EquipmentTwin.Core/EquipmentCellController.cs
```

### 8.1 단순 문자열 알람의 한계

초반에는 알람을 문자열로만 표현할 수도 있다.

```text
"Door opened"
"Emergency stop"
"Timeout"
```

하지만 실무적으로는 부족하다.

왜냐하면 복구 조건이 다르기 때문이다.

```text
DoorOpened:
- 문이 다시 닫혀야 ClearAlarm 가능

EmergencyStop:
- 비상정지 버튼이 해제되어야 ClearAlarm 가능

Timeout:
- 작업자 확인 후 ClearAlarm 가능
```

그래서 알람 코드를 만들었다.

### 8.2 복구 조건을 둔 이유

`ClearAlarm`은 아무 때나 되면 안 된다.

예:

```text
문이 아직 열려 있는데 ClearAlarm이 되면 안 된다.
비상정지가 아직 눌려 있는데 ClearAlarm이 되면 안 된다.
```

그래서 `CheckAlarmRecoveryCondition()`이 있다.

이 함수는 알람을 해제해도 되는지 판단한다.

주니어가 봐야 할 핵심:

```text
알람 해제 버튼은 단순 Reset 버튼이 아니다.
원인이 제거됐는지 확인해야 한다.
```

## 9. Goal 014~015: Motion Axis 모델

관련 파일:

```text
src/EquipmentTwin.Core/Motion/MotionAxis.cs
src/EquipmentTwin.Core/Motion/MotionAxisState.cs
src/EquipmentTwin.Core/Motion/MotionAxisAlarm.cs
src/EquipmentTwin.Core/Motion/MotionAxisAlarmCode.cs
src/EquipmentTwin.Core/Motion/MotionCommandResult.cs
```

### 9.1 왜 Motion을 따로 만들었나

제조 장비에는 축이 있다.

예:

- X축
- Z축
- Stage
- Robot arm
- Lift
- Chuck

실제 축은 서보 드라이브와 통신하지만, 이 프로젝트에서는 가상 축으로 모델링했다.

### 9.2 MotionAxis의 흐름

축의 기본 흐름은 이렇다.

```text
Disabled
  -> ServoOn
  -> Ready
  -> StartHome
  -> Homing
  -> Poll
  -> InPosition
  -> StartMove
  -> Moving
  -> Poll
  -> InPosition
```

중요한 규칙:

- Servo가 켜져야 Home 가능
- Home이 되어야 Move 가능
- Move 중에는 다른 Move를 시작하면 안 됨
- 시간이 너무 오래 걸리면 Timeout 알람
- Servo alarm이 발생할 수 있음

### 9.3 Motion도 Clock을 사용한 이유

Motion에도 시간이 있다.

```text
Home duration = 500 ms
Move duration = 1000 ms
```

실제 시간을 기다리면 테스트가 느려진다.

그래서 `ManualClock`을 같이 사용한다.

```text
StartMove
ManualClock.Advance(1000 ms)
Poll
InPosition 확인
```

이 방식은 장비 시뮬레이션에서 중요하다.

## 10. Goal 016~024: Template, Recipe, Inspection, Report

관련 파일:

```text
src/EquipmentTwin.Core/Templates/EquipmentTemplate.cs
src/EquipmentTwin.Core/Templates/ProductRecipe.cs
src/EquipmentTwin.Core/Templates/TemplateRunner.cs
src/EquipmentTwin.Core/Templates/TemplateRunResult.cs
src/EquipmentTwin.Core/Templates/InspectionResult.cs
src/EquipmentTwin.Core/Templates/FaultScenario.cs
```

### 10.1 왜 Template이 생겼나

초반 Core는 고정된 장비 흐름이었다.

하지만 사용자는 “유저가 원하는 대로 장비를 커스텀하는 기능”을 이야기했다.

그래서 방향이 확장됐다.

```text
장비 구조 = Template
제품별 조건 = ProductRecipe
불량/고장 조건 = FaultScenario
검사 결과 = InspectionScenario
```

이렇게 나누면 장비를 코드로만 바꾸지 않고 데이터로 바꿀 수 있다.

### 10.2 ProductRecipe의 의미

Recipe는 제품을 어떻게 처리할지 정의한다.

예:

```text
panel-normal:
- xAxis -> 100
- zAxis -> 20
- inspection = pass

tall-part:
- xAxis -> 100
- zAxis -> 80
- inspection = fail
```

제조 장비에서 Recipe는 매우 중요한 개념이다.

장비는 같아도 제품마다 조건이 다를 수 있다.

### 10.3 TemplateRunner는 무엇을 하나

`TemplateRunner`는 Template과 Recipe를 실제 Motion 실행으로 바꾼다.

흐름:

```text
Template 검증
Recipe 찾기
FaultScenario 찾기
MotionAxis 생성
ServoOn
Home
Recipe의 AxisTarget대로 Move
Fault 주입 여부 확인
InspectionResult 생성
TemplateRunResult 반환
```

코드 흐름은 이런 모양이다.

```csharp
foreach (var axisTemplate in template.MotionAxes)
{
    var axis = axes[axisTemplate.Name];
    Add(commandLog, "ServoOn", axis, axis.ServoOn());

    if (axisTemplate.RequiresHome)
    {
        Add(commandLog, "StartHome", axis, axis.StartHome(_options.HomeDuration));
        _clock.Advance(_options.HomeDuration);
        Add(commandLog, "PollHome", axis, axis.Poll());
    }
}
```

주니어가 이해해야 할 점:

```text
TemplateRunner는 화면이 아니다.
데이터로 정의된 장비/제품을 실행 가능한 Motion 흐름으로 바꾸는 실행기다.
```

### 10.4 InspectionResult는 아직 실제 비전이 아니다

중요한 정직성 포인트다.

현재 `InspectionResult`는 실제 이미지를 보고 판단하지 않는다.

Recipe/Scenario에 정의된 결과를 모델링한다.

즉 현재 상태:

```text
검사 결과를 표현하는 데이터 모델은 있다.
실제 비전 알고리즘은 아직 없다.
```

면접에서 말할 때는 이렇게 말해야 한다.

```text
현재는 검사 결과 모델과 자동 리포트 흐름까지 구현했습니다.
실제 이미지 기반 비전 검사는 다음 확장 후보입니다.
```

## 11. Goal 025~026: 공개 기준 ALD 공정 모델

관련 파일:

```text
src/EquipmentTwin.Core/Processes/MolyAldRecipe.cs
src/EquipmentTwin.Core/Processes/MolyAldRunner.cs
src/EquipmentTwin.Core/Processes/MolyAldRunResult.cs
src/EquipmentTwin.Core/Processes/MolyAldStepLog.cs
src/EquipmentTwin.Core/Processes/MolyAldTimelineDocument.cs
```

### 11.1 왜 ALD로 방향을 잡았나

사용자는 Lam Research 쪽 장비 경험이 있고, 증착 장비 테스트를 하고 있다.

하지만 실제 회사 장비 코드나 내부 자료는 쓰면 안 된다.

그래서 방향은 이렇게 잡았다.

```text
공개적으로 설명 가능한 ALD 개념을 기반으로,
vendor CAD나 내부 공정 조건이 아닌 synthetic demo process를 만든다.
```

문서와 화면에도 “public-reference”, “synthetic” 표현을 넣었다.

이것은 정직성과 보안 때문에 중요하다.

### 11.2 MolyAldRunner의 공정 흐름

`MolyAldRunner`는 합성 ALD 공정을 순서대로 실행한다.

흐름:

```text
LoadWafer
PumpDown
StabilizeTemperature

Cycle 반복:
  DoseMetalPrecursor
  PurgeAfterPrecursor
  DoseReactant
  PurgeAfterReactant

PostPurge
TransferOut
Complete
```

여기서 ALD의 핵심 느낌은 다음이다.

```text
전구체 주입
퍼지
반응물 주입
퍼지
얇은 막 성장
반복
```

### 11.3 StepLog를 남긴 이유

각 공정 step은 `MolyAldStepLog`로 남는다.

여기에는 다음이 들어간다.

- step index
- 공정 step 이름
- cycle 번호
- 시작/종료 시간
- 성공 여부
- 메시지
- chamber pressure
- wafer temperature
- valve 상태
- estimated thickness

이 데이터를 나중에 Unity/WPF에서 그대로 사용한다.

중요한 구조:

```text
Core가 timeline 데이터를 만든다.
UI는 timeline을 보여준다.
```

그래서 WPF 화면은 Core 공정 결과를 읽는 구조가 된다.

## 12. Goal 027~049: Unity 시각화 시도

관련 폴더:

```text
unity/EquipmentTwin.Unity
docs/unity-*.md
goals/027~049
```

### 12.1 왜 Unity를 시도했나

처음에는 차별화를 위해 3D 장비 시뮬레이터를 만들고 싶었다.

의도는 좋았다.

```text
왼쪽에는 장비 3D 모델
오른쪽에는 조작 HMI
Core 공정 timeline에 맞춰 3D가 움직임
```

포트폴리오 관점에서 눈에 띄는 방향이었다.

### 12.2 왜 Unity가 메인이 되지 않았나

문제는 학습/유지보수 비용이었다.

Unity는 다음 지식이 필요하다.

- Scene
- GameObject
- Component
- Canvas
- Prefab
- Material
- Camera
- Build setting
- Play mode
- Unity Editor 사용법

사용자는 Unity 자체를 깊게 공부하려는 목표가 아니었다.

또 실제 장비처럼 보이려면 3D 모델링 품질도 필요했다.

primitive 도형으로 만든 장비는 금방 한계가 보였다.

```text
장비처럼 보이는 것이 아니라,
장비를 가리키는 도형 배치처럼 보였다.
```

그래서 결론은:

```text
Unity는 optional viewer로 둔다.
메인 조작/디버깅 화면은 WPF로 간다.
```

이 결정은 포기가 아니라 범위 조정이다.

현재 목표는 “Unity 전문가가 되는 것”이 아니라 “장비 SW 포트폴리오를 만드는 것”이다.

## 13. Goal 050~054: WPF 메인 HMI 전환

관련 파일:

```text
src/EquipmentTwin.Hmi.Wpf/App.xaml
src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml
src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml.cs
src/EquipmentTwin.Hmi.Wpf/ViewModels/OperatorConsoleViewModel.cs
src/EquipmentTwin.Hmi.Wpf/ViewModels/ObservableObject.cs
src/EquipmentTwin.Hmi.Wpf/ViewModels/RelayCommand.cs
src/EquipmentTwin.Hmi.Wpf/Models/StepRowViewModel.cs
src/EquipmentTwin.Hmi.Wpf/Models/OperatorLogEntry.cs
src/EquipmentTwin.Hmi.Wpf/Services/MolyAldRecipeService.cs
```

### 13.1 왜 WPF가 현재 메인이 되었나

WPF는 Windows desktop app을 만드는 기술이다.

이 프로젝트에 맞는 이유:

- Visual Studio에서 디버깅하기 쉽다.
- C#/.NET Core와 직접 연결된다.
- 장비 HMI 스타일 화면을 만들 수 있다.
- Unity보다 업무용 UI에 가깝다.
- 사용자가 Unity보다 유지보수하기 쉽다.

그래서 현재 메인 방향은:

```text
WPF = 메인 Operator Console
Unity = 필요할 때만 여는 optional viewer
Core = 둘 다 사용하는 진짜 장비 로직
```

### 13.2 WPF의 기본 구조

WPF는 XAML과 C# ViewModel로 나뉜다.

```text
MainWindow.xaml
  = 화면 배치, 색상, 버튼, 표, 텍스트

OperatorConsoleViewModel.cs
  = 화면에 보여줄 데이터와 버튼 동작
```

간단히 말하면:

```text
XAML = 어떻게 보일지
ViewModel = 무엇을 보여줄지 / 버튼 누르면 뭘 할지
```

### 13.3 Binding이 무엇인가

XAML에는 이런 코드가 많다.

```xml
<TextBlock Text="{Binding CurrentStepName}"/>
```

이 뜻은:

```text
화면의 TextBlock.Text 값을
ViewModel의 CurrentStepName 속성과 연결한다.
```

그래서 `OperatorConsoleViewModel.CurrentStepName` 값이 바뀌면 화면 텍스트도 바뀐다.

### 13.4 Command가 무엇인가

버튼에는 이런 코드가 있다.

```xml
<Button Content="START" Command="{Binding StartCommand}"/>
```

이 뜻은:

```text
START 버튼을 누르면 ViewModel의 StartCommand를 실행한다.
```

ViewModel 생성자에서는 이렇게 연결한다.

```csharp
StartCommand = new RelayCommand(Start);
PauseCommand = new RelayCommand(Pause);
ResetCommand = new RelayCommand(Reset);
FaultReplayCommand = new RelayCommand(FaultReplay);
StepForwardCommand = new RelayCommand(StepForward);
```

즉 버튼은 직접 Core를 모른다.

버튼은 Command를 호출하고, Command가 ViewModel 메서드를 실행한다.

### 13.5 DispatcherTimer를 쓴 이유

WPF HMI는 START를 누르면 timeline을 자동 재생한다.

이때 `DispatcherTimer`를 사용한다.

```csharp
playbackTimer = new DispatcherTimer
{
    Interval = TimeSpan.FromMilliseconds(750)
};
playbackTimer.Tick += (_, _) => StepForwardFromTimer();
```

의미:

```text
750ms마다 다음 공정 step으로 이동한다.
```

장점:

- UI thread에서 안전하게 화면을 갱신한다.
- START/STOP/STEP 버튼과 자연스럽게 연결된다.

### 13.6 WPF 바인딩 오류가 났던 이유

이전에 Visual Studio에서 이런 오류가 있었다.

```text
TwoWay 또는 OneWayToSource 바인딩은 읽기 전용 속성에서 작동할 수 없다.
```

원인은 `ProgressBar.Value`였다.

WPF의 일부 컨트롤은 기본 Binding Mode가 TwoWay일 수 있다.

그런데 ViewModel의 progress 값은 계산 속성이다.

```csharp
public double FilmProgress => ...
```

setter가 없다.

그래서 XAML에서 명시적으로 `Mode=OneWay`를 넣었다.

```xml
<ProgressBar Value="{Binding FilmProgress, Mode=OneWay}" Maximum="100"/>
```

이것이 Goal 051에서 해결한 문제다.

주니어가 기억할 점:

```text
화면이 값을 보여주기만 하면 OneWay.
화면에서 사용자가 값을 바꾸고 ViewModel에 써야 하면 TwoWay.
```

### 13.7 DataGrid가 하얗게 깨졌던 이유

WPF 기본 DataGrid 스타일은 Windows 기본 테마를 따른다.

우리는 어두운 HMI 화면을 만들었지만, DataGrid 내부 Header/Cell/Row가 기본 흰색으로 남아 있었다.

그래서 Goal 052에서 명시적 스타일을 추가했다.

```text
DataGridColumnHeader
DataGridRow
DataGridCell
ComboBox
ComboBoxItem
```

이것은 WPF에서 흔한 문제다.

겉 Border만 어둡게 해도 내부 컨트롤 템플릿은 여전히 기본 스타일일 수 있다.

### 13.8 Goal 053과 Goal 054의 색상 결정

Goal 053에서는 ISA-101 / High Performance HMI를 참고해서 회색 중심, 색 절제 방향을 실험했다.

장점:

- 산업 HMI 원칙에 가깝다.
- 알람 색상이 튄다.
- 정상 상태 색 노이즈가 줄어든다.

하지만 사용자 목표와 완전히 맞지는 않았다.

현재 프로젝트는 면접/포트폴리오 데모이기도 하다.

그래서 Goal 054에서 결정했다.

```text
엄격한 색상 규칙보다 데모 가독성을 우선한다.
정상 상태 초록/파랑 표현은 유지한다.
```

현재 색상 의미:

```text
Green  = 정상, NO ALARM, ready/running, target
Blue   = 공정 진행, active process, progress
Amber  = warning, fault replay, 주의
Red    = stop, alarm
Gray   = 배경, 비활성, 구조
```

## 14. 현재 전체 아키텍처

전체 구조를 단순화하면 이렇다.

```text
tests
  -> Core를 직접 검증

CLI
  -> Core를 실행하고 리포트 생성

WPF
  -> Core의 ALD timeline을 읽어서 HMI로 표시

Unity
  -> Core timeline을 선택적으로 시각화

Core
  -> 상태, IO, Motion, Recipe, ALD 공정의 진실
```

더 구체적으로는:

```text
EquipmentTwin.Core
  EquipmentStateMachine
  VirtualIoController
  EquipmentCellController
  MotionAxis
  TemplateRunner
  MolyAldRunner

EquipmentTwin.Cli
  Program.cs
  Scenario 실행
  Template 실행
  ALD report 생성

EquipmentTwin.Hmi.Wpf
  MainWindow.xaml
  OperatorConsoleViewModel
  MolyAldRecipeService
  StepRowViewModel
```

핵심 의존성 방향:

```text
WPF -> Core
CLI -> Core
Tests -> Core
Core -> WPF 모름
Core -> CLI 모름
Core -> Unity 모름
```

이게 중요하다.

Core가 UI를 모르면 UI를 바꿔도 Core가 안전하다.

## 15. 이 프로젝트에서 반복되는 설계 패턴

### 15.1 Result 객체 패턴

많은 실행 결과가 전용 result 객체로 나온다.

예:

```text
TransitionResult
EquipmentCellStepResult
ScenarioRunResult
MotionCommandResult
TemplateRunResult
MolyAldRunResult
```

이유:

```text
성공/실패만으로는 디버깅할 수 없다.
왜 성공했는지, 왜 실패했는지, 어떤 상태였는지를 같이 남겨야 한다.
```

### 15.2 History/Log 패턴

여러 객체가 history를 가진다.

예:

```text
EquipmentStateMachine.History
VirtualIoController.History
MotionAxis.History
OperatorLog
MolyAldStepLog
```

제조 장비 SW에서 history는 중요하다.

문제가 생기면 “마지막 값”보다 “어떤 순서로 바뀌었는가”가 더 중요하다.

### 15.3 Data-driven 패턴

JSON이나 recipe로 동작을 표현한다.

장점:

- 코드 수정 없이 시나리오 변경 가능
- 테스트 케이스 추가가 쉬움
- 데모를 여러 버전으로 만들 수 있음
- 문서/리포트와 연결하기 쉬움

### 15.4 Deterministic test 패턴

Deterministic은 “항상 같은 입력이면 항상 같은 결과”라는 뜻이다.

이 프로젝트는 테스트가 매번 같은 결과를 내도록 설계했다.

그 핵심은 `ManualClock`이다.

시간을 직접 밀기 때문에 테스트가 흔들리지 않는다.

## 16. 새 기능을 추가할 때의 사고 순서

앞으로 새로운 기능을 추가할 때는 이 순서로 생각하면 된다.

```text
1. 이 기능은 Core 로직인가, UI 표현인가?
2. Core 상태/데이터 모델이 먼저 필요한가?
3. 테스트로 증명할 수 있는가?
4. CLI나 문서로 검증할 수 있는가?
5. 마지막에 WPF에 표시할 것인가?
```

예를 들어 “새 알람을 추가하자”면:

```text
1. AlarmCode에 새 코드 추가
2. AlarmInfo.FromEvent 또는 별도 생성 경로 추가
3. EquipmentCellController 복구 조건 추가
4. 테스트 추가
5. CLI/시나리오 추가
6. WPF AlarmCard에 표시
7. 문서/로그 업데이트
```

예를 들어 “새 공정 step을 추가하자”면:

```text
1. MolyAldProcessStep enum에 step 추가
2. MolyAldRunner에 AddStep 흐름 추가
3. MolyAldStepLog에 필요한 값이 충분한지 확인
4. 테스트에서 step 순서 확인
5. TimelineDocument 변환 확인
6. WPF StepRowViewModel 표시 확인
7. 문서와 데모 설명 업데이트
```

예를 들어 “WPF 화면에 새 계기값을 추가하자”면:

```text
1. Core에 그 값이 있는지 확인
2. ViewModel에 표시용 property 추가
3. 값이 바뀔 때 OnPropertyChanged가 호출되는지 확인
4. XAML에 TextBlock/ProgressBar 추가
5. 필요하면 Style 추가
6. WPF 실행 스모크
7. 스크린샷으로 눈검증
```

## 17. 테스트를 읽는 방법

테스트 파일:

```text
tests/EquipmentTwin.Core.Tests/Program.cs
```

이 프로젝트의 테스트는 xUnit/NUnit이 아니라 간단한 콘솔 테스트 스타일이다.

테스트 실행:

```powershell
dotnet run --project tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj --configuration Release
```

성공하면 이런 식으로 나온다.

```text
PASS Normal sequence reaches Complete
PASS Invalid transition is rejected
...
All EquipmentTwin.Core tests passed.
```

테스트를 읽을 때는 이름을 먼저 본다.

예:

```text
Door alarm cannot clear while door remains open
```

이 이름 자체가 요구사항이다.

뜻:

```text
문이 계속 열린 상태에서는 DoorOpened 알람을 Clear할 수 없어야 한다.
```

좋은 테스트 이름은 문서 역할도 한다.

## 18. Visual Studio에서 디버깅할 때 보는 순서

WPF를 디버깅할 때는 먼저 Startup Project를 확인한다.

실행 가능한 프로젝트:

```text
EquipmentTwin.Hmi.Wpf
EquipmentTwin.Cli
EquipmentTwin.Core.Tests
```

직접 실행할 수 없는 프로젝트:

```text
EquipmentTwin.Core
```

Core는 class library다.

즉 혼자 실행되는 프로그램이 아니라, WPF/CLI/Tests가 참조해서 사용하는 로직 모음이다.

Core를 디버깅하려면:

```text
1. WPF 또는 Tests를 Startup Project로 설정
2. Core 코드에 breakpoint 설정
3. F5 실행
4. WPF/Tests가 Core를 호출할 때 breakpoint에 걸림
```

이게 class library 디버깅 방식이다.

## 19. WPF에서 화면이 이상할 때 보는 순서

화면 문제가 생기면 아래 순서로 본다.

```text
1. ViewModel property 값이 맞나?
2. OnPropertyChanged가 호출되나?
3. XAML Binding 경로가 맞나?
4. Binding Mode가 맞나?
5. Style/Resource 색상이 충돌하지 않나?
6. Grid Row/Column이 원하는 위치인가?
7. 컨트롤 기본 스타일이 새지 않았나?
```

예:

```text
글씨가 안 보임
```

가능 원인:

- Foreground가 배경색과 비슷함
- 기본 WPF 컨트롤 스타일이 흰색 배경을 사용함
- TextBlock이 작은 영역 안에서 잘림
- Grid 위치가 잘못됨
- FontSize가 너무 작음

Goal 052에서 실제로 이런 문제가 있었다.

## 20. 현재 프로젝트의 정직한 한계

이 프로젝트는 강점도 있지만 아직 한계도 명확하다.

### 20.1 실제 장비와 연결되어 있지 않다

현재는 실제 PLC, sensor, motor driver와 통신하지 않는다.

대신 가상 IO와 가상 Motion으로 장비 사고방식을 모델링한다.

면접에서는 이렇게 말해야 한다.

```text
실제 장비 통신은 포함하지 않았고,
장비 SW 구조를 시뮬레이션 가능한 Core로 모델링했습니다.
```

### 20.2 실제 비전 검사는 아니다

현재 inspection은 데이터 기반 결과 모델이다.

이미지를 보고 판정하는 알고리즘은 아직 없다.

### 20.3 ALD는 public/synthetic 모델이다

회사 장비 내부 동작이나 vendor CAD를 복제하지 않는다.

공개적으로 설명 가능한 ALD 개념을 바탕으로 synthetic process를 만든다.

이 표현이 중요하다.

## 21. 이 프로젝트를 면접에서 설명하는 방법

짧은 버전:

```text
실제 장비 없이도 장비 SW의 핵심 흐름을 검증할 수 있도록,
상태머신, 가상 IO, Timeout, Motion, Recipe, 공정 Timeline을 Core로 모델링하고
CLI/WPF HMI에서 실행·시각화하는 포트폴리오 프로젝트입니다.
```

조금 긴 버전:

```text
처음에는 장비 상태머신부터 만들었습니다.
그 다음 가상 IO를 붙여 센서/출력 개념을 분리했고,
ManualClock으로 Timeout과 Motion을 deterministic하게 테스트했습니다.
이후 JSON Scenario와 CLI를 붙여 자동 검증 루프를 만들었고,
Template/Recipe 모델로 장비 커스터마이징 가능성을 추가했습니다.
최근에는 공개 가능한 ALD 개념을 기반으로 synthetic process runner를 만들고,
그 timeline을 WPF HMI에서 조작/디버깅할 수 있게 연결했습니다.
```

강조할 포인트:

- 화면보다 Core를 먼저 만들었다.
- 테스트 가능한 구조로 만들었다.
- UI와 Core를 분리했다.
- 실제 장비가 없어도 장비 SW 사고방식을 보여준다.
- 공개 가능한 정보만 사용했다.
- 현재 한계도 알고 있다.

## 22. 앞으로 이 문서에 추가해야 할 것

이 문서는 전체 개발 과정을 설명한다.

하지만 실제 코드를 완전히 내 것으로 만들려면 다음 문서가 추가로 필요하다.

```text
1. EquipmentStateMachine.cs 줄 단위 해설
2. VirtualIoController.cs 줄 단위 해설
3. MolyAldRunner.cs 줄 단위 해설
4. OperatorConsoleViewModel.cs 줄 단위 해설
5. MainWindow.xaml 줄 단위 해설
6. tests/Program.cs 테스트 작성법 해설
```

다음 작업으로 가장 좋은 것은 `OperatorConsoleViewModel` 또는 `MainWindow.xaml`이다.

이유:

```text
지금 사용자가 직접 보고 만지는 화면이 WPF HMI이기 때문이다.
```

하지만 전체 구조를 이해하는 순서로는 Core부터 읽는 것이 더 좋다.

추천 학습 순서:

```text
1. EquipmentStateMachine
2. VirtualIoController
3. EquipmentCellController
4. MolyAldRunner
5. OperatorConsoleViewModel
6. MainWindow.xaml
```

## 23. 핵심 용어 요약

| 용어 | 뜻 |
|---|---|
| Core | 장비 동작의 진짜 로직이 있는 프로젝트 |
| UI | Core 상태를 사용자에게 보여주는 화면 |
| State | 장비의 현재 단계 |
| Event | State를 바꾸는 입력 |
| Transition | State가 다른 State로 바뀌는 것 |
| IO | Input/Output 신호 |
| Input | 센서처럼 장비 SW가 읽는 값 |
| Output | 장비 SW가 밖으로 내보내는 명령 |
| Alarm | 정상 진행을 멈춰야 하는 이상 상태 |
| Recovery | 알람 원인을 제거하고 복구하는 절차 |
| Clock | 현재 시간을 알려주는 객체 |
| ManualClock | 테스트에서 시간을 직접 움직이는 가짜 시계 |
| Timeout | 정해진 시간 안에 완료되지 않아 발생하는 이상 |
| Motion Axis | 가상 모터/축 |
| Recipe | 제품이나 공정 조건 |
| Template | 장비 구조/축/검사/고장 조건의 틀 |
| Runner | 모델을 실제 순서대로 실행하는 객체 |
| CLI | PowerShell에서 실행하는 프로그램 |
| WPF | Windows 데스크톱 UI 기술 |
| XAML | WPF 화면 배치를 적는 XML 기반 문법 |
| ViewModel | 화면에 보여줄 데이터와 버튼 동작을 가진 C# 객체 |
| Binding | XAML 화면과 ViewModel 속성을 연결하는 것 |
| Command | 버튼 클릭을 ViewModel 메서드와 연결하는 것 |
| CI | GitHub에서 자동으로 빌드/테스트하는 흐름 |

## 24. 이 프로젝트를 직접 확장하기 위한 최소 루틴

매일 작업할 때는 아래 순서가 좋다.

```text
1. state/triage.md에서 다음 작업 확인
2. 관련 Core 파일 먼저 읽기
3. 테스트를 먼저 생각하기
4. 작은 코드 변경
5. dotnet build
6. Core tests 실행
7. WPF 실행해서 눈검증
8. docs/learning 또는 logs에 배운 점 기록
9. PR로 병합
```

코드를 이해하지 못한 상태에서 UI만 계속 붙이면 유지보수 debt가 생긴다.

이 프로젝트의 목적은 빠르게 만들어지는 결과물보다, 장비 SW 엔지니어로서 설명 가능한 구조를 만드는 것이다.
