# 목표 060: WPF 알람/리포트/서버 전송 Stepper UI

작성일: 2026-07-10

## 목표

오퍼레이터가 알람 발생 후 리포트와 서버 전송까지 어느 단계에 있는지 한눈에 볼 수 있도록 `알람/리포트` 탭에 단계형 UI를 추가한다.

## 완료 기준

- `알람/리포트` 탭 상단에 6단계 workflow가 표시된다.
- 단계는 알람, 체크, 대응, 리포트, 대기열, 전송으로 구성된다.
- 각 단계는 현재 상태와 색상을 가진다.
- 기존 체크리스트, 대응 선택, 리포트 저장, 서버 대기열 저장, Mock Server 전송 기능은 유지한다.
- WPF build가 통과한다.
- 전체 solution build와 Core tests가 통과한다.

## 구현 내용

- `src/EquipmentTwin.Hmi.Wpf/ViewModels/OperatorConsoleViewModel.cs`
  - workflow stepper 표시용 computed property를 추가했다.
  - 체크리스트 완료, 대응 선택, 리포트 저장, 서버 대기열 저장, Mock Server 전송 성공 상태를 표시한다.
  - 새 fault replay 또는 정상 timeline load 시 이전 report/server workflow 표시 상태를 초기화한다.
- `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml`
  - `알람/리포트` 탭 상단에 6개 step card를 추가했다.

## Workflow 단계

```text
1 ALARM   → active alarm guide loaded
2 CHECK   → required checklist confirmed
3 RESPONSE → response choice selected
4 REPORT  → issue report exported
5 QUEUE   → server outbox payload queued
6 SEND    → mock server received payload
```

## 검증

통과:

```powershell
dotnet build .\src\EquipmentTwin.Hmi.Wpf\EquipmentTwin.Hmi.Wpf.csproj -c Release
dotnet build .\EquipmentTwinLab.sln -c Release
dotnet run --project .\tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj
```

결과:

- WPF Release build: warning 0, error 0
- Solution Release build: warning 0, error 0
- Core tests: 전체 통과

## 다음 작업

```text
목표 061: 실제 화면 기준 stepper와 schematic 시각 QA
```

이유:

- 이번 작업은 기능/빌드 검증은 완료했다.
- 화면 배치, 카드 크기, 스크롤 위치는 실제 WPF 화면 screenshot 기준으로 미세 조정해야 한다.
