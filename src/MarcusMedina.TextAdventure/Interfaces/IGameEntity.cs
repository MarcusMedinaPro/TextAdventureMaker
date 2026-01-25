// <copyright file="IGameEntity.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Interfaces;

public interface IGameEntity
{
    string Id { get; }
    string Name { get; }
    IDictionary<string, string> Properties { get; }
}
