// <copyright file="GameExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Extensions;

public static class GameExtensions
{
    public static INarrativeVoiceSystem SetNarrativeVoice(this Game game, Voice voice)
    {
        ArgumentNullException.ThrowIfNull(game);
        game.State.NarrativeVoice.SetVoice(voice);
        return game.State.NarrativeVoice;
    }
}
