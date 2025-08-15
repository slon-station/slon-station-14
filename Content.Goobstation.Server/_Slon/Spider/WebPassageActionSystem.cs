using Content.Goobstation.Shared._Slon.Spider;
using Content.Server.DoAfter;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Goobstation.Server._Slon.Spider;

public sealed class WebPassageActionSystem : SharedWebPassageActionSystem
{
    [Dependency] private readonly DoAfterSystem _doAfter = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<WebPassageActionComponent, WebPassageActionEvent>(OnAction);
        SubscribeLocalEvent<WebPassageActionComponent, WebPassageDoAfterEvent>(OnDoAfter);
    }

    private void OnAction(EntityUid uid, WebPassageActionComponent comp, WebPassageActionEvent args)
    {
        var ev = new WebPassageDoAfterEvent();
        var doAfterArgs = new DoAfterArgs(EntityManager, uid, TimeSpan.FromSeconds(comp.DoAfter), ev, uid)
        {
            BreakOnMove = true,
            BreakOnDamage = true
        };

        _doAfter.TryStartDoAfter(doAfterArgs);
    }

    private void OnDoAfter(EntityUid uid, WebPassageActionComponent comp, WebPassageDoAfterEvent ev)
    {
        if (ev.Cancelled
            || ev.Handled)
            return;

        var coords = Transform(uid).Coordinates;
        EntityManager.SpawnEntity(comp.SpawnId, coords);
        ev.Handled = true;
    }

}
