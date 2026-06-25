# Goal 001: 장비 상태머신 MVP

작성일: 2026-06-25

## 목표

실제 장비가 없어도 제조 장비 SW의 기본 공정 흐름을 검증할 수 있도록 장비 상태머신을 만든다.

## 왜 필요한가

장비 SW는 화면보다 먼저 “장비가 지금 어떤 상태인지”를 정확히 관리해야 한다.

상태머신이 있어야 PLC/IO, 모션, 센서, 카메라 검사, 알람 처리를 같은 기준으로 연결할 수 있다.

## 범위

포함:

- 기본 공정 상태
- 정상 상태 전이
- 잘못된 전이 거부
- DoorOpened, EmergencyStop 알람 전이
- 간단한 테스트 러너

제외:

- 실제 PLC 통신
- 실제 모션 제어
- 실제 카메라 검사
- Unity 3D 화면

## 위험 등급

- [x] L1 테스트/샘플/작은 리팩터링
- [ ] L2 핵심 구조/상태머신/검사 로직
- [ ] L3 안전, 인터록, 알람 우선순위, 데이터 삭제

현재 구현은 실제 장비 안전 로직이 아니라 시뮬레이션 MVP다.

## 완료 기준

- 상태머신 코드가 있다.
- 정상 시퀀스가 Complete까지 간다.
- 잘못된 이벤트가 상태를 바꾸지 않는다.
- DoorOpened/EmergencyStop이 Alarmed 상태로 전환된다.
- 테스트 러너가 통과한다.

## 검증 방법

```powershell
dotnet restore --ignore-failed-sources
dotnet build --no-restore
dotnet run --project tests\EquipmentTwin.Core.Tests --no-restore
```
