# Goal 068 — WPF 한국어 일반 UI 라벨 일관성 정리

## 목표

한국어 모드에서 조작 버튼, 영역 제목, 상태 설명이 실제로 한국어로 전환되게 하고, 장비·공정·소프트웨어 도메인 용어는 무리하게 번역하지 않는다.

## 완료 기준

- 한국어 모드에서 시작, 정지, 다음 Step, Fault 재현, 초기화 버튼이 읽힌다.
- 장비 모듈 구성, 공정 진행도, 현재 Step, 공정 계측값, 알람 우선순위 제목이 한국어로 전환된다.
- 일시정지, 운전 중, 정지 유지와 계측 상태가 한국어로 표시된다.
- 영어 모드는 기존 영어 문구를 유지한다.
- `LOAD PORT`, `PROCESS CHAMBER`, `PUMP`, `Valve`, `Recipe`, `ALD`, 단위, Trace payload는 기술 용어로 유지한다.
- 한국어/영어 5개 대표 Screenshot이 1600×900 및 anchor 4/4 검사를 통과한다.
- 전체 solution build, Core tests, 일반 WPF smoke test가 통과한다.

## 구현 결정

- 별도 다국어 framework나 resource dictionary를 추가하지 않는다.
- 기존 `OperatorConsoleViewModel.L(english, korean)`과 `ToggleLanguageCommand`를 재사용한다.
- XAML의 고정 일반 UI 문자열을 이미 존재하는 ViewModel 속성 binding으로 교체한다.
- 표시 상태의 색상은 번역된 문자열에도 동일하게 유지한다.
- 브랜드, 장비 구성 요소, 공정 약어, 개발자용 trace 데이터는 번역 범위에서 제외한다.

## 검증 명령

```powershell
.\scripts\Capture-WpfDemoScreenshots.ps1 -Language ko
.\scripts\Capture-WpfDemoScreenshots.ps1 -Language en -SkipBuild
dotnet build .\EquipmentTwinLab.sln -c Release
dotnet run --project .\tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj -c Release --no-build
```

## 다음 작업

- `Goal 069: 알람 작업지시서 한국어 문장 품질 정리`
- 체크리스트와 대응 선택지의 기술 용어는 유지하되, 조사와 서술어가 섞인 영어식 문장을 작업자가 빠르게 이해할 수 있는 한국어로 정리한다.
