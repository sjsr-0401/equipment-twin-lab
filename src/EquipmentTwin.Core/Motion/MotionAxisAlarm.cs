namespace EquipmentTwin.Core.Motion;

/// <summary>
/// 모션 축 알람의 코드와 메시지를 함께 기록한다.
/// </summary>
public sealed record MotionAxisAlarm(
    MotionAxisAlarmCode Code,
    string Message)
{
    public static MotionAxisAlarm None { get; } = new(MotionAxisAlarmCode.None, "No active motion alarm.");
}
