# 로컬 Mock Server

`EquipmentTwin.MockServer`는 WPF HMI가 만든 server-outbox payload를 받아보는 작은 local receiver다.

이 서버는 실제 MES, SECS/GEM, 공장 서버를 의미하지 않는다. Portfolio/demo용 도구이며, HMI가 alarm issue payload를 만들고 local service가 그것을 받을 수 있다는 것을 검증하기 위한 장치다.

## 실행

```powershell
dotnet run --project .\src\EquipmentTwin.MockServer\EquipmentTwin.MockServer.csproj
```

서버 주소:

```text
http://127.0.0.1:5088
```

## 상태 확인

```powershell
Invoke-RestMethod http://127.0.0.1:5088/health
```

기대 응답:

```json
{
  "status": "ok",
  "service": "EquipmentTwin.MockServer"
}
```

## Alarm issue report 수신

```powershell
$payload = Get-Content .\artifacts\server-outbox\<payload-file>.json -Raw
Invoke-RestMethod `
  -Method Post `
  -Uri http://127.0.0.1:5088/alarm-issue-report `
  -ContentType "application/json" `
  -Body $payload
```

수신된 payload는 아래 폴더에 저장된다.

```text
artifacts/mock-server-received/
```

## 이 서버를 만든 이유

현재 WPF HMI는 서버로 보낼 payload를 local outbox 파일로 먼저 저장한다. 이 mock server는 그 다음 단계를 검증한다.

1. WPF가 alarm issue report payload를 만든다.
2. Payload가 `artifacts/server-outbox/`에 저장된다.
3. 개발자가 그 JSON을 local mock server에 POST한다.
4. Mock server가 받은 payload를 `artifacts/mock-server-received/` 아래에 저장한다.

이 구조는 프로젝트를 정직하게 유지한다. 아직 실제 공장 integration layer를 구현했다고 주장하지 않으면서도, 서버로 보내질 데이터 흐름은 실제로 검증한다.
