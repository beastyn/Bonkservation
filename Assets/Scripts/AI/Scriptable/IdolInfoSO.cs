namespace AI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Rendering;

    [CreateAssetMenu(fileName = "IdolInfo", menuName = "Bonk/IdolInfo")]
    public class IdolInfoSO : ScriptableObject
    {
        public UnityAction DodgeEvent;

        [SerializeField] string idolName;
        [SerializeField] int roomNumber = 1;

        [Header("General")]
        [SerializeField] float speedRate = 0.2f;
        [SerializeField] float restingTimeRate = 0.3f;

        [Header("Walking")]
        [SerializeField] float startWalkingSpeed = 1.5f;
        [SerializeField] float currentWalkingSpeed = 1.5f;
        [SerializeField] float rotSpeed = 30f;
        [SerializeField] float acceleration = 6f;

        [Header("Running")]
        [SerializeField] float runningSpeed = 3f;
        [SerializeField] float runRotSpeed = 50f;
        [SerializeField] float runAcceleration = 10f;

        [Header("Dodging")]
        [SerializeField] float dodgeSpeed = 150f;
        [SerializeField] float dodgeRotationSpeed = 150f;
        [SerializeField] float dodgeAcceleration = 150f;

        int timesBonked = 0;

        public string Name => this.idolName;
        public int RoomNumber => this.roomNumber;

        public float RestingTimeRate => this.restingTimeRate;

        public float CurrentWalkingSpeed => this.currentWalkingSpeed;
        public float RotSpeed => this.rotSpeed;
        public float Acceleration => this.acceleration;

        public float RunningSpeed => this.runningSpeed;
        public float RunRotSpeed => this.runRotSpeed;
        public float RunAcceleration => this.runAcceleration;

        public float DodgeSpeed => this.dodgeSpeed;
        public float DodgeRotSpeed => this.dodgeRotationSpeed;
        public float DodgeAcceleration => this.dodgeAcceleration;

        public int TimesBonked => this.timesBonked;

        public void ResetSpeed() => this.currentWalkingSpeed = this.startWalkingSpeed;
        public void SetRotSpeed(float rotSpeed) => this.rotSpeed = rotSpeed;

        public void ResetBonks() => this.timesBonked = 0;
        public void AddBonk() => this.timesBonked++;

        public void IsDodging() => this.DodgeEvent?.Invoke();
    }
}