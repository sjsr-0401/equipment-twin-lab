# Equipment Twin Lab 개발 과정 처음부터 이해하기

이 문서는 이 프로젝트를 처음부터 다시 이해하기 위한 설명서다.

목표 독자는 주니어 개발자 또는 한동안 소프트웨어에서 떨어져 있다가 다시 감을 되찾는 사람이다.

## 0. 이 프로젝트를 왜 시작했나

처음 목표는 단순했다.

```text
실제 장비가 없어도 장비 SW 엔지니어의 사고방식을 보여주는 대표 프로젝트를 만들자.
```

사용자는 장비 SW 경험이 있지만, 실제 장비는 개인이 집에서 테스트하기 어렵다.

그래서 방향을 이렇게 잡았다.

```text
실제 장비 대신 가상 장비 모델을 만들고,
그 가상 장비를 대상으로 상태 전이, IO, 알람, 공정, UI를 검증한다.
```

즉 이 프로젝트는 게임이나 예쁜 화면만 만드는 프로젝트가 아니다.

핵심은 다음이다.

- 장비 상태가 어떻게 바뀌는가
- 센서/액추에이터 IO가 어떻게 연결되는가
- 알람이 언제 발생하고 어떻게 복구되는가
- 공정 레시피가 어떻게 실행되는가
- 사용자가 HMI에서 그 상태를 어떻게 보고 조작하는가

## 1. 처음부터 UI를 만들지 않은 이유

처음부터 WPF나 Unity 화면을 만들면 빠르게 “보이는 것”은 생긴다.

하지만 장비 SW 프로젝트에서는 화면보다 먼저 정해야 하는 것이 있다.

```text
진짜 상태는 어디에 있는가?
```

이 프로젝트에서는 그 답을 `EquipmentTwin.Core`로 정했다.

```text
Core = 장비 동작의 진실
```

화면은 언제든 바뀔 수 있다.

- CLI
- Unity
- WPF
- 나중의 웹 UI

하지만 Core가 안정되어 있으면 화면은 Core를 보여주는 껍데기가 된다.

그래서 첫 개발 순서는 이렇게 갔다.

```text
Core 먼저
테스트 다음
CLI 자동화 다음
시각화/화면은 그 다음
```

## 2. 1단계: 상태머신부터 만든 이유

장비 SW의 가장 기본은 상태다.

예를 들면 장비는 보통 이런 흐름을 가진다.

```text
Idle
  -> Loading
  -> Processing
  -> Inspection
  -> Complete
```

그리고 문제가 생기면:

```text
Any State
  -> Alarm
```

처음 구현한 것은 이 상태 전이를 코드로 표현하는 일이었다.

왜 중요하냐면, 상태머신이 없으면 장비가 아무 때나 아무 행동을 하게 된다.

예를 들어:

- 문이 열렸는데 공정이 계속 진행됨
- 알람 상태인데 Complete로 넘어감
- 초기화도 안 했는데 검사 시작함

이런 일이 생기면 장비 SW로는 신뢰할 수 없다.

그래서 첫 번째 핵심은:

```text
허용된 상태 전이만 가능하게 만들기
```

## 3. 2단계: 가상 IO를 만든 이유

장비는 외부 세계와 IO로 대화한다.

예:

- DoorClosed 센서
- EmergencyStop 버튼
- Clamp 출력
- Vacuum 출력

실제 장비가 없으므로 우리는 가상 IO를 만들었다.

중요한 설계는 방향성이다.

```text
Input  = 장비 SW가 읽는 값
Output = 장비 SW가 쓰는 값
```

그래서 장비 SW가 Input을 마음대로 쓰면 안 된다.

예:

```text
DoorClosed는 센서 입력이다.
장비 SW가 DoorClosed=true로 조작하면 현실성이 깨진다.
```

반대로 Simulator는 Input을 조작할 수 있다.

```text
Simulator가 DoorClosed=false로 바꾸면
장비 SW는 문이 열린 것으로 판단해야 한다.
```

이게 실제 장비 테스트의 핵심을 흉내 낸 부분이다.

## 4. 3단계: 테스트를 먼저 붙인 이유

이 프로젝트는 매일 조금씩 커진다.

매일 기능을 추가하면 이전 기능이 깨질 수 있다.

그래서 테스트를 계속 추가했다.

테스트의 목적은 단순하다.

```text
내가 고친 코드가 예전 기능을 망가뜨리지 않았는지 확인한다.
```

예:

- 정상 시나리오가 Complete까지 가는가
- 잘못된 상태 전이를 거부하는가
- Door open 알람이 발생하는가
- Emergency stop 복구 조건이 맞는가
- Timeout이 알람으로 바뀌는가

테스트가 많아지면 개발 속도가 느려지는 것처럼 보일 수 있다.

하지만 실제로는 반대다.

```text
테스트가 있으니 더 과감하게 고칠 수 있다.
```

## 5. 4단계: Clock/Timeout을 만든 이유

장비에는 시간이 중요하다.

예:

- 10초 안에 센서가 안 들어오면 알람
- 모션이 정해진 시간 안에 끝나야 함
- 공정 스텝마다 시간이 있음

하지만 테스트에서 실제 10초를 기다리면 비효율적이다.

그래서 `ManualClock` 같은 가상 시간을 만들었다.

개념은 이렇다.

```text
현실 시간 기다리기 X
테스트가 시간을 직접 앞으로 밀기 O
```

이렇게 하면 Timeout 테스트를 빠르고 정확하게 할 수 있다.

## 6. 5단계: Scenario JSON을 만든 이유

코드 안에 시나리오를 하드코딩하면 매번 빌드해야 한다.

그래서 JSON 파일로 시나리오를 분리했다.

예:

```text
normal-cycle.json
loading-timeout.json
door-open-alarm.json
```

이렇게 하면 사용자는 코드를 몰라도 시나리오를 볼 수 있다.

그리고 CLI로 실행할 수 있다.

```text
dotnet run --project src/EquipmentTwin.Cli -- scenarios/normal-cycle.json
```

이것이 중요한 이유:

```text
장비 동작을 코드가 아니라 데이터로 설명할 수 있다.
```

## 7. 6단계: CLI를 만든 이유

CLI는 화면 없이 실행하는 도구다.

왜 필요하냐면 자동화와 CI에 좋기 때문이다.

WPF나 Unity는 사람이 눈으로 봐야 한다.

하지만 CLI는 GitHub Actions에서 자동으로 실행할 수 있다.

```text
빌드
테스트
시나리오 실행
리포트 생성
```

이 흐름을 자동화하면 매일 작업할 때 안정성이 생긴다.

## 8. 7단계: Template/Recipe 구조를 만든 이유

처음에는 단일 장비만 생각할 수 있다.

하지만 대표 프로젝트로 만들려면 사용자가 장비 구성을 바꾸는 방향이 필요했다.

그래서 Template/Recipe 개념이 들어왔다.

```text
Template = 장비 구성
Recipe   = 제품/공정 조건
Fault    = 일부러 넣는 문제 조건
```

이 구조는 면접에서 설명하기 좋다.

```text
하나의 장비 SW 코어가 여러 구성과 레시피를 실행할 수 있게 설계했습니다.
```

## 9. 8단계: 제조/공정 도메인으로 방향을 바꾼 이유

처음에는 비전 검사도 고려했다.

하지만 사용자의 실무 배경은 장비/공정 쪽에 더 가깝다.

그래서 공개 가능한 범위에서 ALD 증착 공정 컨셉을 모델링했다.

주의할 점:

```text
이 프로젝트는 실제 특정 회사 장비 내부 동작을 복제하지 않는다.
공개 자료와 일반적인 공정 개념을 바탕으로 만든 synthetic demo다.
```

이 정직성이 중요하다.

면접에서 과장하면 안 된다.

말할 수 있는 표현:

```text
반도체 증착 장비의 일반적인 공정 흐름을 참고해,
가상의 ALD 공정 시뮬레이션과 HMI를 만들었습니다.
```

말하면 안 되는 표현:

```text
실제 장비와 동일하게 구현했습니다.
```

## 10. 9단계: Unity를 붙인 이유

처음에는 3D 시뮬레이터가 차별화 포인트가 될 수 있다고 봤다.

Unity를 붙인 이유:

- 공정 흐름을 눈으로 보여주기 좋음
- 장비 외형/3D 모델 교체 가능성 있음
- 포트폴리오 첫인상에 강함

하지만 실제로 해보니 문제가 있었다.

- 장비 3D 모델링 퀄리티를 올리는 데 시간이 많이 듦
- Unity 학습 비용이 큼
- 사용자가 직접 디버깅하기 불편함
- 장비 SW 설명보다 게임/그래픽 작업처럼 보일 위험이 있음

그래서 결론은:

```text
Unity = 선택형 3D viewer
WPF   = 메인 HMI/debug surface
```

## 11. 10단계: WPF로 전환한 이유

WPF는 Windows 데스크톱 UI 기술이다.

장비 HMI 느낌과 잘 맞는다.

그리고 Visual Studio에서 바로 디버깅하기 좋다.

```text
F5 실행
버튼 클릭
breakpoint 확인
ViewModel 상태 확인
Core runner 진입
```

사용자가 직접 내 것으로 만들기에는 WPF가 Unity보다 낫다.

그래서 메인 방향을 WPF로 바꿨다.

## 12. WPF 구조 이해하기

현재 WPF 흐름은 이렇다.

```text
MainWindow.xaml
  -> OperatorConsoleViewModel
  -> MolyAldRecipeService
  -> MolyAldRunner
  -> MolyAldTimelineDocument
  -> 화면 갱신
```

각 파일 역할:

| 파일 | 역할 |
|---|---|
| `MainWindow.xaml` | 화면 배치와 색상, 버튼, 표 정의 |
| `OperatorConsoleViewModel.cs` | 화면에 보여줄 상태와 버튼 동작 |
| `MolyAldRecipeService.cs` | 공정 레시피 파일 로드 |
| `MolyAldRunner` | 실제 공정 실행 로직 |
| `MolyAldTimelineDocument` | 실행 결과를 화면/Unity/리포트가 읽기 좋은 형태로 변환 |

## 13. 내가 코드를 고칠 때 생각하는 순서

매번 대충 고치는 것이 아니라 이런 순서로 접근한다.

### 1. 사용자의 문제를 재정의한다

예:

```text
“글씨가 안 보여”
```

이걸 개발 언어로 바꾸면:

```text
DataGrid 기본 WPF 스타일이 다크 테마와 충돌한다.
특정 Label이 컨테이너 크기 때문에 clipping된다.
```

### 2. 관련 파일을 찾는다

WPF 화면 문제면 보통:

```text
MainWindow.xaml
OperatorConsoleViewModel.cs
```

Core 동작 문제면:

```text
src/EquipmentTwin.Core
tests/EquipmentTwin.Core.Tests
```

CLI 문제면:

```text
src/EquipmentTwin.Cli/Program.cs
```

### 3. 로직 문제인지 표현 문제인지 구분한다

예:

```text
NO ALARM이 초록색이면 좋겠다
```

이건 공정 로직 문제가 아니다.

```text
Core 수정 X
ViewModel/XAML 수정 O
```

### 4. 작은 단위로 수정한다

한 번에 다 바꾸면 원인을 추적하기 어렵다.

예:

```text
1. 색상 토큰 수정
2. ViewModel 상태 문자열 수정
3. XAML 바인딩 수정
4. 문서 수정
5. 테스트
```

### 5. 검증한다

현재 기본 검증:

```powershell
dotnet build EquipmentTwinLab.sln --configuration Release
dotnet run --project tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj --no-restore --configuration Release
```

WPF 시작 확인:

```powershell
dotnet run --project src\EquipmentTwin.Hmi.Wpf\EquipmentTwin.Hmi.Wpf.csproj --configuration Release
```

### 6. 로그를 남긴다

왜 바꿨는지 적어야 나중에 유지보수할 수 있다.

이 프로젝트에서는 보통 다음 위치에 남긴다.

```text
goals/
logs/
docs/
state/
```

## 14. 현재 색상 방향을 왜 다시 바꿨나

한 번은 엄격한 산업 HMI 스타일을 적용했다.

원칙은 좋았다.

하지만 사용자 목표는 지금 “실제 팹 인증 HMI”가 아니라 “포트폴리오 데모”다.

그래서 결론을 바꿨다.

```text
데모에서는 정상 상태 초록/파랑을 유지한다.
색상 규칙을 너무 엄격하게 가져가지 않는다.
대신 화면이 읽히고 설명 가능해야 한다.
```

이게 중요한 이유:

```text
좋은 엔지니어링은 원칙을 외우는 것이 아니라,
목표에 맞게 원칙을 적용하거나 버리는 것이다.
```

## 15. 지금부터 사용자가 직접 따라할 루틴

작업 전:

```powershell
git status
```

WPF 실행:

```powershell
dotnet run --project src\EquipmentTwin.Hmi.Wpf\EquipmentTwin.Hmi.Wpf.csproj
```

전체 빌드:

```powershell
dotnet build EquipmentTwinLab.sln
```

Core 테스트:

```powershell
dotnet run --project tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj
```

Visual Studio에서는:

```text
EquipmentTwin.Hmi.Wpf를 시작 프로젝트로 설정
F5
버튼 클릭
breakpoint 확인
```

## 16. 모르는 용어가 나오면 추가할 곳

아래 용어장은 계속 확장한다.

| 용어 | 짧은 설명 |
|---|---|
| Core | 화면과 상관없는 핵심 장비 로직 |
| ViewModel | 화면이 보여줄 데이터를 준비하고 버튼 동작을 처리하는 계층 |
| XAML | WPF 화면 레이아웃을 작성하는 XML 기반 파일 |
| Binding | ViewModel 값과 화면 컨트롤을 연결하는 방식 |
| OneWay Binding | ViewModel에서 화면으로만 값이 흐르는 바인딩 |
| TwoWay Binding | 화면 입력이 ViewModel로 다시 들어가는 바인딩 |
| CLI | 명령줄에서 실행하는 프로그램 |
| CI | GitHub에서 빌드/테스트를 자동으로 돌리는 시스템 |
| Scenario | 장비 동작을 재현하기 위한 입력 시나리오 |
| Recipe | 공정 조건 묶음 |
| Fault | 의도적으로 넣는 문제 조건 |
| HMI | 작업자가 장비 상태를 보고 조작하는 화면 |

## 17. 다음에 이 문서에 추가할 것

- WPF `MainWindow.xaml` 한 줄씩 읽기
- `OperatorConsoleViewModel.cs` 한 함수씩 읽기
- `MolyAldRunner` 공정 실행 흐름 읽기
- 테스트 하나를 직접 추가하는 과정
- 버튼 하나를 새로 추가하는 과정
