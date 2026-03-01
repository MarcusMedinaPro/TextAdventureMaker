# Flat Viewing

_Slice tag: Slice 27 — Questions + reveal loop._

## Story beats (max ~10 steps)
1) Arrive at the lobby.
2) Meet the agent.
3) Inspect the flat.
4) Ask questions and decide.

## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐     ┌────────────┐     ┌────────────┐
│   Lobby    │─────│    Unit    │─────│  Balcony   │
│     B      │  In │  I, Agent  │  E  │            │
└────────────┘     └────────────┘     └────────────┘

B = Brochure
I = Inspection list
Agent = NPC
```

## Example (agent questions)
```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

// Slice 27 — Dynamic Descriptions + Dialog Templates
// Tests:
// - DynamicDescription conditions + first visit
// - Variable substitution in descriptions
// - Dialog rule template with parameters

Location lobby = (id: "flat_lobby", description: "A tidy lobby with a directory and fresh paint.");
Location unit = (id: "flat_unit", description: "A bright flat with tall windows and clean floors.");
Location balcony = (id: "balcony", description: "A small balcony overlooking the street.");

lobby.AddExit(Direction.In, unit);
unit.AddExit(Direction.East, balcony);

lobby.AddItem(new Item("brochure", "brochure", "A brochure listing amenities and fees."));
unit.AddItem(new Item("inspection", "inspection list", "A checklist of items to review."));

var agent = new Npc("agent", "agent")
    .Description("An estate agent waits with a small stack of keys.")
    .SetDialog(new DialogNode("Any questions about the flat?")
        .AddOption("Ask about noise levels")
        .AddOption("Ask about utilities")
        .AddOption("Ask about move-in dates"));

unit.AddNpc(agent);

var state = new GameState(lobby, worldLocations: new[] { lobby, unit, balcony });
var parser = new KeywordParser(KeywordParserConfig.Default);

var unitDescription = new DynamicDescription()
    .Default("A bright flat with tall windows and clean floors. The agent looks {agent_mood}.")
    .FirstVisit("The flat smells of fresh paint. The agent looks {agent_mood}.")
    .When(s => s.WorldState.GetFlag("brochure_taken"), "With the brochure in hand, the flat feels {agent_mood}.");

unit.SetDynamicDescription(unitDescription);
UpdateAgentMood(state);

agent.AddDialogRule("agent_mood")
    .Say(ctx =>
    {
        var mood = GetAgentMood(ctx.State);
        return $"\"I hope the viewing feels {mood},\" the agent says.";
    })
    .Priority(1);

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
        if (command is TalkCommand)
        {
            var relationship = g.State.WorldState.GetRelationship("agent") + 1;
            g.State.WorldState.SetRelationship("agent", relationship);
            UpdateAgentMood(g.State);
        }
    })
    .Build();

state.Events.Subscribe(GameEventType.PickupItem, e =>
{
    if (e.Item?.Id == "brochure")
    {
        state.WorldState.SetFlag("brochure_taken", true);
        UpdateAgentMood(state);
    }
});

SetupC64("Flat Viewing - Text Adventure Sandbox");
game.Run();

void UpdateAgentMood(GameState current)
{
    unitDescription.WithVariable("agent_mood", GetAgentMood(current));
}

string GetAgentMood(GameState current)
{
    int relationship = current.WorldState.GetRelationship("agent");
    if (relationship >= 2)
        return "encouraging";
    if (current.WorldState.GetFlag("brochure_taken"))
        return "attentive";
    return "professional";
}
```
