# Goal 041: Canvas Operator Panel Implementation

## 목표

Goal 040에서 정한 UI/UX 기준을 실제 Unity 구현으로 반영한다.

오른쪽 operator panel과 하단 process timeline을 3D `TextMesh`가 아니라 Unity Canvas 기반 UI로 만든다.

## 왜 필요한가

기존 화면은 장비 형태와 조작 패널 방향은 맞았지만, UI 글씨가 3D 공간에 놓여 있어 전문 HMI처럼 보이기 어려웠다.

이번 Goal의 핵심은 UI layer를 분리하는 것이다.

```text
Unity 3D Scene = equipment body
Unity Canvas   = operator panel, telemetry, alarm, timeline
```

## 구현한 것

- `MolyAldOperatorCanvas` 추가
  - Screen Space Camera Canvas 생성
  - 오른쪽 operator panel 생성
  - Start / Stop / Fault / Reset 버튼 표시
  - Recipe/current step card 표시
  - Live telemetry card 표시
  - Alarm card 표시
  - 하단 process timeline 표시
- `MolyAldDemoBootstrap`에서 Canvas 컴포넌트 자동 생성
- Unity smoke test에서 Canvas 생성 여부 검증
- Unity UI package `com.unity.ugui` 활성화
- 기존 `MolyAldPrimitiveVisualizer`는 3D 장비 라벨 중심으로 축소
- 새 screenshot을 `docs/demo/moly-ald-demo.png`에 반영

## 설계 원칙

Unity Canvas도 공정을 계산하지 않는다.

```text
MolyAldProcessPlayer.CurrentStep
    -> MolyAldVisualStateMapper
    -> MolyAldOperatorCanvas
    -> Canvas operator panel
```

공정 로직, fault 판단, telemetry 값은 기존 Core/CLI timeline에서 온다.

## 검증

- `.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot -ScreenshotPath artifacts\unity-demo\moly-ald-demo-goal041.png`
- `.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot -ScreenshotPath artifacts\unity-demo\moly-ald-demo.png`
- `dotnet build EquipmentTwinLab.sln --no-restore --configuration Release`
- `dotnet run --project tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj --no-restore --configuration Release`
- `git diff --check`

검증 결과:

- Unity smoke test/screenshot capture 통과
- Release build 통과
- Core console test 81개 통과
- whitespace check 통과

## 남은 한계

- Canvas 버튼은 아직 실제 click handler와 연결되지 않았다.
- Fault selector도 아직 UI mock 수준이다.
- 다음 단계에서 버튼 interaction과 fault scenario selector를 연결해야 한다.
