// <copyright file="IGrammarProvider.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IGrammarProvider
{
    string WithArticle(string noun);
    string Plural(string noun, int count);
    string NaturalList(IEnumerable<string> items);
}
