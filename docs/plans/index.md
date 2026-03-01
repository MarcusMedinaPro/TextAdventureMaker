# Text Adventure Engine — Slice Index

- Overview: `docs/plans/overview.md`

| Slice | Title | Goal | File |
| --- | --- | --- | --- |
| 001 | Projekt-setup + Location + Navigation | Spelaren kan röra sig mellan rum. Sandbox visar det. | `slice001.md` |
| 002 | Doors + Keys | Dörrar som blockerar utgångar, kräver nycklar. | `slice002.md` |
| 003 | Command Pattern + Parser | Kommandon som objekt. Keyword-parser. "go north", "look", "quit". | `slice003.md` |
| 004 | Items + Inventory | Items i rum, plocka upp, släpp, visa inventory. Containers och kombinationer. | `slice004.md` |
| 005 | NPCs + Dialog + Movement | NPCs i rum, prata med dem, dialog-träd. NPCs rör sig. | `slice005.md` |
| 006 | Event System (Observer) | Triggers när saker händer. | `slice006.md` |
| 007 | Combat (Strategy) | Utbytbart stridssystem. | `slice007.md` |
| 008 | Quest System | Objectives och progress. | `slice008.md` |
| 009 | World State System | Centralt state för att spåra global världsstatus. Foundation för quests, events, stories. | `slice009.md` |
| 010 | Save/Load (Memento) | Spara och ladda spelstatus. | `slice010.md` |
| 011 | Language System | Ladda språkfil för all speltext. Default engelska. | `slice011.md` |
| 012 | DSL Parser | Ladda spel från .adventure-filer. | `slice012.md` |
| 013 | GameBuilder (Fluent API) | Bygga spel helt i C# med fluent syntax. | `slice013.md` |
| 014 | Loggers | Story logger + dev logger. | `slice014.md` |
| 015 | Pathfinder | Guida spelaren genom kartan. | `slice015.md` |
| 016 | AI-paket (MarcusMedina.TextAdventure.AI) | Ollama-integration som ICommandParser. | `slice016.md` |
| 017 | NuGet-paketering | Publicera båda paketen. | `slice017.md` |
| 018 | Story Branches & Consequences | Hantera storylines baserat på spelarval. | `slice018.md` |
| 019 | Multi-Stage Quests | Quests med stages, optional objectives, failure paths. | `slice019.md` |
| 020 | Conditional Event Chains | Sekvenser av events som påverkar varandra. | `slice020.md` |
| 021 | Time System | Dag/natt cycles, tidbaserade events. | `slice021.md` |
| 022 | Faction & Reputation System | NPC-grupper med gemensam reputation. | `slice022.md` |
| 023 | Random Event Pool | Dynamiska slumpmässiga events. | `slice023.md` |
| 024 | Location Discovery System | Hidden locations som upptäcks genom exploration. | `slice024.md` |
| 025 | Story Mapper Tool (Visual Editor) | Grafiskt verktyg för content creation. | `slice025.md` |
| 026 | Mood & Atmosphere System | Stämning som påverkar spelarupplevelsen. | `slice026.md` |
| 027 | Dynamic Description System | Beskrivningar som ändras baserat på context. | `slice027.md` |
| 028 | Character Arc Tracking | NPCs utvecklas över tid. | `slice028.md` |
| 029 | Pacing & Tension System | Balans mellan action och lugn. | `slice029.md` |
| 030 | Foreshadowing & Callbacks | Chekov's Gun — plantera och betala av. | `slice030.md` |
| 031 | Scene Transitions & Beats | Stories flödar mellan scenes, inte bara locations. | `slice031.md` |
| 032 | Emotional Stakes System | Spelaren bryr sig om saker de investerat i. | `slice032.md` |
| 033 | Narrative Voice System | Flexibel berättarröst. | `slice033.md` |
| 034 | Player Agency Tracking | Anpassa story till spelarstil. | `slice034.md` |
| 035 | Dramatic Irony Tracker | Spänning när spelaren vet mer än karaktären. | `slice035.md` |
| 036 | Hero's Journey & Narrative Templates | Inbyggda dramaturgiska strukturer som guide för story design. | `slice036.md` |
| 037 | Generic Chapter System | Flexibel kapitelstruktur utan låst template. Bygg din egen arc. | `slice037.md` |
| 038 | Time/Action Triggered Objects & Doors | Objekt och dörrar som spawnar/öppnas baserat på tid eller actions. | `slice038.md` |
| 039 | Fluent API & Språksnygghet | All syntaktisk socker för snygg, läsbar kod. | `slice039.md` |
| 040 | GitHub Wiki (../TextAdventure.wiki) | Komplett dokumentation för användare på engelska | `slice040.md` |
| 041 | Testing & Validation Tools | Verktyg för att testa och validera textäventyr. | `slice041.md` |
| 042 | API Design Philosophy — Fluent Query Extensions | Säkerställa att hela bibliotekets API följer samma fluent, kedjebara mönster som C#-utvecklare känner igen. Svaret på livet, universum och allting. | `slice042.md` |
| 043 | Map Generator | Skapa en enkel map generator som kan rendera en ASCII-karta baserat på location-grafen och exits. | `slice043.md` |
| 044 | String Case Utilities | Enkla stränghelpers för casing i UI/texter. | `slice044.md` |
| 045 | Generic Fixes | Ersätt hårdkodade input-checkar (t.ex. `IsSitInput`) med en generisk, fluent alias‑lösning. | `slice045.md` |

## Framtida Slices (050+)

*Importerade idéer från MarcusTextAdventureMaker*

| Slice | Title | Goal | File |
| --- | --- | --- | --- |
| 050 | Directional Looking | "look north" - se in i angränsande rum genom öppna dörrar. | `slice050.md` |
| 051 | Directional Actions | "throw wallet north", "shout east" - agera mot angränsande rum. | `slice051.md` |
| 052 | Spatial Awareness System | Komplett rumsmedvetenhet - sikt, ljud och interaktion mellan rum. | `slice052.md` |
| 053 | Advanced Map Generator | Procedural kartgenerering och ASCII-visualisering. | `slice053.md` |
| 054 | Player History System | Spåra spelarens resa, achievements och analytics. | `slice054.md` |
| 055 | Economic & Store System | Valutor, butiker, handel och dynamiska priser. | `slice055.md` |
| 056 | Advanced NPC Communication | NPC-minne, personlighet, relationer och kontextmedveten dialog. | `slice056.md` |
| 057 | Advanced Semantic Parser | Naturligt språk, kontext och komplexa kommandon. | `slice057.md` |
| 058 | Cinematic Presentation | Typewriter-effekter, pauser, ASCII-art och atmosfär. | `slice058.md` |
| 059 | Procedural Storytelling | Generera sidouppdrag, events, lore och textvariation. | `slice059.md` |
| 060 | Puzzle Toolkit | Återanvändbara pusselmönster (kombinationslås, gåtor, sekvenser). | `slice060.md` |
| 061 | Debug Console & Dev Tools | /teleport, /give, /inspect, undo/redo, testrunner. | `slice061.md` |
| 062 | Countdown & Deadline Events | Tidsbegränsade utmaningar med nedräkning och konsekvenser. | `slice062.md` |
| 063 | Chase & Pursuit System | NPCs som jagar, följer eller flyr från spelaren. | `slice063.md` |
| 064 | Status Effects System | Temporära tillståndseffekter (poison, burning, blessed). | `slice064.md` |
| 065 | Test Helpers & Infrastructure | TestWorldBuilder, GameAssert, fixtures, kategorisering. | `slice065.md` |
| 066 | Weather & Environment System | Dynamiskt väder som påverkar gameplay och visibility. | `slice066.md` |
| 067 | Vehicles & Teleporters | Fordon och teleporters som transportmedel. | `slice067.md` |
| 068 | Stealth & Detection System | Smyg- och upptäcktssystem för att passera NPCs. | `slice068.md` |
| 069 | Light & Darkness System | Ljus och mörker som påverkar synlighet och gameplay. | `slice069.md` |
| 070 | Food, Hunger & Survival | Mat, hunger, törst och överlevnadsmekanik. | `slice070.md` |
| 071 | Crafting System | Kombinera items för att skapa nya föremål. | `slice071.md` |
| 072 | Machines & Electronics | Elektriska och mekaniska maskiner. | `slice072.md` |

---

## Systemslices (900+)

| Slice | Title | Goal | File |
| --- | --- | --- | --- |
| 900 | Plugin System | Utbyggbart plugin-system för externa moduler, mods och integrationer. | `slice900.md` |

---

## Importerade Planer

Detaljerade designdokument från tidigare projekt finns i `docs/plans/imported/`:

- `Directional_Looking_System.md` - Design för "look north"
- `Directional_Actions_System.md` - Design för riktade actions
- `Spatial_Awareness_Revolution.md` - Komplett spatial awareness-koncept
- `Map_Generator_System.md` - Kartgenereringsalgoritmer
- `Player_History_System.md` - Journey tracking och achievements
- `Economic_Store_System.md` - Butiker och ekonomi
- `NPC_Communication_System.md` - Avancerad NPC-dialog
- `Semantic_Parser_Design.md` - Natural language parsing
- `Semantic_Parser_Advanced.md` - Kontextmedveten parser
- `API_Design.md` - Fluent API-mönster
- `Architecture_Plan.md` - Övergripande arkitektur
- `Pathway_Connection_Design.md` - Kopplingar mellan rum
- `Event_System_Dynamic_Economics.md` - Eventdriven ekonomi
- `Future_Development_Ideas.md` - Framtida funktioner
