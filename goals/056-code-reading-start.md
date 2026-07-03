# Goal 056: Visual Studio Code Reading Start

Date: 2026-07-03
Branch: `goal/056-code-reading-start`

## Objective

Start a code-reading documentation set for understanding the project directly inside Visual Studio.

The user clarified that they want to open code from the beginning and understand the flow and individual lines, even if the explanation becomes long.

## Implemented

- Added `docs/learning/code-reading/README.md`.
- Added `docs/learning/code-reading/001-core-state-machine.md`.
- Linked the new code-reading documents from `docs/learning/README.md`.
- Added CI checks to keep the first code-reading documents present.

## First reading scope

The first detailed guide covers:

- `EquipmentState.cs`
- `EquipmentEvent.cs`
- `TransitionResult.cs`
- `EquipmentTransition.cs`
- `EquipmentStateMachine.cs`

This is the correct starting point because the state machine is the first Core concept behind the whole project.

## Why not start from WPF

WPF is the visible screen, but it is not the source of truth.

The project should be read from:

```text
Core state machine
  -> clock/timeout
  -> virtual IO
  -> controller
  -> scenario/CLI
  -> process runner
  -> WPF ViewModel
  -> WPF XAML
```

## Validation

Passed in this branch:

- `git diff --check`
- `dotnet build EquipmentTwinLab.sln --configuration Release`
- `dotnet run --project tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj --no-restore --configuration Release`
  - 81 Core tests passed.

## Next recommended work

```text
Goal 057: Clock and timeout line-by-line reading note
```

Reason:

The state machine records state entry time and checks timeout through `IClock`, `ManualClock`, and `StateTimeoutPolicy`. Those files are the natural next reading step.
