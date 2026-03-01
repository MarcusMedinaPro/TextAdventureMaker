## Slice 900: Plugin System

**Mål:** Utbyggbart plugin-system för att ladda externa moduler, mods och anpassningar.

### Task 900.1: IPlugin interface

```csharp
public interface IPlugin
{
    string Id { get; }
    string Name { get; }
    string Version { get; }
    string Author { get; }
    string Description { get; }
    IReadOnlyList<string> Dependencies { get; }

    void Initialize(IPluginContext context);
    void Shutdown();
}

public interface IPluginContext
{
    IGame Game { get; }
    IServiceProvider Services { get; }
    IEventBus Events { get; }
    ICommandRegistry Commands { get; }

    void RegisterService<T>(T service) where T : class;
    T? GetService<T>() where T : class;
}
```

### Task 900.2: PluginManager

```csharp
public interface IPluginManager
{
    IReadOnlyList<IPlugin> LoadedPlugins { get; }

    void LoadPlugin(string path);
    void LoadPluginsFromDirectory(string directory);
    void UnloadPlugin(string pluginId);
    void ReloadPlugin(string pluginId);

    bool IsPluginLoaded(string pluginId);
    IPlugin? GetPlugin(string pluginId);

    event Action<IPlugin>? OnPluginLoaded;
    event Action<IPlugin>? OnPluginUnloaded;
}

public class PluginManager : IPluginManager
{
    private readonly Dictionary<string, IPlugin> _plugins = [];
    private readonly IPluginContext _context;

    public IReadOnlyList<IPlugin> LoadedPlugins => _plugins.Values.ToList();

    public event Action<IPlugin>? OnPluginLoaded;
    public event Action<IPlugin>? OnPluginUnloaded;

    public void LoadPlugin(string path)
    {
        // Ladda assembly
        var assembly = Assembly.LoadFrom(path);

        // Hitta IPlugin-implementationer
        var pluginTypes = assembly.GetTypes()
            .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var type in pluginTypes)
        {
            var plugin = (IPlugin)Activator.CreateInstance(type)!;

            // Kolla dependencies
            foreach (var dep in plugin.Dependencies)
            {
                if (!IsPluginLoaded(dep))
                    throw new PluginDependencyException($"Plugin {plugin.Id} requires {dep}");
            }

            plugin.Initialize(_context);
            _plugins[plugin.Id] = plugin;
            OnPluginLoaded?.Invoke(plugin);
        }
    }

    public void UnloadPlugin(string pluginId)
    {
        if (_plugins.TryGetValue(pluginId, out var plugin))
        {
            // Kolla att inga andra plugins beror på denna
            var dependents = _plugins.Values
                .Where(p => p.Dependencies.Contains(pluginId))
                .ToList();

            if (dependents.Any())
                throw new PluginDependencyException($"Cannot unload {pluginId}, required by: {string.Join(", ", dependents.Select(p => p.Id))}");

            plugin.Shutdown();
            _plugins.Remove(pluginId);
            OnPluginUnloaded?.Invoke(plugin);
        }
    }

    public bool IsPluginLoaded(string pluginId) => _plugins.ContainsKey(pluginId);
    public IPlugin? GetPlugin(string pluginId) => _plugins.GetValueOrDefault(pluginId);
}
```

### Task 900.3: Plugin Types

```csharp
// Content plugin - lägger till nytt innehåll
public interface IContentPlugin : IPlugin
{
    IEnumerable<ILocation> GetLocations();
    IEnumerable<IItem> GetItems();
    IEnumerable<INpc> GetNpcs();
    IEnumerable<Quest> GetQuests();
}

// Command plugin - lägger till nya kommandon
public interface ICommandPlugin : IPlugin
{
    IEnumerable<(string keyword, Func<string, ICommand> factory)> GetCommands();
}

// UI plugin - anpassar presentation
public interface IUIPlugin : IPlugin
{
    ITextPresenter? GetTextPresenter();
    IEnumerable<(string trigger, string asciiArt)> GetAsciiArt();
}

// AI plugin - lägger till AI-funktionalitet
public interface IAIPlugin : IPlugin
{
    ICommandParser? GetAIParser();
    INpcBehavior? GetNpcBehavior(string behaviorId);
}

// Integration plugin - kopplar till externa system
public interface IIntegrationPlugin : IPlugin
{
    Task ConnectAsync();
    Task DisconnectAsync();
    bool IsConnected { get; }
}
```

### Task 900.4: Plugin Manifest

```json
{
  "id": "my-awesome-plugin",
  "name": "My Awesome Plugin",
  "version": "1.0.0",
  "author": "Marcus",
  "description": "Adds awesome features",
  "dependencies": ["core-extensions"],
  "minEngineVersion": "2.0.0",
  "entryPoint": "MyAwesomePlugin.dll",
  "permissions": [
    "file-read",
    "network",
    "custom-commands"
  ],
  "settings": {
    "enableFeatureX": true,
    "maxItems": 100
  }
}
```

### Task 900.5: Plugin Security Sandbox

```csharp
public class PluginSandbox
{
    private readonly HashSet<string> _allowedPermissions = [];

    public void GrantPermission(string permission) =>
        _allowedPermissions.Add(permission);

    public bool HasPermission(string permission) =>
        _allowedPermissions.Contains(permission);

    public void ValidatePlugin(IPlugin plugin, PluginManifest manifest)
    {
        // Kolla att plugin inte försöker göra farliga saker
        var assembly = plugin.GetType().Assembly;

        // Kolla efter förbjudna typer
        var forbiddenTypes = new[] { "System.IO.File", "System.Net.Http", "System.Diagnostics.Process" };

        foreach (var type in assembly.GetTypes())
        {
            foreach (var method in type.GetMethods())
            {
                // Analysera IL för farliga anrop
                // (förenklad version)
            }
        }
    }
}
```

### Task 900.6: Plugin Configuration

```csharp
public interface IPluginConfiguration
{
    T GetSetting<T>(string key, T defaultValue = default!);
    void SetSetting<T>(string key, T value);
    void Save();
    void Load();
}

public class PluginConfiguration : IPluginConfiguration
{
    private readonly string _configPath;
    private readonly Dictionary<string, object> _settings = [];

    public PluginConfiguration(string pluginId)
    {
        _configPath = Path.Combine("plugins", pluginId, "config.json");
    }

    public T GetSetting<T>(string key, T defaultValue = default!)
    {
        if (_settings.TryGetValue(key, out var value))
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        return defaultValue;
    }

    public void SetSetting<T>(string key, T value)
    {
        _settings[key] = value!;
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_configPath, json);
    }

    public void Load()
    {
        if (File.Exists(_configPath))
        {
            var json = File.ReadAllText(_configPath);
            var loaded = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            if (loaded != null)
            {
                foreach (var (key, value) in loaded)
                    _settings[key] = value;
            }
        }
    }
}
```

### Task 900.7: Plugin Events

```csharp
public class PluginEventBus
{
    private readonly Dictionary<string, List<Delegate>> _handlers = [];

    public void Subscribe<T>(string eventName, Action<T> handler)
    {
        if (!_handlers.ContainsKey(eventName))
            _handlers[eventName] = [];

        _handlers[eventName].Add(handler);
    }

    public void Unsubscribe<T>(string eventName, Action<T> handler)
    {
        if (_handlers.TryGetValue(eventName, out var handlers))
            handlers.Remove(handler);
    }

    public void Publish<T>(string eventName, T data)
    {
        if (_handlers.TryGetValue(eventName, out var handlers))
        {
            foreach (var handler in handlers.OfType<Action<T>>())
            {
                try
                {
                    handler(data);
                }
                catch (Exception ex)
                {
                    // Log error but don't crash
                    Console.WriteLine($"Plugin event handler error: {ex.Message}");
                }
            }
        }
    }
}

// Standardhändelser
public static class PluginEvents
{
    public const string GameStarted = "game.started";
    public const string GameEnded = "game.ended";
    public const string TurnStarted = "turn.started";
    public const string TurnEnded = "turn.ended";
    public const string CommandExecuted = "command.executed";
    public const string LocationChanged = "location.changed";
    public const string ItemAcquired = "item.acquired";
    public const string NpcInteraction = "npc.interaction";
    public const string QuestUpdated = "quest.updated";
    public const string SaveGame = "save.game";
    public const string LoadGame = "load.game";
}
```

### Task 900.8: Example Plugin

```csharp
public class ExamplePlugin : IPlugin, ICommandPlugin
{
    public string Id => "example-plugin";
    public string Name => "Example Plugin";
    public string Version => "1.0.0";
    public string Author => "Marcus";
    public string Description => "Demonstrates plugin capabilities";
    public IReadOnlyList<string> Dependencies => [];

    private IPluginContext? _context;

    public void Initialize(IPluginContext context)
    {
        _context = context;

        // Registrera event handlers
        context.Events.Subscribe<LocationChangedEvent>(PluginEvents.LocationChanged, OnLocationChanged);

        // Registrera services
        context.RegisterService(new MyCustomService());

        Console.WriteLine($"{Name} v{Version} loaded!");
    }

    public void Shutdown()
    {
        Console.WriteLine($"{Name} unloaded.");
    }

    public IEnumerable<(string keyword, Func<string, ICommand> factory)> GetCommands()
    {
        yield return ("dance", args => new DanceCommand());
        yield return ("sing", args => new SingCommand(args));
    }

    private void OnLocationChanged(LocationChangedEvent e)
    {
        // Gör något när spelaren byter rum
    }
}

public class DanceCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        return CommandResult.Ok("You dance a little jig!");
    }
}
```

### Task 900.9: Plugin CLI Commands

```csharp
// /plugins - lista plugins
// /plugin load <path> - ladda plugin
// /plugin unload <id> - ta bort plugin
// /plugin info <id> - visa info om plugin
// /plugin reload <id> - ladda om plugin

public class PluginCommands
{
    private readonly IPluginManager _manager;

    public CommandResult ListPlugins()
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== Loaded Plugins ===");

        foreach (var plugin in _manager.LoadedPlugins)
        {
            sb.AppendLine($"  {plugin.Id} v{plugin.Version} - {plugin.Name}");
        }

        return CommandResult.Ok(sb.ToString());
    }

    public CommandResult PluginInfo(string pluginId)
    {
        var plugin = _manager.GetPlugin(pluginId);
        if (plugin == null)
            return CommandResult.Fail($"Plugin '{pluginId}' not found.");

        var sb = new StringBuilder();
        sb.AppendLine($"=== {plugin.Name} ===");
        sb.AppendLine($"ID: {plugin.Id}");
        sb.AppendLine($"Version: {plugin.Version}");
        sb.AppendLine($"Author: {plugin.Author}");
        sb.AppendLine($"Description: {plugin.Description}");
        if (plugin.Dependencies.Any())
            sb.AppendLine($"Dependencies: {string.Join(", ", plugin.Dependencies)}");

        return CommandResult.Ok(sb.ToString());
    }
}
```

### Task 900.10: Tester

```csharp
[Fact]
public void PluginManager_LoadsPlugin()
{
    var manager = new PluginManager(CreateContext());

    manager.LoadPlugin("TestPlugin.dll");

    Assert.True(manager.IsPluginLoaded("test-plugin"));
}

[Fact]
public void PluginManager_RejectsMissingDependency()
{
    var manager = new PluginManager(CreateContext());

    Assert.Throws<PluginDependencyException>(() =>
        manager.LoadPlugin("PluginWithMissingDep.dll"));
}

[Fact]
public void PluginEventBus_DeliversEvents()
{
    var bus = new PluginEventBus();
    var received = false;

    bus.Subscribe<string>("test.event", _ => received = true);
    bus.Publish("test.event", "data");

    Assert.True(received);
}
```

### Task 900.11: Sandbox — plugin-demo

Demo som laddar ett exempel-plugin och visar hur det utökar spelet.

---

## Framtida Plugin-Idéer

- **Grafik-plugin**: Koppla mot Unity/Godot/MonoGame för 3D-rendering
- **Multiplayer-plugin**: Lägg till nätverksstöd
- **Databas-plugin**: Persistent storage med SQLite/Redis
- **Analytics-plugin**: Spelarstatistik och telemetri
- **Mod-manager-plugin**: Steam Workshop-integration
- **Voice-plugin**: Text-to-speech med olika röster
- **Translation-plugin**: Automatisk översättning med AI
- **Discord-plugin**: Bot-integration för textäventyr i Discord
- **Web-plugin**: REST API för webbklienter
- **Mobile-plugin**: Touch-optimerad UI
