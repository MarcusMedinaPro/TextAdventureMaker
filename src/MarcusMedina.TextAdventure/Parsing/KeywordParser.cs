using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Parsing;

public class KeywordParser : ICommandParser
{
    private readonly KeywordParserConfig _config;

    public KeywordParser(KeywordParserConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

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

        if (_config.Quit.Contains(keyword))
        {
            return new QuitCommand();
        }

        if (_config.Look.Contains(keyword))
        {
            var target = ParseItemName(tokens, 1);
            return new LookCommand(target);
        }

        if (_config.Inventory.Contains(keyword))
        {
            return new InventoryCommand();
        }

        if (_config.Stats.Contains(keyword))
        {
            return new StatsCommand();
        }

        if (_config.Open.Contains(keyword))
        {
            return new OpenCommand();
        }

        if (_config.Unlock.Contains(keyword))
        {
            return new UnlockCommand();
        }

        if (_config.Take.Contains(keyword))
        {
            if (tokens.Length >= 2 && _config.All.Contains(tokens[1]))
            {
                return new TakeAllCommand();
            }

            var itemName = ParseItemName(tokens, 1);
            return itemName != null ? new TakeCommand(itemName) : new UnknownCommand();
        }

        if (_config.Drop.Contains(keyword))
        {
            if (tokens.Length >= 2 && _config.All.Contains(tokens[1]))
            {
                return new DropAllCommand();
            }

            var itemName = ParseItemName(tokens, 1);
            return itemName != null ? new DropCommand(itemName) : new UnknownCommand();
        }

        if (_config.Use.Contains(keyword))
        {
            var itemName = ParseItemName(tokens, 1);
            return itemName != null ? new UseCommand(itemName) : new UnknownCommand();
        }

        if (_config.Combine.Contains(keyword))
        {
            return ParseCombine(tokens);
        }

        if (_config.Pour.Contains(keyword))
        {
            return ParsePour(tokens);
        }

        if (_config.Go.Contains(keyword))
        {
            return tokens.Length >= 2 && TryParseDirection(tokens[1], out var direction)
                ? new GoCommand(direction)
                : ParseGoTarget(tokens);
        }

        return TryParseDirection(keyword, out var directDirection)
            ? new GoCommand(directDirection)
            : new UnknownCommand();
    }

    private bool TryParseDirection(string token, out Direction direction)
    {
        if (_config.DirectionAliases.TryGetValue(token, out direction))
        {
            return true;
        }

        if (_config.AllowDirectionEnumNames)
        {
            return Enum.TryParse(token, true, out direction);
        }

        direction = default;
        return false;
    }

    private string? ParseItemName(string[] tokens, int startIndex)
    {
        if (tokens.Length <= startIndex)
        {
            return null;
        }

        var parts = tokens.Skip(startIndex)
            .Where(t => !_config.IgnoreItemTokens.Contains(t))
            .ToArray();

        return parts.Length == 0 ? null : parts.SpaceJoin();
    }

    private ICommand ParseGoTarget(string[] tokens)
    {
        var target = ParseItemName(tokens, 1);
        return target != null ? new GoToCommand(target) : new UnknownCommand();
    }

    private ICommand ParseCombine(string[] tokens)
    {
        if (tokens.Length < 3)
        {
            return new UnknownCommand();
        }

        var parts = tokens.Skip(1)
            .Where(t => !_config.CombineSeparators.Contains(t))
            .ToArray();

        if (parts.Length < 2)
        {
            return new UnknownCommand();
        }

        var right = parts[^1];
        var left = parts.Take(parts.Length - 1).SpaceJoin();
        return new CombineCommand(left, right);
    }

    private ICommand ParsePour(string[] tokens)
    {
        var index = Array.FindIndex(tokens, t => _config.PourPrepositions.Contains(t));
        if (index <= 1 || index >= tokens.Length - 1)
        {
            return new UnknownCommand();
        }

        var fluid = tokens.Skip(1).Take(index - 1).SpaceJoin();
        var container = tokens.Skip(index + 1).SpaceJoin();
        return new PourCommand(fluid, container);
    }
}
