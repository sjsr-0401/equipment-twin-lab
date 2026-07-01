# Goal 020: Template Runner CLI

## 목표

사용자가 명령어로 Equipment Template과 Product Recipe를 직접 실행하고, 장비 실행 결과와 제품 검사 결과를 확인할 수 있게 만든다.

## 왜 필요한가

지금까지는 template/recipe/fault/inspection 모델이 Core 안에 있었다.

하지만 사용자가 직접 실행해볼 수 없으면 포트폴리오 데모로 보여주기 어렵다.

이번 Goal은 Core 기능을 CLI로 노출해서 사람이 직접 확인할 수 있게 만든다.

## 한 일

- CLI에 `template run` 모드를 추가했다.
- 사용자가 template JSON과 recipe 이름을 넘길 수 있게 했다.
- `--fault <name>` 옵션으로 fault scenario를 주입할 수 있게 했다.
- 콘솔 출력에 아래 정보를 표시했다.
  - Template
  - Recipe
  - Execution
  - Product
  - Fault
  - Inspection
  - Motion axes
  - Command log
- GitHub Actions CI에 template CLI 실행 2개를 추가했다.
  - `default-panel`: Execution PASS, Product PASS
  - `tall-part`: Execution PASS, Product FAIL
- Visual Studio launch profile에 template 실행 프로필 3개를 추가했다.

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
- Template CLI `default-panel` 실행 성공
- Template CLI `tall-part` 실행 성공
- Template CLI fault 주입 출력 확인
- Draft PR #18 생성: `https://github.com/sjsr-0401/equipment-twin-lab/pull/18`
- GitHub Actions `push`, `pull_request` 성공

## 막힌 점과 해결 방법

- fault 주입 케이스는 의도적으로 `Execution: FAIL`이므로 CLI exit code가 1이다.
- 그래서 CI에는 fault 케이스를 넣지 않고, 성공 exit code를 기대할 수 있는 `default-panel`, `tall-part`만 넣었다.
- fault 출력은 로컬에서 수동 확인했다.

## 보류한 판단

- template 실행 결과를 Markdown 리포트로 저장하는 기능은 아직 없다.
- template batch 실행은 아직 없다.
- fault 케이스를 “예상된 실패”로 CI에서 검증하는 방식은 아직 정하지 않았다.
- CLI 출력 포맷은 MVP 수준이다.

## 소프트웨어 아키텍처 설명

```text
EquipmentTwin.Cli
    ↓
CliOptions.Parse()
    ↓
RunTemplate()
    ↓
EquipmentTemplate.FromJson()
    ↓
TemplateRunner.RunRecipe()
    ↓
PrintTemplateResult()
```

중요한 출력 구분:

```text
Execution = 장비 실행 성공/실패
Product   = 제품 검사 PASS/FAIL
```

`tall-part`는 장비 실행은 성공하지만 제품 검사는 Fail이다.

## 유지보수할 때 봐야 할 파일

- CLI command parsing: `src/EquipmentTwin.Cli/Program.cs`
- template 실행: `RunTemplate()`
- template 출력: `PrintTemplateResult()`
- CI 검증 명령: `.github/workflows/ci.yml`
- Visual Studio 실행 프로필: `src/EquipmentTwin.Cli/Properties/launchSettings.json`

## 사용자가 이해해야 할 개념

이번 작업은 새 장비 로직을 만든 것이 아니라, 이미 만든 Template Runner를 사람이 직접 실행할 수 있게 CLI로 노출한 것이다.

Core 기능이 CLI로 실행되면 이후 Unity UI도 같은 Core를 호출하면 된다.

즉 CLI는 나중에 Unity를 만들기 전 검증용 조작 패널 역할을 한다.

## 다음 작업

PR #18을 병합한 뒤 다음 후보는 `Inspection Scenario Selection` 또는 `Template Run Markdown Report`이다.

Inspection Scenario Selection을 하면 같은 recipe에서 PASS/FAIL 케이스를 더 자유롭게 고를 수 있다.

Template Run Markdown Report를 하면 template 실행 결과를 포트폴리오 자료로 저장하기 쉬워진다.
