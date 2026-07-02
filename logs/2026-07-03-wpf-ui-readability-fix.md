# 2026-07-03 WPF UI 가독성 수정 작업 로그

## 사용자가 발견한 문제

Visual Studio에서 WPF HMI를 실제로 띄워본 결과 다음 문제가 보였다.

- 왼쪽 `LOAD PORT` 글씨가 잘려서 안 보임
- 하단 `TIMELINE DEBUG TABLE` 헤더가 흰색 배경/밝은 글씨 조합으로 보여서 안 읽힘
- 전체적으로 WPF 기본 컨트롤 느낌이 강해서 HMI 화면으로는 고급스럽지 않음

## 원인

WPF는 기본 컨트롤이 운영체제 테마를 많이 따른다.

그래서 화면 전체는 다크 테마로 만들었어도, `DataGridColumnHeader`, `DataGridCell`, `ComboBox` 같은 세부 컨트롤은 기본 윈도우 스타일이 섞일 수 있다.

이번 문제는 로직 문제가 아니라 UI 스타일을 충분히 명시하지 않은 문제다.

## 수정한 내용

### 1. LOAD PORT 라벨 클리핑 수정

기존에는 박스 높이에 비해 내부 여백과 라벨 위치가 커서 글씨가 아래로 잘렸다.

수정:

- load-port 박스 크기 증가
- 내부 margin 감소
- rectangle 높이 감소
- 라벨에 `FontWeight=SemiBold` 적용

### 2. PROCESS TIMELINE 텍스트 간격 수정

기존 `DockPanel` 구조에서는 `PROCESS TIMELINE`과 `100%`가 붙어 보일 수 있었다.

수정:

- 두 컬럼 `Grid`로 변경
- 왼쪽: 라벨
- 오른쪽: 퍼센트 값
- 퍼센트 값에 좌측 margin 추가

### 3. Timeline Debug Table 다크 테마 고정

기존 DataGrid는 `Background`와 `Foreground`만 지정되어 있었다.

그 결과 헤더/셀/선택 상태는 기본 윈도우 스타일을 타면서 흰색 배경 충돌이 발생했다.

수정:

- `DataGridColumnHeader` 스타일 추가
- `DataGridRow` 스타일 추가
- `DataGridCell` 스타일 추가
- grid line 색상 지정
- row/header height 지정
- table border 지정

## 소프트웨어적으로 배운 점

WPF에서 다크 테마를 만들 때는 최상위 배경만 바꾸면 부족하다.

특히 다음 컨트롤은 반드시 세부 스타일을 잡아야 한다.

- `DataGrid`
- `ComboBox`
- `Button`
- `TabControl`
- `ScrollBar`
- `Menu`

이번에는 DataGrid와 ComboBox의 최소 스타일을 잡았다.

## 남은 한계

이번 작업은 "안 보이는 문제"를 해결하는 수준이다.

아직 전문적인 HMI 디자인 시스템은 아니다. 다음에는 별도 R&D로 다음을 정해야 한다.

- 글씨 크기 규칙
- 색상 의미 규칙
- 카드/패널 여백
- 알람 강조 방식
- 계기값 정상범위 표시 방식
- 실제 장비 HMI처럼 보이는 레이아웃 기준

## 검증

- WPF Release 빌드 통과
- `git diff --check` 통과
