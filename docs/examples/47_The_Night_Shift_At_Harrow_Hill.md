# The Night Shift at Harrow Hill

_Slice tag: Slice 47 — Time loop with NPC follower, DramaticIrony, and escalating patrol rounds._

## Story beats (max ~10 steps)
1) Arrive at the hospital for the night shift.
2) Begin patrol: reception, ward, morgue, office.
3) Notice the logbook entry is wrong — tonight's date is last week.
4) A figure appears in the ward. She says she's a nurse. She follows you.
5) Round 2: the ward has changed. Beds are occupied. They weren't before.
6) Round 3: the nurse speaks of patients who died here. All of them. Tonight.
7) The logbook now has your name. Time of death: 03:17.
8) Round 4: the morgue drawer is open. Your name is on the tag.
9) The nurse is gone. She was never here.
10) The phone rings. "Your shift is over."

## Map (rough layout)

```
          N
    W           E
          S

┌──────────┐    ┌──────────┐
│ Office   ├────┤ Reception│
│   L  P   │    │   D      │
└────┬─────┘    └────┬─────┘
     │               │
┌────┴─────┐    ┌────┴─────┐
│ Morgue   ├────┤  Ward    │
│   D      │    │   B  N   │
└──────────┘    └──────────┘

L = Logbook
P = Phone (fixed)
D = Desk/Drawer
B = Beds (fixed)
N = Nurse (NPC, appears round 2)
```

## Example (the night shift)
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

// === THE NIGHT SHIFT AT HARROW HILL ===
// A creepypasta about a night watchman trapped in a time loop.
// Features: DynamicDescription by round, NPC follower, DramaticIrony, TimeSystem

// --- Round tracking ---
int round = 1;
int roomsVisited = 0;
bool nurseAppeared = false;
bool logbookRead = false;
bool drawerOpened = false;

// --- Locations ---
Location reception = (id: "reception", description: "The hospital reception desk. A single lamp casts a yellow circle on the sign-in sheet.");
Location ward = (id: "ward", description: "A long ward with two rows of empty beds. The sheets are pulled tight.");
Location morgue = (id: "morgue", description: "The hospital morgue. Steel drawers line the far wall. The air is five degrees colder.");
Location office = (id: "office", description: "The night watchman's office. A logbook sits open on the desk. A telephone waits beside it.");

reception.AddExit(Direction.South, ward);
reception.AddExit(Direction.West, office);
ward.AddExit(Direction.North, reception);
ward.AddExit(Direction.West, morgue);
morgue.AddExit(Direction.East, ward);
morgue.AddExit(Direction.North, office);
office.AddExit(Direction.East, reception);
office.AddExit(Direction.South, morgue);

// --- Dynamic descriptions by round ---
reception.SetDynamicDescription(new DynamicDescription()
    .FirstVisit("You push through the double doors into reception. The fluorescent lights buzz overhead. A sign-in sheet lies on the counter. The last entry reads: 'Night shift — J. Harker — 14 Feb'. Today is the 21st.")
    .When(s => s.WorldState.GetFlag("round_3"),
        "Reception is darker now. One of the fluorescent tubes has gone out. The sign-in sheet has more entries than before. You don't remember anyone else arriving.")
    .When(s => s.WorldState.GetFlag("round_4"),
        "The reception is silent. All the lights are on, but they give no warmth. The sign-in sheet is full. Every entry is your name.")
    .Default("The hospital reception. The lamp hums. The sign-in sheet is undisturbed."));

ward.SetDynamicDescription(new DynamicDescription()
    .FirstVisit("The ward stretches ahead. Twenty beds, all empty, all immaculately made. Your footsteps echo on the linoleum. At the far end, a curtain stirs.")
    .When(s => s.WorldState.GetFlag("round_2") && !s.WorldState.GetFlag("round_3"),
        "The ward has changed. The beds are no longer empty. Shapes lie beneath the sheets, perfectly still. You count them — twenty, exactly. None of them are breathing. None of them need to.")
    .When(s => s.WorldState.GetFlag("round_3"),
        "The ward is full. The shapes in the beds have turned to face the door. You cannot see their eyes beneath the sheets, but you know they are watching. One of the beds has a clipboard. Your name is on it.")
    .When(s => s.WorldState.GetFlag("round_4"),
        "The ward is empty again. Every bed is made with hospital corners. A single clipboard lies on the nearest pillow. It reads: 'Discharged — all patients — 03:17.'")
    .Default("A long ward. The beds are empty. The curtain at the far end stirs."));

morgue.SetDynamicDescription(new DynamicDescription()
    .FirstVisit("The morgue door swings open with a hydraulic sigh. Steel drawers fill the wall, each with a paper label. All are closed. The drain in the centre of the floor is dry.")
    .When(s => s.WorldState.GetFlag("round_2") && !s.WorldState.GetFlag("round_3"),
        "The morgue is colder. One of the drawers is slightly ajar. You didn't leave it that way.")
    .When(s => s.WorldState.GetFlag("round_3"),
        "Three drawers are open now. The tags read names you don't recognise. The drain in the floor is no longer dry.")
    .When(s => s.WorldState.GetFlag("round_4"),
        "Every drawer is open. Every tag is blank except one. The furthest drawer on the bottom row. The tag reads your name. The drawer is empty. It is waiting.")
    .Default("The hospital morgue. Cold steel and silence."));

office.SetDynamicDescription(new DynamicDescription()
    .FirstVisit("Your office for the night. A metal desk, a swivel chair, a logbook, and a black telephone. The logbook is open to tonight's page.")
    .When(s => s.WorldState.GetFlag("round_3"),
        "The office feels smaller. The logbook is open to a different page. The telephone receiver is slightly off the hook. You can hear breathing.")
    .When(s => s.WorldState.GetFlag("round_4"),
        "The office. The logbook is closed. The telephone is ringing. It has been ringing for a very long time.")
    .Default("The night watchman's office. The logbook and telephone wait on the desk."));

// --- Items ---
var logbook = new Item("logbook", "Logbook", "The night watchman's logbook. A leather-bound ledger with entries in different hands.")
    .SetReadable()
    .SetReadText("Tonight's entry: 'Night shift — J. Harker — 14 Feb.' But today is the 21st. The entry is in your handwriting.")
    .SetTakeable(false);

var phone = new Item("phone", "Telephone", "A black rotary telephone. It sits silently on the desk.")
    .SetTakeable(false);

var signinSheet = new Item("sheet", "Sign-in Sheet", "The reception sign-in sheet. The last entry is dated a week ago.")
    .SetTakeable(false)
    .SetReadable()
    .SetReadText("'Night shift — J. Harker — 14 Feb.' One entry. Just the one. In your handwriting.");

var drawer = new Item("drawer", "Morgue Drawer", "A steel mortuary drawer. The label slot is empty.")
    .SetTakeable(false);

var beds = new Item("beds", "Hospital Beds", "Twenty beds in two neat rows. The sheets are white and taut.")
    .SetTakeable(false);

office.AddItem(logbook);
office.AddItem(phone);
reception.AddItem(signinSheet);
morgue.AddItem(drawer);
ward.AddItem(beds);

// --- Nurse NPC ---
var nurse = new Npc("nurse", "Nurse Ashby")
    .Description("A woman in a pale blue nurse's uniform. Her name badge reads 'Ashby'. She smiles, but her eyes don't move.")
    .SetDialog(new DialogNode("I'm on the night shift too. Shall I walk with you? It gets lonely here after dark.")
        .AddOption("Who are you?",
            new DialogNode("Nurse Ashby. I've worked here for... a long time. Longer than I can remember, actually. Isn't that funny?"))
        .AddOption("What happened to the patients?",
            new DialogNode("They're resting. They're all resting now. That's what this place is for, isn't it? Rest.")));

// --- Game state ---
var state = new GameState(reception, worldLocations: [reception, ward, morgue, office]);

// DramaticIrony: player learns the truth about Nurse Ashby
state.DramaticIrony.PlayerLearn("nurse_ashby_died_14_feb");

var parser = new KeywordParser(KeywordParserConfigBuilder.BritishDefaults().Build());

SetupC64("The Night Shift at Harrow Hill");
WriteLineC64("=== THE NIGHT SHIFT AT HARROW HILL ===");
WriteLineC64();
WriteLineC64("You are J. Harker, night watchman at Harrow Hill Hospital.");
WriteLineC64("The building was decommissioned six months ago. Your job is to");
WriteLineC64("patrol four areas each night: reception, ward, morgue, office.");
WriteLineC64();
WriteLineC64("Goal: Complete your patrol. Survive until morning.");
WriteLineC64();
WriteLineC64("Round 1 of 4. Begin your patrol.");

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

    // Custom: answer phone
    if (trimmed.TextCompare("answer phone") || trimmed.TextCompare("use phone") || trimmed.TextCompare("pick up phone"))
    {
        if (!state.IsCurrentRoomId("office"))
        {
            WriteLineC64("There is no telephone here.");
            continue;
        }

        if (state.WorldState.GetFlag("round_4"))
        {
            WriteLineC64("You pick up the receiver.");
            WriteLineC64();
            WriteLineC64("A voice, calm and distant: 'Your shift is over, Mr Harker.'");
            WriteLineC64();
            WriteLineC64("The line goes dead. The lights go out.");
            WriteLineC64("When they come back on, you are standing outside the hospital.");
            WriteLineC64("The doors are chained shut. They have been chained shut for months.");
            WriteLineC64();
            WriteLineC64("In your pocket, you find tonight's logbook page, torn out.");
            WriteLineC64("The entry reads: 'Night shift — J. Harker — 14 Feb.'");
            WriteLineC64("Below it, in a hand you do not recognise:");
            WriteLineC64("'Time of death: 03:17.'");
            WriteLineC64();
            WriteLineC64("=== THE END ===");
            break;
        }

        WriteLineC64("You pick up the receiver. Dial tone. No one is calling.");
        continue;
    }

    // Custom: open drawer
    if (trimmed.TextCompare("open drawer") || trimmed.TextCompare("examine drawer"))
    {
        if (!state.IsCurrentRoomId("morgue"))
        {
            WriteLineC64("There are no drawers here.");
            continue;
        }

        drawerOpened = true;

        if (state.WorldState.GetFlag("round_4"))
        {
            WriteLineC64("You slide open the drawer with your name on it. Inside, on the cold steel tray, there is nothing.");
            WriteLineC64("Nothing except a folded note in your handwriting: 'Don't answer the phone.'");
        }
        else if (state.WorldState.GetFlag("round_3"))
        {
            WriteLineC64("You pull open one of the drawers. Empty. But the paper label has fresh ink. You close it quickly.");
        }
        else
        {
            WriteLineC64("The drawers are all closed and labelled. The labels are faded and illegible. You leave them shut.");
        }
        continue;
    }

    var command = parser.Parse(trimmed);
    var result = state.Execute(command);

    // Track logbook reading
    if (command is ReadCommand && result.Success && state.IsCurrentRoomId("office"))
    {
        logbookRead = true;
        if (round >= 3)
        {
            WriteLineC64("The logbook entry has changed. It now reads: 'Night shift — J. Harker — 14 Feb. Time of death: 03:17.'");
            WriteLineC64("The handwriting is yours.");
        }
    }

    state.DisplayResult(command, result);

    // Track room visits for round progression
    if (command is GoCommand && result.Success)
    {
        roomsVisited++;

        // Every 4 room transitions = new round
        if (roomsVisited % 4 == 0 && round < 4)
        {
            round++;
            WriteLineC64();

            if (round == 2)
            {
                state.WorldState.SetFlag("round_2", true);
                WriteLineC64("--- Round 2 ---");
                WriteLineC64("The air has changed. It's colder. The lights flicker once.");

                // Nurse appears
                if (!nurseAppeared)
                {
                    nurseAppeared = true;
                    ward.AddNpc(nurse);
                    WriteLineC64("You hear footsteps from the ward. Someone else is here.");
                }
            }
            else if (round == 3)
            {
                state.WorldState.SetFlag("round_3", true);
                WriteLineC64("--- Round 3 ---");
                WriteLineC64("The hospital has changed. You are certain of it now.");
                WriteLineC64("The walls are closer. The corridors are longer.");

                // Nurse dialogue changes
                nurse.SetDialog(new DialogNode("They all died on the same night, you know. All twenty of them. The fourteenth of February. Isn't that romantic?")
                    .AddOption("What happened?",
                        new DialogNode("A fire in the east wing. The doors were locked. No one came. No one ever comes to Harrow Hill at night."))
                    .AddOption("Who are you, really?",
                        new DialogNode("I told you. Nurse Ashby. I was here that night. I'm always here that night.")));
            }
            else if (round == 4)
            {
                state.WorldState.SetFlag("round_4", true);
                WriteLineC64("--- Round 4 (Final) ---");
                WriteLineC64("The hospital is silent. Nurse Ashby is gone.");
                WriteLineC64("You are alone. You have always been alone.");
                WriteLineC64("From the office, you hear the telephone ringing.");

                // Remove nurse
                ward.RemoveNpc(nurse);

                // The character now learns what the player already knew
                state.DramaticIrony.NpcLearn(nurse, "nurse_ashby_died_14_feb");
            }
        }
    }

    if (result.ShouldQuit)
        break;
}
```
