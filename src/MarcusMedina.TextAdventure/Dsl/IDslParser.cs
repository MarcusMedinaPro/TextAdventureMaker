// <copyright file="IDslParser.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Dsl;

public interface IDslParser
{
    DslAdventure ParseFile(string path);
}
