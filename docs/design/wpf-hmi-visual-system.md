# WPF HMI Visual System

This document defines the visual direction for the WPF operator console.

## Current decision

The current project direction is **demo-first color**.

We reviewed ISA-101 / High-Performance HMI ideas, but we are not enforcing a strict industrial gray-base color rule right now.

Reason:

- this is a portfolio/demo HMI;
- the reviewer needs to understand the state quickly;
- green/blue normal-state signals make the demo easier to read;
- strict HMI color discipline can be revisited later if the project becomes more realistic.

This project does not claim ISA-101 certification or vendor UI equivalence.

## Demo color rules

Use color as a clear demo signal:

| Color | Meaning in this project |
|---|---|
| Green | normal, ready, target, no alarm |
| Blue | progress, active process flow, normal process signal |
| Amber | warning, caution, fault replay action |
| Red | alarm, stop, operator action required |
| Gray | inactive, background, structure, debug table |

This is intentionally more colorful than a strict high-performance industrial HMI.

## Still keep text readable

Even though this demo uses color, important states still need text.

Examples:

- valve boxes show `ON` / `OFF`;
- status labels show `OK`, `HI`, `COOL`, `TARGET`, or `GROWING`;
- alarm card shows title, priority, code, and icon;
- run state shows `RUNNING`, `PAUSED`, or `OPERATOR ACTION REQUIRED`.

## Why not use a full UI theme package yet?

Do not replace the whole WPF UI with a general-purpose modern app theme yet.

The base HMI look should remain custom because the design goal is an equipment software portfolio demo, not a generic desktop-app style.

Recommended path:

1. Keep custom WPF resource tokens for layout, colors, and typography.
2. Add chart/gauge libraries only where they reduce real implementation cost.
3. Evaluate `LiveCharts2` first for trend charts and simple gauges.
4. Evaluate `Syncfusion WPF` only if the project needs richer chart/gauge controls and the license conditions are acceptable.

Current decision:

```text
No chart/gauge dependency added yet.
```

## References to verify before dependency adoption

- ISA-101 overview: <https://www.isa.org/standards-and-publications/isa-standards/isa-101-standards>
- LiveCharts2 WPF docs: <https://livecharts.dev/docs/wpf/2.0.0-rc6.1/Overview.Installation>
- Syncfusion Community License: <https://www.syncfusion.com/products/communitylicense>

## Current implementation notes

Goal 054 restores the demo-friendly color strategy:

- normal progress bars use blue;
- `NO ALARM` uses green;
- `TARGET` uses green;
- valve active states use gas/process colors;
- alarm remains red;
- fault replay action remains amber;
- DataGrid readability fixes remain in place.

## Next design work

Suggested next design goal:

```text
Goal 055: WPF HMI Instrument Trend Panel
```

That goal should add a small pressure/temperature/film trend area and decide whether to draw it directly or use LiveCharts2.
