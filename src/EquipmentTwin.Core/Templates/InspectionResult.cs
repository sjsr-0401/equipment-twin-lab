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
        IReadOnlyDictionary<string, double> measurements)
    {
        ProductCode = productCode;
        Mode = mode;
        Outcome = outcome;
        DefectCode = defectCode;
        Message = message;
        Measurements = new Dictionary<string, double>(measurements, StringComparer.OrdinalIgnoreCase);
    }

    public string ProductCode { get; }

    public InspectionMode Mode { get; }

    public InspectionOutcome Outcome { get; }

    public string DefectCode { get; }

    public string Message { get; }

    public IReadOnlyDictionary<string, double> Measurements { get; }

    public bool Passed => Outcome == InspectionOutcome.Pass;

    public static InspectionResult FromRecipe(ProductRecipe recipe)
    {
        ArgumentNullException.ThrowIfNull(recipe);

        if (recipe.InspectionMode == InspectionMode.None)
        {
            throw new InvalidOperationException($"Product recipe '{recipe.Name}' does not require inspection.");
        }

        var spec = recipe.InspectionResult
            ?? throw new InvalidOperationException($"Product recipe '{recipe.Name}' requires an inspection result.");
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
            spec.Measurements);
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
