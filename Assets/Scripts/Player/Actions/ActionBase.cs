using UnityEngine;
using UnityEngine.Events;

namespace Player.Actions
{
    public class ActionBase : MonoBehaviour
    {
        public UnityAction ActionEvent;

        [SerializeField] int startDamage;
        [SerializeField] float animationTime = 1f;
        [SerializeField] float cooldown = 1f;

        [SerializeField] AudioClip audioClip;

        int currentDamage;
        float nextActivationTime = 0f;
        public int CurrentDamage => this.currentDamage;
        public float AnimationTime => this.animationTime;
        public float Delay => this.cooldown;
        public AudioClip AudioClip => this.audioClip;

        protected bool activated = false;
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }

        void Start() => this.ResetAction();

        void Update()
        {
            if (this.activated && this.CanBeUsed(Time.time))
            {
                this.DoAction(Time.time);
                //ManagersSOHolder.AudioManagerSO.PlaySFX(this.actionSO.AudioClip, this.transform, 1f);
            }
            this.activated = false;
        }
        public void UpdateDamageValue(int bonus) => this.currentDamage += bonus;

        bool CanBeUsed(float time) => time > this.nextActivationTime;

        void DoAction(float time)
        {
            this.nextActivationTime = time + this.cooldown;
            this.OnDoAction();
            this.ActionEvent?.Invoke();
        }
        void ResetAction()
        {
            nextActivationTime = 0f;
            this.currentDamage = this.startDamage;
        }

        protected virtual void OnDoAction() { }
    }
}

