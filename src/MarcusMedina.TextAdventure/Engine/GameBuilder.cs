// <copyright file="GameBuilder.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Engine;

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
    private IPathfinder? _pathfinder;
    private IForeshadowingSystem? _foreshadowingSystem;
    private INarrativeVoiceSystem? _narrativeVoiceSystem;
    private IAgencyTracker? _agencyTracker;
    private IDramaticIronySystem? _dramaticIronySystem;
    private IFlashbackSystem? _flashbackSystem;
    private IChapterSystem? _chapterSystem;
    private IScheduleQueue? _scheduleQueue;
    private IActionTriggerSystem? _actionTriggerSystem;
    private readonly List<Action<Game>> _turnStartHandlers = [];
    private readonly List<Action<Game, ICommand, CommandResult>> _turnEndHandlers = [];
    private IStoryLogger? _storyLogger;
    private IDevLogger? _devLogger;

    public static GameBuilder Create()
    {
        return new();
    }

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

    public GameBuilder UsePathfinder(IPathfinder pathfinder)
    {
        _pathfinder = pathfinder ?? throw new ArgumentNullException(nameof(pathfinder));
        return this;
    }

    public GameBuilder UseForeshadowingSystem(IForeshadowingSystem foreshadowingSystem)
    {
        _foreshadowingSystem = foreshadowingSystem ?? throw new ArgumentNullException(nameof(foreshadowingSystem));
        return this;
    }

    public GameBuilder UseNarrativeVoiceSystem(INarrativeVoiceSystem narrativeVoiceSystem)
    {
        _narrativeVoiceSystem = narrativeVoiceSystem ?? throw new ArgumentNullException(nameof(narrativeVoiceSystem));
        return this;
    }

    public GameBuilder UseAgencyTracker(IAgencyTracker agencyTracker)
    {
        _agencyTracker = agencyTracker ?? throw new ArgumentNullException(nameof(agencyTracker));
        return this;
    }

    public GameBuilder UseDramaticIronySystem(IDramaticIronySystem dramaticIronySystem)
    {
        _dramaticIronySystem = dramaticIronySystem ?? throw new ArgumentNullException(nameof(dramaticIronySystem));
        return this;
    }

    public GameBuilder UseFlashbackSystem(IFlashbackSystem flashbackSystem)
    {
        _flashbackSystem = flashbackSystem ?? throw new ArgumentNullException(nameof(flashbackSystem));
        return this;
    }

    public GameBuilder UseChapterSystem(IChapterSystem chapterSystem)
    {
        _chapterSystem = chapterSystem ?? throw new ArgumentNullException(nameof(chapterSystem));
        return this;
    }

    public GameBuilder UseScheduleQueue(IScheduleQueue scheduleQueue)
    {
        _scheduleQueue = scheduleQueue ?? throw new ArgumentNullException(nameof(scheduleQueue));
        return this;
    }

    public GameBuilder UseActionTriggerSystem(IActionTriggerSystem actionTriggerSystem)
    {
        _actionTriggerSystem = actionTriggerSystem ?? throw new ArgumentNullException(nameof(actionTriggerSystem));
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
        {
            return this;
        }

        foreach (ILocation location in locations)
        {
            if (location == null)
            {
                continue;
            }

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

    public GameBuilder UseStoryLogger(IStoryLogger logger)
    {
        _storyLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        return this;
    }

    public GameBuilder UseDevLogger(IDevLogger logger)
    {
        _devLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        return this;
    }

    public Game Build()
    {
        ICommandParser parser = _parser ?? throw new InvalidOperationException("Parser must be provided.");
        GameState state = _state ?? BuildStateFromLocations();

        if (_state != null && _locations.Count > 0)
        {
            state.RegisterLocations(_locations);
        }

        if (_timeSystem != null)
        {
            state.SetTimeSystem(_timeSystem);
        }

        if (_pathfinder != null)
        {
            state.SetPathfinder(_pathfinder);
        }

        if (_foreshadowingSystem != null)
        {
            state.SetForeshadowingSystem(_foreshadowingSystem);
        }

        if (_narrativeVoiceSystem != null)
        {
            state.SetNarrativeVoiceSystem(_narrativeVoiceSystem);
        }

        if (_agencyTracker != null)
        {
            state.SetAgencyTracker(_agencyTracker);
        }

        if (_dramaticIronySystem != null)
        {
            state.SetDramaticIronySystem(_dramaticIronySystem);
        }

        if (_flashbackSystem != null)
        {
            state.SetFlashbackSystem(_flashbackSystem);
        }

        if (_chapterSystem != null)
        {
            state.SetChapterSystem(_chapterSystem);
        }

        if (_scheduleQueue != null)
        {
            state.SetScheduleQueue(_scheduleQueue);
        }

        if (_actionTriggerSystem != null)
        {
            state.SetActionTriggerSystem(_actionTriggerSystem);
        }

        Game game = new(state, parser, _input ?? Console.In, _output ?? Console.Out, _prompt);

        foreach (Action<Game> handler in _turnStartHandlers)
        {
            game.AddTurnStartHandler(handler);
        }

        foreach (Action<Game, ICommand, CommandResult> handler in _turnEndHandlers)
        {
            game.AddTurnEndHandler(handler);
        }

        if (_storyLogger != null)
        {
            game.AddTurnEndHandler((g, command, result) => _storyLogger.LogTurn(g.State, command, result));
        }

        if (_devLogger != null)
        {
            game.AddTurnEndHandler((g, command, result) => _devLogger.LogTurn(g.State, command, result));
        }

        return game;
    }

    private GameState BuildStateFromLocations()
    {
        ILocation? start = _startLocation ?? _locations.FirstOrDefault();
        if (start == null)
        {
            throw new InvalidOperationException("Start location must be provided.");
        }

        HashSet<ILocation> allLocations =
        [
.. _locations,             start
        ];

        return new GameState(
            start,
            timeSystem: _timeSystem,
            pathfinder: _pathfinder,
            foreshadowingSystem: _foreshadowingSystem,
            narrativeVoiceSystem: _narrativeVoiceSystem,
            agencyTracker: _agencyTracker,
            dramaticIronySystem: _dramaticIronySystem,
            flashbackSystem: _flashbackSystem,
            chapterSystem: _chapterSystem,
            scheduleQueue: _scheduleQueue,
            actionTriggerSystem: _actionTriggerSystem,
            worldLocations: allLocations);
    }
}
