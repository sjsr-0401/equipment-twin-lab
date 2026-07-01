namespace EquipmentTwin.Core.Processes;

public sealed class MolyAldGasFlow
{
    public string Name { get; init; } = string.Empty;

    public double FlowSccm { get; init; }

    public void Validate(string recipeName, int index)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidOperationException($"Moly ALD recipe '{recipeName}' reactant gas #{index + 1} requires a name.");
        }

        if (FlowSccm <= 0 || double.IsNaN(FlowSccm) || double.IsInfinity(FlowSccm))
        {
            throw new InvalidOperationException($"Moly ALD recipe '{recipeName}' gas '{Name}' requires a positive flowSccm.");
        }
    }
}
