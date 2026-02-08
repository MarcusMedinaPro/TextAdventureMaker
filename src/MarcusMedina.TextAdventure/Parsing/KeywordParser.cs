// <copyright file="KeywordParser.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using System.Linq;

namespace MarcusMedina.TextAdventure.Parsing;

public class KeywordParser(KeywordParserConfig config) : ICommandParser
{
    private readonly KeywordParserConfig _config = config ?? throw new ArgumentNullException(nameof(config));
    private string? _lastInput;
    private string? _lastTarget;
    private bool _pronounMissing;

    public ICommand Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return new UnknownCommand();
        }

        _pronounMissing = false;

        string trimmed = input.Trim();
        string lower = trimmed.ToLowerInvariant();

        if (_config.Again.Contains(lower))
        {
            if (string.IsNullOrWhiteSpace(_lastInput))
            {
                return new ParserErrorCommand("Nothing to repeat.");
            }

            trimmed = _lastInput;
        }

        trimmed = ApplyPhraseAliases(trimmed);
        _lastInput = trimmed;

        ICommand? custom = TryParseCustomCommand(trimmed);
        if (custom != null)
        {
            return GuardPronoun(custom);
        }

        string[] tokens = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length == 0)
        {
            return new UnknownCommand();
        }

        string keyword = NormalizeKeyword(tokens[0]);

        if (_config.Quit.Contains(keyword))
        {
            return GuardPronoun(new QuitCommand());
        }

        if (_config.Examine.Contains(keyword))
        {
            string? target = ParseItemName(tokens, 1);
            return GuardPronoun(new ExamineCommand(target));
        }

        if (_config.Look.Contains(keyword))
        {
            string? target = ParseItemName(tokens, 1);
            return GuardPronoun(new LookCommand(target));
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

        if (_config.Close.Contains(keyword))
        {
            return new CloseCommand();
        }

        if (_config.LockDoor.Contains(keyword))
        {
            return new LockCommand();
        }

        if (_config.Destroy.Contains(keyword))
        {
            return GuardPronoun(new DestroyCommand(ParseItemName(tokens, 1)));
        }

        if (_config.Take.Contains(keyword))
        {
            if (tokens.Length >= 2 && _config.All.Contains(tokens[1]))
            {
                return new TakeAllCommand();
            }

            string? itemName = ParseItemName(tokens, 1);
            ICommand command = itemName != null ? new TakeCommand(itemName) : new UnknownCommand();
            return GuardPronoun(command);
        }

        if (_config.Drop.Contains(keyword))
        {
            if (tokens.Length >= 2 && _config.All.Contains(tokens[1]))
            {
                return new DropAllCommand();
            }

            string? itemName = ParseItemName(tokens, 1);
            ICommand command = itemName != null ? new DropCommand(itemName) : new UnknownCommand();
            return GuardPronoun(command);
        }

        if (_config.Use.Contains(keyword))
        {
            string? itemName = ParseItemName(tokens, 1);
            ICommand command = itemName != null ? new UseCommand(itemName) : new UnknownCommand();
            return GuardPronoun(command);
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
            if (tokens.Length >= 2 && TryParseDirection(tokens[1], out Direction moveDirection))
            {
                return new GoCommand(moveDirection);
            }

            string? target = ParseItemName(tokens, 1);
            ICommand command = target != null ? new MoveCommand(target) : new MoveCommand(string.Empty);
            return GuardPronoun(command);
        }

        if (_config.Read.Contains(keyword))
        {
            string? target = ParseItemName(tokens, 1);
            ICommand command = target != null ? new ReadCommand(target) : new UnknownCommand();
            return GuardPronoun(command);
        }

        if (_config.Talk.Contains(keyword))
        {
            string? target = ParseItemName(tokens, 1);
            return GuardPronoun(new TalkCommand(target));
        }

        if (_config.Attack.Contains(keyword))
        {
            string? target = ParseItemName(tokens, 1);
            return GuardPronoun(new AttackCommand(target));
        }

        if (_config.Flee.Contains(keyword))
        {
            string? target = ParseItemName(tokens, 1);
            return GuardPronoun(new FleeCommand(target));
        }

        if (_config.Save.Contains(keyword))
        {
            string? target = ParseItemName(tokens, 1);
            return GuardPronoun(new SaveCommand(target));
        }

        if (_config.Load.Contains(keyword))
        {
            string? target = ParseItemName(tokens, 1);
            return GuardPronoun(new LoadCommand(target));
        }

        if (_config.Quest.Contains(keyword))
        {
            return new QuestCommand();
        }

        if (_config.Hint.Contains(keyword))
        {
            string? target = ParseItemName(tokens, 1);
            return GuardPronoun(new HintCommand(target));
        }

        if (_config.Go.Contains(keyword))
        {
            return tokens.Length >= 2 && TryParseDirection(tokens[1], out Direction direction)
                ? new GoCommand(direction)
                : ParseGoTarget(tokens);
        }

        if (_config.EnableFuzzyMatching)
        {
            ICommand? fuzzy = TryParseFuzzyKeyword(keyword, tokens);
            if (fuzzy != null)
            {
                return fuzzy;
            }
        }

        ICommand finalCommand = TryParseDirection(keyword, out Direction directDirection)
            ? new GoCommand(directDirection)
            : new UnknownCommand();

        return GuardPronoun(finalCommand);
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
            string? best = FuzzyMatcher.FindBestToken(token, _config.DirectionAliases.Keys, _config.FuzzyMaxDistance);
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
        string? best = FuzzyMatcher.FindBestToken(keyword, candidates, _config.FuzzyMaxDistance);
        if (string.IsNullOrWhiteSpace(best))
        {
            return null;
        }

        ICommand? command = BuildCommandForKeyword(best, tokens);
        return command == null
            ? null
            : new FuzzyCommand(command, best.ToProperCase());
    }

    private ICommand? BuildCommandForKeyword(string keyword, string[] tokens)
    {
        keyword = NormalizeKeyword(keyword);
        if (_config.Quit.Contains(keyword))
        {
            return new QuitCommand();
        }

        if (_config.Examine.Contains(keyword))
        {
            return GuardPronoun(new ExamineCommand(ParseItemName(tokens, 1)));
        }

        if (_config.Look.Contains(keyword))
        {
            return GuardPronoun(new LookCommand(ParseItemName(tokens, 1)));
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

        if (_config.Close.Contains(keyword))
        {
            return new CloseCommand();
        }

        if (_config.LockDoor.Contains(keyword))
        {
            return new LockCommand();
        }

        if (_config.Destroy.Contains(keyword))
        {
            return GuardPronoun(new DestroyCommand(ParseItemName(tokens, 1)));
        }

        if (_config.Take.Contains(keyword))
        {
            if (tokens.Length >= 2 && _config.All.Contains(tokens[1]))
            {
                return new TakeAllCommand();
            }

            string? itemName = ParseItemName(tokens, 1);
            ICommand command = itemName != null ? new TakeCommand(itemName) : new UnknownCommand();
            return GuardPronoun(command);
        }

        if (_config.Drop.Contains(keyword))
        {
            if (tokens.Length >= 2 && _config.All.Contains(tokens[1]))
            {
                return new DropAllCommand();
            }

            string? itemName = ParseItemName(tokens, 1);
            ICommand command = itemName != null ? new DropCommand(itemName) : new UnknownCommand();
            return GuardPronoun(command);
        }

        if (_config.Use.Contains(keyword))
        {
            string? itemName = ParseItemName(tokens, 1);
            ICommand command = itemName != null ? new UseCommand(itemName) : new UnknownCommand();
            return GuardPronoun(command);
        }

        if (_config.Combine.Contains(keyword))
        {
            return ParseCombine(tokens);
        }

        if (_config.Pour.Contains(keyword))
        {
            return ParsePour(tokens);
        }

        if (_config.Read.Contains(keyword))
        {
            var target = ParseItemName(tokens, 1);
            ICommand command = target != null ? new ReadCommand(target) : new UnknownCommand();
            return GuardPronoun(command);
        }

        if (_config.Move.Contains(keyword))
        {
            if (tokens.Length >= 2 && TryParseDirection(tokens[1], out Direction moveDirection))
            {
                return new GoCommand(moveDirection);
            }

            string? target = ParseItemName(tokens, 1);
            ICommand command = target != null ? new MoveCommand(target) : new MoveCommand(string.Empty);
            return GuardPronoun(command);
        }

        if (_config.Talk.Contains(keyword))
        {
            return GuardPronoun(new TalkCommand(ParseItemName(tokens, 1)));
        }

        if (_config.Attack.Contains(keyword))
        {
            return GuardPronoun(new AttackCommand(ParseItemName(tokens, 1)));
        }

        if (_config.Flee.Contains(keyword))
        {
            return GuardPronoun(new FleeCommand(ParseItemName(tokens, 1)));
        }

        if (_config.Save.Contains(keyword))
        {
            return GuardPronoun(new SaveCommand(ParseItemName(tokens, 1)));
        }

        if (_config.Load.Contains(keyword))
        {
            return GuardPronoun(new LoadCommand(ParseItemName(tokens, 1)));
        }

        if (_config.Quest.Contains(keyword))
        {
            return new QuestCommand();
        }

        if (_config.Hint.Contains(keyword))
        {
            return GuardPronoun(new HintCommand(ParseItemName(tokens, 1)));
        }

        if (_config.Go.Contains(keyword))
        {
            return tokens.Length >= 2 && TryParseDirection(tokens[1], out Direction direction)
                ? new GoCommand(direction)
                : ParseGoTarget(tokens);
        }

        return null;
    }

    private string NormalizeKeyword(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return string.Empty;
        }

        string keyword = token.ToLowerInvariant();
        return _config.Synonyms.TryGetValue(keyword, out string? canonical) ? canonical : keyword;
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
            .Concat(_config.Close)
            .Concat(_config.LockDoor)
            .Concat(_config.Destroy)
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
            .Concat(_config.Quest)
            .Concat(_config.Hint)
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
            .ToList();

        if (parts.Count == 0)
        {
            return null;
        }

        if (parts.Count == 1 && _config.Pronouns.Contains(parts[0]))
        {
            if (string.IsNullOrWhiteSpace(_lastTarget))
            {
                _pronounMissing = true;
                return null;
            }

            return _lastTarget;
        }

        string target = parts.SpaceJoin();
        if (!string.IsNullOrWhiteSpace(target))
        {
            _lastTarget = target;
        }

        return target;
    }

    private string ApplyPhraseAliases(string input)
    {
        foreach ((string phrase, string canonical) in _config.PhraseAliases)
        {
            if (input.StartsWith(phrase, StringComparison.OrdinalIgnoreCase))
            {
                string remainder = input[phrase.Length..].TrimStart();
                return string.IsNullOrWhiteSpace(remainder) ? canonical : $"{canonical} {remainder}";
            }
        }

        return input;
    }

    private ICommand? TryParseCustomCommand(string input)
    {
        string? match = _config.CustomCommands
            .Keys
            .OrderByDescending(key => key.Length)
            .FirstOrDefault(key => input.StartsWith(key, StringComparison.OrdinalIgnoreCase));

        if (match == null)
        {
            return null;
        }

        if (_config.CustomCommands.TryGetValue(match, out Func<string[], ICommand>? handler))
        {
            string[] tokens = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return handler(tokens);
        }

        return null;
    }

    private ICommand GuardPronoun(ICommand command)
    {
        return _pronounMissing ? new ParserErrorCommand("I do not know what that refers to.") : command;
    }

    private ICommand ParseGoTarget(string[] tokens)
    {
        string? target = ParseItemName(tokens, 1);
        ICommand command = target != null ? new GoToCommand(target) : new UnknownCommand();
        return GuardPronoun(command);
    }

    private ICommand ParseCombine(string[] tokens)
    {
        if (tokens.Length < 3)
        {
            return new UnknownCommand();
        }

        var parts = tokens.Skip(1)
            .Where(t => !_config.CombineSeparators.Contains(t))
            .ToList();

        if (parts.Count < 2)
            return new UnknownCommand();

        string right = parts[^1];
        string left = parts.Take(parts.Count - 1).SpaceJoin();
        return new CombineCommand(left, right);
    }

    private ICommand ParsePour(string[] tokens)
    {
        int index = Array.FindIndex(tokens, _config.PourPrepositions.Contains);
        if (index <= 1 || index >= tokens.Length - 1)
        {
            return new UnknownCommand();
        }

        string fluid = tokens.Skip(1).Take(index - 1).SpaceJoin();
        string container = tokens.Skip(index + 1).SpaceJoin();
        return new PourCommand(fluid, container);
    }
}
