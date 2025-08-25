namespace Content.Goobstation.Server._Slon.Goals;

public sealed class ToggleStationGoalEvent : EntityEventArgs
{
    public bool Enabled { get; }
    public ToggleStationGoalEvent(bool enabled) => Enabled = enabled;
}
