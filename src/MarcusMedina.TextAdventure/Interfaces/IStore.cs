// <copyright file="IStore.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Interfaces;

/// <summary>
/// Represents a store or shop where players can buy and sell items.
/// </summary>
public interface IStore
{
    /// <summary>
    /// Unique identifier for the store.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Display name of the store.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Description of the store.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Items currently available for sale.
    /// </summary>
    IReadOnlyList<StoreItem> Inventory { get; }

    /// <summary>
    /// Currency the store uses (typically "gold").
    /// </summary>
    string CurrencyId { get; }

    /// <summary>
    /// Whether the store is currently open for business.
    /// </summary>
    bool IsOpen { get; }

    /// <summary>
    /// Price multiplier (1.0 = normal, 1.5 = 50% more expensive).
    /// </summary>
    float PriceModifier { get; }

    /// <summary>
    /// Attempts to buy an item from the store.
    /// </summary>
    BuyResult TryBuy(IGameState state, string itemId, int quantity = 1);

    /// <summary>
    /// Attempts to sell an item to the store.
    /// </summary>
    SellResult TrySell(IGameState state, IItem item, int quantity = 1);
}

/// <summary>
/// Represents an item available in a store.
/// </summary>
public sealed record StoreItem(
    string ItemId,
    string Name,
    int BasePrice,
    int Stock,
    int MaxPerCustomer
);

/// <summary>
/// Result of a buy operation.
/// </summary>
public sealed record BuyResult(bool Success, string Message, IItem? Item = null);

/// <summary>
/// Result of a sell operation.
/// </summary>
public sealed record SellResult(bool Success, string Message, int? Amount = null);
