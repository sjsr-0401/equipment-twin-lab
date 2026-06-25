# Equipment Twin Lab

실제 장비가 없어도 장비 SW의 핵심 구조를 검증할 수 있게 만드는 제조 장비 디지털 트윈 프로젝트다.

## 목표

- 장비 상태 전이, PLC/IO, 모션, 센서, 카메라 검사를 소프트웨어로 모델링한다.
- Unity 3D 시뮬레이터는 나중에 붙이고, 핵심 장비 로직은 C#/.NET으로 먼저 검증한다.
- 작업 결과를 초보자도 이해할 수 있게 문서화한다.

## 현재 단계

첫 MVP는 장비 상태머신이다.

```text
Idle → Loading → Aligning → Inspecting → Unloading → Complete
```

알람 상황에서는 어떤 단계에서든 `Alarmed` 상태로 전환된다.

## 프로젝트 구조

```text
src/EquipmentTwin.Core
  장비 핵심 로직

tests/EquipmentTwin.Core.Tests
  외부 테스트 패키지 없이 실행하는 간단한 테스트 러너

state/
  현재 진행 상태와 다음 작업

goals/
  Goal 단위 작업 정의

logs/
  일일 작업 로그
```

## 실행 방법

```powershell
dotnet restore --ignore-failed-sources
dotnet build --no-restore
dotnet run --project tests\EquipmentTwin.Core.Tests --no-restore
```
