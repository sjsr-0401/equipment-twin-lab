# Operator Console Design System

## 목적

Unity operator console을 “예쁘게 보이는 화면”이 아니라 유지보수 가능한 HMI UI로 만들기 위한 최소 디자인 시스템이다.

다음 구현 Goal부터는 이 문서를 기준으로 판단한다.

## Layout Contract

권장 screenshot 기준: `1280 x 720`

```text
┌──────────────────────────────────────────────┬────────────────────────────┐
│                                              │ Operator Panel             │
│  3D Equipment View                           │                            │
│  - cabinet                                   │ Recipe / Current Step      │
│  - load port                                 │ Telemetry                  │
│  - chamber / wafer / film                    │ Alarm                      │
│  - gas / valve / pump                        │ Commands                   │
├──────────────────────────────────────────────┴────────────────────────────┤
│  Process Timeline + Event Log                                             │
└───────────────────────────────────────────────────────────────────────────┘
```

비율:

- Left equipment view: 58-62%
- Right operator panel: 38-42%
- Bottom timeline: 12-16% height

## Color Tokens

| Token | Purpose | Suggested Hex |
|---|---|---|
| `background` | app background | `#0B0F14` |
| `surface` | panel/card | `#151C24` |
| `surfaceRaised` | active card | `#1D2733` |
| `border` | panel border | `#2F3C4C` |
| `textPrimary` | main text | `#EAF0F7` |
| `textMuted` | secondary text | `#9AA8B7` |
| `primary` | active process / selected step | `#2EA8FF` |
| `success` | ready / nominal | `#2AD17D` |
| `warning` | caution / hold | `#FFB84D` |
| `alarm` | fault / unsafe | `#FF3B3B` |
| `precursor` | precursor gas | `#F7A83B` |
| `reactant` | reactant gas | `#38CFFF` |
| `purge` | purge gas | `#31D86B` |

규칙:

- `alarm`은 fault/alarm 외에는 쓰지 않는다.
- `success`는 정상/ready 외에는 쓰지 않는다.
- gas 색상은 장비 line과 UI legend에서 동일하게 쓴다.
- background/surface 계열은 밝기를 많이 올리지 않는다. 값과 alarm이 튀어야 한다.

## Typography Scale

| Role | Use | Target Size |
|---|---|---:|
| App title | `Synthetic Moly ALD Module` | 30-34 |
| Panel title | `Operator Interface` | 24-28 |
| Section header | `RECIPE`, `LIVE`, `ALARM` | 15-18 |
| Primary value | current step, alarm state | 22-28 |
| Telemetry value | pressure/temp/film | 18-22 |
| Body | labels and hints | 13-15 |
| Caption | architecture note | 11-12 |

규칙:

- 현재 step은 body text로 쓰지 않는다. 반드시 primary value로 보이게 한다.
- alarm text는 section header보다 커야 한다.
- screenshot 50% 축소에서도 current step과 alarm state는 읽혀야 한다.

## Component Contract

### Operator Panel

필수 영역:

- Current recipe
- Current step
- Run state
- Alarm state
- Telemetry values
- Command buttons

### Instrument Card

값은 label, value/unit readout, status, range track을 분리한다.

```text
PRESSURE   850  mTorr   OK      normal 800-900
TEMP       250  C       OK      normal 245-255
FILM       2    A       TARGET  target 100%
```

규칙:

- 숫자는 우측 정렬한다.
- 단위는 값 readout 안에 붙이되 status/range와는 분리한다.
- 정상 범위는 track band로 보여준다.
- 현재값은 fill로 보여준다.
- valve state는 event line에 남기고 numeric instrument card를 과밀하게 만들지 않는다.

### Alarm Card

정상:

```text
NO ALARM
Interlocks nominal
```

알람:

```text
ALARM ACTIVE
Gas delivery fault
Hold sequence
```

### Timeline

필수:

- step chip list
- active step highlight
- progress fill
- optional event summary

## Canvas Operator Panel Migration Plan

현재 문제:

- `TextMesh`는 3D 공간에 존재해서 글자 품질과 정렬이 약하다.
- operator panel이 perspective 영향을 받아 professional HMI처럼 보이지 않는다.

다음 구현 방향:

```text
Unity 3D Scene
    -> equipment body only

Unity Canvas
    -> right operator panel
    -> bottom timeline
    -> text/button/card components
```

권장 Canvas:

- Render mode: `Screen Space - Camera`
- Reference resolution: `1280 x 720`
- Scale mode: `Scale With Screen Size`
- Match: `0.5`

## Goal 041 Acceptance Criteria

- [x] 오른쪽 operator panel은 Canvas 기반이다.
- [x] TextMesh 기반 UI text는 장비 label 수준으로만 남긴다.
- [x] current step, telemetry, alarm, timeline은 Canvas text로 표시된다.
- [x] 버튼은 Canvas button-like UI element로 구성된다.
- [x] 색상은 이 문서의 token 이름으로 설명 가능하다.
- [x] screenshot 50% 축소 상태에서도 current step과 alarm state가 읽힌다.

## Goal 042 Acceptance Criteria

```text
Goal 042: HMI Typography and Instrument Panel
```

- [x] HMI title, run state strip, command buttons의 typography hierarchy가 분리된다.
- [x] Pressure/Temp/Film 값은 value/unit readout, 상태, 정상범위, fill로 분리된다.
- [x] 숫자는 작은 row에서도 읽히도록 우측 정렬하고 truncation 문제를 피한다.
- [x] Alarm card는 priority와 synthetic fault code 영역을 가진다.
- [x] screenshot에서 instrument value가 직접 읽힌다.

## Goal 043 Candidate

```text
Goal 043: Process Schematic Main View
```

Next acceptance criteria:

- [x] 왼쪽 메인 view는 primitive 3D가 아니라 process schematic이다.
- [x] schematic은 chamber, showerhead, wafer/film, heater, gas valves, exhaust, pump를 연결된 시스템으로 보여준다.
- [x] active valve state가 valve/line color에 반영된다.
- [x] film thickness ratio가 wafer film fill에 반영된다.
- [x] 실제 vendor CAD/UI/process를 복제하지 않는 reference boundary 문서가 있다.

## Goal 044 Acceptance Criteria

```text
Goal 044: Gas Flow Animation and Fault Highlight
```

- [x] Reactant/Precursor/Purge active valve state drives gas pulse color.
- [x] Active gas pulses move from valve path toward chamber/showerhead.
- [x] Showerhead gas dots pulse while a gas valve is open.
- [x] Pump/exhaust pulse path is available for pump/purge/fault states.
- [x] Fault state can blink chamber/exhaust/gate/pump highlight.
- [x] Screenshot shows `FLOW: Reactant pulse -> chamber`.

## Goal 045 Acceptance Criteria

```text
Goal 045: Canvas Button Interaction and Fault Selector
```

- [x] START button restarts and plays the timeline.
- [x] STOP button pauses the timeline.
- [x] RESET button returns to the first step.
- [x] FAULT control toggles a synthetic operator fault scenario.
- [x] Unity smoke test validates button creation, EventSystem creation, and player state transitions.

Implementation boundary:

- FAULT is currently an operator override, not a real process fault selected from a JSON fault matrix.
- START does not clear a fault. RESET or FAULT toggle must clear the held state first.
- This keeps the HMI behavior closer to equipment safety expectations.

## Goal 046 Acceptance Criteria

```text
Goal 046: Fault Mode Screenshot and Operator Action Log
```

- [x] Capture a fault-mode screenshot that shows red alarm card and schematic fault highlight.
- [x] Add an operator action log for START/STOP/FAULT/RESET.
- [x] Show `normal run -> fault hold` in demo artifacts.
- [x] Document how the synthetic override differs from process-runner fault scenarios.
- [x] Unity smoke script supports `-CaptureFaultScreenshot`.

Implemented artifacts:

- `docs/demo/moly-ald-demo.png`
- `docs/demo/moly-ald-demo-fault.png`

Implementation boundary:

- The action log is an in-memory Canvas log.
- The fault screenshot uses synthetic `OperatorFaultActive`.
- It does not yet select a named JSON fault scenario from the process fault matrix.

## Goal 047 Candidate

```text
Goal 047: Reset Recovery Screenshot and Fault Scenario Selector
```

Next acceptance criteria:

- [ ] Add reset/recovery screenshot or storyboard artifact.
- [ ] Connect FAULT button to at least one named process fault scenario.
- [ ] Show `START -> FAULT -> RESET -> START` as a documented operator flow.
- [ ] Keep synthetic override and process fault injection clearly separated in docs.
