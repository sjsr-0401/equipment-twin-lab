namespace EquipmentTwin.Core.Io;

/// <summary>
/// IO 값이 바뀐 기록이다.
/// 실제 장비 로그에서는 누가, 언제, 어떤 신호를 바꿨는지가 문제 분석에 중요하다.
/// </summary>
public sealed record IoChange(
    string Name,
    IoSignalDirection Direction,
    bool PreviousValue,
    bool CurrentValue,
    string Source,
    DateTimeOffset OccurredAtUtc);
