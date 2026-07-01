# Goal 028: Unity Chamber/Wafer/Valve Visual

## Objective

Add a simple but visible Unity 3D process visual for the public/synthetic molybdenum ALD timeline.

## Scope

- Auto-created primitive chamber visual
- Wafer disk visual
- Film thickness overlay visual
- Pressure/vacuum gauge visual
- Metal precursor / reactant / purge valve indicators
- One-component demo bootstrap
- Local Unity manual validation notes
- CI file-presence check for the new Unity scripts

## Architecture

```text
Core/CLI timeline JSON
    -> MolyAldProcessPlayer
    -> MolyAldPrimitiveVisualizer
    -> Unity primitive objects
```

`MolyAldPrimitiveVisualizer` is a display adapter. It does not calculate process state and does not decide PASS/FAIL.

## Validation plan

- .NET Release build
- Core console tests
- process CLI timeline generation
- sample Unity timeline JSON parse
- CI Unity script file-presence checks
- manual Unity Editor smoke test

## Known limitation

Unity Editor compile/play is still manual because this repository's CI does not run Unity Editor.

Local workstation note:

- Unity `6000.3.2f1` is installed.
- Batchmode compile was attempted but blocked by `No valid Unity Editor license found`.
- The next human validation step is Unity Hub sign-in/license activation, then Play Mode smoke test.
