namespace EquipmentTwin.Core.Templates;

/// <summary>
/// TemplateRunner가 recipe 실행 후 남기는 제품 검사 결과다.
/// </summary>
public sealed class InspectionResult
{
    public InspectionResult(
        string productCode,
        InspectionMode mode,
        InspectionOutcome outcome,
        string defectCode,
        string message,
        IReadOnlyDictionary<string, double> measurements,
        string scenarioName = "")
    {
        ProductCode = productCode;
        Mode = mode;
        Outcome = outcome;
        DefectCode = defectCode;
        Message = message;
        Measurements = new Dictionary<string, double>(measurements, StringComparer.OrdinalIgnoreCase);
        ScenarioName = scenarioName;
    }

    public string ProductCode { get; }

    public InspectionMode Mode { get; }

    public InspectionOutcome Outcome { get; }

    public string DefectCode { get; }

    public string Message { get; }

    public IReadOnlyDictionary<string, double> Measurements { get; }

    public bool Passed => Outcome == InspectionOutcome.Pass;

    public string ScenarioName { get; }

    public static InspectionResult FromRecipe(ProductRecipe recipe, string? inspectionScenarioName = null)
    {
        ArgumentNullException.ThrowIfNull(recipe);

        if (recipe.InspectionMode == InspectionMode.None)
        {
            throw new InvalidOperationException($"Product recipe '{recipe.Name}' does not require inspection.");
        }

        var spec = recipe.ResolveInspectionResult(inspectionScenarioName);
        spec.Validate(recipe.Name);

        var message = string.IsNullOrWhiteSpace(spec.Message)
            ? spec.Outcome == InspectionOutcome.Pass ? "Inspection passed." : "Inspection failed."
            : spec.Message;

        return new InspectionResult(
            recipe.ProductCode,
            recipe.InspectionMode,
            spec.Outcome,
            spec.DefectCode,
            message,
            spec.Measurements,
            inspectionScenarioName ?? string.Empty);
    }

    public bool TryGetMeasurement(string name, out double value)
    {
        foreach (var measurement in Measurements)
        {
            if (string.Equals(measurement.Key, name, StringComparison.OrdinalIgnoreCase))
            {
                value = measurement.Value;
                return true;
            }
        }

        value = default;
        return false;
    }
}
