using MarcusMedina.TextAdventure.AI.Models;
using MarcusMedina.TextAdventure.AI.Plugin;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

// AI Plugin Demo
// Environment variables:
// - TA_AI_PROVIDER (ollama|lmstudio|dockerai|openai|claude|mistral|openrouter|1minai|gemini)
// - TA_AI_API_KEY
// - TA_AI_MODEL
// - TA_AI_ENDPOINT
// - TA_AI_FALLBACK_PROVIDER (+ matching TA_AI_FALLBACK_API_KEY/MODEL/ENDPOINT)
// - TA_AI_ENABLE_DESCRIPTION_CACHE_INVALIDATION (true/false)
// - TA_AI_RUNTIME_TIMEOUT_MS
// - TA_AI_NPC_MOVE_AI_EVERY_TURNS
// - TA_AI_STORY_AI_EVERY_TURNS
// - TA_AI_PREFER_LOCAL_PARSE_FIRST (true/false)
// - TA_AI_SPINNER (true/false)

Location square = new("square", "A rain-soaked town square with a flickering lamp.");
Location inn = new("inn", "A warm inn with polished oak tables and soft firelight.");
Location alley = new("alley", "A narrow alley where loose shutters rattle in the wind.");

square.AddExit(Direction.North, inn);
square.AddExit(Direction.East, alley);
inn.AddExit(Direction.South, square);
alley.AddExit(Direction.West, square);

Item map = new("map", "Town Map", "A weathered map of side streets and old service tunnels.");
Item token = new("token", "Brass Token", "A stamped token from the inn cellar.");
square.AddItem(map);
inn.AddItem(token);

var innkeeper = new Npc("innkeeper", "Innkeeper")
    .Description("Pragmatic and observant. Speaks in short, careful sentences.")
    .Dialog("Evening. Keep your voice down if you're planning trouble.");

var watchman = new Npc("watchman", "Watchman")
    .Description("Tired but dutiful. Keeps circling known trouble spots.")
    .Dialog("Move along. No one lingers here without reason.");

inn.AddNpc(innkeeper);
square.AddNpc(watchman);

GameState state = new(square, worldLocations: [square, inn, alley]);
var baseParser = new KeywordParser(KeywordParserConfigBuilder.BritishDefaults().Build());
using var spinner = new AiConsoleSpinner(ReadEnvBool("TA_AI_SPINNER", true));
AiPluginBootstrap bootstrap = CreateAiBootstrap(baseParser, spinner);

Game game = GameBuilder.Create()
    .UseState(state)
    .UseParser(bootstrap.Parser)
    .UsePrompt("> ")
    .Build();

bootstrap.EnableRuntime(game);
state.EnableAiNpcMovement(bootstrap.Module, bootstrap.PluginOptions);

SetupC64("AI Plugin Sandbox");
WriteLineC64("=== AI PLUGIN SANDBOX ===");
WriteLineC64($"Provider: {ReadEnv("TA_AI_PROVIDER", "openai")}");
WriteLineC64("Goal: talk to NPCs and explore. AI can shape dialogue, movement, descriptions, and story timeline.");
WriteLineC64();
WriteLineC64("Try: look, talk innkeeper, go north, look map, go south, go east, talk watchman, quit");

state.ShowRoom();
game.Run();

static AiPluginBootstrap CreateAiBootstrap(KeywordParser baseParser, AiConsoleSpinner spinner)
{
    try
    {
        AiProviderInitOptions primary = BuildProvider("TA_AI_", defaultProvider: AiProviderKind.Ollama);
        List<AiProviderInitOptions> fallbacks = [];

        string? fallbackRaw = Environment.GetEnvironmentVariable("TA_AI_FALLBACK_PROVIDER");
        if (!string.IsNullOrWhiteSpace(fallbackRaw))
            fallbacks.Add(BuildProvider("TA_AI_FALLBACK_", ParseProvider(fallbackRaw)));

        return AiPluginBootstrapFactory.Create(new AiPluginInitOptions
        {
            PrimaryProvider = primary,
            FallbackProviders = fallbacks,
            BaseParser = baseParser,
            RouterDecorator = router => new SpinnerAiProviderRouter(router, spinner),
            PluginOptions = new AiPluginOptions
            {
                EnableAiDialogue = ReadEnvBool("TA_AI_ENABLE_DIALOGUE", true),
                EnableAiDescriptions = ReadEnvBool("TA_AI_ENABLE_DESCRIPTIONS", true),
                EnableAiDescriptionCacheInvalidation = ReadEnvBool("TA_AI_ENABLE_DESCRIPTION_CACHE_INVALIDATION", true),
                EnableAiNpcMovement = ReadEnvBool("TA_AI_ENABLE_NPC_MOVEMENT", true),
                EnableAiStoryDirector = ReadEnvBool("TA_AI_ENABLE_STORY", true),
                RuntimeFeatureTimeoutMs = ReadEnvInt("TA_AI_RUNTIME_TIMEOUT_MS", 1200),
                NpcMovementAiEveryTurns = ReadEnvInt("TA_AI_NPC_MOVE_AI_EVERY_TURNS", 3),
                StoryDirectorAiEveryTurns = ReadEnvInt("TA_AI_STORY_AI_EVERY_TURNS", 4)
            },
            ParserOptions = new MarcusMedina.TextAdventure.AI.Models.AiParserOptions
            {
                Enabled = ReadEnvBool("TA_AI_ENABLED", true),
                PreferLocalCommandFirst = ReadEnvBool("TA_AI_PREFER_LOCAL_PARSE_FIRST", true),
                StrictMode = ReadEnvBool("TA_AI_STRICT", false),
                TimeoutMs = ReadEnvInt("TA_AI_TIMEOUT_MS", 8000),
                EstimatedTokensPerRequest = ReadEnvInt("TA_AI_ESTIMATED_TOKENS", 128)
            }
        });
    }
    catch (Exception ex)
    {
        WriteLineC64($"AI bootstrap failed: {ex.Message}");
        WriteLineC64("Falling back to Ollama defaults.");

        return AiPluginBootstrapFactory.Create(new AiPluginInitOptions
        {
            PrimaryProvider = new AiProviderInitOptions { Provider = AiProviderKind.Ollama },
            BaseParser = baseParser,
            RouterDecorator = router => new SpinnerAiProviderRouter(router, spinner)
        });
    }
}

static AiProviderInitOptions BuildProvider(string prefix, AiProviderKind defaultProvider)
{
    string providerRaw = ReadEnv($"{prefix}PROVIDER", defaultProvider.ToString());
    AiProviderKind provider = ParseProvider(providerRaw);

    return new AiProviderInitOptions
    {
        Provider = provider,
        ApiKey = ReadEnvOptional($"{prefix}API_KEY"),
        Model = ReadEnvOptional($"{prefix}MODEL"),
        Endpoint = ReadEnvOptional($"{prefix}ENDPOINT"),
        SystemPrompt = ReadEnvOptional($"{prefix}SYSTEM_PROMPT"),
        TimeoutMs = ReadEnvIntOptional($"{prefix}TIMEOUT_MS"),
        Temperature = ReadEnvDoubleOptional($"{prefix}TEMPERATURE"),
        DailyTokenLimit = ReadEnvIntOptional($"{prefix}DAILY_TOKEN_LIMIT"),
        Enabled = ReadEnvBool($"{prefix}ENABLED", true)
    };
}

static AiProviderKind ParseProvider(string raw)
{
    string key = raw.Trim().ToLowerInvariant();
    return key switch
    {
        "ollama" => AiProviderKind.Ollama,
        "lmstudio" or "lm_studio" or "lm-studio" => AiProviderKind.LmStudio,
        "dockerai" or "docker_ai" or "docker-ai" or "docker" => AiProviderKind.DockerAi,
        "openai" => AiProviderKind.OpenAi,
        "claude" or "anthropic" => AiProviderKind.Claude,
        "mistral" => AiProviderKind.Mistral,
        "openrouter" => AiProviderKind.OpenRouter,
        "1minai" or "one_min_ai" or "oneminai" => AiProviderKind.OneMinAi,
        "gemini" => AiProviderKind.Gemini,
        _ => throw new ArgumentException($"Unknown provider: {raw}")
    };
}

static string ReadEnv(string name, string fallback)
{
    string? value = Environment.GetEnvironmentVariable(name);
    return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
}

static string? ReadEnvOptional(string name)
{
    string? value = Environment.GetEnvironmentVariable(name);
    return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

static bool ReadEnvBool(string name, bool fallback)
{
    string? value = ReadEnvOptional(name);
    return bool.TryParse(value, out bool parsed) ? parsed : fallback;
}

static int ReadEnvInt(string name, int fallback)
{
    string? value = ReadEnvOptional(name);
    return int.TryParse(value, out int parsed) ? parsed : fallback;
}

static int? ReadEnvIntOptional(string name)
{
    string? value = ReadEnvOptional(name);
    return int.TryParse(value, out int parsed) ? parsed : null;
}

static double? ReadEnvDoubleOptional(string name)
{
    string? value = ReadEnvOptional(name);
    return double.TryParse(value, out double parsed) ? parsed : null;
}
