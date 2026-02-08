// <copyright file="EconomySystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Engine;

public sealed class EconomySystem
{
    public int Balance { get; private set; }

    public void AddCurrency(int amount)
    {
        Balance = Math.Max(0, Balance + amount);
    }

    public bool Spend(int amount)
    {
        if (amount <= 0)
        {
            return true;
        }

        if (Balance < amount)
        {
            return false;
        }

        Balance -= amount;
        return true;
    }
}
