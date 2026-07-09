# 2026-07-04 WPF HMI 계측값 Trend Panel

## 1. 한 일

- WPF HMI의 `PROCESS INSTRUMENTS` 카드에 `RECENT TREND` 영역을 추가했다.
- Pressure, Temp, Film 각각에 대해 현재 step까지의 변화 흐름을 작은 sparkline으로 표시한다.
- 외부 chart library는 추가하지 않았다.

## 2. 바뀐 파일

- `src/EquipmentTwin.Hmi.Wpf/Models/InstrumentTrendRowViewModel.cs`
- `src/EquipmentTwin.Hmi.Wpf/ViewModels/OperatorConsoleViewModel.cs`
- `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml`
- `goals/055-wpf-hmi-instrument-trend-panel.md`
- `logs/2026-07-04-wpf-instrument-trend-panel.md`
- `state/loop-state.md`
- `state/triage.md`

## 3. 검증 결과

통과:

```powershell
dotnet build .\src\EquipmentTwin.Hmi.Wpf\EquipmentTwin.Hmi.Wpf.csproj
dotnet run --project .\tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj
dotnet build .\EquipmentTwinLab.sln
```

WPF startup smoke:

```text
Started     = True
ExitedEarly = False
```

의미:

- WPF XAML binding/compile 문제 없음.
- Core test regression 없음.
- Solution 전체 project reference 문제 없음.
- WPF가 시작 직후 crash 나지는 않음.

## 4. 막힌 점과 해결 방법

- 현재 막힌 점 없음.
- 단, 실제 화면에서 trend panel의 글자 크기와 폭이 적절한지는 사용자가 WPF를 직접 열어 확인해야 한다.

## 5. 보류한 판단

- LiveCharts2 같은 chart library는 추가하지 않았다.
- 이유: 지금 목표는 작은 trend visibility이며, 외부 chart dependency를 넣기에는 아직 이르다.
- WPF screenshot 자동 캡처도 하지 않았다.
- 이유: 이번 목표는 코드 구현과 startup smoke까지이며, 시각 품질은 다음 manual visual QA에서 조정하는 편이 낫다.

## 6. 소프트웨어 아키텍처 설명

Core는 공정 step 데이터를 만든다.

```text
EquipmentTwin.Core
→ MolyAldTimelineDocument.Steps
→ OperatorConsoleViewModel.InstrumentTrends
→ MainWindow.xaml Polyline
```

중요한 점:

- Core는 WPF를 모른다.
- WPF ViewModel이 Core timeline을 UI 표시용 trend row로 바꾼다.
- XAML은 계산하지 않고 이미 만들어진 `TrendPoints`를 그리기만 한다.

## 7. 유지보수할 때 봐야 할 파일

- trend row 데이터 구조: `src/EquipmentTwin.Hmi.Wpf/Models/InstrumentTrendRowViewModel.cs`
- trend row 생성 로직: `src/EquipmentTwin.Hmi.Wpf/ViewModels/OperatorConsoleViewModel.cs`
- trend panel 화면 배치: `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml`

## 8. 사용자가 이해해야 할 개념

- `Polyline`: 여러 점을 선으로 이어 그리는 WPF 도형이다.
- `PointCollection`: Polyline이 그릴 좌표 목록이다.
- `ViewModel`: Core 데이터를 화면에 표시하기 좋은 형태로 바꾸는 계층이다.
- `Sparkline`: 작은 공간에 값의 흐름만 간단히 보여주는 미니 차트다.

## 9. 다음 작업

추천:

```text
목표 056: WPF HMI 수동 화면 검수와 커밋 정리
```

이유:

- 이번 변경은 UI라서 실제 화면 확인이 필요하다.
- 커밋 전 `unity/EquipmentTwin.Unity/ProjectSettings/PackageManagerSettings.asset` 포함 여부도 결정해야 한다.
