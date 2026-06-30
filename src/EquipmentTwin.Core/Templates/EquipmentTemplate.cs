using System.Text.Json;
using System.Text.Json.Serialization;
using EquipmentTwin.Core.Motion;

namespace EquipmentTwin.Core.Templates;

/// <summary>
/// 사용자가 선택할 수 있는 장비 구성의 최소 단위다.
/// </summary>
public sealed class EquipmentTemplate
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public List<MotionAxisTemplate> MotionAxes { get; init; } = new();

    public List<ProductRecipe> ProductRecipes { get; init; } = new();

    public List<FaultScenario> FaultScenarios { get; init; } = new();

    public static EquipmentTemplate FromJson(string json)
    {
        var template = JsonSerializer.Deserialize<EquipmentTemplate>(json, JsonOptions)
            ?? throw new InvalidOperationException("Equipment template JSON is empty.");

        template.Validate();
        return template;
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
            throw new InvalidOperationException("Equipment template name is required.");
        }

        if (MotionAxes.Count == 0)
        {
            throw new InvalidOperationException($"Equipment template '{Name}' requires at least one motion axis.");
        }

        var axisNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (var index = 0; index < MotionAxes.Count; index++)
        {
            var axis = MotionAxes[index];
            axis.Validate(index);

            if (!axisNames.Add(axis.Name))
            {
                throw new InvalidOperationException($"Equipment template '{Name}' contains duplicate motion axis '{axis.Name}'.");
            }
        }

        if (ProductRecipes.Count == 0)
        {
            throw new InvalidOperationException($"Equipment template '{Name}' requires at least one product recipe.");
        }

        var recipeNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (var index = 0; index < ProductRecipes.Count; index++)
        {
            var recipe = ProductRecipes[index];
            recipe.Validate(index, axisNames);

            if (!recipeNames.Add(recipe.Name))
            {
                throw new InvalidOperationException($"Equipment template '{Name}' contains duplicate product recipe '{recipe.Name}'.");
            }
        }

        var faultNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (var index = 0; index < FaultScenarios.Count; index++)
        {
            var fault = FaultScenarios[index];
            fault.Validate(index, axisNames);

            if (!faultNames.Add(fault.Name))
            {
                throw new InvalidOperationException($"Equipment template '{Name}' contains duplicate fault scenario '{fault.Name}'.");
            }
        }
    }

    public IReadOnlyDictionary<string, MotionAxis> CreateMotionAxes(IClock clock)
    {
        ArgumentNullException.ThrowIfNull(clock);
        Validate();

        return MotionAxes.ToDictionary(
            axis => axis.Name,
            axis => new MotionAxis(axis.Name, clock, axis.InitialPosition),
            StringComparer.OrdinalIgnoreCase);
    }

    public ProductRecipe FindProductRecipe(string recipeName)
    {
        if (string.IsNullOrWhiteSpace(recipeName))
        {
            throw new ArgumentException("Recipe name is required.", nameof(recipeName));
        }

        return ProductRecipes.FirstOrDefault(recipe =>
            string.Equals(recipe.Name, recipeName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"Product recipe '{recipeName}' was not found in template '{Name}'.");
    }

    public FaultScenario FindFaultScenario(string faultName)
    {
        if (string.IsNullOrWhiteSpace(faultName))
        {
            throw new ArgumentException("Fault scenario name is required.", nameof(faultName));
        }

        return FaultScenarios.FirstOrDefault(fault =>
            string.Equals(fault.Name, faultName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"Fault scenario '{faultName}' was not found in template '{Name}'.");
    }
}
