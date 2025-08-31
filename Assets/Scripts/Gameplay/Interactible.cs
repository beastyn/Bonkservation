using UnityEngine;
using Player;
using Agent;

namespace Gameplay
{

    public class Interactible : MonoBehaviour
    {
        [SerializeField] BonkDetector bonkDetector;
        void OnEnable()
        {
            PlayerInputReciever.InteractActionEvent += OnInteractActionEvent;
        }
        void OnDisable()
        {
            PlayerInputReciever.InteractActionEvent -= OnInteractActionEvent;
        }


        void OnInteractActionEvent()
        {
            if (this.bonkDetector == null) return;
            if (this.bonkDetector.IsBonkerDetected)
                OnInteract();
        }

        public virtual void OnInteract() { }
    }
}