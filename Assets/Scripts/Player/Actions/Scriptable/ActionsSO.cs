namespace Player
{
    using System.Collections;
    using System.Collections.Generic;
    using Unity.VisualScripting;
    using UnityEngine;
    using UnityEngine.Events;

    [CreateAssetMenu(fileName = "PlayerAction", menuName = "Bonk/PlayerAction")]
    public class ActionsSO : ScriptableObject
    {
        public UnityAction ActionEvent;

        [SerializeField] int startDamage;
        [SerializeField] int currentDamage;


        [SerializeField] float animationTime = 1f;
        [SerializeField] float cooldown = 1f;

        [SerializeField] AudioClip audioClip;

        float nextActivationTime = 0f;
        public int CurrentDamage => this.currentDamage;
        public float AnimationTime => this.animationTime;
        public float Delay => this.cooldown;
        public AudioClip AudioClip => this.audioClip;

        public void UpdateDamageValue(int bonus) => this.currentDamage += bonus;

        public bool CanBeUsed(float time) => time > this.nextActivationTime;
        public void SetNextActivation(float time)
        {
            this.nextActivationTime = time + this.cooldown;
            this.ActionEvent?.Invoke();
        }
        public void ResetAction()
        {
            nextActivationTime = 0f;
        }
    }
}