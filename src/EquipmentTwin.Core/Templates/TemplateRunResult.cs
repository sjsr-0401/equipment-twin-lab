using EquipmentTwin.Core.Motion;

namespace EquipmentTwin.Core.Templates;

/// <summary>
/// 장비 템플릿과 제품 recipe를 실행한 결과다.
/// </summary>
public sealed class TemplateRunResult
{
    public TemplateRunResult(
        string templateName,
        ProductRecipe recipe,
        IReadOnlyDictionary<string, MotionAxis> motionAxes,
        IReadOnlyList<TemplateMotionCommandLog> commandLog)
    {
        TemplateName = templateName;
        Recipe = recipe;
        MotionAxes = motionAxes;
        CommandLog = commandLog;
    }

    public string TemplateName { get; }

    public ProductRecipe Recipe { get; }

    public IReadOnlyDictionary<string, MotionAxis> MotionAxes { get; }

    public IReadOnlyList<TemplateMotionCommandLog> CommandLog { get; }

    public bool Success => CommandLog.All(log => log.Result.Accepted)
        && MotionAxes.Values.All(axis => axis.LastAlarm is null);
}
