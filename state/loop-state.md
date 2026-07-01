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
- Next required action:
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

Next recommended goal:

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

Next required action:

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

Next recommended goal:

```text
Goal 027: Unity Process Player Skeleton
```

## 2026-07-01 Update: Goal 027 in progress

- PR #24 merged into `main`.
- Current branch: `goal/027-unity-process-player-skeleton`.
- Goal 027 adds first Unity-side process player skeleton.
- Unity folder: `unity/EquipmentTwin.Unity`.
- Current validation boundary:
  - .NET build/tests still run in CI.
  - Unity skeleton file presence is checked in CI.
  - Unity Editor compile/play is still a manual next-step validation.

Next required action:

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
- Next recommended goal: `Goal 028: Unity Chamber/Wafer/Valve Visual`.

## 2026-07-01 Update: Goal 028 in progress

- PR #25 merged into `main`.
- Current branch: `goal/028-unity-chamber-wafer-valve-visual`.
- Goal 028 adds primitive Unity visuals driven by `MolyAldProcessPlayer.CurrentStep`.
- New visual components:
  - `MolyAldPrimitiveVisualizer`
  - `MolyAldDemoBootstrap`
- Current validation boundary:
  - .NET build/tests still run locally and in CI.
  - Unity runtime script file presence is checked in CI.
  - Unity Editor compile/play is still manual.

Next required action:

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
- Next recommended goal: `Goal 029: Unity Hub license activation + Play Mode smoke test checklist`.

## 2026-07-01 Update: Goal 029 in progress

- PR #26 merged into `main`.
- Current branch: `goal/029-unity-smoke-test-harness`.
- Goal 029 adds repeatable Unity smoke-test tooling.
- New files:
  - `scripts/Invoke-UnitySmokeTest.ps1`
  - `docs/unity-smoke-test.md`
  - `unity/EquipmentTwin.Unity/Assets/EquipmentTwin/Editor/MolyAldEditorSmokeTest.cs`
  - `unity/EquipmentTwin.Unity/Assets/EquipmentTwin/Editor/EquipmentTwin.Unity.Editor.asmdef`

Current validation boundary:

- .NET build/tests can be validated normally.
- Unity smoke-test script can be syntax/wiring checked.
- Actual Unity compile/run still requires Unity Hub license activation.

Next required action:

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
- Next recommended goal: `Goal 030: Unity Hub license activation and first demo screenshot`.

## 2026-07-01 Update: Goal 030 in progress

- PR #27 merged into `main`.
- Current branch: `goal/030-unity-demo-screenshot-capture`.
- Goal 030 adds screenshot capture to the Unity smoke-test harness.
- New command:
  - `.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot`
- Default output:
  - `artifacts/unity-demo/moly-ald-demo.png`

Current validation boundary:

- Script syntax and file wiring can be validated now.
- Actual screenshot capture requires Unity Hub license activation.

Next required action:

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
- Next recommended goal: `Goal 031: Unity license activation, real screenshot capture, README demo image`.

## 2026-07-01 Update: Goal 031 in progress

- PR #28 merged into `main`.
- Current branch: `goal/031-portfolio-demo-package`.
- Goal 031 documents the portfolio/demo story while Unity license activation remains a user-side blocker.
- New files:
  - `docs/portfolio-demo-package.md`
  - `goals/031-portfolio-demo-package.md`

Current validation boundary:

- .NET build/tests and CLI process run can be validated normally.
- CI can verify the portfolio/demo documents exist.
- Actual Unity screenshot generation still requires Unity Hub license activation.

Next required action:

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
- Next recommended goal: `Goal 032: Unity license activation result capture and README demo image`.
