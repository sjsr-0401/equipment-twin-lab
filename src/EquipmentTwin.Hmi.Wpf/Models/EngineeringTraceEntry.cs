namespace EquipmentTwin.Hmi.Wpf.Models;

public sealed class EngineeringTraceEntry
{
    public EngineeringTraceEntry(string source, string message)
    {
        Time = DateTime.Now.ToString("HH:mm:ss.fff");
        Source = string.IsNullOrWhiteSpace(source) ? "TRACE" : source.Trim();
        Message = string.IsNullOrWhiteSpace(message) ? "-" : message.Trim();
    }

    public string Time { get; }

    public string Source { get; }

    public string Message { get; }
}
