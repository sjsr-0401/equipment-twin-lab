# 목표 058: WPF HMI 오른쪽 패널 탭 구조 적용

작성일: 2026-07-10

## 목표

WPF HMI 오른쪽 패널의 조작, 알람 대응, 작업 로그가 한 화면에 길게 쌓여 있던 구조를 탭 기반 구조로 나눈다.

## 완료 기준

- 오른쪽 패널에 WPF `TabControl`을 적용한다.
- 운전 Overview, 알람/리포트, 작업 로그를 서로 다른 탭으로 분리한다.
- 탭 제목은 한국어/영어 전환에 맞춰 바뀐다.
- 기존 알람, 리포트 저장, 서버 대기열 저장, Mock Server 전송 기능은 유지한다.
- WPF build가 통과한다.
- 전체 solution build와 Core tests가 통과한다.

## 구현 판단

왼쪽 장비 schematic은 계속 고정 표시한다.

이유:

- HMI에서는 장비 상태와 알람 상태가 항상 보여야 한다.
- 전체 화면을 탭으로 바꾸면 사용자가 다른 탭에 있을 때 장비 상태를 놓칠 수 있다.
- 따라서 이번 단계에서는 오른쪽 기능 패널만 탭으로 나눴다.

## 변경 내용

- `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml`
  - `TabControl`, `TabItem` dark theme style 추가
  - 오른쪽 패널을 `운전 Overview`, `알람 / 리포트`, `작업 로그` 탭으로 분리
- `src/EquipmentTwin.Hmi.Wpf/ViewModels/OperatorConsoleViewModel.cs`
  - 탭 제목용 computed property 추가

## 검증

통과:

```powershell
dotnet build .\src\EquipmentTwin.Hmi.Wpf\EquipmentTwin.Hmi.Wpf.csproj
dotnet build .\EquipmentTwinLab.sln
dotnet run --project .\tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj
```

## 다음 작업

```text
목표 059: 왼쪽 장비 schematic을 cabinet/module/gas-line/pump 구조로 개선
```

이번 작업은 화면 정보 구조를 정리한 것이다.
다음 작업에서 왼쪽 장비 화면을 “큰 빨간 박스”가 아니라 실제 장비 모듈처럼 보이게 개선한다.
