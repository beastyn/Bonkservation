namespace Gameplay.Buffs.Internal
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ShieldBuff: BuffBase
    {
        public ShieldBuff(Damageable damageable, BuffSO buffSO)
        {
            this.damageableObject = damageable;
            this.buffSO = buffSO;
        }

        public override void ApplyOneTimeEffect()
        {
            base.ApplyOneTimeEffect();
            this.damageableObject.SetProtection(true);
        }

        public override void RemoveOneTimeEffect()
        {
            base.RemoveOneTimeEffect();
            this.damageableObject.SetProtection(false);
        }
    }
}
