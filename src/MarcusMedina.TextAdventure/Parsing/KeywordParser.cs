using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Parsing;

public class KeywordParser : ICommandParser
{
    private static readonly Dictionary<string, Direction> DirectionAliases = new(StringComparer.OrdinalIgnoreCase)
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
    };

    public ICommand Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return new UnknownCommand();
        }

        var tokens = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length == 0)
        {
            return new UnknownCommand();
        }

        var keyword = tokens[0].Lower();

        if (keyword is "quit" or "exit" or "q")
        {
            return new QuitCommand();
        }

        if (keyword is "look" or "l" or "ls")
        {
            var target = ParseItemName(tokens, 1);
            return new LookCommand(target);
        }

        if (keyword is "inventory" or "inv" or "i")
        {
            return new InventoryCommand();
        }

        if (keyword is "stats" or "stat" or "hp" or "health")
        {
            return new StatsCommand();
        }

        if (keyword is "open")
        {
            return new OpenCommand();
        }

        if (keyword is "unlock")
        {
            return new UnlockCommand();
        }

        if (keyword is "take" or "get" or "pickup" or "pick" or "ta")
        {
            if (tokens.Length >= 2 && tokens[1].TextCompare("all"))
            {
                return new TakeAllCommand();
            }

            var itemName = ParseItemName(tokens, 1);
            return itemName != null ? new TakeCommand(itemName) : new UnknownCommand();
        }

        if (keyword is "drop")
        {
            if (tokens.Length >= 2 && tokens[1].TextCompare("all"))
            {
                return new DropAllCommand();
            }

            var itemName = ParseItemName(tokens, 1);
            return itemName != null ? new DropCommand(itemName) : new UnknownCommand();
        }

        if (keyword is "use" or "eat" or "bite")
        {
            var itemName = ParseItemName(tokens, 1);
            return itemName != null ? new UseCommand(itemName) : new UnknownCommand();
        }

        if (keyword is "combine" or "mix")
        {
            return ParseCombine(tokens);
        }

        if (keyword is "pour")
        {
            return ParsePour(tokens);
        }

        if (keyword is "go" or "move" or "cd")
        {
            return tokens.Length >= 2 && TryParseDirection(tokens[1], out var direction)
                ? new GoCommand(direction)
                : ParseGoTarget(tokens);
        }

        return TryParseDirection(keyword, out var directDirection)
            ? new GoCommand(directDirection)
            : new UnknownCommand();
    }

    private static bool TryParseDirection(string token, out Direction direction)
    {
        if (DirectionAliases.TryGetValue(token, out direction))
        {
            return true;
        }

        return Enum.TryParse(token, true, out direction);
    }

    private static string? ParseItemName(string[] tokens, int startIndex)
    {
        if (tokens.Length <= startIndex)
        {
            return null;
        }

        var parts = tokens.Skip(startIndex)
            .Where(t => !t.TextCompare("up") && !t.TextCompare("to"))
            .ToArray();

        return parts.Length == 0 ? null : parts.SpaceJoin();
    }

    private static ICommand ParseGoTarget(string[] tokens)
    {
        var target = ParseItemName(tokens, 1);
        return target != null ? new GoToCommand(target) : new UnknownCommand();
    }

    private static ICommand ParseCombine(string[] tokens)
    {
        if (tokens.Length < 3)
        {
            return new UnknownCommand();
        }

        var parts = tokens.Skip(1)
            .Where(t => !t.TextCompare("and") && !t.TextCompare("+"))
            .ToArray();

        if (parts.Length < 2)
        {
            return new UnknownCommand();
        }

        var right = parts[^1];
        var left = parts.Take(parts.Length - 1).SpaceJoin();
        return new CombineCommand(left, right);
    }

    private static ICommand ParsePour(string[] tokens)
    {
        var index = Array.FindIndex(tokens, t => t.TextCompare("into") || t.TextCompare("in"));
        if (index <= 1 || index >= tokens.Length - 1)
        {
            return new UnknownCommand();
        }

        var fluid = tokens.Skip(1).Take(index - 1).SpaceJoin();
        var container = tokens.Skip(index + 1).SpaceJoin();
        return new PourCommand(fluid, container);
    }
}
