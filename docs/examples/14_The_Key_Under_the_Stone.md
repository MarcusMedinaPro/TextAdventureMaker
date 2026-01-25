# The Key Under the Stone

_Slice tag: Slice 14 — Events + Fluent setup. Demo focuses on a tiny trigger/reveal loop with a more fluent style._

## Story beats (max ~10 steps)
1) You stand in a small garden.
2) A flat stone lies by a gate.
3) Lift the stone.
4) Find the hidden key.
5) Unlock the gate.

## Example (fluent + events)
```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

Location garden = (id: "garden", description: "A quiet garden with a flat stone near an old gate.");
Location courtyard = (id: "courtyard", description: "A small courtyard where something waits.");

Item stone = (id: "stone", name: "stone", description: "A heavy flat stone.");
Key gateKey = (id: "gate_key", name: "iron key", description: "A small iron key.");
Door gate = (id: "gate", name: "gate", description: "An old iron gate.")
    .RequiresKey(gateKey)
    .SetReaction(DoorAction.Unlock, "The gate creaks open.");

garden.AddItem(stone);

garden.AddExit(Direction.Out, courtyard, gate);

var state = new GameState(garden, worldLocations: new[] { garden, courtyard });

state.Events.Subscribe(GameEventType.PickupItem, e =>
{
    if (e.Item != null && e.Item.Id.TextCompare("stone"))
    {
        if (!garden.Items.Contains(gateKey))
        {
            garden.AddItem(gateKey);
            Console.WriteLine("You lift the stone and find a key beneath it.");
        }
    }
});

var parser = new KeywordParser(KeywordParserConfig.Default);

var game = GameBuilder.Create()
    .UseState(state)
    .UseParser(parser)
    .AddTurnStart(g =>
    {
        var look = g.State.Look();
        g.Output.WriteLine($"\n{look.Message}");
    })
    .Build();

game.Run();
```
