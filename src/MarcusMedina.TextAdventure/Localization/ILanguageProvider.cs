// <copyright file="ILanguageProvider.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Localization;

public interface ILanguageProvider
{
    string Get(string key);
    string Format(string key, params object[] args);
}
