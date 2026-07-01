# Unity Demo Recording Checklist

이 문서는 `Equipment Twin Lab`을 3분 이내 포트폴리오 영상으로 녹화할 때 쓰는 체크리스트다.

목표는 “멋진 Unity 화면”만 보여주는 것이 아니라, 장비 SW 엔지니어 관점에서 Core/CLI/Unity가 왜 분리되어 있는지 설명하는 것이다.

## 녹화 전 준비

PowerShell을 저장소 root에서 연다.

```powershell
cd "C:\Users\admin\Claude\Projects\취업 준비\active\equipment-twin-lab"
```

아래 명령으로 공정 report와 Unity timeline을 먼저 만든다.

```powershell
dotnet run --project src\EquipmentTwin.Cli -- process run processes\public-moly-ald-metallization.json --report artifacts\moly-ald-process-report.md --timeline artifacts\moly-ald-timeline.json
```

Unity screenshot도 최신 상태로 갱신한다.

```powershell
.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot
```

녹화 전에 열어둘 파일:

- `README.md`
- `docs/portfolio-demo-package.md`
- `artifacts/moly-ald-process-report.md`
- `artifacts/moly-ald-timeline.json`
- `docs/demo/moly-ald-demo.png`

## 3분 데모 흐름

### 0:00-0:30 — 문제 정의

말할 내용:

```text
장비 SW는 실제 장비 없이 테스트하기 어렵습니다.
그래서 공정 로직은 C#/.NET Core에 두고,
Unity는 그 결과를 재생하는 시각화 계층으로 분리했습니다.
```

보여줄 화면:

- README 첫 화면
- 대표 Unity screenshot

### 0:30-1:10 — Core/CLI가 source of truth임을 보여주기

실행 또는 이미 생성된 report를 보여준다.

```powershell
dotnet run --project src\EquipmentTwin.Cli -- process run processes\public-moly-ald-metallization.json --report artifacts\moly-ald-process-report.md --timeline artifacts\moly-ald-timeline.json
```

말할 내용:

```text
Unity가 공정을 계산하지 않습니다.
Core가 공정 step, pressure, temperature, valve, thickness를 계산하고
timeline JSON으로 저장합니다.
```

### 1:10-1:50 — Fault handling 보여주기

실행:

```powershell
dotnet run --project src\EquipmentTwin.Cli -- process run processes\public-moly-ald-metallization.json --fault pumpdown-timeout
```

말할 내용:

```text
정상 cycle만 보여주는 것이 아니라,
pumpdown timeout 같은 fault가 들어왔을 때
공정이 실패 상태로 멈추는 흐름도 검증합니다.
```

### 1:50-2:30 — Unity replay 보여주기

보여줄 것:

- `docs/demo/moly-ald-demo.png`
- 가능하면 Unity Editor에서 demo scene 또는 screenshot 결과

말할 내용:

```text
Unity는 timeline의 현재 step을 읽어서 chamber, wafer, film,
vacuum gauge, valve, gas line 상태로 표시합니다.
3D 모델이 나중에 생겨도 Core 공정 로직은 유지하고 visual adapter만 교체하는 구조입니다.
```

### 2:30-3:00 — 정직한 한계와 다음 단계

말할 내용:

```text
이 데모는 실제 Lam/ALTUS/Halo 내부 sequence를 복제한 것이 아닙니다.
공개 ALD 개념과 합성 값을 이용해 장비 SW 구조를 설명하는 프로젝트입니다.
다음 단계는 더 좋은 3D model 교체 지점과 짧은 영상 데모를 완성하는 것입니다.
```

## 녹화 품질 체크

- 화면 배율을 너무 크게 하지 않는다. 터미널, README, screenshot이 한 화면에서 읽혀야 한다.
- 명령어는 천천히 실행한다. 결과가 너무 빨리 지나가면 report 파일을 열어 보여준다.
- Unity screenshot은 `docs/demo/moly-ald-demo.png`를 사용한다.
- 실제 회사 장비명, 실제 recipe, 실제 alarm code처럼 들리는 표현은 피한다.
- “실제 장비를 그대로 구현했다”가 아니라 “공개/합성 공정으로 장비 SW 구조를 재현했다”고 말한다.

## 녹화 후 확인

- 영상 길이: 3분 이내
- README 첫 화면이 보이는가?
- CLI 정상 실행 결과가 보이는가?
- fault 실행 결과가 보이는가?
- Unity screenshot이 보이는가?
- 실제 vendor 내부 정보처럼 들리는 표현이 없는가?
