using Content.Shared.Interaction;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Content.Server.Chat.Systems;
using Robust.Shared.Timing;
using Content.Server.GameTicking;
using Robust.Shared.Random;
using System.Linq;
using Robust.Shared.Utility;
using Content.Goobstation.Server._Slon.Goals.StationGoals;
using Content.Shared.Chat;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Content.Goobstation.Server._Slon.Goals;

public sealed class StationGoalSystem : EntitySystem
{
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly GameTicker _gameTicker = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    private readonly TimeSpan _goalSelectionDelay = TimeSpan.FromMinutes(1); // todo 5
    public StationGoalType CurrentGoal { get; private set; } = StationGoalType.None;
    public EntityUid? GoalEntity { get; private set; }

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<StationGoalTogglerComponent, ActivateInWorldEvent>(OnActivate);
        SubscribeLocalEvent<RoundStationGoalComponent, ComponentInit>(OnGoalInit);
    }

    private void OnGoalInit(EntityUid uid, RoundStationGoalComponent component, ComponentInit args)
    {
        GoalEntity = uid;
    }

    private void OnActivate(EntityUid uid, StationGoalTogglerComponent comp, ActivateInWorldEvent args)
    {
        var roundDuration = _gameTiming.CurTime - _gameTicker.RoundStartTimeSpan;
        var canActivateGoal = roundDuration.TotalMinutes <= 1; // todo 5

        if (!canActivateGoal)
        {
            _audio.PlayPvs(new SoundPathSpecifier("/Audio/Effects/Diseases/beepboop.ogg"), uid);
            args.Handled = true;
            return;
        }

        comp.Enabled = !comp.Enabled;
        RaiseLocalEvent(new ToggleStationGoalEvent(comp.Enabled));

        string message;
        if (comp.Enabled)
        {
            if (TryComp<RoundStationGoalComponent>(GoalEntity, out var goalComp))
            {
                goalComp.GoalExpected = true;
            }

            var timeLeft = _goalSelectionDelay - roundDuration;
            if (timeLeft < TimeSpan.Zero)
                timeLeft = TimeSpan.Zero;

            var formattedTime = timeLeft.ToString(@"mm\:ss");
            message = Loc.GetString("centcom-radio-station-goal-request-sent", ("time", formattedTime));
        }
        else
        {
            if (GoalEntity != null &&
                TryComp<RoundStationGoalComponent>(GoalEntity.Value, out var goalComp))
            {
                goalComp.GoalExpected = false;
            }

            message = Loc.GetString("centcom-radio-station-goal-request-cancel");
        }

        _audio.PlayPvs(new SoundPathSpecifier("/Audio/Effects/Diseases/beepboop.ogg"), uid);
        _chat.TrySendInGameICMessage(uid, message, InGameICChatType.Speak, false);
        args.Handled = true;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (GoalEntity == null)
            return;

        var roundTime = _gameTiming.CurTime - _gameTicker.RoundStartTimeSpan;
        if (roundTime < _goalSelectionDelay)
            return;

        if (!TryComp<RoundStationGoalComponent>(GoalEntity.Value, out var comp)
            || !comp.GoalExpected
            || comp.GoalSelected)
            return;

        CurrentGoal = PickRandomGoal();
        comp.SelectedGoalType = CurrentGoal;
        comp.GoalSelected = true;

        RaiseLocalEvent(new StationGoalSelectedEvent(CurrentGoal));

        if (CurrentGoal == StationGoalType.CommunicationShield)
        {// todo make more generic for fuutre goals
            _chat.DispatchGlobalAnnouncement(
                Loc.GetString("commtower-goal-start"),
                playSound: true,
                colorOverride: Color.Cyan
            );
        }
    }

    private StationGoalType PickRandomGoal()
    {
        var values = Enum.GetValues<StationGoalType>().Where(g => g != StationGoalType.None).ToArray();
        return _random.Pick(values);
    }

    public EntityUid? GetGoalEntity() => GoalEntity;
}

public enum StationGoalType
{
    None = 0,
    CommunicationShield
}
