﻿using Subterfuge.Remake.Core.Entities;
using Subterfuge.Remake.Core.GameEvents.Base;
using Subterfuge.Remake.Core.Timing;

namespace Subterfuge.Remake.Core.GameEvents.Combat.CombatEvents
{
    public class SpecialistStealEffect : NaturalGameEvent
    {
        
        private IEntity _combatant1;
        private IEntity _combatant2;
        public SpecialistStealEffect(GameTick occursAt) : base(occursAt, Priority.SPECIALIST_STEAL_EFFECT)
        {
        }

        public override bool ForwardAction(TimeMachine timeMachine, GameState state)
        {
            throw new System.NotImplementedException();
        }

        public override bool BackwardAction(TimeMachine timeMachine, GameState state)
        {
            throw new System.NotImplementedException();
        }

        public override bool WasEventSuccessful()
        {
            throw new System.NotImplementedException();
        }
    }
}