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
    private readonly IClock _clock;

    public EquipmentStateMachine()
        : this(SystemClock.Instance)
    {
    }

    public EquipmentStateMachine(IClock clock)
    {
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        StateEnteredAtUtc = _clock.UtcNow;
    }

    public EquipmentState CurrentState { get; private set; } = EquipmentState.Idle;

    public DateTimeOffset StateEnteredAtUtc { get; private set; }

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
            return ApplyAlarmEvent(previous, equipmentEvent);
        }

        if (!AllowedTransitions.TryGetValue((CurrentState, equipmentEvent), out var nextState))
        {
            return Reject(previous, equipmentEvent, $"'{equipmentEvent}' event is not allowed while equipment is '{CurrentState}'.");
        }

        MoveTo(nextState);

        if (CurrentState == EquipmentState.Idle)
        {
            LastAlarmReason = null;
        }

        return Accept(previous, equipmentEvent, nextState, $"State changed from '{previous}' to '{nextState}'.");
    }

    public TimeoutCheckResult CheckTimeout(StateTimeoutPolicy policy)
    {
        ArgumentNullException.ThrowIfNull(policy);

        var elapsed = _clock.UtcNow - StateEnteredAtUtc;

        if (!policy.TryGetTimeout(CurrentState, out var timeout))
        {
            return new TimeoutCheckResult(CurrentState, false, elapsed, null, null);
        }

        if (elapsed < timeout)
        {
            return new TimeoutCheckResult(CurrentState, false, elapsed, timeout, null);
        }

        var timedOutState = CurrentState;
        var message = $"State '{timedOutState}' timed out after {timeout}.";
        var transition = MoveToAlarm(timedOutState, EquipmentEvent.Timeout, message);

        return new TimeoutCheckResult(timedOutState, true, elapsed, timeout, transition);
    }

    private TransitionResult ApplyAlarmEvent(EquipmentState previous, EquipmentEvent equipmentEvent)
    {
        if (CurrentState == EquipmentState.Alarmed)
        {
            return Reject(previous, equipmentEvent, $"Equipment is already alarmed. Clear the alarm before applying '{equipmentEvent}'.");
        }

        LastAlarmReason = equipmentEvent switch
        {
            EquipmentEvent.DoorOpened => "Door opened during operation.",
            EquipmentEvent.EmergencyStop => "Emergency stop requested.",
            EquipmentEvent.Timeout => "Timeout event requested.",
            _ => "Safety event."
        };

        return MoveToAlarm(previous, equipmentEvent, LastAlarmReason);
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
        return equipmentEvent is EquipmentEvent.DoorOpened or EquipmentEvent.EmergencyStop or EquipmentEvent.Timeout;
    }

    private TransitionResult MoveToAlarm(EquipmentState previous, EquipmentEvent equipmentEvent, string reason)
    {
        MoveTo(EquipmentState.Alarmed);
        LastAlarmReason = reason;
        return Accept(previous, equipmentEvent, CurrentState, reason);
    }

    private void MoveTo(EquipmentState state)
    {
        CurrentState = state;
        StateEnteredAtUtc = _clock.UtcNow;
    }
}
