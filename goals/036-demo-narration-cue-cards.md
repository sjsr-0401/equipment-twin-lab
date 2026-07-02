# Goal 036: Demo Narration Cue Cards

## 목표

3분 포트폴리오 데모를 녹화할 때 사용자가 영어 로그나 긴 문서를 읽지 않고, 한글 큐카드만 보고 설명할 수 있게 만든다.

## 배경

Goal 035에서 리허설 스크립트는 build/test/process/fault/Unity screenshot 준비 상태를 자동 확인한다.

하지만 리허설이 통과해도 실제 녹화에서는 아래 문제가 남는다.

- 어떤 순서로 말할지 헷갈릴 수 있다.
- fault exit code `1`을 실패처럼 오해할 수 있다.
- 실제 장비 복제처럼 과장해서 말할 위험이 있다.
- Core/CLI/Unity 분리 이유를 짧게 설명하기 어렵다.

## 구현 범위

- `scripts/New-PortfolioDemoCueCards.ps1`
  - 리허설 결과를 읽고 한글 녹화 큐카드를 생성한다.
  - 기본 출력: `artifacts/demo-rehearsal/recording-cue-cards.md`
- `scripts/Invoke-PortfolioDemoRehearsal.ps1`
  - 리허설이 끝나면 큐카드도 함께 생성한다.
- `docs/portfolio-demo-narration.md`
  - 3분 대본, 면접 질문 대응, 피해야 할 표현을 정리한다.

## 완료 기준

- 큐카드 생성 스크립트가 실행된다.
- 리허설 스크립트가 큐카드까지 생성한다.
- 문서에 “실제 장비 복제 아님” 경계가 명확히 들어간다.
- README와 demo package에서 새 문서로 이동할 수 있다.

## 검증

```powershell
.\scripts\New-PortfolioDemoCueCards.ps1
.\scripts\Invoke-PortfolioDemoRehearsal.ps1 -SkipUnity
dotnet build EquipmentTwinLab.sln --no-restore --configuration Release
dotnet run --project tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj --no-restore --configuration Release
```

## 이번 Goal에서 하지 않는 것

- 실제 화면 녹화
- 음성 녹음
- Unity 카메라 애니메이션
- 실제 CAD/Blender asset import
