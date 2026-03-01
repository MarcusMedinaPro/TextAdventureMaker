// <copyright file="NarrativeVoiceSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class NarrativeVoiceSystem : INarrativeVoiceSystem
{
    public Voice Voice { get; private set; } = Voice.SecondPerson;
    public Tense Tense { get; private set; } = Tense.Present;
    public string SubjectName { get; private set; } = "the hero";

    public INarrativeVoiceSystem SetVoice(Voice voice)
    {
        Voice = voice;
        return this;
    }

    public INarrativeVoiceSystem SetTense(Tense tense)
    {
        Tense = tense;
        return this;
    }

    public INarrativeVoiceSystem Subject(string subjectName)
    {
        SubjectName = string.IsNullOrWhiteSpace(subjectName) ? SubjectName : subjectName;
        return this;
    }

    public string Transform(string text)
    {
        return Apply(text ?? "", Tense);
    }

    public string TransformFlashback(string text)
    {
        return Apply(text ?? "", Enums.Tense.Past);
    }

    private string Apply(string text, Tense tense)
    {
        string subject = Voice switch
        {
            Voice.FirstPerson => "I",
            Voice.SecondPerson => "You",
            _ => SubjectName
        };

        string be = (Voice, tense) switch
        {
            (Voice.FirstPerson, Enums.Tense.Past) => "was",
            (Voice.FirstPerson, _) => "am",
            (Voice.SecondPerson, Enums.Tense.Past) => "were",
            (Voice.SecondPerson, _) => "are",
            (_, Enums.Tense.Past) => "was",
            _ => "is"
        };

        string have = (Voice, tense) switch
        {
            (_, Enums.Tense.Past) => "had",
            (Voice.ThirdPerson, _) => "has",
            _ => "have"
        };

        string doVerb = (Voice, tense) switch
        {
            (_, Enums.Tense.Past) => "did",
            (Voice.ThirdPerson, _) => "does",
            _ => "do"
        };

        return text
            .Replace("{subject}", subject, StringComparison.OrdinalIgnoreCase)
            .Replace("{be}", be, StringComparison.OrdinalIgnoreCase)
            .Replace("{have}", have, StringComparison.OrdinalIgnoreCase)
            .Replace("{do}", doVerb, StringComparison.OrdinalIgnoreCase);
    }
}
