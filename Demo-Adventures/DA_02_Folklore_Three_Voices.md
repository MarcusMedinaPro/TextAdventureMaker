# Demo Adventure 02: Folklore Three Voices

DSL-first demo adventure with three sequential perspectives:
- Act 1: James or Augustine (player chooses order)
- Act 2: the remaining counterpart
- Act 3: Betty

```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Dsl;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

const string JamesRole = "james";
const string AugustineRole = "augustine";
const string BettyRole = "betty";

string dsl = """
world: Folklore: Three Voices
goal: Play James and Augustine (in chosen order), then decide Betty's ending.
start: j_road

location: j_road | A dusty lane beyond the ice cream parlour. A cream-coloured car idles by the curb.
item: skateboard | Skateboard | A worn skateboard with chipped paint and muddy wheels. | aliases=board
exit: north -> j_crosswalk
exit: east -> j_beach

location: j_crosswalk | A quiet crosswalk where Inez waits with folded arms, watching everything.
exit: south -> j_road

location: j_beach | A wind-brushed beach at dusk. The tide keeps time with questions you cannot dodge.
exit: west -> j_road

location: a_roadside | The roadside is warm with late summer light. James stands by the passenger door.
exit: north -> a_mall
exit: east -> a_beach

location: a_mall | Behind the mall, shadows stretch across cracked tarmac where secrets feel louder.
item: note | Crumpled Note | A folded note that reads: 'Meet me behind the mall.' | aliases=paper
exit: south -> a_roadside

location: a_beach | The same beach, now from Augustine's side of the silence.
exit: west -> a_roadside

location: b_hallway | A school hallway lined with lockers and half-heard rumours.
exit: north -> b_driveway
exit: east -> b_porch

location: b_driveway | Rain taps the driveway outside Betty's party. One knock could change everything.
exit: south -> b_hallway

location: b_porch | A covered porch with damp steps and a cardigan folded on the rail.
item: cardigan | Cardigan | A soft cardigan carrying old warmth and unfinished conversations.
exit: west -> b_hallway
""";

DslParser dslParser = new();
DslAdventure adventure = dslParser.ParseString(dsl);
GameState state = adventure.State;

Location jRoad = adventure.Locations["j_road"];
Location jCrosswalk = adventure.Locations["j_crosswalk"];
Location jBeach = adventure.Locations["j_beach"];
Location aRoadside = adventure.Locations["a_roadside"];
Location aMall = adventure.Locations["a_mall"];
Location aBeach = adventure.Locations["a_beach"];
Location bHallway = adventure.Locations["b_hallway"];
Location bDriveway = adventure.Locations["b_driveway"];
Location bPorch = adventure.Locations["b_porch"];

jBeach.SetDynamicDescription(new DynamicDescription()
    .When(s => s.WorldState.GetFlag("james_told_truth"), "The beach air feels cleaner now. Truth still hurts, but it breathes.")
    .Default("A wind-brushed beach at dusk. The tide keeps time with questions you cannot dodge."));

aBeach.SetDynamicDescription(new DynamicDescription()
    .When(s => s.WorldState.GetFlag("james_told_truth"), "The tide sounds final. The truth was blunt, but real.")
    .Default("The same beach, now from Augustine's side of the silence."));

bDriveway.SetDynamicDescription(new DynamicDescription()
    .When(s => s.WorldState.GetFlag("betty_opened_door"), "The driveway is empty now, but the door is still ajar.")
    .When(s => s.WorldState.GetFlag("betty_closed_door"), "The rain keeps falling. The closed door keeps its own answer.")
    .Default("Rain taps the driveway outside Betty's party. One knock could change everything."));

jRoad.AddNpc(new Npc("augustine_j", "Augustine")
    .Description("Bright-eyed and restless. Her offer sounds casual, but it is not."));

jCrosswalk.AddNpc(new Npc("inez_j", "Inez")
    .Description("Sharp, observant, and impossible to fool."));

aRoadside.AddNpc(new Npc("james_a", "James")
    .Description("Present in body, absent in certainty."));

aMall.AddNpc(new Npc("inez_a", "Inez")
    .Description("She does not need many words. She rarely misses."));

bHallway.AddNpc(new Npc("inez_b", "Inez")
    .Description("A rumour can be cruel, but silence can be crueller."));

bDriveway.AddNpc(new Npc("james_b", "James")
    .Description("Wet hair, shaking hands, and a rehearsed apology that keeps changing."));

KeywordParser parser = new(KeywordParserConfigBuilder.BritishDefaults().Build());

SetupC64("Folklore Triangle Demo");
WriteLineC64("=== FOLKLORE: THREE VOICES ===");
WriteLineC64("DSL world loaded. Play three perspectives in one run.");
WriteLineC64($"World: {adventure.WorldName}");
WriteLineC64($"Goal: {adventure.Goal}");
WriteLineC64();

string firstRole = AskFirstRole();
string[] actOrder = firstRole.TextCompare(JamesRole)
    ? [JamesRole, AugustineRole, BettyRole]
    : [AugustineRole, JamesRole, BettyRole];

int actIndex = 0;
bool gameEnded = false;

InitialiseCounters();
StartAct();

while (!gameEnded)
{
    WriteLineC64();
    WritePromptC64("> ");
    string? input = Console.ReadLine();
    if (input is null)
        break;

    string trimmed = input.Trim();
    if (string.IsNullOrWhiteSpace(trimmed))
        continue;

    if (trimmed.TextCompare("status"))
    {
        PrintStatus();
        continue;
    }

    if (trimmed.TextCompare("help"))
    {
        ShowHelpForCurrentRole();
        continue;
    }

    if (trimmed.TextCompare("role"))
    {
        WriteLineC64($"Current role: {CurrentRole().ToProperCase()} (Act {actIndex + 1} of {actOrder.Length})");
        continue;
    }

    if (trimmed.StartsWithIgnoreCase("choose "))
    {
        string option = trimmed["choose ".Length..].ToId();
        HandleChoice(option);
        continue;
    }

    ICommand command = parser.Parse(trimmed);
    CommandResult result = state.Execute(command);
    state.DisplayResult(command, result);

    if (result.ShouldQuit)
        break;

    if (command is GoCommand && result.Success)
        ShowContextualChoices();
}

string AskFirstRole()
{
    while (true)
    {
        WritePromptC64("Choose first role (james/augustine): ");
        string? raw = Console.ReadLine();
        string role = raw?.Trim().ToId() ?? string.Empty;

        if (role.TextCompare(JamesRole) || role.TextCompare(AugustineRole))
            return role;

        WriteLineC64("Please type 'james' or 'augustine'.");
    }
}

Location GetStartLocation(string role) => role switch
{
    JamesRole => jRoad,
    AugustineRole => aRoadside,
    BettyRole => bHallway,
    _ => jRoad
};

string CurrentRole() => actOrder[actIndex];

void InitialiseCounters()
{
    // TEST: Baseline meters before any choices.
    AdjustCounter("integrity", 0, 0, 10);
    AdjustCounter("cowardice", 0, 0, 10);
    AdjustCounter("heartbreak", 0, 0, 12);

    AdjustCounter("trust", 5, 0, 10);
    AdjustCounter("resolve", 0, 0, 10);

    AdjustCounter("aug_hope", 5, 0, 10);
    AdjustCounter("aug_confusion", 0, 0, 10);
    AdjustCounter("aug_heartbreak", 0, 0, 10);
}

void StartAct()
{
    string role = CurrentRole();
    state.Inventory.Clear();
    state.Teleport(GetStartLocation(role));

    WriteLineC64();
    WriteLineC64($"--- Act {actIndex + 1}: {role.ToProperCase()} ---");

    if (role.TextCompare(JamesRole))
    {
        WriteLineC64("You are James. One summer decision can become a whole season of consequences.");
        WriteLineC64("Objective: make your roadside and beach choices.");
    }
    else if (role.TextCompare(AugustineRole))
    {
        WriteLineC64("You are Augustine. Hope and clarity pull in opposite directions.");
        WriteLineC64("Objective: choose how you pursue answers, then face the beach truth.");
    }
    else
    {
        WriteLineC64("You are Betty. Rumour, pride, and truth collide at your front door.");
        WriteLineC64("Objective: choose your stance, then decide whether to open the door.");
    }

    state.ShowRoom();
    ShowHelpForCurrentRole();
}

void HandleChoice(string option)
{
    if (string.IsNullOrWhiteSpace(option))
    {
        WriteLineC64("Choose what?");
        return;
    }

    bool handled = CurrentRole() switch
    {
        JamesRole => HandleJamesChoice(option),
        AugustineRole => HandleAugustineChoice(option),
        BettyRole => HandleBettyChoice(option),
        _ => false
    };

    if (!handled)
    {
        WriteLineC64($"'{option}' is not a valid choice here. Type 'help' for local options.");
        return;
    }

    // TEST: Automatic act transition when role-completion conditions are met.
    if (!IsCurrentRoleComplete())
        return;

    WriteRoleSummary(CurrentRole());

    actIndex++;
    if (actIndex >= actOrder.Length)
    {
        RenderFinalEnding();
        gameEnded = true;
        return;
    }

    StartAct();
}

bool HandleJamesChoice(string option)
{
    if (state.IsCurrentRoomId("j_road"))
    {
        if (option.TextCompare("ride"))
        {
            state.WorldState.SetFlag("james_road_decision", true);
            state.WorldState.SetFlag("james_took_ride", true);
            AdjustCounter("cowardice", 1, 0, 10);
            AdjustCounter("heartbreak", 1, 0, 12);
            state.WorldState.AddTimeline("James accepted Augustine's ride.");
            WriteLineC64("You slide into the passenger seat. Relief arrives first; guilt follows close behind.");
            return true;
        }

        if (option.TextCompare("stay"))
        {
            state.WorldState.SetFlag("james_road_decision", true);
            state.WorldState.SetFlag("james_took_ride", false);
            AdjustCounter("integrity", 2, 0, 10);
            AdjustCounter("trust", 1, 0, 10);
            state.WorldState.AddTimeline("James declined the ride and stayed with the difficult truth.");
            WriteLineC64("You step back from the car and choose the harder road home.");
            return true;
        }

        return false;
    }

    if (state.IsCurrentRoomId("j_crosswalk"))
    {
        if (option.TextCompare("hide"))
        {
            state.WorldState.SetFlag("james_crosswalk_decision", true);
            AdjustCounter("cowardice", 1, 0, 10);
            state.WorldState.AddTimeline("James hid from Inez at the crosswalk.");
            WriteLineC64("You duck your head. It buys a second, not absolution.");
            return true;
        }

        if (option.TextCompare("nod"))
        {
            state.WorldState.SetFlag("james_crosswalk_decision", true);
            AdjustCounter("integrity", 1, 0, 10);
            state.WorldState.AddTimeline("James acknowledged Inez at the crosswalk.");
            WriteLineC64("You meet Inez's gaze and nod once. She reads everything in it.");
            return true;
        }

        return false;
    }

    if (!state.IsCurrentRoomId("j_beach"))
        return false;

    if (option.TextCompare("truth"))
    {
        state.WorldState.SetFlag("james_beach_decision", true);
        state.WorldState.SetFlag("james_told_truth", true);
        AdjustCounter("integrity", 2, 0, 10);
        AdjustCounter("heartbreak", 1, 0, 12);
        state.WorldState.AddTimeline("James told Augustine the truth at the beach.");
        WriteLineC64("You tell the truth: summer was real, but not forever. It lands like cold rain.");
        return true;
    }

    if (option.TextCompare("lie"))
    {
        state.WorldState.SetFlag("james_beach_decision", true);
        state.WorldState.SetFlag("james_told_truth", false);
        AdjustCounter("cowardice", 2, 0, 10);
        AdjustCounter("heartbreak", 2, 0, 12);
        state.WorldState.AddTimeline("James lied to Augustine at the beach.");
        WriteLineC64("You choose a comforting lie. It sounds kind for a moment and cruel forever after.");
        return true;
    }

    return false;
}

bool HandleAugustineChoice(string option)
{
    if (state.IsCurrentRoomId("a_roadside"))
    {
        if (option.TextCompare("ask"))
        {
            state.WorldState.SetFlag("aug_road_decision", true);
            AdjustCounter("aug_hope", -1, 0, 10);
            AdjustCounter("aug_confusion", 2, 0, 10);
            state.WorldState.AddTimeline("Augustine asked James what the summer meant.");
            WriteLineC64("You ask for definition, not daydreams. His hesitation says enough.");
            return true;
        }

        if (option.TextCompare("drift"))
        {
            state.WorldState.SetFlag("aug_road_decision", true);
            AdjustCounter("aug_hope", 1, 0, 10);
            AdjustCounter("aug_confusion", 1, 0, 10);
            state.WorldState.AddTimeline("Augustine chose to drift in the present.");
            WriteLineC64("You decide to stay in the moment, even as doubt gathers at its edges.");
            return true;
        }

        return false;
    }

    if (state.IsCurrentRoomId("a_mall"))
    {
        if (option.TextCompare("meet"))
        {
            state.WorldState.SetFlag("aug_mall_decision", true);
            AdjustCounter("aug_hope", 1, 0, 10);
            AdjustCounter("aug_heartbreak", 1, 0, 10);
            state.WorldState.AddTimeline("Augustine agreed to the behind-the-mall meeting.");
            WriteLineC64("You go anyway. Hope and dread keep pace beside you.");
            return true;
        }

        if (option.TextCompare("cancel"))
        {
            state.WorldState.SetFlag("aug_mall_decision", true);
            AdjustCounter("aug_hope", -1, 0, 10);
            AdjustCounter("aug_heartbreak", 2, 0, 10);
            state.WorldState.AddTimeline("Augustine cancelled the meeting behind the mall.");
            WriteLineC64("You cancel the meeting. Silence hurts, but so does certainty.");
            return true;
        }

        return false;
    }

    if (!state.IsCurrentRoomId("a_beach"))
        return false;

    if (option.TextCompare("demand"))
    {
        state.WorldState.SetFlag("aug_beach_decision", true);
        bool jamesToldTruth = state.WorldState.GetFlag("james_told_truth");

        AdjustCounter("aug_confusion", jamesToldTruth ? 1 : 2, 0, 10);
        AdjustCounter("aug_heartbreak", jamesToldTruth ? 2 : 3, 0, 10);
        state.WorldState.AddTimeline(jamesToldTruth
            ? "Augustine demanded clarity and received a painful truth."
            : "Augustine demanded clarity and received another evasion.");

        WriteLineC64(jamesToldTruth
            ? "You force the question and finally get a clean answer. It breaks, but it clarifies."
            : "You force the question and still leave with fog where certainty should be.");
        return true;
    }

    if (option.TextCompare("retreat"))
    {
        state.WorldState.SetFlag("aug_beach_decision", true);
        AdjustCounter("aug_confusion", 1, 0, 10);
        AdjustCounter("aug_heartbreak", 1, 0, 10);
        state.WorldState.AddTimeline("Augustine retreated from the beach confrontation.");
        WriteLineC64("You step back before the question lands. The ache is smaller, but unresolved.");
        return true;
    }

    return false;
}

bool HandleBettyChoice(string option)
{
    if (state.IsCurrentRoomId("b_hallway"))
    {
        if (option.TextCompare("seek"))
        {
            state.WorldState.SetFlag("betty_hall_decision", true);
            AdjustCounter("trust", -1, 0, 10);
            AdjustCounter("resolve", 2, 0, 10);
            AdjustCounter("heartbreak", 1, 0, 12);
            state.WorldState.AddTimeline("Betty chose to seek the full truth.");
            WriteLineC64("You ask for details, even the parts that sting. Resolve grows where certainty used to be.");
            return true;
        }

        if (option.TextCompare("ignore"))
        {
            state.WorldState.SetFlag("betty_hall_decision", true);
            AdjustCounter("trust", 1, 0, 10);
            AdjustCounter("resolve", -1, 0, 10);
            state.WorldState.AddTimeline("Betty chose to ignore the rumours for now.");
            WriteLineC64("You choose distance from the noise. It buys peace, not clarity.");
            return true;
        }

        return false;
    }

    if (!state.IsCurrentRoomId("b_driveway"))
        return false;

    // TEST: Final branch is controlled only by Betty's driveway decision.
    if (option.TextCompare("open"))
    {
        state.WorldState.SetFlag("betty_door_decision", true);
        state.WorldState.SetFlag("betty_opened_door", true);
        state.WorldState.SetFlag("betty_closed_door", false);
        state.WorldState.AddTimeline("Betty opened the door.");
        WriteLineC64("You open the door before doubt can shut it for you.");
        return true;
    }

    if (option.TextCompare("close"))
    {
        state.WorldState.SetFlag("betty_door_decision", true);
        state.WorldState.SetFlag("betty_opened_door", false);
        state.WorldState.SetFlag("betty_closed_door", true);
        state.WorldState.AddTimeline("Betty closed the door.");
        WriteLineC64("You leave the door closed. Some endings are chosen, not suffered.");
        return true;
    }

    return false;
}

bool IsCurrentRoleComplete()
{
    string role = CurrentRole();

    if (role.TextCompare(JamesRole))
        return state.WorldState.GetFlag("james_road_decision") && state.WorldState.GetFlag("james_beach_decision");

    if (role.TextCompare(AugustineRole))
        return state.WorldState.GetFlag("aug_road_decision") && state.WorldState.GetFlag("aug_beach_decision");

    return state.WorldState.GetFlag("betty_hall_decision") && state.WorldState.GetFlag("betty_door_decision");
}

void WriteRoleSummary(string role)
{
    WriteLineC64();

    if (role.TextCompare(JamesRole))
    {
        WriteLineC64("James arc complete: one choice sought relief, the next demanded honesty.");
        return;
    }

    if (role.TextCompare(AugustineRole))
    {
        WriteLineC64("Augustine arc complete: hope met reality, and clarity came at a cost.");
        return;
    }

    WriteLineC64("Betty arc complete: the final threshold has been crossed.");
}

void RenderFinalEnding()
{
    int integrity = state.WorldState.GetCounter("integrity");
    int cowardice = state.WorldState.GetCounter("cowardice");
    int heartbreak = state.WorldState.GetCounter("heartbreak");
    int trust = state.WorldState.GetCounter("trust");
    int resolve = state.WorldState.GetCounter("resolve");

    int augHope = state.WorldState.GetCounter("aug_hope");
    int augConfusion = state.WorldState.GetCounter("aug_confusion");
    int augHeartbreak = state.WorldState.GetCounter("aug_heartbreak");

    bool openedDoor = state.WorldState.GetFlag("betty_opened_door");

    WriteLineC64();
    WriteLineC64("=== FINAL OUTCOME ===");

    if (!openedDoor)
    {
        WriteLineC64("Ending: The Closed Door");
        WriteLineC64("Betty protects her boundary. James is left on the driveway with rain and consequences.");
    }
    else if (integrity >= cowardice && resolve >= 2 && trust >= 3)
    {
        WriteLineC64("Ending: The Open Door, Earned");
        WriteLineC64("Betty lets James in. No one pretends the summer was harmless, but truth gives them a place to begin again.");
    }
    else
    {
        WriteLineC64("Ending: The Open Door, Uncertain");
        WriteLineC64("Betty opens the door, but not the past. Reconciliation remains possible, not guaranteed.");
    }

    WriteLineC64();
    WriteLineC64("Character summary:");
    WriteLineC64($"James -> Integrity {integrity}, Cowardice {cowardice}, Heartbreak Caused {heartbreak}");
    WriteLineC64($"Augustine -> Hope {augHope}, Confusion {augConfusion}, Heartbreak {augHeartbreak}");
    WriteLineC64($"Betty -> Trust {trust}, Resolve {resolve}");

    WriteLineC64();
    WriteLineC64("Timeline:");
    foreach (string entry in state.WorldState.Timeline)
        WriteLineC64($"- {entry}");

    WriteLineC64();
    WriteLineC64("=== END OF DEMO ===");
}

void PrintStatus()
{
    WriteLineC64($"Act: {actIndex + 1}/{actOrder.Length}");
    WriteLineC64($"Role: {CurrentRole().ToProperCase()}");
    WriteLineC64($"James -> Integrity {state.WorldState.GetCounter("integrity")}, Cowardice {state.WorldState.GetCounter("cowardice")}, Heartbreak {state.WorldState.GetCounter("heartbreak")}");
    WriteLineC64($"Augustine -> Hope {state.WorldState.GetCounter("aug_hope")}, Confusion {state.WorldState.GetCounter("aug_confusion")}, Heartbreak {state.WorldState.GetCounter("aug_heartbreak")}");
    WriteLineC64($"Betty -> Trust {state.WorldState.GetCounter("trust")}, Resolve {state.WorldState.GetCounter("resolve")}");
}

void ShowHelpForCurrentRole()
{
    WriteLineC64("Commands: look, go <north/east/south/west>, talk <npc>, take <item>, inventory, status, role, help, quit");
    ShowContextualChoices();
}

void ShowContextualChoices()
{
    if (CurrentRole().TextCompare(JamesRole))
    {
        if (state.IsCurrentRoomId("j_road"))
            WriteLineC64("Choices here: choose ride | choose stay");
        else if (state.IsCurrentRoomId("j_crosswalk"))
            WriteLineC64("Choices here: choose hide | choose nod");
        else if (state.IsCurrentRoomId("j_beach"))
            WriteLineC64("Choices here: choose truth | choose lie");

        return;
    }

    if (CurrentRole().TextCompare(AugustineRole))
    {
        if (state.IsCurrentRoomId("a_roadside"))
            WriteLineC64("Choices here: choose ask | choose drift");
        else if (state.IsCurrentRoomId("a_mall"))
            WriteLineC64("Choices here: choose meet | choose cancel");
        else if (state.IsCurrentRoomId("a_beach"))
            WriteLineC64("Choices here: choose demand | choose retreat");

        return;
    }

    if (state.IsCurrentRoomId("b_hallway"))
        WriteLineC64("Choices here: choose seek | choose ignore");
    else if (state.IsCurrentRoomId("b_driveway"))
        WriteLineC64("Choices here: choose open | choose close");
}

void AdjustCounter(string key, int delta, int min, int max)
{
    int current = state.WorldState.GetCounter(key);
    int target = (current + delta).Clamp(min, max);
    int change = target - current;

    if (change != 0)
        state.WorldState.Increment(key, change);
}
```
