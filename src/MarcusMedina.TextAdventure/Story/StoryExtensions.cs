// <copyright file="StoryExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;
using System.Linq;

namespace MarcusMedina.TextAdventure.Story;

public static class StoryExtensions
{
    public static IEnumerable<T> ThatMatch<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        => source.Where(predicate);

    public static bool ThereAreAny<T>(this IEnumerable<T> source)
        => source.Any();

    public static bool ThereExists<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        => source.Any(predicate);

    public static bool AllAre<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        => source.All(predicate);

    public static bool Includes<T>(this IEnumerable<T> source, T item)
        => source.Contains(item);

    public static T TheFirst<T>(this IEnumerable<T> source)
        => source.First();

    public static T? TheFirstOrNone<T>(this IEnumerable<T> source)
        => source.FirstOrDefault();

    public static T TheLast<T>(this IEnumerable<T> source)
        => source.Last();

    public static T? TheLastOrNone<T>(this IEnumerable<T> source)
        => source.LastOrDefault();

    public static T TheOnly<T>(this IEnumerable<T> source)
        => source.Single();

    public static T? TheOnlyOrNone<T>(this IEnumerable<T> source)
        => source.SingleOrDefault();

    public static T TheNth<T>(this IEnumerable<T> source, int index)
        => source.ElementAt(index);

    public static T? TheNthOrNone<T>(this IEnumerable<T> source, int index)
        => source.ElementAtOrDefault(index);

    public static IEnumerable<string> TheirNames(this IEnumerable<IGameEntity> source)
        => source.Select(entity => entity.Name);

    public static IEnumerable<TResult> Their<T, TResult>(this IEnumerable<T> source, Func<T, TResult> selector)
        => source.Select(selector);

    public static IEnumerable<T> InReverseOrder<T>(this IEnumerable<T> source)
        => source.Reverse();

    public static int HowMany<T>(this IEnumerable<T> source)
        => source.Count();

    public static int HowManyAre<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        => source.Count(predicate);

    public static int TotalOf<T>(this IEnumerable<T> source, Func<T, int> selector)
        => source.Sum(selector);

    public static double AverageOf<T>(this IEnumerable<T> source, Func<T, int> selector)
        => source.Average(selector);

    public static IEnumerable<T> TheFirstFew<T>(this IEnumerable<T> source, int count)
        => source.Take(count);

    public static IEnumerable<T> ExceptTheFirst<T>(this IEnumerable<T> source, int count)
        => source.Skip(count);

    public static IEnumerable<T> WithoutDuplicates<T>(this IEnumerable<T> source)
        => source.Distinct();

    public static IEnumerable<T> CombinedWith<T>(this IEnumerable<T> source, IEnumerable<T> other)
        => source.Union(other);

    public static IEnumerable<T> InCommonWith<T>(this IEnumerable<T> source, IEnumerable<T> other)
        => source.Intersect(other);

    public static IEnumerable<T> ExcludingThoseIn<T>(this IEnumerable<T> source, IEnumerable<T> other)
        => source.Except(other);

    public static IEnumerable<T> FollowedBy<T>(this IEnumerable<T> source, IEnumerable<T> other)
        => source.Concat(other);

    public static List<T> Gathered<T>(this IEnumerable<T> source)
        => source.ToList();

    public static T[] AsArray<T>(this IEnumerable<T> source)
        => source.ToArray();

    public static IEnumerable<T> OrIfNone<T>(this IEnumerable<T> source, T defaultValue)
        => source.DefaultIfEmpty(defaultValue);

    public static IEnumerable<T> AndAlso<T>(this IEnumerable<T> source, T element)
        => source.Append(element);

    public static IEnumerable<T> StartingWith<T>(this IEnumerable<T> source, T element)
        => source.Prepend(element);

    public static bool MatchesExactly<T>(this IEnumerable<T> source, IEnumerable<T> other)
        => source.SequenceEqual(other);

    public static IEnumerable<TResult> PairedWith<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> selector)
        => first.Zip(second, selector);

    public static IEnumerable<IGrouping<TKey, T>> GroupedBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        => source.GroupBy(keySelector);

    public static IEnumerable<IGameEntity> Alphabetically(this IEnumerable<IGameEntity> source)
        => source.OrderBy(entity => entity.Name);

    public static IEnumerable<T> ByAscending<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        => source.OrderBy(keySelector);

    public static IEnumerable<T> ByDescending<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        => source.OrderByDescending(keySelector);
}
