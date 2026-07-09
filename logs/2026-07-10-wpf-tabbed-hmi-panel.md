# 2026-07-10 WPF HMI 오른쪽 패널 탭 구조 적용

## 1. 한 일

- 오른쪽 조작 패널에 WPF `TabControl`을 추가했다.
- 기존에 한 화면에 길게 쌓여 있던 조작/알람/리포트/로그 영역을 탭으로 분리했다.
- 탭 제목이 한국어/영어 전환에 맞춰 바뀌도록 ViewModel property를 추가했다.

## 2. 바뀐 파일

- `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml`
- `src/EquipmentTwin.Hmi.Wpf/ViewModels/OperatorConsoleViewModel.cs`
- `goals/058-wpf-tabbed-hmi-panel.md`
- `logs/2026-07-10-wpf-tabbed-hmi-panel.md`
- `state/loop-state.md`
- `state/triage.md`

## 3. 설계 판단

전체 화면을 탭으로 바꾸지 않고 오른쪽 패널만 탭으로 나눴다.

이유:

- 왼쪽 장비 상태와 알람 색상은 항상 보여야 한다.
- 장비 HMI에서 현재 장비 상태가 탭 뒤로 숨으면 안 된다.
- 오른쪽 기능만 탭으로 나누면 공정 상태는 유지하면서 조작/알람/리포트/로그 밀도를 줄일 수 있다.

## 4. 탭 구성

- `운전 Overview`: 조작 버튼, fault selector, 현재 step, 계측값, 알람 상태
- `알람 / 리포트`: 체크리스트, 대응 선택지, 이슈 리포트, 서버 대기열, Mock Server 전송
- `작업 로그`: 오퍼레이터 액션 로그, optional Unity viewer folder 버튼

## 5. 막힌 점 / 보류한 판단

- 왼쪽 장비 schematic은 이번 작업에서 크게 바꾸지 않았다.
- 장비 그림 개선은 XAML 구조 변경량이 커지므로 다음 goal로 분리한다.
- 서버 전송 흐름도 아직 알람/리포트 탭 안에 있으며, 별도 stepper UI는 다음 UI/UX 개선 후보로 둔다.

## 6. 소프트웨어적으로 알아야 할 점

WPF `TabControl`은 여러 화면을 같은 자리에서 전환하는 컨트롤이다.

이번 작업에서는 Core 로직을 전혀 바꾸지 않았다.
즉, 장비 상태 계산, 알람 guide, report 생성, server 전송은 그대로이고 화면 배치만 바뀌었다.

이런 변경은 UI 계층 변경이다.
Core tests가 그대로 통과해야 정상이다.

## 7. 검증

통과:

```powershell
dotnet build .\src\EquipmentTwin.Hmi.Wpf\EquipmentTwin.Hmi.Wpf.csproj
dotnet build .\EquipmentTwinLab.sln
dotnet run --project .\tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj
```

## 8. 다음 작업

추천:

```text
목표 059: 왼쪽 장비 schematic 고도화
```

목표:

- cabinet 외곽 프레임
- load port / transfer / chamber / gas box / exhaust pump 구획
- gas line과 exhaust line 연결감
- fault 발생 시 문제 구간 중심 강조

## 9. 탭 라벨/간격 후속 수정

사용자 화면 확인 결과, 탭 영역 자체가 임시 버튼처럼 보이고 첫 탭의 `운전 Overview` 문구가 한영 혼합으로 어색했다.

수정:

- `운전 Overview` → `공정 운전`
- `알람 / 리포트` → `알람/리포트`
- English mode의 `OVERVIEW` → `OPERATE`
- English mode의 `OPERATOR LOG` → `LOG`
- 탭 높이, 최소 폭, 중앙 정렬을 명시했다.

검증:

```powershell
dotnet build .\src\EquipmentTwin.Hmi.Wpf\EquipmentTwin.Hmi.Wpf.csproj -c Release
dotnet run --project .\tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj
```

결과:

- WPF Release build: warning 0, error 0
- Core tests: 전체 통과

보류 / 주의:

- Debug build는 실행 중인 `EquipmentTwin.Hmi.Wpf.exe`가 output 파일을 잠그고 있어서 실패했다.
- 코드 문제가 아니라 현재 WPF 창이 켜져 있어서 생긴 파일 lock이다.
- Debug build를 다시 확인하려면 실행 중인 WPF 창을 닫고 빌드하면 된다.
