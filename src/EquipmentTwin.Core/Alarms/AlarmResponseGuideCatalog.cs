using System.Text.Json;
using System.Text.Json.Serialization;

namespace EquipmentTwin.Core.Alarms;

/// <summary>
/// 알람 코드별 작업지시서를 로드하고 조회하는 카탈로그다.
/// </summary>
public sealed class AlarmResponseGuideCatalog
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private Dictionary<string, AlarmResponseGuide>? guidesByCode;

    public string SchemaVersion { get; init; } = string.Empty;

    public string Source { get; init; } = string.Empty;

    public List<AlarmResponseGuide> Guides { get; init; } = new();

    public static AlarmResponseGuideCatalog FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new InvalidOperationException("Alarm response guide JSON is empty.");
        }

        var catalog = JsonSerializer.Deserialize<AlarmResponseGuideCatalog>(json, JsonOptions)
            ?? throw new InvalidOperationException("Alarm response guide JSON is empty.");

        catalog.Validate();
        return catalog;
    }

    public string ToJson()
    {
        Validate();
        return JsonSerializer.Serialize(this, JsonOptions);
    }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(SchemaVersion))
        {
            throw new InvalidOperationException("Alarm response guide catalog requires a schemaVersion.");
        }

        if (string.IsNullOrWhiteSpace(Source))
        {
            throw new InvalidOperationException("Alarm response guide catalog requires a source.");
        }

        if (Guides is null || Guides.Count == 0)
        {
            throw new InvalidOperationException("Alarm response guide catalog requires at least one guide.");
        }

        var map = new Dictionary<string, AlarmResponseGuide>(StringComparer.OrdinalIgnoreCase);
        for (var index = 0; index < Guides.Count; index++)
        {
            var guide = Guides[index] ?? throw new InvalidOperationException($"Alarm response guide #{index + 1} is empty.");
            guide.Validate(index);

            if (!map.TryAdd(guide.AlarmCode, guide))
            {
                throw new InvalidOperationException($"Alarm response guide catalog contains duplicate alarmCode '{guide.AlarmCode}'.");
            }
        }

        guidesByCode = map;
    }

    public AlarmResponseGuide FindGuide(string alarmCode)
    {
        if (string.IsNullOrWhiteSpace(alarmCode))
        {
            throw new ArgumentException("Alarm code is required.", nameof(alarmCode));
        }

        var map = EnsureMap();
        return map.TryGetValue(alarmCode, out var guide)
            ? guide
            : throw new InvalidOperationException($"Alarm response guide '{alarmCode}' was not found.");
    }

    public bool HasGuide(string alarmCode)
    {
        if (string.IsNullOrWhiteSpace(alarmCode))
        {
            return false;
        }

        return EnsureMap().ContainsKey(alarmCode);
    }

    public IReadOnlyList<string> MissingGuideCodes(IEnumerable<string> alarmCodes)
    {
        ArgumentNullException.ThrowIfNull(alarmCodes);

        var map = EnsureMap();
        return alarmCodes
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(code => !map.ContainsKey(code))
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private Dictionary<string, AlarmResponseGuide> EnsureMap()
    {
        if (guidesByCode is null)
        {
            Validate();
        }

        return guidesByCode!;
    }
}
