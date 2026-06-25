using EquipmentTwin.Core;

var tests = new (string Name, Action Body)[]
{
    ("Normal sequence reaches Complete", NormalSequenceReachesComplete),
    ("Invalid transition is rejected", InvalidTransitionIsRejected),
    ("Door opened during inspection creates alarm", DoorOpenedDuringInspectionCreatesAlarm),
    ("Clear alarm returns to Idle", ClearAlarmReturnsToIdle),
    ("History records accepted and rejected transitions", HistoryRecordsAcceptedAndRejectedTransitions)
};

var failures = 0;

foreach (var test in tests)
{
    try
    {
        test.Body();
        Console.WriteLine($"PASS {test.Name}");
    }
    catch (Exception ex)
    {
        failures++;
        Console.WriteLine($"FAIL {test.Name}");
        Console.WriteLine(ex.Message);
    }
}

if (failures > 0)
{
    Console.WriteLine($"{failures} test(s) failed.");
    Environment.Exit(1);
}

Console.WriteLine("All EquipmentTwin.Core tests passed.");

static void NormalSequenceReachesComplete()
{
    var machine = new EquipmentStateMachine();

    ApplyAndAssert(machine, EquipmentEvent.StartLoad, EquipmentState.Loading);
    ApplyAndAssert(machine, EquipmentEvent.LoadComplete, EquipmentState.Aligning);
    ApplyAndAssert(machine, EquipmentEvent.AlignmentComplete, EquipmentState.Inspecting);
    ApplyAndAssert(machine, EquipmentEvent.InspectionComplete, EquipmentState.Unloading);
    ApplyAndAssert(machine, EquipmentEvent.UnloadComplete, EquipmentState.Complete);
}

static void InvalidTransitionIsRejected()
{
    var machine = new EquipmentStateMachine();

    var result = machine.Apply(EquipmentEvent.InspectionComplete);

    AssertFalse(result.Accepted, "InspectionComplete must be rejected from Idle.");
    AssertEqual(EquipmentState.Idle, machine.CurrentState, "State must stay Idle after rejected transition.");
}

static void DoorOpenedDuringInspectionCreatesAlarm()
{
    var machine = MoveToInspection();

    var result = machine.Apply(EquipmentEvent.DoorOpened);

    AssertTrue(result.Accepted, "DoorOpened must be accepted as a safety event.");
    AssertEqual(EquipmentState.Alarmed, machine.CurrentState, "DoorOpened must move equipment to Alarmed.");
    AssertEqual("Door opened during operation.", machine.LastAlarmReason, "Alarm reason mismatch.");
}

static void ClearAlarmReturnsToIdle()
{
    var machine = MoveToInspection();
    machine.Apply(EquipmentEvent.EmergencyStop);

    var result = machine.Apply(EquipmentEvent.ClearAlarm);

    AssertTrue(result.Accepted, "ClearAlarm must be accepted from Alarmed.");
    AssertEqual(EquipmentState.Idle, machine.CurrentState, "ClearAlarm must return equipment to Idle.");
    AssertEqual(null, machine.LastAlarmReason, "Alarm reason must be cleared after returning to Idle.");
}

static void HistoryRecordsAcceptedAndRejectedTransitions()
{
    var machine = new EquipmentStateMachine();

    machine.Apply(EquipmentEvent.InspectionComplete);
    machine.Apply(EquipmentEvent.StartLoad);

    AssertEqual(2, machine.History.Count, "History count mismatch.");
    AssertFalse(machine.History[0].Accepted, "First transition must be rejected.");
    AssertTrue(machine.History[1].Accepted, "Second transition must be accepted.");
}

static EquipmentStateMachine MoveToInspection()
{
    var machine = new EquipmentStateMachine();
    machine.Apply(EquipmentEvent.StartLoad);
    machine.Apply(EquipmentEvent.LoadComplete);
    machine.Apply(EquipmentEvent.AlignmentComplete);
    return machine;
}

static void ApplyAndAssert(EquipmentStateMachine machine, EquipmentEvent equipmentEvent, EquipmentState expectedState)
{
    var result = machine.Apply(equipmentEvent);

    AssertTrue(result.Accepted, $"{equipmentEvent} must be accepted.");
    AssertEqual(expectedState, machine.CurrentState, $"Expected state {expectedState}.");
}

static void AssertTrue(bool value, string message)
{
    if (!value)
    {
        throw new InvalidOperationException(message);
    }
}

static void AssertFalse(bool value, string message)
{
    if (value)
    {
        throw new InvalidOperationException(message);
    }
}

static void AssertEqual<T>(T expected, T actual, string message)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
    {
        throw new InvalidOperationException($"{message} Expected: {expected}, Actual: {actual}");
    }
}
