using EquipmentTwin.Core.Io;

namespace EquipmentTwin.Core;

/// <summary>
/// 상태머신과 가상 IO를 연결하는 얇은 장비 셀 컨트롤러다.
/// 실제 PLC가 없어도 IO 입력 변화가 장비 상태 전이로 이어지는지 검증할 수 있다.
/// </summary>
public sealed class EquipmentCellController
{
    public EquipmentCellController(EquipmentStateMachine stateMachine, VirtualIoController io)
    {
        StateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
        Io = io ?? throw new ArgumentNullException(nameof(io));
        SyncOutputsForCurrentState();
    }

    public EquipmentStateMachine StateMachine { get; }

    public VirtualIoController Io { get; }

    public TransitionResult StartCycle()
    {
        if (!Io.Read(EquipmentIoMap.DoorClosed))
        {
            return ApplyAndSync(EquipmentEvent.DoorOpened);
        }

        if (Io.Read(EquipmentIoMap.EmergencyStopPressed))
        {
            return ApplyAndSync(EquipmentEvent.EmergencyStop);
        }

        return ApplyAndSync(EquipmentEvent.StartLoad);
    }

    public TransitionResult ClearAlarm()
    {
        return ApplyAndSync(EquipmentEvent.ClearAlarm);
    }

    public EquipmentCellStepResult PollInputs(StateTimeoutPolicy? timeoutPolicy = null)
    {
        if (StateMachine.CurrentState == EquipmentState.Alarmed)
        {
            SyncOutputsForCurrentState();
            return EquipmentCellStepResult.NoChange("Equipment is alarmed. Clear the alarm before polling normal IO.");
        }

        var safetyEvent = ReadSafetyEvent();
        if (safetyEvent is not null)
        {
            var transition = ApplyAndSync(safetyEvent.Value);
            return EquipmentCellStepResult.FromTransition(safetyEvent.Value, transition);
        }

        if (timeoutPolicy is not null)
        {
            var timeout = StateMachine.CheckTimeout(timeoutPolicy);
            if (timeout.TimedOut)
            {
                SyncOutputsForCurrentState();
                return EquipmentCellStepResult.FromTimeout(timeout);
            }
        }

        var processEvent = ReadProcessEventForCurrentState();
        if (processEvent is null)
        {
            SyncOutputsForCurrentState();
            return EquipmentCellStepResult.NoChange($"No IO event matched state '{StateMachine.CurrentState}'.");
        }

        var processTransition = ApplyAndSync(processEvent.Value);
        return EquipmentCellStepResult.FromTransition(processEvent.Value, processTransition);
    }

    private EquipmentEvent? ReadSafetyEvent()
    {
        if (!Io.Read(EquipmentIoMap.DoorClosed))
        {
            return EquipmentEvent.DoorOpened;
        }

        if (Io.Read(EquipmentIoMap.EmergencyStopPressed))
        {
            return EquipmentEvent.EmergencyStop;
        }

        return null;
    }

    private EquipmentEvent? ReadProcessEventForCurrentState()
    {
        return StateMachine.CurrentState switch
        {
            EquipmentState.Loading when Io.Read(EquipmentIoMap.LoadPresent) => EquipmentEvent.LoadComplete,
            EquipmentState.Aligning when Io.Read(EquipmentIoMap.AlignmentDone) => EquipmentEvent.AlignmentComplete,
            EquipmentState.Inspecting when Io.Read(EquipmentIoMap.InspectionDone) => EquipmentEvent.InspectionComplete,
            EquipmentState.Unloading when Io.Read(EquipmentIoMap.UnloadComplete) => EquipmentEvent.UnloadComplete,
            _ => null
        };
    }

    private TransitionResult ApplyAndSync(EquipmentEvent equipmentEvent)
    {
        var transition = StateMachine.Apply(equipmentEvent);
        SyncOutputsForCurrentState();
        return transition;
    }

    private void SyncOutputsForCurrentState()
    {
        switch (StateMachine.CurrentState)
        {
            case EquipmentState.Idle:
            case EquipmentState.Complete:
                SetNormalOutputs(vacuumOn: false, stageMoveRequested: false);
                break;

            case EquipmentState.Loading:
            case EquipmentState.Inspecting:
            case EquipmentState.Unloading:
                SetNormalOutputs(vacuumOn: true, stageMoveRequested: false);
                break;

            case EquipmentState.Aligning:
                SetNormalOutputs(vacuumOn: true, stageMoveRequested: true);
                break;

            case EquipmentState.Alarmed:
                Io.WriteOutput(EquipmentIoMap.VacuumOn, false);
                Io.WriteOutput(EquipmentIoMap.StageMoveRequested, false);
                Io.WriteOutput(EquipmentIoMap.TowerLampRed, true);
                Io.WriteOutput(EquipmentIoMap.BuzzerOn, true);
                break;

            default:
                throw new InvalidOperationException($"Unsupported equipment state '{StateMachine.CurrentState}'.");
        }
    }

    private void SetNormalOutputs(bool vacuumOn, bool stageMoveRequested)
    {
        Io.WriteOutput(EquipmentIoMap.VacuumOn, vacuumOn);
        Io.WriteOutput(EquipmentIoMap.StageMoveRequested, stageMoveRequested);
        Io.WriteOutput(EquipmentIoMap.TowerLampRed, false);
        Io.WriteOutput(EquipmentIoMap.BuzzerOn, false);
    }
}
