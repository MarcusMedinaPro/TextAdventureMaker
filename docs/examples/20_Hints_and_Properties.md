# Hints & Properties

_Slice tag: Slice 20 — Extra Properties on Entities. Demo focuses on attaching dynamic metadata like `hint` to items, keys, doors, and NPCs._

## Story beats (max ~10 steps)
1) You see a locked door and a key.
2) The door hints that it needs a key.
3) After unlocking, the hint changes.
4) A coffee cup's hint changes when used.

## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐     ┌────────────┐
│    Room    │─────│   Hall     │
│  K, C      │  Out│            │
└────────────┘     └────────────┘

K = Key
C = Coffee cup
```

## Example (properties + dynamic hints)
```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

Location room = (id: "room", description: "A quiet room with a locked door.");
Location hall = (id: "hall", description: "A calm hallway beyond.");

Key key = (id: "room_key", name: "small key", description: "A tiny key.")
    .SetHint("Hmm, what do we usually use keys for...duh");

Door door = (id: "oak_door", name: "oak door", description: "A sturdy oak door.")
    .RequiresKey(key)
    .SetHint("It needs a key, duh");

Item coffee = (id: "coffee", name: "coffee cup", description: "A cup waiting for coffee.")
    .SetHint("This cup would look nicer with some coffee in it");

room.AddItem(key);
room.AddItem(coffee);
room.AddExit(Direction.Out, hall, door);

coffee.OnUse += _ => coffee.SetHint("Yum! Coffee good!");

var state = new GameState(room, worldLocations: new[] { room, hall });
EventChain chain = new();
int turns = 0;

_ = chain
    .Step(s => s.IsCurrentRoomId("hall"), _ => Console.WriteLine("You feel a chill as you step into the hall."))
    .Step(s => s.Inventory.FindItem("coffee") != null, _ => Console.WriteLine("The coffee steadies your hands."))
    .Step(_ => turns >= 3, _ => Console.WriteLine("Time passes. The quiet grows heavier."));

state.Events.Subscribe(GameEventType.UnlockDoor, e =>
{
    if (e.Door != null && e.Door.Id.TextCompare("oak_door"))
    {
        e.Door.SetHint("It's a nice unlocked door.");
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
    .AddTurnEnd((g, command, result) =>
    {
        turns += 1;
        _ = chain.Check(g.State);

        if (command is LookCommand)
        {
            g.Output.WriteLine($"Hint (door): {door.GetHint()}");
            g.Output.WriteLine($"Hint (key): {key.GetHint()}");
            g.Output.WriteLine($"Hint (coffee): {coffee.GetHint()}");
        }
    })
    .Build();

SetupC64("Hints & Properties - Text Adventure Sandbox");
game.Run();
```
