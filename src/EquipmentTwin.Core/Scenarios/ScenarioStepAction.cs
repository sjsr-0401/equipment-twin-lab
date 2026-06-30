namespace EquipmentTwin.Core.Scenarios;

/// <summary>
/// 시나리오 한 단계에서 수행할 동작이다.
/// JSON 파일에서 장비 운전 흐름을 표현할 때 사용한다.
/// </summary>
public enum ScenarioStepAction
{
    StartCycle,
    SetInput,
    AdvanceTime,
    PollInputs,
    ClearAlarm,
    ExpectState,
    ExpectSignal,
    MotionServoOn,
    StartMotionHome,
    StartMotionMove,
    PollMotion,
    CheckMotionTimeout,
    ExpectMotionState
}
