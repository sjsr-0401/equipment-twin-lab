# Unity Smoke Test

This checklist validates the visible Unity side of the equipment twin.

## Why this exists

The .NET Core/CLI code can be tested in normal CI. Unity Editor compilation and Play Mode need Unity Hub, a valid Editor install, and an active Unity license.

Goal 029 adds a repeatable local smoke-test path so the visual demo does not depend on memory or manual guesswork.

## Prerequisites

- Unity Hub is installed.
- Unity Editor `6000.3.2f1` is installed.
- Unity Hub is signed in.
- Unity Editor license is active.

If the license is not active, batchmode can fail with:

```text
No valid Unity Editor license found
```

## Fast batch smoke test

From the repository root:

```powershell
.\scripts\Invoke-UnitySmokeTest.ps1
```

Expected success marker in the Unity log:

```text
EQUIPMENT_TWIN_UNITY_SMOKE_TEST_PASS
```

The script writes:

```text
unity/EquipmentTwin.Unity/Logs/codex-unity-smoke-test.log
```

## Capture demo screenshot

After the smoke test passes, capture a portfolio screenshot:

```powershell
.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot
```

Default output:

```text
artifacts/unity-demo/moly-ald-demo.png
```

Custom output:

```powershell
.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot -ScreenshotPath artifacts\unity-demo\my-demo.png
```

Expected screenshot marker:

```text
EQUIPMENT_TWIN_UNITY_SCREENSHOT_SAVED
```

## Manual Unity smoke test

If batchmode is blocked by licensing, use Unity Hub:

1. Open `unity/EquipmentTwin.Unity`.
2. Sign in / activate the Unity license if prompted.
3. In the Unity menu, run `Equipment Twin > Run Moly ALD Smoke Test`.
4. Confirm the Console logs `EQUIPMENT_TWIN_UNITY_SMOKE_TEST_PASS`.
5. In the Unity menu, run `Equipment Twin > Create Moly ALD Demo Scene`.
6. In the Unity menu, run `Equipment Twin > Capture Moly ALD Demo Screenshot`.
7. Press Play.
8. Confirm the generated chamber, wafer, film overlay, pressure gauge, and three valve indicators appear.

## What the smoke test checks

- sample timeline JSON exists;
- timeline JSON parses with the Unity loader;
- timeline contains at least one step and a recipe name;
- demo GameObject can be created;
- player, HUD, primitive visualizer, and bootstrap components are present;
- primitive renderer objects are generated.
- optional screenshot capture writes a PNG file.

## What it does not prove

- production-grade 3D model quality;
- real equipment physical accuracy;
- Unity build packaging;
- actual process physics.

Those are separate goals. This smoke test only proves that the current Unity visual layer can load and generate the demo scene.
