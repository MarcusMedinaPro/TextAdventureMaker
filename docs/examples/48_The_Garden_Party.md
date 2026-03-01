# The Garden Party

_Slice tag: Slice 48 — Social horror with FactionSystem, suspicion tracking, and branching endings._

## Story beats (max ~10 steps)
1) Arrive at Mrs Ashworth's garden party. Everything is perfect.
2) Explore the garden, marquee, and house. Meet the guests.
3) Notice something wrong: the guests repeat the same phrases.
4) Find the locked shed. Peer through the window.
5) Suspicion rises. The guests notice you aren't eating the cake.
6) Discover the truth: the garden was built over something buried.
7) Find the garden shears and the rope in the greenhouse.
8) Choose: combine shears + rope to escape, or confront Mrs Ashworth.
9) Escape ending: you flee through the hedge. The party continues without you.
10) Confrontation ending: Mrs Ashworth reveals the truth. You are the guest of honour.

## Map (rough layout)

```
          N
    W           E
          S

┌──────────────┐    ┌──────────┐
│  Greenhouse  ├────┤  Garden  │
│   S  R       │    │   C  G   │
└──────────────┘    └────┬─────┘
                         │
┌──────────────┐    ┌────┴─────┐
│   Shed       ├────┤ Marquee  │
│   (locked)   │    │   T  A   │
└──────────────┘    └────┬─────┘
                         │
                    ┌────┴─────┐
                    │  House   │
                    │   H      │
                    └──────────┘

S = Shears
R = Rope
C = Cake (poisoned)
G = Guests (NPCs)
T = Table (fixed)
A = Mrs Ashworth (NPC)
H = Hallway items
```

## Example (the garden party)
```csharp
using System;
using System.Linq;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

// === THE GARDEN PARTY ===
// A creepypasta about a garden party where nothing is as it seems.
// Features: FactionSystem, suspicion counter, DynamicDescription, branching endings

// --- Suspicion tracking ---
int suspicion = 0;

// --- Locations ---
Location garden = (id: "garden", description: "A manicured garden bathed in afternoon sunlight. Rose bushes line the path. Guests mill about with glasses of champagne.");
Location marquee = (id: "marquee", description: "A white marquee with linen tablecloths. A three-tiered cake sits in the centre. Mrs Ashworth presides from a wicker chair.");
Location greenhouse = (id: "greenhouse", description: "A glass greenhouse, humid and overgrown. Tomato vines press against the panes. Garden tools hang from hooks.");
Location shed = (id: "shed", description: "A wooden shed at the far end of the garden. The padlock is new and heavy.");
Location house = (id: "house", description: "The hallway of Mrs Ashworth's house. Family photographs line the walls. The air smells of lavender and something older.");

garden.AddExit(Direction.South, marquee);
garden.AddExit(Direction.West, greenhouse);
garden.AddExit(Direction.East, shed);
marquee.AddExit(Direction.North, garden);
marquee.AddExit(Direction.South, house);
greenhouse.AddExit(Direction.East, garden);
house.AddExit(Direction.North, marquee);

// Shed is locked — no exit from garden initially
// (Player can peer through window via custom command)

// --- Dynamic descriptions by suspicion ---
garden.SetDynamicDescription(new DynamicDescription()
    .FirstVisit("You arrive at the garden gate. The party is in full swing. Bunting flutters from the trees. The guests smile as you enter, every one of them, in perfect unison.")
    .When(s => s.WorldState.GetFlag("suspicion_high"),
        "The garden is quieter now. The guests have stopped talking. They stand among the roses, watching you. Their smiles haven't changed. Their smiles never change.")
    .When(s => s.WorldState.GetFlag("suspicion_medium"),
        "The garden feels different. The roses are the same, but the guests have rearranged themselves. You could swear they were standing in these exact positions when you arrived.")
    .Default("A perfect English garden. The guests chat in small groups. The champagne flows."));

marquee.SetDynamicDescription(new DynamicDescription()
    .FirstVisit("The marquee is pristine. Mrs Ashworth sits in her wicker chair like a queen receiving courtiers. The cake is enormous, three tiers of white icing. Everyone has a slice. Everyone.")
    .When(s => s.WorldState.GetFlag("suspicion_high"),
        "The marquee is silent. Mrs Ashworth sits perfectly still. The cake has been cut to the last slice. It sits on a plate with your name card beside it. 'Do try the cake, dear,' she says. 'Everyone does, eventually.'")
    .When(s => s.WorldState.GetFlag("suspicion_medium"),
        "The marquee. Mrs Ashworth is watching you. 'You haven't tried the cake,' she says pleasantly. The other guests turn to look at you. 'Everyone tries the cake.'")
    .Default("The white marquee. Mrs Ashworth holds court. The cake gleams."));

greenhouse.SetDynamicDescription(new DynamicDescription()
    .FirstVisit("The greenhouse is sweltering. Tomato vines climb the glass walls. Garden shears hang from a hook. A coil of rope sits in the corner. Through the back pane, you can see something odd in the soil — a pattern, like rows.")
    .When(s => s.WorldState.GetFlag("suspicion_high"),
        "The greenhouse. Through the glass, you can see the guests have formed a circle around the garden. They are facing inward. Waiting.")
    .Default("A humid greenhouse. Tools hang on hooks. Vines press against the glass."));

shed.SetDynamicDescription(new DynamicDescription()
    .FirstVisit("The shed is locked with a heavy new padlock. Through the grimy window, you can see shelves lined with jars. The jars contain something dark. Below them, the earth floor has been recently dug.")
    .When(s => s.WorldState.GetFlag("suspicion_high"),
        "The shed. The padlock is scratched, as though something inside has been trying to get out. The jars on the shelves have moved. You are certain they have moved.")
    .Default("A locked shed. Through the window, jars of something dark line the shelves."));

house.SetDynamicDescription(new DynamicDescription()
    .FirstVisit("The hallway is cool after the garden heat. Photographs cover every surface: Mrs Ashworth at garden parties, year after year. The guests are different in each photograph. But Mrs Ashworth wears the same dress. The same smile. Going back decades.")
    .When(s => s.WorldState.GetFlag("suspicion_high"),
        "The hallway. The photographs have changed. In every one, the guest of honour is the same person. The face is yours. The dates span fifty years. That is not possible.")
    .Default("Mrs Ashworth's hallway. Photographs of garden parties past. She hasn't aged in any of them."));

// --- Items ---
var cake = new Item("cake", "Cake", "A slice of white cake on a china plate. The icing is perfect. It smells faintly of almonds.")
    .SetTakeable(true)
    .AsFood(0)
    .SetPoisoned()
    .SetPoisonDamage(10, 3);

var shears = new Item("shears", "Garden Shears", "Heavy iron garden shears. The blades are sharp and recently oiled.")
    .SetTakeable(true)
    .SetWeight(1.5f);

var rope = new Item("rope", "Rope", "A coil of thick hemp rope. About ten metres.")
    .SetTakeable(true)
    .SetWeight(1.0f);

var photographs = new Item("photographs", "Photographs", "Framed photographs of garden parties. Mrs Ashworth appears in every one, wearing the same dress, the same pearls. The dates go back to 1962.")
    .SetTakeable(false)
    .SetReadable()
    .SetReadText("The oldest photograph is dated 1923. Mrs Ashworth stands in the centre, surrounded by guests. She looks exactly the same. On the back, in faded pencil: 'The garden feeds. The garden provides.'");

var table = new Item("table", "Table", "A long table covered in white linen. China plates, silver cutlery. Every place is set. Every plate has cake. One plate has your name card.")
    .SetTakeable(false);

var champagne = new Item("champagne", "Champagne", "A crystal flute of champagne. The bubbles rise endlessly.")
    .SetTakeable(true)
    .AsDrink(0);

garden.AddItem(champagne);
marquee.AddItem(cake);
marquee.AddItem(table);
greenhouse.AddItem(shears);
greenhouse.AddItem(rope);
house.AddItem(photographs);

// --- NPCs ---
var ashworth = new Npc("ashworth", "Mrs Ashworth")
    .Description("An elegant woman in her seventies — or perhaps her forties. It's hard to tell. She wears a cream dress and a string of pearls. Her smile is warm and absolute.")
    .SetDialog(new DialogNode("So glad you could come, dear. Do try the cake. It's my grandmother's recipe. And her grandmother's before that.")
        .AddOption("What's in the shed?",
            new DialogNode("Oh, that old thing? Just garden supplies. Nothing to trouble yourself with. Now, about that cake..."))
        .AddOption("The photographs in the hall...",
            new DialogNode("Family tradition, dear. We've held this party every year since the house was built. The garden demands it. Now do try a slice.")));

var guest1 = new Npc("guest1", "Gerald")
    .Description("A man in a linen suit. He holds a champagne glass and smiles. He has been holding that glass since you arrived. The level hasn't changed.")
    .SetDialog(new DialogNode("Lovely party, isn't it? Mrs Ashworth always throws a wonderful party. Have you tried the cake?"));

var guest2 = new Npc("guest2", "Daphne")
    .Description("A woman in a floral dress. She stands near the roses, absolutely still. When you approach, she animates suddenly, as if switched on.")
    .SetDialog(new DialogNode("The roses are beautiful this year, aren't they? Mrs Ashworth says it's the soil. Something special about the soil."));

marquee.AddNpc(ashworth);
garden.AddNpc(guest1);
garden.AddNpc(guest2);

// --- Faction system: the party's opinion of you ---
var factionSystem = new FactionSystem();
var partyFaction = factionSystem.AddFaction("the_party")
    .WithNpcs("ashworth", "guest1", "guest2");
state.SetFactionSystem(factionSystem);

var parser = new KeywordParser(KeywordParserConfigBuilder.BritishDefaults().Build());

SetupC64("The Garden Party");
WriteLineC64("=== THE GARDEN PARTY ===");
WriteLineC64();
WriteLineC64("You received an invitation to Mrs Ashworth's garden party.");
WriteLineC64("Cream card, copperplate script, hand-delivered.");
WriteLineC64("'You are cordially invited. We do so hope you'll come.'");
WriteLineC64("You don't remember RSVPing. But here you are.");
WriteLineC64();
WriteLineC64("Goal: Enjoy the party. Or survive it.");
WriteLineC64();

state.ShowRoom();

while (true)
{
    // Tick poisons (from the cake)
    if (state is GameState gs)
    {
        var poisonResults = gs.TickPoisons();
        foreach (var (sourceName, damage) in poisonResults)
        {
            WriteLineC64(MarcusMedina.TextAdventure.Localization.Language.PoisonTick(sourceName, damage));
        }

        if (state.Stats.Health <= 0)
        {
            WriteLineC64();
            WriteLineC64("The garden spins. You fall to your knees on the perfect lawn.");
            WriteLineC64("Mrs Ashworth stands over you. 'Don't worry, dear,' she says.");
            WriteLineC64("'Everyone stays for the garden party. Everyone stays.'");
            WriteLineC64("The guests gather round. They are still smiling.");
            WriteLineC64("The garden feeds. The garden provides.");
            WriteLineC64();
            WriteLineC64("=== THE END (You stayed) ===");
            break;
        }
    }

    WriteLineC64();
    WritePromptC64("> ");
    var input = Console.ReadLine();
    if (input is null)
        break;

    var trimmed = input.Trim();
    if (string.IsNullOrWhiteSpace(trimmed))
        continue;

    // Custom: peer through shed window
    if (trimmed.TextCompare("look window") || trimmed.TextCompare("peer window")
        || trimmed.TextCompare("look through window") || trimmed.TextCompare("examine window"))
    {
        if (state.IsCurrentRoomId("shed"))
        {
            suspicion += 2;
            state.Factions.ModifyReputation("the_party", -10, state);
            WriteLineC64("You press your face to the grimy glass. The jars contain something that was once red and is now dark brown. Below the shelves, the earth has been dug in neat rectangles. Person-sized rectangles.");
            WriteLineC64("Behind you, Gerald says: 'Find anything interesting?' He is standing too close. You didn't hear him approach.");
            UpdateSuspicion();
            continue;
        }

        WriteLineC64("There is no window to look through here.");
        continue;
    }

    // Custom: combine shears + rope = escape tool
    if (trimmed.TextCompare("combine shears rope") || trimmed.TextCompare("combine rope shears")
        || trimmed.TextCompare("tie rope") || trimmed.TextCompare("cut hedge"))
    {
        bool hasShears = state.Inventory.FindItem("shears") != null;
        bool hasRope = state.Inventory.FindItem("rope") != null;

        if (!hasShears || !hasRope)
        {
            WriteLineC64("You need both the garden shears and the rope to attempt this.");
            continue;
        }

        WriteLineC64("You hack at the hedge with the shears, cutting a narrow gap. You tie the rope to the fence post beyond and haul yourself through.");
        WriteLineC64();
        WriteLineC64("You tumble onto Ashworth Lane, scratched and gasping.");
        WriteLineC64("Behind you, through the hedge, you can still hear the party.");
        WriteLineC64("Glasses clinking. Laughter. Mrs Ashworth's voice: 'Don't worry, dears. They always come back. The garden remembers.'");
        WriteLineC64();
        WriteLineC64("You run. You do not look back.");
        WriteLineC64("You tell yourself you will never go back.");
        WriteLineC64();
        WriteLineC64("On your doormat the next morning: a cream envelope, copperplate script.");
        WriteLineC64("'Thank you for attending. We do so hope you'll come again.'");
        WriteLineC64();
        WriteLineC64("=== THE END (You escaped) ===");
        break;
    }

    // Custom: confront Ashworth
    if (trimmed.TextCompare("confront ashworth") || trimmed.TextCompare("accuse ashworth"))
    {
        if (!state.IsCurrentRoomId("marquee"))
        {
            WriteLineC64("Mrs Ashworth is in the marquee.");
            continue;
        }

        if (suspicion < 4)
        {
            WriteLineC64("Confront her about what? Everything seems perfectly pleasant. Doesn't it?");
            continue;
        }

        WriteLineC64("'What's buried in the garden, Mrs Ashworth?'");
        WriteLineC64();
        WriteLineC64("The party stops. Every guest freezes mid-gesture. Mrs Ashworth sets down her teacup.");
        WriteLineC64();
        WriteLineC64("'Guests, dear. Guests are buried in the garden. Every one of them came to a party just like this one. Every one of them tried the cake. Every one of them stayed.'");
        WriteLineC64();
        WriteLineC64("She picks up the plate with your name on it.");
        WriteLineC64("'You are the guest of honour. You were always the guest of honour.'");
        WriteLineC64();
        WriteLineC64("The guests close in. Their smiles are fixed. Their hands are cold.");
        WriteLineC64("The last thing you see is the garden, green and perfect in the afternoon sun.");
        WriteLineC64("The garden feeds. The garden provides.");
        WriteLineC64();
        WriteLineC64("=== THE END (You stayed) ===");
        break;
    }

    var command = parser.Parse(trimmed);
    var result = state.Execute(command);

    // Track suspicion-raising actions
    if (command is ExamineCommand && result.Success)
    {
        suspicion++;
        UpdateSuspicion();
    }

    if (command is ReadCommand && result.Success)
    {
        suspicion += 2;
        state.Factions.ModifyReputation("the_party", -10, state);
        UpdateSuspicion();
    }

    // Eating the cake triggers poison
    if (command is EatCommand eat && eat.ItemName.TextCompare("cake"))
    {
        if (result.Success)
        {
            WriteLineC64("Mrs Ashworth smiles. 'There's a good dear.'");
            WriteLineC64("The cake tastes of almonds and earth and something you cannot name.");
        }
    }

    state.DisplayResult(command, result);

    if (result.ShouldQuit)
        break;
}

void UpdateSuspicion()
{
    if (suspicion >= 6 && !state.WorldState.GetFlag("suspicion_high"))
    {
        state.WorldState.SetFlag("suspicion_high", true);
        WriteLineC64();
        WriteLineC64("The atmosphere has shifted. The guests have stopped pretending to talk. They watch you openly now, heads tilted at identical angles.");

        ashworth.SetDialog(new DialogNode("You've been exploring, haven't you, dear? Curious ones always explore. But the party isn't over. The party is never over. Do sit down. Have some cake.")
            .AddOption("I'm leaving.",
                new DialogNode("Oh, I don't think so, dear. The gate is locked. It's been locked since you arrived. But you knew that, didn't you?")));
    }
    else if (suspicion >= 3 && !state.WorldState.GetFlag("suspicion_medium"))
    {
        state.WorldState.SetFlag("suspicion_medium", true);
        WriteLineC64();
        WriteLineC64("You feel eyes on you. The guests are still smiling, but their conversations have quieted. Mrs Ashworth is watching you from the marquee.");

        ashworth.SetDialog(new DialogNode("You seem restless, dear. Why not sit and enjoy the party? The cake really is quite special. My grandmother's recipe. The secret ingredient is in the soil.")
            .AddOption("What do you mean, the soil?",
                new DialogNode("The garden soil, dear. It's very rich. Very... nourishing. Everything grows so well here. Everything stays."))
            .AddOption("I'd like to leave.",
                new DialogNode("Leave? But you only just arrived. Besides, the gate locks automatically. For security, you understand. Now — cake?")));
    }
}
```
