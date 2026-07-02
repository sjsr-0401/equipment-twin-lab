# Goal 040: UI/UX Agent Review System

## 목표

Unity 화면 품질을 “감으로 예쁘게 만들기”에서 “UI/UX 기준으로 리뷰하고 고치기”로 바꾼다.

이번 Goal은 별도 AI agent나 Claude/Codex에게 같은 역할을 줄 수 있도록 UI/UX reviewer brief, review template, 현재 screenshot 1차 리뷰, Canvas 전환 설계를 문서화한다.

## 왜 필요한가

현재 Unity 화면은 기능적으로는 발전했지만, 전문가 시선에서는 아직 다음 문제가 있다.

- TextMesh 기반 UI라 글씨 품질이 약하다.
- 색상과 정보 위계가 HMI 기준으로 정리되지 않았다.
- 장비 3D와 operator UI가 같은 3D 공간에서 경쟁한다.
- alarm/fault가 telemetry보다 강한 우선순위를 갖지 못한다.

## 추가한 문서

| File | Purpose |
|---|---|
| `docs/design/uiux-agent-brief.md` | UI/UX reviewer 역할과 출력 형식 |
| `docs/design/uiux-review-template.md` | 매 screenshot마다 반복 사용할 review 양식 |
| `docs/design/uiux-review-log.md` | 현재 screenshot에 대한 1차 냉정 리뷰 |
| `docs/design/operator-console-design-system.md` | 색상, typography, layout, Canvas 전환 기준 |

## 핵심 판단

다음 구현은 3D TextMesh UI를 계속 키우는 방향이 아니라 Canvas 기반 operator panel로 전환한다.

```text
Unity 3D Scene
    -> equipment body

Unity Canvas
    -> operator panel
    -> telemetry
    -> alarm
    -> buttons
    -> timeline
```

## 이번 Goal에서 하지 않는 것

- Canvas UI 실제 구현
- 버튼 실제 interaction 연결
- 새 screenshot 생성
- 3D 장비 mesh 품질 개선

## 다음 권장 Goal

```text
Goal 041: Canvas Operator Panel Implementation
```

Acceptance criteria:

- 오른쪽 operator panel은 Canvas 기반이다.
- current step, telemetry, alarm, timeline은 Canvas text로 표시된다.
- TextMesh는 장비 label 수준으로만 남긴다.
- screenshot 50% 축소 상태에서도 current step과 alarm state가 읽힌다.

