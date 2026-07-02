# Goal 045: Canvas Button Interaction and Fault Selector

## 목적

이전 Goal까지의 Unity Canvas HMI는 화면 구성과 animation은 있었지만, 오른쪽 command button이 실제 process player를 제어하지 않았다.

이번 Goal은 HMI command button을 실제 Unity runtime state와 연결한다.

## 구현 내용

- `MolyAldProcessPlayer`
  - `IsPlaying` 상태를 START/STOP button에서 제어할 수 있게 유지.
  - `OperatorFaultActive` 상태 추가.
  - `ToggleOperatorFault()` 추가.
  - `ResetToStart()` 추가.
  - fault가 active일 때는 `Play()`가 재생을 시작하지 않도록 guard 추가.

- `MolyAldVisualStateMapper`
  - `forceFault` 입력 추가.
  - 실제 timeline fault가 아니어도 HMI fault mode를 동일한 visual state 경로로 표시.

- `MolyAldOperatorCanvas`
  - Canvas button에 `UnityEngine.UI.Button` component 추가.
  - `START` button: player 재생.
  - `STOP` button: player 정지.
  - `FAULT` button: synthetic operator fault toggle.
  - `RESET` button: 첫 step으로 복귀, fault 해제, 정지.
  - `EventSystem`이 없으면 자동 생성.
  - button label/color가 재생, 정지, fault 상태에 따라 갱신된다.

- `MolyAldEditorSmokeTest`
  - Canvas command button이 4개 이상 생성되는지 검증.
  - `EventSystem` 생성 여부 검증.
  - Play/Pause/Fault/Reset 상태 전이를 batch smoke test에서 검증.
  - forced fault가 `MolyAldVisualState.HasFault`에 반영되는지 검증.

## 설계 판단

START가 fault를 자동 해제하지 않게 했다.

이유:

- 실제 장비 HMI에서 fault/alarm은 명시적 acknowledge/reset 없이 조용히 사라지면 안 된다.
- demo에서도 operator가 `FAULT -> RESET -> START` 흐름을 설명할 수 있어야 한다.
- 현재 fault는 실제 장비 fault가 아니라 synthetic operator fault이므로, 동작 범위를 문서에 명확히 남긴다.

## 검증 기준

- [x] START button이 timeline player를 running 상태로 만든다.
- [x] STOP button이 timeline player를 pause/hold 상태로 만든다.
- [x] RESET button이 첫 step으로 돌아가고 fault를 해제한다.
- [x] FAULT button이 synthetic fault mode를 toggle한다.
- [x] Unity smoke test가 button, EventSystem, player state transition을 검증한다.

## 남은 한계

- 아직 mouse click으로 fault screenshot을 자동 capture하지는 않는다.
- fault는 JSON scenario 선택이 아니라 operator가 강제로 넣는 synthetic override다.
- 다음 단계에서는 fault mode screenshot 또는 operator action log를 별도로 만들 수 있다.

## 다음 권장 Goal

```text
Goal 046: Fault Mode Screenshot and Operator Action Log
```

추천 이유:

- 버튼은 실제로 동작하게 되었으므로, 이제 fault 상태가 화면에서 어떻게 보이는지 산출물로 남겨야 한다.
- 면접/포트폴리오 설명에서 `정상 공정 -> Fault hold -> Reset recovery` 흐름을 보여줄 수 있다.
