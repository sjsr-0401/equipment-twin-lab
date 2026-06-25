using EquipmentTwin.Core;
using EquipmentTwin.Core.Io;

var tests = new (string Name, Action Body)[]
{
    ("Normal sequence reaches Complete", NormalSequenceReachesComplete),
    ("Invalid transition is rejected", InvalidTransitionIsRejected),
    ("Door opened during inspection creates alarm", DoorOpenedDuringInspectionCreatesAlarm),
    ("Clear alarm returns to Idle", ClearAlarmReturnsToIdle),
    ("History records accepted and rejected transitions", HistoryRecordsAcceptedAndRejectedTransitions),
    ("Default IO map exposes expected inputs and outputs", DefaultIoMapExposesExpectedInputsAndOutputs),
    ("Equipment software can write outputs", EquipmentSoftwareCanWriteOutputs),
    ("Equipment software cannot write inputs", EquipmentSoftwareCannotWriteInputs),
    ("Simulator can set inputs", SimulatorCanSetInputs),
    ("Simulator cannot set outputs", SimulatorCannotSetOutputs),
    ("IO history records value changes", IoHistoryRecordsValueChanges),
    ("Manual clock advances deterministically", ManualClockAdvancesDeterministically),
    ("State entered time updates after accepted transition", StateEnteredTimeUpdatesAfterAcceptedTransition),
    ("Timeout before limit does not alarm", TimeoutBeforeLimitDoesNotAlarm),
    ("Timeout at limit moves equipment to alarm", TimeoutAtLimitMovesEquipmentToAlarm),
    ("Unconfigured state does not timeout", UnconfiguredStateDoesNotTimeout),
    ("Timeout policy rejects invalid rules", TimeoutPolicyRejectsInvalidRules)
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

static void DefaultIoMapExposesExpectedInputsAndOutputs()
{
    var io = EquipmentIoMap.CreateDefaultCellIo();
    var snapshot = io.Snapshot();

    AssertTrue(io.IsDefined(EquipmentIoMap.DoorClosed), "DoorClosed input must exist.");
    AssertTrue(io.IsDefined(EquipmentIoMap.VacuumOn), "VacuumOn output must exist.");
    AssertEqual(9, snapshot.Count, "Default IO point count mismatch.");
    AssertEqual(true, io.Read(EquipmentIoMap.DoorClosed), "DoorClosed must start true.");
    AssertEqual(false, io.Read(EquipmentIoMap.VacuumOn), "VacuumOn must start false.");
}

static void EquipmentSoftwareCanWriteOutputs()
{
    var io = EquipmentIoMap.CreateDefaultCellIo();

    io.WriteOutput(EquipmentIoMap.VacuumOn, true);

    AssertEqual(true, io.Read(EquipmentIoMap.VacuumOn), "Vacuum output must be true after command.");
}

static void EquipmentSoftwareCannotWriteInputs()
{
    var io = EquipmentIoMap.CreateDefaultCellIo();

    AssertThrows<InvalidOperationException>(
        () => io.WriteOutput(EquipmentIoMap.LoadPresent, true),
        "Writing an input with WriteOutput must fail.");
}

static void SimulatorCanSetInputs()
{
    var io = EquipmentIoMap.CreateDefaultCellIo();

    io.SetInput(EquipmentIoMap.LoadPresent, true);

    AssertEqual(true, io.Read(EquipmentIoMap.LoadPresent), "LoadPresent input must be true after simulator update.");
}

static void SimulatorCannotSetOutputs()
{
    var io = EquipmentIoMap.CreateDefaultCellIo();

    AssertThrows<InvalidOperationException>(
        () => io.SetInput(EquipmentIoMap.BuzzerOn, true),
        "Setting an output with SetInput must fail.");
}

static void IoHistoryRecordsValueChanges()
{
    var io = EquipmentIoMap.CreateDefaultCellIo();

    io.SetInput(EquipmentIoMap.LoadPresent, true, source: "TestSensor");
    io.WriteOutput(EquipmentIoMap.VacuumOn, true, source: "TestSequence");
    io.WriteOutput(EquipmentIoMap.VacuumOn, true, source: "RepeatedCommand");

    AssertEqual(2, io.History.Count, "History should record only actual value changes.");
    AssertEqual(EquipmentIoMap.LoadPresent, io.History[0].Name, "First IO history item mismatch.");
    AssertEqual("TestSensor", io.History[0].Source, "First IO history source mismatch.");
    AssertEqual(EquipmentIoMap.VacuumOn, io.History[1].Name, "Second IO history item mismatch.");
    AssertEqual("TestSequence", io.History[1].Source, "Second IO history source mismatch.");
}

static void ManualClockAdvancesDeterministically()
{
    var clock = new ManualClock(new DateTimeOffset(2026, 6, 25, 0, 0, 0, TimeSpan.Zero));

    clock.Advance(TimeSpan.FromSeconds(10));

    AssertEqual(new DateTimeOffset(2026, 6, 25, 0, 0, 10, TimeSpan.Zero), clock.UtcNow, "ManualClock time mismatch.");
    AssertThrows<ArgumentOutOfRangeException>(
        () => clock.Advance(TimeSpan.FromSeconds(-1)),
        "ManualClock must reject moving backward.");
}

static void StateEnteredTimeUpdatesAfterAcceptedTransition()
{
    var clock = new ManualClock(new DateTimeOffset(2026, 6, 25, 0, 0, 0, TimeSpan.Zero));
    var machine = new EquipmentStateMachine(clock);

    clock.Advance(TimeSpan.FromSeconds(5));
    machine.Apply(EquipmentEvent.StartLoad);

    AssertEqual(clock.UtcNow, machine.StateEnteredAtUtc, "StateEnteredAtUtc must update on accepted transition.");
}

static void TimeoutBeforeLimitDoesNotAlarm()
{
    var clock = new ManualClock(new DateTimeOffset(2026, 6, 25, 0, 0, 0, TimeSpan.Zero));
    var machine = new EquipmentStateMachine(clock);
    var policy = new StateTimeoutPolicy();
    policy.SetTimeout(EquipmentState.Loading, TimeSpan.FromSeconds(30));

    machine.Apply(EquipmentEvent.StartLoad);
    clock.Advance(TimeSpan.FromSeconds(29));

    var result = machine.CheckTimeout(policy);

    AssertFalse(result.TimedOut, "Loading must not timeout before the limit.");
    AssertEqual(EquipmentState.Loading, machine.CurrentState, "State must stay Loading before timeout.");
}

static void TimeoutAtLimitMovesEquipmentToAlarm()
{
    var clock = new ManualClock(new DateTimeOffset(2026, 6, 25, 0, 0, 0, TimeSpan.Zero));
    var machine = new EquipmentStateMachine(clock);
    var policy = new StateTimeoutPolicy();
    policy.SetTimeout(EquipmentState.Loading, TimeSpan.FromSeconds(30));

    machine.Apply(EquipmentEvent.StartLoad);
    clock.Advance(TimeSpan.FromSeconds(30));

    var result = machine.CheckTimeout(policy);

    AssertTrue(result.TimedOut, "Loading must timeout at the limit.");
    AssertEqual(EquipmentState.Alarmed, machine.CurrentState, "Timeout must move equipment to Alarmed.");
    AssertEqual(EquipmentEvent.Timeout, machine.History[^1].Event, "Timeout transition must be recorded.");
    AssertEqual("State 'Loading' timed out after 00:00:30.", machine.LastAlarmReason, "Timeout alarm reason mismatch.");
}

static void UnconfiguredStateDoesNotTimeout()
{
    var clock = new ManualClock(new DateTimeOffset(2026, 6, 25, 0, 0, 0, TimeSpan.Zero));
    var machine = new EquipmentStateMachine(clock);
    var policy = new StateTimeoutPolicy();

    clock.Advance(TimeSpan.FromHours(1));

    var result = machine.CheckTimeout(policy);

    AssertFalse(result.TimedOut, "Idle must not timeout when no timeout rule is configured.");
    AssertEqual(EquipmentState.Idle, machine.CurrentState, "State must stay Idle without timeout rule.");
}

static void TimeoutPolicyRejectsInvalidRules()
{
    var policy = new StateTimeoutPolicy();

    AssertThrows<ArgumentOutOfRangeException>(
        () => policy.SetTimeout(EquipmentState.Loading, TimeSpan.Zero),
        "Zero timeout must be rejected.");
    AssertThrows<InvalidOperationException>(
        () => policy.SetTimeout(EquipmentState.Alarmed, TimeSpan.FromSeconds(1)),
        "Alarmed timeout rule must be rejected.");
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

static void AssertThrows<TException>(Action action, string message)
    where TException : Exception
{
    try
    {
        action();
    }
    catch (TException)
    {
        return;
    }

    throw new InvalidOperationException(message);
}
