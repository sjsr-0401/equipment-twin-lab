# Goal 054: Demo Color Restore and Junior Development Notes

Date: 2026-07-03
Branch: `goal/054-demo-color-restore-junior-notes`

## Objective

Restore the WPF HMI to a demo-friendly color direction and add a beginner-readable development history from the very beginning of the project.

## Why this changed

Goal 053 explored a strict gray-base industrial HMI direction.

The user clarified that this is still a portfolio demo, so normal states should keep the more readable green/blue visual language.

Decision:

```text
Demo readability first.
Strict HMI color discipline later, only if needed.
```

## Implemented

- Restored demo colors:
  - green for `NO ALARM`, ready/running, target;
  - blue for process progress;
  - gas/process colors for active valves and flow;
  - amber for warning/fault replay action;
  - red for alarm/stop.
- Kept the previous DataGrid readability fixes.
- Updated `docs/design/wpf-hmi-visual-system.md` to reflect the demo-first color decision.
- Added `docs/learning/development-from-zero.md`.

## Learning document scope

The new learning document starts from the beginning:

- why the project exists;
- why Core came before UI;
- why state machine and IO came first;
- why tests/CLI/CI were added;
- why Unity was tried;
- why WPF became the main HMI;
- how to think when modifying ViewModel/XAML/Core;
- terms to expand as the user asks questions.

## Validation

Passed in this branch:

- `git diff --check`
- `dotnet build EquipmentTwinLab.sln --configuration Release`
- `dotnet run --project tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj --no-restore --configuration Release`
  - 81 Core tests passed.
- WPF startup smoke:
  - `EquipmentTwin.Hmi.Wpf` stayed alive for the startup smoke window.

## Next recommended work

```text
Goal 055: Read MainWindow.xaml line by line
```

The next best learning task is to walk through the actual WPF screen file and annotate what each block does.
