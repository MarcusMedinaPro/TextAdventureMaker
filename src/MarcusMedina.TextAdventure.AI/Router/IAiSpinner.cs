// <copyright file="IAiSpinner.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Router;

/// <summary>Abstraction for a spinner shown while an AI provider call is in flight.</summary>
public interface IAiSpinner
{
    /// <summary>Start the spinner. Dispose the returned scope to stop it.</summary>
    IDisposable Begin();
}
