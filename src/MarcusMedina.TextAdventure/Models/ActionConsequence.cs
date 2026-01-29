// <copyright file="ActionConsequence.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Models;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

/// <summary>
/// Defines consequences that occur when an action is performed on an item.
/// </summary>
public class ActionConsequence
{
    /// <summary>
    /// Message to display when the action occurs.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// If true, the original item is destroyed after the action.
    /// </summary>
    public bool DestroyItem { get; set; }

    /// <summary>
    /// Items to create when the action occurs (replaces the original if destroyed).
    /// </summary>
    public List<Item> CreatedItems { get; set; } = [];

    /// <summary>
    /// Flag to set when the action occurs.
    /// </summary>
    public string? SetFlag { get; set; }

    /// <summary>
    /// Counter to increment when the action occurs.
    /// </summary>
    public string? IncrementCounter { get; set; }

    /// <summary>
    /// Custom action to execute.
    /// </summary>
    public Action<IGameState>? CustomAction { get; set; }

    /// <summary>
    /// Creates a consequence that destroys the item.
    /// </summary>
    public static ActionConsequence Destroy(string? message = null) => new()
    {
        DestroyItem = true,
        Message = message
    };

    /// <summary>
    /// Creates a consequence that transforms the item into something else.
    /// </summary>
    public static ActionConsequence Transform(Item newItem, string? message = null) => new()
    {
        DestroyItem = true,
        CreatedItems = [newItem],
        Message = message
    };

    /// <summary>
    /// Creates a consequence that breaks the item into pieces.
    /// </summary>
    public static ActionConsequence Break(string message, params Item[] pieces) => new()
    {
        DestroyItem = true,
        CreatedItems = pieces.ToList(),
        Message = message
    };
}

/// <summary>
/// Extension methods for setting up item consequences.
/// </summary>
public static class ItemConsequenceExtensions
{
    private static readonly Dictionary<(string ItemId, ItemAction Action), ActionConsequence> Consequences = [];

    /// <summary>
    /// Sets a consequence for when this item is dropped.
    /// </summary>
    public static Item SetDropConsequence(this Item item, ActionConsequence consequence)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(consequence);
        Consequences[(item.Id, ItemAction.Drop)] = consequence;
        return item;
    }

    /// <summary>
    /// Sets a consequence for when this item is used.
    /// </summary>
    public static Item SetUseConsequence(this Item item, ActionConsequence consequence)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(consequence);
        Consequences[(item.Id, ItemAction.Use)] = consequence;
        return item;
    }

    /// <summary>
    /// Sets a consequence for when this item is taken.
    /// </summary>
    public static Item SetTakeConsequence(this Item item, ActionConsequence consequence)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(consequence);
        Consequences[(item.Id, ItemAction.Take)] = consequence;
        return item;
    }

    /// <summary>
    /// Gets the consequence for an action on an item.
    /// </summary>
    public static ActionConsequence? GetConsequence(this Item item, ItemAction action) => Consequences.TryGetValue((item.Id, action), out var consequence) ? consequence : null;

    /// <summary>
    /// Checks if an item has a consequence for the given action.
    /// </summary>
    public static bool HasConsequence(this Item item, ItemAction action) => Consequences.ContainsKey((item.Id, action));

    /// <summary>
    /// Makes the item fragile - it breaks when dropped.
    /// </summary>
    public static Item MakeFragile(this Item item, string breakMessage, Item? brokenVersion = null)
    {
        ArgumentNullException.ThrowIfNull(item);

        var consequence = new ActionConsequence
        {
            DestroyItem = true,
            Message = breakMessage
        };

        if (brokenVersion != null)
        {
            consequence.CreatedItems.Add(brokenVersion);
        }

        Consequences[(item.Id, ItemAction.Drop)] = consequence;
        return item;
    }

    /// <summary>
    /// Makes the item consumable - it's destroyed when used.
    /// </summary>
    public static Item MakeConsumable(this Item item, string consumeMessage)
    {
        ArgumentNullException.ThrowIfNull(item);

        Consequences[(item.Id, ItemAction.Use)] = ActionConsequence.Destroy(consumeMessage);
        return item;
    }
}
