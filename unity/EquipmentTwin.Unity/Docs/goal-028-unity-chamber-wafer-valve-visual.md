# Goal 028: Unity Chamber/Wafer/Valve Visual

## Goal

Add the first visible 3D layer for the public/synthetic molybdenum ALD timeline.

This is not a polished equipment model. It is a software-debuggable visual adapter that proves timeline state can drive Unity objects.

## Added scripts

```text
MolyAldPrimitiveVisualizer
    Reads MolyAldProcessPlayer.CurrentStep and updates primitive visuals.

MolyAldDemoBootstrap
    Adds player, HUD, primitive visualizer, camera, and light from one component.
```

## Visual mapping

| Timeline field | Visual behavior |
|---|---|
| `chamberPressureMtorr` | chamber color, vacuum column height, gauge needle angle |
| `waferTemperatureC` | wafer color from cold gray to hot orange |
| `estimatedThicknessAngstrom` | film overlay disk grows and changes color |
| `valves.metalPrecursor` | precursor valve sphere turns orange and grows |
| `valves.reactant` | reactant valve sphere turns blue and grows |
| `valves.purge` | purge valve sphere turns green and grows |
| `success` | chamber turns red when timeline/step reports failure |

## Manual Unity test

1. Open `unity/EquipmentTwin.Unity` in Unity Hub.
2. Create one empty GameObject.
3. Add `MolyAldDemoBootstrap`.
4. Press Play.
5. Confirm primitives update as the timeline advances.

## Current limitation

GitHub Actions does not run Unity Editor, so CI checks file presence and .NET regression only. Unity compile/play remains a local manual check.

Local Unity note:

- Unity `6000.3.2f1` is installed on the workstation.
- Batchmode compile was attempted, but Unity reported `No valid Unity Editor license found`.
- Activate/sign in through Unity Hub before doing the Play Mode smoke test.
