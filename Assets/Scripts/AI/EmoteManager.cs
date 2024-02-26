namespace AI
{
    using DG.Tweening;
    using Gameplay;
    using MEC;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class EmoteManager : MonoBehaviour
    {
        [SerializeField] EmotesSO emotesSO;
        [SerializeField] EnergySO idolEnergy;
        [SerializeField] Image emoteHolder;
        [SerializeField] float appearTime;
        [SerializeField] float fadeTime;

        void OnEnable()
        {
            this.idolEnergy.EnergyChangeEvent += OnEnergyChangeEvent;
        }
        void OnDisable()
        {
            this.idolEnergy.EnergyChangeEvent -= OnEnergyChangeEvent;
        }

        void OnEnergyChangeEvent(float energy, bool isRestoring)
        {
            if (!isRestoring) this.PlayBonkEmotion(2f);
        }

        void PlayBonkEmotion(float playTime)
        {
            this.emoteHolder.sprite = this.emotesSO.BonkEmote;
            this.PlayEmotion(playTime);
        }

        void PlayEmotion(float playTime)
        {
            DOTween.Kill(this.emoteHolder.transform);
            this.emoteHolder.transform.DOScale(1f, this.appearTime).SetEase(Ease.OutBounce);

            Timing.RunCoroutine(RemoveEmote(this.emoteHolder.gameObject, playTime).CancelWith(this.emoteHolder.gameObject));
        }

        IEnumerator<float> RemoveEmote(GameObject emoteHolderObject,  float time)
        {
            yield return Timing.WaitForSeconds(time);

            DOTween.Kill(this.emoteHolder.transform);
            this.emoteHolder.transform.DOScale(0f, this.fadeTime).SetEase(Ease.InOutBack);
        }

    }
}
