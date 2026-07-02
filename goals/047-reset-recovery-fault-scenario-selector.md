# Goal 047: Reset Recovery Screenshot and Fault Scenario Selector

## 목적

Goal 046은 정상 화면과 fault hold 화면을 만들었다.

이번 Goal은 두 가지를 보강한다.

1. RESET 이후 복구 상태를 별도 screenshot으로 남긴다.
2. FAULT가 단순 synthetic override가 아니라 공개 process fault matrix의 named scenario를 선택했다는 근거를 HMI에 표시한다.

## 구현 내용

- `MolyAldProcessPlayer`
  - public demo fault scenario 목록 추가:
    - `pumpdown-timeout`
    - `temperature-not-stable`
    - `precursor-dose-timeout`
    - `purge-timeout`
  - `SelectedFaultScenarioName` 추가.
  - `ActiveFaultScenarioName` 추가.
  - `SelectFaultScenario()` 추가.
  - `SelectNextFaultScenario()` 추가.
  - `ActivateSelectedFaultScenario()` 추가.

- `MolyAldOperatorCanvas`
  - fault alarm detail에 selected scenario name 표시.
  - FAULT action log에 selected scenario name 표시.
  - selected/active scenario text를 HMI state pipeline에서 갱신.

- `MolyAldEditorSmokeTest`
  - `CaptureRecoveryScreenshot()` 추가.
  - `RunBatchRecoveryScreenshotCapture()` 추가.
  - recovery screenshot 준비 흐름:

    ```text
    START
        -> FAULT precursor-dose-timeout
        -> RESET
        -> READY at Load Wafer
    ```

- `Invoke-UnitySmokeTest.ps1`
  - `-CaptureRecoveryScreenshot` 추가.

## 생성 artifact

```text
docs/demo/moly-ald-demo.png
docs/demo/moly-ald-demo-fault.png
docs/demo/moly-ald-demo-recovery.png
```

## 검증 기준

- [x] fault screenshot에 `precursor-dose-timeout`이 보인다.
- [x] recovery screenshot에 `RESET` action log가 보인다.
- [x] recovery screenshot이 `PAUSED | READY` 상태를 보여준다.
- [x] recovery screenshot이 first step인 `Load Wafer`로 돌아간 것을 보여준다.
- [x] Unity smoke script로 recovery screenshot을 생성할 수 있다.

## 남은 한계

- Unity UI에서 사용자가 scenario list를 직접 cycle하는 별도 selector button은 아직 없다.
- named scenario는 HMI 표시와 action log에 연결됐지만, Unity visual timeline 자체가 fault timeline JSON으로 교체되는 구조는 아직 아니다.
- 실제 process-runner fault result와 Unity replay를 완전히 결합하는 단계는 다음 goal로 남긴다.

## 다음 권장 Goal

```text
Goal 048: Fault Timeline Replay Binding
```

추천 이유:

- 지금은 named scenario가 HMI에 표시된다.
- 다음은 CLI/core에서 생성한 fault timeline JSON을 Unity가 실제로 replay하도록 연결하면 simulation claim이 더 강해진다.
