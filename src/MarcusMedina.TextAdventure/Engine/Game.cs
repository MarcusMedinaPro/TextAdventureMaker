// <copyright file="Game.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class Game(
    GameState state,
    ICommandParser parser,
    TextReader? input = null,
    TextWriter? output = null,
    string? prompt = null) : IGame
{
    private readonly List<Action<Game>> _turnStartHandlers = [];
    private readonly List<Action<Game, ICommand, CommandResult>> _turnEndHandlers = [];
    private bool _stopRequested;

    public GameState State { get; } = state ?? throw new ArgumentNullException(nameof(state));
    public ICommandParser Parser { get; } = parser ?? throw new ArgumentNullException(nameof(parser));
    public TextReader Input { get; } = input ?? Console.In;
    public TextWriter Output { get; } = output ?? Console.Out;
    public string Prompt { get; set; } = prompt ?? "> ";

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
        ICommand command = Parser.Parse(input);
        CommandResult result = State.Execute(command);
        if (!result.ShouldQuit)
        {
            TickNpcs();
        }

        return result;
    }

    public void TickNpcs()
    {
        List<(INpc npc, ILocation from, ILocation to)> moves = [];

        foreach (ILocation location in State.Locations)
        {
            foreach (INpc? npc in location.Npcs.ToList())
            {
                ILocation? next = npc.GetNextLocation(location, State);
                if (next != null && !ReferenceEquals(next, location))
                {
                    moves.Add((npc, location, next));
                }
            }
        }

        foreach ((INpc? npc, ILocation? from, ILocation? to) in moves)
        {
            _ = from.RemoveNpc(npc);
            to.AddNpc(npc);
        }
    }

    public void Run()
    {
        while (true)
        {
            foreach (Action<Game> handler in _turnStartHandlers)
            {
                handler(this);
            }

            Output.Write(Prompt);
            string? input = Input.ReadLine();
            if (input == null)
            {
                break;
            }

            input = input.Trim();
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            ICommand command = Parser.Parse(input);
            CommandResult result = State.Execute(command);

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                Output.WriteLine(result.Message);
            }

            foreach (string reaction in result.ReactionsList)
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

            foreach (Action<Game, ICommand, CommandResult> handler in _turnEndHandlers)
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
