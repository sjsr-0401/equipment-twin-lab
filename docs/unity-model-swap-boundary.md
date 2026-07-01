# Unity Model Swap Boundary

이 문서는 CAD/Blender/FBX/glTF 모델이 생겼을 때 `Equipment Twin Lab`의 Unity 외형을 어떻게 교체할지 정리한다.

핵심 원칙은 아래 한 줄이다.

```text
공정 계산은 Core/CLI가 하고, Unity model은 MolyAldVisualState만 받아서 표시한다.
```

## 현재 구조

```text
MolyAldTimelineDocumentDto
    -> MolyAldProcessPlayer.CurrentStep
    -> MolyAldVisualStateMapper
    -> MolyAldVisualState
    -> MolyAldPrimitiveVisualizer
```

Goal 034에서 새로 분리한 경계는 `MolyAldVisualState`다.

`MolyAldVisualState`는 Unity renderer가 알아야 할 표시용 값만 담는다.

- 현재 step 이름
- pressure/vacuum 비율
- wafer temperature 비율
- film thickness 비율
- metal precursor / reactant / purge valve 상태
- label에 표시할 문자열
- fault 여부

## 왜 이 경계가 필요한가

primitive object와 imported model은 생김새가 다르다.

하지만 둘 다 필요한 정보는 같다.

```text
현재 chamber는 vacuum 상태인가?
wafer는 뜨거운 상태인가?
film은 얼마나 쌓였는가?
어떤 valve/gas line이 열려 있는가?
fault 상태인가?
```

그래서 model마다 공정 JSON을 다시 해석하지 않고, 공통 `MolyAldVisualState`를 받도록 만든다.

## 현재 제공하는 visual adapter

### 1. Primitive visual

파일:

```text
unity/EquipmentTwin.Unity/Assets/EquipmentTwin/Runtime/MolyAldPrimitiveVisualizer.cs
```

역할:

- demo scene을 자동 생성한다.
- chamber/wafer/film/gauge/valve/gas line을 Unity primitive로 만든다.
- `MolyAldVisualState`를 받아 색상, 크기, label을 갱신한다.

이 adapter는 현재 README screenshot을 만드는 기본 adapter다.

### 2. Imported model binding

파일:

```text
unity/EquipmentTwin.Unity/Assets/EquipmentTwin/Runtime/MolyAldImportedModelVisualBinding.cs
```

역할:

- 외부에서 가져온 CAD/Blender model의 renderer/transform을 Inspector에서 연결한다.
- scene을 자동 생성하지 않는다.
- `MolyAldVisualState`를 받아 연결된 model part의 색상, 선택적 film scale, label을 갱신한다.
- imported model의 valve/line scale은 강제로 `Vector3.one`으로 바꾸지 않는다. FBX/glTF 원래 scale을 보존해야 하기 때문이다.

이 adapter는 아직 실제 CAD/Blender asset을 포함하지 않는다. 모델 파일이 생겼을 때 붙이는 경계다.

## 모델이 생겼을 때 교체 순서

1. Unity 프로젝트에 모델을 import한다.

```text
unity/EquipmentTwin.Unity/Assets/Models/<model-name>.fbx
```

2. Scene root를 만든다.

```text
Moly ALD Demo Root
    MolyAldProcessPlayer
    MolyAldImportedModelVisualBinding
    VisualRoot
        Chamber
        Wafer
        Film
        PressureIndicator
        PressureNeedle
        PrecursorValve
        ReactantValve
        PurgeValve
        PrecursorLine
        ReactantLine
        PurgeLine
```

3. `MolyAldImportedModelVisualBinding`에 model part를 연결한다.

| Binding field | 연결할 model part |
|---|---|
| `chamberRenderer` | chamber/body renderer |
| `waferRenderer` | wafer renderer |
| `filmRenderer` | film 표시 renderer |
| `filmScaleRoot` | film 두께/크기 표시용 transform |
| `pressureIndicatorRenderer` | vacuum/pressure 표시 part |
| `pressureNeedle` | gauge needle transform |
| `precursorValveRenderer` | metal precursor valve renderer |
| `reactantValveRenderer` | reactant valve renderer |
| `purgeValveRenderer` | purge valve renderer |
| `precursorLineRenderer` | metal precursor gas line renderer |
| `reactantLineRenderer` | reactant gas line renderer |
| `purgeLineRenderer` | purge gas line renderer |
| `stepLabel` | step label TextMesh |
| `valueLabel` | value label TextMesh |

4. `MolyAldPrimitiveVisualizer`는 끄거나 제거한다.

5. 같은 timeline JSON으로 Play 또는 screenshot capture를 실행한다.

## 유지보수 규칙

- imported model script가 process JSON을 직접 읽게 만들지 않는다.
- model part 이름이 바뀌면 `MolyAldImportedModelVisualBinding`의 Inspector 연결만 고친다.
- Core/CLI process model을 바꾸지 않고 model 외형만 바꾸는 것이 목표다.
- 실제 CAD가 들어와도 실제 vendor 내부 구조를 재현했다고 말하지 않는다.

## 지금 단계의 정직한 한계

- 실제 CAD/Blender asset은 아직 없다.
- imported model binding은 compile 가능한 교체 지점이다.
- 실제 model을 import하면 scale, pivot, material, light 조정은 별도 작업이 필요하다.
- gauge needle처럼 회전하는 part는 pivot 방향을 맞춰야 한다.
- 현재 GitHub Actions는 Unity Editor를 실행하지 않으므로 file-level check만 한다.
