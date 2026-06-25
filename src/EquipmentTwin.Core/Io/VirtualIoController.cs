namespace EquipmentTwin.Core.Io;

/// <summary>
/// 실제 PLC 없이 디지털 IO를 흉내 내는 가상 IO 컨트롤러다.
/// 장비 SW는 Output을 쓰고 Input을 읽는다.
/// 시뮬레이터는 Input을 바꿔서 센서 변화를 만든다.
/// </summary>
public sealed class VirtualIoController
{
    private readonly Dictionary<string, IoSignalDefinition> _definitions = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, bool> _values = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, DateTimeOffset> _updatedAt = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<IoChange> _history = new();

    public IReadOnlyList<IoChange> History => _history;

    public void DefineInput(string name, string description, bool initialValue = false)
    {
        Define(name, IoSignalDirection.Input, description, initialValue);
    }

    public void DefineOutput(string name, string description, bool initialValue = false)
    {
        Define(name, IoSignalDirection.Output, description, initialValue);
    }

    public bool Read(string name)
    {
        EnsureDefined(name);
        return _values[name];
    }

    public void WriteOutput(string name, bool value, string source = "EquipmentSoftware")
    {
        var definition = EnsureDirection(name, IoSignalDirection.Output, "Only output signals can be written by equipment software.");
        SetValue(definition, value, source);
    }

    public void SetInput(string name, bool value, string source = "Simulator")
    {
        var definition = EnsureDirection(name, IoSignalDirection.Input, "Only input signals can be changed by the simulator.");
        SetValue(definition, value, source);
    }

    public IReadOnlyList<IoSignalSnapshot> Snapshot()
    {
        return _definitions.Values
            .OrderBy(definition => definition.Name, StringComparer.OrdinalIgnoreCase)
            .Select(definition => new IoSignalSnapshot(
                definition.Name,
                definition.Direction,
                _values[definition.Name],
                definition.Description,
                _updatedAt[definition.Name]))
            .ToArray();
    }

    public bool IsDefined(string name)
    {
        return _definitions.ContainsKey(name);
    }

    private void Define(string name, IoSignalDirection direction, string description, bool initialValue)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("IO signal name is required.", nameof(name));
        }

        if (_definitions.ContainsKey(name))
        {
            throw new InvalidOperationException($"IO signal '{name}' is already defined.");
        }

        var definition = new IoSignalDefinition(name, direction, description);
        _definitions.Add(name, definition);
        _values.Add(name, initialValue);
        _updatedAt.Add(name, DateTimeOffset.UtcNow);
    }

    private IoSignalDefinition EnsureDefined(string name)
    {
        if (!_definitions.TryGetValue(name, out var definition))
        {
            throw new KeyNotFoundException($"IO signal '{name}' is not defined.");
        }

        return definition;
    }

    private IoSignalDefinition EnsureDirection(string name, IoSignalDirection expectedDirection, string message)
    {
        var definition = EnsureDefined(name);

        if (definition.Direction != expectedDirection)
        {
            throw new InvalidOperationException($"{message} Signal '{name}' is '{definition.Direction}'.");
        }

        return definition;
    }

    private void SetValue(IoSignalDefinition definition, bool value, string source)
    {
        var previous = _values[definition.Name];

        if (previous == value)
        {
            _updatedAt[definition.Name] = DateTimeOffset.UtcNow;
            return;
        }

        _values[definition.Name] = value;
        _updatedAt[definition.Name] = DateTimeOffset.UtcNow;
        _history.Add(new IoChange(
            definition.Name,
            definition.Direction,
            previous,
            value,
            source,
            _updatedAt[definition.Name]));
    }
}
