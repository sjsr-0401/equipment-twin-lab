# 목표 054: WPF 리포트 / Mock Server 안정화

작성일: 2026-07-04
브랜치: 현재 작업 브랜치

## 목표

새 UI 기능을 더 추가하기 전에, 현재 WPF 알람 리포트 흐름이 깨지지 않았는지 안정화한다.

검증 대상 흐름:

```text
WPF HMI
→ Fault Replay 실행
→ Alarm Response Guide 표시
→ Issue Report Export
→ Server Outbox 저장
→ Mock Server로 전송
→ Mock Server 수신 payload 저장
```

## 완료 기준

- 전체 solution build가 통과한다.
- Core tests가 통과한다.
- WPF project build가 통과한다.
- Mock Server project build가 통과한다.
- Mock Server가 `GET /health`에 응답한다.
- Mock Server가 `POST /alarm-issue-report`를 받는다.
- 수신된 payload가 `artifacts/mock-server-received/` 아래에 저장된다.
- state 문서에 다음 기능 목표가 명확히 정리된다.

## 검증 결과

통과.

확인한 명령:

```powershell
dotnet build .\EquipmentTwinLab.sln
dotnet run --project .\tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj
dotnet build .\src\EquipmentTwin.Hmi.Wpf\EquipmentTwin.Hmi.Wpf.csproj
dotnet build .\src\EquipmentTwin.MockServer\EquipmentTwin.MockServer.csproj
```

Mock Server endpoint 검증:

```text
GET  http://127.0.0.1:5088/health
POST http://127.0.0.1:5088/alarm-issue-report
```

결과:

```text
HealthStatus       = ok
HealthService      = EquipmentTwin.MockServer
PostStatus         = received
ReceivedFileExists = True
```

## 메모

- 실제 production MES, SECS/GEM, vendor 내부 서버를 구현한 것이 아니다.
- 이 goal은 local demo 경계만 검증한다. 즉, WPF가 만든 report payload를 local outbox에 저장하고 local mock server로 보낼 수 있는지 확인한다.
- WPF 버튼 클릭 자동화는 아직 보류했다. WPF는 build로 확인했고, server endpoint는 실제 HTTP POST로 검증했다.

## 커밋 전 주의점

현재 working tree에는 WPF alarm guide / report / mock server 작업에서 생긴 미커밋 파일이 많이 남아 있다.

커밋 전에 아래 untracked Unity 파일을 포함할지 확인해야 한다.

```text
unity/EquipmentTwin.Unity/ProjectSettings/PackageManagerSettings.asset
```

이 파일은 Unity editor가 생성한 project setting으로 보이며, 이번 안정화 goal에서는 건드리지 않았다.

## 다음 추천 작업

```text
목표 055: WPF HMI 계측값 Trend Panel
```

압력, 온도, 막두께가 시간에 따라 어떻게 변했는지 보여줘서 operator가 현재값뿐 아니라 공정 흐름도 볼 수 있게 만든다.
