# Goal 023: Inspection Scenario Selection

## 목표

같은 product recipe에서 검사 케이스를 이름으로 선택해서 PASS/FAIL 결과를 바꿀 수 있게 한다.

## 왜 필요한가

실제 카메라가 없으면 이미지 기반 검사를 현실처럼 돌릴 수 없다.

그래서 MVP에서는 실제 이미지 대신 데이터 기반 검사 케이스를 둔다. 사용자는 `--inspection scratch-detected`처럼 케이스를 선택하고, Template Runner는 선택한 케이스의 outcome, defectCode, measurement를 검사 결과로 남긴다.

이 구조는 나중에 실제 카메라, 데이터셋 카메라, Unity 가상 카메라를 붙여도 같은 `InspectionResult` 출력 구조를 유지하기 위한 준비다.

## 한 일

- `InspectionScenario` 모델을 추가했다.
- `ProductRecipe`에 `inspectionScenarios` 목록을 추가했다.
- 기존 `inspectionResult`는 default 검사 결과로 유지했다.
- `TemplateRunner.RunRecipe()`에 `inspectionScenarioName` 선택 인자를 추가했다.
- `InspectionResult`와 `TemplateRunResult`가 선택된 inspection scenario 이름을 보존하게 했다.
- CLI `template run`에 `--inspection <name>` 옵션을 추가했다.
- `templates/vision-inspection-cell.json`에 선택형 검사 케이스를 추가했다.
  - `default-panel` + `scratch-detected` → Product FAIL, `SURFACE_SCRATCH`
  - `tall-part` + `nominal-height` → Product PASS
- Visual Studio launch profile에 inspection scenario 실행 프로필을 추가했다.
- CI에 selected inspection scenario 실행을 추가했다.
- README, architecture, validation, Visual Studio 문서를 갱신했다.
- Draft PR #21을 만들었다.
- GitHub Actions push/pull_request CI 성공을 확인했다.

## 바뀐 파일

- `src/EquipmentTwin.Core/Templates/InspectionScenario.cs`
- `src/EquipmentTwin.Core/Templates/ProductRecipe.cs`
- `src/EquipmentTwin.Core/Templates/InspectionResult.cs`
- `src/EquipmentTwin.Core/Templates/TemplateRunner.cs`
- `src/EquipmentTwin.Core/Templates/TemplateRunResult.cs`
- `src/EquipmentTwin.Cli/Program.cs`
- `src/EquipmentTwin.Cli/Properties/launchSettings.json`
- `templates/vision-inspection-cell.json`
- `tests/EquipmentTwin.Core.Tests/Program.cs`
- `.github/workflows/ci.yml`
- `README.md`
- `docs/architecture.md`
- `docs/core-validation.md`
- `docs/visual-studio.md`

## 검증 결과

- Release 빌드 성공
- Core 콘솔 테스트 72개 통과
- selected inspection CLI 실행 성공
- selected inspection Markdown report 생성 확인
- 기존 default-panel template run 성공
- 기존 template batch report 실행 성공
- GitHub Actions push CI 성공
- GitHub Actions pull_request CI 성공

## 막힌 점과 해결 방법

- `InspectionResult.ScenarioName`을 `init` 속성으로 두고 생성 이후 할당하려 하면 C# 규칙상 빌드 오류가 날 수 있었다.
- 생성자 인자로 `scenarioName`을 받는 구조로 바꿔 해결했다.

## 소프트웨어 아키텍처 설명

```text
template JSON
    ↓
ProductRecipe.InspectionScenarios
    ↓
CLI --inspection <name>
    ↓
TemplateRunner.RunRecipe(..., inspectionScenarioName)
    ↓
ProductRecipe.ResolveInspectionResult()
    ↓
InspectionResult
    ↓
Console / Markdown Report
```

핵심은 실제 카메라가 없어도 검사 결과를 “데이터셋 선택”처럼 바꿀 수 있게 만든 것이다.

## 유지보수할 때 봐야 할 파일

- 검사 케이스 데이터 모델: `src/EquipmentTwin.Core/Templates/InspectionScenario.cs`
- recipe 검증/검색: `src/EquipmentTwin.Core/Templates/ProductRecipe.cs`
- 실행 시 검사 선택 적용: `src/EquipmentTwin.Core/Templates/TemplateRunner.cs`
- CLI 옵션 파싱/출력/report: `src/EquipmentTwin.Cli/Program.cs`
- 샘플 검사 케이스: `templates/vision-inspection-cell.json`

## 사용자가 이해해야 할 개념

`inspectionResult`는 기본 검사 결과다.

`inspectionScenarios`는 같은 recipe에서 추가로 선택할 수 있는 검사 케이스다.

예:

```powershell
dotnet run --project src\EquipmentTwin.Cli -- template run templates\vision-inspection-cell.json default-panel --inspection scratch-detected
```

이 명령은 `default-panel`의 모션 위치는 그대로 사용하지만, 검사 결과만 `scratch-detected` 케이스로 바꾼다.

## 다음 작업

먼저 PR #21을 Ready로 전환하고 병합한다.

그 다음 후보는 `Fault Expected-Failure Report` 또는 `Inspection Scenario Batch Matrix`다.

Fault Expected-Failure Report는 fault 주입처럼 CLI exit code 1이 정상 기대값인 케이스를 report/CI에서 안전하게 검증하게 만든다.

Inspection Scenario Batch Matrix는 recipe × inspection scenario 조합을 한 번에 돌려 PASS/FAIL matrix를 만드는 기능이다.
