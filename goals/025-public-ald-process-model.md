# Goal 025: Public Molybdenum ALD Process Model

## 목표

공개 자료에서 말할 수 있는 범위만 사용해서 molybdenum ALD metallization 장비의 공정 제어 골격을 만든다.

이 Goal은 실제 Halo HX 또는 ALTUS Halo 장비를 복제하는 작업이 아니다. 내부 장비 구조, 실제 recipe, 실제 alarm code, 실제 test procedure는 사용하지 않는다.

## 공개 자료 기준으로 사용할 수 있는 내용

- ALTUS Halo는 molybdenum ALD metallization 장비로 공개 소개되어 있다.
- 공개 자료에서는 advanced memory/logic metallization, void-free/low-resistance molybdenum deposition, quad-station module architecture 같은 표현이 나온다.
- ALD는 일반적으로 precursor dose, purge, reactant dose, purge cycle을 반복하는 방식으로 설명할 수 있다.

## 구현 범위

- `MolyAldRecipe`
  - station count
  - cycle count
  - target thickness
  - chamber pressure
  - wafer temperature
  - dose/purge timing
  - synthetic reactant gas flow
  - generic fault scenario
- `MolyAldRunner`
  - LoadWafer
  - PumpDown
  - StabilizeTemperature
  - DoseMetalPrecursor
  - PurgeAfterPrecursor
  - DoseReactant
  - PurgeAfterReactant
  - PostPurge
  - TransferOut
  - Complete
- Unity replay용 step log
  - pressure
  - temperature
  - precursor valve
  - reactant valve
  - purge valve
  - estimated film thickness
- CLI
  - `process run <process-recipe.json>`
  - `--fault <fault-name>`
  - `--report <report.md>`
- CI
  - public moly ALD process CLI 정상 실행

## 이번 Goal에서 일부러 하지 않은 것

- 실제 Lam 장비 recipe 재현
- 실제 Halo HX 내부 구조명 사용
- 실제 precursor/reactant 화학종 사용
- 실제 process spec 주장
- 실제 plasma/thermal/flow physics 계산
- Unity scene 구현

## 검증 기준

- Release build 성공
- Core console tests 통과
- 정상 ALD recipe load 통과
- 정상 ALD process complete 통과
- valve timeline 기록 검증
- pumpdown fault injection 검증
- invalid recipe validation 검증
- CLI process normal run 성공
- CLI process fault run이 exit code 1로 실패하는지 확인
- Markdown report 생성 확인
- GitHub Actions CI 통과

## 면접에서 말할 수 있는 설명

> 공개 자료로 설명 가능한 molybdenum ALD metallization 공정 흐름을 C# Core 모델로 만들었습니다. 실제 장비 recipe나 내부 사양은 사용하지 않았고, synthetic recipe로 Load, PumpDown, Temperature Stabilization, ALD Dose/Purge Cycle, PostPurge, TransferOut을 실행합니다. 각 step은 Unity에서 replay할 수 있도록 pressure, temperature, valve state, film thickness log를 남깁니다.

## 다음 단계

1. Unity가 읽기 쉬운 JSON timeline export를 추가한다.
2. Unity에서 chamber, wafer, valve, film thickness를 timeline에 맞춰 시각화한다.
3. quad-station UI를 단순화해서 4개 station 상태가 동시에 보이게 만든다.
4. fault 발생 시 chamber 색상/알람 패널/step stop을 표시한다.
