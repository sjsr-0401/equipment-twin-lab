# Comprehension Notes

이 파일은 사용자가 “내가 뭘 만들고 있는지”를 잊지 않도록 정리하는 곳이다.

## 오늘 이해해야 할 핵심

- Loop Engineering은 프롬프트를 많이 치는 방식이 아니라, 목표와 검증 기준을 파일에 남기고 에이전트가 그 기준을 반복해서 따르게 하는 방식이다.
- 현재는 자동화 자체보다 “에이전트가 기억할 문서 구조”를 먼저 만드는 단계다.
- 자동화가 강해질수록 사용자가 코드를 이해하지 못하는 위험도 커지므로, 매 작업마다 이해 체크를 남긴다.

## 2026-06-25 이해 요약

### 오늘 한 일

- 대표 프로젝트 폴더 `active/equipment-twin-lab`를 만들었다.
- 기존 계획/운영 문서를 새 프로젝트 폴더로 옮겼다.
- .NET 8 솔루션을 만들었다.
- 첫 MVP로 장비 상태머신을 구현했다.
- 콘솔 테스트 5개로 정상 시퀀스, 잘못된 전이, 알람 전이를 확인했다.

### 왜 필요한가

장비 SW는 화면보다 상태 관리가 먼저다.

예를 들어 장비가 `Inspecting` 상태인지 `Alarmed` 상태인지 알아야 PLC 신호, 모션 명령, 카메라 검사, 사용자 버튼을 안전하게 판단할 수 있다.

### 제조 장비 SW와 어떤 관련이 있는가

- `Idle`: 장비가 대기 중인 상태
- `Loading`: 자재 또는 웨이퍼를 투입하는 상태
- `Aligning`: 위치를 맞추는 상태
- `Inspecting`: 카메라 또는 검사 로직이 동작하는 상태
- `Unloading`: 자재를 배출하는 상태
- `Complete`: 한 사이클이 끝난 상태
- `Alarmed`: 문 열림, 비상정지 같은 이상 상태

### 내가 이해해야 할 개념

상태머신은 “현재 상태에서 어떤 이벤트만 허용할지” 정하는 규칙이다.

잘못된 이벤트가 들어오면 무시하거나 알람으로 보내야 한다. 실제 장비에서는 이 부분이 장비 오동작과 안전 문제를 줄이는 핵심이다.

### 확인 방법

```powershell
dotnet restore --ignore-failed-sources
dotnet build --no-restore
dotnet run --project tests\EquipmentTwin.Core.Tests --no-restore
```

### 아직 모르는 것

- 실제 장비 기준으로 DoorOpened가 언제 알람이고 언제 무시 가능한지
- EmergencyStop 이후 복귀 절차를 `ClearAlarm` 하나로 단순화해도 되는지
- PLC/IO 신호명을 어떤 방식으로 모델링할지

## 2026-06-25 추가 이해 요약: 가상 IO

### 오늘 한 일

- GitHub 공개 저장소 `equipment-twin-lab`를 만들었다.
- 현재 프로젝트를 GitHub `main` 브랜치에 푸시했다.
- 새 브랜치 `goal/002-virtual-io`에서 가상 IO 모델을 만들었다.
- 입력 신호와 출력 신호를 분리했다.
- 테스트를 5개에서 11개로 늘렸다.

### 왜 필요한가

장비 SW는 PLC와 계속 신호를 주고받는다.

실제 PLC가 없어도 IO 모델이 있으면 “센서가 켜졌을 때 장비가 어떻게 반응해야 하는지”, “장비가 어떤 출력을 켜야 하는지”를 테스트할 수 있다.

### 제조 장비 SW와 어떤 관련이 있는가

- Input: 문 닫힘 센서, 비상정지 입력, 자재 감지 센서처럼 장비 SW가 읽는 값
- Output: 진공 ON, 스테이지 이동 요청, 적색 램프, 부저처럼 장비 SW가 내보내는 명령

실제 장비에서는 이런 신호가 PLC 주소나 태그와 연결된다.

### 내가 이해해야 할 개념

장비 SW는 Input을 마음대로 바꾸면 안 된다.

Input은 실제 센서나 PLC가 바꾸는 값이다. 반대로 Output은 장비 SW가 명령하는 값이다.

이번 구현은 이 방향을 강제로 구분한다. 그래서 장비 SW가 Input을 쓰려고 하면 실패하고, 시뮬레이터가 Output을 바꾸려고 해도 실패한다.

### 확인 방법

```powershell
dotnet build --no-restore
dotnet run --project tests\EquipmentTwin.Core.Tests --no-restore
```

### 아직 모르는 것

- 실제 PLC 주소 체계를 어떻게 표현할지
- IO 값 변화와 장비 상태머신 이벤트를 어떻게 연결할지
- 비상정지/문열림 같은 안전 입력을 어떤 우선순위로 처리할지

## 2026-06-25 추가 이해 요약: CI

### 오늘 한 일

- GitHub Actions CI 파일을 추가했다.
- push 또는 PR이 올라오면 GitHub가 자동으로 빌드와 테스트를 실행하게 했다.

### 왜 필요한가

에이전트가 코드를 고칠 때마다 사람이 매번 빌드/테스트를 직접 돌리면 오래 걸린다.

CI는 GitHub에서 자동으로 “이 변경이 최소한 빌드되고 테스트를 통과하는지” 확인한다.

### 제조 장비 SW와 어떤 관련이 있는가

장비 SW는 작은 변경 하나가 상태 전이, IO, 알람 처리에 영향을 줄 수 있다.

CI는 실제 장비 안전성을 증명하지는 못하지만, 기존 시뮬레이션 로직이 깨졌는지는 빠르게 잡아준다.

### 내가 이해해야 할 개념

CI는 “자동 검증기”다.

Codex나 Claude가 “테스트 통과”라고 말하는 것보다 GitHub Actions가 매번 같은 명령을 실행해서 결과를 남기는 편이 더 신뢰할 수 있다.

### 확인 방법

GitHub Actions 페이지에서 확인한다.

```text
https://github.com/sjsr-0401/equipment-twin-lab/actions
```

### 아직 모르는 것

- Unity 프로젝트가 생겼을 때 Unity 빌드를 CI에 포함할지
- 테스트 커버리지 리포트가 필요한지
- PR 병합 조건으로 CI 필수를 설정할지

## 2026-06-25 추가 이해 요약: Clock / Timeout

### 오늘 한 일

- PR #1을 CI 성공 확인 후 main에 병합했다.
- 새 브랜치 `goal/004-clock-timeout`를 만들었다.
- 실제 시간용 `SystemClock`과 테스트용 `ManualClock`을 추가했다.
- 상태별 Timeout 정책을 추가했다.
- 상태머신이 Timeout을 감지하면 `Alarmed` 상태로 전환되게 했다.
- 테스트를 11개에서 17개로 늘렸다.

### 왜 필요한가

장비는 특정 상태에서 무한히 기다리면 안 된다.

예를 들어 Loading 상태에서 자재 감지 신호가 정해진 시간 안에 들어오지 않으면 알람을 띄우고 멈춰야 한다.

### 제조 장비 SW와 어떤 관련이 있는가

실제 장비에서는 Timeout이 자주 쓰인다.

- 실린더 전진 완료 센서가 안 들어옴
- 스테이지 이동 완료가 안 들어옴
- 카메라 검사 완료가 안 들어옴
- 자재 감지 센서가 안 들어옴

이런 상황을 장비 SW가 감지하지 못하면 장비가 멈추지 않고 계속 기다리거나 다음 단계로 잘못 넘어갈 수 있다.

### 내가 이해해야 할 개념

`ManualClock`은 테스트용 가짜 시간이다.

실제로 30초를 기다리지 않고 코드에서 시간을 30초 전진시켜 Timeout 상황을 만든다. 그래서 테스트가 빠르고 반복 가능하다.

### 확인 방법

```powershell
dotnet build --no-restore
dotnet run --project tests\EquipmentTwin.Core.Tests --no-restore
```

현재 로컬 결과:

- 빌드 성공
- 경고 0개
- 오류 0개
- 테스트 17개 통과

### 아직 모르는 것

- Timeout이 발생한 뒤 실제 장비 복구 절차를 어떻게 모델링할지
- Timeout 우선순위를 DoorOpened/EmergencyStop과 어떻게 정렬할지
- IO 입력 변화와 Timeout 정책을 어떻게 연결할지

## 2026-06-25 추가 이해 요약: 상태머신 + IO 연결

### 오늘 한 일

- PR #2를 CI 성공 확인 후 main에 병합했다.
- 새 브랜치 `goal/005-io-state-bridge`를 만들었다.
- `EquipmentCellController`를 추가했다.
- IO 입력을 상태머신 이벤트로 변환했다.
- 상태별 출력 동기화를 추가했다.
- 테스트를 17개에서 23개로 늘렸다.

### 왜 필요한가

상태머신과 IO가 따로 있으면 아직 “장비처럼 동작한다”고 말하기 어렵다.

이번 작업으로 센서 입력이 장비 상태를 바꾸고, 장비 상태가 출력 신호를 바꾸는 기본 흐름이 생겼다.

### 제조 장비 SW와 어떤 관련이 있는가

실제 장비에서는 PLC 입력이 들어오면 장비 시퀀스가 진행된다.

예:

- 자재 감지 센서 ON → Loading 완료
- 정렬 완료 센서 ON → Aligning 완료
- 검사 완료 신호 ON → Inspecting 완료
- 배출 완료 신호 ON → Unloading 완료

또한 상태에 따라 출력이 바뀐다.

- Loading/Inspecting/Unloading → Vacuum ON
- Aligning → Stage move request ON
- Alarmed → Red lamp, buzzer ON

### 내가 이해해야 할 개념

`EquipmentCellController`는 실제 PLC가 아니라 “상태머신과 가상 IO 사이의 연결부”다.

이 연결부가 있어야 나중에 Unity 시뮬레이터, 시나리오 파일, 실제 PLC 어댑터를 붙일 수 있다.

### 확인 방법

```powershell
dotnet build --no-restore
dotnet run --project tests\EquipmentTwin.Core.Tests --no-restore
```

현재 로컬 결과:

- 빌드 성공
- 경고 0개
- 오류 0개
- 테스트 23개 통과

### 아직 모르는 것

- 출력 정책이 실제 장비 기준으로 충분히 현실적인지
- 상태별 출력 동기화를 설정 파일로 뺄지
- 여러 센서가 동시에 들어왔을 때 우선순위를 더 명확히 테이블로 분리할지

## 2026-06-25 추가 이해 요약: 유지보수를 위한 기록 방식

### 오늘 정한 규칙

앞으로 작업로그에는 단순히 “무엇을 만들었다”만 쓰지 않는다.

반드시 아래 내용을 남긴다.

- 막힌 점
- 해결 방법
- 보류한 판단
- 어떤 계층을 바꿨는지
- 왜 그런 구조로 짰는지
- 나중에 유지보수할 때 어떤 파일을 봐야 하는지

### 왜 필요한가

에이전트가 빠르게 코드를 만들수록 사용자가 이해하지 못하는 코드가 쌓일 수 있다.

이 프로젝트는 포트폴리오용이면서 동시에 사용자가 직접 설명하고 유지보수할 수 있어야 한다. 그래서 코드 변경마다 아키텍처 설명을 남긴다.

### 현재 아키텍처 기준

```text
Scenario / Unity / User Command
        ↓
EquipmentCellController
        ↓
EquipmentStateMachine
        ↓
VirtualIoController
```

### 유지보수할 때 먼저 볼 문서

- `docs/architecture.md`
- `logs/YYYY-MM-DD.md`
- `state/loop-state.md`
- `goals/NNN-*.md`

## 2026-06-25 추가 이해 요약: 공정 시나리오 JSON Runner

### 오늘 한 일

- PR #3을 CI 성공 확인 후 main에 병합했다.
- 새 브랜치 `goal/006-scenario-json`를 만들었다.
- JSON 시나리오 모델을 추가했다.
- ScenarioRunner를 추가했다.
- 정상 사이클 JSON과 Loading Timeout JSON을 추가했다.
- 테스트를 23개에서 27개로 늘렸다.

### 왜 필요한가

코드에 직접 테스트 순서를 써두면 사용자가 시나리오를 바꾸기 어렵다.

JSON 파일로 분리하면 정상 공정, Timeout, 비상정지 같은 흐름을 파일로 만들고 같은 실행기로 반복 검증할 수 있다.

### 제조 장비 SW와 어떤 관련이 있는가

실제 장비에서는 recipe, sequence, test scenario 같은 개념이 중요하다.

이번 구현은 실제 recipe 시스템은 아니지만, 장비 동작 흐름을 데이터 파일로 분리하는 첫 단계다.

### 소프트웨어 아키텍처 설명

```text
Scenario JSON
    ↓
EquipmentScenario
    ↓
ScenarioRunner
    ↓
EquipmentCellController
    ↓
EquipmentStateMachine + VirtualIoController + ManualClock
```

시나리오는 “무엇을 할지”를 담고, Runner는 “어떻게 실행할지”를 담당한다.

### 유지보수 포인트

- 시나리오 문법 변경: `ScenarioStep.cs`
- 새 action 추가: `ScenarioStepAction.cs`, `ScenarioRunner.cs`
- 샘플 시나리오 추가: `scenarios/`
- 시나리오 테스트: `tests/EquipmentTwin.Core.Tests/Program.cs`

### 막힌 점과 해결 방법

- 현재 막힌 점 없음.
- 외부 JSON 패키지 없이 .NET 기본 `System.Text.Json`으로 처리했다.

### 보류한 판단

- 분기/반복/변수 기능은 아직 넣지 않았다.
- CLI 실행기는 다음 Goal 후보로 남겼다.
- Unity에서 시나리오를 선택하는 UI는 아직 만들지 않았다.

### 확인 방법

```powershell
dotnet build --no-restore
dotnet run --project tests\EquipmentTwin.Core.Tests --no-restore
```

현재 로컬 결과:

- 빌드 성공
- 경고 0개
- 오류 0개
- 테스트 27개 통과

## 2026-06-25 추가 이해 요약: Scenario CLI 실행기

### 오늘 한 일

- PR #4를 CI 성공 확인 후 main에 병합했다.
- 새 브랜치 `goal/007-scenario-cli`를 만들었다.
- `EquipmentTwin.Cli` 콘솔 프로젝트를 추가했다.
- JSON 시나리오를 명령어로 실행할 수 있게 했다.
- CI에서 정상 시나리오와 Timeout 시나리오를 CLI로 실행하게 했다.

### 왜 필요한가

테스트 코드 안에서만 시나리오가 실행되면 사용자가 직접 눈으로 확인하기 어렵다.

CLI가 있으면 Unity가 없어도 터미널에서 장비 시나리오를 실행하고 단계별 결과와 최종 IO 상태를 볼 수 있다.

### 소프트웨어 아키텍처 설명

```text
Command Line
    ↓
EquipmentTwin.Cli
    ↓
EquipmentScenario.FromJson()
    ↓
ScenarioRunner
    ↓
EquipmentCellController
```

CLI는 Core 로직을 직접 갖지 않는다. Core의 ScenarioRunner를 호출하는 얇은 실행 계층이다.

### 유지보수 포인트

- CLI 인자 처리: `src/EquipmentTwin.Cli/Program.cs`
- 시나리오 실행 로직: `src/EquipmentTwin.Core/Scenarios/ScenarioRunner.cs`
- 샘플 시나리오: `scenarios/`
- CI CLI 실행 단계: `.github/workflows/ci.yml`

### 막힌 점과 해결 방법

- 현재 막힌 점 없음.
- 외부 CLI 파서 패키지를 쓰지 않고 직접 인자 parsing을 구현했다.

### 보류한 판단

- 여러 시나리오 일괄 실행은 아직 넣지 않았다.
- CLI 결과를 JSON/Markdown 리포트로 저장하는 기능은 아직 넣지 않았다.
- Unity UI와 연결하는 기능은 아직 보류했다.

### 확인 방법

```powershell
dotnet run --project src\EquipmentTwin.Cli -- scenarios\normal-cycle.json
dotnet run --project src\EquipmentTwin.Cli -- scenarios\loading-timeout.json --default-timeouts
```

## 2026-06-25 추가 이해 요약: CLI batch 실행 + Markdown 리포트

### 오늘 한 일

- PR #5를 CI 성공 확인 후 main에 병합했다.
- 새 브랜치 `goal/008-cli-batch-report`를 만들었다.
- CLI에 `batch` 명령을 추가했다.
- `scenarios/` 폴더의 모든 JSON 시나리오를 한 번에 실행하게 했다.
- `--report` 옵션으로 Markdown 리포트를 저장하게 했다.
- CI에서 batch 실행과 리포트 생성을 검증하게 했다.

### 왜 필요한가

시나리오가 늘어나면 하나씩 실행하는 방식은 유지보수가 어렵다.

Batch 실행은 “전체 장비 시뮬레이션 검증 세트”를 한 번에 돌리는 기능이다. 이 기능이 있으면 포트폴리오에서 자동 검증 루프를 설명하기 쉬워진다.

### 소프트웨어 아키텍처 설명

```text
Command Line batch
    ↓
EquipmentTwin.Cli
    ↓
ResolveScenarioPaths()
    ↓
ExecuteScenario() repeated
    ↓
BuildMarkdownReport()
```

중요한 점은 Core 로직을 바꾸지 않았다는 것이다. Batch는 CLI 계층에서 ScenarioRunner를 반복 호출하는 구조다.

### 유지보수 포인트

- Batch 인자 처리: `src/EquipmentTwin.Cli/Program.cs`
- 리포트 생성: `BuildMarkdownReport()`
- CI batch 실행: `.github/workflows/ci.yml`
- 생성 리포트 위치: `artifacts/scenario-report.md`

### 막힌 점과 해결 방법

- 현재 막힌 점 없음.
- `artifacts/`는 이미 `.gitignore`에 포함되어 있어 생성된 리포트가 Git에 섞이지 않는다.

### 보류한 판단

- JSON/HTML 리포트 export는 아직 만들지 않았다.
- GitHub Actions artifact 업로드는 아직 넣지 않았다.
- 시나리오 필터링/tag 기능은 아직 넣지 않았다.

### 확인 방법

```powershell
dotnet run --project src\EquipmentTwin.Cli -- batch scenarios --default-timeouts --report artifacts\scenario-report.md
```

현재 로컬 결과:

- 빌드 성공
- 경고 0개
- 오류 0개
- 테스트 27개 통과
- batch 실행 성공
- Markdown 리포트 생성 성공

## 초보자 설명 템플릿

## 2026-06-25 추가 이해 요약: 알람/복구 시나리오

### 오늘 한 일

- PR #6을 CI 성공 확인 후 main에 병합했다.
- 새 브랜치 `goal/009-alarm-recovery-scenarios`를 만들었다.
- 문 열림 알람 시나리오를 추가했다.
- 비상정지 알람 시나리오를 추가했다.
- 문 열림 알람 이후 ClearAlarm 복구 시나리오를 추가했다.
- 테스트를 27개에서 30개로 늘렸다.
- CLI batch 검증 대상이 2개에서 5개 시나리오로 늘었다.

### 왜 필요한가

장비 SW는 정상 공정만 검증하면 부족하다.

현업 장비에서는 문 열림, 비상정지, Timeout 같은 예외 상황에서 위험 출력이 꺼지고 알람 상태로 멈추는지가 더 중요하다. 이번 작업은 이런 흐름을 JSON 파일로 남겨서 매번 같은 방식으로 반복 검증할 수 있게 만든 것이다.

### 소프트웨어 아키텍처 설명

```text
Alarm Scenario JSON
    ↓
ScenarioRunner
    ↓
EquipmentCellController
    ↓
ReadSafetyEvent()
    ↓
EquipmentStateMachine
    ↓
Alarmed 또는 Idle
```

`ScenarioRunner`는 JSON에 적힌 step을 실행한다. `EquipmentCellController`는 `DI_DOOR_CLOSED`, `DI_EMERGENCY_STOP_PRESSED` 같은 safety 입력을 먼저 읽고, 상태머신에 `DoorOpened` 또는 `EmergencyStop` 이벤트를 전달한다.

상태가 `Alarmed`가 되면 출력은 아래처럼 바뀐다.

- `DO_VACUUM_ON`: 꺼짐
- `DO_STAGE_MOVE_REQUESTED`: 꺼짐
- `DO_TOWER_LAMP_RED`: 켜짐
- `DO_BUZZER_ON`: 켜짐

### 유지보수 포인트

- 알람 상태 전이: `src/EquipmentTwin.Core/EquipmentStateMachine.cs`
- Safety 입력 우선순위: `src/EquipmentTwin.Core/EquipmentCellController.cs`
- IO 이름과 방향: `src/EquipmentTwin.Core/Io/EquipmentIoMap.cs`
- 알람 시나리오 파일: `scenarios/`
- 테스트: `tests/EquipmentTwin.Core.Tests/Program.cs`

### 막힌 점과 해결 방법

- 현재 막힌 점 없음.

### 보류한 판단

- 알람 코드 체계는 아직 없다.
- 알람 이력/발생 시간 리포트는 아직 단순하다.
- 작업자 확인, Reset 조건, Door close 확인 같은 실제 복구 절차는 아직 단순화되어 있다.
- 현재 `ClearAlarm`은 MVP용 단순 복구 명령이다.

### 확인 방법

```powershell
dotnet run --project tests\EquipmentTwin.Core.Tests --no-restore --configuration Release
dotnet run --project src\EquipmentTwin.Cli --no-restore --configuration Release -- batch scenarios --default-timeouts --report artifacts\scenario-report.md
```

현재 로컬 결과:

- Release 빌드 성공
- 경고 0개
- 오류 0개
- 테스트 30개 통과
- batch 시나리오 5개 통과

### 내가 이해해야 할 개념

알람 시나리오는 “실제 장비가 안전하다”를 증명하는 것이 아니다.

지금 증명하는 것은 소프트웨어 모델 안에서 safety 입력이 정상 공정 입력보다 먼저 처리되고, 알람 상태에서 위험 출력이 꺼지며, 복구 명령 후 Idle로 돌아온다는 것이다.

## 2026-06-26 이해 요약: Core 검증 정리

### 오늘 한 일

- `docs/core-validation.md`를 추가했다.
- 현재 Core가 검증하는 것과 검증하지 않는 것을 정리했다.
- 시나리오 5개의 의미를 표로 정리했다.
- 면접에서 말할 수 있는 설명 문장을 추가했다.

### 왜 필요한가

포트폴리오에서는 “코드를 만들었다”보다 “무엇을 어떤 기준으로 검증했다”가 더 중요하다.

특히 장비 SW는 실제 장비 안전성과 연결될 수 있으므로, 검증하지 않은 것을 검증했다고 말하면 안 된다.

### 내가 이해해야 할 개념

현재 검증은 소프트웨어 모델 검증이다.

실제 장비 안전성, 실제 PLC 통신, 실제 모션, 실제 카메라 검사는 아직 검증하지 않았다. 대신 상태 전이, 가상 IO, Timeout, 알람/복구 시나리오가 Core 안에서 반복 가능하게 동작하는지를 확인한다.

### 면접용 한 문장

실제 장비가 없는 상황에서 장비 SW의 상태 전이, 가상 IO, Timeout, 알람/복구 시나리오를 C# Core로 분리해 자동 검증했습니다.

### 확인 방법

```powershell
dotnet run --project tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj --no-restore --configuration Release
dotnet run --project src\EquipmentTwin.Cli\EquipmentTwin.Cli.csproj --no-restore --configuration Release -- batch scenarios --default-timeouts --report artifacts\scenario-report.md
```

현재 로컬 결과:

- Release 빌드 성공
- 경고 0개
- 오류 0개
- 테스트 30개 통과
- batch 시나리오 5개 통과

### 아직 모르는 것

- 알람 코드와 복구 조건을 어떤 형태로 모델링할지
- Unity 시뮬레이터를 언제 붙일지
- 실제 PLC 어댑터를 어느 수준까지 일반화할지

작업이 끝날 때마다 아래 형식으로 정리한다.

## 2026-06-30 이해 요약: Motion Axis와 커스텀 장비 목표

### 오늘 한 일

- PR #11을 병합했다.
- Goal 014로 `MotionAxis` 모델을 시작했다.
- Servo On, Home, Move, InPosition, Timeout, Alarm 흐름을 테스트로 검증했다.
- 테스트가 49개로 늘었다.

### 왜 중요한가

사용자가 원하는 장기 목표는 사용자가 원하는 장비/제품/공정/트러블을 선택해서 실제처럼 제조/검사 시뮬레이션하는 것이다.

이 목표를 달성하려면 장비를 하나의 거대한 코드로 만들면 안 된다. Loader, Stage, Robot, Inspector 같은 모듈로 쪼개야 한다.

그중 가장 먼저 필요한 공통 모듈이 모션 축이다.

### 네가 이해해야 할 개념

모션 축은 장비의 움직이는 부품 하나다.

```text
Servo Off
    ↓
Servo On
    ↓
Home
    ↓
Move
    ↓
InPosition
```

문제가 생기면 알람으로 간다.

```text
Move 중 Timeout
    ↓
MoveTimeout
    ↓
Alarmed
```

### 유지보수 포인트

- 모션 축 동작: `src/EquipmentTwin.Core/Motion/MotionAxis.cs`
- 모션 축 상태: `src/EquipmentTwin.Core/Motion/MotionAxisState.cs`
- 모션 알람 코드: `src/EquipmentTwin.Core/Motion/MotionAxisAlarmCode.cs`
- 테스트: `tests/EquipmentTwin.Core.Tests/Program.cs`

### 아직 모르는 것

- 모션을 장비 공정 상태와 어떻게 연결할지
- Scenario JSON에 어떤 action 이름으로 노출할지
- Unity에서 축 상태를 어떤 UI/3D 오브젝트로 보여줄지

## 2026-06-28 이해 요약: CLI 리포트 알람/복구 조건 표시

### 오늘 한 일

- CLI Markdown batch 리포트의 Summary 표에 `Active Alarm`과 `Clear Condition` 컬럼을 추가했다.
- Details 섹션에도 활성 알람과 ClearAlarm 조건을 표시하게 했다.
- 성공한 시나리오에도 `Errors`가 표시되던 기존 리포트 오류를 수정했다.

### 왜 필요한가

PASS/FAIL만 있으면 “장비가 왜 Alarmed로 끝났는지”와 “지금 ClearAlarm을 눌러도 되는지”가 바로 보이지 않는다.

제조 장비 SW에서는 알람 상태 자체보다 원인 코드와 복구 가능 조건이 중요하다. 이번 변경은 장비가 없어도 시나리오 리포트만 보고 알람 원인과 복구 조건을 리뷰할 수 있게 만든 것이다.

### 네가 이해해야 할 개념

CLI 리포트는 장비 로직을 새로 판단하지 않는다.

이미 `ScenarioRunner`가 실행한 결과에서 `StateMachine.LastAlarm`을 읽고, `EquipmentCellController.CheckAlarmRecoveryCondition()`을 읽기 전용으로 호출해서 표시한다.

즉, 실제 동작 판단은 Core에 남아 있고 CLI는 보고서 표현만 담당한다.

```text
ScenarioRunner 실행 완료
    ↓
ScenarioCliRun
    ↓
BuildMarkdownReport()
    ↓
DescribeActiveAlarm()
DescribeClearCondition()
```

### 유지보수 포인트

- 리포트 형식: `src/EquipmentTwin.Cli/Program.cs`
- 활성 알람 표시: `DescribeActiveAlarm()`
- ClearAlarm 조건 표시: `DescribeClearCondition()`
- 성공/실패 오류 목록: `ScenarioCliRun.Errors`
- 생성 리포트 샘플: `artifacts/scenario-report.md`

### 아직 모르는 것

- 리포트를 HTML/JSON으로도 export할지
- 알람 조치 문구를 별도 테이블로 관리할지
- 시나리오 tag/filter 기능을 먼저 넣을지

## 2026-06-28 이해 요약: 알람 복구 조건

### 오늘 한 일

- `ClearAlarm` 전에 복구 조건을 확인하게 했다.
- 문 열림 알람은 문이 닫혀야 해제되게 했다.
- 비상정지 알람은 비상정지 입력이 해제되어야 해제되게 했다.
- 테스트를 41개로 늘렸다.
- batch 시나리오를 7개로 늘렸다.

### 왜 필요한가

알람 원인이 남아 있는데 알람이 해제되면 실제 장비 관점에서는 위험하다.

이번 작업은 “알람 해제 = 원인 제거 후 가능”이라는 장비 SW의 기본 감각을 코드에 반영한 것이다.

### 내가 이해해야 할 개념

상태머신은 알람 상태를 관리한다.

하지만 복구 조건은 센서 입력을 봐야 하므로 `EquipmentCellController`가 판단한다.

### 아직 모르는 것

- Timeout 알람은 어떤 조건에서 해제해야 하는지
- 작업자 확인 버튼을 별도 이벤트로 둘지
- 알람 조치 문구를 코드에 둘지 문서/설정에 둘지

## 2026-06-27 이해 요약: Visual Studio build/debug

### 오늘 한 일

- Visual Studio에서 build/debug 가능한지 확인했다.
- `docs/visual-studio.md`를 추가했다.
- CLI와 테스트 프로젝트에 launch profile을 추가했다.

### 왜 필요한가

터미널 명령만으로 작업하면 코드 흐름을 눈으로 따라가기 어렵다.

Visual Studio에서 breakpoint를 걸면 상태머신, IO, Timeout, 알람 코드가 어떤 순서로 바뀌는지 직접 볼 수 있다.

### 내가 이해해야 할 개념

Visual Studio에서 중요한 것은 시작 프로젝트다.

- `EquipmentTwin.Cli`: 시나리오 JSON을 실행하며 디버그한다.
- `EquipmentTwin.Core.Tests`: 테스트 전체를 실행하며 디버그한다.
- `EquipmentTwin.Core`: 라이브러리라 단독 실행은 안 하고, CLI나 테스트를 통해 들어간다.

### 아직 모르는 것

- Test Explorer를 쓸 만큼 테스트 프로젝트를 xUnit/NUnit으로 바꿀지
- Unity 프로젝트가 생긴 뒤 Visual Studio와 Unity 디버그를 어떻게 연결할지

## 2026-06-26 이해 요약: 알람 코드 체계

### 오늘 한 일

- `AlarmCode`와 `AlarmInfo`를 추가했다.
- 문 열림, 비상정지, Timeout 알람에 코드를 부여했다.
- 상태 전이 결과와 이력에서 알람 코드를 확인할 수 있게 했다.
- 테스트를 30개에서 34개로 늘렸다.

### 왜 필요한가

실제 장비에서는 알람 문구보다 알람 코드가 더 안정적인 기준이 된다.

문구는 화면이나 언어에 따라 바뀔 수 있지만, 코드는 로그, UI, 복구 절차에서 같은 의미로 추적된다.

### 내가 이해해야 할 개념

알람 코드는 고장 원인을 분류하는 식별자다.

현재 MVP에서는 아래처럼 단순하게 시작했다.

- `DoorOpened = 1001`
- `EmergencyStop = 1002`
- `StateTimeout = 1003`

### 아직 모르는 것

- 실제 장비처럼 알람 레벨을 나눌지
- 작업자 조치 문구를 어디에 둘지
- ClearAlarm 전에 어떤 복구 조건을 요구할지

### 오늘 한 일

-

### 왜 필요한가

-

### 제조 장비 SW와 어떤 관련이 있는가

-

### 내가 이해해야 할 개념

-

### 확인 방법

-

### 아직 모르는 것

-

## 2026-07-01 이해 요약: Motion Scenario Actions

### 오늘 한 일

- PR #12를 병합했다.
- `MotionAxis`를 Scenario JSON action으로 실행할 수 있게 했다.
- `motion-axis-normal.json`과 `motion-axis-timeout.json`을 추가했다.
- CLI batch 리포트에 `Motion Axes` 컬럼을 추가했다.
- 테스트를 51개로 늘렸고, batch 시나리오가 9개로 늘었다.

### 왜 필요한가

모션 축 모델이 C# 테스트 안에만 있으면 포트폴리오에서 “내부 구현은 했다”고 말할 수는 있지만, 사용자가 장비 동작을 선택해서 돌리는 프로그램이라고 말하기는 어렵다.

이번 변경은 모션을 JSON 시나리오로 노출했다. 그래서 나중에 Unity UI, 장비 템플릿, 자동화 루프가 같은 시나리오 파일을 사용할 수 있다.

### 내가 이해해야 할 개념

`ScenarioStepAction`은 JSON에서 사용할 수 있는 명령 이름 목록이다.

예:

```json
{
  "name": "Start X axis move",
  "action": "StartMotionMove",
  "axis": "X",
  "targetPosition": 25,
  "durationMilliseconds": 2000,
  "expectMotionState": "Moving"
}
```

이 step은 `X`축을 25 위치로 2초 동안 이동시키기 시작하고, 실행 직후 상태가 `Moving`인지 확인한다.

### 소프트웨어 아키텍처 설명

```text
Scenario JSON
    ↓
ScenarioStep validation
    ↓
ScenarioRunner
    ↓
MotionAxis
    ↓
Motion expectation
    ↓
CLI report
```

중요한 점은 CLI가 모션 동작을 계산하지 않는다는 것이다.

CLI는 `ScenarioRunner.MotionAxes`를 읽어서 사람이 보기 좋은 리포트를 만든다. 실제 동작 판단은 Core에 남아 있다.

### 유지보수 포인트

- 새 action 이름 추가: `src/EquipmentTwin.Core/Scenarios/ScenarioStepAction.cs`
- JSON 필드 검증: `src/EquipmentTwin.Core/Scenarios/ScenarioStep.cs`
- 실제 실행: `src/EquipmentTwin.Core/Scenarios/ScenarioRunner.cs`
- 리포트 표시: `src/EquipmentTwin.Cli/Program.cs`
- 샘플 시나리오: `scenarios/motion-axis-normal.json`, `scenarios/motion-axis-timeout.json`

### 아직 모르는 것

- 축 알람을 장비 전체 알람과 어떤 기준으로 연결할지
- 장비 템플릿에서 축, IO, 검사기를 어떤 JSON 구조로 묶을지
- Unity에서 X축 상태를 어떤 3D 오브젝트 움직임으로 표시할지

## 2026-07-01 이해 요약: Equipment Template / Product Recipe

### 오늘 한 일

- PR #13을 병합했다.
- `EquipmentTemplate` 모델을 추가했다.
- `MotionAxisTemplate` 모델을 추가했다.
- `ProductRecipe` 모델을 추가했다.
- `InspectionMode`를 추가했다.
- `templates/vision-inspection-cell.json` 샘플을 추가했다.
- 테스트가 56개로 늘었다.

### 왜 필요한가

사용자가 원하는 프로그램은 단순히 축 하나를 움직이는 프로그램이 아니다.

사용자가 장비와 제품을 선택하면 그 장비에 맞는 축, 위치, 검사 방식이 따라와야 한다.

그래서 장비를 코드가 아니라 데이터로 정의하는 단계가 필요하다.

### 내가 이해해야 할 개념

Template와 Recipe는 다르다.

- Template: 장비가 어떤 부품을 갖는지 정의한다.
- Recipe: 특정 제품을 처리할 때 목표 위치나 검사 방식을 정의한다.

예:

```text
vision-inspection-cell
    ├─ X axis
    ├─ Z axis
    ├─ default-panel recipe: X=25, Z=5
    └─ tall-part recipe: X=40, Z=12
```

### 소프트웨어 아키텍처 설명

```text
Template JSON
    ↓
EquipmentTemplate
    ↓
MotionAxisTemplate + ProductRecipe
    ↓
CreateMotionAxes(clock)
```

아직 template가 공정을 자동 실행하지는 않는다.

현재는 “장비 구성 데이터가 올바른가”를 검증하는 단계다.

### 유지보수 포인트

- 템플릿 구조: `src/EquipmentTwin.Core/Templates/EquipmentTemplate.cs`
- 축 정의: `src/EquipmentTwin.Core/Templates/MotionAxisTemplate.cs`
- 제품 recipe: `src/EquipmentTwin.Core/Templates/ProductRecipe.cs`
- 검사 모드: `src/EquipmentTwin.Core/Templates/InspectionMode.cs`
- 샘플 JSON: `templates/vision-inspection-cell.json`

### 아직 모르는 것

- template/recipe를 ScenarioRunner 실행 계획으로 어떻게 바꿀지
- IO 정의를 template 안에 언제 넣을지
- Fault 조건을 recipe와 별도로 둘지, template 옵션으로 둘지
- Unity에서 이 template를 어떻게 시각화할지

## 2026-07-01 이해 요약: Template Runner

### 오늘 한 일

- PR #14를 병합했다.
- `TemplateRunner`를 추가했다.
- 선택한 장비 템플릿과 제품 recipe를 실제 모션 실행으로 바꿨다.
- 테스트가 60개로 늘었다.

### 왜 필요한가

Template와 Recipe는 설계 데이터다.

하지만 설계 데이터만 있으면 아직 장비가 움직이지 않는다.

Template Runner는 그 데이터를 읽어서 실제 가상 축 명령을 실행한다.

### 내가 이해해야 할 개념

```text
Template = 장비 설계도
Recipe = 제품 조건
Runner = 설계도와 조건을 읽고 실행하는 코드
```

예를 들어 `default-panel` recipe는 X=25, Z=5를 목표 위치로 갖는다.

Template Runner는 X축과 Z축을 만들고, Servo On, Home, Move를 실행해서 그 위치까지 보낸다.

### 소프트웨어 아키텍처 설명

```text
EquipmentTemplate
    ↓
ProductRecipe
    ↓
TemplateRunner
    ↓
MotionAxis
    ↓
TemplateRunResult
```

### 유지보수 포인트

- 실행 순서: `src/EquipmentTwin.Core/Templates/TemplateRunner.cs`
- 실행 옵션: `src/EquipmentTwin.Core/Templates/TemplateRunnerOptions.cs`
- 실행 결과: `src/EquipmentTwin.Core/Templates/TemplateRunResult.cs`
- 명령 로그: `src/EquipmentTwin.Core/Templates/TemplateMotionCommandLog.cs`

### 아직 모르는 것

- IO를 Template Runner에 어떻게 붙일지
- 검사 결과를 recipe와 어떻게 연결할지
- Fault 조건을 언제 실행 중에 주입할지
- Template Runner와 ScenarioRunner를 합칠지, 계속 분리할지

## 2026-07-01 이해 요약: Fault Model

### 오늘 한 일

- PR #15를 병합했다.
- Fault Model을 추가했다.
- `x-axis-move-timeout`, `z-axis-servo-alarm` 트러블 조건을 template에 추가했다.
- Template Runner가 fault를 실행 중 주입하게 했다.
- 테스트가 66개로 늘었다.

### 왜 필요한가

정상 동작만 되는 시뮬레이터는 제조 장비 프로젝트로는 부족하다.

현업 장비 SW에서는 정상 공정보다 트러블 상황에서 어떻게 멈추고, 어떤 알람을 남기는지가 중요하다.

### 내가 이해해야 할 개념

Fault Scenario는 사용자가 선택하는 트러블 조건이다.

예:

```text
x-axis-move-timeout
    → X축 move 중 Timeout 발생

z-axis-servo-alarm
    → Z축 move 중 Servo Alarm 발생
```

### 소프트웨어 아키텍처 설명

```text
FaultScenario
    ↓
TemplateRunner
    ↓
MotionAxis
    ↓
Alarmed
```

### 유지보수 포인트

- fault 종류: `src/EquipmentTwin.Core/Templates/FaultKind.cs`
- fault 정의/검증: `src/EquipmentTwin.Core/Templates/FaultScenario.cs`
- fault 실행: `src/EquipmentTwin.Core/Templates/TemplateRunner.cs`
- 샘플 fault: `templates/vision-inspection-cell.json`

### 아직 모르는 것

- 검사 NG/PASS를 어떻게 데이터로 표현할지
- IO fault와 safety fault를 template fault로 통합할지
- fault severity와 operator action을 어디에 둘지

## 2026-07-01 이해 요약: Inspection Result Model

### 오늘 한 일

- PR #16을 병합했다.
- Inspection Result Model을 추가했다.
- `default-panel`은 PASS 검사 결과를 갖게 했다.
- `tall-part`는 `HEIGHT_OVER_LIMIT` Fail 검사 결과를 갖게 했다.
- 테스트가 68개로 늘었다.

### 왜 필요한가

장비가 움직이는 것과 제품이 양품인지 불량인지는 다르다.

현업에서는 장비가 정상으로 움직였는데도 제품 검사는 NG가 될 수 있다.

그래서 장비 실행 결과와 제품 검사 결과를 코드에서 분리했다.

### 내가 이해해야 할 개념

```text
TemplateRunResult.Success
    → 장비가 알람 없이 동작했는가

TemplateRunResult.ProductPassed
    → 제품 검사가 PASS인가
```

예를 들어 `tall-part`는 장비가 정상으로 움직여도 높이 초과로 Fail이 될 수 있다.

### 소프트웨어 아키텍처 설명

```text
ProductRecipe
    ↓
InspectionResultSpec
    ↓
TemplateRunner
    ↓
InspectionResult
    ↓
TemplateRunResult
```

### 유지보수 포인트

- 검사 결과 종류: `src/EquipmentTwin.Core/Templates/InspectionOutcome.cs`
- recipe 검사 결과 정의: `src/EquipmentTwin.Core/Templates/InspectionResultSpec.cs`
- 실행 후 검사 결과: `src/EquipmentTwin.Core/Templates/InspectionResult.cs`
- 검사 결과 생성 시점: `src/EquipmentTwin.Core/Templates/TemplateRunner.cs`
- 실행 결과에서 PASS/FAIL 확인: `src/EquipmentTwin.Core/Templates/TemplateRunResult.cs`

### 아직 모르는 것

- 같은 recipe에서 여러 검사 scenario를 어떻게 선택할지
- 실제 dataset camera를 어떤 인터페이스로 붙일지
- Unity camera 결과를 어떤 시점에 Core로 넘길지

## 2026-07-01 이해 요약: Template Runner CLI

### 오늘 한 일

- PR #17을 병합했다.
- Template Runner CLI를 추가했다.
- `template run templates\vision-inspection-cell.json default-panel`로 제품 PASS 케이스를 실행할 수 있게 했다.
- `template run templates\vision-inspection-cell.json tall-part`로 제품 FAIL 케이스를 실행할 수 있게 했다.
- `--fault x-axis-move-timeout`으로 fault 주입 출력도 확인했다.

### 왜 필요한가

Core 안에 있는 기능은 테스트에서는 검증되지만 사용자가 직접 보기 어렵다.

CLI는 Unity UI를 만들기 전 단계의 조작 패널이다.

### 내가 이해해야 할 개념

```text
Execution = 장비 실행 성공/실패
Product   = 제품 검사 PASS/FAIL
```

`tall-part`는 `Execution: PASS`이지만 `Product: FAIL`이다.

이 차이가 제조 장비 SW에서 중요하다.

### 소프트웨어 아키텍처 설명

```text
CLI argument
    ↓
CliOptions.Parse()
    ↓
RunTemplate()
    ↓
TemplateRunner.RunRecipe()
    ↓
PrintTemplateResult()
```

### 유지보수 포인트

- CLI 파싱: `src/EquipmentTwin.Cli/Program.cs`
- template 실행: `RunTemplate()`
- 출력 포맷: `PrintTemplateResult()`
- Visual Studio 실행 프로필: `src/EquipmentTwin.Cli/Properties/launchSettings.json`
- CI 검증: `.github/workflows/ci.yml`

### 아직 모르는 것

- template 실행 결과를 Markdown으로 저장할지
- 여러 template/recipe를 batch로 실행할지
- fault 케이스를 CI에서 예상 실패로 검증할지

## 이해 체크 질문

작업이 끝난 뒤 아래 질문에 답할 수 있어야 한다.

1. 이 변경은 장비의 어떤 부분과 연결되는가?
2. 실제 장비가 있다면 어떤 신호나 센서와 연결될 가능성이 있는가?
3. 이 코드는 시뮬레이션용인가, 실제 장비에도 가져갈 수 있는 구조인가?
4. 테스트는 무엇을 증명하고, 무엇을 증명하지 못하는가?
5. 면접에서 이 작업을 한 문장으로 어떻게 설명할 수 있는가?
