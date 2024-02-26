namespace Gameplay.Buffs.Internal
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using MEC;

    public class RegenBuff : BuffBase
    {
        public RegenBuff(EnergySO energyToRegen, BuffSO buffSO)
        {
            this.energyObject= energyToRegen;
            this.buffSO= buffSO;
        }

        public override void ApplyContinuousEffect()
        {
            base.ApplyContinuousEffect();

            this.energyObject.RestoreEnergyTick(this.buffSO.SpecificValue * Timing.DeltaTime);
        }
    }
}
