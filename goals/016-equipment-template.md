# Goal 016: Equipment Template / Product Recipe

## 목표

사용자가 선택할 수 있는 장비 구성과 제품 recipe를 JSON으로 정의하는 최소 모델을 추가한다.

이번 Goal은 Unity 장비 빌더가 아니다. 먼저 Core에서 안전하게 읽고 검증할 수 있는 데이터 구조를 만든다.

## 왜 필요한가

이전 단계까지는 축을 JSON 시나리오에서 직접 움직일 수 있었다.

하지만 사용자가 원하는 최종 프로그램은 “장비/제품/공정/트러블 조건을 선택하는 프로그램”이다.

그러려면 아래 데이터가 필요하다.

```text
어떤 장비인가?
    ↓
어떤 축과 IO를 갖는가?
    ↓
어떤 제품 recipe가 있는가?
    ↓
제품별 목표 위치와 검사 방식은 무엇인가?
```

## 한 일

- `EquipmentTemplate` 모델을 추가했다.
- `MotionAxisTemplate` 모델을 추가했다.
- `ProductRecipe` 모델을 추가했다.
- `InspectionMode` enum을 추가했다.
- `templates/vision-inspection-cell.json` 샘플을 추가했다.
- 장비 템플릿 테스트 5개를 추가했다.

## 바뀐 파일

- `src/EquipmentTwin.Core/Templates/EquipmentTemplate.cs`
- `src/EquipmentTwin.Core/Templates/MotionAxisTemplate.cs`
- `src/EquipmentTwin.Core/Templates/ProductRecipe.cs`
- `src/EquipmentTwin.Core/Templates/InspectionMode.cs`
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
- 콘솔 테스트 56개 통과
- CLI batch 시나리오 9개 통과
- Draft PR #14 생성
- GitHub Actions push/pull_request CI 성공

## 막힌 점과 해결 방법

- 처음에 Release build와 Release test를 병렬로 실행해서 같은 DLL 파일을 동시에 쓰는 lock 문제가 발생했다.
- 코드 문제가 아니라 실행 방식 문제였고, 순차 실행으로 다시 돌려 테스트 56개 통과를 확인했다.

## 보류한 판단

- 템플릿을 아직 ScenarioRunner 실행 계획으로 자동 변환하지 않는다.
- IO 정의를 아직 template로 확장하지 않았다.
- Fault 조건을 아직 template에 넣지 않았다.
- Unity UI에서 template를 선택하는 기능은 아직 없다.

## 소프트웨어 아키텍처 설명

```text
templates/vision-inspection-cell.json
    ↓
EquipmentTemplate.FromJson()
    ↓
Validate motion axes
    ↓
Validate product recipes
    ↓
CreateMotionAxes(clock)
```

`EquipmentTemplate`는 장비 전체 구성을 담당한다.

`MotionAxisTemplate`는 장비에 들어있는 축 이름과 초기 위치를 담당한다.

`ProductRecipe`는 제품별 목표 위치와 검사 모드를 담당한다.

## 유지보수할 때 봐야 할 파일

- 장비 템플릿 전체 구조: `src/EquipmentTwin.Core/Templates/EquipmentTemplate.cs`
- 축 정의: `src/EquipmentTwin.Core/Templates/MotionAxisTemplate.cs`
- 제품 recipe: `src/EquipmentTwin.Core/Templates/ProductRecipe.cs`
- 검사 모드: `src/EquipmentTwin.Core/Templates/InspectionMode.cs`
- 샘플 템플릿: `templates/vision-inspection-cell.json`
- 테스트: `tests/EquipmentTwin.Core.Tests/Program.cs`

## 사용자가 이해해야 할 개념

이번 작업은 “장비를 화면에 그린 것”이 아니다.

이번 작업은 “장비를 데이터로 정의하기 시작한 것”이다.

화면보다 데이터 구조가 먼저 필요한 이유는, 나중에 Unity UI가 생겨도 같은 템플릿을 읽어서 장비를 만들고, 같은 recipe를 읽어서 공정을 실행해야 하기 때문이다.

## 다음 작업

다음 후보는 PR #14 병합 후 `Template Runner`다.

선택한 `EquipmentTemplate`과 `ProductRecipe`를 읽어서 실제 `ScenarioRunner` 또는 모션 실행 계획으로 바꾸는 계층이 필요하다.
