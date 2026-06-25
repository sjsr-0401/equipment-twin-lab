# Goal 002: 가상 IO 모델

작성일: 2026-06-25

## 목표

실제 PLC 없이도 장비 SW가 입력 센서와 출력 명령을 구분해서 다룰 수 있도록 가상 IO 모델을 만든다.

## 왜 필요한가

제조 장비 SW는 PLC/IO 신호와 계속 상호작용한다.

실제 장비가 없어도 IO 모델이 있으면 센서 입력, 출력 명령, 알람 조건을 시뮬레이션에서 검증할 수 있다.

## 범위

포함:

- Input/Output 방향 구분
- 기본 장비 셀 IO 맵
- 장비 SW가 Output을 쓰는 기능
- 시뮬레이터가 Input을 바꾸는 기능
- 잘못된 방향 접근 거부
- IO 변경 이력
- 테스트 추가

제외:

- 실제 PLC 통신
- Modbus/EtherCAT/Melsec 프로토콜
- 상태머신과 IO의 자동 연결
- Unity 화면 표시

## 위험 등급

- [x] L1 테스트/샘플/작은 리팩터링
- [ ] L2 핵심 구조/상태머신/검사 로직
- [ ] L3 안전, 인터록, 알람 우선순위, 데이터 삭제

현재 구현은 실제 장비 IO 제어가 아니라 시뮬레이션용 IO 모델이다.

## 완료 기준

- 기본 IO 맵이 있다.
- Input과 Output이 구분된다.
- 장비 SW는 Output만 쓸 수 있다.
- 시뮬레이터는 Input만 바꿀 수 있다.
- 잘못된 방향 접근은 실패한다.
- 테스트가 통과한다.

## 검증 방법

```powershell
dotnet build --no-restore
dotnet run --project tests\EquipmentTwin.Core.Tests --no-restore
```
