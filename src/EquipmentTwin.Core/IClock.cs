namespace EquipmentTwin.Core;

/// <summary>
/// 현재 시간을 제공하는 인터페이스다.
/// 실제 실행에서는 시스템 시간을 쓰고, 테스트/시뮬레이션에서는 수동 Clock을 써서 시간을 제어한다.
/// </summary>
public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
