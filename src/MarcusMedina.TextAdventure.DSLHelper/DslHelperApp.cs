using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Extensions;
using static MarcusMedina.TextAdventure.Engine.ConsoleRenderer;

namespace MarcusMedina.TextAdventure.DSLHelper;

internal sealed class DslHelperApp(DslHelperAiClient aiClient, DslWorldBuilder world)
{
    private readonly DslHelperAiClient _aiClient = aiClient ?? throw new ArgumentNullException(nameof(aiClient));
    private readonly DslWorldBuilder _world = world ?? throw new ArgumentNullException(nameof(world));

    public static DslHelperApp CreateFromEnvironment()
    {
        DslWorldBuilder world = new();
        DslHelperAiClient aiClient = DslHelperAiClient.CreateFromEnvironment();
        return new DslHelperApp(aiClient, world);
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        SetupC64("TextAdventure DSL Helper");
        WriteLineC64("=== TEXT ADVENTURE DSL HELPER ===");
        WriteLineC64($"Provider: {DslHelperAiClient.ActiveProviderFromEnvironment()}");
        WriteLineC64("Goal: quickly create and maintain rooms, doors, items, and NPCs.");
        WriteLineC64();
        WriteLineC64("You are in a room.");
        WriteLineC64("You can see nothing.");
        WriteLineC64();
        WriteLineC64("Use '/AI' at the end of a description to ask AI to improve it.");
        WriteLineC64("Example: describe room: The room is spooky /AI");
        WriteLineC64();
        PrintHelp();
        PrintCurrentRoom();

        while (!cancellationToken.IsCancellationRequested)
        {
            WritePromptC64("> ");
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                continue;

            string command = input.Trim();
            if (command.TextCompare("quit") || command.TextCompare("exit"))
                break;

            if (command.TextCompare("help"))
            {
                PrintHelp();
                continue;
            }

            if (command.TextCompare("look"))
            {
                PrintCurrentRoom();
                continue;
            }

            if (command.TextCompare("rooms") || command.TextCompare("list rooms"))
            {
                PrintRooms();
                continue;
            }

            if (command.TextCompare("items") || command.StartsWithIgnoreCase("list items"))
            {
                HandleListItems(command);
                continue;
            }

            if (command.TextCompare("npcs") || command.StartsWithIgnoreCase("list npcs"))
            {
                HandleListNpcs(command);
                continue;
            }

            if (command.TextCompare("doors") || command.TextCompare("list doors"))
            {
                PrintLines(_world.ListDoors());
                continue;
            }

            if (command.TextCompare("dsl"))
            {
                WriteLineC64(_world.ExportDsl());
                continue;
            }

            if (command.StartsWithIgnoreCase("world:"))
            {
                _world.WorldTitle = command["world:".Length..].Trim();
                WriteLineC64($"World set to: {_world.WorldTitle}");
                continue;
            }

            if (command.StartsWithIgnoreCase("goal:"))
            {
                _world.Goal = command["goal:".Length..].Trim();
                WriteLineC64($"Goal set to: {_world.Goal}");
                continue;
            }

            if (command.StartsWithIgnoreCase("save"))
            {
                HandleSave(command);
                continue;
            }

            if (command.StartsWithIgnoreCase("go "))
            {
                HandleMove(command["go".Length..].Trim());
                continue;
            }

            if (command.StartsWithIgnoreCase("create room:"))
            {
                HandleCreateRoom(command["create room:".Length..].Trim());
                continue;
            }

            if (command.StartsWithIgnoreCase("create item:"))
            {
                HandleCreateItem(command["create item:".Length..].Trim());
                continue;
            }

            if (command.StartsWithIgnoreCase("create npc:"))
            {
                HandleCreateNpc(command["create npc:".Length..].Trim());
                continue;
            }

            if (command.StartsWithIgnoreCase("create door:"))
            {
                HandleCreateDoor(command["create door:".Length..].Trim());
                continue;
            }

            if (command.StartsWithIgnoreCase("delete room"))
            {
                string roomId = command["delete room".Length..].TrimStart(':', ' ');
                WriteLineC64(_world.DeleteRoom(roomId));
                PrintCurrentRoom();
                continue;
            }

            if (command.StartsWithIgnoreCase("delete item"))
            {
                string itemId = command["delete item".Length..].TrimStart(':', ' ');
                WriteLineC64(_world.DeleteItem(itemId));
                continue;
            }

            if (command.StartsWithIgnoreCase("delete npc"))
            {
                string npcId = command["delete npc".Length..].TrimStart(':', ' ');
                WriteLineC64(_world.DeleteNpc(npcId));
                continue;
            }

            if (command.StartsWithIgnoreCase("delete door"))
            {
                string doorId = command["delete door".Length..].TrimStart(':', ' ');
                WriteLineC64(_world.DeleteDoor(doorId));
                continue;
            }

            if (command.StartsWithIgnoreCase("update room"))
            {
                await HandleDescribeRoomAsync(RewritePrefix(command, "update room", "describe room"), cancellationToken).ConfigureAwait(false);
                continue;
            }

            if (command.StartsWithIgnoreCase("update item"))
            {
                await HandleDescribeItemAsync(RewritePrefix(command, "update item", "describe item"), cancellationToken).ConfigureAwait(false);
                continue;
            }

            if (command.StartsWithIgnoreCase("update npc"))
            {
                await HandleDescribeNpcAsync(RewritePrefix(command, "update npc", "describe npc"), cancellationToken).ConfigureAwait(false);
                continue;
            }

            if (command.StartsWithIgnoreCase("update door"))
            {
                await HandleDescribeDoorAsync(RewritePrefix(command, "update door", "describe door"), cancellationToken).ConfigureAwait(false);
                continue;
            }

            if (command.StartsWithIgnoreCase("describe room"))
            {
                await HandleDescribeRoomAsync(command, cancellationToken).ConfigureAwait(false);
                continue;
            }

            if (command.StartsWithIgnoreCase("describe item"))
            {
                await HandleDescribeItemAsync(command, cancellationToken).ConfigureAwait(false);
                continue;
            }

            if (command.StartsWithIgnoreCase("describe npc"))
            {
                await HandleDescribeNpcAsync(command, cancellationToken).ConfigureAwait(false);
                continue;
            }

            if (command.StartsWithIgnoreCase("describe door"))
            {
                await HandleDescribeDoorAsync(command, cancellationToken).ConfigureAwait(false);
                continue;
            }

            if (command.StartsWithIgnoreCase("add "))
            {
                await HandleAiActionPlanAsync(command, cancellationToken).ConfigureAwait(false);
                continue;
            }

            WriteLineC64("Unknown command. Type 'help' for available commands.");
        }
    }

    private void HandleCreateRoom(string payload)
    {
        (string roomId, string? description) = SplitPipe(payload);
        WriteLineC64(_world.CreateRoom(roomId, description));
    }

    private void HandleCreateDoor(string payload)
    {
        (string main, string? direction) = SplitPipe(payload);
        string[] parts = main.Split("->", 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            WriteLineC64("Use: create door: <from_room> -> <to_room> | <optional direction>");
            return;
        }

        WriteLineC64(_world.AddDoorStrict(
            parts[0],
            parts[1],
            directionRaw: direction,
            doorIdRaw: null,
            doorName: null,
            doorDescription: null));
    }

    private void HandleCreateItem(string payload)
    {
        if (!TrySplitInPattern(payload, out string itemId, out string roomId))
        {
            WriteLineC64("Use: create item: <item_id> in <room_id>");
            return;
        }

        WriteLineC64(_world.AddItem(roomId, itemId, itemName: null, description: null));
    }

    private void HandleCreateNpc(string payload)
    {
        if (!TrySplitInPattern(payload, out string npcId, out string roomId))
        {
            WriteLineC64("Use: create npc: <npc_id> in <room_id>");
            return;
        }

        WriteLineC64(_world.AddNpc(roomId, npcId, npcName: null, description: null));
    }

    private void HandleListItems(string command)
    {
        string tail = command.TextCompare("items")
            ? string.Empty
            : command["list items".Length..].Trim();
        if (tail.StartsWithIgnoreCase("in "))
            tail = tail[3..].Trim();

        PrintLines(string.IsNullOrWhiteSpace(tail) ? _world.ListItems() : _world.ListItems(tail));
    }

    private void HandleListNpcs(string command)
    {
        string tail = command.TextCompare("npcs")
            ? string.Empty
            : command["list npcs".Length..].Trim();
        if (tail.StartsWithIgnoreCase("in "))
            tail = tail[3..].Trim();

        PrintLines(string.IsNullOrWhiteSpace(tail) ? _world.ListNpcs() : _world.ListNpcs(tail));
    }

    private async Task HandleDescribeRoomAsync(string command, CancellationToken cancellationToken)
    {
        if (!TryParseDescribeCommand(command, "describe room", out string target, out string text))
        {
            WriteLineC64("Use: describe room: <text> [/AI] or describe room <room_id>: <text> [/AI]");
            return;
        }

        (string content, bool useAi) = SplitAiSuffix(text);
        if (string.IsNullOrWhiteSpace(content))
        {
            WriteLineC64("Missing room description text.");
            return;
        }

        string roomTarget = string.IsNullOrWhiteSpace(target) ? "this" : target;
        if (!useAi)
        {
            WriteLineC64(_world.DescribeRoom(roomTarget, content));
            PrintCurrentRoom();
            return;
        }

        DslAiTextResult result = await _aiClient.EnhanceDescriptionAsync(
            "room",
            roomTarget,
            content,
            _world.BuildPromptContext(),
            cancellationToken).ConfigureAwait(false);

        if (!result.Success)
        {
            WriteLineC64($"AI unavailable: {result.Error ?? "No provider response."}");
            return;
        }

        WriteLineC64($"AI ({result.Provider}): {result.Text}");
        WriteLineC64(_world.DescribeRoom(roomTarget, result.Text));
        PrintCurrentRoom();
    }

    private async Task HandleDescribeItemAsync(string command, CancellationToken cancellationToken)
    {
        if (!TryParseDescribeCommand(command, "describe item", out string itemId, out string text))
        {
            WriteLineC64("Use: describe item <item_id>: <text> [/AI]");
            return;
        }

        (string content, bool useAi) = SplitAiSuffix(text);
        if (string.IsNullOrWhiteSpace(itemId) || string.IsNullOrWhiteSpace(content))
        {
            WriteLineC64("Both item id and description are required.");
            return;
        }

        if (!useAi)
        {
            WriteLineC64(_world.DescribeItem(itemId, content));
            return;
        }

        DslAiTextResult result = await _aiClient.EnhanceDescriptionAsync(
            "item",
            itemId,
            content,
            _world.BuildPromptContext(),
            cancellationToken).ConfigureAwait(false);

        if (!result.Success)
        {
            WriteLineC64($"AI unavailable: {result.Error ?? "No provider response."}");
            return;
        }

        WriteLineC64($"AI ({result.Provider}): {result.Text}");
        WriteLineC64(_world.DescribeItem(itemId, result.Text));
    }

    private async Task HandleDescribeNpcAsync(string command, CancellationToken cancellationToken)
    {
        if (!TryParseDescribeCommand(command, "describe npc", out string npcId, out string text))
        {
            WriteLineC64("Use: describe npc <npc_id>: <text> [/AI]");
            return;
        }

        (string content, bool useAi) = SplitAiSuffix(text);
        if (string.IsNullOrWhiteSpace(npcId) || string.IsNullOrWhiteSpace(content))
        {
            WriteLineC64("Both NPC id and description are required.");
            return;
        }

        if (!useAi)
        {
            WriteLineC64(_world.DescribeNpc(npcId, content));
            return;
        }

        DslAiTextResult result = await _aiClient.EnhanceDescriptionAsync(
            "npc",
            npcId,
            content,
            _world.BuildPromptContext(),
            cancellationToken).ConfigureAwait(false);

        if (!result.Success)
        {
            WriteLineC64($"AI unavailable: {result.Error ?? "No provider response."}");
            return;
        }

        WriteLineC64($"AI ({result.Provider}): {result.Text}");
        WriteLineC64(_world.DescribeNpc(npcId, result.Text));
    }

    private async Task HandleDescribeDoorAsync(string command, CancellationToken cancellationToken)
    {
        if (!TryParseDescribeCommand(command, "describe door", out string doorId, out string text))
        {
            WriteLineC64("Use: describe door <door_id>: <text> [/AI]");
            return;
        }

        (string content, bool useAi) = SplitAiSuffix(text);
        if (string.IsNullOrWhiteSpace(doorId) || string.IsNullOrWhiteSpace(content))
        {
            WriteLineC64("Both door id and description are required.");
            return;
        }

        if (!useAi)
        {
            WriteLineC64(_world.DescribeDoor(doorId, content));
            return;
        }

        DslAiTextResult result = await _aiClient.EnhanceDescriptionAsync(
            "door",
            doorId,
            content,
            _world.BuildPromptContext(),
            cancellationToken).ConfigureAwait(false);

        if (!result.Success)
        {
            WriteLineC64($"AI unavailable: {result.Error ?? "No provider response."}");
            return;
        }

        WriteLineC64($"AI ({result.Provider}): {result.Text}");
        WriteLineC64(_world.DescribeDoor(doorId, result.Text));
    }

    private async Task HandleAiActionPlanAsync(string command, CancellationToken cancellationToken)
    {
        DslAiPlanResult plan = await _aiClient.PlanWorldChangesAsync(
            command,
            _world.BuildPromptContext(),
            cancellationToken).ConfigureAwait(false);

        if (!plan.Success || plan.Actions.Count == 0)
        {
            string error = plan.Error ?? "AI returned no usable actions.";
            WriteLineC64($"AI unavailable: {error}");
            return;
        }

        WriteLineC64($"AI ({plan.Provider}): {plan.RawResponse}");
        foreach (DslAiAction action in plan.Actions)
            WriteLineC64(ApplyAction(action));

        PrintCurrentRoom();
    }

    private string ApplyAction(DslAiAction action)
    {
        return action.Action.ToId() switch
        {
            "create_room" => _world.CreateRoom(action.RoomId ?? action.ToId, action.Description),
            "add_door" => _world.AddDoorStrict(
                action.FromId,
                action.ToId,
                action.Direction,
                action.DoorId,
                action.DoorName,
                action.Description),
            "add_item" => _world.AddItem(
                action.RoomId,
                action.ItemId,
                action.ItemName,
                action.Description),
            "add_npc" => _world.AddNpc(
                action.RoomId,
                action.NpcId,
                action.NpcName,
                action.Description),
            "describe_room" => _world.DescribeRoom(action.RoomId, action.Description),
            "describe_item" => _world.DescribeItem(action.ItemId, action.Description),
            "describe_npc" => _world.DescribeNpc(action.NpcId, action.Description),
            "describe_door" => _world.DescribeDoor(action.DoorId, action.Description),
            "move_to" => _world.MoveTo(action.RoomId ?? action.ToId),
            "delete_room" => _world.DeleteRoom(action.RoomId),
            "delete_item" => _world.DeleteItem(action.ItemId),
            "delete_npc" => _world.DeleteNpc(action.NpcId),
            "delete_door" => _world.DeleteDoor(action.DoorId),
            "none" => string.IsNullOrWhiteSpace(action.Reason) ? "AI suggested no world changes." : $"AI note: {action.Reason}",
            _ => $"Skipped unsupported action '{action.Action}'."
        };
    }

    private static bool TryParseDescribeCommand(string command, string prefix, out string target, out string description)
    {
        target = string.Empty;
        description = string.Empty;

        string trimmed = command.Trim();
        if (!trimmed.StartsWithIgnoreCase(prefix))
            return false;

        string remainder = trimmed[prefix.Length..].Trim();
        int colon = remainder.IndexOf(':');
        if (colon < 0)
            return false;

        target = remainder[..colon].Trim();
        description = remainder[(colon + 1)..].Trim();
        return true;
    }

    private static (string Main, string? Extra) SplitPipe(string text)
    {
        int index = text.IndexOf('|');
        if (index < 0)
            return (text.Trim(), null);

        string main = text[..index].Trim();
        string extra = text[(index + 1)..].Trim();
        return (main, string.IsNullOrWhiteSpace(extra) ? null : extra);
    }

    private static (string Content, bool UseAi) SplitAiSuffix(string text)
    {
        string trimmed = text.Trim();
        if (!trimmed.EndsWith("/AI", StringComparison.OrdinalIgnoreCase))
            return (trimmed, false);

        string content = trimmed[..^3].TrimEnd();
        return (content, true);
    }

    private static string RewritePrefix(string command, string currentPrefix, string newPrefix) =>
        $"{newPrefix}{command[currentPrefix.Length..]}";

    private static bool TrySplitInPattern(string text, out string left, out string right)
    {
        left = string.Empty;
        right = string.Empty;

        int marker = text.IndexOf(" in ", StringComparison.OrdinalIgnoreCase);
        if (marker <= 0 || marker >= text.Length - 4)
            return false;

        left = text[..marker].Trim();
        right = text[(marker + 4)..].Trim();
        return !string.IsNullOrWhiteSpace(left) && !string.IsNullOrWhiteSpace(right);
    }

    private void HandleSave(string command)
    {
        string? rawPath = command["save".Length..].Trim();
        string path = string.IsNullOrWhiteSpace(rawPath)
            ? $"generated_{DateTime.UtcNow:yyyyMMdd_HHmmss}.adventure"
            : rawPath!;
        string fullPath = _world.Save(path);
        WriteLineC64($"Saved DSL to: {fullPath}");
    }

    private void HandleMove(string roomId)
    {
        string result = _world.MoveTo(roomId);
        WriteLineC64(result);
        if (result.StartsWith("You move", StringComparison.OrdinalIgnoreCase))
            PrintCurrentRoom();
    }

    private void PrintCurrentRoom()
    {
        foreach (string line in _world.GetCurrentRoomLines())
            WriteLineC64(line);
    }

    private void PrintRooms()
    {
        WriteLineC64("Rooms:");
        foreach (string room in _world.ListRooms())
            WriteLineC64($"- {room}");
    }

    private static void PrintLines(IEnumerable<string> lines)
    {
        foreach (string line in lines)
            WriteLineC64($"- {line}");
    }

    private static void PrintHelp()
    {
        WriteLineC64("CREATE");
        WriteLineC64("create room: <room_id> | <optional description>");
        WriteLineC64("create item: <item_id> in <room_id>");
        WriteLineC64("create npc: <npc_id> in <room_id>");
        WriteLineC64("create door: <from_room> -> <to_room> | <optional direction>");
        WriteLineC64("READ");
        WriteLineC64("rooms or list rooms");
        WriteLineC64("items or list items [in <room_id>]");
        WriteLineC64("npcs or list npcs [in <room_id>]");
        WriteLineC64("doors or list doors");
        WriteLineC64("UPDATE");
        WriteLineC64("describe room: <text> [/AI]");
        WriteLineC64("describe room <room_id>: <text> [/AI]");
        WriteLineC64("describe item <item_id>: <text> [/AI]");
        WriteLineC64("describe npc <npc_id>: <text> [/AI]");
        WriteLineC64("describe door <door_id>: <text> [/AI]");
        WriteLineC64("update room/item/npc/door ... (alias for describe)");
        WriteLineC64("DELETE");
        WriteLineC64("delete room: <room_id>");
        WriteLineC64("delete item: <item_id>");
        WriteLineC64("delete npc: <npc_id>");
        WriteLineC64("delete door: <door_id>");
        WriteLineC64("OTHER");
        WriteLineC64("add <instruction> (AI action planner)");
        WriteLineC64("go <room_id>, look, dsl, save [path], world:, goal:, help, quit");
    }
}
