// <copyright file="ChapterDslParser.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Engine;

public sealed class ChapterDslParser
{
    public ChapterSystem Parse(string dsl)
    {
        _ = dsl;
        return new ChapterSystem();
    }
}
