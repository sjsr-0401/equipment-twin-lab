# 001. Core 상태머신 코드 독해

읽을 파일:

```text
src/EquipmentTwin.Core/EquipmentState.cs
src/EquipmentTwin.Core/EquipmentEvent.cs
src/EquipmentTwin.Core/TransitionResult.cs
src/EquipmentTwin.Core/EquipmentTransition.cs
src/EquipmentTwin.Core/EquipmentStateMachine.cs
```

이 문서는 Visual Studio에서 위 파일들을 열어 놓고 같이 보는 문서다.

## 0. 이 묶음의 역할

이 다섯 파일은 프로젝트의 출발점이다.

장비 SW를 가장 단순하게 표현하면 다음 세 가지가 필요하다.

```text
1. 지금 장비가 어디에 있는가?        -> EquipmentState
2. 어떤 신호가 들어왔는가?            -> EquipmentEvent
3. 그 신호를 적용한 결과가 무엇인가?  -> TransitionResult / EquipmentTransition
```

그리고 이 세 가지를 실제로 움직이는 클래스가 있다.

```text
EquipmentStateMachine
```

상태머신은 이 프로젝트에서 “장비 동작의 가장 작은 진실”이다.

WPF, Unity, CLI가 없어도 상태머신만 있으면 장비의 기본 흐름을 테스트할 수 있다.

## 1. `EquipmentState.cs`

코드:

```csharp
namespace EquipmentTwin.Core;

/// <summary>
/// 장비가 현재 어느 공정 단계에 있는지 표현한다.
/// </summary>
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

### 줄 단위 해설

| 줄 | 코드 | 설명 |
|---:|---|---|
| 1 | `namespace EquipmentTwin.Core;` | 이 파일이 `EquipmentTwin.Core` 영역에 속한다는 뜻이다. 같은 namespace 안의 타입들은 서로 짧은 이름으로 접근할 수 있다. |
| 3~5 | XML summary 주석 | Visual Studio에서 타입에 마우스를 올렸을 때 설명으로 보인다. 코드 실행에는 영향이 없다. |
| 6 | `public enum EquipmentState` | `EquipmentState`라는 enum을 공개한다. enum은 정해진 값 중 하나만 가질 수 있는 타입이다. |
| 8 | `Idle` | 장비가 대기 중인 상태다. 아직 공정이 시작되지 않았다. |
| 9 | `Loading` | 제품/웨이퍼/파트를 장비에 넣는 단계다. |
| 10 | `Aligning` | 위치 정렬 단계다. 실제 장비에서는 stage나 wafer alignment가 여기에 해당할 수 있다. |
| 11 | `Inspecting` | 검사 또는 처리 중인 단계다. 초기 MVP에서는 inspection이라는 이름을 썼다. |
| 12 | `Unloading` | 작업이 끝난 대상을 밖으로 내보내는 단계다. |
| 13 | `Complete` | 정상 사이클이 끝난 상태다. |
| 14 | `Alarmed` | 안전 이벤트나 timeout 때문에 정상 진행이 멈춘 상태다. |

### 왜 enum으로 만들었나

문자열로 만들 수도 있다.

```csharp
string state = "Idle";
```

하지만 문자열은 오타에 약하다.

```csharp
state = "Idel"; // 컴파일은 되지만 의미는 깨진다.
```

enum은 정해진 값만 쓸 수 있다.

```csharp
EquipmentState state = EquipmentState.Idle;
```

그래서 장비 상태처럼 정해진 목록이 있는 값은 enum이 적합하다.

### 이 파일을 수정해야 하는 경우

새 장비 상태가 생기면 여기부터 수정한다.

예:

```text
PumpDown
Heating
Processing
PostPurge
```

하지만 enum에 값만 추가하면 끝이 아니다.

상태를 추가하면 반드시 같이 확인해야 한다.

```text
1. EquipmentStateMachine의 AllowedTransitions
2. EquipmentCellController의 SyncOutputsForCurrentState
3. 테스트
4. WPF 표시 이름
5. 문서
```

## 2. `EquipmentEvent.cs`

코드:

```csharp
namespace EquipmentTwin.Core;

/// <summary>
/// 장비 상태머신에 들어오는 명령 또는 외부 이벤트다.
/// 실제 장비에서는 PLC 신호, 센서 신호, 작업 완료 신호, 사용자 조작이 이런 이벤트로 변환된다.
/// </summary>
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

### 줄 단위 해설

| 줄 | 코드 | 설명 |
|---:|---|---|
| 1 | `namespace EquipmentTwin.Core;` | 이 enum도 Core namespace에 둔다. |
| 3~6 | XML summary 주석 | 이벤트가 실제 장비의 센서/PLC/사용자 조작에서 온다는 의도를 설명한다. |
| 7 | `public enum EquipmentEvent` | 상태를 바꾸려고 들어오는 이벤트 목록이다. |
| 9 | `StartLoad` | Idle 상태에서 Loading으로 가기 위한 시작 이벤트다. |
| 10 | `LoadComplete` | Loading이 끝났다는 이벤트다. 다음 상태는 Aligning이다. |
| 11 | `AlignmentComplete` | 정렬이 끝났다는 이벤트다. 다음 상태는 Inspecting이다. |
| 12 | `InspectionComplete` | 검사/처리가 끝났다는 이벤트다. 다음 상태는 Unloading이다. |
| 13 | `UnloadComplete` | 배출이 끝났다는 이벤트다. 다음 상태는 Complete다. |
| 14 | `Reset` | Complete에서 Idle로 돌아가기 위한 이벤트다. |
| 15 | `DoorOpened` | 문이 열린 안전 이벤트다. 정상 흐름보다 우선되어 Alarmed로 간다. |
| 16 | `EmergencyStop` | 비상정지 이벤트다. 정상 흐름보다 우선되어 Alarmed로 간다. |
| 17 | `Timeout` | 상태가 제한 시간 안에 끝나지 않았다는 이벤트다. Alarmed로 간다. |
| 18 | `ClearAlarm` | Alarmed에서 Idle로 돌아가려는 이벤트다. |

### State와 Event 차이

이 프로젝트를 이해할 때 가장 중요한 구분이다.

```text
State = 현재 위치
Event = 현재 위치를 바꾸려는 입력
```

예:

```text
현재 State: Loading
들어온 Event: LoadComplete
결과 State: Aligning
```

주니어가 자주 헷갈리는 부분:

```text
LoadingComplete라는 상태를 만들지 않는다.
LoadComplete는 상태가 아니라 이벤트다.
```

완료 신호가 들어오면 상태가 바뀐다.

## 3. `TransitionResult.cs`

코드:

```csharp
using EquipmentTwin.Core.Alarms;

namespace EquipmentTwin.Core;

/// <summary>
/// 이벤트를 적용한 결과다.
/// UI, 로그, 테스트는 이 결과를 보고 장비 상태가 정상적으로 바뀌었는지 판단한다.
/// </summary>
public sealed record TransitionResult(
    EquipmentState PreviousState,
    EquipmentEvent Event,
    EquipmentState CurrentState,
    bool Accepted,
    string Message,
    AlarmInfo? Alarm = null);
```

### 줄 단위 해설

| 줄 | 코드 | 설명 |
|---:|---|---|
| 1 | `using EquipmentTwin.Core.Alarms;` | `AlarmInfo` 타입을 쓰기 위해 Alarms namespace를 가져온다. |
| 3 | `namespace EquipmentTwin.Core;` | 이 결과 타입도 Core에 속한다. |
| 5~8 | XML summary 주석 | 이 타입이 UI/로그/테스트에서 공통으로 읽히는 결과라는 뜻이다. |
| 9 | `public sealed record TransitionResult(` | 상태 전이 결과를 담는 record를 만든다. `sealed`라 상속할 수 없다. |
| 10 | `EquipmentState PreviousState` | 이벤트 적용 전 상태다. |
| 11 | `EquipmentEvent Event` | 적용하려고 한 이벤트다. |
| 12 | `EquipmentState CurrentState` | 이벤트 적용 후 상태다. 실패하면 이전 상태와 같을 수 있다. |
| 13 | `bool Accepted` | 이벤트를 받아들였는지 여부다. |
| 14 | `string Message` | 사람이 읽을 수 있는 설명이다. 로그와 디버깅에 중요하다. |
| 15 | `AlarmInfo? Alarm = null` | 알람 전이인 경우 알람 정보를 담는다. `?`는 null일 수 있다는 뜻이다. |

### record를 쓴 이유

`record`는 “값 묶음”을 표현할 때 편하다.

`TransitionResult`는 동작을 많이 가진 객체가 아니다.

그냥 결과 데이터를 담는다.

그래서 class보다 record가 잘 맞는다.

### `TransitionResult`와 `EquipmentTransition` 차이

둘이 비슷해 보이지만 용도가 다르다.

```text
TransitionResult = 이번 Apply 호출의 반환값
EquipmentTransition = History에 저장되는 기록
```

현재는 필드가 거의 같지만, 의도가 다르기 때문에 타입을 나눴다.

## 4. `EquipmentTransition.cs`

코드:

```csharp
using EquipmentTwin.Core.Alarms;

namespace EquipmentTwin.Core;

/// <summary>
/// 상태 전이 시도 기록이다.
/// Accepted가 false면 상태는 바뀌지 않았고, 잘못된 이벤트가 들어온 것이다.
/// </summary>
public sealed record EquipmentTransition(
    EquipmentState From,
    EquipmentEvent Event,
    EquipmentState To,
    bool Accepted,
    string Message,
    AlarmInfo? Alarm = null);
```

### 줄 단위 해설

| 줄 | 코드 | 설명 |
|---:|---|---|
| 1 | `using EquipmentTwin.Core.Alarms;` | History에도 알람 정보를 같이 남기기 위해 필요하다. |
| 3 | `namespace EquipmentTwin.Core;` | Core 영역에 둔다. |
| 5~8 | XML summary 주석 | 이 record가 “기록”이라는 점을 설명한다. |
| 9 | `public sealed record EquipmentTransition(` | 상태 전이 시도를 저장하는 record다. |
| 10 | `EquipmentState From` | 시도 전 상태다. |
| 11 | `EquipmentEvent Event` | 들어온 이벤트다. |
| 12 | `EquipmentState To` | 시도 후 상태다. 실패하면 `From`과 같다. |
| 13 | `bool Accepted` | 전이가 받아들여졌는지 여부다. |
| 14 | `string Message` | 사람이 읽는 설명이다. |
| 15 | `AlarmInfo? Alarm = null` | 알람 전이였다면 알람 정보를 보관한다. |

### 왜 실패도 History에 남기나

실무에서는 실패한 명령도 중요하다.

예:

```text
장비가 Idle인데 InspectionComplete 이벤트가 들어왔다.
```

이 이벤트는 거부되어야 한다.

하지만 거부 사실을 기록하지 않으면 나중에 원인 분석이 어렵다.

그래서 실패도 History에 남긴다.

```text
Accepted = false
From = Idle
To = Idle
Event = InspectionComplete
Message = "'InspectionComplete' event is not allowed while equipment is 'Idle'."
```

## 5. `EquipmentStateMachine.cs`

이 파일이 핵심이다.

상태머신은 두 가지 일을 한다.

```text
1. 이벤트가 현재 상태에서 허용되는지 판단한다.
2. 허용되면 상태를 바꾸고 기록한다.
```

## 5.1 using / namespace / class 선언

코드:

```csharp
using EquipmentTwin.Core.Alarms;

namespace EquipmentTwin.Core;

/// <summary>
/// 제조 장비의 기본 공정 흐름을 표현하는 상태머신이다.
/// 이 클래스는 Unity 화면이나 실제 PLC에 직접 의존하지 않는다.
/// </summary>
public sealed class EquipmentStateMachine
{
```

### 줄 단위 해설

| 줄 | 코드 | 설명 |
|---:|---|---|
| 1 | `using EquipmentTwin.Core.Alarms;` | `AlarmInfo`, `AlarmCode` 관련 타입을 쓰기 위해 필요하다. |
| 3 | `namespace EquipmentTwin.Core;` | Core namespace에 속한다. |
| 5~8 | XML summary 주석 | 이 클래스가 UI나 PLC가 아니라 순수 상태 흐름을 담당한다는 의도를 설명한다. |
| 9 | `public sealed class EquipmentStateMachine` | 외부에서 사용할 수 있는 상태머신 클래스다. `sealed`라 상속으로 동작을 바꾸지 못하게 했다. |
| 10 | `{` | 클래스 본문 시작이다. |

### 왜 sealed인가

상태머신은 장비 동작의 중심이다.

누군가 상속해서 일부 동작만 몰래 바꾸면 테스트하기 어려워진다.

그래서 현재 MVP에서는 상속을 막았다.

확장이 필요하면 상속보다 새 정책 객체나 새 메서드로 확장하는 편이 안전하다.

## 5.2 AllowedTransitions

코드:

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

### 줄 단위 해설

| 줄 | 코드 | 설명 |
|---:|---|---|
| 11 | `private static readonly ... AllowedTransitions =` | 허용된 상태 전이 표를 만든다. 클래스 내부에서만 쓰고, 모든 인스턴스가 공유한다. |
| 11 | `IReadOnlyDictionary` | 읽기 전용으로 노출되는 dictionary다. 외부에서 수정하지 못한다는 의도다. |
| 11 | `(EquipmentState State, EquipmentEvent Event)` | dictionary key가 tuple이다. 현재 상태와 이벤트를 한 쌍으로 묶어 key로 쓴다. |
| 11 | `EquipmentState` | dictionary value다. key에 해당하는 다음 상태다. |
| 12 | `new Dictionary<...>` | 실제 저장소는 Dictionary다. |
| 13 | `{` | dictionary 초기화 시작이다. |
| 14 | `(Idle, StartLoad) -> Loading` | Idle에서 StartLoad 이벤트가 오면 Loading으로 간다. |
| 15 | `(Loading, LoadComplete) -> Aligning` | Loading이 완료되면 Aligning으로 간다. |
| 16 | `(Aligning, AlignmentComplete) -> Inspecting` | 정렬이 끝나면 Inspecting으로 간다. |
| 17 | `(Inspecting, InspectionComplete) -> Unloading` | 검사/처리가 끝나면 Unloading으로 간다. |
| 18 | `(Unloading, UnloadComplete) -> Complete` | 배출이 끝나면 Complete로 간다. |
| 19 | `(Complete, Reset) -> Idle` | 정상 완료 후 Reset하면 Idle로 돌아간다. |
| 20 | `(Alarmed, ClearAlarm) -> Idle` | 알람 상태에서 ClearAlarm하면 Idle로 돌아간다. 단, 복구 조건은 CellController에서 별도로 검사한다. |
| 21 | `};` | dictionary 초기화 끝이다. |

### 왜 dictionary인가

상태 전이를 표처럼 보기 위해서다.

이 코드는 사실상 아래 표다.

| 현재 상태 | 이벤트 | 다음 상태 |
|---|---|---|
| Idle | StartLoad | Loading |
| Loading | LoadComplete | Aligning |
| Aligning | AlignmentComplete | Inspecting |
| Inspecting | InspectionComplete | Unloading |
| Unloading | UnloadComplete | Complete |
| Complete | Reset | Idle |
| Alarmed | ClearAlarm | Idle |

상태머신을 읽을 때 가장 먼저 이 표를 이해해야 한다.

## 5.3 내부 필드

코드:

```csharp
private readonly List<EquipmentTransition> _history = new();
private readonly IClock _clock;
```

### 줄 단위 해설

| 줄 | 코드 | 설명 |
|---:|---|---|
| 23 | `_history` | 상태 전이 시도를 순서대로 저장하는 내부 리스트다. 성공과 실패 모두 들어간다. |
| 23 | `readonly` | `_history` 변수가 다른 리스트로 바뀌지 않는다는 뜻이다. 리스트 안의 내용 추가는 가능하다. |
| 24 | `_clock` | 현재 시간을 알기 위한 clock이다. timeout과 상태 진입 시간을 기록하는 데 쓴다. |

### 왜 `_history` 앞에 underscore가 있나

C#에서 private field에 `_`를 붙이는 스타일이다.

강제 문법은 아니지만 읽을 때 도움이 된다.

```text
_history = 클래스 내부 필드
History = 외부에서 읽는 public property
```

## 5.4 생성자

코드:

```csharp
public EquipmentStateMachine()
    : this(SystemClock.Instance)
{
}

public EquipmentStateMachine(IClock clock)
{
    _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    StateEnteredAtUtc = _clock.UtcNow;
}
```

### 줄 단위 해설

| 줄 | 코드 | 설명 |
|---:|---|---|
| 26 | `public EquipmentStateMachine()` | 인자를 받지 않는 기본 생성자다. |
| 27 | `: this(SystemClock.Instance)` | 실제 시스템 시간을 쓰는 생성자로 연결한다. 생성자 체이닝이라고 한다. |
| 28~29 | 빈 본문 | 실제 초기화는 아래 생성자에서 한다. |
| 31 | `public EquipmentStateMachine(IClock clock)` | clock을 외부에서 주입받는 생성자다. 테스트에서는 ManualClock을 넣는다. |
| 33 | `_clock = clock ?? throw ...` | clock이 null이면 예외를 던지고, 아니면 내부 필드에 저장한다. |
| 34 | `StateEnteredAtUtc = _clock.UtcNow;` | 상태머신이 만들어진 시점을 현재 상태 진입 시간으로 기록한다. |

### 왜 생성자가 두 개인가

운영 코드에서는 간단히 쓰고 싶다.

```csharp
var machine = new EquipmentStateMachine();
```

테스트에서는 시간을 제어하고 싶다.

```csharp
var clock = new ManualClock(startTime);
var machine = new EquipmentStateMachine(clock);
```

그래서 두 생성자를 제공한다.

## 5.5 public property

코드:

```csharp
public EquipmentState CurrentState { get; private set; } = EquipmentState.Idle;

public DateTimeOffset StateEnteredAtUtc { get; private set; }

public string? LastAlarmReason { get; private set; }

public AlarmInfo? LastAlarm { get; private set; }

public IReadOnlyList<EquipmentTransition> History => _history;
```

### 줄 단위 해설

| 줄 | 코드 | 설명 |
|---:|---|---|
| 37 | `CurrentState` | 현재 장비 상태다. 외부에서는 읽을 수 있지만, 바꾸는 것은 상태머신 내부만 가능하다. |
| 37 | `= EquipmentState.Idle` | 상태머신은 처음에 Idle에서 시작한다. |
| 39 | `StateEnteredAtUtc` | 현재 상태에 들어온 시간이다. timeout 계산에 사용한다. |
| 41 | `LastAlarmReason` | 마지막 알람 메시지다. 없을 수 있으므로 `string?`다. |
| 43 | `LastAlarm` | 마지막 알람 상세 정보다. 없을 수 있으므로 `AlarmInfo?`다. |
| 45 | `History => _history` | 내부 history 리스트를 읽기 전용 인터페이스로 외부에 보여준다. |

### `get; private set;` 의미

```csharp
public EquipmentState CurrentState { get; private set; }
```

뜻:

```text
외부에서는 읽을 수 있다.
외부에서는 직접 바꿀 수 없다.
이 클래스 내부에서만 바꿀 수 있다.
```

이게 중요하다.

외부에서 마음대로 상태를 바꾸면 상태머신 규칙이 깨진다.

상태는 반드시 `Apply()`를 통해서만 바뀌어야 한다.

## 5.6 CanApply

코드:

```csharp
public bool CanApply(EquipmentEvent equipmentEvent)
{
    if (IsSafetyEvent(equipmentEvent))
    {
        return CurrentState != EquipmentState.Alarmed;
    }

    return AllowedTransitions.ContainsKey((CurrentState, equipmentEvent));
}
```

### 줄 단위 해설

| 줄 | 코드 | 설명 |
|---:|---|---|
| 47 | `public bool CanApply(...)` | 특정 이벤트를 현재 상태에 적용할 수 있는지 미리 확인하는 메서드다. |
| 49 | `if (IsSafetyEvent(equipmentEvent))` | 이벤트가 DoorOpened/EmergencyStop/Timeout 같은 안전 이벤트인지 확인한다. |
| 51 | `return CurrentState != EquipmentState.Alarmed;` | 이미 Alarmed가 아니라면 안전 이벤트는 적용 가능하다. 이미 Alarmed면 중복 알람은 거부한다. |
| 54 | `return AllowedTransitions.ContainsKey(...)` | 일반 이벤트는 전이 표에 등록되어 있을 때만 가능하다. |

### 이 메서드는 언제 쓰나

UI에서 버튼 활성화 여부를 판단할 때 쓸 수 있다.

예:

```text
현재 Idle이면 Start 가능
현재 Loading이면 Start 불가능
```

현재 WPF에서는 모든 버튼을 단순하게 노출하지만, 나중에 버튼 enable/disable을 고도화할 때 이 메서드를 쓸 수 있다.

## 5.7 Apply

코드:

```csharp
public TransitionResult Apply(EquipmentEvent equipmentEvent)
{
    var previous = CurrentState;

    if (IsSafetyEvent(equipmentEvent))
    {
        return ApplyAlarmEvent(previous, equipmentEvent);
    }

    if (!AllowedTransitions.TryGetValue((CurrentState, equipmentEvent), out var nextState))
    {
        return Reject(previous, equipmentEvent, $"'{equipmentEvent}' event is not allowed while equipment is '{CurrentState}'.");
    }

    MoveTo(nextState);

    if (CurrentState == EquipmentState.Idle)
    {
        LastAlarmReason = null;
        LastAlarm = null;
    }

    return Accept(previous, equipmentEvent, nextState, $"State changed from '{previous}' to '{nextState}'.");
}
```

### 줄 단위 해설

| 줄 | 코드 | 설명 |
|---:|---|---|
| 57 | `public TransitionResult Apply(...)` | 외부에서 이벤트를 상태머신에 적용하는 핵심 메서드다. |
| 59 | `var previous = CurrentState;` | 변경 전 상태를 저장한다. 결과와 history에 필요하다. |
| 61 | `if (IsSafetyEvent(...))` | 안전 이벤트는 일반 전이 표보다 먼저 처리한다. |
| 63 | `return ApplyAlarmEvent(...)` | 안전 이벤트면 Alarmed로 보내는 별도 로직으로 위임한다. |
| 66 | `TryGetValue(...)` | 현재 상태와 이벤트 조합이 전이 표에 있는지 찾는다. 있으면 `nextState`에 다음 상태가 들어간다. |
| 68 | `return Reject(...)` | 허용되지 않은 이벤트면 상태를 바꾸지 않고 거부 결과를 반환한다. |
| 71 | `MoveTo(nextState);` | 허용된 이벤트라면 실제 현재 상태를 다음 상태로 바꾼다. |
| 73 | `if (CurrentState == EquipmentState.Idle)` | Idle로 돌아왔는지 확인한다. |
| 75 | `LastAlarmReason = null;` | Idle로 돌아오면 마지막 알람 메시지를 지운다. |
| 76 | `LastAlarm = null;` | Idle로 돌아오면 마지막 알람 객체도 지운다. |
| 79 | `return Accept(...)` | 성공 결과를 만들고 history에도 남긴다. |

### Apply를 읽는 순서

이 메서드는 아래 흐름으로 읽으면 된다.

```text
1. 이전 상태 저장
2. 안전 이벤트인지 확인
3. 일반 전이 표에서 다음 상태 찾기
4. 없으면 Reject
5. 있으면 MoveTo
6. Idle이면 알람 정보 초기화
7. Accept 결과 반환
```

### 왜 안전 이벤트를 먼저 보나

장비 SW에서 안전 이벤트는 일반 공정 이벤트보다 우선이다.

예:

```text
현재 Loading
LoadComplete와 DoorOpened가 거의 동시에 들어옴
```

이 경우 LoadComplete보다 DoorOpened가 우선이어야 한다.

상태머신 자체에서는 이벤트 하나씩만 받지만, 설계 의도는 안전 이벤트를 별도 처리하는 것이다.

## 5.8 RejectCommand

코드:

```csharp
public TransitionResult RejectCommand(EquipmentEvent equipmentEvent, string message)
{
    return Reject(CurrentState, equipmentEvent, message, LastAlarm);
}
```

### 줄 단위 해설

| 줄 | 코드 | 설명 |
|---:|---|---|
| 82 | `public TransitionResult RejectCommand(...)` | 외부 Controller가 명령을 거부해야 할 때 쓰는 메서드다. |
| 84 | `return Reject(...)` | 현재 상태를 유지하면서 거부 기록을 남긴다. |

### 왜 필요한가

알람 해제는 상태머신 전이표만으로 결정하면 안 된다.

예:

```text
DoorOpened 알람인데 문이 아직 열려 있다.
```

전이표에는 `Alarmed + ClearAlarm -> Idle`이 있다.

하지만 실제로는 문이 닫히기 전까지 ClearAlarm을 거부해야 한다.

그 판단은 `EquipmentCellController`가 한다.

Controller가 “아직 ClearAlarm 안 됨”이라고 판단하면 `RejectCommand()`로 상태머신 history에 거부 기록을 남긴다.

## 5.9 CheckTimeout

코드:

```csharp
public TimeoutCheckResult CheckTimeout(StateTimeoutPolicy policy)
{
    ArgumentNullException.ThrowIfNull(policy);

    var elapsed = _clock.UtcNow - StateEnteredAtUtc;

    if (!policy.TryGetTimeout(CurrentState, out var timeout))
    {
        return new TimeoutCheckResult(CurrentState, false, elapsed, null, null);
    }

    if (elapsed < timeout)
    {
        return new TimeoutCheckResult(CurrentState, false, elapsed, timeout, null);
    }

    var timedOutState = CurrentState;
    var alarm = AlarmInfo.FromEvent(EquipmentEvent.Timeout, timedOutState) with
    {
        Message = $"State '{timedOutState}' timed out after {timeout}."
    };
    var transition = MoveToAlarm(timedOutState, alarm);

    return new TimeoutCheckResult(timedOutState, true, elapsed, timeout, transition);
}
```

### 줄 단위 해설

| 줄 | 코드 | 설명 |
|---:|---|---|
| 87 | `public TimeoutCheckResult CheckTimeout(...)` | 현재 상태가 timeout인지 검사하는 메서드다. |
| 89 | `ArgumentNullException.ThrowIfNull(policy);` | timeout 정책이 null이면 바로 예외를 던진다. |
| 91 | `var elapsed = _clock.UtcNow - StateEnteredAtUtc;` | 현재 상태에 들어온 뒤 얼마나 지났는지 계산한다. |
| 93 | `if (!policy.TryGetTimeout(...))` | 현재 상태에 timeout 규칙이 있는지 확인한다. |
| 95 | `return ... false ... null, null` | timeout 규칙이 없으면 timeout이 아니다. |
| 98 | `if (elapsed < timeout)` | 제한 시간보다 덜 지났는지 확인한다. |
| 100 | `return ... false ... timeout, null` | 아직 제한 시간 전이면 timeout이 아니다. |
| 103 | `var timedOutState = CurrentState;` | timeout이 발생한 원래 상태를 보관한다. 이후 Alarmed로 바뀌기 때문이다. |
| 104 | `AlarmInfo.FromEvent(...) with` | Timeout 이벤트에 해당하는 알람 정보를 만들고 일부 값을 덮어쓴다. |
| 106 | `Message = ...` | timeout 메시지를 더 구체적으로 바꾼다. |
| 108 | `var transition = MoveToAlarm(...)` | 실제 상태를 Alarmed로 바꾸고 transition을 기록한다. |
| 110 | `return new TimeoutCheckResult(...)` | timeout 검사 결과를 반환한다. |

### `with` 문법이 무엇인가

`AlarmInfo`는 record다.

record는 기존 값을 복사하면서 일부 property만 바꿀 수 있다.

```csharp
var alarm = AlarmInfo.FromEvent(...) with
{
    Message = "new message"
};
```

뜻:

```text
기존 AlarmInfo를 기반으로 새 AlarmInfo를 만들되,
Message만 바꾼다.
```

## 5.10 ApplyAlarmEvent

코드:

```csharp
private TransitionResult ApplyAlarmEvent(EquipmentState previous, EquipmentEvent equipmentEvent)
{
    if (CurrentState == EquipmentState.Alarmed)
    {
        return Reject(previous, equipmentEvent, $"Equipment is already alarmed. Clear the alarm before applying '{equipmentEvent}'.");
    }

    return MoveToAlarm(previous, AlarmInfo.FromEvent(equipmentEvent, previous));
}
```

### 줄 단위 해설

| 줄 | 코드 | 설명 |
|---:|---|---|
| 113 | `private TransitionResult ApplyAlarmEvent(...)` | 안전 이벤트를 처리하는 내부 메서드다. 외부에서 직접 부르지 않는다. |
| 115 | `if (CurrentState == EquipmentState.Alarmed)` | 이미 알람 상태인지 확인한다. |
| 117 | `return Reject(...)` | 이미 알람이면 새 알람 이벤트를 중복 적용하지 않고 거부한다. |
| 120 | `return MoveToAlarm(...)` | 알람이 아니면 AlarmInfo를 만들고 Alarmed로 이동한다. |

### 왜 중복 알람을 거부하나

현재 MVP에서는 하나의 active alarm만 추적한다.

이미 Alarmed인데 새 DoorOpened나 Timeout을 계속 받으면 어떤 알람이 원인인지 흐려질 수 있다.

그래서 먼저 기존 알람을 Clear하도록 요구한다.

## 5.11 Accept

코드:

```csharp
private TransitionResult Accept(EquipmentState previous, EquipmentEvent equipmentEvent, EquipmentState nextState, string message, AlarmInfo? alarm = null)
{
    var transition = new EquipmentTransition(previous, equipmentEvent, nextState, true, message, alarm);
    _history.Add(transition);
    return new TransitionResult(previous, equipmentEvent, nextState, true, message, alarm);
}
```

### 줄 단위 해설

| 줄 | 코드 | 설명 |
|---:|---|---|
| 123 | `private TransitionResult Accept(...)` | 성공한 전이를 처리하는 내부 helper다. |
| 125 | `new EquipmentTransition(...)` | history에 남길 기록을 만든다. `Accepted`는 true다. |
| 126 | `_history.Add(transition);` | 성공한 전이를 history에 추가한다. |
| 127 | `return new TransitionResult(...)` | 이번 Apply 호출의 반환값을 만든다. |

### 왜 helper로 뺐나

성공할 때마다 해야 하는 일이 같다.

```text
1. History에 남긴다.
2. TransitionResult를 반환한다.
```

이걸 여러 곳에 복붙하지 않기 위해 helper로 만들었다.

## 5.12 Reject

코드:

```csharp
private TransitionResult Reject(EquipmentState previous, EquipmentEvent equipmentEvent, string message, AlarmInfo? alarm = null)
{
    var transition = new EquipmentTransition(previous, equipmentEvent, previous, false, message, alarm);
    _history.Add(transition);
    return new TransitionResult(previous, equipmentEvent, previous, false, message, alarm);
}
```

### 줄 단위 해설

| 줄 | 코드 | 설명 |
|---:|---|---|
| 130 | `private TransitionResult Reject(...)` | 실패한 전이를 처리하는 내부 helper다. |
| 132 | `new EquipmentTransition(previous, equipmentEvent, previous, false, ...)` | 실패했으므로 From과 To가 둘 다 previous다. 상태가 바뀌지 않았다. |
| 133 | `_history.Add(transition);` | 실패도 history에 남긴다. |
| 134 | `return new TransitionResult(...)` | 실패 결과를 반환한다. |

### 실패를 기록하는 이유

장비에서는 “잘못된 명령이 들어왔다”도 중요한 사건이다.

나중에 작업자 로그나 디버깅에서 원인을 찾을 수 있다.

## 5.13 IsSafetyEvent

코드:

```csharp
private static bool IsSafetyEvent(EquipmentEvent equipmentEvent)
{
    return equipmentEvent is EquipmentEvent.DoorOpened or EquipmentEvent.EmergencyStop or EquipmentEvent.Timeout;
}
```

### 줄 단위 해설

| 줄 | 코드 | 설명 |
|---:|---|---|
| 137 | `private static bool IsSafetyEvent(...)` | 이벤트가 안전 이벤트인지 판단한다. 인스턴스 상태를 쓰지 않으므로 static이다. |
| 139 | `equipmentEvent is ... or ...` | C# pattern matching 문법이다. 세 이벤트 중 하나면 true다. |

### 왜 안전 이벤트를 따로 분류하나

안전 이벤트는 정상 전이 표에 없어도 Alarmed로 이동해야 한다.

예:

```text
Loading + DoorOpened -> Alarmed
Inspecting + EmergencyStop -> Alarmed
Unloading + Timeout -> Alarmed
```

이걸 모든 상태별로 dictionary에 넣으면 전이표가 지저분해진다.

그래서 안전 이벤트는 별도 메서드로 분류했다.

## 5.14 MoveToAlarm

코드:

```csharp
private TransitionResult MoveToAlarm(EquipmentState previous, AlarmInfo alarm)
{
    MoveTo(EquipmentState.Alarmed);
    LastAlarm = alarm;
    LastAlarmReason = alarm.Message;
    return Accept(previous, alarm.SourceEvent, CurrentState, alarm.Message, alarm);
}
```

### 줄 단위 해설

| 줄 | 코드 | 설명 |
|---:|---|---|
| 142 | `private TransitionResult MoveToAlarm(...)` | 알람 상태로 이동하는 공통 helper다. |
| 144 | `MoveTo(EquipmentState.Alarmed);` | 실제 상태를 Alarmed로 바꾼다. |
| 145 | `LastAlarm = alarm;` | 마지막 알람 상세 정보를 저장한다. |
| 146 | `LastAlarmReason = alarm.Message;` | 마지막 알람 메시지를 저장한다. |
| 147 | `return Accept(...)` | 알람 전이도 성공한 전이로 history에 남긴다. |

### 왜 알람 전이는 Accepted=true인가

알람은 “실패”처럼 보이지만 상태머신 관점에서는 정상적으로 처리된 이벤트다.

예:

```text
DoorOpened 이벤트가 들어왔다.
상태머신이 Alarmed로 잘 이동했다.
```

그러면 이 전이는 Accepted=true다.

장비 공정은 실패했지만, 상태머신은 안전 이벤트를 정상 처리한 것이다.

이 구분이 중요하다.

## 5.15 MoveTo

코드:

```csharp
private void MoveTo(EquipmentState state)
{
    CurrentState = state;
    StateEnteredAtUtc = _clock.UtcNow;
}
```

### 줄 단위 해설

| 줄 | 코드 | 설명 |
|---:|---|---|
| 150 | `private void MoveTo(...)` | 현재 상태를 바꾸는 내부 helper다. 반환값은 없다. |
| 152 | `CurrentState = state;` | 실제 현재 상태를 새 상태로 바꾼다. |
| 153 | `StateEnteredAtUtc = _clock.UtcNow;` | 새 상태에 들어온 시간을 기록한다. Timeout 계산에 필요하다. |

### 왜 상태 변경을 한 메서드에 모았나

상태가 바뀔 때마다 상태 진입 시간도 같이 바뀌어야 한다.

만약 여기저기서 직접 이렇게 쓰면 실수할 수 있다.

```csharp
CurrentState = nextState;
```

시간 업데이트를 빼먹을 수 있다.

그래서 `MoveTo()` 안에 묶었다.

```text
상태 변경 = CurrentState 변경 + StateEnteredAtUtc 갱신
```

이 규칙을 한 곳에서 강제한다.

## 6. 전체 실행 예시

정상 흐름을 머릿속으로 따라가면 다음과 같다.

```csharp
var machine = new EquipmentStateMachine();

machine.Apply(EquipmentEvent.StartLoad);
machine.Apply(EquipmentEvent.LoadComplete);
machine.Apply(EquipmentEvent.AlignmentComplete);
machine.Apply(EquipmentEvent.InspectionComplete);
machine.Apply(EquipmentEvent.UnloadComplete);
```

상태 변화:

```text
Idle
  -> Loading
  -> Aligning
  -> Inspecting
  -> Unloading
  -> Complete
```

각 Apply마다:

```text
1. previous 저장
2. event 허용 여부 확인
3. MoveTo
4. History 추가
5. TransitionResult 반환
```

## 7. 잘못된 이벤트 예시

```csharp
var machine = new EquipmentStateMachine();
var result = machine.Apply(EquipmentEvent.InspectionComplete);
```

현재 상태는 Idle이다.

전이표에는 이런 항목이 없다.

```text
Idle + InspectionComplete -> ?
```

그래서 결과는:

```text
Accepted = false
PreviousState = Idle
CurrentState = Idle
History에 실패 기록 추가
```

이것이 “상태머신이 장비를 보호하는 방식”이다.

## 8. 알람 예시

```csharp
var machine = new EquipmentStateMachine();
machine.Apply(EquipmentEvent.StartLoad);
var result = machine.Apply(EquipmentEvent.DoorOpened);
```

상태 변화:

```text
Idle -> Loading -> Alarmed
```

`DoorOpened`는 전이표에 없어도 안전 이벤트라서 Alarmed로 간다.

## 9. Visual Studio 디버깅 실습

### 9.1 Breakpoint 위치

처음 디버깅할 때는 여기에 breakpoint를 건다.

```text
EquipmentStateMachine.cs
- Apply 메서드 첫 줄
- TryGetValue 줄
- MoveTo 줄
- Accept 줄
- Reject 줄
- MoveToAlarm 줄
```

### 9.2 테스트에서 따라가기

`tests/EquipmentTwin.Core.Tests/Program.cs`에서 아래 테스트 이름을 찾는다.

```text
NormalSequenceReachesComplete
InvalidTransitionIsRejected
DoorOpenedDuringInspectionCreatesAlarm
ClearAlarmReturnsToIdle
HistoryRecordsAcceptedAndRejectedTransitions
```

테스트를 실행하면 상태머신 breakpoint에 걸린다.

### 9.3 Watch 창에 넣을 값

Visual Studio Watch 창에 아래 값을 넣는다.

```text
CurrentState
equipmentEvent
previous
nextState
LastAlarm
History.Count
StateEnteredAtUtc
```

이 값을 보면서 Apply가 어떤 순서로 움직이는지 확인한다.

## 10. 이 파일을 수정할 때의 규칙

상태머신을 고칠 때는 아래 순서를 지킨다.

```text
1. 새 상태가 필요한가?
2. 새 이벤트가 필요한가?
3. AllowedTransitions에 전이를 추가해야 하는가?
4. 안전 이벤트인가, 일반 이벤트인가?
5. History에 어떤 메시지가 남아야 하는가?
6. 테스트 이름을 먼저 정할 수 있는가?
```

예를 들어 `Processing` 상태를 추가한다면:

```text
1. EquipmentState에 Processing 추가
2. EquipmentEvent에 ProcessComplete 추가
3. AllowedTransitions에서 Inspecting 대신 Processing 흐름 재설계
4. 테스트 추가
5. WPF 표시 이름 수정
```

상태 하나를 추가하는 것은 enum 한 줄 추가가 아니다.

흐름 전체를 바꾸는 일이다.

## 11. 이번 장에서 이해해야 하는 핵심

이 장을 읽고 아래 질문에 답할 수 있어야 한다.

```text
1. State와 Event의 차이는 무엇인가?
2. AllowedTransitions는 왜 dictionary인가?
3. Apply는 어떤 순서로 동작하는가?
4. Reject와 Accept는 왜 History에 모두 남기는가?
5. DoorOpened는 왜 전이표에 없어도 Alarmed로 가는가?
6. MoveTo가 StateEnteredAtUtc를 같이 바꾸는 이유는 무엇인가?
7. TransitionResult와 EquipmentTransition은 왜 둘 다 있는가?
```

이 질문에 답할 수 있으면 상태머신의 기본 구조는 이해한 것이다.
