# Goal 030: Unity Demo Screenshot Capture

## Goal

Add a repeatable way to capture the first Unity demo screenshot after Unity license activation.

## Added behavior

```text
Invoke-UnitySmokeTest.ps1 -CaptureScreenshot
    -> RunBatchScreenshotCapture
    -> smoke test
    -> camera render
    -> PNG output
```

## Default output

```text
artifacts/unity-demo/moly-ald-demo.png
```

## Unity menu

```text
Equipment Twin > Capture Moly ALD Demo Screenshot
```

## Current limitation

The capture path requires Unity Editor license activation. Without that, Unity exits before project scripts run.
