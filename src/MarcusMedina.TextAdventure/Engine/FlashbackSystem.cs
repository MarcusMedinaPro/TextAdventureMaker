// <copyright file="FlashbackSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class FlashbackSystem : IFlashbackSystem
{
    private readonly List<Memory> _memories = [];

    public IReadOnlyList<Memory> Memories => _memories;

    public Memory AddMemory(string id)
    {
        Memory memory = new(id);
        _memories.Add(memory);
        return memory;
    }

    public Memory? GetMemory(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        return _memories.FirstOrDefault(memory =>
            string.Equals(memory.Id, id, StringComparison.OrdinalIgnoreCase));
    }
}
