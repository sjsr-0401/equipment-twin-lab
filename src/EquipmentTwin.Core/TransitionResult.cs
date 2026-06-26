using EquipmentTwin.Core.Alarms;

namespace EquipmentTwin.Core;

/// <summary>
/// 이벤트를 적용한 결과다.
/// UI, 로그, 테스트는 이 결과를 보고 장비 상태가 정상적으로 바뀌었는지 판단한다.
/// </summary>
public sealed record TransitionResult(
    EquipmentState PreviousState,
    EquipmentEvent Event,
    EquipmentState CurrentState,
    bool Accepted,
    string Message,
    AlarmInfo? Alarm = null);
