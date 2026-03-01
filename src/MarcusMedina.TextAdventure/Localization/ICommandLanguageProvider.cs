// <copyright file="ICommandLanguageProvider.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Localization;

public interface ICommandLanguageProvider : ILanguageProvider
{
    IReadOnlyList<string> GetCommandAliases(string command);
    IReadOnlyDictionary<string, List<string>> GetCommandMap();
    IReadOnlyDictionary<string, Direction> GetDirectionAliases();
}
