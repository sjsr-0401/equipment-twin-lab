# Portfolio Demo Narration

이 문서는 `Equipment Twin Lab`을 3분 안에 설명할 때 쓰는 한글 대본이다.

기존 `docs/unity-demo-recording-checklist.md`가 “무엇을 보여줄지”를 정리한다면, 이 문서는 “실제로 뭐라고 말할지”를 고정한다.

## 핵심 원칙

- 공정 로직은 C#/.NET Core가 담당한다.
- CLI는 Core 결과를 사람이 읽는 report와 Unity가 읽는 timeline JSON으로 저장한다.
- Unity는 timeline을 재생하는 시각화 계층이다.
- 실제 Lam/ALTUS/Halo/Halo HX 내부 구조나 recipe를 복제했다고 말하지 않는다.
- 공개 ALD 개념과 합성 값을 이용해 장비 SW 구조를 보여주는 프로젝트라고 말한다.

## 실행해서 큐카드 만들기

저장소 root에서 아래 명령을 실행한다.

```powershell
.\scripts\New-PortfolioDemoCueCards.ps1
```

리허설까지 같이 하고 싶으면 아래 명령이 더 편하다.

```powershell
.\scripts\Invoke-PortfolioDemoRehearsal.ps1
```

리허설 스크립트는 마지막에 큐카드도 함께 만든다.

생성 파일:

```text
artifacts/demo-rehearsal/recording-cue-cards.md
```

## 3분 말하기 흐름

| 시간 | 말할 핵심 | 보여줄 화면 |
|---|---|---|
| 0:00-0:20 | 실제 장비 없이 장비 SW 흐름을 검증하는 디지털 트윈 프로젝트 | `README.md` |
| 0:20-0:50 | Core/CLI/Unity를 분리했다 | README 아키텍처 설명 |
| 0:50-1:25 | 정상 ALD 공정 report와 timeline JSON을 만든다 | 정상 공정 report |
| 1:25-1:55 | pumpdown timeout fault가 들어오면 Alarmed로 멈춘다 | fault 실행 결과 |
| 1:55-2:30 | Unity가 timeline을 읽어 chamber/wafer/valve/gas line으로 보여준다 | Unity screenshot |
| 2:30-3:00 | 실제 장비 복제가 아니라 공개/합성 공정 replay라는 한계를 설명한다 | portfolio demo package |

## 그대로 읽을 수 있는 대본

```text
이 프로젝트는 실제 장비가 없어도 장비 SW의 공정 흐름과 fault handling을 검증하기 위한 디지털 트윈 포트폴리오입니다.

핵심은 공정 로직을 Unity 화면에 넣지 않은 것입니다.
C#/.NET Core가 공정 결과의 기준이고, CLI는 그 결과를 report와 timeline JSON으로 저장합니다.
Unity는 그 timeline을 읽어서 chamber, wafer, valve, gas line 상태로 재생합니다.

정상 공정은 공개된 ALD 개념을 바탕으로 합성 molybdenum ALD sequence로 구성했습니다.
정상 cycle에서는 pumpdown, dose, purge step이 끝까지 진행되고 Complete로 끝납니다.

장비 SW에서 중요한 것은 정상 흐름뿐 아니라 고장 흐름입니다.
예를 들어 pumpdown timeout fault를 넣으면 공정은 의도적으로 Alarmed 상태로 멈춥니다.
이때 exit code 1은 데모 실패가 아니라 expected failure입니다.

Unity 화면은 이 결과 timeline을 사람이 이해하기 쉽게 보여주는 계층입니다.
나중에 CAD나 Blender 모델이 생기면 공정 로직은 유지하고 visual adapter만 교체할 수 있게 분리했습니다.

이 데모는 실제 Lam이나 ALTUS/Halo 내부 recipe를 복제한 것이 아닙니다.
공개 정보와 합성 값을 이용해 장비 SW 엔지니어가 중요하게 보는 sequence, fault, validation boundary를 구현한 프로젝트입니다.
```

## 면접 질문 대응

### 실제 장비 테스트가 없는데 의미가 있나?

실제 장비 성능을 증명하는 프로젝트는 아니다.

대신 실제 장비 없이도 아래를 검증할 수 있게 만든 프로젝트다.

- 상태 전이
- timeout
- fault stop
- report
- replay timeline
- Unity visual boundary

### 왜 Unity 안에서 공정을 계산하지 않았나?

Unity가 공정을 계산하면 화면 코드가 공정 기준이 된다. 그러면 테스트하기 어렵고 나중에 3D 모델을 바꿀 때 공정 로직도 흔들릴 수 있다.

그래서 Core/CLI가 source of truth이고 Unity는 replay 계층으로 제한했다.

### 이게 제조 Process에서 실용적인가?

실용적인 부분은 실제 장비 복제가 아니라 검증 구조다.

- recipe 결과를 파일로 남긴다.
- fault를 주입해 Alarmed 흐름을 검증한다.
- Unity 화면은 Core 결과를 재생하므로 UI와 공정 로직이 분리된다.
- 나중에 실제 데이터나 모델이 생기면 adapter를 바꾸는 방식으로 확장할 수 있다.

## 피해야 할 표현

아래 표현은 과장이다.

- 실제 장비를 그대로 구현했다.
- 실제 Lam/ALTUS/Halo recipe를 구현했다.
- 실제 증착 물리 모델이다.
- Unity 화면으로 실제 장비 안전성을 검증했다.

대신 이렇게 말한다.

```text
공개 ALD 개념과 합성 값을 사용해 장비 SW 구조를 테스트 가능하게 만든 포트폴리오입니다.
```
