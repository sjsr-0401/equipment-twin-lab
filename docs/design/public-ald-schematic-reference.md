# Public ALD Schematic Reference

이 문서는 Unity 데모의 `Process Schematic Main View`가 어떤 공개 자료와 어떤 추상화 기준을 따르는지 정리한다.

목표는 실제 Lam/ALTUS/Halo/Halo HX 장비 내부 구조를 복제하는 것이 아니다.
목표는 공개적으로 설명 가능한 ALD/metallization 개념을 HMI 스키매틱으로 표현하는 것이다.

## Public sources used

| Source | Publicly usable point |
|---|---|
| Lam Research ALTUS Product Family | ALTUS는 CVD/ALD 기술을 결합해 advanced metallization용 conformal film을 증착한다 |
| Lam Research ALTUS Halo blog / press release | ALTUS Halo는 molybdenum ALD metallization과 feature fill, low-resistance metallization 방향성을 가진다 |
| Lam Research Metallization page | Mo ALD manufacturing에는 high temperature, reactor/process sequence design, wafer temperature control, precursor handling 같은 과제가 있다 |
| Oxford Instruments ALD overview | ALD는 ultra-thin film, precise thickness control, conformal coating, high-aspect-ratio coverage를 목적으로 한다 |

References:

- https://www.lamresearch.com/product/altus-product-family/
- https://newsroom.lamresearch.com/everything-about-altus-halo
- https://newsroom.lamresearch.com/2025-02-19-Lam-Research-Ushers-in-New-Era-of-Semiconductor-Metallization-with-ALTUS-R-Halo-for-Molybdenum-Atomic-Layer-Deposition
- https://www.lamresearch.com/products/our-solutions/metallization/
- https://plasma.oxinst.com/technology/atomic-layer-deposition

## Schematic abstraction

Unity 화면에서 표현할 수 있는 공개/일반 구성요소:

- gas delivery lines
- precursor/reactant/purge valve symbols
- sealed vacuum chamber
- showerhead/gas distributor
- wafer + film
- susceptor heater
- pressure/temperature tap
- exhaust line
- gate valve
- vacuum pump

표현하지 않을 것:

- 실제 vendor CAD geometry
- 실제 chamber internal dimensions
- 실제 recipe parameter
- 실제 gas/precursor name
- 실제 interlock map
- 실제 tool UI layout
- 실제 station/module arrangement

## Visual direction

3D blockout은 장비 외형을 흉내 내는 메인 화면으로 쓰지 않는다.

메인 화면:

```text
2D Process Schematic
    -> valve state
    -> gas path
    -> chamber/wafer/film state
    -> exhaust/pump path
```

보조 화면:

```text
3D Cutaway / debug view
    -> future optional view
    -> state visualization only
    -> not CAD realism
```

## Engineering value

이 방향은 포트폴리오에서 다음을 보여준다.

- process sequence를 UI state로 변환하는 능력
- valve/pressure/temp/film 상태를 operator가 읽을 수 있게 만드는 능력
- 실제 장비가 없어도 testable digital twin 구조를 만드는 능력
- 공개 자료와 사내/비공개 지식을 명확히 분리하는 판단력
