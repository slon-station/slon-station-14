using Robust.Shared.Audio;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Goobstation.Server.Gangs.GameTicking.Rules;

[RegisterComponent]
public sealed partial class GangRuleComponent : Component
{
    [DataField]
    public SoundPathSpecifier BriefingSound = new("/Audio/_Goobstation/Ambience/Antag/gang_start.ogg");

    [DataField]
    public SoundPathSpecifier MemberBriefingSound = new("/Audio/_Goobstation/Ambience/Antag/gang_start.ogg");

    [DataField]
    public EntProtoId GangLeaderMindRole = "GangLeaderMindRole";

    [DataField]
    public LocId GangMemberGreeting = "gang-member-antag-greeter";

    [DataField]
    public float DropInterval = 48f; // 8 mikn todo 480

    [DataField]
    public float WarningTime = 18f; // 2 min todo 120

    [DataField]
    public string ChannelId = "GangRadio";

    public float Accumulator;
    public bool Announced;
    public EntityCoordinates? DropLocation;
}
