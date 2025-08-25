using Robust.Shared.GameStates;

namespace Content.Goobstation.Server._Slon.Goals;

[RegisterComponent]
public sealed partial class StationGoalTogglerComponent : Component
{
    [DataField]
    public bool Enabled;
}
