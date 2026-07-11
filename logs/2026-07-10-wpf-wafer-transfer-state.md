# 2026-07-10 — ALD Timeline 기반 Wafer Transfer 상태 시각화

## 1. 한 일

- Core가 만든 ALD timeline의 현재 Step을 WPF Wafer 위치와 Gate 상태로 변환했다.
- Load Port 내부, 이송 경로, Process Chamber 내부에 Wafer 표시 위치를 만들었다.
- 같은 물리 경로에서 Transfer In과 Transfer Out 방향을 서로 다른 indicator로 표시했다.
- transfer gate의 OPEN/CLOSED 상태를 색과 텍스트로 표시했다.
- 현재 Step 아래에 Wafer 위치 설명을 추가했다.
- 공정 fault 중에는 Wafer가 Chamber에 Hold되고 Gate가 닫힌 것으로 표시한다.

상태 매핑:

```text
LoadWafer
  → Gate OPEN
  → Wafer transfer in

PumpDown ~ PostPurge
  → Gate CLOSED
  → Wafer in process chamber

TransferOut
  → Gate OPEN
  → Wafer transfer out

Complete
  → Gate CLOSED
  → Wafer returned to load port

Alarm during chamber process
  → Gate CLOSED
  → Wafer held in chamber
```

## 2. 바뀐 파일

- `src/EquipmentTwin.Hmi.Wpf/ViewModels/OperatorConsoleViewModel.cs`
  - 현재 timeline step 해석
  - Wafer 위치/opacity, Gate 상태/색상, 한영 설명 계산
  - 화면 갱신 알림 추가
- `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml`
  - Load Port Wafer
  - transfer track와 Gate
  - Transfer In/Out indicator
  - Chamber Wafer opacity
  - Wafer 상태 설명
- `goals/065-wpf-wafer-transfer-state.md`
- `logs/2026-07-10-wpf-wafer-transfer-state.md`
- `state/loop-state.md`
- `state/triage.md`

로컬 상태 검증은 Git에서 제외된 `artifacts/goal-063-workflow-state-test/`를 재사용했으며 공개 커밋에는 포함하지 않는다.

## 3. 검증 결과

- WPF Release build: 통과, 경고 0, 오류 0
- 전체 solution Release build: 통과, 경고 0, 오류 0
- Core tests: 전체 통과
- timeline 상태별 직접 검증: 통과
  - LoadWafer: Transfer In 표시, Gate OPEN
  - PumpDown: Chamber Wafer 표시, Gate CLOSED
  - TransferOut: Transfer Out 표시, Gate OPEN
  - Complete: Load Port Wafer 표시, Gate CLOSED
  - Alarm: Wafer Chamber Hold, Gate CLOSED
  - 한국어 Complete/Gate 문구
- WPF 구현 파일 `MotionAxis` 참조 검색: 없음
- `git diff --check`: 통과

## 4. 막힌 점과 해결 방법

현재 막힌 점 없음.

도메인 검토 중 Exhaust/Pump는 Wafer 출구가 아니라 진공 배기 경로라는 점을 확인했다. 따라서 TransferOut Wafer를 오른쪽 Exhaust 방향으로 보내지 않고 왼쪽 Load Port로 복귀시키도록 표현했다.

## 5. 보류한 판단

- 연속 좌표, 속도, 가감속은 구현하지 않았다.
- WPF Storyboard 기반 부드러운 이동 animation은 추가하지 않았다.
- 실제 transfer robot이나 load lock sequence를 모델링하지 않았다.
- 기존 비전 검사 셀의 MotionAxis는 재사용하지 않았다.
- Gate CLOSED 표시는 demo timeline 상태이며 실제 안전 인증 로직이 아니다.

## 6. 소프트웨어 아키텍처 설명

```text
MolyAldTimelineStep.Step
  → ViewModel의 CurrentProcessStep
  → Wafer 위치/Gate computed property
  → XAML shape의 Opacity/Brush/Text binding
```

Core는 `LoadWafer`, `PumpDown`, `TransferOut` 같은 공정 Step만 제공한다. ViewModel은 그 의미를 화면 상태로 해석하고, XAML은 정해진 위치의 shape를 보이거나 숨긴다.

## 7. 유지보수할 때 봐야 할 파일

- Step 해석: `OperatorConsoleViewModel.CurrentProcessStep`
- Chamber 공정 Step 범위: `OperatorConsoleViewModel.IsWaferInChamber`
- Gate 개폐 판단: `OperatorConsoleViewModel.IsTransferGateOpen`
- 사용자 설명: `OperatorConsoleViewModel.WaferTransferStatusText`
- Wafer 표시 위치: `MainWindow.xaml`의 Load Port/transfer track/Process Chamber
- 실제 ALD Step 목록: `src/EquipmentTwin.Core/Processes/MolyAldProcessStep.cs`

## 8. 사용자가 이해해야 할 개념

- `상태 기반 시각화`: 실제 좌표를 계산하는 대신 현재 공정 상태에 맞는 그림을 선택하는 방식이다.
- `Opacity binding`: ViewModel 값이 1이면 shape가 보이고 0이면 숨겨진다.
- `timeline projection`: Core의 실행 기록을 UI에서 이해하기 쉬운 위치와 문구로 변환하는 것이다.
- `MotionAxis와의 차이`: MotionAxis는 Servo/Home/Position/Timeout을 가진 축 모델이고, 이번 기능은 공정 Step의 의미만 화면에 표현한다.
- `배기 경로와 이송 경로`: Exhaust/Pump는 gas/vacuum 경로이고 Wafer는 Load Port와 Chamber 사이에서 이동한다.

## 9. 다음 작업

- `Goal 066: WPF Demo 상태 자동 Screenshot Capture`
- WPF 자체 RenderTargetBitmap 경로를 검토해 정상/알람/이송 상태를 반복 가능한 PNG로 남기고 최신 UI를 자동 검수한다.
