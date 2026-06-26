namespace EquipmentTwin.Core.Alarms;

/// <summary>
/// 알람 코드와 사용자/로그용 메시지를 함께 담는다.
/// </summary>
public sealed record AlarmInfo(
    AlarmCode Code,
    string Message,
    EquipmentEvent SourceEvent)
{
    public static AlarmInfo None { get; } = new(AlarmCode.None, "No active alarm.", EquipmentEvent.Reset);

    public static AlarmInfo FromEvent(EquipmentEvent equipmentEvent, EquipmentState state)
    {
        return equipmentEvent switch
        {
            EquipmentEvent.DoorOpened => new AlarmInfo(
                AlarmCode.DoorOpened,
                "Door opened during operation.",
                equipmentEvent),
            EquipmentEvent.EmergencyStop => new AlarmInfo(
                AlarmCode.EmergencyStop,
                "Emergency stop requested.",
                equipmentEvent),
            EquipmentEvent.Timeout => new AlarmInfo(
                AlarmCode.StateTimeout,
                $"State '{state}' timed out.",
                equipmentEvent),
            _ => throw new InvalidOperationException($"'{equipmentEvent}' is not an alarm event.")
        };
    }
}
