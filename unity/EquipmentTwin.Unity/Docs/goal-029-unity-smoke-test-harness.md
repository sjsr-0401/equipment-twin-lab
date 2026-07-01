# Goal 029: Unity Smoke Test Harness

## Goal

Make Unity validation repeatable after Unity Hub license activation.

## Added files

```text
Assets/EquipmentTwin/Editor/MolyAldEditorSmokeTest.cs
Assets/EquipmentTwin/Editor/EquipmentTwin.Unity.Editor.asmdef
scripts/Invoke-UnitySmokeTest.ps1
docs/unity-smoke-test.md
```

## What the test does

- loads `Assets/StreamingAssets/moly-ald-timeline.sample.json`;
- parses it through `MolyAldTimelineLoader`;
- creates a demo scene root;
- adds player, HUD, primitive visualizer, and bootstrap;
- forces primitive scene generation through `MolyAldPrimitiveVisualizer.EnsureScene()`;
- checks that primitive renderers exist;
- logs `EQUIPMENT_TWIN_UNITY_SMOKE_TEST_PASS`.

## Manual menu

```text
Equipment Twin > Run Moly ALD Smoke Test
Equipment Twin > Create Moly ALD Demo Scene
```

## Batch command

```powershell
.\scripts\Invoke-UnitySmokeTest.ps1
```

## Current blocker

If Unity Hub is not signed in or the license is inactive, batchmode fails before script compilation. That is expected and should be fixed by activating the Unity license, not by changing project code.
