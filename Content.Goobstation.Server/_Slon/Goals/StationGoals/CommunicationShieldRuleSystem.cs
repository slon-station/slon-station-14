using Content.Server.GameTicking;
using Robust.Shared.Timing;
using Content.Goobstation.Server._Slon.Goals;
using Content.Server.Chat.Systems;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Goobstation.Server._Slon.Goals.StationGoals;

public sealed class CommunicationShieldRuleSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly GameTicker _ticker = default!;
    [Dependency] private readonly StationGoalSystem _goalSystem = default!;
    [Dependency] private readonly ChatSystem _chat = default!;

    // todo put these timespan shit into the components god this is awful
    private readonly TimeSpan _sleeperAgentsTriggerTime = TimeSpan.FromMinutes(50);
    private readonly TimeSpan _warningTime = TimeSpan.FromMinutes(45);
    private bool _sleeperAgentsTriggered;
    private bool _warningSent;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<StationGoalSelectedEvent>(OnGoalSelected);
        SubscribeLocalEvent<CommTowerComponent, ComponentInit>(OnTowerBuilt);
    }

    private void OnGoalSelected(StationGoalSelectedEvent ev)
    {
        if (ev.GoalType != StationGoalType.CommunicationShield)
            return;

        _sleeperAgentsTriggered = false;
        _warningSent = false;
    }

    private void OnTowerBuilt(EntityUid uid, CommTowerComponent component, ComponentInit args)
    {
        if (_goalSystem.CurrentGoal != StationGoalType.CommunicationShield)
            return;

        var goalEntity = _goalSystem.GetGoalEntity();
        if (goalEntity != null &&
            TryComp<RoundStationGoalComponent>(goalEntity, out var goal) &&
            !goal.GoalCompleted)
        {
            goal.GoalCompleted = true;

            _chat.DispatchGlobalAnnouncement(
                Loc.GetString("commtower-goal-ok"),
                playSound: true,
                colorOverride: Color.Green
            );
        }
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (_ticker.RunLevel != GameRunLevel.InRound
            || _goalSystem.CurrentGoal != StationGoalType.CommunicationShield
            || _sleeperAgentsTriggered)
            return;

        var roundTime = _timing.CurTime - _ticker.RoundStartTimeSpan;
        var towerBuilt = CheckTowerBuilt();

        // warn the crew 5 minutes before the TriggerSleeperAgents
        if (!_warningSent && roundTime >= _warningTime && !towerBuilt)
        {
            _chat.DispatchGlobalAnnouncement(
                Loc.GetString("commtower-goal-warn"),
                playSound: true,
                colorOverride: Color.Red
            );
            _warningSent = true;
        }

        if (roundTime < _sleeperAgentsTriggerTime)
            return;

        var goalEntity = _goalSystem.GetGoalEntity();
        if (goalEntity != null &&
            TryComp<RoundStationGoalComponent>(goalEntity, out var goal))
        {
            if (!towerBuilt && !goal.GoalCompleted)
                TriggerSleeperAgents();

            else if (!goal.GoalCompleted)
            {
                goal.GoalCompleted = true;
                _chat.DispatchGlobalAnnouncement(
                    Loc.GetString("commtower-goal-ok"),
                    playSound: true,
                    colorOverride: Color.Green
                );
            }
        }

        _sleeperAgentsTriggered = true;
    }
    private bool CheckTowerBuilt()
    {
        var towerQuery = EntityQueryEnumerator<CommTowerComponent>();
        while (towerQuery.MoveNext(out var uid, out var tower))
        {
            if (tower.Completed)
                return true;
        }
        return false;
    }

    private void TriggerSleeperAgents()
    {
        _ticker.AddGameRule("SleeperAgentsGoalEnd");
    }
}
