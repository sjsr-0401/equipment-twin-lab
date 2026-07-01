# Portfolio Demo Package

이 문서는 `Equipment Twin Lab`을 면접·포트폴리오에서 설명할 때 사용할 데모 패키지다.

목표는 “멋진 화면만 만든 프로젝트”가 아니라, 장비 SW 엔지니어가 실제 장비 없이도 공정 sequence, fault handling, replay visualization, validation loop를 어떻게 설계했는지 보여주는 것이다.

## 한 줄 소개

> 실제 장비가 없어도 C#/.NET으로 제조 장비 공정 로직을 검증하고, Unity 3D에서 공개/합성 ALD 공정 timeline을 재생하는 장비 SW 디지털 트윈 프로젝트입니다.

## 3분 데모 흐름

### 0:00-0:30 — 문제 정의

설명:

```text
장비 SW는 실제 장비 없이 테스트하기 어렵습니다.
그래서 Core 로직, CLI 검증, Unity 시각화를 분리해서
장비가 없어도 sequence와 fault를 반복 검증할 수 있는 구조를 만들었습니다.
```

보여줄 것:

- `README.md`
- `docs/architecture.md`
- `processes/public-moly-ald-metallization.json`

### 0:30-1:20 — Core/CLI가 공정의 source of truth임을 보여주기

실행:

```powershell
dotnet run --project src\EquipmentTwin.Cli -- process run processes\public-moly-ald-metallization.json --report artifacts\moly-ald-process-report.md --timeline artifacts\moly-ald-timeline.json
```

설명:

```text
Unity가 공정을 계산하지 않습니다.
Core가 공정 결과와 timeline JSON을 만들고,
Unity는 그 결과를 읽어서 재생합니다.
```

보여줄 결과:

- `artifacts/moly-ald-process-report.md`
- `artifacts/moly-ald-timeline.json`

### 1:20-2:10 — Fault가 들어오면 공정이 어떻게 멈추는지 보여주기

실행:

```powershell
dotnet run --project src\EquipmentTwin.Cli -- process run processes\public-moly-ald-metallization.json --fault pumpdown-timeout
```

설명:

```text
정상 cycle만 있는 데모가 아니라,
장비 SW에서 중요한 timeout/fault stop 흐름을 데이터로 재현합니다.
```

### 2:10-2:40 — Unity replay 구조 설명

Unity 라이선스 활성화 후 실행:

```powershell
.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot
```

예상 산출물:

```text
artifacts/unity-demo/moly-ald-demo.png
```

설명:

```text
Unity는 timeline JSON의 pressure, temperature, valve, thickness 값을
primitive chamber, wafer, gauge, valve indicator, film overlay로 표시합니다.
```

### 2:40-3:00 — 정직한 한계와 다음 단계

설명:

```text
이 프로젝트는 실제 Lam 내부 recipe나 장비 구조를 복제하지 않습니다.
공개 ALD 개념과 합성 값을 사용해 장비 SW 구조를 보여주는 프로젝트입니다.
다음 단계는 Unity 라이선스 활성화 후 실제 screenshot/video를 남기고,
CAD/Blender 모델이 생기면 primitive visual만 교체하는 것입니다.
```

## 현재 데모가 증명하는 것

| 항목 | 증명 수준 | 근거 |
|---|---|---|
| 상태머신 | 구현/테스트 완료 | `EquipmentStateMachine`, console tests |
| 가상 IO | 구현/테스트 완료 | `VirtualIoController`, IO direction tests |
| Timeout/Fault | 구현/테스트 완료 | `ManualClock`, scenario/template fault tests |
| 모션 축 모델 | 구현/테스트 완료 | `MotionAxis`, motion scenario tests |
| Product/Inspection data model | 구현/테스트 완료 | `ProductRecipe`, `InspectionResult`, template CLI |
| 공개/합성 ALD process | 구현/CLI 검증 완료 | `MolyAldRunner`, `process run` |
| Unity timeline data contract | 구현/파일 검증 완료 | `MolyAldTimelineDocument`, sample JSON |
| Unity primitive visual | 실제 Unity Editor batch 검증 완료 | `MolyAldPrimitiveVisualizer`, `docs/demo/moly-ald-demo.png` |
| Unity screenshot capture | 실제 PNG 생성 완료 | `Invoke-UnitySmokeTest.ps1 -CaptureScreenshot` |

## 현재 주장하면 안 되는 것

면접에서 아래 표현은 피한다.

- “실제 ALTUS/Halo 장비를 그대로 구현했다.”
- “실제 Lam recipe나 내부 sequence를 재현했다.”
- “실제 증착 물리를 시뮬레이션했다.”
- “Unity compile/play가 CI에서 자동 검증된다.”
- “실제 카메라 비전 검사를 구현했다.”

대신 아래처럼 말한다.

```text
공개된 ALD/장비 SW 개념을 기반으로 합성 공정 모델을 만들었습니다.
핵심은 실제 장비 복제가 아니라, 장비 SW에서 중요한 sequence, fault, replay, validation boundary를 분리한 구조입니다.
```

## 면접 답변 포인트

### 왜 Unity를 썼나?

Unity는 공정 로직을 계산하기 위한 엔진이 아니라, 사람이 장비 상태를 이해하기 위한 시각화 계층으로 사용한다.

```text
Core/CLI = source of truth
Unity = replay and visual adapter
```

이렇게 나누면 Unity Scene이나 3D 모델을 바꿔도 장비 sequence 테스트가 깨지지 않는다.

### 장비가 없는데 어떻게 테스트했나?

실제 PLC/서보/챔버 대신 software model을 만들었다.

- PLC input/output은 `VirtualIoController`
- 시간은 `ManualClock`
- 모션 축은 `MotionAxis`
- 공정 sequence는 `MolyAldRunner`
- Unity 재생 데이터는 `MolyAldTimelineDocument`

즉, 장비 물리 자체를 완벽히 대신한 것이 아니라, 장비 SW가 판단해야 하는 입력·상태·fault boundary를 테스트 가능하게 만들었다.

### CAD/Blender 모델이 생기면 어떻게 바꾸나?

현재 Unity visual은 primitive object다.

교체 기준:

```text
MolyAldProcessPlayer.CurrentStep
    -> visual adapter
    -> primitive renderer 또는 CAD/Blender imported model renderer
```

데이터 입력은 유지하고 renderer만 바꾸는 구조가 목표다.

### 실제 제조 Process에서 실용적인가?

실용적인 부분:

- recipe validation
- process step log
- timeout/fault injection
- alarm/fault report
- replay timeline
- operator/debug 설명 문서

아직 부족한 부분:

- 실제 PLC 통신
- 실제 장비 IO map
- 실제 recipe parameter
- 실제 sensor feedback
- Unity Editor CI
- 실제 공정 데이터 calibration

## 실제 Unity screenshot 확인

Unity Hub 로그인/라이선스 활성화 후 아래 명령으로 검증했다.

```powershell
.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot
```

성공 조건:

```text
EQUIPMENT_TWIN_UNITY_SMOKE_TEST_PASS
EQUIPMENT_TWIN_UNITY_SCREENSHOT_SAVED
artifacts/unity-demo/moly-ald-demo.png 생성
```

대표 이미지:

```text
docs/demo/moly-ald-demo.png
```

다음 작업:

- 3분 데모 영상 체크리스트 작성
- Unity visual에서 CAD/Blender 모델 교체 지점 설계
- screenshot 품질 개선: camera framing, labels, simple chamber layout
