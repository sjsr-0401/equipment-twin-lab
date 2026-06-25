# Loop State

마지막 갱신: 2026-06-25

## 현재 상태

- 프로젝트 이름: `Equipment Twin Lab`
- 저장소 폴더: `active/equipment-twin-lab`
- GitHub: `https://github.com/sjsr-0401/equipment-twin-lab`
- 단계: 장비 상태머신, 가상 IO 모델, CI, Clock/Timeout 모델, IO-상태 연결 계층, Scenario JSON Runner, Scenario CLI 실행기, CLI batch 리포트, 알람/복구 시나리오 구현 완료
- 루프 방식: 수동 실행
- 자동화 상태: 아직 없음
- 자동 병합 상태: 금지

## 프로젝트 한 줄 설명

실제 장비가 없어도 장비 SW의 핵심 구조를 검증할 수 있도록, 가상 PLC/모션/센서/카메라/3D 셀을 연결한 제조 장비 디지털 트윈 사이드 프로젝트를 만든다.

## 사용자의 목표

- 장비 SW 엔지니어로 현업 복귀 가능한 역량을 보여준다.
- RayWork에서 했던 실무 경험을 방어 가능하게 정리한다.
- 사이드 프로젝트를 제조 공정 관점에서 실용적인 대표 프로젝트로 만든다.
- 매일 작업 내용을 초보자도 이해할 수 있게 문서화한다.

## 현재까지 확정한 방향

- 대표 저장소 이름은 `equipment-twin-lab`으로 둔다.
- Unity를 3D 가상 장비 시뮬레이터 후보로 둔다.
- 핵심 로직은 Unity 화면에 종속시키지 않고 C#/.NET 쪽에 둔다.
- 카메라는 `실제 카메라`, `데이터셋 카메라`, `Unity 가상 카메라`를 같은 인터페이스로 교체 가능하게 설계한다.
- CAD/Blender 모델이 나중에 생기면 외형만 교체할 수 있게 `동작 로직`과 `외형 모델`을 분리한다.
- 자동화는 처음부터 완전 자동이 아니라, 문서화 → 검증 → 저위험 자동화 순서로 도입한다.
- 첫 구현은 C#/.NET 8 기반 장비 상태머신으로 시작한다.
- 현재 정상 시퀀스와 알람 전이 테스트가 통과한다.
- 공개 GitHub 저장소를 생성했고 `main` 브랜치에 초기 커밋을 푸시했다.
- Goal 002에서 가상 IO 모델을 추가했다.
- 가상 IO는 Input/Output 방향을 구분하고 잘못된 방향 접근을 거부한다.
- Goal 003에서 GitHub Actions CI를 추가했다.
- CI는 push/PR마다 restore, build, console test를 실행한다.
- PR #1은 CI 성공 후 main에 squash merge했다.
- Goal 004에서 Clock/Timeout 모델을 추가했다.
- ManualClock으로 실제 대기 없이 Timeout 상황을 테스트할 수 있다.
- PR #2는 CI 성공 후 main에 squash merge했다.
- Goal 005에서 가상 IO 입력을 상태머신 이벤트로 변환하는 연결 계층을 추가했다.
- 앞으로 모든 작업로그에는 막힌 점, 보류한 판단, 아키텍처 설명, 유지보수 포인트를 필수로 남긴다.
- 현재 코드 구조는 `docs/architecture.md`에 정리한다.
- PR #3은 CI 성공 후 main에 squash merge했다.
- Goal 006에서 JSON 시나리오 모델과 ScenarioRunner를 추가했다.
- PR #4는 CI 성공 후 main에 squash merge했다.
- Goal 007에서 JSON 시나리오를 명령어로 실행하는 CLI를 추가했다.
- PR #5는 CI 성공 후 main에 squash merge했다.
- Goal 008에서 여러 시나리오를 한 번에 실행하고 Markdown 리포트를 저장하는 batch 기능을 추가했다.
- PR #6은 CI 성공 확인 후 main에 병합했다.
- Goal 009에서 문 열림, 비상정지, ClearAlarm 복구 시나리오를 추가했다.
- Goal 009 로컬 검증 결과 Release 빌드 성공, 테스트 30개 통과, batch 시나리오 5개 통과를 확인했다.

## 아직 확정하지 않은 것

- 자동 Triage 실행 시간
- Goal 하나당 최대 반복 횟수
- 자동 병합 허용 범위
- Unity 버전
- CLI batch filter/tag 방식
- Timeout 이후 복구 절차 상세 설계

## 완료한 작업

| 날짜 | Goal | 결과 |
|---|---|---|
| 2026-06-25 | Goal 001: 장비 상태머신 MVP | 빌드 성공, 콘솔 테스트 5개 통과 |
| 2026-06-25 | Goal 002: 가상 IO 모델 | 빌드 성공, 콘솔 테스트 11개 통과, Draft PR #1 생성 |
| 2026-06-25 | Goal 003: GitHub Actions CI | 로컬 빌드/테스트 통과, GitHub Actions push/PR 실행 성공 |
| 2026-06-25 | PR #1 병합 | CI 성공 확인 후 main에 squash merge |
| 2026-06-25 | Goal 004: Clock/Timeout 모델 | 로컬 빌드 성공, 콘솔 테스트 17개 통과, Draft PR #2 생성, CI 성공 |
| 2026-06-25 | PR #2 병합 | CI 성공 확인 후 main에 squash merge |
| 2026-06-25 | Goal 005: 상태머신 + IO 연결 | 로컬 빌드 성공, 콘솔 테스트 23개 통과, Draft PR #3 생성, CI 성공 |
| 2026-06-25 | PR #3 병합 | CI 성공 확인 후 main에 squash merge |
| 2026-06-25 | Goal 006: 공정 시나리오 JSON Runner | 로컬 빌드 성공, 콘솔 테스트 27개 통과, Draft PR #4 생성, CI 성공 |
| 2026-06-25 | PR #4 병합 | CI 성공 확인 후 main에 squash merge |
| 2026-06-25 | Goal 007: Scenario CLI 실행기 | 로컬 빌드 성공, 콘솔 테스트 27개 통과, CLI 시나리오 2개 실행 성공, Draft PR #5 생성, CI 성공 |
| 2026-06-25 | PR #5 병합 | CI 성공 확인 후 main에 squash merge |
| 2026-06-25 | Goal 008: CLI batch 실행 + Markdown 리포트 | 로컬 빌드 성공, 콘솔 테스트 27개 통과, batch 실행과 리포트 생성 성공, Draft PR #6 생성, CI 성공 |
| 2026-06-25 | PR #6 병합 | CI 성공 확인 후 main에 squash merge |
| 2026-06-25 | Goal 009: 알람/복구 시나리오 | 문 열림, 비상정지, ClearAlarm 복구 시나리오 추가, 로컬 테스트 30개 통과, batch 5개 통과 |

## 열린 PR

| PR | 브랜치 | 상태 |
|---|---|---|
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/1` | `goal/002-virtual-io` | 병합 완료 |
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/2` | `goal/004-clock-timeout` | 병합 완료 |
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/3` | `goal/005-io-state-bridge` | 병합 완료 |
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/4` | `goal/006-scenario-json` | 병합 완료 |
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/5` | `goal/007-scenario-cli` | 병합 완료 |
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/6` | `goal/008-cli-batch-report` | 병합 완료 |

## 최근 CI 결과

| 날짜 | 브랜치 | 이벤트 | 결과 |
|---|---|---|---|
| 2026-06-25 | `goal/002-virtual-io` | push | 성공 |
| 2026-06-25 | `goal/002-virtual-io` | pull_request | 성공 |
| 2026-06-25 | `goal/004-clock-timeout` | push | 성공 |
| 2026-06-25 | `goal/004-clock-timeout` | pull_request | 성공 |
| 2026-06-25 | `goal/005-io-state-bridge` | push | 성공 |
| 2026-06-25 | `goal/005-io-state-bridge` | pull_request | 성공 |
| 2026-06-25 | `goal/006-scenario-json` | push | 성공 |
| 2026-06-25 | `goal/006-scenario-json` | pull_request | 성공 |
| 2026-06-25 | `goal/007-scenario-cli` | push | 성공 |
| 2026-06-25 | `goal/007-scenario-cli` | pull_request | 성공 |
| 2026-06-25 | `goal/008-cli-batch-report` | push | 성공 |
| 2026-06-25 | `goal/008-cli-batch-report` | pull_request | 성공 |

## 다음 안전한 작업

1. Goal 009 PR을 만들고 CI를 확인한다.
2. CI 통과 후 PR #7 병합 여부를 결정한다.
3. 다음 후보는 Core 검증 정리 문서 또는 알람 코드 체계다.

## 금지할 것

- 검증 없이 “실제 장비에서 안전하다”고 표현하지 않는다.
- CI가 안정되기 전 자동 병합하지 않는다.
- Unity 화면 제작에 먼저 매몰되지 않는다.
- 이해하지 못한 코드를 포트폴리오 핵심 성과로 적지 않는다.
