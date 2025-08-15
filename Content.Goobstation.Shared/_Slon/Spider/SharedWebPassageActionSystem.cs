using Content.Shared.Actions;

namespace Content.Goobstation.Shared._Slon.Spider;

public abstract class SharedWebPassageActionSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _action = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<WebPassageActionComponent, MapInitEvent>(OnInit);
    }

    private void OnInit(EntityUid uid, WebPassageActionComponent component, MapInitEvent args)
    {
        _action.AddAction(uid, ref component.Action, component.WebAction, uid);
    }
}
