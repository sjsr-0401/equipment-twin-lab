# Goal 019: Inspection Result Model

## 목표

실제 카메라가 없어도 제품 검사 PASS/FAIL을 데이터로 표현하고, Template Runner 실행 결과에 검사 결과를 남긴다.

이번 Goal은 실제 이미지 처리나 Unity 카메라가 아니다. 먼저 검사 결과를 담는 공통 데이터 구조를 만든다.

## 왜 필요한가

사용자가 원하는 대표 프로젝트는 “장비가 움직인다”에서 끝나면 부족하다.

제조 장비는 결국 어떤 제품을 만들거나 검사하고, 그 결과가 PASS인지 FAIL인지 남겨야 한다.

그래서 장비 실행 성공과 제품 검사 결과를 분리해야 한다.

```text
장비 실행 성공
    = 축이 알람 없이 목표 위치까지 갔는가

제품 검사 PASS/FAIL
    = 검사 기준상 제품이 양품인가 불량인가
```

## 한 일

- `InspectionOutcome`을 추가했다.
- `InspectionResultSpec`을 추가했다.
- `InspectionResult`를 추가했다.
- `ProductRecipe`에 `InspectionResult` 정의를 연결했다.
- `DatasetCamera`, `UnityCamera` recipe는 inspection result를 반드시 갖도록 검증했다.
- Template Runner가 정상 모션 완료 후 검사 결과를 생성하게 했다.
- 모션 fault가 발생하면 검사 결과를 만들지 않게 했다.
- 샘플 template에 PASS 제품과 FAIL 제품을 추가했다.
  - `default-panel`: PASS
  - `tall-part`: `HEIGHT_OVER_LIMIT` Fail
- 검사 결과 관련 테스트 2개를 추가하고 기존 Template Runner 테스트를 보강했다.

## 바뀐 파일

- `src/EquipmentTwin.Core/Templates/InspectionOutcome.cs`
- `src/EquipmentTwin.Core/Templates/InspectionResultSpec.cs`
- `src/EquipmentTwin.Core/Templates/InspectionResult.cs`
- `src/EquipmentTwin.Core/Templates/ProductRecipe.cs`
- `src/EquipmentTwin.Core/Templates/TemplateRunner.cs`
- `src/EquipmentTwin.Core/Templates/TemplateRunResult.cs`
- `templates/vision-inspection-cell.json`
- `tests/EquipmentTwin.Core.Tests/Program.cs`
- `README.md`
- `docs/architecture.md`
- `docs/core-validation.md`
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

## 막힌 점과 해결 방법

- 실제 카메라가 없으므로 이미지 처리 결과는 만들 수 없다.
- 그래서 지금은 recipe 안의 `inspectionResult`를 데이터 기반 가상 검사 결과로 사용했다.
- PowerShell에서 `Select-Object -Tail` 옵션이 동작하지 않아 문서 확인은 `Get-Content -Tail`로 대체했다.

## 보류한 판단

- 같은 recipe에서 PASS/FAIL inspection scenario를 별도로 선택하는 구조는 아직 없다.
- 실제 dataset camera adapter는 아직 없다.
- Unity 가상 카메라 결과와 연결하지 않았다.
- 검사 기준식, tolerance, defect classification은 아직 단순화했다.

## 소프트웨어 아키텍처 설명

```text
ProductRecipe
    ↓
InspectionResultSpec
    ↓
TemplateRunner.RunRecipe()
    ↓
InspectionResult
    ↓
TemplateRunResult
```

`TemplateRunResult.Success`는 장비 실행 성공이다.

`TemplateRunResult.ProductPassed`는 제품 검사 결과다.

이 둘을 분리한 이유는 제품이 NG여도 장비 자체는 정상 동작했을 수 있기 때문이다.

## 유지보수할 때 봐야 할 파일

- 검사 결과 종류: `src/EquipmentTwin.Core/Templates/InspectionOutcome.cs`
- recipe에 들어가는 검사 결과 정의: `src/EquipmentTwin.Core/Templates/InspectionResultSpec.cs`
- 실행 후 남는 검사 결과: `src/EquipmentTwin.Core/Templates/InspectionResult.cs`
- recipe validation: `src/EquipmentTwin.Core/Templates/ProductRecipe.cs`
- 검사 결과 생성 시점: `src/EquipmentTwin.Core/Templates/TemplateRunner.cs`
- 실행 결과 노출: `src/EquipmentTwin.Core/Templates/TemplateRunResult.cs`
- 샘플 검사 데이터: `templates/vision-inspection-cell.json`
- 테스트: `tests/EquipmentTwin.Core.Tests/Program.cs`

## 사용자가 이해해야 할 개념

검사 결과 모델은 실제 카메라 검사가 아니다.

지금은 “나중에 카메라가 어떤 결과를 줘야 하는지”를 먼저 정한 데이터 계약이다.

이 계약이 있어야 실제 카메라, 데이터셋 카메라, Unity 가상 카메라가 나중에 붙어도 같은 결과 구조를 사용할 수 있다.

## 다음 작업

다음 후보는 `Template Runner CLI` 또는 `Inspection Scenario Selection`이다.

CLI를 먼저 만들면 사용자가 template/recipe/fault를 명령어로 실행해 결과를 볼 수 있다.

Inspection Scenario Selection을 먼저 만들면 같은 제품에서 PASS/FAIL 케이스를 더 자유롭게 선택할 수 있다.
