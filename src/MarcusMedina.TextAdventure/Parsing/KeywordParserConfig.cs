// <copyright file="KeywordParserConfig.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Parsing;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;

/// <summary>Configuration for the keyword-based command parser.</summary>
public sealed class KeywordParserConfig(
        ISet<string> quit,
        ISet<string> look,
        ISet<string> examine,
        ISet<string> inventory,
        ISet<string> stats,
        ISet<string> open,
        ISet<string> unlock,
        ISet<string> take,
        ISet<string> drop,
        ISet<string> use,
        ISet<string> combine,
        ISet<string> pour,
        ISet<string> move,
        ISet<string> go,
        ISet<string> read,
        ISet<string> talk,
        ISet<string> attack,
        ISet<string> flee,
        ISet<string> save,
        ISet<string> load,
        ISet<string> quest,
        ISet<string> all,
        ISet<string> ignoreItemTokens,
        ISet<string> combineSeparators,
        ISet<string> pourPrepositions,
        IReadOnlyDictionary<string, Direction> directionAliases,
        IReadOnlyDictionary<string, string>? synonyms = null,
        bool allowDirectionEnumNames = false,
        bool enableFuzzyMatching = false,
        int fuzzyMaxDistance = 1)
{

    /// <summary>Default configuration with common command keywords and direction aliases.</summary>
    public static KeywordParserConfig Default { get; } = new(
        quit: CommandHelper.NewCommands("quit", "exit", "q"),
        look: CommandHelper.NewCommands("look", "l"),
        examine: CommandHelper.NewCommands("examine"),
        inventory: CommandHelper.NewCommands("inventory", "inv", "i"),
        stats: CommandHelper.NewCommands("stats", "stat", "hp", "health"),
        open: CommandHelper.NewCommands("open"),
        unlock: CommandHelper.NewCommands("unlock"),
        take: CommandHelper.NewCommands("take", "get", "pickup", "pick"),
        drop: CommandHelper.NewCommands("drop"),
        use: CommandHelper.NewCommands("use"),
        combine: CommandHelper.NewCommands("combine", "mix"),
        pour: CommandHelper.NewCommands("pour"),
        move: CommandHelper.NewCommands("move", "push", "shift", "lift", "slide"),
        go: CommandHelper.NewCommands("go", "move"),
        read: CommandHelper.NewCommands("read"),
        talk: CommandHelper.NewCommands("talk", "speak"),
        attack: CommandHelper.NewCommands("attack", "fight"),
        flee: CommandHelper.NewCommands("flee", "run"),
        save: CommandHelper.NewCommands("save"),
        load: CommandHelper.NewCommands("load"),
        quest: CommandHelper.NewCommands("quests", "quest", "journal"),
        all: CommandHelper.NewCommands("all"),
        ignoreItemTokens: CommandHelper.NewCommands("up", "to", "at", "the", "a"),
        combineSeparators: CommandHelper.NewCommands("and", "+"),
        pourPrepositions: CommandHelper.NewCommands("into", "in"),
        directionAliases: new Dictionary<string, Direction>(StringComparer.OrdinalIgnoreCase)
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
        },
        synonyms: new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
        allowDirectionEnumNames: true);

    /// <summary>Keywords that indicate "all" in take/drop commands.</summary>
    public ISet<string> All { get; } = all ?? throw new ArgumentNullException(nameof(all));

    /// <summary>Allow enum names like "North" as directions.</summary>
    public bool AllowDirectionEnumNames { get; } = allowDirectionEnumNames;

    /// <summary>Keywords used to attack NPCs.</summary>
    public ISet<string> Attack { get; } = attack ?? throw new ArgumentNullException(nameof(attack));

    /// <summary>Keywords used to combine items.</summary>
    public ISet<string> Combine { get; } = combine ?? throw new ArgumentNullException(nameof(combine));

    /// <summary>Tokens that separate items in combine commands.</summary>
    public ISet<string> CombineSeparators { get; } = combineSeparators ?? throw new ArgumentNullException(nameof(combineSeparators));

    /// <summary>Direction aliases used for movement.</summary>
    public IReadOnlyDictionary<string, Direction> DirectionAliases { get; } = directionAliases ?? throw new ArgumentNullException(nameof(directionAliases));

    /// <summary>Keywords used to drop items.</summary>
    public ISet<string> Drop { get; } = drop ?? throw new ArgumentNullException(nameof(drop));

    /// <summary>Enable fuzzy matching for command keywords and directions.</summary>
    public bool EnableFuzzyMatching { get; } = enableFuzzyMatching;

    /// <summary>Keywords used to examine a target.</summary>
    public ISet<string> Examine { get; } = examine ?? throw new ArgumentNullException(nameof(examine));

    /// <summary>Keywords used to flee combat.</summary>
    public ISet<string> Flee { get; } = flee ?? throw new ArgumentNullException(nameof(flee));

    /// <summary>Maximum edit distance for fuzzy matching.</summary>
    public int FuzzyMaxDistance { get; } = Math.Max(0, fuzzyMaxDistance);

    /// <summary>Keywords used to move.</summary>
    public ISet<string> Go { get; } = go ?? throw new ArgumentNullException(nameof(go));

    /// <summary>Tokens to ignore when parsing item names.</summary>
    public ISet<string> IgnoreItemTokens { get; } = ignoreItemTokens ?? throw new ArgumentNullException(nameof(ignoreItemTokens));

    /// <summary>Keywords used to show inventory.</summary>
    public ISet<string> Inventory { get; } = inventory ?? throw new ArgumentNullException(nameof(inventory));

    /// <summary>Keywords used to load the game.</summary>
    public ISet<string> Load { get; } = load ?? throw new ArgumentNullException(nameof(load));

    /// <summary>Keywords used to look around or at a target.</summary>
    public ISet<string> Look { get; } = look ?? throw new ArgumentNullException(nameof(look));

    /// <summary>Keywords used to move objects.</summary>
    public ISet<string> Move { get; } = move ?? throw new ArgumentNullException(nameof(move));

    /// <summary>Keywords used to open a door.</summary>
    public ISet<string> Open { get; } = open ?? throw new ArgumentNullException(nameof(open));

    /// <summary>Keywords used to pour items.</summary>
    public ISet<string> Pour { get; } = pour ?? throw new ArgumentNullException(nameof(pour));

    /// <summary>Prepositions used for pour commands.</summary>
    public ISet<string> PourPrepositions { get; } = pourPrepositions ?? throw new ArgumentNullException(nameof(pourPrepositions));

    /// <summary>Keywords used to show quest log.</summary>
    public ISet<string> Quest { get; } = quest ?? throw new ArgumentNullException(nameof(quest));

    /// <summary>Keywords used to quit the game.</summary>
    public ISet<string> Quit { get; } = quit ?? throw new ArgumentNullException(nameof(quit));

    /// <summary>Keywords used to read items.</summary>
    public ISet<string> Read { get; } = read ?? throw new ArgumentNullException(nameof(read));

    /// <summary>Keywords used to save the game.</summary>
    public ISet<string> Save { get; } = save ?? throw new ArgumentNullException(nameof(save));

    /// <summary>Keywords used to show stats.</summary>
    public ISet<string> Stats { get; } = stats ?? throw new ArgumentNullException(nameof(stats));

    /// <summary>Synonym map for command keywords.</summary>
    public IReadOnlyDictionary<string, string> Synonyms { get; } = synonyms ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    /// <summary>Keywords used to take items.</summary>
    public ISet<string> Take { get; } = take ?? throw new ArgumentNullException(nameof(take));

    /// <summary>Keywords used to talk to NPCs.</summary>
    public ISet<string> Talk { get; } = talk ?? throw new ArgumentNullException(nameof(talk));

    /// <summary>Keywords used to unlock a door.</summary>
    public ISet<string> Unlock { get; } = unlock ?? throw new ArgumentNullException(nameof(unlock));

    /// <summary>Keywords used to use items.</summary>
    public ISet<string> Use { get; } = use ?? throw new ArgumentNullException(nameof(use));
}
