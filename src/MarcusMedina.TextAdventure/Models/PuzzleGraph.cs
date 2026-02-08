// <copyright file="PuzzleGraph.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Text;

namespace MarcusMedina.TextAdventure.Models;

public sealed class PuzzleGraph
{
    private readonly List<(string From, string To)> _edges = [];

    public IReadOnlyList<(string From, string To)> Edges => _edges;

    public void AddEdge(string from, string to)
    {
        if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
        {
            return;
        }

        _edges.Add((from, to));
    }

    public string ToMermaid()
    {
        StringBuilder builder = new();
        _ = builder.AppendLine("graph LR");
        foreach ((string from, string to) in _edges)
        {
            _ = builder.AppendLine($"    {from} --> {to}");
        }

        return builder.ToString().TrimEnd();
    }

    public IReadOnlyCollection<string> FindCircularDependencies()
    {
        HashSet<string> visited = new(StringComparer.OrdinalIgnoreCase);
        HashSet<string> stack = new(StringComparer.OrdinalIgnoreCase);
        List<string> cycles = [];

        foreach (string node in _edges.Select(edge => edge.From).Distinct())
        {
            if (DetectCycle(node, visited, stack))
            {
                cycles.Add(node);
            }
        }

        return cycles;
    }

    private bool DetectCycle(string node, HashSet<string> visited, HashSet<string> stack)
    {
        if (!visited.Add(node))
        {
            return false;
        }

        stack.Add(node);

        foreach (string next in _edges.Where(edge => edge.From.Equals(node, StringComparison.OrdinalIgnoreCase)).Select(edge => edge.To))
        {
            if (!visited.Contains(next) && DetectCycle(next, visited, stack))
            {
                return true;
            }

            if (stack.Contains(next))
            {
                return true;
            }
        }

        stack.Remove(node);
        return false;
    }
}
