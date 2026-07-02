# ALD Fault Matrix Report 설명서

## 한 줄 요약

`process batch`는 공개/합성 ALD recipe의 정상 케이스와 모든 fault 케이스를 자동으로 실행해서, 장비 SW가 기대한 대로 완료 또는 알람 정지하는지 확인하는 명령이다.

## 왜 이 기능을 만들었나

장비 SW는 “정상 공정이 끝난다”만으로는 부족하다.

실제 장비에서는 다음 상황도 중요하다.

- 압력이 목표치에 도달하지 않으면 공정을 계속하면 안 된다.
- 온도가 안정화되지 않으면 wafer에 공정을 진행하면 안 된다.
- gas dose나 purge가 timeout이면 다음 step으로 넘어가면 안 된다.
- 실패 위치와 메시지가 로그에 남아야 나중에 원인을 추적할 수 있다.

그래서 이번 기능은 정상 공정과 fault 공정을 한 번에 실행하고, 기대 결과와 실제 결과가 맞는지 표로 남긴다.

## 실행 방법

```powershell
dotnet run --project src\EquipmentTwin.Cli -- process batch processes\public-moly-ald-metallization.json --report artifacts\moly-ald-fault-matrix-report.md
```

생성되는 report:

```text
artifacts/moly-ald-fault-matrix-report.md
```

## 코드 흐름

```text
CLI args
    -> CliOptions.Parse()
    -> CliMode.ProcessBatch
    -> RunProcessBatch()
    -> MolyAldRecipe.FromJson()
    -> MolyAldRunner.Run(recipe)
    -> MolyAldRunner.Run(recipe, faultName)
    -> ProcessBatchRun.ExpectationMet
    -> PrintProcessBatchResult()
    -> WriteProcessBatchMarkdownReport()
```

## 핵심 파일

| File | Role |
|---|---|
| `processes/public-moly-ald-metallization.json` | 공개/합성 ALD recipe와 fault scenario 목록 |
| `src/EquipmentTwin.Core/Processes/MolyAldRunner.cs` | 실제 공정 step을 순서대로 실행하는 core runner |
| `src/EquipmentTwin.Cli/Program.cs` | `process batch` 명령을 파싱하고 report를 생성 |
| `tests/EquipmentTwin.Core.Tests/Program.cs` | 모든 configured fault가 `Alarmed`로 이동하는지 검증 |
| `artifacts/moly-ald-fault-matrix-report.md` | 실행 후 생성되는 matrix report |

## 소프트웨어 설계 관점

이번 작업에서 중요한 선택은 batch 기능이 공정 로직을 복사하지 않는 것이다.

나쁜 구조:

```text
process run 로직
process batch 전용 로직
```

이렇게 나누면 나중에 한쪽만 수정돼서 결과가 달라질 수 있다.

현재 구조:

```text
MolyAldRunner = 공정 실행 source of truth
process run   = MolyAldRunner를 한 번 호출
process batch = MolyAldRunner를 여러 번 호출
```

그래서 유지보수할 때는 공정 순서나 fault 처리 규칙을 `MolyAldRunner` 중심으로 보면 된다.

## 현재 fault matrix

| Case | Expected | Reason |
|---|---|---|
| `normal` | PASS | 모든 ALD cycle을 끝내고 `Complete`가 되어야 한다. |
| `pumpdown-timeout` | FAIL | chamber pressure가 setpoint에 도달하지 못하면 `Alarmed`가 되어야 한다. |
| `temperature-not-stable` | FAIL | wafer temperature가 안정화되지 않으면 `Alarmed`가 되어야 한다. |
| `precursor-dose-timeout` | FAIL | metal precursor dose 중 timeout이면 해당 cycle에서 멈춰야 한다. |
| `purge-timeout` | FAIL | purge 중 timeout이면 다음 dose로 넘어가면 안 된다. |

## 면접에서 정직하게 말할 범위

말해도 되는 것:

- 공개/합성 ALD 공정 모델을 만들었다.
- 정상 시퀀스와 fault 시나리오를 자동 검증한다.
- fault가 발생하면 runner가 실패 step을 기록하고 `Alarmed`로 종료한다.
- CI에서 batch 검증 명령을 실행한다.

과장하면 안 되는 것:

- 실제 ALTUS/Halo/Halo HX 내부 recipe를 구현했다.
- 실제 장비 alarm table을 재현했다.
- 실제 증착 물리 현상을 정밀 시뮬레이션했다.

## 나중에 유지보수할 때 보는 순서

1. 새 fault를 추가하고 싶으면 `processes/public-moly-ald-metallization.json`의 `faultScenarios`에 추가한다.
2. fault kind가 새로 필요하면 Core의 ALD fault enum/model부터 확장한다.
3. 그 fault가 어느 step에서 멈춰야 하는지 `MolyAldRunner`에 구현한다.
4. `tests/EquipmentTwin.Core.Tests/Program.cs`에 기대 동작을 추가한다.
5. `process batch`를 돌려 report가 의도대로 나오는지 확인한다.

