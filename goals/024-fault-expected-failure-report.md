# Goal 024: Fault Expected-Failure Report

## 목표

fault 주입처럼 장비 실행 실패가 정상 기대값인 케이스를 CLI, report, CI에서 안전하게 검증한다.

## 왜 필요한가

fault scenario는 일부러 장비 실행을 실패시키는 테스트다.

예를 들어 `x-axis-move-timeout`은 X축 이동 중 Timeout 알람을 만들기 위한 케이스다. 이때 `Execution: FAIL`은 버그가 아니라 기대한 결과다.

하지만 일반 CLI exit code 기준으로는 실패가 exit code 1이므로 CI에서 그대로 실행하면 전체 CI가 실패한다.

그래서 `--expect-execution-failure` 옵션을 추가해 “이번 실행에서는 실패가 기대값이다”를 명시한다.

## 한 일

- CLI `template run`에 `--expect-execution-failure` 옵션을 추가했다.
- 옵션이 없으면 기존처럼 `Execution: FAIL`은 exit code 1로 유지했다.
- 옵션이 있으면 `Execution: FAIL`을 expectation met로 보고 exit code 0을 반환한다.
- `--expect-execution-failure`는 반드시 `--fault`와 함께 쓰도록 제한했다.
- 콘솔 출력에 아래 항목을 추가했다.
  - `Expected execution`
  - `Execution expectation`
- Markdown report summary에 아래 항목을 추가했다.
  - Expected Execution
  - Expectation
- CI에 expected fault 실행을 추가했다.
- Visual Studio launch profile에 expected fault 실행을 추가했다.
- README, architecture, validation, Visual Studio 문서를 갱신했다.
- Draft PR #22를 만들었다.
- GitHub Actions push/pull_request CI 성공을 확인했다.

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
- expected fault CLI 실행 성공
- expected fault Markdown report 생성 확인
- 기존 fault CLI는 옵션이 없으면 exit code 1 유지 확인
- `--expect-execution-failure`를 `--fault` 없이 쓰면 거부되는 것 확인
- GitHub Actions push CI 성공
- GitHub Actions pull_request CI 성공

## 막힌 점과 해결 방법

- 일반 fault 실행은 실패가 맞지만 CI에서는 실패로 처리되는 문제가 있었다.
- 실행 결과 자체를 바꾸지 않고, 기대값 판정만 추가했다.
- 그래서 report에는 `Execution: FAIL`이 그대로 남고, 별도 컬럼으로 `Expected execution: FAIL`, `Execution expectation: MET`를 표시한다.

## 소프트웨어 아키텍처 설명

```text
template run + --fault
    ↓
TemplateRunner.RunRecipe()
    ↓
Execution FAIL
    ↓
CliOptions.ExpectExecutionFailure
    ↓
exit code 0 or 1 결정
    ↓
Markdown report
```

중요한 점은 `TemplateRunResult.Success`의 의미를 바꾸지 않았다는 것이다.

`Success`는 계속 “장비 실행이 알람 없이 완료됐는가”를 뜻한다.

새 옵션은 “이번 실행에서 기대한 결과와 실제 결과가 맞았는가”만 판단한다.

## 유지보수할 때 봐야 할 파일

- CLI option parsing: `src/EquipmentTwin.Cli/Program.cs`
- exit code 판단: `IsTemplateExecutionExpectationMet()`
- 콘솔 출력: `PrintTemplateResult()`
- report 생성: `BuildTemplateMarkdownReport()`
- CI 검증: `.github/workflows/ci.yml`
- Visual Studio 실행 프로필: `src/EquipmentTwin.Cli/Properties/launchSettings.json`

## 사용자가 이해해야 할 개념

```text
Execution FAIL
    = 장비 실행은 실패했다.

Expected execution FAIL
    = 이번 테스트에서는 실패가 기대값이다.

Execution expectation MET
    = 실제 결과가 기대값과 맞았다.
```

이 구조 덕분에 fault 재현 테스트를 CI에서 안전하게 돌릴 수 있다.

## 다음 작업

먼저 PR #22를 Ready로 전환하고 병합한다.

그 다음 후보는 `Inspection Scenario Batch Matrix` 또는 `Fault Scenario Catalog`다.

Inspection Scenario Batch Matrix는 recipe × inspection scenario 조합을 한 번에 돌려 PASS/FAIL matrix를 만든다.

Fault Scenario Catalog는 fault 종류별 기대 알람, 기대 축 상태, 복구 조건을 문서와 report로 정리한다.
