// <copyright file="IItem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Interfaces;

using MarcusMedina.TextAdventure.Enums;

public interface IItem : IGameEntity
{
    event Action<IItem>? OnDestroy;

    event Action<IItem>? OnDrop;

    /// <summary>Raised when the item is moved.</summary>
    event Action<IItem>? OnMove;

    event Action<IItem>? OnTake;

    event Action<IItem>? OnUse;

    IReadOnlyList<string> Aliases { get; }
    bool HiddenFromItemList { get; }
    new string Id { get; }
    new string Name { get; }
    bool Readable { get; }

    int ReadingCost { get; }

    bool RequiresTakeToRead { get; }

    bool Takeable { get; }

    float Weight { get; }

    IItem AddAliases(params string[] aliases);

    bool CanRead(IGameState state);

    IItem Clone();

    IItem Description(string text);

    void Destroy();

    void Drop();

    string GetDescription();

    string? GetReaction(ItemAction action);

    string? GetReadText();

    IItem HideFromItemList(bool hidden = true);

    bool Matches(string name);

    /// <summary>Trigger the move event for this item.</summary>
    void Move();

    IItem RequiresToRead(Func<IGameState, bool> predicate);

    IItem RequireTakeToRead();

    IItem SetReaction(ItemAction action, string text);

    IItem SetReadable(bool readable = true);

    IItem SetReadingCost(int turns);

    IItem SetReadText(string text);

    IItem SetTakeable(bool takeable);

    IItem SetWeight(float weight);

    void Take();

    void Use();
}