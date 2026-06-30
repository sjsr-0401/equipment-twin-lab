using EquipmentTwin.Core;
using EquipmentTwin.Core.Alarms;
using EquipmentTwin.Core.Io;
using EquipmentTwin.Core.Motion;
using EquipmentTwin.Core.Scenarios;
using EquipmentTwin.Core.Templates;

var tests = new (string Name, Action Body)[]
{
    ("Normal sequence reaches Complete", NormalSequenceReachesComplete),
    ("Invalid transition is rejected", InvalidTransitionIsRejected),
    ("Door opened during inspection creates alarm", DoorOpenedDuringInspectionCreatesAlarm),
    ("Clear alarm returns to Idle", ClearAlarmReturnsToIdle),
    ("History records accepted and rejected transitions", HistoryRecordsAcceptedAndRejectedTransitions),
    ("Door opened records alarm code", DoorOpenedRecordsAlarmCode),
    ("Emergency stop records alarm code", EmergencyStopRecordsAlarmCode),
    ("Timeout records alarm code in history", TimeoutRecordsAlarmCodeInHistory),
    ("Clear alarm clears alarm code", ClearAlarmClearsAlarmCode),
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
    ("Timeout policy rejects invalid rules", TimeoutPolicyRejectsInvalidRules),
    ("Cell controller starts cycle and sets outputs", CellControllerStartsCycleAndSetsOutputs),
    ("Cell controller advances sequence from IO inputs", CellControllerAdvancesSequenceFromIoInputs),
    ("Cell controller prioritizes safety inputs", CellControllerPrioritizesSafetyInputs),
    ("Cell controller reports no change when sensor is missing", CellControllerReportsNoChangeWhenSensorIsMissing),
    ("Cell controller applies timeout policy", CellControllerAppliesTimeoutPolicy),
    ("Clear alarm resets normal outputs", ClearAlarmResetsNormalOutputs),
    ("Door alarm cannot clear while door remains open", DoorAlarmCannotClearWhileDoorRemainsOpen),
    ("Door alarm clears after door closes", DoorAlarmClearsAfterDoorCloses),
    ("Emergency stop alarm cannot clear while pressed", EmergencyStopAlarmCannotClearWhilePressed),
    ("Emergency stop alarm clears after release", EmergencyStopAlarmClearsAfterRelease),
    ("Timeout alarm can clear in MVP model", TimeoutAlarmCanClearInMvpModel),
    ("Motion axis starts disabled", MotionAxisStartsDisabled),
    ("Motion axis servo on moves to ready", MotionAxisServoOnMovesToReady),
    ("Motion axis home requires servo on", MotionAxisHomeRequiresServoOn),
    ("Motion axis home completes after duration", MotionAxisHomeCompletesAfterDuration),
    ("Motion axis move requires home", MotionAxisMoveRequiresHome),
    ("Motion axis move completes after duration", MotionAxisMoveCompletesAfterDuration),
    ("Motion axis timeout creates alarm", MotionAxisTimeoutCreatesAlarm),
    ("Motion axis clear alarm returns to in position", MotionAxisClearAlarmReturnsToInPosition),
    ("Equipment template JSON loads vision inspection cell file", EquipmentTemplateJsonLoadsVisionInspectionCellFile),
    ("Equipment template creates motion axes", EquipmentTemplateCreatesMotionAxes),
    ("Equipment template finds product recipe case insensitively", EquipmentTemplateFindsProductRecipeCaseInsensitively),
    ("Equipment template finds fault scenario case insensitively", EquipmentTemplateFindsFaultScenarioCaseInsensitively),
    ("Equipment template rejects duplicate motion axes", EquipmentTemplateRejectsDuplicateMotionAxes),
    ("Equipment template rejects unknown recipe axis", EquipmentTemplateRejectsUnknownRecipeAxis),
    ("Equipment template rejects missing inspection result", EquipmentTemplateRejectsMissingInspectionResult),
    ("Equipment template rejects failed inspection without defect code", EquipmentTemplateRejectsFailedInspectionWithoutDefectCode),
    ("Equipment template rejects unknown fault axis", EquipmentTemplateRejectsUnknownFaultAxis),
    ("Equipment template rejects invalid timeout fault", EquipmentTemplateRejectsInvalidTimeoutFault),
    ("Template runner runs default panel recipe", TemplateRunnerRunsDefaultPanelRecipe),
    ("Template runner runs tall part recipe", TemplateRunnerRunsTallPartRecipe),
    ("Template runner injects motion timeout fault", TemplateRunnerInjectsMotionTimeoutFault),
    ("Template runner injects servo alarm fault", TemplateRunnerInjectsServoAlarmFault),
    ("Template runner rejects missing recipe", TemplateRunnerRejectsMissingRecipe),
    ("Template runner rejects missing fault scenario", TemplateRunnerRejectsMissingFaultScenario),
    ("Template runner rejects invalid durations", TemplateRunnerRejectsInvalidDurations),
    ("Scenario JSON loads normal cycle file", ScenarioJsonLoadsNormalCycleFile),
    ("Scenario runner completes normal cycle file", ScenarioRunnerCompletesNormalCycleFile),
    ("Scenario runner handles loading timeout file", ScenarioRunnerHandlesLoadingTimeoutFile),
    ("Scenario runner handles door open alarm file", ScenarioRunnerHandlesDoorOpenAlarmFile),
    ("Scenario runner handles emergency stop alarm file", ScenarioRunnerHandlesEmergencyStopAlarmFile),
    ("Scenario runner handles clear alarm recovery file", ScenarioRunnerHandlesClearAlarmRecoveryFile),
    ("Scenario runner handles door open clear blocked file", ScenarioRunnerHandlesDoorOpenClearBlockedFile),
    ("Scenario runner handles emergency stop recovery file", ScenarioRunnerHandlesEmergencyStopRecoveryFile),
    ("Scenario runner handles motion axis normal file", ScenarioRunnerHandlesMotionAxisNormalFile),
    ("Scenario runner handles motion axis timeout file", ScenarioRunnerHandlesMotionAxisTimeoutFile),
    ("Scenario runner reports expectation failure", ScenarioRunnerReportsExpectationFailure)
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

static void DoorOpenedRecordsAlarmCode()
{
    var machine = MoveToInspection();

    var result = machine.Apply(EquipmentEvent.DoorOpened);

    AssertEqual(AlarmCode.DoorOpened, machine.LastAlarm?.Code, "DoorOpened must store DoorOpened alarm code.");
    AssertEqual(AlarmCode.DoorOpened, result.Alarm?.Code, "TransitionResult must expose DoorOpened alarm code.");
    AssertEqual(AlarmCode.DoorOpened, machine.History[^1].Alarm?.Code, "History must store DoorOpened alarm code.");
}

static void EmergencyStopRecordsAlarmCode()
{
    var machine = MoveToInspection();

    var result = machine.Apply(EquipmentEvent.EmergencyStop);

    AssertEqual(AlarmCode.EmergencyStop, machine.LastAlarm?.Code, "EmergencyStop must store EmergencyStop alarm code.");
    AssertEqual(EquipmentEvent.EmergencyStop, result.Alarm?.SourceEvent, "Alarm source event mismatch.");
}

static void TimeoutRecordsAlarmCodeInHistory()
{
    var clock = new ManualClock(new DateTimeOffset(2026, 6, 26, 0, 0, 0, TimeSpan.Zero));
    var machine = new EquipmentStateMachine(clock);
    var policy = new StateTimeoutPolicy();
    policy.SetTimeout(EquipmentState.Loading, TimeSpan.FromSeconds(30));

    machine.Apply(EquipmentEvent.StartLoad);
    clock.Advance(TimeSpan.FromSeconds(30));
    machine.CheckTimeout(policy);

    AssertEqual(AlarmCode.StateTimeout, machine.LastAlarm?.Code, "Timeout must store StateTimeout alarm code.");
    AssertEqual(AlarmCode.StateTimeout, machine.History[^1].Alarm?.Code, "History must store timeout alarm code.");
    AssertEqual(EquipmentEvent.Timeout, machine.LastAlarm?.SourceEvent, "Timeout alarm source mismatch.");
}

static void ClearAlarmClearsAlarmCode()
{
    var machine = MoveToInspection();
    machine.Apply(EquipmentEvent.DoorOpened);

    machine.Apply(EquipmentEvent.ClearAlarm);

    AssertEqual(null, machine.LastAlarm, "ClearAlarm must clear active alarm info.");
    AssertEqual(null, machine.LastAlarmReason, "ClearAlarm must clear active alarm reason.");
}

static void DefaultIoMapExposesExpectedInputsAndOutputs()
{
    var io = EquipmentIoMap.CreateDefaultCellIo();
    var snapshot = io.Snapshot();

    AssertTrue(io.IsDefined(EquipmentIoMap.DoorClosed), "DoorClosed input must exist.");
    AssertTrue(io.IsDefined(EquipmentIoMap.UnloadComplete), "UnloadComplete input must exist.");
    AssertTrue(io.IsDefined(EquipmentIoMap.VacuumOn), "VacuumOn output must exist.");
    AssertEqual(10, snapshot.Count, "Default IO point count mismatch.");
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

static void CellControllerStartsCycleAndSetsOutputs()
{
    var io = EquipmentIoMap.CreateDefaultCellIo();
    var machine = new EquipmentStateMachine();
    var cell = new EquipmentCellController(machine, io);

    var result = cell.StartCycle();

    AssertTrue(result.Accepted, "StartCycle must be accepted from Idle.");
    AssertEqual(EquipmentState.Loading, machine.CurrentState, "StartCycle must move equipment to Loading.");
    AssertEqual(true, io.Read(EquipmentIoMap.VacuumOn), "Vacuum must be on while Loading.");
    AssertEqual(false, io.Read(EquipmentIoMap.TowerLampRed), "Red lamp must stay off during normal Loading.");
}

static void CellControllerAdvancesSequenceFromIoInputs()
{
    var io = EquipmentIoMap.CreateDefaultCellIo();
    var machine = new EquipmentStateMachine();
    var cell = new EquipmentCellController(machine, io);

    cell.StartCycle();

    io.SetInput(EquipmentIoMap.LoadPresent, true);
    var loadResult = cell.PollInputs();
    AssertEqual(EquipmentEvent.LoadComplete, loadResult.Event, "LoadPresent must create LoadComplete.");
    AssertEqual(EquipmentState.Aligning, machine.CurrentState, "LoadPresent must move equipment to Aligning.");
    AssertEqual(true, io.Read(EquipmentIoMap.StageMoveRequested), "Stage move output must be on while Aligning.");

    io.SetInput(EquipmentIoMap.AlignmentDone, true);
    var alignResult = cell.PollInputs();
    AssertEqual(EquipmentEvent.AlignmentComplete, alignResult.Event, "AlignmentDone must create AlignmentComplete.");
    AssertEqual(EquipmentState.Inspecting, machine.CurrentState, "AlignmentDone must move equipment to Inspecting.");
    AssertEqual(false, io.Read(EquipmentIoMap.StageMoveRequested), "Stage move output must be off while Inspecting.");

    io.SetInput(EquipmentIoMap.InspectionDone, true);
    var inspectResult = cell.PollInputs();
    AssertEqual(EquipmentEvent.InspectionComplete, inspectResult.Event, "InspectionDone must create InspectionComplete.");
    AssertEqual(EquipmentState.Unloading, machine.CurrentState, "InspectionDone must move equipment to Unloading.");

    io.SetInput(EquipmentIoMap.UnloadComplete, true);
    var unloadResult = cell.PollInputs();
    AssertEqual(EquipmentEvent.UnloadComplete, unloadResult.Event, "UnloadComplete input must create UnloadComplete event.");
    AssertEqual(EquipmentState.Complete, machine.CurrentState, "UnloadComplete must move equipment to Complete.");
    AssertEqual(false, io.Read(EquipmentIoMap.VacuumOn), "Vacuum must be off after Complete.");
}

static void CellControllerPrioritizesSafetyInputs()
{
    var io = EquipmentIoMap.CreateDefaultCellIo();
    var machine = new EquipmentStateMachine();
    var cell = new EquipmentCellController(machine, io);

    cell.StartCycle();
    io.SetInput(EquipmentIoMap.LoadPresent, true);
    io.SetInput(EquipmentIoMap.EmergencyStopPressed, true);

    var result = cell.PollInputs();

    AssertEqual(EquipmentEvent.EmergencyStop, result.Event, "EmergencyStop must be processed before LoadPresent.");
    AssertEqual(EquipmentState.Alarmed, machine.CurrentState, "EmergencyStop must move equipment to Alarmed.");
    AssertEqual(true, io.Read(EquipmentIoMap.TowerLampRed), "Red lamp must be on while Alarmed.");
    AssertEqual(true, io.Read(EquipmentIoMap.BuzzerOn), "Buzzer must be on while Alarmed.");
    AssertEqual(false, io.Read(EquipmentIoMap.VacuumOn), "Vacuum must be off while Alarmed.");
}

static void CellControllerReportsNoChangeWhenSensorIsMissing()
{
    var io = EquipmentIoMap.CreateDefaultCellIo();
    var machine = new EquipmentStateMachine();
    var cell = new EquipmentCellController(machine, io);

    cell.StartCycle();
    var result = cell.PollInputs();

    AssertFalse(result.Changed, "PollInputs must report no change when no matching sensor is active.");
    AssertEqual(null, result.Event, "No event must be reported when no matching sensor is active.");
    AssertEqual(EquipmentState.Loading, machine.CurrentState, "State must stay Loading without LoadPresent.");
}

static void CellControllerAppliesTimeoutPolicy()
{
    var clock = new ManualClock(new DateTimeOffset(2026, 6, 25, 0, 0, 0, TimeSpan.Zero));
    var io = EquipmentIoMap.CreateDefaultCellIo();
    var machine = new EquipmentStateMachine(clock);
    var cell = new EquipmentCellController(machine, io);
    var policy = new StateTimeoutPolicy();
    policy.SetTimeout(EquipmentState.Loading, TimeSpan.FromSeconds(30));

    cell.StartCycle();
    clock.Advance(TimeSpan.FromSeconds(30));

    var result = cell.PollInputs(policy);

    AssertEqual(EquipmentEvent.Timeout, result.Event, "Timeout policy must create Timeout event.");
    AssertTrue(result.Timeout?.TimedOut == true, "Timeout result must be marked timed out.");
    AssertEqual(EquipmentState.Alarmed, machine.CurrentState, "Timeout must move equipment to Alarmed.");
    AssertEqual(true, io.Read(EquipmentIoMap.TowerLampRed), "Red lamp must be on after timeout.");
    AssertEqual(true, io.Read(EquipmentIoMap.BuzzerOn), "Buzzer must be on after timeout.");
}

static void ClearAlarmResetsNormalOutputs()
{
    var io = EquipmentIoMap.CreateDefaultCellIo();
    var machine = new EquipmentStateMachine();
    var cell = new EquipmentCellController(machine, io);

    cell.StartCycle();
    io.SetInput(EquipmentIoMap.DoorClosed, false);
    cell.PollInputs();
    io.SetInput(EquipmentIoMap.DoorClosed, true);

    var result = cell.ClearAlarm();

    AssertTrue(result.Accepted, "ClearAlarm must be accepted from Alarmed.");
    AssertEqual(EquipmentState.Idle, machine.CurrentState, "ClearAlarm must return to Idle.");
    AssertEqual(false, io.Read(EquipmentIoMap.TowerLampRed), "Red lamp must be off after ClearAlarm.");
    AssertEqual(false, io.Read(EquipmentIoMap.BuzzerOn), "Buzzer must be off after ClearAlarm.");
}

static void DoorAlarmCannotClearWhileDoorRemainsOpen()
{
    var io = EquipmentIoMap.CreateDefaultCellIo();
    var machine = new EquipmentStateMachine();
    var cell = new EquipmentCellController(machine, io);

    cell.StartCycle();
    io.SetInput(EquipmentIoMap.DoorClosed, false);
    cell.PollInputs();

    var result = cell.ClearAlarm();

    AssertFalse(result.Accepted, "ClearAlarm must be rejected while door remains open.");
    AssertEqual(EquipmentState.Alarmed, machine.CurrentState, "Door alarm must stay Alarmed while door remains open.");
    AssertEqual(AlarmCode.DoorOpened, result.Alarm?.Code, "Rejected ClearAlarm must keep DoorOpened alarm info.");
    AssertTrue(result.Message.Contains("Door must be closed"), "Rejected ClearAlarm must explain door recovery condition.");
    AssertEqual(true, io.Read(EquipmentIoMap.TowerLampRed), "Red lamp must stay on while alarm remains active.");
    AssertEqual(true, io.Read(EquipmentIoMap.BuzzerOn), "Buzzer must stay on while alarm remains active.");
}

static void DoorAlarmClearsAfterDoorCloses()
{
    var io = EquipmentIoMap.CreateDefaultCellIo();
    var machine = new EquipmentStateMachine();
    var cell = new EquipmentCellController(machine, io);

    cell.StartCycle();
    io.SetInput(EquipmentIoMap.DoorClosed, false);
    cell.PollInputs();
    io.SetInput(EquipmentIoMap.DoorClosed, true);

    var result = cell.ClearAlarm();

    AssertTrue(result.Accepted, "ClearAlarm must be accepted after door closes.");
    AssertEqual(EquipmentState.Idle, machine.CurrentState, "Door alarm must clear to Idle after door closes.");
    AssertEqual(null, machine.LastAlarm, "Door alarm info must clear after successful ClearAlarm.");
}

static void EmergencyStopAlarmCannotClearWhilePressed()
{
    var io = EquipmentIoMap.CreateDefaultCellIo();
    var machine = new EquipmentStateMachine();
    var cell = new EquipmentCellController(machine, io);

    cell.StartCycle();
    io.SetInput(EquipmentIoMap.EmergencyStopPressed, true);
    cell.PollInputs();

    var result = cell.ClearAlarm();

    AssertFalse(result.Accepted, "ClearAlarm must be rejected while emergency stop remains pressed.");
    AssertEqual(EquipmentState.Alarmed, machine.CurrentState, "EmergencyStop alarm must stay Alarmed while pressed.");
    AssertEqual(AlarmCode.EmergencyStop, result.Alarm?.Code, "Rejected ClearAlarm must keep EmergencyStop alarm info.");
    AssertTrue(result.Message.Contains("Emergency stop must be released"), "Rejected ClearAlarm must explain emergency stop recovery condition.");
}

static void EmergencyStopAlarmClearsAfterRelease()
{
    var io = EquipmentIoMap.CreateDefaultCellIo();
    var machine = new EquipmentStateMachine();
    var cell = new EquipmentCellController(machine, io);

    cell.StartCycle();
    io.SetInput(EquipmentIoMap.EmergencyStopPressed, true);
    cell.PollInputs();
    io.SetInput(EquipmentIoMap.EmergencyStopPressed, false);

    var result = cell.ClearAlarm();

    AssertTrue(result.Accepted, "ClearAlarm must be accepted after emergency stop releases.");
    AssertEqual(EquipmentState.Idle, machine.CurrentState, "EmergencyStop alarm must clear to Idle after release.");
    AssertEqual(null, machine.LastAlarm, "EmergencyStop alarm info must clear after successful ClearAlarm.");
}

static void TimeoutAlarmCanClearInMvpModel()
{
    var clock = new ManualClock(new DateTimeOffset(2026, 6, 28, 0, 0, 0, TimeSpan.Zero));
    var io = EquipmentIoMap.CreateDefaultCellIo();
    var machine = new EquipmentStateMachine(clock);
    var cell = new EquipmentCellController(machine, io);
    var policy = new StateTimeoutPolicy();
    policy.SetTimeout(EquipmentState.Loading, TimeSpan.FromSeconds(30));

    cell.StartCycle();
    clock.Advance(TimeSpan.FromSeconds(30));
    cell.PollInputs(policy);

    var result = cell.ClearAlarm();

    AssertTrue(result.Accepted, "Timeout alarm can be cleared in the MVP model.");
    AssertEqual(EquipmentState.Idle, machine.CurrentState, "Timeout alarm must clear to Idle in MVP model.");
}

static void MotionAxisStartsDisabled()
{
    var axis = CreateMotionAxis();

    AssertEqual(MotionAxisState.Disabled, axis.State, "New axis must start disabled.");
    AssertFalse(axis.ServoEnabled, "New axis servo must be disabled.");
    AssertFalse(axis.IsHomed, "New axis must not start homed.");
    AssertEqual(0.0, axis.Position, "New axis position mismatch.");
}

static void MotionAxisServoOnMovesToReady()
{
    var axis = CreateMotionAxis();

    var result = axis.ServoOn();

    AssertTrue(result.Accepted, "ServoOn must be accepted.");
    AssertTrue(axis.ServoEnabled, "Servo must be enabled.");
    AssertEqual(MotionAxisState.Ready, axis.State, "ServoOn must move an unhomed axis to Ready.");
}

static void MotionAxisHomeRequiresServoOn()
{
    var axis = CreateMotionAxis();

    var result = axis.StartHome(TimeSpan.FromSeconds(1));

    AssertTrue(result.Accepted, "Servo-off home attempt must be recorded as an alarm result.");
    AssertEqual(MotionAxisState.Alarmed, axis.State, "Home without servo must alarm.");
    AssertEqual(MotionAxisAlarmCode.ServoOff, axis.LastAlarm?.Code, "Home without servo must create ServoOff alarm.");
}

static void MotionAxisHomeCompletesAfterDuration()
{
    var clock = CreateMotionClock();
    var axis = new MotionAxis("X", clock, initialPosition: 12.5);

    axis.ServoOn();
    axis.StartHome(TimeSpan.FromSeconds(2));
    clock.Advance(TimeSpan.FromMilliseconds(1999));
    axis.Poll();

    AssertEqual(MotionAxisState.Homing, axis.State, "Axis must keep homing before duration expires.");

    clock.Advance(TimeSpan.FromMilliseconds(1));
    var result = axis.Poll();

    AssertTrue(result.Accepted, "Home completion poll must be accepted.");
    AssertEqual(MotionAxisState.InPosition, axis.State, "Axis must be InPosition after homing.");
    AssertTrue(axis.IsHomed, "Axis must be homed after homing completes.");
    AssertEqual(0.0, axis.Position, "Homing must move position to zero.");
}

static void MotionAxisMoveRequiresHome()
{
    var axis = CreateMotionAxis();

    axis.ServoOn();
    var result = axis.StartMove(25.0, TimeSpan.FromSeconds(1));

    AssertTrue(result.Accepted, "Unhomed move attempt must be recorded as an alarm result.");
    AssertEqual(MotionAxisState.Alarmed, axis.State, "Move before home must alarm.");
    AssertEqual(MotionAxisAlarmCode.NotHomed, axis.LastAlarm?.Code, "Move before home must create NotHomed alarm.");
}

static void MotionAxisMoveCompletesAfterDuration()
{
    var clock = CreateMotionClock();
    var axis = CreateHomedMotionAxis(clock);

    axis.StartMove(25.0, TimeSpan.FromSeconds(3));
    clock.Advance(TimeSpan.FromSeconds(3));
    var result = axis.Poll();

    AssertTrue(result.Accepted, "Move completion poll must be accepted.");
    AssertEqual(MotionAxisState.InPosition, axis.State, "Axis must be InPosition after move completes.");
    AssertEqual(25.0, axis.Position, "Axis position mismatch after move.");
    AssertEqual(null, axis.TargetPosition, "Target position must clear after move completes.");
}

static void MotionAxisTimeoutCreatesAlarm()
{
    var clock = CreateMotionClock();
    var axis = CreateHomedMotionAxis(clock);

    axis.StartMove(100.0, TimeSpan.FromSeconds(10));
    clock.Advance(TimeSpan.FromSeconds(6));
    var result = axis.CheckTimeout(TimeSpan.FromSeconds(5));

    AssertTrue(result.Accepted, "Motion timeout must be recorded as an accepted alarm result.");
    AssertEqual(MotionAxisState.Alarmed, axis.State, "Motion timeout must move axis to Alarmed.");
    AssertEqual(MotionAxisAlarmCode.MoveTimeout, axis.LastAlarm?.Code, "Motion timeout alarm code mismatch.");
}

static void MotionAxisClearAlarmReturnsToInPosition()
{
    var clock = CreateMotionClock();
    var axis = CreateHomedMotionAxis(clock);

    axis.StartMove(100.0, TimeSpan.FromSeconds(10));
    clock.Advance(TimeSpan.FromSeconds(6));
    axis.CheckTimeout(TimeSpan.FromSeconds(5));

    var result = axis.ClearAlarm();

    AssertTrue(result.Accepted, "ClearAlarm must be accepted for motion alarm.");
    AssertEqual(MotionAxisState.InPosition, axis.State, "Homed servo-on axis must return to InPosition after ClearAlarm.");
    AssertEqual(null, axis.LastAlarm, "Motion alarm must clear.");
}

static void EquipmentTemplateJsonLoadsVisionInspectionCellFile()
{
    var template = LoadTemplate("vision-inspection-cell.json");

    AssertEqual("vision-inspection-cell", template.Name, "Template name mismatch.");
    AssertEqual(2, template.MotionAxes.Count, "Template motion axis count mismatch.");
    AssertEqual(2, template.ProductRecipes.Count, "Template product recipe count mismatch.");
    AssertEqual(2, template.FaultScenarios.Count, "Template fault scenario count mismatch.");
    AssertEqual(InspectionMode.DatasetCamera, template.ProductRecipes[0].InspectionMode, "Inspection mode mismatch.");
    AssertEqual(InspectionOutcome.Pass, template.ProductRecipes[0].InspectionResult?.Outcome, "Default panel inspection outcome mismatch.");
    AssertTrue(template.ProductRecipes[0].InspectionResult!.Measurements.ContainsKey("edgeOffsetMm"), "Default panel inspection must include edge offset measurement.");
    AssertEqual(InspectionOutcome.Fail, template.ProductRecipes[1].InspectionResult?.Outcome, "Tall part inspection outcome mismatch.");
    AssertEqual("HEIGHT_OVER_LIMIT", template.ProductRecipes[1].InspectionResult?.DefectCode, "Tall part defect code mismatch.");
    AssertTrue(template.ProductRecipes[0].TryGetAxisTarget("x", out var xTarget), "Recipe must find X target case insensitively.");
    AssertEqual(25.0, xTarget, "Default panel X target mismatch.");
}

static void EquipmentTemplateCreatesMotionAxes()
{
    var template = LoadTemplate("vision-inspection-cell.json");
    var axes = template.CreateMotionAxes(CreateMotionClock());

    AssertEqual(2, axes.Count, "Created motion axis count mismatch.");
    AssertTrue(axes.TryGetValue("X", out var xAxis), "Template must create X axis.");
    AssertTrue(axes.TryGetValue("z", out var zAxis), "Template must create Z axis case insensitively.");
    AssertEqual(MotionAxisState.Disabled, xAxis!.State, "Created X axis must start disabled.");
    AssertEqual(0.0, zAxis!.Position, "Created Z axis initial position mismatch.");
}

static void EquipmentTemplateFindsProductRecipeCaseInsensitively()
{
    var template = LoadTemplate("vision-inspection-cell.json");

    var recipe = template.FindProductRecipe("TALL-PART");

    AssertEqual("PART-TALL", recipe.ProductCode, "Recipe product code mismatch.");
    AssertTrue(recipe.TryGetAxisTarget("z", out var zTarget), "Recipe must find Z target case insensitively.");
    AssertEqual(12.0, zTarget, "Tall part Z target mismatch.");
}

static void EquipmentTemplateFindsFaultScenarioCaseInsensitively()
{
    var template = LoadTemplate("vision-inspection-cell.json");

    var fault = template.FindFaultScenario("X-AXIS-MOVE-TIMEOUT");

    AssertEqual(FaultKind.MotionTimeout, fault.Kind, "Fault kind mismatch.");
    AssertEqual("X", fault.Axis, "Fault axis mismatch.");
    AssertEqual(500, fault.TimeoutMilliseconds, "Fault timeout mismatch.");
}

static void EquipmentTemplateRejectsDuplicateMotionAxes()
{
    AssertThrows<InvalidOperationException>(
        () => EquipmentTemplate.FromJson(
            """
            {
              "name": "bad-template",
              "motionAxes": [
                { "name": "X" },
                { "name": "x" }
              ],
              "productRecipes": [
                {
                  "name": "default",
                  "productCode": "P1",
                  "axisTargets": { "X": 1 }
                }
              ]
            }
            """),
        "Duplicate motion axes must be rejected.");
}

static void EquipmentTemplateRejectsUnknownRecipeAxis()
{
    AssertThrows<InvalidOperationException>(
        () => EquipmentTemplate.FromJson(
            """
            {
              "name": "bad-template",
              "motionAxes": [
                { "name": "X" }
              ],
              "productRecipes": [
                {
                  "name": "default",
                  "productCode": "P1",
                  "axisTargets": { "Z": 1 }
                }
              ]
            }
            """),
        "Recipe targets for unknown axes must be rejected.");
}

static void EquipmentTemplateRejectsMissingInspectionResult()
{
    AssertThrows<InvalidOperationException>(
        () => EquipmentTemplate.FromJson(
            """
            {
              "name": "bad-template",
              "motionAxes": [
                { "name": "X" }
              ],
              "productRecipes": [
                {
                  "name": "default",
                  "productCode": "P1",
                  "inspectionMode": "DatasetCamera",
                  "axisTargets": { "X": 1 }
                }
              ]
            }
            """),
        "DatasetCamera recipe must define an inspection result.");
}

static void EquipmentTemplateRejectsFailedInspectionWithoutDefectCode()
{
    AssertThrows<InvalidOperationException>(
        () => EquipmentTemplate.FromJson(
            """
            {
              "name": "bad-template",
              "motionAxes": [
                { "name": "X" }
              ],
              "productRecipes": [
                {
                  "name": "default",
                  "productCode": "P1",
                  "inspectionMode": "DatasetCamera",
                  "inspectionResult": {
                    "outcome": "Fail"
                  },
                  "axisTargets": { "X": 1 }
                }
              ]
            }
            """),
        "Failed inspection result must define a defect code.");
}

static void EquipmentTemplateRejectsUnknownFaultAxis()
{
    AssertThrows<InvalidOperationException>(
        () => EquipmentTemplate.FromJson(
            """
            {
              "name": "bad-template",
              "motionAxes": [
                { "name": "X" }
              ],
              "productRecipes": [
                {
                  "name": "default",
                  "productCode": "P1",
                  "axisTargets": { "X": 1 }
                }
              ],
              "faultScenarios": [
                {
                  "name": "bad-fault",
                  "kind": "ServoAlarm",
                  "axis": "Z",
                  "message": "Unknown axis fault."
                }
              ]
            }
            """),
        "Fault targets for unknown axes must be rejected.");
}

static void EquipmentTemplateRejectsInvalidTimeoutFault()
{
    AssertThrows<InvalidOperationException>(
        () => EquipmentTemplate.FromJson(
            """
            {
              "name": "bad-template",
              "motionAxes": [
                { "name": "X" }
              ],
              "productRecipes": [
                {
                  "name": "default",
                  "productCode": "P1",
                  "axisTargets": { "X": 1 }
                }
              ],
              "faultScenarios": [
                {
                  "name": "bad-timeout",
                  "kind": "MotionTimeout",
                  "axis": "X",
                  "timeoutMilliseconds": 1000,
                  "elapsedMilliseconds": 500
                }
              ]
            }
            """),
        "Timeout fault elapsed time must reach timeout.");
}

static void TemplateRunnerRunsDefaultPanelRecipe()
{
    var template = LoadTemplate("vision-inspection-cell.json");
    var runner = new TemplateRunner(CreateMotionClock());

    var result = runner.RunRecipe(template, "default-panel");

    AssertTrue(result.Success, "Template runner must complete default-panel recipe.");
    AssertEqual("vision-inspection-cell", result.TemplateName, "Template result name mismatch.");
    AssertEqual("PANEL-A", result.Recipe.ProductCode, "Recipe product code mismatch.");
    AssertEqual(10, result.CommandLog.Count, "Default two-axis recipe command count mismatch.");
    AssertEqual(MotionAxisState.InPosition, result.MotionAxes["X"].State, "X axis must end InPosition.");
    AssertEqual(25.0, result.MotionAxes["X"].Position, "X axis final position mismatch.");
    AssertEqual(5.0, result.MotionAxes["Z"].Position, "Z axis final position mismatch.");
    AssertTrue(result.ProductPassed == true, "Default panel product must pass inspection.");
    AssertEqual(InspectionOutcome.Pass, result.InspectionResult?.Outcome, "Default panel inspection outcome mismatch.");
    AssertTrue(result.InspectionResult!.TryGetMeasurement("surfaceScore", out var surfaceScore), "Inspection result must expose measurements case insensitively.");
    AssertEqual(98.5, surfaceScore, "Surface score mismatch.");
}

static void TemplateRunnerRunsTallPartRecipe()
{
    var template = LoadTemplate("vision-inspection-cell.json");
    var runner = new TemplateRunner(CreateMotionClock());

    var result = runner.RunRecipe(template, "tall-part");

    AssertTrue(result.Success, "Template runner must complete tall-part recipe.");
    AssertEqual("PART-TALL", result.Recipe.ProductCode, "Tall part product code mismatch.");
    AssertEqual(40.0, result.MotionAxes["X"].Position, "Tall part X target mismatch.");
    AssertEqual(12.0, result.MotionAxes["Z"].Position, "Tall part Z target mismatch.");
    AssertTrue(result.ProductPassed == false, "Tall part product must fail inspection.");
    AssertEqual(InspectionOutcome.Fail, result.InspectionResult?.Outcome, "Tall part inspection outcome mismatch.");
    AssertEqual("HEIGHT_OVER_LIMIT", result.InspectionResult?.DefectCode, "Tall part defect code mismatch.");
}

static void TemplateRunnerInjectsMotionTimeoutFault()
{
    var template = LoadTemplate("vision-inspection-cell.json");
    var runner = new TemplateRunner(CreateMotionClock());

    var result = runner.RunRecipe(template, "default-panel", "x-axis-move-timeout");

    AssertFalse(result.Success, "Motion timeout fault must fail the template run.");
    AssertEqual("x-axis-move-timeout", result.FaultScenario?.Name, "Fault scenario name mismatch.");
    AssertEqual(MotionAxisState.Alarmed, result.MotionAxes["X"].State, "X axis must end Alarmed after timeout fault.");
    AssertEqual(MotionAxisAlarmCode.MoveTimeout, result.MotionAxes["X"].LastAlarm?.Code, "Timeout fault alarm mismatch.");
    AssertFalse(result.MotionAxes["Z"].IsHomed && result.MotionAxes["Z"].Position > 0, "Z move must not continue after X fault.");
    AssertTrue(result.CommandLog.Any(log => log.Step == "FaultMotionTimeout"), "Command log must record motion timeout fault.");
    AssertEqual(null, result.InspectionResult, "Inspection must not run after motion timeout fault.");
}

static void TemplateRunnerInjectsServoAlarmFault()
{
    var template = LoadTemplate("vision-inspection-cell.json");
    var runner = new TemplateRunner(CreateMotionClock());

    var result = runner.RunRecipe(template, "default-panel", "z-axis-servo-alarm");

    AssertFalse(result.Success, "Servo alarm fault must fail the template run.");
    AssertEqual(MotionAxisState.InPosition, result.MotionAxes["X"].State, "X axis must complete before Z fault.");
    AssertEqual(25.0, result.MotionAxes["X"].Position, "X axis final position mismatch before Z fault.");
    AssertEqual(MotionAxisState.Alarmed, result.MotionAxes["Z"].State, "Z axis must end Alarmed after servo fault.");
    AssertEqual(MotionAxisAlarmCode.ServoAlarm, result.MotionAxes["Z"].LastAlarm?.Code, "Servo fault alarm mismatch.");
    AssertTrue(result.CommandLog.Any(log => log.Step == "FaultServoAlarm"), "Command log must record servo alarm fault.");
    AssertEqual(null, result.InspectionResult, "Inspection must not run after servo alarm fault.");
}

static void TemplateRunnerRejectsMissingRecipe()
{
    var template = LoadTemplate("vision-inspection-cell.json");
    var runner = new TemplateRunner(CreateMotionClock());

    AssertThrows<InvalidOperationException>(
        () => runner.RunRecipe(template, "missing-recipe"),
        "Template runner must reject unknown recipe names.");
}

static void TemplateRunnerRejectsMissingFaultScenario()
{
    var template = LoadTemplate("vision-inspection-cell.json");
    var runner = new TemplateRunner(CreateMotionClock());

    AssertThrows<InvalidOperationException>(
        () => runner.RunRecipe(template, "default-panel", "missing-fault"),
        "Template runner must reject unknown fault scenario names.");
}

static void TemplateRunnerRejectsInvalidDurations()
{
    AssertThrows<InvalidOperationException>(
        () => new TemplateRunner(
            CreateMotionClock(),
            new TemplateRunnerOptions
            {
                HomeDuration = TimeSpan.Zero,
                MoveDuration = TimeSpan.FromSeconds(1)
            }),
        "Template runner must reject invalid home duration.");

    AssertThrows<InvalidOperationException>(
        () => new TemplateRunner(
            CreateMotionClock(),
            new TemplateRunnerOptions
            {
                HomeDuration = TimeSpan.FromSeconds(1),
                MoveDuration = TimeSpan.Zero
            }),
        "Template runner must reject invalid move duration.");
}

static void ScenarioJsonLoadsNormalCycleFile()
{
    var scenario = LoadScenario("normal-cycle.json");

    AssertEqual("normal-cycle", scenario.Name, "Scenario name mismatch.");
    AssertEqual(9, scenario.Steps.Count, "Normal cycle step count mismatch.");
    AssertEqual(ScenarioStepAction.StartCycle, scenario.Steps[0].Action, "First scenario step mismatch.");
}

static void ScenarioRunnerCompletesNormalCycleFile()
{
    var scenario = LoadScenario("normal-cycle.json");
    var runner = ScenarioRunner.CreateDefault(new DateTimeOffset(2026, 6, 25, 0, 0, 0, TimeSpan.Zero));

    var result = runner.Run(scenario);

    AssertTrue(result.Success, ScenarioFailureMessage(result));
    AssertEqual(EquipmentState.Complete, result.FinalState, "Normal cycle scenario must end at Complete.");
    AssertEqual(false, runner.Cell.Io.Read(EquipmentIoMap.VacuumOn), "Vacuum must be off after normal cycle scenario.");
}

static void ScenarioRunnerHandlesLoadingTimeoutFile()
{
    var scenario = LoadScenario("loading-timeout.json");
    var runner = ScenarioRunner.CreateDefault(
        new DateTimeOffset(2026, 6, 25, 0, 0, 0, TimeSpan.Zero),
        StateTimeoutPolicy.CreateDefaultMvpPolicy());

    var result = runner.Run(scenario);

    AssertTrue(result.Success, ScenarioFailureMessage(result));
    AssertEqual(EquipmentState.Alarmed, result.FinalState, "Timeout scenario must end at Alarmed.");
    AssertEqual(true, runner.Cell.Io.Read(EquipmentIoMap.TowerLampRed), "Red lamp must be on after timeout scenario.");
    AssertEqual(true, runner.Cell.Io.Read(EquipmentIoMap.BuzzerOn), "Buzzer must be on after timeout scenario.");
}

static void ScenarioRunnerHandlesDoorOpenAlarmFile()
{
    var scenario = LoadScenario("door-open-alarm.json");
    var runner = ScenarioRunner.CreateDefault(new DateTimeOffset(2026, 6, 25, 0, 0, 0, TimeSpan.Zero));

    var result = runner.Run(scenario);

    AssertTrue(result.Success, ScenarioFailureMessage(result));
    AssertEqual(EquipmentState.Alarmed, result.FinalState, "Door open alarm scenario must end at Alarmed.");
    AssertEqual(false, runner.Cell.Io.Read(EquipmentIoMap.VacuumOn), "Vacuum must be off after door open alarm.");
    AssertEqual(true, runner.Cell.Io.Read(EquipmentIoMap.TowerLampRed), "Red lamp must be on after door open alarm.");
    AssertEqual(true, runner.Cell.Io.Read(EquipmentIoMap.BuzzerOn), "Buzzer must be on after door open alarm.");
}

static void ScenarioRunnerHandlesEmergencyStopAlarmFile()
{
    var scenario = LoadScenario("emergency-stop-alarm.json");
    var runner = ScenarioRunner.CreateDefault(new DateTimeOffset(2026, 6, 25, 0, 0, 0, TimeSpan.Zero));

    var result = runner.Run(scenario);

    AssertTrue(result.Success, ScenarioFailureMessage(result));
    AssertEqual(EquipmentState.Alarmed, result.FinalState, "Emergency stop scenario must end at Alarmed.");
    AssertEqual(false, runner.Cell.Io.Read(EquipmentIoMap.VacuumOn), "Vacuum must be off after emergency stop.");
    AssertEqual(true, runner.Cell.Io.Read(EquipmentIoMap.TowerLampRed), "Red lamp must be on after emergency stop.");
    AssertEqual(true, runner.Cell.Io.Read(EquipmentIoMap.BuzzerOn), "Buzzer must be on after emergency stop.");
}

static void ScenarioRunnerHandlesClearAlarmRecoveryFile()
{
    var scenario = LoadScenario("clear-alarm-recovery.json");
    var runner = ScenarioRunner.CreateDefault(new DateTimeOffset(2026, 6, 25, 0, 0, 0, TimeSpan.Zero));

    var result = runner.Run(scenario);

    AssertTrue(result.Success, ScenarioFailureMessage(result));
    AssertEqual(EquipmentState.Idle, result.FinalState, "Clear alarm recovery scenario must end at Idle.");
    AssertEqual(false, runner.Cell.Io.Read(EquipmentIoMap.VacuumOn), "Vacuum must be off after ClearAlarm.");
    AssertEqual(false, runner.Cell.Io.Read(EquipmentIoMap.TowerLampRed), "Red lamp must be off after ClearAlarm.");
    AssertEqual(false, runner.Cell.Io.Read(EquipmentIoMap.BuzzerOn), "Buzzer must be off after ClearAlarm.");
}

static void ScenarioRunnerHandlesDoorOpenClearBlockedFile()
{
    var scenario = LoadScenario("door-open-clear-blocked.json");
    var runner = ScenarioRunner.CreateDefault(new DateTimeOffset(2026, 6, 28, 0, 0, 0, TimeSpan.Zero));

    var result = runner.Run(scenario);

    AssertTrue(result.Success, ScenarioFailureMessage(result));
    AssertEqual(EquipmentState.Alarmed, result.FinalState, "Door-open blocked recovery scenario must stay Alarmed.");
    AssertEqual(AlarmCode.DoorOpened, runner.Cell.StateMachine.LastAlarm?.Code, "Door-open blocked recovery must keep DoorOpened alarm.");
    AssertEqual(true, runner.Cell.Io.Read(EquipmentIoMap.TowerLampRed), "Red lamp must stay on while ClearAlarm is blocked.");
    AssertEqual(true, runner.Cell.Io.Read(EquipmentIoMap.BuzzerOn), "Buzzer must stay on while ClearAlarm is blocked.");
}

static void ScenarioRunnerHandlesEmergencyStopRecoveryFile()
{
    var scenario = LoadScenario("emergency-stop-recovery.json");
    var runner = ScenarioRunner.CreateDefault(new DateTimeOffset(2026, 6, 28, 0, 0, 0, TimeSpan.Zero));

    var result = runner.Run(scenario);

    AssertTrue(result.Success, ScenarioFailureMessage(result));
    AssertEqual(EquipmentState.Idle, result.FinalState, "Emergency stop recovery scenario must end at Idle.");
    AssertEqual(null, runner.Cell.StateMachine.LastAlarm, "Emergency stop recovery scenario must clear alarm info.");
    AssertEqual(false, runner.Cell.Io.Read(EquipmentIoMap.TowerLampRed), "Red lamp must be off after emergency stop recovery.");
    AssertEqual(false, runner.Cell.Io.Read(EquipmentIoMap.BuzzerOn), "Buzzer must be off after emergency stop recovery.");
}

static void ScenarioRunnerHandlesMotionAxisNormalFile()
{
    var scenario = LoadScenario("motion-axis-normal.json");
    var runner = ScenarioRunner.CreateDefault(new DateTimeOffset(2026, 7, 1, 0, 0, 0, TimeSpan.Zero));

    var result = runner.Run(scenario);

    AssertTrue(result.Success, ScenarioFailureMessage(result));
    AssertTrue(runner.MotionAxes.TryGetValue("X", out var axis), "Motion scenario must create X axis.");
    AssertEqual(MotionAxisState.InPosition, axis!.State, "X axis must end InPosition.");
    AssertEqual(25.0, axis.Position, "X axis final position mismatch.");
    AssertEqual(null, axis.LastAlarm, "Normal motion scenario must not leave an alarm.");
}

static void ScenarioRunnerHandlesMotionAxisTimeoutFile()
{
    var scenario = LoadScenario("motion-axis-timeout.json");
    var runner = ScenarioRunner.CreateDefault(new DateTimeOffset(2026, 7, 1, 0, 0, 0, TimeSpan.Zero));

    var result = runner.Run(scenario);

    AssertTrue(result.Success, ScenarioFailureMessage(result));
    AssertTrue(runner.MotionAxes.TryGetValue("X", out var axis), "Motion timeout scenario must create X axis.");
    AssertEqual(MotionAxisState.Alarmed, axis!.State, "X axis must end Alarmed after timeout.");
    AssertEqual(MotionAxisAlarmCode.MoveTimeout, axis.LastAlarm?.Code, "Motion timeout scenario alarm mismatch.");
}

static void ScenarioRunnerReportsExpectationFailure()
{
    var scenario = EquipmentScenario.FromJson(
        """
        {
          "name": "bad-expectation",
          "steps": [
            {
              "name": "Start cycle",
              "action": "StartCycle",
              "expectState": "Complete"
            }
          ]
        }
        """);
    var runner = ScenarioRunner.CreateDefault(new DateTimeOffset(2026, 6, 25, 0, 0, 0, TimeSpan.Zero));

    var result = runner.Run(scenario);

    AssertFalse(result.Success, "Scenario with wrong expected state must fail.");
    AssertEqual(1, result.FailedSteps.Count, "Failed step count mismatch.");
    AssertTrue(result.FailedSteps[0].ValidationErrors[0].Contains("Expected state"), "Failure must explain expected state mismatch.");
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

static ManualClock CreateMotionClock()
{
    return new ManualClock(new DateTimeOffset(2026, 6, 30, 0, 0, 0, TimeSpan.Zero));
}

static MotionAxis CreateMotionAxis()
{
    return new MotionAxis("X", CreateMotionClock());
}

static MotionAxis CreateHomedMotionAxis(ManualClock clock)
{
    var axis = new MotionAxis("X", clock);

    axis.ServoOn();
    axis.StartHome(TimeSpan.FromSeconds(1));
    clock.Advance(TimeSpan.FromSeconds(1));
    axis.Poll();

    AssertEqual(MotionAxisState.InPosition, axis.State, "Test helper must create a homed axis.");
    return axis;
}

static EquipmentScenario LoadScenario(string fileName)
{
    var root = FindRepositoryRoot();
    var path = Path.Combine(root, "scenarios", fileName);
    var json = File.ReadAllText(path);
    return EquipmentScenario.FromJson(json);
}

static EquipmentTemplate LoadTemplate(string fileName)
{
    var root = FindRepositoryRoot();
    var path = Path.Combine(root, "templates", fileName);
    var json = File.ReadAllText(path);
    return EquipmentTemplate.FromJson(json);
}

static string FindRepositoryRoot()
{
    var directory = new DirectoryInfo(AppContext.BaseDirectory);

    while (directory is not null)
    {
        if (File.Exists(Path.Combine(directory.FullName, "EquipmentTwinLab.sln")))
        {
            return directory.FullName;
        }

        directory = directory.Parent;
    }

    throw new DirectoryNotFoundException("Could not find repository root containing EquipmentTwinLab.sln.");
}

static string ScenarioFailureMessage(ScenarioRunResult result)
{
    if (result.Success)
    {
        return "Scenario succeeded.";
    }

    return string.Join(
        Environment.NewLine,
        result.FailedSteps.Select(step => $"Step {step.Index} '{step.Name}' failed: {string.Join("; ", step.ValidationErrors)}"));
}
