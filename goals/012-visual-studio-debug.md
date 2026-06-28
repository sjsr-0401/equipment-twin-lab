# Goal 012: Visual Studio build/debug 지원

## 목표

Visual Studio에서 솔루션을 열고 build/debug할 수 있는 방법을 문서화하고, 자주 쓰는 CLI/테스트 실행 프로필을 추가한다.

## 왜 필요한가

사용자가 나중에 직접 코드를 따라가려면 터미널 명령만으로는 불편하다.

Visual Studio에서 breakpoint를 걸고 상태머신, ScenarioRunner, 알람 코드 흐름을 볼 수 있어야 유지보수와 학습이 쉬워진다.

## 한 일

- `docs/visual-studio.md`를 추가했다.
- `EquipmentTwin.Cli`에 launch profile 3개를 추가했다.
  - `CLI - normal cycle`
  - `CLI - loading timeout`
  - `CLI - batch report`
- `EquipmentTwin.Core.Tests`에 테스트 실행 launch profile을 추가했다.
  - `Tests - run all console tests`
- README에 Visual Studio 가이드 링크를 추가했다.

## 바뀐 파일

- `docs/visual-studio.md`
- `src/EquipmentTwin.Cli/Properties/launchSettings.json`
- `tests/EquipmentTwin.Core.Tests/Properties/launchSettings.json`
- `README.md`
- `logs/2026-06-27.md`
- `plan.md`
- `state/comprehension.md`
- `state/loop-state.md`
- `state/triage.md`

## 검증 결과

- Debug 빌드 성공, 경고 0개, 오류 0개
- Visual Studio CLI 정상 시나리오 profile 실행 성공
- Visual Studio CLI Timeout 시나리오 profile 실행 성공
- Visual Studio CLI batch report profile 실행 성공
- Visual Studio 테스트 profile 실행 성공, 테스트 34개 통과
- Release 빌드 성공, 경고 0개, 오류 0개
- Release 테스트 34개 통과
- Release CLI batch 시나리오 5개 통과
- Draft PR #10 생성 완료
- GitHub Actions push 이벤트: 성공
- GitHub Actions pull_request 이벤트: 성공

## 막힌 점과 해결 방법

- 현재 막힌 점 없음.

## 보류한 판단

- Visual Studio Test Explorer 연동은 아직 하지 않는다.
- xUnit/NUnit 전환은 아직 하지 않는다.
- Unity 디버그는 Unity 프로젝트가 생긴 뒤 별도 문서로 다룬다.

## 소프트웨어 아키텍처 설명

```text
Visual Studio launch profile
    ↓
EquipmentTwin.Cli 또는 EquipmentTwin.Core.Tests
    ↓
ScenarioRunner / Console Test Runner
    ↓
EquipmentTwin.Core
```

이번 작업은 Core 구조를 바꾸지 않는다. Visual Studio가 기존 CLI/테스트 러너를 쉽게 실행하게 하는 진입점을 추가한다.

## 유지보수할 때 봐야 할 파일

- Visual Studio 가이드: `docs/visual-studio.md`
- CLI 실행 프로필: `src/EquipmentTwin.Cli/Properties/launchSettings.json`
- 테스트 실행 프로필: `tests/EquipmentTwin.Core.Tests/Properties/launchSettings.json`

## 사용자가 이해해야 할 개념

Visual Studio에서 debug하려면 “어떤 프로젝트를 시작 프로젝트로 실행할지”가 중요하다.

- 시나리오 실행을 보고 싶으면 `EquipmentTwin.Cli`
- 테스트 전체를 보고 싶으면 `EquipmentTwin.Core.Tests`
- Core 내부 흐름을 보고 싶으면 Core 파일에 breakpoint를 건 뒤 위 두 프로젝트 중 하나로 실행한다.

## 다음 작업

다음 후보는 PR #10 병합 후 `복구 조건 세분화`다.
