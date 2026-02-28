// <copyright file="ConditionalExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Extensions;

public static class ConditionalExtensions
{
    public static ConditionalResult<string> Then(this bool condition, string trueValue) =>
        new(condition, trueValue);

    public static ConditionalResult<T> Then<T>(this bool condition, Func<T> trueAction) =>
        new(condition, condition ? trueAction() : default);

    public static void Then(this bool condition, Action action)
    {
        if (condition)
            action();
    }
}

public sealed class ConditionalResult<T>(bool condition, T? value)
{
    public T Else(T falseValue) =>
        condition ? value! : falseValue;

    public T Else(Func<T> falseAction) =>
        condition ? value! : falseAction();
}
