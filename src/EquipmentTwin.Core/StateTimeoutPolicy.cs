namespace EquipmentTwin.Core;

/// <summary>
/// 장비 상태별 허용 시간을 정의한다.
/// 예를 들어 Loading 상태에서 정해진 시간 안에 LoadComplete가 오지 않으면 Timeout으로 판단할 수 있다.
/// </summary>
public sealed class StateTimeoutPolicy
{
    private readonly Dictionary<EquipmentState, TimeSpan> _timeouts = new();

    public IReadOnlyDictionary<EquipmentState, TimeSpan> Timeouts => _timeouts;

    public void SetTimeout(EquipmentState state, TimeSpan timeout)
    {
        if (timeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be greater than zero.");
        }

        if (state == EquipmentState.Alarmed)
        {
            throw new InvalidOperationException("Alarmed state cannot have a timeout rule.");
        }

        _timeouts[state] = timeout;
    }

    public bool TryGetTimeout(EquipmentState state, out TimeSpan timeout)
    {
        return _timeouts.TryGetValue(state, out timeout);
    }

    public static StateTimeoutPolicy CreateDefaultMvpPolicy()
    {
        var policy = new StateTimeoutPolicy();

        policy.SetTimeout(EquipmentState.Loading, TimeSpan.FromSeconds(30));
        policy.SetTimeout(EquipmentState.Aligning, TimeSpan.FromSeconds(15));
        policy.SetTimeout(EquipmentState.Inspecting, TimeSpan.FromSeconds(60));
        policy.SetTimeout(EquipmentState.Unloading, TimeSpan.FromSeconds(30));

        return policy;
    }
}
