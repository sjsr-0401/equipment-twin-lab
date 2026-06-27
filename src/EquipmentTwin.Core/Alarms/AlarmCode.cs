namespace EquipmentTwin.Core.Alarms;

/// <summary>
/// 장비 알람을 현장/로그/문서에서 같은 이름으로 추적하기 위한 코드다.
/// 실제 장비 알람 번호 체계가 아니라 MVP용 소프트웨어 모델이다.
/// </summary>
public enum AlarmCode
{
    None = 0,
    DoorOpened = 1001,
    EmergencyStop = 1002,
    StateTimeout = 1003
}
