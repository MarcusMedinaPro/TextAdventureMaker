// <copyright file="ICurrency.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Interfaces;

/// <summary>
/// Represents a currency type used in the game economy.
/// </summary>
public interface ICurrency
{
    /// <summary>
    /// Unique identifier for the currency.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Display name of the currency.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Symbol used to display currency amounts (£, €, $, etc.).
    /// </summary>
    string Symbol { get; }

    /// <summary>
    /// Ratio of subunits to main unit (e.g. 100 for pounds to pence).
    /// </summary>
    int SubunitRatio { get; }
}
