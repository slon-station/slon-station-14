using Robust.Shared.GameStates;

namespace Content.Goobstation.Server._Slon.Goals.StationGoals;

[RegisterComponent]
public sealed partial class RoundStationGoalComponent : Component
{
    [DataField]
    public bool GoalExpected;

    [DataField]
    public bool GoalSelected;

    [DataField]
    public StationGoalType SelectedGoalType = StationGoalType.None;

    [DataField]
    public bool GoalCompleted;
}
