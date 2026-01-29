// <copyright file="StringExtensionsTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Extensions;

public class StringExtensionsTests
{
    [Fact]
    public void LevenshteinDistanceTo_KnownExample_ReturnsExpected() => Assert.Equal(3, "kitten".LevenshteinDistanceTo("sitting"));

    [Fact]
    public void SimilarTo_UsesLevenshteinDistance() => Assert.Equal(3, "kitten".SimilarTo("sitting"));

    [Fact]
    public void FuzzyDistanceTo_CollapsesRepeats() => Assert.Equal(1, "loook".FuzzyDistanceTo("look", 1));

    [Fact]
    public void FuzzyMatch_RespectsMaxDistance()
    {
        Assert.True("lokk".FuzzyMatch("look", 1));
        Assert.False("lokk".FuzzyMatch("look", 0));
    }

    [Fact]
    public void CollapseRepeats_RemovesConsecutiveDuplicates() => Assert.Equal("balon", "baallooon".CollapseRepeats());

    [Fact]
    public void SoundexKey_ReturnsStableFourChars()
    {
        Assert.Equal(4, "Steven".SoundexKey().Length);
        Assert.StartsWith("S", "Steven".SoundexKey());
    }

    [Fact]
    public void SoundsLike_MatchesCommonVariants() => Assert.True("Steven".SoundsLike("Stephen"));
}
