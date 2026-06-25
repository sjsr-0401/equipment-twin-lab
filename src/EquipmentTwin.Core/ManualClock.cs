namespace EquipmentTwin.Core;

/// <summary>
/// 테스트와 시뮬레이션에서 시간을 직접 전진시키기 위한 Clock이다.
/// 실제로 기다리지 않고 Timeout 상황을 만들 수 있다.
/// </summary>
public sealed class ManualClock : IClock
{
    public ManualClock(DateTimeOffset initialUtc)
    {
        UtcNow = initialUtc;
    }

    public DateTimeOffset UtcNow { get; private set; }

    public void Advance(TimeSpan duration)
    {
        if (duration < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(duration), "Clock cannot move backward.");
        }

        UtcNow = UtcNow.Add(duration);
    }
}
