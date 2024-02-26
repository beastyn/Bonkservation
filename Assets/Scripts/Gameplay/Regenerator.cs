namespace Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Regenerator : MonoBehaviour
    {
        [SerializeField] EnergySO energySO;

        // Update is called once per frame
        void Update()
        {
            if(this.energySO.IsRegenerating)
                this.energySO.FullRestoreEnergy(this.energySO.RestoreEnergyValue * Time.deltaTime);
        }
    }
}
