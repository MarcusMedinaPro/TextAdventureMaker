// <copyright file="StoryMapper.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Dsl;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tools;

public sealed class StoryMapper
{
    public StoryMap ImportFromDsl(string path)
    {
        AdventureDslParser parser = new();
        DslAdventure adventure = parser.ParseFile(path);
        StoryMap map = new();

        foreach ((string id, Location location) in adventure.Locations)
        {
            map.AddNode(id, location.GetDescription());
        }

        foreach ((string id, Location location) in adventure.Locations)
        {
            foreach (KeyValuePair<Direction, Exit> exit in location.Exits)
            {
                map.AddEdge(id, exit.Value.Target.Id, exit.Key.ToString());
            }
        }

        return map;
    }

    public void ExportToDsl(DslAdventure adventure, string path)
    {
        AdventureDslExporter exporter = new();
        string dsl = exporter.Export(adventure);
        File.WriteAllText(path, dsl);
    }
}
