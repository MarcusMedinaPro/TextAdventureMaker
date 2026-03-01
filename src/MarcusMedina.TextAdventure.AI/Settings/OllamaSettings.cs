// <copyright file="OllamaSettings.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Settings;

public sealed class OllamaSettings(string endpoint = "http://localhost:11434", string model = "llama2", string systemPrompt = "You are a helpful text adventure game command parser. Respond with only the command name and arguments. Examples: look, go north, take sword, talk to guard")
{
    public string Endpoint { get; set; } = endpoint;
    public string Model { get; set; } = model;
    public string SystemPrompt { get; set; } = systemPrompt;
    public double Temperature { get; set; } = 0.3;
    public int TimeoutMs { get; set; } = 5000;
    public bool Enabled { get; set; } = true;
}
