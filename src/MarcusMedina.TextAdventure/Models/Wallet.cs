// <copyright file="Wallet.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Manages player currency balances for one or more currencies.
/// </summary>
public sealed class Wallet
{
    private readonly Dictionary<string, int> _balances = [];

    /// <summary>
    /// Gets the balance for a given currency.
    /// </summary>
    public int GetBalance(string currencyId) =>
        _balances.TryGetValue(currencyId, out var balance) ? balance : 0;

    /// <summary>
    /// Checks if player can afford a purchase.
    /// </summary>
    public bool CanAfford(string currencyId, int amount) =>
        GetBalance(currencyId) >= amount;

    /// <summary>
    /// Attempts to spend currency. Returns true if successful.
    /// </summary>
    public bool TrySpend(string currencyId, int amount)
    {
        if (!CanAfford(currencyId, amount))
            return false;

        _balances[currencyId] = GetBalance(currencyId) - amount;
        return true;
    }

    /// <summary>
    /// Adds currency to the wallet.
    /// </summary>
    public void Add(string currencyId, int amount) =>
        _balances[currencyId] = GetBalance(currencyId) + amount;

    /// <summary>
    /// Formats a balance as a string with currency symbol.
    /// </summary>
    public string Format(string currencyId, ICurrency currency) =>
        $"{currency.Symbol}{GetBalance(currencyId)}";
}
