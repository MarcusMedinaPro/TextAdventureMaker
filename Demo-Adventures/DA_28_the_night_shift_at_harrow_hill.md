# Demo Adventure 28: The Night Shift at Harrow Hill

Modernised to DSL v2-first format. DSL carries world/story structure; C# is only a thin runner.

## Premise
You are dropped into the night shift at harrow hill, where each decision pushes the night towards a harsher outcome.

## Story Beats
1) Arrive at the hospital for the night shift.
2) Begin patrol: reception, ward, morgue, office.
3) Notice the logbook entry is wrong - tonight's date is last week.
4) A figure appears in the ward. She says she's a nurse. She follows you.
5) Round 2: the ward has changed. Beds are occupied. They weren't before.
6) Round 3: the nurse speaks of patients who died here. All of them. Tonight.
7) The logbook now has your name. Time of death: 03:17.
8) Round 4: the morgue drawer is open. Your name is on the tag.
9) The nurse is gone. She was never here.
10) The phone rings. "Your shift is over."

## ASCII Map
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

## Red Herrings and Interactables
- Framed noticeboard with outdated warnings that seem important but are not.
- Locked drawer containing harmless paperwork and old receipts.
- Cracked mirror that offers flavour text and mood reactions only.
- Vending machine or cabinet that can be opened, inspected, and dismissed.
- Discarded personal item (ticket, keyring, glove) with no quest value.
- Window, vent, or hatch with detailed responses but no progression impact.
- Wall clock that can be checked repeatedly for creeping tension.
- Mundane furniture (chair, trolley, cabinet) with tactile descriptions.
- Readable leaflet or memo with lore hints and false leads.
- Ambient sound source that changes text over time without blocking progress.

## DSL v2 Adventure (Primary)
```adventure
world: The Night Shift at Harrow Hill
goal: Reach the final turn with enough context to act decisively.
current_location: start

// --- DSL v2 entity definitions ---
define item: the_night_shift_at_harrow_hill_note | Case Note | A folded note containing partial context and names.
define item: the_night_shift_at_harrow_hill_decoy | Red Herring | A convincing but ultimately irrelevant object.
define item: the_night_shift_at_harrow_hill_token | Proof Token | A concrete token that confirms what is really happening.
define npc: the_night_shift_at_harrow_hill_watcher | Watcher | A tense observer who reacts to your choices.

// --- World layout ---
location: start | Arrive at the hospital for the night shift.
  item: lantern | hand lantern| A hand lantern with a faint, wavering glow. | takeable=false | aliases=hand,lantern
  item: clock | wall clock| A wall clock ticking half a beat too slowly. | takeable=false | aliases=wall,clock
  item: the_night_shift_at_harrow_hill_note | case note| A folded note containing partial context and names. | aliases=case,note
  exit: north -> crossroads
  exit: east -> sidetrack

location: crossroads | Begin patrol: reception, ward, morgue, office.
  npc: the_night_shift_at_harrow_hill_watcher
  item: railing | iron railing| Cold iron slick with condensation. | takeable=false | aliases=iron,railing
  exit: south -> start
  exit: north -> finale

location: sidetrack | Notice the logbook entry is wrong - tonight's date is last week.
  item: the_night_shift_at_harrow_hill_decoy | red herring| A convincing but ultimately irrelevant object. | aliases=red,herring
  item: cabinet | utility cabinet| A paint-chipped cabinet full of mundane supplies. | takeable=false | aliases=utility,cabinet
  exit: west -> start

location: finale | A figure appears in the ward. She says she's a nurse. She follows you.
  item: the_night_shift_at_harrow_hill_token | proof token| A concrete token that confirms what is really happening. | aliases=proof,token
  exit: south -> crossroads

// --- Start state ---
start_inventory: the_night_shift_at_harrow_hill_note
start_stats: health=100 | resolve=50 | trust=0
flag: alarm_heard=false
counter: tension=0
relationship: the_night_shift_at_harrow_hill_watcher=0

// --- DSL v2 quests/rules/triggers ---
quest: truth_path | Follow the Truth Path | Gather context and reach the final location.
quest_stage: truth_path | stage_1 | required=start | required=crossroads | required=finale
quest_objective: truth_path | obj_1 | Track the signs | Read room cues and avoid false certainty
quest_on_complete: truth_path | effects="message:You force clarity out of fear and confusion."

on_enter: crossroads | effects="message:The air tightens as if the corridor itself is listening."
on_enter: sidetrack | effects="message:A convincing detail appears useful, but leads nowhere."
on_pickup: the_night_shift_at_harrow_hill_token | effects="message:This detail is real. Build your choice around it."

npc_acceptance_default: the_night_shift_at_harrow_hill_watcher | text="The watcher says little, but notes every move."
npc_rule: the_night_shift_at_harrow_hill_watcher | when=counter.tension>=2 | priority=1 | then="message:The watcher glances away, as if expecting impact."

room_desc: crossroads | A transition space where every sound seems delayed by a heartbeat.
room_desc_when: crossroads | flag:alarm_heard=true | text="A distant alarm threads through the dark like wire."

branch: sharp_end | when=has_item:the_night_shift_at_harrow_hill_token | then="message:You reach the ending with evidence, not guesswork."
chapter: chapter_1 | Rising Pressure | objectives=obj_1 | next=chapter_2
chapter: chapter_2 | Final Choice | objectives=obj_1 | is_ending=true

// --- Parser preferences ---
command_alias: examine=x
command_alias: inventory=i
command_alias: look=l
direction_alias: n=north
direction_alias: s=south
direction_alias: e=east
direction_alias: w=west
parser_option: fuzzy=true
parser_option: max_distance=1
```

## Minimal C# Runner
```csharp
using MarcusMedina.TextAdventure.Dsl;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

const string dsl = """
world: The Night Shift at Harrow Hill
goal: Reach the final turn with enough context to act decisively.
current_location: start

// --- DSL v2 entity definitions ---
define item: the_night_shift_at_harrow_hill_note | Case Note | A folded note containing partial context and names.
define item: the_night_shift_at_harrow_hill_decoy | Red Herring | A convincing but ultimately irrelevant object.
define item: the_night_shift_at_harrow_hill_token | Proof Token | A concrete token that confirms what is really happening.
define npc: the_night_shift_at_harrow_hill_watcher | Watcher | A tense observer who reacts to your choices.

// --- World layout ---
location: start | Arrive at the hospital for the night shift.
  item: lantern | hand lantern| A hand lantern with a faint, wavering glow. | takeable=false | aliases=hand,lantern
  item: clock | wall clock| A wall clock ticking half a beat too slowly. | takeable=false | aliases=wall,clock
  item: the_night_shift_at_harrow_hill_note | case note| A folded note containing partial context and names. | aliases=case,note
  exit: north -> crossroads
  exit: east -> sidetrack

location: crossroads | Begin patrol: reception, ward, morgue, office.
  npc: the_night_shift_at_harrow_hill_watcher
  item: railing | iron railing| Cold iron slick with condensation. | takeable=false | aliases=iron,railing
  exit: south -> start
  exit: north -> finale

location: sidetrack | Notice the logbook entry is wrong - tonight's date is last week.
  item: the_night_shift_at_harrow_hill_decoy | red herring| A convincing but ultimately irrelevant object. | aliases=red,herring
  item: cabinet | utility cabinet| A paint-chipped cabinet full of mundane supplies. | takeable=false | aliases=utility,cabinet
  exit: west -> start

location: finale | A figure appears in the ward. She says she's a nurse. She follows you.
  item: the_night_shift_at_harrow_hill_token | proof token| A concrete token that confirms what is really happening. | aliases=proof,token
  exit: south -> crossroads

// --- Start state ---
start_inventory: the_night_shift_at_harrow_hill_note
start_stats: health=100 | resolve=50 | trust=0
flag: alarm_heard=false
counter: tension=0
relationship: the_night_shift_at_harrow_hill_watcher=0

// --- DSL v2 quests/rules/triggers ---
quest: truth_path | Follow the Truth Path | Gather context and reach the final location.
quest_stage: truth_path | stage_1 | required=start | required=crossroads | required=finale
quest_objective: truth_path | obj_1 | Track the signs | Read room cues and avoid false certainty
quest_on_complete: truth_path | effects="message:You force clarity out of fear and confusion."

on_enter: crossroads | effects="message:The air tightens as if the corridor itself is listening."
on_enter: sidetrack | effects="message:A convincing detail appears useful, but leads nowhere."
on_pickup: the_night_shift_at_harrow_hill_token | effects="message:This detail is real. Build your choice around it."

npc_acceptance_default: the_night_shift_at_harrow_hill_watcher | text="The watcher says little, but notes every move."
npc_rule: the_night_shift_at_harrow_hill_watcher | when=counter.tension>=2 | priority=1 | then="message:The watcher glances away, as if expecting impact."

room_desc: crossroads | A transition space where every sound seems delayed by a heartbeat.
room_desc_when: crossroads | flag:alarm_heard=true | text="A distant alarm threads through the dark like wire."

branch: sharp_end | when=has_item:the_night_shift_at_harrow_hill_token | then="message:You reach the ending with evidence, not guesswork."
chapter: chapter_1 | Rising Pressure | objectives=obj_1 | next=chapter_2
chapter: chapter_2 | Final Choice | objectives=obj_1 | is_ending=true

// --- Parser preferences ---
command_alias: examine=x
command_alias: inventory=i
command_alias: look=l
direction_alias: n=north
direction_alias: s=south
direction_alias: e=east
direction_alias: w=west
parser_option: fuzzy=true
parser_option: max_distance=1
""";

DslV2Parser dslParser = new();
DslAdventure adventure = dslParser.ParseString(dsl);
GameState state = adventure.State;
KeywordParser parser = new(KeywordParserConfigBuilder.BritishDefaults().Build());

SetupC64("The Night Shift at Harrow Hill");
WriteLineC64($"=== {adventure.WorldName} ===");
WriteLineC64($"Goal: {adventure.Goal}");
state.ShowRoom();

while (true)
{
    WriteLineC64();
    WritePromptC64("> ");
    string? input = Console.ReadLine();
    if (input is null)
        break;

    string trimmed = input.Trim();
    if (string.IsNullOrWhiteSpace(trimmed))
        continue;

    var command = parser.Parse(trimmed);
    var result = state.Execute(command);
    state.DisplayResult(command, result);

    if (result.ShouldQuit)
        break;
}
```
