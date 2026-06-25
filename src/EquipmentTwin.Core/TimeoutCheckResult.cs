namespace EquipmentTwin.Core;

/// <summary>
/// Timeout 검사 결과다.
/// TimedOut이 true면 상태머신은 Alarmed로 전환되며 Transition에 그 기록이 남는다.
/// </summary>
public sealed record TimeoutCheckResult(
    EquipmentState CheckedState,
    bool TimedOut,
    TimeSpan Elapsed,
    TimeSpan? Timeout,
    TransitionResult? Transition);
