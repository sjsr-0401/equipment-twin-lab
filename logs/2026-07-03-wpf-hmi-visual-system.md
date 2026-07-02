# 2026-07-03 WPF HMI Visual System 작업 로그

## 작업 배경

사용자가 산업 HMI에서는 색을 많이 쓰는 것이 오히려 아마추어처럼 보일 수 있다고 지적했다.

핵심 방향:

- 화면 대부분은 무채색 기반
- 빨강은 알람
- 노랑은 경고
- 파랑은 조작 필요
- 정상 상태는 가능한 한 회색/텍스트로 표현
- 색각 이상 사용자를 고려해 색만으로 상태를 표현하지 않음

## 이번 작업에서 바꾼 것

### 1. 색상 팔레트 변경

기존에는 정상 상태도 초록/파랑/노랑이 많이 보였다.

수정 후:

- 정상 진행률: 회색
- 정상 알람 카드: 회색
- 정상 밸브 상태: 회색 + `OPEN` / `CLOSED`
- 알람 상태: 붉은 계열
- 경고 상태: 노란 계열

### 2. 밸브 이중 부호화

기존:

```text
색으로 ON/OFF 구분
```

수정:

```text
PRE OPEN / PRE CLOSED
RCT OPEN / RCT CLOSED
PRG OPEN / PRG CLOSED
```

색 없이도 상태를 읽을 수 있게 했다.

### 3. 계기 상태 이중 부호화

기존:

```text
OK / HI / COOL / TARGET
```

수정:

```text
▲ HI
▼ COOL
OK
OK TARGET
… GROWING
```

상태 텍스트 자체에 방향/형태 정보를 넣었다.

### 4. 알람 카드 변경

기존 `NO ALARM`은 초록 계열이라 정상 상태도 강한 색 신호처럼 보였다.

수정:

- 정상: `NO ACTIVE ALARM`, 회색 카드
- 알람: `ALARM ACTIVE`, 붉은 카드
- 아이콘/우선순위/코드/텍스트를 같이 보여줌

## 소프트웨어 구조 관점

이번 작업은 비즈니스 로직이 아니라 ViewModel의 표현 속성과 XAML 리소스 토큰을 수정했다.

중요한 분리:

```text
Core = 공정 truth
ViewModel = 상태를 HMI 표현으로 변환
XAML = 그 표현을 화면에 배치
```

색상 판단이 Core에 들어가면 안 된다. Core는 공정 상태만 알고, HMI 표현은 WPF 계층이 담당한다.

## 남은 한계

- 아직 실제 트렌드 차트가 없다.
- 게이지/차트 라이브러리는 아직 추가하지 않았다.
- 현재 화면은 Level 1/2/3이 한 화면에 섞여 있다.
- 다음에는 overview와 debug table을 더 명확히 분리해야 한다.

## 검증

- WPF 프로젝트 Release 빌드 통과
- 전체 솔루션 Release 빌드 통과
- Core 테스트 81개 통과
- WPF 시작 스모크 통과
- `git diff --check` 통과

## 다음 작업 후보

```text
Goal 054: WPF HMI Instrument Trend Panel
```

압력/온도/막두께의 짧은 트렌드를 넣고, LiveCharts2를 쓸지 직접 그릴지 결정한다.
