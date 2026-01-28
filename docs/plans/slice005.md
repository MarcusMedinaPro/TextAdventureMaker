## Slice 5: NPCs + Dialog + Movement

**Mål:** NPCs i rum, prata med dem, dialog-träd. NPCs rör sig.

### Task 5.1: INpc + Npc (State pattern — Friendly/Hostile/Dead) ✅

### Task 5.2: Dialog system (Composite — konversationsträd) ✅

### Task 5.3: NPC Movement (Strategy — None, Random, Patrol, Follow) ✅

### Task 5.4: TalkCommand ✅

### Task 5.5: Sandbox — prata med räven, drake patrullerar mellan grottor ✅

### Task 5.6: Rule-Based Dialog System (from Procedural Storytelling)

_Ersätter/kompletterar dialog-träd med flexibla regler._

**Koncept:** Dialog väljs baserat på världstillstånd, inte fördefinierad träd.

```csharp
// Definiera dialog-regler
npc.AddDialogRule("cat_comment")
    .When(ctx => ctx.Player.IsLookingAt("cat"))
    .When(ctx => !ctx.NpcMemory.HasSaid("cat_comment"))
    .Say("A cat! The Internet must be nearby.")
    .Then(ctx => ctx.NpcMemory.MarkSaid("cat_comment"));

// Specificitet vinner - regel med fler kriterier väljs
npc.AddDialogRule("greeting_rain")
    .When(ctx => ctx.FirstMeeting)
    .When(ctx => ctx.World.Weather == Weather.Rain)
    .Say("Terrible weather for traveling, stranger.");

npc.AddDialogRule("greeting_default")
    .When(ctx => ctx.FirstMeeting)
    .Say("Hello, stranger.")
    .Priority(0);  // Fallback

// Memory system
npc.AddDialogRule("remember_name")
    .When(ctx => ctx.NpcMemory.Knows("player_name"))
    .Say($"Ah, {ctx.NpcMemory.Get("player_name")}! Good to see you again.");

// Running gag / progress
npc.AddDialogRule("bird_joke_progress")
    .When(ctx => ctx.NpcMemory.GetCounter("bird_jokes") < 3)
    .Say("Why did the chicken cross the road? To get to the other side!")
    .Then(ctx => ctx.NpcMemory.Increment("bird_jokes"));

npc.AddDialogRule("bird_joke_exhausted")
    .When(ctx => ctx.NpcMemory.GetCounter("bird_jokes") >= 3)
    .Say("I'm all out of bird jokes, I'm afraid.");
```

**Automatiska triggers:**

```csharp
npc.OnSee("player")
    .After(seconds: 2)
    .SayOnce("Over here, traveler!");

npc.OnHear("combat")
    .Flee();
```

### Task 5.7: Synonym System (from Write Your Own Adventure Programs)

_Tillåt flera ord för samma handling._

```csharp
// I parser-konfiguration
parser.AddSynonyms("take", "get", "grab", "pick", "pickup");
parser.AddSynonyms("look", "examine", "inspect", "view", "check");
parser.AddSynonyms("go", "walk", "move", "head", "travel");
parser.AddSynonyms("attack", "hit", "strike", "fight", "kill");

// Item-specifika alias
item.AddAliases("sword", "blade", "weapon");

// Automatisk normalisering i parser
// "get sword" → "take sword"
// "examine blade" → "look sword"
```

**"Did you mean?" suggestions:**

```csharp
parser.EnableSuggestions(true);
// Input: "tke sword"
// Output: "Unknown command. Did you mean 'take sword'?"
```

---
