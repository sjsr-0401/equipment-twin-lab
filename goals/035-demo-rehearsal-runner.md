# Goal 035: Demo Rehearsal Runner

## 목표

3분 포트폴리오 영상을 찍기 전에 필요한 명령과 산출물을 한 번에 준비하는 리허설 스크립트를 만든다.

## 배경

Goal 033에서 3분 녹화 체크리스트를 만들었고, Goal 034에서 CAD/Blender model 교체 경계를 만들었다.

이제 실제 녹화 전에는 아래 작업을 매번 수동으로 실행해야 한다.

- build
- test
- 정상 공정 report/timeline 생성
- fault demo 확인
- Unity screenshot 생성
- 녹화 순서 확인

Goal 035는 이 반복을 `scripts/Invoke-PortfolioDemoRehearsal.ps1` 하나로 묶는다.

## 완료 조건

- [x] `scripts/Invoke-PortfolioDemoRehearsal.ps1`를 추가한다.
- [x] 정상 공정 report와 timeline을 `artifacts/demo-rehearsal/`에 생성한다.
- [x] pumpdown timeout fault를 expected failure로 처리한다.
- [x] Unity screenshot capture를 리허설에 포함한다.
- [x] Unity screenshot은 기본 capture path에서 만든 뒤 리허설 폴더로 복사한다.
- [x] `-SkipUnity` 옵션으로 Core/CLI만 리허설할 수 있게 한다.
- [x] 리허설 결과 Markdown report를 생성한다.
- [x] 문서와 CI file-level check를 추가한다.

## 사용자에게 설명할 핵심

```text
이 스크립트는 영상을 대신 찍는 것이 아니라,
영상 찍기 전에 보여줄 결과물이 깨지지 않는지 한 번에 확인하는 리허설 버튼이다.
```

## 검증 명령

```powershell
.\scripts\Invoke-PortfolioDemoRehearsal.ps1
```

Unity 없이 빠르게 확인:

```powershell
.\scripts\Invoke-PortfolioDemoRehearsal.ps1 -SkipUnity
```

## 정직한 한계

- 실제 화면 녹화는 사용자가 직접 해야 한다.
- 이 스크립트는 녹화 품질을 보장하지 않는다.
- Unity Editor가 없는 환경에서는 `-SkipUnity`를 사용해야 한다.
- Windows 한글 경로 문제를 피하기 위해 Unity custom screenshot path를 직접 넘기지 않는다.
