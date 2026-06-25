using System.Text.Json;
using System.Text.Json.Serialization;

namespace EquipmentTwin.Core.Scenarios;

/// <summary>
/// JSON으로 표현하는 장비 운전 시나리오다.
/// </summary>
public sealed class EquipmentScenario
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public List<ScenarioStep> Steps { get; init; } = new();

    public static EquipmentScenario FromJson(string json)
    {
        var scenario = JsonSerializer.Deserialize<EquipmentScenario>(json, JsonOptions)
            ?? throw new InvalidOperationException("Scenario JSON is empty.");

        scenario.Validate();
        return scenario;
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
            throw new InvalidOperationException("Scenario name is required.");
        }

        if (Steps.Count == 0)
        {
            throw new InvalidOperationException("Scenario must contain at least one step.");
        }

        for (var index = 0; index < Steps.Count; index++)
        {
            Steps[index].Validate(index);
        }
    }
}
