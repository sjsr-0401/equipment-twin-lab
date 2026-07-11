# Goal 062 — WPF Stepper/Server/Schematic 시각 QA

## 목표

최근 추가한 장비 schematic, 알람 workflow stepper, Mock Server 상태 카드가 창 크기와 긴 문구 때문에 잘리거나 불필요하게 길어지지 않도록 레이아웃을 정리한다.

## 완료 기준

- WPF가 시작할 때 오퍼레이터 콘솔에 맞게 최대화된다.
- 낮은 창 높이에서도 왼쪽 chamber가 고정 행에 과도하게 눌리지 않는다.
- 알람/리포트 탭에 세로 ScrollViewer가 중첩되지 않는다.
- 긴 workflow detail과 파일 경로는 한 줄로 표시하고 전체 값은 ToolTip으로 확인할 수 있다.
- Mock Server 상태 문구가 확인 버튼 때문에 지나치게 좁아지지 않는다.
- WPF Release build, 전체 solution Release build, Core tests가 통과한다.

## 구현 결정

- 새 ViewModel이나 UI 라이브러리를 추가하지 않고 `MainWindow.xaml`만 수정한다.
- 하단 debug 영역은 고정 220px 대신 최소/최대 범위를 가진 비율 행으로 바꾼다.
- 알람 탭 전체는 바깥 ScrollViewer 하나가 담당한다.
- screenshot 자동 캡처가 실패하면 그 사실을 로그에 남기고, 기존 실제 screenshot과 XAML 구조 분석으로 범위를 제한한다.

## 검증

- `dotnet build .\src\EquipmentTwin.Hmi.Wpf\EquipmentTwin.Hmi.Wpf.csproj -c Release`
- `dotnet build .\EquipmentTwinLab.sln -c Release`
- `dotnet run --project .\tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj -c Release`
- `git diff --check`
