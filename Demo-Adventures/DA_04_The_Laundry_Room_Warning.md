# Demo Adventure 04: The Laundry Room Warning

Modernised to DSL v2-first format. DSL carries world/story structure; C# is only a thin runner.

## Premise
_Slice tag: Slice 50 - Linear staircase horror with delayed trigger, door gating, and heavy red-herring interaction density._

## Story Beats
1) You are a daughter in your room, studying homework.
2) The bedroom door stays effectively closed while dinner is supposedly being prepared.
3) You can inspect, read, open, and close many irrelevant objects (red herrings).
4) After enough study time, a voice downstairs calls you to dinner.
5) You head out to the landing and approach the stairs.
6) Before you get downstairs, hands drag you into the laundry room.
7) It is your real mother, shaking and tearful.
8) She says: "Don't go down there, honey. I heard it too."

## ASCII Map
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

## Red Herrings and Interactables
- Framed noticeboard with outdated notices and dead-end hints.
- Cracked mirror that only returns atmospheric flavour text.
- Locked drawer containing harmless paperwork and receipts.
- Vending cabinet that opens but offers no progression items.
- Wall clock that can be checked for creeping time pressure.
- Window or vent with rich description and no route unlock.

## DSL v2 Adventure (Primary)
```adventure
world: The Laundry Room Warning
goal: Reach the final turn with enough context to act decisively.
current_location: start

// --- DSL v2 entity definitions ---
define item: the_laundry_room_warning_note | Case Note | A folded note containing partial context and names.
define item: the_laundry_room_warning_decoy | Red Herring | A convincing but ultimately irrelevant object.
define item: the_laundry_room_warning_token | Proof Token | A concrete token that confirms what is really happening.
define npc: the_laundry_room_warning_watcher | Watcher | A tense observer who reacts to your choices.

// --- World layout ---
location: start | You are a daughter in your room, studying homework.
  item: lantern | hand lantern| A hand lantern with a faint, wavering glow. | takeable=false | aliases=hand,lantern
  item: clock | wall clock| A wall clock ticking half a beat too slowly. | takeable=false | aliases=wall,clock
  item: the_laundry_room_warning_note | case note| A folded note containing partial context and names. | aliases=case,note
  exit: north -> crossroads
  exit: east -> sidetrack

location: crossroads | The bedroom door stays effectively closed while dinner is supposedly being prepared.
  npc: the_laundry_room_warning_watcher
  item: railing | iron railing| Cold iron slick with condensation. | takeable=false | aliases=iron,railing
  exit: south -> start
  exit: north -> finale

location: sidetrack | You can inspect, read, open, and close many irrelevant objects (red herrings).
  item: the_laundry_room_warning_decoy | red herring| A convincing but ultimately irrelevant object. | aliases=red,herring
  item: cabinet | utility cabinet| A paint-chipped cabinet full of mundane supplies. | takeable=false | aliases=utility,cabinet
  exit: west -> start

location: finale | After enough study time, a voice downstairs calls you to dinner.
  item: the_laundry_room_warning_token | proof token| A concrete token that confirms what is really happening. | aliases=proof,token
  exit: south -> crossroads

// --- Start state ---
start_inventory: the_laundry_room_warning_note
start_stats: health=100 | resolve=50 | trust=0
flag: alarm_heard=false
counter: tension=0
relationship: the_laundry_room_warning_watcher=0

// --- DSL v2 quests/rules/triggers ---
quest: truth_path | Follow the Truth Path | Gather context and reach the final location.
quest_stage: truth_path | stage_1 | required=start | required=crossroads | required=finale
quest_objective: truth_path | obj_1 | Track the signs | Read room cues and avoid false certainty
quest_on_complete: truth_path | effects="message:You force clarity out of fear and confusion."

on_enter: crossroads | effects="message:The air tightens as if the corridor itself is listening."
on_enter: sidetrack | effects="message:A convincing detail appears useful, but leads nowhere."
on_pickup: the_laundry_room_warning_token | effects="message:This detail is real. Build your choice around it."

npc_acceptance_default: the_laundry_room_warning_watcher | text="The watcher says little, but notes every move."
npc_rule: the_laundry_room_warning_watcher | when=counter.tension>=2 | priority=1 | then="message:The watcher glances away, as if expecting impact."

room_desc: crossroads | A transition space where every sound seems delayed by a heartbeat.
room_desc_when: crossroads | flag:alarm_heard=true | text="A distant alarm threads through the dark like wire."

branch: sharp_end | when=has_item:the_laundry_room_warning_token | then="message:You reach the ending with evidence, not guesswork."
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
world: The Laundry Room Warning
goal: Reach the final turn with enough context to act decisively.
current_location: start

// --- DSL v2 entity definitions ---
define item: the_laundry_room_warning_note | Case Note | A folded note containing partial context and names.
define item: the_laundry_room_warning_decoy | Red Herring | A convincing but ultimately irrelevant object.
define item: the_laundry_room_warning_token | Proof Token | A concrete token that confirms what is really happening.
define npc: the_laundry_room_warning_watcher | Watcher | A tense observer who reacts to your choices.

// --- World layout ---
location: start | You are a daughter in your room, studying homework.
  item: lantern | hand lantern| A hand lantern with a faint, wavering glow. | takeable=false | aliases=hand,lantern
  item: clock | wall clock| A wall clock ticking half a beat too slowly. | takeable=false | aliases=wall,clock
  item: the_laundry_room_warning_note | case note| A folded note containing partial context and names. | aliases=case,note
  exit: north -> crossroads
  exit: east -> sidetrack

location: crossroads | The bedroom door stays effectively closed while dinner is supposedly being prepared.
  npc: the_laundry_room_warning_watcher
  item: railing | iron railing| Cold iron slick with condensation. | takeable=false | aliases=iron,railing
  exit: south -> start
  exit: north -> finale

location: sidetrack | You can inspect, read, open, and close many irrelevant objects (red herrings).
  item: the_laundry_room_warning_decoy | red herring| A convincing but ultimately irrelevant object. | aliases=red,herring
  item: cabinet | utility cabinet| A paint-chipped cabinet full of mundane supplies. | takeable=false | aliases=utility,cabinet
  exit: west -> start

location: finale | After enough study time, a voice downstairs calls you to dinner.
  item: the_laundry_room_warning_token | proof token| A concrete token that confirms what is really happening. | aliases=proof,token
  exit: south -> crossroads

// --- Start state ---
start_inventory: the_laundry_room_warning_note
start_stats: health=100 | resolve=50 | trust=0
flag: alarm_heard=false
counter: tension=0
relationship: the_laundry_room_warning_watcher=0

// --- DSL v2 quests/rules/triggers ---
quest: truth_path | Follow the Truth Path | Gather context and reach the final location.
quest_stage: truth_path | stage_1 | required=start | required=crossroads | required=finale
quest_objective: truth_path | obj_1 | Track the signs | Read room cues and avoid false certainty
quest_on_complete: truth_path | effects="message:You force clarity out of fear and confusion."

on_enter: crossroads | effects="message:The air tightens as if the corridor itself is listening."
on_enter: sidetrack | effects="message:A convincing detail appears useful, but leads nowhere."
on_pickup: the_laundry_room_warning_token | effects="message:This detail is real. Build your choice around it."

npc_acceptance_default: the_laundry_room_warning_watcher | text="The watcher says little, but notes every move."
npc_rule: the_laundry_room_warning_watcher | when=counter.tension>=2 | priority=1 | then="message:The watcher glances away, as if expecting impact."

room_desc: crossroads | A transition space where every sound seems delayed by a heartbeat.
room_desc_when: crossroads | flag:alarm_heard=true | text="A distant alarm threads through the dark like wire."

branch: sharp_end | when=has_item:the_laundry_room_warning_token | then="message:You reach the ending with evidence, not guesswork."
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

SetupC64("The Laundry Room Warning");
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
