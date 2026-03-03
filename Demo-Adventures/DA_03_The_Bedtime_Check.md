# Demo Adventure 03: The Bedtime Check

Modernised to DSL v2-first format. DSL carries world/story structure; C# is only a thin runner.

## Premise
_Slice tag: Slice 49 - Linear domestic horror with state-gated progression, object state toggles, and false-path side interactions._

## Story Beats
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

## ASCII Map
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

## Red Herrings and Interactables
- Framed noticeboard with outdated notices and dead-end hints.
- Cracked mirror that only returns atmospheric flavour text.
- Locked drawer containing harmless paperwork and receipts.
- Vending cabinet that opens but offers no progression items.
- Wall clock that can be checked for creeping time pressure.
- Window or vent with rich description and no route unlock.

## DSL v2 Adventure (Primary)
```adventure
world: The Bedtime Check
goal: Reach the final turn with enough context to act decisively.
current_location: start

// --- DSL v2 entity definitions ---
define item: the_bedtime_check_note | Case Note | A folded note containing partial context and names.
define item: the_bedtime_check_decoy | Red Herring | A convincing but ultimately irrelevant object.
define item: the_bedtime_check_token | Proof Token | A concrete token that confirms what is really happening.
define npc: the_bedtime_check_watcher | Watcher | A tense observer who reacts to your choices.

// --- World layout ---
location: start | Father and son sit together in the living room before bedtime.
  item: lantern | hand lantern| A hand lantern with a faint, wavering glow. | takeable=false | aliases=hand,lantern
  item: clock | wall clock| A wall clock ticking half a beat too slowly. | takeable=false | aliases=wall,clock
  item: the_bedtime_check_note | case note| A folded note containing partial context and names. | aliases=case,note
  exit: north -> crossroads
  exit: east -> sidetrack

location: crossroads | The player can watch telly or speak with the son.
  npc: the_bedtime_check_watcher
  item: railing | iron railing| Cold iron slick with condensation. | takeable=false | aliases=iron,railing
  exit: south -> start
  exit: north -> finale

location: sidetrack | At 21:30, the evening turns toward bedtime.
  item: the_bedtime_check_decoy | red herring| A convincing but ultimately irrelevant object. | aliases=red,herring
  item: cabinet | utility cabinet| A paint-chipped cabinet full of mundane supplies. | takeable=false | aliases=utility,cabinet
  exit: west -> start

location: finale | The father tells the son it is time for bed.
  item: the_bedtime_check_token | proof token| A concrete token that confirms what is really happening. | aliases=proof,token
  exit: south -> crossroads

// --- Start state ---
start_inventory: the_bedtime_check_note
start_stats: health=100 | resolve=50 | trust=0
flag: alarm_heard=false
counter: tension=0
relationship: the_bedtime_check_watcher=0

// --- DSL v2 quests/rules/triggers ---
quest: truth_path | Follow the Truth Path | Gather context and reach the final location.
quest_stage: truth_path | stage_1 | required=start | required=crossroads | required=finale
quest_objective: truth_path | obj_1 | Track the signs | Read room cues and avoid false certainty
quest_on_complete: truth_path | effects="message:You force clarity out of fear and confusion."

on_enter: crossroads | effects="message:The air tightens as if the corridor itself is listening."
on_enter: sidetrack | effects="message:A convincing detail appears useful, but leads nowhere."
on_pickup: the_bedtime_check_token | effects="message:This detail is real. Build your choice around it."

npc_acceptance_default: the_bedtime_check_watcher | text="The watcher says little, but notes every move."
npc_rule: the_bedtime_check_watcher | when=counter.tension>=2 | priority=1 | then="message:The watcher glances away, as if expecting impact."

room_desc: crossroads | A transition space where every sound seems delayed by a heartbeat.
room_desc_when: crossroads | flag:alarm_heard=true | text="A distant alarm threads through the dark like wire."

branch: sharp_end | when=has_item:the_bedtime_check_token | then="message:You reach the ending with evidence, not guesswork."
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
world: The Bedtime Check
goal: Reach the final turn with enough context to act decisively.
current_location: start

// --- DSL v2 entity definitions ---
define item: the_bedtime_check_note | Case Note | A folded note containing partial context and names.
define item: the_bedtime_check_decoy | Red Herring | A convincing but ultimately irrelevant object.
define item: the_bedtime_check_token | Proof Token | A concrete token that confirms what is really happening.
define npc: the_bedtime_check_watcher | Watcher | A tense observer who reacts to your choices.

// --- World layout ---
location: start | Father and son sit together in the living room before bedtime.
  item: lantern | hand lantern| A hand lantern with a faint, wavering glow. | takeable=false | aliases=hand,lantern
  item: clock | wall clock| A wall clock ticking half a beat too slowly. | takeable=false | aliases=wall,clock
  item: the_bedtime_check_note | case note| A folded note containing partial context and names. | aliases=case,note
  exit: north -> crossroads
  exit: east -> sidetrack

location: crossroads | The player can watch telly or speak with the son.
  npc: the_bedtime_check_watcher
  item: railing | iron railing| Cold iron slick with condensation. | takeable=false | aliases=iron,railing
  exit: south -> start
  exit: north -> finale

location: sidetrack | At 21:30, the evening turns toward bedtime.
  item: the_bedtime_check_decoy | red herring| A convincing but ultimately irrelevant object. | aliases=red,herring
  item: cabinet | utility cabinet| A paint-chipped cabinet full of mundane supplies. | takeable=false | aliases=utility,cabinet
  exit: west -> start

location: finale | The father tells the son it is time for bed.
  item: the_bedtime_check_token | proof token| A concrete token that confirms what is really happening. | aliases=proof,token
  exit: south -> crossroads

// --- Start state ---
start_inventory: the_bedtime_check_note
start_stats: health=100 | resolve=50 | trust=0
flag: alarm_heard=false
counter: tension=0
relationship: the_bedtime_check_watcher=0

// --- DSL v2 quests/rules/triggers ---
quest: truth_path | Follow the Truth Path | Gather context and reach the final location.
quest_stage: truth_path | stage_1 | required=start | required=crossroads | required=finale
quest_objective: truth_path | obj_1 | Track the signs | Read room cues and avoid false certainty
quest_on_complete: truth_path | effects="message:You force clarity out of fear and confusion."

on_enter: crossroads | effects="message:The air tightens as if the corridor itself is listening."
on_enter: sidetrack | effects="message:A convincing detail appears useful, but leads nowhere."
on_pickup: the_bedtime_check_token | effects="message:This detail is real. Build your choice around it."

npc_acceptance_default: the_bedtime_check_watcher | text="The watcher says little, but notes every move."
npc_rule: the_bedtime_check_watcher | when=counter.tension>=2 | priority=1 | then="message:The watcher glances away, as if expecting impact."

room_desc: crossroads | A transition space where every sound seems delayed by a heartbeat.
room_desc_when: crossroads | flag:alarm_heard=true | text="A distant alarm threads through the dark like wire."

branch: sharp_end | when=has_item:the_bedtime_check_token | then="message:You reach the ending with evidence, not guesswork."
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

SetupC64("The Bedtime Check");
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
