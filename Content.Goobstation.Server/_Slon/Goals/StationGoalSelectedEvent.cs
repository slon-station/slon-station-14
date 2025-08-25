namespace Content.Goobstation.Server._Slon.Goals;

public sealed class StationGoalSelectedEvent : EntityEventArgs
{
    public StationGoalType GoalType { get; }

    public StationGoalSelectedEvent(StationGoalType goalType)
    {
        GoalType = goalType;
    }
}
