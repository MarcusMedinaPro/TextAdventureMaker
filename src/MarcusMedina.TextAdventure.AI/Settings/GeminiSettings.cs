// <copyright file="GeminiSettings.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Settings;

public sealed class GeminiSettings(
    string apiKey = "",
    string endpointTemplate = "https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}",
    string model = "gemini-1.5-flash")
{
    public string ApiKey { get; set; } = apiKey;
    public string EndpointTemplate { get; set; } = endpointTemplate;
    public string Model { get; set; } = model;
    public string? SystemPrompt { get; set; }
    public double Temperature { get; set; } = 0.2;
    public int TimeoutMs { get; set; } = 8000;
    public bool Enabled { get; set; } = true;
}
