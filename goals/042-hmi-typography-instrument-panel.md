# Goal 042: HMI Typography and Instrument Panel

## 목표

Unity Canvas operator panel을 더 전문적인 HMI처럼 읽히게 만든다.

이번 목표는 버튼 기능 구현이 아니라, 화면의 정보 위계와 판독성을 올리는 것이다.

## 왜 필요한가

Goal 041에서 Canvas UI 분리는 완료했지만, telemetry 값은 아직 텍스트 중심이었다.

제조 HMI에서는 숫자만 보여주는 것보다 아래 정보가 같이 보여야 한다.

- 현재값
- 단위
- 정상 범위
- 현재값이 정상 범위 대비 어디에 있는지
- 알람 priority

## 구현한 것

- 오른쪽 panel title을 `MOLY ALD HMI`로 변경
- run/interlock state strip 추가
- command button 색상 체계 정리
  - START: green
  - STOP: red
  - FAULT: warning yellow
  - RESET: neutral dark
- Pressure/Temp/Film instrument row 추가
  - label
  - value/unit readout
  - status
  - normal range band
  - actual value fill
- Alarm card에 priority와 synthetic fault code 영역 추가
- Valve 상태는 instrument card에서 제거하고 event line에 유지
- 데모 screenshot 갱신

## 설계 원칙

`InstrumentView`는 UI reference 묶음이다.

```text
MolyAldVisualState
    -> UpdateInstruments()
    -> InstrumentView
       -> value/unit readout text
       -> status text
       -> range track
       -> fill image
```

공정 계산은 `MolyAldOperatorCanvas`에 넣지 않는다.
Canvas는 이미 만들어진 visual state를 표시만 한다.

## 막혔던 점

- 처음에는 instrument card에 Pressure/Temp/Film/Valve 4개 row를 넣었지만 너무 빽빽했다.
  - 해결: Valve는 event line으로 빼고, 숫자형 process variable 3개만 크게 표시했다.
- Unity `Text`의 RectTransform이 너무 작아서 numeric value가 screenshot에 보이지 않았다.
  - 해결: 숫자 font size와 RectTransform 영역을 조정했다.

## 검증

- `dotnet build EquipmentTwinLab.sln --no-restore --configuration Release`
- `dotnet run --project tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj --no-restore --configuration Release`
- `.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot -ScreenshotPath artifacts\unity-demo\moly-ald-demo-goal042.png`
- `.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot`
- `git diff --check`
- screenshot 육안 확인

검증 결과:

- Release build 통과
- Core console test 81개 통과
- Unity screenshot capture 통과
- whitespace check 통과
- Pressure 850 mTorr, Temp 250 C, Film 2 A 값이 screenshot에서 읽힘

## 남은 한계

- 정상 범위는 현재 synthetic demo 기준이다. 실제 장비 recipe range가 아니다.
- 버튼은 아직 실제 click handler와 연결되지 않았다.
- Fault selector도 아직 없다.

다음 단계:

```text
Goal 043: Canvas Button Interaction and Fault Selector
```
