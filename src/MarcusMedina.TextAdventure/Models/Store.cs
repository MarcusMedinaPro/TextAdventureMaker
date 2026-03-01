// <copyright file="Store.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Concrete implementation of a store where players can buy and sell items.
/// </summary>
public sealed class Store : IStore
{
    private readonly List<StoreItem> _inventory = [];
    private readonly Dictionary<string, int> _purchaseCounts = [];

    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IReadOnlyList<StoreItem> Inventory => _inventory.AsReadOnly();
    public string CurrencyId { get; init; } = "gold";
    public bool IsOpen { get; set; } = true;
    public float PriceModifier { get; set; } = 1.0f;

    /// <summary>
    /// Adds an item to the store's inventory.
    /// </summary>
    public void AddItem(StoreItem item) => _inventory.Add(item);

    public BuyResult TryBuy(IGameState state, string itemId, int quantity = 1)
    {
        if (!IsOpen)
            return new BuyResult(false, "The store is closed.");

        var storeItem = _inventory.FirstOrDefault(i => i.ItemId.Equals(itemId, StringComparison.OrdinalIgnoreCase));
        if (storeItem is null)
            return new BuyResult(false, "Item not found.");

        if (storeItem.Stock != -1 && storeItem.Stock < quantity)
            return new BuyResult(false, "Not enough in stock.");

        var purchased = _purchaseCounts.GetValueOrDefault(itemId, 0);
        if (storeItem.MaxPerCustomer != -1 && purchased + quantity > storeItem.MaxPerCustomer)
            return new BuyResult(false, $"You can only buy {storeItem.MaxPerCustomer} of these.");

        var totalPrice = (int)(storeItem.BasePrice * quantity * PriceModifier);

        if (!state.Wallet.TrySpend(CurrencyId, totalPrice))
            return new BuyResult(false, $"You need {totalPrice} gold.");

        // Update stock and purchase count
        if (storeItem.Stock != -1)
        {
            var idx = _inventory.IndexOf(storeItem);
            _inventory[idx] = storeItem with { Stock = storeItem.Stock - quantity };
        }

        _purchaseCounts[itemId] = purchased + quantity;

        var item = new Item(itemId, storeItem.Name);
        _ = state.Inventory.Add(item);

        return new BuyResult(true, $"You bought {item.Name} for {totalPrice} gold.", item);
    }

    public SellResult TrySell(IGameState state, IItem item, int quantity = 1)
    {
        if (!IsOpen)
            return new SellResult(false, "The store is closed.");

        var baseValue = item.GetProperty<int>("value", 1);
        var sellPrice = (int)(baseValue * 0.5f);

        _ = state.Inventory.Remove(item);
        state.Wallet.Add(CurrencyId, sellPrice);

        return new SellResult(true, $"You sold {item.Name} for {sellPrice} gold.", sellPrice);
    }
}
