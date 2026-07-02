# Goal 044: Gas Flow Animation and Fault Highlight

## 목표

ALD process schematic을 정적인 그림에서 현재 step에 반응하는 HMI schematic으로 개선한다.

이번 목표는 버튼 interaction이 아니라 flow/fault 시각화다.

## 왜 필요한가

Goal 043에서 2D process schematic은 생겼지만, 화면은 아직 정적인 diagram에 가까웠다.

장비 SW 데모에서는 현재 step에서 무엇이 흐르는지 보여주는 것이 중요하다.

## 구현한 것

- active gas valve에 따라 gas pulse color 변경
  - precursor: amber
  - reactant: cyan
  - purge: green
- gas pulse가 valve path에서 chamber/showerhead 방향으로 이동하도록 구현
- showerhead 아래 gas distribution dots가 active gas color로 pulse
- pump/purge/fault 상태에서 exhaust pulse path를 사용할 수 있게 구현
- fault 상태에서 chamber/exhaust/gate/pump highlight가 blink 가능하도록 구현
- schematic flow text 추가
  - `FLOW: Reactant pulse -> chamber`
- demo screenshot 갱신

## 설계 원칙

```text
MolyAldVisualState
    -> UpdateProcessSchematic()
       -> UpdateGasFlowPulses()
       -> UpdateExhaustFlowPulses()
```

flow animation도 공정을 계산하지 않는다.
현재 step/valve/fault 상태를 읽어서 UI element의 위치와 색만 바꾼다.

## 막혔던 점

- 첫 pulse 크기가 커서 Reactant valve text와 showerhead를 덮었다.
  - 해결: pulse 경로를 valve 아래/showerhead 아래로 낮추고 크기를 줄였다.
- Unity smoke test 1회는 screenshot marker까지 성공했지만 exit code가 1이었다.
  - 원인: Unity 종료 직후 내부 Curl/memory warning으로 보인다.
  - 해결: screenshot과 marker를 확인한 뒤 재실행했고, 재실행은 exit code 0으로 통과했다.

## 검증

- `dotnet build EquipmentTwinLab.sln --no-restore --configuration Release`
- `dotnet run --project tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj --no-restore --configuration Release`
- `.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot -ScreenshotPath artifacts\unity-demo\moly-ald-demo-goal044.png`
- `.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot`
- `git diff --check`
- screenshot 육안 확인

검증 결과:

- Release build 통과
- Core console test 81개 통과
- Unity screenshot capture 통과
- Default Unity screenshot capture 통과
- local CI marker check 통과
- screenshot에서 Reactant pulse와 flow label이 보임

## 남은 한계

- 실제 runtime button interaction은 아직 없다.
- fault selector는 아직 없다.
- gas pulse는 simple Canvas animation이다. 물리 유동 해석이 아니다.

다음 단계:

```text
Goal 045: Canvas Button Interaction and Fault Selector
```
