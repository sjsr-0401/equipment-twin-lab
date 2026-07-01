# EquipmentTwin.Unity

Unity-side skeleton for replaying Equipment Twin process timelines.

This folder is intentionally lightweight. It does not try to generate a complete Unity scene yet. It gives Unity scripts that can be copied into or opened from a Unity project and wired to a simple GameObject.

## Goal 027 scope

- Read `equipment-twin.moly-ald.timeline.v1` JSON.
- Advance through process steps over time.
- Expose current step, chamber pressure, wafer temperature, valve states, and film thickness.
- Draw a minimal runtime HUD using Unity `OnGUI`.

## Quick Unity setup

1. Open Unity Hub.
2. Create a 3D project or open `unity/EquipmentTwin.Unity` as a Unity project folder.
3. Create an empty GameObject named `MolyAldProcessPlayer`.
4. Add these components:
   - `MolyAldProcessPlayer`
   - `MolyAldProcessHud`
   - `MolyAldPrimitiveVisualizer`
5. Assign a timeline JSON as a `TextAsset`, or place a JSON file under `Assets/StreamingAssets/`.

Sample file:

```text
Assets/StreamingAssets/moly-ald-timeline.sample.json
```

## Script responsibility

```text
MolyAldTimelineData
    JSON DTO classes matching the Core timeline schema

MolyAldTimelineLoader
    JSON loading/parsing helpers

MolyAldProcessPlayer
    Playback state machine for timeline steps

MolyAldProcessHud
    Minimal debug UI rendered with OnGUI

MolyAldPrimitiveVisualizer
    Auto-created primitive chamber/wafer/valve/pressure visuals

MolyAldDemoBootstrap
    Optional one-component setup helper for player, HUD, visualizer, camera, and light
```

## Fastest visual setup

1. Create one empty GameObject.
2. Add only `MolyAldDemoBootstrap`.
3. Press Play.

The bootstrap component adds the player, HUD, primitive visualizer, camera, and directional light automatically.

Primitive mapping:

| Timeline value | Visual |
|---|---|
| Chamber pressure | chamber color, vacuum column, gauge needle |
| Wafer temperature | wafer color |
| Metal precursor valve | orange valve sphere |
| Reactant valve | blue valve sphere |
| Purge valve | green valve sphere |
| Film thickness | film overlay disk size/color |

## Smoke test

After Unity Hub sign-in/license activation, run from the repository root:

```powershell
.\scripts\Invoke-UnitySmokeTest.ps1
```

Or use the Unity menu:

```text
Equipment Twin > Run Moly ALD Smoke Test
Equipment Twin > Create Moly ALD Demo Scene
```

Expected marker:

```text
EQUIPMENT_TWIN_UNITY_SMOKE_TEST_PASS
```

Screenshot capture:

```powershell
.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot
```

Manual menu:

```text
Equipment Twin > Capture Moly ALD Demo Screenshot
```

## Design boundary

This Unity skeleton is a visual replay of public/synthetic software-state data. It is not a physics simulation and does not represent real equipment internals.
