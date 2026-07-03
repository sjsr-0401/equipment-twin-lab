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
| P2 | RayWork 경험과 새 프로젝트 연결 문서 작성 | 면접에서 “실무 경험 → 사이드 프로젝트”로 설명 가능해진다 | 대기 |
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

Current P1 focus changed from vision-inspection expansion to process-equipment modeling.

Reason:

- The user's work background is closer to equipment/deposition process SW than camera inspection.
- Public/synthetic ALD metallization gives a stronger manufacturing-equipment portfolio story.
- Vision remains optional; it should not block the representative project.

Active item:

| Priority | Work | Reason | Status |
|---|---|---|---|
| P1 | Public molybdenum ALD process model | Shows process sequence, recipe validation, fault stop, and Unity replay data | In progress |
| P1 | Process timeline JSON export | Needed before Unity can replay the ALD sequence cleanly | Next |
| P1 | Unity process player skeleton | Needed for visible portfolio demo | Next |
## 2026-07-01 Triage Update: Goal 026

Goal 026 changed the next Unity dependency from "concept" to "data contract".

| Priority | Work | Reason | Status |
|---|---|---|---|
| P1 | Process Timeline JSON Export | Unity needs structured replay data, not Markdown parsing | In progress |
| P1 | Unity Process Player Skeleton | Next visible demo step | Next |
| P1 | Unity Chamber/Wafer/Valve Visual | Needed for portfolio video | Next |

Current blocker handled:

- `EquipmentStateMachine.cs` typo `prev0ious` was fixed because it broke compilation.

## 2026-07-01 Triage Update: Goal 027

Active focus moved from JSON data contract to the first Unity replay skeleton.

| Priority | Work | Reason | Status |
|---|---|---|---|
| P1 | Unity Process Player Skeleton | First visible Unity integration point | In progress |
| P1 | Unity Chamber/Wafer/Valve Visual | Needed for portfolio demo video | Next |
| P2 | Unity Editor compile/play validation guide | Needed because CI does not run Unity Editor yet | Next |

Current validation boundary:

- CI validates .NET and Unity file presence.
- Unity Editor compile/play is still manual.

PR status:

- Goal 027 draft PR: <https://github.com/sjsr-0401/equipment-twin-lab/pull/25>
- CI status: passed
- Next work: add simple Unity chamber/wafer/valve visuals and local Unity Editor validation notes.

## 2026-07-01 Triage Update: Goal 028

Active focus moved from Unity timeline replay to visible primitive process visualization.

| Priority | Work | Reason | Status |
|---|---|---|---|
| P1 | Unity Chamber/Wafer/Valve Visual | Needed for visible portfolio demo | In progress |
| P1 | Unity Editor local smoke test | Needed because CI does not compile Unity scripts | Next |
| P2 | Imported CAD/Blender model swap points | Useful after primitives prove data flow | Later |

Current blocker:

- Unity Editor is not available in the current automation environment.
- Workaround: CI checks file presence and .NET regression; local Unity Play Mode remains manual.

PR status:

- Goal 028 draft PR: <https://github.com/sjsr-0401/equipment-twin-lab/pull/26>
- CI status: passed
- Current blocker update: Unity Editor is installed, but batchmode compile is blocked by `No valid Unity Editor license found`.
- Next work: activate/sign in through Unity Hub, then run Play Mode smoke test.

## 2026-07-01 Triage Update: Goal 029

Active focus moved from primitive visual implementation to repeatable Unity validation.

| Priority | Work | Reason | Status |
|---|---|---|---|
| P1 | Unity smoke-test harness | Makes Unity validation repeatable after license activation | In progress |
| P1 | Unity Hub license activation | Required before Unity batchmode/Play Mode can compile | User action |
| P2 | Demo screenshot capture | Useful for portfolio README after smoke test passes | Next |

Current blocker:

- Unity license activation cannot be completed from repository code.
- The project can still add the harness, scripts, and documentation so the user can run validation immediately after signing in.

PR status:

- Goal 029 draft PR: <https://github.com/sjsr-0401/equipment-twin-lab/pull/27>
- CI status: passed
- Next work: activate Unity Hub license, run the smoke-test script, capture first demo screenshot.

## 2026-07-01 Triage Update: Goal 030

Active focus moved from smoke-test execution to screenshot capture.

| Priority | Work | Reason | Status |
|---|---|---|---|
| P1 | Screenshot capture command | Needed for README/portfolio visual proof | In progress |
| P1 | Unity license activation | Required before actual PNG can be generated | User action |
| P2 | README demo image | Requires a generated screenshot first | Next |

Current blocker:

- The repository can provide screenshot automation, but cannot activate the Unity license.
- Actual screenshot generation remains blocked until Unity Hub login/license activation is completed.

PR status:

- Goal 030 draft PR: <https://github.com/sjsr-0401/equipment-twin-lab/pull/28>
- CI status: passed
- Next work: activate Unity license, run `.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot`, then add the generated screenshot to README/demo docs.

## 2026-07-01 Triage Update: Goal 031

Active focus moved from screenshot execution to portfolio/demo explanation because Unity license activation requires user-side interaction.

| Priority | Work | Reason | Status |
|---|---|---|---|
| P1 | Portfolio demo package | Needed so the project is explainable before the first real screenshot | In progress |
| P1 | Unity license activation | Required before actual PNG/video capture | User action |
| P1 | README demo image | Requires generated screenshot first | Next |

Current blocker:

- Unity Hub license activation cannot be completed while the user is outside.
- The repository can still document the demo flow, honest boundaries, and next verification command.

PR status:

- Goal 031 branch: `goal/031-portfolio-demo-package`
- Next work after this PR: activate Unity license, run `.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot`, then add real screenshot to README.

Completion status:

- Draft PR #29: <https://github.com/sjsr-0401/equipment-twin-lab/pull/29>
- CI: passed
- Remaining blocker: Unity Hub license activation

Next priority:

| Priority | Work | Reason | Status |
|---|---|---|---|
| P1 | Unity screenshot capture on licensed machine | Converts Unity visual code into visible portfolio proof | User/local action |
| P1 | README demo image | Needed for first-impression GitHub portfolio page | Next |
| P2 | 3-minute video checklist | Useful after a real screenshot exists | Later |

## 2026-07-01 Triage Update: Goal 032

Active focus moved from screenshot capture to demo polish.

| Priority | Work | Reason | Status |
|---|---|---|---|
| P1 | Unity screenshot capture | First visible proof of Unity visual layer | Completed |
| P1 | README demo image | First-impression GitHub portfolio page | Completed |
| P1 | Unity demo polish | Current screenshot is functional but primitive | Next |
| P2 | 3-minute recording checklist | Needed before portfolio video | Next |

Current blocker resolved:

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

Active focus moved from first screenshot proof to portfolio recording readiness.

| Priority | Work | Reason | Status |
|---|---|---|---|
| P1 | Unity demo polish | The screenshot exists, but it needs clearer process-state visuals for portfolio use | In progress |
| P1 | 3-minute recording checklist | The user needs a repeatable script to explain the project without overclaiming | In progress |
| P2 | CAD/Blender model swap design | Useful after the primitive visual story is clear | Next |

Current validation boundary:

- Unity screenshot generation works locally with Unity Hub Personal license.
- GitHub Actions still validates .NET and file-level Unity wiring only; it does not run Unity Editor.
- The demo must continue to avoid claims about real vendor internals.

Completion status:

- Draft PR #31: <https://github.com/sjsr-0401/equipment-twin-lab/pull/31>
- CI: passed
- Next priority:
  - record the actual 3-minute demo video, or
  - design the CAD/Blender model swap boundary before importing assets.

## 2026-07-01 Triage Update: Goal 034

Active focus moved to the CAD/Blender model swap boundary.

| Priority | Work | Reason | Status |
|---|---|---|---|
| P1 | Unity visual adapter boundary | Prevents process logic from being duplicated in every future model renderer | In progress |
| P1 | Imported model binding component | Gives future CAD/Blender assets a concrete connection point | In progress |
| P2 | Name-based auto-binding | Useful only after a real imported model exists | Later |
| P2 | Actual 3-minute video recording | Still valuable, but code boundary was chosen first to reduce model-swap risk | Next |

Current validation boundary:

- The project still has no real imported CAD/Blender asset.
- Goal 034 proves the software boundary, not the final imported-model visual quality.

Completion status:

- Draft PR #32: <https://github.com/sjsr-0401/equipment-twin-lab/pull/32>
- CI: passed
- Next priority:
  - record the actual 3-minute demo video and find explanation gaps;
  - add imported-model auto-binding only after a real asset naming convention exists.

## 2026-07-01 Triage Update: Goal 035

Active focus moved to demo recording readiness.

| Priority | Work | Reason | Status |
|---|---|---|---|
| P1 | Demo rehearsal runner | Reduces the chance of a broken demo during recording | In progress |
| P1 | Expected fault handling in rehearsal | Prevents `pumpdown-timeout` exit code 1 from being mistaken for a script failure | Complete |
| P1 | Full rehearsal with Unity screenshot | Confirms the recording material exists before screen recording | Complete |
| P2 | Actual 3-minute video recording | Needs user screen/audio action | Next |

Current validation boundary:

- The script prepares recording material but does not record the screen.
- Unity screenshot requires a local Unity Editor license; use `-SkipUnity` without Unity.

Completion status:

- Draft PR #33: <https://github.com/sjsr-0401/equipment-twin-lab/pull/33>
- CI: passed
- Next priority:
  - run the rehearsal once before recording;
  - record the actual 3-minute demo;
  - adjust docs/visuals based on where the explanation gets stuck.

## 2026-07-01 Triage Update: Goal 036

Active focus moved from rehearsal verification to narration readiness.

| Priority | Work | Reason | Status |
|---|---|---|---|
| P1 | Demo narration cue cards | The demo can pass technically, but the user needs a Korean script to explain it clearly | Complete |
| P1 | Rehearsal-generated cue card | Reduces English-log confusion during recording | Complete |
| P2 | Actual 3-minute video recording | Requires user screen/audio action | Next |

Current validation boundary:

- The repository can generate Korean cue cards and validate demo material.
- It still cannot press the screen-recording button or record the user's voice.

Next priority:

- Run `.\scripts\Invoke-PortfolioDemoRehearsal.ps1`.
- Open `artifacts/demo-rehearsal/recording-cue-cards.md`.
- Record the actual 3-minute demo and fix whatever explanation or visual gap appears.

Completion status:

- Draft PR #34: <https://github.com/sjsr-0401/equipment-twin-lab/pull/34>
- CI: passed after replacing a fragile Korean grep check with an ASCII marker check.

## 2026-07-02 Triage Update: Goal 037

Active focus moved from narration readiness to screenshot clarity.

| Priority | Work | Reason | Status |
|---|---|---|---|
| P1 | Explanatory Unity screenshot | The previous image rendered correctly but was not self-explanatory | In progress |
| P1 | README demo image update | Portfolio first impression depends on the screenshot | In progress |
| P2 | Actual 3-minute recording | User will handle recording after image quality improves | Next |

Current validation boundary:

- The screenshot can show labels, color key, status, and process flow.
- It still does not claim real CAD, real vendor sequence, or real deposition physics.

Next priority:

- Open `docs/demo/moly-ald-demo.png` and decide if it is good enough for first recording.
- If yes, record the 3-minute demo.

Validation status:

- Full rehearsal passed and updated `artifacts/demo-rehearsal/moly-ald-demo.png`.

Completion status:

- Draft PR #35: <https://github.com/sjsr-0401/equipment-twin-lab/pull/35>
- CI: passed
- Next: user records the 3-minute demo using the improved screenshot.

## 2026-07-02 Triage Update: Goal 038

Active focus moved from recording/demo visuals back to development validation.

| Priority | Work | Reason | Status |
|---|---|---|---|
| P1 | ALD fault matrix CLI | Batch validation proves normal and fault behavior without relying on a screen recording | In progress |
| P1 | CI process batch check | Fault matrix should fail automatically if expectations drift | In progress |
| P2 | Better visual/demo polish | User explicitly postponed recording for now | Deferred |

Current validation boundary:

- The ALD model is public/synthetic.
- `process batch` verifies expected software behavior, not real vendor alarm tables.
- Normal case should complete; configured fault cases should stop in `Alarmed`.

Next priority:

- Finish docs/logs for Goal 038.
- Run Release build, Core tests, process batch CLI, and `git diff --check`.
- Open PR and verify CI.

## 2026-07-02 Triage Update: Goal 039

Active focus moved from development validation back to Unity presentation quality.

| Priority | Work | Reason | Status |
|---|---|---|---|
| P1 | Unity operator console layout | User wants a public-reference equipment style with a friendly control interface | In progress |
| P1 | Screenshot refresh | README first impression depends on the visual quality | In progress |
| P2 | Real interactive buttons | Current buttons are visual controls, not yet wired to runtime commands | Next |

Current validation boundary:

- The layout can look like a semiconductor equipment console.
- It must not claim to be a real vendor CAD/UI/sequence.
- Unity still replays Core/CLI timeline data instead of calculating process logic.

Next priority:

- Validate Unity screenshot capture.
- Update docs and tracked screenshot.
- Open PR after local and CI validation.

## 2026-07-02 Triage Update: Goal 040

Active focus moved from visual implementation to repeatable UI/UX review.

| Priority | Work | Reason | Status |
|---|---|---|---|
| P1 | UI/UX agent brief | Gives Codex/Claude/another model a stable reviewer role | In progress |
| P1 | Current screenshot review | Converts subjective “looks bad” feedback into actionable criteria | In progress |
| P1 | Canvas operator panel plan | Prevents more 3D TextMesh UI debt | In progress |
| P2 | Canvas implementation | Should happen after review criteria are fixed | Next |

Current validation boundary:

- Goal 040 is documentation and process design only.
- It does not implement the Canvas UI yet.
- It sets acceptance criteria for Goal 041.

Next priority:

- Implement Canvas-based operator panel.

## 2026-07-02 Triage Update: Goal 041

Active focus moved from UI/UX review criteria to Canvas UI implementation.

| Priority | Work | Reason | Status |
|---|---|---|---|
| P1 | Canvas operator panel | Replaces fragile 3D TextMesh UI with readable HMI-style UI | Done |
| P1 | Unity UI package activation | Required for Canvas/Text/Image components | Done |
| P1 | Screenshot refresh | README/demo image should show Canvas UI, not old TextMesh panel | Done |
| P2 | Button interaction | Visual buttons exist but are not wired yet | Next |
| P2 | Fault selector | Needed to show alarm mode interactively | Next |

Current validation boundary:

- Canvas displays state from the existing timeline.
- Canvas does not calculate process logic.
- Buttons are visual controls only in this goal.

Next priority:

- Improve HMI typography/instrument readability before adding button behavior.

## 2026-07-02 Triage Update: Goal 042

Active focus moved from basic Canvas UI to HMI readability.

| Priority | Work | Reason | Status |
|---|---|---|---|
| P1 | HMI typography hierarchy | Current screen needs professional operator-console readability | Done |
| P1 | Process instrument rows | Numeric process values need range/context, not just text | Done |
| P1 | Alarm priority fields | Alarm card should show priority/code structure | Done |
| P2 | Button interaction | Visual buttons exist but are not wired yet | Next |
| P2 | Fault selector | Needed to show alarm mode interactively | Next |

Current validation boundary:

- Instrument ranges are synthetic demo ranges.
- Unity still replays timeline data; it does not simulate physical process dynamics.
- Buttons are visual controls only.

Next priority:

- Add button click handling and fault scenario selection.

## 2026-07-02 Triage Update: Goal 043

Active focus moved from HMI instrument readability to the left-side main visual direction.

| Priority | Work | Reason | Status |
|---|---|---|---|
| P1 | Process schematic main view | Avoids weak primitive 3D realism and better matches manufacturing HMI practice | Done |
| P1 | Public reference boundary | Keeps the project honest and avoids vendor CAD/UI/process copying | Done |
| P1 | Active valve schematic state | Shows process state as connected system, not isolated shapes | Done |
| P2 | Gas flow animation | Makes schematic feel alive | Next |
| P2 | 3D cutaway helper view | Keeps 3D differentiation without requiring CAD realism | Next |

Current validation boundary:

- Schematic is public/synthetic.
- It is not a real tool schematic.
- It is not a vendor UI.

Next priority:

- Validate screenshot and then wire button/fault interaction.

## 2026-07-02 Triage Update: Goal 044

Active focus moved from static process schematic to live flow indication.

| Priority | Work | Reason | Status |
|---|---|---|---|
| P1 | Gas flow pulse | Makes active process route visible | Done |
| P1 | Showerhead dot pulse | Shows gas entering chamber | Done |
| P1 | Fault highlight path | Prepares alarm/fault visual state | Done |
| P2 | Button interaction | Needed for operator-controlled demo | Next |
| P2 | Fault selector | Needed for fault screenshot/demo | Next |

Current validation boundary:

- Pulse animation is HMI indication only.
- It is not fluid simulation.
- Buttons are still not wired.

Next priority:

- Add button click handling and fault scenario selection.
## 2026-07-02 Triage Update after Goal 045

| Priority | Item | Status | Next action |
|---|---|---|---|
| P1 | Fault mode screenshot | Missing portfolio artifact | Capture red alarm/schematic fault state |
| P1 | Operator action log | Missing on-screen action history | Add START/STOP/FAULT/RESET event list |
| P2 | Process fault scenario selector | Current FAULT is synthetic override | Later connect to process fault matrix |
| P2 | Manual Unity click checklist | Not fully documented | Add short Visual Studio/Unity Play Mode checklist |

Completed:

- Button interaction is no longer the next blocker.
- Unity smoke test now covers Button, EventSystem, and player state transitions.

## 2026-07-02 Triage Update after Goal 046

| Priority | Item | Status | Next action |
|---|---|---|---|
| P1 | Reset recovery screenshot | Still missing | Capture or storyboard RESET -> READY state |
| P1 | Process fault scenario selector | Still synthetic override | Connect FAULT to named fault matrix scenario |
| P2 | Action log persistence | In-memory only | Later connect to run report/file log if useful |
| P2 | Log panel readability | Acceptable but small | Expand if layout changes |

Completed:

- Fault mode screenshot exists.
- Operator action log exists in Canvas.
- Normal and fault screenshots are available in `docs/demo`.

## 2026-07-02 Triage Update after Goal 047

| Priority | Item | Status | Next action |
|---|---|---|---|
| P1 | Fault timeline replay binding | Still missing | Drive Unity process state from selected process-runner fault timeline |
| P1 | Scenario truth boundary | Improved but not complete | Keep synthetic HMI hold separate from replayed fault timeline |
| P2 | User-facing fault scenario selector | Not exposed yet | Add cycle/select control if useful after replay binding |
| P2 | Action log persistence | In-memory only | Later connect to report/file log if it helps demo explanation |

Completed:

- Reset recovery screenshot exists.
- FAULT now carries the selected public scenario name `precursor-dose-timeout`.
- Normal, fault, and recovery screenshots are available in `docs/demo`.

## 2026-07-02 Triage Update after Goal 048

| Priority | Item | Status | Next action |
|---|---|---|---|
| P1 | Fault scenario selector UI | Missing | Add a small HMI control to cycle selected scenario |
| P1 | Replay truth boundary | Improved | Keep documenting that Core/CLI timeline is process truth |
| P2 | Fault timeline regeneration script | Missing | Add script if static JSON upkeep becomes annoying |
| P2 | Operator log persistence | In-memory only | Later export action log/run report |

Completed:

- Unity fault replay now loads `StreamingAssets/faults/moly-ald-timeline.{scenario}.json`.
- `precursor-dose-timeout` moves to the failed `DoseMetalPrecursor` step.
- HMI shows `FAULT REPLAY`.

## 2026-07-03 Triage Update after Goal 049

| Priority | Item | Status | Next action |
|---|---|---|---|
| P1 | Fault recovery procedure panel | Next | Show operator-facing recovery checklist after a replay alarm |
| P1 | Fault selector UI | Done | Keep selector locked while fault replay is active |
| P2 | Fault timeline regeneration script | Waiting | Add only if static JSON maintenance becomes repetitive |
| P2 | Operator log persistence | Waiting | Later export action log/run report if it improves the demo story |

Completed:

- HMI now exposes four public fault scenarios as selectable chips.
- Scenario selection flows through `MolyAldOperatorCanvas` into `MolyAldProcessPlayer`.
- Unity smoke test validates scenario selection, replay loading, active-fault selection lockout, and reset behavior.

## 2026-07-03 Triage Update after Goal 050

| Priority | Item | Status | Next action |
|---|---|---|---|
| P1 | WPF main HMI | Done | Use as daily debug surface |
| P1 | WPF alarm recovery procedure | Next | Add operator recovery checklist by fault area |
| P2 | Unity optional viewer launcher | Waiting | Later launch built Unity viewer executable from WPF |
| P2 | WPF styling polish | Waiting | Improve visual density after recovery panel exists |

Completed:

- Main HMI direction changed from Unity-first to WPF-first.
- WPF project is in the solution and builds.
- WPF reads the public ALD recipe and calls Core runner directly.
- Unity remains optional, not deleted.

## 2026-07-03 Triage Update after Goal 051

| Priority | Item | Status | Next action |
|---|---|---|---|
| P0 | WPF startup binding exception | Done | Keep progress indicators one-way bound |
| P1 | WPF alarm recovery procedure | Next | Add operator recovery checklist by fault area |
| P2 | WPF styling polish | Waiting | Improve visual density after recovery panel exists |
| P2 | Unity optional viewer launcher | Waiting | Later launch built Unity viewer executable from WPF |

Completed:

- Fixed the WPF `ProgressBar.Value` binding exception caused by read-only calculated ViewModel properties.
- Added a goal record and work log explaining the MVVM binding rule.
- Added CI markers so the same progress binding regression is easier to catch.

## 2026-07-03 Triage Update after Goal 052

| Priority | Item | Status | Next action |
|---|---|---|---|
| P0 | WPF unreadable DataGrid header | Done | Keep explicit DataGrid styles |
| P0 | WPF clipped load-port label | Done | Keep schematic labels inside visible bounds |
| P1 | WPF HMI visual system R&D | Next | Define professional HMI typography, spacing, and color rules |
| P1 | WPF alarm recovery procedure | Waiting | Add after visual system direction is agreed |

Completed:

- Fixed the immediate WPF UI readability defects found from the screenshot.
- Added a work log explaining why default WPF control styles leaked into the dark HMI.
- Reprioritized the next work from recovery-panel implementation to visual-system R&D.

## 2026-07-03 Triage Update after Goal 053

| Priority | Item | Status | Next action |
|---|---|---|---|
| P1 | WPF HMI visual system | Done | Keep gray-base/color-reserved rules |
| P1 | Instrument trend panel | Next | Add pressure/temp/film trend visibility |
| P1 | WPF alarm recovery procedure | Waiting | Add after trend/overview hierarchy is cleaner |
| P2 | Chart/gauge dependency decision | Waiting | Evaluate LiveCharts2 only when trend scope is concrete |

Completed:

- Applied gray-base HMI visual direction to WPF.
- Reduced normal-operation color noise.
- Added text/shape state encoding for valves, measurements, and alarm card.
- Added R&D/design note for custom WPF tokens versus chart/gauge libraries.

## 2026-07-03 Triage Update after Goal 054

| Priority | Item | Status | Next action |
|---|---|---|---|
| P1 | Demo-friendly WPF color direction | Done | Keep green/blue normal-state readability for the portfolio demo |
| P1 | From-zero development explanation | Done | Extend it whenever the user asks about unknown terms |
| P1 | WPF line-by-line learning note | Next | Explain `MainWindow.xaml` and `OperatorConsoleViewModel` in junior-readable Korean |
| P1 | Instrument trend panel | Waiting | Add after the user understands the current WPF layout |
| P1 | WPF alarm recovery procedure | Waiting | Add after trend/learning pass or when demo story needs recovery flow |

Completed:

- Reverted the overly strict gray-base normal-state color direction.
- Kept readability fixes from the previous WPF pass.
- Added `docs/learning/development-from-zero.md` so the project history can be studied from the beginning.
- Updated CI markers to keep the learning note and demo color indicators present.

## 2026-07-03 Triage Update after Goal 055

| Priority | Item | Status | Next action |
|---|---|---|---|
| P0 | Complete development-process guide | Done | Use as the project-level learning spine |
| P1 | `EquipmentStateMachine.cs` line-by-line note | Next | Explain the first Core file in junior-readable Korean |
| P1 | `VirtualIoController.cs` line-by-line note | Waiting | Add after state-machine walkthrough |
| P1 | WPF line-by-line learning note | Waiting | Add after Core basics or when UI debugging is the user's priority |
| P1 | Instrument trend panel | Waiting | Resume feature work after learning debt is reduced |

Completed:

- Added a complete guide explaining the project from the initial Core state-machine direction to the current WPF HMI.
- Added a learning README so future explanation documents have a stable index.
- Updated CI to keep the complete guide present.

## 2026-07-03 Triage Update after Goal 056

| Priority | Item | Status | Next action |
|---|---|---|---|
| P0 | Visual Studio code-reading index | Done | Use as the stable reading path |
| P0 | State-machine line-by-line note | Done | Read with `EquipmentStateMachine.cs` open in Visual Studio |
| P1 | Clock/Timeout line-by-line note | Next | Explain `IClock`, `ManualClock`, `SystemClock`, `StateTimeoutPolicy`, `TimeoutCheckResult` |
| P1 | Virtual IO line-by-line note | Waiting | Add after Clock/Timeout |
| P1 | WPF line-by-line note | Waiting | Add after Core basics or when UI debugging is priority |

Completed:

- Started the code-reading documentation set.
- Added the first detailed guide for the state-machine files.
- Added Visual Studio reading instructions and breakpoint/watch guidance.

## 2026-07-03 Triage Update after Goal 057

| Priority | Item | Status | Next action |
|---|---|---|---|
| P0 | Program-entry reading guide | Done | Start code reading from `000-program-entry.md` |
| P0 | Code-reading order correction | Done | Treat state-machine guide as Core start, not execution start |
| P1 | `OperatorConsoleViewModel.cs` line-by-line note | Next | Continue from `DataContext = new OperatorConsoleViewModel()` |
| P1 | `MainWindow.xaml` binding walkthrough | Waiting | Add after ViewModel control flow |
| P1 | Core state-machine review | Waiting | Revisit after WPF entry/control flow is clear |

Completed:

- Corrected the code-reading sequence after user feedback.
- Added the WPF program entry guide from generated `Main()` through ViewModel initialization and button command flow.
