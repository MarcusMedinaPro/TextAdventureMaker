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

const int JamesFinalScene = 9;
const int AugustineFinalScene = 5;
const int BettyFinalScene = 4;

string dsl = """
world: Folklore: Three Voices
goal: Follow James, Augustine, and Betty across the full summer arc with cross-linked consequences.
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

AdventureDslParser dslParser = new();
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
    .When(s => s.WorldState.GetFlag("james_beach_truth"), "The beach air feels cleaner now. Truth still hurts, but it breathes.")
    .When(s => s.WorldState.GetFlag("james_beach_lie"), "The tide sounds accusatory, as if the sea itself heard the lie.")
    .Default("A wind-brushed beach at dusk. The tide keeps time with questions you cannot dodge."));

aBeach.SetDynamicDescription(new DynamicDescription()
    .When(s => s.WorldState.GetFlag("james_beach_truth"), "The tide sounds final. The truth was blunt, but real.")
    .When(s => s.WorldState.GetFlag("james_beach_lie"), "The horizon looks beautiful, but hollow. The words said here do not feel trustworthy.")
    .Default("The same beach, now from Augustine's side of the silence."));

bDriveway.SetDynamicDescription(new DynamicDescription()
    .When(s => s.WorldState.GetFlag("betty_opened_door"), "The driveway is empty now, but the door is still ajar.")
    .When(s => s.WorldState.GetFlag("betty_closed_door"), "The rain keeps falling. The closed door keeps its own answer.")
    .When(s => s.WorldState.GetFlag("betty_confronted_publicly"), "The rain carries away old fear. The porch light burns, but no one waits in silence anymore.")
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
WriteLineC64("Sandbox edition aligned to the full story plans for James, Augustine, and Betty.");
WriteLineC64($"World: {adventure.WorldName}");
WriteLineC64($"Goal: {adventure.Goal}");
WriteLineC64();

string firstRole = AskFirstRole();
string[] actOrder = firstRole.TextCompare(JamesRole)
    ? [JamesRole, AugustineRole, BettyRole]
    : [AugustineRole, JamesRole, BettyRole];

int actIndex = 0;
int jamesScene = 0;
int augustineScene = 0;
int bettyScene = 0;
string jamesEnding = "Unresolved";
string augustineEnding = "Unresolved";
string bettyEnding = "Unresolved";
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

    if (trimmed.TextCompare("scene"))
    {
        DescribeCurrentScene(showRoom: false);
        continue;
    }

    if (trimmed.TextCompare("timeline"))
    {
        PrintTimeline();
        continue;
    }

    if (trimmed.TextCompare("choices"))
    {
        ShowContextualChoices();
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

string CurrentRole() => actOrder[actIndex];

int GetScene(string role) => role switch
{
    JamesRole => jamesScene,
    AugustineRole => augustineScene,
    BettyRole => bettyScene,
    _ => 0
};

void SetScene(string role, int scene)
{
    if (role.TextCompare(JamesRole))
        jamesScene = scene;
    else if (role.TextCompare(AugustineRole))
        augustineScene = scene;
    else if (role.TextCompare(BettyRole))
        bettyScene = scene;
}

int GetFinalScene(string role) => role switch
{
    JamesRole => JamesFinalScene,
    AugustineRole => AugustineFinalScene,
    BettyRole => BettyFinalScene,
    _ => 0
};

bool IsRoleComplete(string role) => GetScene(role) > GetFinalScene(role);

bool IsCurrentRoleComplete() => IsRoleComplete(CurrentRole());

void AdvanceScene(string role)
{
    SetScene(role, GetScene(role) + 1);

    if (!IsRoleComplete(role))
        EnterScene(role);
}

void CompleteRole(string role)
{
    SetScene(role, GetFinalScene(role) + 1);
}

void InitialiseCounters()
{
    AdjustCounter("integrity", 0, 0, 10);
    AdjustCounter("cowardice", 0, 0, 10);
    AdjustCounter("j_heartbreak", 0, 0, 12);

    AdjustCounter("trust", 5, 0, 10);
    AdjustCounter("resolve", 0, 0, 10);
    AdjustCounter("b_heartbreak", 0, 0, 12);

    AdjustCounter("aug_hope", 5, 0, 10);
    AdjustCounter("aug_confusion", 0, 0, 10);
    AdjustCounter("aug_heartbreak", 0, 0, 12);
}

void StartAct()
{
    string role = CurrentRole();
    state.Inventory.Clear();

    SetScene(role, 0);

    WriteLineC64();
    WriteLineC64($"--- Act {actIndex + 1}: {role.ToProperCase()} ---");

    if (role.TextCompare(JamesRole))
    {
        WriteLineC64("You are James. Summer starts with one mistake and keeps asking for harder truths.");
        WriteLineC64("Objective: navigate every major scene from the parlour fallout to the driveway finale.");
    }
    else if (role.TextCompare(AugustineRole))
    {
        WriteLineC64("You are Augustine. Hope, confusion, and heartbreak shift with every answer James avoids.");
        WriteLineC64("Objective: follow the full arc from the roadside spark to the beach reckoning.");

        if (JamesSkippedTriangle())
            WriteLineC64("James previously avoided the triangle route, so neutral outcomes are now possible.");
    }
    else
    {
        WriteLineC64("You are Betty. Rumour, dignity, and intuition collide before the final door decision.");
        WriteLineC64("Objective: follow the full route from suspicion to the driveway ending.");

        if (JamesSkippedTriangle())
            WriteLineC64("James previously avoided the triangle route, so a peaceful neutral end is possible.");
    }

    EnterScene(role);
    ShowHelpForCurrentRole();
}

void EnterScene(string role)
{
    state.Teleport(GetSceneLocation(role, GetScene(role)));
    DescribeCurrentScene(showRoom: true);
}

Location GetSceneLocation(string role, int scene)
{
    return role switch
    {
        JamesRole => scene switch
        {
            0 or 1 or 3 or 4 or 5 => jRoad,
            2 or 6 or 8 => jCrosswalk,
            7 => jBeach,
            9 => bDriveway,
            _ => jRoad
        },
        AugustineRole => scene switch
        {
            0 or 1 or 2 or 3 => aRoadside,
            4 => aMall,
            5 => aBeach,
            _ => aRoadside
        },
        BettyRole => scene switch
        {
            0 or 1 or 2 => bHallway,
            3 => bPorch,
            4 => bDriveway,
            _ => bHallway
        },
        _ => jRoad
    };
}

void DescribeCurrentScene(bool showRoom)
{
    (string title, string description) = GetSceneInfo(CurrentRole(), GetScene(CurrentRole()));
    WriteLineC64();
    WriteLineC64($"Scene: {title}");
    WriteLineC64(description);

    if (showRoom)
        state.ShowRoom();

    ShowContextualChoices();
}

(string Title, string Description) GetSceneInfo(string role, int scene)
{
    if (role.TextCompare(JamesRole))
    {
        return scene switch
        {
            0 => ("Prologue: Ice Cream Parlour Fallout", "Betty has just walked away. You are left with guilt, heat, and one chance to choose how you respond."),
            1 => ("Encounter: Augustine's Car", "On the roadside, Augustine offers a ride and an escape route. This is the first major fork of the whole triangle."),
            2 => ("Inez's Eye 1", "At the crosswalk, Inez sees you with Augustine. Whether you hide, nod, jump out, or flirt changes how rumours begin."),
            3 => ("Quiet Moment: Betty's Message", "Betty texts: 'Everything okay? Miss you.' The reply you send shapes trust and guilt."),
            4 => ("Accidental Encounter: Grocery Store", "You and Augustine spot Betty in a supermarket aisle. Avoidance and confrontation carry different costs."),
            5 => ("Sneaking Out", "At night you can continue to Augustine, lie to Betty, or attempt a partial truth at Betty's window."),
            6 => ("Inez's Eye 2", "At a party near the stone bridge, Inez sees everything again. Your public behaviour now echoes into Betty's story."),
            7 => ("Beach: Moment of Truth", "Augustine asks directly whether you love her. This answer drives every later reconciliation branch."),
            8 => ("School Start", "Autumn begins. You see Betty in the hallway. Fear or forward motion both leave marks."),
            9 => ("Finale: The Driveway", "Rain, party lights, and one final decision: knock or turn away."),
            _ => ("Epilogue", "James's arc is complete.")
        };
    }

    if (role.TextCompare(AugustineRole))
    {
        return scene switch
        {
            0 => ("Prologue: The Roadside Encounter", "Augustine's summer begins with a lift offer and a belief that this could be real."),
            1 => ("Early Summer: Golden Hour", "The relationship glows. You decide whether to probe, float in the present, or seek deeper integration."),
            2 => ("Inez's First Afterthought", "A look from Inez unsettles everything. You can challenge, dismiss, or vent your frustration."),
            3 => ("The Evening Creeps In", "Rumours grow. You decide whether to confront, deny, or distance yourself."),
            4 => ("Secret Meeting: Behind the Mall", "A message calls you to the old meeting place. Hope, suspicion, or avoidance all carry different weight."),
            5 => ("Beach: Augustine's Reckoning", "You ask for a clear answer and face what James has already chosen in his own arc."),
            _ => ("Epilogue", "Augustine's arc is complete.")
        };
    }

    return scene switch
    {
        0 => ("Early Summer: Longing and Wonder", "James is distant. You choose how to handle the first wave of doubt."),
        1 => ("An Unexpected Sight", "You spot James near Augustine's car. You can pass in silence or force acknowledgement."),
        2 => ("Inez's Second Hint", "Inez reports what happened at the bridge party. You decide whether to dismiss, dig, or lash out."),
        3 => ("A Conversation with Myself", "On the porch, you choose rationalisation, anger, or one last attempt to demand truth."),
        4 => ("The Driveway", "At the threshold, you decide the ending: open, close, avoid, or confront publicly."),
        _ => ("Epilogue", "Betty's arc is complete.")
    };
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
        WriteLineC64($"'{option}' is not a valid choice here. Type 'choices' or 'help'.");
        return;
    }

    if (!IsCurrentRoleComplete())
        return;

    WriteRoleSummary(CurrentRole());

    actIndex++;
    if (actIndex >= actOrder.Length)
    {
        RenderFinalOutcome();
        gameEnded = true;
        return;
    }

    StartAct();
}

bool HandleJamesChoice(string option)
{
    return GetScene(JamesRole) switch
    {
        0 => HandleJamesPrologue(option),
        1 => HandleJamesEncounter(option),
        2 => HandleJamesInezOne(option),
        3 => HandleJamesMessage(option),
        4 => HandleJamesGrocery(option),
        5 => HandleJamesSneakingOut(option),
        6 => HandleJamesInezTwo(option),
        7 => HandleJamesBeach(option),
        8 => HandleJamesSchoolStart(option),
        9 => HandleJamesFinale(option),
        _ => false
    };
}

bool HandleJamesPrologue(string option)
{
    if (IsAny(option, "a", "dumb", "cowardly", "silly"))
    {
        AdjustCounter("cowardice", 1, 0, 10);
        AddBeat("James offered a weak, evasive excuse at the parlour.");
        WriteLineC64("You blurt out something flimsy. It lands badly and makes the silence colder.");
        AdvanceScene(JamesRole);
        return true;
    }

    if (IsAny(option, "b", "apologise", "apologize", "sorry"))
    {
        AdjustCounter("integrity", 1, 0, 10);
        AddBeat("James tried to apologise, but too late to stop the immediate damage.");
        WriteLineC64("You try to apologise. The words are real, but incomplete.");
        AdvanceScene(JamesRole);
        return true;
    }

    if (IsAny(option, "c", "joke", "distract"))
    {
        AdjustCounter("cowardice", 1, 0, 10);
        AdjustCounter("j_heartbreak", 1, 0, 12);
        AddBeat("James hid discomfort behind humour at the parlour.");
        WriteLineC64("You hide behind a joke. It protects nothing.");
        AdvanceScene(JamesRole);
        return true;
    }

    return false;
}

bool HandleJamesEncounter(string option)
{
    if (IsAny(option, "1", "direct", "get_in"))
    {
        state.WorldState.SetFlag("james_in_triangle", true);
        AddBeat("James got into Augustine's car without hesitation.");
        WriteLineC64("You get in directly. Summer tilts in a dangerous direction.");
        AdvanceScene(JamesRole);
        return true;
    }

    if (IsAny(option, "2", "hesitate", "hesitated"))
    {
        state.WorldState.SetFlag("james_in_triangle", true);
        state.WorldState.SetFlag("james_hesitated_then_got_in", true);
        AdjustCounter("cowardice", 1, 0, 10);
        AddBeat("James hesitated, then accepted Augustine's ride.");
        WriteLineC64("You hesitate, then climb in. Guilt arrives before the music starts.");
        AdvanceScene(JamesRole);
        return true;
    }

    if (IsAny(option, "3", "decline", "stay", "walk"))
    {
        state.WorldState.SetFlag("james_declined_ride", true);
        state.WorldState.SetFlag("james_in_triangle", false);
        AdjustCounter("integrity", 2, 0, 10);
        AddBeat("James declined the ride and walked home to face consequences.");
        jamesEnding = "Neutral Ending (The Shortcut)";
        WriteLineC64("You decline and keep walking. The triangle does not fully ignite.");
        CompleteRole(JamesRole);
        return true;
    }

    if (IsAny(option, "4", "drive", "drive_far", "exile"))
    {
        state.WorldState.SetFlag("james_drove_away", true);
        state.WorldState.SetFlag("james_in_triangle", false);
        AdjustCounter("cowardice", 2, 0, 10);
        AddBeat("James asked Augustine to drive far away and fled town accountability.");
        jamesEnding = "Bad Ending (The Exile)";
        WriteLineC64("You ask to drive far away. Avoidance becomes the only plan.");
        CompleteRole(JamesRole);
        return true;
    }

    return false;
}

bool HandleJamesInezOne(string option)
{
    if (IsAny(option, "a", "hide", "duck"))
    {
        state.WorldState.SetFlag("james_inez1_hide", true);
        AdjustCounter("cowardice", 1, 0, 10);
        AdjustCounter("j_heartbreak", 1, 0, 12);
        AddBeat("Inez saw James hide in Augustine's car.");
        WriteLineC64("You duck down. Inez notices anyway.");
        AdvanceScene(JamesRole);
        return true;
    }

    if (IsAny(option, "b", "nod"))
    {
        state.WorldState.SetFlag("james_inez1_nod", true);
        AdjustCounter("integrity", 1, 0, 10);
        AddBeat("Inez saw James acknowledge her with a tense nod.");
        WriteLineC64("You nod once. The silence between you and Inez says enough.");
        AdvanceScene(JamesRole);
        return true;
    }

    if (IsAny(option, "c", "jump", "jump_out"))
    {
        state.WorldState.SetFlag("james_inez1_jump", true);
        state.WorldState.SetFlag("james_broke_off_early", true);
        AdjustCounter("integrity", 2, 0, 10);
        AddBeat("James jumped out of Augustine's car after seeing Inez.");
        WriteLineC64("You jump out and walk. It is abrupt, messy, and honest.");
        AdvanceScene(JamesRole);
        return true;
    }

    if (IsAny(option, "d", "flirt", "flirt_with_augustine"))
    {
        state.WorldState.SetFlag("james_inez1_flirt", true);
        AdjustCounter("cowardice", 1, 0, 10);
        AdjustCounter("j_heartbreak", 1, 0, 12);
        AddBeat("James doubled down and flirted while Inez watched.");
        WriteLineC64("You lean into the performance. It looks bold and feels hollow.");
        AdvanceScene(JamesRole);
        return true;
    }

    return false;
}

bool HandleJamesMessage(string option)
{
    if (IsAny(option, "a", "reply", "truthful", "partly_truthful"))
    {
        state.WorldState.SetFlag("james_message_reply", true);
        AdjustCounter("integrity", 1, 0, 10);
        AdjustCounter("j_heartbreak", -1, 0, 12);
        AddBeat("James replied to Betty with a partial truth.");
        WriteLineC64("You reply carefully, keeping the line open without telling the full story.");
        AdvanceScene(JamesRole);
        return true;
    }

    if (IsAny(option, "b", "lie"))
    {
        state.WorldState.SetFlag("james_message_lie", true);
        AdjustCounter("cowardice", 1, 0, 10);
        AdjustCounter("j_heartbreak", 1, 0, 12);
        AddBeat("James lied to Betty by text.");
        WriteLineC64("You type a clean lie. It looks harmless and lands heavily.");
        AdvanceScene(JamesRole);
        return true;
    }

    if (IsAny(option, "c", "ignore"))
    {
        state.WorldState.SetFlag("james_message_ignore", true);
        AdjustCounter("cowardice", 2, 0, 10);
        AdjustCounter("j_heartbreak", 1, 0, 12);
        AddBeat("James ignored Betty's message.");
        WriteLineC64("You put the phone away. Silence becomes another decision.");
        AdvanceScene(JamesRole);
        return true;
    }

    return false;
}

bool HandleJamesGrocery(string option)
{
    if (IsAny(option, "a", "leave", "pull_away"))
    {
        state.WorldState.SetFlag("james_grocery_leave", true);
        AdjustCounter("cowardice", 2, 0, 10);
        AdjustCounter("j_heartbreak", 1, 0, 12);
        AddBeat("James pulled Augustine away from Betty at the grocery store.");
        WriteLineC64("You leave quickly. Panic wins the moment.");
        AdvanceScene(JamesRole);
        return true;
    }

    if (IsAny(option, "b", "pretend", "pretend_not_to_see"))
    {
        state.WorldState.SetFlag("james_grocery_pretend", true);
        AdjustCounter("cowardice", 1, 0, 10);
        AddBeat("James pretended not to see Betty at the grocery store.");
        WriteLineC64("You pretend not to see her. She sees you anyway.");
        AdvanceScene(JamesRole);
        return true;
    }

    if (IsAny(option, "c", "greet", "say_hi", "go_over"))
    {
        state.WorldState.SetFlag("james_grocery_greet", true);
        AdjustCounter("integrity", 1, 0, 10);
        AdjustCounter("cowardice", -1, 0, 10);
        AddBeat("James chose to face Betty at the grocery store.");
        WriteLineC64("You walk over and greet Betty. The cold in her tone does not leave room for illusion.");
        AdvanceScene(JamesRole);
        return true;
    }

    return false;
}

bool HandleJamesSneakingOut(string option)
{
    if (IsAny(option, "a", "augustine", "continue"))
    {
        state.WorldState.SetFlag("james_sneak_augustine", true);
        AdjustCounter("j_heartbreak", 1, 0, 12);
        AdjustCounter("cowardice", 1, 0, 10);
        AddBeat("James continued to Augustine during the night meeting.");
        WriteLineC64("You continue to Augustine. The haze deepens.");
        AdvanceScene(JamesRole);
        return true;
    }

    if (IsAny(option, "b", "betty_lie", "lie_to_betty"))
    {
        state.WorldState.SetFlag("james_sneak_betty_lie", true);
        state.WorldState.SetFlag("cardigan_found", true);
        AdjustCounter("cowardice", 2, 0, 10);
        AddBeat("James visited Betty at night and lied; the cardigan became symbolic evidence.");

        IItem? cardigan = bPorch.FindItem("cardigan");
        if (cardigan != null)
        {
            _ = bPorch.RemoveItem(cardigan);
            _ = state.Inventory.Add(cardigan);
        }

        WriteLineC64("You go to Betty and lie. You notice the cardigan and cannot ignore what it represents.");
        AdvanceScene(JamesRole);
        return true;
    }

    if (IsAny(option, "c", "betty_explain", "explain"))
    {
        state.WorldState.SetFlag("james_sneak_betty_explain", true);
        AdjustCounter("integrity", 1, 0, 10);
        AdjustCounter("j_heartbreak", 1, 0, 12);
        AddBeat("James attempted an incomplete explanation to Betty.");
        WriteLineC64("You try to explain without fully confessing. It helps less than you hoped.");
        AdvanceScene(JamesRole);
        return true;
    }

    return false;
}

bool HandleJamesInezTwo(string option)
{
    if (IsAny(option, "a", "hold", "hold_tightly"))
    {
        state.WorldState.SetFlag("james_inez2_hold", true);
        AdjustCounter("j_heartbreak", 2, 0, 12);
        AdjustCounter("cowardice", 1, 0, 10);
        AddBeat("At the bridge party, James held Augustine tightly in full view.");
        WriteLineC64("You hold Augustine tightly while Inez watches. Rumours accelerate.");
        AdvanceScene(JamesRole);
        return true;
    }

    if (IsAny(option, "b", "pull", "pull_away"))
    {
        state.WorldState.SetFlag("james_inez2_pull", true);
        AdjustCounter("cowardice", 1, 0, 10);
        AdjustCounter("integrity", 1, 0, 10);
        AddBeat("At the bridge party, James pulled away once he felt seen.");
        WriteLineC64("You pull away from Augustine. It is awkward, but not meaningless.");
        AdvanceScene(JamesRole);
        return true;
    }

    if (IsAny(option, "c", "confront", "confront_inez"))
    {
        state.WorldState.SetFlag("james_inez2_confront", true);
        AdjustCounter("integrity", 1, 0, 10);
        AddBeat("James confronted Inez directly at the party.");
        WriteLineC64("You confront Inez directly. Tension rises in every direction.");
        AdvanceScene(JamesRole);
        return true;
    }

    if (IsAny(option, "d", "kiss_demo", "kiss_demonstratively"))
    {
        state.WorldState.SetFlag("james_inez2_kiss", true);
        AdjustCounter("j_heartbreak", 3, 0, 12);
        AdjustCounter("cowardice", 1, 0, 10);
        AddBeat("James kissed Augustine demonstratively in front of Inez.");
        WriteLineC64("You kiss Augustine to make a point. The point costs everyone.");
        AdvanceScene(JamesRole);
        return true;
    }

    return false;
}

bool HandleJamesBeach(string option)
{
    if (IsAny(option, "a", "say_lie", "lie", "love_lie"))
    {
        state.WorldState.SetFlag("james_beach_lie", true);
        AdjustCounter("j_heartbreak", 2, 0, 12);
        AdjustCounter("cowardice", 1, 0, 10);
        AddBeat("James told Augustine a direct lie at the beach.");
        WriteLineC64("You say what she needs to hear, not what is true.");
        AdvanceScene(JamesRole);
        return true;
    }

    if (IsAny(option, "b", "truth", "tell_truth"))
    {
        state.WorldState.SetFlag("james_beach_truth", true);
        AdjustCounter("integrity", 1, 0, 10);
        AdjustCounter("j_heartbreak", 2, 0, 12);
        AddBeat("James told Augustine the truth: it was summer, not forever.");
        WriteLineC64("You tell the truth. It hurts immediately, but it is real.");
        AdvanceScene(JamesRole);
        return true;
    }

    if (IsAny(option, "c", "avoid", "avoid_question"))
    {
        state.WorldState.SetFlag("james_beach_avoid", true);
        AdjustCounter("cowardice", 1, 0, 10);
        AdjustCounter("j_heartbreak", 1, 0, 12);
        AddBeat("James avoided Augustine's direct question at the beach.");
        WriteLineC64("You avoid the question and call it confusion.");
        AdvanceScene(JamesRole);
        return true;
    }

    if (IsAny(option, "d", "kiss", "kiss_silently"))
    {
        state.WorldState.SetFlag("james_beach_kiss", true);
        AdjustCounter("cowardice", 1, 0, 10);
        AdjustCounter("j_heartbreak", 1, 0, 12);
        AddBeat("James used a kiss to delay the truth at the beach.");
        WriteLineC64("You kiss her instead of answering. The question remains.");
        AdvanceScene(JamesRole);
        return true;
    }

    return false;
}

bool HandleJamesSchoolStart(string option)
{
    if (IsAny(option, "a", "look_away", "lookaway"))
    {
        state.WorldState.SetFlag("james_school_lookaway", true);
        AdjustCounter("cowardice", 1, 0, 10);
        AddBeat("At school start, James looked away from Betty.");
        WriteLineC64("You look away. The moment passes, unresolved.");
        AdvanceScene(JamesRole);
        return true;
    }

    if (IsAny(option, "b", "forward", "go_forward", "approach"))
    {
        state.WorldState.SetFlag("james_school_forward", true);
        AdjustCounter("integrity", 1, 0, 10);
        AddBeat("At school start, James moved towards Betty despite pressure.");
        WriteLineC64("You move forward. It is late, but no longer evasive.");
        AdvanceScene(JamesRole);
        return true;
    }

    return false;
}

bool HandleJamesFinale(string option)
{
    if (IsAny(option, "a", "knock"))
    {
        state.WorldState.SetFlag("james_final_knock", true);
        AddBeat("James knocked at Betty's driveway in the rain.");
        WriteLineC64("You knock. Whatever follows now belongs to truth, not fantasy.");
        jamesEnding = DetermineJamesEnding();
        CompleteRole(JamesRole);
        return true;
    }

    if (IsAny(option, "b", "turn_away", "turn", "leave"))
    {
        state.WorldState.SetFlag("james_final_turn_away", true);
        AdjustCounter("cowardice", 2, 0, 10);
        AddBeat("James turned away from Betty's driveway.");
        WriteLineC64("You turn away before the door can answer.");
        jamesEnding = DetermineJamesEnding();
        CompleteRole(JamesRole);
        return true;
    }

    return false;
}

bool HandleAugustineChoice(string option)
{
    return GetScene(AugustineRole) switch
    {
        0 => HandleAugustinePrologue(option),
        1 => HandleAugustineGoldenHour(option),
        2 => HandleAugustineAfterthought(option),
        3 => HandleAugustineEvening(option),
        4 => HandleAugustineMeeting(option),
        5 => HandleAugustineBeach(option),
        _ => false
    };
}

bool HandleAugustinePrologue(string option)
{
    if (JamesSkippedTriangle() && IsAny(option, "a", "b", "c", "pick_up", "drive_past", "flirt_pickup"))
    {
        AdjustCounter("aug_hope", -1, 0, 10);
        AdjustCounter("aug_confusion", 1, 0, 10);
        AddBeat("Augustine's route stayed uncomplicated because James avoided the triangle path.");
        WriteLineC64("James never truly entered this summer arc with you. It fades into a brief possibility.");
        augustineEnding = "Neutral Ending (An Uncomplicated Summer)";
        CompleteRole(AugustineRole);
        return true;
    }

    if (IsAny(option, "a", "pick_up", "pickup", "standard"))
    {
        state.WorldState.SetFlag("aug_prologue_pickup", true);
        AddBeat("Augustine picked James up and leaned into possibility.");
        WriteLineC64("You pick him up. Hope feels bright and immediate.");
        AdvanceScene(AugustineRole);
        return true;
    }

    if (IsAny(option, "b", "drive_past", "past"))
    {
        state.WorldState.SetFlag("aug_prologue_drive_past", true);
        AdjustCounter("aug_hope", -1, 0, 10);
        AdjustCounter("aug_confusion", 1, 0, 10);
        AddBeat("Augustine chose to drive past and protect herself.");
        WriteLineC64("You drive past and keep your composure. The ache is quieter but still present.");
        augustineEnding = "Neutral Ending (An Uncomplicated Summer)";
        CompleteRole(AugustineRole);
        return true;
    }

    if (IsAny(option, "c", "flirt_pickup", "flirt"))
    {
        state.WorldState.SetFlag("aug_prologue_flirt_pickup", true);
        AdjustCounter("aug_hope", 1, 0, 10);
        AddBeat("Augustine picked James up with open flirtation.");
        WriteLineC64("You pick him up with playful confidence and a stronger claim on the moment.");
        AdvanceScene(AugustineRole);
        return true;
    }

    return false;
}

bool HandleAugustineGoldenHour(string option)
{
    if (IsAny(option, "a", "probe", "future"))
    {
        AdjustCounter("aug_hope", -1, 0, 10);
        AdjustCounter("aug_confusion", 1, 0, 10);
        AddBeat("Augustine probed the future and felt James recoil.");
        WriteLineC64("You ask where this is going. His hesitation lands like a crack in glass.");
        AdvanceScene(AugustineRole);
        return true;
    }

    if (IsAny(option, "b", "present", "immerse"))
    {
        AdjustCounter("aug_confusion", 1, 0, 10);
        AddBeat("Augustine tried to stay in the present despite unease.");
        WriteLineC64("You choose the present tense and try to ignore what does not fit.");
        AdvanceScene(AugustineRole);
        return true;
    }

    if (IsAny(option, "c", "integrate", "bonfire"))
    {
        AdjustCounter("aug_hope", -2, 0, 10);
        AdjustCounter("aug_confusion", 2, 0, 10);
        AddBeat("Augustine attempted to integrate James into her wider life and met distance.");
        WriteLineC64("You invite him closer to your real life. He retreats.");
        AdvanceScene(AugustineRole);
        return true;
    }

    return false;
}

bool HandleAugustineAfterthought(string option)
{
    if (IsAny(option, "a", "challenge"))
    {
        AdjustCounter("aug_hope", -1, 0, 10);
        AdjustCounter("aug_confusion", 2, 0, 10);
        AddBeat("Augustine challenged James after Inez's look.");
        WriteLineC64("You challenge him directly. The answers do not match his face.");
        AdvanceScene(AugustineRole);
        return true;
    }

    if (IsAny(option, "b", "dismiss", "drama_queen"))
    {
        AdjustCounter("aug_hope", 1, 0, 10);
        AdjustCounter("aug_confusion", -1, 0, 10);
        AddBeat("Augustine tried to dismiss warning signs and preserve the illusion.");
        WriteLineC64("You dismiss it as noise and hold onto the golden version a little longer.");
        AdvanceScene(AugustineRole);
        return true;
    }

    if (IsAny(option, "c", "frustrated", "frustration"))
    {
        AdjustCounter("aug_hope", -1, 0, 10);
        AdjustCounter("aug_confusion", 1, 0, 10);
        AddBeat("Augustine vented frustration at James's evasiveness.");
        WriteLineC64("You snap at his evasiveness. Tension replaces tenderness.");
        AdvanceScene(AugustineRole);
        return true;
    }

    return false;
}

bool HandleAugustineEvening(string option)
{
    if (IsAny(option, "a", "confront"))
    {
        AdjustCounter("aug_confusion", 1, 0, 10);
        AddBeat("Augustine confronted James about rumours as evening closed in.");
        WriteLineC64("You confront the rumours head-on and ask for clarity.");
        AdvanceScene(AugustineRole);
        return true;
    }

    if (IsAny(option, "b", "ignore", "win_him_back"))
    {
        AdjustCounter("aug_hope", 1, 0, 10);
        AdjustCounter("aug_confusion", 1, 0, 10);
        AddBeat("Augustine tried to out-run rumours by recreating early magic.");
        WriteLineC64("You try harder and call it strategy, though doubt keeps pace.");
        AdvanceScene(AugustineRole);
        return true;
    }

    if (IsAny(option, "c", "distance", "pull_back"))
    {
        AdjustCounter("aug_hope", -1, 0, 10);
        AdjustCounter("aug_confusion", 1, 0, 10);
        AdjustCounter("aug_heartbreak", 1, 0, 12);
        AddBeat("Augustine distanced herself while rumours hardened.");
        WriteLineC64("You pull back to protect yourself, but the hurt still follows.");
        AdvanceScene(AugustineRole);
        return true;
    }

    return false;
}

bool HandleAugustineMeeting(string option)
{
    if (IsAny(option, "a", "hope", "cling_hope"))
    {
        AdjustCounter("aug_hope", 1, 0, 10);
        AdjustCounter("aug_confusion", -1, 0, 10);
        AddBeat("Augustine went to the mall meeting clinging to hope.");
        WriteLineC64("You go believing this is where he finally chooses clearly.");
        AdvanceScene(AugustineRole);
        return true;
    }

    if (IsAny(option, "b", "suspicious", "suspicion"))
    {
        AdjustCounter("aug_confusion", 1, 0, 10);
        AdjustCounter("aug_heartbreak", 1, 0, 12);
        AddBeat("Augustine went to the mall meeting with growing suspicion.");
        WriteLineC64("You go with dread already in your chest.");
        AdvanceScene(AugustineRole);
        return true;
    }

    if (IsAny(option, "c", "cancel"))
    {
        AdjustCounter("aug_hope", -3, 0, 10);
        AdjustCounter("aug_confusion", 2, 0, 10);
        AdjustCounter("aug_heartbreak", 3, 0, 12);
        AddBeat("Augustine cancelled the mall meeting and sat with unanswered loss.");
        WriteLineC64("You cancel. The conversation never happens, but the pain does.");
        AdvanceScene(AugustineRole);
        return true;
    }

    return false;
}

bool HandleAugustineBeach(string option)
{
    if (IsAny(option, "a", "demand", "direct_answer"))
    {
        AddBeat("Augustine demanded a direct answer at the beach.");
        WriteLineC64("You demand a direct answer and refuse to leave it vague.");
    }
    else if (IsAny(option, "b", "recreate", "magic"))
    {
        AdjustCounter("aug_hope", 1, 0, 10);
        AdjustCounter("aug_confusion", -1, 0, 10);
        AddBeat("Augustine tried to recreate intimacy instead of forcing explicit truth.");
        WriteLineC64("You try to pull the moment back to what it used to be.");
    }
    else if (IsAny(option, "c", "confess", "heart_bare"))
    {
        AdjustCounter("aug_hope", 1, 0, 10);
        AdjustCounter("aug_confusion", -1, 0, 10);
        AddBeat("Augustine laid her heart bare on the beach.");
        WriteLineC64("You confess fully and wait for him to meet you there.");
    }
    else
    {
        return false;
    }

    ResolveAugustineJamesAnswer();
    augustineEnding = DetermineAugustineEnding();
    CompleteRole(AugustineRole);
    return true;
}

void ResolveAugustineJamesAnswer()
{
    if (state.WorldState.GetFlag("james_beach_lie"))
    {
        AdjustCounter("aug_hope", -5, 0, 10);
        AdjustCounter("aug_heartbreak", 5, 0, 12);
        AdjustCounter("aug_confusion", 3, 0, 10);
        AddBeat("Augustine perceived James's 'I love you' as a lie.");
        WriteLineC64("James says the words, but they feel false. The fallout is brutal.");
        return;
    }

    if (state.WorldState.GetFlag("james_beach_truth"))
    {
        AdjustCounter("aug_hope", -4, 0, 10);
        AdjustCounter("aug_heartbreak", 4, 0, 12);
        AdjustCounter("aug_confusion", 2, 0, 10);
        AddBeat("James gave Augustine the raw truth about summer's limit.");
        WriteLineC64("James tells the truth: it was summer, not forever.");
        return;
    }

    if (state.WorldState.GetFlag("james_beach_avoid"))
    {
        AdjustCounter("aug_hope", -3, 0, 10);
        AdjustCounter("aug_heartbreak", 3, 0, 12);
        AdjustCounter("aug_confusion", 3, 0, 10);
        AddBeat("James evaded Augustine's question and left her with partial ruin.");
        WriteLineC64("James avoids a direct answer. It hurts and clarifies nothing.");
        return;
    }

    if (state.WorldState.GetFlag("james_beach_kiss"))
    {
        AdjustCounter("aug_hope", -4, 0, 10);
        AdjustCounter("aug_heartbreak", 4, 0, 12);
        AdjustCounter("aug_confusion", 2, 0, 10);
        AddBeat("James answered Augustine with a kiss instead of truth.");
        WriteLineC64("James answers with a kiss. The ambiguity feels worse than a no.");
        return;
    }

    AdjustCounter("aug_hope", -4, 0, 10);
    AdjustCounter("aug_heartbreak", 4, 0, 12);
    AdjustCounter("aug_confusion", 2, 0, 10);
    AddBeat("Augustine received silence where certainty was needed.");
    WriteLineC64("James gives no clear answer. Silence becomes the verdict.");
}

bool HandleBettyChoice(string option)
{
    return GetScene(BettyRole) switch
    {
        0 => HandleBettyEarlySummer(option),
        1 => HandleBettyUnexpectedSight(option),
        2 => HandleBettyInezSecond(option),
        3 => HandleBettyConversation(option),
        4 => HandleBettyDriveway(option),
        _ => false
    };
}

bool HandleBettyEarlySummer(string option)
{
    if (JamesSkippedTriangle() && IsAny(option, "a", "b", "c", "reach", "ignore", "seek_inez"))
    {
        AdjustCounter("trust", 2, 0, 10);
        AdjustCounter("b_heartbreak", -1, 0, 12);
        AdjustCounter("resolve", 1, 0, 10);
        AddBeat("Betty reached a peaceful summer ending because the triangle never fully formed.");
        WriteLineC64("James came back before the triangle became a full betrayal. The summer stays bruised, not broken.");
        bettyEnding = "Neutral Ending (Peaceful Summer)";
        CompleteRole(BettyRole);
        return true;
    }

    if (IsAny(option, "a", "reach", "reach_james"))
    {
        AdjustCounter("trust", -1, 0, 10);
        AdjustCounter("b_heartbreak", 1, 0, 12);
        AddBeat("Betty tried to reach James and got evasive replies.");
        WriteLineC64("You reach out. The answers are short and evasive.");
        AdvanceScene(BettyRole);
        return true;
    }

    if (IsAny(option, "b", "ignore", "false_peace"))
    {
        AdjustCounter("b_heartbreak", 1, 0, 12);
        AddBeat("Betty attempted distraction and social composure.");
        WriteLineC64("You perform normality for everyone, including yourself.");
        AdvanceScene(BettyRole);
        return true;
    }

    if (IsAny(option, "c", "seek_inez", "inez"))
    {
        ApplyBettyInezFirstHintEffects();
        AddBeat("Betty sought Inez early and forced the first truth check.");
        WriteLineC64("You seek Inez and ask directly. The details cut through denial.");
        AdvanceScene(BettyRole);
        return true;
    }

    return false;
}

void ApplyBettyInezFirstHintEffects()
{
    if (state.WorldState.GetFlag("james_inez1_hide"))
    {
        AdjustCounter("trust", -3, 0, 10);
        AdjustCounter("b_heartbreak", 3, 0, 12);
        AdjustCounter("resolve", 1, 0, 10);
        return;
    }

    if (state.WorldState.GetFlag("james_inez1_nod"))
    {
        AdjustCounter("trust", -2, 0, 10);
        AdjustCounter("b_heartbreak", 2, 0, 12);
        AdjustCounter("resolve", 1, 0, 10);
        return;
    }

    if (state.WorldState.GetFlag("james_inez1_jump"))
    {
        AdjustCounter("trust", -1, 0, 10);
        AdjustCounter("b_heartbreak", 1, 0, 12);
        AdjustCounter("resolve", 2, 0, 10);
        return;
    }

    if (state.WorldState.GetFlag("james_inez1_flirt"))
    {
        AdjustCounter("trust", -4, 0, 10);
        AdjustCounter("b_heartbreak", 4, 0, 12);
        AdjustCounter("resolve", 1, 0, 10);
        return;
    }

    AdjustCounter("resolve", 1, 0, 10);
}

bool HandleBettyUnexpectedSight(string option)
{
    bool jamesJumpedOutEarlier = state.WorldState.GetFlag("james_inez1_jump");

    if (IsAny(option, "a", "pass", "cycle_past"))
    {
        if (jamesJumpedOutEarlier)
        {
            AdjustCounter("trust", -1, 0, 10);
            AdjustCounter("resolve", 1, 0, 10);
            AdjustCounter("b_heartbreak", 1, 0, 12);
            AddBeat("Betty saw James alone and cycled past to preserve composure.");
        }
        else
        {
            AdjustCounter("b_heartbreak", 1, 0, 12);
            AddBeat("Betty saw James with Augustine and cycled past in pain.");
        }

        WriteLineC64("You pass quickly and keep your face steady, even as your chest tightens.");
        AdvanceScene(BettyRole);
        return true;
    }

    if (IsAny(option, "b", "lock_eyes", "stop"))
    {
        AdjustCounter("resolve", 1, 0, 10);
        AddBeat("Betty forced eye contact and refused to disappear from her own story.");
        WriteLineC64("You stop and lock eyes. Silence becomes a clear accusation.");
        AdvanceScene(BettyRole);
        return true;
    }

    return false;
}

bool HandleBettyInezSecond(string option)
{
    ApplyBettyInezSecondHintEffects();

    if (IsAny(option, "a", "dismiss", "gossip"))
    {
        AdjustCounter("trust", 1, 0, 10);
        AdjustCounter("resolve", -1, 0, 10);
        AddBeat("Betty dismissed Inez's second report as gossip, briefly preserving trust.");
        WriteLineC64("You dismiss Inez aloud, though the details still linger.");
        AdvanceScene(BettyRole);
        return true;
    }

    if (IsAny(option, "b", "details", "tell_me_everything"))
    {
        AdjustCounter("b_heartbreak", 2, 0, 12);
        AdjustCounter("resolve", 2, 0, 10);
        AddBeat("Betty demanded full details from Inez.");
        WriteLineC64("You ask for every detail. Pain sharpens into clarity.");
        AdvanceScene(BettyRole);
        return true;
    }

    if (IsAny(option, "c", "angry", "angry_at_inez"))
    {
        AdjustCounter("b_heartbreak", 1, 0, 12);
        AdjustCounter("trust", -1, 0, 10);
        AddBeat("Betty redirected pain into anger at Inez.");
        WriteLineC64("You snap at Inez. The anger is real, but it does not dissolve the facts.");
        AdvanceScene(BettyRole);
        return true;
    }

    return false;
}

void ApplyBettyInezSecondHintEffects()
{
    if (state.WorldState.GetFlag("james_inez2_hold"))
    {
        AdjustCounter("trust", -3, 0, 10);
        AdjustCounter("b_heartbreak", 4, 0, 12);
        AdjustCounter("resolve", 2, 0, 10);
        return;
    }

    if (state.WorldState.GetFlag("james_inez2_pull"))
    {
        AdjustCounter("trust", -2, 0, 10);
        AdjustCounter("b_heartbreak", 2, 0, 12);
        AdjustCounter("resolve", 1, 0, 10);
        return;
    }

    if (state.WorldState.GetFlag("james_inez2_confront"))
    {
        AdjustCounter("trust", -3, 0, 10);
        AdjustCounter("b_heartbreak", 3, 0, 12);
        AdjustCounter("resolve", 2, 0, 10);
        return;
    }

    if (state.WorldState.GetFlag("james_inez2_kiss"))
    {
        AdjustCounter("trust", -4, 0, 10);
        AdjustCounter("b_heartbreak", 5, 0, 12);
        AdjustCounter("resolve", 3, 0, 10);
        return;
    }

    AdjustCounter("trust", 1, 0, 10);
    AdjustCounter("b_heartbreak", -1, 0, 12);
}

bool HandleBettyConversation(string option)
{
    if (IsAny(option, "a", "rationalise", "rationalise_it"))
    {
        AdjustCounter("resolve", -1, 0, 10);
        AdjustCounter("trust", 1, 0, 10);
        AddBeat("Betty attempted rationalisation to reduce immediate pain.");
        WriteLineC64("You rationalise it as youth and summer chaos. It soothes little.");
        AdvanceScene(BettyRole);
        return true;
    }

    if (IsAny(option, "b", "anger", "let_anger_grow"))
    {
        AdjustCounter("b_heartbreak", 1, 0, 12);
        AdjustCounter("resolve", 1, 0, 10);
        AddBeat("Betty let anger harden into resolve.");
        WriteLineC64("You let anger harden your boundary.");
        AdvanceScene(BettyRole);
        return true;
    }

    if (IsAny(option, "c", "talk", "try_to_talk", "last_chance"))
    {
        AdjustCounter("resolve", 1, 0, 10);
        AddBeat("Betty gave James one last chance to speak honestly.");
        WriteLineC64("You choose one last attempt at truth.");
        ApplyJamesVisitEffectsOnBetty();
        AdvanceScene(BettyRole);
        return true;
    }

    return false;
}

void ApplyJamesVisitEffectsOnBetty()
{
    if (state.WorldState.GetFlag("james_inez2_kiss") || state.WorldState.GetFlag("james_beach_lie"))
    {
        AdjustCounter("trust", -2, 0, 10);
        AdjustCounter("b_heartbreak", 2, 0, 12);
        WriteLineC64("James arrives looking wrecked, but the pattern of betrayal is hard to ignore.");
        return;
    }

    if (state.WorldState.GetFlag("james_beach_truth"))
    {
        AdjustCounter("trust", -1, 0, 10);
        AdjustCounter("b_heartbreak", 1, 0, 12);
        AdjustCounter("resolve", 1, 0, 10);
        WriteLineC64("James arrives with painful honesty rather than pure evasion.");
        return;
    }

    AdjustCounter("trust", -1, 0, 10);
    AdjustCounter("b_heartbreak", 1, 0, 12);
    WriteLineC64("James arrives uncertain and late, without enough clarity.");
}

bool HandleBettyDriveway(string option)
{
    if (IsAny(option, "a", "open"))
    {
        state.WorldState.SetFlag("betty_opened_door", true);
        state.WorldState.SetFlag("betty_closed_door", false);
        state.WorldState.SetFlag("betty_confronted_publicly", false);
        AddBeat("Betty opened the door.");
        WriteLineC64("You open the door. Forgiveness is not guaranteed, but conversation is possible.");
        bettyEnding = DetermineBettyEnding("open");
        CompleteRole(BettyRole);
        return true;
    }

    if (IsAny(option, "b", "close"))
    {
        state.WorldState.SetFlag("betty_opened_door", false);
        state.WorldState.SetFlag("betty_closed_door", true);
        state.WorldState.SetFlag("betty_confronted_publicly", false);
        AddBeat("Betty closed the door and ended the conversation.");
        WriteLineC64("You close the door and choose distance.");
        bettyEnding = DetermineBettyEnding("close");
        CompleteRole(BettyRole);
        return true;
    }

    if (IsAny(option, "c", "avoid", "back_way"))
    {
        state.WorldState.SetFlag("betty_opened_door", false);
        state.WorldState.SetFlag("betty_closed_door", true);
        state.WorldState.SetFlag("betty_confronted_publicly", false);
        AddBeat("Betty avoided the door and left through the back.");
        WriteLineC64("You avoid the door entirely. Silence becomes your ending.");
        bettyEnding = DetermineBettyEnding("avoid");
        CompleteRole(BettyRole);
        return true;
    }

    if (IsAny(option, "d", "confront", "public"))
    {
        state.WorldState.SetFlag("betty_opened_door", false);
        state.WorldState.SetFlag("betty_closed_door", false);
        state.WorldState.SetFlag("betty_confronted_publicly", true);
        AddBeat("Betty confronted James publicly and reclaimed the narrative.");
        WriteLineC64("You step outside and confront him publicly. No more private evasions.");
        bettyEnding = DetermineBettyEnding("confront");
        CompleteRole(BettyRole);
        return true;
    }

    return false;
}

string DetermineJamesEnding()
{
    int integrity = state.WorldState.GetCounter("integrity");
    int cowardice = state.WorldState.GetCounter("cowardice");
    int heartbreak = state.WorldState.GetCounter("j_heartbreak");

    if (state.WorldState.GetFlag("james_declined_ride"))
        return "Neutral Ending (The Shortcut)";

    if (state.WorldState.GetFlag("james_drove_away") || state.WorldState.GetFlag("james_final_turn_away"))
        return "Bad Ending (The Exile)";

    if (state.WorldState.GetFlag("james_final_knock")
        && state.WorldState.GetFlag("james_beach_truth")
        && integrity >= cowardice)
        return "Good Ending (Canonical)";

    if (heartbreak >= 8)
        return "Sad Ending (The Bitter Truth)";

    return cowardice >= integrity
        ? "Bad Ending (The Exile)"
        : "Mixed Ending (Unsteady Autumn)";
}

string DetermineAugustineEnding()
{
    int hope = state.WorldState.GetCounter("aug_hope");
    int confusion = state.WorldState.GetCounter("aug_confusion");
    int heartbreak = state.WorldState.GetCounter("aug_heartbreak");

    if (state.WorldState.GetFlag("aug_prologue_drive_past") || JamesSkippedTriangle())
        return "Neutral Ending (An Uncomplicated Summer)";

    if (hope <= 2 && heartbreak >= 6 && confusion >= 5)
        return "Heartbreak Ending (The Truth Revealed)";

    if (hope >= 6 && heartbreak <= 5 && confusion <= 4)
        return "Sad Ending (Clinging to the Illusion)";

    if (hope <= 3 && heartbreak >= 6 && confusion <= 4)
        return "Bitter Ending (The Scorned Lover)";

    return "Rain Ending (Bittersweet Clarity)";
}

string DetermineBettyEnding(string doorwayChoice)
{
    int trust = state.WorldState.GetCounter("trust");
    int heartbreak = state.WorldState.GetCounter("b_heartbreak");
    int resolve = state.WorldState.GetCounter("resolve");

    if (JamesSkippedTriangle())
        return "Neutral Ending (Peaceful Summer)";

    if (doorwayChoice.TextCompare("confront"))
        return "Fiery Ending (The Unbreakable Spirit)";

    if (doorwayChoice.TextCompare("close") || doorwayChoice.TextCompare("avoid"))
        return "Sad Ending (The Unmended Tear)";

    if (doorwayChoice.TextCompare("open")
        && trust >= 4
        && heartbreak <= 6
        && resolve >= 5
        && state.WorldState.GetFlag("james_beach_truth"))
        return "Good Ending (The Healing Cardigan)";

    if (resolve >= 6 && heartbreak >= 7)
        return "Fiery Ending (The Unbreakable Spirit)";

    return "Open Door, Uncertain";
}

void WriteRoleSummary(string role)
{
    WriteLineC64();

    if (role.TextCompare(JamesRole))
    {
        if (string.IsNullOrWhiteSpace(jamesEnding) || jamesEnding.TextCompare("Unresolved"))
            jamesEnding = DetermineJamesEnding();

        WriteLineC64($"James arc complete: {jamesEnding}");
        return;
    }

    if (role.TextCompare(AugustineRole))
    {
        if (string.IsNullOrWhiteSpace(augustineEnding) || augustineEnding.TextCompare("Unresolved"))
            augustineEnding = DetermineAugustineEnding();

        WriteLineC64($"Augustine arc complete: {augustineEnding}");
        return;
    }

    if (string.IsNullOrWhiteSpace(bettyEnding) || bettyEnding.TextCompare("Unresolved"))
        bettyEnding = DetermineBettyEnding("open");

    WriteLineC64($"Betty arc complete: {bettyEnding}");
}

void RenderFinalOutcome()
{
    if (jamesEnding.TextCompare("Unresolved"))
        jamesEnding = DetermineJamesEnding();
    if (augustineEnding.TextCompare("Unresolved"))
        augustineEnding = DetermineAugustineEnding();
    if (bettyEnding.TextCompare("Unresolved"))
        bettyEnding = DetermineBettyEnding("open");

    WriteLineC64();
    WriteLineC64("=== FINAL OUTCOME ===");
    WriteLineC64($"James: {jamesEnding}");
    WriteLineC64($"Augustine: {augustineEnding}");
    WriteLineC64($"Betty: {bettyEnding}");

    WriteLineC64();
    WriteLineC64("Scoreboard:");
    WriteLineC64($"James -> Integrity {state.WorldState.GetCounter("integrity")}, Cowardice {state.WorldState.GetCounter("cowardice")}, Heartbreak Caused {state.WorldState.GetCounter("j_heartbreak")}");
    WriteLineC64($"Augustine -> Hope {state.WorldState.GetCounter("aug_hope")}, Confusion {state.WorldState.GetCounter("aug_confusion")}, Heartbreak {state.WorldState.GetCounter("aug_heartbreak")}");
    WriteLineC64($"Betty -> Trust {state.WorldState.GetCounter("trust")}, Resolve {state.WorldState.GetCounter("resolve")}, Heartbreak {state.WorldState.GetCounter("b_heartbreak")}");

    PrintTimeline();

    WriteLineC64();
    WriteLineC64("=== END OF DEMO ===");
}

void PrintStatus()
{
    string role = CurrentRole();
    int scene = GetScene(role);
    (string title, _) = GetSceneInfo(role, scene);

    WriteLineC64($"Act: {actIndex + 1}/{actOrder.Length}");
    WriteLineC64($"Role: {role.ToProperCase()}");
    WriteLineC64($"Scene: {scene + 1} - {title}");
    WriteLineC64($"James -> Integrity {state.WorldState.GetCounter("integrity")}, Cowardice {state.WorldState.GetCounter("cowardice")}, Heartbreak {state.WorldState.GetCounter("j_heartbreak")}");
    WriteLineC64($"Augustine -> Hope {state.WorldState.GetCounter("aug_hope")}, Confusion {state.WorldState.GetCounter("aug_confusion")}, Heartbreak {state.WorldState.GetCounter("aug_heartbreak")}");
    WriteLineC64($"Betty -> Trust {state.WorldState.GetCounter("trust")}, Resolve {state.WorldState.GetCounter("resolve")}, Heartbreak {state.WorldState.GetCounter("b_heartbreak")}");
}

void PrintTimeline()
{
    WriteLineC64();
    WriteLineC64("Timeline:");

    if (state.WorldState.Timeline.Count == 0)
    {
        WriteLineC64("- (No events logged yet)");
        return;
    }

    foreach (string entry in state.WorldState.Timeline)
        WriteLineC64($"- {entry}");
}

void ShowHelpForCurrentRole()
{
    WriteLineC64("Commands: look, go <north/east/south/west>, talk <npc>, take <item>, inventory, status, role, scene, choices, timeline, help, quit");
    ShowContextualChoices();
}

void ShowContextualChoices()
{
    string role = CurrentRole();
    int scene = GetScene(role);

    if (role.TextCompare(JamesRole))
    {
        string line = scene switch
        {
            0 => "Choices: choose dumb | apologise | joke",
            1 => "Choices: choose direct | hesitate | decline | drive",
            2 => "Choices: choose hide | nod | jump | flirt",
            3 => "Choices: choose reply | lie | ignore",
            4 => "Choices: choose leave | pretend | greet",
            5 => "Choices: choose augustine | betty_lie | betty_explain",
            6 => "Choices: choose hold | pull | confront | kiss_demo",
            7 => "Choices: choose say_lie | truth | avoid | kiss",
            8 => "Choices: choose look_away | forward",
            9 => "Choices: choose knock | turn_away",
            _ => "No further choices in this act."
        };

        WriteLineC64(line);
        return;
    }

    if (role.TextCompare(AugustineRole))
    {
        string line = scene switch
        {
            0 => "Choices: choose pick_up | drive_past | flirt_pickup",
            1 => "Choices: choose probe | present | integrate",
            2 => "Choices: choose challenge | dismiss | frustrated",
            3 => "Choices: choose confront | ignore | distance",
            4 => "Choices: choose hope | suspicious | cancel",
            5 => "Choices: choose demand | recreate | confess",
            _ => "No further choices in this act."
        };

        WriteLineC64(line);
        return;
    }

    string bettyLine = scene switch
    {
        0 => "Choices: choose reach | ignore | seek_inez",
        1 => "Choices: choose pass | lock_eyes",
        2 => "Choices: choose dismiss | details | angry",
        3 => "Choices: choose rationalise | anger | talk",
        4 => "Choices: choose open | close | avoid | confront",
        _ => "No further choices in this act."
    };

    WriteLineC64(bettyLine);
}

bool JamesSkippedTriangle()
{
    return state.WorldState.GetFlag("james_declined_ride") || state.WorldState.GetFlag("james_drove_away");
}

bool IsAny(string option, params string[] aliases)
{
    foreach (string alias in aliases)
    {
        if (option.TextCompare(alias))
            return true;
    }

    return false;
}

void AddBeat(string entry)
{
    state.WorldState.AddTimeline(entry);
}

void AdjustCounter(string key, int delta, int min, int max)
{
    int current = state.WorldState.GetCounter(key);
    int target = (current + delta).Clamp(min, max);
    int change = target - current;

    if (change != 0)
        state.WorldState.Increment(key, change);
}
