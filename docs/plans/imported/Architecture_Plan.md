# 🏗️ TAF Architecture Plan - Detaljerad Systemdesign

## 1. Systemarktiektur Overview

```
┌─────────────────────────────────────────────────────────────┐
│                    TAF.Core Framework                       │
├─────────────────────────────────────────────────────────────┤
│  Game Loop Controller                                       │
│  ┌─────────┐ ┌─────────┐ ┌─────────┐ ┌─────────────────┐   │
│  │ Parser  │ │  World  │ │ Events  │ │     Player      │   │
│  │ System  │ │ Model   │ │ System  │ │     Model       │   │
│  └─────────┘ └─────────┘ └─────────┘ └─────────────────┘   │
├─────────────────────────────────────────────────────────────┤
│                    IO & Utilities                           │
│  ┌─────────────┐ ┌──────────────┐ ┌─────────────────────┐  │
│  │ Text Output │ │ Save/Load    │ │ Configuration       │  │
│  │ Formatting  │ │ System       │ │ & Data Loading      │  │
│  └─────────────┘ └──────────────┘ └─────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

## 2. Komponentspecifika Planer

### 2.1 Parser System
**Ansvar**: Tolka spelarens textinput till strukturerade kommandon

**Arkitektur**:
- `ICommandParser` interface för flexibilitet
- `StandardParser` implementation för klassisk verb-noun parsing
- `VerbRegistry` för utbyggbara kommandon
- `SynonymDictionary` för flexibel ordförståelse

**Key Features**:
- Stödja komplexa kommandon: "ta den röda nyckeln från lådan"
- Kontextmedvetenhet: "öppna den" = senast refererade objekt
- Fuzzy matching för stavfel
- Multi-språk support

### 2.2 World Model
**Ansvar**: Representera spelvärlden och dess tillstånd

**Kärnentiteter**:
- `Room`: Noder i världsgrafen med connections
- `GameObject`: Bas för allt interaktivt (items, containers, doors)
- `NPC`: Intelligenta agenter med beteenden
- `World`: Container och manager för alla entiteter

**Key Features**:
- Graph-baserad rumstruktur för pathfinding
- Attributsystem för flexibla objektegenskaper
- Hierarkiska behållare (objekt i objekt)
- Dynamic descriptions baserat på world state

### 2.3 Event System
**Ansvar**: Hantera tid, triggers och världsförändringar

**Komponenter**:
- `GameClock`: Central tidshantering (ticks + real time)
- `EventScheduler`: Schemalägg framtida events
- `TriggerSystem`: Reagera på spelactions
- `ConditionalLogic`: Komplext villkorssystem

**Event Types**:
- Time-based: "efter X ticks", "kl 14:00"
- Action-triggered: "när spelare tar X", "när NPC går till Y"
- State-based: "när hälsa < 50%", "när alla nycklar hittade"
- Chain events: En händelse triggar andra

### 2.4 Player Model
**Ansvar**: Spelarens tillstånd, inventory och progression

**Komponenter**:
- `Inventory`: Objektsamling med kapacitetsgränser
- `Statistics`: Hälsa, energi, mood, custom stats
- `KnowledgeBase`: Vad spelaren lärt sig/upptäckt
- `ProgressionFlags`: Berättelseframsteg och achievements

## 3. API Design Philosophy

### 3.1 Fluent Builder Pattern
```csharp
// Exempel på tänkt API-design (planeringsstadie)
var rum = new RoomBuilder("kök")
    .Description("Ett rustik kök med gamla kopparkastruller")
    .ConnectTo("hall", Direction.North)
    .AddItem("nyckel", item => item.Hidden().Description("En gammal mässingsnyckel"))
    .OnEnter(context => context.Output("Du känner doften av gamla kryddor"))
    .Build();
```

### 3.2 Event-Driven Interactions
```csharp
// Flexibel event-baserad design istället för hårdkodad logik
myDoor.On("open", (context, item) => {
    if (item?.Id == "golden_key") {
        OpenSecretPassage();
        context.Output("Dörren öppnas med ett mystiskt ljud...");
    }
});
```

### 3.3 Data-Driven Configuration
```yaml
# Äventyr definieras i YAML/JSON för flexibilitet
rooms:
  kitchen:
    description: "Ett rustikt kök"
    connections:
      north: hall
    items:
      - id: key
        hidden: true
        description: "En gammal mässingsnyckel"
```

## 4. Utvecklingsfaser - MVP till Full Framework

### Phase 1: Core MVP (1-2 veckor)
- Grundläggande rum-navigation
- Enkel item-hantering (ta/släpp)
- Minimal parser (verb + noun)
- Basic output-system

### Phase 2: World Building (2-3 veckor)
- Komplex objektinteraktion
- Container-system (lådor, väskor)
- NPC grundfunktioner
- Save/Load system

### Phase 3: Advanced Features (3-4 veckor)
- Event-systemet fullt utbyggt
- Conditional logic för pussel
- Dialog-system för NPCs
- Multi-room pathfinding

### Phase 4: Polish & Extensibility (2-3 veckor)
- Fluent API komplett
- YAML/JSON world loading
- Text formatting & ASCII art
- Performance optimizations

## 5. Tekniska Överväganden

### 5.1 Performance
- Lazy loading av rum och objekt
- Effektiv pathfinding algoritmer
- Memory pooling för ofta skapade objekt
- Caching av parsed kommandon

### 5.2 Extensibility
- Plugin-arkitektur för custom verbs
- Event hooks på alla nivåer
- Modulärt designmönster
- Reflection-baserad objektskapning

### 5.3 Testing Strategy
- Unit tests för varje komponent
- Integration tests för game loops
- Mock-system för IO operations
- Performance benchmarks

## 6. Design Decisions & Trade-offs

### 6.1 Beslut: Graph-baserad världsmodell
**För**: Konsekvent navigation, smart pathfinding
**Mot**: Mer komplex än enkel rumslista
**Motivering**: Nödvändigt för advanced features

### 6.2 Beslut: Event-driven arkitektur
**För**: Extremt flexibelt, lätt att utöka
**Mot**: Kan bli komplext att debugga
**Motivering**: Moderne games kräver komplex interaktion

### 6.3 Beslut: Data-driven world definition
**För**: Non-programmers kan skapa äventyr
**Mot**: Mer initial complexity
**Motivering**: Ramen ska vara användbar av alla

## 7. Success Metrics

**MVP Success**:
- Spelaren kan navigera mellan rum
- Grundläggande objektinteraktion fungerar
- Ett enkelt pussel kan lösas

**Full Framework Success**:
- Komplett äventyr kan skapas utan kod-ändringar
- Performance för 1000+ rum och objekt
- 90%+ test coverage på kärnfunktioner

---

*Detta är en levande plan som kommer att utvecklas under projektets gång*