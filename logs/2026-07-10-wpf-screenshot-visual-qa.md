# 2026-07-10 — 자동 Screenshot 기반 WPF Visual QA 2차

## 1. 한 일

- Goal 066에서 만든 WPF 자동 캡처 5종을 반복 실행해 부분 렌더링 문제를 재현했다.
- 캡처 모드의 WPF 창을 고정 DIP 크기에서 최대화 방식으로 변경했다.
- WPF 내부 렌더 버퍼가 아니라 화면에 최종 합성된 client 영역을 저장하는 `WindowScreenshotService`를 추가했다.
- 현재 모니터의 작업 영역만 캡처해 Windows 작업표시줄을 제외했다.
- 캡처 이미지를 1600×900으로 고품질 축소한다.
- PowerShell 검증을 해상도 확인에서 다음 항목까지 확장했다.
  - 순수 검정 픽셀 격자 표본 검사
  - 상단 제목 영역 확인
  - 왼쪽 장비 영역 확인
  - 오른쪽 조작/상태 영역 확인
  - 하단 debug/trace 영역 확인
- 한국어 5종과 영어 5종을 모두 다시 생성하고 검증했다.
- 자동 캡처 결과를 기준으로 다음 UI 작업의 범위를 정리했다.

## 2. 바뀐 파일

- `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml`
  - 더 이상 사용하지 않는 캡처용 `RootLayout` 이름을 제거했다.
  - 알람 캡처에서 사용하는 `WorkspaceTabs` 이름은 유지했다.
- `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml.cs`
  - 캡처 언어를 첫 DataContext 표시 전에 적용한다.
  - 캡처 모드에서만 창을 최대화하고 최상단에 둔다.
  - 기존 Reset/Step/Fault Replay 명령으로 상태를 준비한 뒤 화면 합성을 기다린다.
  - `WindowScreenshotService`에 PNG 저장을 위임한다.
- `src/EquipmentTwin.Hmi.Wpf/Services/WindowScreenshotService.cs`
  - WPF client 좌표를 실제 화면 좌표로 변환한다.
  - 현재 모니터의 작업 영역까지만 `CopyFromScreen`으로 읽는다.
  - 결과를 1600×900 PNG로 정규화한다.
- `src/EquipmentTwin.Hmi.Wpf/EquipmentTwin.Hmi.Wpf.csproj`
  - `System.Drawing` 화면 캡처와 `Screen.WorkingArea` 사용을 위해 Windows Forms 지원을 활성화했다.
  - WPF의 `Application`, `Point`, `Size`와 이름이 충돌하지 않도록 Windows Forms/Drawing 암시적 using은 제거했다.
- `scripts/Capture-WpfDemoScreenshots.ps1`
  - 캡처 창을 숨기지 않도록 변경했다.
  - 검정 표본과 네 개 화면 anchor 검사를 추가했다.
- `goals/067-wpf-screenshot-visual-qa.md`
- `logs/2026-07-10-wpf-screenshot-visual-qa.md`
- `plan.md`
- `state/loop-state.md`
- `state/triage.md`

## 3. 검증 결과

- 한국어 자동 캡처 5종: 모두 성공
  - 해상도: 모두 1600×900
  - 순수 검정 격자 표본: 모두 `0/1200`
  - 핵심 영역: 모두 `4/4`
- 영어 자동 캡처 5종: 모두 성공
  - 해상도: 모두 1600×900
  - 순수 검정 격자 표본: 모두 `0/1200`
  - 핵심 영역: 모두 `4/4`
- 전체 solution Release build: 통과
  - 경고 0개
  - 오류 0개
- Core tests: 전체 통과
- 일반 WPF smoke test: 3초 동안 정상 실행 유지
- 캡처 PNG는 `artifacts/wpf-demo-screenshots/`에만 생성됨

## 4. 막힌 점과 해결 방법

### 문제 1 — 고정 크기의 단위가 실제 픽셀이 아니었다

WPF의 `Width=1600`, `Height=900`은 실제 모니터 픽셀이 아니라 DIP(Device Independent Pixel)다. 현재 Windows 배율이 160%이므로 1600 DIP는 실제 약 2560 pixel이 된다. 여기에 작업표시줄과 window chrome 공간까지 필요해 화면 높이를 넘겼고, 일부 렌더링 경로가 불안정해졌다.

해결:

- 캡처 모드에서 창을 현재 화면 크기에 맞게 최대화했다.
- 캡처가 끝난 뒤 최종 PNG만 1600×900 pixel로 정규화했다.

### 실패 1 — `RenderTargetBitmap` 재사용

WPF Visual Tree를 메모리 bitmap으로 직접 그리는 기존 방식을 유지하면서 대기 시간과 `UpdateLayout` 호출을 늘렸다. 하지만 상태 변경과 탭 전환 후 일부 Visual이 계속 비었다.

판단:

- 단순 대기 시간 증가는 PC 속도에 따라 결과가 달라진다.
- 이번 화면은 많은 binding, ScrollViewer, DataGrid를 포함하므로 내부 렌더 버퍼 하나만 신뢰하기 어려웠다.

### 실패 2 — `VisualBrush`로 다시 그리기

Root Visual을 `VisualBrush`로 복사해 별도 DrawingVisual에 그렸지만 같은 유형의 부분 누락이 남았다.

### 실패 3 — `PrintWindow`와 강제 다시 그리기

Win32 `PrintWindow`, `RedrawWindow`, 1초 대기, 창 활성화를 조합했다. 일반 화면은 잡혔지만 상태에 따라 WPF 자식 컨트롤 일부가 누락됐다.

### 실패 4 — 초기 화면과 상태 화면 합성

초기 화면을 먼저 저장하고 상태 화면의 검정 영역만 초기 화면으로 채우는 방식을 시험했다. 몇 상태는 개선됐지만, 이전 상태가 섞일 가능성이 있고 `transfer-out`에서 일관되지 않았다. 유지보수하기 어려운 우회라 제거했다.

### 최종 해결 — 사용자가 보는 최종 화면을 캡처

캡처 창을 최대화·최상단으로 표시한 뒤 `Graphics.CopyFromScreen`으로 실제 client 영역을 읽었다. 이 방법은 Windows가 WPF 렌더링을 끝낸 결과를 가져오므로 WPF 내부 dirty region이나 GPU cache 상태에 덜 의존한다.

추가로 `Screen.WorkingArea`를 적용해 작업표시줄이 PNG에 섞이지 않게 했다.

### 검증 도중 발견한 이미지 미리보기 차이

일부 이미지 미리보기에서 어두운 UI 영역이 검게 축약되어 보였다. 원본 PNG의 동일 영역을 pixel 단위로 비교하니 상단 제목과 조작 영역 데이터가 정상적으로 존재했다. 그래서 육안 한 번만 믿지 않고, 원본 bitmap을 다시 읽어 네 영역의 밝은 픽셀 수를 자동 검사하도록 했다.

## 5. 보류한 판단

- 정상 기준 PNG를 Git에 저장하고 모든 픽셀을 비교하는 golden-image regression은 추가하지 않았다.
- 전체 화면은 시계와 trace 시간처럼 실행마다 달라지는 값이 있어 단순한 100% pixel 비교가 적합하지 않다.
- 모든 fault scenario를 캡처하지 않았다. 현재 대표 알람은 GAS-301이다.
- 자동 캡처는 실제 화면을 사용하므로 실행 중 창이 잠깐 표시된다. 완전한 무인 CI 캡처는 별도 virtual display 환경이 필요하다.
- 이번 Goal에서는 번역 문구나 HMI layout을 넓게 수정하지 않았다. 먼저 캡처 기반이 신뢰 가능해야 UI 변경 전후를 비교할 수 있기 때문이다.

## 6. 소프트웨어 아키텍처 설명

```text
Core 공정 timeline
  -> OperatorConsoleViewModel의 기존 Command
  -> MainWindow가 대표 상태 준비
  -> WPF가 실제 화면에 렌더링
  -> WindowScreenshotService가 client 영역 캡처
  -> 1600×900 PNG
  -> PowerShell이 5개 상태와 시각 anchor 검증
```

- Core는 화면 캡처를 모른다.
- ViewModel은 PNG 저장을 모른다.
- `MainWindow`는 캡처 모드의 상태 준비와 생명주기만 담당한다.
- `WindowScreenshotService`는 Windows 좌표와 bitmap 처리만 담당한다.
- PowerShell은 반복 실행과 결과 판정만 담당한다.

`System.Drawing`과 Windows Forms 지원을 켰지만, WPF UI를 Windows Forms로 바꾼 것은 아니다. `Screen.WorkingArea`와 bitmap API만 빌려 쓴다.

## 7. 유지보수할 때 봐야 할 파일

- 캡처 인자와 상태 매핑: `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml.cs`
- 실제 화면 좌표/PNG 저장: `src/EquipmentTwin.Hmi.Wpf/Services/WindowScreenshotService.cs`
- 캡처할 알람 탭: `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml`의 `WorkspaceTabs`
- 상태 목록과 자동 판정 기준: `scripts/Capture-WpfDemoScreenshots.ps1`
- 실제 공정 Step: `src/EquipmentTwin.Core/Processes/MolyAldProcessStep.cs`
- 상태를 만드는 명령: `src/EquipmentTwin.Hmi.Wpf/ViewModels/OperatorConsoleViewModel.cs`

## 8. 사용자가 이해해야 할 개념

- `DIP`: WPF가 화면 크기를 표현하는 논리 단위다. Windows 배율이 100%가 아니면 DIP와 실제 pixel 수가 다르다.
- `DPI scaling`: 글씨와 UI가 고해상도 모니터에서 너무 작아지지 않도록 Windows가 논리 크기를 실제 pixel로 확대하는 기능이다.
- `client area`: window border나 외부 장식이 아니라 프로그램 내용이 그려지는 내부 영역이다.
- `CopyFromScreen`: 화면에 최종 표시된 pixel을 bitmap으로 복사하는 API다.
- `WorkingArea`: 모니터 전체에서 작업표시줄 같은 예약 영역을 제외한 사용 가능한 사각형이다.
- `anchor 검사`: 전체 pixel을 똑같다고 비교하지 않고, 반드시 보여야 하는 화면 구역에 실제 내용이 있는지 확인하는 검사다.
- `golden image`: 정상이라고 승인한 기준 이미지를 저장하고 새 결과와 비교하는 테스트 방식이다. 시간·로그처럼 계속 변하는 영역은 마스킹 규칙이 필요하다.
- `smoke test`: 프로그램이 시작되고 즉시 종료되지 않는지 확인하는 최소 실행 검사다.

## 9. 다음 작업

- `Goal 068: WPF 한국어 일반 UI 라벨 일관성 정리`
- 한국어 화면의 조작 버튼, 영역 제목, 상태 설명을 우선 정리한다.
- `Pump`, `Valve`, `Recipe`, `ALD`처럼 현장에서 그대로 쓰는 도메인 용어는 유지하고, `CURRENT STEP`, `PROCESS INSTRUMENTS`, `FAULT SCENARIO SELECTOR` 같은 일반 UI 제목은 언어 전환에 연결한다.
