# Goal 069 — WPF 장비 Schematic 시각 깊이 고도화

## 목표

왼쪽 장비 영역을 단순한 사각형 배치에서 벗어나, ALD 장비의 주요 구성요소와 연결 관계를 한눈에 이해할 수 있는 장비 단면/P&ID 혼합 화면으로 개선한다.

## 완료 기준

- Gas Box가 Precursor/Reactant/Purge 3채널 MFC와 isolation valve 구조로 읽힌다.
- Process Chamber에서 showerhead, process zone, wafer, susceptor, heater stage, bottom plenum이 구분된다.
- Chamber 내부에 실제 ViewModel pressure와 wafer temperature가 표시된다.
- Load Port가 FOUP slot과 slit valve를 가진 모듈처럼 보인다.
- Exhaust path에 throttle valve와 vacuum pump 형상이 보인다.
- 기존 Valve, Gas flow, Vacuum path, Wafer 위치, Gate, Fault 색상 Binding을 유지한다.
- 가짜 공정값이나 vendor 전용 형상을 추가하지 않는다.
- 한국어/영어 대표 Screenshot과 전체 build/tests/smoke test가 통과한다.

## 구현 결정

- WPF vector shape와 gradient만 사용한다.
- 실제 장비 CAD를 복제하지 않고 공개적인 ALD 구성 개념을 합성한 단면도로 유지한다.
- 재질과 형상은 XAML이 담당하고, 동적 상태는 기존 ViewModel이 담당한다.
- 정보 전달에 필요한 깊이감만 주고, 장식용 애니메이션은 추가하지 않는다.

## 검증 명령

```powershell
.\scripts\Capture-WpfDemoScreenshots.ps1 -Language ko
.\scripts\Capture-WpfDemoScreenshots.ps1 -Language en -SkipBuild
dotnet build .\EquipmentTwinLab.sln -c Release
dotnet run --project .\tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj -c Release --no-build
```

## 다음 작업

- `Goal 070: 알람 작업지시서 한국어 문장 품질 정리`
- 장비 외형 다음으로 작업자가 실제로 읽는 알람 요약, 체크리스트, 대응 선택지의 혼합 문장을 다듬는다.
