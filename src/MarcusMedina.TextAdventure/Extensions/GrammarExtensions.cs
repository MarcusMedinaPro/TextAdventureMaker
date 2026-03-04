// <copyright file="GrammarExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Extensions;

public static class GrammarExtensions
{
    public static IGrammarProvider Provider { get; set; } = new EnglishGrammar();

    public static string WithArticle(this string noun) => Provider.WithArticle(noun);

    public static string Plural(this string noun, int count) => Provider.Plural(noun, count);

    public static string NaturalList(this IEnumerable<string> items) => Provider.NaturalList(items);
}
