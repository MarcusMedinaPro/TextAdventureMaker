// <copyright file="ConditionalExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Extensions;

public static class ConditionalExtensions
{
    public static ConditionalResult<string> Then(this bool condition, string trueValue)
    {
        return new ConditionalResult<string>(condition, trueValue);
    }

    public static ConditionalResult<T> Then<T>(this bool condition, Func<T> trueAction)
    {
        return new ConditionalResult<T>(condition, condition ? trueAction() : default);
    }

    public static void Then(this bool condition, Action action)
    {
        if (condition)
        {
            action();
        }
    }
}

public sealed class ConditionalResult<T>
{
    private readonly bool _condition;
    private readonly T? _value;

    public ConditionalResult(bool condition, T? value)
    {
        _condition = condition;
        _value = value;
    }

    public T Else(T falseValue)
    {
        return _condition ? _value! : falseValue;
    }

    public T Else(Func<T> falseAction)
    {
        return _condition ? _value! : falseAction();
    }
}
