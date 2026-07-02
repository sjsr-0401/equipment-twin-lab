# WPF HMI Visual System

This document defines the visual direction for the WPF operator console.

The project uses an `ISA-101 inspired` and `High-Performance HMI inspired` approach. It does not claim certification or vendor equivalence.

## Goal

Make the WPF HMI look and behave like a practical equipment-operator screen:

- calm during normal operation;
- obvious during abnormal operation;
- readable without relying on color alone;
- explainable in an interview as a deliberate engineering decision, not decoration.

## Core principles

### 1. Gray base first

Normal operation should be mostly grayscale.

The base palette is:

| Token | Role |
|---|---|
| `Background` | full window background |
| `Surface` | main panel background |
| `SurfaceRaised` | cards, controls, idle components |
| `SurfaceActive` | active-but-normal component state |
| `Border`, `BorderSoft` | structure and table lines |
| `TextPrimary`, `TextMuted` | normal text hierarchy |
| `NeutralSignal` | normal progress/measurement fill |

### 2. Color is reserved for attention

Color is not used to make normal operation look exciting.

| Color role | Meaning |
|---|---|
| Red / `Alarm` | alarm, unsafe, operator must act |
| Amber / `Warning` | warning, caution, not-yet-normal value |
| Muted blue / `ActionBlue` | primary available operator action |
| Gray / neutral | normal, idle, complete, inactive |

### 3. Do not encode state with color only

Every important state must also have text or shape.

Examples in the current WPF HMI:

- valves show `OPEN` / `CLOSED`, not only colored boxes;
- pressure and temperature use `▲ HI`, `▼ COOL`, `OK`;
- alarm card uses color, label, priority, code, and icon;
- run state uses text such as `RUNNING`, `PAUSED`, and `OPERATOR ACTION REQUIRED`.

### 4. Keep level hierarchy clear

The current WPF screen acts as a Level 1/Level 2 hybrid:

- left: process overview schematic;
- right: operation and current state;
- bottom: debug table for engineering verification.

Future work should split this more cleanly:

- Level 1: overview / abnormality-first status;
- Level 2: equipment module detail;
- Level 3: engineering/debug table;
- Level 4: raw log/config.

## Library decision

Do not replace the whole WPF UI with a general-purpose modern app theme.

The base HMI look should remain custom because the design goal is industrial operator readability, not a generic desktop-app style.

Recommended path:

1. Keep custom WPF resource tokens for layout, colors, and typography.
2. Add chart/gauge libraries only where they reduce real implementation cost.
3. Evaluate `LiveCharts2` first for trend charts and simple gauges.
4. Evaluate `Syncfusion WPF` only if the project needs richer chart/gauge controls and the license conditions are acceptable.

Current decision:

```text
No chart/gauge dependency added in Goal 053.
```

## References to verify before dependency adoption

- ISA-101 overview: <https://www.isa.org/standards-and-publications/isa-standards/isa-101-standards>
- LiveCharts2 WPF docs: <https://livecharts.dev/docs/wpf/2.0.0-rc6.1/Overview.Installation>
- Syncfusion Community License: <https://www.syncfusion.com/products/communitylicense>

## Current implementation notes

Goal 053 changes the WPF shell from colorful demo UI to restrained HMI UI:

- normal progress bars are neutral gray;
- `NO ACTIVE ALARM` is neutral, not green;
- alarm state remains red/amber;
- valve state uses text and neutral active state;
- measurement status uses symbols plus text;
- normal `OK TARGET` is not green;
- action buttons use subdued borders rather than large saturated fills.

## Next design work

Suggested next design goal:

```text
Goal 054: WPF HMI Instrument Trend Panel
```

That goal should decide whether to add LiveCharts2 for small pressure/temperature/film trend charts, or to draw simple custom mini-trends first.
