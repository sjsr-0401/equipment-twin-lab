# Goal 073: 서버 전송 실패 Retry와 Outbox 상태 관리

## 목표

- 서버 전송 결과를 메모리가 아니라 Outbox JSON 파일에 보존한다.
- 전송 실패 후 작업자가 같은 payload를 수동으로 재전송할 수 있게 한다.
- 이미 성공한 payload의 중복 전송을 막는다.
- 작업자가 WPF에서 현재 상태, 시도 횟수, 마지막 시도 시각과 오류를 확인하게 한다.

## 완료 기준

- 상태가 `queued → sending → sent` 또는 `queued → sending → failed`로 전이된다.
- 재전송을 시작하면 시도 횟수가 증가하고 이전 오류가 지워진다.
- 성공 시 `sentAt`, 실패 시 `lastError`가 JSON에 기록된다.
- `sent` payload는 다시 전송할 수 없다.
- WPF 버튼이 상태에 따라 `Mock Server로 전송`, `재전송`, `전송 완료`로 바뀐다.
- 상태 전이와 중복 전송 방지 테스트가 자동으로 통과한다.
- 전체 solution build, Core tests, WPF contract tests가 통과한다.

## 구현 범위

- 자동 재시도, 지수 백오프, 백그라운드 worker는 추가하지 않는다.
- 다중 프로세스가 같은 Outbox 파일을 동시에 보내는 분산 잠금은 다루지 않는다.
- 현재 데모의 수동 전송 흐름 안에서 파일 기반 상태를 정확히 관리한다.

## 검증 명령

```powershell
dotnet build .\EquipmentTwinLab.sln -c Release
dotnet run --project .\tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj -c Release --no-build
dotnet run --project .\tests\EquipmentTwin.Hmi.Wpf.Tests\EquipmentTwin.Hmi.Wpf.Tests.csproj -c Release --no-build
```

## 다음 작업

- `Goal 074: Mock Server 중복 수신 방지와 전송 Receipt`
