# Spider's Lair - Reference Adventure

**Purpose:** Comprehensive example demonstrating rooms, objects, NPCs, time-based events, chase mechanics, puzzles, and multiple endings.

---

## World Definition (JSON)

```json
{
  "world": {
    "rooms": [
      {
        "id": "village_square",
        "name": "Village Square",
        "description": "You stand in the village square. A city gate leads north, and a basement door leads south.",
        "exits": {
          "north": "city_gate",
          "south": "basement"
        },
        "objects": ["torch", "poster"]
      },
      {
        "id": "city_gate",
        "name": "City Gate",
        "description": "A massive iron gate that can be sealed at sundown.",
        "exits": {
          "south": "village_square"
        },
        "objects": []
      },
      {
        "id": "basement",
        "name": "Basement",
        "description": "A damp, dark basement. Water seeps through the stone walls.",
        "exits": {
          "north": "village_square",
          "east": "cave_entrance"
        },
        "objects": ["wooden_plank"]
      },
      {
        "id": "cave_entrance",
        "name": "Cave Entrance",
        "description": "A black opening leads into the mountain. A faint skittering sound echoes within.",
        "exits": {
          "west": "basement"
        },
        "objects": ["egg_cluster"]
      }
    ],
    "objects": [
      {
        "id": "torch",
        "name": "Torch",
        "attributes": ["light_source", "portable"],
        "state": "unlit",
        "description": "A simple torch. It must be lit to pierce the dark."
      },
      {
        "id": "poster",
        "name": "Poster",
        "attributes": ["readable"],
        "description": "A torn notice: 'Warning: mutant spiders!'"
      },
      {
        "id": "wooden_plank",
        "name": "Wooden Plank",
        "attributes": ["portable", "buildable"],
        "description": "A sturdy plank. It could be used for a makeshift bridge or raft."
      },
      {
        "id": "egg_cluster",
        "name": "Egg Cluster",
        "attributes": ["fragile"],
        "description": "A clutch of glossy spider eggs. They seem to twitch when you stare too long."
      }
    ],
    "npcs": [
      {
        "id": "village_guard",
        "name": "Village Guard",
        "location": "city_gate",
        "dialogue": [
          "Stay indoors when the sun goes down!",
          "I have seen the spiders moving in the woods..."
        ],
        "schedule": ["city_gate", "village_square"]
      },
      {
        "id": "mutant_spider",
        "name": "Mutant Spider",
        "location": "cave_entrance",
        "behaviour": "hostile",
        "dialogue": [],
        "on_tick": "chase_player"
      }
    ],
    "events": [
      {
        "type": "at_time",
        "time": "18:00",
        "action": "close_door",
        "target": "city_gate",
        "message": "The gate slams shut!"
      },
      {
        "type": "countdown",
        "duration": 10,
        "action": "flood_room",
        "target": "basement",
        "message": "Water rises in the basement - you must flee!"
      },
      {
        "type": "after_ticks",
        "ticks": 20,
        "action": "spawn",
        "target": "mutant_spider",
        "location": "cave_entrance",
        "message": "The eggs split - spiders spill out!"
      },
      {
        "type": "chase",
        "ticks": 1,
        "npc": "mutant_spider",
        "follow": "player",
        "message": "You hear the spider creeping behind you..."
      }
    ]
  },
  "player": {
    "start_room": "village_square",
    "inventory": [],
    "stats": {
      "health": 100,
      "energy": 50,
      "fear": 0
    }
  }
}
```

---

## ASCII Map

```
                ┌─────────────┐
                │   City Gate │
                │  NPC: Guard │
                │ [Closes 18:00]
                └───────▲─────┘
                        │
                        │
┌─────────────┐   ┌─────┴─────┐   ┌──────────────┐
│Village Square│   │  Basement │──▶│ Cave Entrance│
│   (Start)    │   │ [Flood +10]│   │ [Eggs +20]   │
│ Torch, Poster│   │    Plank   │   │ Mutant Spider│
└───────▲──────┘   └───────────┘   └──────────────┘
```

---

## Features Demonstrated

| Feature | Implementation |
|---------|----------------|
| **4 rooms** | village square, city gate, basement, cave entrance |
| **4 objects** | torch, poster, wooden plank, egg cluster |
| **2 NPCs** | scheduled guard, hostile mutant spider |
| **4 events** | gate closure, basement flood, egg hatch, per-turn chase |

---

## Gameplay Flow

### 1. Start - Village Square

- The player starts here.
- They can find **Torch (unlit)** and **Poster**.
- **Poster** gives the core warning.
- **Torch** is useful later in dark zones.

### 2. Explore - City Gate

- The gate is open during daytime.
- **Village Guard** gives time-pressure hints.
- If the player waits too long (after 18:00), the gate closes.

### 3. Explore - Basement

- A damp basement with **Wooden Plank**.
- Event: after 10 turns, water rises.
- Delay too long and the basement becomes a fail state.

### 4. Cave Entrance

- Contains **Egg Cluster**.
- After 20 turns, eggs hatch and spawn **Mutant Spider**.
- Once spawned, the spider pursues the player each turn.

### 5. Puzzle Potential - Wooden Plank

- **Wooden Plank** can support traversal puzzles.
- Works well as a future quest item or route unlock.

### 6. Critical Choices

- If the torch remains unlit, cave navigation becomes risky.
- If the player stalls, deadlines stack: closed gate, flood, hatch.
- If they act quickly, they can destroy eggs before hatching.

---

## Multiple Endings

| Ending | Condition |
|--------|-----------|
| **Victory 1** | Destroy eggs before they hatch, saving the village |
| **Victory 2** | Escape before gate closure, surviving while danger remains |
| **Loss 1** | Trapped in the flooded basement |
| **Loss 2** | Spider hatches, catches, and kills the player |
| **Loss 3** | Gate closes and the spiders overrun the area |

---

## Design Points

- **Time as enemy**: multiple parallel deadlines.
- **Meaningful choice**: different endings from route and pacing.
- **Replayability**: strategic experimentation across runs.
- **Atmosphere**: classic C64-inspired survival pressure.

---

## Expanded Version: Dallas Quest Style

### Additional Rooms

```json
{
  "id": "river_bank",
  "name": "River Bank",
  "description": "A broad river blocks the inner cave route. A damaged boat rests at the shore.",
  "exits": { "west": "cave_entrance", "east": "spider_temple" },
  "objects": ["boat"]
},
{
  "id": "jungle_clearing",
  "name": "Jungle Clearing",
  "description": "A monkey hangs from a branch and studies you with bright, curious eyes.",
  "exits": { "north": "basement" },
  "objects": ["banana"],
  "npcs": ["monkey"]
},
{
  "id": "spider_temple",
  "name": "Spider Temple",
  "description": "Final chamber of the Spider Queen. The door is sealed by spider glyphs.",
  "exits": { "west": "river_bank" },
  "locked_until": "egg_offering"
}
```

### New Objects

```json
{
  "id": "banana",
  "name": "Banana",
  "attributes": ["portable", "edible"],
  "description": "A ripe yellow banana. Monkeys adore these."
},
{
  "id": "boat",
  "name": "Boat",
  "attributes": ["vehicle"],
  "state": "broken",
  "description": "A small boat with a hole in the hull."
}
```

### Monkey NPC

```json
{
  "id": "monkey",
  "name": "Mischievous Monkey",
  "location": "jungle_clearing",
  "behaviour": "neutral",
  "on_tick": {
    "chance": 0.1,
    "action": "steal_item",
    "exclude": ["banana"]
  },
  "on_give": {
    "banana": {
      "action": "plug_boat_hole",
      "message": "The monkey chews happily on the banana, then wedges its tail into the hull gap. The boat is watertight."
    }
  },
  "dialogue": [
    "Ook ook!",
    "*The monkey watches your banana with suspicious interest...*"
  ]
}
```

### Humorous Interactions (Dallas Quest Style)

| Command | Response |
|---------|----------|
| `climb monkey` | *"The monkey giggles and climbs you instead. You now smell of bananas."* |
| `attack monkey` | *"The monkey counters with a banana peel to the face."* |
| `use torch on boat` | *"Well, you burned the boat. Hope you enjoy swimming."* |

### Expanded Flow

1. The player collects the torch in the village.
2. They reach the jungle and find monkey + banana.
3. They give the banana and gain a chaotic monkey ally.
4. At River Bank, the monkey helps patch the boat.
5. They cross the river and reach Spider Temple.
6. They offer an egg at the altar to unlock the door.
7. Final boss: Spider Queen showdown or puzzle-based resolution.

---

## C# Implementation Example

```csharp
var game = new GameBuilder("Spider's Lair")
    .WithIntro("The village is threatened by mutant spiders. Can you stop them before it is too late?")
    .WithGoal("Destroy the spider eggs or flee the village.")
    .AddLocation("village_square", loc => loc
        .Name("Village Square")
        .Description("You stand in a small village square.")
        .AddItem("torch", item => item.IsLightSource().State("unlit"))
        .AddItem("poster", item => item.IsReadable().OnRead("Warning: mutant spiders!"))
        .AddExit(Direction.North, "city_gate")
        .AddExit(Direction.South, "basement"))
    .AddLocation("city_gate", loc => loc
        .Name("City Gate")
        .Description("A massive iron gate.")
        .AddExit(Direction.South, "village_square")
        .AddNpc("guard", npc => npc
            .AddDialogue("Stay indoors when the sun goes down!")
            .WithSchedule("city_gate", "village_square")))
    .AddLocation("basement", loc => loc
        .Name("Basement")
        .Description("A damp, dark basement.")
        .AddItem("wooden_plank", item => item.IsTakeable())
        .AddExit(Direction.North, "village_square")
        .AddExit(Direction.East, "cave_entrance"))
    .AddLocation("cave_entrance", loc => loc
        .Name("Cave Entrance")
        .Description("A dark opening leads into the mountain.")
        .AddItem("egg_cluster", item => item.IsFragile())
        .AddExit(Direction.West, "basement"))
    // Events
    .AddEvent(evt => evt
        .AtTime(18, 0)
        .CloseDoor("city_gate")
        .WithMessage("The gate slams shut!"))
    .AddEvent(evt => evt
        .AfterTicks(10)
        .FloodRoom("basement")
        .WithMessage("Water rises in the basement - you must flee!"))
    .AddEvent(evt => evt
        .AfterTicks(20)
        .SpawnNpc("mutant_spider", "cave_entrance")
        .WithMessage("The eggs crack open - spiders crawl free!"))
    .AddNpc("mutant_spider", npc => npc
        .IsHostile()
        .ChasesPlayer()
        .WithChaseMessage("You hear the spider creeping behind you..."))
    .StartAt("village_square")
    .Build();
```

---

## Slices Demonstrated

This adventure demonstrates features from:

- **Slice 001**: Location & Navigation
- **Slice 002**: Doors & Keys
- **Slice 004**: Items & Inventory
- **Slice 005**: NPCs & Dialogue
- **Slice 006**: Event System
- **Slice 021**: Time System
- **Slice 038**: Time-Triggered Objects
- **Slice 062**: Countdown & Deadlines
- **Slice 063**: Chase & Pursuit
