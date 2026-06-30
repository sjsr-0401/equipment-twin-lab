# Goal 014: Motion Axis 모델

## 목표

실제 모션 컨트롤러 없이도 Servo On, Home, Move, InPosition, Motion Timeout, Servo Alarm 흐름을 검증할 수 있는 가상 모션 축 모델을 추가한다.

## 사용자가 말한 큰 목표에 대한 판단

사용자가 원하는 방향은 “원하는 장비를 커스텀하고, 무엇을 제조/검사할지 선택하고, 실제 장비처럼 트러블까지 발생하는 제조 Digital Twin”이다.

이 목표는 대표 프로젝트 방향으로 좋다. 다만 바로 Unity UI에서 자유 배치형 장비 빌더를 만들면 범위가 너무 커진다. 30일 안에는 아래처럼 구조화된 커스텀으로 가야 한다.

```text
정해진 장비 모듈 선택
    ↓
제품/공정/검사 조건 선택
    ↓
트러블 조건 선택
    ↓
Core 엔진이 시뮬레이션
    ↓
Unity가 상태를 시각화
```

모션 축은 이 구조의 첫 번째 공통 부품이다. 장비가 커져도 Loader, Stage, Robot, Inspector는 대부분 Servo/Home/Move/InPosition/Alarm 패턴을 공유한다.

## 한 일

- `MotionAxisState`를 추가했다.
- `MotionAxisAlarmCode`를 추가했다.
- `MotionAxisAlarm`을 추가했다.
- `MotionCommandResult`를 추가했다.
- `MotionAxis`를 추가했다.
- 모션 축 테스트 8개를 추가했다.

## 바뀐 파일

- `src/EquipmentTwin.Core/Motion/MotionAxis.cs`
- `src/EquipmentTwin.Core/Motion/MotionAxisState.cs`
- `src/EquipmentTwin.Core/Motion/MotionAxisAlarmCode.cs`
- `src/EquipmentTwin.Core/Motion/MotionAxisAlarm.cs`
- `src/EquipmentTwin.Core/Motion/MotionCommandResult.cs`
- `tests/EquipmentTwin.Core.Tests/Program.cs`

## 검증 결과

- Release 빌드 성공
- 경고 0개
- 오류 0개
- 콘솔 테스트 49개 통과
- CLI batch 시나리오 7개 통과

## 막힌 점과 해결 방법

- 현재 막힌 점 없음.

## 보류한 판단

- 아직 `EquipmentCellController` 공정 흐름에 모션 축을 연결하지 않았다.
- 아직 JSON 시나리오에서 모션 명령을 실행하지 않는다.
- 아직 속도/가속도/감속도 곡선은 모델링하지 않는다.
- 아직 Unity 화면과 연결하지 않는다.

## 소프트웨어 아키텍처 설명

```text
MotionAxis
    ↓
ServoOn()
    ↓
StartHome()
    ↓
Poll()
    ↓
StartMove()
    ↓
Poll() 또는 CheckTimeout()
    ↓
InPosition 또는 Alarmed
```

`MotionAxis`는 실제 하드웨어 드라이버가 아니다. 실제 장비 없이 모션 제어 소프트웨어의 핵심 상태 흐름을 검증하기 위한 모델이다.

시간은 `IClock`을 통해 주입한다. 테스트에서는 `ManualClock`을 사용하므로 실제로 기다리지 않고도 1초, 3초, 10초 뒤 상황을 바로 검증할 수 있다.

## 유지보수할 때 봐야 할 파일

- 모션 축 동작: `src/EquipmentTwin.Core/Motion/MotionAxis.cs`
- 모션 축 상태: `src/EquipmentTwin.Core/Motion/MotionAxisState.cs`
- 모션 알람 코드: `src/EquipmentTwin.Core/Motion/MotionAxisAlarmCode.cs`
- 테스트: `tests/EquipmentTwin.Core.Tests/Program.cs`

## 사용자가 이해해야 할 개념

모션 축은 장비 안에서 “움직이는 부품 하나”를 의미한다.

예:

- X Stage
- Z Lift
- Conveyor
- Robot Arm Axis
- Camera Focus Axis

모션 축은 보통 아래 흐름을 가진다.

```text
Servo Off
    ↓
Servo On
    ↓
Home
    ↓
Move
    ↓
InPosition
```

문제가 생기면 `Alarmed`로 간다.

```text
Move 중 시간 초과
    ↓
MoveTimeout
    ↓
Alarmed
```

## 다음 작업

다음 후보는 모션 축을 시나리오 JSON과 CLI 리포트에 연결하는 것이다. 그 다음 Equipment Template / Product Recipe / Fault Model로 확장한다.
