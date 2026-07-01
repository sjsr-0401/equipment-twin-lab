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

The first useful screenshot should show:

- chamber body;
- wafer disk;
- film overlay;
- pressure/vacuum gauge;
- metal precursor, reactant, and purge valve indicators;
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
