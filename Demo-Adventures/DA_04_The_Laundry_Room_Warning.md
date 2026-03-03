# The Laundry Room Warning

_Slice tag: Slice 50 — Linear staircase horror with delayed trigger, door gating, and heavy red-herring interaction density._

## Story beats (max ~10 steps)
1) You are a daughter in your room, studying homework.
2) The bedroom door stays effectively closed while dinner is supposedly being prepared.
3) You can inspect, read, open, and close many irrelevant objects (red herrings).
4) After enough study time, a voice downstairs calls you to dinner.
5) You head out to the landing and approach the stairs.
6) Before you get downstairs, hands drag you into the laundry room.
7) It is your real mother, shaking and tearful.
8) She says: “Don’t go down there, honey. I heard it too.”

## Map (rough layout)

```
┌────────────────────┐
│      Bedroom       │
│  Desk, Books, Box  │
└─────────┬──────────┘
          │
┌─────────┴──────────┐
│      Landing       │
└──────┬───────┬─────┘
       │       │
   Downstairs  Laundry
      Hall      Room
```

## Example (the laundry room warning)
```csharp
using System;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

// === THE LAUNDRY ROOM WARNING ===
// A linear home-horror scenario with deliberate false leads.

// TEST: Player gets a short study phase before the call event can happen.
var stage = SceneStage.Studying;
var studyTurns = 0;

// --- Locations ---
Location bedroom = (id: "bedroom", description: "Your bedroom smells of paper and fabric softener drifting up from below.");
Location landing = (id: "landing", description: "A narrow landing above the staircase, lined with old family frames.");
Location downstairsHall = (id: "downstairs_hall", description: "The downstairs hall is dim, with warm light leaking from the kitchen doorway.");
Location laundryRoom = (id: "laundry_room", description: "A cramped laundry room with baskets, detergent, and a rattling machine.");

var bedroomDoor = new Door("bedroom_door", "Bedroom Door", DoorState.Closed)
    .Description("A painted wooden door with a brass handle.")
    .SetReaction(DoorAction.Open, "The hinges complain softly.")
    .SetReaction(DoorAction.Close, "The latch clicks shut.");

bedroom.AddExit(Direction.West, landing, bedroomDoor);
landing.AddExit(Direction.Down, downstairsHall);
landing.AddExit(Direction.South, laundryRoom);
downstairsHall.AddExit(Direction.Up, landing);
laundryRoom.AddExit(Direction.North, landing);

// --- Red-herring items ---
var homework = new Item("homework", "Homework", "Maths exercises half-finished in neat pencil lines.")
    .SetTakeable(false)
    .SetReadable()
    .SetReadText("Quadratic equations, margin notes, and one doodle of a cat wearing a crown.");

var textbook = new Item("textbook", "Textbook", "A battered history textbook, open to Tudor trade routes.")
    .SetTakeable(false)
    .SetReadable()
    .SetReadText("Henry VIII taxed wool exports heavily. None of this helps your algebra.");

var diary = new Item("diary", "Old Diary", "A floral notebook with a tiny rusted lock that no longer works.")
    .SetTakeable(false)
    .SetReadable()
    .SetReadText("'Remember to buy detergent. Also, do not forget Tuesday's parent evening.' Nothing sinister. Just chores.");

var drawer = new Item("drawer", "Desk Drawer", "A wooden drawer that sticks unless pulled firmly.")
    .SetTakeable(false)
    .SetReaction(ItemAction.Move, "Pens, a ruler, and stale peppermint wrappers. No secrets.");

var jewelleryBox = new Item("jewellery_box", "Jewellery Box", "A little tin jewellery box painted with faded stars.")
    .SetTakeable(false)
    .SetReadable()
    .SetReadText("A friendship bracelet, two loose buttons, and a ring from a toy machine.");

var photoFrame = new Item("photo_frame", "Photo Frame", "A family photograph from a seaside holiday.")
    .SetTakeable(false)
    .SetReadable()
    .SetReadText("You are missing both front teeth. Mum is laughing with windblown hair. Dad burned the sausages that day.");

var laundryBasket = new Item("basket", "Laundry Basket", "A basket full of folded towels and one missing sock.")
    .SetTakeable(false);

var detergent = new Item("detergent", "Detergent", "Lavender detergent with a cap that never closes properly.")
    .SetTakeable(false)
    .SetReadable()
    .SetReadText("Ultra clean. Spring fresh. Not a clue in sight.");

bedroom.AddItem(homework);
bedroom.AddItem(textbook);
bedroom.AddItem(diary);
bedroom.AddItem(drawer);
bedroom.AddItem(jewelleryBox);
landing.AddItem(photoFrame);
laundryRoom.AddItem(laundryBasket);
laundryRoom.AddItem(detergent);

// --- NPC ---
var mother = new Npc("mother", "Mum")
    .Description("Your mother is pale, teary-eyed, and breathing too fast.")
    .SetDialog(new DialogNode("Don't go down there, honey. I heard it too."));

var state = new GameState(bedroom, worldLocations: [bedroom, landing, downstairsHall, laundryRoom]);
var parser = new KeywordParser(KeywordParserConfigBuilder.BritishDefaults().Build());

SetupC64("The Laundry Room Warning");
WriteLineC64("=== THE LAUNDRY ROOM WARNING ===");
WriteLineC64();
WriteLineC64("You are upstairs, finishing homework before dinner.");
WriteLineC64("Everything in the house sounds normal. Probably.");
WriteLineC64();
WriteLineC64("Goal: Keep studying until called, then head downstairs.");
WriteLineC64();
WriteLineC64("Try: study, read homework, look, examine items, open/close things.");
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

        continue;
    }

    var command = parser.Parse(trimmed);

    // TEST: Bedroom door stays effectively closed before the call event.
    if (stage == SceneStage.Studying && state.IsCurrentRoomId("bedroom"))
    {
        if (command is OpenCommand)
        {
            WriteLineC64("Nah... mum's making dinner, don't want to disturb her.");
            continue;
        }

        if (command is GoCommand go && go.Direction == Direction.West)
        {
            WriteLineC64("Nah... mum's making dinner, don't want to disturb her.");
            continue;
        }
    }

    // TEST: Going downstairs after the call triggers the reveal sequence.
    if (stage == SceneStage.CalledToDinner
        && state.IsCurrentRoomId("landing")
        && command is GoCommand goDown
        && goDown.Direction == Direction.Down)
    {
        TriggerLaundryReveal();
        break;
    }

    var result = state.Execute(command);
    state.DisplayResult(command, result);

    if (stage == SceneStage.Studying
        && command is ReadCommand read
        && result.Success
        && IsStudyMaterial(read.Target))
    {
        RegisterStudyTurn();
    }

    if (result.ShouldQuit)
        break;
}

bool HandleCustomCommand(string inputText, out bool shouldQuit)
{
    shouldQuit = false;

    if (IsStudyCommand(inputText))
    {
        if (!state.IsCurrentRoomId("bedroom"))
        {
            WriteLineC64("You cannot focus on homework from here.");
            return true;
        }

        if (stage != SceneStage.Studying)
        {
            WriteLineC64("Studying feels impossible now.");
            return true;
        }

        RegisterStudyTurn();
        return true;
    }

    // Red-herring open/close interactions that lead nowhere.
    if (IsOpenDrawer(inputText))
    {
        WriteLineC64("You tug the drawer open. Stationery, old receipts, and nothing useful.");
        return true;
    }

    if (IsCloseDrawer(inputText))
    {
        WriteLineC64("You close the drawer. It sticks at the end, then clicks.");
        return true;
    }

    if (IsOpenBox(inputText))
    {
        WriteLineC64("You open the jewellery box. Trinkets and childhood clutter. No answers.");
        return true;
    }

    if (IsCloseBox(inputText))
    {
        WriteLineC64("You close the little tin box and push it back into place.");
        return true;
    }

    return false;
}

void RegisterStudyTurn()
{
    studyTurns++;

    var line = studyTurns switch
    {
        1 => "You solve another page of algebra and rewrite a messy equation.",
        2 => "You underline key formulas and check your working twice.",
        _ => "You keep studying, trying to ignore every creak in the house."
    };

    WriteLineC64(line);

    if (studyTurns < 2 || stage != SceneStage.Studying)
        return;

    stage = SceneStage.CalledToDinner;

    WriteLineC64();
    WriteLineC64("From downstairs, your mother's voice calls up through the stairwell:");
    WriteLineC64("\"Dinner is ready, sweetheart!\"");
    WriteLineC64("The house goes quiet immediately after.");
}

void TriggerLaundryReveal()
{
    _ = state.Move(Direction.South);
    stage = SceneStage.Revealed;
    laundryRoom.AddNpc(mother);

    WriteLineC64("You step toward the stairs.");
    WriteLineC64("Suddenly, hands seize your arms and yank you sideways into the laundry room.");
    WriteLineC64();
    state.ShowRoom();
    WriteLineC64();
    WriteLineC64("Your mother is trembling, eyes red and wet.");
    WriteLineC64("\"Don't go down there, honey. I heard it too.\"");
    WriteLineC64();
    WriteLineC64("=== GAME OVER ===");
}

static bool IsStudyMaterial(string itemName) =>
    itemName.TextCompare("homework")
    || itemName.TextCompare("textbook")
    || itemName.TextCompare("diary");

static bool IsStudyCommand(string inputText) =>
    MatchesAny(inputText, "study", "do homework", "continue homework", "revise", "do maths", "do math");

static bool IsOpenDrawer(string inputText) =>
    MatchesAny(inputText, "open drawer", "open desk drawer", "pull drawer");

static bool IsCloseDrawer(string inputText) =>
    MatchesAny(inputText, "close drawer", "shut drawer");

static bool IsOpenBox(string inputText) =>
    MatchesAny(inputText, "open box", "open jewellery box", "open jewelry box");

static bool IsCloseBox(string inputText) =>
    MatchesAny(inputText, "close box", "close jewellery box", "close jewelry box", "shut box");

static bool MatchesAny(string inputText, params string[] values)
{
    foreach (var value in values)
        if (inputText.TextCompare(value))
            return true;

    return false;
}

enum SceneStage
{
    Studying,
    CalledToDinner,
    Revealed
}
```
