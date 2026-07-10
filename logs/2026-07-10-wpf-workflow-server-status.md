# 2026-07-10 — Mock Server 상태와 Workflow 전송 단계 연결

## 1. 한 일

- 알람 workflow의 6번 전송 카드가 Mock Server health 상태를 표시하도록 연결했다.
- 전송 카드의 상태 우선순위를 정의했다.

```text
전송 성공
  → 전송 중
  → 서버 확인 중
  → OFFLINE
  → ONLINE
  → 대기
```

- ONLINE 상태는 파란색, OFFLINE 상태는 빨간색, 실제 전송 성공은 초록색으로 구분했다.
- 서버는 켜져 있지만 payload가 없을 때와 payload가 준비됐을 때의 안내 문구를 나눴다.
- 영어와 한국어 상태 문구를 모두 검증했다.

## 2. 바뀐 파일

- `src/EquipmentTwin.Hmi.Wpf/ViewModels/OperatorConsoleViewModel.cs`: 전송 단계의 상태, 상세 문구, 색상 판단
- `goals/063-wpf-workflow-server-status.md`: 목표와 완료 기준
- `logs/2026-07-10-wpf-workflow-server-status.md`: 작업 과정과 유지보수 설명
- `state/loop-state.md`: Goal 완료 상태와 다음 작업
- `state/triage.md`: UI 우선순위 갱신

로컬 검증용 파일은 `.gitignore`가 적용되는 `artifacts/goal-063-workflow-state-test/`에 만들었으며 공개 커밋에는 포함하지 않는다.

## 3. 검증 결과

- WPF Release build: 통과, 경고 0, 오류 0
- 전체 solution Release build: 통과, 경고 0, 오류 0
- Core tests: 전체 통과
- 상태 전이 직접 검증: 전체 통과
  - 초기 `WAIT`
  - 확인 중 `CHECKING`
  - 연결 실패 `OFFLINE`, 빨간색
  - 연결 가능 `ONLINE`, 파란색
  - 전송 성공 `HTTP OK`, 초록색
  - 한국어 `전송 성공`, `Mock Server 수신`
- `git diff --check`: 통과

## 4. 막힌 점과 해결 방법

### Windows PowerShell 5.1에서 .NET 8 WPF 어셈블리 로드 실패

- 증상: `System.Runtime, Version=8.0.0.0`을 찾지 못해 ViewModel reflection 검증이 시작되지 않았다.
- 원인: Windows PowerShell 5.1은 .NET Framework 기반이라 .NET 8 WPF 어셈블리를 직접 실행할 수 없다.
- 해결: `net8.0-windows`를 대상으로 하는 임시 console 검증 프로그램을 사용했다.
- 결과: 같은 .NET 8 런타임에서 모든 표시 상태와 색상을 직접 읽어 검증했다.

현재 남은 코드 blocker는 없다.

## 5. 보류한 판단

- Mock Server 자동 polling은 추가하지 않았다.
- ViewModel 전용 테스트 프로젝트를 새로 만들지 않았다. 현재는 작은 표시 로직이므로 임시 .NET 8 검증으로 확인했다.
- health check 실패 시 자동 재시도하지 않는다.
- 실제 장비 서버의 장애 판단이나 네트워크 복구 정책으로 표현하지 않는다. 현재 기능은 local demo Mock Server 상태 표시다.

## 6. 소프트웨어 아키텍처 설명

이번 변경은 ViewModel의 표시 판단만 수정했다.

```text
MockServerPayloadSender
  → health/send 결과 생성
OperatorConsoleViewModel
  → 결과를 Workflow 상태/문구/색상으로 변환
MainWindow.xaml
  → 기존 바인딩으로 그대로 표시
```

Service와 XAML은 수정하지 않았다. 서버 호출 방법과 화면 배치를 바꾸지 않고, 두 영역 사이의 상태 의미만 일치시킨 것이다.

## 7. 유지보수할 때 봐야 할 파일

- 전송 단계 상태 우선순위: `OperatorConsoleViewModel.WorkflowSendStepStatus`
- 전송 단계 안내 문구: `OperatorConsoleViewModel.WorkflowSendStepDetail`
- 전송 단계 색상: `OperatorConsoleViewModel.WorkflowSendStepBrush`
- health 상태가 알려졌는지 판단: `OperatorConsoleViewModel.HasKnownMockServerHealth`
- 실제 HTTP health/send 처리: `Services/MockServerPayloadSender.cs`

## 8. 사용자가 이해해야 할 개념

- `상태 우선순위`: 여러 상태가 동시에 참일 수 있을 때 화면에 무엇을 먼저 보여줄지 정하는 규칙이다.
- `computed property`: 별도 값을 저장하지 않고 현재 field들을 조합해 계산하는 속성이다.
- `표시 상태와 업무 결과의 차이`: ONLINE은 현재 서버가 살아 있다는 뜻이고, HTTP OK는 이 workflow의 payload가 실제 전송됐다는 뜻이다.
- `runtime mismatch`: 코드가 같아도 실행 환경의 .NET 세대가 다르면 어셈블리를 불러오지 못할 수 있다.
- `reflection 검증`: private field를 테스트 상황으로 설정해 public property 결과를 읽는 방식이다. 이번에는 영구 설계가 아니라 일회성 검증에만 사용했다.

## 9. 다음 작업

- `Goal 064: fault code별 schematic 진단 라인 강조`
- GAS-301이면 Gas Box/가스 공급 라인, VAC 계열이면 Exhaust/Pump, TMP 계열이면 heater 영역처럼 문제 위치를 화면에서 바로 찾게 한다.
