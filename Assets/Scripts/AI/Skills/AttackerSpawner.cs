namespace AI.Skills
{
    using System.Collections;
    using System.Collections.Generic;
    using Unity.VisualScripting;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class AttackerSpawner : MonoBehaviour
    {
        [SerializeField] AttackersPool attackersPool;
        [SerializeField] SkillSO skillSO;

        public delegate void OnAttackEndCallback();
        public OnAttackEndCallback AttackEndMethod;

        bool allowSpawn = false;
        void OnEnable()
        {
            this.allowSpawn = true;
            StartCoroutine(this.SpawnAttacker());
        }

        public void SwitchSpawner(bool allowSpawn)
        {
            this.allowSpawn = allowSpawn;
            if (!allowSpawn && this.attackersPool.ActiveObjectsCount == 0) this.AttackEndMethod();
        }

        IEnumerator SpawnAttacker()
        {
            while (allowSpawn)
            {
                var spawnedAttacker = this.attackersPool.GetObject();
                spawnedAttacker.ReturnMeEvent += OnReturnMeEvent;
                yield return new WaitForSeconds(this.skillSO.CurrentAttackersFrequency);
            }
        }

        void OnReturnMeEvent(Attacker attacker)
        {
            attacker.ReturnMeEvent -= OnReturnMeEvent;
            this.attackersPool.ReturnObject(attacker);
            if (!allowSpawn && this.attackersPool.ActiveObjectsCount == 0) this.AttackEndMethod();
        }

    }
}