# The Last Night of the Council
_Slice tag: Slice 36 — Ensemble Journey (Creepypasta style, British English)._


    ## Premise
    Seven people sit around a table. Each knows one of them is no longer human, but no one knows who. The meeting must continue anyway.

    ## Arc structure
    - Multiple leads → Each carries a different truth.
- Shifting perspectives → The council fractures.
- Collective ordeal → The inhuman must be named.
- Shared resolution → The group survives or falls together.

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

var council = new Location("council", "A council hall with a long table and too many chairs.");
var archive = new Location("archive", "An archive of votes and forgotten promises.");
var roof = new Location("roof", "A roof where the city speaks in wind.");

council.AddExit(Direction.East, archive);
archive.AddExit(Direction.Up, roof);

var seal = new Door("seal", "bronze seal", "A seal that clicks like a verdict.");
var sealKey = new Key("seal_key", "seal key", "A key with a councillor's crest.");
archive.AddItem(sealKey);
roof.AddExit(Direction.Down, archive, seal);

var speaker = new Npc("speaker", "speaker")
    .Description("The speaker does not look up from the ledger.")
    .SetDialog(new DialogNode("We will not vote without consent.")
        .AddOption("Ask for consent")
        .AddOption("Ask about the seal"));

var runner = new Npc("runner", "runner")
    .Description("A runner who never sits still.")
    .SetDialog(new DialogNode("Messages are heavy tonight.")
        .AddOption("Ask about the archive")
        .AddOption("Ask about the roof"));
runner.SetMovement(new RandomNpcMovement(new[] { council, archive, roof }));

var archivist = new Npc("archivist", "archivist")
    .Description("An archivist with ink-stained hands.")
    .SetDialog(new DialogNode("You will need a key, and a reason.")
        .AddOption("Offer your reason")
        .AddOption("Ask for the key"));

council.AddNpc(speaker);
council.AddNpc(runner);
archive.AddNpc(archivist);

var state = new GameState(council, worldLocations: new[] { council, archive, roof });

state.Events.Subscribe(GameEventType.TalkToNpc, e =>
{
    if (e.Npc?.Id == "speaker")
    {
        state.WorldState.SetRelationship("speaker", state.WorldState.GetRelationship("speaker") + 5);
    }
    if (e.Npc?.Id == "archivist")
    {
        if (!state.WorldState.GetFlag("seal_key_given"))
        {
            state.Inventory.Add(sealKey);
            state.WorldState.SetFlag("seal_key_given", true);
        }
    }
});

var quest = new Quest("ensemble", "Reach a Shared Decision", "Gain consent and open the seal.")
    .AddCondition(new RelationshipCondition("speaker", 5))
    .AddCondition(new WorldFlagCondition("seal_key_given", true))
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
        if (quest.CheckProgress(g.State) && g.State.IsCurrentRoomId("roof"))
        {
            g.Output.WriteLine("The council stands together for one night, and the city listens.");
            g.RequestStop();
        }
    })
    .Build();

game.Run();
```
