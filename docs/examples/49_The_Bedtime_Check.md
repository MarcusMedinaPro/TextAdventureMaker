# The Bedtime Check

_Slice tag: Slice 49 — Linear domestic horror with state-gated progression, object state toggles, and false-path side interactions._

## Story beats (max ~10 steps)
1) Father and son sit together in the living room before bedtime.
2) The player can watch telly or speak with the son.
3) At 21:30, the evening turns toward bedtime.
4) The father tells the son it is time for bed.
5) If the lamp is off, the son refuses to go upstairs.
6) The son brushes his teeth and eventually gets into bed.
7) The stairs are blocked until the son is in bed.
8) Upstairs, something is wrong: footsteps move in the room.
9) The bed skirt hangs too low to see underneath.
10) Pull up the quilt and reveal the twist.

## Map (rough layout)

```
            UP
┌────────────────────┐
│   Child Bedroom    │
│   Bed, Quilt       │
└─────────┬──────────┘
          │
┌─────────┴──────────┐
│      Landing       │
└─────────┬──────────┘
          │ DOWN
┌─────────┴──────────┐
│    Living Room     │
│ Lamp, TV, Table    │
└────────────────────┘
```

## Example (the bedtime check)
```csharp
using System;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

// === THE BEDTIME CHECK ===
// A linear home-horror story.
// Core focus: gated progression, custom stateful interactions, and a single inevitable ending.

// --- Story stage ---
// TEST: 21:30 should trigger only after the player chooses to watch telly or talk to the son.
var bedtimeStage = BedtimeStage.OnSofa;
var openingChoiceMade = false;
var clockStruck2130 = false;
var lampOn = true;
var tvOn = true;
var sonUpsetByTv = false;
var footstepsHeard = false;
var pauseBedtimeProgressThisTurn = false;

// --- Locations ---
Location livingRoom = (id: "living_room", description: "A modest living room with a worn sofa and a low coffee table. Evening settles against the windows.");
Location landing = (id: "landing", description: "A narrow upstairs landing with thin carpet and family photos along the wall.");
Location childBedroom = (id: "child_bedroom", description: "A small bedroom lit by warm light. The quiet here feels too careful.");

livingRoom.AddExit(Direction.Up, landing);
landing.AddExit(Direction.East, childBedroom);

// --- Items ---
var lamp = new Item("lamp", "Lamp", "A brass floor lamp. It is currently on.")
    .SetTakeable(false)
    .AddAliases("light");

var television = new Item("tv", "Television", "An old telly playing a late film at low volume.")
    .SetTakeable(false)
    .AddAliases("television", "telly");

var table = new Item("table", "Coffee Table", "A scratched coffee table with biscuit crumbs and a glass of juice.")
    .SetTakeable(false);

var biscuit = new Item("biscuit", "Biscuit", "A round chocolate biscuit on a small plate.")
    .SetTakeable(true)
    .AsFood(1)
    .SetReaction(ItemAction.Use, "Buttery, chocolatey, and slightly stale.");

var juice = new Item("juice", "Apple Juice", "A glass of cloudy apple juice.")
    .SetTakeable(true)
    .AddAliases("glass", "squash")
    .AsDrink(1);

var bed = new Item("bed", "Children's Bed", "A comfy children's bed with a long quilt hanging down to the floor. You cannot see under the bed.")
    .SetTakeable(false)
    .AddAliases("childrens bed", "child's bed");

var quilt = new Item("quilt", "Quilt", "A thick quilt with rockets and stars stitched into the fabric.")
    .SetTakeable(false)
    .AddAliases("blanket", "duvet", "cover");

livingRoom.AddItem(lamp);
livingRoom.AddItem(television);
livingRoom.AddItem(table);
livingRoom.AddItem(biscuit);
livingRoom.AddItem(juice);
childBedroom.AddItem(bed);
childBedroom.AddItem(quilt);

// --- NPC ---
var son = new Npc("son", "Son")
    .Description("Your son sits curled up on the sofa in dinosaur pyjamas, eyes fixed on the telly.");

livingRoom.AddNpc(son);

// --- Game setup ---
var state = new GameState(livingRoom, worldLocations: [livingRoom, landing, childBedroom]);
var parser = new KeywordParser(KeywordParserConfigBuilder.BritishDefaults().Build());

SetupC64("The Bedtime Check");
WriteLineC64("=== THE BEDTIME CHECK ===");
WriteLineC64();
WriteLineC64("You are winding down for the night with your son.");
WriteLineC64("He always asks for the same bedtime ritual, and tonight is no different.");
WriteLineC64();
WriteLineC64("Goal: Put your son to bed and check under the bed when the moment comes.");
WriteLineC64();
WriteLineC64("For now, choose your next moment: watch the telly, or talk to your son.");
WriteLineC64();

state.ShowRoom();

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

    if (HandleCustomCommand(trimmed, out var shouldQuit))
    {
        if (shouldQuit)
            break;

        AdvanceStoryTurn();
        continue;
    }

    var command = parser.Parse(trimmed);

    // TEST: Stairs are blocked before the child is in bed.
    if (command is GoCommand go
        && go.Direction == Direction.Up
        && state.IsCurrentRoomId("living_room")
        && bedtimeStage != BedtimeStage.InBed)
    {
        WriteLineC64("No need to go upstairs.");
        AdvanceStoryTurn();
        continue;
    }

    // TEST: Son dialogue is stateful and only works when he is present in the current room.
    if (command is TalkCommand talk
        && talk.Target is not null
        && talk.Target.TextCompare("son")
        && state.CurrentLocation.FindNpc("son") is not null)
    {
        HandleTalkToSon();
        AdvanceStoryTurn();
        continue;
    }

    var result = state.Execute(command);
    state.DisplayResult(command, result);

    // TEST: Optional side flavour interactions.
    if (command is DrinkCommand drink && IsAppleJuiceName(drink.ItemName) && result.Success)
    {
        WriteLineC64("It tastes like Grandma's apple juice.");
    }

    if (command is EatCommand eat && IsBiscuitName(eat.ItemName) && result.Success)
    {
        WriteLineC64("You dust crumbs from your fingers and glance back at the sofa.");
    }

    AdvanceStoryTurn();

    if (result.ShouldQuit)
        break;
}

bool HandleCustomCommand(string inputText, out bool shouldQuit)
{
    shouldQuit = false;

    if (IsWatchTv(inputText))
    {
        if (!state.IsCurrentRoomId("living_room"))
        {
            WriteLineC64("You can only watch the telly from the living room.");
            return true;
        }

        if (!tvOn)
        {
            WriteLineC64("The telly is off.");
            return true;
        }

        openingChoiceMade = true;
        WriteLineC64("You sit with him and watch in comfortable silence for a moment.");
        return true;
    }

    if (IsTurnOffLamp(inputText))
    {
        if (!state.IsCurrentRoomId("living_room"))
        {
            WriteLineC64("The lamp is downstairs in the living room.");
            return true;
        }

        if (!lampOn)
        {
            WriteLineC64("The lamp is already off.");
            return true;
        }

        lampOn = false;
        lamp.SetDescription("A brass floor lamp. It is currently off.");
        WriteLineC64("You switch off the lamp. Shadows gather in the corners.");
        return true;
    }

    if (IsTurnOnLamp(inputText))
    {
        if (!state.IsCurrentRoomId("living_room"))
        {
            WriteLineC64("The lamp is downstairs in the living room.");
            return true;
        }

        if (lampOn)
        {
            WriteLineC64("The lamp is already on.");
            return true;
        }

        lampOn = true;
        lamp.SetDescription("A brass floor lamp. It is currently on.");
        WriteLineC64("You switch the lamp back on. Warm light fills the room again.");
        return true;
    }

    // TEST: Turning off TV before 21:30 makes the son upset until TV is back on or the clock strikes 21:30.
    if (IsTurnOffTv(inputText))
    {
        if (!state.IsCurrentRoomId("living_room"))
        {
            WriteLineC64("The telly is downstairs in the living room.");
            return true;
        }

        if (!tvOn)
        {
            WriteLineC64("The telly is already off.");
            return true;
        }

        tvOn = false;
        television.SetDescription("An old telly with a dark, reflective screen.");
        WriteLineC64("You switch the telly off.");

        if (!clockStruck2130)
        {
            sonUpsetByTv = true;
            WriteLineC64("Your son frowns. \"Hey... I was watching that.\"");
        }

        return true;
    }

    if (IsTurnOnTv(inputText))
    {
        if (!state.IsCurrentRoomId("living_room"))
        {
            WriteLineC64("The telly is downstairs in the living room.");
            return true;
        }

        if (tvOn)
        {
            WriteLineC64("The telly is already on.");
            return true;
        }

        tvOn = true;
        sonUpsetByTv = false;
        television.SetDescription("An old telly playing a late film at low volume.");
        WriteLineC64("You switch the telly back on.");
        WriteLineC64("He relaxes into the cushions again.");
        return true;
    }

    if (IsLookUnderBed(inputText))
    {
        if (!state.IsCurrentRoomId("child_bedroom"))
        {
            WriteLineC64("There is no bed here.");
            return true;
        }

        WriteLineC64("The quilt hangs too low. You cannot see under the bed.");
        return true;
    }

    // TEST: Final horror trigger can only occur in the correct late-story state.
    if (IsPullUpQuilt(inputText))
    {
        if (!state.IsCurrentRoomId("child_bedroom"))
        {
            WriteLineC64("There is nothing here to pull up.");
            return true;
        }

        if (bedtimeStage != BedtimeStage.InBed)
        {
            WriteLineC64("Not yet.");
            return true;
        }

        if (!footstepsHeard)
        {
            WriteLineC64("You hesitate. Something still feels unfinished.");
            return true;
        }

        WriteLineC64("You grab the quilt and pull it up in one quick motion.");
        WriteLineC64();
        WriteLineC64("Under the bed, your real son is curled tight, pale and shaking.");
        WriteLineC64("\"Daddy, there's someone on my bed.\"");
        WriteLineC64();
        WriteLineC64("AAAAAAAAAGGGGGGGGGGHHHHHHHHHHHHHH");
        WriteLineC64();
        WriteLineC64("=== GAME OVER ===");
        shouldQuit = true;
        return true;
    }

    return false;
}

void HandleTalkToSon()
{
    openingChoiceMade = true;

    if (bedtimeStage == BedtimeStage.InBed && state.IsCurrentRoomId("child_bedroom"))
    {
        WriteLineC64("\"Daddy, I am in bed.\"");

        if (footstepsHeard)
            return;

        footstepsHeard = true;
        WriteLineC64("You hear soft footsteps moving around the room.");
        WriteLineC64("\"I hear you walking around!\" you call out.");
        return;
    }

    if (!clockStruck2130)
    {
        if (sonUpsetByTv)
            WriteLineC64("\"Dad... please put the telly back on.\"");
        else
            WriteLineC64("\"Shh, I'm watching the film.\"");

        return;
    }

    if (bedtimeStage != BedtimeStage.OnSofa)
    {
        WriteLineC64("You hear him upstairs.");
        return;
    }

    if (!lampOn)
    {
        WriteLineC64("\"It's too dark, Dad. I'm not going upstairs like this.\"");
        return;
    }

    WriteLineC64("You nod toward the hallway. \"Time for bed.\"");
    WriteLineC64("He groans and slides off the sofa.");
    WriteLineC64("\"Fine...\" he mutters, then shuffles towards the bathroom.");

    livingRoom.RemoveNpc(son);
    bedtimeStage = BedtimeStage.GoingToBathroom;
    pauseBedtimeProgressThisTurn = true;
}

void AdvanceStoryTurn()
{
    if (!clockStruck2130 && openingChoiceMade)
    {
        clockStruck2130 = true;
        sonUpsetByTv = false;

        WriteLineC64();
        WriteLineC64("The mantel clock strikes 21:30.");
        WriteLineC64("The film suddenly feels much too loud for the hour.");
        return;
    }

    if (!clockStruck2130)
    {
        WriteLineC64("The evening hangs in a pause. You can watch the telly or talk to your son.");
        return;
    }

    if (pauseBedtimeProgressThisTurn)
    {
        pauseBedtimeProgressThisTurn = false;
        return;
    }

    if (bedtimeStage == BedtimeStage.GoingToBathroom)
    {
        bedtimeStage = BedtimeStage.BrushingTeeth;
        WriteLineC64("From upstairs, you hear tap water and patient tooth-brushing.");
        return;
    }

    if (bedtimeStage != BedtimeStage.BrushingTeeth)
        return;

    bedtimeStage = BedtimeStage.InBed;
    livingRoom.RemoveNpc(son);
    childBedroom.AddNpc(son);

    WriteLineC64("Floorboards creak above you.");
    WriteLineC64("\"Daddy, I am in bed.\"");
}

static bool IsWatchTv(string inputText) =>
    MatchesAny(inputText, "watch tv", "watch telly", "watch television", "watch movie", "watch film");

static bool IsTurnOffLamp(string inputText) =>
    MatchesAny(inputText, "turn off lamp", "switch off lamp", "turn lamp off", "switch lamp off", "turn off light", "switch off light");

static bool IsTurnOnLamp(string inputText) =>
    MatchesAny(inputText, "turn on lamp", "switch on lamp", "turn lamp on", "switch lamp on", "turn on light", "switch on light");

static bool IsTurnOffTv(string inputText) =>
    MatchesAny(inputText, "turn off tv", "switch off tv", "turn tv off", "switch tv off", "turn off television", "switch off television", "turn off telly", "switch off telly");

static bool IsTurnOnTv(string inputText) =>
    MatchesAny(inputText, "turn on tv", "switch on tv", "turn tv on", "switch tv on", "turn on television", "switch on television", "turn on telly", "switch on telly");

static bool IsLookUnderBed(string inputText) =>
    MatchesAny(inputText, "look under bed", "check under bed", "look beneath bed", "peek under bed");

static bool IsPullUpQuilt(string inputText) =>
    MatchesAny(inputText, "pull up quilt", "pull up blanket", "pull quilt", "pull blanket", "lift quilt", "lift blanket", "lift duvet");

static bool MatchesAny(string inputText, params string[] values)
{
    foreach (var value in values)
        if (inputText.TextCompare(value))
            return true;

    return false;
}

static bool IsAppleJuiceName(string itemName) =>
    itemName.TextCompare("juice")
    || itemName.TextCompare("apple juice")
    || itemName.TextCompare("squash")
    || itemName.TextCompare("glass");

static bool IsBiscuitName(string itemName) =>
    itemName.TextCompare("biscuit")
    || itemName.TextCompare("cookie");

enum BedtimeStage
{
    OnSofa,
    GoingToBathroom,
    BrushingTeeth,
    InBed
}
```
