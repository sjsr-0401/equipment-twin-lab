# 2026-07-03 WPF 바인딩 예외 수정 작업 로그

## 상황

Visual Studio에서 WPF HMI를 디버그 실행하던 중 다음 예외가 발생했다.

```text
System.InvalidOperationException:
TwoWay 또는 OneWayToSource 바인딩은
OperatorConsoleViewModel 형식의 읽기 전용 속성 'TimelineProgress'에서 작동하지 않습니다.
```

## 원인

`TimelineProgress`는 ViewModel에서 계산해서 화면에 보여주는 읽기 전용 값이다.

그런데 WPF의 `ProgressBar.Value` 바인딩이 값을 다시 ViewModel에 쓰려고 하면서 문제가 생겼다.

쉽게 말하면:

```text
ViewModel: "현재 진행률은 내가 계산해서 알려줄게"
ProgressBar: "그럼 내가 이 값을 다시 ViewModel에 저장할게"
ViewModel: "저장용 setter가 없는데?"
WPF: 예외 발생
```

## 수정

다음 네 개 진행률 바인딩에 `Mode=OneWay`를 명시했다.

- `TimelineProgress`
- `PressureProgress`
- `TemperatureProgress`
- `FilmProgress`

수정 후 방향은 이렇게 고정된다.

```text
ViewModel 계산값 -> WPF ProgressBar 표시
```

반대로 `ProgressBar -> ViewModel` 쓰기는 하지 않는다.

## 소프트웨어적으로 중요한 점

WPF/MVVM에서 모든 바인딩을 무조건 TwoWay로 두면 안 된다.

- 화면에 보여주기만 하는 값: `OneWay`
- 사용자가 직접 선택/입력하는 값: `TwoWay`
- 버튼 클릭: `Command`

이번 문제는 `ProgressBar`가 표시 전용 값인데 쓰기 가능한 값처럼 묶인 것이 원인이었다.

## 검증

통과한 검증:

- WPF 프로젝트 Release 빌드
- 전체 솔루션 Release 빌드
- Core 테스트 81개 통과
- WPF 시작 스모크 테스트: 앱이 시작 직후 종료되지 않고 4초 이상 유지됨
- `git diff --check`

## 막힌 점

실제 Visual Studio 디버거의 예외 팝업은 자동화로 직접 클릭해서 재현하지 않았다.

대신 원인이 된 바인딩 모드를 수정했고, WPF 앱이 시작 직후 죽지 않는지 프로세스 스모크로 확인했다.

## 다음 작업 후보

이제 다시 원래 예정대로 WPF HMI 기능을 키울 수 있다.

추천 다음 작업:

```text
Goal 052: WPF Alarm Recovery Procedure Panel
```

알람 발생 시 작업자가 무엇을 확인하고 어떤 순서로 복구해야 하는지 화면에 보여주는 패널이다.
