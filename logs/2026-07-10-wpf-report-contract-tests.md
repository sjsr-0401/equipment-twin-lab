# 2026-07-10 WPF 알람 리포트·Payload 계약 자동 테스트 작업 로그

## 1. 한 일

- `EquipmentTwin.Hmi.Wpf.Tests` 콘솔 테스트 프로젝트를 추가했다.
- 한국어 Markdown과 JSON 계약을 자동 검증했다.
- 영어 Markdown 회귀를 자동 검증했다.
- 서버 Outbox Envelope와 Payload 계약을 자동 검증했다.
- 테스트가 만든 리포트/Outbox 파일을 각 테스트의 `finally`에서 삭제하도록 했다.
- 새 테스트 프로젝트를 `EquipmentTwinLab.sln`에 추가했다.
- GitHub Actions에 `Run WPF report contract tests` 단계를 추가했다.

## 2. 바뀐 파일

- `tests/EquipmentTwin.Hmi.Wpf.Tests/EquipmentTwin.Hmi.Wpf.Tests.csproj`
  - `net8.0-windows` 콘솔 테스트 프로젝트다.
  - WPF 프로젝트만 참조한다.
- `tests/EquipmentTwin.Hmi.Wpf.Tests/Program.cs`
  - 세 가지 계약 테스트와 최소 Assertion helper를 포함한다.
- `EquipmentTwinLab.sln`
  - WPF 계약 테스트 프로젝트를 solution build에 포함한다.
- `.github/workflows/ci.yml`
  - Core tests 다음에 WPF 계약 테스트를 실행한다.
- `goals/072-wpf-report-contract-tests.md`
- `plan.md`, `state/loop-state.md`, `state/triage.md`

사용자가 별도로 만든 것으로 보이는 `logs/2026-07-10-portfolio-screenshot-refresh.md`는 수정하거나 커밋 범위에 포함하지 않았다.

## 3. 검증 결과

- 전체 solution Release build: 경고 0, 오류 0.
- Core tests: 전체 통과.
- WPF 계약 테스트 3개 통과:
  - Korean issue report localizes Markdown and preserves JSON contract.
  - English issue report keeps English Markdown.
  - Server outbox preserves envelope and payload contract.
- 테스트 실행 후 생성 파일 정리 확인.
- `git diff --check`: 통과.

## 4. 막힌 점과 해결 방법

현재 blocker는 없다.

- 첫 solution build는 이전 UI 검증에서 실행 중이던 `EquipmentTwin.Hmi.Wpf` 프로세스가 Release DLL을 잠가 실패했다.
- 오류에 표시된 PID 40488의 실행 경로가 현재 저장소 WPF 실행 파일과 정확히 일치하는지 확인한 뒤 해당 프로세스만 종료했다.
- 두 번째 build에서는 새 WPF 테스트 프로젝트가 `System.IO`를 암시적 using으로 가져오지 않아 `File` 식별자 오류와 정적 함수 연쇄 오류가 발생했다.
- `using System.IO;`를 명시한 뒤 경고 0, 오류 0으로 build와 테스트가 통과했다.

## 5. 보류한 판단

- xUnit, NUnit, MSTest 같은 외부 테스트 프레임워크는 추가하지 않았다.
- 실제 HTTP 전송 Retry와 실패 상태 저장은 이번 계약 테스트 범위에 포함하지 않았다.
- Mock Server를 실제로 실행하는 통합 테스트는 Goal 073 이후 필요성을 검토한다.
- CI에서 생성된 계약 테스트 파일을 Artifact로 업로드하지 않는다. 테스트 안에서 읽고 삭제한다.

## 6. 소프트웨어 아키텍처 설명

```text
EquipmentTwin.Core.Tests (net8.0)
  -> Core 공정/상태/IO/모션 검증

EquipmentTwin.Hmi.Wpf.Tests (net8.0-windows)
  -> WPF Export Service
  -> WPF Outbox Service
  -> Markdown / JSON 계약 검증
```

- Core 테스트는 Windows UI에 의존하지 않아 계속 `net8.0`으로 유지한다.
- WPF 계약 테스트만 `net8.0-windows`를 사용한다.
- 테스트가 WPF View나 Window를 띄우지는 않는다. 파일 생성 Service를 직접 호출한다.
- 각 계약 테스트는 실제 파일을 생성하고 다시 읽어 직렬화 결과를 검증한다.
- 파일 경로를 반환받아 `finally`에서 삭제하므로 테스트가 실패하더라도 가능한 범위에서 정리한다.

## 7. 유지보수할 때 봐야 할 파일

- 테스트 목록과 계약 값: `tests/EquipmentTwin.Hmi.Wpf.Tests/Program.cs`
- 테스트 프로젝트 설정: `tests/EquipmentTwin.Hmi.Wpf.Tests/EquipmentTwin.Hmi.Wpf.Tests.csproj`
- Markdown 생성 코드: `src/EquipmentTwin.Hmi.Wpf/Services/AlarmIssueReportExportService.cs`
- Outbox 생성 코드: `src/EquipmentTwin.Hmi.Wpf/Services/AlarmIssueReportOutboxService.cs`
- CI 실행 순서: `.github/workflows/ci.yml`

Payload 필드를 추가하거나 이름을 바꾸면 제품 코드와 함께 `Program.cs`의 JSON field assertion을 수정한다.

## 8. 사용자가 이해해야 할 개념

- `contract test`: 내부 구현이 아니라 외부에 약속한 파일 구조와 필드값을 검증하는 테스트다.
- `regression`: 전에 되던 기능이 새 변경 때문에 다시 깨지는 현상이다.
- `test fixture`: 테스트에 사용하는 일정한 입력 데이터다. `CreateRequest`가 이 역할을 한다.
- `finally`: 성공 또는 실패와 관계없이 마지막에 실행되는 C# 블록이다. 생성 파일 정리에 사용했다.
- `ProjectReference`: NuGet 배포 없이 같은 solution의 다른 프로젝트 코드를 직접 참조하는 방법이다.
- `net8.0-windows`: Windows 기능을 사용하는 프로젝트를 위한 .NET Target Framework다.
- `CI gate`: 자동 테스트가 실패하면 변경을 통과시키지 않는 품질 관문이다.

## 9. 다음 작업

- `Goal 073: 서버 전송 실패 Retry와 Outbox 상태 관리`
- 연결 실패 Payload를 다시 보낼 수 있게 하고 queued/sent/failed 상태를 명확히 관리한다.
