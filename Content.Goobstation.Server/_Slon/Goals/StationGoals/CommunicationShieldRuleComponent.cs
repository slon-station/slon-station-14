using Robust.Shared.GameStates;

namespace Content.Goobstation.Server._Slon.Goals.StationGoals
{
    [RegisterComponent]
    public sealed partial class CommunicationShieldRuleComponent : Component
    {
        [DataField]
        public bool Started = false;

        [DataField]
        public bool GoalAssigned = false;

        [DataField]
        public bool Triggered;
    }
}
