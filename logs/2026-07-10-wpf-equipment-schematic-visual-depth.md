# 2026-07-10 — WPF 장비 Schematic 시각 깊이 고도화

## 1. 한 일

- 사용자가 지적한 “장비가 아니라 큰 빈 사각형처럼 보이는 문제”를 다시 평가했다.
- 왼쪽 장비 영역을 WPF vector 기반 장비 단면/P&ID 혼합 화면으로 개선했다.
- 공통 시각 재질을 추가했다.
  - Module metal gradient
  - Chamber metal gradient
  - Brushed steel gradient
  - Heater metal gradient
- Load Port를 FOUP interface 형태로 개선했다.
  - 외곽 cabinet
  - FOUP slot 5개
  - Wafer 위치
  - Slit valve 형상
  - Gate 상태 badge
- Gas Box를 3채널 delivery manifold로 개선했다.
  - Precursor/Reactant/Purge N2 channel
  - 채널별 MFC block
  - isolation valve와 ON/OFF 상태
  - 공통 manifold pipe
- Process Chamber를 단면 구조로 개선했다.
  - 금속 chamber body와 alarm tint
  - bolt가 있는 showerhead/gas distribution plate
  - gas가 분사되는 process zone
  - 실제 chamber pressure와 wafer temperature readout
  - wafer 원판
  - susceptor/heater stage와 heater coil
  - lift shaft와 bottom plenum
  - throttle valve 방향 outlet
- Exhaust module을 실제 계통처럼 개선했다.
  - foreline
  - throttle valve symbol
  - vacuum pump housing
  - impeller 형태
  - vacuum/exhaust 상태색
- 기존 Core timeline과 ViewModel 상태 Binding은 그대로 유지했다.

## 2. 바뀐 파일

- `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml`
  - 장비 schematic의 재질, 형상, 배관, 계측 callout을 고도화했다.
- `src/EquipmentTwin.Hmi.Wpf/ViewModels/OperatorConsoleViewModel.cs`
  - 새 chamber metal/fault tint 구조에서 사용되지 않게 된 기존 `ChamberBrush` 표시 속성과 갱신 알림을 제거했다.
- `goals/069-wpf-equipment-schematic-visual-depth.md`
- `logs/2026-07-10-wpf-equipment-schematic-visual-depth.md`
- `plan.md`
- `state/loop-state.md`
- `state/triage.md`

## 3. 검증 결과

- WPF Release build: 통과, 경고 0, 오류 0
- 한국어 자동 Screenshot 5종: 모두 통과
  - 해상도 1600×900
  - 검정 표본 0/1200
  - 핵심 영역 4/4
- 영어 자동 Screenshot 5종: 모두 통과
  - 해상도 1600×900
  - 검정 표본 0/1200
  - 핵심 영역 4/4
- Load 화면 직접 확인:
  - FOUP/Slit valve, Gas Box, Chamber 단면, Throttle valve, Pump 형상 확인
- Process 화면 직접 확인:
  - Wafer가 chamber stage에 표시됨
  - Purge/Vacuum path가 초록색으로 연결됨
  - 실제 Pressure/Temperature 값이 chamber callout에 표시됨
- GAS-301 Alarm 화면 직접 확인:
  - Gas Box와 manifold가 빨간색으로 강조됨
  - Process zone이 빨간색 fault path로 표시됨
  - Chamber/wafer/pump 전체 구조는 유지됨
- Transfer Out 화면 직접 확인:
  - Wafer 이동과 Gate 상태 표시 유지
- 전체 solution Release build: 통과, 경고 0, 오류 0
- Core tests: 전체 통과
- 일반 WPF smoke test: 3초 동안 정상 실행 유지
- `git diff --check`: 통과

## 4. 막힌 점과 해결 방법

현재 남은 blocker는 없다.

Screenshot 미리보기 도구가 어두운 이미지 일부를 간헐적으로 검게 축약해 표시하는 기존 현상이 있었지만, 원본 PNG의 1600×900 해상도, 검정 표본, 네 anchor 검사와 상태별 pixel 영역 비교는 모두 통과했다. Load/GAS-301/Transfer Out 원본은 각각 직접 확인했다.

## 5. 보류한 판단

- 실제 Lam 장비 CAD나 내부 구조를 복제하지 않았다.
- 현재 화면은 공개적인 ALD 구성 개념을 사용한 synthetic schematic이다.
- 실제 MFC setpoint, valve position percentage, pump speed 같은 값은 Core에 없으므로 만들지 않았다.
- 3D perspective, CAD import, Blender model은 WPF 화면에 넣지 않았다.
- process gas particle animation이나 pump rotation animation은 추가하지 않았다.
- 외형 고도화 때문에 공정 로직, 알람 판단, 서버 전송 코드는 수정하지 않았다.

## 6. 소프트웨어 아키텍처 설명

```text
Core timeline의 실제 상태
  -> OperatorConsoleViewModel
     - Valve brush
     - Gas/Vacuum path brush
     - Wafer/Gate opacity와 상태
     - Pressure/Temperature 값
  -> MainWindow.xaml
     - 장비 금속 외형
     - 부품 형상과 배치
     - Binding된 상태색/값
```

이번 작업의 핵심은 “장비 외형”과 “장비 상태”의 분리다.

- 금속 gradient, pump impeller, showerhead bolt는 정적 presentation이다.
- Valve ON/OFF, Gas flow, Vacuum path, Wafer 위치, Alarm focus는 동적 process state다.
- XAML은 공정 상태를 계산하지 않는다.
- Core와 ViewModel은 pump가 원형으로 그려졌는지 모른다.

따라서 나중에 CAD/3D Viewer를 추가하더라도 Core 공정 계산은 그대로 재사용할 수 있다.

## 7. 유지보수할 때 봐야 할 파일

- 장비 전체 XAML: `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml`
- 상태색과 Wafer/Gate mapping: `src/EquipmentTwin.Hmi.Wpf/ViewModels/OperatorConsoleViewModel.cs`
- 실제 ALD Step: `src/EquipmentTwin.Core/Processes/MolyAldProcessStep.cs`
- 자동 Screenshot 검증: `scripts/Capture-WpfDemoScreenshots.ps1`

`MainWindow.xaml`에서 다음 문자열을 검색하면 모듈별 위치를 바로 찾을 수 있다.

- `GAS BOX / DELIVERY MANIFOLD`
- `PROCESS CHAMBER`
- `SHOWERHEAD / GAS DISTRIBUTION PLATE`
- `WAFER / SUSCEPTOR / HEATER STAGE`
- `EXHAUST MODULE`

## 8. 사용자가 이해해야 할 개념

- `schematic`: 실제 외형을 사진처럼 복제하는 대신 구성요소와 연결 관계를 설명하는 도식이다.
- `P&ID`: pipe, valve, instrument가 어떻게 연결됐는지 표현하는 공정 계통도다.
- `cross-section`: 장비를 잘라 내부를 보는 것처럼 표현한 단면도다.
- `MFC`: Mass Flow Controller. 공정 가스 유량을 제어하는 구성요소다. 이번 화면에서는 구성요소만 표시하며 가짜 유량값은 만들지 않았다.
- `isolation valve`: 가스 경로를 열거나 차단하는 밸브다. 기존 Core valve ON/OFF와 연결된다.
- `showerhead`: 공정 가스를 wafer 표면에 분배하는 plate다.
- `susceptor`: wafer를 지지하고 가열하는 stage 구성요소다.
- `bottom plenum`: chamber 아래쪽에서 배기 흐름을 모으는 공간을 뜻한다.
- `throttle valve`: chamber와 pump 사이에서 배기 conductance를 조절하는 valve다.
- `presentation state`: 색, 형상, 배치처럼 화면에 보이는 표현 상태다.
- `process state`: 실제 Step, Valve, Pressure, Alarm처럼 공정 동작을 나타내는 상태다.

## 9. 다음 작업

- `Goal 070: 알람 작업지시서 한국어 문장 품질 정리`
- 장비 화면은 포트폴리오 첫인상을 줄 수준으로 개선됐다.
- 다음에는 Alarm Response Guide를 작업자가 빠르게 읽을 수 있는 자연스러운 한국어로 다듬는다.

## 10. 실패한 검사와 원인

- 첫 공개 범위 검사에서 `token`, `private-notes`라는 단어 자체를 위험 문자열로 취급해 검사가 중단됐다.
- 실제 비밀값이 발견된 것은 아니며, 기존 기록에 있던 `디자인 token`과 `private-notes 공개 차단` 설명이 오탐지 원인이었다.
- 재검사에서는 단어 존재 여부가 아니라 API key·password의 값 형태와 사용자 절대 경로처럼 실제 유출 위험이 있는 패턴만 확인하도록 범위를 좁혔다.
