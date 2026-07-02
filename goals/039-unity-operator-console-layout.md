# Goal 039: Unity Operator Console Layout

## 목표

기존 Unity screenshot을 단순한 3D primitive render에서 “사용자가 장비를 조작하는 프로그램 화면”처럼 보이도록 개선한다.

화면 구도는 다음처럼 잡는다.

```text
왼쪽: 3D equipment view
오른쪽: operator interface
아래: process timeline + event summary
```

## 디자인 방향

공개된 ALD/metallization 장비 자료에서 볼 수 있는 장비다운 분위기만 참고한다.

- 큰 cabinet 형태
- load port
- process chamber
- gas / valve panel
- vacuum pump / exhaust
- alarm beacon
- operator button row
- recipe / live telemetry / alarm card
- process timeline

실제 vendor CAD, 실제 장비 내부 구조, 실제 UI, 실제 recipe, 실제 alarm table은 복제하지 않는다.

## 구현한 것

- `MolyAldPrimitiveVisualizer`를 user-friendly operator console layout으로 재구성했다.
- 왼쪽 3D 장비 영역을 추가했다.
- 오른쪽 operator interface를 추가했다.
- 아래 timeline 영역을 개선했다.
- 카메라를 orthographic 기반으로 조정해 UI layout처럼 보이게 했다.
- Unity screenshot을 다시 생성해 `docs/demo/moly-ald-demo.png`에 반영했다.

## 설계 원칙

Unity는 여전히 공정을 계산하지 않는다.

```text
Core/CLI
    -> timeline JSON
    -> MolyAldProcessPlayer
    -> MolyAldVisualStateMapper
    -> MolyAldPrimitiveVisualizer
```

이번 작업은 visual layer만 바꾼다.
공정 로직, fault 판단, telemetry 값은 기존 timeline에서 온다.

## 검증

- `.\scripts\Invoke-UnitySmokeTest.ps1 -CaptureScreenshot`
- `dotnet build EquipmentTwinLab.sln --no-restore --configuration Release`
- `dotnet run --project tests\EquipmentTwin.Core.Tests\EquipmentTwin.Core.Tests.csproj --no-restore --configuration Release`
- `git diff --check`

