# Goal 037: Explanatory Unity Demo Screenshot

## 목표

Unity screenshot이 단순한 “렌더링 성공 증거”가 아니라, 처음 보는 사람도 무엇을 보고 있는지 이해할 수 있는 포트폴리오 이미지가 되게 한다.

## 배경

이전 screenshot은 chamber, wafer, gas line, valve가 렌더링되지만 아래 정보가 부족했다.

- 어떤 부품이 무엇인지 알기 어렵다.
- 색상이 무엇을 뜻하는지 알기 어렵다.
- 현재 공정 step과 active valve가 한눈에 들어오지 않는다.
- Core/CLI와 Unity의 역할 분리가 화면에 드러나지 않는다.

## 구현 범위

- Unity primitive visual에 부품 라벨 추가
- status panel 추가
- color key panel 추가
- process flow bar 추가
- Core/CLI와 Unity replay 경계 문구 추가
- README 대표 screenshot 갱신

## 완료 기준

- Unity screenshot에 `Chamber`, `Wafer + film`, `Vacuum gauge`, `Reactant ON`, `Color key`, `Status`, `Process flow`가 보인다.
- `.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot`가 통과한다.
- `docs/demo/moly-ald-demo.png`가 새 screenshot으로 갱신된다.
- 기존 Core 테스트가 깨지지 않는다.

## 이번 Goal에서 하지 않는 것

- 실제 CAD/Blender 모델 import
- 실제 장비 외형 복제
- 실제 증착 물리 시뮬레이션
- 화면 녹화 파일 생성
