// <copyright file="QueryExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Linq;

namespace MarcusMedina.TextAdventure.Linq;

public static class QueryExtensions
{
    public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        => Enumerable.Where(source, predicate);

    public static IEnumerable<TResult> Select<T, TResult>(this IEnumerable<T> source, Func<T, TResult> selector)
        => Enumerable.Select(source, selector);

    public static IEnumerable<TResult> SelectMany<T, TResult>(this IEnumerable<T> source, Func<T, IEnumerable<TResult>> selector)
        => Enumerable.SelectMany(source, selector);

    public static bool Any<T>(this IEnumerable<T> source)
        => Enumerable.Any(source);

    public static bool Any<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        => Enumerable.Any(source, predicate);

    public static bool All<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        => Enumerable.All(source, predicate);

    public static T First<T>(this IEnumerable<T> source)
        => Enumerable.First(source);

    public static T? FirstOrDefault<T>(this IEnumerable<T> source)
        => Enumerable.FirstOrDefault(source);

    public static T Last<T>(this IEnumerable<T> source)
        => Enumerable.Last(source);

    public static T? LastOrDefault<T>(this IEnumerable<T> source)
        => Enumerable.LastOrDefault(source);

    public static T Single<T>(this IEnumerable<T> source)
        => Enumerable.Single(source);

    public static T? SingleOrDefault<T>(this IEnumerable<T> source)
        => Enumerable.SingleOrDefault(source);

    public static T ElementAt<T>(this IEnumerable<T> source, int index)
        => Enumerable.ElementAt(source, index);

    public static T? ElementAtOrDefault<T>(this IEnumerable<T> source, int index)
        => Enumerable.ElementAtOrDefault(source, index);

    public static IEnumerable<T> OrderBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector)
        => Enumerable.OrderBy(source, selector);

    public static IEnumerable<T> OrderByDescending<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector)
        => Enumerable.OrderByDescending(source, selector);

    public static IEnumerable<T> ThenBy<T, TKey>(this IOrderedEnumerable<T> source, Func<T, TKey> selector)
        => Enumerable.ThenBy(source, selector);

    public static IEnumerable<T> ThenByDescending<T, TKey>(this IOrderedEnumerable<T> source, Func<T, TKey> selector)
        => Enumerable.ThenByDescending(source, selector);

    public static IEnumerable<T> Reverse<T>(this IEnumerable<T> source)
        => Enumerable.Reverse(source);

    public static int Count<T>(this IEnumerable<T> source)
        => Enumerable.Count(source);

    public static int Count<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        => Enumerable.Count(source, predicate);

    public static int Sum(this IEnumerable<int> source)
        => Enumerable.Sum(source);

    public static int Sum<T>(this IEnumerable<T> source, Func<T, int> selector)
        => Enumerable.Sum(source, selector);

    public static double Average(this IEnumerable<int> source)
        => Enumerable.Average(source);

    public static double Average<T>(this IEnumerable<T> source, Func<T, int> selector)
        => Enumerable.Average(source, selector);

    public static IEnumerable<T> Take<T>(this IEnumerable<T> source, int count)
        => Enumerable.Take(source, count);

    public static IEnumerable<T> Skip<T>(this IEnumerable<T> source, int count)
        => Enumerable.Skip(source, count);

    public static IEnumerable<T> TakeWhile<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        => Enumerable.TakeWhile(source, predicate);

    public static IEnumerable<T> SkipWhile<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        => Enumerable.SkipWhile(source, predicate);

    public static IEnumerable<T[]> Chunk<T>(this IEnumerable<T> source, int size)
        => Enumerable.Chunk(source, size);

    public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source)
        => Enumerable.Distinct(source);

    public static IEnumerable<T> Union<T>(this IEnumerable<T> first, IEnumerable<T> second)
        => Enumerable.Union(first, second);

    public static IEnumerable<T> Intersect<T>(this IEnumerable<T> first, IEnumerable<T> second)
        => Enumerable.Intersect(first, second);

    public static IEnumerable<T> Except<T>(this IEnumerable<T> first, IEnumerable<T> second)
        => Enumerable.Except(first, second);

    public static IEnumerable<T> Concat<T>(this IEnumerable<T> first, IEnumerable<T> second)
        => Enumerable.Concat(first, second);

    public static List<T> ToList<T>(this IEnumerable<T> source)
        => Enumerable.ToList(source);

    public static T[] ToArray<T>(this IEnumerable<T> source)
        => Enumerable.ToArray(source);

    public static Dictionary<TKey, T> ToDictionary<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        where TKey : notnull
        => Enumerable.ToDictionary(source, keySelector);

    public static ILookup<TKey, T> ToLookup<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        => Enumerable.ToLookup(source, keySelector);

    public static IEnumerable<T> DefaultIfEmpty<T>(this IEnumerable<T> source, T defaultValue)
        => Enumerable.DefaultIfEmpty(source, defaultValue);

    public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T element)
        => Enumerable.Append(source, element);

    public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T element)
        => Enumerable.Prepend(source, element);

    public static bool SequenceEqual<T>(this IEnumerable<T> first, IEnumerable<T> second)
        => Enumerable.SequenceEqual(first, second);

    public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        => Enumerable.Zip(first, second, resultSelector);

    public static IEnumerable<IGrouping<TKey, T>> GroupBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        => Enumerable.GroupBy(source, keySelector);
}
