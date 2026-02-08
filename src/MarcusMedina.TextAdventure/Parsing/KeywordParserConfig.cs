// <copyright file="KeywordParserConfig.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Parsing;
/// <summary>Configuration for the keyword-based command parser.</summary>
public sealed class KeywordParserConfig
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
        close: CommandHelper.NewCommands("close"),
        lockDoor: CommandHelper.NewCommands("lock"),
        destroy: CommandHelper.NewCommands("destroy", "smash", "break"),
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
        hint: CommandHelper.NewCommands("hint", "path"),
        again: CommandHelper.NewCommands("again", "repeat"),
        pronouns: CommandHelper.NewCommands("it", "them"),
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
        phraseAliases: new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
        customCommands: new Dictionary<string, Func<string[], ICommand>>(StringComparer.OrdinalIgnoreCase),
        allowDirectionEnumNames: true);
    /// <summary>Keywords used to quit the game.</summary>
    public ISet<string> Quit { get; }
    /// <summary>Keywords used to look around or at a target.</summary>
    public ISet<string> Look { get; }
    /// <summary>Keywords used to examine a target.</summary>
    public ISet<string> Examine { get; }
    /// <summary>Keywords used to show inventory.</summary>
    public ISet<string> Inventory { get; }
    /// <summary>Keywords used to show stats.</summary>
    public ISet<string> Stats { get; }
    /// <summary>Keywords used to open a door.</summary>
    public ISet<string> Open { get; }
    /// <summary>Keywords used to unlock a door.</summary>
    public ISet<string> Unlock { get; }
    /// <summary>Keywords used to close a door.</summary>
    public ISet<string> Close { get; }
    /// <summary>Keywords used to lock a door.</summary>
    public ISet<string> LockDoor { get; }
    /// <summary>Keywords used to destroy something.</summary>
    public ISet<string> Destroy { get; }
    /// <summary>Keywords used to take items.</summary>
    public ISet<string> Take { get; }
    /// <summary>Keywords used to drop items.</summary>
    public ISet<string> Drop { get; }
    /// <summary>Keywords used to use items.</summary>
    public ISet<string> Use { get; }
    /// <summary>Keywords used to combine items.</summary>
    public ISet<string> Combine { get; }
    /// <summary>Keywords used to pour items.</summary>
    public ISet<string> Pour { get; }
    /// <summary>Keywords used to move objects.</summary>
    public ISet<string> Move { get; }
    /// <summary>Keywords used to move.</summary>
    public ISet<string> Go { get; }
    /// <summary>Keywords used to read items.</summary>
    public ISet<string> Read { get; }
    /// <summary>Keywords used to talk to NPCs.</summary>
    public ISet<string> Talk { get; }
    /// <summary>Keywords used to attack NPCs.</summary>
    public ISet<string> Attack { get; }
    /// <summary>Keywords used to flee combat.</summary>
    public ISet<string> Flee { get; }
    /// <summary>Keywords used to save the game.</summary>
    public ISet<string> Save { get; }
    /// <summary>Keywords used to load the game.</summary>
    public ISet<string> Load { get; }
    /// <summary>Keywords used to show quest log.</summary>
    public ISet<string> Quest { get; }
    /// <summary>Keywords used to request a path hint.</summary>
    public ISet<string> Hint { get; }
    /// <summary>Keywords used to repeat the previous command.</summary>
    public ISet<string> Again { get; }
    /// <summary>Pronouns that can refer to the last target.</summary>
    public ISet<string> Pronouns { get; }
    /// <summary>Synonym map for command keywords.</summary>
    public IReadOnlyDictionary<string, string> Synonyms { get; }
    /// <summary>Multi-word phrase aliases mapped to canonical phrases.</summary>
    public IReadOnlyDictionary<string, string> PhraseAliases { get; }
    /// <summary>Custom command handlers keyed by full phrase.</summary>
    public IReadOnlyDictionary<string, Func<string[], ICommand>> CustomCommands { get; }

    /// <summary>Keywords that indicate "all" in take/drop commands.</summary>
    public ISet<string> All { get; }
    /// <summary>Tokens to ignore when parsing item names.</summary>
    public ISet<string> IgnoreItemTokens { get; }
    /// <summary>Tokens that separate items in combine commands.</summary>
    public ISet<string> CombineSeparators { get; }
    /// <summary>Prepositions used for pour commands.</summary>
    public ISet<string> PourPrepositions { get; }

    /// <summary>Direction aliases used for movement.</summary>
    public IReadOnlyDictionary<string, Direction> DirectionAliases { get; }
    /// <summary>Allow enum names like "North" as directions.</summary>
    public bool AllowDirectionEnumNames { get; }
    /// <summary>Enable fuzzy matching for command keywords and directions.</summary>
    public bool EnableFuzzyMatching { get; }
    /// <summary>Maximum edit distance for fuzzy matching.</summary>
    public int FuzzyMaxDistance { get; }

    public KeywordParserConfig(
        ISet<string> quit,
        ISet<string> look,
        ISet<string> examine,
        ISet<string> inventory,
        ISet<string> stats,
        ISet<string> open,
        ISet<string> unlock,
        ISet<string> close,
        ISet<string> lockDoor,
        ISet<string> destroy,
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
        ISet<string> hint,
        ISet<string> again,
        ISet<string> pronouns,
        ISet<string> all,
        ISet<string> ignoreItemTokens,
        ISet<string> combineSeparators,
        ISet<string> pourPrepositions,
        IReadOnlyDictionary<string, Direction> directionAliases,
        IReadOnlyDictionary<string, string>? synonyms = null,
        IReadOnlyDictionary<string, string>? phraseAliases = null,
        IReadOnlyDictionary<string, Func<string[], ICommand>>? customCommands = null,
        bool allowDirectionEnumNames = false,
        bool enableFuzzyMatching = false,
        int fuzzyMaxDistance = 1)
    {
        Quit = quit ?? throw new ArgumentNullException(nameof(quit));
        Look = look ?? throw new ArgumentNullException(nameof(look));
        Examine = examine ?? throw new ArgumentNullException(nameof(examine));
        Inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
        Stats = stats ?? throw new ArgumentNullException(nameof(stats));
        Open = open ?? throw new ArgumentNullException(nameof(open));
        Unlock = unlock ?? throw new ArgumentNullException(nameof(unlock));
        Close = close ?? throw new ArgumentNullException(nameof(close));
        LockDoor = lockDoor ?? throw new ArgumentNullException(nameof(lockDoor));
        Destroy = destroy ?? throw new ArgumentNullException(nameof(destroy));
        Take = take ?? throw new ArgumentNullException(nameof(take));
        Drop = drop ?? throw new ArgumentNullException(nameof(drop));
        Use = use ?? throw new ArgumentNullException(nameof(use));
        Combine = combine ?? throw new ArgumentNullException(nameof(combine));
        Pour = pour ?? throw new ArgumentNullException(nameof(pour));
        Move = move ?? throw new ArgumentNullException(nameof(move));
        Go = go ?? throw new ArgumentNullException(nameof(go));
        Read = read ?? throw new ArgumentNullException(nameof(read));
        Talk = talk ?? throw new ArgumentNullException(nameof(talk));
        Attack = attack ?? throw new ArgumentNullException(nameof(attack));
        Flee = flee ?? throw new ArgumentNullException(nameof(flee));
        Save = save ?? throw new ArgumentNullException(nameof(save));
        Load = load ?? throw new ArgumentNullException(nameof(load));
        Quest = quest ?? throw new ArgumentNullException(nameof(quest));
        Hint = hint ?? throw new ArgumentNullException(nameof(hint));
        Again = again ?? throw new ArgumentNullException(nameof(again));
        Pronouns = pronouns ?? throw new ArgumentNullException(nameof(pronouns));
        Synonyms = synonyms ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        PhraseAliases = phraseAliases ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        CustomCommands = customCommands ?? new Dictionary<string, Func<string[], ICommand>>(StringComparer.OrdinalIgnoreCase);
        All = all ?? throw new ArgumentNullException(nameof(all));
        IgnoreItemTokens = ignoreItemTokens ?? throw new ArgumentNullException(nameof(ignoreItemTokens));
        CombineSeparators = combineSeparators ?? throw new ArgumentNullException(nameof(combineSeparators));
        PourPrepositions = pourPrepositions ?? throw new ArgumentNullException(nameof(pourPrepositions));
        DirectionAliases = directionAliases ?? throw new ArgumentNullException(nameof(directionAliases));
        AllowDirectionEnumNames = allowDirectionEnumNames;
        EnableFuzzyMatching = enableFuzzyMatching;
        FuzzyMaxDistance = Math.Max(0, fuzzyMaxDistance);
    }

    public KeywordParserConfig(
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
        ISet<string> hint,
        ISet<string> all,
        ISet<string> ignoreItemTokens,
        ISet<string> combineSeparators,
        ISet<string> pourPrepositions,
        IReadOnlyDictionary<string, Direction> directionAliases,
        IReadOnlyDictionary<string, string>? synonyms = null,
        bool allowDirectionEnumNames = false,
        bool enableFuzzyMatching = false,
        int fuzzyMaxDistance = 1)
        : this(
            quit,
            look,
            examine,
            inventory,
            stats,
            open,
            unlock,
            CommandHelper.NewCommands("close"),
            CommandHelper.NewCommands("lock"),
            CommandHelper.NewCommands("destroy", "smash", "break"),
            take,
            drop,
            use,
            combine,
            pour,
            move,
            go,
            read,
            talk,
            attack,
            flee,
            save,
            load,
            quest,
            hint,
            all,
            ignoreItemTokens,
            combineSeparators,
            pourPrepositions,
            directionAliases,
            synonyms,
            allowDirectionEnumNames,
            enableFuzzyMatching,
            fuzzyMaxDistance)
    {
    }
}
