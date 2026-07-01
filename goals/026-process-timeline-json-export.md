# Goal 026: Process Timeline JSON Export

## 목표

Unity가 Markdown report를 파싱하지 않고도 ALD 공정 결과를 바로 재생할 수 있도록 구조화된 JSON timeline export를 추가한다.

## 구현 범위

- `MolyAldTimelineDocument`
- `MolyAldTimelineStep`
- `MolyAldTimelineValves`
- CLI `process run ... --timeline <timeline.json>`
- 정상/fault timeline 생성
- Core tests
- CI process command에 timeline export 포함
- Visual Studio launch profile에 timeline export 포함

## JSON schema 의도

Unity에서 아래 값을 그대로 읽어 chamber/wafer/valve UI를 움직일 수 있게 한다.

```text
schemaVersion
recipeName
success
finalStep
faultScenarioName
stationCount
cycleCount
targetThicknessAngstrom
estimatedThicknessAngstrom
steps[]
  index
  step
  cycle
  startedAtUtc
  completedAtUtc
  durationMilliseconds
  chamberPressureMtorr
  waferTemperatureC
  valves
    metalPrecursor
    reactant
    purge
  estimatedThicknessAngstrom
```

## 이번 Goal에서 발견한 blocker

PR #23 병합 직후 `EquipmentStateMachine.cs`에 `previous`가 `prev0ious`로 바뀐 오타가 있었다.

이 오타는 `Reject()` 본문이 여전히 `previous`를 참조하기 때문에 빌드를 깨뜨리는 blocker였다.

Goal 026 브랜치에서 원래 이름인 `previous`로 복구했다.

## 검증 기준

- Release build 성공
- Core console tests 통과
- process normal CLI가 report와 timeline을 생성
- process fault CLI가 exit code 1로 실패하면서 fault timeline을 생성
- 생성된 timeline JSON의 schema/step/valve 구조 확인
- GitHub Actions CI 통과

## 다음 단계

`Goal 027: Unity Process Player Skeleton`

Unity 프로젝트에서 이 JSON을 읽고, 아직 고급 3D 모델 없이도 chamber 상태, valve 상태, wafer thickness를 화면에 표시한다.
