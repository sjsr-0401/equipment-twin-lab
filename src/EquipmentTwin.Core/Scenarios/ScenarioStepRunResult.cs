namespace EquipmentTwin.Core.Scenarios;

/// <summary>
/// 시나리오 한 단계 실행 결과다.
/// </summary>
public sealed record ScenarioStepRunResult(
    int Index,
    string Name,
    ScenarioStepAction Action,
    bool Success,
    EquipmentState StateAfterStep,
    string Message,
    IReadOnlyList<string> ValidationErrors);
