namespace AI
{
    using Managers;
    using Gameplay;
    using UnityEngine;

    public class StatisticTracker : MonoBehaviour
    {
        [SerializeField] IdolInfoSO idolInfo;
        [SerializeField] EnergySO idolHealth;

        void OnEnable() => this.idolHealth.EnergyChangeEvent += OnEnergyChangeEvent;
        void OnDisable() => this.idolHealth.EnergyChangeEvent -= OnEnergyChangeEvent;

        void OnEnergyChangeEvent(float energy, bool isRestoring)
        {
            if(!isRestoring)
                ManagersSOHolder.ScoreManagerSO.AddBonk(this.idolInfo.Name);
        }
    }
}