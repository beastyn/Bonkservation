namespace Gameplay.Buffs
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ShieldBuff: BuffBase
    {
        public override void ApplyOneTineEffect()
        {
            base.ApplyOneTineEffect();
            this.damageableObject.SetProtection(true);
        }

        public override void RemoveOneTimeEffect()
        {
            base.RemoveOneTimeEffect();
            this.damageableObject.SetProtection(false);
        }
    }
}
