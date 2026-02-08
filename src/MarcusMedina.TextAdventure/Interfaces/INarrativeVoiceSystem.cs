// <copyright file="INarrativeVoiceSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface INarrativeVoiceSystem
{
    Voice Voice { get; }
    Tense Tense { get; }
    string SubjectName { get; }
    INarrativeVoiceSystem SetVoice(Voice voice);
    INarrativeVoiceSystem Tense(Tense tense);
    INarrativeVoiceSystem Subject(string subjectName);
    string Transform(string text);
    string TransformFlashback(string text);
}
