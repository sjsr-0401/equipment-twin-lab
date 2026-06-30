namespace EquipmentTwin.Core.Motion;

/// <summary>
/// 모션 축 명령 실행 결과다.
/// 상태 변경, 거부 사유, 알람 정보를 테스트와 리포트에서 같은 형식으로 읽기 위해 둔다.
/// </summary>
public sealed record MotionCommandResult(
    string AxisName,
    MotionAxisState State,
    bool Accepted,
    string Message,
    MotionAxisAlarm? Alarm = null);
