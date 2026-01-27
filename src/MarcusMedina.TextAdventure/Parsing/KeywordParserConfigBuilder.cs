// <copyright file="KeywordParserConfigBuilder.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;

namespace MarcusMedina.TextAdventure.Parsing;

/// <summary>Fluent builder for <see cref="KeywordParserConfig"/>.</summary>
public sealed class KeywordParserConfigBuilder
{
    private ISet<string> _quit;
    private ISet<string> _look;
    private ISet<string> _examine;
    private ISet<string> _inventory;
    private ISet<string> _stats;
    private ISet<string> _open;
    private ISet<string> _unlock;
    private ISet<string> _take;
    private ISet<string> _drop;
    private ISet<string> _use;
    private ISet<string> _combine;
    private ISet<string> _pour;
    private ISet<string> _move;
    private ISet<string> _go;
    private ISet<string> _read;
    private ISet<string> _talk;
    private ISet<string> _attack;
    private ISet<string> _flee;
    private ISet<string> _save;
    private ISet<string> _load;
    private ISet<string> _all;
    private ISet<string> _ignoreItemTokens;
    private ISet<string> _combineSeparators;
    private ISet<string> _pourPrepositions;
    private IReadOnlyDictionary<string, Direction> _directionAliases;
    private bool _allowDirectionEnumNames;
    private bool _enableFuzzyMatching;
    private int _fuzzyMaxDistance;

    private KeywordParserConfigBuilder()
    {
        _quit = CommandHelper.NewCommands("quit", "exit", "q");
        _look = CommandHelper.NewCommands("look", "l");
        _examine = CommandHelper.NewCommands("examine");
        _inventory = CommandHelper.NewCommands("inventory", "inv", "i");
        _stats = CommandHelper.NewCommands("stats", "stat", "hp", "health");
        _open = CommandHelper.NewCommands("open");
        _unlock = CommandHelper.NewCommands("unlock");
        _take = CommandHelper.NewCommands("take", "get", "pickup", "pick");
        _drop = CommandHelper.NewCommands("drop");
        _use = CommandHelper.NewCommands("use", "turn", "switch", "light", "torch");
        _combine = CommandHelper.NewCommands("combine", "mix");
        _pour = CommandHelper.NewCommands("pour");
        _move = CommandHelper.NewCommands("move", "push", "shift", "lift", "slide");
        _go = CommandHelper.NewCommands("go", "move");
        _read = CommandHelper.NewCommands("read");
        _talk = CommandHelper.NewCommands("talk", "speak");
        _attack = CommandHelper.NewCommands("attack", "fight");
        _flee = CommandHelper.NewCommands("flee", "run");
        _save = CommandHelper.NewCommands("save");
        _load = CommandHelper.NewCommands("load");
        _all = CommandHelper.NewCommands("all");
        _ignoreItemTokens = CommandHelper.NewCommands("up", "to", "on", "off", "at", "the");
        _combineSeparators = CommandHelper.NewCommands("and", "+");
        _pourPrepositions = CommandHelper.NewCommands("into", "in");
        _directionAliases = new Dictionary<string, Direction>(StringComparer.OrdinalIgnoreCase)
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
        _allowDirectionEnumNames = true;
        _enableFuzzyMatching = false;
        _fuzzyMaxDistance = 1;
    }

    /// <summary>Create a builder with British English defaults.</summary>
    public static KeywordParserConfigBuilder BritishDefaults() => new();

    /// <summary>Set keywords for quit/exit.</summary>
    public KeywordParserConfigBuilder WithQuit(params string[] commands)
    {
        _quit = CommandHelper.NewCommands(commands);
        return this;
    }

    /// <summary>Set keywords for look.</summary>
    public KeywordParserConfigBuilder WithLook(params string[] commands)
    {
        _look = CommandHelper.NewCommands(commands);
        return this;
    }

    /// <summary>Set keywords for examine.</summary>
    public KeywordParserConfigBuilder WithExamine(params string[] commands)
    {
        _examine = CommandHelper.NewCommands(commands);
        return this;
    }

    /// <summary>Set keywords for inventory.</summary>
    public KeywordParserConfigBuilder WithInventory(params string[] commands)
    {
        _inventory = CommandHelper.NewCommands(commands);
        return this;
    }

    /// <summary>Set keywords for stats/health.</summary>
    public KeywordParserConfigBuilder WithStats(params string[] commands)
    {
        _stats = CommandHelper.NewCommands(commands);
        return this;
    }

    /// <summary>Set keywords for open.</summary>
    public KeywordParserConfigBuilder WithOpen(params string[] commands)
    {
        _open = CommandHelper.NewCommands(commands);
        return this;
    }

    /// <summary>Set keywords for unlock.</summary>
    public KeywordParserConfigBuilder WithUnlock(params string[] commands)
    {
        _unlock = CommandHelper.NewCommands(commands);
        return this;
    }

    /// <summary>Set keywords for take/pick up.</summary>
    public KeywordParserConfigBuilder WithTake(params string[] commands)
    {
        _take = CommandHelper.NewCommands(commands);
        return this;
    }

    /// <summary>Set keywords for drop.</summary>
    public KeywordParserConfigBuilder WithDrop(params string[] commands)
    {
        _drop = CommandHelper.NewCommands(commands);
        return this;
    }

    /// <summary>Set keywords for use.</summary>
    public KeywordParserConfigBuilder WithUse(params string[] commands)
    {
        _use = CommandHelper.NewCommands(commands);
        return this;
    }

    /// <summary>Set keywords for combine.</summary>
    public KeywordParserConfigBuilder WithCombine(params string[] commands)
    {
        _combine = CommandHelper.NewCommands(commands);
        return this;
    }

    /// <summary>Set keywords for pour.</summary>
    public KeywordParserConfigBuilder WithPour(params string[] commands)
    {
        _pour = CommandHelper.NewCommands(commands);
        return this;
    }

    /// <summary>Set keywords for move.</summary>
    public KeywordParserConfigBuilder WithMove(params string[] commands)
    {
        _move = CommandHelper.NewCommands(commands);
        return this;
    }

    /// <summary>Set keywords for movement.</summary>
    public KeywordParserConfigBuilder WithGo(params string[] commands)
    {
        _go = CommandHelper.NewCommands(commands);
        return this;
    }

    /// <summary>Set keywords for read.</summary>
    public KeywordParserConfigBuilder WithRead(params string[] commands)
    {
        _read = CommandHelper.NewCommands(commands);
        return this;
    }

    /// <summary>Set keywords for talk/speak.</summary>
    public KeywordParserConfigBuilder WithTalk(params string[] commands)
    {
        _talk = CommandHelper.NewCommands(commands);
        return this;
    }

    /// <summary>Set keywords for attack/fight.</summary>
    public KeywordParserConfigBuilder WithAttack(params string[] commands)
    {
        _attack = CommandHelper.NewCommands(commands);
        return this;
    }

    /// <summary>Set keywords for flee/run.</summary>
    public KeywordParserConfigBuilder WithFlee(params string[] commands)
    {
        _flee = CommandHelper.NewCommands(commands);
        return this;
    }

    /// <summary>Set keywords for save.</summary>
    public KeywordParserConfigBuilder WithSave(params string[] commands)
    {
        _save = CommandHelper.NewCommands(commands);
        return this;
    }

    /// <summary>Set keywords for load.</summary>
    public KeywordParserConfigBuilder WithLoad(params string[] commands)
    {
        _load = CommandHelper.NewCommands(commands);
        return this;
    }

    /// <summary>Set keywords for "all".</summary>
    public KeywordParserConfigBuilder WithAll(params string[] commands)
    {
        _all = CommandHelper.NewCommands(commands);
        return this;
    }

    /// <summary>Set tokens to ignore when parsing item names.</summary>
    public KeywordParserConfigBuilder WithIgnoreItemTokens(params string[] tokens)
    {
        _ignoreItemTokens = CommandHelper.NewCommands(tokens);
        return this;
    }

    /// <summary>Set separators for combine commands.</summary>
    public KeywordParserConfigBuilder WithCombineSeparators(params string[] separators)
    {
        _combineSeparators = CommandHelper.NewCommands(separators);
        return this;
    }

    /// <summary>Set prepositions used for pour commands.</summary>
    public KeywordParserConfigBuilder WithPourPrepositions(params string[] prepositions)
    {
        _pourPrepositions = CommandHelper.NewCommands(prepositions);
        return this;
    }

    /// <summary>Set aliases for direction tokens.</summary>
    public KeywordParserConfigBuilder WithDirectionAliases(IReadOnlyDictionary<string, Direction> aliases)
    {
        _directionAliases = aliases;
        return this;
    }

    /// <summary>Allow enum direction names like "North".</summary>
    public KeywordParserConfigBuilder AllowDirectionEnumNames(bool allow = true)
    {
        _allowDirectionEnumNames = allow;
        return this;
    }

    /// <summary>Enable fuzzy matching for command keywords and directions.</summary>
    public KeywordParserConfigBuilder WithFuzzyMatching(bool enabled = true, int maxDistance = 1)
    {
        _enableFuzzyMatching = enabled;
        _fuzzyMaxDistance = Math.Max(0, maxDistance);
        return this;
    }

    /// <summary>Build the parser configuration.</summary>
    public KeywordParserConfig Build()
    {
        return new KeywordParserConfig(
            quit: _quit,
            look: _look,
            examine: _examine,
            inventory: _inventory,
            stats: _stats,
            open: _open,
            unlock: _unlock,
            take: _take,
            drop: _drop,
            use: _use,
            combine: _combine,
            pour: _pour,
            move: _move,
            go: _go,
            read: _read,
            talk: _talk,
            attack: _attack,
            flee: _flee,
            save: _save,
            load: _load,
            all: _all,
            ignoreItemTokens: _ignoreItemTokens,
            combineSeparators: _combineSeparators,
            pourPrepositions: _pourPrepositions,
            directionAliases: _directionAliases,
            allowDirectionEnumNames: _allowDirectionEnumNames,
            enableFuzzyMatching: _enableFuzzyMatching,
            fuzzyMaxDistance: _fuzzyMaxDistance);
    }
}
