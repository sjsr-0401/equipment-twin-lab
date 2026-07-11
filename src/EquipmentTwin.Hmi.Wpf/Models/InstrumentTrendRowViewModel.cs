using System.Windows;
using System.Windows.Media;

namespace EquipmentTwin.Hmi.Wpf.Models;

public sealed class InstrumentTrendRowViewModel
{
    public InstrumentTrendRowViewModel(
        string label,
        string currentValue,
        string rangeText,
        PointCollection trendPoints,
        Brush trendBrush)
    {
        Label = label;
        CurrentValue = currentValue;
        RangeText = rangeText;
        TrendPoints = trendPoints;
        TrendBrush = trendBrush;
    }

    public string Label { get; }

    public string CurrentValue { get; }

    public string RangeText { get; }

    public PointCollection TrendPoints { get; }

    public Brush TrendBrush { get; }
}
