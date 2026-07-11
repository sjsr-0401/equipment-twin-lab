# 목표 057: WPF 읽기 전용 TextBox 바인딩 예외 수정

작성일: 2026-07-08

## 목표

Visual Studio 디버그 실행 중 발생한 WPF 바인딩 예외를 수정한다.

## 완료 기준

- `LatestServerPayloadPreviewText` 바인딩 예외 원인을 확인한다.
- 필요한 XAML 바인딩만 최소 수정한다.
- WPF project build가 통과한다.
- 전체 solution build가 통과한다.
- Core tests가 통과한다.
- 수정 원인과 검증 결과를 작업 로그에 남긴다.

## 원인

`LatestServerPayloadPreviewText`는 ViewModel의 읽기 전용 계산 속성이다.

WPF `TextBox.Text` 바인딩은 기본값이 `TwoWay`라서, 화면 값이 ViewModel 속성으로 다시 쓰이려고 한다.
하지만 이 속성에는 setter가 없기 때문에 실행 중 `InvalidOperationException`이 발생했다.

## 변경 내용

- `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml`
  - 서버 payload preview `TextBox`의 `Text` 바인딩을 `Mode=OneWay`로 명시했다.

## 검증

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

## 막힌 점

처음 검증 때 WPF build와 Core tests를 동시에 실행하면서 `obj` 폴더의 DLL 파일 lock 오류가 발생했다.
코드 오류가 아니라 병렬 검증 방식 문제였고, 순차 실행으로 해결했다.

## 유지보수 메모

읽기 전용 ViewModel 속성을 `TextBox.Text`, `ComboBox.SelectedItem`, `CheckBox.IsChecked`처럼 기본 `TwoWay`인 속성에 연결할 때는 `Mode=OneWay`를 명시해야 한다.
