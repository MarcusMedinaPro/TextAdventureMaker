// <copyright file="AiParserOptions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Models;

public sealed class AiParserOptions
{
    public bool Enabled { get; set; } = true;
    public bool PreferLocalCommandFirst { get; set; } = true;
    public bool StrictMode { get; set; }
    public bool FallbackOnSafetyRejection { get; set; } = true;
    public bool FallbackOnInvalidAiCommand { get; set; } = true;
    public int TimeoutMs { get; set; } = 5000;
    public int EstimatedTokensPerRequest { get; set; } = 128;
    public Action<string, string>? DebugProbe { get; set; }
}
