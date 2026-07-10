# Goal 065 — ALD Timeline 기반 Wafer Transfer 상태 시각화

## 목표

기존 MotionAxis를 사용하지 않고 Core의 ALD timeline step에 따라 왼쪽 WPF schematic에서 Wafer 위치와 transfer gate 상태를 보여준다.

## 완료 기준

- `LoadWafer`에서 Gate OPEN과 Chamber 방향 이송 Wafer를 표시한다.
- PumpDown부터 PostPurge까지 Gate CLOSED와 Chamber 내부 Wafer를 표시한다.
- `TransferOut`에서 Gate OPEN과 Load Port 방향 이송 Wafer를 표시한다.
- `Complete`에서 Gate CLOSED와 Load Port 복귀 Wafer를 표시한다.
- 공정 중 알람이면 Wafer가 Chamber에 Hold되고 Gate가 닫힌 상태를 표시한다.
- Wafer 이송 경로와 Exhaust/Pump 배기 경로를 혼동하지 않는다.
- 기존 MotionAxis를 참조하지 않는다.
- WPF Release build, 전체 solution Release build, Core tests, timeline 상태별 검증이 통과한다.

## 구현 결정

- 연속 위치나 속도를 계산하지 않고 현재 timeline step을 네 가지 화면 상태로 변환한다.
- Wafer 이동은 Load Port와 Process Chamber 사이에서만 표현한다.
- Gate OPEN은 파란색, CLOSED는 중립색과 텍스트로 표현한다.
- 알람 중 Gate CLOSED 표시는 실제 안전 인증 로직이 아니라 fault timeline이 TransferOut까지 진행하지 못한 결과다.

## 검증

- `dotnet build .\src\EquipmentTwin.Hmi.Wpf\EquipmentTwin.Hmi.Wpf.csproj -c Release`
- `dotnet build .\EquipmentTwinLab.sln -c Release`
- `dotnet run --project .\tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj -c Release`
- Git에서 제외된 .NET 8 검증 프로그램으로 Load/PumpDown/TransferOut/Complete/Alarm/한글 상태 확인
- WPF 구현 파일의 `MotionAxis` 참조가 없는지 검색
- `git diff --check`
