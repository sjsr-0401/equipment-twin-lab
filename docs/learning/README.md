# Equipment Twin Lab 학습 문서

이 폴더는 코드를 “만들어달라고 맡기는 것”이 아니라, 나중에 직접 유지보수할 수 있도록 이해를 쌓기 위한 문서다.

읽는 순서는 아래가 좋다.

## 1. 빠른 전체 그림

- [development-from-zero.md](./development-from-zero.md)

프로젝트를 왜 시작했고, Core -> CLI -> Unity -> WPF 순서로 왜 이동했는지 빠르게 훑는다.

## 2. 전체 개발 과정 상세 해설

- [project-development-complete-guide.md](./project-development-complete-guide.md)

이 문서가 메인이다.

처음 상태머신을 만든 순간부터 WPF HMI까지, 어떤 생각으로 파일을 만들고 코드를 확장했는지 설명한다.

## 3. 앞으로 추가할 학습 문서

아직 별도 문서로 분리하면 좋은 것들:

- `MainWindow.xaml` 줄 단위 해설
- `OperatorConsoleViewModel` 줄 단위 해설
- `MolyAldRunner` 공정 실행 흐름 해설
- 테스트 코드 작성법 해설
- Visual Studio 디버깅 실습 문서
