namespace EquipmentTwin.Core.Scenarios;

/// <summary>
/// 시나리오 전체 실행 결과다.
/// </summary>
public sealed record ScenarioRunResult(
    string ScenarioName,
    bool Success,
    EquipmentState FinalState,
    IReadOnlyList<ScenarioStepRunResult> Steps)
{
    public IReadOnlyList<ScenarioStepRunResult> FailedSteps => Steps.Where(step => !step.Success).ToArray();
}
