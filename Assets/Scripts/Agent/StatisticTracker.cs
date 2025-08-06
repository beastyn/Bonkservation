namespace Agent
{
    using Managers;
    using Gameplay;
    using UnityEngine;

    public class StatisticTracker : MonoBehaviour
    {
        [SerializeField] string idolName;
        
        SOEnergy idolHealth;

        void OnEnable() => this.idolHealth.EnergyChangeEvent += OnEnergyChangeEvent;
        void OnDisable() => this.idolHealth.EnergyChangeEvent -= OnEnergyChangeEvent;

        void Awake()
        {
            var soHolder = this.GetComponent<AgentManagersAndData>();
            this.idolHealth = soHolder?.AgentEnergy;
        }

        void OnEnergyChangeEvent(float energy, bool isRestoring)
        {
            if(!isRestoring)
                ManagersSOHolder.SOScoreManager.AddBonk(this.idolName);
        }
    }
}