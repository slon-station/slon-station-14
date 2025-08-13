using Robust.Shared.GameStates;

namespace Content.Goobstation.Server._Slon.Goals.StationGoals;

[RegisterComponent]
public sealed partial class CommTowerComponent : Component
{
    [DataField]
    public bool Completed;
}
