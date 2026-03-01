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

    public static ChapterBuilder DefineChapters(this Game game)
    {
        ArgumentNullException.ThrowIfNull(game);
        return new ChapterBuilder();
    }

    public static IScheduleQueue Schedule(this Game game)
    {
        ArgumentNullException.ThrowIfNull(game);
        return game.State.Schedule;
    }

    public static Game OnAction(this Game game, string actionId, Action<ActionTriggerContext> handler)
    {
        ArgumentNullException.ThrowIfNull(game);
        game.State.ActionTriggers.OnAction(actionId, handler);
        return game;
    }

    public static Game OnNpcDeath(this Game game, string npcId, Action<ActionTriggerContext> handler)
    {
        ArgumentNullException.ThrowIfNull(game);
        game.State.ActionTriggers.OnNpcDeath(npcId, handler);
        return game;
    }

    public static Game OnItemPickup(this Game game, string itemId, Action<ActionTriggerContext> handler)
    {
        ArgumentNullException.ThrowIfNull(game);
        game.State.ActionTriggers.OnItemPickup(itemId, handler);
        return game;
    }

    public static GameValidator CreateValidator(this Game game)
    {
        ArgumentNullException.ThrowIfNull(game);
        return new GameValidator(game);
    }

    public static GameExplorer CreateExplorer(this Game game)
    {
        ArgumentNullException.ThrowIfNull(game);
        return new GameExplorer(game);
    }

    public static Game EnableTestingMode(this Game game)
    {
        ArgumentNullException.ThrowIfNull(game);
        game.State.TestingModeEnabled = true;
        return game;
    }
}
