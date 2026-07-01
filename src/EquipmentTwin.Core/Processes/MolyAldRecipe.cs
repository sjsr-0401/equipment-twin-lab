using System.Text.Json;
using System.Text.Json.Serialization;

namespace EquipmentTwin.Core.Processes;

/// <summary>
/// Public/synthetic recipe for a molybdenum ALD metallization demo.
/// Values are portfolio-demo values, not real fab or vendor process parameters.
/// </summary>
public sealed class MolyAldRecipe
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public List<string> PublicBasis { get; init; } = new();

    public int StationCount { get; init; } = 4;

    public int CycleCount { get; init; }

    public double TargetThicknessAngstrom { get; init; }

    public double ChamberPressureMtorr { get; init; }

    public double WaferTemperatureC { get; init; }

    public double PumpDownSeconds { get; init; }

    public double TemperatureStabilizeSeconds { get; init; }

    public double MetalPrecursorDoseMilliseconds { get; init; }

    public double ReactantDoseMilliseconds { get; init; }

    public double PurgeMilliseconds { get; init; }

    public double CarrierGasFlowSccm { get; init; }

    public List<MolyAldGasFlow> ReactantGases { get; init; } = new();

    public List<MolyAldFaultScenario> FaultScenarios { get; init; } = new();

    public static MolyAldRecipe FromJson(string json)
    {
        var recipe = JsonSerializer.Deserialize<MolyAldRecipe>(json, JsonOptions)
            ?? throw new InvalidOperationException("Moly ALD recipe JSON is empty.");

        recipe.Validate();
        return recipe;
    }

    public string ToJson()
    {
        Validate();
        return JsonSerializer.Serialize(this, JsonOptions);
    }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidOperationException("Moly ALD recipe name is required.");
        }

        if (StationCount <= 0 || StationCount > 8)
        {
            throw new InvalidOperationException($"Moly ALD recipe '{Name}' stationCount must be between 1 and 8.");
        }

        if (CycleCount <= 0 || CycleCount > 10000)
        {
            throw new InvalidOperationException($"Moly ALD recipe '{Name}' cycleCount must be between 1 and 10000.");
        }

        ValidatePositive(TargetThicknessAngstrom, nameof(TargetThicknessAngstrom));
        ValidateRange(ChamberPressureMtorr, 1, 5000, nameof(ChamberPressureMtorr));
        ValidateRange(WaferTemperatureC, 0, 600, nameof(WaferTemperatureC));
        ValidatePositive(PumpDownSeconds, nameof(PumpDownSeconds));
        ValidatePositive(TemperatureStabilizeSeconds, nameof(TemperatureStabilizeSeconds));
        ValidatePositive(MetalPrecursorDoseMilliseconds, nameof(MetalPrecursorDoseMilliseconds));
        ValidatePositive(ReactantDoseMilliseconds, nameof(ReactantDoseMilliseconds));
        ValidatePositive(PurgeMilliseconds, nameof(PurgeMilliseconds));
        ValidatePositive(CarrierGasFlowSccm, nameof(CarrierGasFlowSccm));

        if (ReactantGases.Count == 0)
        {
            throw new InvalidOperationException($"Moly ALD recipe '{Name}' requires at least one reactant gas.");
        }

        var gasNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (var index = 0; index < ReactantGases.Count; index++)
        {
            var gas = ReactantGases[index];
            gas.Validate(Name, index);

            if (!gasNames.Add(gas.Name))
            {
                throw new InvalidOperationException($"Moly ALD recipe '{Name}' contains duplicate gas '{gas.Name}'.");
            }
        }

        foreach (var basis in PublicBasis)
        {
            if (string.IsNullOrWhiteSpace(basis))
            {
                throw new InvalidOperationException($"Moly ALD recipe '{Name}' contains a blank publicBasis entry.");
            }
        }

        var faultNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (var index = 0; index < FaultScenarios.Count; index++)
        {
            var fault = FaultScenarios[index];
            fault.Validate(Name, index, CycleCount);

            if (!faultNames.Add(fault.Name))
            {
                throw new InvalidOperationException($"Moly ALD recipe '{Name}' contains duplicate fault scenario '{fault.Name}'.");
            }
        }
    }

    public MolyAldFaultScenario FindFaultScenario(string faultName)
    {
        if (string.IsNullOrWhiteSpace(faultName))
        {
            throw new ArgumentException("Fault scenario name is required.", nameof(faultName));
        }

        return FaultScenarios.FirstOrDefault(fault =>
            string.Equals(fault.Name, faultName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"Moly ALD fault scenario '{faultName}' was not found in recipe '{Name}'.");
    }

    private void ValidatePositive(double value, string propertyName)
    {
        if (value <= 0 || double.IsNaN(value) || double.IsInfinity(value))
        {
            throw new InvalidOperationException($"Moly ALD recipe '{Name}' requires positive {propertyName}.");
        }
    }

    private void ValidateRange(double value, double min, double max, string propertyName)
    {
        if (value < min || value > max || double.IsNaN(value) || double.IsInfinity(value))
        {
            throw new InvalidOperationException($"Moly ALD recipe '{Name}' {propertyName} must be between {min} and {max}.");
        }
    }
}
