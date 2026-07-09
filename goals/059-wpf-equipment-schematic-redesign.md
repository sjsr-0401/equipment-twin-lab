# 목표 059: WPF 왼쪽 장비 schematic 고도화

작성일: 2026-07-10

## 목표

왼쪽 장비 화면이 단순한 큰 chamber 박스처럼 보이던 문제를 개선하고, 장비 모듈이 연결된 schematic처럼 보이게 만든다.

## 완료 기준

- Load Port, Gas Box, Process Chamber, Exhaust/Pump module이 구획되어 보인다.
- Gas line, transfer line, exhaust line의 연결감이 보인다.
- 알람 발생 시 chamber 전체를 빨갛게 칠하지 않고, flow/outline 중심으로 fault 상태를 표현한다.
- 기존 공정 step, valve on/off, flow text binding은 유지한다.
- WPF build가 통과한다.
- 전체 solution build와 Core tests가 통과한다.

## 변경 내용

- `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml`
  - 왼쪽 schematic layout을 장비 모듈형 구조로 재배치했다.
  - `LOAD PORT`, `GAS BOX / DELIVERY MANIFOLD`, `PROCESS CHAMBER`, `EXHAUST MODULE` 구획을 추가했다.
  - 배관/연결선 역할의 `Rectangle` 요소를 추가했다.
  - chamber 내부에 showerhead, wafer/film, susceptor heater 구조를 더 명확히 배치했다.
- `src/EquipmentTwin.Hmi.Wpf/ViewModels/OperatorConsoleViewModel.cs`
  - 알람 시 `ChamberBrush`가 chamber 전체를 진한 빨강으로 칠하지 않도록 중립 색상으로 조정했다.

## 설계 판단

이번 작업은 실제 vendor CAD를 흉내내는 작업이 아니다.

포트폴리오 목적에 맞게, 공개 자료 기반의 synthetic ALD HMI로서 다음을 보여주는 데 집중했다.

- 장비가 여러 모듈로 구성되어 있다는 점
- 가스 공급, 챔버, 배기/펌프가 연결된 시스템이라는 점
- 알람은 화면 전체를 칠하는 것이 아니라 문제 흐름과 상태를 강조해야 한다는 점

## 검증

통과:

```powershell
dotnet build .\src\EquipmentTwin.Hmi.Wpf\EquipmentTwin.Hmi.Wpf.csproj -c Release
dotnet build .\EquipmentTwinLab.sln -c Release
dotnet run --project .\tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj
```

결과:

- WPF Release build: warning 0, error 0
- Solution Release build: warning 0, error 0
- Core tests: 전체 통과

## 다음 작업

```text
목표 060: 알람/리포트/서버 전송 흐름을 stepper UI로 정리
```

이유:

- 왼쪽 장비 schematic의 1차 구조는 개선됐다.
- 다음으로는 오른쪽 `알람/리포트` 탭에서 사용자가 현재 어느 단계까지 진행했는지 더 명확히 보여줘야 한다.
