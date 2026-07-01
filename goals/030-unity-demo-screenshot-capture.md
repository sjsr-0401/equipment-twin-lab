# Goal 030: Unity Demo Screenshot Capture

## Objective

Add a repeatable screenshot capture path for the Unity ALD demo.

## Scope

- batch screenshot capture mode;
- Unity menu screenshot capture;
- screenshot output documentation;
- CI file/wiring checks;
- daily log and loop-state updates.

## Command

```powershell
.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot
```

## Default output

```text
artifacts/unity-demo/moly-ald-demo.png
```

## Validation boundary

The repository can validate script syntax, file wiring, and .NET regression. Actual screenshot generation requires Unity Hub sign-in/license activation.
