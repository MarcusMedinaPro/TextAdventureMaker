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
        "name": "Bytorget",
        "description": "Du står på torget i en liten by. En port leder norrut, en källardörr söderut.",
        "exits": {
          "north": "city_gate",
          "south": "basement"
        },
        "objects": ["torch", "poster"]
      },
      {
        "id": "city_gate",
        "name": "Stadsporten",
        "description": "En massiv port av järn. Den verkar kunna stängas vid solnedgång.",
        "exits": {
          "south": "village_square"
        },
        "objects": []
      },
      {
        "id": "basement",
        "name": "Källare",
        "description": "En fuktig, mörk källare. Vattnet sipprar in från väggarna.",
        "exits": {
          "north": "village_square",
          "east": "cave_entrance"
        },
        "objects": ["wooden_plank"]
      },
      {
        "id": "cave_entrance",
        "name": "Grottans mynning",
        "description": "En mörk öppning leder in i berget. Ett svagt rassel hörs inifrån...",
        "exits": {
          "west": "basement"
        },
        "objects": ["egg_cluster"]
      }
    ],
    "objects": [
      {
        "id": "torch",
        "name": "Fackla",
        "attributes": ["light_source", "portable"],
        "state": "unlit",
        "description": "En enkel fackla. Den måste tändas för att lysa upp mörker."
      },
      {
        "id": "poster",
        "name": "Affisch",
        "attributes": ["readable"],
        "description": "En sliten affisch: 'Varning för muterade spindlar!'"
      },
      {
        "id": "wooden_plank",
        "name": "Planka",
        "attributes": ["portable", "buildable"],
        "description": "En stadig träplanka. Kan användas för att bygga en bro eller flotte."
      },
      {
        "id": "egg_cluster",
        "name": "Äggklase",
        "attributes": ["fragile"],
        "description": "En klase glänsande spindelägg. De rör sig nästan..."
      }
    ],
    "npcs": [
      {
        "id": "village_guard",
        "name": "Byvakt",
        "location": "city_gate",
        "dialogue": [
          "Håll dig inne när solen går ner!",
          "Jag har sett spindlarna röra sig i skogen..."
        ],
        "schedule": ["city_gate", "village_square"]
      },
      {
        "id": "mutant_spider",
        "name": "Muterad spindel",
        "location": "cave_entrance",
        "behavior": "hostile",
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
        "message": "Porten stängs med ett brak!"
      },
      {
        "type": "countdown",
        "duration": 10,
        "action": "flood_room",
        "target": "basement",
        "message": "Vattnet stiger i källaren – du måste fly!"
      },
      {
        "type": "after_ticks",
        "ticks": 20,
        "action": "spawn",
        "target": "mutant_spider",
        "location": "cave_entrance",
        "message": "Äggen spricker – spindlar kryper fram ur klasen!"
      },
      {
        "type": "chase",
        "ticks": 1,
        "npc": "mutant_spider",
        "follow": "player",
        "message": "Du hör spindelns steg bakom dig..."
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
                │   (NPC: Guard)
                │   [Port stängs 18:00]
                └───────▲─────┘
                        │
                        │
┌─────────────┐   ┌─────┴─────┐   ┌──────────────┐
│ Village Sq. │   │  Basement │──▶│ Cave Entrance│
│ (Start)     │   │ (Översväm-│   │ (Äggklase +  │
│ Obj: Torch, │   │  mas efter│   │ mutant spider│
│ Poster      │   │   10 drag)│   │ spawn efter  │
└───────▲─────┘   └───────────┘   │   20 drag)   │
        │                         └──────────────┘
        │
        │
        ▼
   [Spelaren startar här]
```

---

## Features Demonstrated

| Feature | Implementation |
|---------|----------------|
| **4 rooms** | torget, porten, källaren, grottan |
| **4 objects** | fackla, affisch, planka, äggklase |
| **2 NPCs** | vakt med schema, mutantspindel som jagar |
| **4 events** | port stänger kl. 18, källare översvämmas, ägg kläcks efter 20 drag, spindeljakt varje drag |

---

## Gameplay Flow

### 1. Start – Village Square

* Spelaren börjar här
* De ser: **Torch (unlit)** + **Poster**
* **Poster** ger ledtråd: *"Varning för muterade spindlar!"*
* **Torch** kan plockas upp → behöver tändas senare

### 2. Utforska – City Gate

* Porten står öppen på dagen
* **NPC Guard** finns här. Dialog hintar: *"Stanna inne när solen går ner."*
* Om spelaren väntar för länge (efter kl. 18) stängs porten → ingen flyktväg

### 3. Utforska – Basement

* En fuktig källare med **Wooden Plank**
* Event: *Efter 10 drag börjar vattnet stiga*
* Om spelaren dröjer för länge → källaren översvämmas → game over

### 4. Cave Entrance

* Här finns **Egg Cluster** (spindelägg)
* Efter 20 drag → äggen spricker → **Mutant Spider** spawnar
* När spindeln finns → den jagar spelaren (ett steg närmare varje drag)

### 5. Pussel – Planka

* **Wooden Plank** kan användas för att skapa en provisorisk bro
* Fungerar som "quest item" för framtida expansion

### 6. Kritiska val

* Om facklan inte är tänd → spelaren famlar i mörkret i grottan
* Om spelaren väntar för länge → porten stängs, källaren översvämmas, spindlarna kläcks
* Om spelaren agerar snabbt → kan förstöra äggen innan de kläcks

---

## Multiple Endings

| Ending | Condition |
|--------|-----------|
| **Victory 1** | Förstör äggen innan de kläcks → byn räddad |
| **Victory 2** | Fly innan porten stängs → du överlever, men spindlarna sprider sig |
| **Loss 1** | Fast i översvämmad källare |
| **Loss 2** | Spindlar kläcks → jagar dig och dödar dig |
| **Loss 3** | Porten stängs, spindlarna tar över |

---

## Design Points

* **Tid som fiende** – flera parallella deadlines
* **Valfrihet** – olika slut, både "heroisk" och "survivalistisk"
* **Replayability** – spelaren kan experimentera med olika strategier
* **Stämning** – klassisk C64-känsla: mörker, enkla resurser, hårda val

---

## Expanded Version: Dallas Quest Style

### Additional Rooms

```json
{
  "id": "river_bank",
  "name": "River Bank",
  "description": "En bred flod blockerar vägen till grottans inre. En trasig båt ligger vid stranden.",
  "exits": { "west": "cave_entrance", "east": "spider_temple" },
  "objects": ["boat"]
},
{
  "id": "jungle_clearing",
  "name": "Jungle Clearing",
  "description": "En apa hänger i trädet och stirrar på dig med nyfikna ögon.",
  "exits": { "north": "basement" },
  "objects": ["banana"],
  "npcs": ["monkey"]
},
{
  "id": "spider_temple",
  "name": "Spider Temple",
  "description": "Slutrummet med spindeldrottningen. Dörren är låst med spindelsymboler.",
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
  "description": "A ripe, yellow banana. Monkeys love these."
},
{
  "id": "boat",
  "name": "Boat",
  "attributes": ["vehicle"],
  "state": "broken",
  "description": "A small boat with a hole in the bottom."
}
```

### Monkey NPC

```json
{
  "id": "monkey",
  "name": "Mischievous Monkey",
  "location": "jungle_clearing",
  "behavior": "neutral",
  "on_tick": {
    "chance": 0.1,
    "action": "steal_item",
    "exclude": ["banana"]
  },
  "on_give": {
    "banana": {
      "action": "plug_boat_hole",
      "message": "Apan gnager lyckligt på bananen och sätter sig sedan i båten. Den sticker in svansen i hålet — båten är tät!"
    }
  },
  "dialogue": [
    "Ook ook!",
    "*The monkey stares at your banana with suspicious interest…*"
  ]
}
```

### Humorous Interactions (Dallas Quest Style)

| Command | Response |
|---------|----------|
| `climb monkey` | *"The monkey giggles and climbs you instead. Now you smell like bananas."* |
| `attack monkey` | *"The monkey counters with a banana peel to the face."* |
| `use torch on boat` | *"Well, you burned the boat. Hope you like swimming."* |

### Expanded Flow

1. Spelaren får fackla i byn
2. Går till djungeln → hittar apan, bananen
3. Ger banan till apan → apan blir din kaotiska sidekick
4. I River Bank → apan hjälper att laga båten med svansen
5. Korsar floden → når Spider Temple
6. Offrar ett ägg på altaret → öppnar dörren
7. Slutboss: Spindeldrottningen → final showdown eller smart pussellösning

---

## C# Implementation Example

```csharp
var game = new GameBuilder("Spider's Lair")
    .WithIntro("Byn hotas av muterade spindlar. Kan du stoppa dem innan det är för sent?")
    .WithGoal("Förstör spindeläggen eller fly från byn.")
    .AddLocation("village_square", loc => loc
        .Name("Bytorget")
        .Description("Du står på torget i en liten by.")
        .AddItem("torch", item => item.IsLightSource().State("unlit"))
        .AddItem("poster", item => item.IsReadable().OnRead("Varning för muterade spindlar!"))
        .AddExit(Direction.North, "city_gate")
        .AddExit(Direction.South, "basement"))
    .AddLocation("city_gate", loc => loc
        .Name("Stadsporten")
        .Description("En massiv port av järn.")
        .AddExit(Direction.South, "village_square")
        .AddNpc("guard", npc => npc
            .AddDialogue("Håll dig inne när solen går ner!")
            .WithSchedule("city_gate", "village_square")))
    .AddLocation("basement", loc => loc
        .Name("Källare")
        .Description("En fuktig, mörk källare.")
        .AddItem("wooden_plank", item => item.IsTakeable())
        .AddExit(Direction.North, "village_square")
        .AddExit(Direction.East, "cave_entrance"))
    .AddLocation("cave_entrance", loc => loc
        .Name("Grottans mynning")
        .Description("En mörk öppning leder in i berget.")
        .AddItem("egg_cluster", item => item.IsFragile())
        .AddExit(Direction.West, "basement"))
    // Events
    .AddEvent(evt => evt
        .AtTime(18, 0)
        .CloseDoor("city_gate")
        .WithMessage("Porten stängs med ett brak!"))
    .AddEvent(evt => evt
        .AfterTicks(10)
        .FloodRoom("basement")
        .WithMessage("Vattnet stiger i källaren – du måste fly!"))
    .AddEvent(evt => evt
        .AfterTicks(20)
        .SpawnNpc("mutant_spider", "cave_entrance")
        .WithMessage("Äggen spricker – spindlar kryper fram!"))
    .AddNpc("mutant_spider", npc => npc
        .IsHostile()
        .ChasesPlayer()
        .WithChaseMessage("Du hör spindelns steg bakom dig..."))
    .StartAt("village_square")
    .Build();
```

---

## Slices Demonstrated

This adventure demonstrates features from:

- **Slice 001**: Location & Navigation
- **Slice 002**: Doors & Keys
- **Slice 004**: Items & Inventory
- **Slice 005**: NPCs & Dialog
- **Slice 006**: Event System
- **Slice 021**: Time System
- **Slice 038**: Time-Triggered Objects
- **Slice 062**: Countdown & Deadlines
- **Slice 063**: Chase & Pursuit
