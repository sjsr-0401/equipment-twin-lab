namespace EquipmentTwin.Core.Processes;

public sealed record MolyAldRunResult(
    string RecipeName,
    MolyAldRecipe Recipe,
    bool Success,
    MolyAldProcessStep FinalStep,
    MolyAldFaultScenario? FaultScenario,
    double EstimatedThicknessAngstrom,
    IReadOnlyList<MolyAldStepLog> Steps,
    string Message)
{
    public IReadOnlyList<MolyAldStepLog> FailedSteps => Steps.Where(step => !step.Success).ToArray();

    public TimeSpan TotalDuration => Steps.Count == 0
        ? TimeSpan.Zero
        : Steps[^1].CompletedAtUtc - Steps[0].StartedAtUtc;
}
