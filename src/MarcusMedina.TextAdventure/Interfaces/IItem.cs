// <copyright file="IItem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IItem : IGameEntity
{
    new string Id { get; }
    new string Name { get; }
    string GetDescription();
    bool Takeable { get; }
    float Weight { get; }
    IReadOnlyList<string> Aliases { get; }
    bool Readable { get; }
    bool RequiresTakeToRead { get; }
    int ReadingCost { get; }
    string? GetReadText();

    event Action<IItem>? OnTake;
    event Action<IItem>? OnDrop;
    event Action<IItem>? OnUse;
    event Action<IItem>? OnDestroy;

    bool Matches(string name);
    IItem Description(string text);
    string? GetReaction(ItemAction action);
    IItem SetReaction(ItemAction action, string text);
    bool CanRead(IGameState state);
    IItem SetReadable(bool readable = true);
    IItem SetReadText(string text);
    IItem RequireTakeToRead();
    IItem SetReadingCost(int turns);
    IItem RequiresToRead(Func<IGameState, bool> predicate);
    void Take();
    void Drop();
    void Use();
    void Destroy();
}
