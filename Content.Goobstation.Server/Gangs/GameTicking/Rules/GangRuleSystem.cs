using System.Linq;
using Content.Goobstation.Server.Gangs.Roles;
using Content.Goobstation.Shared.Gangs;
using Content.Server.Antag;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules;
using Content.Server.Mind;
using Content.Server.Roles;
using Content.Shared.GameTicking.Components;
using Robust.Shared.Player;

namespace Content.Goobstation.Server.Gangs.GameTicking.Rules;


public sealed class GangRuleSystem : GameRuleSystem<GangRuleComponent>
{
    [Dependency] private readonly AntagSelectionSystem _antag = default!;
    [Dependency] private readonly MindSystem _mind = default!;
    [Dependency] private readonly RoleSystem _role = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<GangRuleComponent, AfterAntagEntitySelectedEvent>(OnSelectAntag);
        SubscribeLocalEvent<GangMemberComponent, GetBriefingEvent>(OnGetMemberBrief);
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

    private void OnGetMemberBrief(Entity<GangMemberComponent> comp, ref GetBriefingEvent args)
    {
        if (args.Mind.Comp.OwnedEntity is { } entity)
            args.Append(MakeMemberBriefing(entity));
    }

    private string MakeMemberBriefing(EntityUid entity)
    {
        return Loc.GetString("gang-member-antag-greeter");
    }

    protected override void AppendRoundEndText(EntityUid uid,
        GangRuleComponent component,
        GameRuleComponent gameRule,
        ref RoundEndTextAppendEvent args)
    {
        base.AppendRoundEndText(uid, component, gameRule, ref args);

        var gangs = new Dictionary<EntityUid, List<string>>();
        var gangLeaders = new Dictionary<EntityUid, string>();
        var gangSignCounts = new Dictionary<EntityUid, int>();
        var graffitiQuery = EntityQueryEnumerator<GangGraffitiComponent>();

        // counts the graffities by gangs to display in the manifest
        while (graffitiQuery.MoveNext(out _, out var graffiti))
        {
            var gangId = graffiti.GangId;
            gangSignCounts.TryGetValue(gangId, out var count);
            gangSignCounts[gangId] = count + 1;
        }

        var memberQuery = EntityQueryEnumerator<GangMemberComponent, MetaDataComponent>();
        while (memberQuery.MoveNext(out var entity, out var member, out var meta))
        {
            var gangId = member.GangId;
            var name = meta.EntityName;

            if (!gangs.ContainsKey(gangId))
                gangs[gangId] = new List<string>();

            gangs[gangId].Add(name);

            if (HasComp<GangLeaderComponent>(entity))
                gangLeaders[gangId] = name;
        }

        foreach (var (gangId, members) in gangs)
        {
            gangSignCounts.TryGetValue(gangId, out var signCount);
            var signText = Loc.GetString("gang-signs-count", ("count", signCount));

            if (gangLeaders.TryGetValue(gangId, out var leaderName))
                args.AddLine(Loc.GetString("gang-gang-led-by", ("leader", leaderName), ("signs", signText)));

            else
                args.AddLine(Loc.GetString("gang-gang-no-leader", ("signs", signText)));

            args.AddLine(Loc.GetString("gang-members-header"));
            foreach (var member in members)
            {
                args.AddLine($"- {member}");
            }
            args.AddLine(""); // peak
        }
    }
}
