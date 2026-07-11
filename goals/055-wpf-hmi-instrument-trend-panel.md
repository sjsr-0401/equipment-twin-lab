# 목표 055: WPF HMI 계측값 Trend Panel

작성일: 2026-07-04
브랜치: 현재 작업 브랜치

## 목표

WPF HMI에 작은 trend panel을 추가해서, 현재 공정 run 동안 pressure, wafer temperature, film thickness가 어떻게 변했는지 operator가 볼 수 있게 한다.

현재 HMI의 약점:

```text
현재값은 보이지만, 값이 어떻게 변해왔는지는 한눈에 보기 어렵다.
```

개선 목표:

```text
Pressure / Temp / Film 현재값
         +
최근 변화 흐름 trend
```

## 범위

첫 버전은 단순한 WPF 직접 drawing 또는 기본 control로 구현한다.

단순 구현이 명확히 부족해지기 전에는 LiveCharts2 같은 chart library를 추가하지 않는다.

## 완료 기준

- WPF가 아래 계측값의 최근 trend를 표시한다.
  - chamber pressure
  - wafer temperature
  - film thickness
- Trend data는 기존 process timeline / snapshot data에서 가져온다.
- Trend panel이 기존 alarm guide와 server outbox layout을 깨지 않는다.
- 전체 solution build가 통과한다.
- Core tests가 통과한다.
- WPF project build가 통과한다.
- 작업 로그에 process result에서 UI까지 trend data가 어떻게 흐르는지 설명한다.

## 구현 내용

- 한 줄의 trend를 표현하는 WPF 전용 view model인 `InstrumentTrendRowViewModel`을 추가했다.
- `OperatorConsoleViewModel`에 `InstrumentTrends`를 추가했다.
- 현재 timeline에서 현재 step까지의 구간을 기준으로 trend row 3개를 만들었다.
  - `Pressure`
  - `Temp`
  - `Film`
- 기존 `PROCESS INSTRUMENTS` 카드 안에 작은 `RECENT TREND` 영역을 추가했다.
- Sparkline은 WPF `Polyline`으로 그렸다.
- 외부 chart package는 추가하지 않았다.
- `EquipmentTwin.Core`는 변경하지 않았다.

## 검증

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

## 보류한 것

- 이번 변경은 WPF UI layout 변경이므로 실제 화면 육안 확인이 아직 필요하다.
- 이번 goal에서는 screenshot을 캡처하지 않았다.
- Chart library 도입 여부는 결정하지 않았다. 첫 trend panel은 단순 WPF `Polyline` 버전으로 충분하다고 판단했다.

## 아키텍처 경계

Trend 시각화는 WPF에 속한다.

Core가 WPF chart type에 의존하면 안 된다. Core는 process step data를 제공하고, WPF가 그 데이터를 어떻게 그릴지 결정한다.

```text
Core process result
→ WPF ViewModel trend points
→ WPF visual trend panel
```

## 디자인 메모

첫 버전은 시각적 복잡함보다 읽기 쉬움을 우선한다.

피해야 할 것:

- 불필요한 chart animation
- 아직 필요하지 않은 chart library
- 너무 많은 grid line
- 색상만으로 의미를 전달하는 방식

선호하는 방향:

- 계측값 1개당 작은 trend row 1개
- 현재값 label 표시
- min/max 또는 정상범위 힌트는 읽기 쉬울 때만 표시
- 정상 trend 장식보다 alarm/fault 상태가 더 눈에 잘 띄어야 함
