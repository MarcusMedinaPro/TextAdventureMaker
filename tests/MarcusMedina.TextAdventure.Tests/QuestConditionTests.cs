using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class QuestConditionTests
{
    [Fact]
    public void HasItemCondition_IsMetWhenItemInInventory()
    {
        var location = new Location("camp");
        var state = new GameState(location);
        state.Inventory.Add(new Item("sword", "sword"));
        var condition = new HasItemCondition("sword");

        var evaluator = new QuestConditionEvaluator(state);

        Assert.True(condition.Accept(evaluator));
    }

    [Fact]
    public void NpcStateCondition_IsMetWhenNpcMatchesState()
    {
        var npc = new Npc("dragon", "Dragon").SetState(NpcState.Dead);
        var condition = new NpcStateCondition(npc, NpcState.Dead);

        var evaluator = new QuestConditionEvaluator(new GameState(new Location("camp")));

        Assert.True(condition.Accept(evaluator));
    }

    [Fact]
    public void Quest_CheckProgress_CompletesWhenConditionsMet()
    {
        var location = new Location("camp");
        var state = new GameState(location);
        state.Inventory.Add(new Item("sword", "sword"));
        var npc = new Npc("dragon", "Dragon").SetState(NpcState.Dead);

        var quest = new Quest("dragon_hunt", "Dragon Hunt", "Find the sword and slay the dragon.")
            .AddCondition(new HasItemCondition("sword"))
            .AddCondition(new NpcStateCondition(npc, NpcState.Dead))
            .Start();

        var completed = quest.CheckProgress(state);

        Assert.True(completed);
        Assert.Equal(QuestState.Completed, quest.State);
    }
}
