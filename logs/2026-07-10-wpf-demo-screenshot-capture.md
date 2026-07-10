# 2026-07-10 — WPF Demo 상태 자동 Screenshot Capture

## 1. 한 일

- WPF 실행 인자로 캡처 모드를 추가했다.
- 다음 5개 대표 상태를 자동 재현하도록 만들었다.
  - `load`: Wafer Load, Gate Open
  - `process`: Pump Down, Wafer In Chamber
  - `alarm`: Precursor Dose Timeout, Wafer Chamber Hold
  - `transfer-out`: Wafer가 Load Port 방향으로 이동
  - `complete`: Wafer가 Load Port로 복귀하고 공정 완료
- `alarm` 캡처는 자동으로 `알람/리포트` 탭을 열어 작업지시서와 대응 단계가 보이게 했다.
- 한국어(`ko`)와 영어(`en`) 캡처를 선택할 수 있게 했다.
- 5개 상태를 한 번에 실행하고 PNG 존재 여부와 1600×900 해상도를 검사하는 PowerShell 스크립트를 추가했다.

## 2. 바뀐 파일

- `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml`
  - 캡처할 루트 화면에 `RootLayout` 이름을 지정했다.
  - 알람 캡처에서 탭을 선택할 수 있도록 `WorkspaceTabs` 이름을 지정했다.
- `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml.cs`
  - 캡처 실행 인자 해석
  - 기존 ViewModel 명령을 사용한 대표 상태 준비
  - 1600×900 창 고정
  - `RenderTargetBitmap` PNG 저장
  - 캡처 완료 후 자동 종료
- `scripts/Capture-WpfDemoScreenshots.ps1`
  - Release build
  - 5개 WPF 프로세스 실행
  - 파일 생성과 해상도 검증
- `goals/066-wpf-demo-screenshot-capture.md`
- `logs/2026-07-10-wpf-demo-screenshot-capture.md`
- `state/loop-state.md`
- `state/triage.md`

## 3. 상태를 어떻게 만들었나

별도의 가짜 화면 데이터를 만들지 않았다. 이미 사용자가 누르는 명령을 자동으로 실행했다.

```text
load
  -> Reset

process
  -> Reset -> Step 1회

alarm
  -> Reset -> precursor-dose-timeout 선택 -> Fault Replay

transfer-out
  -> Reset -> 끝에서 두 번째 Step까지 전진

complete
  -> Reset -> 마지막 Step까지 전진
```

이 방식의 장점은 화면 캡처와 사용자의 실제 조작 경로가 같은 코드를 사용한다는 것이다. UI 모양만 맞추기 위한 별도 상태를 만들면 실제 동작과 캡처가 달라질 수 있는데, 이번 구현은 그 차이를 만들지 않았다.

## 4. 검증 결과

- WPF Release build: 통과, 경고 0, 오류 0
- 자동 캡처: 5개 상태 모두 성공
- PNG 해상도: 5개 모두 1600×900
- 직접 이미지 확인:
  - Load 화면의 Wafer 이송 시작과 Gate Open 확인
  - Alarm 화면의 GAS-301 Gas Box 강조, Chamber Hold, 알람 작업지시서 확인
  - Transfer Out 화면의 Load Port 방향 Wafer 이동과 Gate Open 확인
- 전체 solution Release build: 통과
- Core tests: 전체 통과
- 일반 WPF 실행 smoke test: 통과
- `git diff --check`: 통과

## 5. 막힌 점, 실패한 시도, 해결

### 첫 번째 문제 — 알람 캡처 일부가 검게 비었다

처음에는 창이 한 번 그려진 뒤 `FaultReplayCommand`로 알람 상태를 바꾸고 바로 캡처했다. 알람 상태는 많은 바인딩과 패널을 한꺼번에 갱신하므로 `RenderTargetBitmap`에 일부 Visual이 비어 저장됐다.

### 실패한 시도 — 250ms 기다린 뒤 다시 캡처

렌더링 시간을 더 주면 해결될 것으로 보고 250ms 대기와 `UpdateLayout` 재호출을 추가했다. 그러나 알람 탭을 선택하는 경우에는 같은 문제가 다시 발생했다. 단순 지연은 컴퓨터 속도에 따라 결과가 달라질 수 있어 근본 해결이 아니었다.

### 최종 해결

- 창이 처음 그려지기 전에 ViewModel 상태를 준비했다.
- 알람 캡처는 첫 렌더 전에 `알람/리포트` 탭을 선택했다.
- 캡처 모드에서만 WPF 소프트웨어 렌더링을 강제했다.

그 결과 정상 화면과 알람 상세 탭이 모두 빠짐없이 저장됐다. 일반 실행은 기존 하드웨어 렌더링 설정을 유지한다.

## 6. 소프트웨어 아키텍처 설명

```text
Core ALD timeline
  -> OperatorConsoleViewModel의 기존 Command
  -> MainWindow 캡처 상태 준비
  -> XAML binding으로 완성된 Visual Tree
  -> RenderTargetBitmap
  -> PNG
```

- Core는 화면 캡처를 전혀 모른다.
- ViewModel도 PNG 파일을 모른다.
- `MainWindow`는 화면 상태 준비와 렌더링만 담당한다.
- PowerShell은 여러 상태 실행과 파일 검증만 담당한다.

이 의존 방향 덕분에 캡처 기능을 추가해도 장비 공정 로직을 변경하지 않았다.

## 7. 유지보수할 때 볼 파일

- 캡처 인자와 상태 매핑: `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml.cs`
- 캡처 대상 전체 화면: `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml`의 `RootLayout`
- 알람용 선택 탭: 같은 파일의 `WorkspaceTabs`
- 일괄 실행과 해상도 검사: `scripts/Capture-WpfDemoScreenshots.ps1`
- 실제 ALD Step 목록: `src/EquipmentTwin.Core/Processes/MolyAldProcessStep.cs`
- 화면 명령: `src/EquipmentTwin.Hmi.Wpf/ViewModels/OperatorConsoleViewModel.cs`

## 8. 주니어가 이해해야 할 개념

- `RenderTargetBitmap`: WPF 화면을 모니터 사진이 아니라 메모리 bitmap으로 그려 파일로 저장하는 클래스다.
- `Visual Tree`: Window 안에 들어 있는 Grid, Border, TextBlock 같은 화면 요소의 계층 구조다.
- `ContentRendered`: Window 내용이 처음 화면에 그려진 뒤 발생하는 이벤트다.
- `Dispatcher`: WPF UI 작업을 UI thread의 적절한 순서에서 실행하게 하는 도구다.
- `software rendering`: GPU 캐시 대신 CPU 경로로 Visual을 다시 그린다. 자동 캡처의 일관성을 위해 캡처 모드에만 사용했다.
- `smoke test`: 모든 기능을 깊게 검사하는 것이 아니라 프로그램이 시작되고 즉시 죽지 않는지 확인하는 최소 실행 검사다.
- `deterministic capture`: 실행할 때마다 같은 상태와 같은 크기로 결과가 나오는 캡처다.

## 9. 보류한 판단

- PNG 픽셀 전체를 이전 파일과 비교하는 visual regression은 아직 추가하지 않았다.
- 캡처 PNG는 저장소 용량을 늘리지 않도록 Git에 올리지 않았다.
- 동영상 녹화는 자동화하지 않았다.
- 모든 fault 종류의 screenshot matrix는 만들지 않았다. 현재 대표 알람은 GAS-301 하나다.

## 10. 다음 작업

- `Goal 067: 자동 Screenshot 기반 WPF Visual QA 2차`
- 5개 대표 화면을 기준으로 잘림, 번역 혼용, 정보 우선순위, 상태별 강조가 적절한지 비교하고 필요한 UI 수정만 수행한다.
