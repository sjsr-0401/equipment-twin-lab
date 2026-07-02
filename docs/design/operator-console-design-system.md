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

### Telemetry Card

값은 label과 value를 분리한다.

```text
Pressure   850 mTorr
Temp       250 C
Film       2 A
Valve      Reactant
```

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

## Goal 042 Candidate

```text
Goal 042: Canvas Button Interaction and Fault Selector
```

Next acceptance criteria:

- [ ] START button restarts and plays the timeline.
- [ ] STOP button pauses the timeline.
- [ ] RESET button returns to the first step.
- [ ] FAULT control selects at least one synthetic fault scenario.
- [ ] fault mode screenshot shows alarm card and equipment highlight.
