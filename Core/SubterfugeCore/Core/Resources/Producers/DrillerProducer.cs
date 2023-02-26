﻿using System;
using Subterfuge.Remake.Core.Entities;
using Subterfuge.Remake.Core.Entities.Components;
using Subterfuge.Remake.Core.Timing;

namespace Subterfuge.Remake.Core.Resources.Producers
{
    public class DrillerProducer : ResourceProducer
    {
        private IEntity ProduceAt;
        public DrillerProducer(
            IEntity produceAt,
            TimeMachine timeMachine
        ) : base(
            Constants.TicksPerProduction,
            Constants.BaseFactoryProductionAmount,
            timeMachine
        ) {
            ProduceAt = produceAt;
        }

        protected override void Produce(int productionAmount)
        {
            ProduceAt.GetComponent<DrillerCarrier>().AlterDrillers(productionAmount);
        }

        protected override void UndoProduce(int amountToRevert)
        {
            ProduceAt.GetComponent<DrillerCarrier>().AlterDrillers(amountToRevert * -1);
        }

        public void SetEntity(IEntity entity)
        {
            ProduceAt = entity;
        }

        public override int GetNextProductionAmount(GameState state)
        {
            var owner = ProduceAt.GetComponent<DrillerCarrier>().GetOwner();
            if (ProduceAt.GetComponent<DrillerCarrier>().IsDestroyed() || (owner != null && owner.IsEliminated()))
            {
                return 0;
            }
            return Math.Min(state.GetExtraDrillerCapcity(ProduceAt.GetComponent<DrillerCarrier>().GetOwner()), BaseValuePerProduction);
        }
    }
}