# Goal 070: WPF 장비 공정 동작 시각화

## 목표

- 정적인 장비 구성도에 실제 공정 상태와 연결된 움직임을 추가한다.
- 장비가 단순한 그림이 아니라 현재 Step에 반응하는 HMI로 보이게 한다.
- Core 공정 로직과 WPF 표현 계층의 분리를 유지한다.

## 완료 기준

- Gas valve가 활성화되면 chamber 방향 유동이 움직인다.
- Vacuum 공정 중에는 배기 경로와 Pump rotor가 움직인다.
- Heater 활성 Step에서는 heater glow가 동작한다.
- Wafer Load/Transfer Out Step에서 wafer와 Gate 동작이 보인다.
- Alarm 중에는 정상 공정 애니메이션이 멈추고 기존 진단 색상만 유지된다.
- 애니메이션 없이도 색상과 텍스트만으로 상태를 구분할 수 있다.
- 한국어/영어 대표 화면 캡처, 전체 build, Core tests, WPF smoke test가 통과한다.

## 구현 범위

- `OperatorConsoleViewModel`에 공정 상태에서 계산한 animation 의미 속성을 추가한다.
- `MainWindow.xaml`의 `DataTrigger`와 `Storyboard`로 화면 움직임을 구현한다.
- 별도 animation engine, timer, worker thread는 만들지 않는다.
- Core recipe, timeline, alarm 판정은 수정하지 않는다.

## 검증 명령

```powershell
.\scripts\Capture-WpfDemoScreenshots.ps1 -Language ko
.\scripts\Capture-WpfDemoScreenshots.ps1 -Language en -SkipBuild
dotnet build .\EquipmentTwinLab.sln -c Release
dotnet run --project .\tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj -c Release --no-build
```

## 다음 작업

- `Goal 071: 알람 작업지시서 한국어 문장 품질 정리`
