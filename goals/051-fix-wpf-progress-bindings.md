# Goal 051: Fix WPF ProgressBar Binding Mode

Date: 2026-07-03
Branch: `goal/051-fix-wpf-progress-bindings`

## Objective

Fix the Visual Studio debug exception raised when the WPF HMI starts.

The reported exception was:

```text
TwoWay or OneWayToSource binding cannot work on the read-only property 'TimelineProgress'
```

## Cause

`TimelineProgress`, `PressureProgress`, `TemperatureProgress`, and `FilmProgress` are calculated ViewModel properties.

They are intentionally read-only because the UI should only display these values. The UI must not write progress values back into the process model.

WPF `ProgressBar.Value` can attempt a two-way binding by default. That made the binding engine try to write into a read-only property, so Visual Studio stopped at runtime.

## Implemented

Updated the four WPF progress bars to use explicit one-way binding:

```text
Value="{Binding TimelineProgress, Mode=OneWay}"
Value="{Binding PressureProgress, Mode=OneWay}"
Value="{Binding TemperatureProgress, Mode=OneWay}"
Value="{Binding FilmProgress, Mode=OneWay}"
```

## Software architecture note

This keeps the intended MVVM direction:

```text
Core process state
  -> ViewModel calculated display properties
  -> WPF display controls
```

The reverse direction is only allowed for real user inputs, such as selected fault scenario or button commands.

Progress indicators are display-only outputs.

## Validation

- `git diff --check`
- `dotnet build src\EquipmentTwin.Hmi.Wpf\EquipmentTwin.Hmi.Wpf.csproj --configuration Release`
- `dotnet build EquipmentTwinLab.sln --configuration Release`
- `dotnet run --project tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj --no-restore --configuration Release`
- WPF startup smoke: app stayed alive for the startup window and did not exit immediately.

## User-facing explanation

If Visual Studio stops on a WPF binding exception, check whether a UI control is trying to write into a read-only ViewModel property.

For values that are calculated by software and only displayed on screen, use:

```text
Mode=OneWay
```

For values that the operator edits, such as selected scenario or text input, use two-way binding only when the ViewModel property has a setter.
