# Goal 046: Fault Mode Screenshot and Operator Action Log

## 목적

Goal 045에서 command button은 실제 runtime state와 연결됐다.

이번 Goal은 그 결과가 portfolio artifact로 보이게 만든다.

핵심 흐름:

```text
normal run
    -> operator selects FAULT
    -> process is held
    -> alarm card / schematic / action log show the reason
```

## 구현 내용

- `MolyAldOperatorCanvas`
  - `OPERATOR ACTION LOG` card 추가.
  - 최근 3개 operator action을 표시한다.
  - `START`, `STOP`, `FAULT`, `RESET` handler가 action log에 event를 남긴다.
  - fault 상태에서는 action log card도 red-tinted 상태로 표시한다.

- `MolyAldEditorSmokeTest`
  - `CaptureFaultScreenshot()` 추가.
  - `RunBatchFaultScreenshotCapture()` 추가.
  - fault screenshot 준비 시:
    - representative Reactant step으로 이동;
    - normal run action 기록;
    - synthetic fault hold 기록;
    - Canvas를 fault 상태로 refresh.
  - smoke test가 operator action log entry recording을 검증한다.

- `Invoke-UnitySmokeTest.ps1`
  - `-CaptureFaultScreenshot` switch 추가.
  - 기본 fault screenshot 경로:

    ```text
    artifacts/unity-demo/moly-ald-demo-fault.png
    ```

- 문서용 artifact
  - `docs/demo/moly-ald-demo.png`
  - `docs/demo/moly-ald-demo-fault.png`

## 설계 판단

현재 FAULT는 real process-runner fault scenario가 아니라 synthetic operator override다.

이유:

- UI/HMI interaction demo가 먼저 필요했다.
- 실제 process fault matrix 선택은 별도 goal로 분리하는 편이 낫다.
- 지금은 `normal -> fault hold -> reset`의 HMI 반응을 보여주는 것이 목표다.

## 검증 기준

- [x] fault screenshot에서 `HELD | OPERATOR ACTION REQUIRED`가 보인다.
- [x] fault screenshot에서 red alarm card와 `ALARM ACTIVE`가 보인다.
- [x] fault screenshot에서 chamber/exhaust/pump path가 red highlight된다.
- [x] operator action log에 `START`와 `FAULT`가 남는다.
- [x] `Invoke-UnitySmokeTest.ps1 -CaptureFaultScreenshot`으로 fault screenshot을 생성할 수 있다.

## 남은 한계

- action log는 in-memory Canvas log다. 파일 로그나 replay log는 아니다.
- FAULT는 process fault matrix JSON 선택이 아니다.
- RESET recovery screenshot은 아직 별도 artifact로 만들지 않았다.

## 다음 권장 Goal

```text
Goal 047: Reset Recovery Screenshot and Fault Scenario Selector
```

추천 이유:

- 현재는 `normal`과 `fault hold` 화면을 보여준다.
- 다음은 `reset recovery` 화면과 process fault matrix selector를 연결하면 demo sequence가 더 완성된다.
