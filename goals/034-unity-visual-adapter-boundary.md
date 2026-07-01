# Goal 034: Unity Visual Adapter Boundary

## 목표

CAD/Blender 모델이 생겼을 때 공정 로직을 바꾸지 않고 Unity 외형만 교체할 수 있도록 visual adapter 경계를 만든다.

## 배경

Goal 033까지는 `MolyAldPrimitiveVisualizer`가 timeline step을 직접 읽고 primitive object를 갱신했다.

이 구조는 빠르게 screenshot을 만들기에는 좋지만, imported model을 붙일 때 같은 공정 해석 코드가 여러 renderer에 중복될 위험이 있다.

Goal 034에서는 `timeline step`을 직접 renderer에 연결하지 않고, 중간에 `MolyAldVisualState`를 둔다.

## 완료 조건

- [x] `MolyAldVisualState`를 추가한다.
- [x] `MolyAldVisualStateMapper`를 추가한다.
- [x] `MolyAldPrimitiveVisualizer`가 `MolyAldVisualState`를 받아 표시하도록 refactor한다.
- [x] imported CAD/Blender model용 `MolyAldImportedModelVisualBinding`을 추가한다.
- [x] Unity smoke test에서 visual-state mapper를 검증한다.
- [x] model swap boundary 문서를 추가한다.
- [x] CI file-level check에 새 Unity 경계 파일을 추가한다.

## 설계

```text
MolyAldTimelineDocumentDto
    -> MolyAldProcessPlayer.CurrentStep
    -> MolyAldVisualStateMapper
    -> MolyAldVisualState
    -> MolyAldPrimitiveVisualizer
    -> MolyAldImportedModelVisualBinding
```

`MolyAldVisualState`는 renderer가 알아야 할 표시용 상태만 담는다.

- valve open/close
- vacuum/pressure ratio
- wafer temperature ratio
- film thickness ratio
- fault 여부
- label text

## 사용자에게 설명할 핵심

```text
이번 작업은 모델을 예쁘게 만드는 작업이 아니라,
나중에 예쁜 모델이 생겼을 때 공정 로직을 건드리지 않고 교체할 수 있게 경계를 만든 작업이다.
```

## 제외 범위

- 실제 CAD/Blender asset import
- FBX/glTF material 정리
- model pivot/scale 보정
- 실제 장비 CAD 재현

## 확인 방법

```powershell
.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot
```

Unity smoke test가 통과하면 `MolyAldVisualStateMapper`와 기존 primitive visual replay 경로가 compile/run 된 것이다.
