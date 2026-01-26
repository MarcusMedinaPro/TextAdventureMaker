# The Call Beyond the Gate
_Slice tag: Slice 36 — Hero Journey (Creepypasta style, British English)._


    ## Premise
    You hear your name called from behind the locked iron gate. The voice sounds like your own, only older and tired. When you open it, there is no one—only a narrow path leading away from everything you know.

    ## Arc structure
    - Ordinary World → The gate remains shut.
- Call → The older voice calls your name.
- Threshold → You step onto the path.
- Trials → The path rewrites your memories.
- Ordeal → You meet your future self.
- Return → You bring back a warning.

    ## Story beats (max ~8 steps)
1) The disturbance arrives and feels personal.
2) A rule is broken or a boundary is crossed.
3) A clue reveals the scale of the problem.
4) A choice narrows the world.
5) The environment answers back.
6) A truth is forced into view.
7) A price is paid, willingly or not.
8) The ending leaves a lingering echo.

    ## Example (detailed setup)

```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

var cottage = new Location("cottage", "A small cottage with a door that shivers in its hinges.");
var lane = new Location("lane", "A lane washed in fog. A gate waits in the hedges.");
var threshold = new Location("threshold", "The threshold hums like a held breath.");
var deepWood = new Location("deep_wood", "Trees tighten like a throat around you.");
var glade = new Location("glade", "A still glade where the air tastes of iron.");
var returnRoad = new Location("return_road", "The road back, quieter than before.");

cottage.AddExit(Direction.North, lane);
lane.AddExit(Direction.North, threshold);
threshold.AddExit(Direction.North, deepWood);
deepWood.AddExit(Direction.North, glade);
glade.AddExit(Direction.South, returnRoad, oneWay: true);

var ironKey = new Key("iron_key", "iron key", "A key that rings like a bell in your palm.")
    .SetHint("It feels meant for the gate.")
    .SetProperty("symbol", "gate");
var shard = new Item("shard", "glass shard", "A shard that reflects a face you do not know.")
    .SetHint("Perhaps it belongs with something softer.");
var ribbon = new Item("ribbon", "red ribbon", "A ribbon torn from a sleeve.");
var letter = new Item("letter", "sealed letter", "A letter addressed to you, in your own hand.")
    .SetReadText("Do not step through the gate until you are ready to return.")
    .RequireTakeToRead()
    .SetReadingCost(1);

cottage.AddItem(letter);
lane.AddItem(ironKey);
deepWood.AddItem(shard);
threshold.AddItem(ribbon);

var gate = new Door("gate", "iron gate", "An iron gate with a lock that dislikes you.")
    .RequiresKey(ironKey)
    .SetReaction(DoorAction.Open, "The gate groans as if it remembers you.");

lane.AddExit(Direction.East, threshold, gate);

var mentor = new Npc("mentor", "mentor")
    .Description("A tired figure with a lantern that never quite brightens the lane.")
    .SetDialog(new DialogNode("You will go anyway, so take this warning.")
        .AddOption("Accept the warning")
        .AddOption("Ask for the key"));
mentor.SetMovement(new PatrolNpcMovement(new[] { cottage, lane }));

cottage.AddNpc(mentor);

var state = new GameState(cottage, worldLocations: new[] { cottage, lane, threshold, deepWood, glade, returnRoad });
state.RecipeBook.Add(new ItemCombinationRecipe("shard", "ribbon", () =>
    new Item("ward", "ward", "A ward that hums when you hold it.")
        .SetHint("It keeps the call at bay for a moment.")));

state.Events.Subscribe(GameEventType.TalkToNpc, e =>
{
    if (e.Npc?.Id == "mentor")
    {
        state.WorldState.SetRelationship("mentor", state.WorldState.GetRelationship("mentor") + 10);
        state.WorldState.SetFlag("heard_call", true);
        state.WorldState.AddTimeline("The mentor named the gate.");
    }
});

state.Events.Subscribe(GameEventType.PickupItem, e =>
{
    if (e.Item?.Id == "iron_key")
    {
        state.WorldState.SetFlag("has_key", true);
        state.WorldState.AddTimeline("The key arrived in your hand.");
    }
});

state.Events.Subscribe(GameEventType.UnlockDoor, e =>
{
    if (e.Door?.Id == "gate")
    {
        state.WorldState.SetFlag("threshold_open", true);
        state.WorldState.AddTimeline("The threshold yielded.");
    }
});

state.Events.Subscribe(GameEventType.EnterLocation, e =>
{
    if (e.Location?.Id == "deep_wood" || e.Location?.Id == "glade")
    {
        state.WorldState.Increment("trials");
    }
});

var quest = new Quest("journey", "Cross the Gate", "Carry the ward into the glade.")
    .AddCondition(new WorldFlagCondition("threshold_open", true))
    .AddCondition(new HasItemCondition("ward"))
    .AddCondition(new WorldCounterCondition("trials", 2))
    .Start();

var parser = new KeywordParser(KeywordParserConfig.Default);

var game = GameBuilder.Create()
    .UseState(state)
    .UseParser(parser)
    .AddTurnStart(g =>
    {
        var look = g.State.Look();
        g.Output.WriteLine($"
{look.Message}");
    })
    .AddTurnEnd((g, command, result) =>
    {
        if (quest.CheckProgress(g.State))
        {
            g.Output.WriteLine("You return with a warning that will not leave your mouth.");
            g.RequestStop();
        }
    })
    .Build();

game.Run();
```
