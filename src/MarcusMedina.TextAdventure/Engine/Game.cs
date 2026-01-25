// <copyright file="Game.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class Game : IGame
{
    private readonly List<Action<Game>> _turnStartHandlers = new();
    private readonly List<Action<Game, ICommand, CommandResult>> _turnEndHandlers = new();
    private bool _stopRequested;

    public GameState State { get; }
    public ICommandParser Parser { get; }
    public TextReader Input { get; }
    public TextWriter Output { get; }
    public string Prompt { get; set; }

    public Game(
        GameState state,
        ICommandParser parser,
        TextReader? input = null,
        TextWriter? output = null,
        string? prompt = null)
    {
        State = state ?? throw new ArgumentNullException(nameof(state));
        Parser = parser ?? throw new ArgumentNullException(nameof(parser));
        Input = input ?? Console.In;
        Output = output ?? Console.Out;
        Prompt = prompt ?? "> ";
    }

    public void AddTurnStartHandler(Action<Game> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        _turnStartHandlers.Add(handler);
    }

    public void AddTurnEndHandler(Action<Game, ICommand, CommandResult> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        _turnEndHandlers.Add(handler);
    }

    public void RequestStop()
    {
        _stopRequested = true;
    }

    public CommandResult Execute(string input)
    {
        var command = Parser.Parse(input);
        var result = State.Execute(command);
        if (!result.ShouldQuit)
        {
            TickNpcs();
        }

        return result;
    }

    public void TickNpcs()
    {
        var moves = new List<(INpc npc, ILocation from, ILocation to)>();

        foreach (var location in State.Locations)
        {
            foreach (var npc in location.Npcs.ToList())
            {
                var next = npc.GetNextLocation(location, State);
                if (next != null && !ReferenceEquals(next, location))
                {
                    moves.Add((npc, location, next));
                }
            }
        }

        foreach (var (npc, from, to) in moves)
        {
            from.RemoveNpc(npc);
            to.AddNpc(npc);
        }
    }

    public void Run()
    {
        while (true)
        {
            foreach (var handler in _turnStartHandlers)
            {
                handler(this);
            }

            Output.Write(Prompt);
            var input = Input.ReadLine();
            if (input == null) break;

            input = input.Trim();
            if (string.IsNullOrWhiteSpace(input)) continue;

            var command = Parser.Parse(input);
            var result = State.Execute(command);

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                Output.WriteLine(result.Message);
            }

            foreach (var reaction in result.ReactionsList)
            {
                if (!string.IsNullOrWhiteSpace(reaction))
                {
                    Output.WriteLine($"> {reaction}");
                }
            }

            if (result.ShouldQuit)
            {
                break;
            }

            TickNpcs();

            foreach (var handler in _turnEndHandlers)
            {
                handler(this, command, result);
            }

            if (_stopRequested)
            {
                break;
            }
        }
    }
}
