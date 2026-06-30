# Core 검증 정리

이 문서는 `Equipment Twin Lab`의 현재 Core가 무엇을 검증하고, 무엇은 아직 검증하지 않는지 정리한다.

면접이나 포트폴리오에서 이 프로젝트를 설명할 때 핵심 문장:

> 실제 장비가 없어도 상태머신, 가상 IO, Timeout, 알람/복구 시나리오를 반복 실행해서 장비 SW의 핵심 제어 흐름을 검증하는 구조를 만들었다.

## 1. 현재 검증 범위 요약

현재 검증은 실제 장비 검증이 아니라 소프트웨어 모델 검증이다.

검증하는 것:

- 장비 상태가 정해진 순서로 전환되는가
- 잘못된 상태 전이가 거부되는가
- 문 열림, 비상정지, Timeout이 알람 상태로 이어지는가
- 알람이 코드와 원인 이벤트로 추적되는가
- 장비 SW가 Input/Output 방향을 잘못 쓰지 못하게 막는가
- 센서 입력이 상태머신 이벤트로 변환되는가
- 알람 상태에서 위험 출력이 꺼지고 램프/부저가 켜지는가
- JSON 시나리오를 반복 실행할 수 있는가
- CLI batch로 전체 시나리오를 한 번에 검증할 수 있는가
- CLI Markdown 리포트가 활성 알람 코드와 ClearAlarm 가능 조건을 보여주는가
- 가상 모션 축이 Servo On, Home, Move, InPosition, Timeout, Alarm 흐름을 검증하는가

검증하지 않는 것:

- 실제 PLC 통신 안정성
- 실제 모션 축의 위치, 속도, 가속도
- 실제 모션 컨트롤러/드라이버 응답
- 실제 안전 회로 또는 안전 인증
- 실제 카메라 영상 품질
- 실제 설비의 기구 충돌
- 실제 생산 tact time
- 실제 작업자 복구 절차 전체

## 2. 검증 계층

현재 검증은 아래 순서로 쌓여 있다.

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

계층을 분리한 이유는 Unity, 실제 PLC, 실제 카메라가 나중에 붙어도 Core 검증을 유지하기 위해서다.

## 3. 검증 매트릭스

| 검증 대상 | 확인하는 질문 | 대표 파일 | 현재 증거 |
|---|---|---|---|
| 상태머신 | 정상 순서와 잘못된 전이를 구분하는가 | `EquipmentStateMachine.cs` | 콘솔 테스트 |
| 알람 전이 | Door open, Emergency stop, Timeout이 `Alarmed`로 가는가 | `EquipmentStateMachine.cs` | 콘솔 테스트, 시나리오 |
| 알람 코드 | 알람 원인을 코드와 이벤트로 남기는가 | `AlarmCode.cs`, `AlarmInfo.cs` | 콘솔 테스트 |
| 가상 IO | Input과 Output 방향을 강제하는가 | `VirtualIoController.cs`, `EquipmentIoMap.cs` | 콘솔 테스트 |
| 시간/Timeout | 실제 대기 없이 Timeout을 재현하는가 | `ManualClock.cs`, `StateTimeoutPolicy.cs` | 콘솔 테스트, `loading-timeout.json` |
| IO-상태 연결 | 센서 입력이 공정 이벤트로 변환되는가 | `EquipmentCellController.cs` | 콘솔 테스트 |
| 안전 입력 우선순위 | 정상 센서보다 안전 입력을 먼저 처리하는가 | `EquipmentCellController.cs` | 콘솔 테스트 |
| JSON 시나리오 | 장비 흐름을 데이터 파일로 반복 실행하는가 | `ScenarioRunner.cs`, `scenarios/` | 콘솔 테스트, CLI |
| CLI batch | 전체 시나리오 묶음을 한 번에 검증하는가 | `EquipmentTwin.Cli/Program.cs` | 로컬 실행, CI |
| CLI 리포트 | 알람 시나리오의 활성 알람과 ClearAlarm 조건을 보여주는가 | `EquipmentTwin.Cli/Program.cs` | `artifacts/scenario-report.md` 샘플 확인 |
| 가상 모션 축 | Servo On, Home, Move, Timeout, Alarm 흐름을 검증하는가 | `MotionAxis.cs` | 콘솔 테스트 |

## 4. 현재 시나리오 세트

| 시나리오 | 기대 최종 상태 | 의미 |
|---|---|---|
| `normal-cycle.json` | `Complete` | 정상 장비 사이클 |
| `loading-timeout.json` | `Alarmed` | 자재 감지 지연 Timeout |
| `door-open-alarm.json` | `Alarmed` | 운전 중 문 열림 |
| `emergency-stop-alarm.json` | `Alarmed` | 비상정지 입력 |
| `clear-alarm-recovery.json` | `Idle` | 문 닫힘 이후 ClearAlarm 복구 |
| `door-open-clear-blocked.json` | `Alarmed` | 문이 열린 상태에서 ClearAlarm 거부 |
| `emergency-stop-recovery.json` | `Idle` | 비상정지 해제 이후 ClearAlarm 복구 |

이 세트는 아직 실제 생산 장비 전체를 표현하지 않는다. 대신 정상/Timeout/안전 입력/기본 복구를 최소 회귀 검증 세트로 묶은 것이다.

## 5. 다시 검증하는 방법

로컬에서 실행:

```powershell
dotnet restore EquipmentTwinLab.sln --ignore-failed-sources
dotnet build EquipmentTwinLab.sln --no-restore --configuration Release
dotnet run --project tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj --no-restore --configuration Release
dotnet run --project src\EquipmentTwin.Cli\EquipmentTwin.Cli.csproj --no-restore --configuration Release -- batch scenarios --default-timeouts --report artifacts\scenario-report.md
```

GitHub에서는 push/PR마다 CI가 아래를 확인한다.

- restore
- Release build
- console tests
- normal scenario CLI
- timeout scenario CLI
- scenario batch CLI

## 6. 실무 관점에서 의미 있는 부분

### 상태머신

장비 SW에서는 현재 장비가 어떤 상태인지 명확해야 한다.

`Idle`, `Loading`, `Aligning`, `Inspecting`, `Unloading`, `Complete`, `Alarmed` 같은 상태가 있어야 PLC 입력, 모션 명령, 검사 요청, 알람 처리를 안전하게 분리할 수 있다.

### 가상 IO

실제 장비에서는 Input과 Output의 책임이 다르다.

- Input: 센서/PLC가 주고 장비 SW가 읽는다.
- Output: 장비 SW가 쓰고 장치가 반응한다.

현재 Core는 이 방향을 강제로 구분한다. 이 부분은 제조 장비 SW 관점에서 실무성이 있다.

### Timeout

장비는 완료 신호를 무한히 기다리면 안 된다.

`ManualClock`은 실제 30초를 기다리지 않고도 Timeout 상황을 재현하게 해준다. 이 구조는 장비 없이도 지연/미응답 상황을 빠르게 반복 검증하는 데 의미가 있다.

### 시나리오 JSON

시나리오 JSON은 테스트 흐름을 코드 밖으로 꺼낸 것이다.

이 구조가 있으면 나중에 Unity UI, CLI, 자동화 루프가 같은 시나리오 파일을 공유할 수 있다.

## 7. 아직 부족한 부분

현재 Core는 MVP 검증 단계다. 아래는 다음 단계에서 보완해야 한다.

| 부족한 점 | 왜 중요한가 | 후보 Goal |
|---|---|---|
| 알람 코드 체계 확장 필요 | 기본 코드는 생겼지만 레벨/조치/복구 조건은 아직 없다 | Alarm Recovery Goal |
| Timeout 복구 조건이 단순함 | 실제 장비는 작업자 확인, 원인 제거, Reset 조건이 필요하다 | Recovery Report Goal |
| 모션 모델 공정 연결 없음 | 축 모델은 생겼지만 아직 장비 공정/시나리오와 연결되지 않았다 | Motion Scenario Goal |
| 카메라 검사 없음 | 비전 검사 장비 컨셉을 설명하려면 검사 결과 흐름이 필요하다 | Inspection Goal |
| Unity 화면 없음 | 데모 시각화와 포트폴리오 전달력에 필요하다 | Unity Goal |
| 실제 PLC 통신 없음 | 현장 연동성을 설명하려면 어댑터 계층이 필요하다 | PLC Adapter Goal |

## 8. 면접에서 말할 수 있는 설명

짧은 버전:

> 실제 장비가 없는 상황에서 장비 SW의 상태 전이, 가상 IO, Timeout, 알람/복구 시나리오를 C# Core로 분리해 자동 검증했습니다.

조금 긴 버전:

> Unity 화면부터 만들면 데모는 빨리 보이지만 제어 로직이 UI에 묶일 수 있다고 판단했습니다. 그래서 먼저 C#/.NET Core에 상태머신, 가상 IO, Timeout, ScenarioRunner를 만들고, 정상/Timeout/문 열림/비상정지/복구 시나리오를 CLI batch와 CI로 반복 검증하게 했습니다. 이 구조는 나중에 Unity, PLC 어댑터, 카메라 검사 계층을 붙여도 같은 Core 검증을 유지할 수 있습니다.

주의해서 말해야 할 것:

- “실제 장비에서 안전성을 검증했다”라고 말하면 안 된다.
- “실제 PLC와 통신했다”라고 말하면 안 된다.
- “실제 카메라 검사를 완료했다”라고 말하면 안 된다.

정확한 표현:

- “실제 장비 없이 장비 SW Core의 동작 모델을 검증했다.”
- “실제 장비 안전 인증이 아니라 소프트웨어 시뮬레이션 검증이다.”
- “실제 PLC/Unity/카메라는 붙일 수 있도록 계층을 분리해 두었다.”

## 9. 유지보수할 때 먼저 볼 파일

- 전체 구조: `docs/architecture.md`
- 검증 범위: `docs/core-validation.md`
- 상태머신: `src/EquipmentTwin.Core/EquipmentStateMachine.cs`
- IO 모델: `src/EquipmentTwin.Core/Io/`
- 상태/IO 연결: `src/EquipmentTwin.Core/EquipmentCellController.cs`
- 시나리오 실행: `src/EquipmentTwin.Core/Scenarios/ScenarioRunner.cs`
- CLI 실행: `src/EquipmentTwin.Cli/Program.cs`
- 회귀 테스트: `tests/EquipmentTwin.Core.Tests/Program.cs`
- CI: `.github/workflows/ci.yml`

## 10. 다음 권장 작업

다음 구현 후보는 `알람 코드 체계`다.

이유:

- 지금은 알람 원인이 문자열 수준이다.
- 현업 장비에서는 알람 코드, 원인, 복구 조건, 표시 메시지가 중요하다.
- 알람 코드 체계가 생기면 포트폴리오에서 “장비 운영/유지보수 관점까지 고려했다”고 설명하기 쉬워진다.
