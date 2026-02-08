// <copyright file="GameExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Extensions;

public static class GameExtensions
{
    public static INarrativeVoiceSystem SetNarrativeVoice(this Game game, Voice voice)
    {
        ArgumentNullException.ThrowIfNull(game);
        game.State.NarrativeVoice.SetVoice(voice);
        return game.State.NarrativeVoice;
    }

    public static HeroJourneyBuilder UseHeroJourneyTemplate(this Game game)
    {
        ArgumentNullException.ThrowIfNull(game);
        return new HeroJourneyBuilder();
    }

    public static TragicArcBuilder UseTragicArc(this Game game)
    {
        ArgumentNullException.ThrowIfNull(game);
        return new TragicArcBuilder();
    }

    public static TransformationArcBuilder UseTransformationArc(this Game game)
    {
        ArgumentNullException.ThrowIfNull(game);
        return new TransformationArcBuilder();
    }

    public static EnsembleJourneyBuilder UseEnsembleJourney(this Game game)
    {
        ArgumentNullException.ThrowIfNull(game);
        return new EnsembleJourneyBuilder();
    }

    public static DescentArcBuilder UseDescentArc(this Game game)
    {
        ArgumentNullException.ThrowIfNull(game);
        return new DescentArcBuilder();
    }

    public static SpiralNarrativeBuilder UseSpiralNarrative(this Game game)
    {
        ArgumentNullException.ThrowIfNull(game);
        return new SpiralNarrativeBuilder();
    }

    public static MoralLabyrinthBuilder UseMoralLabyrinth(this Game game)
    {
        ArgumentNullException.ThrowIfNull(game);
        return new MoralLabyrinthBuilder();
    }

    public static CaretakerArcBuilder UseCaretakerArc(this Game game)
    {
        ArgumentNullException.ThrowIfNull(game);
        return new CaretakerArcBuilder();
    }

    public static WitnessArcBuilder UseWitnessArc(this Game game)
    {
        ArgumentNullException.ThrowIfNull(game);
        return new WitnessArcBuilder();
    }

    public static WorldShiftArcBuilder UseWorldShiftArc(this Game game)
    {
        ArgumentNullException.ThrowIfNull(game);
        return new WorldShiftArcBuilder();
    }

    public static PrescriptiveStructureBuilder UsePrescriptiveStructure(this Game game)
    {
        ArgumentNullException.ThrowIfNull(game);
        return new PrescriptiveStructureBuilder();
    }

    public static FamiliarToForeignBuilder UseFamiliarToForeign(this Game game)
    {
        ArgumentNullException.ThrowIfNull(game);
        return new FamiliarToForeignBuilder();
    }

    public static FramedNarrativeBuilder UseFramedNarrative(this Game game)
    {
        ArgumentNullException.ThrowIfNull(game);
        return new FramedNarrativeBuilder();
    }

    public static ProppianStructureBuilder UseProppianStructure(this Game game)
    {
        ArgumentNullException.ThrowIfNull(game);
        return new ProppianStructureBuilder();
    }

    public static Memory AddMemory(this Game game, string id)
    {
        ArgumentNullException.ThrowIfNull(game);
        return game.State.Flashbacks.AddMemory(id);
    }

    public static LayeredNarrativeBuilder UseLayeredNarrative(this Game game)
    {
        ArgumentNullException.ThrowIfNull(game);
        return new LayeredNarrativeBuilder();
    }
}
