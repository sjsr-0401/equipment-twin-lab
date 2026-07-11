# Goal 061 — WPF Mock Server 연결 상태 표시

## 목표

알람/리포트 탭에서 Mock Server가 실행 중인지 사용자가 바로 확인할 수 있게 한다.

## 완료 기준

- 서버 payload 영역에 Mock Server 연결 상태가 표시된다.
- 사용자가 `서버 확인` 버튼으로 `/health` endpoint를 확인할 수 있다.
- payload 전송 성공/실패 결과가 서버 연결 상태에도 반영된다.
- 기존 report export, server outbox, mock server 전송 기능은 유지된다.
- WPF Release build, 전체 solution Release build, Core tests가 통과한다.

## 구현 결정

- 새 서버 기능을 만들지 않고 기존 `EquipmentTwin.MockServer`의 `/health` endpoint를 재사용한다.
- WPF에는 상태 카드와 버튼만 추가한다.
- 상태는 `미확인`, `확인 중`, `연결 가능`, `연결 실패`로 단순화한다.

## 검증

- `dotnet build .\src\EquipmentTwin.Hmi.Wpf\EquipmentTwin.Hmi.Wpf.csproj -c Release`
- `dotnet build .\EquipmentTwinLab.sln -c Release`
- `dotnet run --project .\tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj -c Release`
- `git diff --check`
