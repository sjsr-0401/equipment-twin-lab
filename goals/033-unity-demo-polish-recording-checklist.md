# Goal 033: Unity Demo Polish and Recording Checklist

## 목표

Goal 032에서 만든 실제 Unity screenshot을 포트폴리오 첫 화면에 더 적합하게 다듬고, 3분 데모 영상을 찍을 때 따라갈 체크리스트를 만든다.

## 배경

Goal 032는 “Unity가 실제로 compile되고 screenshot을 만들 수 있다”는 것을 증명했다.

하지만 첫 screenshot은 pipeline 검증용에 가까웠다. 이번 Goal은 새 backend 기능을 늘리기보다, 현재 공정 replay가 더 명확하게 보이도록 camera framing, base plate, gas line, title/label을 개선한다.

## 완료 조건

- [x] Unity primitive visual에 장비 base plate를 추가한다.
- [x] metal precursor / reactant / purge gas line을 화면에 표시한다.
- [x] title label을 추가해 screenshot의 맥락을 바로 알 수 있게 한다.
- [x] demo camera framing과 FOV를 조정한다.
- [x] screenshot capture script의 false failure exit-code 처리를 수정한다.
- [x] `.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot`로 실제 PNG를 다시 생성한다.
- [x] 대표 이미지를 `docs/demo/moly-ald-demo.png`로 갱신한다.
- [x] 3분 녹화 체크리스트 문서를 추가한다.
- [x] CI에서 새 문서와 Unity polish 코드가 존재하는지 확인한다.

## 이번 작업의 설계 포인트

```text
MolyAldTimelineStepDto
    -> MolyAldProcessPlayer.CurrentStep
    -> MolyAldPrimitiveVisualizer
    -> chamber / wafer / film / gauge / valve / gas line / label
```

중요한 점은 여전히 Unity가 공정을 계산하지 않는다는 것이다.

Unity는 Core/CLI가 만든 timeline JSON을 읽고, 현재 step의 pressure, temperature, valve, thickness 값을 사람이 볼 수 있게 표시한다.

## 사용자에게 설명할 핵심

```text
이번 작업은 기능 확장이 아니라 데모 전달력 개선이다.
같은 timeline 데이터를 더 읽기 쉬운 3D 화면으로 보여주도록 Unity visual adapter를 다듬었다.
```

## 검증 명령

```powershell
git diff --check
dotnet build EquipmentTwinLab.sln --no-restore --configuration Release
dotnet run --project tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj --no-restore --configuration Release
dotnet run --project src\EquipmentTwin.Cli\EquipmentTwin.Cli.csproj --no-restore --configuration Release -- process run processes\public-moly-ald-metallization.json --report artifacts\moly-ald-process-report.md --timeline artifacts\moly-ald-timeline.json
.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot
```

## 정직한 한계

- 실제 Lam/ALTUS/Halo/Halo HX CAD가 아니다.
- 실제 vendor 내부 sequence가 아니다.
- 실제 증착 물리 시뮬레이션이 아니다.
- 현재 목표는 장비 SW의 sequence/fault/replay 구조를 설명 가능한 형태로 보여주는 것이다.
