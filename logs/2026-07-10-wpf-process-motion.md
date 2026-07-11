# 2026-07-10 WPF 장비 공정 동작 시각화 작업 로그

## 1. 한 일

- 기존 ALD timeline 상태를 WPF animation에 연결했다.
- Gas flow를 움직이는 점선으로 표현했다.
- Vacuum path를 이동하는 점선으로 표현했다.
- Vacuum 공정 중 Pump rotor가 회전하도록 했다.
- Heater 활성 Step에서 heater 주변 glow가 천천히 밝아졌다 어두워지도록 했다.
- 활성 isolation valve가 작게 pulse하도록 했다.
- Load Wafer와 Transfer Out Step에서 wafer가 transfer rail 위를 이동하도록 했다.
- Transfer Gate가 열린 상태에서 위로 올라가도록 했다.
- Alarm 중에는 정상 공정 animation을 중지하고 fault 색상만 남겼다.

## 2. 바뀐 파일

- `src/EquipmentTwin.Hmi.Wpf/ViewModels/OperatorConsoleViewModel.cs`
  - Core 상태를 화면 의미로 바꾸는 `IsGasFlowActive`, `IsVacuumFlowActive`, `IsHeaterActive` 등을 추가했다.
- `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml`
  - 공통 animation Style과 장비 구성요소별 `DataTrigger`/`Storyboard`를 추가했다.
- `goals/070-wpf-process-motion.md`
  - 목표와 완료 기준을 기록했다.
- `plan.md`, `state/loop-state.md`, `state/triage.md`
  - 우선순위 변경, 완료 상태, 다음 작업을 기록했다.

## 3. 검증 결과

- WPF Release build: 경고 0, 오류 0.
- 한국어 5개 상태 Screenshot: 모두 1600×900, 검정 표본 0/1200, anchor 4/4 통과.
- 영어 5개 상태 Screenshot: 모두 1600×900, 검정 표본 0/1200, anchor 4/4 통과.
- 정상 Process 화면에서 gas/vacuum/heater/pump 상태 표현을 직접 확인했다.
- GAS-301 Alarm 화면에서 animation 대신 red fault path가 유지되는 것을 확인했다.
- Transfer Out 화면에서 Gate와 wafer transfer 상태가 유지되는 것을 확인했다.
- 전체 solution Release build: 경고 0, 오류 0.
- Core tests: 전체 통과.
- 일반 WPF smoke test: 4초 동안 조기 종료 없이 실행됨.
- `git diff --check`: 통과.

## 4. 막힌 점과 해결 방법

현재 blocker는 없다.

- 알람 Screenshot을 처음 확인할 때 파일명을 `wpf-alarm-gas-ko.png`로 잘못 추측해 파일 열기에 실패했다.
- 실제 생성 목록을 확인해 `wpf-alarm-ko.png`를 열었고 화면을 정상 검증했다.
- 별도 animation timer를 만들지 않고 WPF Storyboard를 사용해 Thread 관리 부담을 피했다.

## 5. 보류한 판단

- 실제 gas particle 물리 시뮬레이션은 구현하지 않았다.
- Pump rpm, MFC flow rate처럼 Core에 없는 수치는 만들지 않았다.
- 3D CAD animation과 Unity 연동은 이번 범위에 포함하지 않았다.
- 애니메이션 속도 사용자 설정과 Reduce Motion 설정은 화면 요구가 생길 때 검토한다.
- 알람 작업지시서 한국어 문장 정리는 다음 Goal로 이동했다.

## 6. 소프트웨어 아키텍처 설명

```text
Core Moly ALD Timeline
  -> OperatorConsoleViewModel
     - IsGasFlowActive
     - IsVacuumFlowActive
     - IsHeaterActive
     - IsWaferTransferIn/OutActive
  -> MainWindow.xaml
     - DataTrigger
     - Storyboard
     - RenderTransform / StrokeDashOffset
```

- Core는 공정의 Step, Valve, Alarm 상태만 결정한다.
- ViewModel은 그 상태를 `가스가 흐르는가`, `Pump가 동작하는가` 같은 화면 의미로 바꾼다.
- XAML은 의미 속성이 `true`일 때 어떤 모양으로 움직일지만 결정한다.
- Alarm일 때 ViewModel의 animation 속성이 `false`가 되어 정상 공정 움직임이 정지한다.
- 색상, ON/OFF 텍스트, 위치 표시는 그대로 남아 animation이 상태 전달의 유일한 수단이 되지 않는다.

## 7. 유지보수할 때 봐야 할 파일

- animation 시작 조건: `src/EquipmentTwin.Hmi.Wpf/ViewModels/OperatorConsoleViewModel.cs`
- animation 속도와 모양: `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml`
- 실제 공정 Step 정의: `src/EquipmentTwin.Core/Processes/MolyAldProcessStep.cs`
- 대표 화면 자동 검증: `scripts/Capture-WpfDemoScreenshots.ps1`

속도를 바꾸려면 `MainWindow.xaml`에서 다음 Style을 찾는다.

- `GasFlowStream`
- `VacuumFlowStream`
- `ValveActuatorMotion`
- `HeaterGlowMotion`
- `PumpRotorMotion`

공정 Step별 동작 조건을 바꾸려면 ViewModel의 `IsGasFlowActive`, `IsVacuumFlowActive`, `IsHeaterActive`를 수정한다.

## 8. 사용자가 이해해야 할 개념

- `Storyboard`: WPF 속성값을 시간에 따라 바꾸는 animation 묶음이다.
- `DataTrigger`: Binding 값이 특정 조건일 때 Setter나 Storyboard를 실행한다.
- `RenderTransform`: 레이아웃을 다시 계산하지 않고 화면 요소의 이동, 회전, 확대를 표현한다.
- `StrokeDashOffset`: 점선의 시작 위치를 움직여 유체가 흐르는 것처럼 보이게 한다.
- `RepeatBehavior`: animation을 한 번만 실행할지 반복할지 정한다.
- `computed property`: 값을 저장하지 않고 현재 상태를 읽어 매번 계산해서 반환하는 속성이다.
- `OnPropertyChanged`: 현재 Step이 바뀌었음을 WPF Binding에 알려 화면을 다시 평가하게 한다.
- 이번 구현은 별도 Thread를 직접 만들지 않는다. WPF animation system이 UI rendering clock을 사용한다.

## 9. 다음 작업

- `Goal 071: 알람 작업지시서 한국어 문장 품질 정리`
- Summary, checklist, response choice, escalation 문장을 작업자가 빠르게 읽을 수 있는 한국어로 다듬는다.
