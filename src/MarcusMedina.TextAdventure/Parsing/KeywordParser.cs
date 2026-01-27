// <copyright file="KeywordParser.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using System.Linq;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Helpers;
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

        if (_config.Examine.Contains(keyword))
        {
            var target = ParseItemName(tokens, 1);
            return new ExamineCommand(target);
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

        if (_config.Move.Contains(keyword))
        {
            if (tokens.Length >= 2 && TryParseDirection(tokens[1], out var moveDirection))
            {
                return new GoCommand(moveDirection);
            }

            var target = ParseItemName(tokens, 1);
            return target != null ? new MoveCommand(target) : new MoveCommand(string.Empty);
        }

        if (_config.Read.Contains(keyword))
        {
            var target = ParseItemName(tokens, 1);
            return target != null ? new ReadCommand(target) : new UnknownCommand();
        }

        if (_config.Talk.Contains(keyword))
        {
            var target = ParseItemName(tokens, 1);
            return new TalkCommand(target);
        }

        if (_config.Attack.Contains(keyword))
        {
            var target = ParseItemName(tokens, 1);
            return new AttackCommand(target);
        }

        if (_config.Flee.Contains(keyword))
        {
            var target = ParseItemName(tokens, 1);
            return new FleeCommand(target);
        }

        if (_config.Save.Contains(keyword))
        {
            var target = ParseItemName(tokens, 1);
            return new SaveCommand(target);
        }

        if (_config.Load.Contains(keyword))
        {
            var target = ParseItemName(tokens, 1);
            return new LoadCommand(target);
        }

        if (_config.Go.Contains(keyword))
        {
            return tokens.Length >= 2 && TryParseDirection(tokens[1], out var direction)
                ? new GoCommand(direction)
                : ParseGoTarget(tokens);
        }

        if (_config.EnableFuzzyMatching)
        {
            var fuzzy = TryParseFuzzyKeyword(keyword, tokens);
            if (fuzzy != null)
            {
                return fuzzy;
            }
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

        if (_config.EnableFuzzyMatching)
        {
            var best = FuzzyMatcher.FindBestToken(token, _config.DirectionAliases.Keys, _config.FuzzyMaxDistance);
            if (!string.IsNullOrWhiteSpace(best) && _config.DirectionAliases.TryGetValue(best, out direction))
            {
                return true;
            }
        }

        direction = default;
        return false;
    }

    private ICommand? TryParseFuzzyKeyword(string keyword, string[] tokens)
    {
        var candidates = GetAllCommandKeywords();
        var best = FuzzyMatcher.FindBestToken(keyword, candidates, _config.FuzzyMaxDistance);
        if (string.IsNullOrWhiteSpace(best))
        {
            return null;
        }

        var command = BuildCommandForKeyword(best, tokens);
        return command == null
            ? null
            : new FuzzyCommand(command, best.ToProperCase());
    }

    private ICommand? BuildCommandForKeyword(string keyword, string[] tokens)
    {
        if (_config.Quit.Contains(keyword)) return new QuitCommand();
        if (_config.Examine.Contains(keyword)) return new ExamineCommand(ParseItemName(tokens, 1));
        if (_config.Look.Contains(keyword)) return new LookCommand(ParseItemName(tokens, 1));
        if (_config.Inventory.Contains(keyword)) return new InventoryCommand();
        if (_config.Stats.Contains(keyword)) return new StatsCommand();
        if (_config.Open.Contains(keyword)) return new OpenCommand();
        if (_config.Unlock.Contains(keyword)) return new UnlockCommand();
        if (_config.Take.Contains(keyword))
        {
            if (tokens.Length >= 2 && _config.All.Contains(tokens[1])) return new TakeAllCommand();
            var itemName = ParseItemName(tokens, 1);
            return itemName != null ? new TakeCommand(itemName) : new UnknownCommand();
        }
        if (_config.Drop.Contains(keyword))
        {
            if (tokens.Length >= 2 && _config.All.Contains(tokens[1])) return new DropAllCommand();
            var itemName = ParseItemName(tokens, 1);
            return itemName != null ? new DropCommand(itemName) : new UnknownCommand();
        }
        if (_config.Use.Contains(keyword))
        {
            var itemName = ParseItemName(tokens, 1);
            return itemName != null ? new UseCommand(itemName) : new UnknownCommand();
        }
        if (_config.Combine.Contains(keyword)) return ParseCombine(tokens);
        if (_config.Pour.Contains(keyword)) return ParsePour(tokens);
        if (_config.Read.Contains(keyword)) return new ReadCommand(ParseItemName(tokens, 1));
        if (_config.Move.Contains(keyword))
        {
            if (tokens.Length >= 2 && TryParseDirection(tokens[1], out var moveDirection))
            {
                return new GoCommand(moveDirection);
            }

            var target = ParseItemName(tokens, 1);
            return target != null ? new MoveCommand(target) : new MoveCommand(string.Empty);
        }
        if (_config.Talk.Contains(keyword)) return new TalkCommand(ParseItemName(tokens, 1));
        if (_config.Attack.Contains(keyword)) return new AttackCommand(ParseItemName(tokens, 1));
        if (_config.Flee.Contains(keyword)) return new FleeCommand(ParseItemName(tokens, 1));
        if (_config.Save.Contains(keyword)) return new SaveCommand(ParseItemName(tokens, 1));
        if (_config.Load.Contains(keyword)) return new LoadCommand(ParseItemName(tokens, 1));
        if (_config.Go.Contains(keyword))
        {
            return tokens.Length >= 2 && TryParseDirection(tokens[1], out var direction)
                ? new GoCommand(direction)
                : ParseGoTarget(tokens);
        }

        return null;
    }

    private IEnumerable<string> GetAllCommandKeywords()
    {
        return _config.Quit
            .Concat(_config.Examine)
            .Concat(_config.Look)
            .Concat(_config.Inventory)
            .Concat(_config.Stats)
            .Concat(_config.Open)
            .Concat(_config.Unlock)
            .Concat(_config.Take)
            .Concat(_config.Drop)
            .Concat(_config.Use)
            .Concat(_config.Combine)
            .Concat(_config.Pour)
            .Concat(_config.Move)
            .Concat(_config.Go)
            .Concat(_config.Read)
            .Concat(_config.Talk)
            .Concat(_config.Attack)
            .Concat(_config.Flee)
            .Concat(_config.Save)
            .Concat(_config.Load)
            .Distinct(StringComparer.OrdinalIgnoreCase);
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
