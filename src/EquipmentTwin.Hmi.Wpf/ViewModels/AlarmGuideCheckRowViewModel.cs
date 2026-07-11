using EquipmentTwin.Core.Alarms;

namespace EquipmentTwin.Hmi.Wpf.ViewModels;

public sealed class AlarmGuideCheckRowViewModel : ObservableObject
{
    private readonly Action<AlarmGuideCheckRowViewModel, bool> checkedChanged;
    private bool isChecked;

    public AlarmGuideCheckRowViewModel(
        OperatorCheckItem check,
        Action<AlarmGuideCheckRowViewModel, bool> checkedChanged,
        string? displayLabel = null,
        string? requiredText = null,
        bool isChecked = false)
    {
        ArgumentNullException.ThrowIfNull(check);

        this.checkedChanged = checkedChanged ?? throw new ArgumentNullException(nameof(checkedChanged));
        this.isChecked = isChecked;
        Id = check.Id;
        Label = displayLabel ?? check.Label;
        Required = check.Required;
        RequiredText = requiredText ?? (Required ? "REQ" : "OPT");
    }

    public string Id { get; }

    public string Label { get; }

    public bool Required { get; }

    public string RequiredText { get; }

    public bool IsChecked
    {
        get => isChecked;
        set
        {
            if (SetProperty(ref isChecked, value))
            {
                checkedChanged(this, value);
            }
        }
    }
}
