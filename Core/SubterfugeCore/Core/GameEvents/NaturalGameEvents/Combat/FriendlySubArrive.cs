﻿using Subterfuge.Remake.Core.Components;
using Subterfuge.Remake.Core.Entities;
using Subterfuge.Remake.Core.Entities.Positions;
using Subterfuge.Remake.Core.GameEvents.Base;
using Subterfuge.Remake.Core.Timing;

namespace Subterfuge.Remake.Core.GameEvents.NaturalGameEvents.combat
{
    /// <summary>
    /// Friendly sub arrival
    /// </summary>
    public class FriendlySubArrive : NaturalGameEvent
    {
        private readonly Sub _arrivingSub;
        private readonly Outpost _outpost;

        /// <summary>
        /// Friendly sub arrival event
        /// </summary>
        /// <param name="combatant1">Combatant 1</param>
        /// <param name="combatant2">Combatant 2</param>
        /// <param name="occursAt">Tick of sub arrival</param>
        public FriendlySubArrive(IEntity combatant1, IEntity combatant2, GameTick occursAt) : base(occursAt, Priority.NaturalPriority9)
        {
            Sub _arrivingSub = combatant1 as Sub;
            if (_arrivingSub == null)
            {
                _arrivingSub = combatant2 as Sub;
                _outpost = combatant1 as Outpost;
            }
            else
            {
                _outpost = combatant2 as Outpost;
            }
        }

        /// <summary>
        /// Undoes the sub's arrival
        /// </summary>
        /// <returns>If the event was undone</returns>
        public override bool BackwardAction(TimeMachine timeMachine,  GameState.GameState state)
        {
            if (EventSuccess)
            {
                _outpost.GetComponent<DrillerCarrier>().RemoveDrillers(_arrivingSub.GetComponent<DrillerCarrier>().GetDrillerCount());
                _outpost.GetComponent<SpecialistManager>()
                    .RemoveSpecialists(_arrivingSub.GetComponent<SpecialistManager>().GetSpecialists());
                state.AddSub(this._arrivingSub);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Perfoms a friendly sub arrival
        /// </summary>
        /// <returns>If the event was successful</returns>
        public override bool ForwardAction(TimeMachine timeMachine, GameState.GameState state)
        {
            if (state.SubExists(_arrivingSub) && state.OutpostExists(_outpost))
            {
                _outpost.GetComponent<DrillerCarrier>().AddDrillers(_arrivingSub.GetComponent<DrillerCarrier>().GetDrillerCount());
                _outpost.GetComponent<SpecialistManager>().AddSpecialists(_arrivingSub.GetComponent<SpecialistManager>().GetSpecialists());
                state.RemoveSub(_arrivingSub);
                EventSuccess = true;
            }
            else
            {
                EventSuccess = false;
            }
            return EventSuccess;
        }
        
        /// <summary>
        /// Determines if the event was successful.
        /// </summary>
        /// <returns>If the event is successful</returns>
        public override bool WasEventSuccessful()
        {
            return EventSuccess;
        }
	}
}
