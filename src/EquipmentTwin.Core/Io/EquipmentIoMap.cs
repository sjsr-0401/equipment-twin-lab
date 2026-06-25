namespace EquipmentTwin.Core.Io;

/// <summary>
/// 첫 MVP 장비 셀에서 사용할 기본 IO 이름 모음이다.
/// 실제 장비에서는 이 이름들이 PLC 주소 또는 태그와 매핑된다.
/// </summary>
public static class EquipmentIoMap
{
    public const string DoorClosed = "DI_DOOR_CLOSED";
    public const string EmergencyStopPressed = "DI_EMERGENCY_STOP_PRESSED";
    public const string LoadPresent = "DI_LOAD_PRESENT";
    public const string AlignmentDone = "DI_ALIGNMENT_DONE";
    public const string InspectionDone = "DI_INSPECTION_DONE";

    public const string VacuumOn = "DO_VACUUM_ON";
    public const string StageMoveRequested = "DO_STAGE_MOVE_REQUESTED";
    public const string TowerLampRed = "DO_TOWER_LAMP_RED";
    public const string BuzzerOn = "DO_BUZZER_ON";

    public static VirtualIoController CreateDefaultCellIo()
    {
        var io = new VirtualIoController();

        io.DefineInput(DoorClosed, "Door closed safety input.", initialValue: true);
        io.DefineInput(EmergencyStopPressed, "Emergency stop input.", initialValue: false);
        io.DefineInput(LoadPresent, "Material or wafer present sensor.", initialValue: false);
        io.DefineInput(AlignmentDone, "Alignment complete sensor.", initialValue: false);
        io.DefineInput(InspectionDone, "Inspection complete signal.", initialValue: false);

        io.DefineOutput(VacuumOn, "Vacuum command output.", initialValue: false);
        io.DefineOutput(StageMoveRequested, "Stage move request output.", initialValue: false);
        io.DefineOutput(TowerLampRed, "Red tower lamp output.", initialValue: false);
        io.DefineOutput(BuzzerOn, "Buzzer output.", initialValue: false);

        return io;
    }
}
