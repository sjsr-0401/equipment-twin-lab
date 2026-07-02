# 2026-07-03 WPF Main Direction 작업 로그

## Goal 050: WPF Operator Console Shell

오늘 방향을 바꿨다. Unity를 메인 HMI로 쓰는 대신, WPF를 메인 operator console로 추가했다.

### 왜 바꿨나

Unity는 장비 3D viewer로는 좋지만, 매일 Visual Studio에서 빌드하고 디버깅하면서 장비 SW 감각을 익히기에는 진입 장벽이 높다.

WPF는 다음 이유로 현재 프로젝트에 더 맞다.

- Visual Studio에서 `F5`로 바로 실행 가능
- 버튼 클릭과 breakpoint 디버깅이 자연스러움
- 장비 HMI/제조 SW 포트폴리오 설명에 더 직접적임
- Core와 UI 분리 구조를 보여주기 좋음

### 구현한 것

- `src/EquipmentTwin.Hmi.Wpf` 프로젝트 추가
- `EquipmentTwinLab.sln`에 WPF 프로젝트 등록
- WPF HMI 화면 추가
  - Start / Stop / Step / Fault Replay / Reset
  - Fault scenario selector
  - Process schematic
  - Pressure / Temp / Film instruments
  - Alarm card
  - Operator action log
  - Timeline debug table
- WPF ViewModel이 `EquipmentTwin.Core`의 `MolyAldRunner`를 직접 호출하게 구성
- `scripts/Invoke-WpfHmi.ps1` 추가
- `docs/wpf-main-hmi.md` 추가

### 소프트웨어 구조

```text
WPF button click
  -> OperatorConsoleViewModel
  -> MolyAldRunner.Run()
  -> MolyAldTimelineDocument
  -> WPF Binding
```

중요한 점은 WPF가 공정 계산을 직접 하지 않는다는 것이다. WPF는 operator console이고, process truth는 계속 Core가 담당한다.

### 막힌 점 / 수정한 점

- 첫 WPF 빌드에서 `System.IO` namespace가 빠져서 `File`, `Path`, `DirectoryInfo`를 못 찾았다.
- `MolyAldRunner`는 `ManualClock`을 요구하는 구조라 WPF ViewModel에서도 deterministic clock을 명시적으로 넣었다.
- 수정 후 WPF 단독 빌드와 전체 솔루션 빌드가 통과했다.

### 검증 결과

- WPF 프로젝트 Release 빌드 통과
- 전체 솔루션 Release 빌드 통과
- Core test 81개 통과
- `git diff --check` 통과

### 다음 작업 후보

다음에는 WPF 화면을 실제로 더 장비답게 다듬는 작업이 좋다.

```text
Goal 051: WPF Alarm Recovery Procedure Panel
```

현재는 fault replay와 alarm 상태까지 보인다. 다음은 fault 종류별로 operator가 어떤 순서로 확인하고 reset해야 하는지 procedure panel로 보여주면 된다.
