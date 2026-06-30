namespace EquipmentTwin.Core.Motion;

/// <summary>
/// 실제 서보 드라이브 없이 Home, Move, InPosition, Timeout, Servo Alarm 흐름을 검증하는 가상 모션 축이다.
/// </summary>
public sealed class MotionAxis
{
    private readonly IClock _clock;
    private readonly List<MotionCommandResult> _history = new();
    private MotionOperation? _operation;

    public MotionAxis(string name, IClock clock, double initialPosition = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Axis name is required.", nameof(name));
        }

        Name = name;
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        Position = initialPosition;
    }

    public string Name { get; }

    public MotionAxisState State { get; private set; } = MotionAxisState.Disabled;

    public bool ServoEnabled { get; private set; }

    public bool IsHomed { get; private set; }

    public double Position { get; private set; }

    public double? TargetPosition { get; private set; }

    public MotionAxisAlarm? LastAlarm { get; private set; }

    public IReadOnlyList<MotionCommandResult> History => _history;

    public MotionCommandResult ServoOn()
    {
        if (State == MotionAxisState.Alarmed)
        {
            return Reject("Cannot enable servo while axis is alarmed.", LastAlarm);
        }

        ServoEnabled = true;
        State = IsHomed ? MotionAxisState.InPosition : MotionAxisState.Ready;

        return Accept("Servo enabled.");
    }

    public MotionCommandResult ServoOff()
    {
        ServoEnabled = false;
        _operation = null;
        TargetPosition = null;

        if (State != MotionAxisState.Alarmed)
        {
            State = MotionAxisState.Disabled;
        }

        return Accept("Servo disabled.");
    }

    public MotionCommandResult StartHome(TimeSpan duration)
    {
        ValidateDuration(duration, nameof(duration));

        if (!ServoEnabled)
        {
            return Alarm(MotionAxisAlarmCode.ServoOff, "Servo must be enabled before homing.");
        }

        if (State == MotionAxisState.Alarmed)
        {
            return Reject("Cannot start homing while axis is alarmed.", LastAlarm);
        }

        if (_operation is not null)
        {
            return Reject($"Axis is already {State}.");
        }

        TargetPosition = 0;
        State = MotionAxisState.Homing;
        _operation = new MotionOperation(MotionAxisState.Homing, _clock.UtcNow, duration, TargetPosition.Value);

        return Accept("Homing started.");
    }

    public MotionCommandResult StartMove(double targetPosition, TimeSpan duration)
    {
        ValidateDuration(duration, nameof(duration));

        if (!ServoEnabled)
        {
            return Alarm(MotionAxisAlarmCode.ServoOff, "Servo must be enabled before moving.");
        }

        if (!IsHomed)
        {
            return Alarm(MotionAxisAlarmCode.NotHomed, "Axis must be homed before moving.");
        }

        if (State == MotionAxisState.Alarmed)
        {
            return Reject("Cannot start move while axis is alarmed.", LastAlarm);
        }

        if (_operation is not null)
        {
            return Reject($"Axis is already {State}.");
        }

        TargetPosition = targetPosition;
        State = MotionAxisState.Moving;
        _operation = new MotionOperation(MotionAxisState.Moving, _clock.UtcNow, duration, targetPosition);

        return Accept($"Move started to {targetPosition}.");
    }

    public MotionCommandResult Poll()
    {
        if (_operation is null)
        {
            return Accept("No active motion command.");
        }

        var elapsed = _clock.UtcNow - _operation.StartedAtUtc;
        if (elapsed < _operation.Duration)
        {
            return Accept($"Motion in progress for {elapsed}.");
        }

        Position = _operation.TargetPosition;
        TargetPosition = null;

        if (_operation.Kind == MotionAxisState.Homing)
        {
            IsHomed = true;
            State = MotionAxisState.InPosition;
            _operation = null;
            return Accept("Homing completed.");
        }

        State = MotionAxisState.InPosition;
        _operation = null;
        return Accept($"Move completed at {Position}.");
    }

    public MotionCommandResult CheckTimeout(TimeSpan timeout)
    {
        ValidateDuration(timeout, nameof(timeout));

        if (_operation is null)
        {
            return Accept("No active motion command to check timeout.");
        }

        var elapsed = _clock.UtcNow - _operation.StartedAtUtc;
        if (elapsed < timeout)
        {
            return Accept($"Motion timeout not reached. Elapsed: {elapsed}.");
        }

        return Alarm(MotionAxisAlarmCode.MoveTimeout, $"Axis '{Name}' motion timed out after {elapsed}.");
    }

    public MotionCommandResult TriggerServoAlarm(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Servo alarm message is required.", nameof(message));
        }

        return Alarm(MotionAxisAlarmCode.ServoAlarm, message);
    }

    public MotionCommandResult ClearAlarm()
    {
        if (State != MotionAxisState.Alarmed)
        {
            return Accept("No active motion alarm.");
        }

        LastAlarm = null;
        _operation = null;
        TargetPosition = null;
        State = ServoEnabled
            ? IsHomed ? MotionAxisState.InPosition : MotionAxisState.Ready
            : MotionAxisState.Disabled;

        return Accept("Motion alarm cleared.");
    }

    private MotionCommandResult Alarm(MotionAxisAlarmCode code, string message)
    {
        var alarm = new MotionAxisAlarm(code, message);
        LastAlarm = alarm;
        State = MotionAxisState.Alarmed;
        _operation = null;
        TargetPosition = null;

        return Accept(message, alarm);
    }

    private MotionCommandResult Accept(string message, MotionAxisAlarm? alarm = null)
    {
        var result = new MotionCommandResult(Name, State, true, message, alarm);
        _history.Add(result);
        return result;
    }

    private MotionCommandResult Reject(string message, MotionAxisAlarm? alarm = null)
    {
        var result = new MotionCommandResult(Name, State, false, message, alarm);
        _history.Add(result);
        return result;
    }

    private static void ValidateDuration(TimeSpan duration, string parameterName)
    {
        if (duration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Motion duration must be greater than zero.");
        }
    }

    private sealed record MotionOperation(
        MotionAxisState Kind,
        DateTimeOffset StartedAtUtc,
        TimeSpan Duration,
        double TargetPosition);
}
