# UI/UX Review Log

## 2026-07-02 Review 001: Unity Operator Console Screenshot

리뷰 대상:

```text
docs/demo/moly-ald-demo.png
```

### Verdict

- 상태: Needs Work
- 한 줄 판단: “장비 + 조작 패널”의 방향은 맞지만, 아직 전문 HMI라기보다는 3D 공간에 텍스트를 얹은 mockup에 가깝다.

### Problems

| Priority | Problem | Evidence |
|---|---|---|
| P1 | UI 텍스트가 3D TextMesh라 선명도와 정렬이 약하다 | 오른쪽 panel 글자가 screenshot에서는 읽히지만, 전문 UI처럼 날카롭지 않다 |
| P1 | 정보 위계가 부족하다 | recipe, live, alarm, event가 비슷한 시각 무게라 현재 step과 alarm 우선순위가 강하지 않다 |
| P1 | 색상 token이 명확하지 않다 | green/yellow/orange/cyan button이 의미를 갖지만 panel/line/timeline 색과 체계적으로 연결되지 않는다 |
| P2 | 장비 3D와 UI가 같은 perspective 공간에 있어 dashboard 느낌이 약하다 | operator panel이 3D 물체처럼 기울어져 실제 HMI보다 mock panel처럼 보인다 |
| P2 | timeline은 보이지만 step label과 progress bar의 관계가 약하다 | 현재 active step은 보이지만 사용자가 “지금 어디쯤인지”를 즉시 읽기 어렵다 |
| P3 | 장비 외형은 나아졌지만 아직 blockout 수준이다 | cabinet/load port/chamber/pump 구성은 생겼지만 bevel, lighting, material contrast가 부족하다 |

### Design Fixes

| Priority | Fix | Implementation hint |
|---|---|---|
| P1 | UI는 3D TextMesh가 아니라 Canvas 또는 UI Toolkit으로 분리한다 | `Screen Space - Camera` Canvas를 만들고 오른쪽 panel을 2D UI로 구성 |
| P1 | semantic color token을 정의한다 | `background`, `surface`, `primary`, `success`, `warning`, `alarm`, `mutedText`, `valueText` |
| P1 | typography scale을 고정한다 | title 32, section 18, body 14, telemetry value 22, alarm 24 이상 |
| P1 | alarm card를 정상 상태에서도 독립 영역으로 유지한다 | alarm 발생 시 red fill + icon/text, 정상 시 dark card + green status |
| P2 | 장비 3D는 왼쪽 60%, UI는 오른쪽 40%의 2D panel로 고정한다 | camera view 안에서 3D object와 Canvas가 겹치지 않게 layout contract 정의 |
| P2 | timeline은 하단 전체 폭 2D component로 만든다 | step chip + progress fill + active step marker |
| P3 | 3D 장비는 blockout을 유지하되 material contrast를 정리한다 | dark cabinet, blue glass, active gas color, alarm highlight만 사용 |

### Acceptance Criteria for Next Visual Goal

- [ ] operator panel text는 Canvas text로 렌더링된다.
- [ ] 오른쪽 UI는 3D perspective에 의해 기울어지지 않는다.
- [ ] 현재 step은 section header보다 큰 value style로 보인다.
- [ ] pressure/temp/film 값은 label과 value가 분리된다.
- [ ] alarm 상태는 정상 telemetry보다 강한 색/크기/위치를 갖는다.
- [ ] timeline은 현재 step chip이 명확히 active 상태를 가진다.
- [ ] screenshot 50% 축소 상태에서도 step, alarm, pressure, temperature를 읽을 수 있다.

### Next Screenshot Checks

- [ ] 첫눈에 `Dose Reactant`가 현재 step임을 알 수 있는가?
- [ ] 현재 active valve가 Reactant라는 점이 UI와 장비 양쪽에서 보이는가?
- [ ] 정상/알람 상태가 색상만이 아니라 text hierarchy로도 구분되는가?
- [ ] “실제 vendor UI 복제”가 아니라 “synthetic operator console”임이 제목/설명에서 보이는가?

## 2026-07-02 Review 002: Canvas Operator Panel

리뷰 대상:

```text
docs/demo/moly-ald-demo.png
```

### Verdict

- 상태: Pass for MVP
- 한 줄 판단: 오른쪽 operator panel과 하단 timeline이 Canvas UI로 분리되면서 글씨 선명도, 정보 위계, HMI 느낌이 크게 개선됐다.

### Improved

| Area | Result |
|---|---|
| Typography | Current step이 큰 Canvas text로 분리되어 첫눈에 보인다 |
| Layout | 장비 3D와 operator UI가 분리되어 서로 덜 방해한다 |
| Telemetry | Pressure, Temp, Film 값이 읽기 쉬운 카드로 정리됐다 |
| Alarm | `NO ALARM` 상태가 독립 card로 표시된다 |
| Timeline | step chip과 progress fill이 2D UI로 정리됐다 |

### Remaining Issues

| Priority | Problem | Next Fix |
|---|---|---|
| P1 | 버튼이 아직 실제 interaction과 연결되지 않았다 | Start/Stop/Fault/Reset click handler 추가 |
| P1 | Fault selector가 실제 scenario 선택 UI가 아니다 | configured fault dropdown 또는 button group 추가 |
| P2 | Alarm 상태 screenshot이 아직 없다 | fault timeline sample 또는 fault mode screenshot capture 추가 |
| P2 | 장비 3D blockout은 아직 primitive 수준이다 | lighting/material/bevel 또는 imported model boundary 활용 |

### Next Acceptance Criteria

- [ ] START button calls `MolyAldProcessPlayer.Play()` or restart/run command.
- [ ] STOP button calls `MolyAldProcessPlayer.Pause()`.
- [ ] RESET button restarts the timeline.
- [ ] FAULT control can select at least one synthetic fault scenario.
- [ ] Alarm screenshot shows red alarm card and matching equipment highlight.

## 2026-07-02 Review 003: HMI Typography and Instrument Panel

리뷰 대상:

```text
docs/demo/moly-ald-demo.png
```

### Verdict

- 상태: Improved, still not final design
- 한 줄 판단: 숫자/단위/상태/범위가 분리되면서 "텍스트 나열"에서 "HMI 계기판"으로 한 단계 올라갔다.

### Improved

| Area | Result |
|---|---|
| Typography | `MOLY ALD HMI`, run state, current step, instrument row의 hierarchy가 분리됐다 |
| Numeric readability | Pressure 850, Temp 250, Film 2가 screenshot에서 직접 읽힌다 |
| Instrument context | 정상 범위 band와 현재값 fill이 추가됐다 |
| Alarm information | priority와 synthetic fault code 영역이 생겼다 |
| Density control | Valve row를 제거해 numeric card 과밀도를 줄였다 |

### Remaining Issues

| Priority | Problem | Next Fix |
|---|---|---|
| P1 | 버튼은 여전히 visual mock이다 | click handler와 process player control 연결 |
| P1 | fault mode 화면이 없다 | fault selector + alarm screenshot capture 추가 |
| P2 | 정상 범위가 synthetic fixed range다 | recipe 또는 timeline metadata에서 range를 읽는 구조 추가 |
| P2 | 폰트는 아직 Unity built-in fallback이다 | public license font asset 추가 여부를 별도 결정 |
| P3 | 3D 장비 blockout은 아직 primitive 수준이다 | imported model 또는 material polish 단계에서 개선 |

### Next Acceptance Criteria

- [ ] START/STOP/RESET button이 실제 player state를 바꾼다.
- [ ] FAULT control이 synthetic fault scenario를 선택한다.
- [ ] fault screenshot에서 alarm card가 red priority state를 보여준다.
- [ ] 버튼 interaction 결과가 작업 로그와 smoke test에 남는다.

## 2026-07-02 Review 004: Process Schematic Main View

리뷰 대상:

```text
docs/demo/moly-ald-demo.png
```

### Verdict

- 상태: Better product direction
- 한 줄 판단: 3D primitive 외형을 메인으로 밀 때 생기던 장난감 느낌을 줄이고, 장비 SW 엔지니어가 설명하기 좋은 HMI schematic 방향으로 전환했다.

### Improved

| Area | Result |
|---|---|
| Equipment logic | chamber, gas valves, showerhead, wafer/film, exhaust/pump가 연결된 시스템으로 보인다 |
| Public-reference boundary | 실제 vendor CAD/UI/process 복제가 아니라 공개 ALD 개념 기반임을 문서화했다 |
| Operator readability | Reactant valve ON 상태가 schematic과 right HMI에서 같이 읽힌다 |
| Portfolio positioning | 3D 아트 실력이 아니라 process/HMI/state mapping 능력이 전면에 나온다 |

### Remaining Issues

| Priority | Problem | Next Fix |
|---|---|---|
| P1 | Schematic은 아직 static diagram에 가깝다 | gas flow pulse animation 추가 |
| P1 | 버튼 interaction이 없다 | Start/Stop/Reset/Fault selector 연결 |
| P2 | 3D cutaway 보조뷰는 아직 없다 | 작은 optional cutaway/debug panel 추가 |
| P2 | 실제 recipe range가 없다 | synthetic range와 future recipe metadata 경계 유지 |

### Next Acceptance Criteria

- [ ] Reactant dose step에서 gas dot/pulse animation이 보인다.
- [ ] fault mode에서 chamber/exhaust/alarm card가 같이 highlight된다.
- [ ] START/STOP/RESET/FAULT가 실제 player/fault scenario와 연결된다.

## 2026-07-02 Review 005: Gas Flow Animation

리뷰 대상:

```text
docs/demo/moly-ald-demo.png
```

### Verdict

- 상태: Improved
- 한 줄 판단: schematic이 단순 도식에서 현재 공정 step에 반응하는 장비 SW 화면으로 한 단계 이동했다.

### Improved

| Area | Result |
|---|---|
| Flow state | Reactant valve ON 상태에서 gas pulse와 flow label이 보인다 |
| Schematic liveliness | showerhead gas dots가 active gas color로 pulse된다 |
| Fault readiness | chamber/exhaust/gate/pump highlight path가 fault 상태에 연결됐다 |
| Demo clarity | `FLOW: Reactant pulse -> chamber`가 현재 step을 설명한다 |

### Remaining Issues

| Priority | Problem | Next Fix |
|---|---|---|
| P1 | 버튼은 아직 visual mock이다 | Start/Stop/Reset/Fault selector 연결 |
| P1 | fault mode screenshot이 없다 | synthetic fault scenario를 UI에서 선택하고 capture |
| P2 | flow pulse는 simple Canvas marker다 | 필요하면 pulse trail/arrow를 더 정교화 |
| P2 | 3D cutaway 보조뷰는 아직 없다 | 작은 cutaway/debug view를 다음 시각화 goal로 분리 |

### Next Acceptance Criteria

- [ ] START/STOP/RESET이 실제 timeline player state를 바꾼다.
- [ ] FAULT가 synthetic fault scenario를 선택한다.
- [ ] fault screenshot에서 flow가 멈추고 alarm/fault highlight가 보인다.

## 2026-07-02 Review 006: Canvas Button Interaction

리뷰 대상:

```text
MolyAldOperatorCanvas
MolyAldProcessPlayer
MolyAldEditorSmokeTest
```

### Verdict

- 상태: Interaction baseline complete
- 한 줄 판단: HMI command button이 visual mock에서 runtime state를 바꾸는 control로 승격됐다.

### Improved

| Area | Result |
|---|---|
| Operator command | START/STOP/RESET/FAULT가 process player 상태와 연결됐다 |
| Safety behavior | START는 fault를 자동 해제하지 않고, RESET 또는 FAULT toggle이 필요하다 |
| Visual feedback | button label/color가 running, paused, fault 상태를 반영한다 |
| Testability | Unity smoke test가 Button, EventSystem, Play/Pause/Fault/Reset 전이를 검증한다 |

### Remaining Issues

| Priority | Problem | Next Fix |
|---|---|---|
| P1 | fault 상태 screenshot artifact가 아직 없다 | red alarm/fault screenshot capture 추가 |
| P1 | operator action history가 화면에 없다 | START/STOP/FAULT/RESET event log panel 추가 |
| P2 | FAULT는 JSON fault scenario 선택이 아니라 synthetic override다 | process fault matrix와 UI selector 연결 |
| P2 | 실제 click play-mode 수동 검증 절차가 문서화되지 않았다 | Unity Editor demo checklist 보강 |

### Next Acceptance Criteria

- [ ] fault mode screenshot에서 alarm card와 schematic highlight가 동시에 보인다.
- [ ] HMI 하단 또는 우측에 최근 operator action 3~5개가 표시된다.
- [ ] demo 설명에서 synthetic override와 process-runner fault를 구분한다.

## 2026-07-02 Review 007: Fault Mode Screenshot and Operator Action Log

리뷰 대상:

```text
docs/demo/moly-ald-demo.png
docs/demo/moly-ald-demo-fault.png
```

### Verdict

- 상태: Demo evidence improved
- 한 줄 판단: 정상 화면과 fault hold 화면이 분리되어, HMI가 상태 변화에 반응한다는 증거가 생겼다.

### Improved

| Area | Result |
|---|---|
| Fault visibility | `HELD | OPERATOR ACTION REQUIRED`, `ALARM ACTIVE`, red schematic highlight가 동시에 보인다 |
| Operator trace | `OPERATOR ACTION LOG`가 START와 FAULT 이력을 보여준다 |
| Demo artifact | 정상 screenshot과 fault screenshot을 따로 생성할 수 있다 |
| Testability | `Invoke-UnitySmokeTest.ps1 -CaptureFaultScreenshot`로 fault artifact를 재현할 수 있다 |

### Remaining Issues

| Priority | Problem | Next Fix |
|---|---|---|
| P1 | reset recovery 화면은 아직 없다 | `RESET -> READY -> START` screenshot/storyboard 추가 |
| P1 | FAULT는 아직 named process fault scenario가 아니다 | fault matrix selector와 연결 |
| P2 | action log는 memory-only Canvas log다 | 필요하면 run report/file log와 연결 |
| P2 | action log row가 작다 | 화면이 더 넓어질 때 dedicated event panel로 확장 |

### Next Acceptance Criteria

- [ ] reset recovery screenshot 또는 storyboard가 있다.
- [ ] FAULT button이 named process fault scenario와 연결된다.
- [ ] demo에서 `normal -> fault -> reset recovery` 흐름을 한 번에 설명할 수 있다.

## 2026-07-02 Review 008: Reset Recovery and Named Fault Scenario

리뷰 대상:

```text
docs/demo/moly-ald-demo.png
docs/demo/moly-ald-demo-fault.png
docs/demo/moly-ald-demo-recovery.png
```

### Verdict

- 상태: 현재 MVP 수준의 operator story는 완성.
- 한 줄 판단: `normal -> held fault -> reset recovery` 흐름이 생겨서, 이제 화면이 단순 정적 UI가 아니라 조작 결과를 설명할 수 있다.

### Improved

| Area | Result |
|---|---|
| Recovery state | `PAUSED | READY`, `NO ALARM`, `Load Wafer`로 RESET이 held state를 지웠다는 점이 보인다 |
| Fault naming | FAULT가 완전한 generic label 대신 public scenario인 `precursor-dose-timeout`을 사용한다 |
| Operator trace | action log에서 START, FAULT, RESET 흐름을 한 화면에서 추적할 수 있다 |
| Testability | `Invoke-UnitySmokeTest.ps1 -CaptureRecoveryScreenshot`로 recovery artifact를 재생성할 수 있다 |

### Remaining Issues

| Priority | Problem | Next Fix |
|---|---|---|
| P1 | Unity는 아직 selected fault JSON timeline을 replay하지 않고 synthetic hold를 건다 | selected scenario를 process-runner fault replay와 연결 |
| P2 | fault scenario를 사용자가 cycle하는 UI control은 아직 없다 | layout 여유가 생기면 scenario selector/cycle control 추가 |
| P2 | action log는 in-memory Canvas log다 | 필요하면 run report/file log로 확장 |

### Next Acceptance Criteria

- [ ] `precursor-dose-timeout`을 선택하고 Unity에서 실제 fault timeline step을 replay한다.
- [ ] HMI hold state는 safety overlay로 유지하고, process truth는 timeline replay에서 가져온다.
- [ ] fault timeline step이 schematic, action log, alarm detail에 일관되게 표시된다.

