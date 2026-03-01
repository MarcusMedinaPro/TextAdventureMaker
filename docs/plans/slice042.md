## Slice 42: API Design Philosophy — Fluent Query Extensions

**Mål:** Säkerställa att hela bibliotekets API följer samma fluent, kedjebara mönster som C#-utvecklare känner igen. Svaret på livet, universum och allting.

---

### 🏗️ Arkitektur: Fyra Lager

```
┌─────────────────────────────────────────────────────────┐
│  .adventure DSL                                         │  ← Deklarativt
│  "Skriv äventyr utan C#-kunskap"                        │    (location: kitchen | A kitchen.)
├─────────────────────────────────────────────────────────┤
│  MarcusMedina.TextAdventure.Story                       │  ← Berättarstil
│  "Narrative Query Extensions"                           │    (ThatAre, Has, Takes)
├─────────────────────────────────────────────────────────┤
│  MarcusMedina.TextAdventure.Linq                        │  ← LINQ-kompatibel
│  "LINQ-compatible Query Extensions"                     │    (Where, Select, Any)
├─────────────────────────────────────────────────────────┤
│  MarcusMedina.TextAdventure.Core                        │  ← Kärna
│  Item, Location, GameState, etc.                        │    (Vanlig C#)
└─────────────────────────────────────────────────────────┘

      ↑ Högre = Enklare syntax, mindre kontroll
      ↓ Lägre = Mer kod, full kontroll
```

**Välj ditt lager efter behov:**

| Lager     | Målgrupp                                | Exempel                                  |
| --------- | --------------------------------------- | ---------------------------------------- |
| **DSL**   | Designers, författare, nybörjare        | `location: cave \| A dark cave.`         |
| **Story** | C#-utvecklare som vill ha läsbar kod    | `hero.Takes("sword").AndGoesTo(dungeon)` |
| **Linq**  | C#-utvecklare som vill ha bekant syntax | `items.Where(i => i.IsTakeable).First()` |
| **Core**  | Avancerade användare, biblioteksbyggare | `new Item("sword", "sword", "A sword.")` |

> **OBS om namngivning:** `.Linq`-namespacet innehåller _LINQ-kompatibla query extensions_ för
> TextAdventure-objekt. Detta är **inte** en Microsoft-produkt och är inte affilierat med Microsoft.
> Vi använder samma mönster och metodnamn (Where, Select, etc.) för att ge en bekant upplevelse,
> men all kod är vår egen implementation.

---

### 🎭 Filosofin: Två Stilar, Samma Kraft

TextAdventure erbjuder **två parallella API-stilar** i separata namespaces:

```csharp
using MarcusMedina.TextAdventure.Linq;   // LINQ-compatible query extensions
using MarcusMedina.TextAdventure.Story;  // Narrative query extensions
```

**Varför två stilar?**

1. **LINQ-kompatibel stil** (`TextAdventure.Linq`)
   - Bekant syntax för C#-utvecklare
   - Samma metodnamn som System.Linq (Where, Select, Any, All, First...)
   - Exakt, teknisk, förutsägbar
   - Perfekt för komplexa queries och villkor

2. **Berättarstil** (`TextAdventure.Story`)
   - Kod som läses som prosa
   - Uttrycksfulla metodnamn (ThatAre, Has, Takes, GoesTo...)
   - Narrativ och poetisk
   - Perfekt för att se spelets logik som en historia

**Samma operation, två uttryck:**

```csharp
// === LINQ-STIL ===
// Tekniskt korrekt, bekant för C#-utvecklare
var weapons = player.Inventory
    .Where(item => item.HasTag("weapon"))
    .Where(item => item.Condition > 50)
    .OrderByDescending(item => item.Damage)
    .ToList();

if (player.Inventory.Any(i => i.Id == "healing_potion"))
    player.Health += 50;

// === STORY-STIL ===
// Läses som en berättelse
var weapons = player.Inventory
    .ThatAre("weapon")
    .InGoodCondition()
    .StrongestFirst();

if (player.Has("healing_potion"))
    player.Heals(50);
```

**Det är samma resultat.** Välj den stil som passar din hjärna, ditt team, eller din dag.

---

### 📦 Namespace-struktur

```
MarcusMedina.TextAdventure
├── Linq/                          # LINQ-kompatibla extensions
│   ├── ItemQueryExtensions.cs     # Where, Select, First, Any, All
│   ├── LocationQueryExtensions.cs # Where, Select, First, Any, All
│   ├── NpcQueryExtensions.cs      # Where, Select, First, Any, All
│   └── StateQueryExtensions.cs    # Queries på GameState
│
├── Story/                         # Berättelse-stil extensions
│   ├── ItemStoryExtensions.cs     # ThatAre, ThatCanBeTaken, TheirNames
│   ├── LocationStoryExtensions.cs # ThatHave, WherePlayerCanGo
│   ├── NpcStoryExtensions.cs      # WhoAre, WhoCanSpeak, TheirNames
│   ├── PlayerStoryExtensions.cs   # Has, Takes, Drops, GoesTo
│   └── NarrativeExtensions.cs     # When, Then, Meanwhile, Finally
│
└── Core/                          # Bas-klasser (används av båda)
    ├── Item.cs
    ├── Location.cs
    └── ...
```

---

### 🔄 Komplett LINQ → Story Mappning

#### Filtrering & Sökning

| Kategori           | LINQ-stil                   | Story-stil                      | Äventyrsexempel                    |
| ------------------ | --------------------------- | ------------------------------- | ---------------------------------- |
| Filter             | `Where(i => i.HasTag("x"))` | `ThatAre("x")`                  | `items.ThatAre("weapon")`          |
| Filter             | `Where(i => predicate)`     | `ThatMatch(predicate)`          | `npcs.ThatMatch(n => n.IsHostile)` |
| Finns?             | `Any()`                     | `ThereAreAny()`                 | `if (room.Items.ThereAreAny())`    |
| Finns med villkor? | `Any(i => ...)`             | `ThereExists(...)`              | `ThereExists(i => i.IsKey)`        |
| Alla matchar?      | `All(i => ...)`             | `AllAre(...)` / `Everyone(...)` | `Everyone(n => n.IsFriendly)`      |
| Innehåller?        | `Contains(item)`            | `Includes(item)`                | `inventory.Includes(sword)`        |

#### Välja element

| Kategori            | LINQ-stil               | Story-stil               | Äventyrsexempel                  |
| ------------------- | ----------------------- | ------------------------ | -------------------------------- |
| Första              | `First()`               | `TheFirst()`             | `enemies.TheFirst()`             |
| Första eller null   | `FirstOrDefault()`      | `TheFirstOrNone()`       | `keys.TheFirstOrNone()`          |
| Sista               | `Last()`                | `TheLast()`              | `breadcrumbs.TheLast()`          |
| Sista eller null    | `LastOrDefault()`       | `TheLastOrNone()`        | `clues.TheLastOrNone()`          |
| Enda                | `Single()`              | `TheOnly()` / `TheOne()` | `TheOnly(k => k.FitsLock(door))` |
| Enda eller null     | `SingleOrDefault()`     | `TheOnlyOrNone()`        | `bosses.TheOnlyOrNone()`         |
| På index            | `ElementAt(n)`          | `TheNth(n)`              | `rooms.TheNth(3)`                |
| På index eller null | `ElementAtOrDefault(n)` | `TheNthOrNone(n)`        | `clues.TheNthOrNone(5)`          |

#### Transformation

| Kategori     | LINQ-stil                  | Story-stil           | Äventyrsexempel                 |
| ------------ | -------------------------- | -------------------- | ------------------------------- |
| Transformera | `Select(i => i.Name)`      | `TheirNames()`       | `npcs.TheirNames()`             |
| Transformera | `Select(i => i.X)`         | `Their(i => i.X)`    | `items.Their(i => i.Weight)`    |
| Platta ut    | `SelectMany(i => i.Items)` | `AndTheirContents()` | `containers.AndTheirContents()` |
| Typfilter    | `OfType<Weapon>()`         | `OnlyWeapons()`      | `inventory.OnlyWeapons()`       |
| Typfilter    | `OfType<Key>()`            | `OnlyKeys()`         | `items.OnlyKeys()`              |
| Typfilter    | `OfType<Npc>()`            | `OnlyCharacters()`   | `entities.OnlyCharacters()`     |

#### Sortering

| Kategori         | LINQ-stil                         | Story-stil               | Äventyrsexempel                 |
| ---------------- | --------------------------------- | ------------------------ | ------------------------------- |
| Sortera stigande | `OrderBy(i => i.Name)`            | `Alphabetically()`       | `items.Alphabetically()`        |
| Sortera stigande | `OrderBy(i => i.X)`               | `ByAscending(i => i.X)`  | `ByAscending(i => i.Weight)`    |
| Sortera fallande | `OrderByDescending(i => i.Value)` | `MostValuableFirst()`    | `treasures.MostValuableFirst()` |
| Sortera fallande | `OrderByDescending(i => i.X)`     | `ByDescending(i => i.X)` | `ByDescending(i => i.Damage)`   |
| Sedan sortera    | `ThenBy(i => i.X)`                | `ThenBy(i => i.X)`       | `.ThenBy(i => i.Name)`          |
| Omvänd ordning   | `Reverse()`                       | `InReverseOrder()`       | `path.InReverseOrder()`         |

#### Aggregering

| Kategori          | LINQ-stil            | Story-stil              | Äventyrsexempel              |
| ----------------- | -------------------- | ----------------------- | ---------------------------- |
| Räkna             | `Count()`            | `HowMany()`             | `enemies.HowMany()`          |
| Räkna med villkor | `Count(i => ...)`    | `HowManyAre(...)`       | `HowManyAre(n => n.IsAlive)` |
| Summa             | `Sum(i => i.Weight)` | `TotalWeight()`         | `inventory.TotalWeight()`    |
| Summa             | `Sum(i => i.Value)`  | `TotalValue()`          | `treasures.TotalValue()`     |
| Summa             | `Sum(i => i.X)`      | `TotalOf(i => i.X)`     | `TotalOf(i => i.Damage)`     |
| Min               | `Min(i => i.Weight)` | `TheLightest()`         | `items.TheLightest()`        |
| Min               | `MinBy(i => i.X)`    | `WithLowest(i => i.X)`  | `WithLowest(i => i.Price)`   |
| Max               | `Max(i => i.Damage)` | `TheStrongest()`        | `weapons.TheStrongest()`     |
| Max               | `MaxBy(i => i.X)`    | `WithHighest(i => i.X)` | `WithHighest(i => i.Value)`  |
| Medel             | `Average(i => i.X)`  | `AverageOf(i => i.X)`   | `AverageOf(i => i.Level)`    |

#### Begränsning & Partitionering

| Kategori    | LINQ-stil             | Story-stil          | Äventyrsexempel                 |
| ----------- | --------------------- | ------------------- | ------------------------------- |
| Ta N        | `Take(3)`             | `TheFirstFew(3)`    | `clues.TheFirstFew(3)`          |
| Hoppa N     | `Skip(2)`             | `ExceptTheFirst(2)` | `pages.ExceptTheFirst(2)`       |
| Ta medan    | `TakeWhile(i => ...)` | `UntilYouFind(...)` | `UntilYouFind(i => i.IsGoal)`   |
| Hoppa medan | `SkipWhile(i => ...)` | `AfterYouFind(...)` | `AfterYouFind(i => i.IsMarker)` |
| Chunk       | `Chunk(5)`            | `InGroupsOf(5)`     | `pages.InGroupsOf(5)`           |

#### Mängdoperationer

| Kategori      | LINQ-stil                 | Story-stil                | Äventyrsexempel                    |
| ------------- | ------------------------- | ------------------------- | ---------------------------------- |
| Unika         | `Distinct()`              | `WithoutDuplicates()`     | `visited.WithoutDuplicates()`      |
| Unika på prop | `DistinctBy(i => i.Type)` | `UniqueByType()`          | `items.UniqueByType()`             |
| Union         | `Union(other)`            | `CombinedWith(other)`     | `myItems.CombinedWith(foundItems)` |

---

## Implementation checklist (engine)
- [x] `MarcusMedina.TextAdventure.Linq` query extensions
- [x] `MarcusMedina.TextAdventure.Story` narrative extensions
- [x] Mapping coverage between LINQ and story style

## Example checklist (docs/examples)
- [x] LINQ/Story fluent extension demos (`42_Fluent_Queries.md`)
| Snitt         | `Intersect(other)`        | `InCommonWith(other)`     | `needs.InCommonWith(available)`    |
| Differens     | `Except(other)`           | `ExcludingThoseIn(other)` | `all.ExcludingThoseIn(taken)`      |
| Konkatenera   | `Concat(other)`           | `FollowedBy(other)`       | `path.FollowedBy(returnPath)`      |

#### Materialisering

| Kategori        | LINQ-stil                 | Story-stil        | Äventyrsexempel             |
| --------------- | ------------------------- | ----------------- | --------------------------- |
| Till lista      | `ToList()`                | `Gathered()`      | `treasures.Gathered()`      |
| Till array      | `ToArray()`               | `AsArray()`       | `clues.AsArray()`           |
| Till dictionary | `ToDictionary(i => i.Id)` | `IndexedById()`   | `items.IndexedById()`       |
| Till lookup     | `ToLookup(i => i.Type)`   | `GroupedByType()` | `inventory.GroupedByType()` |

#### Villkor & Default

| Kategori       | LINQ-stil           | Story-stil           | Äventyrsexempel              |
| -------------- | ------------------- | -------------------- | ---------------------------- |
| Default om tom | `DefaultIfEmpty(x)` | `OrIfNone(x)`        | `weapons.OrIfNone(fists)`    |
| Append         | `Append(item)`      | `AndAlso(item)`      | `inventory.AndAlso(newItem)` |
| Prepend        | `Prepend(item)`     | `StartingWith(item)` | `queue.StartingWith(urgent)` |

#### Jämförelse & Join

| Kategori      | LINQ-stil                  | Story-stil               | Äventyrsexempel                 |
| ------------- | -------------------------- | ------------------------ | ------------------------------- |
| Lika sekvens? | `SequenceEqual(other)`     | `MatchesExactly(other)`  | `code.MatchesExactly(solution)` |
| Zip           | `Zip(other, (a,b) => ...)` | `PairedWith(other)`      | `keys.PairedWith(locks)`        |
| Group         | `GroupBy(i => i.Type)`     | `GroupedBy(i => i.Type)` | `GroupedBy(i => i.Category)`    |

---

### 🎮 Äventyrsspecifika Extensions (utöver LINQ)

Story-namespacet har även extensions som inte har LINQ-motsvarighet:

```csharp
// Rumsrelaterade
items.InCurrentRoom()           // Filtrerar till nuvarande rum
items.InLocation(room)          // Filtrerar till specifikt rum
items.NearThePlayer()           // Inom räckhåll
items.VisibleToPlayer()         // Synliga (ej dolda)

// Tillståndsrelaterade
items.ThatCanBeTaken()          // Takeable items
items.ThatAreUsable()           // Har use-action
items.ThatAreLocked()           // Låsta
items.ThatAreOpen()             // Öppna

// NPC-relaterade
npcs.WhoAreHostile()            // Fientliga
npcs.WhoAreFriendly()           // Vänliga
npcs.WhoCanSpeak()              // Har dialog
npcs.WhoAreAlive()              // Levande

// Container-relaterade
containers.ThatContain("key")   // Har specifikt item
containers.ThatAreEmpty()       // Tomma
containers.WithItems()          // Har något i sig

// Narrativa
items.MentionedInStory()        // Refererade i text
items.ImportantToPlot()         // Har quest-koppling
items.SeenBefore()              // Spelaren har sett dem
```

---

### 📋 Universella Extensions per Objekttyp

#### Matris: Vilka extensions gäller var?

| Extension           | Player | Item | NPC | Door | Key | Location |
| ------------------- | :----: | :--: | :-: | :--: | :-: | :------: |
| **Plats**           |
| `.IsIn(location)`   |   ✅   |  ✅  | ✅  |  ✅  | ✅  |    ❌    |
| `.IsInRoom(id)`     |   ✅   |  ✅  | ✅  |  ❌  | ✅  |    ❌    |
| `.IsHere()`         |   ❌   |  ✅  | ✅  |  ✅  | ✅  |    ❌    |
| `.IsNearby()`       |   ❌   |  ✅  | ✅  |  ✅  | ✅  |    ✅    |
| `.IsInInventory()`  |   ❌   |  ✅  | ❌  |  ❌  | ✅  |    ❌    |
| `.IsCarriedBy(who)` |   ❌   |  ✅  | ❌  |  ❌  | ✅  |    ❌    |
| **Inventory**       |
| `.Has(item)`        |   ✅   | ✅\* | ✅  |  ❌  | ❌  |    ✅    |
| `.HasItem(id)`      |   ✅   | ✅\* | ✅  |  ❌  | ❌  |    ✅    |
| `.IsCarrying(item)` |   ✅   | ✅\* | ✅  |  ❌  | ❌  |    ❌    |
| `.HasAny(tag)`      |   ✅   | ✅\* | ✅  |  ❌  | ❌  |    ✅    |
| `.IsEmpty()`        |   ❌   | ✅\* | ✅  |  ❌  | ❌  |    ✅    |
| `.IsFull()`         |   ✅   | ✅\* | ✅  |  ❌  | ❌  |    ❌    |
| **Hälsa & Status**  |
| `.IsAlive()`        |   ✅   |  ❌  | ✅  |  ❌  | ❌  |    ❌    |
| `.IsDead()`         |   ✅   |  ❌  | ✅  |  ❌  | ❌  |    ❌    |
| `.IsWounded()`      |   ✅   |  ❌  | ✅  |  ❌  | ❌  |    ❌    |
| `.IsHealthy()`      |   ✅   |  ❌  | ✅  |  ❌  | ❌  |    ❌    |
| `.IsSleeping()`     |   ✅   |  ❌  | ✅  |  ❌  | ❌  |    ❌    |
| `.IsAwake()`        |   ✅   |  ❌  | ✅  |  ❌  | ❌  |    ❌    |
| `.IsConscious()`    |   ✅   |  ❌  | ✅  |  ❌  | ❌  |    ❌    |
| **Tillstånd**       |
| `.IsOpen()`         |   ❌   | ✅\* | ❌  |  ✅  | ❌  |    ❌    |
| `.IsClosed()`       |   ❌   | ✅\* | ❌  |  ✅  | ❌  |    ❌    |
| `.IsLocked()`       |   ❌   | ✅\* | ❌  |  ✅  | ❌  |    ❌    |
| `.IsUnlocked()`     |   ❌   | ✅\* | ❌  |  ✅  | ❌  |    ❌    |
| `.IsBroken()`       |   ❌   |  ✅  | ❌  |  ✅  | ✅  |    ❌    |
| `.IsWorking()`      |   ❌   |  ✅  | ❌  |  ✅  | ✅  |    ❌    |
| `.IsUsed()`         |   ❌   |  ✅  | ❌  |  ❌  | ✅  |    ❌    |
| `.IsNew()`          |   ❌   |  ✅  | ❌  |  ❌  | ✅  |    ❌    |
| **Synlighet**       |
| `.IsVisible()`      |   ✅   |  ✅  | ✅  |  ✅  | ✅  |    ❌    |
| `.IsHidden()`       |   ❌   |  ✅  | ✅  |  ✅  | ✅  |    ❌    |
| `.IsDiscovered()`   |   ❌   |  ✅  | ✅  |  ✅  | ✅  |    ✅    |
| `.IsExamined()`     |   ❌   |  ✅  | ✅  |  ✅  | ✅  |    ✅    |
| **Förmågor**        |
| `.CanTake()`        |   ❌   |  ✅  | ❌  |  ❌  | ✅  |    ❌    |
| `.CanUse()`         |   ❌   |  ✅  | ❌  |  ❌  | ✅  |    ❌    |
| `.CanOpen()`        |   ❌   | ✅\* | ❌  |  ✅  | ❌  |    ❌    |
| `.CanUnlock()`      |   ❌   | ✅\* | ❌  |  ✅  | ❌  |    ❌    |
| `.CanSpeak()`       |   ✅   |  ❌  | ✅  |  ❌  | ❌  |    ❌    |
| `.CanMove()`        |   ✅   |  ❌  | ✅  |  ❌  | ❌  |    ❌    |
| `.CanSee()`         |   ✅   |  ❌  | ✅  |  ❌  | ❌  |    ❌    |
| `.CanReach(target)` |   ✅   |  ❌  | ✅  |  ❌  | ❌  |    ❌    |

_\* = Endast för containers/lockable items_

---

#### 🧑 Player Extensions

```csharp
// === HÄLSA ===
player.IsAlive()                    // health > 0
player.IsDead()                     // health <= 0
player.IsWounded()                  // health < maxHealth
player.IsHealthy()                  // health == maxHealth
player.IsNearDeath()                // health < 10%
player.HealthPercentage()           // 0-100

// === TILLSTÅND ===
player.IsSleeping()
player.IsAwake()
player.IsConscious()
player.IsPoisoned()
player.IsBlinded()
player.IsConfused()
player.IsFighting()
player.IsResting()
player.IsMoving()
player.IsTalking()

// === INVENTORY ===
player.Has("sword")                 // Har item med id
player.Has(sword)                   // Har item-objekt
player.HasItem("sword")             // Alias
player.HasAny("weapon")             // Har något med tag
player.HasAll("key", "map")         // Har alla
player.IsCarrying(item)             // Samma som Has
player.IsHolding(item)              // I aktiv hand
player.IsWearing(item)              // Har på sig
player.IsEquipped(item)             // Utrustad med
player.InventoryCount()             // Antal items
player.InventoryWeight()            // Total vikt
player.IsFull()                     // Kan inte bära mer
player.CanCarry(item)               // Har plats för
player.FreeSpace()                  // Återstående kapacitet

// === PLATS ===
player.IsIn(location)               // I specifikt rum
player.IsInRoom("kitchen")          // I rum med id
player.IsAt(location)               // Alias
player.CurrentLocation              // Nuvarande rum
player.WasIn(location)              // Har varit där
player.HasVisited(location)         // Har besökt
player.VisitedRooms()               // Lista på besökta

// === FÖRMÅGOR ===
player.CanSee()                     // Inte blind, har ljus
player.CanMove()                    // Inte paralyserad
player.CanSpeak()                   // Inte tystad
player.CanTake(item)                // Kan ta upp
player.CanReach(target)             // Inom räckhåll
player.CanAttack(target)            // Kan attackera
player.CanOpen(door)                // Har rätt nyckel
player.CanUnlock(door)              // Har rätt nyckel

// === RELATIONER ===
player.Knows(npc)                   // Har pratat med
player.IsFriendsWith(npc)           // Relation > 50
player.IsEnemyOf(npc)               // Relation < 0
player.RelationshipWith(npc)        // Numeriskt värde
player.HasMet(npc)                  // Har träffat

// === QUEST/PROGRESS ===
player.HasFlag("found_treasure")    // Flag är satt
player.FlagValue("attempts")        // Räknarvärde
player.HasCompleted("quest1")       // Quest klar
player.IsOnQuest("quest1")          // Aktiv quest
player.Score()                      // Poäng
```

---

#### 📦 Item Extensions

```csharp
// === PLATS ===
item.IsIn(location)                 // I specifikt rum
item.IsInRoom("kitchen")            // I rum med id
item.IsHere()                       // I spelarens rum
item.IsNearby()                     // I angränsande rum
item.IsInInventory()                // Spelaren har den
item.IsCarriedBy(player)            // Bärs av specifik
item.IsCarriedBy(npc)               // NPC har den
item.IsInContainer(container)       // I behållare
item.IsOnGround()                   // Ligger i rum
item.WhereIs()                      // Returnerar location

// === TILLSTÅND ===
item.IsBroken()                     // Trasig
item.IsWorking()                    // Fungerar
item.IsNew()                        // Oanvänd
item.IsUsed()                       // Har använts
item.IsDamaged()                    // Delvis skadad
item.ConditionPercentage()          // 0-100%

// === SYNLIGHET ===
item.IsVisible()                    // Synlig
item.IsHidden()                     // Gömd
item.IsDiscovered()                 // Har hittats
item.IsExamined()                   // Har undersökts
item.IsRevealed()                   // Avslöjad (var gömd)

// === FÖRMÅGOR ===
item.CanTake()                      // Kan tas upp (takeable)
item.CanUse()                       // Har use-action
item.CanDrop()                      // Kan släppas
item.CanThrow()                     // Kan kastas
item.CanEquip()                     // Kan utrustas
item.CanEat()                       // Ätbar
item.CanDrink()                     // Drickbar
item.CanRead()                      // Läsbar
item.CanOpen()                      // Kan öppnas (container)
item.CanClose()                     // Kan stängas
item.CanLock()                      // Kan låsas
item.CanUnlock()                    // Kan låsas upp

// === EGENSKAPER ===
item.IsHeavy()                      // Vikt > threshold
item.IsLight()                      // Vikt < threshold
item.IsValuable()                   // Värde > threshold
item.IsWorthless()                  // Värde == 0
item.IsWeapon()                     // HasTag("weapon")
item.IsArmor()                      // HasTag("armor")
item.IsFood()                       // HasTag("food")
item.IsContainer()                  // Kan innehålla saker
item.IsKey()                        // Är en nyckel
item.IsTool()                       // HasTag("tool")
item.IsLight()                      // Ger ljus

// === CONTAINER (om item är container) ===
item.IsOpen()                       // Container är öppen
item.IsClosed()                     // Container är stängd
item.IsLocked()                     // Container är låst
item.IsUnlocked()                   // Container är olåst
item.IsEmpty()                      // Inga items i
item.IsFull()                       // Kan inte ta mer
item.Contains("key")                // Har specifikt item
item.ContainsAny("weapon")          // Har item med tag
item.ItemCount()                    // Antal items i
item.Contents()                     // Lista på innehåll

// === KOMBINATION ===
item.CanCombineWith(other)          // Kan kombineras
item.CombinesWith()                 // Lista på möjliga
item.ResultOfCombining(other)       // Vad blir det?
```

---

#### 🧙 NPC Extensions

```csharp
// === HÄLSA ===
npc.IsAlive()
npc.IsDead()
npc.IsWounded()
npc.IsHealthy()
npc.IsDying()
npc.IsUnconscious()

// === TILLSTÅND ===
npc.IsSleeping()
npc.IsAwake()
npc.IsAlert()
npc.IsDistracted()
npc.IsBusy()
npc.IsIdle()
npc.IsFighting()
npc.IsFleeing()
npc.IsFollowing(player)
npc.IsGuarding(location)
npc.IsPatrolling()

// === PLATS ===
npc.IsIn(location)
npc.IsInRoom("tavern")
npc.IsHere()                        // I spelarens rum
npc.IsNearby()                      // I angränsande rum
npc.IsVisible()                     // Synlig för spelaren
npc.IsHidden()                      // Gömd

// === RELATION ===
npc.IsHostile()                     // Fientlig
npc.IsFriendly()                    // Vänlig
npc.IsNeutral()                     // Neutral
npc.IsScared()                      // Rädd
npc.IsAngry()                       // Arg
npc.IsHappy()                       // Glad
npc.Knows(player)                   // Känner spelaren
npc.Trusts(player)                  // Litar på spelaren
npc.Fears(player)                   // Är rädd för spelaren
npc.RelationshipWith(player)        // Numeriskt

// === FÖRMÅGOR ===
npc.CanSpeak()                      // Har dialog
npc.CanTrade()                      // Kan handla
npc.CanFight()                      // Kan slåss
npc.CanFollow()                     // Kan följa
npc.CanTeach()                      // Kan lära ut
npc.CanHeal()                       // Kan hela
npc.CanGive(item)                   // Kan ge item
npc.WillTalk()                      // Villig att prata nu
npc.WillTrade()                     // Villig att handla nu
npc.WillHelp()                      // Villig att hjälpa

// === INVENTORY ===
npc.Has(item)
npc.HasItem("sword")
npc.HasAny("weapon")
npc.IsCarrying(item)
npc.IsEquipped(item)
npc.IsWearing(item)
npc.WillSell(item)                  // Vill sälja
npc.WillBuy(item)                   // Vill köpa
npc.PriceFor(item)                  // Pris

// === DIALOG ===
npc.HasSaidTo(player, "topic")      // Har nämnt
npc.KnowsAbout("treasure")          // Vet om
npc.WillTellAbout("secret")         // Villig berätta
npc.HasQuest()                      // Har uppdrag
npc.QuestIsAvailable()              // Uppdrag tillgängligt
npc.QuestIsComplete()               // Uppdrag klart
```

---

#### 🚪 Door Extensions

```csharp
// === TILLSTÅND ===
door.IsOpen()
door.IsClosed()
door.IsLocked()
door.IsUnlocked()
door.IsBroken()                     // Kan inte öppnas/låsas
door.IsBlocked()                    // Blockerad av något
door.IsJammed()                     // Kärvar

// === EGENSKAPER ===
door.RequiresKey()                  // Behöver nyckel
door.RequiresKey(key)               // Behöver specifik nyckel
door.CanBePickedLocked()            // Kan dyrkas
door.CanBeBrokenDown()              // Kan slås in
door.CanBePeekedThrough()           // Kan kika genom

// === KOPPLINGAR ===
door.Connects(room1, room2)         // Kopplar rum
door.LeadsTo()                      // Vart leder den?
door.LeadsFrom()                    // Varifrån?
door.IsExitFrom(location)           // Är utgång från
door.IsEntranceTo(location)         // Är ingång till

// === SYNLIGHET ===
door.IsVisible()
door.IsHidden()                     // Hemlig dörr
door.IsDiscovered()
door.IsObvious()                    // Uppenbar
door.IsSecret()                     // Hemlig

// === INTERAKTION ===
door.CanOpen()                      // Kan öppnas nu
door.CanClose()                     // Kan stängas nu
door.CanLock()                      // Kan låsas nu
door.CanUnlock()                    // Kan låsas upp nu
door.CanPass()                      // Kan passera nu
door.WasUsed()                      // Har använts
door.TimesUsed()                    // Antal gånger
```

---

#### 🔑 Key Extensions

```csharp
// === PLATS ===
key.IsIn(location)
key.IsInRoom("bedroom")
key.IsHere()
key.IsInInventory()
key.IsCarriedBy(player)
key.IsCarriedBy(npc)
key.IsHidden()
key.IsVisible()
key.IsDiscovered()

// === TILLSTÅND ===
key.IsUsed()                        // Har använts
key.IsNew()                         // Aldrig använts
key.IsBroken()                      // Trasig
key.IsWorking()                     // Fungerar

// === KOPPLINGAR ===
key.Opens(door)                     // Öppnar specifik dörr
key.OpensAny()                      // Lista på vad den öppnar
key.FitsLock(door)                  // Passar i låset
key.IsMasterKey()                   // Öppnar flera
key.IsOneTimeUse()                  // Förbrukas vid användning

// === EGENSKAPER ===
key.CanTake()
key.CanUse()
key.CanCopy()                       // Kan kopieras
```

---

#### 🏠 Location Extensions

```csharp
// === INNEHÅLL ===
location.HasItem(item)              // Har specifikt item
location.HasItem("sword")           // Har item med id
location.HasAny("weapon")           // Har item med tag
location.HasNpc(npc)                // Har specifik NPC
location.HasNpc("guard")            // Har NPC med id
location.HasPlayer()                // Spelaren är här
location.IsEmpty()                  // Inga items
location.IsDeserted()               // Inga NPCs
location.IsOccupied()               // Har NPCs
location.ItemCount()                // Antal items
location.NpcCount()                 // Antal NPCs

// === TILLSTÅND ===
location.IsLit()                    // Har ljus
location.IsDark()                   // Mörkt
location.IsVisited()                // Har besökts
location.IsUnvisited()              // Aldrig besökt
location.IsExplored()               // Fullt utforskat
location.IsSafe()                   // Inga fiender
location.IsDangerous()              // Har fiender
location.IsLocked()                 // Inlåst (kan ej lämna)
location.IsAccessible()             // Går att nå

// === UTGÅNGAR ===
location.HasExit(direction)         // Har utgång
location.HasExitTo(other)           // Har utgång till
location.ExitCount()                // Antal utgångar
location.Exits()                    // Lista på utgångar
location.ExitDirections()           // Lista på riktningar
location.IsDeadEnd()                // Endast en utgång
location.IsHub()                    // Många utgångar (>3)

// === DÖRRAR ===
location.HasDoor(direction)         // Har dörr i riktning
location.HasLockedDoor()            // Har låst dörr
location.HasOpenDoor()              // Har öppen dörr
location.DoorsCount()               // Antal dörrar
location.Doors()                    // Lista på dörrar

// === EGENSKAPER ===
location.IsIndoors()
location.IsOutdoors()
location.IsUnderground()
location.IsUnderwater()
location.IsHighUp()                 // Högt upp
location.HasWater()
location.HasFire()
location.Temperature()              // Temperatur

// === NARRATIV ===
location.IsStartLocation()          // Startplats
location.IsGoalLocation()           // Målplats
location.IsCheckpoint()             // Checkpoint
location.IsBossRoom()               // Bossrum
location.IsShop()                   // Butik
location.IsSavePoint()              // Sparplats
```

---

### 💡 Kombinera stilar

Du kan mixa! Båda returnerar samma typer:

```csharp
using MarcusMedina.TextAdventure.Linq;
using MarcusMedina.TextAdventure.Story;

// Börja med LINQ, avsluta med Story
var result = player.Inventory
    .Where(i => i.Weight < 5)      // LINQ: precis filtrering
    .ThatAre("valuable")            // Story: läsbar tag-check
    .OrderByDescending(i => i.Value) // LINQ: exakt sortering
    .TheFirstFew(3);                // Story: läsbar limit

// Eller tvärtom
if (room.Items.ThatAre("weapon").Any(w => w.Damage > 50))
    hero.Says("Now we're talking!");
```

---

### 🎬 Berättelse-stil i praktiken

Kod som läses som en historia:

```csharp
// Kapitel 1: Hjälten vaknar
When(hero.IsInLocation("bedroom"))
    .And(timeOfDay.IsMorning())
    .Then(hero.WakesUp())
    .AndSays("Another day, another adventure.");

// Kapitel 2: Faran lurar
if (hero.IsWounded)
{
    doctor.ArrivesAt(hero.Location);
    doctor.Says("Hold still, this is goind to hurt you more than me.");
    hero.Heals(30);
}

// Kapitel 3: Skatten hittas
var treasures = dungeon.Items
    .ThatAre("valuable")
    .ThatCanBeTaken()
    .NotGuardedByMonsters();

foreach (var treasure in treasures)
{
    hero.Takes(treasure);
    narrator.Says($"The {treasure.Name} glimmers in the torchlight.");
}

// Kapitel 4: Draken
When(dragon.IsSleeping)
    .TheHero.CanSneak().PastIt()
    .Otherwise()
    .TheHero.MustFight().OrFlee();
```

---

### 🔧 Teknisk implementation

Båda stilarna är tunna wrappers runt samma logik:

```csharp
// I Linq-namespace
public static class ItemQueryExtensions
{
    public static IEnumerable<Item> Where(
        this IEnumerable<Item> items,
        Func<Item, bool> predicate)
        => System.Linq.Enumerable.Where(items, predicate);
}

// I Story-namespace
public static class ItemStoryExtensions
{
    public static IEnumerable<Item> ThatAre(
        this IEnumerable<Item> items,
        string tag)
        => items.Where(i => i.HasTag(tag));  // Använder LINQ internt

    public static IEnumerable<Item> ThatCanBeTaken(
        this IEnumerable<Item> items)
        => items.Where(i => i.IsTakeable);
}
```

**Inga prestandaskillnader** — Story-stil kompileras till samma IL som LINQ-stil.

---

### Kärnkoncept

```
State → Transition → Meaning
(Var vi är) → (Vad som händer) → (Varför det spelar roll)
```

### Task 42.1: IStoryState — Narrativt tillstånd

```csharp
public interface IStoryState
{
    // Yttre tillstånd
    ILocation Location { get; }
    Mood WorldMood { get; }
    TimePhase TimePhase { get; }

    // Inre tillstånd
    InnerState PlayerInnerState { get; }
    IReadOnlyCollection<Need> ActiveNeeds { get; }

    // Tematisk tracking
    IReadOnlyDictionary<Theme, float> ThemeProgress { get; }

    // Minne
    IReadOnlyCollection<string> Memories { get; }
    IReadOnlyCollection<string> RevealedTruths { get; }
}

public enum Mood
{
    Stillness, Fear, Hope, Decay, Trust,
    Awakening, Tension, Release, Wonder, Dread
}

public enum InnerState
{
    Drowsy, Groggy, Awake, Alert, Focused,
    Confused, Determined, Guilty, Healed, Present
}

public enum Need
{
    WakeUp, Safety, Connection, Understanding,
    Closure, Revenge, Redemption, Rest
}

public enum Theme
{
    Trust, Loss, Growth, Corruption, Hope,
    Isolation, Connection, Power, Sacrifice, Truth
}
```

### Task 42.2: LINQ-liknande Extension Methods

```csharp
// Dessa extension methods följer LINQ-mönstret:
// - Kedjebara (returnerar samma/liknande typ)
// - Använder lambda-uttryck för flexibilitet
// - Läsbara som naturligt språk

public static class StoryExtensions
{
    // === QUERY OPERATORS (som Where/Any i LINQ) ===

    // When - precondition (motsvarar Where)
    public static IStoryQuery When(this IStory story,
        Func<IStoryState, bool> condition);

    // And - kombinera villkor
    public static IStoryQuery And(this IStoryQuery query,
        Func<IStoryState, bool> condition);

    // Or - alternativa villkor
    public static IStoryQuery Or(this IStoryQuery query,
        Func<IStoryState, bool> condition);

    // === TRANSITION OPERATORS ===

    // Shift - ändra tillstånd
    public static IStoryQuery Shift<T>(this IStoryQuery query,
        Expression<Func<IStoryState, T>> property, T newValue);

    // ShiftGradually - ändra över tid
    public static IStoryQuery ShiftGradually<T>(this IStoryQuery query,
        Expression<Func<IStoryState, T>> property, T target, int ticks);

    // === MEANING OPERATORS ===

    // Mark - påverka tema
    public static IStoryQuery Mark(this IStoryQuery query,
        Theme theme, float delta);

    // Satisfy - uppfyll behov
    public static IStoryQuery Satisfy(this IStoryQuery query, Need need);

    // Awaken - skapa nytt behov
    public static IStoryQuery Awaken(this IStoryQuery query, Need need);

    // === NARRATIVE OPERATORS ===

    // Reveal - visa text/sanning
    public static IStoryQuery Reveal(this IStoryQuery query, string text);

    // Remember - lägg till minne
    public static IStoryQuery Remember(this IStoryQuery query, string memory);

    // Echo - referera till tidigare minne
    public static IStoryQuery Echo(this IStoryQuery query, string memoryPattern);

    // === EXECUTION ===

    // AsBeats - materialisera till story beats
    public static IEnumerable<IStoryBeat> AsBeats(this IStoryQuery query);

    // Execute - kör direkt
    public static StoryResult Execute(this IStoryQuery query);
}
```

### Task 42.3: Fluent Beat Builder

```csharp
// Definiera ett beat
story.DefineBeat("morning_coffee")
    .When(s => s.Player.Has("coffee"))
    .And(s => s.Location.Id == "livingroom")
    .And(s => s.PlayerInnerState <= InnerState.Groggy)
    .Shift(s => s.WorldMood, Mood.Awakening)
    .Shift(s => s.PlayerInnerState, InnerState.Awake)
    .Satisfy(Need.WakeUp)
    .Mark(Theme.Connection, +0.1)  // Morgonritualen binder till hemmet
    .Reveal("Warmth spreads through you. The day feels possible now.");

// Beats kan ärva från varandra
story.DefineBeat("morning_coffee_with_book")
    .Extends("morning_coffee")
    .And(s => s.Player.Has("book"))
    .Shift(s => s.PlayerInnerState, InnerState.Present)
    .Mark(Theme.Growth, +0.2)
    .Reveal("The words on the page find a quiet mind.");
```

### Task 42.4: Composable Story Fragments

```csharp
// Återanvändbara fragment (som LINQ-metoder)
public static class StoryFragments
{
    public static IStoryQuery HasMorningEssentials(this IStoryQuery q) =>
        q.And(s => s.Player.Has("coffee"))
         .And(s => s.TimePhase == TimePhase.Morning);

    public static IStoryQuery IsCalm(this IStoryQuery q) =>
        q.And(s => s.WorldMood.In(Mood.Stillness, Mood.Hope))
         .And(s => s.PlayerInnerState >= InnerState.Awake);

    public static IStoryQuery SeekingRest(this IStoryQuery q) =>
        q.And(s => s.ActiveNeeds.Contains(Need.Rest));
}

// Användning - läsbar som en mening
story.DefineBeat("peaceful_reading")
    .When(s => s.Location.HasTag("comfortable"))
    .HasMorningEssentials()
    .IsCalm()
    .And(s => s.Player.Has("book"))
    .Shift(s => s.PlayerInnerState, InnerState.Present)
    .Satisfy(Need.Rest)
    .Mark(Theme.Connection, +0.3);
```

### Task 42.5: Narrative Query — Fråga berättelsen

```csharp
// Vilka beats kan aktiveras nu?
var possibleBeats = story.Beats
    .Where(b => b.CanActivate(currentState))
    .OrderByDescending(b => b.Specificity)  // Mest specifika först
    .ToList();

// Vilka behov är ouppfyllda?
var unmetNeeds = story.State.ActiveNeeds
    .Where(n => !n.IsSatisfied)
    .OrderBy(n => n.Urgency);

// Vilka teman utvecklas?
var risingThemes = story.State.ThemeProgress
    .Where(t => t.Value > 0.3f && t.Value < 0.8f)
    .Select(t => t.Key);

// Hitta alla vägar till ett mål-tillstånd
var pathsToHope = story.FindPaths(
    from: currentState,
    to: s => s.WorldMood == Mood.Hope,
    maxDepth: 10
);

// Vilka minnen är relevanta här?
var echoes = story.State.Memories
    .Where(m => m.RelatesTo(currentState.Location))
    .Select(m => m.Echo());
```

### Task 42.6: Three-Layer Output

```csharp
// Samma beat, tre uttrycksnivåer
story.DefineBeat("open_mysterious_door")
    .When(s => s.Player.Uses("key").On("ancient_door"))
    .Shift(s => s.WorldMood, Mood.Dread)
    .Awaken(Need.Understanding)
    .Mark(Theme.Truth, +0.2)
    .Mark(Theme.Loss, +0.1)

    // Tre nivåer av berättande
    .Mechanical("The door opens. Cold air escapes.")
    .Narrative("Some doors are closed for a reason. This one remembers.")
    .Psychological("A weight settles in your chest. You wanted to know.");

// Motorn väljer nivå baserat på:
// - Spelarinställning (prefer_mechanical, prefer_narrative, prefer_psychological)
// - Tematisk intensitet (hög Theme.Truth → mer psychological)
// - Pacing (snabba sekvenser → mechanical, lugna → narrative)
```

### Task 42.7: Character Arcs som Queries

```csharp
// NPC har sin egen narrativa kurva
var hermit = story.DefineCharacter("hermit")
    .WithWound("lost_family")
    .WithMask("cynicism")
    .WithNeed("connection")
    .TransformsWhen(s =>
        s.Relationship("hermit") > 50 &&
        s.ThemeProgress[Theme.Trust] > 0.6f
    );

// Karaktärens dialog som query
hermit.Says()
    .When(s => s.FirstMeeting)
    .And(s => s.WorldMood == Mood.Fear)
    .Line("Another lost soul. The forest collects them.")
    .Mark(Theme.Isolation, +0.1);

hermit.Says()
    .When(s => s.Relationship("hermit") > 30)
    .And(s => s.Player.Has("family_locket"))
    .Line("That locket... where did you find it?")
    .Shift(s => hermit.Mask, "vulnerability")
    .Awaken(Need.Closure)
    .Mark(Theme.Connection, +0.3);
```

### Task 42.8: Theme Arc Tracking

```csharp
// Definiera tematisk båge
story.DefineThemeArc(Theme.Trust)
    .StartsAt(0.0f, "Isolation", "You trust no one. Wise.")
    .DevelopsAt(0.3f, "First Doubt", "Perhaps not everyone is an enemy.")
    .DevelopsAt(0.5f, "Tentative Bond", "You find yourself caring.")
    .DevelopsAt(0.7f, "Tested", "Trust is easy until it costs something.")
    .ResolvesAt(1.0f, "Mutual Dependence", "Together, or not at all.");

// Query: Var är vi i bågen?
var trustPhase = story.GetThemePhase(Theme.Trust);
// => "Tentative Bond" (om ThemeProgress[Trust] == 0.55)

// Automatiska callbacks vid trösklar
story.OnThemeReaches(Theme.Trust, 0.5f, ctx => {
    ctx.Reveal("Something has shifted. You catch yourself hoping.");
    ctx.TriggerBeat("trust_milestone");
});
```

### Task 42.9: DSL för Narrative Beats

```dsl
# Deklarativ syntax för icke-programmerare

beat "morning_peace" {
    when player.has("coffee")
     and player.has("book")
     and location.tag("comfortable")
     and time.phase == morning

    shift mood to stillness
    shift inner to present
    satisfy need:rest

    mark theme:connection +0.3
    mark theme:growth +0.1

    reveal "The world holds its breath. So do you."
}

character "hermit" {
    wound: "lost_family"
    mask: "cynicism"
    need: "connection"

    transforms when relationship > 50 and theme:trust > 0.6

    says when first_meeting and mood == fear {
        "Another lost soul. The forest collects them."
    }
}

theme_arc "trust" {
    0.0: "Isolation" -> "You trust no one."
    0.3: "First Doubt" -> "Perhaps not everyone is an enemy."
    0.5: "Tentative Bond" -> "You find yourself caring."
    0.7: "Tested" -> "Trust costs something."
    1.0: "Mutual Dependence" -> "Together, or not at all."
}
```

### Task 42.10: Integration med existerande slices

```csharp
// Fluent narrative API integrerar med Event System (Slice 6)
story.OnEvent("player_enters_location")
    .When(s => s.Location.HasTag("childhood_home"))
    .And(s => s.State.Memories.Contains("happy_childhood"))
    .Echo("happy_childhood")
    .Shift(s => s.WorldMood, Mood.Stillness)
    .Reveal("The smell of old wood. You remember.");

// Integration med NPC Dialog (Slice 5)
npc.DialogRule()
    .When(s => s.ThemeProgress[Theme.Trust] > 0.5f)
    .And(s => s.PlayerInnerState == InnerState.Present)
    .Say("You're different now. Calmer. What changed?")
    .Mark(Theme.Growth, +0.1);

// Integration med Quest System (Slice 8)
quest.OnComplete()
    .Shift(s => s.WorldMood, Mood.Hope)
    .Satisfy(Need.Closure)
    .Mark(Theme.Sacrifice, +0.4)
    .Reveal("It cost everything. It was worth everything.");
```

### Task 42.11: Sandbox — Morning Ritual

```csharp
// Komplett implementation av kaffe-och-bok-scenariot

var morningGame = new GameBuilder()
    .WithNarrativeBeats()  // Aktivera narrative beat system
    .Build();

// Definiera tillstånd
morningGame.DefineNeeds(Need.WakeUp, Need.Rest);
morningGame.DefineThemes(Theme.Connection, Theme.Growth);

// Definiera beats
morningGame.DefineBeat("leave_bed")
    .When(s => s.Location.Id == "bedroom")
    .And(s => s.PlayerPosture == Posture.Lying)
    .Shift(s => s.PlayerPosture, Posture.Standing)
    .Shift(s => s.PlayerInnerState, InnerState.Groggy)
    .Mechanical("You get out of bed.")
    .Narrative("The day begins, whether you want it to or not.")
    .Psychological("Inertia broken. Now what?");

morningGame.DefineBeat("brew_coffee")
    .When(s => s.Location.Id == "kitchen")
    .And(s => s.Player.Has("mug"))
    .And(s => s.ActiveNeeds.Contains(Need.WakeUp))
    .Yields("coffee")
    .Shift(s => s.WorldMood, Mood.Awakening)
    .Shift(s => s.PlayerInnerState, InnerState.Awake)
    .Mark(Theme.Connection, +0.1)
    .Mechanical("Coffee made.")
    .Narrative("Warmth enters the body. Consciousness returns.")
    .Psychological("A small act of self-care.");

morningGame.DefineBeat("peaceful_reading")
    .When(s => s.Location.Id == "livingroom")
    .And(s => s.Player.Has("coffee"))
    .And(s => s.Player.Has("book"))
    .Shift(s => s.PlayerPosture, Posture.Sitting)
    .Shift(s => s.PlayerInnerState, InnerState.Present)
    .Shift(s => s.WorldMood, Mood.Stillness)
    .Satisfy(Need.WakeUp)
    .Satisfy(Need.Rest)
    .Mark(Theme.Connection, +0.3)
    .Mark(Theme.Growth, +0.2)
    .Mechanical("You sit down, drink coffee, read.")
    .Narrative("The world quiets. The book opens like a small, private universe.")
    .Psychological("Arrival. Presence. Enough.");

// Dramaturgisk båge i 4 beats:
// Inertia → Awakening → Comfort → Presence
```

---

En tanke på hur det ska fungera

_storytelling som kodstruktur_, inte som löpande text.

Om vi tänker rent arkitektoniskt kan du se berättelsen som tre lager som samverkar:

1. **Tillstånd (State)** – hur världen _är_
2. **Händelser (Events)** – vad som _händer_
3. **Mening (Narrative Logic)** – varför det _betyder något_

I kod kan det bli ungefär så här.

---

### 1. Story som tillståndsmaskin

Berättelsen är inte en linje, utan ett nät av tillstånd:

```csharp
StoryState {
    WorldMood: Fear | Hope | Decay | Trust
    PlayerInnerState: Confused | Determined | Guilty | Healed
    Factions: { Order: Weakening, Chaos: Rising }
}
```

Alltså: inte “kapitel 1, kapitel 2”, utan _existentiella lägen_.

---

### 2. Story Beats som kodobjekt

Varje betydelsefull händelse är ett objekt med:

```csharp
StoryBeat {
    Id: "MeetTheHermit"
    Preconditions: Player.Has("Lantern") && WorldMood == Fear
    Effect:
        WorldMood = Hope
        PlayerInnerState = LessAlone
    Symbol: "Light in Darkness"
    MemoryTag: "FirstTrust"
}
```

Detta är inte bara quest-logik.
Det är dramaturgisk semantik.

---

### 3. Tematiska bågar som system, inte manus

```csharp
ThemeArc "Trust" {
    StartsAt: Isolation
    DevelopsThrough: Betrayal, Repair
    ResolvesAt: MutualDependence
}
```

Och varje StoryBeat deklarerar vilket tema den påverkar:

```csharp
AffectsTheme("Trust", +0.2)
```

---

### 4. Karaktärer som narrativa processer

NPC är inte bara state machines, utan _pågående berättelser_:

```csharp
CharacterArc "Hermit" {
    Wound: "LostFamily"
    Mask: "Cynical"
    Need: "Connection"
    TransformationCondition: PlayerShowsConsistency
}
```

---

### 5. Orsak → Konsekvens → Betydelse

Varje handling får tre lager:

```csharp
Action: OpenDoor
Consequence: ReleaseMonster
Meaning: "Curiosity has a cost"
```

Motorn kan då visa olika berättarröster:

– Mekanisk: “The door opens. A creature emerges.”
– Narrativ: “Some things were sealed for a reason.”
– Psykologisk: “You feel responsible.”

---

### 6. Struktur som kan bli DSL

Exempel på framtida “Story DSL”:

```dsl
beat "LightInTheDark" {
    when player.enters Cave and player.has Lantern
    shift world.mood from Fear to Hope
    reveal memory "You are not alone"
    mark theme Trust += 1
}
```

---

Om jag ska sammanfatta kärnan:

Berättelse i kod =
inte text → struktur
inte sekvens → relation
inte plot → tillståndsförändring med mening

Du bygger inte:

“En historia.”

Du bygger:

“Ett system som skapar meningsfull förändring.”

## Exempel

OK vi säger exempelvis ett spel där spelaren ligger i sin säng och ska gå till köket göra kaffe, ta kaffe, gå till vardagsrum, hämta en bok, sätta sig i soffan dricka kaffe och läsa boken hur skulle det beskrivas i detta fluent state

Bra, det här är ett perfekt mikroscenario att visa hur “story som tillstånd” fungerar.

Vi beskriver det inte som steg, utan som **meningsfulla övergångar mellan tillstånd**.

Först definierar vi världens och spelarens narrativa tillstånd:

```csharp
StoryState {
    Location: Bedroom
    PlayerPosture: LyingInBed
    WorldMood: MorningStillness
    PlayerNeed: WakeUp
    InnerState: Drowsy
}
```

Nu definierar vi _beats_ som inte bara är handlingar, utan förändrar tillstånd och mening.

### Beat 1 – Stiga upp

```csharp
StoryBeat "LeaveBed" {
    Preconditions:
        Location == Bedroom
        PlayerPosture == LyingInBed

    Effects:
        PlayerPosture = Standing
        InnerState = Groggy
        Theme("Transition") += 0.2

    Meaning:
        "The day begins. Inertia is broken."
}
```

### Beat 2 – Gå till köket och göra kaffe

```csharp
StoryBeat "BrewCoffee" {
    Preconditions:
        Location == Kitchen
        PlayerHas("Mug")
        PlayerNeed == WakeUp

    Effects:
        PlayerHas("Coffee")
        InnerState = WarmingUp
        WorldMood = Awakening
        Theme("Comfort") += 0.3

    Meaning:
        "Warmth enters the body. Consciousness returns."
}
```

### Beat 3 – Ta boken i vardagsrummet

```csharp
StoryBeat "FindBook" {
    Preconditions:
        Location == LivingRoom
        BookIsPresent
        InnerState >= Awake

    Effects:
        PlayerHas("Book")
        Theme("Contemplation") += 0.2

    Meaning:
        "The mind reaches for stories."
}
```

### Beat 4 – Sätta sig, dricka kaffe, läsa

Det här är kul: det är egentligen _ett nytt existentiellt tillstånd_.

```csharp
StoryBeat "SettleWithBook" {
    Preconditions:
        Location == LivingRoom
        PlayerHas("Coffee")
        PlayerHas("Book")
        PlayerPosture == Standing

    Effects:
        PlayerPosture = Sitting
        InnerState = CalmFocus
        WorldMood = QuietPresence
        PlayerNeed = None
        Theme("Rest") += 0.5
        Theme("Meaning") += 0.2

    Meaning:
        "The world slows. Attention becomes presence."
}
```

Nu kan motorn uttrycka samma händelse på tre nivåer:

### Mekanisk nivå

“You sit down. You drink coffee. You read the book.”

### Narrativ nivå

“With the warmth of the cup in your hands, the room grows still. The book opens like a small, private world.”

### Psykologisk nivå

“A sense of arrival replaces the morning’s restlessness.”

Allt detta kommer från **samma state transition**.

Det viktiga är att historien inte ligger i texten – den ligger i:

– Förändring av behov
– Förändring av inre tillstånd
– Förändring av stämning
– Tematisk progression

Så din “kaffe och bok”-sekvens är i själva verket:

Inertia → Awakening → Comfort → Presence

Det är redan en komplett dramaturgisk båge.

## Exempel på spelkörning

Om vi låter samma scenario spelas igenom i din motor, med både mekanik och narrativt lager, skulle en spelrunda kunna se ut ungefär så här:

---

**Starttillstånd**

> You are lying in bed.
> Morning light filters through the curtains.
> You feel heavy, half-awake.

Tillstånd:
Location = Bedroom
Posture = Lying
InnerState = Drowsy
Need = WakeUp

---

**Spelaren skriver:**
`get up`

Motorn matchar beat: _LeaveBed_

> You swing your legs over the edge of the bed and stand up.
> The day has begun, whether you like it or not.

Tillstånd nu:
Posture = Standing
InnerState = Groggy
Theme(Transition) +

---

**Spelaren:**
`go kitchen`

> The kitchen is quiet. The coffee maker waits on the counter.

Location = Kitchen

---

**Spelaren:**
`brew coffee`

Beat: _BrewCoffee_

> You fill the mug and start the machine.
> The smell of coffee slowly pushes the fog out of your mind.

Tillstånd:
HasCoffee = true
InnerState = WarmingUp
WorldMood = Awakening
Theme(Comfort) +

---

**Spelaren:**
`take coffee`
`go living room`

> The living room is still. A book lies on the table by the sofa.

---

**Spelaren:**
`take book`

Beat: _FindBook_

> You pick up the book. Its weight feels familiar.

HasBook = true
Theme(Contemplation) +

---

**Spelaren:**
`sit down`

Motorn ser: Location = LivingRoom, HasCoffee, HasBook, Posture = Standing
Beat: _SettleWithBook_

> You sit down on the sofa, the warm cup in your hands.
> You open the book.
> The world grows quiet in a way that feels intentional.

Tillstånd:
Posture = Sitting
InnerState = CalmFocus
WorldMood = QuietPresence
Need = None
Theme(Rest) +
Theme(Meaning) +

---

**Nu är berättelsen “klar” utan att vara slut.**

Spelaren har gått från:

Drowsy → Awake → Comforted → Present

Och systemet vet varför det är ett avslut:

– Behovet “WakeUp” är uppfyllt
– Inre tillstånd är stabilt
– Temat “Rest” har nått sitt mål
– Inga drivande konflikter kvar

Så motorn skulle kunna lägga till, helt automatiskt:

> For a moment, there is nothing you need to do.
> And that, too, is part of the story.

Det fina är att exakt samma struktur kan skala till:

– Trauma-berättelser
– Krigsdraman
– Kärlek
– Skräck
– Existentiell tomhet

Det är bara tillstånd, behov, teman och övergångar.

---

## Kodjämförelse

**CSharp**

```cs
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Parsing;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Localization;

// Create items (all styles are valid)
Key cabinKey = (id: "cabin_key", name: "brass key", description: "A small brass key with worn teeth.");
cabinKey.SetWeight(0.1f).AddAliases("key", "brass key");

Item sword = (id: "sword", name: "rusty sword", description: "A rusted blade with a dull edge.");
sword.SetWeight(3.5f).AddAliases("blade", "sword", "pointy thing");

Item apple = (id: "apple", name: "red apple", description: "A crisp red apple.");
apple.SetWeight(0.4f).AddAliases("apple");

Glass glass = (id: "glass", name: "glass", description: "A clear drinking glass.");
glass.SetWeight(0.6f)
    .SetReaction(ItemAction.Take, "The glas surface is smooth")
    .SetReaction(ItemAction.Drop, "The glass bounces on the floor") // can we destroy it from this reaction?
    .SetReaction(ItemAction.Destroy, "The glass shatters into 1000 pieces");
    ;

Item ice = (id: "ice", name: "ice", description: "A cold chunk of ice.");
ice.SetWeight(0.5f)
    .SetReaction(ItemAction.Take, "The cold chills your hand.")
    .SetReaction(ItemAction.Drop, "It lands with a soft thump.")
    .SetReaction(ItemAction.Use, "You take a bite. Your teeth ache.");

Item fire = (id: "fire", name: "fire", description: "A flickering flame.");
fire.SetWeight(0.5f);

Item lantern = (id: "lantern", name: "lantern", description: "A lantern that casts a warm glow.");
lantern.SetWeight(1.2f);

Item sign = (id: "sign", name: "wooden sign", description: "A weathered wooden sign.");
sign.SetTakeable(false)
    .SetReadable()
    .SetReadText("Welcome to the Dark Forest!");

Item newspaper = (id: "newspaper", name: "daily news", description: "A crinkled newspaper.");
newspaper.SetReadable()
    .RequireTakeToRead()
    .SetReadText("HEADLINE: Dragon spotted near village!");

Item tome = (id: "tome", name: "ancient tome", description: "A heavy book with faded runes.");
tome.SetReadable()
    .RequireTakeToRead()
    .SetReadingCost(3)
    .SetReadText("The secret to defeating the dragon is...");

Item letter = (id: "letter", name: "sealed letter", description: "A sealed letter with red wax.");
letter.SetReadable()
    .RequireTakeToRead()
    .RequiresToRead(s => s.Inventory.Items.Any(i => i.Id == "lantern"))
    .SetReadText("Meet me at midnight...");

// Create locations
Location entrance = (
    id: "entrance",
    description: "You stand at the forest gate. It's dark and foreboding."
    );
Location forest = (
    id: "forest",
    description: "A thick forest surrounds you. Shadows stretch long between ancient trees."
    );
Location cave = (
    id: "cave",
    description: "A dark cave with glowing mushrooms. A brass key glints on the ground!"
    );
Location clearing = (
    id: "clearing", description: "A sunny clearing with wildflowers. A small cabin stands here."
    );
Location cabin = (
    id: "cabin",
    description: "Inside a cozy wooden cabin. A treasure chest sits in the corner!"
    );

// Place items
cave.AddItem(cabinKey);
entrance.AddItem(ice);
entrance.AddItem(sign);
forest.AddItem(apple);
forest.AddItem(fire);
forest.AddItem(lantern);
clearing.AddItem(sword);
clearing.AddItem(glass);
clearing.AddItem(tome);
cave.AddItem(letter);
cabin.AddItem(newspaper);

// Create locked door
Door cabinDoor = (id: "cabin_door", name: "cabin door", description: "A sturdy wooden door with iron hinges.");
cabinDoor.RequiresKey(cabinKey);
cabinDoor
    .SetReaction(DoorAction.Unlock, "The lock clicks open.")
    .SetReaction(DoorAction.Open, "The door creaks as it swings wide.");

// Connect locations
entrance.AddExit(Direction.North, forest);
forest.AddExit(Direction.East, cave);
forest.AddExit(Direction.West, clearing);
clearing.AddExit(Direction.In, cabin, cabinDoor);  // Locked!

// One-way trap
cave.AddExit(Direction.Down, entrance, oneWay: true);

// Game state
var recipeBook = new RecipeBook()
    .Add(new ItemCombinationRecipe("ice", "fire", () => new FluidItem("water", "water", "Clear and cold.")));
var state = new GameState(entrance, recipeBook: recipeBook);

Console.WriteLine("=== FOREST ADVENTURE ===");
Console.WriteLine("Find the key and unlock the cabin!");
var commands = new[]
{
    "go", "look", "read", "open", "unlock", "take", "drop", "use", "inventory", "stats", "combine", "pour", "quit"
};
Console.WriteLine($"Commands: {commands.CommaJoin()} (or just type a direction)\n");

var parserConfig = new KeywordParserConfig(
    quit: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "quit", "exit", "q" },
    look: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "look", "l", "ls" },
    inventory: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "inventory", "inv", "i" },
    stats: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "stats", "stat", "hp", "health" },
    open: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "open" },
    unlock: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "unlock" },
    take: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "take", "get", "pickup", "pick" },
    drop: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "drop" },
    use: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "use", "eat", "bite" },
    combine: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "combine", "mix" },
    pour: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "pour" },
    go: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "go", "move", "cd" },
    read: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "read" },
    all: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "all" },
    ignoreItemTokens: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "up", "to" },
    combineSeparators: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "and", "+" },
    pourPrepositions: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "into", "in" },
    directionAliases: new Dictionary<string, Direction>(StringComparer.OrdinalIgnoreCase)
    {
        ["n"] = Direction.North,
        ["s"] = Direction.South,
        ["e"] = Direction.East,
        ["w"] = Direction.West,
        ["ne"] = Direction.NorthEast,
        ["nw"] = Direction.NorthWest,
        ["se"] = Direction.SouthEast,
        ["sw"] = Direction.SouthWest,
        ["u"] = Direction.Up,
        ["d"] = Direction.Down,
        ["in"] = Direction.In,
        ["out"] = Direction.Out
    },
    allowDirectionEnumNames: true);

var parser = new KeywordParser(parserConfig);

while (true)
{
    var lookResult = state.Look();
    Console.WriteLine($"\n{lookResult.Message}");
    var inventoryResult = state.InventoryView();
    Console.WriteLine(inventoryResult.Message);

    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim().Lower();

    if (string.IsNullOrEmpty(input)) continue;

    var command = parser.Parse(input);
    var result = state.Execute(command);

    if (!string.IsNullOrWhiteSpace(result.Message))
    {
        Console.WriteLine(result.Message);
    }

    foreach (var reaction in result.ReactionsList)
    {
        if (!string.IsNullOrWhiteSpace(reaction))
        {
            Console.WriteLine($"> {reaction}");
        }
    }

    if (result.ShouldQuit)
    {
        break;
    }

    if (command is GoCommand go && result.Success)
    {
        // Win condition
        if (state.IsCurrentRoomId("cabin"))
        {
            Console.WriteLine("\n*** CONGRATULATIONS! You found the treasure! ***");
            break;
        }

        // Fall trap
        if (go.Direction == Direction.Down && state.CurrentLocation.Id == "entrance")
        {
            Console.WriteLine("Oops! You fell through a hole and ended up back at the entrance!");
        }
    }
}

Console.WriteLine("\nThanks for playing!");
```

\*_DSL (.adventure)_

```cs
Story
    .World("Dark Forest")
    .Mood(Mood.Ominous)
    .Goal("Find the key and unlock the cabin")

    .Location("Entrance")
        .Description("You stand at the forest gate. It's dark and foreboding.")
        .Contains("Ice").As(Cold, Fragile)
        .Contains("Sign").Readable("Welcome to the Dark Forest!")
        .Exit(North).To("Forest")

    .Location("Forest")
        .Description("A thick forest surrounds you. Shadows stretch long between ancient trees.")
        .Contains("Apple").As(Food, Comfort)
        .Contains("Fire").As(Heat, Danger)
        .Contains("Lantern").As(Light, Safety)
        .Exit(East).To("Cave")
        .Exit(West).To("Clearing")

    .Location("Cave")
        .Description("A dark cave with glowing mushrooms. A brass key glints on the ground.")
        .Contains("CabinKey").As(Key, Progress)
        .Contains("Letter")
            .ReadableWhen(Player.Has("Lantern"))
            .Text("Meet me at midnight...")
        .Trap(Down).LeadsTo("Entrance").As(FallBack)

    .Location("Clearing")
        .Description("A sunny clearing with wildflowers. A small cabin stands here.")
        .Contains("Sword").As(Weapon, Old)
        .Contains("Glass").As(Container, Fragile)
        .Contains("Tome")
            .Readable()
            .Costs(Time: 3)
            .Reveals("The secret to defeating the dragon is...")
        .Exit(In)
            .To("Cabin")
            .BlockedBy("CabinDoor")

    .Location("Cabin")
        .Description("Inside a cozy wooden cabin. A treasure chest sits in the corner!")
        .Contains("Newspaper")
            .Readable()
            .Text("HEADLINE: Dragon spotted near village!")

    .Door("CabinDoor")
        .Requires("CabinKey")
        .OnUnlock("The lock clicks open.")
        .OnOpen("The door creaks as it swings wide.")

    .Item("Ice")
        .OnTake("The cold chills your hand.")
        .OnUse("Your teeth ache from the cold.")

    .Item("Fire")
        .OnCombineWith("Ice")
        .Creates("Water")
        .Meaning("Opposites dissolve into balance.")

    .Item("Lantern")
        .Affects(World.Darkness, -1)
        .Meaning("Light makes truth readable.")

    .NarrativeArc("Awakening")
        .StartsAt("Entrance", State: Uncertain)
        .DeepensIn("Forest", Theme: Fear)
        .RevealsIn("Cave", Theme: Knowledge)
        .ResolvesIn("Cabin", Theme: Safety)

    .WinCondition(
        when: Player.IsIn("Cabin"),
        meaning: "You have found shelter, warmth, and understanding."
    );
```

---
