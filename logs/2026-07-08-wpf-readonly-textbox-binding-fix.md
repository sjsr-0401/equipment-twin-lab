# 2026-07-08 WPF 읽기 전용 TextBox 바인딩 예외 수정

## 1. 한 일

- Visual Studio 디버그 중 발생한 WPF `InvalidOperationException` 원인을 확인했다.
- `LatestServerPayloadPreviewText`가 읽기 전용 ViewModel 속성인데 `TextBox.Text` 기본 `TwoWay` 바인딩으로 연결되어 있었다.
- 해당 바인딩만 `Mode=OneWay`로 바꿨다.

## 2. 바뀐 파일

- `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml`
- `goals/057-wpf-readonly-textbox-binding-fix.md`
- `logs/2026-07-08-wpf-readonly-textbox-binding-fix.md`
- `state/loop-state.md`
- `state/triage.md`

## 3. 검증 결과

통과:

```powershell
dotnet build .\src\EquipmentTwin.Hmi.Wpf\EquipmentTwin.Hmi.Wpf.csproj
dotnet build .\EquipmentTwinLab.sln
dotnet run --project .\tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj
git diff --check -- src\EquipmentTwin.Hmi.Wpf\MainWindow.xaml
```

결과:

- WPF build: warning 0, error 0
- Solution build: warning 0, error 0
- Core tests: 전체 통과
- diff check: 실제 공백 오류 없음

## 4. 막힌 점과 해결

처음에는 WPF build와 Core tests를 병렬로 검증하면서 `EquipmentTwin.Core.dll` 파일 lock 오류가 났다.
이것은 코드 문제가 아니라 동시에 같은 build output을 만져서 생긴 검증 방식 문제였다.

해결:

- WPF build
- 전체 solution build
- Core tests

위 순서로 순차 실행했다.

## 5. 소프트웨어적으로 알아야 할 점

WPF MVVM에서 ViewModel의 계산 속성은 보통 getter만 가진다.
화면은 이 값을 읽기만 해야 하므로 `OneWay` 바인딩이 맞다.

반대로 사용자가 입력해서 ViewModel 값을 바꿔야 하는 필드는 setter가 있고 `TwoWay` 바인딩을 사용한다.

이번 문제는 화면이 읽기 전용 값을 다시 ViewModel에 쓰려고 해서 발생했다.

## 6. 다음 작업

추천:

```text
목표 058: WPF 실제 화면 기준 레이아웃/가독성 조정
```

이유:

- 현재 기능은 다시 빌드 가능하다.
- 다음은 실제 화면 screenshot 기준으로 잘림, 간격, 콤보박스 색상, 한국어/영어 라벨 혼합 문제를 정리하는 것이 맞다.
