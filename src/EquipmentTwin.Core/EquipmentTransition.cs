using EquipmentTwin.Core.Alarms;

namespace EquipmentTwin.Core;

/// <summary>
/// 상태 전이 시도 기록이다.
/// Accepted가 false면 상태는 바뀌지 않았고, 잘못된 이벤트가 들어온 것이다.
/// </summary>
public sealed record EquipmentTransition(
    EquipmentState From,
    EquipmentEvent Event,
    EquipmentState To,
    bool Accepted,
    string Message,
    AlarmInfo? Alarm = null);
