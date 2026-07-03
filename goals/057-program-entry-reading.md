# Goal 057: Program Entry Reading Guide

Date: 2026-07-03
Branch: `goal/057-program-entry-reading`

## Objective

Correct the code-reading sequence so it starts from the real Visual Studio execution entry point, not from the Core state machine.

## User correction

The user clarified:

```text
처음 프로그램 진입점부터 싹다 해달라니까 왜 상태머신만해?
```

The previous state-machine guide is still useful, but it was not the correct first document for Visual Studio F5 debugging.

## Implemented

- Added `docs/learning/code-reading/000-program-entry.md`.
- Updated `docs/learning/code-reading/README.md` so the reading order starts at program entry.
- Updated `docs/learning/code-reading/001-core-state-machine.md` to clarify that it is the Core logic starting point, not the program execution entry point.
- Added CI checks for the program-entry reading guide.

## Program-entry scope

The new guide covers:

- `EquipmentTwin.Hmi.Wpf.csproj`
- generated WPF `App.g.cs` `Main()`
- `App.xaml`
- `App.xaml.cs`
- `MainWindow.xaml`
- `MainWindow.xaml.cs`
- `OperatorConsoleViewModel`
- `RelayCommand`
- `ObservableObject`
- `MolyAldRecipeService`
- initial normal timeline loading
- START / FAULT REPLAY / RESET flow
- WPF, CLI, and Tests entry-point differences

## Validation

Passed in this branch:

- `git diff --check`
- `dotnet build EquipmentTwinLab.sln --configuration Release`
- `dotnet run --project tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj --no-restore --configuration Release`
  - 81 Core tests passed.

## Next recommended work

```text
Goal 058: Continue from program entry into OperatorConsoleViewModel line-by-line
```

Reason:

The new guide fixes the top-level execution path. The next step should continue deeper into `OperatorConsoleViewModel` because it is the first large file reached after `MainWindow`.
