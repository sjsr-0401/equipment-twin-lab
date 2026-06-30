# Goal 017: Template Runner

## 목표

`EquipmentTemplate`과 `ProductRecipe`를 실제 가상 모션 실행으로 바꾸는 최소 실행 계층을 추가한다.

이번 Goal은 전체 공정 엔진이 아니다. 우선 template/recipe가 X/Z 축 목표 위치까지 실제로 실행되는지 검증한다.

## 왜 필요한가

Goal 016에서는 장비와 제품 recipe를 JSON으로 정의했다.

하지만 정의만 있고 실행이 없으면 사용자가 “장비와 제품을 선택하면 동작한다”고 말하기 어렵다.

이번 Goal은 아래 연결을 만든다.

```text
Equipment Template
    ↓
Product Recipe
    ↓
Template Runner
    ↓
MotionAxis ServoOn/Home/Move
```

## 한 일

- `TemplateRunner`를 추가했다.
- `TemplateRunnerOptions`를 추가했다.
- `TemplateRunResult`를 추가했다.
- `TemplateMotionCommandLog`를 추가했다.
- Template Runner 테스트 4개를 추가했다.

## 바뀐 파일

- `src/EquipmentTwin.Core/Templates/TemplateRunner.cs`
- `src/EquipmentTwin.Core/Templates/TemplateRunnerOptions.cs`
- `src/EquipmentTwin.Core/Templates/TemplateRunResult.cs`
- `src/EquipmentTwin.Core/Templates/TemplateMotionCommandLog.cs`
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
- 콘솔 테스트 60개 통과
- CLI batch 시나리오 9개 통과
- Draft PR #15 생성
- GitHub Actions push/pull_request CI 성공

## 막힌 점과 해결 방법

- build와 test를 다시 병렬로 돌려 Windows DLL lock 문제가 재발했다.
- 순차 실행으로 검증해서 정상 통과를 확인했다.
- `default-panel` recipe의 명령 수 기대값을 처음에 12로 잘못 계산했다. 실제로는 2축 기준 ServoOn/Home/PollHome 6개 + Move/PollMove 4개 = 10개라 테스트를 수정했다.

## 보류한 판단

- Template Runner는 아직 모션 축만 실행한다.
- IO 입력/출력은 아직 template runner에 연결하지 않았다.
- 검사 결과와 Fault Injection은 아직 없다.
- ScenarioRunner와 통합하지 않았다.

## 소프트웨어 아키텍처 설명

```text
TemplateRunner.RunRecipe(template, "default-panel")
    ↓
template.FindProductRecipe()
    ↓
template.CreateMotionAxes(clock)
    ↓
ServoOn → Home → Move
    ↓
TemplateRunResult
```

`TemplateRunResult`에는 최종 축 상태와 명령 로그가 남는다.

## 유지보수할 때 봐야 할 파일

- 실행 순서: `src/EquipmentTwin.Core/Templates/TemplateRunner.cs`
- home/move 시간 옵션: `src/EquipmentTwin.Core/Templates/TemplateRunnerOptions.cs`
- 실행 결과: `src/EquipmentTwin.Core/Templates/TemplateRunResult.cs`
- 명령 로그: `src/EquipmentTwin.Core/Templates/TemplateMotionCommandLog.cs`
- 테스트: `tests/EquipmentTwin.Core.Tests/Program.cs`

## 사용자가 이해해야 할 개념

Template는 장비의 설계도이고, Recipe는 제품별 조건이다.

Template Runner는 그 둘을 읽어서 실제로 축을 움직이는 실행기다.

아직은 모션만 다루지만, 이 계층이 있어야 나중에 IO, 검사, fault를 같은 흐름에 붙일 수 있다.

## 다음 작업

다음 후보는 PR #15 병합 후 `Fault Model` 또는 `Inspection Result Model`이다.

정상 동작은 template/recipe로 연결됐으므로, 이제 사용자가 원하는 “트러블 조건”이나 “검사 결과”를 데이터로 넣는 단계가 필요하다.
