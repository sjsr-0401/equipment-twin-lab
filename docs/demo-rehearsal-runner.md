# Demo Rehearsal Runner

이 문서는 3분 포트폴리오 영상을 찍기 전에 실행하는 리허설 절차다.

핵심 목표는 아래를 한 번에 확인하는 것이다.

```text
빌드 가능 → 테스트 통과 → 정상 공정 report 생성 → fault demo 확인 → Unity screenshot 준비
```

## 실행 명령

저장소 root에서 실행한다.

```powershell
.\scripts\Invoke-PortfolioDemoRehearsal.ps1
```

Unity screenshot을 건너뛰고 Core/CLI 리허설만 하려면:

```powershell
.\scripts\Invoke-PortfolioDemoRehearsal.ps1 -SkipUnity
```

## 생성되는 파일

기본 출력 위치:

```text
artifacts/demo-rehearsal/
```

주요 파일:

| 파일 | 용도 |
|---|---|
| `recording-rehearsal.md` | 녹화 전 최종 체크 리포트 |
| `moly-ald-process-report.md` | 정상 공정 설명용 report |
| `moly-ald-timeline.json` | Unity replay 입력 데이터 |
| `moly-ald-demo.png` | 리허설용 Unity screenshot |

Unity screenshot은 검증된 기본 Unity capture 경로에서 먼저 생성한 뒤 리허설 폴더로 복사한다.
이 방식은 Windows 한글 경로가 Unity command-line custom argument에서 깨질 수 있는 위험을 피하기 위한 것이다.

## 리허설에서 확인하는 것

1. Release build가 되는가?
2. Core console test 80개가 통과하는가?
3. 정상 public moly ALD process가 `Complete`로 끝나는가?
4. `pumpdown-timeout` fault가 의도적으로 실패하는가?
5. Unity screenshot이 생성되는가?

## 중요한 포인트

`pumpdown-timeout` fault 명령은 exit code `1`이 정상이다.

이 데모에서는 “고장이 들어오면 공정이 실패 상태로 멈춘다”를 보여줘야 하기 때문이다.

즉 아래 결과는 실패가 아니라 의도한 fault demo다.

```text
Execution: FAIL
Final:     Alarmed
Fault:     pumpdown-timeout
```

## 실제 녹화 순서

리허설이 통과하면 아래 순서로 녹화한다.

1. `README.md` 첫 화면과 대표 Unity screenshot을 보여준다.
2. 정상 공정 report를 보여준다.
3. pumpdown timeout fault를 보여준다.
4. Unity screenshot 또는 Unity Editor scene을 보여준다.
5. 실제 장비 복제가 아니라 공개/합성 process replay라는 한계를 설명한다.

## 이 스크립트가 대신하지 않는 것

- 화면 녹화 버튼을 누르는 일
- 말하는 연습
- Unity Editor에서 camera path를 직접 조정하는 일
- 실제 CAD/Blender asset 품질 검증
- Unity command-line custom screenshot path 검증

이 스크립트는 녹화 전 재료를 준비하고, 데모가 깨지지 않는지만 검증한다.
