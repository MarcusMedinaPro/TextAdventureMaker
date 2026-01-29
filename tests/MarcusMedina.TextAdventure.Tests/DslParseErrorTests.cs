// <copyright file="DslParseErrorTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Dsl;

public class DslParseErrorTests
{
    [Theory]
    [InlineData("locaton", "location")]
    [InlineData("loction", "location")]
    [InlineData("iem", "item")]
    [InlineData("itm", "item")]
    [InlineData("dor", "door")]
    [InlineData("exi", "exit")]
    [InlineData("wold", "world")]
    [InlineData("gol", "goal")]
    public void SuggestKeyword_SuggestsCorrectKeyword(string input, string expected)
    {
        var suggestion = DslErrorHelper.SuggestKeyword(input);

        Assert.Equal(expected, suggestion);
    }

    [Fact]
    public void SuggestKeyword_ReturnsNull_ForVeryDifferentInput()
    {
        var suggestion = DslErrorHelper.SuggestKeyword("xyz123");

        Assert.Null(suggestion);
    }

    [Fact]
    public void UnknownKeyword_CreatesErrorWithSuggestion()
    {
        var error = DslErrorHelper.UnknownKeyword(5, "locaton: test", "locaton");

        Assert.Equal(5, error.Line);
        Assert.Contains("Unknown keyword", error.Message);
        Assert.Contains("location", error.Suggestion);
    }

    [Fact]
    public void NoCurrentLocation_CreatesHelpfulError()
    {
        var error = DslErrorHelper.NoCurrentLocation(3, "item: sword | sword | A sword.", "item");

        Assert.Equal(3, error.Line);
        Assert.Contains("requires a location", error.Message);
        Assert.Contains("Add a 'location:'", error.Suggestion);
    }

    [Fact]
    public void InvalidExitSyntax_CreatesHelpfulError()
    {
        var error = DslErrorHelper.InvalidExitSyntax(7, "exit: north forest");

        Assert.Contains("Invalid exit syntax", error.Message);
        Assert.Contains("direction -> target", error.Suggestion);
    }

    [Fact]
    public void DslParseError_ToString_FormatsCorrectly()
    {
        var error = new DslParseError(
            line: 10,
            lineContent: "locaton: test_room",
            message: "Unknown keyword: 'locaton'",
            suggestion: "Did you mean 'location'?",
            column: 1);

        var result = error.ToString();

        Assert.Contains("Line 10:", result);
        Assert.Contains("Unknown keyword", result);
        Assert.Contains("Did you mean 'location'?", result);
        Assert.Contains("locaton: test_room", result);
    }

    [Fact]
    public void DslParseException_WithSingleError_FormatsCorrectly()
    {
        var error = new DslParseError(1, "bad line", "Something went wrong");
        var exception = new DslParseException(error);

        Assert.Contains("parsing error", exception.Message);
        _ = Assert.Single(exception.Errors);
    }

    [Fact]
    public void DslParseException_WithMultipleErrors_FormatsCorrectly()
    {
        var errors = new[]
        {
            new DslParseError(1, "error 1", "First error"),
            new DslParseError(5, "error 2", "Second error")
        };
        var exception = new DslParseException(errors);

        Assert.Contains("2 errors", exception.Message);
        Assert.Equal(2, exception.Errors.Count);
    }
}
