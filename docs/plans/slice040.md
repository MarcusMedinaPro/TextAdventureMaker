## Slice 40: GitHub Wiki (../TextAdventure.wiki)

**Mål:** Komplett dokumentation för användare på engelska

### Wiki-sidor

- **Home** - Projektöversikt och vision
- **Getting Started** - Installation och första spelet
- **API Reference** - Fluent API dokumentation
- **Commands** - Alla inbyggda kommandon (go, take, look, etc.)
- **DSL Guide** - .adventure filformat
- **Examples** - Exempelspel (se nedan)
- **Localization** - Hur man lägger till nya språk
- **Extending** - Skapa egna commands, parsers, etc.
- **Storytelling Guide** - Översikt narrativa verktyg
- **Narrative Arcs** - Alla mallar (se nedan)
- **Story Structures** - (från böcker: Hero's Journey, Familiar-to-Foreign, Framed, Layered)
- **Propp's Functions** - Procedural narrative byggstenar
- **Rule-Based Dialog** - Left 4 Dead-stil dynamisk dialog
- **Testing Your Adventure** - (från Usborne: test allt, låt andra spela)
- **Fluent API Guide** - LINQ-liknande syntax för äventyrsskapande

### Wiki: Reference & Theory (Nörd-sektionen 🤓)

#### 📜 History of Interactive Fiction

- **1976**: Colossal Cave Adventure (Will Crowther) — den första
- **1977-1979**: Zork (MIT) — definierade genren
- **1979-1989**: Infocom-eran — kommersiell höjdpunkt
- **1990-tal**: Decline & IF-community bildas
- **2000-tal**: Renaissance via IFComp, Twine, Ink
- **Idag**: Narrative games, walking simulators, vårt framework

#### 🔬 Parser Theory

**Tokenization:**

```
"take the rusty sword" → ["take", "the", "rusty", "sword"]
```

**Disambiguation:**

```
> take key
Which key do you mean?
1. The brass key
2. The skeleton key
```

**Parse tree:**

```
Command
├── Verb: "take"
├── Article: "the" (ignored)
├── Adjective: "rusty"
└── Noun: "sword"
```

**Vårt framework:** KeywordParser hanterar detta automatiskt.

#### 🎓 Design Patterns (varför vi använder dem)

| Pattern       | Varför                                 | Exempel i vårt framework                          |
| ------------- | -------------------------------------- | ------------------------------------------------- |
| **State**     | Objekt byter beteende utan if-else     | Door (Open/Closed/Locked), NPC (Friendly/Hostile) |
| **Command**   | Undo, logging, queuing                 | GoCommand, TakeCommand                            |
| **Observer**  | Löskopplad event-hantering             | EventSystem, OnEnter triggers                     |
| **Memento**   | Save/Load utan att exponera internals  | GameMemento                                       |
| **Strategy**  | Utbytbara algoritmer                   | CombatSystem, MovementPattern                     |
| **Composite** | Träd-strukturer                        | Containers, Dialog trees                          |
| **Factory**   | Skapa objekt utan att veta konkret typ | Items.Create("sword")                             |
| **Builder**   | Fluent konstruktion                    | GameBuilder, LocationBuilder                      |
| **Decorator** | Lägg till beteende runtime             | RustyModifier, EnchantedModifier                  |
| **Facade**    | Förenkla komplext subsystem            | OllamaFacade, GameEngine                          |

#### ⚠️ Common Pitfalls (Anti-patterns)

| Gör INTE                     | Gör istället                            |
| ---------------------------- | --------------------------------------- |
| Hardcoded strings överallt   | Language system / constants             |
| Giant switch-statements      | Command pattern + registry              |
| "Guess the verb" puzzles     | Synonym system, helpful errors          |
| Maze without purpose         | Every room should have meaning          |
| Pixel-hunting (text-version) | Clear, fair descriptions                |
| Softlock utan varning        | Validator + point-of-no-return warnings |
| Dålig inventory management   | Weight/count limits, "take all"         |
| One-way doors utan hint      | Tydliga "no return" meddelanden         |

#### ♿ Accessibility Guide

- **Screen readers**: Strukturerad output, ARIA-liknande hints
- **Dyslexia-friendly**: Konfigurerbar font, radavstånd, färger
- **Cognitive load**: "recap" command, quest log, hints system
- **Motor impairment**: Kortkommandon, command history
- **Color blindness**: Symboler + färg, aldrig bara färg

#### 🧩 Puzzle Design Theory

**Puzzle-typer:**

| Typ                  | Exempel                   | Framework-stöd         |
| -------------------- | ------------------------- | ---------------------- |
| **Lock & Key**       | Dörr + nyckel             | Door.RequiresKey()     |
| **Combination**      | Blanda items              | ItemCombinations       |
| **Sequence**         | Gör saker i rätt ordning  | EventChain             |
| **Information**      | Lär dig något, använd det | WorldState flags       |
| **Environment**      | Manipulera rummet         | Dynamic descriptions   |
| **Dialog**           | Säg rätt sak till NPC     | Rule-based dialog      |
| **Time-based**       | Gör något innan timer     | TimedEvents            |
| **Lateral thinking** | Oväntad användning        | UseCommand flexibility |

**Difficulty curve:**

```
Easy: Lock & Key (1 item, 1 door)
  ↓
Medium: Multi-step (key A → door A → key B → door B)
  ↓
Hard: Interconnected (item X needed for puzzle Y which gives item Z for puzzle W)
  ↓
Expert: Timed multi-step with consequences
```

#### ✍️ Writing Good Descriptions

**Show, don't tell:**

```
❌ "This is a scary room."
✅ "Shadows pool in the corners. Something drips in the darkness."
```

**Be specific:**

```
❌ "There is a sword here."
✅ "A notched longsword leans against the wall, rust creeping up the blade."
```

**Vary sentence structure:**

```
❌ "You are in a cave. There is a torch. There is a chest."
✅ "Firelight dances across the cave walls. A battered chest sits in the corner,
    its lock long since broken."
```

**First visit vs return:**

```
First: "You push through the undergrowth into a sun-dappled clearing."
Return: "The familiar clearing opens before you."
```

#### 📚 Recommended Reading

**Böcker:**

- _Twisty Little Passages_ (Nick Montfort) — IF-historia
- _Creating Interactive Fiction with Inform 7_ (Aaron Reed)
- _Procedural Storytelling in Game Design_ (Short & Adams)
- _The Art of Game Design_ (Jesse Schell)

**Online:**

- [IFDB](https://ifdb.org) — Interactive Fiction Database
- [IFComp](https://ifcomp.org) — Årlig tävling
- [Emily Short's Blog](https://emshort.blog) — Narrative design
- [Brass Lantern](http://brasslantern.org) — IF-tutorials

**Spel att studera:**

- _Zork I_ — Klassiker, parser-design
- _Photopia_ — Narrativ innovation
- _Spider and Web_ — Unreliable narrator
- _Anchorhead_ — Atmosfär, Lovecraft
- _80 Days_ — Modern IF, choices matter
- _Disco Elysium_ — Dialog-system, skill checks
- _Leather Goddesses of Phobos_ — Puzzles, world-building, humor

#### 🔄 Migration Guide

**Från Inform 7:**

```inform7
The Kitchen is a room. "A warm, cluttered space."
The wooden spoon is in the Kitchen.
```

```csharp
var kitchen = new Location("kitchen", "Kitchen")
    .Description("A warm, cluttered space.");
var spoon = new Item("spoon", "Wooden Spoon");
kitchen.AddItem(spoon);
```

**Från TADS:**

---

## Implementation checklist (docs)
- [x] GitHub wiki created (`../TextAdventure.wiki`)
- [x] Core wiki pages (Home, Getting Started, API Reference, Commands, DSL Guide, Examples)
- [x] Localization + Extending guides
- [x] Storytelling guides + narrative arcs
- [x] Testing guide + Fluent API guide

## Example checklist (docs/examples)
- [ ] Not applicable (wiki)

```tads
kitchen: Room 'Kitchen'
    "A warm, cluttered space."
;
```

```csharp
// Samma som ovan — vårt API är medvetet Inform/TADS-inspirerat
```

**Från Twine/Ink:**

```ink
=== kitchen ===
A warm, cluttered space.
+ [Take the spoon] -> has_spoon
+ [Leave] -> hallway
```

```csharp
// Fluent API (LINQ-liknande) för samma effekt:
story.When(s => s.Location == kitchen)
    .Choice("Take the spoon", () => player.Take(spoon))
    .Choice("Leave", () => player.GoTo(hallway));
```

#### 📊 Comparison with Other Systems

| Feature         | Inform 7        | TADS        | Twine     | Ink          | **Vårt**                     |
| --------------- | --------------- | ----------- | --------- | ------------ | ---------------------------- |
| Language        | Natural English | C-like      | HTML/CSS  | Markdown-ish | **C#**                       |
| Parser          | Built-in        | Built-in    | None      | None         | **KeywordParser**            |
| State mgmt      | World model     | World model | Variables | Variables    | **WorldState + Fluent API**  |
| Extensible      | Limited         | Yes         | JS        | C#           | **Full .NET**                |
| IDE support     | Inform IDE      | TADS WB     | Web       | Unity/Inky   | **VS/Rider**                 |
| Testing         | Skein           | -           | -         | -            | **Validator**                |
| Narrative tools | Basic           | Basic       | Basic     | Weave/Knots  | **Fluent API, Arcs, Themes** |
| NuGet/Package   | No              | No          | No        | Yes          | **Yes**                      |

#### 📖 Glossary

| Term                  | Definition                                |
| --------------------- | ----------------------------------------- |
| **IF**                | Interactive Fiction                       |
| **Parser**            | System som tolkar spelarens input         |
| **CYOA**              | Choose Your Own Adventure (inget parsing) |
| **Softlock**          | Spelet kan inte vinnas men fortsätter     |
| **Hardlock**          | Spelet kraschar/fastnar                   |
| **Feelie**            | Fysiskt objekt som medföljer spel         |
| **Implementor**       | Person som skapar IF                      |
| **Transcript**        | Logg av spelarsession                     |
| **Walkthrough**       | Steg-för-steg lösning                     |
| **Hint system**       | Gradvis avslöjande av hjälp               |
| **Compass rose**      | N/S/E/W navigation                        |
| **Inventory puzzle**  | Lösning kräver rätt items                 |
| **Conversation tree** | Förgrenad dialog                          |
| **World model**       | Internrepresentation av spelvärlden       |

---

### Wiki: Objects & Items (Best Practices)

#### 🗝️ Item Design Principles

**Single Responsibility:**

```csharp
// ❌ God-item
class MagicSword : Item, IWeapon, IKey, ILightSource, IContainer { }

// ✅ Composable
var sword = new Item("sword")
    .AsWeapon(damage: 10)
    .WithEnchantment("glowing")  // Light source via decorator
    .CanUnlock("magic_door");    // Key-like behavior
```

**Item Categories:**

| Type             | Properties              | Use Case              |
| ---------------- | ----------------------- | --------------------- |
| **Basic**        | id, name, description   | Background items      |
| **Takeable**     | + weight, takeable=true | Inventory items       |
| **Key**          | + unlocks(door_id)      | Access control        |
| **Container**    | + contents[], capacity  | Storage, puzzles      |
| **Readable**     | + text, requiresLight   | Documents, signs      |
| **Consumable**   | + uses, onUse effect    | Potions, food         |
| **Weapon**       | + damage, durability    | Combat                |
| **Light Source** | + brightness, fuel      | Dark rooms            |
| **Tool**         | + canUseOn(targets)     | Specific interactions |

**Naming Conventions:**

```csharp
// ID: snake_case, unik
// Name: Human readable, kan ha prefix

new Item("brass_key", "Brass Key")           // ✅
new Item("key1", "Key")                       // ❌ Generic
new Item("the_brass_key", "The Brass Key")   // ❌ Article i ID
```

#### 🚪 Door Design

```csharp
// Simple locked door
door.Locked().RequiresKey("brass_key");

// Combination lock
door.Locked().RequiresCode("1234");

// Conditional
door.Locked().OpensWhen(w => w.GetFlag("alarm_disabled"));

// Destructible
door.Destructible().Health(50).WeakTo("fire");

// One-way
exit.OneWay().Message("The door slams shut behind you!");
```

#### 📦 Container Patterns

```csharp
// Basic chest
var chest = new Container("chest")
    .Capacity(10)
    .InitiallyLocked()
    .RequiresKey("chest_key");

// Nested containers
var bag = new Container("bag").Contains(
    new Container("pouch").Contains(
        new Item("gem")
    )
);

// Type-restricted
var quiver = new Container<Arrow>("quiver").MaxItems(20);
var flask = new Container<Liquid>("flask").MaxVolume(500);
```

---

### Wiki: NPC Design (Best Practices)

#### 👥 NPC Archetypes

| Archetype       | Behavior                                         | Example               |
| --------------- | ------------------------------------------------ | --------------------- |
| **Shopkeeper**  | Stationary, trade dialog, inventory              | Blacksmith, Innkeeper |
| **Quest Giver** | Stationary, quest dialog, tracks progress        | Village Elder, King   |
| **Guide**       | Follows, gives hints, comments on locations      | Companion, Fairy      |
| **Guard**       | Patrols, blocks access, can be bribed/distracted | Castle Guard          |
| **Enemy**       | Hostile, combat AI, drops loot                   | Goblin, Dragon        |
| **Informant**   | Stationary, reveals info based on conditions     | Bartender, Spy        |
| **Victim**      | Needs rescue, follows after saved                | Prisoner, Child       |
| **Merchant**    | Wanders, random inventory, haggling              | Traveling Trader      |

```csharp
// Quick archetype setup
var blacksmith = Npc.Shopkeeper("blacksmith", "Gruff Blacksmith")
    .Sells("sword", "shield", "armor")
    .Buys(ItemType.Metal)
    .Greeting("What'll it be?");

var guard = Npc.Guard("guard", "Castle Guard")
    .Patrols("gate", "courtyard", "tower")
    .Blocks("throne_room")
    .CanBeBribed(gold: 50)
    .CanBeDistracted("noise_elsewhere");
```

#### 💬 Dialog Design Patterns

**Branching (traditional tree):**

```csharp
npc.Dialog()
    .Say("Greetings, traveler.")
    .Choice("Ask about the weather", d => d.Say("Storm's coming."))
    .Choice("Ask about rumors", d => d
        .Say("Heard the dragon's been spotted.")
        .Choice("Where?", d2 => d2.Say("North mountains."))
        .Choice("Thanks", d2 => d2.End())
    )
    .Choice("Goodbye", d => d.End());
```

**Conditional (rule-based):**

```csharp
npc.AddDialogRule("knows_player")
    .When(ctx => ctx.Relationship(npc) > 50)
    .Say("Ah, my friend! Good to see you again.");

npc.AddDialogRule("default_greeting")
    .Priority(0)  // Fallback
    .Say("Hmm? What do you want?");
```

**Mood-affected:**

```csharp
npc.AddDialogRule("angry_greeting")
    .When(ctx => npc.Mood == Mood.Angry)
    .Say("What?! Can't you see I'm busy!");

npc.AddDialogRule("happy_greeting")
    .When(ctx => npc.Mood == Mood.Happy)
    .Say("Hello there! Lovely day, isn't it?");
```

**Random variety:**

```csharp
npc.AddDialogRule("idle_chatter")
    .When(ctx => ctx.IsIdleConversation)
    .SayRandom(
        "Nice weather we're having.",
        "Did you hear about the festival?",
        "My back's been killing me lately."
    );
```

#### 🚶 Movement Patterns

```csharp
// Stationary
npc.Movement(MovementPattern.None);

// Random wandering
npc.Movement(MovementPattern.Wander)
    .InArea("village")
    .Speed(every: 5);  // Move every 5 ticks

// Patrol route
npc.Movement(MovementPattern.Patrol)
    .Route("gate", "courtyard", "tower", "courtyard")
    .Speed(every: 3);

// Follow player
npc.Movement(MovementPattern.Follow)
    .Target(player)
    .MaxDistance(1);

// Schedule-based
npc.Movement(MovementPattern.Scheduled)
    .At(TimePhase.Morning, "bedroom")
    .At(TimePhase.Day, "shop")
    .At(TimePhase.Evening, "tavern")
    .At(TimePhase.Night, "bedroom");
```

#### 😊 Emotion System

```csharp
public enum NpcMood
{
    Neutral, Happy, Sad, Angry, Fearful, Suspicious, Grateful
}

// Mood changes based on events
npc.OnEvent("player_helps")
    .ShiftMood(Mood.Grateful, +0.5f)
    .Say("Thank you so much!");

npc.OnEvent("player_steals")
    .ShiftMood(Mood.Angry, +0.8f)
    .ShiftRelationship(-30);

// Mood decays over time
npc.MoodDecay(rate: 0.1f, toward: Mood.Neutral);
```

#### 📋 Quest-Giving Patterns

```csharp
// Simple fetch quest
npc.OffersQuest("fetch_herbs")
    .Description("Bring me 5 healing herbs.")
    .Requires(player.Has("herb", count: 5))
    .Reward(gold: 50, xp: 100)
    .OnComplete(ctx => ctx.Say("Excellent! Here's your reward."));

// Multi-stage
npc.OffersQuest("dragon_slayer")
    .Stage(1, "Find the dragon's lair")
        .CompleteWhen(w => w.GetFlag("lair_discovered"))
    .Stage(2, "Retrieve the dragon's weakness from the library")
        .CompleteWhen(player.Has("dragon_lore"))
    .Stage(3, "Slay the dragon")
        .CompleteWhen(w => w.GetFlag("dragon_dead"))
    .FinalReward(gold: 1000, title: "Dragonslayer");

// Escort quest
npc.OffersQuest("escort_merchant")
    .Type(QuestType.Escort)
    .Escort(merchant)
    .To("city_gates")
    .FailIf(merchant.IsDead);
```

#### ⚔️ Combat Behavior

```csharp
npc.CombatBehavior()
    .Aggressive()           // Attacks on sight
    .TargetPriority(player) // Who to attack first
    .FleeWhen(health < 20)  // Run away at low health
    .CallForHelp(radius: 3) // Alert nearby allies
    .DropOnDeath("gold", "rusty_sword");

// More nuanced
npc.CombatBehavior()
    .Defensive()                        // Only attacks if attacked
    .Warns(count: 2)                    // Warns before attacking
    .SurrenderWhen(health < 10)         // Gives up
    .OnSurrender(ctx => ctx.CanSpare()) // Player choice
    .LootTable(LootTable.Bandit);
```

#### 🌍 World State Integration

```csharp
// React to player reputation
npc.AddDialogRule("hero_greeting")
    .When(ctx => ctx.World.GetReputation("village") > 80)
    .Say("The hero of our village! What an honor!");

npc.AddDialogRule("villain_greeting")
    .When(ctx => ctx.World.GetReputation("village") < -50)
    .Say("You! Guards! Seize this criminal!")
    .Then(ctx => ctx.TriggerCombat());

// React to time
npc.AddDialogRule("night_warning")
    .When(ctx => ctx.World.TimePhase == TimePhase.Night)
    .Say("You shouldn't be out at night. Dangerous creatures about.");

// React to world events
npc.AddDialogRule("after_dragon_death")
    .When(ctx => ctx.World.GetFlag("dragon_dead"))
    .Say("I heard you killed the dragon! Amazing!")
    .Then(ctx => ctx.ShiftRelationship(+20));
```

---

### Wiki: Language & Localization

#### 🗣️ Object Grammar

**Articles (a/an/the):**

```csharp
// English
item.Grammar(en => en
    .Article(Article.Indefinite, "a")   // "a sword"
    .Article(Article.Definite, "the")); // "the sword"

// Swedish
item.Grammar(sv => sv
    .Article(Article.Indefinite, "ett") // "ett svärd"
    .Article(Article.Definite, "et"));  // "svärdet" (suffix)
```

**Plurals:**

```csharp
item.Grammar(en => en
    .Singular("key")
    .Plural("keys"));

item.Grammar(en => en
    .Singular("knife")
    .Plural("knives"));  // Irregular

// Auto-detect common patterns
item.Grammar(en => en.AutoPlural()); // sword → swords
```

**Natural lists:**

```csharp
// "a sword, a shield, and a torch"
var text = items.ToNaturalList(culture: "en-US");

// "ett svärd, en sköld och en fackla"
var text = items.ToNaturalList(culture: "sv-SE");
```

#### 🔤 Command Synonyms

```csharp
// Global synonyms
parser.AddSynonyms("en", "take", "get", "grab", "pick up", "acquire");
parser.AddSynonyms("sv", "ta", "plocka", "hämta", "grip");

// Item-specific
sword.AddAliases("en", "blade", "weapon", "steel");
sword.AddAliases("sv", "klinga", "vapen", "stål");

// Verb normalization
parser.Normalize("pick up") → "take"
parser.Normalize("plocka upp") → "ta"
```

#### 🌐 Multi-language Support

```csharp
// Load language at startup
game.LoadLanguage("sv-SE", "languages/swedish.json");

// Runtime switching
game.SetLanguage("en-US");

// Fallback chain
game.LanguageFallback("sv-SE", "en-US", "en");

// Per-string localization
location.Description(
    ("en", "A dark cave."),
    ("sv", "En mörk grotta."),
    ("de", "Eine dunkle Höhle.")
);
```

---

### Wiki: Advanced Topics

#### ⏰ Time System Deep Dive

```csharp
// Time phases
public enum TimePhase { Dawn, Morning, Midday, Afternoon, Dusk, Evening, Night, Midnight }

// Automatic progression
game.Time.TicksPerPhase = 20;
game.Time.PhasesPerDay = 8;

// Manual control
game.Time.Advance(ticks: 5);
game.Time.SetPhase(TimePhase.Night);

// Time-dependent descriptions
location.Description()
    .At(TimePhase.Day, "Sunlight streams through the windows.")
    .At(TimePhase.Night, "Moonlight casts long shadows.");

// Scheduled events
game.Time.At(TimePhase.Midnight)
    .Every(days: 1)
    .Do(ctx => ctx.SpawnNpc("ghost", "graveyard"));
```

#### 👁️ Perception System

**Light levels:**

```csharp
public enum LightLevel { Pitch, Dark, Dim, Normal, Bright, Blinding }

location.BaseLightLevel(LightLevel.Dark);

// Light sources affect perception
torch.ProvidesLight(LightLevel.Normal, radius: 2);

// Can't see in darkness
if (location.EffectiveLightLevel < LightLevel.Dim)
    return "It's too dark to see.";

// Items revealed at different light levels
item.VisibleAt(LightLevel.Dim, "You make out a shape in the darkness.");
item.VisibleAt(LightLevel.Normal, "A rusty sword lies on the ground.");
```

**Sound:**

```csharp
// Sound propagates through locations
combat.EmitsSound("combat", loudness: 3, radius: 2);

// NPCs react to sound
npc.OnHear("combat")
    .InRange(2)
    .React(ctx => ctx.Investigate());

// Player can hear through walls
location.OnSound("footsteps")
    .From(Direction.North)
    .Message("You hear footsteps from the north.");
```

**Memory:**

```csharp
// Player remembers visited locations
player.Memory.HasVisited("cave");

// NPCs remember interactions
npc.Memory.Remember("player_helped", timestamp: game.Time.Now);
npc.Memory.Recall("player_helped", maxAge: 100); // Within 100 ticks

// Memories fade
npc.Memory.DecayRate = 0.01f; // Per tick
```

#### 🤝 Social System

**Relationships:**

```csharp
// Player-NPC relationship (-100 to +100)
player.SetRelationship("blacksmith", 50);  // Friendly

// Relationship thresholds
npc.DialogWhen(rel => rel < -50, "Get out of my shop, scum!");
npc.DialogWhen(rel > 50, "Ah, my best customer! Special discount today.");

// Actions affect relationships
player.OnAction("give_gift", npc)
    .ShiftRelationship(+10);

player.OnAction("steal_from", npc)
    .ShiftRelationship(-30);
```

**Reputation:**

```csharp
// Faction-wide reputation
player.Reputation.With("village").Set(75);
player.Reputation.With("thieves_guild").Set(-20);

// Reputation spreads
player.OnAction("heroic_deed")
    .SpreadReputation("region", +10, decay: 0.5f);

// NPCs check faction reputation
npc.Faction = "village";
npc.DialogBasedOnFaction(); // Uses player.Reputation.With("village")
```

#### 🎯 Intention System (Goal-based AI)

```csharp
// NPC has goals
npc.Goals.Add(new Goal("survive", priority: 100));
npc.Goals.Add(new Goal("earn_money", priority: 50));
npc.Goals.Add(new Goal("find_love", priority: 30));

// Goals influence behavior
npc.OnLowHealth()
    .If(goal => goal.Is("survive"))
    .Then(flee);

npc.OnSeeItem("gold")
    .If(goal => goal.Is("earn_money"))
    .Then(negotiate);

// Goals can conflict
npc.OnConflict("survive", "protect_child")
    .Resolve(higherPriority); // or custom logic
```

#### 🎭 Narrative Logic

**Theme tracking:**

```csharp
// Themes are tracked across the game
game.Themes.Track(Theme.Redemption);
game.Themes.Track(Theme.Loss);

// Beats affect themes
beat.Mark(Theme.Redemption, +0.2f);

// Query theme state
var dominantTheme = game.Themes.Dominant();
var redemptionProgress = game.Themes.Progress(Theme.Redemption);
```

**Foreshadowing system:**

```csharp
// Plant a seed
game.Foreshadow("dragon_attack")
    .Hint(1, "You notice claw marks on the trees.")
    .Hint(2, "A farmer mentions missing livestock.")
    .Hint(3, "Smoke rises from the mountains.");

// Callback on reveal
game.OnForeshadowComplete("dragon_attack")
    .Do(ctx => ctx.TriggerEvent("dragon_arrives"));

// Players can recall foreshadowing
player.Memory.Foreshadowing.Contains("dragon_attack");
```

#### 🔄 Meta Perspective

**Undo system:**

```csharp
// Every action creates a checkpoint
game.EnableUndo(maxHistory: 50);

// Player can undo
> undo
"Undoing: open chest"

// Certain actions block undo
action.CannotUndo("This action is permanent.");
```

**What-if scenarios:**

```csharp
// Branching exploration (debugging/design tool)
var scenario = game.WhatIf(ctx => {
    ctx.Player.Take("poison");
    ctx.Player.Use("poison").On("king");
});

// Inspect outcome without affecting real state
scenario.Outcome.GetFlag("king_dead"); // true
scenario.Consequences; // List of changes

// Discard or apply
scenario.Discard(); // Nothing happened
// or
scenario.Apply(); // Commit changes to real game
```

---

### Wiki: Game Design Guides (Planning Section)

Dessa sidor hjälper spelskapare att planera innan de kodar.

#### 📝 Planning Your Adventure

**Innehåll:**

1. **Börja med slutet** — Vad är målet? Vad är "vinst"?
2. **Grid-metoden** (Usborne) — Rita kartan på rutat papper först
3. **The Rule of Three** — 3 puzzles, 3 areas, 3 keys
4. **Scope control** — Börja med 5 rum, expandera senare
5. **Playtest loop** — Skriv → Testa → Fixa → Upprepa

**Checklista innan kodning:**

```
□ Karta ritad (rum + kopplingar)
□ Alla rum numrerade/namngivna
□ Mål definierat (vad är "vinst"?)
□ Kritiska items listade (nycklar, verktyg)
□ Minst en väg från start till mål verifierad
□ "Target story" skriven (vad ska spelaren berätta efteråt?)
```

#### 🗺️ Story → Adventure Mapping

**Från berättelse till spelmekanik:**

| Story Element       | Adventure Mechanic             |
| ------------------- | ------------------------------ |
| Karaktärens mål     | Win condition                  |
| Hinder              | Locked doors, puzzles          |
| Viktiga föremål     | Key items                      |
| Bikaraktärer        | NPCs med dialog                |
| Plats               | Location + description         |
| Tid/press           | Turn limits, timed events      |
| Hemligheter         | Hidden rooms, conditional text |
| Karaktärsutveckling | Theme tracking, InnerState     |
| Spänning            | Mood shifts, pacing            |

**Exempel: Rödluvan → Adventure**

| Story                 | Mechanic                                                 |
| --------------------- | -------------------------------------------------------- |
| "Gå till mormors hus" | Goal: reach `grandmas_house`                             |
| "Genom skogen"        | Locations: `home → forest → crossroads → grandmas_house` |
| "Vargen lurar"        | NPC `wolf` med dialog, blocks path                       |
| "Ta inte genvägen"    | Two paths: safe (long) vs dangerous (short)              |
| "Korg med mat"        | Item `basket` required for win                           |
| "Mormor är konstig"   | Conditional description based on `wolf_arrived_first`    |

#### 🔤 Common Verbs & Nouns

**Standard verbs (alla spel bör stödja):**

| Verb        | Aliases                          | Användning               |
| ----------- | -------------------------------- | ------------------------ |
| `go`        | walk, move, head, travel         | Navigation               |
| `look`      | examine, inspect, check, view, l | Describe room/item       |
| `take`      | get, grab, pick, pickup          | Add to inventory         |
| `drop`      | put, leave, discard              | Remove from inventory    |
| `open`      | -                                | Doors, containers        |
| `close`     | shut                             | Doors, containers        |
| `use`       | -                                | Generic item interaction |
| `talk`      | speak, ask, say                  | NPC dialog               |
| `read`      | -                                | Signs, books, letters    |
| `inventory` | i, inv, items                    | Show carried items       |
| `help`      | ?                                | Show commands            |
| `save`      | -                                | Save game                |
| `load`      | restore                          | Load game                |
| `quit`      | exit, q                          | End game                 |

**Genre-specifika verbs:**

| Genre   | Extra Verbs                       |
| ------- | --------------------------------- |
| Combat  | attack, fight, kill, flee, defend |
| Mystery | search, investigate, accuse       |
| Magic   | cast, enchant, dispel             |
| Social  | bribe, threaten, persuade, charm  |
| Stealth | hide, sneak, steal, lockpick      |

**Common nouns (items):**

| Category   | Examples                                 |
| ---------- | ---------------------------------------- |
| Keys       | key, keycard, password, code             |
| Light      | torch, lantern, lamp, candle, flashlight |
| Containers | bag, chest, box, drawer, safe            |
| Tools      | rope, crowbar, hammer, lockpick          |
| Weapons    | sword, knife, gun, staff                 |
| Documents  | letter, book, map, note, diary           |
| Valuables  | gold, gem, coin, treasure, artifact      |

#### 🎯 Framework Simplification

**Hur vårt framework förenklar planeringen:**

| Traditionellt              | Med vårt framework                     |
| -------------------------- | -------------------------------------- |
| Skriv parser från scratch  | `KeywordParser` + synonymer inbyggt    |
| Manuell state-hantering    | `WorldState` + flags/counters          |
| If-else-helvete för dialog | Rule-based dialog system               |
| Svårt att testa            | `Validator.FindUnreachableLocations()` |
| Hårdkodade beskrivningar   | Dynamic descriptions + layers          |
| Ingen struktur för story   | Fluent API + Narrative Arcs            |

**Från plan till kod — 1:1 mapping:**

```
PLAN                          CODE
────                          ────
"Rum: Kök"                →   new Location("kitchen", "Kitchen")
"Utgång norr till hall"   →   .AddExit(Direction.North, hall)
"Låst dörr"               →   .WithDoor().Locked()
"Nyckel i lådan"          →   drawer.Contains(key)
"Prata med kocken"        →   chef.AddDialogRule(...)
"Mål: hitta receptet"     →   quest.Goal(player.Has("recipe"))
```

**Fluent API för narrativ (LINQ-liknande mönster):**

```
PLAN                              FLUENT API
────                              ──────────
"Spelaren vaknar trött"       →   .DefineNeed(Need.WakeUp)
"Kaffe gör en pigg"           →   .When(has("coffee")).Satisfy(Need.WakeUp)
"Stämningen ljusnar"          →   .Shift(s => s.Mood, Mood.Hope)
"Temat är ensamhet → gemenskap" → .DefineThemeArc(Theme.Connection)
```

#### 📐 Planning Templates

**Minimal Adventure (5 rum):**

```
START ──→ HUB ──→ GOAL
           │
           ├──→ PUZZLE_ROOM (has KEY)
           │
           └──→ LOCKED_ROOM (needs KEY, has TREASURE)
```

**Three-Act Adventure:**

```
ACT 1: Setup (3 rum)
  START → VILLAGE → CROSSROADS

ACT 2: Confrontation (5 rum)
  CROSSROADS → FOREST → CAVE → MOUNTAIN → CASTLE_GATE

ACT 3: Resolution (2 rum)
  CASTLE_GATE → THRONE_ROOM → ENDING
```

**Mystery Template:**

```
CRIME_SCENE (clue 1)
     │
     ├──→ SUSPECT_A_LOCATION (clue 2, alibi)
     │
     ├──→ SUSPECT_B_LOCATION (clue 3, motive)
     │
     └──→ HIDDEN_LOCATION (requires clues 1+2+3, reveals TRUTH)
            │
            └──→ CONFRONTATION (accuse with TRUTH)
```

### Wiki: Narrative Arcs

Alla inbyggda narrativa mallar med beskrivning och användningsområden.

| Arc                    | Struktur                                                                | Bra för                           |
| ---------------------- | ----------------------------------------------------------------------- | --------------------------------- |
| **Hero's Journey**     | Call → Trials → Transformation → Return                                 | Klassiska äventyr, fantasy        |
| **Tragic Arc**         | Hybris → Felsteg → Konsekvens → Sen insikt                              | Mörka berättelser, moraliska val  |
| **Transformation Arc** | Fragmenterad → Skuggkonfrontation → Integration → Ny självbild          | Psykologisk mognad, trauma, sorg  |
| **Ensemble Journey**   | Flera hjältar → Växlande perspektiv → Gruppkonflikter → Kollektiv seger | Politik, motstånd, "Jedi Council" |
| **The Descent**        | Nedstigning → Kontrollförlust → Möte med tomhet → Förändrad återkomst   | Psykologisk skräck, Silent Hill   |
| **Spiral Narrative**   | Upprepning → Små variationer → Djupare förståelse                       | Tidsloopar, minnesglitchar        |
| **Moral Labyrinth**    | Inget rätt slut → Alla val kostar → Situationsbunden sanning            | Etik, ledarskap, ansvar           |
| **World-Shift Arc**    | Gradvis världsförändring → Spelaren som katalysator → Ny jämvikt        | Dune-stil, ekologi, politik       |
| **Caretaker Arc**      | Reparera → Hela → Skydda → Kamp mot entropi                             | Mogna, lågmälda spel              |
| **Witness Arc**        | Observera → Samla berättelser → Sanning genom förståelse                | Textmysterier, detektiv           |

### Exempelspel - Creation Styles (ett per stil)

| Stil                    | Spel               | Beskrivning                                  |
| ----------------------- | ------------------ | -------------------------------------------- |
| **Fluent Builder**      | The Haunted Manor  | Klassisk spökhistoria, visar method chaining |
| **Implicit Conversion** | Quick Escape       | Minimalistiskt rum-escape, snabbaste sättet  |
| **Factory/Tuple**       | Dungeon Loot       | Massa items, visar JSON-tänk bulk creation   |
| **DSL**                 | Forest Adventure   | Config-fil driven, visar `.adventure` format |
| **Mixed**               | The Complete Quest | Kombinerar alla stilar, best practices       |

### Exempelspel - Storytelling Features

Dessa ska vara så fluent som möjligt och visa varje storytelling-funktion.

#### 🎭 Narrativ Struktur & Arcs

| Feature                | Spel                  | Visar                              |
| ---------------------- | --------------------- | ---------------------------------- |
| **Hero's Journey**     | The Chosen One        | Full 12-stegs resa                 |
| **Tragic Arc**         | The King's Folly      | Hybris → fall → sen insikt         |
| **Transformation Arc** | Shattered Mirror      | Inre resa, trauma, integration     |
| **Ensemble Journey**   | The Resistance        | Flera hjältar, växlande perspektiv |
| **The Descent**        | Into the Abyss        | Katabasis, psykologisk skräck      |
| **Spiral Narrative**   | Groundhog Dungeon     | Tidsloop med variationer           |
| **Moral Labyrinth**    | The Tribunal          | Inga rätta svar, etiska val        |
| **World-Shift Arc**    | Seeds of Change       | Spelaren som katalysator           |
| **Caretaker Arc**      | The Lighthouse Keeper | Reparera, hela, skydda             |
| **Witness Arc**        | The Collector         | Observera, samla sanningen         |
| **Chapters**           | The Saga              | Akter, kapitelövergångar           |
| **Scene Beats**        | The Interview         | Dialog-driven, dramatiska pauser   |

#### 👥 Karaktärer & Relationer

| Feature              | Spel               | Visar                                     |
| -------------------- | ------------------ | ----------------------------------------- |
| **Character Arcs**   | The Reluctant Hero | NPC utvecklas genom spelarens val         |
| **Emotional Stakes** | The Last Goodbye   | Relationer, förlust, val med konsekvenser |

#### 🌙 Atmosfär & Beskrivningar

| Feature                  | Spel              | Visar                                    |
| ------------------------ | ----------------- | ---------------------------------------- |
| **Mood & Atmosphere**    | The Lighthouse    | Väder, ljus, ljud påverkar beskrivningar |
| **Dynamic Descriptions** | The Living Castle | Rum ändras baserat på tid/händelser      |
| **Narrative Voice**      | Noir Detective    | Berättarröst, stiliserad text            |

#### ⏱️ Spänning & Tempo

| Feature              | Spel           | Visar                                |
| -------------------- | -------------- | ------------------------------------ |
| **Pacing & Tension** | Countdown      | Ökande press, timer-baserad spänning |
| **Foreshadowing**    | Murder Mystery | Ledtrådar som kopplas ihop senare    |
| **Time Triggers**    | The Bomb       | Objekt aktiveras efter tid/händelser |

#### 🎮 Spelarupplevelse

| Feature            | Spel            | Visar                          |
| ------------------ | --------------- | ------------------------------ |
| **Player Agency**  | Branching Paths | Val som faktiskt spelar roll   |
| **Dramatic Irony** | The Traitor     | Spelaren vet mer än karaktären |

### Sandbox kommentarer

Sandbox ska visa alla stilar med kommentarer:

```csharp
// ============================================
// ITEM CREATION STYLES - Choose your favorite!
// ============================================

// 1. FLUENT BUILDER - Most readable, full control
var sword = new Item("sword", "Rusty Sword")
    .SetWeight(2.5f)
    .SetTakeable(true)
    .SetDescription("A worn blade");

// 2. IMPLICIT CONVERSION - Fastest for simple items
Item torch = "Torch";

// 3. FACTORY/TUPLE - JSON-like bulk creation
var items = Items.CreateMany(
    ("gem", "Ruby Gem", 0.1f),
    ("coin", "Gold Coin", 0.05f)
);

// 4. DSL - Most compact, config-file friendly
cave.AddDSLItems(
    "Gem(0.1kg) | A sparkling ruby",
    "Torch",
    "Statue(fixed)"
);

// 5. PARAMS ARRAY - Quick add simple items
hall.AddItems("Sword", "Shield", "Torch");
```

---
