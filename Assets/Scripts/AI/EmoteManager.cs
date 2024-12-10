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

        [SerializeField] GameObject bone1;
        [SerializeField] Vector3 punchDelta1;

        [SerializeField] GameObject bone2;
        [SerializeField] Vector3 punchDelta2;

/*        [SerializeField] float duration = 0.2f;
        [SerializeField] int vibrato = 10;
        [SerializeField] float elasticity = 1f;*/
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
            /*this.emoteHolder.sprite = this.emotesSO.BonkEmote;
            this.PlayEmotion(playTime);*/
            //var newPos =this.punchDelta1;
           /* DOTween.Kill(this.bone1.transform);
            DOTween.Kill(this.bone2.transform);
            this.bone1.transform.DOPunchPosition(this.punchDelta1, this.duration, this.vibrato, this.elasticity);
            this.bone2.transform.DOPunchPosition(this.punchDelta2, this.duration, this.vibrato, this.elasticity);*/
        }

        void PlayEmotion(float playTime)
        {
            this.emoteHolder.gameObject.SetActive(true);
            DOTween.Kill(this.emoteHolder.transform);
            this.emoteHolder.transform.DOScale(1f, this.appearTime).SetEase(Ease.OutBounce);

            Timing.RunCoroutine(RemoveEmote(this.emoteHolder.gameObject, playTime).CancelWith(this.emoteHolder.gameObject));
        }

        IEnumerator<float> RemoveEmote(GameObject emoteHolderObject,  float time)
        {
            yield return Timing.WaitForSeconds(time);

            DOTween.Kill(this.emoteHolder.transform);
            this.emoteHolder.transform.DOScale(0f, this.fadeTime).SetEase(Ease.InOutBack).OnComplete(() => this.emoteHolder.gameObject.SetActive(false));
        }

    }
}
