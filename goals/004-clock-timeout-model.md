# Goal 004: Clock / Timeout 모델

작성일: 2026-06-25

## 목표

실제 장비나 실제 대기 시간 없이도 장비 상태별 Timeout을 테스트할 수 있는 시간 모델을 만든다.

## 왜 필요한가

제조 장비 SW에서는 “정해진 시간 안에 센서 입력이나 완료 신호가 오지 않는 상황”을 반드시 처리해야 한다.

예를 들어 Loading 상태에서 자재 감지 신호가 오지 않거나, Aligning 상태에서 정렬 완료 신호가 오지 않으면 장비는 계속 기다리면 안 되고 알람으로 빠져야 한다.

## 범위

포함:

- `IClock` 인터페이스
- 실제 시간용 `SystemClock`
- 테스트/시뮬레이션용 `ManualClock`
- 상태별 Timeout 정책
- Timeout 검사 결과 모델
- 상태머신의 Timeout 알람 전환
- 테스트 추가

제외:

- 실제 PLC 타이머
- 실제 장비 안전 회로
- 상태머신과 IO 자동 연결
- Unity 시간 시스템 연결

## 위험 등급

- [ ] L0 문서/자동 검증 설정
- [x] L1 테스트/샘플/작은 리팩터링
- [x] L2 핵심 구조/상태머신/검사 로직
- [ ] L3 안전, 인터록, 알람 우선순위, 데이터 삭제

현재 구현은 시뮬레이션과 테스트를 위한 Timeout 모델이다. 실제 장비 안전성을 보장하지 않는다.

## 완료 기준

- 수동 Clock으로 시간을 전진시킬 수 있다.
- 상태 진입 시간이 기록된다.
- Timeout 전에는 상태가 유지된다.
- Timeout 시간이 지나면 `Alarmed` 상태로 전환된다.
- Timeout 이력이 상태 전이 기록에 남는다.
- 테스트가 통과한다.

## 검증 방법

```powershell
dotnet build --no-restore
dotnet run --project tests\EquipmentTwin.Core.Tests --no-restore
```
