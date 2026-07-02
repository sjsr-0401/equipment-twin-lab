# Goal 050: WPF Operator Console Shell

Date: 2026-07-03
Branch: `goal/050-wpf-operator-console-shell`

## Objective

Move the main operator-console direction from Unity-first to WPF-first.

Unity is not removed. It becomes an optional 3D/replay viewer. The main app for daily debugging, HMI interaction, and portfolio explanation is now WPF because it runs directly from Visual Studio with `F5`.

## Implemented

- Added `src/EquipmentTwin.Hmi.Wpf`.
- Registered `EquipmentTwin.Hmi.Wpf` in `EquipmentTwinLab.sln`.
- Added a WPF main window with:
  - process schematic;
  - Start / Stop / Step / Fault Replay / Reset buttons;
  - fault scenario selector;
  - pressure, temperature, and film instruments;
  - alarm card;
  - operator action log;
  - timeline debug table.
- Connected WPF directly to `EquipmentTwin.Core`.
- Added `scripts/Invoke-WpfHmi.ps1` for PowerShell launch.
- Added `docs/wpf-main-hmi.md`.

## Architecture

```text
EquipmentTwin.Hmi.Wpf
  -> OperatorConsoleViewModel
  -> MolyAldRecipeService
  -> EquipmentTwin.Core.Processes.MolyAldRunner
  -> MolyAldTimelineDocument
  -> WPF data binding
```

The important separation:

- WPF is the main operator console.
- Core remains the process truth.
- CLI remains automation/report tooling.
- Unity remains optional visualization.

## Debug value

The user can now:

- set `EquipmentTwin.Hmi.Wpf` as startup project;
- press `F5`;
- click HMI buttons;
- put breakpoints in `OperatorConsoleViewModel` and `MolyAldRunner`;
- debug the operator flow without learning Unity scene setup first.

## Fault replay behavior

WPF uses the selected fault scenario and calls:

```text
MolyAldRunner.Run(recipe, selectedFaultScenario)
```

Then it converts the result to:

```text
MolyAldTimelineDocument.FromRunResult(result)
```

The first failed step becomes the current step and the HMI enters alarm state.

## Validation

- `git diff --check`
- `dotnet build src\EquipmentTwin.Hmi.Wpf\EquipmentTwin.Hmi.Wpf.csproj --configuration Release`
- `dotnet build EquipmentTwinLab.sln --configuration Release`
- `dotnet run --project tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj --no-restore --configuration Release`

## Known boundary

- WPF currently renders a practical schematic, not vendor CAD.
- Unity is opened as an optional folder/viewer path only; it is not embedded in WPF.
- A later goal can build a Unity viewer executable and have WPF launch it with a timeline path.
