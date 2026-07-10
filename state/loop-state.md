# Loop State

마지막 갱신: 2026-07-01

## 현재 상태

- 프로젝트 이름: `Equipment Twin Lab`
- 저장소 폴더: `active/equipment-twin-lab`
- GitHub: `https://github.com/sjsr-0401/equipment-twin-lab`
- 단계: 장비 상태머신, 가상 IO 모델, CI, Clock/Timeout 모델, IO-상태 연결 계층, Scenario JSON Runner, Scenario CLI 실행기, CLI batch 리포트, 알람/복구 시나리오, Core 검증 정리 문서, 알람 코드 체계, Visual Studio build/debug 지원, 알람 복구 조건 구현 완료, CLI 리포트 알람/복구 조건 표시 완료, 가상 모션 축 모델 완료, 모션 Scenario JSON action 구현, Equipment Template / Product Recipe 최소 모델 구현, Template Runner 구현, Fault Model 구현, Inspection Result Model 구현, Template Runner CLI 구현, Template Run Markdown Report 구현, Template Batch Report 구현, Inspection Scenario Selection 구현, Fault Expected-Failure Report 구현
- 루프 방식: 수동 실행
- 자동화 상태: 아직 없음
- 자동 병합 상태: 금지

## 프로젝트 한 줄 설명

실제 장비가 없어도 장비 SW의 핵심 구조를 검증할 수 있도록, 가상 PLC/모션/센서/카메라/3D 셀을 연결한 제조 장비 디지털 트윈 사이드 프로젝트를 만든다.

## 사용자의 목표

- 장비 SW 엔지니어로 현업 복귀 가능한 역량을 보여준다.
- 이전 장비 SW 실무 경험을 방어 가능하게 정리한다.
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
- Goal 009 Draft PR #7을 만들고 GitHub Actions push/pull_request 성공을 확인했다.
- PR #7은 CI 성공 확인 후 main에 squash merge했다.
- Goal 010에서 Core 검증 범위와 한계를 `docs/core-validation.md`에 정리했다.
- Goal 010 Draft PR #8을 만들고 GitHub Actions push/pull_request 성공을 확인했다.
- PR #8은 CI 성공 확인 후 main에 squash merge했다.
- Goal 011에서 알람 코드와 알람 정보 모델을 추가했다.
- Goal 011 Draft PR #9를 만들고 GitHub Actions push/pull_request 성공을 확인했다.
- PR #9는 CI 성공 확인 후 main에 squash merge했다.
- Goal 012에서 Visual Studio build/debug 가이드와 launch profile을 추가했다.
- Goal 012 Draft PR #10을 만들고 GitHub Actions push/pull_request 성공을 확인했다.
- PR #10은 CI 성공 확인 후 main에 squash merge했다.
- Goal 013에서 문 열림/비상정지 알람 복구 조건을 추가했다.
- Goal 013 Draft PR #11을 만들고 GitHub Actions push/pull_request 성공을 확인했다.
- Goal 013 후속으로 CLI Markdown 리포트에 활성 알람 코드와 ClearAlarm 조건 표시를 추가했다.
- PR #11을 main에 squash merge했다.
- Goal 014에서 가상 모션 축 모델을 시작했다.
- Goal 014 Draft PR #12를 만들고 main에 병합했다.
- Goal 015에서 모션 축을 Scenario JSON action과 CLI 리포트에 연결했다.
- Goal 015 Draft PR #13을 만들고 GitHub Actions push/pull_request 성공을 확인했다.
- PR #13을 main에 squash merge했다.
- Goal 016에서 Equipment Template / Product Recipe 최소 모델을 추가했다.
- Goal 016 Draft PR #14를 만들고 GitHub Actions push/pull_request 성공을 확인했다.
- PR #14를 main에 squash merge했다.
- Goal 017에서 Template Runner를 추가했다.
- Goal 017 Draft PR #15를 만들고 GitHub Actions push/pull_request 성공을 확인했다.
- PR #15를 main에 squash merge했다.
- Goal 018에서 Fault Model을 추가했다.
- Goal 018 Draft PR #16을 만들고 GitHub Actions push/pull_request 성공을 확인했다.
- PR #16을 main에 squash merge했다.
- Goal 019에서 Inspection Result Model을 추가했다.
- Goal 019 Draft PR #17을 만들고 GitHub Actions push/pull_request 성공을 확인했다.
- PR #17을 main에 squash merge했다.
- Goal 020에서 Template Runner CLI를 추가했다.
- Goal 020 Draft PR #18을 만들고 GitHub Actions push/pull_request 성공을 확인했다.
- PR #18을 main에 squash merge했다.
- Goal 021에서 Template Run Markdown Report를 추가했다.
- Goal 021 Draft PR #19를 만들고 GitHub Actions push/pull_request 성공을 확인했다.
- PR #19를 main에 squash merge했다.
- Goal 022에서 Template Batch Report를 추가했다.
- Goal 022 Draft PR #20을 만들고 GitHub Actions push/pull_request 성공을 확인했다.
- PR #20을 main에 squash merge했다.
- Goal 023에서 Inspection Scenario Selection을 추가했다.
- Goal 023 Draft PR #21을 만들고 GitHub Actions push/pull_request 성공을 확인했다.
- PR #21을 main에 squash merge했다.
- Goal 024에서 Fault Expected-Failure Report를 추가했다.
- Goal 024 Draft PR #22를 만들고 GitHub Actions push/pull_request 성공을 확인했다.

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
| 2026-06-25 | Goal 009 PR #7 | Draft PR 생성, GitHub Actions push/pull_request 성공, main에 squash merge |
| 2026-06-26 | Goal 010: Core 검증 정리 문서 | 검증 범위, 한계, 실행 방법, 면접 설명 정리 |
| 2026-06-26 | Goal 010 PR #8 | Draft PR 생성, GitHub Actions push/pull_request 성공 |
| 2026-06-26 | PR #8 병합 | CI 성공 확인 후 main에 squash merge |
| 2026-06-26 | Goal 011: 알람 코드 체계 | AlarmCode/AlarmInfo 추가, 콘솔 테스트 34개 통과, batch 5개 통과 |
| 2026-06-26 | Goal 011 PR #9 | Draft PR 생성, GitHub Actions push/pull_request 성공 |
| 2026-06-27 | PR #9 병합 | CI 성공 확인 후 main에 squash merge |
| 2026-06-27 | Goal 012: Visual Studio build/debug 지원 | Visual Studio 가이드와 launch profile 추가 |
| 2026-06-27 | Goal 012 PR #10 | Draft PR 생성, GitHub Actions push/pull_request 성공 |
| 2026-06-28 | PR #10 병합 | CI 성공 확인 후 main에 squash merge |
| 2026-06-28 | Goal 013: 알람 복구 조건 세분화 | 문 열림/비상정지 복구 조건 추가, 테스트 41개 통과, batch 7개 통과 |
| 2026-06-28 | Goal 013 PR #11 | Draft PR 생성, GitHub Actions push/pull_request 성공 |
| 2026-06-28 | Goal 013 후속: CLI 리포트 알람/복구 조건 표시 | Active Alarm/Clear Condition 컬럼 추가, 성공 시나리오 Errors 표시 버그 수정 |
| 2026-06-30 | PR #11 병합 | 알람 복구 조건과 리포트 개선 main 반영 |
| 2026-06-30 | Goal 014: Motion Axis 모델 | Servo On/Home/Move/InPosition/Timeout/Alarm 모델 추가, 테스트 49개 통과 |
| 2026-07-01 | PR #12 병합 | Motion Axis 모델 main 반영 |
| 2026-07-01 | Goal 015: Motion Scenario Actions | 모션 JSON action과 CLI Motion Axes 리포트 추가, 테스트 51개 통과, batch 9개 통과 |
| 2026-07-01 | Goal 015 PR #13 | Draft PR 생성, GitHub Actions push/pull_request 성공 |
| 2026-07-01 | PR #13 병합 | Motion Scenario Actions main 반영 |
| 2026-07-01 | Goal 016: Equipment Template / Product Recipe | 템플릿/recipe 최소 모델 추가, 테스트 56개 통과, batch 9개 통과 |
| 2026-07-01 | Goal 016 PR #14 | Draft PR 생성, GitHub Actions push/pull_request 성공 |
| 2026-07-01 | PR #14 병합 | Equipment Template / Product Recipe main 반영 |
| 2026-07-01 | Goal 017: Template Runner | template/recipe를 모션 실행으로 변환, 테스트 60개 통과, batch 9개 통과 |
| 2026-07-01 | Goal 017 PR #15 | Draft PR 생성, GitHub Actions push/pull_request 성공 |
| 2026-07-01 | PR #15 병합 | Template Runner main 반영 |
| 2026-07-01 | Goal 018: Fault Model | MotionTimeout/ServoAlarm fault 추가, 테스트 66개 통과, batch 9개 통과 |
| 2026-07-01 | Goal 018 PR #16 | Draft PR 생성, GitHub Actions push/pull_request 성공 |
| 2026-07-01 | PR #16 병합 | Fault Model main 반영 |
| 2026-07-01 | Goal 019: Inspection Result Model | 제품 PASS/FAIL 검사 결과 모델 추가, 테스트 68개 통과, batch 9개 통과 |
| 2026-07-01 | Goal 019 PR #17 | Draft PR 생성, GitHub Actions push/pull_request 성공 |
| 2026-07-01 | PR #17 병합 | Inspection Result Model main 반영 |
| 2026-07-01 | Goal 020: Template Runner CLI | template/recipe/fault를 CLI로 실행, 테스트 68개 통과, batch 9개 통과 |
| 2026-07-01 | Goal 020 PR #18 | Draft PR 생성, GitHub Actions push/pull_request 성공 |
| 2026-07-01 | PR #18 병합 | Template Runner CLI main 반영 |
| 2026-07-01 | Goal 021: Template Run Markdown Report | template run 결과 Markdown 저장 기능 추가 |
| 2026-07-01 | Goal 021 PR #19 | Draft PR 생성, GitHub Actions push/pull_request 성공 |
| 2026-07-01 | PR #19 병합 | Template Run Markdown Report main 반영 |
| 2026-07-01 | Goal 022: Template Batch Report | 여러 recipe batch 실행과 Markdown 비교 report 추가, Draft PR #20 생성, CI 성공 |
| 2026-07-01 | PR #20 병합 | Template Batch Report main 반영 |
| 2026-07-01 | Goal 023: Inspection Scenario Selection | 같은 recipe에서 검사 케이스 선택 기능 추가, 테스트 72개 통과, Draft PR #21 생성, CI 성공 |
| 2026-07-01 | PR #21 병합 | Inspection Scenario Selection main 반영 |
| 2026-07-01 | Goal 024: Fault Expected-Failure Report | fault 실행 실패를 기대값으로 검증하는 CLI/report 추가, Draft PR #22 생성, CI 성공 |

## 열린 PR

| PR | 브랜치 | 상태 |
|---|---|---|
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/1` | `goal/002-virtual-io` | 병합 완료 |
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/2` | `goal/004-clock-timeout` | 병합 완료 |
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/3` | `goal/005-io-state-bridge` | 병합 완료 |
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/4` | `goal/006-scenario-json` | 병합 완료 |
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/5` | `goal/007-scenario-cli` | 병합 완료 |
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/6` | `goal/008-cli-batch-report` | 병합 완료 |
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/7` | `goal/009-alarm-recovery-scenarios` | 병합 완료 |
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/8` | `goal/010-core-validation-doc` | 병합 완료 |
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/9` | `goal/011-alarm-code-system` | 병합 완료 |
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/10` | `goal/012-visual-studio-debug` | 병합 완료 |
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/11` | `goal/013-alarm-recovery-conditions` | 병합 완료 |
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/12` | `goal/014-motion-axis-model` | 병합 완료 |
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/13` | `goal/015-motion-scenario-actions` | 병합 완료 |
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/14` | `goal/016-equipment-template` | 병합 완료 |
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/15` | `goal/017-template-runner` | 병합 완료 |
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/16` | `goal/018-fault-model` | 병합 완료 |
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/17` | `goal/019-inspection-result` | 병합 완료 |
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/18` | `goal/020-template-runner-cli` | 병합 완료 |
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/19` | `goal/021-template-run-report` | 병합 완료 |
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/20` | `goal/022-template-batch-report` | 병합 완료 |
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/21` | `goal/023-inspection-scenario-selection` | 병합 완료 |
| `https://github.com/sjsr-0401/equipment-twin-lab/pull/22` | `goal/024-fault-expected-failure-report` | Draft, CI 성공 |

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
| 2026-06-25 | `goal/009-alarm-recovery-scenarios` | push | 성공 |
| 2026-06-25 | `goal/009-alarm-recovery-scenarios` | pull_request | 성공 |
| 2026-06-26 | `goal/010-core-validation-doc` | push | 성공 |
| 2026-06-26 | `goal/010-core-validation-doc` | pull_request | 성공 |
| 2026-06-26 | `goal/011-alarm-code-system` | push | 성공 |
| 2026-06-26 | `goal/011-alarm-code-system` | pull_request | 성공 |
| 2026-06-27 | `goal/012-visual-studio-debug` | push | 성공 |
| 2026-06-27 | `goal/012-visual-studio-debug` | pull_request | 성공 |
| 2026-06-28 | `goal/013-alarm-recovery-conditions` | push | 성공 |
| 2026-06-28 | `goal/013-alarm-recovery-conditions` | pull_request | 성공 |
| 2026-06-30 | `goal/014-motion-axis-model` | push | 성공 |
| 2026-06-30 | `goal/014-motion-axis-model` | pull_request | 성공 |
| 2026-07-01 | `goal/015-motion-scenario-actions` | push | 성공 |
| 2026-07-01 | `goal/015-motion-scenario-actions` | pull_request | 성공 |
| 2026-07-01 | `goal/016-equipment-template` | push | 성공 |
| 2026-07-01 | `goal/016-equipment-template` | pull_request | 성공 |
| 2026-07-01 | `goal/017-template-runner` | push | 성공 |
| 2026-07-01 | `goal/017-template-runner` | pull_request | 성공 |
| 2026-07-01 | `goal/018-fault-model` | push | 성공 |
| 2026-07-01 | `goal/018-fault-model` | pull_request | 성공 |
| 2026-07-01 | `goal/019-inspection-result` | push | 성공 |
| 2026-07-01 | `goal/019-inspection-result` | pull_request | 성공 |
| 2026-07-01 | `goal/020-template-runner-cli` | push | 성공 |
| 2026-07-01 | `goal/020-template-runner-cli` | pull_request | 성공 |
| 2026-07-01 | `goal/021-template-run-report` | push | 성공 |
| 2026-07-01 | `goal/021-template-run-report` | pull_request | 성공 |
| 2026-07-01 | `goal/022-template-batch-report` | push | 성공 |
| 2026-07-01 | `goal/022-template-batch-report` | pull_request | 성공 |
| 2026-07-01 | `goal/023-inspection-scenario-selection` | push | 성공 |
| 2026-07-01 | `goal/023-inspection-scenario-selection` | pull_request | 성공 |
| 2026-07-01 | `goal/024-fault-expected-failure-report` | push | 성공 |
| 2026-07-01 | `goal/024-fault-expected-failure-report` | pull_request | 성공 |

## 다음 안전한 작업

1. PR #22를 Ready로 전환하고 병합한다.
2. `main`을 최신화한다.
3. 이후 Inspection Scenario Batch Matrix 또는 Fault Scenario Catalog로 확장한다.

## 금지할 것

- 검증 없이 “실제 장비에서 안전하다”고 표현하지 않는다.
- CI가 안정되기 전 자동 병합하지 않는다.
- Unity 화면 제작에 먼저 매몰되지 않는다.
- 이해하지 못한 코드를 포트폴리오 핵심 성과로 적지 않는다.
## 2026-07-01 Update: Goal 025 in progress

- PR #22 merged into `main`.
- Current branch: `goal/025-public-ald-process-model`.
- Goal 025 adds a public/synthetic molybdenum ALD process model.
- Local validation passed:
  - Release build
  - Core console tests
  - normal process CLI
  - pumpdown fault CLI expected failure
- 다음 필요 조치:
  - commit Goal 025
  - push branch
  - open draft PR #23
  - watch CI
## 2026-07-01 Update: Goal 025 PR created

- PR #23: `https://github.com/sjsr-0401/equipment-twin-lab/pull/23`
- Branch: `goal/025-public-ald-process-model`
- Commit: `bb92a4c feat: add public moly ald process model`
- State: Draft
- push CI: success
- pull_request CI: success

다음 추천 goal:

```text
Goal 026: Process Timeline JSON Export
```
## 2026-07-01 Update: Goal 026 in progress

- PR #23 merged into `main`.
- Current branch: `goal/026-process-timeline-json-export`.
- Goal 026 adds process timeline JSON export for Unity replay.
- Pre-existing/merge-time blocker fixed:
  - `EquipmentStateMachine.Reject()` parameter typo `prev0ious` -> `previous`.
- Local validation so far:
  - Release build success
  - Core console tests success
  - normal process report/timeline CLI success
  - pumpdown fault timeline CLI expected failure success

다음 필요 조치:

- run full CI-like validation
- commit
- push
- create draft PR #24
- watch CI
## 2026-07-01 Update: Goal 026 PR created

- PR #24: `https://github.com/sjsr-0401/equipment-twin-lab/pull/24`
- Branch: `goal/026-process-timeline-json-export`
- Commit: `c471b09 feat: add process timeline json export`
- State: Draft
- Merge state: Clean
- push CI: success
- pull_request CI: success

다음 추천 goal:

```text
Goal 027: Unity Process Player Skeleton
```

## 2026-07-01 Update: Goal 027 in progress

- PR #24 merged into `main`.
- Current branch: `goal/027-unity-process-player-skeleton`.
- Goal 027 adds first Unity-side process player skeleton.
- Unity folder: `unity/EquipmentTwin.Unity`.
- 현재 검증 경계:
  - .NET build/tests still run in CI.
  - Unity skeleton file presence is checked in CI.
  - Unity Editor compile/play is still a manual next-step validation.

다음 필요 조치:

- run full local validation
- commit
- push
- create draft PR #25
- watch CI

Completion update:

- Draft PR #25 opened: <https://github.com/sjsr-0401/equipment-twin-lab/pull/25>
- Commit: `308d167 feat: add unity process player skeleton`
- Local validation passed.
- PR CI passed on run `28501761492`.
- 다음 추천 goal: `Goal 028: Unity Chamber/Wafer/Valve Visual`.

## 2026-07-01 Update: Goal 028 in progress

- PR #25 merged into `main`.
- Current branch: `goal/028-unity-chamber-wafer-valve-visual`.
- Goal 028 adds primitive Unity visuals driven by `MolyAldProcessPlayer.CurrentStep`.
- New visual components:
  - `MolyAldPrimitiveVisualizer`
  - `MolyAldDemoBootstrap`
- 현재 검증 경계:
  - .NET build/tests still run locally and in CI.
  - Unity runtime script file presence is checked in CI.
  - Unity Editor compile/play is still manual.

다음 필요 조치:

- run local validation
- commit
- push
- create draft PR #26
- watch CI

Completion update:

- Draft PR #26 opened: <https://github.com/sjsr-0401/equipment-twin-lab/pull/26>
- Commit: `afeb2f6 feat: add unity primitive process visual`
- Local validation passed.
- PR CI passed on run `28502801252`.
- Unity batchmode compile was blocked by inactive Unity license.
- 다음 추천 goal: `Goal 029: Unity Hub license activation + Play Mode smoke test checklist`.

## 2026-07-01 Update: Goal 029 in progress

- PR #26 merged into `main`.
- Current branch: `goal/029-unity-smoke-test-harness`.
- Goal 029 adds repeatable Unity smoke-test tooling.
- New files:
  - `scripts/Invoke-UnitySmokeTest.ps1`
  - `docs/unity-smoke-test.md`
  - `unity/EquipmentTwin.Unity/Assets/EquipmentTwin/Editor/MolyAldEditorSmokeTest.cs`
  - `unity/EquipmentTwin.Unity/Assets/EquipmentTwin/Editor/EquipmentTwin.Unity.Editor.asmdef`

현재 검증 경계:

- .NET build/tests can be validated normally.
- Unity smoke-test script can be syntax/wiring checked.
- Actual Unity compile/run still requires Unity Hub license activation.

다음 필요 조치:

- run local validation
- commit
- push
- create draft PR #27
- watch CI

Completion update:

- Draft PR #27 opened: <https://github.com/sjsr-0401/equipment-twin-lab/pull/27>
- Commit: `e2b6b84 feat: add unity smoke test harness`
- Local validation passed.
- PR CI passed on run `28503702866`.
- Unity smoke-test runner reaches Unity but is blocked by inactive license.
- 다음 추천 goal: `Goal 030: Unity Hub license activation and first demo screenshot`.

## 2026-07-01 Update: Goal 030 in progress

- PR #27 merged into `main`.
- Current branch: `goal/030-unity-demo-screenshot-capture`.
- Goal 030 adds screenshot capture to the Unity smoke-test harness.
- New command:
  - `.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot`
- Default output:
  - `artifacts/unity-demo/moly-ald-demo.png`

현재 검증 경계:

- Script syntax and file wiring can be validated now.
- Actual screenshot capture requires Unity Hub license activation.

다음 필요 조치:

- run local validation
- commit
- push
- create draft PR #28
- watch CI

Completion update:

- Draft PR #28 opened: <https://github.com/sjsr-0401/equipment-twin-lab/pull/28>
- Commit: `b4cdf69 feat: add unity demo screenshot capture`
- Local validation passed.
- PR CI passed on run `28504193106`.
- Screenshot runner reaches Unity but actual PNG generation is blocked by inactive Unity license.
- 다음 추천 goal: `Goal 031: Unity license activation, real screenshot capture, README demo image`.

## 2026-07-01 Update: Goal 031 in progress

- PR #28 merged into `main`.
- Current branch: `goal/031-portfolio-demo-package`.
- Goal 031 documents the portfolio/demo story while Unity license activation remains a user-side blocker.
- New files:
  - `docs/portfolio-demo-package.md`
  - `goals/031-portfolio-demo-package.md`

현재 검증 경계:

- .NET build/tests and CLI process run can be validated normally.
- CI can verify the portfolio/demo documents exist.
- Actual Unity screenshot generation still requires Unity Hub license activation.

다음 필요 조치:

- run local validation
- commit
- push
- create draft PR
- watch CI

Local validation update:

- `git diff --check`: passed
- Release build: passed
- Core console tests: passed, 80 tests
- Public moly ALD process CLI report/timeline run: passed

Completion update:

- Draft PR #29 opened: <https://github.com/sjsr-0401/equipment-twin-lab/pull/29>
- Commit: `0354a51 docs: add portfolio demo package`
- Local validation passed.
- PR CI passed:
  - push run `28506128034`
  - pull_request run `28506142593`
- Unity screenshot generation remains blocked by inactive Unity license.
- 다음 추천 goal: `Goal 032: Unity license activation result capture and README demo image`.

## 2026-07-01 Update: Goal 032 in progress

- PR #29 merged into `main`.
- Current branch: `goal/032-unity-screenshot-readme-image`.
- Unity Hub Personal license is active.
- Unity batch screenshot now reaches compile/render.
- First real screenshot generated:
  - `artifacts/unity-demo/moly-ald-demo.png`
  - `docs/demo/moly-ald-demo.png`

Fixes made during Goal 032:

- removed runtime dependency on `JsonUtility`;
- removed IMGUI `GUI/GUILayout` HUD dependency;
- disabled `-nographics` only for screenshot capture;
- added explicit visual refresh for Editor batch screenshot;
- copied representative image into tracked docs.

다음 필요 조치:

- run local validation
- commit
- push
- create draft PR
- watch CI

Completion update:

- Draft PR #30 opened: <https://github.com/sjsr-0401/equipment-twin-lab/pull/30>
- Commit: `0afd356 feat: add verified unity demo screenshot`
- Local validation passed.
- Unity screenshot validation passed.
- PR CI passed:
  - push run `28513825158`
  - pull_request run `28513842354`
- 다음 추천 goal: `Goal 033: Unity demo polish and 3-minute recording checklist`.

## 2026-07-01 Update: Goal 033 in progress

- PR #30 merged into `main`.
- Current branch: `goal/033-unity-demo-polish-recording-checklist`.
- Goal 033 improves Unity demo readability and adds a 3-minute recording checklist.
- Visual changes:
  - equipment base plate;
  - gas lines for metal precursor, reactant, and purge;
  - title label;
  - tighter demo camera framing/FOV.
- Script fix:
  - `Invoke-UnitySmokeTest.ps1` now treats a blank `$LASTEXITCODE` as `0` after Unity returns successfully.
- Local validation passed:
  - `git diff --check`
  - Release build
  - Core console tests: 80 passed
  - public moly ALD process report/timeline CLI
  - Unity screenshot capture with success markers

다음 필요 조치:

- commit Goal 033
- push branch
- create draft PR
- watch CI

Completion update:

- Draft PR #31 opened: <https://github.com/sjsr-0401/equipment-twin-lab/pull/31>
- Commit: `d4b76b4 feat: polish unity demo recording flow`
- Local validation passed.
- Unity screenshot validation passed.
- PR CI passed:
  - push run `28514554674`
  - pull_request run `28514568014`
- 다음 추천 goal: `Goal 034: record the 3-minute demo video or design CAD/Blender model swap points`.

## 2026-07-01 Update: Goal 034 in progress

- PR #31 merged into `main`.
- Current branch: `goal/034-unity-visual-adapter-boundary`.
- Goal 034 adds the Unity visual adapter boundary for future CAD/Blender model swaps.
- New Unity runtime boundary:
  - `MolyAldVisualState`
  - `MolyAldVisualStateMapper`
  - `MolyAldImportedModelVisualBinding`
- `MolyAldPrimitiveVisualizer` now applies `MolyAldVisualState` instead of directly owning all timeline-to-renderer mapping.
- Documentation:
  - `docs/unity-model-swap-boundary.md`
  - `goals/034-unity-visual-adapter-boundary.md`

다음 필요 조치:

- commit Goal 034
- push branch
- create draft PR
- watch CI

Local validation update:

- `git diff --check`: passed
- Release build: passed
- Core console tests: 80 passed
- public moly ALD process report/timeline CLI: passed
- Unity screenshot capture: passed

Completion update:

- Draft PR #32 opened: <https://github.com/sjsr-0401/equipment-twin-lab/pull/32>
- Commit: `6d002b5 feat: add unity visual adapter boundary`
- Local validation passed.
- Unity screenshot validation passed.
- PR CI passed:
  - push run `28515480062`
  - pull_request run `28515494369`
- 다음 추천 goal: `Goal 035: record the actual 3-minute demo or add imported-model auto-binding after a real asset exists`.

## 2026-07-01 Update: Goal 035 in progress

- PR #32 merged into `main`.
- Current branch: `goal/035-demo-rehearsal-runner`.
- Goal 035 adds a demo rehearsal runner for 3-minute recording prep.
- New files:
  - `scripts/Invoke-PortfolioDemoRehearsal.ps1`
  - `docs/demo-rehearsal-runner.md`
  - `goals/035-demo-rehearsal-runner.md`
- The rehearsal runner validates:
  - Release build
  - Core console tests
  - normal public moly ALD process report/timeline
  - expected `pumpdown-timeout` fault
  - Unity screenshot capture unless `-SkipUnity` is used
- Local validation passed:
  - `git diff --check`
  - `.\scripts\Invoke-PortfolioDemoRehearsal.ps1 -SkipUnity`
  - `.\scripts\Invoke-PortfolioDemoRehearsal.ps1`

다음 필요 조치:

- commit Goal 035
- push branch
- create draft PR
- watch CI

Completion update:

- Draft PR #33 opened: <https://github.com/sjsr-0401/equipment-twin-lab/pull/33>
- Commit: `a1f2eec feat: add demo rehearsal runner`
- Local validation passed.
- PR CI passed:
  - push run `28516872338`
  - pull_request run `28516885953`
- 다음 추천 goal: `Goal 036: record the actual 3-minute demo and fix explanation or visual gaps`.

## 2026-07-01 Update: Goal 036 in progress

- PR #33 merged into `main`.
- Current branch: `goal/036-demo-narration-cue-cards`.
- Goal 036 adds Korean demo narration cue cards.
- New files:
  - `scripts/New-PortfolioDemoCueCards.ps1`
  - `docs/portfolio-demo-narration.md`
  - `goals/036-demo-narration-cue-cards.md`
- `Invoke-PortfolioDemoRehearsal.ps1` now creates `artifacts/demo-rehearsal/recording-cue-cards.md` after the rehearsal report.

현재 검증 경계:

- The cue card can prepare what to say during recording.
- It does not record screen/video/audio.

다음 필요 조치:

- run local validation
- commit
- push branch
- create draft PR
- watch CI

Completion update:

- Draft PR #34 opened: <https://github.com/sjsr-0401/equipment-twin-lab/pull/34>
- Commit: `bb3c59b feat: add demo narration cue cards`
- Local validation passed:
  - `git diff --check`
  - Release build
  - Core console tests: 80 passed
  - cue-card script
  - rehearsal runner with `-SkipUnity`
  - full rehearsal runner with Unity screenshot
- First PR CI failed on a fragile Korean grep check in the GitHub Actions file-presence step.
- Fix pushed: use ASCII marker `Lam/ALTUS/Halo/Halo HX` for that CI check.
- PR CI passed:
  - push run `28522333300`
  - pull_request run `28522335584`
- 다음 추천 goal: `Goal 037: record the actual 3-minute demo and fix explanation or visual gaps`.

## 2026-07-02 Update: Goal 037 in progress

- PR #34 merged into `main`.
- Current branch: `goal/037-explanatory-unity-demo-screenshot`.
- Goal 037 improves the Unity screenshot so it is understandable without a long verbal explanation.
- User feedback that triggered this goal:
  - The previous screenshot proved Unity rendered, but did not clearly show what each part meant.
- New visual elements:
  - component labels;
  - status panel;
  - color key;
  - process flow bar;
  - bottom note explaining `Core/CLI calculates, Unity replays`.
- `MolyAldVisualState` now carries `CycleCount` so the visualizer does not hardcode cycle display.
- New tracked screenshot:
  - `docs/demo/moly-ald-demo.png`

Local validation so far:

- Release build passed.
- Core console tests passed: 80 tests.
- Unity screenshot capture passed.
- Full demo rehearsal passed.

다음 필요 조치:

- update final state/log if needed;
- commit, push, open draft PR, watch CI.

Completion update:

- Draft PR #35 opened: <https://github.com/sjsr-0401/equipment-twin-lab/pull/35>
- Commit: `8ce7365 feat: improve unity demo screenshot clarity`
- Local validation passed:
  - `git diff --check`
  - Release build
  - Core console tests: 80 passed
  - Unity screenshot capture
  - full demo rehearsal
- PR CI passed:
  - push run `28563274336`
  - pull_request run `28563282879`
- 다음 추천 goal: `Goal 038: record the actual 3-minute demo and fix explanation or visual gaps`.

## 2026-07-02 Update: Goal 038 in progress

- User postponed recording because the current visual is not worth recording yet.
- Current branch: `goal/038-ald-fault-matrix-report`.
- Goal 038 adds a development-focused ALD fault matrix.
- New CLI path:
  - `process batch processes/public-moly-ald-metallization.json`
- Batch behavior:
  - run normal process once and expect `PASS`;
  - run every configured fault scenario and expect `FAIL`;
  - treat all failures as valid only when the runner ends in the expected failed state;
  - return exit code `0` only when all expectations are met.

Files being changed:

- `src/EquipmentTwin.Cli/Program.cs`
- `tests/EquipmentTwin.Core.Tests/Program.cs`
- `.github/workflows/ci.yml`
- `docs/ald-fault-matrix-report.md`
- `goals/038-ald-fault-matrix-report.md`
- `README.md`
- `docs/portfolio-demo-package.md`
- state/log files

Validation plan:

- Release build
- Core console tests
- `process batch` CLI report generation
- `git diff --check`

Local validation so far:

- Release build passed.
- Core console tests passed: 81 tests.
- `process batch` generated `artifacts/moly-ald-fault-matrix-report.md` and returned exit code 0.
- `git diff --check` passed.

완료 후 다음 추천 goal:

- Add one new synthetic ALD fault kind, or improve report readability with a simple local dashboard.

## 2026-07-02 Update: Goal 039 in progress

- Current branch: `goal/039-unity-operator-console-layout`.
- User requested stronger design quality and a layout similar to public equipment presentation:
  - equipment on the left;
  - operation interface on the right;
  - cleaner, more user-friendly visual composition.
- Goal 039 changes the Unity screenshot into a synthetic equipment console.
- It keeps the same architecture:
  - Core/CLI calculates the process;
  - Unity replays and visualizes the timeline.

New visual composition:

- left 3D equipment view:
  - cabinet;
  - load port;
  - process chamber;
  - wafer/film;
  - gas/valve panel;
  - pump/exhaust;
- right operator interface:
  - Start/Stop/Fault/Reset buttons;
  - recipe card;
  - live telemetry card;
  - alarm card;
- bottom process timeline and event summary.

Validation so far:

- Unity screenshot capture passed locally.
- `docs/demo/moly-ald-demo.png` was refreshed.
- Release build passed.
- Core console tests passed: 81 tests.
- `git diff --check` passed.

## 2026-07-02 Update: Goal 040 in progress

- Current branch: `goal/040-uiux-agent-review-system`.
- User asked how to keep a separate UI/UX agent/reviewer because the current visual still looks poor by expert standards.
- Goal 040 creates a documented review system instead of spawning a permanent background agent.

Added review system:

- `docs/design/uiux-agent-brief.md`
- `docs/design/uiux-review-template.md`
- `docs/design/uiux-review-log.md`
- `docs/design/operator-console-design-system.md`

Key decision:

- Stop growing UI text through 3D `TextMesh`.
- Move operator panel/timeline/alarm UI to Unity Canvas in the next implementation goal.

다음 추천 goal:

- `Goal 041: Canvas Operator Panel Implementation`

## 2026-07-02 Update: Goal 041 complete

- Current branch: `goal/041-canvas-operator-panel`.
- Goal 041 implements the Canvas UI direction from Goal 040.
- New runtime component:
  - `MolyAldOperatorCanvas`
- New Unity package dependency:
  - `com.unity.ugui`

Implementation notes:

- `MolyAldPrimitiveVisualizer` keeps the 3D equipment body.
- `MolyAldOperatorCanvas` owns the operator panel, telemetry, alarm card, and bottom timeline.
- `MolyAldDemoBootstrap` auto-adds the Canvas component.
- `MolyAldEditorSmokeTest` validates Canvas creation.

Local validation:

- Unity compile passed after enabling `com.unity.ugui`.
- Unity screenshot generated:
  - `artifacts/unity-demo/moly-ald-demo-goal041.png`
- `docs/demo/moly-ald-demo.png` was refreshed from the Goal 041 screenshot.
- `dotnet build EquipmentTwinLab.sln --no-restore --configuration Release` passed.
- Core console test passed: 81 tests.
- `git diff --check` passed.

Known limitation:

- Buttons are not interactive yet.
- 다음 추천 goal: `Goal 042: HMI Typography and Instrument Panel`.

## 2026-07-02 Update: Goal 042 in progress

- Current branch: `goal/042-hmi-typography-instrument-panel`.
- Goal 042 improves HMI readability before wiring button interaction.
- Main runtime file:
  - `MolyAldOperatorCanvas`

Implementation notes:

- Telemetry text was replaced with process instrument rows.
- Pressure/Temp/Film now show:
  - value/unit readout;
  - status;
  - normal range band;
  - actual value fill.
- Valve state stays in the event line to prevent instrument-card overcrowding.
- Alarm card now has priority and synthetic fault code fields.

Validation so far:

- Release build passed.
- Core console test passed: 81 tests.
- Unity screenshot capture passed:
  - `artifacts/unity-demo/moly-ald-demo-goal042.png`
- Default Unity screenshot capture passed:
  - `artifacts/unity-demo/moly-ald-demo.png`
- `docs/demo/moly-ald-demo.png` was refreshed.
- `git diff --check` passed.

Known limitation:

- Buttons are still visual only.
- 다음 추천 goal: `Goal 043: Process Schematic Main View`.

## 2026-07-02 Update: Goal 043 complete

- Current branch: `goal/043-process-schematic-main-view`.
- Goal 043 changes the left main visual from primitive 3D equipment blockout to 2D ALD process schematic.

Implementation notes:

- `MolyAldOperatorCanvas` now builds:
  - left process schematic;
  - right HMI instrument panel;
  - bottom timeline.
- Schematic elements:
  - gas delivery;
  - precursor/reactant/purge valves;
  - vacuum chamber;
  - showerhead;
  - wafer/film;
  - susceptor heater;
  - P/T tap;
  - exhaust/gate/pump path.
- Active valve and film fill are driven by `MolyAldVisualState`.

검증:

- Release build passed.
- Core console test passed: 81 tests.
- Unity screenshot capture passed:
  - `artifacts/unity-demo/moly-ald-demo-goal043.png`
- Default Unity screenshot capture passed:
  - `artifacts/unity-demo/moly-ald-demo.png`
- `docs/demo/moly-ald-demo.png` was refreshed.
- `git diff --check` passed.
- Local CI marker check passed.

Known limitation:

- Flow animation is not implemented yet.
- Button interaction and fault selector remain next.

## 2026-07-02 Update: Goal 044 complete

- Current branch: `goal/044-gas-flow-animation-fault-highlight`.
- Goal 044 adds simple Canvas pulse animation to the process schematic.

Implementation notes:

- `UpdateGasFlowPulses()` moves gas pulse markers from active valve path toward chamber/showerhead.
- `UpdateExhaustFlowPulses()` supports pump/exhaust path pulse for pump/purge/fault states.
- Showerhead gas dots pulse with the active gas color.
- Fault state can blink chamber/exhaust/gate/pump highlight.

Validation so far:

- Release build passed.
- Core console test passed: 81 tests.
- Unity screenshot capture passed after one retry.
- Default Unity screenshot capture passed.
- `docs/demo/moly-ald-demo.png` was refreshed.
- `git diff --check` passed.
- Local CI marker check passed.

Known limitation:

- Buttons are still visual only.
- Fault selector remains next.
## 2026-07-02 Loop Update: Goal 045 complete

완료한 goal:

- `Goal 045: Canvas Button Interaction and Fault Selector`

변경 내용:

- Canvas command buttons are now actual Unity `Button` components.
- `START`, `STOP`, `FAULT`, and `RESET` are connected to `MolyAldProcessPlayer`.
- `OperatorFaultActive` can force the existing alarm/fault visual path.
- `EventSystem` is automatically created for click interaction.
- Unity smoke test validates button creation and state transitions.

현재 다음 추천 작업:

- `Goal 046: Fault Mode Screenshot and Operator Action Log`

이유:

- Interaction exists now, but the portfolio artifact does not yet show the fault mode.
- Next loop should make the `normal -> fault hold -> reset` demo visible and explainable.

## 2026-07-02 Loop Update: Goal 046 complete

완료한 goal:

- `Goal 046: Fault Mode Screenshot and Operator Action Log`

변경 내용:

- HMI now has an `OPERATOR ACTION LOG` card.
- START/STOP/FAULT/RESET actions can be recorded into Canvas action rows.
- Fault screenshot generation is available through `Invoke-UnitySmokeTest.ps1 -CaptureFaultScreenshot`.
- Normal and fault screenshots are available under `docs/demo`.

현재 다음 추천 작업:

- `Goal 047: Reset Recovery Screenshot and Fault Scenario Selector`

이유:

- Normal and fault hold screens exist now.
- The next missing demo segment is reset/recovery.
- The next technical gap is replacing synthetic FAULT override with a named process fault scenario selector.

## 2026-07-02 Loop Update: Goal 047 complete

완료한 goal:

- `Goal 047: Reset Recovery Screenshot and Fault Scenario Selector`

변경 내용:

- FAULT now uses the selected public scenario name `precursor-dose-timeout`.
- Fault alarm detail and action log expose the selected scenario name.
- Recovery screenshot generation is available through `Invoke-UnitySmokeTest.ps1 -CaptureRecoveryScreenshot`.
- Normal, fault, and recovery screenshots are available under `docs/demo`.

현재 다음 추천 작업:

- `Goal 048: Fault Timeline Replay Binding`

이유:

- The HMI can now tell the operator story: normal run, named fault hold, reset recovery.
- The next technical gap is process truth: Unity should replay the actual selected fault timeline instead of only applying a synthetic hold overlay.

## 2026-07-02 Loop Update: Goal 048 complete

완료한 goal:

- `Goal 048: Fault Timeline Replay Binding`

변경 내용:

- Unity now loads selected public fault timeline JSON from `StreamingAssets/faults`.
- `precursor-dose-timeout` replay moves the player to the first failed process step.
- HMI shows `FAULT REPLAY` instead of only a synthetic hold label.
- Smoke test validates `FaultTimelineReplayActive`, scenario name match, and failed-step positioning.

현재 다음 추천 작업:

- `Goal 049: Fault Scenario Selector UI`

이유:

- Replay binding is now present.
- The next interaction gap is letting the operator choose which public fault scenario to replay from the HMI.

## 2026-07-03 Loop Update: Goal 049 complete

완료한 goal:

- `Goal 049: Fault Scenario Selector UI`

변경 내용:

- HMI now has a `FAULT SCENARIO SELECTOR` card.
- Four public fault scenarios are visible as operator-selectable chips.
- `MolyAldProcessPlayer` exposes the public fault scenario catalog and selected index.
- `MolyAldOperatorCanvas.SelectFaultScenarioForOperator(index)` changes the selected replay scenario.
- Selector changes are blocked while a replay alarm is active, so the HMI cannot show a different selected fault than the active replay timeline.
- Normal, fault, and recovery screenshots were regenerated with the new selector UI.

현재 다음 추천 작업:

- `Goal 050: Fault Recovery Procedure Panel`

이유:

- The operator can now select and replay a fault.
- The next portfolio improvement is to show what an operator should check after the alarm: alarm cause, affected subsystem, reset condition, and recovery steps.

## 2026-07-03 Loop Update: Goal 050 complete

완료한 goal:

- `Goal 050: WPF Operator Console Shell`

Direction change:

- WPF is now the main HMI/debug surface.
- Unity is retained as an optional 3D/replay viewer.
- Core remains the process truth.
- CLI remains automation/report tooling.

변경 내용:

- Added `src/EquipmentTwin.Hmi.Wpf`.
- Added a WPF operator console with process schematic, commands, fault selector, instruments, alarm card, operator log, and timeline debug table.
- WPF calls `MolyAldRunner` directly and renders `MolyAldTimelineDocument`.
- Added `scripts/Invoke-WpfHmi.ps1` and `docs/wpf-main-hmi.md`.

현재 다음 추천 작업:

- `Goal 051: WPF Alarm Recovery Procedure Panel`

이유:

- WPF can now run and replay faults.
- The next HMI value is a recovery guide that explains what an operator should check after each fault.

## 2026-07-03 Loop Update: Goal 051 complete

완료한 goal:

- `Goal 051: Fix WPF ProgressBar Binding Mode`

이 작업을 한 이유:

- Visual Studio stopped on a WPF binding exception while debugging the new WPF HMI.
- The failing property was `OperatorConsoleViewModel.TimelineProgress`.

변경 내용:

- WPF progress bars now bind to calculated ViewModel properties with explicit `Mode=OneWay`.
- A short WPF binding rule was added to `docs/wpf-main-hmi.md`.
- CI now checks that the WPF progress indicators keep the one-way binding markers.

검증:

- WPF Release build passed.
- Full solution Release build passed.
- Core tests passed: 81 tests.
- WPF startup smoke passed.

현재 다음 추천 작업:

- `Goal 052: WPF Alarm Recovery Procedure Panel`

이유:

- The WPF HMI is now startable/debuggable again.
- The next value-add is showing operator-facing recovery steps after selected public faults.

## 2026-07-03 Loop Update: Goal 052 complete

완료한 goal:

- `Goal 052: WPF UI Readability Fix`

이 작업을 한 이유:

- Manual WPF debugging exposed visible UI defects:
  - clipped `LOAD PORT` label;
  - timeline label and progress percentage too cramped;
  - default white DataGrid header/background conflict inside the dark theme.

변경 내용:

- Added explicit dark-theme styles for `DataGridColumnHeader`, `DataGridRow`, `DataGridCell`, `ComboBox`, and `ComboBoxItem`.
- Enlarged the load-port block so the label is readable.
- Replaced the timeline header layout with a two-column grid.
- Increased the bottom debug-table area.
- Added WPF UI readability rules to `docs/wpf-main-hmi.md`.

현재 다음 추천 작업:

- `Goal 053: WPF HMI Visual System R&D`

이유:

- The immediate readability bugs are fixed.
- Before adding more WPF panels, the project needs a stronger visual system for typography, spacing, color semantics, and industrial-HMI layout.

## 2026-07-03 Loop Update: Goal 053 complete

완료한 goal:

- `목표 053: WPF HMI Visual System`

방향:

- WPF를 메인 HMI/debug 화면으로 유지한다.
- `ISA-101 inspired` / `High-Performance HMI inspired` visual direction을 사용한다.
- 공식 표준 인증이나 vendor 동등성을 주장하지 않는다.

변경 내용:

- WPF palette를 colorful demo color에서 gray-base HMI token으로 옮겼다.
- 정상 운전 상태에서 큰 green/blue/yellow fill을 쓰지 않도록 조정했다.
- Red/amber는 alarm/warning 상태에 집중해서 사용한다.
- Valve state는 neutral visual state와 함께 `OPEN` / `CLOSED` text를 사용한다.
- Instrument status는 `▲ HI`, `▼ COOL`처럼 symbol과 text를 같이 사용한다.
- Alarm card는 색상만이 아니라 icon, priority, text, code를 함께 사용한다.
- `docs/design/wpf-hmi-visual-system.md`를 추가했다.

현재 다음 추천 작업:

- `목표 054: WPF HMI Instrument Trend Panel`

이유:

- WPF visual system 방향이 문서화됐고 화면에도 반영됐다.
- 다음으로 가치가 큰 HMI 기능은 pressure, temperature, film thickness의 trend 표시다.
- LiveCharts2를 추가하기 전에 단순 custom drawing으로 충분한지 먼저 확인한다.

## 2026-07-04 Loop Update: Goal 054 완료

완료한 goal:

- `목표 054: WPF 리포트 / Mock Server 안정화`

이 작업을 한 이유:

- WPF alarm guide, issue report export, server outbox, mock server, README, architecture 변경이 working tree에 많이 쌓여 있었다.
- 새 기능을 추가하기 전에 현재 WPF-to-server demo path가 실제로 깨지지 않았는지 검증할 필요가 있었다.

검증한 내용:

- 전체 solution build 통과.
- Core tests 통과.
- WPF project build 통과.
- Mock Server project build 통과.
- Mock Server가 `GET /health`에 응답.
- Mock Server가 `POST /alarm-issue-report` 수신.
- 수신 payload 파일이 `artifacts/mock-server-received/` 아래에 생성됨.

중요한 경계:

- 아직 local demo 경계다. 실제 MES, SECS/GEM, vendor server integration이 아니다.
- WPF 버튼 클릭 자동화는 보류했다. UI code는 build로 검증했고, HTTP endpoint는 실제 POST로 직접 검증했다.

커밋 전 주의:

- `unity/EquipmentTwin.Unity/ProjectSettings/PackageManagerSettings.asset`는 untracked 상태이며, 커밋 전에 포함 여부를 확인해야 한다.

현재 다음 추천 작업:

- `목표 055: WPF HMI 계측값 Trend Panel`

이유:

- Alarm/report/server 흐름이 검증됐다.
- 다음으로 가치가 큰 HMI 기능은 pressure, temperature, film thickness의 trend 표시다.
- Chart dependency를 추가하기 전에 단순 custom drawing으로 시작한다.

## 2026-07-04 Loop Update: Goal 055 완료

완료한 goal:

- `목표 055: WPF HMI 계측값 Trend Panel`

변경 내용:

- WPF `PROCESS INSTRUMENTS` 카드에 작은 `RECENT TREND` 영역을 추가했다.
- Pressure, Temp, Film이 작은 WPF `Polyline` sparkline으로 표시된다.
- Trend row는 `MolyAldTimelineDocument.Steps` 중 현재 step까지의 데이터를 사용해 생성된다.
- Chart library는 추가하지 않았다.
- `EquipmentTwin.Core`는 변경하지 않았다.

검증:

- WPF project build 통과.
- Core tests 통과.
- 전체 solution build 통과.
- WPF startup smoke에서 조기 종료 없이 유지됨.

현재 다음 추천 작업:

- `목표 056: WPF HMI 수동 화면 검수와 커밋 정리`

이유:

- UI layout 변경이므로 실제 화면 육안 검수가 필요하다.
- 커밋 전에 untracked Unity `PackageManagerSettings.asset` 파일이 공개 repo에 들어가야 하는지 결정해야 한다.

## 2026-07-05 Loop Update: Goal 056 완료

완료한 goal:

- `목표 056: WPF 화면 검수와 커밋 정리`

변경 내용:

- `.gitignore`에 `private/`, `private-notes/`를 명시했다.
- `.gitignore`에 Unity local 생성 folder인 `Library/`, `Temp/`, `Obj/`, `Logs/`, `UserSettings/`를 명시했다.
- `PackageManagerSettings.asset`를 확인했고, 개인 정보 파일이 아니라 Unity project setting으로 판단했다.
- 최근 state 문서 일부에 남아 있던 영어 설명을 한국어 중심으로 정리했다.

검증:

- 전체 solution build 통과.
- Core tests 통과.
- `git diff --check` 통과. Windows line ending 경고만 있음.
- `private-notes/`가 repo `.gitignore` 규칙으로 ignore되는 것 확인.

막힌 점:

- WPF 자동 screenshot 검수는 완료하지 못했다.
- `dotnet run` 캡처는 console window를 잡았고, `.exe` 직접 실행은 process는 살아 있으나 visible window handle을 찾지 못했다.

현재 다음 추천 작업:

- `목표 057: WPF 실제 화면 확인 후 trend panel spacing 조정`

이유:

- 자동 visual QA가 막혔으므로 사용자가 직접 WPF를 실행한 화면을 기준으로 spacing/readability를 조정하는 것이 현실적이다.

## 2026-07-08 Loop Update: Goal 057 완료

완료한 goal:

- `목표 057: WPF 읽기 전용 TextBox 바인딩 예외 수정`

변경 내용:

- `LatestServerPayloadPreviewText`를 표시하는 WPF `TextBox.Text` 바인딩에 `Mode=OneWay`를 명시했다.
- ViewModel의 읽기 전용 계산 속성에 `TextBox.Text` 기본 `TwoWay` 바인딩이 걸리면서 발생한 runtime 예외를 수정했다.
- 공개 작업 로그에는 작업 사실만 남기고, 자세한 학습 메모는 `private-notes/` 아래에 따로 작성했다.

검증:

- WPF project build 통과.
- 전체 solution build 통과.
- Core tests 통과.
- `git diff --check -- src/EquipmentTwin.Hmi.Wpf/MainWindow.xaml` 통과.

막힌 점:

- 처음 검증 때 build/test 병렬 실행으로 `obj` DLL file lock이 발생했다.
- 코드 문제가 아니라 검증 방식 문제였고, 순차 실행으로 해결했다.

현재 다음 추천 작업:

- `목표 058: WPF 실제 화면 기준 레이아웃/가독성 조정`

이유:

- 실행 예외는 수정됐다.
- 다음은 사용자가 보낸 실제 screenshot 기준으로 하단 잘림, 콤보박스 색상, 한영 라벨 혼합, 간격 문제를 정리하는 것이 맞다.

## 2026-07-10 Loop Update: Goal 058 완료

완료한 goal:

- `목표 058: WPF HMI 오른쪽 패널 탭 구조 적용`

변경 내용:

- 오른쪽 기능 패널에 WPF `TabControl`을 적용했다.
- `운전 Overview`, `알람 / 리포트`, `작업 로그` 탭으로 화면을 나눴다.
- 탭 제목은 한국어/영어 전환에 맞춰 바뀌도록 ViewModel property로 만들었다.
- 왼쪽 장비 schematic은 항상 보이도록 유지했다.

검증:

- WPF project build 통과.
- 전체 solution build 통과.
- Core tests 통과.

보류한 판단:

- 왼쪽 장비 schematic 개선은 다음 goal로 분리했다.
- 서버 전송 흐름 stepper UI도 다음 UI/UX 개선 후보로 남겼다.

현재 다음 추천 작업:

- `목표 059: 왼쪽 장비 schematic을 cabinet/module/gas-line/pump 구조로 개선`

이유:

- 오른쪽 정보 구조는 탭으로 정리됐다.
- 포트폴리오 첫인상에서 가장 약한 부분은 아직 왼쪽 장비 화면이다.

## 2026-07-10 Loop Update: Goal 059 완료

완료한 goal:

- `목표 059: WPF 왼쪽 장비 schematic 고도화`

변경 내용:

- 왼쪽 schematic을 `Load Port`, `Gas Box`, `Process Chamber`, `Exhaust Module` 구조로 재배치했다.
- 배관/연결선 역할의 shape를 추가해 장비가 연결된 시스템처럼 보이게 했다.
- chamber 내부에 showerhead, wafer/film, susceptor heater 구조를 더 명확히 표현했다.
- 알람 발생 시 chamber 전체를 빨갛게 칠하지 않고, flow/outline 중심으로 상태를 표현하도록 바꿨다.

검증:

- WPF Release build 통과.
- 전체 solution Release build 통과.
- Core tests 통과.

보류한 판단:

- 실제 CAD/3D 모델링은 하지 않았다.
- fault 종류별 특정 line 강조와 active flow animation은 다음 UI 개선 후보로 둔다.

현재 다음 추천 작업:

- `목표 060: 알람/리포트/서버 전송 stepper UI`

이유:

- 왼쪽 장비 schematic은 1차 개선됐다.
- 다음 병목은 오른쪽 알람/리포트 탭에서 사용자가 현재 진행 단계를 한눈에 파악하기 어렵다는 점이다.

## 2026-07-10 Loop Update: Goal 060 완료

완료한 goal:

- `목표 060: WPF 알람/리포트/서버 전송 Stepper UI`

변경 내용:

- `알람/리포트` 탭 상단에 6단계 workflow stepper를 추가했다.
- 단계는 알람, 체크, 대응, 리포트, 대기열, 전송으로 구성했다.
- 각 단계는 상태 text, detail, 상태 color를 가진다.
- 새 fault replay 또는 정상 timeline load 시 이전 workflow 표시 상태를 초기화한다.

검증:

- WPF Release build 통과.
- 전체 solution Release build 통과.
- Core tests 통과.

보류한 판단:

- Stepper card의 실제 화면 크기와 spacing은 screenshot 기준으로 추가 조정한다.
- Mock Server online/offline health check는 아직 추가하지 않았다.

현재 다음 추천 작업:

- `목표 061: 실제 화면 기준 stepper와 schematic 시각 QA`

이유:

- 최근 작업은 UI 구조 변경이 많다.
- 실제 화면 screenshot을 기준으로 글자 잘림, scroll 위치, 카드 밀도, schematic 균형을 조정해야 한다.

## 2026-07-10 Loop Update: Goal 061 완료

완료한 goal:

- `목표 061: WPF Mock Server 연결 상태 표시`

변경 내용:

- WPF 알람/리포트 탭의 server payload 영역에 Mock Server 연결 상태 카드를 추가했다.
- `서버 확인` 버튼으로 `http://127.0.0.1:5088/health`를 확인할 수 있게 했다.
- payload 전송 성공/실패 결과가 Mock Server 연결 상태에도 반영되도록 했다.

검증:

- WPF Release build 통과.
- 전체 solution Release build 통과.
- Core tests 통과.
- `git diff --check` 통과.

보류한 판단:

- 자동 polling은 넣지 않았다.
- 지금 단계에서는 사용자가 명시적으로 누르는 health check가 더 단순하고 설명하기 쉽다.

현재 다음 추천 작업:

- `목표 062: 실제 화면 기준 stepper/server card/schematic 시각 QA`

이유:

- 서버 상태 카드를 추가하면서 오른쪽 알람/리포트 탭 밀도가 다시 높아졌다.
- 다음은 기능 추가보다 실제 화면에서 잘림, 간격, scroll 위치를 조정하는 작업이 맞다.

## 2026-07-10 Loop Update: Goal 062 완료

완료한 goal:

- `목표 062: WPF stepper/server card/schematic 시각 QA`

변경 내용:

- WPF가 최대화 상태로 시작하도록 했다.
- 하단 debug 영역을 최소/최대 높이를 가진 비율 행으로 바꿨다.
- 왼쪽 schematic의 고정 행 높이와 chamber 여백을 줄였다.
- 알람/리포트 탭의 중첩 ScrollViewer를 제거했다.
- 긴 workflow detail과 파일 경로는 말줄임+ToolTip으로 표시한다.
- Mock Server 상태 메시지가 카드 전체 폭을 사용하도록 재배치했다.

검증:

- WPF Release build 통과.
- 전체 solution Release build 통과.
- Core tests 통과.
- `git diff --check` 통과.

막힌 점:

- Windows 화면 캡처 계층 오류로 최신 WPF screenshot 자동 캡처는 실패했다.
- 첫 build는 화면 확인용 WPF 프로세스가 DLL을 잠가 실패했고, 해당 프로세스를 종료한 뒤 재실행해 통과했다.

현재 다음 추천 작업:

- `목표 063: Mock Server 상태와 workflow 전송 단계 연결`

이유:

- 화면 밀도와 스크롤 구조는 1차 정리됐다.
- 다음에는 서버가 꺼져 있을 때 stepper 6단계가 단순 대기가 아니라 `OFFLINE` 원인을 직접 보여주도록 연결하는 것이 좋다.

## 2026-07-10 Loop Update: Goal 063 완료

완료한 goal:

- `목표 063: Mock Server 상태와 workflow 전송 단계 연결`

변경 내용:

- workflow 6번 전송 카드에 Mock Server health 상태를 연결했다.
- 연결 확인 중, OFFLINE, ONLINE, 전송 중, 전송 성공 상태를 구분한다.
- OFFLINE은 빨간색, ONLINE은 파란색, 실제 전송 성공은 초록색으로 표시한다.
- 서버 준비 전/후와 payload 준비 전/후 안내 문구를 구분했다.

검증:

- WPF Release build 통과.
- 전체 solution Release build 통과.
- Core tests 통과.
- .NET 8 상태 전이 직접 검증 통과.
- 영어/한국어 상태 표시 검증 통과.
- `git diff --check` 통과.

막힌 점과 해결:

- Windows PowerShell 5.1 reflection 검증은 .NET 8 runtime mismatch로 실패했다.
- Git에서 제외된 `net8.0-windows` 임시 검증 프로그램으로 전환해 모든 상태를 확인했다.

현재 다음 추천 작업:

- `목표 064: fault code별 schematic 진단 라인 강조`

이유:

- 알람 대응과 서버 전송 workflow는 이제 상태가 연결됐다.
- 다음에는 알람 코드가 장비 schematic의 실제 문제 영역과 연결되어야 오퍼레이터가 원인을 더 빨리 찾을 수 있다.

## 2026-07-10 Loop Update: Goal 064 완료

완료한 goal:

- `목표 064: fault code별 schematic 진단 라인 강조`

변경 내용:

- 정상/GAS-301/TMP-201/VAC-101 진단 포커스 badge를 추가했다.
- GAS-301은 Gas Box와 delivery line을 강조한다.
- TMP-201은 Chamber와 heater를 강조한다.
- VAC-101은 vacuum path와 Exhaust/Pump를 강조한다.
- 관련 없는 모듈은 중립색을 유지한다.
- 색상과 알람 코드/영역 텍스트를 함께 사용한다.

검증:

- WPF Release build 통과.
- 전체 solution Release build 통과.
- Core tests 통과.
- 정상/GAS/TMP/VAC/한글 ViewModel 상태 검증 통과.
- `git diff --check` 통과.

보류:

- Core에 없는 FILM 계열 fault는 UI에도 만들지 않았다.
- 기존 MotionAxis는 사용하지 않았다.

현재 다음 추천 작업:

- `목표 065: ALD timeline 기반 Wafer Transfer 상태 시각화`

이유:

- fault 위치는 이제 schematic에서 찾을 수 있다.
- 다음에는 Load/Process/Unload step에 따라 wafer 위치와 gate 상태가 변해야 왼쪽 화면이 공정 흐름을 더 직접적으로 보여줄 수 있다.

## 2026-07-10 Loop Update: Goal 065 완료

완료한 goal:

- `목표 065: ALD timeline 기반 Wafer Transfer 상태 시각화`

변경 내용:

- Load Port, transfer track, Process Chamber에 Wafer 상태 표시를 추가했다.
- LoadWafer/공정 중/TransferOut/Complete를 서로 다른 Wafer 위치로 표현한다.
- transfer gate OPEN/CLOSED를 색과 텍스트로 표시한다.
- 알람 중 Wafer Chamber Hold와 Gate CLOSED를 표시한다.
- Wafer 이송 경로를 Exhaust/Pump 배기 경로와 분리했다.

검증:

- WPF Release build 통과.
- 전체 solution Release build 통과.
- Core tests 통과.
- Load/PumpDown/TransferOut/Complete/Alarm/한글 상태 검증 통과.
- WPF 구현의 MotionAxis 참조 없음 확인.
- `git diff --check` 통과.

보류:

- 연속 좌표나 WPF animation은 추가하지 않았다.
- 실제 transfer robot/load lock 제어 모델은 아니다.

현재 다음 추천 작업:

- `목표 066: WPF Demo 상태 자동 Screenshot Capture`

이유:

- 왼쪽 schematic의 정적 fault 위치와 Wafer/Gate 상태가 모두 연결됐다.
- 다음에는 정상/알람/이송 상태를 반복 가능한 PNG로 생성해 실제 화면 품질을 자동 검수할 필요가 있다.

## 2026-07-10 Loop Update: Goal 066 완료

완료한 goal:

- `목표 066: WPF Demo 상태 자동 Screenshot Capture`

변경 내용:

- load/process/alarm/transfer-out/complete 5개 WPF 상태를 자동 PNG로 저장한다.
- 기존 Reset/Step/Fault Replay 명령으로 상태를 재현한다.
- 한국어/영어 캡처를 지원한다.
- 알람 캡처는 알람/리포트 탭을 열어 작업지시서를 보여준다.
- 5개 PNG의 1600×900 해상도를 PowerShell에서 자동 검증한다.
- 산출 이미지는 Git에서 제외된 `artifacts/wpf-demo-screenshots/`에 저장한다.

검증:

- WPF Release build 통과.
- 전체 solution Release build 통과.
- Core tests 통과.
- 5개 한국어 PNG 자동 생성과 1600×900 검증 통과.
- Load/Alarm/Transfer Out 이미지 직접 확인 통과.
- 일반 WPF 실행 smoke test 통과.
- `git diff --check` 통과.

막힌 점과 해결:

- 첫 알람 캡처는 상태 변경 직후 일부 Visual이 비어 저장됐다.
- 단순 250ms 대기만으로는 탭 전환 캡처가 안정화되지 않았다.
- 첫 렌더 전에 상태/탭을 준비하고 캡처 모드에서만 소프트웨어 렌더링을 적용해 해결했다.

현재 다음 추천 작업:

- `목표 067: 자동 Screenshot 기반 WPF Visual QA 2차`

이유:

- 최신 대표 화면을 반복 가능한 파일로 확보했다.
- 이제 감으로 UI를 수정하지 않고 같은 1600×900 기준에서 상태별 잘림, 번역, 강조를 비교할 수 있다.

## 2026-07-10 Loop Update: Goal 067 완료

완료한 goal:

- `목표 067: 자동 Screenshot 기반 WPF Visual QA 2차`

변경 내용:

- 160% Windows DPI에서 고정 DIP 창이 화면을 넘던 문제를 최대화 방식으로 해결했다.
- WPF 내부 bitmap 대신 실제 client 화면을 `CopyFromScreen`으로 저장한다.
- 현재 모니터 작업 영역으로 제한해 Windows 작업표시줄을 제외한다.
- 1600×900, 검정 표본, 상단/왼쪽/오른쪽/하단 네 anchor를 자동 검사한다.
- 한국어/영어 각각 5개 상태를 검증했다.

검증:

- 한국어 5종: 1600×900, 검정 표본 0/1200, anchor 4/4 통과.
- 영어 5종: 1600×900, 검정 표본 0/1200, anchor 4/4 통과.
- 전체 solution Release build 통과, 경고 0, 오류 0.
- Core tests 전체 통과.
- 일반 WPF 3초 smoke test 통과.

막힌 점과 해결:

- RenderTargetBitmap, VisualBrush, PrintWindow, 지연 증가, 초기/상태 화면 합성은 상태별 부분 누락을 완전히 막지 못했다.
- Windows가 최종 합성한 실제 화면을 캡처하는 방식으로 전환했다.

현재 다음 추천 작업:

- `목표 068: WPF 한국어 일반 UI 라벨 일관성 정리`

이유:

- 캡처 기반이 안정되어 변경 전후를 같은 조건으로 비교할 수 있다.
- 한국어 모드에 남은 일반 UI 제목과 버튼을 정리하되 Pump/Valve/Recipe/ALD 같은 도메인 용어는 유지한다.

## 2026-07-10 Loop Update: Goal 068 완료

완료한 goal:

- `목표 068: WPF 한국어 일반 UI 라벨 일관성 정리`

변경 내용:

- XAML의 고정 영어 버튼/일반 제목을 기존 ViewModel binding으로 교체했다.
- 한국어 모드에서 시작/정지/다음 Step/Fault 재현/초기화가 표시된다.
- 일반 영역 제목, 운전 상태, 계측 상태를 한국어로 표시한다.
- Load Port/Pump/Valve/Recipe/ALD/Trace 같은 도메인 용어는 유지했다.
- 번역된 상태 문자열에도 기존 색상 의미를 유지했다.

검증:

- 한국어/영어 각각 5개 Screenshot: 1600×900, 검정 표본 0/1200, anchor 4/4 통과.
- 전체 solution Release build 통과, 경고 0, 오류 0.
- Core tests 전체 통과.
- 일반 WPF 3초 smoke test 통과.
- `git diff --check` 통과.

막힌 점과 해결:

- 첫 일괄 patch는 XAML 예상 문맥이 실제 구조와 달라 적용되지 않았다.
- 파일 변경이 없음을 확인한 뒤 ViewModel/XAML을 분리해 정확한 줄만 수정했다.

현재 다음 추천 작업:

- `목표 069: 알람 작업지시서 한국어 문장 품질 정리`

이유:

- UI chrome과 상태 라벨은 정리됐다.
- 다음에는 작업자가 실제로 읽는 알람 요약/체크리스트/대응 선택지의 혼합 문장을 자연스럽게 만들어야 한다.

## 2026-07-10 Loop Update: Goal 069 완료

완료한 goal:

- `목표 069: WPF 장비 Schematic 시각 깊이 고도화`

우선순위 변경:

- 기존 추천은 알람 작업지시서 문장 정리였다.
- 사용자의 직접 피드백에 따라 포트폴리오 첫인상에 더 큰 영향을 주는 장비 화면 고도화를 먼저 수행했다.

변경 내용:

- Load Port/FOUP/Slit Valve를 모듈 형태로 개선했다.
- Gas Box를 MFC/Isolation Valve 3채널 manifold로 개선했다.
- Chamber 단면에 showerhead/process zone/wafer/susceptor/heater/plenum을 추가했다.
- 실제 Pressure/Temperature 값을 chamber에 표시했다.
- Exhaust path에 throttle valve와 vacuum pump 형상을 추가했다.
- 기존 동적 상태 Binding은 모두 유지했다.

검증:

- 한국어/영어 각각 5개 Screenshot: 1600×900, 검정 표본 0/1200, anchor 4/4 통과.
- Load/Process/GAS-301/Transfer Out 화면 직접 확인.
- 전체 solution Release build 통과, 경고 0, 오류 0.
- Core tests 전체 통과.
- 일반 WPF 3초 smoke test 통과.
- `git diff --check` 통과.

현재 다음 추천 작업:

- `목표 070: 알람 작업지시서 한국어 문장 품질 정리`

이유:

- 장비 외형은 구성요소와 연결이 읽히는 수준으로 개선됐다.
- 다음에는 작업자가 실제로 읽는 대응 문장의 품질을 개선한다.

## 2026-07-10 Loop Update: Goal 070 완료

완료한 goal:

- `목표 070: WPF 장비 공정 동작 시각화`

우선순위 변경:

- 기존 추천은 알람 작업지시서 문장 정리였다.
- 사용자가 대표 화면을 최종 품질까지 높이도록 요청해 장비 animation을 먼저 수행했다.

변경 내용:

- Gas/Vacuum flow animation 추가.
- Valve pulse, Heater glow, Pump rotor rotation 추가.
- Wafer transfer와 Gate motion 추가.
- Alarm 중 정상 공정 animation 정지.
- Core 상태를 ViewModel 의미 속성으로 변환하고 XAML Storyboard에 연결했다.

검증:

- 한국어/영어 각각 5개 Screenshot 검증 통과.
- 전체 solution Release build 경고 0, 오류 0.
- Core tests 전체 통과.
- WPF smoke test 통과.
- `git diff --check` 통과.

현재 다음 추천 작업:

- `목표 071: 알람 작업지시서 한국어 문장 품질 정리`

이유:

- 장비 외형과 공정 동작 시각화가 연결됐다.
- 다음에는 작업자가 실제로 읽는 대응 문장의 품질을 높인다.
