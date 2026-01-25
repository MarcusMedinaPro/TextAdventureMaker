using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Parsing;

public sealed class KeywordParserConfig
{
    public ISet<string> Quit { get; }
    public ISet<string> Look { get; }
    public ISet<string> Inventory { get; }
    public ISet<string> Stats { get; }
    public ISet<string> Open { get; }
    public ISet<string> Unlock { get; }
    public ISet<string> Take { get; }
    public ISet<string> Drop { get; }
    public ISet<string> Use { get; }
    public ISet<string> Combine { get; }
    public ISet<string> Pour { get; }
    public ISet<string> Go { get; }
    public ISet<string> Read { get; }
    public ISet<string> Talk { get; }
    public ISet<string> Attack { get; }
    public ISet<string> Flee { get; }

    public ISet<string> All { get; }
    public ISet<string> IgnoreItemTokens { get; }
    public ISet<string> CombineSeparators { get; }
    public ISet<string> PourPrepositions { get; }

    public IReadOnlyDictionary<string, Direction> DirectionAliases { get; }
    public bool AllowDirectionEnumNames { get; }

    public KeywordParserConfig(
        ISet<string> quit,
        ISet<string> look,
        ISet<string> inventory,
        ISet<string> stats,
        ISet<string> open,
        ISet<string> unlock,
        ISet<string> take,
        ISet<string> drop,
        ISet<string> use,
        ISet<string> combine,
        ISet<string> pour,
        ISet<string> go,
        ISet<string> read,
        ISet<string> talk,
        ISet<string> attack,
        ISet<string> flee,
        ISet<string> all,
        ISet<string> ignoreItemTokens,
        ISet<string> combineSeparators,
        ISet<string> pourPrepositions,
        IReadOnlyDictionary<string, Direction> directionAliases,
        bool allowDirectionEnumNames = false)
    {
        Quit = quit ?? throw new ArgumentNullException(nameof(quit));
        Look = look ?? throw new ArgumentNullException(nameof(look));
        Inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
        Stats = stats ?? throw new ArgumentNullException(nameof(stats));
        Open = open ?? throw new ArgumentNullException(nameof(open));
        Unlock = unlock ?? throw new ArgumentNullException(nameof(unlock));
        Take = take ?? throw new ArgumentNullException(nameof(take));
        Drop = drop ?? throw new ArgumentNullException(nameof(drop));
        Use = use ?? throw new ArgumentNullException(nameof(use));
        Combine = combine ?? throw new ArgumentNullException(nameof(combine));
        Pour = pour ?? throw new ArgumentNullException(nameof(pour));
        Go = go ?? throw new ArgumentNullException(nameof(go));
        Read = read ?? throw new ArgumentNullException(nameof(read));
        Talk = talk ?? throw new ArgumentNullException(nameof(talk));
        Attack = attack ?? throw new ArgumentNullException(nameof(attack));
        Flee = flee ?? throw new ArgumentNullException(nameof(flee));
        All = all ?? throw new ArgumentNullException(nameof(all));
        IgnoreItemTokens = ignoreItemTokens ?? throw new ArgumentNullException(nameof(ignoreItemTokens));
        CombineSeparators = combineSeparators ?? throw new ArgumentNullException(nameof(combineSeparators));
        PourPrepositions = pourPrepositions ?? throw new ArgumentNullException(nameof(pourPrepositions));
        DirectionAliases = directionAliases ?? throw new ArgumentNullException(nameof(directionAliases));
        AllowDirectionEnumNames = allowDirectionEnumNames;
    }
}
