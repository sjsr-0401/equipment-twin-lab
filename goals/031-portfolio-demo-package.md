# Goal 031: Portfolio Demo Package

## 목표

현재까지 만든 Core/CLI/Unity 흐름을 포트폴리오와 면접에서 설명 가능한 demo package로 정리한다.

## 배경

Goal 030까지 Unity screenshot capture 자동화는 만들었지만, 실제 PNG 생성은 Unity Hub 라이선스 활성화가 필요하다.

사용자가 밖에 있는 상태에서는 Unity 로그인/화면 조작을 할 수 없으므로, 이번 Goal은 실행 환경에 막히지 않는 문서·README·상태 정리를 진행한다.

## 완료 조건

- [x] 3분 데모 스크립트를 문서화한다.
- [x] 현재 구현이 무엇을 증명하는지 표로 정리한다.
- [x] 아직 주장하면 안 되는 내용을 명시한다.
- [x] README에서 portfolio demo 문서로 연결한다.
- [x] architecture 문서에 portfolio/demo boundary를 추가한다.
- [x] 작업 로그에 막힌 점과 다음 사용자 액션을 기록한다.
- [x] .NET build/test/CLI process run을 통과시킨다.
- [x] GitHub Actions에서 문서 파일 존재를 확인한다.

## 제외 범위

- Unity Hub 로그인/라이선스 활성화
- 실제 Unity screenshot PNG 생성
- README에 실제 이미지 삽입
- 실제 ALTUS/Halo/Halo HX 내부 sequence 재현
- 실제 비전 검사 알고리즘 구현

## 사용자에게 설명할 핵심

```text
이번 작업은 새 기능 구현보다 “면접에서 방어 가능한 설명 구조”를 만드는 작업이다.
Core/CLI가 공정 결과를 만들고, Unity가 그 결과를 읽어 보여주는 경계를 명확히 했다.
```

## 검증

```powershell
dotnet build EquipmentTwinLab.sln --no-restore --configuration Release
dotnet run --project tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj --no-restore --configuration Release
dotnet run --project src\EquipmentTwin.Cli\EquipmentTwin.Cli.csproj --no-restore --configuration Release -- process run processes\public-moly-ald-metallization.json --report artifacts\moly-ald-process-report.md --timeline artifacts\moly-ald-timeline.json
```
