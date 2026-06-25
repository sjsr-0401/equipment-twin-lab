namespace EquipmentTwin.Core;

/// <summary>
/// 장비 상태머신에 들어오는 외부/내부 이벤트다.
/// 실제 장비에서는 PLC 신호, 센서 신호, 작업 완료 신호, 사용자 조작이 이런 이벤트로 변환된다.
/// </summary>
public enum EquipmentEvent
{
    StartLoad,
    LoadComplete,
    AlignmentComplete,
    InspectionComplete,
    UnloadComplete,
    Reset,
    DoorOpened,
    EmergencyStop,
    ClearAlarm
}
