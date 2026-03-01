# The Bar Fight

_Slice tag: Slice 28 — Escalation + de-escalation choice._

## Story beats (max ~10 steps)
1) Enter a noisy bar.
2) Notice rising tension.
3) Choose to calm things down or escalate.
4) Leave the bar.

## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐
│    Bar     │
│  St, G     │
│  Bnc, Brw  │
└─────┬──────┘
      │
      │
┌────────────┐
│   Alley    │
└────────────┘

St = Stool
G = Glass
Bnc = Bouncer (NPC)
Brw = Brawler (NPC)
```

## Example (tension choice)
```csharp
using System;
using System.Linq;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

// Slice 28 — Character Arc
// Tests:
// - NPC arc milestones + trait changes
// - Dialog changes based on arc progress

Location bar = (id: "bar", description: "A noisy bar with clinking glasses.");
Location alley = (id: "bar_alley", description: "A narrow alley behind the bar.");

bar.AddExit(Direction.South, alley);

bar.AddItem(new Item("stool", "bar stool", "A sturdy stool bolted to the floor.").SetTakeable(false));
bar.AddItem(new Item("glass", "glass", "An empty glass with a chipped rim."));

var bouncer = new Npc("bouncer", "bouncer")
    .Description("A bouncer watches the room with folded arms.")
    .SetDialog(new DialogNode("Keep it calm in here.")
        .AddOption("Back off")
        .AddOption("Ask for help"));

var brawler = new Npc("brawler", "brawler")
    .SetState(NpcState.Hostile)
    .SetStats(new Stats(10))
    .Description("A restless patron looks for a fight.")
    .SetDialog(new DialogNode("What are you looking at?")
        .AddOption("Apologize and step away")
        .AddOption("Stand your ground"));

bar.AddNpc(bouncer);
bar.AddNpc(brawler);

var arc = bouncer.DefineArc("BouncerArc")
    .StartState(Trait.Fearful)
    .Milestone(1, "stand_up", Trait.Hopeful)
    .Milestone(2, "intervene", Trait.Brave)
    .EndState(Trait.Heroic)
    .OnComplete(_ => WriteLineC64("The bouncer steps forward, steady and sure."));

var state = new GameState(bar, worldLocations: new[] { bar, alley });
var parser = new KeywordParser(KeywordParserConfig.Default);

SetupC64("The Bar Fight - Text Adventure Sandbox");
WriteLineC64("=== THE BAR FIGHT (Slice 28) ===");
WriteLineC64("Goal: help the bouncer move from fearful to brave.");
WriteLineC64("Commands: talk bouncer, encourage, intervene, go south, look, quit.");
ShowRoom();

while (true)
{
    WriteLineC64();
    WritePromptC64("> ");
    var input = Console.ReadLine();
    if (input is null)
        break;

    var trimmed = input.Trim();
    if (string.IsNullOrWhiteSpace(trimmed))
        continue;

    if (trimmed.Is("quit") || trimmed.Is("exit"))
        break;

    if (trimmed.TextCompare("encourage"))
    {
        _ = arc.Advance("stand_up", state);
        WriteLineC64("You urge the bouncer to stand firm.");
        continue;
    }

    if (trimmed.TextCompare("intervene"))
    {
        _ = arc.Advance("intervene", state);
        WriteLineC64("You call for the bouncer to step in.");
        continue;
    }

    var command = parser.Parse(trimmed);
    if (command is TalkCommand { Target: var target } && target?.TextCompare("bouncer") == true)
    {
        WriteLineC64(GetBouncerDialog(arc.CurrentTrait));
        continue;
    }

    var result = state.Execute(command);
    if (!string.IsNullOrWhiteSpace(result.Message))
        WriteLineC64(result.Message);

    foreach (var reaction in result.ReactionsList.Where(r => !string.IsNullOrWhiteSpace(r)))
        WriteLineC64($"> {reaction}");

    if (command is GoCommand && result.Success)
        ShowRoom();
}

void ShowRoom()
{
    WriteLineC64();
    WriteLineC64($"Room: {state.CurrentLocation.Id.ToProperCase()}");
    WriteLineC64(state.CurrentLocation.GetDescription());
    WriteLineC64($"Bouncer trait: {arc.CurrentTrait}");
}

static string GetBouncerDialog(Trait trait)
{
    return trait switch
    {
        Trait.Fearful => "\"I keep my head down,\" the bouncer mutters.",
        Trait.Hopeful => "\"Maybe it's time to step in,\" the bouncer says.",
        Trait.Brave => "\"I'll handle it,\" the bouncer says, steady now.",
        Trait.Heroic => "\"No one else gets hurt on my watch,\" the bouncer declares.",
        _ => "\"Stay calm,\" the bouncer says."
    };
}
```
