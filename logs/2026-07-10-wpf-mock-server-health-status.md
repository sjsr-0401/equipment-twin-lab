# 2026-07-10 — WPF Mock Server 연결 상태 표시

## 1. 목표

사용자가 알람 리포트를 서버로 전송하기 전에 Mock Server가 켜져 있는지 HMI에서 확인할 수 있게 했다.

## 2. 바뀐 파일

- `src/EquipmentTwin.Hmi.Wpf/Services/MockServerPayloadSender.cs`
- `src/EquipmentTwin.Hmi.Wpf/ViewModels/OperatorConsoleViewModel.cs`
- `src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml`
- `goals/061-wpf-mock-server-health-status.md`
- `logs/2026-07-10-wpf-mock-server-health-status.md`
- `state/loop-state.md`
- `state/triage.md`

## 3. 검증 결과

- WPF Release build 통과.
- 전체 solution Release build 통과.
- Core tests 통과.
- `git diff --check` 통과.

## 4. 막힘 점과 해결 방법

큰 막힘은 없었다.

기존 Mock Server에 이미 `/health` endpoint가 있었기 때문에, 새 API를 만들지 않고 WPF에서 그 endpoint를 호출하는 방식으로 해결했다.

## 5. 보류한 판단

- 서버 상태를 주기적으로 자동 polling하지 않았다.
- 실제 운영 장비라면 주기적 health check, retry, timeout 정책이 필요하지만 지금은 데모 HMI이므로 수동 확인 버튼으로 충분하다고 판단했다.

## 6. 소프트웨어/아키텍처 설명

이번 작업은 기존 책임 분리를 유지했다.

- `MockServerPayloadSender`: HTTP 통신 담당
- `OperatorConsoleViewModel`: 서버 상태를 화면에 표시할 상태값 관리
- `MainWindow.xaml`: 상태 카드와 버튼 배치

즉, XAML에서 직접 HTTP를 호출하지 않는다. 화면은 ViewModel의 속성을 보여주고, 실제 통신은 Service가 맡는다.

## 7. 유지보수할 때 봐야 할 파일

- 서버 endpoint를 바꿀 때: `MockServerPayloadSender.cs`
- 서버 상태 문구/색상을 바꿀 때: `OperatorConsoleViewModel.cs`
- 화면 위치/간격을 바꿀 때: `MainWindow.xaml`

## 8. 사용자가 이해해야 할 개념

- `health check`: 서버가 살아 있는지 확인하는 아주 작은 요청이다.
- `ViewModel`: 화면에 보여줄 상태와 버튼 동작을 가진 코드다.
- `Service`: 파일 저장, HTTP 전송처럼 화면과 분리된 실제 작업을 담당하는 코드다.

## 9. 다음 작업

- 실제 화면 기준으로 Mock Server 상태 카드의 크기와 간격을 확인한다.
- 필요하면 서버 상태를 상단 title bar나 workflow stepper에도 연결한다.

