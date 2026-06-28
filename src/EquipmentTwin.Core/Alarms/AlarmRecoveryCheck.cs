namespace EquipmentTwin.Core.Alarms;

/// <summary>
/// 현재 알람을 해제해도 되는지 판단한 결과다.
/// </summary>
public sealed record AlarmRecoveryCheck(bool CanClear, string Message)
{
    public static AlarmRecoveryCheck Ready(string message)
    {
        return new AlarmRecoveryCheck(true, message);
    }

    public static AlarmRecoveryCheck Blocked(string message)
    {
        return new AlarmRecoveryCheck(false, message);
    }
}
