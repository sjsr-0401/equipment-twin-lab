# 2026-07-03 전체 개발 과정 해설서 작업 로그

## 사용자 피드백

사용자는 기존 `development-from-zero.md`가 충분하지 않다고 정정했다.

핵심은 “처음부터 훑는 개요”가 아니라, 프로젝트를 처음 보는 주니어가 개발 과정을 따라가며 코드를 짤 수 있을 정도의 설명이다.

## 결정

기존 문서를 억지로 계속 늘리는 대신 learning 폴더를 학습 문서 묶음으로 만들었다.

구조:

```text
docs/learning/README.md
docs/learning/development-from-zero.md
docs/learning/project-development-complete-guide.md
```

## 작성한 문서

`project-development-complete-guide.md`를 새로 추가했다.

설명 범위:

- 프로젝트 목적
- UI보다 Core를 먼저 만든 이유
- 상태머신
- 가상 IO
- CI
- Clock/Timeout
- Cell Controller
- Scenario JSON
- CLI
- Alarm/Recovery
- MotionAxis
- Template/Recipe/Inspection
- ALD Process Runner
- Unity 시도와 WPF 전환
- WPF Binding/Command/Timer
- 현재 아키텍처
- 새 기능을 추가하는 사고 순서
- 테스트/디버깅 순서
- 현재 한계
- 면접 설명법

## 소프트웨어적으로 중요한 점

이 작업은 기능을 추가하지 않았다.

대신 프로젝트의 지식을 코드 밖에 고정했다.

이유:

```text
AI와 함께 빠르게 만든 코드는 사용자가 이해하지 못하면 유지보수 자산이 아니라 부채가 된다.
```

그래서 이번 작업은 comprehension debt를 줄이는 작업이다.

## 다음 작업 제안

전체 흐름 문서를 만들었으니, 다음에는 실제 코드 한 파일을 줄 단위로 읽는 것이 좋다.

추천 순서:

```text
1. EquipmentStateMachine.cs
2. VirtualIoController.cs
3. EquipmentCellController.cs
4. MolyAldRunner.cs
5. OperatorConsoleViewModel.cs
6. MainWindow.xaml
```

첫 번째로는 `EquipmentStateMachine.cs`가 가장 좋다.

이 파일이 프로젝트의 출발점이기 때문이다.

## 검증 결과

아래 검증을 통과했다.

```powershell
git diff --check
dotnet run --project tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj --no-restore --configuration Release
dotnet build EquipmentTwinLab.sln --configuration Release
```

결과:

- Core 테스트 81개 통과
- Release 빌드 통과
- 빌드 경고 0개
- 빌드 오류 0개

## 작업 중 막힌 부분

처음에는 `dotnet build`와 Core 테스트를 병렬로 실행했다.

그 결과 Windows에서 `EquipmentTwin.Core.dll` 파일을 한 프로세스가 잡고 있는 동안 다른 프로세스가 같은 DLL을 쓰려고 해서 아래 유형의 오류가 났다.

```text
The process cannot access the file ... because it is being used by another process.
```

이것은 코드 오류가 아니라 병렬 실행으로 인한 파일 잠금 문제다.

해결:

```text
빌드와 테스트를 동시에 돌리지 않고, 순서대로 다시 실행했다.
```

그 결과 빌드는 정상 통과했다.
