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

