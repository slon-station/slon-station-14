using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Goobstation.Server.Gangs.GameTicking.Rules;

[RegisterComponent]
public sealed partial class GangRuleComponent : Component
{
    [DataField]
    public SoundPathSpecifier BriefingSound = new("/Audio/_Goobstation/Ambience/Antag/gang_start.ogg");

    [DataField]
    public EntProtoId GangLeaderMindRole = "GangLeaderMindRole";
}
