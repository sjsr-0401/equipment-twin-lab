namespace EquipmentTwin.Core;

/// <summary>
/// 장비가 현재 어느 공정 단계에 있는지 표현한다.
/// </summary>
public enum EquipmentState
{
    Idle,
    Loading,
    Aligning,
    Inspecting,
    Unloading,
    Complete,
    Alarmed
}
