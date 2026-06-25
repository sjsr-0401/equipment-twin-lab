namespace EquipmentTwin.Core.Io;

/// <summary>
/// IO 신호의 이름, 방향, 설명을 정의한다.
/// </summary>
public sealed record IoSignalDefinition(
    string Name,
    IoSignalDirection Direction,
    string Description);
