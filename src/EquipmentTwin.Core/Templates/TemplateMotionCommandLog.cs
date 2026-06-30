using EquipmentTwin.Core.Motion;

namespace EquipmentTwin.Core.Templates;

/// <summary>
/// TemplateRunner가 축에 내린 명령 하나의 실행 기록이다.
/// </summary>
public sealed record TemplateMotionCommandLog(
    string Step,
    string AxisName,
    MotionCommandResult Result);
