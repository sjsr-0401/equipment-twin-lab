# Goal 022: Template Batch Report

## 목표

template 안의 모든 product recipe를 한 번에 실행하고, 장비 실행 결과와 제품 검사 결과를 Markdown report로 비교한다.

## 왜 필요한가

단일 recipe report는 한 제품 결과를 자세히 보여준다.

하지만 포트폴리오 데모에서는 여러 제품 조건을 한 번에 돌려서 어떤 제품은 PASS이고 어떤 제품은 FAIL인지 비교하는 화면이 더 설득력 있다.

이번 Goal은 `default-panel`과 `tall-part`를 한 번에 실행해서 장비 실행 성공과 제품 검사 결과를 비교한다.

## 한 일

- CLI에 `template batch` 모드를 추가했다.
- template JSON 안의 모든 `ProductRecipe`를 순서대로 실행하게 했다.
- 각 recipe는 독립적인 `TemplateRunner`와 `ManualClock`으로 실행한다.
- 콘솔에 batch 요약을 출력했다.
  - Execution pass/fail
  - Product pass/fail
  - Not inspected
- Markdown batch report를 생성했다.
- report에는 Summary와 Details를 포함했다.
- CI에 template batch report 생성을 추가했다.
- Visual Studio launch profile에 template batch report를 추가했다.

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
- Template batch CLI 실행 성공
- `artifacts\template-batch-report.md` 내용 확인

## 막힌 점과 해결 방법

- 구현 중 C# top-level file에서 local function과 type 선언 순서 오류가 발생했다.
- `IsTemplateMode()`를 `CliOptions` 내부 static method로 옮겨 해결했다.
- batch 실행에서는 제품 FAIL을 프로그램 실패로 보지 않기로 했다. 제품 FAIL은 품질 결과이고, CLI exit code 실패는 장비 실행 실패에만 대응한다.

## 보류한 판단

- fault scenario를 batch에 같이 넣는 기능은 아직 없다.
- recipe별로 다른 fault나 inspection scenario를 지정하는 manifest는 아직 없다.
- batch report 결과물은 생성 시간이 달라지므로 커밋하지 않는다.

## 소프트웨어 아키텍처 설명

```text
template batch <template.json>
    ↓
EquipmentTemplate.ProductRecipes
    ↓
Recipe별 TemplateRunner.RunRecipe()
    ↓
TemplateBatchRun[]
    ↓
BuildTemplateBatchMarkdownReport()
```

중요한 설계 판단:

```text
Execution FAIL = 장비 실행 실패
Product FAIL   = 제품 검사 불합격
```

제품이 FAIL이어도 장비가 정상 실행됐으면 batch exit code는 성공이다.

## 유지보수할 때 봐야 할 파일

- batch command parsing: `src/EquipmentTwin.Cli/Program.cs`
- batch 실행: `RunTemplateBatch()`
- batch 콘솔 출력: `PrintTemplateBatchResult()`
- batch report 생성: `BuildTemplateBatchMarkdownReport()`
- CI 검증 명령: `.github/workflows/ci.yml`
- Visual Studio 실행 프로필: `src/EquipmentTwin.Cli/Properties/launchSettings.json`

## 사용자가 이해해야 할 개념

이 기능은 “여러 제품 조건을 한 번에 돌리는 데모”다.

실무적으로는 생산 lot이나 recipe group을 돌려서 어떤 제품이 PASS/FAIL인지 요약하는 개념과 연결된다.

## 다음 작업

다음 후보는 `Inspection Scenario Selection` 또는 `Fault Expected-Failure Report`이다.

Inspection Scenario Selection은 같은 recipe에서 PASS/FAIL 케이스를 더 자유롭게 고르게 만든다.

Fault Expected-Failure Report는 fault 주입처럼 exit code 1이 정상 기대값인 케이스를 CI/report에서 안전하게 검증하게 만든다.
