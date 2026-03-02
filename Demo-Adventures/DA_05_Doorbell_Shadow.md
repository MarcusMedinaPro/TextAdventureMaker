# Demo Adventure 05: Doorbell Shadow

Linear psychological thriller with heavy red herrings and replay-locked progression.

## Progression Rules

- **Run 1**: Ends with the doorbell reveal and a scream.
- **Run 2+**: You unlock the chance to warn her before she reaches the house. If you run with her, you die, but she escapes.
- **Run 3+**: After two completed runs, a **wheel spanner** appears on the road. If you carry it into the final scene, you can defend yourself.

## Drop-In Sandbox Program (C#)

```csharp
using System.Text.Json;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

const string ProgressFileName = "doorbell_shadow_progress.json";
string progressPath = Path.Combine(AppContext.BaseDirectory, ProgressFileName);

RunProgress progress = LoadProgress(progressPath);
RunTier tier = progress.CompletedRuns switch
{
    <= 0 => RunTier.First,
    1 => RunTier.Second,
    _ => RunTier.ThirdPlus
};

StoryStage stage = StoryStage.Following;
HashSet<string> beats = new(StringComparer.OrdinalIgnoreCase);

Location platform = new("platform", "A narrow station platform slick with old rain. She is already walking away from you.");
Location concourse = new("concourse", "The station concourse hums with footsteps, ticket beeps, and stale coffee.");
Location pavement = new("pavement", "A long pavement outside the station. Streetlights smear gold on wet concrete.");
Location oakCorner = new("oak_corner", "An old oak leans over the road. Terraced houses stare back with blank windows.");
Location gate = new("gate", "Her front gate clicks in the wind. A stone path leads to the steps.");
Location steps = new("steps", "You stand at her front steps, close enough to ring the bell.");
Location alley = new("alley", "A service alley behind the houses, lined with bins and broken fences.");

platform.AddExit(Direction.East, concourse);
concourse.AddExit(Direction.West, platform);
concourse.AddExit(Direction.East, pavement);
pavement.AddExit(Direction.West, concourse);
pavement.AddExit(Direction.North, oakCorner);
oakCorner.AddExit(Direction.South, pavement);
oakCorner.AddExit(Direction.East, gate);
gate.AddExit(Direction.West, oakCorner);
gate.AddExit(Direction.East, steps);
steps.AddExit(Direction.West, gate);
oakCorner.AddExit(Direction.North, alley);
alley.AddExit(Direction.South, oakCorner);

// Red herrings: takeable clutter
platform.AddItem(new Item("ticket_stub", "Ticket Stub", "Yesterday's crumpled ticket with no useful information."));
platform.AddItem(new Item("paper_cup", "Paper Cup", "A cold paper cup with lipstick on the rim."));
concourse.AddItem(new Item("flyer", "Club Flyer", "A neon flyer for a student night you will never attend.").SetReadable().SetReadText("Two-for-one cocktails, Friday only. Completely irrelevant."));
pavement.AddItem(new Item("pebble", "Pebble", "A smooth pebble from the gutter."));
oakCorner.AddItem(new Item("wrapper", "Sweet Wrapper", "A silver sweet wrapper folded into a tiny square."));

// Red herrings: scenery objects
platform.AddItem(new Item("timetable", "Timetable Board", "Rows of delayed departures flicker in tired yellow pixels.").SetTakeable(false).SetReadable().SetReadText("18:02 delayed. 18:12 delayed. 18:24 cancelled."));
concourse.AddItem(new Item("vending", "Vending Machine", "A humming machine with stuck crisps behind glass.").SetTakeable(false));
pavement.AddItem(new Item("shop_window", "Shop Window", "A dark window reflecting your shape and someone else's, maybe.").SetTakeable(false));
oakCorner.AddItem(new Item("oak", "Oak Tree", "A thick old oak with low branches scratching the streetlight glow.").SetTakeable(false));
gate.AddItem(new Item("mailbox", "Mailbox", "A brass letterbox set into a peeling blue door.").SetTakeable(false));
gate.AddItem(new Item("house_window", "House Window", "Curtains half-drawn. Someone could be watching from inside.").SetTakeable(false));
steps.AddItem(new Item("doormat", "Doormat", "WELCOME, faded by years of rain and shoes.").SetTakeable(false).SetReadable().SetReadText("The corners are frayed. Mud marks are fresh."));
steps.AddItem(new Item("gnome", "Garden Gnome", "A chipped garden gnome with one missing eye.").SetTakeable(false));
alley.AddItem(new Item("bins", "Wheelie Bins", "Black bins lined up like silent witnesses.").SetTakeable(false));

if (tier == RunTier.ThirdPlus)
{
    pavement.AddItem(new Item("spanner", "Wheel Spanner", "A heavy wheel spanner near roadworks cones. It could break bone.").AddAliases("wrench", "iron", "weapon"));
}

GameState state = new(platform, worldLocations: [platform, concourse, pavement, oakCorner, gate, steps, alley]);
KeywordParser parser = new(KeywordParserConfigBuilder.BritishDefaults().Build());

SetupC64("Doorbell Shadow");
WriteLineC64("=== DOORBELL SHADOW ===");
WriteLineC64();
WriteLineC64("She was the prettiest girl in class. You never told her.");
WriteLineC64("Tonight, you follow her from the train with your pulse in your throat.");
WriteLineC64();
WriteLineC64(tier switch
{
    RunTier.First => "First run: no shortcuts. Just follow her and ring the bell.",
    RunTier.Second => "Second run unlocked: you may try to warn her before she reaches the door.",
    _ => "Third run unlocked: after two full runs, you may find something heavy enough to fight back."
});
WriteLineC64();
WriteLineC64("Commands: look, go <direction>, take <item>, read <item>, inventory, examine <item>");
WriteLineC64("Special: ring bell, warn her, run, defend");

state.ShowRoom();

while (stage != StoryStage.Ended)
{
    WriteLineC64();
    WritePromptC64("> ");
    string? input = Console.ReadLine();
    if (input is null)
        break;

    string commandText = input.Trim();
    if (string.IsNullOrWhiteSpace(commandText))
        continue;

    if (HandleSpecial(commandText))
        continue;

    ICommand command = parser.Parse(commandText);
    CommandResult result = state.Execute(command);
    state.DisplayResult(command, result);

    if (result.ShouldQuit)
        break;

    if (command is GoCommand && result.Success)
        OnMoveBeat();
}

return;

bool HandleSpecial(string text)
{
    if (HandleRedHerringOpens(text))
        return true;

    if (text.TextCompare("ring bell") || text.TextCompare("ring doorbell") || text.TextCompare("bell"))
    {
        if (!state.IsCurrentRoomId("steps"))
        {
            WriteLineC64("You press air with your finger. The bell is not here.");
            return true;
        }

        TriggerDoorbellScene();
        return true;
    }

    if (text.TextCompare("warn her") || text.TextCompare("warn") || text.TextCompare("shout"))
    {
        if (tier == RunTier.First)
        {
            WriteLineC64("The chance has not opened yet. You keep following instead.");
            return true;
        }

        if (!state.IsCurrentRoomId("oak_corner") && !state.IsCurrentRoomId("gate"))
        {
            WriteLineC64("You need to be close enough to her before she reaches the front steps.");
            return true;
        }

        WriteLineC64("You shout her name. She spins, sees your face, then sees the shadow behind you.");
        WriteLineC64("" +
            "'Run!' you both say at once.");
        WriteLineC64("She bolts towards the lit main road.");

        state.Teleport(alley);
        stage = StoryStage.Warned;
        state.ShowRoom();
        WriteLineC64("Footsteps thunder behind you. Command: run");
        return true;
    }

    if (text.TextCompare("run") || text.TextCompare("flee") || text.TextCompare("sprint"))
    {
        if (stage != StoryStage.Warned)
            return false;

        WriteLineC64("You sprint into the alley, drawing him after you.");
        WriteLineC64("She reaches the main road and screams for help.");
        WriteLineC64("Something heavy crashes into your back. The ground comes up hard.");
        WriteLineC64("Her footsteps keep going. Yours stop.");
        WriteLineC64();
        WriteLineC64("=== YOU DIED. SHE GOT AWAY. ===");

        EndRun(EndingType.SelfSacrifice);
        return true;
    }

    if (text.TextCompare("defend") || text.TextCompare("attack") || text.TextCompare("use spanner") || text.TextCompare("use wrench"))
    {
        if (stage != StoryStage.Reveal)
            return false;

        IItem? weapon = state.Inventory.FindItem("spanner") ?? state.Inventory.FindItem("wrench") ?? state.Inventory.FindItem("wheel spanner");
        if (weapon is null)
        {
            WriteLineC64("You swing empty hands at a shape built for violence.");
            WriteLineC64("A scream tears through the hallway.");
            WriteLineC64();
            WriteLineC64("=== GAME OVER ===");
            EndRun(EndingType.ScreamEnding);
            return true;
        }

        WriteLineC64("You grip the wheel spanner and turn as the doorbell sounds again.");
        WriteLineC64("The shadow lunges. You swing first.");
        WriteLineC64("Bone cracks. He collapses against the banister.");
        WriteLineC64("She grabs your sleeve and drags you into the street as neighbours' lights flicker on.");
        WriteLineC64();
        WriteLineC64("=== YOU BOTH SURVIVED. ===");
        EndRun(EndingType.Defended);
        return true;
    }

    return false;
}

bool HandleRedHerringOpens(string text)
{
    if (text.TextCompare("open vending machine"))
    {
        WriteLineC64("You pry at the flap. It refuses. Your life remains unchanged.");
        return true;
    }

    if (text.TextCompare("open mailbox") || text.TextCompare("open letterbox"))
    {
        WriteLineC64("Leaflets, bills, and a pizza menu. No warnings from fate.");
        return true;
    }

    if (text.TextCompare("open window") || text.TextCompare("open house window"))
    {
        WriteLineC64("The sash does not move. Inside, only silence and curtains.");
        return true;
    }

    if (text.TextCompare("open gate") || text.TextCompare("open front gate"))
    {
        WriteLineC64("The gate clicks open, then swings back in the wind.");
        return true;
    }

    if (text.TextCompare("look tree") || text.TextCompare("examine oak") || text.TextCompare("look oak"))
    {
        WriteLineC64("The bark is scarred with initials. None of them are yours.");
        return true;
    }

    return false;
}

void OnMoveBeat()
{
    if (state.IsCurrentRoomId("concourse") && beats.Add("concourse"))
        WriteLineC64("She does not look back here. You almost pretend this is innocent.");

    if (state.IsCurrentRoomId("pavement") && beats.Add("pavement"))
        WriteLineC64("She glances over her shoulder. You look at your phone and miss a kerb.");

    if (state.IsCurrentRoomId("oak_corner") && beats.Add("oak"))
        WriteLineC64("Another furtive glance. You bend as if tying your shoe.");

    if (state.IsCurrentRoomId("gate") && beats.Add("gate"))
        WriteLineC64("She unlocks the gate quickly and hurries up the path.");

    if (state.IsCurrentRoomId("steps") && beats.Add("steps"))
        WriteLineC64("Your heart pounds as you reach for the bell.");
}

void TriggerDoorbellScene()
{
    WriteLineC64("You ring the bell. A chime sounds from inside.");

    if (tier == RunTier.ThirdPlus && state.Inventory.FindItem("spanner") is not null)
    {
        WriteLineC64("The door opens a fraction. She recognises you and gestures for you to hurry in.");
        WriteLineC64("'There was a guy following me,' she whispers.");
        WriteLineC64("'Yeah, it was me,' you say before your brain can stop your mouth.");
        WriteLineC64("'I know that,' she says, breath thin. 'I'm talking about the man behind you.'");
        WriteLineC64("The bell chimes again. The butterflies in your stomach stand still.");
        WriteLineC64("Command: defend");
        stage = StoryStage.Reveal;
        return;
    }

    WriteLineC64("The door opens a fraction. She recognises you and gestures for you to hurry in.");
    WriteLineC64("'There was a guy following me,' she whispers.");
    WriteLineC64("'Yeah, it was me,' you blurt.");
    WriteLineC64("'I know that,' she says, voice breaking. 'I'm talking about the man behind you.'");
    WriteLineC64("The bell chimes again, and the butterflies in your stomach stand still.");
    WriteLineC64("A scream rips through the house.");
    WriteLineC64();
    WriteLineC64("=== GAME OVER ===");

    EndRun(EndingType.ScreamEnding);
}

void EndRun(EndingType ending)
{
    progress = progress with { CompletedRuns = progress.CompletedRuns + 1 };
    SaveProgress(progressPath, progress);

    WriteLineC64();
    WriteLineC64($"Run complete: {progress.CompletedRuns}");

    if (progress.CompletedRuns == 1)
        WriteLineC64("New route unlocked next run: you can try to warn her before the doorbell scene.");
    else if (progress.CompletedRuns == 2)
        WriteLineC64("New route unlocked next run: a weapon may appear on the road.");
    else
        WriteLineC64("The loop remembers you. You remember it back.");

    stage = StoryStage.Ended;
}

static RunProgress LoadProgress(string path)
{
    try
    {
        if (!File.Exists(path))
            return new RunProgress();

        string json = File.ReadAllText(path);
        RunProgress? parsed = JsonSerializer.Deserialize<RunProgress>(json);
        return parsed ?? new RunProgress();
    }
    catch
    {
        return new RunProgress();
    }
}

static void SaveProgress(string path, RunProgress progress)
{
    try
    {
        string json = JsonSerializer.Serialize(progress, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }
    catch
    {
        // Keep gameplay running even if persistence fails.
    }
}

enum StoryStage
{
    Following,
    Warned,
    Reveal,
    Ended
}

enum RunTier
{
    First,
    Second,
    ThirdPlus
}

enum EndingType
{
    ScreamEnding,
    SelfSacrifice,
    Defended
}

sealed record RunProgress
{
    public int CompletedRuns { get; init; }
}
```
