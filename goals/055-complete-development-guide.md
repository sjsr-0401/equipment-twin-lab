# Goal 055: Complete Development Process Guide

Date: 2026-07-03
Branch: `goal/055-complete-development-guide`

## Objective

Create a much more detailed learning document that explains the whole project development process from the first Core model to the current WPF HMI.

The user clarified that the previous document was not enough. The target is not a short overview. The target is a guide that a junior developer could read to understand why the project was built in this order and how to add similar code later.

## User clarification

The requested scope:

```text
우리가 했던 모든 과정을 설명듣고 싶은거야.
프로젝트 맨 처음부터 주니어가 그걸 보고 코드를 짤 수 있을 정도로.
```

## Implemented

- Added `docs/learning/README.md`.
- Added `docs/learning/project-development-complete-guide.md`.
- Updated `docs/learning/development-from-zero.md` so it points to the complete guide.
- Added CI file/marker checks for the new learning guide.

## Guide scope

The complete guide covers:

- why Core came before UI;
- state machine design;
- virtual IO and input/output direction;
- CI purpose;
- clock and timeout design;
- cell controller;
- scenario JSON and CLI;
- alarm code and recovery;
- motion axis model;
- template/recipe/inspection/report model;
- public/synthetic ALD process model;
- Unity experiment and why it became optional;
- WPF main HMI transition;
- recurring design patterns;
- how to add new features;
- testing and debugging flow;
- honest current limitations.

## Validation

Passed in this branch:

- `git diff --check`
- `dotnet build EquipmentTwinLab.sln --configuration Release`
- `dotnet run --project tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj --no-restore --configuration Release`
  - 81 Core tests passed.

Note:

- The first build attempt was run in parallel with the Core test command and hit a temporary Windows DLL file lock in `obj/Release`.
- Re-running the build by itself passed with 0 warnings and 0 errors.

## Next recommended work

```text
Goal 056: Core state-machine line-by-line learning note
```

Reason:

The complete guide explains the whole story. The next step should zoom into one real code file so the user can connect the explanation to actual C# syntax.
