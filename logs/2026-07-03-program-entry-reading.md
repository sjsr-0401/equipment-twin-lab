# 2026-07-03 프로그램 진입점 독해 문서 작업 로그

## 사용자 correction

사용자는 이전 작업이 원하는 시작점과 다르다고 지적했다.

이전 작업:

```text
Core 상태머신부터 설명
```

사용자가 원한 것:

```text
Visual Studio에서 F5를 눌렀을 때 프로그램이 어디서 시작해서
어떤 코드 순서로 화면까지 도달하는지
진입점부터 싹 다 설명
```

## 판단

사용자 지적이 맞다.

`EquipmentStateMachine`은 Core 로직의 출발점이지, 프로그램 실행의 출발점은 아니다.

WPF 실행 기준 실제 시작점은 빌드가 생성하는 `App.g.cs`의 `Main()`이다.

## 변경

추가:

```text
docs/learning/code-reading/000-program-entry.md
```

수정:

```text
docs/learning/code-reading/README.md
docs/learning/code-reading/001-core-state-machine.md
```

## 새 문서가 설명하는 흐름

```text
Visual Studio F5
  -> WPF project build
  -> generated App.g.cs Main()
  -> App.InitializeComponent()
  -> App.xaml StartupUri
  -> MainWindow.xaml
  -> MainWindow.xaml.cs constructor
  -> InitializeComponent()
  -> DataContext = new OperatorConsoleViewModel()
  -> OperatorConsoleViewModel constructor
  -> Commands
  -> DispatcherTimer
  -> LoadRecipeAndTimeline()
  -> MolyAldRecipeService
  -> MolyAldRunner(Core)
  -> Timeline
  -> XAML Binding
```

## 소프트웨어적으로 중요한 점

실행 진입점과 Core 설계 시작점은 다르다.

```text
실행 진입점 = 프로그램이 실제로 처음 실행되는 위치
설계 시작점 = 프로젝트 로직을 이해하기 좋은 개념적 시작점
```

이번 사용자는 실행 진입점을 원했다.

그래서 코드 독해 순서를 `000-program-entry.md`부터 시작하도록 고쳤다.

## 다음 작업

다음에는 `OperatorConsoleViewModel.cs`를 더 깊게 들어가는 것이 맞다.

이유:

```text
WPF 실행 흐름에서 MainWindow 다음으로 바로 도달하는 큰 파일이 OperatorConsoleViewModel이기 때문이다.
```

그 다음에 Core 상태머신/ALD Runner로 내려가는 순서가 더 자연스럽다.

## 검증 결과

아래 검증을 통과했다.

```powershell
git diff --check
dotnet build EquipmentTwinLab.sln --configuration Release
dotnet run --project tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj --no-restore --configuration Release
```

결과:

- Release 빌드 통과
- 빌드 경고 0개
- 빌드 오류 0개
- Core 테스트 81개 통과
