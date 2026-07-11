# 2026-07-10 — WPF Stepper/Server/Schematic 시각 QA

## 1. 한 일

- WPF를 오퍼레이터 콘솔 용도에 맞게 최대화 상태로 시작하도록 했다.
- 하단 debug 영역 높이를 고정값에서 `0.28*`, 최소 160px, 최대 220px의 반응형 행으로 바꿨다.
- 왼쪽 schematic의 Gas Box/현재 Step 고정 높이와 chamber 내부 여백을 줄여 chamber가 눌리는 문제를 완화했다.
- 알람/리포트 탭의 중첩 ScrollViewer를 제거해 탭 전체가 하나의 세로 스크롤만 사용하도록 했다.
- workflow detail과 리포트/payload 경로를 한 줄 말줄임으로 표시하고 전체 값은 ToolTip으로 확인하도록 했다.
- Mock Server 상태 카드에서 상태 메시지를 버튼 아래 전체 폭으로 배치했다.
- payload JSON 미리보기 높이를 112px에서 92px로 줄였다.

## 2. 바뀐 파일

- `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml`: 창 시작 상태, Grid 행 크기, schematic 여백, 스크롤, stepper, server card 배치
- `goals/062-wpf-stepper-server-schematic-visual-qa.md`: 목표와 완료 기준
- `logs/2026-07-10-wpf-stepper-server-schematic-visual-qa.md`: 작업 과정과 유지보수 설명
- `state/loop-state.md`: 완료 상태와 다음 작업
- `state/triage.md`: UI 우선순위 갱신

## 3. 검증 결과

- WPF Release build: 통과, 경고 0, 오류 0
- 전체 solution Release build: 통과, 경고 0, 오류 0
- Core tests: 전체 통과
- `git diff --check`: 통과

## 4. 막힌 점과 해결 방법

### WPF 자동 screenshot 캡처 실패

- 증상: Windows 화면 캡처 중 `SetIsBorderRequired failed: 해당 인터페이스를 지원하지 않습니다. (0x80004002)` 오류가 발생했다.
- 영향: Goal 061 이후 화면의 새 screenshot을 자동으로 저장하지 못했다.
- 대응: 사용자가 제공한 기존 실제 화면과 현재 XAML의 고정 높이/스크롤 구조를 대조해 구조적으로 확인 가능한 문제만 수정했다.
- 남은 확인: 사용자가 최신 빌드를 한 번 실행하면 pixel 단위 간격과 글자 크기를 최종 확인할 수 있다.

### 첫 WPF build의 DLL 잠금

- 증상: 화면 확인을 위해 실행한 `EquipmentTwin.Hmi.Wpf` 프로세스가 Release `EquipmentTwin.Core.dll`을 사용해 복사 단계가 실패했다.
- 원인: 실행 중인 프로그램의 DLL은 Windows가 잠글 수 있다.
- 해결: 이번 작업에서 실행한 WPF 프로세스만 종료한 뒤 같은 build를 다시 실행했고 경고/오류 없이 통과했다.

## 5. 보류한 판단

- workflow stepper와 Mock Server health 상태를 논리적으로 연결하지 않았다. 이번 Goal은 화면 배치만 다룬다.
- 자동 health polling은 추가하지 않았다.
- 새 UI 라이브러리나 공통 card abstraction을 만들지 않았다.
- 최신 빌드의 pixel 단위 screenshot 최종 판정은 자동 캡처 제약 때문에 보류했다.

## 6. 소프트웨어 아키텍처 설명

이번 변경은 표시 계층인 WPF XAML에만 있다.

```text
EquipmentTwin.Core          변경 없음
        ↑
OperatorConsoleViewModel    변경 없음
        ↑
MainWindow.xaml             레이아웃만 변경
```

공정 로직과 서버 통신 동작을 건드리지 않았기 때문에, UI 간격을 바꿔도 ALD timeline, alarm guide, report payload 결과는 동일하다.

## 7. 유지보수할 때 봐야 할 파일

- 전체 창의 행/열 비율: `MainWindow.xaml`의 최상위 content Grid
- 왼쪽 장비 배치: `EQUIPMENT MODULE LAYOUT` 아래 Grid
- 오른쪽 탭 스크롤: `AlarmReportTabHeader` TabItem
- workflow 카드 크기/간격: `WorkflowStepperLabel` 아래 UniformGrid
- Mock Server 카드: `MockServerConnectionLabel` 바인딩이 있는 Border

## 8. 사용자가 이해해야 할 개념

- `Grid * 크기`: 남은 공간을 비율로 나누는 WPF 배치 방식이다. `0.28*`은 같은 Grid 안의 `1*` 행보다 작은 비율로 공간을 받는다.
- `MinHeight/MaxHeight`: 화면이 작거나 커져도 영역이 너무 작아지거나 너무 커지는 것을 막는 경계다.
- `중첩 ScrollViewer`: 스크롤 영역 안에 또 스크롤 영역이 있으면 마우스 휠이 어느 영역을 움직여야 하는지 사용자가 혼란스러울 수 있다.
- `TextTrimming`: 긴 문자열을 `...`으로 줄여 카드 높이를 지킨다.
- `ToolTip`: 줄인 문자열 위에 마우스를 올렸을 때 전체 값을 보여준다.
- `DLL 잠금`: 실행 중인 프로그램이 DLL을 사용하면 빌드가 같은 파일을 교체하지 못할 수 있다. 디버깅 중 재빌드가 안 될 때 먼저 실행 중인 앱을 확인한다.

## 9. 다음 작업

- 최신 WPF 화면에서 한 번 더 육안 확인한다.
- 다음 기능 Goal은 Mock Server 상태와 workflow 6단계 전송 상태를 연결해, 서버가 꺼졌을 때 전송 단계가 명확히 `OFFLINE`으로 보이게 하는 작업이 적절하다.
