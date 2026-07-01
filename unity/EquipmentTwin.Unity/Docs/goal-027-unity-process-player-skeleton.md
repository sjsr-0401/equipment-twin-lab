# Goal 027: Unity Process Player Skeleton

## What this adds

This goal adds the first Unity-side code for the equipment twin.

It does not create a polished 3D simulator yet. It creates the minimum runtime skeleton that can read a timeline JSON and display the current ALD process state.

## Data flow

```text
dotnet CLI
  -> artifacts/moly-ald-timeline.json
  -> Unity TextAsset or StreamingAssets file
  -> MolyAldTimelineLoader
  -> MolyAldProcessPlayer
  -> MolyAldProcessHud
```

## Why this step matters

Unity should be a viewer/player for the tested C# process model, not a separate source of truth.

The C# Core owns process correctness. Unity reads the timeline and shows:

- current process step
- cycle number
- chamber pressure
- wafer temperature
- precursor/reactant/purge valve states
- estimated film thickness
- fault/final state

## Next goal

`Goal 028: Unity Chamber/Wafer/Valve Visual`

Replace or supplement the `OnGUI` HUD with simple 3D primitives:

- chamber cylinder
- wafer disk
- gas/valve indicators
- film thickness color/scale overlay
- alarm panel
