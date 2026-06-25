# Goal 003: GitHub Actions CI

작성일: 2026-06-25

## 목표

GitHub에 push 또는 PR이 올라올 때마다 .NET 빌드와 테스트가 자동으로 실행되게 한다.

## 왜 필요한가

Loop Engineering을 운영하려면 에이전트가 만든 변경이 실제로 깨졌는지 자동으로 확인하는 기준이 필요하다.

CI가 없으면 “빌드됨”, “테스트 통과”가 대화 속 주장으로 남는다. CI가 있으면 GitHub PR에서 증거로 남는다.

## 범위

포함:

- GitHub Actions 워크플로 추가
- push/PR 트리거 설정
- .NET 8 설치
- restore/build/test 실행
- 문서와 작업 로그 갱신

제외:

- 자동 병합
- 배포
- Unity 빌드
- 테스트 커버리지 리포트

## 위험 등급

- [x] L0 문서/자동 검증 설정
- [ ] L1 테스트/샘플/작은 리팩터링
- [ ] L2 핵심 구조/상태머신/검사 로직
- [ ] L3 안전, 인터록, 알람 우선순위, 데이터 삭제

## 완료 기준

- `.github/workflows/ci.yml`이 있다.
- GitHub Actions에서 push/PR마다 빌드와 테스트가 실행된다.
- 로컬에서도 기존 빌드/테스트가 통과한다.
- PR #1에서 CI 결과를 확인할 수 있다.

## 검증 방법

로컬:

```powershell
dotnet build --no-restore
dotnet run --project tests\EquipmentTwin.Core.Tests --no-restore
```

GitHub:

```text
https://github.com/sjsr-0401/equipment-twin-lab/actions
```
