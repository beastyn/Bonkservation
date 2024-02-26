namespace Player.Actions
{
    using Managers;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class ActionBase : MonoBehaviour
    {
        public ActionsSO actionSO;

        protected bool activated = false;
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }

        private void Start()
        {
            this.actionSO.ResetAction();
        }

        // Update is called once per frame
        void Update()
        {
            if (this.activated && this.actionSO.CanBeUsed(Time.time))
            {
                this.actionSO.SetNextActivation(Time.time);
                ManagersSOHolder.AudioManagerSO.PlaySFX(this.actionSO.AudioClip, this.transform, 1f);
            }
            this.activated = false;
        }
    }
}
