namespace EquipmentTwin.Core;

/// <summary>
/// 장비 셀 컨트롤러가 IO를 읽고 상태머신에 반영한 결과다.
/// Transition이 null이면 이번 평가에서는 상태 전이가 없었다.
/// </summary>
public sealed record EquipmentCellStepResult(
    EquipmentEvent? Event,
    TransitionResult? Transition,
    TimeoutCheckResult? Timeout,
    string Message)
{
    public bool Changed => Transition?.Accepted == true || Timeout?.TimedOut == true;

    public static EquipmentCellStepResult NoChange(string message)
    {
        return new EquipmentCellStepResult(null, null, null, message);
    }

    public static EquipmentCellStepResult FromTransition(EquipmentEvent equipmentEvent, TransitionResult transition)
    {
        return new EquipmentCellStepResult(equipmentEvent, transition, null, transition.Message);
    }

    public static EquipmentCellStepResult FromTimeout(TimeoutCheckResult timeout)
    {
        return new EquipmentCellStepResult(EquipmentEvent.Timeout, timeout.Transition, timeout, timeout.Transition?.Message ?? "Timeout checked.");
    }
}
