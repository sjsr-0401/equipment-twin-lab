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
- JSON 시나리오에서 모션 축을 실행하고 CLI 리포트에 최종 축 상태를 보여주는가
- Equipment Template JSON이 축과 제품 recipe를 올바르게 정의하는가
- recipe가 존재하지 않는 축을 참조하면 거부되는가
- Template Runner가 선택한 recipe의 목표 위치까지 가상 축을 실행하는가
- Fault Scenario가 선택되면 Template Runner가 모션 Timeout 또는 Servo Alarm을 주입하는가
- fault 실행 실패가 기대값일 때 CI에서 성공으로 검증할 수 있는가
- ProductRecipe가 검사 PASS/FAIL 결과와 측정값을 데이터로 정의하는가
- ProductRecipe가 같은 제품의 여러 inspection scenario를 이름으로 선택하게 하는가
- Template Runner가 정상 모션 완료 후 검사 결과를 남기는가
- 장비 실행 성공과 제품 검사 PASS/FAIL을 분리하는가
- CLI에서 template/recipe를 직접 실행하고 장비 실행 결과와 제품 검사 결과를 보여주는가
- template 실행 결과를 Markdown report로 저장하는가
- template 안의 여러 recipe를 batch로 실행하고 PASS/FAIL을 비교하는가

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
| 모션 시나리오 | JSON action으로 축을 움직이고 축 상태를 검증하는가 | `ScenarioRunner.cs`, `motion-axis-*.json` | 콘솔 테스트, CLI batch |
| Equipment Template | 장비 축과 제품 recipe를 데이터로 정의하고 검증하는가 | `EquipmentTemplate.cs`, `templates/` | 콘솔 테스트 |
| Template Runner | 선택한 template/recipe를 가상 모션 실행으로 변환하는가 | `TemplateRunner.cs` | 콘솔 테스트 |
| Fault Model | 선택한 트러블 조건을 모션 실행 중 주입하는가 | `FaultScenario.cs`, `TemplateRunner.cs` | 콘솔 테스트 |
| Fault Expected-Failure Report | fault로 인한 실행 실패를 기대값으로 report/CI에서 검증하는가 | `EquipmentTwin.Cli/Program.cs` | 로컬 실행, CI |
| Inspection Result Model | 제품 검사 PASS/FAIL과 측정값을 데이터로 남기는가 | `InspectionResult.cs`, `ProductRecipe.cs`, `TemplateRunner.cs` | 콘솔 테스트 |
| Inspection Scenario Selection | 같은 recipe에서 선택한 검사 케이스로 PASS/FAIL을 바꾸는가 | `InspectionScenario.cs`, `ProductRecipe.cs`, `TemplateRunner.cs` | 콘솔 테스트, CLI, CI |
| Template Runner CLI | template/recipe를 명령어로 실행하고 결과를 출력/저장하는가 | `EquipmentTwin.Cli/Program.cs` | 로컬 실행, CI |
| Template Batch Report | 여러 recipe를 한 번에 실행하고 결과를 Markdown으로 비교하는가 | `EquipmentTwin.Cli/Program.cs` | 로컬 실행, CI |

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
| `motion-axis-normal.json` | `Idle` + X축 `InPosition` | X축 Servo On, Home, Move 정상 흐름 |
| `motion-axis-timeout.json` | `Idle` + X축 `Alarmed` | X축 이동 중 Timeout 알람 흐름 |

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
- template default-panel CLI
- template tall-part CLI
- template selected inspection scenario CLI
- template expected fault CLI
- template run Markdown report 생성
- template batch Markdown report 생성

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
| Fault Model 범위가 모션에 한정됨 | MotionTimeout/ServoAlarm은 지원하지만 IO fault, sensor fault는 아직 없다 | IO Fault Goal |
| 카메라 검사 없음 | 검사 결과 데이터와 CLI 출력은 생겼지만 실제 이미지 처리나 Unity 카메라는 아직 없다 | Camera Adapter Goal |
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

다음 구현 후보는 `Inspection Scenario Batch Matrix` 또는 `Fault Scenario Catalog`이다.

이유:

- Fault Expected-Failure Report로 fault 주입처럼 실행 실패가 기대값인 케이스를 CI에서 안전하게 검증할 수 있게 됐다.
- 다음은 여러 inspection scenario를 batch matrix로 비교하거나, fault 종류와 기대 알람을 catalog로 정리하는 단계가 적절하다.

## Unity Process Player Skeleton Validation

Goal 027 adds Unity scripts, but GitHub Actions does not run Unity Editor.

Current CI validation checks:

- .NET build still succeeds.
- Core tests still pass.
- process CLI still generates `moly-ald-timeline.json`.
- Unity skeleton source files exist in the expected paths.

CI file-existence check:

```text
unity/EquipmentTwin.Unity/Assets/EquipmentTwin/Runtime/MolyAldTimelineData.cs
unity/EquipmentTwin.Unity/Assets/EquipmentTwin/Runtime/MolyAldTimelineLoader.cs
unity/EquipmentTwin.Unity/Assets/EquipmentTwin/Runtime/MolyAldProcessPlayer.cs
unity/EquipmentTwin.Unity/Assets/EquipmentTwin/Runtime/MolyAldProcessHud.cs
unity/EquipmentTwin.Unity/Assets/StreamingAssets/moly-ald-timeline.sample.json
```

What this proves:

- Unity adapter files are present.
- Core/CLI changes were not broken by adding Unity files.
- A sample timeline exists for local Unity Editor testing.

What this does not prove yet:

- Unity Editor compilation.
- Unity Play Mode behavior.
- 3D rendering quality.

Those are intentionally next-step validations once the Unity visual layer exists.

## Unity Primitive Visual Validation

Goal 028 adds runtime Unity visual scripts:

- `MolyAldPrimitiveVisualizer`
- `MolyAldDemoBootstrap`

Current automated validation:

- .NET solution still builds.
- Core tests still pass.
- process CLI still generates the ALD timeline.
- CI checks that Unity runtime scripts exist at expected paths.
- sample timeline JSON still parses locally.

Current manual validation boundary:

- Unity Editor compilation is still not executed by GitHub Actions.
- Local Unity `6000.3.2f1` exists, but batchmode compile was blocked by Unity licensing: `No valid Unity Editor license found`.
- Unity Play Mode must be checked locally after opening `unity/EquipmentTwin.Unity`.

Manual Unity smoke test:

1. Open `unity/EquipmentTwin.Unity` in Unity Hub.
2. Confirm Unity Hub is signed in and the Editor license is active.
3. Create one empty GameObject.
4. Add `MolyAldDemoBootstrap`.
5. Press Play.
6. Confirm the chamber, wafer, film overlay, pressure column, gauge needle, and three valve spheres update while the process advances.

## Unity Smoke Test Harness

Goal 029 adds a repeatable smoke-test harness for the Unity side.

Files:

```text
unity/EquipmentTwin.Unity/Assets/EquipmentTwin/Editor/MolyAldEditorSmokeTest.cs
scripts/Invoke-UnitySmokeTest.ps1
docs/unity-smoke-test.md
```

Batch command:

```powershell
.\scripts\Invoke-UnitySmokeTest.ps1
```

Manual Unity command:

```text
Equipment Twin > Run Moly ALD Smoke Test
```

Expected marker:

```text
EQUIPMENT_TWIN_UNITY_SMOKE_TEST_PASS
```

What it checks:

- sample timeline JSON exists;
- Unity timeline loader parses the sample;
- demo scene root can be created;
- player, HUD, visualizer, and bootstrap components are present;
- primitive renderers are generated.

## Process Timeline JSON Export Validation

Goal 026 validates that the ALD process result can be exported as Unity-ready JSON.

Validated by console tests:

- `Moly ALD timeline document maps run result`
- `Moly ALD timeline JSON uses Unity friendly shape`

Validated by CLI:

```powershell
dotnet run --project src\EquipmentTwin.Cli -- process run processes\public-moly-ald-metallization.json --report artifacts\moly-ald-process-report.md --timeline artifacts\moly-ald-timeline.json
```

Expected output includes:

```text
Report: artifacts\moly-ald-process-report.md
Timeline: artifacts\moly-ald-timeline.json
```

Fault timeline validation:

```powershell
dotnet run --project src\EquipmentTwin.Cli -- process run processes\public-moly-ald-metallization.json --fault pumpdown-timeout --timeline artifacts\moly-ald-fault-timeline.json
```

Expected behavior:

- CLI exits with code `1` because the process fault is a real execution failure.
- The fault timeline file is still written.
- The JSON contains `success: false`, `finalStep: "Alarmed"`, and `faultScenarioName: "pumpdown-timeout"`.

What this proves:

- Unity does not need to parse Markdown.
- The process runner produces a stable software-consumable data contract.
- Normal and fault process paths can both be replayed visually later.

## Public Molybdenum ALD Process Validation

Goal 025 validates a public/synthetic molybdenum ALD process model.

Validated by console tests:

- `Moly ALD recipe JSON loads public demo file`
- `Moly ALD runner completes public demo process`
- `Moly ALD runner records valve timeline`
- `Moly ALD runner injects pumpdown fault`
- `Moly ALD recipe rejects invalid cycle count`
- `Moly ALD recipe rejects duplicate fault names`

Validated by CLI:

```powershell
dotnet run --project src\EquipmentTwin.Cli -- process run processes\public-moly-ald-metallization.json --report artifacts\moly-ald-process-report.md
```

Expected normal result:

```text
Execution: PASS
Final:     Complete
Stations:  4
Cycles:    4
Thickness: 8 A / target 8 A
```

Validated fault behavior:

```powershell
dotnet run --project src\EquipmentTwin.Cli -- process run processes\public-moly-ald-metallization.json --fault pumpdown-timeout
```

Expected fault result:

```text
Execution: FAIL
Final:     Alarmed
Fault:     pumpdown-timeout
Thickness: 0 A / target 8 A
```

What this proves:

- Recipe JSON can be loaded and validated.
- The process sequence runs deterministically without real equipment.
- Dose/purge/valve/tickness timeline data is produced for Unity.
- A setup fault stops the process before film growth.

What this does not prove:

- It does not prove real ALD process accuracy.
- It does not prove real equipment safety.
- It does not use real vendor recipe values.
