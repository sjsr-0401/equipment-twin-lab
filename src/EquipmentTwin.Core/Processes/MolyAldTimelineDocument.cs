using System.Text.Json;

namespace EquipmentTwin.Core.Processes;

/// <summary>
/// Stable JSON shape for Unity or another visualizer to replay a synthetic ALD run.
/// This document intentionally contains public/synthetic process state only.
/// </summary>
public sealed record MolyAldTimelineDocument(
    string SchemaVersion,
    string Source,
    string RecipeName,
    bool Success,
    string FinalStep,
    string? FaultScenarioName,
    int StationCount,
    int CycleCount,
    double TargetThicknessAngstrom,
    double EstimatedThicknessAngstrom,
    double TotalDurationMilliseconds,
    IReadOnlyList<string> PublicBasis,
    IReadOnlyList<MolyAldTimelineStep> Steps)
{
    public const string CurrentSchemaVersion = "equipment-twin.moly-ald.timeline.v1";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public static MolyAldTimelineDocument FromRunResult(MolyAldRunResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return new MolyAldTimelineDocument(
            CurrentSchemaVersion,
            "public-synthetic-moly-ald",
            result.RecipeName,
            result.Success,
            result.FinalStep.ToString(),
            result.FaultScenario?.Name,
            result.Recipe.StationCount,
            result.Recipe.CycleCount,
            result.Recipe.TargetThicknessAngstrom,
            result.EstimatedThicknessAngstrom,
            result.TotalDuration.TotalMilliseconds,
            result.Recipe.PublicBasis.ToArray(),
            result.Steps.Select(MolyAldTimelineStep.FromStepLog).ToArray());
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this, JsonOptions);
    }
}
