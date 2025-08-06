using UnityEngine;

namespace Agent
{
    public class EffectsController : MonoBehaviour
    {
        [SerializeField] ParticleSystem mischieveMarker;

        public void SwitchMischieveEffect(bool switchOn)
        {
            if(switchOn)
                this.mischieveMarker.Play();
            else
                this.mischieveMarker.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
