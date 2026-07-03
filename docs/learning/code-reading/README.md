# Visual Studio 코드 독해 순서

이 폴더는 Visual Studio에서 실제 코드를 열어 놓고, 파일을 위에서 아래로 읽기 위한 문서다.

목표는 “이 프로젝트가 대충 뭘 하는지”가 아니다.

목표는 다음이다.

```text
코드를 직접 보면서
왜 이 줄이 있는지,
이 줄이 어떤 흐름에 연결되는지,
나중에 내가 어디를 고쳐야 하는지
이해한다.
```

## 먼저 알아야 할 원칙

이 프로젝트를 처음 읽을 때 WPF 화면부터 보면 안 된다.

화면은 결과다.

처음 봐야 하는 것은 장비 동작의 진실인 Core다.

읽는 순서:

```text
1. Core 상태머신
2. Clock/Timeout
3. 가상 IO
4. Cell Controller
5. Scenario Runner
6. Motion Axis
7. Template/Recipe
8. ALD Process Runner
9. WPF ViewModel
10. WPF XAML
11. CLI
12. Tests
```

## Visual Studio에서 여는 방법

1. Visual Studio에서 `EquipmentTwinLab.sln`을 연다.
2. Solution Explorer에서 `src/EquipmentTwin.Core`를 펼친다.
3. 아래 문서의 순서대로 파일을 연다.
4. 코드 왼쪽 줄 번호를 켜 둔다.
   - `Tools`
   - `Options`
   - `Text Editor`
   - `All Languages`
   - `Line numbers`
5. 문서와 코드를 같이 본다.

## 코드 독해 문서 목록

| 순서 | 문서 | 읽을 코드 |
|---:|---|---|
| 1 | [001-core-state-machine.md](./001-core-state-machine.md) | `EquipmentState`, `EquipmentEvent`, `TransitionResult`, `EquipmentTransition`, `EquipmentStateMachine` |

## 앞으로 추가할 문서

아래 문서는 아직 작성 대상이다.

| 순서 | 예정 문서 | 읽을 코드 |
|---:|---|---|
| 2 | `002-clock-timeout.md` | `IClock`, `ManualClock`, `SystemClock`, `StateTimeoutPolicy`, `TimeoutCheckResult` |
| 3 | `003-virtual-io.md` | `EquipmentIoMap`, `VirtualIoController`, IO record들 |
| 4 | `004-cell-controller.md` | `EquipmentCellController`, `EquipmentCellStepResult`, Alarm recovery |
| 5 | `005-scenario-runner.md` | `ScenarioRunner`, `EquipmentScenario`, `ScenarioStep` |
| 6 | `006-motion-axis.md` | `MotionAxis`, Motion state/result/alarm |
| 7 | `007-template-recipe.md` | `EquipmentTemplate`, `ProductRecipe`, `TemplateRunner` |
| 8 | `008-ald-process-runner.md` | `MolyAldRecipe`, `MolyAldRunner`, timeline records |
| 9 | `009-wpf-viewmodel.md` | `OperatorConsoleViewModel`, `RelayCommand`, `ObservableObject` |
| 10 | `010-wpf-xaml.md` | `MainWindow.xaml` |
| 11 | `011-cli-program.md` | `EquipmentTwin.Cli/Program.cs` |
| 12 | `012-tests.md` | `tests/EquipmentTwin.Core.Tests/Program.cs` |

## 읽을 때 표시할 것

Visual Studio에서 코드를 보면서 아래 표시를 직접 해두면 좋다.

- 상태를 나타내는 enum에는 “명사”라고 표시
- 이벤트를 나타내는 enum에는 “동사/신호”라고 표시
- `public` property에는 “외부에서 읽는 값”이라고 표시
- `private` field에는 “내부 상태”라고 표시
- `Apply`, `Poll`, `Run` 같은 method에는 “행동”이라고 표시
- `Result`, `Log`, `History` record에는 “증거/결과”라고 표시

## 독해 기준

코드를 읽을 때 계속 이 질문을 한다.

```text
1. 이 코드는 어떤 책임을 갖는가?
2. 누가 이 코드를 호출하는가?
3. 이 코드가 바꾸는 상태는 무엇인가?
4. 실패하면 어떤 결과를 반환하는가?
5. 테스트에서는 이 동작을 어떻게 증명하는가?
```

이 질문에 답할 수 있으면 그 코드는 “읽은 것”이다.
