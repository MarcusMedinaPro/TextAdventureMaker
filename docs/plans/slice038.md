## Slice 38: Time/Action Triggered Objects & Doors

**Mål:** Objekt och dörrar som spawnar/öppnas baserat på tid eller actions.

### Task 38.1: ITimedSpawn — objekt som dyker upp

```csharp
location.AddTimedSpawn("treasure_chest")
    .AppearsAt(tick: 100)
    .Or()
    .AppearsWhen(w => w.GetFlag("dragon_dead"))
    .DisappearsAfter(ticks: 50)  // Begränsad tid
    .Message("A treasure chest materializes from thin air!");

location.AddTimedSpawn("ghost")
    .AppearsAt(TimePhase.Night)
    .DisappearsAt(TimePhase.Dawn);
```

### Task 38.2: ITimedDoor — dörrar som öppnas/stängs

```csharp
location.AddExit(Direction.North, secretRoom)
    .WithTimedDoor("magic_portal")
        .OpensAt(tick: 50)
        .ClosesAt(tick: 100)
        .Or()
        .OpensWhen(w => w.TimePhase == TimePhase.Midnight)
        .Message("The portal shimmers into existence...");

location.AddExit(Direction.East, vault)
    .WithTimedDoor("bank_vault")
        .OpensAt(GameTime.Hours(9))   // 9 AM
        .ClosesAt(GameTime.Hours(17)) // 5 PM
        .ClosedMessage("The bank is closed.");
```

### Task 38.3: Action-triggered spawns

```csharp
// Object appears after player action
game.OnAction("pull_lever", ctx => {
    ctx.SpawnItem("hidden_key", in: "throne_room");
    ctx.Message("You hear a click. Something fell somewhere...");
});

// Object appears after NPC dies
game.OnNpcDeath("dragon", ctx => {
    ctx.SpawnItem("dragon_heart", in: ctx.Location);
    ctx.OpenDoor("treasure_vault");
});

// Chain reaction
game.OnItemPickup("crystal", ctx => {
    ctx.After(ticks: 10, () => {
        ctx.CollapseDoor("entrance");
        ctx.Message("The cave begins to collapse!");
    });
});
```

### Task 38.4: Conditional permanent changes

```csharp
// Door permanently opens after condition
door.PermanentlyOpensWhen(w =>
    w.HasItem("master_key") || w.GetFlag("lockpick_master"));

// Location transforms
location.TransformsInto("ruined_village")
    .When(w => w.GetCounter("village_fires") >= 10)
    .Irreversible();
```

### Task 38.5: Scheduled events queue

```csharp
game.Schedule()
    .At(tick: 0, ctx => ctx.Message("Your journey begins..."))
    .At(tick: 50, ctx => ctx.SpawnNpc("merchant", "crossroads"))
    .At(tick: 100, ctx => ctx.TriggerEvent("storm_begins"))
    .At(tick: 200, ctx => ctx.OpenDoor("flooded_passage"))
    .Every(ticks: 25, ctx => ctx.SpawnRandomEncounter("forest"));
```

### Task 38.6: DSL för timed objects

```
location "ancient_temple" {
    timed_spawn "ghost_guardian" {
        appears_at: night
        disappears_at: dawn
    }

    timed_door "sun_door" direction: east {
        opens_when: time_phase == "day"
        closes_when: time_phase == "night"
        message: "Sunlight reveals a hidden passage"
    }

    action_spawn on: "place_gem_in_altar" {
        spawn: "portal" type: exit direction: up
        message: "A portal opens above the altar!"
    }
}

schedule {
    at tick:100 -> spawn "wandering_merchant" in "crossroads"
    every tick:50 -> random_event "forest_encounters"
    when flag:"volcano_active" -> transform "village" into "destroyed_village"
}
```

### Task 38.7: Sandbox — tempel där dörrar öppnas vid rätt tid, skatt spawnar efter boss

---

## Implementation checklist (engine)
- [ ] `ITimedSpawn`
- [ ] `ITimedDoor`
- [ ] Action-triggered spawns
- [ ] Conditional permanent changes
- [ ] Scheduled events queue
- [ ] DSL for timed objects

## Example checklist (docs/examples)
- [ ] Timed spawns/doors demo

---

## Future Roadmap (v2+)

### Världssimulation

**Time System**

- Dag/natt-cykler, `TicksPerDay`
- Delayed events: `After(ticks: 50, ctx => ctx.TriggerEvent("cave_collapse"))`
- Saker förändras över tid (blod torkar, eld slocknar, NPC blir trött)
- TimePhase: Dawn, Day, Dusk, Night

**Perception System**

- LightLevel per location (mörker kräver fackla)
- Synfält, dimma, blindhet
- Hörsel: ljud från angränsande rum
- Minnesystem: vad spelaren har sett, hört, lärt sig
- Rykten och osäker information

**Semantic Objects**

- Traits: `Flammable, Brittle, Sharp, Heavy, Holy, Cursed, Frozen, Hot`
- Affordances: vad kan man rimligen göra med objektet
- Emergent interactions: `ice + fire → water` utan hårdkodade recept
- Fysikaliska relationer: vätskor, gaser, temperatur

**Social System**

- Relationer: Trust, Fear, Loyalty, Guilt
- Rykte som sprids mellan NPCs
- Minnesbaserade reaktioner ("du svek mig förut")
- Gruppdynamik (NPC påverkar varandra)

**Intention System**

- Målbaserade handlingar: `player.Intent("escape the cave")`
- Delplaner: om låst → hitta nyckel → gå tillbaka
- Avbrutna handlingar (börjar klättra, blir attackerad)
- Kräver AI-integration

**Narrative Logic**

- Teman och motiv (rädsla, skuld, hopp)
- Foreshadowing-hooks
- Dramaturgiska tillstånd (uppbyggnad, kris, klimax)
- "Författarmedvetenhet" i ramverket

**Meta Perspective**

- Undo / spola tillbaka (tidslinjer)
- What-if scenarier
- Debug-visning av orsak–verkan

---

### Developer Experience

**Prose-like Fluent API**

```csharp
World.Room("Cave")
    .Description("A dark, wet cave")
    .NorthTo("Forest")
    .WithDoor("IronGate").LockedBy("RustyKey")
    .Contains(item => item.Sword("Old Blade"))
    .Contains(npc => npc.Hermit().Hostile());
```

**DSL för författare**

```
room "Cave" {
    description "A dark cave"
    exit north -> "Forest" locked by "RustyKey"
    contains {
        sword "Old Blade"
        npc "Hermit" hostile
    }
}
```

**Extension Hooks överallt**

```csharp
game.OnEnter("Cave", state => {
    state.Spawn("Bat");
    state.Message("You feel watched.");
});

game.OnUse("Torch", "Curtain", ctx => ctx.StartFire());
game.OnTurn(state => state.AdvanceTime());
game.OnEmotionChange("Hermit", Emotion.Angry, ctx => ctx.Attack());
```

**Archetypes / Prefabs**

```csharp
World.Add(Archetypes.DragonBoss()
    .WithName("Vermithrax")
    .Guards("GoldenHall")
    .WeakTo(Element.Fire));

World.Add(Archetypes.LockedTreasureChest()
    .In("Cave")
    .RequiresKey("GoldenKey")
    .Contains("DiamondRing"));

World.Add(Archetypes.WanderingMerchant()
    .Patrol("Town", "Forest", "Cave")
    .Sells("Potion", "Map", "Torch"));
```

**Visual World Inspector**

- Graf av rum och kopplingar
- NPC-rörelser i realtid
- Quest states
- "Varför misslyckades kommandot?" → orsakskedja

**Story-First Tooling**

- Story beats editor
- Quest-flödesgraf
- Relationsgraf
- Tidslinje
- Genererar builder-kod eller DSL

---

### Arkitekturvision

```
┌─────────────────────────────────────────────────────┐
│                   Story Tools                        │
│         (Visual Editor, Quest Graph, Timeline)       │
└─────────────────────┬───────────────────────────────┘
                      │ generates
┌─────────────────────▼───────────────────────────────┐
│                    DSL Layer                         │
│              (.adventure files)                      │
└─────────────────────┬───────────────────────────────┘
                      │ parses to
┌─────────────────────▼───────────────────────────────┐
│               Fluent Builder API                     │
│          (C# prose-like builders)                    │
└─────────────────────┬───────────────────────────────┘
                      │ builds
┌─────────────────────▼───────────────────────────────┐
│                 Core Engine                          │
│    (Interfaces, Events, State, Commands)             │
├─────────────────────────────────────────────────────┤
│  Modules:                                            │
│  ├── Combat (Strategy)                               │
│  ├── Social (Relationships)                          │
│  ├── Time (Day/Night, Delays)                        │
│  ├── Perception (Light, Sound, Memory)               │
│  ├── Semantics (Traits, Affordances)                 │
│  ├── Narrative (Themes, Foreshadowing)               │
│  └── AI (Ollama, Intent parsing)                     │
└─────────────────────────────────────────────────────┘
```

---

## Sammanfattning av slices

### Core Engine (v1)

**Status legend:** ✅ implemented in code · 🟨 demo/docs only · ⬜ planned
| # | Slice | Patterns | Status |
|---|-------|----------|--------|
| 1 | Location + Navigation | - | ✅ |
| 2 | Doors + Keys | State | ✅ |
| 3 | Command + Parser | Command, Chain of Responsibility | ✅ |
| 4 | Items + Inventory | Factory, Prototype, Decorator, Composite | ✅ |
| 5 | NPCs + Dialog + Movement | State, Composite, Strategy | ✅ |
| 6 | Event System | Observer | ✅ |
| 7 | Combat | Strategy, Command | ✅ |
| 8 | Quest System | State, Visitor | ✅ |
| 9 | World State System | - | ✅ |
| 10 | Save/Load | Memento | ✅ |
| 11 | Language System | Flyweight | ✅ |
| 12 | DSL Parser | Template Method | ✅ |
| 13 | GameBuilder | Builder, Facade | ✅ |
| 14 | Loggers | Observer, Proxy | 🟨 |
| 15 | Pathfinder | Strategy | 🟨 |
| 16 | AI-paket | Facade, Strategy | 🟨 |
| 17 | NuGet-paketering | - | 🟨 |

### Advanced Systems (v1.5)

| #   | Slice                             | Patterns           | Status |
| --- | --------------------------------- | ------------------ | ------ |
| 18  | Story Branches & Consequences     | State              | 🟨     |
| 19  | Multi-Stage Quests                | State, Strategy    | 🟨     |
| 20  | Hints & Properties (current impl) | -                  | ✅     |
| 21  | Time System                       | Observer, Strategy | ✅     |
| 22  | Faction & Reputation              | Observer           | ✅     |
| 23  | Random Event Pool                 | Strategy           | ✅     |
| 24  | Location Discovery                | -                  | ✅     |
| 25  | Story Mapper Tool                 | -                  | 🟨     |

### Storytelling Systems (v2)

| #   | Slice                                | Patterns                    | Status |
| --- | ------------------------------------ | --------------------------- | ------ |
| 26  | Mood & Atmosphere                    | State, Observer             | 🟨     |
| 27  | Dynamic Descriptions                 | Strategy, Template          | 🟨     |
| 28  | Character Arc Tracking               | State                       | 🟨     |
| 29  | Pacing & Tension                     | Observer, State             | 🟨     |
| 30  | Foreshadowing & Callbacks            | Observer                    | 🟨     |
| 31  | Scene Transitions & Beats            | State, Command              | 🟨     |
| 32  | Emotional Stakes                     | Observer                    | 🟨     |
| 33  | Narrative Voice                      | Strategy                    | 🟨     |
| 34  | Player Agency Tracking               | Observer                    | 🟨     |
| 35  | Dramatic Irony Tracker               | Observer                    | 🟨     |
| 36  | Hero's Journey & Narrative Templates | Template, Strategy, Builder | 🟨     |
| 37  | Generic Chapter System               | State, Builder              | 🟨     |
| 38  | Time/Action Triggered Objects        | Observer, Scheduler         | 🟨     |

### Polish & Documentation (v2+)

| #   | Slice                                 | Patterns             | Status |
| --- | ------------------------------------- | -------------------- | ------ |
| 39  | Fluent API & Språksnygghet            | Builder, Factory     | 🟨     |
| 40  | GitHub Wiki (TextAdventure.wiki)      | -                    | 🟨     |
| 41  | Testing & Validation Tools            | Visitor, Strategy    | ⬜     |
| 42  | API Design: Fluent Query Extensions   | Fluent API, Builder  | ⬜     |
| 43  | Map Generator                         | -                    | ⬜     |
| 44  | String Case Utilities                 | -                    | ✅     |
| 45  | Generic Fixes                         | -                    | ⬜     |

---
