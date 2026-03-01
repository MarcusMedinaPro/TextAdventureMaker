// <copyright file="StoryMap.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Tools;

public sealed class StoryMap
{
    private readonly List<StoryNode> _nodes = [];
    private readonly List<StoryEdge> _edges = [];

    public IReadOnlyList<StoryNode> Nodes => _nodes;
    public IReadOnlyList<StoryEdge> Edges => _edges;

    public StoryNode AddNode(string id, string? description = null)
    {
        StoryNode node = new(id, description);
        _nodes.Add(node);
        return node;
    }

    public StoryEdge AddEdge(string fromId, string toId, string? label = null)
    {
        StoryEdge edge = new(fromId, toId, label);
        _edges.Add(edge);
        return edge;
    }
}

public sealed record StoryNode(string Id, string? Description);
public sealed record StoryEdge(string FromId, string ToId, string? Label);
