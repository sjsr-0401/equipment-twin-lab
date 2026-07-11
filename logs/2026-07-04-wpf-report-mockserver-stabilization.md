# 2026-07-04 WPF 리포트 / Mock Server 안정화

## 1. 한 일

- 현재 WPF alarm guide / issue report / server outbox / mock server 흐름을 기능 추가 없이 검증했다.
- README와 architecture 문서가 최신 WPF 대표 흐름을 설명하도록 맞춘 뒤, 실제 build/test/server endpoint 검증을 수행했다.
- 다음 기능을 바로 추가하기 전에 현재 변경분이 깨지지 않는지 확인했다.

## 2. 바뀐 파일

이번 안정화 기록으로 추가/수정한 파일:

- `goals/054-wpf-report-mockserver-stabilization.md`
- `logs/2026-07-04-wpf-report-mockserver-stabilization.md`
- `state/loop-state.md`
- `state/triage.md`

이미 이번 작업 흐름에서 바뀌어 있던 주요 구현 파일:

- `src/EquipmentTwin.Hmi.Wpf/ViewModels/OperatorConsoleViewModel.cs`
- `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml`
- `src/EquipmentTwin.Hmi.Wpf/Services/AlarmIssueReportExportService.cs`
- `src/EquipmentTwin.Hmi.Wpf/Services/AlarmIssueReportOutboxService.cs`
- `src/EquipmentTwin.Hmi.Wpf/Services/MockServerPayloadSender.cs`
- `src/EquipmentTwin.MockServer/Program.cs`
- `docs/mock-server.md`
- `docs/architecture.md`
- `README.md`

## 3. 검증 결과

통과:

```powershell
dotnet build .\EquipmentTwinLab.sln
dotnet run --project .\tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj
dotnet build .\src\EquipmentTwin.Hmi.Wpf\EquipmentTwin.Hmi.Wpf.csproj
dotnet build .\src\EquipmentTwin.MockServer\EquipmentTwin.MockServer.csproj
```

결과:

- solution build: warning 0, error 0
- Core tests: 전체 통과
- WPF build: warning 0, error 0
- Mock Server build: warning 0, error 0

Mock Server 실제 endpoint 검증:

```text
GET  /health                 → status ok
POST /alarm-issue-report     → status received
received file exists         → True
```

수신 파일은 `artifacts/mock-server-received/` 아래에 생성됐다.

## 4. 막힌 점과 해결 방법

- 처음 `http://127.0.0.1:5088/health` 확인 시 mock server가 꺼져 있어서 연결 실패가 났다.
- mock server를 숨김 프로세스로 실행한 뒤 health check와 POST를 다시 검증했다.
- 검증 명령을 입력하는 과정에서 잘못된 `dotnet run` 인자를 한 번 실행했다. 해당 프로세스는 남지 않았고 5088 포트도 열리지 않았다. 이후 올바른 명령으로 재검증했다.

## 5. 보류한 판단

- WPF 버튼 클릭 자동화는 하지 않았다.
- 이유: 현재 목표는 안정화이며, WPF UI 자동화까지 추가하면 범위가 커진다.
- 대신 WPF는 build로 확인했고, 서버 endpoint는 실제 HTTP POST로 확인했다.
- `unity/EquipmentTwin.Unity/ProjectSettings/PackageManagerSettings.asset`는 untracked 상태다. Unity editor-generated 설정으로 보이며, 이번 안정화 범위에서는 포함 여부를 판단하지 않았다.

## 6. 소프트웨어 아키텍처 설명

현재 흐름은 아래처럼 책임이 나뉜다.

```text
EquipmentTwin.Core
  공정 실행, 알람 코드, 알람 가이드 데이터

EquipmentTwin.Hmi.Wpf
  operator 화면, checklist, response 선택, report export, outbox 생성

EquipmentTwin.MockServer
  WPF가 만든 payload를 받는 local HTTP server

artifacts/
  실행 결과물 저장 위치
```

중요한 점은 Core가 WPF나 Mock Server를 참조하지 않는다는 것이다. Core는 장비 도메인 로직만 갖고, WPF와 Mock Server는 바깥 adapter 역할을 한다.

## 7. 유지보수할 때 봐야 할 파일

- 알람 가이드 데이터: `alarm-guides/moly-ald-alarm-guides.json`
- 알람 가이드 로딩: `src/EquipmentTwin.Hmi.Wpf/Services/AlarmResponseGuideService.cs`
- WPF 화면 상태: `src/EquipmentTwin.Hmi.Wpf/ViewModels/OperatorConsoleViewModel.cs`
- WPF 화면 배치: `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml`
- report export: `src/EquipmentTwin.Hmi.Wpf/Services/AlarmIssueReportExportService.cs`
- server outbox 생성: `src/EquipmentTwin.Hmi.Wpf/Services/AlarmIssueReportOutboxService.cs`
- mock server 전송: `src/EquipmentTwin.Hmi.Wpf/Services/MockServerPayloadSender.cs`
- mock server 수신: `src/EquipmentTwin.MockServer/Program.cs`

## 8. 사용자가 이해해야 할 개념

- `Build`: 코드가 컴파일되고 프로젝트 참조가 깨지지 않았는지 확인하는 단계.
- `Core test`: UI 없이 장비 로직이 기대대로 동작하는지 확인하는 단계.
- `Mock Server`: 실제 회사 서버 대신, 내 컴퓨터에서 HTTP 요청을 받아보는 테스트용 서버.
- `Outbox`: 서버로 보낼 데이터를 바로 날리지 않고 파일로 먼저 저장해두는 경계.
- `Adapter`: Core 바깥에서 UI, HTTP, 파일 저장 같은 외부 연결을 담당하는 계층.

## 9. 다음 작업

추천 다음 작업:

```text
목표 055: WPF HMI 계측값 Trend Panel
```

압력, 온도, 막두께가 시간에 따라 어떻게 변했는지 작은 trend panel로 보여준다. 이 기능은 operator가 현재값뿐 아니라 공정 흐름을 이해하는 데 도움이 된다.
