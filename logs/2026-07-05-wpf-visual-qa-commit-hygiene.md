# 2026-07-05 WPF 화면 검수와 커밋 정리

## 1. 한 일

- 공개 repo에 올라가면 안 되는 개인 기록 경계를 다시 확인했다.
- `private-notes/`가 local exclude에만 들어 있던 상태를 repo `.gitignore`에도 명시했다.
- Unity local 생성 folder를 `.gitignore`에 추가했다.
- `PackageManagerSettings.asset`가 개인 정보 파일인지 project setting인지 확인했다.
- 전체 solution build, Core tests, diff check를 실행했다.
- WPF 자동 screenshot 검수를 시도했다.

## 2. 바뀐 파일

- `.gitignore`
- `state/loop-state.md`
- `state/triage.md`
- `goals/056-wpf-visual-qa-commit-hygiene.md`
- `logs/2026-07-05-wpf-visual-qa-commit-hygiene.md`

## 3. 검증 결과

통과:

```powershell
dotnet build .\EquipmentTwinLab.sln
dotnet run --project .\tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj
git diff --check
```

결과:

- solution build: warning 0, error 0
- Core tests: 전체 통과
- `git diff --check`: 실제 diff 오류 없음
- Windows line ending 경고만 있음

`private-notes/` ignore 확인:

```text
.gitignore:14:private-notes/ private-notes
```

## 4. 막힌 점과 해결 방법

막힌 점:

- WPF 자동 screenshot 검수는 완료하지 못했다.

시도한 방법:

1. `dotnet run --project ...` 실행 후 screenshot 캡처
   - 결과: WPF가 아니라 `dotnet.exe` console window를 캡처함.
2. WPF `.exe` 직접 실행 후 `MainWindowHandle` 캡처
   - 결과: WPF process는 살아 있었지만 `MainWindowHandle`이 0.
3. process id 기준 visible window enumerate
   - 결과: visible window를 찾지 못함.

해결 / 판단:

- 자동 visual QA는 이번 환경에서 막혔다.
- 사용자가 직접 WPF를 실행해 screenshot을 보내주면 layout 조정을 이어가는 것이 현실적이다.

## 5. 보류한 판단

- `unity/EquipmentTwin.Unity/ProjectSettings/PackageManagerSettings.asset`는 삭제하거나 ignore하지 않았다.
- 이유: Unity project setting 성격이고, 개인 token/local path가 보이지 않는다.
- 다음 커밋에 포함 후보로 둔다.

## 6. 소프트웨어 아키텍처 설명

이번 작업은 runtime architecture를 바꾸지 않았다.

바뀐 것은 repository hygiene이다.

```text
public repo에 들어갈 것
  - source code
  - public docs
  - Unity project settings

public repo에 들어가지 않을 것
  - private-notes/
  - artifacts/
  - Unity Library/Temp/Logs/UserSettings
```

## 7. 유지보수할 때 봐야 할 파일

- 공개/비공개 파일 경계: `.gitignore`
- 현재 작업 상태: `state/loop-state.md`
- 다음 작업 우선순위: `state/triage.md`
- 이번 goal 기록: `goals/056-wpf-visual-qa-commit-hygiene.md`

## 8. 사용자가 이해해야 할 개념

- `.gitignore`: Git에 올리지 않을 파일/폴더 규칙이다.
- `.git/info/exclude`: 내 PC에만 적용되는 local ignore 규칙이다.
- Public repo hygiene: 공개 저장소에 개인 메모, local build 결과, 자동 생성 파일이 섞이지 않게 관리하는 것.
- Unity `ProjectSettings`: 보통 project 재현에 필요한 설정이므로 공개 repo에 포함될 수 있다.
- Unity `Library/`, `Temp/`, `Logs/`, `UserSettings/`: 로컬에서 재생성되는 파일이라 보통 Git에 올리지 않는다.

## 9. 다음 작업

추천:

```text
목표 057: WPF 실제 화면 확인 후 trend panel spacing 조정
```

사용자가 WPF 화면 screenshot을 보내주면 trend panel의 폭, 글자 크기, 오른쪽 panel 밀도를 기준으로 바로 수정한다.
