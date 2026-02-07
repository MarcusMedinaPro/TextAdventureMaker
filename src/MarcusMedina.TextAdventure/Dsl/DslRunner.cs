// <copyright file="DslRunner.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>


using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Parsing;

namespace MarcusMedina.TextAdventure.Dsl;
/// <summary>
/// Simple runner for .adventure DSL files from command line.
/// </summary>
public static class DslRunner
{
    /// <summary>
    /// Runs a .adventure file. Returns true if a file was run.
    /// Call this at the start of Program.cs to handle CLI arguments.
    /// </summary>
    /// <example>
    /// <code>
    /// // In Program.cs:
    /// if (DslRunner.TryRunFromArgs(args)) return;
    /// // ... rest of your game setup
    /// </code>
    /// </example>
    public static bool TryRunFromArgs(string[] args)
    {
        if (args.Length == 0)
        {
            return false;
        }

        string path = args[0];
        if (!path.EndsWith(".adventure", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!File.Exists(path))
        {
            Console.WriteLine($"Error: File not found: {path}");
            return true;
        }

        Run(path);
        return true;
    }

    /// <summary>
    /// Runs a .adventure file.
    /// </summary>
    public static void Run(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            AdventureDslParser parser = new();
            DslAdventure adventure = parser.ParseFile(path);

            string title = adventure.Metadata.TryGetValue("world", out string? w) ? w : Path.GetFileNameWithoutExtension(path);
            string? goal = adventure.Metadata.TryGetValue("goal", out string? g) ? g : null;

            Console.WriteLine($"=== {title} ===");
            if (!string.IsNullOrWhiteSpace(goal))
            {
                Console.WriteLine($"Goal: {goal}");
            }

            Console.WriteLine();

            Game game = GameBuilder.Create()
                .UseState(adventure.State)
                .UseParser(new KeywordParser(KeywordParserConfig.Default))
                .AddTurnStart(g =>
                {
                    CommandResult look = g.State.Look();
                    g.Output.WriteLine(look.Message);
                })
                .Build();

            game.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading adventure: {ex.Message}");
        }
    }
}
