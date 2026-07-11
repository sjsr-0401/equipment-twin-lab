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
| P1 | Template Batch Report | 여러 recipe 실행 결과를 한 번에 비교해야 한다 | 완료 |
| P1 | Fault Expected-Failure Report | fault 주입처럼 실패가 기대값인 케이스를 안전하게 검증해야 한다 | PR 대기 |
| P1 | Inspection Scenario Selection | 같은 제품에서 PASS/FAIL 검사 케이스를 선택할 수 있어야 한다 | 완료 |
| P1 | Fault Scenario Catalog | fault 종류별 기대 알람과 복구 조건을 정리해야 한다 | 대기 |
| P1 | Inspection Scenario Batch Matrix | recipe와 inspection scenario 조합을 한 번에 비교해야 한다 | 대기 |
| P2 | Core 검증 정리 문서 | 면접/포트폴리오에서 현재 구조를 설명할 수 있어야 한다 | 완료 |
| P2 | 이전 장비 SW 경험과 새 프로젝트 연결 문서 작성 | 면접에서 “실무 경험 → 사이드 프로젝트”로 설명 가능해진다 | 대기 |
| P2 | Visual Studio build/debug 가이드 | 사용자가 직접 breakpoint를 걸고 Core 흐름을 이해할 수 있어야 한다 | 완료 |
| P2 | 매일 작업 로그 템플릿 정착 | 사용자가 나중에 복기하기 쉬워진다 | 진행 중 |

## 오늘의 추천 작업

1. PR #22를 Ready로 전환하고 병합한다.
2. `main`을 최신화한다.
3. 다음 후보로 Inspection Scenario Batch Matrix 또는 Fault Scenario Catalog를 선택한다.

## 보류 작업

- 실제 자동 스케줄 등록
- 자동 PR 생성
- 자동 병합
- Unity 에셋 제작
- CAD/Blender 모델 적용
- MES 연동 어댑터 (`IMesGateway`): 장비 상태/생산이력/알람을 상위 시스템에 발행하는 인터페이스. SECS/GEM은 풀구현이 아니라 이벤트 보고 개념만 일반화. MVP의 비전 검사(Phase 4)로 생산 이력이 실제로 쌓이기 시작한 뒤 Goal로 분리. "MES 구축"이 아니라 "장비-MES 연동 계층"으로 정직하게 포지셔닝.

위 항목들은 프로젝트 골격과 검증 루프가 생긴 뒤 진행한다.
## 2026-07-01 Triage Update

현재 P1 초점이 vision-inspection 확장에서 process-equipment modeling으로 이동했다.

이유:

- The user's work background is closer to equipment/deposition process SW than camera inspection.
- Public/synthetic ALD metallization gives a stronger manufacturing-equipment portfolio story.
- Vision remains optional; it should not block the representative project.

Active item:

| 우선순위 | 작업 | 이유 | 상태 |
|---|---|---|---|
| P1 | Public molybdenum ALD process model | Shows process sequence, recipe validation, fault stop, and Unity replay data | In progress |
| P1 | Process timeline JSON export | Needed before Unity can replay the ALD sequence cleanly | Next |
| P1 | Unity process player skeleton | Needed for visible portfolio demo | Next |
## 2026-07-01 Triage Update: Goal 026

Goal 026 changed the next Unity dependency from "concept" to "data contract".

| 우선순위 | 작업 | 이유 | 상태 |
|---|---|---|---|
| P1 | Process Timeline JSON Export | Unity needs structured replay data, not Markdown parsing | In progress |
| P1 | Unity Process Player Skeleton | Next visible demo step | Next |
| P1 | Unity Chamber/Wafer/Valve Visual | Needed for portfolio video | Next |

현재 blocker 처리:

- `EquipmentStateMachine.cs` typo `prev0ious` was fixed because it broke compilation.

## 2026-07-01 Triage Update: Goal 027

작업 초점 이동 from JSON data contract to the first Unity replay skeleton.

| 우선순위 | 작업 | 이유 | 상태 |
|---|---|---|---|
| P1 | Unity Process Player Skeleton | First visible Unity integration point | In progress |
| P1 | Unity Chamber/Wafer/Valve Visual | Needed for portfolio demo video | Next |
| P2 | Unity Editor compile/play validation guide | Needed because CI does not run Unity Editor yet | Next |

현재 검증 경계:

- CI validates .NET and Unity file presence.
- Unity Editor compile/play is still manual.

PR status:

- Goal 027 draft PR: <https://github.com/sjsr-0401/equipment-twin-lab/pull/25>
- CI status: passed
- Next work: add simple Unity chamber/wafer/valve visuals and local Unity Editor validation notes.

## 2026-07-01 Triage Update: Goal 028

작업 초점 이동 from Unity timeline replay to visible primitive process visualization.

| 우선순위 | 작업 | 이유 | 상태 |
|---|---|---|---|
| P1 | Unity Chamber/Wafer/Valve Visual | Needed for visible portfolio demo | In progress |
| P1 | Unity Editor local smoke test | Needed because CI does not compile Unity scripts | Next |
| P2 | Imported CAD/Blender model swap points | Useful after primitives prove data flow | Later |

현재 blocker:

- Unity Editor is not available in the current automation environment.
- Workaround: CI checks file presence and .NET regression; local Unity Play Mode remains manual.

PR status:

- Goal 028 draft PR: <https://github.com/sjsr-0401/equipment-twin-lab/pull/26>
- CI status: passed
- 현재 blocker 업데이트: Unity Editor is installed, but batchmode compile is blocked by `No valid Unity Editor license found`.
- Next work: activate/sign in through Unity Hub, then run Play Mode smoke test.

## 2026-07-01 Triage Update: Goal 029

작업 초점 이동 from primitive visual implementation to repeatable Unity validation.

| 우선순위 | 작업 | 이유 | 상태 |
|---|---|---|---|
| P1 | Unity smoke-test harness | Makes Unity validation repeatable after license activation | In progress |
| P1 | Unity Hub license activation | Required before Unity batchmode/Play Mode can compile | User action |
| P2 | Demo screenshot capture | Useful for portfolio README after smoke test passes | Next |

현재 blocker:

- Unity license activation cannot be completed from repository code.
- The project can still add the harness, scripts, and documentation so the user can run validation immediately after signing in.

PR status:

- Goal 029 draft PR: <https://github.com/sjsr-0401/equipment-twin-lab/pull/27>
- CI status: passed
- Next work: activate Unity Hub license, run the smoke-test script, capture first demo screenshot.

## 2026-07-01 Triage Update: Goal 030

작업 초점 이동 from smoke-test execution to screenshot capture.

| 우선순위 | 작업 | 이유 | 상태 |
|---|---|---|---|
| P1 | Screenshot capture command | Needed for README/portfolio visual proof | In progress |
| P1 | Unity license activation | Required before actual PNG can be generated | User action |
| P2 | README demo image | Requires a generated screenshot first | Next |

현재 blocker:

- The repository can provide screenshot automation, but cannot activate the Unity license.
- Actual screenshot generation remains blocked until Unity Hub login/license activation is completed.

PR status:

- Goal 030 draft PR: <https://github.com/sjsr-0401/equipment-twin-lab/pull/28>
- CI status: passed
- Next work: activate Unity license, run `.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot`, then add the generated screenshot to README/demo docs.

## 2026-07-01 Triage Update: Goal 031

작업 초점 이동 from screenshot execution to portfolio/demo explanation because Unity license activation requires user-side interaction.

| 우선순위 | 작업 | 이유 | 상태 |
|---|---|---|---|
| P1 | Portfolio demo package | Needed so the project is explainable before the first real screenshot | In progress |
| P1 | Unity license activation | Required before actual PNG/video capture | User action |
| P1 | README demo image | Requires generated screenshot first | Next |

현재 blocker:

- Unity Hub license activation cannot be completed while the user is outside.
- The repository can still document the demo flow, honest boundaries, and next verification command.

PR status:

- Goal 031 branch: `goal/031-portfolio-demo-package`
- Next work after this PR: activate Unity license, run `.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot`, then add real screenshot to README.

Completion status:

- Draft PR #29: <https://github.com/sjsr-0401/equipment-twin-lab/pull/29>
- CI: passed
- Remaining blocker: Unity Hub license activation

다음 우선순위:

| 우선순위 | 작업 | 이유 | 상태 |
|---|---|---|---|
| P1 | Unity screenshot capture on licensed machine | Converts Unity visual code into visible portfolio proof | User/local action |
| P1 | README demo image | Needed for first-impression GitHub portfolio page | Next |
| P2 | 3-minute video checklist | Useful after a real screenshot exists | Later |

## 2026-07-01 Triage Update: Goal 032

작업 초점 이동 from screenshot capture to demo polish.

| 우선순위 | 작업 | 이유 | 상태 |
|---|---|---|---|
| P1 | Unity screenshot capture | First visible proof of Unity visual layer | Completed |
| P1 | README demo image | First-impression GitHub portfolio page | Completed |
| P1 | Unity demo polish | Current screenshot is functional but primitive | Next |
| P2 | 3-minute recording checklist | Needed before portfolio video | Next |

현재 blocker 해결:

- Unity Hub license activation is complete.
- Unity batch screenshot generation is working.

New risk:

- The screenshot proves the pipeline but still looks like an MVP primitive scene.
- Next work should improve visual clarity, not add more backend features.

PR status:

- Goal 032 draft PR: <https://github.com/sjsr-0401/equipment-twin-lab/pull/30>
- CI status: passed
- Next work: improve the Unity demo scene readability and prepare a 3-minute recording checklist.

## 2026-07-01 Triage Update: Goal 033

작업 초점 이동 from first screenshot proof to portfolio recording readiness.

| 우선순위 | 작업 | 이유 | 상태 |
|---|---|---|---|
| P1 | Unity demo polish | The screenshot exists, but it needs clearer process-state visuals for portfolio use | In progress |
| P1 | 3-minute recording checklist | The user needs a repeatable script to explain the project without overclaiming | In progress |
| P2 | CAD/Blender model swap design | Useful after the primitive visual story is clear | Next |

현재 검증 경계:

- Unity screenshot generation works locally with Unity Hub Personal license.
- GitHub Actions still validates .NET and file-level Unity wiring only; it does not run Unity Editor.
- The demo must continue to avoid claims about real vendor internals.

Completion status:

- Draft PR #31: <https://github.com/sjsr-0401/equipment-twin-lab/pull/31>
- CI: passed
- 다음 우선순위:
  - record the actual 3-minute demo video, or
  - design the CAD/Blender model swap boundary before importing assets.

## 2026-07-01 Triage Update: Goal 034

작업 초점 이동 to the CAD/Blender model swap boundary.

| 우선순위 | 작업 | 이유 | 상태 |
|---|---|---|---|
| P1 | Unity visual adapter boundary | Prevents process logic from being duplicated in every future model renderer | In progress |
| P1 | Imported model binding component | Gives future CAD/Blender assets a concrete connection point | In progress |
| P2 | Name-based auto-binding | Useful only after a real imported model exists | Later |
| P2 | Actual 3-minute video recording | Still valuable, but code boundary was chosen first to reduce model-swap risk | Next |

현재 검증 경계:

- The project still has no real imported CAD/Blender asset.
- Goal 034 proves the software boundary, not the final imported-model visual quality.

Completion status:

- Draft PR #32: <https://github.com/sjsr-0401/equipment-twin-lab/pull/32>
- CI: passed
- 다음 우선순위:
  - record the actual 3-minute demo video and find explanation gaps;
  - add imported-model auto-binding only after a real asset naming convention exists.

## 2026-07-01 Triage Update: Goal 035

작업 초점 이동 to demo recording readiness.

| 우선순위 | 작업 | 이유 | 상태 |
|---|---|---|---|
| P1 | Demo rehearsal runner | Reduces the chance of a broken demo during recording | In progress |
| P1 | Expected fault handling in rehearsal | Prevents `pumpdown-timeout` exit code 1 from being mistaken for a script failure | Complete |
| P1 | Full rehearsal with Unity screenshot | Confirms the recording material exists before screen recording | Complete |
| P2 | Actual 3-minute video recording | Needs user screen/audio action | Next |

현재 검증 경계:

- The script prepares recording material but does not record the screen.
- Unity screenshot requires a local Unity Editor license; use `-SkipUnity` without Unity.

Completion status:

- Draft PR #33: <https://github.com/sjsr-0401/equipment-twin-lab/pull/33>
- CI: passed
- 다음 우선순위:
  - run the rehearsal once before recording;
  - record the actual 3-minute demo;
  - adjust docs/visuals based on where the explanation gets stuck.

## 2026-07-01 Triage Update: Goal 036

작업 초점 이동 from rehearsal verification to narration readiness.

| 우선순위 | 작업 | 이유 | 상태 |
|---|---|---|---|
| P1 | Demo narration cue cards | The demo can pass technically, but the user needs a Korean script to explain it clearly | Complete |
| P1 | Rehearsal-generated cue card | Reduces English-log confusion during recording | Complete |
| P2 | Actual 3-minute video recording | Requires user screen/audio action | Next |

현재 검증 경계:

- The repository can generate Korean cue cards and validate demo material.
- It still cannot press the screen-recording button or record the user's voice.

다음 우선순위:

- Run `.\scripts\Invoke-PortfolioDemoRehearsal.ps1`.
- Open `artifacts/demo-rehearsal/recording-cue-cards.md`.
- Record the actual 3-minute demo and fix whatever explanation or visual gap appears.

Completion status:

- Draft PR #34: <https://github.com/sjsr-0401/equipment-twin-lab/pull/34>
- CI: passed after replacing a fragile Korean grep check with an ASCII marker check.

## 2026-07-02 Triage Update: Goal 037

작업 초점 이동 from narration readiness to screenshot clarity.

| 우선순위 | 작업 | 이유 | 상태 |
|---|---|---|---|
| P1 | Explanatory Unity screenshot | The previous image rendered correctly but was not self-explanatory | In progress |
| P1 | README demo image update | Portfolio first impression depends on the screenshot | In progress |
| P2 | Actual 3-minute recording | User will handle recording after image quality improves | Next |

현재 검증 경계:

- The screenshot can show labels, color key, status, and process flow.
- It still does not claim real CAD, real vendor sequence, or real deposition physics.

다음 우선순위:

- Open `docs/demo/moly-ald-demo.png` and decide if it is good enough for first recording.
- If yes, record the 3-minute demo.

검증 상태:

- Full rehearsal passed and updated `artifacts/demo-rehearsal/moly-ald-demo.png`.

Completion status:

- Draft PR #35: <https://github.com/sjsr-0401/equipment-twin-lab/pull/35>
- CI: passed
- 다음: user records the 3-minute demo using the improved screenshot.

## 2026-07-02 Triage Update: Goal 038

작업 초점 이동 from recording/demo visuals back to development validation.

| 우선순위 | 작업 | 이유 | 상태 |
|---|---|---|---|
| P1 | ALD fault matrix CLI | Batch validation proves normal and fault behavior without relying on a screen recording | In progress |
| P1 | CI process batch check | Fault matrix should fail automatically if expectations drift | In progress |
| P2 | Better visual/demo polish | User explicitly postponed recording for now | Deferred |

현재 검증 경계:

- The ALD model is public/synthetic.
- `process batch` verifies expected software behavior, not real vendor alarm tables.
- Normal case should complete; configured fault cases should stop in `Alarmed`.

다음 우선순위:

- Finish docs/logs for Goal 038.
- Run Release build, Core tests, process batch CLI, and `git diff --check`.
- Open PR and verify CI.

## 2026-07-02 Triage Update: Goal 039

작업 초점 이동 from development validation back to Unity presentation quality.

| 우선순위 | 작업 | 이유 | 상태 |
|---|---|---|---|
| P1 | Unity operator console layout | User wants a public-reference equipment style with a friendly control interface | In progress |
| P1 | Screenshot refresh | README first impression depends on the visual quality | In progress |
| P2 | Real interactive buttons | Current buttons are visual controls, not yet wired to runtime commands | Next |

현재 검증 경계:

- The layout can look like a semiconductor equipment console.
- It must not claim to be a real vendor CAD/UI/sequence.
- Unity still replays Core/CLI timeline data instead of calculating process logic.

다음 우선순위:

- Validate Unity screenshot capture.
- Update docs and tracked screenshot.
- Open PR after local and CI validation.

## 2026-07-02 Triage Update: Goal 040

작업 초점 이동 from visual implementation to repeatable UI/UX review.

| 우선순위 | 작업 | 이유 | 상태 |
|---|---|---|---|
| P1 | UI/UX agent brief | Gives Codex/Claude/another model a stable reviewer role | In progress |
| P1 | Current screenshot review | Converts subjective “looks bad” feedback into actionable criteria | In progress |
| P1 | Canvas operator panel plan | Prevents more 3D TextMesh UI debt | In progress |
| P2 | Canvas implementation | Should happen after review criteria are fixed | Next |

현재 검증 경계:

- Goal 040 is documentation and process design only.
- It does not implement the Canvas UI yet.
- It sets acceptance criteria for Goal 041.

다음 우선순위:

- Implement Canvas-based operator panel.

## 2026-07-02 Triage Update: Goal 041

작업 초점 이동 from UI/UX review criteria to Canvas UI implementation.

| 우선순위 | 작업 | 이유 | 상태 |
|---|---|---|---|
| P1 | Canvas operator panel | Replaces fragile 3D TextMesh UI with readable HMI-style UI | Done |
| P1 | Unity UI package activation | Required for Canvas/Text/Image components | Done |
| P1 | Screenshot refresh | README/demo image should show Canvas UI, not old TextMesh panel | Done |
| P2 | Button interaction | Visual buttons exist but are not wired yet | Next |
| P2 | Fault selector | Needed to show alarm mode interactively | Next |

현재 검증 경계:

- Canvas displays state from the existing timeline.
- Canvas does not calculate process logic.
- Buttons are visual controls only in this goal.

다음 우선순위:

- Improve HMI typography/instrument readability before adding button behavior.

## 2026-07-02 Triage Update: Goal 042

작업 초점 이동 from basic Canvas UI to HMI readability.

| 우선순위 | 작업 | 이유 | 상태 |
|---|---|---|---|
| P1 | HMI typography hierarchy | Current screen needs professional operator-console readability | Done |
| P1 | Process instrument rows | Numeric process values need range/context, not just text | Done |
| P1 | Alarm priority fields | Alarm card should show priority/code structure | Done |
| P2 | Button interaction | Visual buttons exist but are not wired yet | Next |
| P2 | Fault selector | Needed to show alarm mode interactively | Next |

현재 검증 경계:

- Instrument ranges are synthetic demo ranges.
- Unity still replays timeline data; it does not simulate physical process dynamics.
- Buttons are visual controls only.

다음 우선순위:

- Add button click handling and fault scenario selection.

## 2026-07-02 Triage Update: Goal 043

작업 초점 이동 from HMI instrument readability to the left-side main visual direction.

| 우선순위 | 작업 | 이유 | 상태 |
|---|---|---|---|
| P1 | Process schematic main view | Avoids weak primitive 3D realism and better matches manufacturing HMI practice | Done |
| P1 | Public reference boundary | Keeps the project honest and avoids vendor CAD/UI/process copying | Done |
| P1 | Active valve schematic state | Shows process state as connected system, not isolated shapes | Done |
| P2 | Gas flow animation | Makes schematic feel alive | Next |
| P2 | 3D cutaway helper view | Keeps 3D differentiation without requiring CAD realism | Next |

현재 검증 경계:

- Schematic is public/synthetic.
- It is not a real tool schematic.
- It is not a vendor UI.

다음 우선순위:

- Validate screenshot and then wire button/fault interaction.

## 2026-07-02 Triage Update: Goal 044

작업 초점 이동 from static process schematic to live flow indication.

| 우선순위 | 작업 | 이유 | 상태 |
|---|---|---|---|
| P1 | Gas flow pulse | Makes active process route visible | Done |
| P1 | Showerhead dot pulse | Shows gas entering chamber | Done |
| P1 | Fault highlight path | Prepares alarm/fault visual state | Done |
| P2 | Button interaction | Needed for operator-controlled demo | Next |
| P2 | Fault selector | Needed for fault screenshot/demo | Next |

현재 검증 경계:

- Pulse animation is HMI indication only.
- It is not fluid simulation.
- Buttons are still not wired.

다음 우선순위:

- Add button click handling and fault scenario selection.
## 2026-07-02 Triage Update after Goal 045

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P1 | Fault mode screenshot | Missing portfolio artifact | Capture red alarm/schematic fault state |
| P1 | Operator action log | Missing on-screen action history | Add START/STOP/FAULT/RESET event list |
| P2 | Process fault scenario selector | Current FAULT is synthetic override | Later connect to process fault matrix |
| P2 | Manual Unity click checklist | Not fully documented | Add short Visual Studio/Unity Play Mode checklist |

완료:

- Button interaction is no longer the next blocker.
- Unity smoke test now covers Button, EventSystem, and player state transitions.

## 2026-07-02 Triage Update after Goal 046

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P1 | Reset recovery screenshot | Still missing | Capture or storyboard RESET -> READY state |
| P1 | Process fault scenario selector | Still synthetic override | Connect FAULT to named fault matrix scenario |
| P2 | Action log persistence | In-memory only | Later connect to run report/file log if useful |
| P2 | Log panel readability | Acceptable but small | Expand if layout changes |

완료:

- Fault mode screenshot exists.
- Operator action log exists in Canvas.
- Normal and fault screenshots are available in `docs/demo`.

## 2026-07-02 Triage Update after Goal 047

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P1 | Fault timeline replay binding | Still missing | Drive Unity process state from selected process-runner fault timeline |
| P1 | Scenario truth boundary | Improved but not complete | Keep synthetic HMI hold separate from replayed fault timeline |
| P2 | User-facing fault scenario selector | Not exposed yet | Add cycle/select control if useful after replay binding |
| P2 | Action log persistence | In-memory only | Later connect to report/file log if it helps demo explanation |

완료:

- Reset recovery screenshot exists.
- FAULT now carries the selected public scenario name `precursor-dose-timeout`.
- Normal, fault, and recovery screenshots are available in `docs/demo`.

## 2026-07-02 Triage Update after Goal 048

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P1 | Fault scenario selector UI | Missing | Add a small HMI control to cycle selected scenario |
| P1 | Replay truth boundary | Improved | Keep documenting that Core/CLI timeline is process truth |
| P2 | Fault timeline regeneration script | Missing | Add script if static JSON upkeep becomes annoying |
| P2 | Operator log persistence | In-memory only | Later export action log/run report |

완료:

- Unity fault replay now loads `StreamingAssets/faults/moly-ald-timeline.{scenario}.json`.
- `precursor-dose-timeout` moves to the failed `DoseMetalPrecursor` step.
- HMI shows `FAULT REPLAY`.

## 2026-07-03 Triage Update after Goal 049

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P1 | Fault recovery procedure panel | Next | Show operator-facing recovery checklist after a replay alarm |
| P1 | Fault selector UI | Done | Keep selector locked while fault replay is active |
| P2 | Fault timeline regeneration script | Waiting | Add only if static JSON maintenance becomes repetitive |
| P2 | Operator log persistence | Waiting | Later export action log/run report if it improves the demo story |

완료:

- HMI now exposes four public fault scenarios as selectable chips.
- Scenario selection flows through `MolyAldOperatorCanvas` into `MolyAldProcessPlayer`.
- Unity smoke test validates scenario selection, replay loading, active-fault selection lockout, and reset behavior.

## 2026-07-03 Triage Update after Goal 050

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P1 | WPF main HMI | Done | Use as daily debug surface |
| P1 | WPF alarm recovery procedure | Next | Add operator recovery checklist by fault area |
| P2 | Unity optional viewer launcher | Waiting | Later launch built Unity viewer executable from WPF |
| P2 | WPF styling polish | Waiting | Improve visual density after recovery panel exists |

완료:

- Main HMI direction changed from Unity-first to WPF-first.
- WPF project is in the solution and builds.
- WPF reads the public ALD recipe and calls Core runner directly.
- Unity remains optional, not deleted.

## 2026-07-03 Triage Update after Goal 051

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P0 | WPF startup binding exception | Done | Keep progress indicators one-way bound |
| P1 | WPF alarm recovery procedure | Next | Add operator recovery checklist by fault area |
| P2 | WPF styling polish | Waiting | Improve visual density after recovery panel exists |
| P2 | Unity optional viewer launcher | Waiting | Later launch built Unity viewer executable from WPF |

완료:

- Fixed the WPF `ProgressBar.Value` binding exception caused by read-only calculated ViewModel properties.
- Added a goal record and work log explaining the MVVM binding rule.
- Added CI markers so the same progress binding regression is easier to catch.

## 2026-07-03 Triage Update after Goal 052

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P0 | WPF DataGrid header 가독성 문제 | 완료 | 명시적인 DataGrid style 유지 |
| P0 | WPF load-port label 잘림 | 완료 | schematic label을 visible bounds 안에 유지 |
| P1 | WPF HMI visual system R&D | 다음 | 전문적인 HMI typography, spacing, color rule 정의 |
| P1 | WPF alarm recovery procedure | 대기 | visual system 방향이 정리된 뒤 추가 |

완료:

- Screenshot에서 확인된 WPF UI 가독성 문제를 즉시 수정했다.
- 기본 WPF control style이 dark HMI에 섞여 들어온 이유를 작업 로그에 설명했다.
- 다음 작업 우선순위를 recovery panel 구현에서 visual-system R&D로 조정했다.

## 2026-07-03 Triage Update after Goal 053

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P1 | WPF HMI visual system | 완료 | gray-base/color-reserved rule 유지 |
| P1 | Instrument trend panel | 다음 | pressure/temp/film trend 표시 추가 |
| P1 | WPF alarm recovery procedure | 대기 | trend/overview hierarchy가 더 정리된 뒤 추가 |
| P2 | Chart/gauge dependency decision | 대기 | trend scope가 구체화된 뒤 LiveCharts2 평가 |

완료:

- WPF에 gray-base HMI visual direction을 적용했다.
- 정상 운전 상태에서 불필요한 색상 노이즈를 줄였다.
- 밸브, 계측값, 알람 카드에 text/shape 기반 상태 표현을 추가했다.
- Custom WPF token과 chart/gauge library 선택 기준을 R&D/design note로 남겼다.

## 2026-07-04 Triage Update after Goal 054

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P0 | WPF 리포트 / mock server demo 경로 | 완료 | 커밋 전 build/test/mock-server 검증 유지 |
| P0 | 추적되지 않은 Unity PackageManagerSettings asset | 확인 필요 | 커밋 포함/무시/삭제 여부를 나중에 결정. 이번 goal에서는 건드리지 않음 |
| P1 | WPF HMI 계측값 trend panel | 다음 | pressure/temp/film trend 표시 추가 |
| P1 | WPF 버튼 클릭 자동화 | 대기 | 수동 WPF regression 위험이 커질 때만 추가 |
| P2 | LiveCharts2 또는 chart dependency | 대기 | 단순 custom trend 범위가 구체화된 뒤 평가 |

완료:

- 전체 solution build 통과.
- Core tests 통과.
- WPF project build 통과.
- Mock Server project build 통과.
- Mock Server `GET /health` 통과.
- Mock Server `POST /alarm-issue-report` 통과 및 수신 payload 파일 생성.
- README가 WPF HMI + alarm guide + server outbox + mock server를 대표 공개 demo 흐름으로 보여주도록 정리됨.

다음:

- `목표 055: WPF HMI 계측값 Trend Panel` 진행.

## 2026-07-04 Triage Update after Goal 055

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P1 | WPF HMI 계측값 trend panel | 완료 | 화면 밀도와 가독성을 직접 확인 |
| P0 | 추적되지 않은 Unity PackageManagerSettings asset | 확인 필요 | 커밋 전 결정 |
| P1 | WPF 수동 화면 검수 | 다음 | WPF를 전체화면으로 실행해 잘림/간격/trend 가독성 확인 |
| P1 | 커밋 정리 | 다음 | 커밋 전 공개/비공개 파일 확인 |
| P2 | Chart library 의존성 | 대기 | custom Polyline trend가 부족해질 때까지 보류 |

완료:

- Pressure, Temp, Film을 위한 WPF `RECENT TREND` panel 추가.
- 단순 WPF `Polyline` sparkline 사용.
- Trend 구현은 WPF에만 둠.
- WPF build, Core tests, 전체 solution build, WPF startup smoke 통과.

다음:

- `목표 056: WPF HMI 수동 화면 검수와 커밋 정리` 진행.

## 2026-07-05 Triage Update after Goal 056

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P0 | private-notes 공개 차단 | 완료 | `.gitignore`에 유지 |
| P0 | Unity local 생성 folder ignore | 완료 | `Library/Temp/Obj/Logs/UserSettings` 유지 |
| P0 | Unity PackageManagerSettings asset | 포함 후보 | 커밋 직전 파일 목록에서 한 번 더 확인 |
| P1 | WPF 자동 screenshot visual QA | 막힘 | 사용자가 직접 WPF 화면 screenshot 제공 |
| P1 | WPF trend panel spacing 조정 | 다음 | 실제 screenshot 기준으로 수정 |

완료:

- `.gitignore`에 개인 기록 폴더와 Unity local 생성 폴더를 추가했다.
- `private-notes/`가 repo ignore 규칙으로 보호되는지 확인했다.
- `PackageManagerSettings.asset`를 확인했고 project setting 성격으로 판단했다.
- 전체 solution build, Core tests, `git diff --check`가 통과했다.

다음:

- `목표 057: WPF 실제 화면 확인 후 trend panel spacing 조정` 진행.

## 2026-07-08 Triage Update after Goal 057

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P0 | WPF 읽기 전용 TextBox 바인딩 예외 | 완료 | 표시 전용 `TextBox.Text`는 `Mode=OneWay` 유지 |
| P1 | WPF 하단 영역 잘림 | 다음 | 실제 screenshot 기준으로 row height/scroll/spacing 조정 |
| P1 | WPF ComboBox dark theme | 다음 | selector와 dropdown 배경/글자색을 dark theme에 맞춤 |
| P1 | WPF 한영 라벨 혼합 | 다음 | demo 용어는 판단해서 한국어/영어 기준 정리 |

완료:

- `LatestServerPayloadPreviewText` 바인딩 예외를 수정했다.
- WPF build, 전체 solution build, Core tests가 통과했다.
- 자세한 학습 메모는 공개 repo가 아닌 `private-notes/` 아래에 작성했다.

다음:

- `목표 058: WPF 실제 화면 기준 레이아웃/가독성 조정` 진행.

## 2026-07-10 Triage Update after Goal 058

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P1 | WPF 오른쪽 패널 탭 구조 | 완료 | 조작/알람/로그 탭 구조 유지 |
| P1 | 왼쪽 장비 schematic 고도화 | 다음 | cabinet, gas box, chamber, pump 연결 구조로 개선 |
| P1 | 서버 전송 흐름 stepper UI | 대기 | report/server flow를 단계형 카드로 정리 |
| P2 | 하단 table/trace console 밀도 조정 | 대기 | 왼쪽 schematic 개선 후 전체 균형 확인 |

완료:

- WPF 오른쪽 패널에 `TabControl`을 적용했다.
- 운전 Overview, 알람/리포트, 작업 로그를 탭으로 분리했다.
- WPF build, 전체 solution build, Core tests가 통과했다.

다음:

- `목표 059: 왼쪽 장비 schematic 고도화` 진행.

## 2026-07-10 Triage Update after Goal 059

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P1 | 왼쪽 장비 schematic 고도화 | 완료 | 실제 화면 screenshot 기준으로 미세 조정 |
| P1 | 알람/리포트/서버 stepper UI | 다음 | 알람 대응 흐름을 단계형 카드로 정리 |
| P1 | fault별 line 강조 | 대기 | GAS-301 등 fault code별 문제 라인을 강조 |
| P2 | active flow animation | 대기 | WPF shape animation은 필요성이 확인된 뒤 진행 |

완료:

- 왼쪽 장비 schematic을 module layout으로 재구성했다.
- chamber 전체 red fill을 줄이고 flow/outline 중심 알람 표현으로 바꿨다.
- WPF Release build, 전체 solution Release build, Core tests가 통과했다.

다음:

- `목표 060: 알람/리포트/서버 전송 stepper UI` 진행.

## 2026-07-10 Triage Update after Goal 060

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P1 | 알람/리포트/서버 stepper UI | 완료 | 실제 화면 screenshot 기준으로 크기/간격 조정 |
| P1 | Stepper와 schematic visual QA | 다음 | 글자 잘림, scroll 위치, 좌우 균형 확인 |
| P1 | Mock Server online/offline 표시 | 대기 | health check 필요성이 확인되면 추가 |
| P2 | fault별 workflow 강조 | 대기 | fault code별 문제 단계 강조 |

완료:

- 알람/리포트 탭에 6단계 workflow stepper를 추가했다.
- 기존 report/server 기능은 유지했다.
- WPF Release build, 전체 solution Release build, Core tests가 통과했다.

다음:

- `목표 061: 실제 화면 기준 stepper와 schematic 시각 QA` 진행.

## 2026-07-10 Triage Update after Goal 061

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P1 | Mock Server online/offline 표시 | 완료 | 실제 화면에서 상태 카드 크기 확인 |
| P1 | Stepper와 schematic visual QA | 다음 | 글자 잘림, scroll 위치, 좌우 균형 확인 |
| P1 | 서버 상태를 workflow stepper와 연결 | 대기 | health check 카드 사용성이 충분한지 본 뒤 결정 |
| P2 | 자동 health polling | 보류 | 데모 단계에서는 수동 확인 유지 |

완료:

- WPF 알람/리포트 탭에 Mock Server 연결 상태 카드를 추가했다.
- `서버 확인` 버튼으로 Mock Server `/health` endpoint를 확인한다.
- payload 전송 성공/실패가 서버 연결 상태 표시에도 반영된다.

다음:

- `목표 062: 실제 화면 기준 stepper/server card/schematic 시각 QA` 진행.

## 2026-07-10 Triage Update after Goal 062

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P1 | Stepper/server card/schematic 구조 QA | 완료 | 최신 실행 화면에서 pixel 단위 최종 확인 |
| P1 | 서버 상태와 workflow 전송 단계 연결 | 다음 | Offline/Online/전송 성공을 6단계 card에 반영 |
| P1 | 최신 WPF screenshot 확인 | 확인 필요 | 자동 캡처 오류 때문에 사용자 실행 화면으로 확인 |
| P2 | 자동 health polling | 보류 | 수동 health check 유지 |

완료:

- WPF 시작 시 최대화 적용.
- 하단 debug row를 반응형 높이로 변경.
- 알람 탭 중첩 ScrollViewer 제거.
- 긴 detail/path를 말줄임과 ToolTip으로 정리.
- Mock Server 상태 카드 폭 사용 개선.

다음:

- `목표 063: Mock Server 상태와 workflow 전송 단계 연결` 진행.

## 2026-07-10 Triage Update after Goal 063

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P1 | 서버 상태와 workflow 전송 단계 연결 | 완료 | 상태 우선순위 유지 |
| P1 | fault code별 schematic 진단 라인 | 다음 | GAS/VAC/TMP/FILM 영역별 강조 |
| P1 | 최신 WPF screenshot 확인 | 확인 필요 | 다음 schematic 변경 후 함께 확인 |
| P2 | WPF ViewModel 전용 영구 테스트 | 대기 | 표시 로직이 더 커질 때 별도 test project 검토 |
| P2 | 자동 health polling | 보류 | 수동 health check 유지 |

완료:

- Workflow 전송 카드에 CHECKING/OFFLINE/ONLINE/HTTP OK 상태 연결.
- Offline red, Online blue, send success green 규칙 적용.
- .NET 8 임시 검증으로 영어/한국어 상태와 색상 확인.

다음:

- `목표 064: fault code별 schematic 진단 라인 강조` 진행.

## 2026-07-10 Triage Update after Goal 064

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P1 | fault code별 schematic 진단 라인 | 완료 | GAS/TMP/VAC mapping 유지 |
| P1 | ALD wafer transfer 상태 시각화 | 다음 | timeline step 기반 위치/gate 표시 |
| P1 | 최신 WPF screenshot 확인 | 확인 필요 | wafer transfer 추가 후 함께 확인 |
| P2 | FILM fault 영역 강조 | 대기 | Core fault와 alarm guide가 생긴 뒤 연결 |
| P2 | 색상 점멸/animation | 보류 | 정적 상태 가독성 확인 후 판단 |

완료:

- GAS-301 Gas Box/Delivery 강조.
- TMP-201 Chamber/Heater 강조.
- VAC-101 Exhaust/Pump/Vacuum path 강조.
- 진단 focus badge와 색상 이중 표현.

다음:

- `목표 065: ALD timeline 기반 Wafer Transfer 상태 시각화` 진행.

## 2026-07-10 Triage Update after Goal 065

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P1 | ALD wafer transfer 상태 시각화 | 완료 | step mapping 유지 |
| P1 | WPF demo 상태 자동 screenshot | 다음 | 정상/알람/이송 PNG 생성 경로 검토 |
| P1 | 최신 WPF 화면 visual QA | 대기 | 자동 screenshot 결과로 확인 |
| P2 | 연속 transfer animation | 보류 | 상태 기반 표시 품질 확인 후 판단 |
| P2 | 실제 transfer robot/load lock 모델 | 제외 | 별도 Core 범위 결정 전 구현하지 않음 |

완료:

- LoadWafer/Process/TransferOut/Complete Wafer 위치 표시.
- Gate OPEN/CLOSED와 alarm hold 표시.
- Exhaust 배기와 Wafer 이송 경로 분리.
- MotionAxis 미사용 검증.

다음:

- `목표 066: WPF Demo 상태 자동 Screenshot Capture` 진행.

## 2026-07-10 Triage Update after Goal 066

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P1 | WPF demo 상태 자동 screenshot | 완료 | 5개 상태 캡처 스크립트 유지 |
| P1 | 최신 WPF 화면 visual QA | 다음 | 1600×900 PNG끼리 상태별 비교 |
| P2 | screenshot pixel regression | 보류 | UI 기준 이미지가 안정된 뒤 검토 |
| P2 | 전체 fault screenshot matrix | 보류 | 대표 GAS-301 외 추가 필요성 판단 |

완료:

- Load/Process/Alarm/TransferOut/Complete 자동 PNG 생성.
- 한국어/영어 선택 지원.
- Alarm은 알람/리포트 탭으로 자동 전환.
- 5개 파일과 1600×900 해상도 자동 검증.
- 캡처 전용 software rendering으로 부분 렌더 문제 해결.

다음:

- `목표 067: 자동 Screenshot 기반 WPF Visual QA 2차` 진행.

## 2026-07-10 Triage Update after Goal 067

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P1 | WPF screenshot 렌더 안정화 | 완료 | 최대화 + client 화면 캡처 유지 |
| P1 | 한국어 일반 UI 라벨 일관성 | 다음 | 조작/영역 제목만 언어 전환 연결 |
| P2 | golden-image pixel regression | 보류 | 시간/trace 마스킹 정책이 필요할 때 검토 |
| P2 | 전체 fault screenshot matrix | 보류 | 대표 GAS-301 외 확장 필요성 판단 |

완료:

- 한국어/영어 5개 상태 자동 캡처.
- 작업표시줄 제외와 1600×900 정규화.
- 검정 표본 및 네 화면 anchor 검사.
- build/tests/일반 WPF smoke test 통과.

다음:

- `목표 068: WPF 한국어 일반 UI 라벨 일관성 정리` 진행.

## 2026-07-10 Triage Update after Goal 068

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P1 | 한국어 일반 UI 라벨 일관성 | 완료 | ViewModel binding 유지 |
| P1 | 알람 작업지시서 한국어 문장 품질 | 다음 | summary/check/choice/escalation 정리 |
| P2 | `.resx` 기반 localization | 보류 | 화면/언어 수가 늘 때 재검토 |
| P2 | debug table/trace payload 번역 | 제외 | 개발자 진단 원문 유지 |

완료:

- 한국어 버튼, 일반 제목, 운전/계측 상태 정리.
- 영어 화면 회귀 없음 확인.
- 한국어/영어 Screenshot 및 전체 build/tests/smoke test 통과.

다음:

- `목표 069: 알람 작업지시서 한국어 문장 품질 정리` 진행.

## 2026-07-10 Triage Update after Goal 069

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P1 | WPF 장비 schematic 시각 깊이 | 완료 | 기존 Core/ViewModel binding 유지 |
| P1 | 알람 작업지시서 한국어 문장 품질 | 다음 | Goal 070으로 이동 |
| P2 | 장비 process animation | 보류 | 정적 가독성 유지 후 필요성 판단 |
| P2 | 실제 CAD/Blender model | 선택 | Unity viewer 또는 향후 별도 surface에서 연결 |

완료:

- FOUP/Gas Box/Chamber/Exhaust module 외형 고도화.
- MFC/Valve/Showerhead/Wafer/Susceptor/Heater/Throttle/Pump 구성 표시.
- 상태별 flow/fault/wafer/gate 시각 연동 유지.
- build/tests/Screenshot/smoke test 통과.

다음:

- `목표 070: 알람 작업지시서 한국어 문장 품질 정리` 진행.

## 2026-07-10 Triage Update after Goal 070

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P1 | WPF 장비 process animation | 완료 | Core/ViewModel/XAML 분리 유지 |
| P1 | 알람 작업지시서 한국어 문장 품질 | 다음 | Goal 071로 이동 |
| P2 | 실제 gas particle 물리 모델 | 제외 | schematic animation으로 충분 |
| P2 | Reduce Motion 사용자 설정 | 보류 | 실제 사용자 요구 발생 시 검토 |

완료:

- Gas/Vacuum/Valve/Heater/Pump/Wafer/Gate animation 추가.
- Alarm 중 정상 animation 정지.
- 한영 Screenshot, build, tests, smoke test 통과.

다음:

- `목표 071: 알람 작업지시서 한국어 문장 품질 정리` 진행.

## 2026-07-10 Triage Update after Goal 071

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P1 | 알람 작업지시서 한국어 문장 품질 | 완료 | 영어 JSON 원본 유지 |
| P1 | 한국어 Markdown 이슈 리포트 | 완료 | Language 기반 렌더링 유지 |
| P1 | 리포트/Payload 계약 자동 테스트 | 다음 | Goal 072에서 CI 연결 |
| P2 | `.resx` localization | 보류 | 언어 또는 화면 수 증가 시 검토 |

완료:

- 4개 가이드 56개 문장 정리.
- 한국어 Severity/Workflow/Markdown 정리.
- JSON Language 추가와 기계용 Severity 유지.
- 한영 Screenshot, Export smoke, build, tests 통과.

다음:

- `목표 072: 알람 리포트와 서버 Payload 계약 자동 테스트` 진행.

## 2026-07-10 Triage Update after Goal 072

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P1 | 리포트/Payload 계약 자동 테스트 | 완료 | CI gate 유지 |
| P1 | 서버 전송 실패 Retry/상태 관리 | 다음 | Goal 073 진행 |
| P2 | Mock Server 통합 테스트 | 보류 | Retry 구현 뒤 판단 |
| P2 | 외부 테스트 프레임워크 | 제외 | 현재 콘솔 테스트로 충분 |

완료:

- WPF 계약 테스트 프로젝트와 CI 단계 추가.
- 한국어/영어 Markdown, JSON, Outbox 계약 검증.
- build, Core tests, WPF contract tests 통과.

다음:

- `목표 073: 서버 전송 실패 Retry와 Outbox 상태 관리` 진행.

## 2026-07-11 Triage Update after Goal 073

| 우선순위 | 항목 | 상태 | 다음 조치 |
|---|---|---|---|
| P1 | 서버 전송 Retry/Outbox 상태 관리 | 완료 | 파일 상태를 source of truth로 유지 |
| P1 | Mock Server 중복 수신 방지 | 다음 | envelopeId 기반 멱등 처리 |
| P2 | 자동 재시도/지수 백오프 | 보류 | 실제 운영 요구와 정책 확정 후 구현 |
| P2 | 다중 프로세스 파일 잠금 | 보류 | 동시 sender가 필요할 때 검토 |

완료:

- queued/sending/failed/sent 상태와 전송 메타데이터 영속화.
- 실패 수동 재전송, 성공 중복 전송 차단.
- WPF 상태/횟수/오류 표시.
- 상태 전이 계약 테스트 추가.

다음:

- `목표 074: Mock Server 중복 수신 방지와 전송 Receipt` 진행.
