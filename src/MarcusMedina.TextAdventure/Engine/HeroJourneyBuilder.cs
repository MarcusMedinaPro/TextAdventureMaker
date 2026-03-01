// <copyright file="HeroJourneyBuilder.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class HeroJourneyBuilder
{
    private readonly HeroJourney _journey = new();
    private HeroJourneyStage? _current;

    public HeroJourneyBuilder OrdinaryWorld(string locationId)
    {
        _current = _journey.GetOrCreateStage(JourneyStage.OrdinaryWorld).SetLocation(locationId);
        return this;
    }

    public HeroJourneyBuilder EstablishNormalcy(string text)
    {
        _current?.AddNote(text);
        return this;
    }

    public HeroJourneyBuilder CallToAdventure()
    {
        _current = _journey.GetOrCreateStage(JourneyStage.CallToAdventure);
        return this;
    }

    public HeroJourneyBuilder Trigger(Action<JourneyTriggerBuilder> configure)
    {
        if (_current == null)
        {
            return this;
        }

        JourneyTriggerBuilder builder = new(_current);
        configure?.Invoke(builder);
        return this;
    }

    public HeroJourneyBuilder MeetingMentor(string npcId)
    {
        _current = _journey.GetOrCreateStage(JourneyStage.MeetingMentor).SetNpc(npcId);
        return this;
    }

    public HeroJourneyBuilder Provides(Action<JourneyGiftBuilder> configure)
    {
        if (_current == null)
        {
            return this;
        }

        JourneyGiftBuilder builder = new(_current);
        configure?.Invoke(builder);
        return this;
    }

    public HeroJourneyBuilder CrossingThreshold()
    {
        _current = _journey.GetOrCreateStage(JourneyStage.CrossingThreshold);
        return this;
    }

    public HeroJourneyBuilder PointOfNoReturn()
    {
        _current?.MarkPointOfNoReturn();
        return this;
    }

    public HeroJourneyBuilder Ordeal()
    {
        _current = _journey.GetOrCreateStage(JourneyStage.Ordeal);
        return this;
    }

    public HeroJourneyBuilder FaceGreatestFear(string targetId)
    {
        _current?.SetTarget(targetId);
        return this;
    }

    public HeroJourneyBuilder ReturnWithElixir()
    {
        _current = _journey.GetOrCreateStage(JourneyStage.ReturnWithElixir);
        return this;
    }

    public HeroJourneyBuilder TransformationComplete(Action<JourneyTransformationBuilder> configure)
    {
        if (_current == null)
        {
            return this;
        }

        JourneyTransformationBuilder builder = new(_current);
        configure?.Invoke(builder);
        return this;
    }

    public HeroJourney Build()
    {
        return _journey;
    }
}

public sealed class JourneyTriggerBuilder(HeroJourneyStage stage)
{
    public JourneyTriggerBuilder OnEvent(string eventId)
    {
        stage.SetData("trigger_event", eventId);
        return this;
    }
}

public sealed class JourneyGiftBuilder(HeroJourneyStage stage)
{
    public JourneyGiftBuilder Item(string itemId)
    {
        stage.SetData("gift_item", itemId);
        return this;
    }
}

public sealed class JourneyTransformationBuilder(HeroJourneyStage stage)
{
    public JourneyTransformationBuilder SetTitle(string title)
    {
        stage.SetData("title", title);
        return this;
    }
}
