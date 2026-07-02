# Goal 048: Fault Timeline Replay Binding

## 목적

Goal 047에서는 Unity HMI가 `precursor-dose-timeout`이라는 fault scenario 이름을 표시했다.

Goal 048의 목적은 한 단계 더 나아가, 그 이름에 대응하는 Core/CLI 생성 fault timeline JSON을 Unity가 실제로 읽고 replay하게 만드는 것이다.

## 구현 내용

- Core/CLI로 public Moly ALD fault timeline JSON을 생성했다.
  - `pumpdown-timeout`
  - `temperature-not-stable`
  - `precursor-dose-timeout`
  - `purge-timeout`
- Unity `StreamingAssets/faults` 아래에 fault timeline JSON을 추가했다.
- `MolyAldProcessPlayer`
  - selected fault scenario 이름으로 fault timeline path를 만든다.
  - `faults/moly-ald-timeline.{scenario}.json`을 로드한다.
  - timeline의 `faultScenarioName`이 selected scenario와 일치하는지 검증한다.
  - 첫 번째 failed step으로 이동한다.
  - `FaultTimelineReplayActive` 상태를 노출한다.
- `MolyAldOperatorCanvas`
  - fault replay 상태에서 button label을 `FAULT REPLAY`로 표시한다.
  - alarm detail을 `scenario | area replay` 형태로 표시한다.
  - action log에 `scenario replay step`을 남긴다.
- `MolyAldEditorSmokeTest`
  - FAULT 검증 시 selected scenario와 timeline scenario가 일치하는지 확인한다.
  - replay가 failed process step으로 이동했는지 확인한다.

## 아키텍처

```text
Core/CLI process runner
    -> public fault timeline JSON
    -> Unity StreamingAssets/faults
    -> MolyAldProcessPlayer
    -> MolyAldVisualStateMapper
    -> HMI schematic / alarm / action log
```

중요한 점:

- Unity가 공정을 계산하지 않는다.
- Core/CLI가 만든 process truth를 Unity가 replay한다.
- HMI hold는 safety/operator overlay이고, fault step 자체는 timeline JSON에서 온다.

## 생성 artifact

```text
unity/EquipmentTwin.Unity/Assets/StreamingAssets/faults/moly-ald-timeline.pumpdown-timeout.json
unity/EquipmentTwin.Unity/Assets/StreamingAssets/faults/moly-ald-timeline.temperature-not-stable.json
unity/EquipmentTwin.Unity/Assets/StreamingAssets/faults/moly-ald-timeline.precursor-dose-timeout.json
unity/EquipmentTwin.Unity/Assets/StreamingAssets/faults/moly-ald-timeline.purge-timeout.json
```

## 검증 기준

- [x] FAULT screenshot이 `FAULT REPLAY`를 표시한다.
- [x] `precursor-dose-timeout` 선택 시 Unity timeline이 `faultScenarioName=precursor-dose-timeout`인 JSON을 읽는다.
- [x] player가 첫 failed step인 `DoseMetalPrecursor`로 이동한다.
- [x] HMI current step이 replay된 failed step을 표시한다.
- [x] Unity smoke test가 `FaultTimelineReplayActive`를 검증한다.

## 남은 한계

- 사용자가 화면에서 scenario를 cycle하는 별도 selector button은 아직 없다.
- fault timeline JSON은 현재 repo에 생성된 static artifact다.
- 다음 단계에서는 scenario selector UI 또는 run report/export와 연결할 수 있다.

## 다음 권장 Goal

```text
Goal 049: Fault Scenario Selector UI
```

추천 이유:

- replay binding은 끝났다.
- 이제 operator가 `pumpdown-timeout`, `temperature-not-stable`, `precursor-dose-timeout`, `purge-timeout`을 화면에서 선택할 수 있으면 demo 조작성이 좋아진다.
