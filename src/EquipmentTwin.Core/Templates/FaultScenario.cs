namespace EquipmentTwin.Core.Templates;

/// <summary>
/// 사용자가 선택할 수 있는 장비 트러블 조건이다.
/// </summary>
public sealed class FaultScenario
{
    public string Name { get; init; } = string.Empty;

    public FaultKind Kind { get; init; }

    public string Axis { get; init; } = string.Empty;

    public int? TimeoutMilliseconds { get; init; }

    public int? ElapsedMilliseconds { get; init; }

    public string Message { get; init; } = string.Empty;

    public void Validate(int index, IReadOnlySet<string> knownAxisNames)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidOperationException($"Fault scenario #{index + 1} requires a name.");
        }

        if (string.IsNullOrWhiteSpace(Axis))
        {
            throw new InvalidOperationException($"Fault scenario '{Name}' requires an axis.");
        }

        if (!knownAxisNames.Contains(Axis))
        {
            throw new InvalidOperationException($"Fault scenario '{Name}' references unknown axis '{Axis}'.");
        }

        switch (Kind)
        {
            case FaultKind.MotionTimeout:
                if (TimeoutMilliseconds is null or <= 0)
                {
                    throw new InvalidOperationException($"Fault scenario '{Name}' requires positive timeoutMilliseconds.");
                }

                if (ElapsedMilliseconds is null or <= 0)
                {
                    throw new InvalidOperationException($"Fault scenario '{Name}' requires positive elapsedMilliseconds.");
                }

                if (ElapsedMilliseconds < TimeoutMilliseconds)
                {
                    throw new InvalidOperationException($"Fault scenario '{Name}' elapsedMilliseconds must be greater than or equal to timeoutMilliseconds.");
                }
                break;

            case FaultKind.ServoAlarm:
                if (string.IsNullOrWhiteSpace(Message))
                {
                    throw new InvalidOperationException($"Fault scenario '{Name}' requires a message.");
                }
                break;

            default:
                throw new InvalidOperationException($"Fault scenario '{Name}' has unsupported kind '{Kind}'.");
        }
    }
}
