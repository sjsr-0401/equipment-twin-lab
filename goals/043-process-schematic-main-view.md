# Goal 043: Process Schematic Main View

## 목표

Unity 데모의 메인 시각화를 3D primitive equipment view에서 2D ALD process schematic 중심으로 전환한다.

3D를 포기하는 것이 아니라, 3D를 메인 판독 화면에서 내리고 나중에 cutaway/debug 보조뷰로 사용한다.

## 왜 필요한가

primitive 3D는 빠르게 시작하기에는 좋았지만, 장비답게 보이려면 모델링/스케일/재질/연결/조명이 모두 필요하다.
지금 단계에서 3D 외형을 계속 키우면 포트폴리오의 핵심인 장비 SW 구조보다 아트 품질이 먼저 평가된다.

제조 HMI 관점에서는 연결된 2D schematic이 더 실용적이다.

## 구현한 것

- 왼쪽 메인 화면을 `Process Schematic Main View`로 변경
- 공개/일반 ALD 구성요소를 Canvas로 표현
  - gas delivery
  - precursor/reactant/purge valve
  - vacuum chamber
  - showerhead
  - wafer + film
  - susceptor heater
  - pressure/temperature tap
  - exhaust line
  - gate valve
  - vacuum pump
- 현재 step에 따라 valve/line/showerhead color가 바뀌게 연결
- film thickness ratio에 따라 wafer film fill이 바뀌게 연결
- alarm state에서 chamber/exhaust highlight가 바뀌게 연결
- public reference boundary 문서 추가

## 설계 원칙

```text
MolyAldProcessPlayer.CurrentStep
    -> MolyAldVisualStateMapper
    -> MolyAldVisualState
    -> MolyAldOperatorCanvas
       -> Process schematic
       -> HMI instrument panel
       -> Timeline
```

스키매틱은 공정을 계산하지 않는다.
이미 계산된 visual state를 읽어서 표시만 한다.

## Reference boundary

사용한 것은 공개적으로 설명 가능한 ALD/metallization 개념이다.

복제하지 않은 것:

- 실제 Lam/ALTUS/Halo/Halo HX CAD
- 실제 chamber geometry
- 실제 process recipe
- 실제 station/module layout
- 실제 tool UI

## 검증

- `dotnet build EquipmentTwinLab.sln --no-restore --configuration Release`
- `dotnet run --project tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj --no-restore --configuration Release`
- `.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot -ScreenshotPath artifacts\unity-demo\moly-ald-demo-goal043.png`
- `.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot`
- `git diff --check`
- screenshot 육안 확인

검증 결과:

- Release build 통과
- Core console test 81개 통과
- Unity screenshot capture 통과
- Default Unity screenshot capture 통과
- local CI marker check 통과
- screenshot에서 Reactant valve ON, chamber/showerhead/wafer/exhaust path가 읽힘

## 남은 한계

- valve click interaction은 아직 없다.
- fault selector는 아직 없다.
- schematic은 still-image 중심이고 flow animation은 아직 없다.
- 3D cutaway 보조뷰는 다음 단계다.
