using System.Windows.Input;
using EquipmentTwin.Core.Alarms;

namespace EquipmentTwin.Hmi.Wpf.ViewModels;

public sealed class AlarmGuideChoiceRowViewModel
{
    public AlarmGuideChoiceRowViewModel(
        AlarmGuideChoice choice,
        Action<AlarmGuideChoiceRowViewModel> selected,
        string? displayLabel = null,
        string? displayNextAction = null,
        string? roleText = null)
    {
        ArgumentNullException.ThrowIfNull(choice);

        Id = choice.Id;
        Label = displayLabel ?? choice.Label;
        NextAction = displayNextAction ?? choice.NextAction;
        RequiresEngineer = choice.RequiresEngineer;
        EscalationText = roleText ?? (RequiresEngineer ? "ENG" : "OP");
        SelectCommand = new RelayCommand(() => selected(this));
    }

    public string Id { get; }

    public string Label { get; }

    public string NextAction { get; }

    public bool RequiresEngineer { get; }

    public string EscalationText { get; }

    public ICommand SelectCommand { get; }
}
