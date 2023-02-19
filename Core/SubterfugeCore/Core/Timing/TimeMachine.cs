﻿using System;
using System.Collections.Generic;
using System.Linq;
using Subterfuge.Remake.Core.GameEvents.Base;
using Subterfuge.Remake.Core.GameEvents.EventPublishers;

namespace Subterfuge.Remake.Core.Timing
{
    public class TimeMachine : ITickEventPublisher
    {
        // List of Player Events mapped to each tick
        private List<GameEvent> eventQueue = new List<GameEvent>();

        // Current representation of the game state
        private GameState.GameState _gameState;

        /// <summary>
        /// Creates a new instance of the TimeMachine. You will likely never need to call this as this is created in the
        /// `Game` object when the game is created.
        /// </summary>
        /// <param name="state">The initial GameState</param>
        public TimeMachine(GameState.GameState state)
        {
            _gameState = state;
        }

        /// <summary>
        /// Get the time machine's current state
        /// </summary>
        /// <returns>The GameState at the current time of the TimeMachine</returns>
        public GameState.GameState GetState()
        {
            return this._gameState;
        }

        /// <summary>
        /// Adds an event to the future event queue
        /// </summary>
        /// <param name="gameEvent">The game event to add to the Queue</param>
        public void AddEvent(GameEvent gameEvent)
        {
            eventQueue.Add(gameEvent);
        }

        /// <summary>
        /// Removes a GameEvent from the game.
        /// </summary>
        /// <param name="gameEvent">The GameEvent to remove from the queue</param>
        public void RemoveEvent(GameEvent gameEvent)
        {
            
            eventQueue.Remove(gameEvent);
        }

        /// <summary>
        /// Jumps to a specific GameTick
        /// </summary>
        /// <param name="tick">The GameTick to jump to</param>
        public void GoTo(GameTick tick)
        {
            TimeMachineDirection direction = tick > _gameState.CurrentTick
                ? TimeMachineDirection.FORWARD
                : TimeMachineDirection.REVERSE;


            while (tick != GetCurrentTick())
            {
                if (direction == TimeMachineDirection.FORWARD)
                {
                    this._gameState.CurrentTick = this._gameState.CurrentTick.Advance(1);
                }
                else
                {
                    this._gameState.CurrentTick = this._gameState.CurrentTick.Rewind(1);
                }

                eventQueue
                    .Where(it => it.GetOccursAt() == this._gameState.CurrentTick)
                    .ToList()
                    .ForEach(it =>
                    {
                        if (direction == TimeMachineDirection.FORWARD)
                        {
                            it.ForwardAction(this, _gameState);
                        }
                        else
                        {
                            it.BackwardAction(this, _gameState);
                        }
                    });

                this.OnTick?.Invoke(this, new OnTickEventArgs()
                {
                    Direction = direction,
                    CurrentState = this._gameState,
                    CurrentTick = this._gameState.CurrentTick,
                });
            }
        }

        /// <summary>
        /// Gets the GameTick that the time machine is currently representing.
        /// </summary>
        /// <returns>The GameTick that the timeMachine is showing</returns>
        public GameTick GetCurrentTick()
        {
            return _gameState.CurrentTick;
        }

        
        /// <summary>
        /// Jumps to a specific GameEvent
        /// </summary>
        /// <param name="eventOfInterest">The GameEvent to jump to</param>
        public void GoTo(GameEvent eventOfInterest)
        {
            this.GoTo(eventOfInterest.GetOccursAt());
        }
        
        /// <summary>
        /// For debugging. Advances the timeMachine by a specified number of ticks.
        /// </summary>
        /// <param name="ticks">The number of ticks to advance</param>
        public void Advance(int ticks)
        {
            GameTick tick = this.GetCurrentTick().Advance(ticks);
            this.GoTo(tick);
        }

        /// <summary>
        /// For debugging. Rewinds the timeMachine by a specified number of ticks.
        /// </summary>
        /// <param name="ticks">The number of ticks to rewind</param>
        public void Rewind(int ticks)
        {
            GameTick tick = this.GetCurrentTick().Rewind(ticks);
            this.GoTo(tick);
        }

        /// <summary>
        /// Gets a list of the queued events
        /// </summary>
        /// <returns>A list of the events in the future event queue</returns>
        public List<GameEvent> GetQueuedEvents()
        {
            return eventQueue
                .Where(it => it.GetOccursAt() > GetCurrentTick())
                .ToList();
        }
        
        /// <summary>
        /// Get past events
        /// </summary>
        /// <returns>A list of the events in the future event queue</returns>
        public List<GameEvent> GetPastEvents()
        {
            return eventQueue
                .Where(it => it.GetOccursAt() <= GetCurrentTick())
                .ToList();
        }

        public event EventHandler<OnTickEventArgs>? OnTick;
    }
}
