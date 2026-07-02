using EquipmentTwin.Core.Processes;
using System.IO;

namespace EquipmentTwin.Hmi.Wpf.Services;

public sealed class MolyAldRecipeService
{
    private const string RecipeRelativePath = "processes/public-moly-ald-metallization.json";

    public MolyAldRecipe LoadRecipe()
    {
        var recipePath = ResolveRecipePath();
        var json = File.ReadAllText(recipePath);
        return MolyAldRecipe.FromJson(json);
    }

    private static string ResolveRecipePath()
    {
        var candidates = new List<string>
        {
            Path.Combine(AppContext.BaseDirectory, RecipeRelativePath),
            Path.Combine(Environment.CurrentDirectory, RecipeRelativePath)
        };

        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            candidates.Add(Path.Combine(current.FullName, RecipeRelativePath));
            current = current.Parent;
        }

        foreach (var candidate in candidates.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        throw new FileNotFoundException($"Could not find '{RecipeRelativePath}'. Build the WPF project from the repository or run from the repo root.");
    }
}
