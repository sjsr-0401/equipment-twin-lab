# Goal 038: ALD Fault Matrix Report

## 목표

녹화/비주얼 개선은 잠시 보류하고, 개발 쪽 산출물을 강화한다.

`processes/public-moly-ald-metallization.json`에 정의된 정상 공정과 모든 fault scenario를 한 번에 실행해서, 기대 결과와 실제 결과가 맞는지 자동 검증하는 CLI 명령을 추가한다.

## 왜 필요한가

장비 SW에서 중요한 것은 “정상 동작”뿐 아니라 “비정상 상황에서 안전하게 멈추는가”다.

기존 `process run`은 한 번에 하나의 정상/fault 케이스만 실행했다.
이번 Goal의 `process batch`는 recipe에 포함된 fault 목록을 전부 돌려서 regression test처럼 확인한다.

## 사용 명령

```powershell
dotnet run --project src\EquipmentTwin.Cli -- process batch processes\public-moly-ald-metallization.json --report artifacts\moly-ald-fault-matrix-report.md
```

## 기대 결과

| Case | Expected | Meaning |
|---|---|---|
| `normal` | PASS | 정상 ALD 공정은 `Complete`로 끝나야 한다. |
| `pumpdown-timeout` | FAIL | pump-down 이상은 `Alarmed`로 멈춰야 한다. |
| `temperature-not-stable` | FAIL | 온도 안정화 실패는 `Alarmed`로 멈춰야 한다. |
| `precursor-dose-timeout` | FAIL | precursor dose timeout은 해당 cycle에서 멈춰야 한다. |
| `purge-timeout` | FAIL | purge timeout은 해당 purge step에서 멈춰야 한다. |

CLI exit code는 모든 기대 결과가 맞으면 `0`, 하나라도 틀리면 `1`이다.

## 구현 범위

- `process batch` CLI mode
- 정상 run + 모든 configured fault scenario 실행
- 기대 PASS/FAIL과 실제 PASS/FAIL 비교
- console summary 출력
- Markdown fault matrix report 생성
- Core test에 “모든 configured fault가 Alarmed로 이동하는지” 검증 추가
- CI에서 `process batch` 명령 실행

## 하지 않은 것

- 실제 vendor fault catalog 구현
- 실제 장비 alarm code 구현
- 실제 증착 물리 모델 구현
- Unity 화면 개선
- 녹화 파일 생성

## 설계 메모

`process batch`는 공정 로직을 새로 만들지 않는다.
이미 있는 `MolyAldRunner`를 여러 번 호출한다.

```text
process recipe JSON
    -> MolyAldRecipe.FromJson()
    -> normal MolyAldRunner.Run(recipe)
    -> foreach fault: MolyAldRunner.Run(recipe, fault.Name)
    -> ProcessBatchRun.ExpectationMet
    -> console summary + markdown report
```

이 구조가 좋은 이유는 `MolyAldRunner`가 single source of truth로 남기 때문이다.
batch mode가 별도 공정 로직을 복사하면 정상 run과 batch run이 서로 다른 결과를 낼 수 있다.

## 검증

- `dotnet build EquipmentTwinLab.sln --no-restore --configuration Release`
- `dotnet run --project tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj --no-restore --configuration Release`
- `dotnet run --project src\EquipmentTwin.Cli\EquipmentTwin.Cli.csproj --no-restore --configuration Release -- process batch processes\public-moly-ald-metallization.json --report artifacts\moly-ald-fault-matrix-report.md`
- `git diff --check`

