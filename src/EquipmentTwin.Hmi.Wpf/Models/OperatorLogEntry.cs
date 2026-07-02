namespace EquipmentTwin.Hmi.Wpf.Models;

public sealed class OperatorLogEntry
{
    public OperatorLogEntry(string action, string detail)
    {
        Time = DateTime.Now.ToString("HH:mm:ss");
        Action = string.IsNullOrWhiteSpace(action) ? "EVENT" : action.Trim();
        Detail = string.IsNullOrWhiteSpace(detail) ? "-" : detail.Trim();
    }

    public string Time { get; }

    public string Action { get; }

    public string Detail { get; }
}
