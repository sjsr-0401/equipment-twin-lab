# Goal 010: Core 검증 정리 문서

## 목표

현재 Core가 검증하는 범위와 검증하지 않는 범위를 포트폴리오/면접용으로 정리한다.

## 왜 필요한가

지금까지 상태머신, 가상 IO, Timeout, ScenarioRunner, CLI batch, 알람/복구 시나리오를 만들었다.

하지만 사용자가 면접에서 설명하려면 “무엇을 만들었다”보다 “무엇을 검증했고 무엇은 아직 검증하지 않았다”를 정확히 말할 수 있어야 한다.

## 한 일

- `docs/core-validation.md`를 추가했다.
- 현재 검증 범위를 매트릭스로 정리했다.
- 5개 시나리오의 의미와 기대 최종 상태를 정리했다.
- 로컬/CI 검증 방법을 정리했다.
- 실제 장비 검증과 소프트웨어 모델 검증의 차이를 명확히 분리했다.
- 면접에서 말할 수 있는 짧은 설명과 긴 설명을 추가했다.

## 바뀐 파일

- `docs/core-validation.md`
- `README.md`
- `docs/architecture.md`
- `state/comprehension.md`
- `state/loop-state.md`
- `state/triage.md`
- `logs/2026-06-26.md`
- `plan.md`

## 검증 결과

문서 중심 작업이지만, 현재 문서가 말하는 검증 명령이 실제로 유지되는지 확인했다.

- `dotnet restore EquipmentTwinLab.sln --ignore-failed-sources`: 성공
- `dotnet build EquipmentTwinLab.sln --no-restore --configuration Release`: 성공, 경고 0개, 오류 0개
- `dotnet run --project tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj --no-restore --configuration Release`: 테스트 30개 통과
- `dotnet run --project src\EquipmentTwin.Cli\EquipmentTwin.Cli.csproj --no-restore --configuration Release -- batch scenarios --default-timeouts --report artifacts\scenario-report.md`: 시나리오 5개 통과

## 막힌 점과 해결 방법

- 현재 막힌 점 없음.

## 보류한 판단

- 알람 코드 체계는 이번 Goal에서 구현하지 않는다.
- 실제 PLC/Unity/카메라 계층은 이번 문서에서 설계 확정하지 않는다.
- 실제 장비 안전성 검증처럼 표현하지 않는다.

## 소프트웨어 아키텍처 설명

```text
Console Tests
    ↓
Scenario JSON
    ↓
ScenarioRunner
    ↓
EquipmentCellController
    ↓
EquipmentStateMachine + VirtualIoController + ManualClock
```

이 문서는 새 아키텍처를 만든 것이 아니라, 현재 아키텍처가 어떤 검증 증거를 갖고 있는지 정리한다.

## 유지보수할 때 봐야 할 파일

- 검증 범위 설명: `docs/core-validation.md`
- 전체 아키텍처: `docs/architecture.md`
- 테스트 목록: `tests/EquipmentTwin.Core.Tests/Program.cs`
- CI 명령: `.github/workflows/ci.yml`
- 시나리오 파일: `scenarios/`

## 사용자가 이해해야 할 개념

테스트가 많다고 실제 장비 안전성이 증명되는 것은 아니다.

현재 증명하는 것은 “소프트웨어 모델 안에서 정해진 입력이 들어오면 정해진 상태와 출력으로 간다”는 것이다. 실제 장비에서 안전하다고 말하려면 PLC, 안전 회로, 기구, 센서, 작업자 복구 절차까지 별도로 검증해야 한다.

## 다음 작업

다음 후보는 `알람 코드 체계`다.

알람 코드, 원인, 복구 조건, 표시 메시지를 모델링하면 제조 장비 유지보수 관점의 실무성이 더 올라간다.
