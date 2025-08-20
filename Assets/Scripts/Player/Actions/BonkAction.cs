using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Player.Actions
{
    using Managers;
    using Gameplay;
    using BrainDesigner.Scripts;
    using Unity.Cinemachine;

    public class BonkAction : ActionBase
    {
        public System.Action WrongBonkEvent; 

        [SerializeField] float bonkForce;
        [SerializeField] AudioSource audioSource;
        [SerializeField] AudioClip[] bonkSounds;
        [SerializeField] Camera cameraToShake;

        bool canDamage = false;
        Collider currentIdol;

        Damageable damagableComp;

        protected override void OnEnable()
        {
            PlayerInputReciever.MainActionEvent += OnMainActionEvent;
        }
        protected override void OnDisable()
        {
            PlayerInputReciever.MainActionEvent -= OnMainActionEvent;
        }


        void OnMainActionEvent() => this.activated = true;

        protected override void OnDoAction ()
        {
            if (this.canDamage)
            {
                this.damagableComp = this.currentIdol.gameObject.GetComponent<Damageable>();
                if (this.damagableComp == null) return;

                this.damagableComp.InflictDamage(this.CurrentDamage, this.gameObject.transform, this.currentIdol.gameObject.GetComponent<NavMeshAgent>());
                this.damagableComp.BobbleHead.AddForce(this.transform.up * this.bonkForce);
                AudioMixerManager.PlayClip(this.audioSource, this.bonkSounds[UnityEngine.Random.Range(0, this.bonkSounds.Length)]);

                this.damagableComp.SoftBodyData.ApplyUpToDownForce();
            }
        }
        public void OnTriggerEnter(Collider collision)
        {
            if (collision.CompareTag("Idol"))
            {
                this.canDamage = true;
                this.currentIdol = collision;
            }
        }

        public void OnTriggerExit(Collider collision)
        {
            if (collision.CompareTag("Idol"))
                this.canDamage = false;
        }
    }
}

