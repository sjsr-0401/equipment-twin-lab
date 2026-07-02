# 2026-07-03 데모 색상 복원 + 처음부터 개발 과정 문서화 로그

## 사용자 피드백

사용자는 엄격한 ISA-101식 색상 절제보다 데모에서 잘 보이는 초록/파랑 정상 상태 표현을 원했다.

핵심 요청:

```text
정상 상태를 초록/파랑으로 하던 건 그대로 가자.
어차피 데모니까 색규칙은 없애자.
개발 과정은 아예 처음부터 주니어도 알 수 있게 세세히 적자.
```

## 결정

이번 결정은 다음과 같다.

```text
산업 HMI 표준은 참고만 한다.
현재 WPF는 포트폴리오 데모 가독성을 우선한다.
```

즉, 정상 상태가 초록/파랑으로 보이는 것은 허용한다.

## 코드 변경

### ViewModel

`OperatorConsoleViewModel`에서 표현용 색상과 상태 문자열을 데모 기준으로 복원했다.

- `NO ACTIVE ALARM` -> `NO ALARM`
- `OK TARGET` -> `TARGET`
- `GROWING`은 ASCII로 정리
- 밸브 텍스트를 `OPEN/CLOSED`에서 `ON/OFF`로 단순화
- 정상 상태 색상을 `SuccessBrush`, `PrimaryBrush` 중심으로 복원

### XAML

`MainWindow.xaml`에서 화면 색상 토큰을 데모용 색상으로 복원했다.

- progress bar 기본 색상: blue
- START 버튼: green
- FAULT REPLAY 버튼: amber
- Temp progress: amber
- Film progress: green
- Reactant label: cyan

단, 이전 Goal에서 고친 DataGrid 다크 테마와 LOAD PORT 가독성 수정은 유지했다.

## 문서 변경

추가한 핵심 문서:

```text
docs/learning/development-from-zero.md
```

이 문서는 이번 작업만 설명하지 않는다.

프로젝트를 처음 시작한 이유부터 다음 순서로 설명한다.

- 상태머신
- 가상 IO
- 테스트
- Clock/Timeout
- Scenario JSON
- CLI
- Template/Recipe
- ALD 공정 모델
- Unity 시도
- WPF 전환
- WPF 구조
- 개발자가 코드를 고칠 때 생각하는 순서

## 소프트웨어적으로 중요한 점

이번 작업은 Core 로직을 건드리지 않았다.

수정한 것은 전부 화면 표현 계층이다.

```text
Core = 공정 truth
ViewModel = 화면 표현용 상태 변환
XAML = 화면 배치와 색상
```

색상 방향을 바꿔도 Core 테스트가 깨지면 안 된다.

## 다음 학습 작업

다음에는 `MainWindow.xaml`을 위에서 아래로 읽으면서:

- Grid가 뭔지
- Binding이 뭔지
- Style이 뭔지
- Button Command가 뭔지
- DataGrid가 왜 필요한지

이런 식으로 한 줄씩 설명하는 문서를 추가하는 것이 좋다.

## 검증 결과

아래 검증을 통과했다.

```powershell
git diff --check
dotnet build EquipmentTwinLab.sln --configuration Release
dotnet run --project tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj --no-restore --configuration Release
```

결과:

- Release 빌드 통과
- Core 테스트 81개 통과
- WPF 시작 스모크 통과
