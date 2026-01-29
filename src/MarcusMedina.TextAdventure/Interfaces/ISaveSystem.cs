// <copyright file="ISaveSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Interfaces;

using MarcusMedina.TextAdventure.Models;

public interface ISaveSystem
{
    void Save(string path, GameMemento memento);
    GameMemento Load(string path);
}
