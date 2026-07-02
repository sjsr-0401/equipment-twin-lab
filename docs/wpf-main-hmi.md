# WPF Main HMI

WPF is now the primary operator-console direction for this project.

Unity remains available as an optional 3D/replay viewer, but the main development/debugging surface is WPF because it is closer to a practical equipment HMI workflow:

- Visual Studio `F5` runs the app directly.
- Breakpoints work while clicking HMI buttons.
- Core process logic stays in `.NET` class libraries.
- The UI can be explained as a normal equipment software/HMI architecture.

## Project

```text
src/EquipmentTwin.Hmi.Wpf
```

Open the solution:

```text
EquipmentTwinLab.sln
```

Set this project as the startup project:

```text
EquipmentTwin.Hmi.Wpf
```

Then press:

```text
F5
```

## Current UI

The WPF shell includes:

- process schematic panel;
- Start / Stop / Step / Fault Replay / Reset commands;
- fault scenario selector;
- pressure, temperature, and film thickness instruments;
- alarm card;
- operator action log;
- timeline debug table.

## Current data flow

```text
WPF HMI button
  -> OperatorConsoleViewModel
  -> EquipmentTwin.Core.Processes.MolyAldRunner
  -> MolyAldTimelineDocument
  -> WPF bindings update the HMI
```

The important boundary:

- WPF is the operator screen.
- Core is the process truth.
- Unity is optional visualization.

## Fault replay flow

```text
Select fault scenario
  -> press FAULT REPLAY
  -> MolyAldRunner.Run(recipe, selectedFaultScenario)
  -> first failed step becomes current step
  -> alarm card and schematic turn red
```

The currently available public/synthetic fault scenarios are:

- `pumpdown-timeout`
- `temperature-not-stable`
- `precursor-dose-timeout`
- `purge-timeout`

## Optional Unity viewer

The WPF app includes an `Open optional Unity viewer folder` button.

This does not embed Unity into WPF. That is intentional. Embedding Unity would add unnecessary complexity. The first practical boundary is:

```text
WPF = main HMI
Unity = optional separate 3D viewer
```

Later, the WPF app can launch a built Unity viewer executable and pass it a timeline JSON path.

## Debugging recommendations

Useful breakpoints:

- `OperatorConsoleViewModel.Start`
- `OperatorConsoleViewModel.FaultReplay`
- `OperatorConsoleViewModel.Reset`
- `MolyAldRunner.Run`
- `MolyAldTimelineDocument.FromRunResult`

Use WPF when you want to debug the operator/HMI flow. Use Core tests when you want to debug low-level process rules.

## Binding rule used in this HMI

Progress values are calculated by the ViewModel and displayed by WPF.

Therefore progress bars must use one-way binding:

```text
Value="{Binding TimelineProgress, Mode=OneWay}"
```

Use this rule when adding more HMI indicators:

- display-only calculated value: `Mode=OneWay`
- operator input/selection: `Mode=TwoWay` only when the ViewModel property has a setter
- operator action: `Command`

## UI readability rule

Do not rely on default WPF control colors inside the dark HMI theme.

Controls such as `DataGrid`, `ComboBox`, and future `TabControl` panels need explicit styles for:

- header background;
- cell foreground;
- selected row color;
- border and grid-line color;
- row height and spacing.

If a new control appears with a white default background, treat it as a UI bug and add a local style before adding more features.
