# Goal 027: Unity Process Player Skeleton

## 목표

Unity에서 `moly-ald-timeline.json`을 읽고 현재 ALD 공정 상태를 화면에 표시하는 최소 player skeleton을 만든다.

이번 목표는 고급 3D chamber를 만드는 것이 아니라, Unity가 C# Core에서 생성한 timeline data contract를 읽고 재생할 수 있음을 보이는 것이다.

## 구현 범위

- Unity project skeleton folder: `unity/EquipmentTwin.Unity`
- Unity runtime assembly definition
- Timeline DTO
- Timeline loader
- Process player
- Minimal `OnGUI` HUD
- StreamingAssets sample timeline
- Unity project README
- CI file-existence validation

## 추가한 Unity runtime scripts

```text
MolyAldTimelineData.cs
    JSON schema field classes

MolyAldTimelineLoader.cs
    TextAsset or StreamingAssets JSON loading

MolyAldProcessPlayer.cs
    timeline step playback state

MolyAldProcessHud.cs
    minimal debug UI
```

## 데이터 흐름

```text
EquipmentTwin.Cli
    -> --timeline artifacts/moly-ald-timeline.json
    -> Unity TextAsset or StreamingAssets
    -> MolyAldTimelineLoader
    -> MolyAldProcessPlayer
    -> MolyAldProcessHud
```

## 이번 Goal에서 하지 않는 것

- polished 3D model
- chamber mesh
- valve mesh animation
- Unity build automation
- real equipment visual claim

## 검증 기준

- .NET Release build 성공
- Core console tests 통과
- process CLI가 timeline JSON 생성
- Unity skeleton files가 CI에서 존재 확인
- Unity Editor에서 열 수 있는 최소 폴더 구조 제공

## 다음 단계

`Goal 028: Unity Chamber/Wafer/Valve Visual`

현재 `OnGUI` HUD를 simple 3D primitives로 확장한다.
