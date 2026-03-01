using System.Text.Json;
using System.Text.RegularExpressions;
using MarcusMedina.TextAdventure.AI;
using MarcusMedina.TextAdventure.AI.Contracts;
using MarcusMedina.TextAdventure.AI.Models;
using MarcusMedina.TextAdventure.AI.Settings;
using MarcusMedina.TextAdventure.Extensions;

namespace MarcusMedina.TextAdventure.DSLHelper;

internal sealed class DslHelperAiClient(IAiProviderRouter router, int estimatedTokens = 256)
{
    private readonly IAiProviderRouter _router = router ?? throw new ArgumentNullException(nameof(router));
    private readonly int _estimatedTokens = Math.Max(1, estimatedTokens);

    public static string ActiveProviderFromEnvironment() =>
        ReadEnv("TA_AI_PROVIDER", "openai").Lower();

    public static DslHelperAiClient CreateFromEnvironment()
    {
        IAiProviderRouter router = BuildRouterFromEnvironment();
        int estimatedTokens = ReadEnvInt("TA_AI_ESTIMATED_TOKENS", 256);
        return new DslHelperAiClient(router, estimatedTokens);
    }

    public async Task<DslAiTextResult> EnhanceDescriptionAsync(
        string entityType,
        string entityId,
        string instruction,
        string worldContext,
        CancellationToken cancellationToken = default)
    {
        string systemPrompt = DslHelperAiPrompts.BuildDescriptionSystemPrompt(entityType);
        string input = DslHelperAiPrompts.BuildDescriptionInput(entityType, entityId, instruction, worldContext);
        AiRoutingResult route = await RouteAsync(systemPrompt, input, worldContext, cancellationToken).ConfigureAwait(false);

        if (!route.HasCommand)
            return new DslAiTextResult(
                Success: false,
                Text: instruction,
                Provider: route.ProviderName ?? "none",
                Error: FormatAttempts(route.Attempts));

        return new DslAiTextResult(
            Success: true,
            Text: route.CommandText!,
            Provider: route.ProviderName ?? "unknown");
    }

    public async Task<DslAiPlanResult> PlanWorldChangesAsync(
        string instruction,
        string worldContext,
        CancellationToken cancellationToken = default)
    {
        string systemPrompt = DslHelperAiPrompts.BuildActionPlannerSystemPrompt();
        string input = DslHelperAiPrompts.BuildActionPlannerInput(instruction, worldContext);
        AiRoutingResult route = await RouteAsync(systemPrompt, input, worldContext, cancellationToken).ConfigureAwait(false);

        if (!route.HasCommand)
        {
            return new DslAiPlanResult(
                Success: false,
                Actions: [],
                RawResponse: string.Empty,
                Provider: route.ProviderName ?? "none",
                Error: FormatAttempts(route.Attempts));
        }

        IReadOnlyList<DslAiAction> actions = ParseActions(route.CommandText!);
        if (actions.Count == 0)
        {
            return new DslAiPlanResult(
                Success: false,
                Actions: [],
                RawResponse: route.CommandText!,
                Provider: route.ProviderName ?? "unknown",
                Error: "AI response could not be parsed into actions.");
        }

        return new DslAiPlanResult(
            Success: true,
            Actions: actions,
            RawResponse: route.CommandText!,
            Provider: route.ProviderName ?? "unknown");
    }

    private async Task<AiRoutingResult> RouteAsync(
        string systemPrompt,
        string input,
        string worldContext,
        CancellationToken cancellationToken)
    {
        return await _router.RouteAsync(
            new AiParseRequest(
                Input: input,
                SystemPrompt: systemPrompt,
                Context: worldContext,
                Language: "en-GB",
                EstimatedTokens: _estimatedTokens),
            cancellationToken).ConfigureAwait(false);
    }

    private static IAiProviderRouter BuildRouterFromEnvironment()
    {
        AiCommandParserBuilder builder = new();
        AddProvider(builder, ActiveProviderFromEnvironment(), "TA_AI_");

        string? fallback = ReadEnvOptional("TA_AI_FALLBACK_PROVIDER");
        if (!string.IsNullOrWhiteSpace(fallback))
            AddProvider(builder, fallback, "TA_AI_FALLBACK_");

        return builder.BuildRouter();
    }

    private static void AddProvider(AiCommandParserBuilder builder, string providerRaw, string prefix)
    {
        string provider = providerRaw.Trim().ToLowerInvariant();
        switch (provider)
        {
            case "ollama":
                builder.UseOllama(new OllamaSettings
                {
                    Endpoint = ReadEnv($"{prefix}ENDPOINT", "http://localhost:11434"),
                    Model = ReadEnv($"{prefix}MODEL", "llama2"),
                    SystemPrompt = ReadEnv($"{prefix}SYSTEM_PROMPT", "You are a British-English text-adventure assistant."),
                    TimeoutMs = ReadEnvInt($"{prefix}TIMEOUT_MS", 5000),
                    Temperature = ReadEnvDouble($"{prefix}TEMPERATURE", 0.2),
                    Enabled = ReadEnvBool($"{prefix}ENABLED", true)
                });
                return;

            case "lmstudio":
            case "lm_studio":
            case "lm-studio":
                builder.UseLmStudio(new LmStudioSettings
                {
                    Endpoint = ReadEnv($"{prefix}ENDPOINT", "http://localhost:1234/v1/chat/completions"),
                    Model = ReadEnv($"{prefix}MODEL", "local-model"),
                    ApiKey = ReadEnvOptional($"{prefix}API_KEY"),
                    SystemPrompt = ReadEnvOptional($"{prefix}SYSTEM_PROMPT"),
                    TimeoutMs = ReadEnvInt($"{prefix}TIMEOUT_MS", 8000),
                    Temperature = ReadEnvDouble($"{prefix}TEMPERATURE", 0.2),
                    Enabled = ReadEnvBool($"{prefix}ENABLED", true)
                });
                return;

            case "dockerai":
            case "docker_ai":
            case "docker-ai":
            case "docker":
                builder.UseDockerAi(new DockerAiSettings
                {
                    Endpoint = ReadEnv($"{prefix}ENDPOINT", "http://localhost:12434/v1/chat/completions"),
                    Model = ReadEnv($"{prefix}MODEL", "local-model"),
                    ApiKey = ReadEnvOptional($"{prefix}API_KEY"),
                    SystemPrompt = ReadEnvOptional($"{prefix}SYSTEM_PROMPT"),
                    TimeoutMs = ReadEnvInt($"{prefix}TIMEOUT_MS", 8000),
                    Temperature = ReadEnvDouble($"{prefix}TEMPERATURE", 0.2),
                    Enabled = ReadEnvBool($"{prefix}ENABLED", true)
                });
                return;

            case "openai":
                builder.UseOpenAi(new OpenAiSettings(ReadEnvOptional($"{prefix}API_KEY") ?? string.Empty)
                {
                    Endpoint = ReadEnv($"{prefix}ENDPOINT", "https://api.openai.com/v1/responses"),
                    Model = ReadEnv($"{prefix}MODEL", "gpt-4o-mini"),
                    SystemPrompt = ReadEnvOptional($"{prefix}SYSTEM_PROMPT"),
                    TimeoutMs = ReadEnvInt($"{prefix}TIMEOUT_MS", 8000),
                    Temperature = ReadEnvDouble($"{prefix}TEMPERATURE", 0.2),
                    Enabled = ReadEnvBool($"{prefix}ENABLED", true)
                });
                return;

            case "claude":
            case "anthropic":
                builder.UseClaude(new ClaudeSettings(ReadEnvOptional($"{prefix}API_KEY") ?? string.Empty)
                {
                    Endpoint = ReadEnv($"{prefix}ENDPOINT", "https://api.anthropic.com/v1/messages"),
                    Model = ReadEnv($"{prefix}MODEL", "claude-3-5-haiku-latest"),
                    SystemPrompt = ReadEnvOptional($"{prefix}SYSTEM_PROMPT"),
                    TimeoutMs = ReadEnvInt($"{prefix}TIMEOUT_MS", 8000),
                    Temperature = ReadEnvDouble($"{prefix}TEMPERATURE", 0.2),
                    Enabled = ReadEnvBool($"{prefix}ENABLED", true)
                });
                return;

            case "mistral":
                builder.UseMistral(new MistralSettings(ReadEnvOptional($"{prefix}API_KEY") ?? string.Empty)
                {
                    Endpoint = ReadEnv($"{prefix}ENDPOINT", "https://api.mistral.ai/v1/chat/completions"),
                    Model = ReadEnv($"{prefix}MODEL", "mistral-small-latest"),
                    SystemPrompt = ReadEnvOptional($"{prefix}SYSTEM_PROMPT"),
                    TimeoutMs = ReadEnvInt($"{prefix}TIMEOUT_MS", 8000),
                    Temperature = ReadEnvDouble($"{prefix}TEMPERATURE", 0.2),
                    Enabled = ReadEnvBool($"{prefix}ENABLED", true)
                });
                return;

            case "openrouter":
                builder.UseOpenRouter(new OpenRouterSettings(ReadEnvOptional($"{prefix}API_KEY") ?? string.Empty)
                {
                    Endpoint = ReadEnv($"{prefix}ENDPOINT", "https://openrouter.ai/api/v1/chat/completions"),
                    Model = ReadEnv($"{prefix}MODEL", "openai/gpt-4o-mini"),
                    SystemPrompt = ReadEnvOptional($"{prefix}SYSTEM_PROMPT"),
                    TimeoutMs = ReadEnvInt($"{prefix}TIMEOUT_MS", 8000),
                    Temperature = ReadEnvDouble($"{prefix}TEMPERATURE", 0.2),
                    Enabled = ReadEnvBool($"{prefix}ENABLED", true)
                });
                return;

            case "1minai":
            case "one_min_ai":
            case "oneminai":
                builder.UseOneMinAi(new OneMinAiSettings(ReadEnvOptional($"{prefix}API_KEY") ?? string.Empty)
                {
                    Endpoint = ReadEnv($"{prefix}ENDPOINT", "https://api.1min.ai/api/features"),
                    Model = ReadEnv($"{prefix}MODEL", "gpt-4o-mini"),
                    Feature = ReadEnv($"{prefix}FEATURE", "ai-text-chat"),
                    SystemPrompt = ReadEnvOptional($"{prefix}SYSTEM_PROMPT"),
                    TimeoutMs = ReadEnvInt($"{prefix}TIMEOUT_MS", 8000),
                    Temperature = ReadEnvDouble($"{prefix}TEMPERATURE", 0.2),
                    DailyTokenLimit = ReadEnvInt($"{prefix}DAILY_TOKEN_LIMIT", 15000),
                    Enabled = ReadEnvBool($"{prefix}ENABLED", true)
                });
                return;

            case "gemini":
                builder.UseGemini(new GeminiSettings(ReadEnvOptional($"{prefix}API_KEY") ?? string.Empty)
                {
                    EndpointTemplate = ReadEnv($"{prefix}ENDPOINT", "https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}"),
                    Model = ReadEnv($"{prefix}MODEL", "gemini-1.5-flash"),
                    SystemPrompt = ReadEnvOptional($"{prefix}SYSTEM_PROMPT"),
                    TimeoutMs = ReadEnvInt($"{prefix}TIMEOUT_MS", 8000),
                    Temperature = ReadEnvDouble($"{prefix}TEMPERATURE", 0.2),
                    Enabled = ReadEnvBool($"{prefix}ENABLED", true)
                });
                return;

            default:
                AddProvider(builder, "openai", prefix);
                return;
        }
    }

    private static IReadOnlyList<DslAiAction> ParseActions(string response)
    {
        if (TryParseJsonActions(response, out IReadOnlyList<DslAiAction>? jsonActions))
            return jsonActions;

        if (TryParseLooseActions(response, out IReadOnlyList<DslAiAction>? looseActions))
            return looseActions;

        return [];
    }

    private static bool TryParseJsonActions(string response, out IReadOnlyList<DslAiAction> actions)
    {
        actions = [];

        try
        {
            using JsonDocument document = JsonDocument.Parse(response);
            JsonElement root = document.RootElement;
            if (root.ValueKind == JsonValueKind.Object)
            {
                if (TryGetPropertyIgnoreCase(root, "actions", out JsonElement actionArray)
                    && actionArray.ValueKind == JsonValueKind.Array)
                {
                    actions = ParseActionArray(actionArray);
                    return actions.Count > 0;
                }

                actions = [ParseAction(root)];
                return true;
            }

            if (root.ValueKind == JsonValueKind.Array)
            {
                actions = ParseActionArray(root);
                return actions.Count > 0;
            }
        }
        catch (JsonException)
        {
            return false;
        }

        return false;
    }

    private static IReadOnlyList<DslAiAction> ParseActionArray(JsonElement array)
    {
        List<DslAiAction> actions = [];
        foreach (JsonElement element in array.EnumerateArray())
        {
            if (element.ValueKind != JsonValueKind.Object)
                continue;
            actions.Add(ParseAction(element));
        }

        return actions;
    }

    private static DslAiAction ParseAction(JsonElement element)
    {
        string action = ReadString(element, "action", "type", "op").ToId();
        return new DslAiAction(
            Action: string.IsNullOrWhiteSpace(action) ? "none" : action,
            RoomId: ReadString(element, "room_id", "room", "location_id"),
            FromId: ReadString(element, "from_id", "from", "source_room"),
            ToId: ReadString(element, "to_id", "to", "target_room"),
            Direction: ReadString(element, "direction", "dir"),
            DoorId: ReadString(element, "door_id"),
            DoorName: ReadString(element, "door_name", "name"),
            ItemId: ReadString(element, "item_id"),
            ItemName: ReadString(element, "item_name"),
            NpcId: ReadString(element, "npc_id"),
            NpcName: ReadString(element, "npc_name"),
            Description: ReadString(element, "description", "desc", "text"),
            Reason: ReadString(element, "reason", "note"));
    }

    private static bool TryParseLooseActions(string response, out IReadOnlyList<DslAiAction> actions)
    {
        List<DslAiAction> parsed = [];
        MatchCollection blocks = Regex.Matches(response, @"\{[^{}]+\}");
        if (blocks.Count == 0)
        {
            actions = [];
            return false;
        }

        foreach (Match block in blocks)
        {
            string body = block.Value.Trim('{', '}').Trim();
            string lower = body.ToLowerInvariant();

            if (lower.Contains("create room", StringComparison.Ordinal))
            {
                Match roomMatch = Regex.Match(body, @"create\s+room[,:\s]+(?<room>[a-zA-Z0-9_\- ]+)", RegexOptions.IgnoreCase);
                string roomId = roomMatch.Success ? roomMatch.Groups["room"].Value.Trim().ToId() : string.Empty;
                if (!string.IsNullOrWhiteSpace(roomId))
                    parsed.Add(new DslAiAction("create_room", RoomId: roomId));
                continue;
            }

            if (lower.Contains("add door", StringComparison.Ordinal))
            {
                Match doorMatch = Regex.Match(
                    body,
                    @"add\s+door\s+(?<from>[a-zA-Z0-9_\- ]+)\s*,\s*(?<to>[a-zA-Z0-9_\- ]+)",
                    RegexOptions.IgnoreCase);
                if (!doorMatch.Success)
                {
                    parsed.Add(new DslAiAction("add_door", FromId: "this", ToId: "entry"));
                    continue;
                }

                parsed.Add(new DslAiAction(
                    "add_door",
                    FromId: doorMatch.Groups["from"].Value.Trim().ToId(),
                    ToId: doorMatch.Groups["to"].Value.Trim().ToId()));
            }
        }

        actions = parsed;
        return parsed.Count > 0;
    }

    private static string ReadString(JsonElement element, params string[] names)
    {
        foreach (string name in names)
        {
            if (!TryGetPropertyIgnoreCase(element, name, out JsonElement value))
                continue;

            if (value.ValueKind == JsonValueKind.String)
                return value.GetString() ?? string.Empty;
            if (value.ValueKind == JsonValueKind.Number)
                return value.ToString();
            if (value.ValueKind == JsonValueKind.True)
                return "true";
            if (value.ValueKind == JsonValueKind.False)
                return "false";
        }

        return string.Empty;
    }

    private static bool TryGetPropertyIgnoreCase(JsonElement element, string propertyName, out JsonElement value)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (JsonProperty property in element.EnumerateObject())
            {
                if (!property.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                    continue;

                value = property.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    private static string FormatAttempts(IReadOnlyList<AiProviderAttempt> attempts)
    {
        if (attempts.Count == 0)
            return "No AI provider attempts were made.";

        return attempts
            .Select(attempt => $"{attempt.ProviderName}: {attempt.Message ?? attempt.Outcome.ToString()}")
            .ToArray()
            .CommaJoin();
    }

    private static string ReadEnv(string name, string fallback)
    {
        string? value = Environment.GetEnvironmentVariable(name);
        return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
    }

    private static string? ReadEnvOptional(string name)
    {
        string? value = Environment.GetEnvironmentVariable(name);
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static bool ReadEnvBool(string name, bool fallback)
    {
        string? value = ReadEnvOptional(name);
        return bool.TryParse(value, out bool parsed) ? parsed : fallback;
    }

    private static int ReadEnvInt(string name, int fallback)
    {
        string? value = ReadEnvOptional(name);
        return int.TryParse(value, out int parsed) ? parsed : fallback;
    }

    private static double ReadEnvDouble(string name, double fallback)
    {
        string? value = ReadEnvOptional(name);
        return double.TryParse(value, out double parsed) ? parsed : fallback;
    }
}
