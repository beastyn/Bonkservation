using UnityEngine;
using UnityEngine.VFX;

namespace Agent
{
    public class EffectsController : MonoBehaviour
    {
        [SerializeField] ParticleSystem mischieveMarker;
        [SerializeField] GameObject targetMarker;
        [SerializeField] Transform face;

        void Start()
        {
            this.SwitchTargetEffect(false);
            
        }

        void Update()
        {
            this.targetMarker.transform.LookAt(face.position);
            
        }

        public void SwitchMischieveEffect(bool switchOn)
        {
            if(switchOn)
                this.mischieveMarker.Play();
            else
                this.mischieveMarker.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        public void SwitchTargetEffect(bool switchOn)
        {
                this.targetMarker.SetActive(switchOn);
        }
    }
}
