# Goal 053: WPF HMI Visual System

Date: 2026-07-03
Branch: `goal/053-wpf-hmi-visual-system`

## Objective

Move the WPF HMI away from a colorful demo-app look and toward an `ISA-101 inspired` / `High-Performance HMI inspired` visual system.

The goal is not formal standard certification. The goal is to apply the practical operator-screen principles:

- mostly grayscale during normal operation;
- reserve red/amber for abnormal conditions;
- avoid color-only state encoding;
- keep overview, operation, and debug hierarchy understandable.

## Implemented

- Reworked WPF color tokens to a gray-base palette.
- Removed green/blue/yellow fills from normal operation indicators.
- Made normal progress bars neutral gray.
- Changed `NO ALARM` into neutral `NO ACTIVE ALARM`.
- Kept red/amber emphasis for alarm and warning conditions.
- Changed valve state from colored gas boxes to neutral boxes with explicit `OPEN` / `CLOSED` text.
- Added measurement status symbols such as `▲ HI` and `▼ COOL`.
- Added an alarm icon field so alarm state is encoded by shape/text as well as color.
- Added `docs/design/wpf-hmi-visual-system.md`.

## Library decision

No UI library was added in this goal.

Decision:

```text
Custom WPF design tokens first.
Chart/gauge library later only when trend/gauge cost becomes real.
```

Candidate libraries for later:

- LiveCharts2 for open-source WPF trends/gauges.
- Syncfusion WPF if richer controls are needed and licensing is acceptable.

## Validation

- `dotnet build src\EquipmentTwin.Hmi.Wpf\EquipmentTwin.Hmi.Wpf.csproj --configuration Release`
- `dotnet build EquipmentTwinLab.sln --configuration Release`
- `dotnet run --project tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj --no-restore --configuration Release`
- WPF startup smoke: app stayed alive for the startup window and did not exit immediately.

## Next recommended work

```text
Goal 054: WPF HMI Instrument Trend Panel
```

That goal can add a small trend area for pressure, temperature, and film thickness. Before adding a package, compare simple custom drawing versus LiveCharts2.
