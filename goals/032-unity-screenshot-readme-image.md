# Goal 032: Unity Screenshot and README Demo Image

## 목표

Unity Hub 라이선스 활성화 후 실제 Unity batch screenshot을 생성하고, README에 대표 데모 이미지로 연결한다.

## 배경

Goal 030에서 screenshot capture 명령은 만들었지만 Unity Hub 라이선스가 없어 실제 PNG 생성은 막혀 있었다.

Goal 032에서는 실제 Unity Editor가 프로젝트를 compile하고, smoke test를 통과하고, screenshot PNG를 생성하는지 확인한다.

## 완료 조건

- [x] Unity Hub에서 Personal license가 활성화된다.
- [x] `.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot`가 실행된다.
- [x] Unity compile error를 수정한다.
- [x] `EQUIPMENT_TWIN_UNITY_SMOKE_TEST_PASS` marker를 확인한다.
- [x] `EQUIPMENT_TWIN_UNITY_SCREENSHOT_SAVED` marker를 확인한다.
- [x] `artifacts/unity-demo/moly-ald-demo.png`가 생성된다.
- [x] 대표 이미지를 `docs/demo/moly-ald-demo.png`로 복사한다.
- [x] README에서 대표 이미지를 보여준다.

## 이번에 발견한 문제

### 1. Unity asmdef 모듈 참조 문제

Unity 6000 batch compile에서 `JsonUtility`, `GUI`, `GUILayout` 참조가 실패했다.

조치:

- `JsonUtility` 의존을 제거하고 stable timeline schema 전용 parser로 교체했다.
- IMGUI 기반 `OnGUI()` HUD를 제거하고 runtime summary/log 컴포넌트로 바꿨다.
- 화면용 label은 `MolyAldPrimitiveVisualizer`의 `TextMesh`가 담당한다.

### 2. `-nographics` screenshot crash

Smoke test는 `-nographics`로 가능하지만, screenshot capture는 `Camera.Render()`가 필요하다.

조치:

- `-CaptureScreenshot` 모드에서는 `-nographics`를 빼도록 `scripts/Invoke-UnitySmokeTest.ps1`를 수정했다.

### 3. Editor batch mode에서 Play Mode lifecycle이 돌지 않음

Editor screenshot에서는 `Start()`/`LateUpdate()`에만 의존하면 label이 기본값으로 남았다.

조치:

- screenshot capture 전에 `MolyAldProcessPlayer.LoadTimeline()`을 명시 호출한다.
- 대표 step으로 `DoseReactant`를 선택한다.
- `MolyAldPrimitiveVisualizer.RefreshVisuals()`를 추가해 즉시 화면 상태를 반영한다.

## 검증 명령

```powershell
.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot
```

생성 파일:

```text
artifacts/unity-demo/moly-ald-demo.png
docs/demo/moly-ald-demo.png
```

## 사용자에게 설명할 핵심

```text
이제 Unity 코드가 실제 Editor에서 compile되고,
sample timeline을 읽고,
primitive 3D 장면에 공정 상태를 반영하고,
PNG screenshot으로 저장되는 것까지 확인했다.
```

주의:

- 아직 실제 CAD 모델은 아니다.
- 실제 장비 구조를 복제한 것도 아니다.
- 공개/합성 ALD timeline을 3D primitive로 replay하는 단계다.
