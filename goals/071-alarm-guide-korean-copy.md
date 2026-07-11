# Goal 071: 알람 작업지시서 한국어 문장 품질 정리

## 목표

- 알람 대응 가이드의 번역투와 불필요한 한영 혼합 문장을 정리한다.
- 작업자가 제목, 요약, 점검 항목, 대응 선택지, 엔지니어 검토 요청 조건을 빠르게 이해하게 한다.
- 한국어 모드에서 저장한 Markdown 이슈 리포트도 한국어로 읽을 수 있게 한다.
- 서버 Payload의 기계용 식별값은 언어 전환과 관계없이 안정적으로 유지한다.

## 완료 기준

- VAC-101, TMP-201, GAS-301, SEQ-001 가이드의 사용자 문장 전체에 한국어 매핑이 있다.
- `active`, `mismatch`, `escalate`, `reading`, `trend` 같은 번역투 표현이 한국어 문장에 남지 않는다.
- Gas, Valve, ALD, HMI, IO, Recipe, Trace처럼 필요한 도메인 용어만 유지한다.
- 한국어 화면의 Severity와 점검 흐름 라벨이 자연스럽게 표시된다.
- 한국어 Markdown 리포트의 제목, 표, 상태, 요청 조건이 한국어로 생성된다.
- JSON Payload에는 `language=ko/en`, `severity=Warning` 같은 기계용 값이 유지된다.
- 한영 Screenshot, 전체 build, Core tests, WPF smoke test가 통과한다.

## 구현 범위

- 영어 원본 `alarm-guides/moly-ald-alarm-guides.json`은 변경하지 않는다.
- WPF ViewModel의 표시 번역을 정리한다.
- Markdown Export 단계에서 Language에 따라 문서 구조를 렌더링한다.
- Core 알람 판정, 공정 Recipe, 서버 전송 동작은 변경하지 않는다.

## 검증

```powershell
.\scripts\Capture-WpfDemoScreenshots.ps1 -Language ko
.\scripts\Capture-WpfDemoScreenshots.ps1 -Language en -SkipBuild
dotnet build .\EquipmentTwinLab.sln -c Release
dotnet run --project .\tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj -c Release --no-build
```

## 다음 작업

- `Goal 072: 알람 리포트와 서버 Payload 계약 자동 테스트`
