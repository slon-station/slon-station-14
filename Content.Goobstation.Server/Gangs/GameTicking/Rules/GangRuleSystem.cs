using Content.Goobstation.Server.Gangs.Roles;
using Content.Goobstation.Shared.Gangs;
using Content.Server.Antag;
using Content.Server.GameTicking.Rules;
using Content.Server.Mind;
using Content.Server.Roles;
using Robust.Shared.Player;

namespace Content.Goobstation.Server.Gangs.GameTicking.Rules;


public sealed class GangRuleSystem : GameRuleSystem<GangRuleComponent>
{
    [Dependency] private readonly AntagSelectionSystem _antag = default!;
    [Dependency] private readonly MindSystem _mind = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GangRuleComponent, AfterAntagEntitySelectedEvent>(OnSelectAntag);
        SubscribeLocalEvent<GangRuleComponent, GetBriefingEvent>(OnGetBrief);
    }

    private void OnSelectAntag(EntityUid uid, GangRuleComponent comp, AfterAntagEntitySelectedEvent args)
    {
        MakeGangLeader(args.EntityUid, comp);
    }

    public bool MakeGangLeader(EntityUid uid, GangRuleComponent component)
    {
        // creating the gang with the id
        var gangId = uid;

        var leaderComp = EnsureComp<GangLeaderComponent>(uid);
        leaderComp.GangId = gangId;

        var memberComp = EnsureComp<GangMemberComponent>(uid);
        memberComp.GangId = gangId;
        leaderComp.Members.Add(uid);

        var briefing = Loc.GetString("gang-leader-antag-greeter");
        _antag.SendBriefing(uid, briefing, Color.Yellow, component.BriefingSound);

        return true;
    }

    private void OnGetBrief(Entity<GangRuleComponent> comp, ref GetBriefingEvent args)
    {
        if (args.Mind.Comp.OwnedEntity is { } entity)
            args.Append(MakeBriefing(entity));
    }

    private string MakeBriefing(EntityUid entity)
    {
        return Loc.GetString("gang-leader-antag-greeter");
    }
}
