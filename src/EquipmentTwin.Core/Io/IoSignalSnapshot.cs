namespace EquipmentTwin.Core.Io;

/// <summary>
/// 특정 시점의 IO 신호 값이다.
/// UI, 로그, 테스트는 Snapshot을 보고 현재 가상 장비 신호를 확인한다.
/// </summary>
public sealed record IoSignalSnapshot(
    string Name,
    IoSignalDirection Direction,
    bool Value,
    string Description,
    DateTimeOffset UpdatedAtUtc);
