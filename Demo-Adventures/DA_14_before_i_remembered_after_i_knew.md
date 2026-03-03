# Demo Adventure 14: Before I Remembered, After I Knew

Modernised to DSL v2-first format. DSL carries world/story structure; C# is only a thin runner.

## Premise
Now: you sit in your kitchen. Then: you were in the cellar. Now again: but your shoes are still wet with something that is not water.

## Story Beats
1) The disturbance arrives and feels personal.
2) A rule is broken or a boundary is crossed.
3) A clue reveals the scale of the problem.
4) A choice narrows the world.
5) The environment answers back.
6) A truth is forced into view.
7) A price is paid, willingly or not.
8) The ending leaves a lingering echo.
9) A misleading clue wastes precious time but reveals character tension.
10) A quiet environmental change signals that the danger has moved closer.

## ASCII Map
```
          N
    W           E
          S

┌────────────┐
│  Memory    │
└─────┬──────┘
      │
      │
┌────────────┐
│ Present    │
│    P       │
└─────┬──────┘
      │ (one-way)
      ▼
┌────────────┐
│  Return    │
└────────────┘

P = Photograph
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
world: Before I Remembered, After I Knew
goal: Reach the final turn with enough context to act decisively.
current_location: start

// --- DSL v2 entity definitions ---
define item: before_i_remembered_after_i_knew_note | Case Note | A folded note containing partial context and names.
define item: before_i_remembered_after_i_knew_decoy | Red Herring | A convincing but ultimately irrelevant object.
define item: before_i_remembered_after_i_knew_token | Proof Token | A concrete token that confirms what is really happening.
define npc: before_i_remembered_after_i_knew_watcher | Watcher | A tense observer who reacts to your choices.

// --- World layout ---
location: start | The disturbance arrives and feels personal.
  item: lantern | hand lantern| A hand lantern with a faint, wavering glow. | takeable=false | aliases=hand,lantern
  item: clock | wall clock| A wall clock ticking half a beat too slowly. | takeable=false | aliases=wall,clock
  item: before_i_remembered_after_i_knew_note | case note| A folded note containing partial context and names. | aliases=case,note
  exit: north -> crossroads
  exit: east -> sidetrack

location: crossroads | A rule is broken or a boundary is crossed.
  npc: before_i_remembered_after_i_knew_watcher
  item: railing | iron railing| Cold iron slick with condensation. | takeable=false | aliases=iron,railing
  exit: south -> start
  exit: north -> finale

location: sidetrack | A clue reveals the scale of the problem.
  item: before_i_remembered_after_i_knew_decoy | red herring| A convincing but ultimately irrelevant object. | aliases=red,herring
  item: cabinet | utility cabinet| A paint-chipped cabinet full of mundane supplies. | takeable=false | aliases=utility,cabinet
  exit: west -> start

location: finale | A choice narrows the world.
  item: before_i_remembered_after_i_knew_token | proof token| A concrete token that confirms what is really happening. | aliases=proof,token
  exit: south -> crossroads

// --- Start state ---
start_inventory: before_i_remembered_after_i_knew_note
start_stats: health=100 | resolve=50 | trust=0
flag: alarm_heard=false
counter: tension=0
relationship: before_i_remembered_after_i_knew_watcher=0

// --- DSL v2 quests/rules/triggers ---
quest: truth_path | Follow the Truth Path | Gather context and reach the final location.
quest_stage: truth_path | stage_1 | required=start | required=crossroads | required=finale
quest_objective: truth_path | obj_1 | Track the signs | Read room cues and avoid false certainty
quest_on_complete: truth_path | effects="message:You force clarity out of fear and confusion."

on_enter: crossroads | effects="message:The air tightens as if the corridor itself is listening."
on_enter: sidetrack | effects="message:A convincing detail appears useful, but leads nowhere."
on_pickup: before_i_remembered_after_i_knew_token | effects="message:This detail is real. Build your choice around it."

npc_acceptance_default: before_i_remembered_after_i_knew_watcher | text="The watcher says little, but notes every move."
npc_rule: before_i_remembered_after_i_knew_watcher | when=counter.tension>=2 | priority=1 | then="message:The watcher glances away, as if expecting impact."

room_desc: crossroads | A transition space where every sound seems delayed by a heartbeat.
room_desc_when: crossroads | flag:alarm_heard=true | text="A distant alarm threads through the dark like wire."

branch: sharp_end | when=has_item:before_i_remembered_after_i_knew_token | then="message:You reach the ending with evidence, not guesswork."
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
world: Before I Remembered, After I Knew
goal: Reach the final turn with enough context to act decisively.
current_location: start

// --- DSL v2 entity definitions ---
define item: before_i_remembered_after_i_knew_note | Case Note | A folded note containing partial context and names.
define item: before_i_remembered_after_i_knew_decoy | Red Herring | A convincing but ultimately irrelevant object.
define item: before_i_remembered_after_i_knew_token | Proof Token | A concrete token that confirms what is really happening.
define npc: before_i_remembered_after_i_knew_watcher | Watcher | A tense observer who reacts to your choices.

// --- World layout ---
location: start | The disturbance arrives and feels personal.
  item: lantern | hand lantern| A hand lantern with a faint, wavering glow. | takeable=false | aliases=hand,lantern
  item: clock | wall clock| A wall clock ticking half a beat too slowly. | takeable=false | aliases=wall,clock
  item: before_i_remembered_after_i_knew_note | case note| A folded note containing partial context and names. | aliases=case,note
  exit: north -> crossroads
  exit: east -> sidetrack

location: crossroads | A rule is broken or a boundary is crossed.
  npc: before_i_remembered_after_i_knew_watcher
  item: railing | iron railing| Cold iron slick with condensation. | takeable=false | aliases=iron,railing
  exit: south -> start
  exit: north -> finale

location: sidetrack | A clue reveals the scale of the problem.
  item: before_i_remembered_after_i_knew_decoy | red herring| A convincing but ultimately irrelevant object. | aliases=red,herring
  item: cabinet | utility cabinet| A paint-chipped cabinet full of mundane supplies. | takeable=false | aliases=utility,cabinet
  exit: west -> start

location: finale | A choice narrows the world.
  item: before_i_remembered_after_i_knew_token | proof token| A concrete token that confirms what is really happening. | aliases=proof,token
  exit: south -> crossroads

// --- Start state ---
start_inventory: before_i_remembered_after_i_knew_note
start_stats: health=100 | resolve=50 | trust=0
flag: alarm_heard=false
counter: tension=0
relationship: before_i_remembered_after_i_knew_watcher=0

// --- DSL v2 quests/rules/triggers ---
quest: truth_path | Follow the Truth Path | Gather context and reach the final location.
quest_stage: truth_path | stage_1 | required=start | required=crossroads | required=finale
quest_objective: truth_path | obj_1 | Track the signs | Read room cues and avoid false certainty
quest_on_complete: truth_path | effects="message:You force clarity out of fear and confusion."

on_enter: crossroads | effects="message:The air tightens as if the corridor itself is listening."
on_enter: sidetrack | effects="message:A convincing detail appears useful, but leads nowhere."
on_pickup: before_i_remembered_after_i_knew_token | effects="message:This detail is real. Build your choice around it."

npc_acceptance_default: before_i_remembered_after_i_knew_watcher | text="The watcher says little, but notes every move."
npc_rule: before_i_remembered_after_i_knew_watcher | when=counter.tension>=2 | priority=1 | then="message:The watcher glances away, as if expecting impact."

room_desc: crossroads | A transition space where every sound seems delayed by a heartbeat.
room_desc_when: crossroads | flag:alarm_heard=true | text="A distant alarm threads through the dark like wire."

branch: sharp_end | when=has_item:before_i_remembered_after_i_knew_token | then="message:You reach the ending with evidence, not guesswork."
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

SetupC64("Before I Remembered, After I Knew");
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
