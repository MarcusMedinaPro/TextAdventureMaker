// <copyright file="CombinationResult.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Models;

using MarcusMedina.TextAdventure.Interfaces;

public class CombinationResult
{
    public bool Success { get; }
    public IReadOnlyList<IItem> Created { get; }

    public CombinationResult(bool success, IReadOnlyList<IItem> created)
    {
        Success = success;
        Created = created;
    }

    public static CombinationResult Fail() => new(false, Array.Empty<IItem>());
    public static CombinationResult Ok(params IItem[] created) => new(true, created);
}
