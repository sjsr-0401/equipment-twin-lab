namespace EquipmentTwin.Core;

/// <summary>
/// 제조 장비의 기본 공정 흐름을 표현하는 상태머신이다.
/// 이 클래스는 Unity 화면이나 실제 PLC에 직접 의존하지 않는다.
/// </summary>
public sealed class EquipmentStateMachine
{
    private static readonly IReadOnlyDictionary<(EquipmentState State, EquipmentEvent Event), EquipmentState> AllowedTransitions =
        new Dictionary<(EquipmentState State, EquipmentEvent Event), EquipmentState>
        {
            [(EquipmentState.Idle, EquipmentEvent.StartLoad)] = EquipmentState.Loading,
            [(EquipmentState.Loading, EquipmentEvent.LoadComplete)] = EquipmentState.Aligning,
            [(EquipmentState.Aligning, EquipmentEvent.AlignmentComplete)] = EquipmentState.Inspecting,
            [(EquipmentState.Inspecting, EquipmentEvent.InspectionComplete)] = EquipmentState.Unloading,
            [(EquipmentState.Unloading, EquipmentEvent.UnloadComplete)] = EquipmentState.Complete,
            [(EquipmentState.Complete, EquipmentEvent.Reset)] = EquipmentState.Idle,
            [(EquipmentState.Alarmed, EquipmentEvent.ClearAlarm)] = EquipmentState.Idle
        };

    private readonly List<EquipmentTransition> _history = new();

    public EquipmentState CurrentState { get; private set; } = EquipmentState.Idle;

    public string? LastAlarmReason { get; private set; }

    public IReadOnlyList<EquipmentTransition> History => _history;

    public bool CanApply(EquipmentEvent equipmentEvent)
    {
        if (IsSafetyEvent(equipmentEvent))
        {
            return CurrentState != EquipmentState.Alarmed;
        }

        return AllowedTransitions.ContainsKey((CurrentState, equipmentEvent));
    }

    public TransitionResult Apply(EquipmentEvent equipmentEvent)
    {
        var previous = CurrentState;

        if (IsSafetyEvent(equipmentEvent))
        {
            return ApplySafetyEvent(previous, equipmentEvent);
        }

        if (!AllowedTransitions.TryGetValue((CurrentState, equipmentEvent), out var nextState))
        {
            return Reject(previous, equipmentEvent, $"'{equipmentEvent}' event is not allowed while equipment is '{CurrentState}'.");
        }

        CurrentState = nextState;

        if (CurrentState == EquipmentState.Idle)
        {
            LastAlarmReason = null;
        }

        return Accept(previous, equipmentEvent, nextState, $"State changed from '{previous}' to '{nextState}'.");
    }

    private TransitionResult ApplySafetyEvent(EquipmentState previous, EquipmentEvent equipmentEvent)
    {
        if (CurrentState == EquipmentState.Alarmed)
        {
            return Reject(previous, equipmentEvent, $"Equipment is already alarmed. Clear the alarm before applying '{equipmentEvent}'.");
        }

        CurrentState = EquipmentState.Alarmed;
        LastAlarmReason = equipmentEvent switch
        {
            EquipmentEvent.DoorOpened => "Door opened during operation.",
            EquipmentEvent.EmergencyStop => "Emergency stop requested.",
            _ => "Safety event."
        };

        return Accept(previous, equipmentEvent, CurrentState, LastAlarmReason);
    }

    private TransitionResult Accept(EquipmentState previous, EquipmentEvent equipmentEvent, EquipmentState nextState, string message)
    {
        var transition = new EquipmentTransition(previous, equipmentEvent, nextState, true, message);
        _history.Add(transition);
        return new TransitionResult(previous, equipmentEvent, nextState, true, message);
    }

    private TransitionResult Reject(EquipmentState previous, EquipmentEvent equipmentEvent, string message)
    {
        var transition = new EquipmentTransition(previous, equipmentEvent, previous, false, message);
        _history.Add(transition);
        return new TransitionResult(previous, equipmentEvent, previous, false, message);
    }

    private static bool IsSafetyEvent(EquipmentEvent equipmentEvent)
    {
        return equipmentEvent is EquipmentEvent.DoorOpened or EquipmentEvent.EmergencyStop;
    }
}
