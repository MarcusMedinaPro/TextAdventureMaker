# 🎯 TAF API Design Specification

## 1. Fluent API Design - Core Interfaces

### 1.1 GameEngine - Central Hub
```csharp
// Huvudklassen som utvecklare interagerar med
public class GameEngine
{
    // Fluent initialization
    public static GameEngineBuilder Create(string gameName) { }

    // Runtime methods
    public void Start();
    public void ProcessCommand(string input);
    public void Stop();
    public GameState GetCurrentState();

    // Event registration
    public GameEngine OnPlayerMove(Action<Room, Room> handler);
    public GameEngine OnItemTaken(Action<Player, GameObject> handler);
    public GameEngine OnTimeAdvanced(Action<GameTime> handler);
}

// Builder pattern för initial setup
public class GameEngineBuilder
{
    public GameEngineBuilder WithWorld(World world);
    public GameEngineBuilder WithPlayer(Player player);
    public GameEngineBuilder WithParser(ICommandParser parser);
    public GameEngineBuilder LoadFromFile(string path);
    public GameEngine Build();
}
```

### 1.2 World Building API
```csharp
// Fluent world creation
public class WorldBuilder
{
    public WorldBuilder Name(string name);
    public WorldBuilder Description(string desc);
    public WorldBuilder AddRoom(string id, Action<RoomBuilder> configure);
    public World Build();
}

public class RoomBuilder
{
    public RoomBuilder Description(string text);
    public RoomBuilder ShortDescription(string text);
    public RoomBuilder ConnectTo(string roomId, Direction direction);
    public RoomBuilder AddItem(string itemId, Action<ItemBuilder> configure = null);
    public RoomBuilder AddNPC(string npcId, Action<NPCBuilder> configure = null);
    public RoomBuilder SetProperty(string key, object value);

    // Event hooks
    public RoomBuilder OnEnter(Action<GameContext> action);
    public RoomBuilder OnExit(Action<GameContext> action);
    public RoomBuilder OnLook(Func<GameContext, string> action);
    public RoomBuilder OnTick(Action<GameContext> action);

    public Room Build();
}
```

### 1.3 GameObject & Item API
```csharp
public class ItemBuilder
{
    public ItemBuilder Name(string name);
    public ItemBuilder Description(string desc);
    public ItemBuilder Synonyms(params string[] synonyms);
    public ItemBuilder Portable(bool canTake = true);
    public ItemBuilder Hidden(bool isHidden = true);
    public ItemBuilder Weight(float weight);

    // Container functionality
    public ItemBuilder Container(Action<ContainerBuilder> configure);

    // Interactive properties
    public ItemBuilder OnUse(Action<GameContext, GameObject> action);
    public ItemBuilder OnExamine(Func<GameContext, string> action);
    public ItemBuilder OnTake(Action<GameContext> action);

    public GameObject Build();
}

public class ContainerBuilder
{
    public ContainerBuilder Capacity(int maxItems);
    public ContainerBuilder Locked(bool isLocked = true);
    public ContainerBuilder RequiresKey(string keyId);
    public ContainerBuilder AddItem(string itemId, Action<ItemBuilder> configure = null);
}
```

### 1.4 NPC API Design
```csharp
public class NPCBuilder
{
    public NPCBuilder Name(string name);
    public NPCBuilder Description(string desc);
    public NPCBuilder InitialRoom(string roomId);
    public NPCBuilder Personality(NPCPersonality personality);
    public NPCBuilder Health(int health);
    public NPCBuilder Inventory(Action<InventoryBuilder> configure);

    // Behavior configuration
    public NPCBuilder Wanders(bool canWander = true);
    public NPCBuilder Hostile(bool isHostile = true);
    public NPCBuilder Dialogue(Action<DialogueBuilder> configure);

    // AI behaviors
    public NPCBuilder OnTick(Action<NPCContext> behavior);
    public NPCBuilder OnPlayerEnter(Action<NPCContext, Player> behavior);
    public NPCBuilder OnCombat(Action<CombatContext> behavior);

    public NPC Build();
}

public class DialogueBuilder
{
    public DialogueBuilder AddTopic(string keyword, string response);
    public DialogueBuilder AddConditionalTopic(string keyword, Func<GameContext, bool> condition, string response);
    public DialogueBuilder DefaultResponse(string response);
    public DialogueBuilder OnGreeting(string greeting);
}
```

## 2. Event System API

### 2.1 Time & Scheduling
```csharp
public class EventScheduler
{
    // Time-based scheduling
    public void ScheduleAfter(int ticks, Action<GameContext> action);
    public void ScheduleAt(GameTime time, Action<GameContext> action);
    public void ScheduleRepeating(int intervalTicks, Action<GameContext> action);

    // Conditional events
    public void ScheduleWhen(Func<GameContext, bool> condition, Action<GameContext> action);
    public void ScheduleOnce(string eventId, Func<GameContext, bool> condition, Action<GameContext> action);
}

public class GameTime
{
    public int TotalTicks { get; }
    public TimeSpan ElapsedGameTime { get; }
    public DateTime GameDateTime { get; } // För day/night cykler

    public void AdvanceTicks(int ticks);
    public void AdvanceTime(TimeSpan duration);
}
```

### 2.2 Event Registration System
```csharp
public class EventRegistry
{
    // Global events
    public void RegisterGlobalEvent(string eventName, Action<GameContext> handler);
    public void TriggerEvent(string eventName, GameContext context);

    // Object-specific events
    public void RegisterItemEvent(string itemId, string eventName, Action<GameContext, GameObject> handler);
    public void RegisterRoomEvent(string roomId, string eventName, Action<GameContext, Room> handler);
    public void RegisterNPCEvent(string npcId, string eventName, Action<GameContext, NPC> handler);
}
```

## 3. Parser System API

### 3.1 Command Processing
```csharp
public interface ICommandParser
{
    ParsedCommand Parse(string input, GameContext context);
    void RegisterVerb(string verb, VerbHandler handler);
    void RegisterSynonym(string synonym, string canonicalWord);
}

public class ParsedCommand
{
    public string Verb { get; set; }
    public string DirectObject { get; set; }
    public string IndirectObject { get; set; }
    public string Preposition { get; set; }
    public Dictionary<string, string> Modifiers { get; set; }
}

public delegate CommandResult VerbHandler(GameContext context, ParsedCommand command);
```

### 3.2 Context-Aware Parsing
```csharp
public class GameContext
{
    public Player CurrentPlayer { get; }
    public Room CurrentRoom { get; }
    public World CurrentWorld { get; }
    public GameTime CurrentTime { get; }

    // Reference resolution
    public GameObject ResolveObject(string objectRef);
    public NPC ResolveNPC(string npcRef);
    public Room ResolveRoom(string roomRef);

    // Output methods
    public void Output(string message);
    public void OutputFormatted(string template, params object[] args);
    public void OutputToPlayer(Player player, string message);
}
```

## 4. Data Loading API

### 4.1 File-Based World Loading
```csharp
public static class WorldLoader
{
    public static World LoadFromYaml(string filePath);
    public static World LoadFromJson(string filePath);
    public static World LoadFromDirectory(string directoryPath); // Multiple files

    public static void SaveWorld(World world, string filePath, WorldFormat format);
}

public enum WorldFormat
{
    Yaml,
    Json,
    Binary
}
```

### 4.2 Runtime World Modification
```csharp
public class World
{
    // Room management
    public Room GetRoom(string id);
    public void AddRoom(Room room);
    public void RemoveRoom(string id);
    public IEnumerable<Room> GetConnectedRooms(string roomId);

    // Object management
    public GameObject GetObject(string id);
    public void AddObject(GameObject obj, string roomId);
    public void MoveObject(string objId, string toRoomId);

    // NPC management
    public NPC GetNPC(string id);
    public void AddNPC(NPC npc);
    public void MoveNPC(string npcId, string toRoomId);

    // World state
    public void SetGlobalFlag(string flag, object value);
    public T GetGlobalFlag<T>(string flag);
    public bool HasGlobalFlag(string flag);
}
```

## 5. Save/Load System API

### 5.1 Game State Management
```csharp
public class SaveLoadManager
{
    public void SaveGame(string saveName, GameState state);
    public GameState LoadGame(string saveName);
    public void DeleteSave(string saveName);
    public IEnumerable<SaveGameInfo> ListSaves();

    public void AutoSave(GameState state);
    public GameState LoadAutoSave();
}

public class GameState
{
    public World World { get; set; }
    public Player Player { get; set; }
    public GameTime CurrentTime { get; set; }
    public Dictionary<string, object> GlobalFlags { get; set; }
    public List<ScheduledEvent> PendingEvents { get; set; }
}

public class SaveGameInfo
{
    public string Name { get; set; }
    public DateTime SavedAt { get; set; }
    public TimeSpan PlayTime { get; set; }
    public string PlayerLocation { get; set; }
    public int PlayerLevel { get; set; }
}
```

## 6. Output & UI API

### 6.1 Text Formatting System
```csharp
public class TextFormatter
{
    public string Format(string template, params object[] args);
    public string Colorize(string text, ConsoleColor color);
    public string Bold(string text);
    public string Italic(string text);

    // Rich text markup
    public string ParseMarkup(string richText); // [color=red]text[/color]
    public void SetTheme(OutputTheme theme);
}

public class OutputTheme
{
    public ConsoleColor DefaultTextColor { get; set; }
    public ConsoleColor ImportantTextColor { get; set; }
    public ConsoleColor ErrorTextColor { get; set; }
    public ConsoleColor SystemTextColor { get; set; }

    public string PromptSymbol { get; set; }
    public bool UseColorCoding { get; set; }
    public bool ShowTimestamps { get; set; }
}
```

### 6.2 Advanced Output Features
```csharp
public class GameOutput
{
    // Basic output
    public void WriteLine(string text);
    public void Write(string text);
    public void WriteError(string error);
    public void WriteSystem(string message);

    // Formatted output
    public void WriteFormatted(string template, params object[] args);
    public void WriteColored(string text, ConsoleColor color);

    // Special effects
    public void WriteSlow(string text, int delayMs = 50); // Typewriter effect
    public void WriteASCII(string asciiArt);
    public void WriteSeparator(char character = '=', int length = 50);

    // Paging for long content
    public void WritePaged(string longText, int linesPerPage = 20);
}
```

## 7. Extension & Plugin API

### 7.1 Custom Verbs
```csharp
public abstract class CustomVerb
{
    public abstract string VerbName { get; }
    public abstract string[] Synonyms { get; }
    public abstract string HelpText { get; }

    public abstract CommandResult Execute(GameContext context, ParsedCommand command);
    public virtual bool CanExecute(GameContext context, ParsedCommand command) => true;
}

// Registrering
engine.RegisterCustomVerb<TrollMagicVerb>();
```

### 7.2 Plugin System
```csharp
public interface IGamePlugin
{
    string Name { get; }
    Version Version { get; }

    void Initialize(GameEngine engine);
    void Shutdown();
}

public class PluginManager
{
    public void LoadPlugin(string assemblyPath);
    public void LoadPlugin<T>() where T : IGamePlugin, new();
    public void UnloadPlugin(string pluginName);
    public IEnumerable<IGamePlugin> GetLoadedPlugins();
}
```

---

*Detta API är designat för att vara intuitivt, kraftfullt och lätt att utöka*