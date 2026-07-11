# Goal 063 — Mock Server 상태와 Workflow 전송 단계 연결

## 목표

Mock Server의 현재 연결 상태와 알람 workflow 6번 전송 카드가 서로 다른 정보를 보여주는 문제를 해결한다.

## 완료 기준

- 서버 확인 중에는 전송 카드가 `CHECKING`을 표시한다.
- 서버 연결 실패 시 `OFFLINE`과 빨간색을 표시한다.
- 서버 연결 가능 시 `ONLINE`과 파란색을 표시한다.
- 실제 payload 전송 성공 시 `HTTP OK`와 초록색을 표시한다.
- 전송 성공 상태는 이후 health 상태보다 해당 workflow의 완료 결과로 우선 표시된다.
- 영어/한국어 상태 문구가 모두 동작한다.
- WPF Release build, 전체 solution Release build, Core tests, 상태 전이 검증이 통과한다.

## 구현 결정

- 기존 health check와 payload 전송 로직은 변경하지 않는다.
- `OperatorConsoleViewModel`의 전송 단계 표시 속성만 수정한다.
- ONLINE은 연결 가능 상태이지 전송 완료가 아니므로 파란색을 사용한다.
- 전송 성공만 완료 의미의 초록색을 사용한다.

## 검증

- `dotnet build .\src\EquipmentTwin.Hmi.Wpf\EquipmentTwin.Hmi.Wpf.csproj -c Release`
- `dotnet build .\EquipmentTwinLab.sln -c Release`
- `dotnet run --project .\tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj -c Release`
- Git에서 제외된 .NET 8 임시 검증 프로그램으로 5개 상태와 한글 표시 확인
- `git diff --check`
