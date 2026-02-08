// <copyright file="FlashbackLocationBuilder.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class FlashbackLocationBuilder(FlashbackTrigger trigger)
{
    public FlashbackLocationBuilder When(Func<IGameState, bool> predicate)
    {
        trigger.When(predicate);
        return this;
    }

    public FlashbackLocationBuilder WithTransition(string text)
    {
        trigger.WithTransition(text);
        return this;
    }

    public FlashbackLocationBuilder ReturnsTo(ILocation location)
    {
        if (location != null)
        {
            trigger.ReturnsTo(location.Id);
        }

        return this;
    }
}
