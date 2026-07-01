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
```

## Design boundary

This Unity skeleton is a visual replay of public/synthetic software-state data. It is not a physics simulation and does not represent real equipment internals.
