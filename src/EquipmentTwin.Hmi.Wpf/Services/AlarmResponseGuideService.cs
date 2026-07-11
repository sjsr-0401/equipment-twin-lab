using System.IO;
using EquipmentTwin.Core.Alarms;

namespace EquipmentTwin.Hmi.Wpf.Services;

public sealed class AlarmResponseGuideService
{
    private const string GuideRelativePath = "alarm-guides/moly-ald-alarm-guides.json";

    public AlarmResponseGuideCatalog LoadCatalog()
    {
        var guidePath = ResolveGuidePath();
        var json = File.ReadAllText(guidePath);
        return AlarmResponseGuideCatalog.FromJson(json);
    }

    private static string ResolveGuidePath()
    {
        var candidates = new List<string>
        {
            Path.Combine(AppContext.BaseDirectory, GuideRelativePath),
            Path.Combine(Environment.CurrentDirectory, GuideRelativePath)
        };

        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            candidates.Add(Path.Combine(current.FullName, GuideRelativePath));
            current = current.Parent;
        }

        foreach (var candidate in candidates.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        throw new FileNotFoundException($"Could not find '{GuideRelativePath}'. Build the WPF project from the repository or run from the repo root.");
    }
}
