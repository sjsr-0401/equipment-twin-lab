# Goal 066 — WPF Demo 상태 자동 Screenshot Capture

## 목표

WPF 오퍼레이터 콘솔의 대표 상태를 매번 같은 크기와 같은 공정 상태로 재현하여 PNG로 저장한다.

## 완료 기준

- `load`, `process`, `alarm`, `transfer-out`, `complete` 5개 상태를 지원한다.
- 상태 준비에는 기존 `Reset`, `Step`, `Fault Replay` 명령을 재사용한다.
- 한국어와 영어를 선택할 수 있다.
- 알람 상태는 `알람/리포트` 탭을 열어 작업지시서가 보이게 한다.
- 결과 PNG는 모두 1600×900이어야 한다.
- 한 PowerShell 명령으로 Release build, 5개 캡처, 파일/해상도 검증을 수행한다.
- 일반 WPF 실행 동작에는 캡처 전용 설정이 적용되지 않아야 한다.
- 캡처 결과는 Git에서 제외된 `artifacts/`에 저장한다.

## 구현 결정

- 화면 캡처에는 WPF의 `RenderTargetBitmap`을 사용한다.
- 캡처할 상태는 창이 처음 렌더링되기 전에 ViewModel에 준비한다.
- 캡처 모드에서만 소프트웨어 렌더링을 사용한다.
- 실제 장비 상태를 흉내 낸 별도 값을 만들지 않고 Core timeline을 사용하는 기존 ViewModel 명령을 실행한다.

## 검증 명령

```powershell
.\scripts\Capture-WpfDemoScreenshots.ps1
```

영문 화면은 다음과 같이 만든다.

```powershell
.\scripts\Capture-WpfDemoScreenshots.ps1 -Language en
```

## 산출 위치

```text
artifacts/wpf-demo-screenshots/
```

이 폴더는 공개 Git 커밋에 포함하지 않는다.
