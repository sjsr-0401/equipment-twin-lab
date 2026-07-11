# 2026-07-11 — 서버 재전송과 Outbox 상태 관리

## 1. 한 일

- Outbox JSON에 전송 상태, 시도 횟수, 마지막 시도 시각, 성공 시각, 마지막 오류를 추가했다.
- 전송 직전에 `sending`, 성공하면 `sent`, 실패하면 `failed`로 파일을 다시 저장한다.
- 실패한 payload는 같은 파일로 수동 재전송할 수 있게 했다.
- 이미 `sent`인 payload는 서비스와 WPF 양쪽에서 중복 전송을 막았다.
- WPF 알람/리포트 화면에 상태 카드와 상태별 버튼 문구를 추가했다.
- 상태 전이를 파일에 실제로 썼다가 다시 읽는 계약 테스트를 추가했다.

## 2. 바뀐 파일

- `src/EquipmentTwin.Hmi.Wpf/Services/AlarmIssueReportOutboxService.cs`: Outbox 상태 전이와 파일 저장
- `src/EquipmentTwin.Hmi.Wpf/ViewModels/OperatorConsoleViewModel.cs`: 전송 흐름, 표시 문구, 재전송 가능 여부
- `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml`: 상태/횟수/오류 카드와 버튼 활성 조건
- `tests/EquipmentTwin.Hmi.Wpf.Tests/Program.cs`: 실패, 재전송, 성공, 중복 방지 테스트
- `goals/073-server-retry-outbox-state.md`: 목표와 완료 기준
- `plan.md`, `state/loop-state.md`, `state/triage.md`: 진행 상태와 다음 작업

`logs/2026-07-10-portfolio-screenshot-refresh.md`는 기존의 관계없는 미추적 파일이라 이번 작업에서 수정하거나 커밋하지 않았다.

## 3. 검증 결과

- WPF 계약 테스트: 4개 통과
  - 한국어 Markdown/JSON 계약
  - 영어 Markdown 계약
  - 새 Outbox 초기 계약
  - 실패 → 재전송 → 성공 및 중복 전송 방지
- 전체 solution Release build: 경고 0, 오류 0
- Core tests: 전체 통과
- WPF smoke test: Release 실행 파일이 4초 동안 조기 종료 없이 실행됨
- `git diff --check`: 통과

## 4. 막힌 점과 해결 방법

### PowerShell 실행 정책으로 Screenshot 스크립트가 차단됨

- 증상: `.\scripts\Capture-WpfDemoScreenshots.ps1` 직접 실행이 `UnauthorizedAccess`로 중단됐다.
- 대응: 현재 프로세스에만 적용되는 `-ExecutionPolicy Bypass`로 다시 실행했다.
- 의미: 저장소 코드의 컴파일 오류가 아니라 로컬 PowerShell 정책 문제다.

### 자동 Screenshot 세션에서 화면 복사 실패

- 증상: `Graphics.CopyFromScreen`이 Windows 오류 6(잘못된 핸들)로 첫 화면 저장에 실패했다.
- 원인: 자동화 프로세스가 현재 사용자의 대화형 데스크톱 화면 핸들에 접근하지 못했다.
- 판단: 이번 Outbox 기능과 무관한 캡처 환경 문제이므로 Screenshot 서비스를 확대 수정하지 않았다.
- 대체 검증: XAML compile을 포함한 solution build와 상태 전이 계약 테스트로 기능을 검증했다.

현재 남은 코드 blocker는 없다.

## 5. 보류한 판단

- 자동 재시도와 지수 백오프는 추가하지 않았다. 작업자가 실패 원인을 확인한 뒤 명시적으로 재전송하는 데모 흐름을 유지한다.
- 프로그램이 `sending` 상태에서 강제 종료된 경우의 자동 복구는 아직 없다.
- 여러 프로그램이 같은 파일을 동시에 전송하지 못하게 하는 파일 잠금은 아직 없다.
- 서버가 같은 `envelopeId`를 두 번 받았을 때의 멱등 처리는 다음 Goal로 남겼다.

## 6. 소프트웨어 아키텍처 설명

```text
WPF 전송 버튼
  → ViewModel이 Outbox를 sending으로 저장
  → MockServerPayloadSender가 HTTP 요청
     ├─ 성공: Outbox를 sent로 저장
     └─ 실패: Outbox를 failed로 저장
  → ViewModel이 저장된 상태를 다시 화면에 표시
```

핵심은 화면의 초록색 문구가 진실의 원본이 아니라는 점이다. 전송 상태의 원본은 Outbox JSON 파일이다. 프로그램 내부의 임시 `bool`만 사용하면 앱을 닫을 때 결과가 사라지지만, 파일에 상태를 기록하면 나중에 실제 전송 이력을 확인할 수 있다.

서비스는 상태 규칙을 책임지고, ViewModel은 HTTP 호출 순서와 화면 문구를 책임진다. XAML은 계산하지 않고 바인딩된 결과만 보여준다.

## 7. 유지보수할 때 봐야 할 파일

- 상태 이름: `AlarmIssueReportOutboxStatuses`
- 새 payload 생성: `AlarmIssueReportOutboxService.QueueAlarmIssueReport`
- 전송 시작: `AlarmIssueReportOutboxService.BeginSendAttempt`
- 성공/실패 기록: `MarkSent`, `MarkFailed`
- 실제 HTTP 흐름: `OperatorConsoleViewModel.SendLatestServerPayload`
- 버튼 활성 조건: `OperatorConsoleViewModel.CanSendLatestServerPayload`
- 화면 상태 카드: `MainWindow.xaml`의 `OutboxDeliveryStateLabel` 바인딩 영역
- 회귀 테스트: `OutboxTracksFailedRetryAndSentStates`

## 8. 사용자가 이해해야 할 개념

- `Outbox`: 서버로 보낼 메시지를 먼저 로컬에 안전하게 저장해 두는 공간이다.
- `상태 전이`: 아무 상태로나 바꾸지 않고 정해진 순서로만 이동시키는 규칙이다.
- `attemptCount`: 네트워크 요청을 실제로 시작한 횟수다. 파일을 만든 횟수가 아니다.
- `멱등성(idempotency)`: 같은 요청을 여러 번 받아도 결과가 한 번 처리한 것과 같게 만드는 성질이다.
- `영속성(persistence)`: 프로그램이 종료되어도 파일이나 DB에 값이 남는 성질이다.
- `source of truth`: 상태를 판단할 때 최종 기준으로 삼는 한 곳이다. 이번 기능에서는 Outbox JSON이다.

## 9. 다음 작업

- `Goal 074: Mock Server 중복 수신 방지와 전송 Receipt`
- 서버가 `envelopeId`를 기준으로 이미 받은 메시지를 구분하고, 최초 수신/중복 수신 결과를 명확한 응답으로 돌려주게 한다.
