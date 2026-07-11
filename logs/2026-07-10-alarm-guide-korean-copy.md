# 2026-07-10 알람 작업지시서 한국어 문장 품질 정리 작업 로그

## 1. 한 일

- VAC-101, TMP-201, GAS-301, SEQ-001 알람 가이드의 한국어 문장을 전부 다듬었다.
- 제목, 요약, 점검 항목, 대응 선택지, 다음 조치, 엔지니어 검토 요청 조건을 작업자 관점으로 정리했다.
- `WARNING`을 한국어 모드에서 `경고`로 표시하도록 했다.
- `2 체크`를 `2 점검`, `checks 완료`를 `점검 n/n 완료`로 바꿨다.
- 한국어 Markdown 이슈 리포트의 제목, 메타데이터, 표, 상태, 요청 조건을 한국어로 렌더링했다.
- Export 요청에 `language=ko/en`을 추가했다.
- JSON의 `severity`는 `Warning` 같은 기계용 값을 유지하고 Markdown에서만 `경고`로 표시했다.
- 화면의 번역된 제목과 요약을 사람이 읽는 이슈 리포트에도 사용하도록 연결했다.

## 2. 바뀐 파일

- `src/EquipmentTwin.Hmi.Wpf/ViewModels/OperatorConsoleViewModel.cs`
  - 56개 알람 가이드 문장 매핑과 Severity/UI 상태 문구를 정리했다.
  - Export 요청에 Language와 한국어 표시 문장을 전달한다.
- `src/EquipmentTwin.Hmi.Wpf/Services/AlarmIssueReportExportService.cs`
  - Language에 따라 Markdown 문서 구조를 한국어 또는 영어로 생성한다.
  - JSON Payload의 Severity는 원래 enum 문자열을 유지한다.
- `goals/071-alarm-guide-korean-copy.md`
- `plan.md`, `state/loop-state.md`, `state/triage.md`

## 3. 검증 결과

- 알람 가이드 4개, 사용자 문장 56개 한국어 매핑 확인.
- 누락 0개, 지정한 번역투 잔존 표현 0개.
- 한국어 Screenshot 5개: 1600×900, anchor 4/4 통과.
- 영어 Screenshot 5개: 1600×900, anchor 4/4 통과.
- 한국어 GAS-301 화면에서 제목, 요약, 점검, 대응 선택지, `경고` 배지를 직접 확인했다.
- Export Service smoke 검증 통과:
  - Markdown 제목 `알람 이슈 리포트` 확인.
  - 한국어 요약/표/점검/대응/요청 조건 확인.
  - JSON `language=ko`, `alarmCode=GAS-301`, `severity=Warning` 확인.
- 전체 solution Release build: 경고 0, 오류 0.
- Core tests: 전체 통과.
- 일반 WPF smoke test: 4초 동안 조기 종료 없음.
- `git diff --check`: 통과.

검증용 리포트는 `artifacts/alarm-reports/` 아래에 있으며 Git에는 포함하지 않는다.

## 4. 막힌 점과 해결 방법

현재 blocker는 없다.

- 알람 JSON을 오래된 `equipment-templates/...` 경로에서 찾으려다 실패했다. Service의 실제 상대 경로를 확인해 `alarm-guides/moly-ald-alarm-guides.json`을 사용했다.
- Windows에서 `rg` 인수에 wildcard 파일 경로를 넘겨 경로 구문 오류가 났다. 검색 대상을 개별 파일 또는 디렉터리로 지정했다.
- 첫 빌드에서 문자열 속성 `AlarmGuideSeverity`와 Core enum `AlarmGuideSeverity`의 이름이 충돌했다. enum을 전체 네임스페이스로 명시했다.
- 한국어 Screenshot 한 장에 Windows Alt+Tab 화면이 겹쳤다. 자동 수치 검사는 통과했지만 사람 QA에서는 실패로 판단하고 알람 화면을 다시 캡처했다.
- PowerShell 7이 없어 .NET 8 WPF 어셈블리를 PowerShell에서 직접 불러오는 검증 방식은 사용하지 못했다.
- 실제 WPF UI 자동 검증은 사용자의 Escape 입력과 이후 Windows 접근성 인터페이스 오류 `0x80004002`로 중단됐다. UI를 추측해서 누르지 않고 Export Service를 직접 호출하는 smoke 검증으로 전환했다.
- Export 요청 패치의 첫 문맥이 실제 생성자 인수 순서와 달라 적용되지 않았다. 호출부를 다시 읽고 정확한 줄만 수정했다.
- 임시 smoke 프로젝트의 기본 파일 줄바꿈이 예상과 달라 첫 패치가 실패했고, 파일 전체를 교체했다.
- 임시 smoke 프로그램에서 `System.IO`가 암시적으로 포함되지 않아 첫 컴파일이 실패했다. `using System.IO;`를 명시한 뒤 통과했다.

## 5. 보류한 판단

- 영어 원본 JSON에 다국어 필드를 추가하지 않았다.
- `.resx` 기반 다국어 시스템은 언어 또는 화면 수가 늘 때 검토한다.
- 현재 한국어 Payload에는 작업자가 읽는 제목과 요약이 포함된다. 서버 자동화는 번역 문장이 아니라 `alarmCode`, `severity`, 수치 필드를 사용해야 한다.
- Export/Outbox 계약을 CI에서 직접 실행하는 자동 테스트는 Goal 072로 남겼다.

## 6. 소프트웨어 아키텍처 설명

```text
alarm-guides JSON
  - 영어 원본 / 알람 지식의 기준
        |
        v
OperatorConsoleViewModel
  - 한국어 표시 문장 선택
  - Language(ko/en) 전달
        |
        +--> WPF 화면
        |
        +--> AlarmIssueReportExportService
               - 한국어/영어 Markdown 렌더링
               - JSON Payload 생성
```

- 영어 JSON은 Core 테스트와 알람 코드 연결의 기준이라 그대로 유지한다.
- ViewModel은 작업자에게 어떤 언어로 보여줄지 결정한다.
- Export Service는 `Language`를 읽어 Markdown 구조를 번역한다.
- JSON의 `alarmCode`, `severity`, Pressure/Temperature 같은 필드는 기계가 읽는 계약이다.
- 제목과 요약은 사람이 읽는 표시 정보이므로 `language`와 함께 해석한다.

## 7. 유지보수할 때 봐야 할 파일

- 영어 알람 원문: `alarm-guides/moly-ald-alarm-guides.json`
- 한국어 문장 매핑: `src/EquipmentTwin.Hmi.Wpf/ViewModels/OperatorConsoleViewModel.cs`
- Markdown/JSON 생성: `src/EquipmentTwin.Hmi.Wpf/Services/AlarmIssueReportExportService.cs`
- 서버 대기열 저장: `src/EquipmentTwin.Hmi.Wpf/Services/AlarmIssueReportOutboxService.cs`

새 알람 문장을 JSON에 추가하면 `LocalizeGuideText`에 같은 영어 문자열 key와 한국어 문장을 추가한다.

## 8. 사용자가 이해해야 할 개념

- `source of truth`: 여러 화면과 문서가 공통으로 참조하는 기준 데이터다. 여기서는 영어 알람 JSON이다.
- `localization boundary`: 언어를 바꾸는 책임이 시작되는 경계다. 이 프로젝트에서는 WPF ViewModel과 Export Service다.
- `machine-readable field`: 서버가 조건 분기나 집계에 사용하는 안정된 필드다. `alarmCode`, `severity`, 수치값이 해당한다.
- `display text`: 작업자가 읽는 제목과 설명이다. 언어에 따라 달라질 수 있다.
- `fully qualified name`: 같은 이름의 속성과 enum이 있을 때 `EquipmentTwin.Core.Alarms.AlarmGuideSeverity.Warning`처럼 전체 경로를 쓰는 방법이다.
- `contract test`: Payload의 필드명과 값 형식이 바뀌지 않았는지 확인하는 테스트다.

## 9. 다음 작업

- `Goal 072: 알람 리포트와 서버 Payload 계약 자동 테스트`
- 이번에 수동 smoke로 확인한 Language, Severity, Markdown 제목을 CI에서 자동 검사한다.
