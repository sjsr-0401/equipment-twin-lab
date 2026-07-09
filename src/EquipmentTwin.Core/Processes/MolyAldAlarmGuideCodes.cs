namespace EquipmentTwin.Core.Processes;

/// <summary>
/// Public/synthetic ALD fault를 작업지시서 알람 코드로 연결한다.
/// 실제 vendor alarm code가 아니라 포트폴리오 데모용 코드다.
/// </summary>
public static class MolyAldAlarmGuideCodes
{
    public static string FromFaultKind(MolyAldFaultKind kind)
    {
        return kind switch
        {
            MolyAldFaultKind.PumpDownTimeout => "VAC-101",
            MolyAldFaultKind.TemperatureNotStable => "TMP-201",
            MolyAldFaultKind.PrecursorDoseTimeout => "GAS-301",
            MolyAldFaultKind.PurgeTimeout => "GAS-301",
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unsupported Moly ALD fault kind.")
        };
    }

    public static string FromProcessStep(MolyAldProcessStep step)
    {
        return step switch
        {
            MolyAldProcessStep.PumpDown => "VAC-101",
            MolyAldProcessStep.StabilizeTemperature => "TMP-201",
            MolyAldProcessStep.DoseMetalPrecursor => "GAS-301",
            MolyAldProcessStep.DoseReactant => "GAS-301",
            MolyAldProcessStep.PurgeAfterPrecursor => "GAS-301",
            MolyAldProcessStep.PurgeAfterReactant => "GAS-301",
            MolyAldProcessStep.PostPurge => "GAS-301",
            _ => "SEQ-001"
        };
    }

    public static IReadOnlyList<string> AllFaultGuideCodes()
    {
        return Enum.GetValues<MolyAldFaultKind>()
            .Select(FromFaultKind)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
