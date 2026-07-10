# 2026-07-10 — Fault Code별 WPF Schematic 진단 영역 강조

## 1. 한 일

- 왼쪽 장비 schematic 상단에 현재 진단 포커스를 나타내는 badge를 추가했다.
- 알람 코드별로 문제 영역을 다음과 같이 연결했다.

```text
GAS-301 → Gas Box / Delivery Line
TMP-201 → Process Chamber / Heater
VAC-101 → Vacuum Path / Exhaust / Pump
```

- 해당 영역의 테두리와 배관은 빨간색으로 강조한다.
- 관련 없는 모듈은 중립색을 유지한다.
- 정상 상태에서는 초록색 `SCHEMATIC FOCUS · NORMAL` badge를 표시한다.
- 기존 MotionAxis나 비전 검사 셀 모션 코드는 사용하지 않았다.

## 2. 바뀐 파일

- `src/EquipmentTwin.Hmi.Wpf/ViewModels/OperatorConsoleViewModel.cs`
  - fault code와 schematic 영역을 연결하는 표시 속성 추가
  - 화면 갱신 시 새 속성들의 PropertyChanged 알림 추가
- `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml`
  - 진단 포커스 badge 추가
  - Gas Box, gas line, chamber, heater, vacuum path, pump 테두리 바인딩
- `goals/064-wpf-fault-schematic-focus.md`
- `logs/2026-07-10-wpf-fault-schematic-focus.md`
- `state/loop-state.md`
- `state/triage.md`

로컬 상태 검증은 Git에서 제외된 `artifacts/goal-063-workflow-state-test/`를 재사용했으며 공개 커밋에는 포함하지 않는다.

## 3. 검증 결과

- WPF Release build: 통과, 경고 0, 오류 0
- 전체 solution Release build: 통과, 경고 0, 오류 0
- Core tests: 전체 통과
- 상태별 직접 검증: 통과
  - 정상 badge/초록색
  - GAS-301 Gas Box와 delivery line 빨간색
  - GAS fault에서 Chamber 중립색 유지
  - TMP-201 Chamber와 heater 빨간색
  - TMP fault에서 Gas Box 중립색 유지
  - VAC-101 Exhaust/Pump와 vacuum path 빨간색
  - VAC fault에서 heater 정상색 유지
  - 한글 VAC 진단 문구 확인
- `git diff --check`: 통과

## 4. 막힌 점과 해결 방법

현재 막힌 점 없음.

확인 과정에서 FILM 계열 알람을 찾았지만 현재 Core에는 해당 fault code가 없었다. UI만 먼저 만드는 대신 현재 존재하는 세 알람만 연결했다.

## 5. 보류한 판단

- Film thickness fault는 Core fault 모델과 alarm guide가 추가될 때 함께 연결한다.
- 색상 점멸과 애니메이션은 추가하지 않았다.
- 실제 장비 P&ID나 vendor fault-location mapping이라고 주장하지 않는다.
- 이번 강조는 공개/합성 ALD demo의 operator diagnostic aid다.

## 6. 소프트웨어 아키텍처 설명

데이터 흐름은 다음과 같다.

```text
Core fault timeline
  → active alarm guide의 AlarmCode
  → ViewModel이 진단 모듈과 brush 계산
  → XAML이 해당 Border/Rectangle에 표시
```

Core는 어떤 fault가 발생했는지만 제공한다. WPF는 그 fault를 화면의 어느 장비 영역에 보여줄지 결정한다. 따라서 공정 로직과 화면 표현의 책임이 섞이지 않는다.

## 7. 유지보수할 때 봐야 할 파일

- fault code와 장비 영역 문구: `OperatorConsoleViewModel.SchematicDiagnosticText`
- Gas Box/공급 라인 색상: `GasModuleBorderBrush`, `GasDeliveryLineBrush`
- Chamber/Heater 색상: `ChamberBorderBrush`, `HeaterDiagnosticBrush`
- Exhaust/Pump 색상: `ExhaustModuleBorderBrush`, `VacuumPathBrush`
- 실제 shape 연결 위치: `MainWindow.xaml`의 `EQUIPMENT MODULE LAYOUT`
- Core 알람 코드 원본: `src/EquipmentTwin.Core/Processes/MolyAldAlarmGuideCodes.cs`

## 8. 사용자가 이해해야 할 개념

- `이중 부호화`: 색뿐 아니라 코드와 텍스트를 같이 사용해 상태를 표현하는 방법이다.
- `fault mapping`: 소프트웨어 알람 코드를 사용자가 확인할 실제 장비 영역과 연결하는 규칙이다.
- `computed brush`: 상태를 저장하는 대신 현재 알람 코드를 읽어 그때그때 색상을 계산하는 WPF 속성이다.
- `PropertyChanged`: ViewModel 값이 달라졌다는 사실을 XAML에 알려 화면을 다시 그리게 한다.
- `책임 분리`: Core는 fault 결과, ViewModel은 표시 판단, XAML은 실제 drawing을 담당한다.

## 9. 다음 작업

- `Goal 065: ALD timeline 기반 Wafer Transfer 상태 시각화`
- 기존 MotionAxis를 억지로 사용하지 않고 Load/Transfer/Process/Unload step에 따라 wafer 위치와 gate 상태를 WPF에서 보여준다.
