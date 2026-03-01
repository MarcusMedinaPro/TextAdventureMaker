// <copyright file="LmStudioSettings.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Settings;

public sealed class LmStudioSettings(
    string endpoint = "http://localhost:1234/v1/chat/completions",
    string model = "local-model")
{
    public string Endpoint { get; set; } = endpoint;
    public string Model { get; set; } = model;
    public string? ApiKey { get; set; }
    public string? SystemPrompt { get; set; }
    public double Temperature { get; set; } = 0.2;
    public int TimeoutMs { get; set; } = 8000;
    public bool Enabled { get; set; } = true;
}
