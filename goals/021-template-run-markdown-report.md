# Goal 021: Template Run Markdown Report

## 목표

`template run` CLI 실행 결과를 콘솔 출력뿐 아니라 Markdown report 파일로 저장할 수 있게 만든다.

## 왜 필요한가

CLI 출력은 실행할 때는 보기 좋지만, 포트폴리오나 작업 로그에 남기기에는 불편하다.

Markdown report가 있으면 장비 실행 결과, 제품 검사 결과, 모션 축 상태, 명령 로그를 파일로 남길 수 있다.

## 한 일

- `template run`에서 `--report <path>` 옵션을 허용했다.
- Template run 결과를 Markdown으로 생성하는 함수를 추가했다.
- report에는 아래 내용을 포함했다.
  - Summary
  - Inspection
  - Motion Axes
  - Command Log
- CI의 `default-panel` template CLI 실행에 report 생성을 포함했다.
- Visual Studio `CLI - template default panel` 프로필에도 report 생성을 포함했다.
- README, architecture, validation, Visual Studio 문서를 갱신했다.

## 바뀐 파일

- `src/EquipmentTwin.Cli/Program.cs`
- `.github/workflows/ci.yml`
- `src/EquipmentTwin.Cli/Properties/launchSettings.json`
- `README.md`
- `docs/architecture.md`
- `docs/core-validation.md`
- `docs/visual-studio.md`
- `logs/2026-07-01.md`
- `plan.md`
- `state/loop-state.md`
- `state/triage.md`
- `state/comprehension.md`

## 검증 결과

- Release 빌드 성공
- 경고 0개
- 오류 0개
- 콘솔 테스트 68개 통과
- CLI batch 시나리오 9개 통과
- Template CLI `default-panel --report artifacts\template-run-report.md` 실행 성공
- Template CLI `tall-part` 실행 성공
- `artifacts\template-run-report.md` 내용 확인

## 막힌 점과 해결 방법

- template CLI 두 개를 병렬 검증했을 때 DLL lock이 발생했다.
- 같은 Release build 출력물을 두 `dotnet run` 프로세스가 동시에 사용했기 때문이다.
- 이후 검증을 순차 실행해서 통과시켰다.
- report 파일은 실행 시점마다 생성 시간이 달라진다.
- 그래서 report 결과물 자체는 소스 커밋에 포함하지 않고, 생성 기능만 검증했다.

## 보류한 판단

- template batch report는 아직 없다.
- 여러 recipe를 한 번에 돌리는 기능은 아직 없다.
- fault expected-failure report를 CI에서 어떻게 검증할지는 아직 정하지 않았다.

## 소프트웨어 아키텍처 설명

```text
template run ... --report path
    ↓
RunTemplate()
    ↓
TemplateRunner.RunRecipe()
    ↓
BuildTemplateMarkdownReport()
    ↓
File.WriteAllText()
```

Markdown report는 Core 로직이 아니라 CLI 표현 계층이다.

Core는 실행 결과만 만들고, CLI가 그 결과를 사람이 읽을 수 있는 문서로 바꾼다.

## 유지보수할 때 봐야 할 파일

- report 옵션 파싱: `src/EquipmentTwin.Cli/Program.cs`
- report 생성: `BuildTemplateMarkdownReport()`
- report 저장: `WriteTemplateMarkdownReport()`
- CI 검증 명령: `.github/workflows/ci.yml`
- Visual Studio 실행 프로필: `src/EquipmentTwin.Cli/Properties/launchSettings.json`

## 사용자가 이해해야 할 개념

이번 작업은 “시뮬레이션 결과를 남기는 방법”이다.

장비 SW 관점에서는 생산 이력, 검사 이력, 알람 이력을 남기는 것이 중요하다.

지금은 파일 report 수준이지만, 나중에 MES나 DB 연동으로 확장할 수 있는 방향이다.

## 다음 작업

다음 후보는 `Inspection Scenario Selection` 또는 `Template Batch Report`이다.

Inspection Scenario Selection은 같은 recipe에서 PASS/FAIL 케이스를 더 자유롭게 선택하게 만든다.

Template Batch Report는 여러 recipe 실행 결과를 한 번에 비교하게 만든다.
