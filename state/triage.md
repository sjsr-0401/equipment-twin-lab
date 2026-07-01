# Triage

이 파일은 “오늘 무엇을 할지”를 고르는 곳이다.

## 우선순위 규칙

- P0: 프로젝트 진행을 막는 결정 또는 고장
- P1: 대표 프로젝트 MVP에 직접 필요한 작업
- P2: 포트폴리오 설명력을 높이는 문서/정리
- P3: 있으면 좋지만 지금 당장 필요하지 않은 개선

## 현재 후보 작업

| 우선순위 | 작업 | 이유 | 상태 |
|---|---|---|---|
| P0 | 대표 저장소 이름 결정 | 이후 GitHub, 문서, 자동화 기준점이 된다 | 완료 |
| P0 | 자동화 권한 결정 | 어디까지 자동으로 해도 되는지 경계가 필요하다 | 부분 완료 |
| P1 | 프로젝트 뼈대 생성 | 실제 코드 작업의 시작점 | 완료 |
| P1 | 장비 상태 모델 초안 작성 | 장비 SW 핵심을 보여주는 중심축 | 완료 |
| P1 | 가상 PLC/IO 인터페이스 설계 | 제조 장비 실무성과 직접 연결된다 | 완료 |
| P1 | GitHub 원격 저장소 추가 | 공개 포트폴리오 저장소가 필요하다 | 완료 |
| P1 | CI 추가 | 매일 작업을 안전하게 검증하려면 필요하다 | 완료 |
| P1 | Clock/Timeout 모델 | 완료 신호가 지연되는 상황을 테스트하기 위해 필요하다 | 완료 |
| P1 | 상태머신과 IO 연결 | 센서 입력으로 장비 이벤트를 만들기 위해 필요하다 | 완료 |
| P1 | 공정 시나리오 JSON | 반복 가능한 장비 운전 시나리오가 필요하다 | 완료 |
| P1 | Scenario CLI 실행기 | 사용자가 JSON 시나리오를 직접 실행해볼 수 있어야 한다 | 완료 |
| P1 | CLI batch 실행/리포트 | 여러 시나리오를 한 번에 검증하고 결과를 저장할 수 있어야 한다 | 완료 |
| P1 | 알람/복구 시나리오 추가 | Door open, Emergency stop, Clear alarm 흐름을 검증해야 한다 | 완료 |
| P1 | 알람 코드 체계 | 알람 원인과 복구 조건을 포트폴리오/현업 기준으로 설명하려면 코드 체계가 필요하다 | 완료 |
| P1 | 알람 복구 조건 세분화 | 원인이 제거되기 전에는 ClearAlarm이 거부되어야 한다 | 완료 |
| P1 | CLI 리포트 알람/복구 조건 표시 | batch 결과에서 알람 코드와 ClearAlarm 가능 여부가 보여야 리뷰가 쉽다 | 완료 |
| P1 | Motion Axis 모델 | 사용자 커스텀 장비의 공통 모션 부품이 필요하다 | 완료 |
| P1 | Motion Scenario JSON action | 축 동작을 코드가 아니라 시나리오 파일로 실행해야 Unity/CLI/자동화가 공유할 수 있다 | 완료 |
| P1 | Equipment Template / Product Recipe | 축, IO, 검사, fault 조건을 사용자가 고르는 장비 구성으로 묶어야 한다 | 완료 |
| P1 | Template Runner | 선택한 template/recipe를 실제 실행 계획으로 바꿔야 한다 | 완료 |
| P1 | Fault Model | 사용자가 트러블 조건을 선택할 수 있게 해야 한다 | 완료 |
| P1 | Inspection Result Model | 제품별 검사 결과를 PASS/FAIL로 표현해야 한다 | 완료 |
| P1 | Template Runner CLI | 사용자가 template/recipe/fault를 명령어로 실행하고 결과를 봐야 한다 | 완료 |
| P1 | Template Run Markdown Report | template 실행 결과를 포트폴리오용 파일로 저장해야 한다 | 완료 |
| P1 | Template Batch Report | 여러 recipe 실행 결과를 한 번에 비교해야 한다 | 진행 중 |
| P1 | Fault Expected-Failure Report | fault 주입처럼 실패가 기대값인 케이스를 안전하게 검증해야 한다 | 대기 |
| P1 | Inspection Scenario Selection | 같은 제품에서 PASS/FAIL 검사 케이스를 선택할 수 있어야 한다 | 대기 |
| P2 | Core 검증 정리 문서 | 면접/포트폴리오에서 현재 구조를 설명할 수 있어야 한다 | 완료 |
| P2 | RayWork 경험과 새 프로젝트 연결 문서 작성 | 면접에서 “실무 경험 → 사이드 프로젝트”로 설명 가능해진다 | 대기 |
| P2 | Visual Studio build/debug 가이드 | 사용자가 직접 breakpoint를 걸고 Core 흐름을 이해할 수 있어야 한다 | 완료 |
| P2 | 매일 작업 로그 템플릿 정착 | 사용자가 나중에 복기하기 쉬워진다 | 진행 중 |

## 오늘의 추천 작업

1. Goal 022 변경을 커밋하고 Draft PR을 만든다.
2. CI 결과를 확인한다.
3. 다음 후보로 Inspection Scenario Selection 또는 Fault Expected-Failure Report를 선택한다.

## 보류 작업

- 실제 자동 스케줄 등록
- 자동 PR 생성
- 자동 병합
- Unity 에셋 제작
- CAD/Blender 모델 적용
- MES 연동 어댑터 (`IMesGateway`): 장비 상태/생산이력/알람을 상위 시스템에 발행하는 인터페이스. SECS/GEM은 풀구현이 아니라 이벤트 보고 개념만 일반화. MVP의 비전 검사(Phase 4)로 생산 이력이 실제로 쌓이기 시작한 뒤 Goal로 분리. "MES 구축"이 아니라 "장비-MES 연동 계층"으로 정직하게 포지셔닝.

위 항목들은 프로젝트 골격과 검증 루프가 생긴 뒤 진행한다.
