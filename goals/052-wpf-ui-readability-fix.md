# Goal 052: WPF UI Readability Fix

Date: 2026-07-03
Branch: `goal/052-wpf-ui-readability-fix`

## Objective

Fix the visible WPF UI readability problems found during manual Visual Studio debugging.

The immediate issues were:

- `LOAD PORT` text on the left schematic was clipped and hard to read.
- `PROCESS TIMELINE` and the percentage value were visually cramped.
- The timeline debug table used default WPF header styling, producing a white header/background conflict inside the dark HMI theme.
- Some default controls still looked like raw Windows controls instead of belonging to the HMI design system.

## Implemented

- Added shared dark-theme tokens:
  - `SurfaceInset`
  - `BorderSoft`
  - `PrimaryDim`
- Added explicit WPF styles for:
  - `ComboBox`
  - `ComboBoxItem`
  - `DataGridColumnHeader`
  - `DataGridRow`
  - `DataGridCell`
- Enlarged the left load-port schematic block so the label is visible.
- Replaced the process timeline header `DockPanel` with a two-column `Grid` so the progress percentage does not collide with the label.
- Increased the bottom debug-table area height.
- Added explicit DataGrid borders, grid lines, row height, and column header height.

## Design note

This is a readability fix, not the final visual design pass.

The current WPF screen still needs a larger visual-system pass later:

- typography scale;
- spacing rhythm;
- HMI color semantics;
- industrial dashboard layout;
- instrument-card hierarchy;
- optional light/dark theme tokens;
- screenshot comparison before and after changes.

## Validation

- `dotnet build src\EquipmentTwin.Hmi.Wpf\EquipmentTwin.Hmi.Wpf.csproj --configuration Release`
- `git diff --check`

## Next recommended work

```text
Goal 053: WPF HMI Visual System R&D
```

That goal should research and define a more professional operator-console style before we keep adding more WPF panels.
