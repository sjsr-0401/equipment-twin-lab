# Goal 064 — Fault Code별 WPF Schematic 진단 영역 강조

## 목표

알람이 발생했을 때 왼쪽 장비 schematic 전체가 동일하게 빨개지는 대신, 알람 코드에 해당하는 장비 모듈과 연결 라인만 강조한다.

## 완료 기준

- 정상 상태에서는 `SCHEMATIC FOCUS · NORMAL`을 표시한다.
- `GAS-301`은 Gas Box와 gas delivery line을 강조한다.
- `TMP-201`은 Process Chamber와 heater를 강조한다.
- `VAC-101`은 vacuum path와 Exhaust/Pump module을 강조한다.
- 문제 영역이 아닌 모듈은 중립색을 유지한다.
- 색상과 함께 알람 코드/영역명 텍스트를 표시한다.
- 기존 MotionAxis 코드는 사용하지 않는다.
- WPF Release build, 전체 solution Release build, Core tests, 상태별 brush 검증이 통과한다.

## 구현 결정

- 현재 Core에 존재하는 `GAS-301`, `TMP-201`, `VAC-101`만 지원한다.
- 아직 존재하지 않는 FILM 계열 알람은 UI에 임의로 만들지 않는다.
- Core fault 결과를 바꾸지 않고 WPF ViewModel에서 진단 영역을 계산한다.
- XAML은 계산된 brush와 진단 문구를 표시하는 역할만 담당한다.

## 검증

- `dotnet build .\src\EquipmentTwin.Hmi.Wpf\EquipmentTwin.Hmi.Wpf.csproj -c Release`
- `dotnet build .\EquipmentTwinLab.sln -c Release`
- `dotnet run --project .\tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj -c Release`
- Git에서 제외된 .NET 8 검증 프로그램으로 정상/GAS/TMP/VAC/한글 상태 확인
- `git diff --check`
