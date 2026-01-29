// <copyright file="JsonSaveSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;
using System.Text.Json;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class JsonSaveSystem : ISaveSystem
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    public void Save(string path, GameMemento memento)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(memento);
        string json = JsonSerializer.Serialize(memento, Options);
        File.WriteAllText(path, json);
    }

    public GameMemento Load(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        string json = File.ReadAllText(path);
        GameMemento? memento = JsonSerializer.Deserialize<GameMemento>(json, Options);
        return memento ?? throw new InvalidDataException("Save file is invalid.");
    }
}
