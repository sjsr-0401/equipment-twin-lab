# Goal 072: 알람 리포트와 서버 Payload 계약 자동 테스트

## 목표

- 한국어/영어 알람 Markdown의 문서 구조가 실수로 깨지지 않게 한다.
- 서버 Outbox Envelope와 Payload의 기계용 필드 계약을 자동 검증한다.
- 로컬과 GitHub Actions에서 같은 테스트를 반복 실행한다.

## 완료 기준

- 한국어 리포트의 한국어 제목, 요약, 표, Severity 표시를 검증한다.
- 영어 리포트의 영어 제목, 요약, 표, Severity 표시를 검증한다.
- JSON의 `language`, `alarmCode`, `severity` 계약을 검증한다.
- Outbox의 `messageType`, `status`, `target`, Payload 수치값을 검증한다.
- 테스트가 생성한 임시 리포트와 Outbox 파일을 실행 후 삭제한다.
- 새 테스트 프로젝트가 solution과 GitHub Actions에 포함된다.
- 전체 solution build, Core tests, WPF contract tests가 통과한다.

## 구현 범위

- 외부 테스트 프레임워크나 NuGet 패키지를 추가하지 않는다.
- Core 테스트와 같은 콘솔 테스트 실행 방식을 사용한다.
- WPF 제품 코드와 서버 전송 코드는 변경하지 않는다.

## 검증 명령

```powershell
dotnet build .\EquipmentTwinLab.sln -c Release
dotnet run --project .\tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj -c Release --no-build
dotnet run --project .\tests\EquipmentTwin.Hmi.Wpf.Tests\EquipmentTwin.Hmi.Wpf.Tests.csproj -c Release --no-build
```

## 다음 작업

- `Goal 073: 서버 전송 실패 Retry와 Outbox 상태 관리`
