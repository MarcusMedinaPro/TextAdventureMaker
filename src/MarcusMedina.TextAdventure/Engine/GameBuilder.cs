// <copyright file="GameBuilder.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Engine;

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Interfaces;

public sealed class GameBuilder
{
    private GameState? _state;
    private ICommandParser? _parser;
    private TextReader? _input;
    private TextWriter? _output;
    private string _prompt = "> ";
    private readonly List<ILocation> _locations = [];
    private ILocation? _startLocation;
    private ITimeSystem? _timeSystem;
    private readonly List<Action<Game>> _turnStartHandlers = [];
    private readonly List<Action<Game, ICommand, CommandResult>> _turnEndHandlers = [];

    public static GameBuilder Create() => new();

    public GameBuilder UseState(GameState state)
    {
        _state = state ?? throw new ArgumentNullException(nameof(state));
        return this;
    }

    public GameBuilder UseParser(ICommandParser parser)
    {
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        return this;
    }

    public GameBuilder UseInput(TextReader input)
    {
        _input = input ?? throw new ArgumentNullException(nameof(input));
        return this;
    }

    public GameBuilder UseOutput(TextWriter output)
    {
        _output = output ?? throw new ArgumentNullException(nameof(output));
        return this;
    }

    public GameBuilder UsePrompt(string prompt)
    {
        _prompt = prompt ?? throw new ArgumentNullException(nameof(prompt));
        return this;
    }

    public GameBuilder UseStartLocation(ILocation startLocation)
    {
        _startLocation = startLocation ?? throw new ArgumentNullException(nameof(startLocation));
        return this;
    }

    public GameBuilder UseTimeSystem(ITimeSystem timeSystem)
    {
        _timeSystem = timeSystem ?? throw new ArgumentNullException(nameof(timeSystem));
        return this;
    }

    public GameBuilder AddLocation(ILocation location, bool isStart = false)
    {
        ArgumentNullException.ThrowIfNull(location);
        _locations.Add(location);
        if (isStart)
        {
            _startLocation = location;
        }

        return this;
    }

    public GameBuilder AddLocations(IEnumerable<ILocation> locations)
    {
        if (locations == null)
            return this;
        foreach (var location in locations)
        {
            if (location == null)
                continue;
            _locations.Add(location);
        }

        return this;
    }

    public GameBuilder AddTurnStart(Action<Game> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        _turnStartHandlers.Add(handler);
        return this;
    }

    public GameBuilder AddTurnEnd(Action<Game, ICommand, CommandResult> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        _turnEndHandlers.Add(handler);
        return this;
    }

    public Game Build()
    {
        var parser = _parser ?? throw new InvalidOperationException("Parser must be provided.");
        var state = _state ?? BuildStateFromLocations();

        if (_state != null && _locations.Count > 0)
        {
            state.RegisterLocations(_locations);
        }

        if (_timeSystem != null)
        {
            state.SetTimeSystem(_timeSystem);
        }

        var game = new Game(state, parser, _input ?? Console.In, _output ?? Console.Out, _prompt);

        foreach (var handler in _turnStartHandlers)
        {
            game.AddTurnStartHandler(handler);
        }

        foreach (var handler in _turnEndHandlers)
        {
            game.AddTurnEndHandler(handler);
        }

        return game;
    }

    private GameState BuildStateFromLocations()
    {
        var start = _startLocation ?? _locations.FirstOrDefault();
        if (start == null)
        {
            throw new InvalidOperationException("Start location must be provided.");
        }

        var allLocations = new HashSet<ILocation>(_locations)
        {
            start
        };

        return new GameState(start, timeSystem: _timeSystem, worldLocations: allLocations);
    }
}
