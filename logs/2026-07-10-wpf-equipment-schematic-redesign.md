# 2026-07-10 WPF 왼쪽 장비 schematic 고도화

## 1. 한 일

- 왼쪽 장비 화면을 기존의 큰 chamber 중심 도형에서 장비 모듈형 schematic으로 바꿨다.
- `Load Port`, `Gas Box`, `Process Chamber`, `Exhaust Module`을 화면에 구획했다.
- 배관/연결선을 추가해서 장비가 하나의 시스템처럼 읽히도록 했다.
- 알람 발생 시 chamber 전체가 빨갛게 보이던 표현을 줄였다.

## 2. 바뀐 파일

- `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml`
- `src/EquipmentTwin.Hmi.Wpf/ViewModels/OperatorConsoleViewModel.cs`
- `goals/059-wpf-equipment-schematic-redesign.md`
- `logs/2026-07-10-wpf-equipment-schematic-redesign.md`
- `state/loop-state.md`
- `state/triage.md`

## 3. 핵심 변경

기존 구조:

```text
Load Port | 큰 Vacuum Chamber 박스 | Pump
```

변경 후 구조:

```text
Equipment Module Layout
├─ Load Port / FOUP
├─ Gas Box / Delivery Manifold
├─ Process Chamber
│  ├─ showerhead
│  ├─ wafer + film
│  └─ susceptor heater
└─ Exhaust Module / Pump
```

## 4. 소프트웨어적으로 알아야 할 점

이번 작업은 Core 로직 변경이 아니다.

변경한 것은 WPF view layer다.

- 공정 step 계산은 기존 Core가 한다.
- valve on/off 값도 기존 ViewModel이 제공한다.
- 화면은 그 값을 더 장비답게 배치해서 보여준다.

즉, 장비 동작 로직과 장비 화면 표현을 분리한 구조가 유지된다.

## 5. 알람 표현 방식 변경

이전에는 알람이 생기면 chamber 전체 배경이 빨갛게 변했다.

문제:

- 화면 전체가 장난감처럼 보인다.
- 어떤 구간이 문제인지 알기 어렵다.
- chamber 내부 구조가 빨간 배경에 묻힌다.

변경:

- chamber 배경은 중립 색상으로 유지한다.
- `FlowBrush`를 chamber outline, 일부 line, flow label에 사용한다.
- 알람 시 문제 상태는 빨간 outline/flow text로 보인다.

## 6. 막힌 점 / 보류한 판단

- 실제 CAD/3D 모델링은 하지 않았다.
- 현재는 WPF shape 기반 2D schematic이다.
- 가스 라인별 active animation은 아직 없다.
- fault 종류별로 특정 valve/line만 강조하는 세분화는 다음 후보로 둔다.

## 7. 검증

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

## 8. 다음 작업

추천:

```text
목표 060: 알람/리포트/서버 전송 stepper UI
```

오른쪽 알람/리포트 탭에서 다음 흐름을 명확히 보여주는 작업이다.

```text
알람 확인 → 체크리스트 → 대응 선택 → 리포트 저장 → 서버 대기열 → Mock Server 전송
```
