# Equipment Twin Lab

실제 장비가 없어도 장비 SW의 핵심 구조를 검증할 수 있게 만드는 제조 장비 디지털 트윈 프로젝트다.

## 목표

- 장비 상태 전이, PLC/IO, 모션, 센서, 카메라 검사를 소프트웨어로 모델링한다.
- Unity 3D 시뮬레이터는 나중에 붙이고, 핵심 장비 로직은 C#/.NET으로 먼저 검증한다.
- 작업 결과를 초보자도 이해할 수 있게 문서화한다.

## 현재 단계

현재 MVP는 장비 상태머신, 가상 IO 모델, Clock/Timeout 모델, IO-상태 연결 계층, 공정 시나리오 JSON Runner, Scenario CLI 실행기, batch 리포트 실행기, 알람/복구 시나리오 검증, 알람 코드 체계, 알람 복구 조건, CLI 리포트 알람/복구 조건 표시, 가상 모션 축 모델, 모션 시나리오 JSON action, CLI 리포트 모션 축 표시, Equipment Template / Product Recipe 최소 모델, Template Runner, Fault Model, Inspection Result Model까지 포함한다.

```text
Idle → Loading → Aligning → Inspecting → Unloading → Complete
```

알람 상황에서는 어떤 단계에서든 `Alarmed` 상태로 전환된다.

현재 알람은 코드와 메시지를 함께 남긴다.

```text
DoorOpened    = 1001
EmergencyStop = 1002
StateTimeout  = 1003
```

가상 모션 축은 실제 서보 드라이버 없이 Servo On, Home, Move, InPosition, Timeout, Servo Alarm 흐름을 테스트한다.

```text
Disabled → Ready → Homing → InPosition → Moving → InPosition
                                      ↘ Alarmed
```

검사 결과 모델은 실제 카메라 없이도 제품 PASS/FAIL과 측정값을 데이터로 남긴다.

```text
ProductRecipe
→ InspectionResultSpec
→ TemplateRunResult.InspectionResult
```

장비 실행 성공과 제품 검사 PASS/FAIL은 분리한다. 예를 들어 장비는 정상 동작했지만 제품은 `HEIGHT_OVER_LIMIT`로 NG가 될 수 있다.

가상 IO는 실제 PLC 없이 입력 센서와 출력 명령을 분리해서 테스트한다.

```text
Input  = 센서/PLC가 장비 SW에게 알려주는 값
Output = 장비 SW가 밸브/램프/부저 같은 장치에 내리는 명령
```

Clock/Timeout 모델은 실제로 기다리지 않고도 “정해진 시간 안에 작업 완료 신호가 오지 않는 상황”을 테스트한다.

예:

```text
Loading 상태에서 30초 안에 LoadComplete가 오지 않으면 Alarmed 전환
```

IO-상태 연결 계층은 센서 입력을 상태머신 이벤트로 바꾼다.

예:

```text
DI_LOAD_PRESENT = true
→ LoadComplete 이벤트
→ Loading 상태에서 Aligning 상태로 전환
```

공정 시나리오 JSON은 위 흐름을 파일로 정의한다.

예:

```text
scenarios/normal-cycle.json
→ 정상 장비 사이클 실행

scenarios/loading-timeout.json
→ Loading 상태 Timeout 알람 실행
```

현재 시나리오 목록:

- `scenarios/normal-cycle.json`: 정상 사이클
- `scenarios/loading-timeout.json`: Loading Timeout 알람
- `scenarios/door-open-alarm.json`: 문 열림 알람
- `scenarios/emergency-stop-alarm.json`: 비상정지 알람
- `scenarios/clear-alarm-recovery.json`: 문 열림 알람 이후 ClearAlarm 복구
- `scenarios/door-open-clear-blocked.json`: 문이 열린 상태에서는 ClearAlarm 거부
- `scenarios/emergency-stop-recovery.json`: 비상정지 해제 이후 ClearAlarm 복구
- `scenarios/motion-axis-normal.json`: X축 Servo On, Home, Move, InPosition 정상 흐름
- `scenarios/motion-axis-timeout.json`: X축 이동 중 Timeout 알람 흐름

## 프로젝트 구조

```text
src/EquipmentTwin.Core
  장비 핵심 로직

tests/EquipmentTwin.Core.Tests
  외부 테스트 패키지 없이 실행하는 간단한 테스트 러너

state/
  현재 진행 상태와 다음 작업

goals/
  Goal 단위 작업 정의

logs/
  일일 작업 로그

docs/
  아키텍처와 유지보수 설명

scenarios/
  반복 실행 가능한 장비 운전 시나리오 JSON

templates/
  장비 구성과 제품 recipe를 정의하는 JSON
```

## 아키텍처 설명

현재 코드 구조와 유지보수 방법은 아래 문서에 정리한다.

[docs/architecture.md](docs/architecture.md)

현재 Core가 검증하는 범위와 한계는 아래 문서에 정리한다.

[docs/core-validation.md](docs/core-validation.md)

Visual Studio에서 build/debug하는 방법은 아래 문서에 정리한다.

[docs/visual-studio.md](docs/visual-studio.md)

## 실행 방법

```powershell
dotnet restore --ignore-failed-sources
dotnet build --no-restore
dotnet run --project tests\EquipmentTwin.Core.Tests --no-restore
```

## 시나리오 직접 실행

정상 사이클:

```powershell
dotnet run --project src\EquipmentTwin.Cli -- scenarios\normal-cycle.json
```

Loading Timeout:

```powershell
dotnet run --project src\EquipmentTwin.Cli -- scenarios\loading-timeout.json --default-timeouts
```

전체 시나리오 batch 실행과 Markdown 리포트 저장:

```powershell
dotnet run --project src\EquipmentTwin.Cli -- batch scenarios --default-timeouts --report artifacts\scenario-report.md
```

생성되는 Markdown 리포트는 각 시나리오의 최종 상태뿐 아니라 `Active Alarm`, `Clear Condition`, `Motion Axes`도 표시한다.

예를 들어 문이 열린 알람 시나리오는 `DoorOpened (1001)`과 `Blocked: Door must be closed before clearing DoorOpened alarm.`처럼 표시된다.

## 자동 검증

GitHub Actions CI가 push/PR마다 아래 검증을 실행한다.

```text
dotnet restore EquipmentTwinLab.sln --ignore-failed-sources
dotnet build EquipmentTwinLab.sln --no-restore --configuration Release
dotnet run --project tests/EquipmentTwin.Core.Tests/EquipmentTwin.Core.Tests.csproj --no-restore --configuration Release
dotnet run --project src/EquipmentTwin.Cli/EquipmentTwin.Cli.csproj --no-restore --configuration Release -- scenarios/normal-cycle.json
dotnet run --project src/EquipmentTwin.Cli/EquipmentTwin.Cli.csproj --no-restore --configuration Release -- scenarios/loading-timeout.json --default-timeouts
dotnet run --project src/EquipmentTwin.Cli/EquipmentTwin.Cli.csproj --no-restore --configuration Release -- batch scenarios --default-timeouts --report artifacts/scenario-report.md
```

## GitHub

공개 저장소:

<https://github.com/sjsr-0401/equipment-twin-lab>
