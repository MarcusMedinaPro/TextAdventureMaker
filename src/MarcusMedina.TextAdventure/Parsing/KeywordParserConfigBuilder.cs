// <copyright file="KeywordParserConfigBuilder.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;

namespace MarcusMedina.TextAdventure.Parsing;

public sealed class KeywordParserConfigBuilder
{
    private ISet<string> _quit;
    private ISet<string> _look;
    private ISet<string> _inventory;
    private ISet<string> _stats;
    private ISet<string> _open;
    private ISet<string> _unlock;
    private ISet<string> _take;
    private ISet<string> _drop;
    private ISet<string> _use;
    private ISet<string> _combine;
    private ISet<string> _pour;
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

    private KeywordParserConfigBuilder()
    {
        _quit = CommandHelper.NewCommands("quit", "exit", "q");
        _look = CommandHelper.NewCommands("look", "l", "examine");
        _inventory = CommandHelper.NewCommands("inventory", "inv", "i");
        _stats = CommandHelper.NewCommands("stats", "stat", "hp", "health");
        _open = CommandHelper.NewCommands("open");
        _unlock = CommandHelper.NewCommands("unlock");
        _take = CommandHelper.NewCommands("take", "get", "pickup", "pick");
        _drop = CommandHelper.NewCommands("drop");
        _use = CommandHelper.NewCommands("use", "turn", "switch", "light", "torch");
        _combine = CommandHelper.NewCommands("combine", "mix");
        _pour = CommandHelper.NewCommands("pour");
        _go = CommandHelper.NewCommands("go", "move");
        _read = CommandHelper.NewCommands("read");
        _talk = CommandHelper.NewCommands("talk", "speak");
        _attack = CommandHelper.NewCommands("attack", "fight");
        _flee = CommandHelper.NewCommands("flee", "run");
        _save = CommandHelper.NewCommands("save");
        _load = CommandHelper.NewCommands("load");
        _all = CommandHelper.NewCommands("all");
        _ignoreItemTokens = CommandHelper.NewCommands("up", "to", "on", "off");
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
    }

    public static KeywordParserConfigBuilder BritishDefaults() => new();

    public KeywordParserConfigBuilder WithQuit(params string[] commands)
    {
        _quit = CommandHelper.NewCommands(commands);
        return this;
    }

    public KeywordParserConfigBuilder WithLook(params string[] commands)
    {
        _look = CommandHelper.NewCommands(commands);
        return this;
    }

    public KeywordParserConfigBuilder WithInventory(params string[] commands)
    {
        _inventory = CommandHelper.NewCommands(commands);
        return this;
    }

    public KeywordParserConfigBuilder WithStats(params string[] commands)
    {
        _stats = CommandHelper.NewCommands(commands);
        return this;
    }

    public KeywordParserConfigBuilder WithOpen(params string[] commands)
    {
        _open = CommandHelper.NewCommands(commands);
        return this;
    }

    public KeywordParserConfigBuilder WithUnlock(params string[] commands)
    {
        _unlock = CommandHelper.NewCommands(commands);
        return this;
    }

    public KeywordParserConfigBuilder WithTake(params string[] commands)
    {
        _take = CommandHelper.NewCommands(commands);
        return this;
    }

    public KeywordParserConfigBuilder WithDrop(params string[] commands)
    {
        _drop = CommandHelper.NewCommands(commands);
        return this;
    }

    public KeywordParserConfigBuilder WithUse(params string[] commands)
    {
        _use = CommandHelper.NewCommands(commands);
        return this;
    }

    public KeywordParserConfigBuilder WithCombine(params string[] commands)
    {
        _combine = CommandHelper.NewCommands(commands);
        return this;
    }

    public KeywordParserConfigBuilder WithPour(params string[] commands)
    {
        _pour = CommandHelper.NewCommands(commands);
        return this;
    }

    public KeywordParserConfigBuilder WithGo(params string[] commands)
    {
        _go = CommandHelper.NewCommands(commands);
        return this;
    }

    public KeywordParserConfigBuilder WithRead(params string[] commands)
    {
        _read = CommandHelper.NewCommands(commands);
        return this;
    }

    public KeywordParserConfigBuilder WithTalk(params string[] commands)
    {
        _talk = CommandHelper.NewCommands(commands);
        return this;
    }

    public KeywordParserConfigBuilder WithAttack(params string[] commands)
    {
        _attack = CommandHelper.NewCommands(commands);
        return this;
    }

    public KeywordParserConfigBuilder WithFlee(params string[] commands)
    {
        _flee = CommandHelper.NewCommands(commands);
        return this;
    }

    public KeywordParserConfigBuilder WithSave(params string[] commands)
    {
        _save = CommandHelper.NewCommands(commands);
        return this;
    }

    public KeywordParserConfigBuilder WithLoad(params string[] commands)
    {
        _load = CommandHelper.NewCommands(commands);
        return this;
    }

    public KeywordParserConfigBuilder WithAll(params string[] commands)
    {
        _all = CommandHelper.NewCommands(commands);
        return this;
    }

    public KeywordParserConfigBuilder WithIgnoreItemTokens(params string[] tokens)
    {
        _ignoreItemTokens = CommandHelper.NewCommands(tokens);
        return this;
    }

    public KeywordParserConfigBuilder WithCombineSeparators(params string[] separators)
    {
        _combineSeparators = CommandHelper.NewCommands(separators);
        return this;
    }

    public KeywordParserConfigBuilder WithPourPrepositions(params string[] prepositions)
    {
        _pourPrepositions = CommandHelper.NewCommands(prepositions);
        return this;
    }

    public KeywordParserConfigBuilder WithDirectionAliases(IReadOnlyDictionary<string, Direction> aliases)
    {
        _directionAliases = aliases;
        return this;
    }

    public KeywordParserConfigBuilder AllowDirectionEnumNames(bool allow = true)
    {
        _allowDirectionEnumNames = allow;
        return this;
    }

    public KeywordParserConfig Build()
    {
        return new KeywordParserConfig(
            quit: _quit,
            look: _look,
            inventory: _inventory,
            stats: _stats,
            open: _open,
            unlock: _unlock,
            take: _take,
            drop: _drop,
            use: _use,
            combine: _combine,
            pour: _pour,
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
            allowDirectionEnumNames: _allowDirectionEnumNames);
    }
}
