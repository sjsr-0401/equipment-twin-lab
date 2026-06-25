# Equipment Twin Lab 개발 계획

> 상태: 초안 v0.7 — Clock/Timeout 모델 추가
> 작성일: 2026-06-25  
> 프로젝트 성격: 장비 SW 엔지니어 대표 포트폴리오  
> 제안 저장소명: `equipment-twin-lab`

## 1. 프로젝트 한 줄 정의

실제 장비가 없어도 PLC·모션·센서·비전·인터록이 포함된 제조 장비 SW를 빌드하고 검증할 수 있는 Unity 기반 3D 가상 시운전(Virtual Commissioning) 플랫폼을 만든다.

## 2. 이 프로젝트를 만드는 이유

- 장비가 없어도 장비 제어 SW의 정상 시퀀스와 장애 대응을 반복 검증한다.
- RayWork에서 쌓은 실무 경험을 회사 코드를 공개하지 않고 일반화해 보여준다.
- 기존 `YOLO26`, `IndustrialCommHub`, `SmartDetector`, `TradeWork`의 강점을 하나의 제조 시스템으로 연결한다.
- 단순 3D 애니메이션이 아니라 실제 제어 SW와 통신하는 Software-in-the-Loop 환경을 만든다.
- 매일 작업 내용을 초보자도 다시 이해할 수 있도록 문서화해 소프트웨어 감각을 복원한다.

## 3. 핵심 원칙

1. **동작과 외형을 분리한다.** 3D 모델을 교체해도 제어 로직은 바뀌지 않아야 한다.
2. **Scene을 손으로 반복 편집하지 않는다.** JSON과 생성 코드로 장비를 구성한다.
3. **실제 장비와 가상 장비가 같은 인터페이스를 사용한다.**
4. **동일한 입력은 동일한 결과를 만든다.** 난수 시드와 시뮬레이션 시간을 통제한다.
5. **고장 상황을 정상 상황만큼 중요하게 다룬다.**
6. **테스트되지 않은 기능은 완료로 표시하지 않는다.**
7. **RayWork 또는 회사 소유 코드를 복사하지 않는다.** 개념과 경험만 일반화한다.
8. **매일 작업 종료 시 초보자용 설명을 남긴다.**

## 4. 1차 목표 장비

비전 검사 및 불량 분류 셀을 첫 번째 가상 장비로 구현한다.

```text
투입 컨베이어
  → 투입 감지센서
  → 스토퍼 실린더
  → XY 검사 스테이지
  → 가상 카메라 및 AI 검사
  → PASS/FAIL 분류 실린더
  → 배출 컨베이어
```

### 포함 장치

- 컨베이어 2개
- XY 모션축
- 스토퍼 및 분류 실린더
- 제품 감지센서 3개
- Home·Positive Limit·Negative Limit 센서
- 안전 도어와 비상정지
- 데이터셋 카메라와 Unity 가상 카메라
- 가상 PLC 메모리와 IO 모니터
- 알람 및 생산 이력

## 5. 목표 구조

```text
Equipment Control SW
        ↕ Modbus TCP 또는 내부 테스트 어댑터
Virtual PLC / Motion Controller
        ↕ IO·명령·현재 위치
Unity 3D Equipment Simulator
        ↕ 촬영 요청·검사 결과
Vision Inspection Service
        ↕ 생산 결과
Trace / Dashboard
```

### 제안 디렉터리

```text
equipment-twin-lab/
├─ src/
│  ├─ Equipment.Core/          # 공정 상태 머신, 알람, 인터록, 레시피
│  ├─ Equipment.Contracts/     # PLC 주소, 명령, 이벤트, 공용 DTO
│  ├─ VirtualController/       # 가상 PLC, 모션, IO, 센서
│  ├─ Inspection.Service/      # DatasetCamera, UnityCamera, 향후 RealCamera
│  └─ Simulator3D/             # Unity 프로젝트
├─ tests/
│  ├─ Equipment.Core.Tests/
│  ├─ VirtualController.Tests/
│  └─ Scenario.Tests/
├─ configs/
│  ├─ machine.json             # 장비 구조와 위치
│  ├─ io-map.json              # PLC·IO 주소
│  └─ scenarios/               # 정상·고장 시나리오
├─ docs/
│  ├─ daily/                   # 매일 작업 기록
│  ├─ decisions/               # 중요한 기술 결정
│  ├─ architecture.md
│  ├─ glossary.md
│  └─ current-task.md
├─ scripts/                    # 빌드·테스트·Scene 생성 자동화
├─ README.md
└─ plan.md
```

## 6. 주요 모듈

### Equipment.Core

- 장비 공정 상태 머신
- Start·Stop·Reset·Home 명령
- 타임아웃과 재시도
- 인터록과 안전 정지
- 알람 발생·해제·이력
- 레시피와 공정 파라미터

```text
PowerOff → Initializing → Homing → Ready
        → Loading → Positioning → Inspecting
        → Sorting → Unloading → Complete
                              ↘ Alarm
```

### VirtualController

- PLC Bit·Word 메모리
- 가상 모션 위치·속도·가속도
- Servo On·Home·InPosition·Alarm 상태
- 센서 강제 On·Off
- 통신 지연·단절
- 결정적인 시뮬레이션 Clock

### Simulator3D

- JSON 기반 장비 자동 생성
- 이동축과 액추에이터 표시
- Collider·Trigger 기반 센서
- 제품 이동과 충돌 표시
- IO·알람·공정 상태 UI
- Fault Injection 조작 화면
- 향후 CAD·Blender 외형 교체용 `VisualRoot`

### Inspection.Service

```text
ICamera
├─ DatasetCamera   # 기존 이미지와 정답 데이터 사용
├─ UnityCamera     # RenderTexture로 합성 이미지 생성
└─ RealCamera      # 실제 카메라가 생겼을 때 추가
```

- 촬영 트리거
- 이미지와 Lot·Recipe·Timestamp 연결
- YOLO 추론
- PASS·FAIL 판정
- 정답과 결과 비교
- 추론 실패·타임아웃 처리

## 7. 1차 완료 기준(MVP)

- [ ] 새 PC에서 문서대로 프로젝트를 실행할 수 있다.
- [ ] JSON으로 3D 검사 셀을 자동 생성한다.
- [ ] 장비 제어 SW와 가상 PLC가 통신한다.
- [ ] Home부터 제품 배출까지 정상 공정이 자동 실행된다.
- [ ] DatasetCamera로 PASS·FAIL 검사를 수행한다.
- [ ] 검사 결과가 PLC 응답과 생산 이력에 반영된다.
- [ ] 최소 5개 고장 시나리오를 재현한다.
- [ ] 비상정지 시 모든 위험 출력과 모션이 정지한다.
- [ ] 자동 시나리오 테스트가 통과한다.
- [ ] 초보자용 실행 방법과 구조 설명이 있다.
- [ ] 3분 이내 데모 영상으로 핵심 가치를 설명할 수 있다.

## 8. 초기 고장 시나리오

- [ ] 투입센서가 들어오지 않아 Loading Timeout 발생
- [ ] X축 Home 실패
- [ ] 모션 이동 중 Servo Alarm 발생
- [ ] 안전 도어가 열려 공정 시작 차단
- [ ] 검사 카메라 응답 Timeout
- [ ] AI 검사 NG 후 불량 배출
- [ ] PLC 통신 단절 후 안전 정지
- [ ] 비상정지 후 Reset·Recovery
- [ ] 제품 걸림 및 배출센서 Timeout

각 시나리오는 다음을 자동 검증한다.

- 예상한 알람 코드가 발생했는가?
- 모션과 위험 출력이 안전하게 정지했는가?
- 잘못된 다음 공정으로 진행하지 않았는가?
- 로그에 원인과 시간이 기록됐는가?
- Reset 후 복구 가능한가?

## 9. 개발 단계

### Phase 0 — 범위 확정과 저장소 준비

- [ ] 프로젝트명 확정
- [ ] 공개 저장소 생성
- [ ] Unity 설치 버전 확정
- [ ] .NET 솔루션과 Unity 프로젝트 기본 구조 생성
- [ ] Git·Unity 메타파일·텍스트 직렬화 설정
- [ ] README·plan·current-task 템플릿 생성

완료 기준: 빈 프로젝트가 빌드되고 기본 테스트가 실행된다.

### Phase 1 — 장비 제어 코어

- [ ] 장비 상태와 명령 정의
- [ ] 공정 상태 머신 구현
- [ ] 알람·인터록 모델 구현
- [ ] 가상 Clock과 Timeout 구현
- [ ] 정상 공정 단위 테스트

완료 기준: 3D 없이도 테스트만으로 정상 공정이 끝까지 통과한다.

### Phase 2 — 가상 PLC·모션·IO

- [ ] PLC 메모리 맵 구현
- [ ] XY축 위치·속도·Home 모델 구현
- [ ] 실린더와 센서 상태 구현
- [ ] Modbus TCP 또는 테스트 어댑터 연결
- [ ] 통신 지연·단절 시뮬레이션

완료 기준: 제어 SW가 가상 장치를 실제 장치처럼 제어한다.

### Phase 3 — Unity 3D 검사 셀

- [ ] 장비 Primitive 모델 생성
- [ ] `machine.json` 기반 Scene 생성기
- [ ] 컨베이어·축·실린더 애니메이션
- [ ] 센서 Trigger 연결
- [ ] 상태·IO·알람 UI
- [ ] 카메라와 기본 조명

완료 기준: 정상 공정이 3D 화면에서 상태와 일치해 움직인다.

### Phase 4 — 데이터 기반 가상 검사

- [ ] `ICamera` 인터페이스
- [ ] DatasetCamera
- [ ] 이미지 정답 메타데이터
- [ ] YOLO26 추론 연결
- [ ] PASS·FAIL 및 PLC 응답
- [ ] 클래스 매핑·경로 문제 수정
- [ ] 모델과 데이터 버전 기록

완료 기준: 고정 테스트셋에서 검사 결과와 정답을 자동 비교한다.

### Phase 5 — Unity 가상 카메라

- [ ] RenderTexture 촬영
- [ ] 제품과 결함의 자동 생성
- [ ] Bounding Box·Mask·정답 자동 생성
- [ ] 조명·각도·노이즈·블러 다양화
- [ ] 난수 시드 기반 재현

완료 기준: 동일 시드에서 동일 이미지와 정답을 다시 생성한다.

### Phase 6 — Fault Injection과 자동 테스트

- [ ] 고장 조작 패널
- [ ] 초기 고장 시나리오 구현
- [ ] 시나리오 JSON 실행기
- [ ] 결과 Report 생성
- [ ] Record·Replay

완료 기준: 정상 및 고장 테스트가 자동 실행되고 결과가 문서로 남는다.

### Phase 7 — 외형 교체와 포트폴리오 마감

- [ ] CAD·Blender 모델용 `VisualRoot` 규칙
- [ ] FBX·glTF 외형 교체 검증
- [ ] 구조도와 시퀀스 다이어그램
- [ ] 성능·테스트 결과 정리
- [ ] 데모 영상과 포트폴리오 페이지
- [ ] 면접용 예상 질문과 설명

완료 기준: 회사 코드를 공개하지 않고도 설계·검증 능력을 설명할 수 있다.

## 10. 첫 번째 8일 작업 사이클

근무일에는 하루 약 1시간, 휴무일에는 집중 작업을 기준으로 한다.

### 근무 1일차

- 프로젝트명과 MVP 범위 검토
- 불필요한 기능 삭제
- 기술 선택 확정

### 근무 2일차

- 저장소와 솔루션 기본 구조 생성
- `Equipment.Contracts` 생성

### 근무 3일차

- 장비 상태·명령·알람 코드 정의
- 초보자용 용어집 시작

### 근무 4일차

- 첫 상태 머신 테스트 작성
- 작업 기록과 다음 작업 정리

### 휴무 1일차

- 회복과 계획 검토
- 최대 1시간 문서 정리

### 휴무 2일차

- `Equipment.Core` 상태 머신 구현
- 정상 공정 테스트

### 휴무 3일차

- `VirtualController`의 가상 IO·모션축 구현
- 타임아웃 테스트

### 휴무 4일차

- 첫 통합 테스트
- 8일 회고
- 다음 사이클 작업 선정

## 11. 매일 작업 방식

### 시작할 때

1. `plan.md`를 읽는다.
2. `docs/current-task.md`를 읽는다.
3. 마지막 커밋과 미완료 테스트를 확인한다.
4. 오늘 끝낼 수 있는 작업 하나를 선택한다.
5. 완료 기준을 먼저 한 문장으로 적는다.

### 끝낼 때

1. 빌드와 관련 테스트를 실행한다.
2. 변경된 파일을 확인한다.
3. `docs/daily/YYYY-MM-DD.md`를 작성한다.
4. `docs/current-task.md`에 다음 작업을 남긴다.
5. 의미 있는 단위로 커밋하고 PR을 만든다.
6. 무엇을 했는지 사용자에게 쉬운 말로 보고한다.

## 12. 매일 설명 문서 형식

```markdown
# YYYY-MM-DD 작업 기록

## 오늘 한 일
한 문장으로 설명

## 왜 필요한가
이 기능이 장비 공정에서 해결하는 문제

## 동작 흐름
입력 → 처리 → 출력

## 변경한 파일
- 파일명: 변경 이유

## 오늘 사용한 프로그래밍 개념
- 상태 머신:
- 인터페이스:
- 비동기 처리:

## 테스트 결과
- 실행한 명령
- 성공·실패
- 확인한 시나리오

## 문제가 있었던 부분
증상, 원인, 해결 방법

## 직접 확인하는 방법
초보자도 따라 할 수 있는 실행 순서

## 다음 작업
다음에 바로 시작할 한 가지
```

## 13. Codex·Claude Cowork 협업 규칙

### 공통 규칙

- 작업 시작 전에 `plan.md`와 `docs/current-task.md`를 읽는다.
- 한 번에 하나의 명확한 작업만 수행한다.
- 기존 작업을 덮어쓰기 전에 Git 상태를 확인한다.
- 같은 Unity Scene을 두 에이전트가 동시에 직접 수정하지 않는다.
- Scene은 가능한 한 생성 코드와 JSON으로 관리한다.
- 하드코딩된 사용자 경로를 추가하지 않는다.
- 예외를 조용히 무시하거나 가짜 성공값을 만들지 않는다.
- 작업 후 반드시 빌드·테스트·문서를 갱신한다.
- 사용자에게 전문용어만 나열하지 말고 쉬운 의미도 설명한다.

### 작업 인계 형식

```markdown
## Current Task
- 목표:
- 담당:
- 브랜치:
- 수정 파일:
- 완료된 내용:
- 테스트 결과:
- 남은 문제:
- 다음 행동:
```

### 충돌 방지

- 기능마다 `feature/...`, 수정마다 `fix/...`, 문서마다 `docs/...` 브랜치를 사용한다.
- 다른 에이전트가 진행 중인 파일은 먼저 확인한다.
- 큰 작업은 Contracts → Core → Simulator → UI 순으로 나눈다.
- `.unity` 파일 수동 편집보다 `SceneGenerator` 수정을 우선한다.

## 14. Git 운영 규칙

- 기본 브랜치: `main`
- 커밋 형식: Conventional Commits

```text
feat: add virtual x-axis motion model
fix: stop conveyor when entry sensor times out
test: add emergency stop recovery scenario
docs: explain plc handshake for beginners
```

- 첫 단계에서는 자동 병합하지 않는다.
- 빌드·테스트가 안정된 후 문서와 저위험 변경부터 자동 병합한다.
- 기능 변경은 PR에서 테스트 결과와 작업 문서를 확인한다.
- `main`은 항상 실행 가능한 상태를 유지한다.

## 15. 작업 완료 정의

기능은 다음 조건을 모두 만족해야 완료다.

- [ ] 요구한 동작이 구현됐다.
- [ ] 정상 시나리오 테스트가 있다.
- [ ] 주요 실패 시나리오 테스트가 있다.
- [ ] 빌드가 성공한다.
- [ ] 새로운 경고를 만들지 않는다.
- [ ] 하드코딩된 개인 경로가 없다.
- [ ] 로그에서 원인과 결과를 확인할 수 있다.
- [ ] 초보자용 작업 문서가 있다.
- [ ] 다음 작업이 명확히 기록됐다.

## 16. 품질 기준

- Core 로직은 Unity 없이 테스트 가능해야 한다.
- UI와 공정 로직을 분리한다.
- 모션과 장치에는 인터페이스를 둔다.
- 모든 장치 명령은 Timeout과 Cancellation을 지원한다.
- 안전 인터록은 일반 공정 명령보다 우선한다.
- 검사 결과에는 모델·데이터·레시피 버전을 기록한다.
- 시뮬레이션 시간과 실제 시간을 분리한다.
- 모델 실패를 임의의 결과나 휴리스틱으로 숨기지 않는다.
- 실측하지 않은 성능을 실제 장비 성능처럼 표현하지 않는다.

## 17. 현재 제외할 범위

- 실제 안전 인증
- 실제 서보 튜닝과 진동 해석
- 정밀 광학 시뮬레이션
- 완전한 MES·SECS/GEM 구현
- 사실적인 전체 공장 모델
- 모바일 앱
- 멀티플레이 및 클라우드 서비스
- 초반부터 정교한 CAD 모델 제작
- RayWork 소스 또는 회사 자산의 재사용·공개

## 18. 주요 위험과 대응

| 위험 | 대응 |
|---|---|
| 프로젝트 범위가 계속 커짐 | MVP 외 기능은 Backlog로 이동 |
| Unity Scene 충돌 | JSON·생성 코드 중심으로 운영 |
| 물리 결과가 매번 달라짐 | 축은 수학 모델, 물리는 보조로 사용 |
| 합성 이미지와 실제 이미지 차이 | 실제 성능 주장 금지, 향후 실제 데이터로 검증 |
| 제어 로직이 Unity에 종속됨 | Core를 별도 .NET 라이브러리로 유지 |
| 테스트 없이 화면만 발전함 | 기능보다 테스트를 먼저 작성 |
| 회사 코드와 유사해짐 | 명칭·주소·프로토콜·구조를 일반화 |
| 여러 에이전트 작업 충돌 | current-task와 브랜치로 소유권 표시 |

## 19. 검토가 필요한 결정

아래 항목은 사용자가 첨삭하고 확정한다.

- [ ] 프로젝트명: `Equipment Twin Lab` 유지 여부
- [ ] 첫 장비: 비전 검사·불량 분류 셀 유지 여부
- [ ] 3D 엔진: Unity 유지 여부
- [ ] 통신: Modbus TCP 우선 여부
- [ ] 첫 카메라: DatasetCamera 우선 여부
- [ ] 검사 모델: 기존 YOLO26을 정리해 사용할지 여부
- [ ] 공개 GitHub 저장소로 시작할지 여부
- [ ] 생산 대시보드를 MVP에 포함할지 이후로 미룰지 여부
- [ ] CAD 외형 교체를 MVP 이후로 미룰지 여부
- [ ] 자동 Triage 실행 시간
- [ ] 자동 병합을 허용할 위험 등급
- [ ] 한 Goal의 최대 반복 횟수와 비용 한도

## 20. 다음 행동

1. 사용자가 이 계획에서 필요 없는 범위를 삭제한다.
2. 빠진 장비 기능과 공정 요구사항을 추가한다.
3. 검토가 필요한 결정을 확정한다.
4. Loop Engineering의 자동화 범위와 중단 조건을 확정한다.
5. 확정 후 Phase 0 저장소와 기본 구조를 생성한다.

## 21. Loop Engineering 운영 모델

### 21.1 한 줄 정의

사용자가 매번 다음 프롬프트를 작성하는 대신, Goal·상태·검증·중단 조건을 저장소에 정의하고 Codex와 Claude Cowork가 완료 조건을 만족할 때까지 정해진 절차를 반복하도록 한다.

이 프로젝트에서 Loop Engineering의 목적은 **사람의 이해를 제거하는 자동화**가 아니라 **반복 작업을 자동화하면서 엔지니어의 이해와 승인 지점을 보존하는 것**이다.

### 21.2 프로젝트 루프

```text
Goal 등록
  ↓
저장소 상태·CI·Backlog Triage
  ↓
오늘 수행할 작은 Task 하나 선택
  ↓
브랜치·Worktree 격리
  ↓
구현 에이전트 작업
  ↓
빌드·테스트·시나리오 검증
  ↓
독립 리뷰 에이전트 검토
  ↓
초보자용 설명과 State 갱신
  ↓
PR 생성
  ↓
위험도별 자동 또는 사용자 승인 병합
  ↓
Goal 완료 판정 또는 다음 반복
```

루프는 한 번에 기능 하나만 처리한다. “장비 전체 완성”처럼 큰 Goal은 직접 구현하지 않고 작은 검증 가능한 Task로 나눈다.

### 21.3 디스크에 남길 State와 Memory

대화 기록은 보조 자료이고 저장소 파일을 공식 상태로 사용한다.

```text
agent/
├─ goals/
│  ├─ active/                  # 진행 중 Goal
│  ├─ completed/               # 완료된 Goal
│  └─ blocked/                 # 사용자 판단이 필요한 Goal
├─ skills/
│  ├─ equipment-domain/SKILL.md
│  ├─ unity-scene/SKILL.md
│  ├─ virtual-plc/SKILL.md
│  ├─ validation/SKILL.md
│  └─ beginner-docs/SKILL.md
└─ state/
   ├─ loop-state.md            # 현재 루프 위치
   ├─ triage.md                # 발견된 작업과 우선순위
   ├─ decisions-needed.md      # 사용자 판단 대기
   └─ comprehension.md         # 사용자가 이해해야 할 내용
```

기존 문서와 역할은 다음과 같다.

| 파일 | 역할 |
|---|---|
| `plan.md` | 전체 범위와 장기 방향 |
| `docs/current-task.md` | 사람이 읽는 현재 작업 인계서 |
| `agent/state/loop-state.md` | 자동 루프의 현재 단계와 마지막 결과 |
| `agent/state/triage.md` | 후보 작업·우선순위·근거 |
| `agent/goals/active/*.md` | Goal과 완료 조건 |
| `docs/daily/*.md` | 매일 초보자용 작업 설명 |
| `docs/decisions/*.md` | 구조적 결정과 이유 |

### 21.4 Goal 파일 형식

```markdown
# GOAL-0001: 가상 X축 Homing 구현

## 목적
가상 X축이 Home 센서를 사용해 원점을 찾고 Ready 상태가 되도록 한다.

## 사용자 가치
실제 장비 없이 Homing 정상·실패 시퀀스를 검증할 수 있다.

## 허용 범위
- Equipment.Core
- VirtualController
- 관련 테스트와 문서

## 제외 범위
- Unity 외형 변경
- 실제 모션 드라이버

## 완료 조건
- 정상 Homing 테스트 통과
- Home 센서 Timeout 테스트 통과
- Limit 센서 진입 시 안전 정지 테스트 통과
- 새로운 빌드 경고 없음
- 초보자용 설명 문서 작성

## 위험 등급
L2

## 반복 제한
- 최대 구현·수정 반복: 5회
- 같은 실패가 3회 반복되면 중단

## 최종 증거
- 테스트 이름과 실행 결과
- 변경 파일
- PR 링크
- 사용자가 이해해야 할 핵심 3개
```

### 21.5 Triage 루프

Triage는 코드를 수정하지 않는 읽기 전용 루프다.

매일 다음을 확인한다.

1. 열린 Goal과 PR
2. 실패한 CI와 테스트
3. 마지막 커밋 이후 변경
4. `docs/current-task.md`의 다음 행동
5. Blocked 상태와 사용자 결정 대기
6. TODO·FIXME·새로운 경고
7. 문서와 실제 코드의 불일치

Triage 결과는 작업을 억지로 만들지 않는다. 가치 있는 변경이 없으면 “오늘 자동 구현 없음”을 정상 결과로 기록한다.

우선순위 기준은 다음과 같다.

```text
안전 오류·데이터 손상
  > 실패한 테스트·빌드
  > 현재 Goal을 막는 문제
  > MVP 필수 기능
  > 문서와 코드 불일치
  > 리팩터링·외형 개선
```

### 21.6 Worktree 운영

동시에 여러 작업이 필요한 경우 각 작업을 독립된 Git Worktree에 둔다.

```text
main repo
├─ worktree/goal-0001-homing
├─ worktree/goal-0002-conveyor
└─ worktree/review-0001
```

규칙:

- Goal 하나당 브랜치와 Worktree 하나
- 동일 파일을 수정할 Goal은 병렬 실행 금지
- Unity Scene 직접 편집 작업은 항상 단독 실행
- Contracts가 바뀌는 작업은 하위 작업보다 먼저 병합
- Worktree 결과는 테스트와 PR 없이 main에 직접 반영하지 않음
- 병합 후 Worktree 정리는 자동화 가능

초기 MVP에서는 병렬 Worktree를 최대 2개로 제한한다. 에이전트 수를 늘리는 것보다 충돌과 검증 비용을 줄이는 것이 우선이다.

### 21.7 프로젝트 Skills

각 Skill은 반복 설명이 필요한 지식을 글과 체크리스트로 고정한다.

#### equipment-domain

- 장비 상태 머신 규칙
- PLC Handshake
- Servo On·Home·InPosition·Alarm
- 안전 인터록 우선순위
- Timeout·Reset·Recovery 원칙

#### unity-scene

- JSON 기반 Scene 생성
- `VisualRoot` 외형 교체 규칙
- 좌표계·단위·Pivot 규칙
- Collider·Trigger·Sensor 규칙
- `.unity` 파일 충돌 방지

#### virtual-plc

- PLC 메모리 맵
- Bit·Word 읽기와 쓰기
- 통신 지연·단절
- 실제 시간과 시뮬레이션 시간 분리

#### validation

- 정상·실패 테스트 필수 조건
- 결정적인 난수 시드
- Fault Injection 검증
- 로그·알람·안전 정지 확인

#### beginner-docs

- 전문용어를 쉬운 말로 다시 설명
- 변경 전·후 비교
- 코드 흐름 설명
- 사용자가 직접 확인할 방법
- 이해 확인 질문 생성

Codex의 `AGENTS.md`와 Claude의 프로젝트 지침에는 이 Skill 문서들을 작업 전 읽도록 연결한다. 특정 제품의 숨겨진 대화 기억보다 저장소에 버전 관리되는 지식을 우선한다.

### 21.8 에이전트 역할 분리

모든 작업에 여러 에이전트를 사용하지 않는다. 위험하거나 구조적인 변경에만 독립 검증자를 둔다.

| 역할 | 책임 |
|---|---|
| Triage Agent | 후보 작업 수집·우선순위·중복 확인, 코드 수정 금지 |
| Implementer | 허용된 파일 안에서 구현·테스트·기술 문서 작성 |
| Reviewer | 요구사항·안전·테스트·회귀·복잡도 검토, 구현자의 설명을 그대로 신뢰하지 않음 |
| Explainer | 사용자에게 초보자 언어로 설명하고 이해 확인 질문 작성 |
| Main Agent | 결과 통합, 위험도 판단, PR·State·사용자 보고 |

다음 경우에는 Reviewer를 반드시 분리한다.

- 장비 상태 머신 변경
- 인터록·비상정지·알람 변경
- PLC 주소나 Handshake 변경
- 병렬 처리·Cancellation·Timeout 변경
- 검사 PASS·FAIL 판정 변경
- 데이터 손실 가능성이 있는 변경

단순 문서 오탈자와 저위험 UI 문구에는 별도 Reviewer를 사용하지 않는다.

### 21.9 Connector와 Plugin 범위

초기에는 GitHub 연결만 사용한다.

- Issue·Goal 연결
- PR 생성과 리뷰
- CI 상태 확인
- Commit·Branch·Merge

Linear·Slack·클라우드 DB 같은 추가 Connector는 실제 필요가 생길 때만 도입한다. 도구를 많이 연결하는 것이 Loop 품질을 보장하지 않는다.

### 21.10 위험 등급과 자동화 권한

| 등급 | 예시 | 자동 구현 | 자동 PR | 자동 병합 |
|---|---|---:|---:|---:|
| L0 | 문서, 오탈자, 주석 | 가능 | 가능 | CI 통과 시 가능 |
| L1 | 테스트 추가, 로그 개선, 생성 코드 | 가능 | 가능 | 초기에는 사용자 승인 |
| L2 | Core 기능, 통신, 모션 모델, 비전 파이프라인 | 가능 | 가능 | 사용자 승인 필수 |
| L3 | 인터록, 비상정지, 알람 우선순위, 데이터 삭제 | 초안만 가능 | Draft만 가능 | 자동 병합 금지 |

자동 병합은 MVP의 CI와 테스트가 안정된 뒤 L0부터 단계적으로 활성화한다.

### 21.11 Done 판정

구현 에이전트의 “완료” 발언은 증거가 아니다. 다음 증거를 별도로 확인한다.

```text
Goal 완료 조건 충족
AND 관련 빌드 성공
AND 정상 테스트 통과
AND 실패 시나리오 테스트 통과
AND Reviewer의 Blocker 없음
AND 작업 설명·State 갱신
AND 위험도에 맞는 사용자 승인
```

Goal 완료 판정은 구현 작업과 분리된 Reviewer 또는 검증 단계가 담당한다.

### 21.12 중단 조건

루프는 다음 조건에서 즉시 자동 작업을 멈추고 `decisions-needed.md`로 이동한다.

- 동일한 실패가 3회 반복됨
- Goal의 허용 범위를 넘어서는 변경이 필요함
- 요구사항이 두 가지 이상으로 해석됨
- 회사 코드·비밀·개인정보가 발견됨
- 안전 인터록을 약화해야 통과할 수 있음
- 테스트를 통과시키기 위해 테스트를 삭제·완화해야 함
- 새로운 외부 서비스 비용이나 유료 라이선스가 필요함
- 데이터 삭제·Git 강제 변경·권한 변경이 필요함
- Unity 라이선스·로그인·사람의 시각 판단이 필요함
- 정해진 반복 횟수나 비용 한도를 초과함

Blocked는 실패가 아니라 사용자 판단이 필요한 정상 상태다.

### 21.13 Comprehension Gate

자동화로 코드가 늘어나는 속도보다 사용자의 이해가 뒤처지지 않도록 별도 완료 조건을 둔다.

매일 문서에 다음을 추가한다.

```markdown
## 내가 직접 설명할 수 있어야 하는 것
1. 이 기능이 장비 공정에서 왜 필요한가?
2. 입력 신호와 출력 신호는 무엇인가?
3. 실패하면 장비는 어떤 상태로 가는가?

## 이해 확인 질문
- Home 센서가 들어오지 않으면 왜 Timeout이 필요한가?
- `Ready`와 `InPosition`은 어떻게 다른가?
- 실제 장비 드라이버와 가상 드라이버를 왜 분리했는가?
```

다음 중 하나라도 해당하면 Goal은 기술적으로 완료돼도 학습 완료로 표시하지 않는다.

- 사용자가 변경 이유를 한 문장으로 설명할 수 없음
- 실행 방법을 문서 없이 전혀 재현할 수 없음
- 주요 클래스의 입력·출력을 설명할 수 없음
- 테스트가 무엇을 보증하는지 이해하지 못함

### 21.14 비용과 반복 제한

- 기본 구현 루프 최대 5회
- 같은 오류 수정 최대 3회
- 병렬 Implementer 최대 2개
- Reviewer는 L2·L3 또는 병합 전 최종 검증에 우선 사용
- 전체 저장소 재분석보다 변경 파일과 관련 의존성만 검토
- 매일 불필요한 코드 변경을 만들지 않음
- 대규모 리팩터링은 별도 Goal로 분리

### 21.15 제안 자동화

저장소와 기본 테스트가 만들어진 뒤 다음 자동화를 단계적으로 등록한다.

#### Daily Triage

- 기본적으로 읽기 전용
- 열린 Goal·PR·CI·State 확인
- 오늘의 후보 Task 최대 3개 제안
- 사용자 결정이 필요하면 Inbox에 기록
- 자동 코드 수정은 수행하지 않음

#### Goal Runner

- 사용자가 활성화한 Goal만 실행
- Worktree와 브랜치 생성
- 구현·테스트·리뷰·문서 반복
- 위험도에 따라 Draft PR 또는 PR 생성
- 중단 조건 발생 시 Blocked 처리

#### Daily Learning Summary

- 당일 변경과 테스트를 초보자용으로 요약
- 사용자가 이해해야 할 개념 3개 제공
- 다음 작업 한 가지 제안
- 코드 변경 없이 문서와 보고만 수행

#### CI Monitor

- PR의 빌드·테스트 실패 확인
- 실패 원인 분류
- 허용 범위 내에서 수정 Goal 제안
- 자동 재시도 반복은 제한함

### 21.16 실제 하루의 루프 예시

```text
08:30 Daily Triage
  → GOAL-0003의 Home Timeout 테스트 실패 확인
  → L2 작업으로 분류
  → goal-0003-homing Worktree 생성
  → Implementer가 Timeout 로직 수정
  → 정상·센서 실패·비상정지 테스트 실행
  → Reviewer가 안전 정지와 회귀 검토
  → Draft PR 생성
  → docs/daily에 쉬운 설명 작성
  → 사용자에게 “왜 바꿨고 무엇을 확인할지” 보고
  → 사용자 승인 후 병합
  → Goal과 loop-state 갱신
```

### 21.17 Loop 도입 순서

처음부터 완전 자동화하지 않는다.

1. **수동 루프:** 사람이 “오늘 작업하자”고 시작하고 State·Goal 형식을 검증한다.
2. **Triage 자동화:** 매일 상태 확인과 후보 작업 제안만 자동화한다.
3. **Goal Runner:** 사용자가 승인한 Goal의 구현·테스트·PR을 자동화한다.
4. **저위험 자동 병합:** 안정된 CI 이후 L0만 자동 병합한다.
5. **선택적 병렬화:** 서로 다른 파일을 다루는 독립 Goal만 Worktree에서 병렬 실행한다.

MVP 초기에는 1단계 수동 루프로 시작한다. 루프 자체를 만드는 데 프로젝트 개발보다 많은 시간을 쓰지 않도록 한다.

### 21.18 Loop Engineering 성공 기준

- 사용자는 매번 긴 프롬프트 대신 Goal을 선택하거나 한 문장으로 요청할 수 있다.
- 다음 세션이 이전 작업 상태를 추측하지 않고 저장소에서 읽는다.
- 모든 병합에는 빌드·테스트·리뷰 증거가 있다.
- 자동화가 범위를 넘어가면 스스로 멈춘다.
- 사용자가 주요 설계와 공정 흐름을 설명할 수 있다.
- 작업 속도가 빨라져도 테스트 실패율과 이해 부채가 증가하지 않는다.

## 22. 현재 생성된 Loop 운영 문서

2026-06-25 기준으로 아래 문서를 생성했다.

| 파일 | 역할 |
|---|---|
| `AGENTS.md` | Codex와 Claude Cowork가 따를 프로젝트 공통 작업 규칙 |
| `state/loop-state.md` | 현재 프로젝트 상태와 다음 작업 기억 |
| `state/triage.md` | 오늘 할 작업 후보와 우선순위 |
| `state/decisions-needed.md` | 사용자가 확정해야 하는 결정사항 |
| `state/comprehension.md` | 사용자가 이해해야 할 내용과 복습 질문 |
| `goals/000-template.md` | Goal 단위 작업을 만들 때 쓸 템플릿 |
| `logs/README.md` | 일일 작업 로그 작성 규칙 |
| `logs/2026-06-25.md` | 오늘 작업 로그 |
| `skills/equipment-twin/SKILL.md` | Equipment Twin Lab 프로젝트 지식 초안 |

아직 만들지 않은 것:

- 자동 스케줄
- 자동 PR 생성
- 자동 병합
- 실제 프로젝트 저장소 뼈대
- Unity 프로젝트
- CI

위 항목은 사용자가 `state/decisions-needed.md`를 검토한 뒤 순서대로 진행한다.

## 23. 2026-06-25 첫 MVP 결과

대표 프로젝트 폴더를 아래 위치에 만들었다.

```text
active/equipment-twin-lab
```

첫 구현은 장비 상태머신이다.

```text
Idle → Loading → Aligning → Inspecting → Unloading → Complete
```

알람 이벤트:

- DoorOpened
- EmergencyStop

검증 결과:

- .NET 8 빌드 성공
- 경고 0개
- 오류 0개
- 콘솔 테스트 5개 통과

아직 하지 않은 것:

- GitHub 원격 저장소 생성
- CI 추가
- 실제 PLC/IO 모델
- Unity 3D 시뮬레이터
- 실제 카메라 검사

## 24. 2026-06-25 Goal 002 결과

GitHub 공개 저장소를 만들었다.

```text
https://github.com/sjsr-0401/equipment-twin-lab
```

두 번째 구현은 가상 IO 모델이다.

핵심 구분:

```text
Input  = 센서/PLC가 바꾸고 장비 SW가 읽는 값
Output = 장비 SW가 쓰고 장치가 반응하는 값
```

추가된 기본 IO 예시:

- `DI_DOOR_CLOSED`
- `DI_EMERGENCY_STOP_PRESSED`
- `DI_LOAD_PRESENT`
- `DI_ALIGNMENT_DONE`
- `DI_INSPECTION_DONE`
- `DO_VACUUM_ON`
- `DO_STAGE_MOVE_REQUESTED`
- `DO_TOWER_LAMP_RED`
- `DO_BUZZER_ON`

검증 결과:

- .NET 8 빌드 성공
- 경고 0개
- 오류 0개
- 콘솔 테스트 11개 통과

다음 후보 작업:

1. Goal 004 PR 생성 및 CI 확인
2. 상태머신과 IO 연결
3. 공정 시나리오 파일 추가
4. Unity 프로젝트 생성 전 Core 검증 강화

## 25. 2026-06-25 Goal 003 결과

GitHub Actions CI를 추가했다.

워크플로 파일:

```text
.github/workflows/ci.yml
```

CI가 실행하는 검증:

```text
dotnet restore EquipmentTwinLab.sln --ignore-failed-sources
dotnet build EquipmentTwinLab.sln --no-restore --configuration Release
dotnet run --project tests/EquipmentTwin.Core.Tests/EquipmentTwin.Core.Tests.csproj --no-restore --configuration Release
```

의미:

- PR마다 최소 빌드/테스트가 자동으로 확인된다.
- 에이전트가 만든 변경을 GitHub에서 반복 검증할 수 있다.
- 아직 자동 병합은 하지 않는다.

확인 결과:

- `goal/002-virtual-io` push 이벤트 CI 성공
- PR #1 pull_request 이벤트 CI 성공

## 26. 2026-06-25 Goal 004 결과

Clock/Timeout 모델을 추가했다.

추가된 핵심 파일:

- `IClock.cs`
- `SystemClock.cs`
- `ManualClock.cs`
- `StateTimeoutPolicy.cs`
- `TimeoutCheckResult.cs`

핵심 개념:

```text
SystemClock = 실제 실행 시간
ManualClock = 테스트/시뮬레이션에서 직접 움직이는 시간
StateTimeoutPolicy = 상태별 허용 시간
CheckTimeout = 현재 상태가 너무 오래 지속됐는지 검사
```

예시:

```text
Loading 상태 진입
30초 동안 LoadComplete 없음
CheckTimeout 실행
Alarmed 상태로 전환
```

검증 결과:

- .NET 8 빌드 성공
- 경고 0개
- 오류 0개
- 콘솔 테스트 17개 통과

한계:

- 실제 장비 안전 회로를 구현한 것이 아니다.
- 실제 PLC 타이머가 아니다.
- Timeout 우선순위와 복구 절차는 다음 단계에서 더 구체화해야 한다.
