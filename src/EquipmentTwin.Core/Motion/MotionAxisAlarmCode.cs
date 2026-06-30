namespace EquipmentTwin.Core.Motion;

/// <summary>
/// 모션 축에서 발생할 수 있는 MVP 수준의 알람 코드다.
/// 실제 장비 알람 번호 체계가 아니라, 모션 모델 검증용 분류다.
/// </summary>
public enum MotionAxisAlarmCode
{
    None = 0,
    ServoOff = 2001,
    NotHomed = 2002,
    MoveTimeout = 2003,
    ServoAlarm = 2004
}
