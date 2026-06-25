# Goal 005: 상태머신 + IO 연결

작성일: 2026-06-25

## 목표

가상 IO 입력을 읽어서 장비 상태머신 이벤트로 변환하는 연결 계층을 만든다.

## 왜 필요한가

장비 SW는 단순히 상태머신만 있거나 IO 목록만 있다고 실무적으로 보이지 않는다.

실제 장비에서는 PLC/센서 입력이 들어오고, 그 입력에 따라 장비 상태가 바뀐다. 또한 상태가 바뀌면 진공, 스테이지 이동 요청, 램프, 부저 같은 출력이 바뀐다.

## 범위

포함:

- `EquipmentCellController`
- `EquipmentCellStepResult`
- IO 입력 기반 정상 상태 전이
- DoorOpened/EmergencyStop 안전 입력 우선 처리
- Timeout 정책 연결
- 상태별 기본 출력 동기화
- UnloadComplete 입력 추가
- 테스트 추가

제외:

- 실제 PLC 통신
- 실제 모션 명령
- 실제 안전 회로
- Unity 화면
- 공정 시나리오 JSON

## 위험 등급

- [ ] L0 문서/자동 검증 설정
- [x] L1 테스트/샘플/작은 리팩터링
- [x] L2 핵심 구조/상태머신/IO 연결 로직
- [ ] L3 안전, 인터록, 알람 우선순위, 데이터 삭제

현재 구현은 시뮬레이션용 연결 계층이다. 실제 장비의 안전 로직을 대체하지 않는다.

## 완료 기준

- StartCycle이 Idle에서 Loading으로 전환된다.
- LoadPresent 입력이 LoadComplete 이벤트로 변환된다.
- AlignmentDone 입력이 AlignmentComplete 이벤트로 변환된다.
- InspectionDone 입력이 InspectionComplete 이벤트로 변환된다.
- UnloadComplete 입력이 UnloadComplete 이벤트로 변환된다.
- DoorOpened/EmergencyStop이 정상 공정 입력보다 우선 처리된다.
- Timeout 정책이 연결 계층을 통해 Alarmed 상태로 전환된다.
- 상태별 출력이 동기화된다.
- 테스트가 통과한다.

## 검증 방법

```powershell
dotnet build --no-restore
dotnet run --project tests\EquipmentTwin.Core.Tests --no-restore
```
