# 2026-07-10 — WPF 한국어 일반 UI 라벨 일관성 정리

## 1. 한 일

- 한국어 캡처에서 고정 영어로 남아 있던 일반 UI 문자열을 조사했다.
- ViewModel에 이미 준비되어 있었지만 XAML에서 사용하지 않던 번역 속성을 실제 화면에 연결했다.
- 다음 조작 버튼을 한국어 모드에서 전환한다.
  - `START` → `시작`
  - `STOP` → `정지`
  - `STEP` → `다음 STEP`
  - `FAULT REPLAY` → `FAULT 재현`
  - `RESET` → `초기화`
- 다음 일반 화면 제목을 한국어 모드에서 전환한다.
  - `EQUIPMENT MODULE LAYOUT` → `장비 모듈 구성`
  - `PROCESS TIMELINE` → `공정 진행도`
  - `FAULT SCENARIO SELECTOR` → `FAULT 시나리오 선택`
  - `CURRENT STEP` → `현재 STEP`
  - `PROCESS INSTRUMENTS` → `공정 계측값`
  - `ALARM PRIORITY` → `알람 우선순위`
  - `OPERATOR ACTION LOG` → `오퍼레이터 작업 로그`
  - `ENGINEERING TRACE CONSOLE` → `엔지니어링 Trace 콘솔`
- 한국어 제목과 부제의 불필요한 영어 혼용을 줄였다.
- 운전 상태와 계측 상태를 한국어로 표시하면서 기존 색상 의미를 유지했다.
- 한국어와 영어 대표 화면을 다시 캡처해 버튼 잘림과 화면 영역을 확인했다.

## 2. 바뀐 파일

- `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml`
  - 고정 영어 버튼/제목을 ViewModel binding으로 교체했다.
- `src/EquipmentTwin.Hmi.Wpf/ViewModels/OperatorConsoleViewModel.cs`
  - 한국어 버튼, 제목, 운전 상태, 계측 상태 문구를 정리했다.
  - 번역된 계측 상태가 기존 성공/경고/알람 색상을 유지하도록 상태 색상 mapping을 보강했다.
- `.github/workflows/ci.yml`
  - Unity Viewer 버튼 검사를 XAML 고정 문구가 아니라 XAML binding과 ViewModel 영어 원문으로 나눴다.
- `goals/068-wpf-korean-ui-label-consistency.md`
- `logs/2026-07-10-wpf-korean-ui-label-consistency.md`
- `plan.md`
- `state/loop-state.md`
- `state/triage.md`

## 3. 검증 결과

- WPF Release build: 통과, 경고 0, 오류 0
- 한국어 자동 캡처 5종: 모두 통과
  - 해상도 1600×900
  - 검정 표본 0/1200
  - 핵심 영역 4/4
- 영어 자동 캡처 5종: 모두 통과
  - 해상도 1600×900
  - 검정 표본 0/1200
  - 핵심 영역 4/4
- 한국어 Load 화면 직접 확인:
  - 시작/정지/다음 STEP/FAULT 재현/초기화 버튼 표시
  - 장비 모듈 구성/공정 진행도/현재 STEP/공정 계측값 표시
  - 일시정지/높음/낮음/성장 중 상태 표시
  - 버튼과 제목 잘림 없음
- 영어 Load 화면 직접 확인:
  - 기존 START/STOP/STEP/FAULT REPLAY/RESET 유지
  - 기존 영어 제목 유지
- XAML 고정 문자열 검사:
  - 제품명과 장비 도식 용어만 고정 문자열로 남은 것을 확인
  - 일반 버튼/영역 제목은 모두 ViewModel binding 사용
- 전체 solution Release build: 통과, 경고 0, 오류 0
- Core tests: 전체 통과
- 일반 WPF smoke test: 3초 동안 정상 실행 유지
- `git diff --check`: 통과

## 4. 막힌 점과 해결 방법

### 첫 일괄 patch의 XAML 문맥 불일치

처음에는 ViewModel과 XAML 변경을 한 patch로 적용하려 했다. 예상한 XAML 요소가 `Grid.RowSpan`이었지만 실제 파일은 `Grid.ColumnSpan`을 사용하고 있어 patch가 적용되지 않았다.

해결:

- 실패한 patch는 파일을 일부만 바꾸지 않고 전체가 취소된 것을 확인했다.
- ViewModel과 XAML patch를 분리했다.
- 실제 XAML 줄을 다시 읽고 정확한 요소만 교체했다.

기능 구현 자체에서 남은 blocker는 없다.

### 첫 GitHub CI의 오래된 XAML 고정 문자열 검사

첫 push의 .NET build와 모든 공정 테스트는 통과했지만, 마지막 파일 정합성 검사가 `MainWindow.xaml` 안에서 `Open optional Unity viewer folder` 문구를 찾다가 실패했다. 이번 작업에서 그 문구를 ViewModel 속성으로 옮겼기 때문에 기능이 사라진 것이 아니라 검사 위치가 달라진 것이다.

해결:

- XAML에서 `OpenUnityViewerButtonText` binding이 존재하는지 검사한다.
- ViewModel에서 영어 원문 `Open optional Unity viewer folder`가 존재하는지 별도로 검사한다.
- 이렇게 하면 버튼이 ViewModel과 연결됐는지와 영어 문구가 보존됐는지를 모두 확인할 수 있다.

첫 로컬 Bash 재현 명령은 PowerShell에서 공백이 포함된 `grep` pattern을 전달하는 quoting 때문에 두 번째 검사가 잘려 실패했다. Bash command 전체를 하나의 문자열 변수로 전달하고 pattern을 작은따옴표로 묶어 다시 실행했으며 두 검사 모두 통과했다.

## 5. 보류한 판단

- 장비 도식의 `LOAD PORT`, `GAS BOX`, `PROCESS CHAMBER`, `PUMP`, `VACUUM`은 현장 용어로 유지했다.
- `Pressure`, `Temp`, `Film`, `Recipe`, `Cycle`, `Valve`, 단위도 공정/계측 용어로 유지했다.
- Timeline debug table의 column 이름과 engineering trace payload는 개발자용 진단 데이터이므로 번역하지 않았다.
- 알람 작업지시서 본문은 기술 용어와 한국어 조사가 많이 섞여 있다. 이번 Goal은 UI chrome과 상태 라벨로 범위를 제한하고 본문 품질은 다음 Goal로 분리했다.
- 별도 `.resx` 또는 localization service는 추가하지 않았다. 현재 한 화면과 두 언어 규모에서는 기존 `L()` 방식이 더 단순하다.

## 6. 소프트웨어 아키텍처 설명

```text
ToggleLanguageCommand
  -> useKorean 변경
  -> OnPropertyChanged(string.Empty)
  -> ViewModel의 L(영어, 한국어) 속성 다시 계산
  -> XAML Binding이 화면 문자열 갱신
```

- XAML은 어느 언어를 선택할지 판단하지 않는다.
- ViewModel이 언어 상태와 표시 문자열을 소유한다.
- Core 공정 로직과 알람 계산은 언어를 전혀 모른다.
- 문자열을 바꿔도 Reset/Step/Fault Replay 동작과 공정 결과는 변하지 않는다.

이 구조에서 XAML에 `Text="CURRENT STEP"`처럼 고정 문자열을 쓰면 언어 전환 알림을 받아도 화면이 바뀌지 않는다. `Text="{Binding CurrentStepLabel}"`로 연결해야 ViewModel이 계산한 현재 언어 문구를 표시할 수 있다.

## 7. 유지보수할 때 봐야 할 파일

- 한국어/영어 문자열과 언어 전환: `src/EquipmentTwin.Hmi.Wpf/ViewModels/OperatorConsoleViewModel.cs`
- 화면에서 문자열을 사용하는 위치: `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml`
- 캡처 언어 인자: `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml.cs`
- 한/영 대표 화면 검증: `scripts/Capture-WpfDemoScreenshots.ps1`
- 알람 작업지시서 원문: `alarm-guides/moly-ald-alarm-guides.json`

## 8. 사용자가 이해해야 할 개념

- `hard-coded string`: XAML이나 코드에 `START`처럼 직접 적은 고정 문자열이다. 언어 전환 상태와 연결되지 않는다.
- `Binding`: XAML 화면 요소가 ViewModel 속성 값을 표시하도록 연결하는 WPF 기능이다.
- `computed property`: 별도 필드에 저장하지 않고 현재 상태로 계산해 반환하는 속성이다. `StartButtonText`는 `useKorean` 값에 따라 결과가 달라진다.
- `PropertyChanged`: ViewModel 값이 바뀌었으니 XAML이 다시 읽어야 한다고 알리는 이벤트다.
- `OnPropertyChanged(string.Empty)`: 특정 속성 하나가 아니라 모든 binding 속성을 다시 읽게 하는 방식이다. 언어 전환처럼 많은 표시 문자열이 동시에 바뀔 때 사용한다.
- `localization boundary`: 무엇을 번역하고 무엇을 원래 용어로 유지할지 정한 경계다. 이번에는 일반 UI와 상태는 번역하고 장비/공정/개발자 용어는 유지했다.
- `UI chrome`: 버튼, 탭, 제목, 상태 배지처럼 실제 데이터 바깥에서 화면 사용법을 알려주는 공통 UI 요소다.

## 9. 다음 작업

- `Goal 069: 알람 작업지시서 한국어 문장 품질 정리`
- 알람 요약, 체크리스트, 대응 선택지, escalation 문장을 자연스러운 한국어로 다듬는다.
- `Valve`, `Recipe`, `ALD`, `Interlock`, `Trace` 같은 현장/기술 용어는 유지하고 조사와 서술어만 자연스럽게 정리한다.
