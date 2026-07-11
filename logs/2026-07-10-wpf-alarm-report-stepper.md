# 2026-07-10 WPF 알람/리포트/서버 전송 Stepper UI

## 1. 한 일

- 오른쪽 `알람/리포트` 탭 상단에 workflow stepper를 추가했다.
- 오퍼레이터가 알람 처리 흐름에서 현재 어디까지 진행했는지 볼 수 있게 했다.
- 기존 버튼과 체크리스트 동작은 유지했다.

## 2. 바뀐 파일

- `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml`
- `src/EquipmentTwin.Hmi.Wpf/ViewModels/OperatorConsoleViewModel.cs`
- `goals/060-wpf-alarm-report-stepper.md`
- `logs/2026-07-10-wpf-alarm-report-stepper.md`
- `state/loop-state.md`
- `state/triage.md`

## 3. 단계 구성

```text
알람 → 체크 → 대응 → 리포트 → 대기열 → 전송
```

각 단계는 다음 정보를 보여준다.

- 단계 이름
- 현재 상태
- 세부 정보
- 상태 색상

## 4. 소프트웨어적으로 알아야 할 점

이번 작업은 새로운 서버 기능을 만든 것이 아니다.

기존 ViewModel이 이미 가지고 있던 상태를 읽어서 화면에 표현한 작업이다.

예:

- `activeAlarmGuide`가 있으면 알람 guide가 로드된 상태
- required checklist가 모두 체크되면 체크 완료
- `selectedAlarmGuideChoice`가 있으면 대응 선택 완료
- `lastIssueReportPath`가 있으면 리포트 저장 완료
- `lastServerOutboxPath`가 있으면 서버 대기열 저장 완료
- `lastMockServerSendSucceeded`가 true면 Mock Server 전송 성공

## 5. 보류한 판단

- Stepper card 디자인은 1차 구현이다.
- 실제 화면에서 카드가 너무 좁거나 글자가 잘리면 다음 작업에서 조정한다.
- 실제 HTTP server 상태 health check는 아직 하지 않았다.
- 지금은 “마지막 전송 결과” 기준으로 전송 단계를 표시한다.

## 6. 검증

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

## 7. 다음 작업

추천:

```text
목표 061: 실제 화면 기준 stepper와 schematic 시각 QA
```

사용자가 WPF 화면 screenshot을 보내면 stepper 크기, 스크롤 위치, schematic 균형을 조정한다.
