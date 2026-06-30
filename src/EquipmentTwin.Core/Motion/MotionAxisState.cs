namespace EquipmentTwin.Core.Motion;

/// <summary>
/// 가상 모션 축의 현재 상태다.
/// 실제 서보 드라이브 상태 전체가 아니라, 시뮬레이션과 검증에 필요한 최소 상태만 둔다.
/// </summary>
public enum MotionAxisState
{
    Disabled,
    Ready,
    Homing,
    Moving,
    InPosition,
    Alarmed
}
