// <copyright file="MoodSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class MoodSystem : IMoodSystem
{
    private readonly Dictionary<string, MoodDetails> _details = new(StringComparer.OrdinalIgnoreCase);
    public Mood DefaultMood { get; }

    public MoodSystem(Mood defaultMood = Mood.Peaceful)
    {
        DefaultMood = defaultMood;
    }

    public void SetMood(ILocation location, Mood mood)
    {
        MoodDetails details = GetOrCreate(location);
        details.Mood = mood;
    }

    public Mood GetMood(ILocation location)
    {
        MoodDetails details = GetOrCreate(location);
        return details.Mood;
    }

    public void SetLighting(ILocation location, LightLevel level)
    {
        MoodDetails details = GetOrCreate(location);
        details.Lighting = level;
    }

    public void SetAmbientSound(ILocation location, string sound)
    {
        MoodDetails details = GetOrCreate(location);
        details.AmbientSound = sound;
    }

    public void SetSmell(ILocation location, string smell)
    {
        MoodDetails details = GetOrCreate(location);
        details.Smell = smell;
    }

    public void SetTemperature(ILocation location, string temperature)
    {
        MoodDetails details = GetOrCreate(location);
        details.Temperature = temperature;
    }

    public MoodDetails GetDetails(ILocation location)
    {
        return GetOrCreate(location);
    }

    public void Propagate(ILocation start, int depth = 1)
    {
        if (start == null || depth <= 0)
        {
            return;
        }

        Mood startMood = GetMood(start);
        Queue<(ILocation Loc, int Depth)> queue = new();
        HashSet<string> visited = new(StringComparer.OrdinalIgnoreCase) { start.Id };
        queue.Enqueue((start, 0));

        while (queue.Count > 0)
        {
            (ILocation loc, int currentDepth) = queue.Dequeue();
            if (currentDepth >= depth)
            {
                continue;
            }

            foreach (Exit exit in loc.Exits.Values)
            {
                ILocation next = exit.Target;
                if (!visited.Add(next.Id))
                {
                    continue;
                }

                if (!_details.ContainsKey(next.Id))
                {
                    SetMood(next, startMood);
                }

                queue.Enqueue((next, currentDepth + 1));
            }
        }
    }

    private MoodDetails GetOrCreate(ILocation location)
    {
        ArgumentNullException.ThrowIfNull(location);
        if (!_details.TryGetValue(location.Id, out MoodDetails? details))
        {
            details = new MoodDetails { Mood = DefaultMood };
            _details[location.Id] = details;
        }

        return details;
    }
}
