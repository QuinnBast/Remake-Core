﻿using System;
using Subterfuge.Remake.Api.Network;
using Subterfuge.Remake.Core.GameEvents.Base;
using Subterfuge.Remake.Core.Players;
using Subterfuge.Remake.Core.Timing;

namespace Subterfuge.Remake.Core.GameEvents.PlayerTriggeredEvents
{
    public abstract class PlayerTriggeredEvent : GameEvent
    {
        protected readonly GameRoomEvent Model;
        protected PlayerTriggeredEvent(GameRoomEvent model)
        {
            this.Model = model;
        }

        public Player IssuedBy()
        {
            return new Player(Model.IssuedBy);
        }

        public override GameTick GetOccursAt()
        {
            return new GameTick(Model.GameEventData.OccursAtTick);
        }

        public DateTime GetUnixTimeIssued()
        {
            return Model.TimeIssued;
        }

        public override string GetEventId()
        {
            return Model.Id;
        }

        public override Priority GetPriority()
        {
            return Priority.PlayerIssuedCommand;
        }

        protected GameRoomEvent GetBaseGameEventModel()
        {
            return new GameRoomEvent()
            {
                Id = GetEventId(),
                IssuedBy = IssuedBy().ToUser(),
                TimeIssued = Model.TimeIssued,
                GameEventData = Model.GameEventData
            };
        }
    }
}