# Goal 067 — 자동 Screenshot 기반 WPF Visual QA 2차

## 목표

Goal 066의 WPF 자동 캡처가 Windows 배율과 렌더링 방식에 따라 일부 영역을 빠뜨리는 문제를 해결하고, 사람이 모든 PNG를 열어보기 전에도 대표 화면의 누락을 자동 검출한다.

## 완료 기준

- 캡처 전용 WPF 창은 현재 모니터의 작업 영역 안에서 완전히 렌더링된다.
- Windows 작업표시줄이나 다른 창이 결과 PNG에 포함되지 않는다.
- `load`, `process`, `alarm`, `transfer-out`, `complete` 5개 상태가 모두 1600×900으로 저장된다.
- 한국어와 영어 캡처가 모두 성공한다.
- PNG 해상도뿐 아니라 과도한 순수 검정 영역과 네 개 핵심 화면 영역을 자동 검사한다.
- 일반 WPF 실행에는 캡처 전용 최대화·최상단 설정이 적용되지 않는다.
- 전체 solution build, Core tests, 일반 WPF smoke test가 통과한다.
- 생성된 PNG는 `artifacts/`에만 두고 공개 Git에는 포함하지 않는다.

## 구현 결정

- 고정 1600×900 DIP 창 대신 캡처 모드에서만 창을 최대화한다.
- WPF의 중간 렌더 버퍼를 읽는 대신 Windows가 화면에 최종 합성한 client 영역을 `CopyFromScreen`으로 캡처한다.
- 현재 모니터의 `WorkingArea`를 사용해 작업표시줄 영역을 제외한다.
- 캡처한 원본은 고품질 보간으로 1600×900 PNG로 정규화한다.
- PowerShell은 순수 검정 표본 비율과 다음 네 영역의 밝은 픽셀 존재를 검사한다.
  1. 상단 제목 영역
  2. 왼쪽 장비 schematic 영역
  3. 오른쪽 조작/상태 영역
  4. 하단 debug/trace 영역

## Visual QA 판정

- 상태별 공정 강조, Wafer 위치, Gate 상태, 알람 탭 전환은 의도대로 구분된다.
- 대표 다섯 화면에서 핵심 영역 잘림은 발견되지 않았다.
- 한국어 모드에도 `CURRENT STEP`, `PROCESS INSTRUMENTS`, `FAULT SCENARIO SELECTOR` 같은 일반 UI 제목이 남아 있다.
- `Pump`, `Valve`, `Recipe`, `ALD` 같은 도메인 용어는 유지할 수 있지만, 일반 조작 문구의 한영 일관성은 별도 Goal로 정리하는 편이 안전하다.
- 오른쪽 알람/리포트 탭은 정보가 많으므로 기능 추가보다 번역과 위계 정리가 먼저다.

## 검증 명령

```powershell
.\scripts\Capture-WpfDemoScreenshots.ps1 -Language ko
.\scripts\Capture-WpfDemoScreenshots.ps1 -Language en -SkipBuild
dotnet build .\EquipmentTwinLab.sln -c Release
dotnet run --project .\tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj -c Release --no-build
```

## 다음 작업

- `Goal 068: WPF 한국어 일반 UI 라벨 일관성 정리`
- 장비 도메인 용어는 유지하고, 조작 버튼·영역 제목·상태 설명처럼 사용자가 읽는 일반 문구만 한영 전환에 연결한다.
