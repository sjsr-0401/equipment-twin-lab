# Unity Demo Screenshot

Goal 030 adds a repeatable screenshot capture path for the Unity demo.

## Prerequisites

- Unity Hub is signed in.
- Unity Editor license is active.
- `.\scripts\Invoke-UnitySmokeTest.ps1` smoke test can run.

## Capture command

From the repository root:

```powershell
.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot
```

Default output:

```text
artifacts/unity-demo/moly-ald-demo.png
```

Tracked portfolio copy:

```text
docs/demo/moly-ald-demo.png
```

Custom output:

```powershell
.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot -ScreenshotPath artifacts\unity-demo\moly-ald-demo-v1.png
```

## Unity menu

Inside Unity:

```text
Equipment Twin > Capture Moly ALD Demo Screenshot
```

The command creates the demo scene if one does not exist, renders the active camera, and writes a PNG.

In screenshot mode the PowerShell runner intentionally does not pass `-nographics`, because `Camera.Render()` needs a graphics context.

## Expected markers

```text
EQUIPMENT_TWIN_UNITY_SMOKE_TEST_PASS
EQUIPMENT_TWIN_UNITY_SCREENSHOT_SAVED
```

## Portfolio usage

The representative screenshot should show:

- chamber body;
- wafer disk;
- film overlay;
- pressure/vacuum gauge;
- metal precursor, reactant, and purge valve indicators;
- metal precursor, reactant, and purge gas lines;
- title label;
- HUD text showing recipe, step, pressure, temperature, and thickness.

Do not claim this is a real equipment CAD model. It is a public/synthetic process-state visual demo.

## Verified local result

Verified on 2026-07-01 with Unity `6000.3.2f1`.

Confirmed markers:

```text
EQUIPMENT_TWIN_UNITY_SMOKE_TEST_PASS
EQUIPMENT_TWIN_UNITY_SCREENSHOT_SAVED
```

Representative image:

```text
docs/demo/moly-ald-demo.png
```

## 2026-07-01 polish update

Goal 033 improved the screenshot readability:

- camera framing and field of view were tightened;
- an equipment base plate was added;
- gas lines were added next to the valve indicators;
- a title label was added;
- the PowerShell runner now treats a blank `$LASTEXITCODE` after a successful Unity invocation as `0`.

## 2026-07-02 explanatory update

Goal 037 changed the screenshot from a render proof into a more self-explanatory portfolio image.

Added visual explanation elements:

- component labels: `Chamber`, `Wafer + film`, `Vacuum gauge`;
- gas/valve labels: `Precursor`, `Reactant ON`, `Purge`;
- status panel showing current step, cycle, film, and active valve;
- color key panel explaining precursor/reactant/purge/off colors;
- process flow bar with the current step highlighted;
- bottom note: `Moly ALD Replay | Core/CLI calculates the process. Unity only replays the timeline.`

The screenshot is still a public/synthetic process visualization. It is not a real equipment CAD model or vendor sequence.

## 2026-07-02 operator console layout update

Goal 039 changed the screenshot from a centered primitive equipment render into a user-friendly equipment console.

Added layout:

- left-side 3D equipment view;
- right-side operator interface;
- bottom process timeline and event summary.

Added equipment-like visual parts:

- synthetic cabinet;
- load port;
- wafer transfer arm;
- process chamber;
- wafer and film overlay;
- gas / valve panel;
- vacuum pump and exhaust line;
- alarm beacon.

Added operator UI parts:

- Start / Stop / Fault / Reset buttons;
- recipe card;
- live telemetry card;
- alarm card;
- event summary.

The design intentionally uses public-reference equipment styling only.
It does not copy a real vendor CAD model, real user interface, real chamber layout, or real process sequence.

## 2026-07-02 Canvas operator panel update

Goal 041 moved the operator panel and process timeline out of 3D `TextMesh` and into Unity Canvas UI.

What changed:

- right-side operator panel is now Canvas UI;
- current step is rendered as a large Canvas text value;
- live telemetry is rendered as structured Canvas text;
- alarm state has its own Canvas card;
- bottom process timeline uses Canvas chips and progress fill;
- `MolyAldPrimitiveVisualizer` keeps the 3D equipment body and equipment labels.

This keeps the architecture cleaner:

```text
3D scene = equipment body
Canvas UI = operator controls, telemetry, alarm, timeline
```

The buttons are still visual controls. Runtime click interaction is a later goal.

## 2026-07-02 HMI typography and instrument panel update

Goal 042 tightened the Canvas UI toward an industrial HMI reading model.

What changed:

- panel title changed from generic `Operator Interface` to `MOLY ALD HMI`;
- run/interlock state is now a dedicated strip;
- command colors now separate normal start, stop, fault, and reset intent;
- pressure, temperature, and film are shown as instrument rows with:
  - larger value/unit readout;
  - status label;
  - range track;
  - normal-range band;
  - actual-value fill;
- alarm card now includes priority and synthetic fault code fields;
- valve state remains in the event line so the instrument card stays focused on numeric process variables.

This is still not a real vendor HMI. The purpose is to show HMI principles: readable state, alarm priority, and process variable context.
