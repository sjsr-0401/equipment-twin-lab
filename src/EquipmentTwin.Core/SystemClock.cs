namespace EquipmentTwin.Core;

/// <summary>
/// 운영체제의 실제 UTC 시간을 사용하는 Clock이다.
/// </summary>
public sealed class SystemClock : IClock
{
    public static SystemClock Instance { get; } = new();

    private SystemClock()
    {
    }

    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
