namespace EquipmentTwin.Core.Io;

/// <summary>
/// PLC/IO 기준의 신호 방향이다.
/// Input은 센서처럼 장비 SW가 읽는 값이고, Output은 밸브/램프처럼 장비 SW가 쓰는 값이다.
/// </summary>
public enum IoSignalDirection
{
    Input,
    Output
}
